using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Database;
using RateMyPet.Storage;
using RateMyPet.Storage.Messaging;

namespace RateMyPet.Jobs.Functions;

public class HandleForgottenPassword(ILogger<HandleForgottenPassword> logger, IEmailSender emailSender, ApplicationDbContext dbContext)
{
    [Function(nameof(HandleForgottenPassword))]
    public async Task ExecuteAsync(
        [QueueTrigger(QueueNames.ForgotPassword)]
        ForgottenPasswordMessage message,
        CancellationToken cancellationToken)
    {
        await emailSender.SendPasswordResetEmailAsync(message.EmailAddress, message.ResetCode, cancellationToken);

        logger.LogInformation("Sent password reset link to email address {EmailAddress}", message.EmailAddress);

        dbContext.UserActivities.Add(UserActivity.ForgotPassword(message.UserId));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
