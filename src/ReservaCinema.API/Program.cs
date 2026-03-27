using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ReservaCinema.Application.Services;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Application.Validators.Sessions;
using ReservaCinema.Infrastructure;
using ReservaCinema.Infrastructure.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register Application Services
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<CreateSessionRequestValidator>();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0.0",
        Title = "Reserva Cinema API",
        Description = "API para gerenciamento de reservas de sessões de cinema",
        Contact = new OpenApiContact
        {
            Name = "Reserva Cinema",
            Url = new Uri("https://github.com/developerviana/Reserva-Cinema")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments in documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ReservaCinemaDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Reserva Cinema API v1");
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = "Reserva Cinema API - Swagger";
    options.DefaultModelsExpandDepth(2);
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
