using System.Text;
using System.Text.Encodings.Web;
using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RateMyPet.Api.Options;

namespace RateMyPet.Api.Services;

public interface IEmailSender
{
    Task SendConfirmationLinkAsync(string emailAddress, string confirmationLink);
    Task SendPasswordResetLinkAsync(string emailAddress, string token);
}

public class EmailSender(
    IOptions<EmailSenderOptions> emailSenderOptions,
    IOptions<FrontendOptions> frontendOptions,
    ILogger<EmailSender> logger,
    EmailClient emailClient) : IEmailSender
{
    private readonly string senderAddress = emailSenderOptions.Value.SenderAddress;
    private readonly string frontendBaseUrl = frontendOptions.Value.BaseUrl;

    public Task SendConfirmationLinkAsync(string emailAddress, string confirmationLink)
    {
        const string subject = "Confirm your email";
        var htmlMessage = $"""
                           <html><body>
                           <h1>Welcome to Rate My Pet!</h1>
                           <p>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.</p>
                           </body></html>
                           """;

        return SendEmailAsync(emailAddress, subject, htmlMessage);
    }

    public Task SendPasswordResetLinkAsync(string emailAddress, string token)
    {
        const string subject = "Password reset";

        var resetCode = HtmlEncoder.Default.Encode(WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)));
        var resetLink = $"{frontendBaseUrl}/auth/reset-password?emailAddress={emailAddress}&resetCode={resetCode}";

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
