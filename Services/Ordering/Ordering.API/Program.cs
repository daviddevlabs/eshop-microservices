using Ordering.API;
using Ordering.Application.Config;
using Ordering.Infrastructure.Config;
using Ordering.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);
var applicationAssembly = typeof(Ordering.Application.Config.DependencyInjection).Assembly;

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration, builder.Environment, applicationAssembly)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.UseApiServices();
app.Run();