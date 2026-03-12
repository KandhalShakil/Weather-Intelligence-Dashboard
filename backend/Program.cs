using Microsoft.Extensions.FileProviders;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpClient();
// builder.Services.AddOpenApi();
builder.Services.AddScoped<WeatherService>();

// Configure CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "http://localhost:5235",
            "http://127.0.0.1:3000",
            "http://127.0.0.1:5173",
            "http://127.0.0.1:5235",
            "https://weather-intelligence-dashboard.kandhal.tech",
            "https://www.weather-intelligence-dashboard.kandhal.tech",
            "https://weather-intelligence-dashboard.onrender.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

var frontendPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "frontend"));
var frontendProvider = new PhysicalFileProvider(frontendPath);

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = frontendProvider
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = frontendProvider
});

// Use CORS before routing
app.UseCors("AllowFrontend");

// Map controllers
app.MapControllers();

app.MapFallback(async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(Path.Combine(frontendPath, "index.html"));
});

app.Run();



