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
    private readonly string frontendBaseUrl = options.Value.FrontendBaseUrl;

    public async Task SendConfirmationEmailAsync(string emailAddress, Guid userId, string confirmationToken,
        CancellationToken cancellationToken = default)
    {
        var confirmationLink =
            $"{frontendBaseUrl}/auth/confirm-email?userId={userId}&token={confirmationToken}";

        var htmlMessage = $"""
                           <html><body>
                           <h1>Welcome to Rate My Pet!</h1>
                           <p>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.</p>
                           </body></html>
                           """;

        await SendEmailAsync(emailAddress, "Confirm your email", htmlMessage, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(string emailAddress, string resetCode,
        CancellationToken cancellationToken = default)
    {
        var resetLink =
            $"{frontendBaseUrl}/auth/reset-password?emailAddress={emailAddress}&resetCode={resetCode}";

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

        await SendEmailAsync(emailAddress, "Password reset", htmlMessage, cancellationToken);
    }

    private async Task SendEmailAsync(string recipientAddress, string subject, string htmlMessage,
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
