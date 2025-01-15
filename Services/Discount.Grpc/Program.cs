using Discount.Grpc.Behaviors;
using Discount.Grpc.Data;
using Discount.Grpc.Middleware;
using Discount.Grpc.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
});

builder.Services.AddGrpc();
builder.Services.AddDbContext<DiscountContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Database")));

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Discount.Grpc"))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter();
    });

var app = builder.Build();

app.UseMigration();
app.MapGrpcService<DiscountService>();
app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();
app.Run();
