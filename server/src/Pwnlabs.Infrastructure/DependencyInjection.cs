using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pwnlabs.Infrastructure.Persistence;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Infrastructure.Persistence.Repositories;
using Pwnlabs.Infrastructure.Security;

namespace Pwnlabs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured. " +
                "Set it with: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"<value>\" --project server/src/Pwnlabs.Api");

        services.AddDbContext<PwnlabsDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddHealthChecks()
            .AddDbContextCheck<PwnlabsDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

        return services;
    }
}