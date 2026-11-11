using Microsoft.Extensions.DependencyInjection;
using Pwnlabs.Application.Auth;

namespace Pwnlabs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}