using FluentValidation;
using Serilog;
using WebApi.Endpoints;
using WebApi.Extensions;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiServices();

builder.AddCustomJsonConfigurations();

builder.Services.AddKestrelConfiguration(builder.Configuration);

// Configures Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add services to the container.
builder.Services.ConfigureCors(builder.Configuration);
builder.Services.ConfigureDomainEventHandlers();
builder.Services.ConfigureFlatConfigurationSync(builder.Configuration);
builder.Services.ConfigureGlobalExceptionHandling();
builder.Services.ConfigureHandlers();
builder.Services.ConfigureJsonOptions();
builder.Services.ConfigureMongoDB(builder.Configuration);
builder.Services.ConfigureRepositories();
builder.Services.ConfigureServices();
builder.Services.ConfigureValidation();

builder.Services.AddHealthChecks();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseOpenApiWithSwagger();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.

app.UseCors("CorsPolicy");

app.UseMiddleware<EndpointLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapAlertsEndpoints();
app.MapUsersEndpoints();
app.MapWishesEndpoints();
app.MapServerStatusesEndpoints();
app.MapServersEndpoints();

app.UseHttpsRedirection();
app.Run();