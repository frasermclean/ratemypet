using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using RateMyPet.Api.Options;

namespace RateMyPet.Api.Services;

public interface IEmailSender
{
    Task SendConfirmationLinkAsync(string emailAddress, string confirmationLink,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetLinkAsync(string emailAddress, string resetLink,
        CancellationToken cancellationToken = default);
}

public class EmailSender(
    IOptions<EmailSenderOptions> options,
    ILogger<EmailSender> logger,
    EmailClient emailClient) : IEmailSender
{
    private readonly string senderAddress = options.Value.SenderAddress;

    public Task SendConfirmationLinkAsync(string emailAddress, string confirmationLink,
        CancellationToken cancellationToken)
    {
        const string subject = "Confirm your email";
        var htmlMessage = $"""
                           <html><body>
                           <h1>Welcome to Rate My Pet!</h1>
                           <p>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.</p>
                           </body></html>
                           """;

        return SendEmailAsync(emailAddress, subject, htmlMessage, cancellationToken);
    }

    public Task SendPasswordResetLinkAsync(string emailAddress, string resetLink, CancellationToken cancellationToken)
    {
        const string subject = "Password reset";

        var htmlMessage = $"""
                           <html><body>
                           <h1>Password reset</h1>
                           <p>
                           A password reset request for your account has been received. If this was you, please
                           <a href='{resetLink}'>click here</a> to reset your password.
                           </p>
                           <p>If you did not request a password reset, please you may safely ignore this email.</p>
                           </body></html>
                           """;

        return SendEmailAsync(emailAddress, subject, htmlMessage, cancellationToken);
    }

    private async Task SendEmailAsync(string recipientAddress, string subject, string htmlMessage,
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
        }
    }
}
