using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleProject.Infrastructure.Interceptors;
using SampleProject.Infrastructure.Persistence;
using SampleProject.Authorization;
using SampleProject.Worker.Temporal;

namespace SampleProject.Infrastructure.Installers;

/// <summary>
/// Installer pattern example: registers DbContext and interceptors.
/// </summary>
public class DbInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<TenantEntityInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, opts) =>
        {
            opts.UseInMemoryDatabase("sample");
            var interceptor = sp.GetRequiredService<TenantEntityInterceptor>();
            opts.AddInterceptors(interceptor);
        });

        // Register stubs for OpenFGA and Temporal for sample purposes
        services.AddSingleton<IOpenFgaService, /*ReplaceWithConcrete*/ object>();
        services.AddSingleton<IProjectWorkflow, /*ReplaceWithConcrete*/ object>();
    }
}

public interface IInstaller
{
    void InstallServices(IServiceCollection services, IConfiguration configuration);
}
