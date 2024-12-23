using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Logic.Options;

namespace RateMyPet.Logic.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services)
    {
        services.AddTransient<ImageProcessor>()
            .AddTransient<IEmailSender, EmailSender>();

        services.AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName);

        return services;
    }
}
