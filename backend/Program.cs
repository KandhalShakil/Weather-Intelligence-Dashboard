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
        policy.SetIsOriginAllowed(origin =>
              {
                  if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                  {
                      return false;
                  }

                  return uri.Host is "localhost" or "127.0.0.1"
                      || uri.Host == "www.weather-intelligence-dashboard.kandhal.tech"
                      || uri.Host == "weather-intelligence-dashboard.kandhal.tech";
              })
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Use CORS before routing
app.UseCors("AllowFrontend");

// Map controllers
app.MapControllers();

app.Run();



