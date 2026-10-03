using Microsoft.EntityFrameworkCore;
using TodoApp.Api;
using TodoApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Render injeta a porta na variavel PORT.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default nao configurada.");

builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache();

// Origens do front (Blazor WASM) separadas por virgula, ex.: Cors__Origins=https://app.onrender.com
var origins = (builder.Configuration["Cors:Origins"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.UseExceptionHandler();
app.UseCors();

app.MapHealthChecks("/healthz");
app.MapTodoEndpoints();
app.MapUsageEndpoints();

app.Run();
