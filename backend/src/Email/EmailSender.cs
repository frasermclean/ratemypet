using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core.Abstractions;

namespace RateMyPet.Email;

public class EmailSender(
    IOptions<EmailOptions> options,
    ILogger<EmailSender> logger,
    EmailClient emailClient) : IEmailSender
{
    private readonly string senderAddress = options.Value.SenderAddress;

    public async Task SendEmailAsync(string recipientAddress, string subject, string htmlMessage,
        CancellationToken cancellationToken)
    {
        var content = new EmailContent(subject) { Html = htmlMessage };
        var message = new EmailMessage(senderAddress, recipientAddress, content);

        var operation = await emailClient.SendAsync(WaitUntil.Started, message, cancellationToken);
        logger.LogInformation("Started sending email to {RecipientAddress}, operation ID: {OperationId}", recipientAddress,
            operation.Id);

        try
        {
            await operation.WaitForCompletionResponseAsync(TimeSpan.FromSeconds(1), cancellationToken);

            if (operation.HasValue)
            {
                logger.LogInformation("Email send completed, status: {Status}, operation ID: {OperationId}",
                    operation.Value.Status, operation.Id);
            }
        }
        catch (RequestFailedException exception)
        {
            logger.LogError(exception, "Failed to send email, code: {Code}", exception.ErrorCode);
            throw;
        }
    }
}
