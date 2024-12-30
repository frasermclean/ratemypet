using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure.Options;

namespace RateMyPet.Infrastructure.Services;

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

        try
        {
            while (true)
            {
                await operation.UpdateStatusAsync(cancellationToken);
                if (operation.HasCompleted)
                {
                    break;
                }

                await Task.Delay(100, cancellationToken);
            }

            if (operation.HasValue)
            {
                logger.LogInformation("Email queued for delivery, status: {Status}, operation ID: {OperationId}",
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
