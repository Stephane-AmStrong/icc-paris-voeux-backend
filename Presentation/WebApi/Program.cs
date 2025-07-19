using AspNetCore.Swagger.Themes;
using FluentValidation;
using Serilog;
using WebApi.Endpoinds;
using WebApi.Extensions;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configures Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureJsonOptions();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureMongoDB(builder.Configuration);
builder.Services.ConfigureRepositories();
builder.Services.ConfigureServices();
builder.Services.ConfigureGlobalExceptionHandling();

builder.Services.AddHealthChecks();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(ModernStyle.Dark);

app.UseCors("CorsPolicy");

app.UseMiddleware<EndpointLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapWishesEndpoints();

app.UseHttpsRedirection();

app.Run();
