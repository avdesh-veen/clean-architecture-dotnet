using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;
using SampleProject.Infrastructure.Persistence;
using SampleProject.Infrastructure.Interceptors;
using SampleProject.Worker;
using SampleProject.Worker.Messages;
using SampleProject.Authorization;

// Minimal host wiring for the sample project. This illustrates registering installers,
// Wolverine message handlers, Temporal workflow stubs, and OpenFGA adapter.
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Db + interceptors
        services.AddSingleton<TenantEntityInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, opts) =>
            opts.UseInMemoryDatabase("sample").AddInterceptors(sp.GetRequiredService<TenantEntityInterceptor>()));

        // MediatR
        services.AddMediatR(typeof(Program));

        // Worker + message handlers (Wolverine-style)
        services.AddHostedService<SampleWorker>();
        services.AddTransient<ProjectCreatedHandler>();

        // Temporal / OpenFGA stubs
        services.AddSingleton<IProjectWorkflow, /*YourProjectWorkflow*/ object>();
        services.AddSingleton<IOpenFgaService, /*YourOpenFgaService*/ object>();
    })
    .Build();

await host.RunAsync();
