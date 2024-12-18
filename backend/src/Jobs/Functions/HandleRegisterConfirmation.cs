using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core.Messages;
using RateMyPet.Jobs.Options;
using RateMyPet.Jobs.Services;
using RateMyPet.Persistence;

namespace RateMyPet.Jobs.Functions;

public class HandleRegisterConfirmation(
    ILogger<HandleRegisterConfirmation> logger,
    IOptions<EmailOptions> options,
    IEmailSender emailSender)
{
    private readonly string? frontendBaseUrl = options.Value.FrontendBaseUrl;

    [Function(nameof(HandleRegisterConfirmation))]
    public async Task ExecuteAsync(
        [QueueTrigger(QueueNames.RegisterConfirmation)] RegisterConfirmationMessage message,
        CancellationToken cancellationToken)
    {
        const string subject = "Confirm your email";

        var confirmationLink =
            $"{frontendBaseUrl}/auth/confirm-email?userId={message.UserId}&token={message.ConfirmationToken}";

        var htmlMessage = $"""
                           <html><body>
                           <h1>Welcome to Rate My Pet!</h1>
                           <p>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.</p>
                           </body></html>
                           """;


        await emailSender.SendEmailAsync(message.EmailAddress, subject, htmlMessage, cancellationToken);
        logger.LogInformation("Sent registration confirmation email to {EmailAddress}", message.EmailAddress);
    }
}
