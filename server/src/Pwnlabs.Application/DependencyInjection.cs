using Microsoft.Extensions.DependencyInjection;
using Pwnlabs.Application.Auth;
using Pwnlabs.Application.Labs;
using Pwnlabs.Application.Rooms;

namespace Pwnlabs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILabService, LabService>();
        services.AddScoped<IRoomService, RoomService>();
        
        return services;
    }
}