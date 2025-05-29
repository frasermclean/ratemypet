using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Messages;
using RateMyPet.Storage;

namespace RateMyPet.Jobs.Functions;

public class HandleRegisterConfirmation(
    ILogger<HandleRegisterConfirmation> logger,
    IEmailSender emailSender)
{
    [Function(nameof(HandleRegisterConfirmation))]
    public async Task ExecuteAsync(
        [QueueTrigger(QueueNames.RegisterConfirmation)]
        RegisterConfirmationMessage message,
        CancellationToken cancellationToken)
    {
        await emailSender.SendConfirmationEmailAsync(message.EmailAddress, message.UserId, message.ConfirmationToken,
            cancellationToken);
        logger.LogInformation("Sent registration confirmation email to {EmailAddress}", message.EmailAddress);
    }
}
