using System.Text;
using EcommerceApi.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;

var builderOptions = new WebApplicationOptions
{
    Args = args
};

var builder = WebApplication.CreateBuilder(builderOptions);

// Desactivar el reloadOnChange para evitar agotar instancias inotify
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();

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

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar servicios de las pasarelas de pago y la factoría
builder.Services.AddScoped<IPaymentGatewayService, StripePaymentService>();
builder.Services.AddScoped<IPaymentGatewayService, PaypalPaymentService>();
builder.Services.AddScoped<IPaymentGatewayService, AbancaPaymentService>();
builder.Services.AddScoped<IPaymentGatewayService, SantanderPaymentService>();
builder.Services.AddScoped<IPaymentGatewayService, WisePaymentService>();
builder.Services.AddScoped<PaymentGatewayFactory>();

// LEER DIRECTAMENTE DESDE LA CONFIGURACIÓN DE APPSETTINGS O ENV VARS
var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
if (string.IsNullOrEmpty(stripeSecretKey))
{
    stripeSecretKey = "sk_test_51UKcUGHBWzHjh2BOQoLwKQC5v2AJEXGFjaI2ssHrtSnv9lgeLdOkK2nnCNrB2U6JHaN6OT7qQzOQL5cbMfmENySh00WIgtKYi2";
}
StripeConfiguration.ApiKey = stripeSecretKey;

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

// RUTA 4: Endpoint Unificado Multi-Pasarela (Stripe, PayPal)
app.MapPost("/api/payment/checkout", async (CheckoutPaymentRequest request, PaymentGatewayFactory factory) =>
{
    try
    {
        var provider = string.IsNullOrWhiteSpace(request.Provider) ? "Stripe" : request.Provider;
        var paymentService = factory.GetService(provider);
        var response = await paymentService.CreatePaymentSessionAsync(new CheckoutRequest(request.ProductId, request.ProductName, request.Price));
        
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// RUTA 4 (COMPATIBILIDAD): Endpoint legado para procesar pagos con Stripe Checkout
app.MapPost("/api/payment/create-checkout-session", async (CheckoutRequest request, PaymentGatewayFactory factory) =>
{
    var paymentService = factory.GetService("Stripe");
    var response = await paymentService.CreatePaymentSessionAsync(request);
    return Results.Ok(new { sessionId = response.SessionId, url = response.Url });
});

// RUTA 5: Webhook de confirmación de Stripe
app.MapPost("/api/payment/webhook", async (HttpRequest req) =>
{
    var json = await new StreamReader(req.Body).ReadToEndAsync();
    var webhookSecret = builder.Configuration["Stripe:WebhookSecret"];

    try
    {
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            req.Headers["Stripe-Signature"],
            webhookSecret,
            throwOnApiVersionMismatch: false
        );

        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Session;
            Console.WriteLine($"[Stripe Webhook OK]: Pago confirmado para la sesión {session?.Id}");
        }

        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Stripe Webhook Error]: {ex.Message}");
        return Results.BadRequest();
    }
});

app.Run();

// -------------------------------------------------------------
// LOS TIPOS Y RECORDS SE DECLARAN ÚNICAMENTE AL FINAL DE TODO
// -------------------------------------------------------------
public record CartItemRequest(int ProductId, string ProductName, decimal Price);
public record IncidentRequest(string Title, string Description, string Severity);
public record IncidentAlertEvent(string Title, string Description, string Severity);
public record CheckoutPaymentRequest(int ProductId, string ProductName, decimal Price, string Provider);