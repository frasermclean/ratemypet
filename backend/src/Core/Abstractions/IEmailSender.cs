namespace RateMyPet.Core.Abstractions;

public interface IEmailSender
{
    Task SendEmailAsync(string recipientAddress, string subject, string htmlMessage,
        CancellationToken cancellationToken = default);
}
