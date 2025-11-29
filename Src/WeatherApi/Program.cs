using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WeatherApi.Configuration;
using WeatherApi.Middlewares;
using WeatherApi.Models;
using WeatherApi.Services;
using WeatherService.Configurations;
 

var builder = WebApplication.CreateBuilder(args);

// --- Serilog ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// --- Services ---
builder.Services.ConfigureWeatherServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- App ---
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler();
app.UseMiddleware<Logging>();

app.MapGet("/api/weather", GetWeather)
   .Produces<string?>(StatusCodes.Status200OK);

app.Run();

// Endpoint implementation
static async Task<IResult> GetWeather(
    [FromServices] WeatherClientService weatherService,
    [FromServices] ApplicationDbContext context,
    CancellationToken cancellationToken)
{
    string? weather;

    // 1) سعی می‌کنیم از سرویس بیرونی بخونیم
    try
    {
        weather = await weatherService.GetWeatherAsync(cancellationToken);
    }
    catch
    {
        weather = null;
    }

    // 2) اگر سرویس بیرونی fail شد → fallback به DB
    if (weather is null)
    {
        try
        {
            var cached = await context.Weathers
                .AsNoTracking()
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => w.JsonApiResult)
                .FirstOrDefaultAsync(cancellationToken);

            if (cached is null)
                return Results.Ok<string?>(null);

            return Results.Content(cached, "application/json");
        }
        catch
        {
            return Results.Ok<string?>(null);
        }
    }

    // 3) اگر داده جدید داریم → ذخیره در DB (به‌صورت best-effort)
    try
    {
        var entity = new WeatherForecast(weather);
        await context.Weathers.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    catch
    {
        // اگر ذخیره خراب شد، طبق قرارداد هنوز باید پاسخ معتبر بدیم
    }

    // 4) در هر صورت داده‌ی تازه را بدون تغییر برمی‌گردونیم
    return Results.Content(weather, "application/json");
}
