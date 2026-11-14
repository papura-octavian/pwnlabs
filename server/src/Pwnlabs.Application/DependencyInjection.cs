using Microsoft.Extensions.DependencyInjection;
using Pwnlabs.Application.Auth;
using Pwnlabs.Application.Labs;

namespace Pwnlabs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILabService, LabService>();
        
        return services;
    }
}