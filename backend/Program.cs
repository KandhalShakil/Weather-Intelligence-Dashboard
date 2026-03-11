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
            "http://localhost:5173",
            "http://localhost:3000",
            "https://www.weather-intelligence-dashboard.kandhal.tech"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});
var app = builder.Build();
// Use CORS before routing
app.UseCors("AllowFrontend");
// Map controllers
app.MapControllers();
app.Run();
