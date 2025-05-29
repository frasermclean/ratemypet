namespace RateMyPet.Core.Abstractions;

public interface IEmailSender
{
    Task SendConfirmationEmailAsync(string emailAddress, Guid userId, string confirmationToken,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetEmailAsync(string emailAddress, string resetCode,
        CancellationToken cancellationToken = default);
}
