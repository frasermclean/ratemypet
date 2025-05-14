using Azure.Communication.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;

namespace RateMyPet.Email;

public static class ServiceRegistration
{
    public static IServiceCollection AddEmailSending(this IServiceCollection services)
    {
        services.AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddSingleton<EmailClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<EmailOptions>>();
            var endpoint = options.Value.AcsEndpoint;
            var credential = TokenCredentialFactory.Create();
            return new EmailClient(endpoint, credential);
        });

        services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }
}
