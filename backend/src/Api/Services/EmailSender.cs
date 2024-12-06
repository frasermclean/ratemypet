using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using RateMyPet.Api.Options;

namespace RateMyPet.Api.Services;

public interface IEmailSender
{
    Task SendConfirmationLinkAsync(string emailAddress, string confirmationLink);
    Task SendPasswordResetCodeAsync(string emailAddress, string resetCode);
}

public class EmailSender(
    IOptions<EmailSenderOptions> options,
    ILogger<EmailSender> logger,
    EmailClient emailClient) : IEmailSender
{
    private readonly string senderAddress = options.Value.SenderAddress;

    public Task SendConfirmationLinkAsync(string emailAddress, string confirmationLink)
    {
        const string subject = "Confirm your email";
        var htmlMessage =
            $"""
             <html><body>
             <h1>Welcome to Rate My Pet!</h1>
             <p>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.</p>;
             </body></html>
             """;

        return SendEmailAsync(emailAddress, subject, htmlMessage);
    }

    public Task SendPasswordResetCodeAsync(string emailAddress, string resetCode)
    {
        const string subject = "Password reset";
        var htmlMessage =
            $"""
             <html><body>
             <h1>Password reset</h1>
             <p>A password reset request for your account has been received. If this was you, please use the following code:</p>
             <code>{resetCode}</code>
             <p>If you did not request a password reset, please ignore this email.</p>
             </body></html>
             """;

        return SendEmailAsync(emailAddress, subject, htmlMessage);
    }

    private async Task SendEmailAsync(string recipientAddress, string subject, string htmlMessage)
    {
        var content = new EmailContent(subject) { Html = htmlMessage };
        var message = new EmailMessage(senderAddress, recipientAddress, content);

        var operation = await emailClient.SendAsync(WaitUntil.Started, message);

        try
        {
            while (true)
            {
                await operation.UpdateStatusAsync();
                if (operation.HasCompleted)
                {
                    break;
                }

                await Task.Delay(100);
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
