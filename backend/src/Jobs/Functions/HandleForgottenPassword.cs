using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core.Events;
using RateMyPet.Jobs.Options;
using RateMyPet.Jobs.Services;
using RateMyPet.Persistence;

namespace RateMyPet.Jobs.Functions;

public class HandleForgottenPassword(
    ILogger<HandleForgottenPassword> logger,
    IOptions<EmailOptions> options,
    IEmailSender emailSender)
{
    private readonly string? frontendBaseUrl = options.Value.FrontendBaseUrl;

    [Function(nameof(HandleForgottenPassword))]
    public async Task ExecuteAsync([QueueTrigger(QueueNames.ForgotPassword)] ForgottenPasswordMessage message,
        CancellationToken cancellationToken)
    {
        const string subject = "Password reset";

        var resetLink =
            $"{frontendBaseUrl}/auth/reset-password?emailAddress={message.EmailAddress}&resetCode={message.ResetCode}";

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

        await emailSender.SendEmailAsync(message.EmailAddress, subject, htmlMessage, cancellationToken);

        logger.LogInformation("Sent password reset link to email address {EmailAddress}", message.EmailAddress);
    }
}
