using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Messages;
using RateMyPet.Storage;

namespace RateMyPet.Jobs.Functions;

public class HandleForgottenPassword(ILogger<HandleForgottenPassword> logger, IEmailSender emailSender)
{
    [Function(nameof(HandleForgottenPassword))]
    public async Task ExecuteAsync(
        [QueueTrigger(QueueNames.ForgotPassword)]
        ForgottenPasswordMessage message,
        CancellationToken cancellationToken)
    {
        await emailSender.SendPasswordResetEmailAsync(message.EmailAddress, message.ResetCode, cancellationToken);
        logger.LogInformation("Sent password reset link to email address {EmailAddress}", message.EmailAddress);
    }
}
