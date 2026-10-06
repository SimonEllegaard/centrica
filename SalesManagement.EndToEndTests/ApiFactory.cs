using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SalesManagement.EndToEndTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Testing.json", optional: false);
        });
    }
}