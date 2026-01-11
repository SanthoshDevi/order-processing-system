using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Configuration;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Services;
using OrderProcessing.Host.BackgroundJobs;
using OrderProcessing.Host.Components;
using OrderProcessing.Host.Endpoints;
using OrderProcessing.Host.Middleware;
using OrderProcessing.Infrastructure.Data;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Razor (Blazor host)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// EF Core
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));

// Application services
builder.Services.AddScoped<IOrderService, OrderService>();

// Background job
builder.Services.AddHostedService<OrderStatusUpdater>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

// Swagger (Minimal APIs)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<OrderRulesOptions>(
    builder.Configuration.GetSection("OrderRules"));
var app = builder.Build();

// GLOBAL EXCEPTION HANDLER
app.UseMiddleware<GlobalExceptionMiddleware>();

// Auto-apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Pipeline
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// APIs mapping

app.MapOrderEndpoints();

app.Run();