using System.Text.Json.Serialization;
using SalesManagement.Application.Interfaces;
using SalesManagement.Application.Services;
using SalesManagement.Data.Database;
using SalesManagement.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

var connectionString =
    builder.Configuration.GetConnectionString("SalesManagement")
    ?? throw new InvalidOperationException(
        "Connection string 'SalesManagement' was not found.");

builder.Services.AddSingleton(
    new SqlConnectionFactory(connectionString));

builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictService, DistrictService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program;