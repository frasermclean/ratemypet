using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using RateMyPet.Api.Options;

namespace RateMyPet.Api.Services;

public class IdentityEmailSender(
    IOptions<EmailSenderOptions> options,
    ILogger<IdentityEmailSender> logger,
    EmailClient emailClient)
    : IEmailSender
{
    private readonly string senderAddress = options.Value.SenderAddress;

    public async Task SendEmailAsync(string recepientAddress, string subject, string htmlMessage)
    {
        var content = new EmailContent(subject) { Html = htmlMessage };
        var message = new EmailMessage(senderAddress, recepientAddress, content);

        var operation = await emailClient.SendAsync(WaitUntil.Completed, message);

        logger.LogInformation("Email queued for delivery, status: {Status}", operation.Value.Status);
    }
}
