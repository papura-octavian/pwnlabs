using Microsoft.Extensions.DependencyInjection;
using Pwnlabs.Application.Auth;
using Pwnlabs.Application.Abstractions;

namespace Pwnlabs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}