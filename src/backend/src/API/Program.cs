using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configuración de JWT
var jwtSecretKey = builder.Configuration["Jwt:Secret"] ?? "SuperSecretKeyGaliciaFeiraEnterprise2026!";
var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// MassTransit & RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// -------------------------------------------------------------
// TODAS LAS RUTAS SE DEFINEN AQUÍ (ANTES DE CUALQUIER TYPE/RECORD)
// -------------------------------------------------------------

// RUTA 1: Catálogo Extendido de Produtos da Feira Gallega
app.MapGet("/api/catalog", () => new[]
{
    new { Id = 1, Name = "Polbo á Feira (Ración)", Price = 18.50 },
    new { Id = 2, Name = "Queixo Arzúa-Ulloa DOP", Price = 9.20 },
    new { Id = 3, Name = "Empanada de Atún e Pementos", Price = 12.00 },
    new { Id = 4, Name = "Vino Albariño Rías Baixas", Price = 16.80 },
    new { Id = 5, Name = "Tarta de Santiago Tradicional", Price = 14.50 },
    new { Id = 6, Name = "Zamburiñas do Saco (12 ud)", Price = 22.00 },
    new { Id = 7, Name = "Pan de Cea Protexido (Bolo)", Price = 4.20 },
    new { Id = 8, Name = "Licor Café Orujo Galego", Price = 13.90 },
    new { Id = 9, Name = "Pementos de Padrón / Herbón", Price = 5.50 },
    new { Id = 10, Name = "Mel Artesanal das Fragas do Eume", Price = 8.80 }
});

// RUTA 2: Recepción de Incidencias enviadas a RabbitMQ -> ServiceNow
app.MapPost("/api/incidents", async (IPublishEndpoint publishEndpoint, IncidentRequest request) =>
{
    await publishEndpoint.Publish<IncidentAlertEvent>(new(
        request.Title ?? "Incidencia sin título",
        request.Description ?? "Sin descripción",
        request.Severity ?? "HIGH"
    ));

    return Results.Accepted(null, new { message = "Incidencia enviada correctamente a ServiceNow" });
});

// RUTA 3: Recepción de órdenes de compra del Carrito
app.MapPost("/api/cart", (CartItemRequest request) =>
{
    return Results.Ok(new { 
        message = $"Producto '{request.ProductName}' (ID: {request.ProductId}) engadido ao carro de compras con éxito.",
        timestamp = DateTime.UtcNow
    });
});

// app.Run() CIERRA LAS TOP-LEVEL STATEMENTS
app.Run();

// -------------------------------------------------------------
// LOS TIPOS Y RECORDS SE DECLARAN ÚNICAMENTE AL FINAL DE TODO
// -------------------------------------------------------------
public record CartItemRequest(int ProductId, string ProductName, decimal Price);
public record IncidentRequest(string Title, string Description, string Severity);
public record IncidentAlertEvent(string Title, string Description, string Severity);