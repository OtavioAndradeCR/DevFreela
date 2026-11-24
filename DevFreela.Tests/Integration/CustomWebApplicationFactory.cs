using DevFreela.Application.Persistence;
using DevFreela.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DevFreela.Tests.Integration;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"DevFreelaIntegrationTests_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<DevFreelaDbContext>>();
            services.RemoveAll<DbContextOptions<DevFreelaDbContext>>();
            services.RemoveAll<DevFreelaDbContext>();
            services.RemoveAll<IDevFreelaDbContext>();

            services.AddDbContext<DevFreelaDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.AddScoped<IDevFreelaDbContext>(serviceProvider =>
                serviceProvider.GetRequiredService<DevFreelaDbContext>());
        });
    }
}
