using System.Net.Http;
using System.Text;
using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    Configuration = new ConfigurationManager()
});

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpClient();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<IncidentAlertConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });
        cfg.ReceiveEndpoint("servicenow-incident-queue", e =>
        {
            e.ConfigureConsumer<IncidentAlertConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();

public record IncidentAlertEvent(string Title, string Description, string Severity);

public class IncidentAlertConsumer : IConsumer<IncidentAlertEvent>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public IncidentAlertConsumer(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<IncidentAlertEvent> context)
    {
        var msg = context.Message;
        var client = _httpClientFactory.CreateClient();
        
        var instanceUrl = _configuration["ServiceNow:InstanceUrl"] ?? "https://dev12345.service-now.com";
        var username = _configuration["ServiceNow:Username"] ?? "admin";
        var password = _configuration["ServiceNow:Password"] ?? "admin";

        var requestUrl = $"{instanceUrl}/api/now/table/incident";
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

        var payload = new
        {
            short_description = msg.Title,
            comments = msg.Description,
            urgency = msg.Severity == "CRITICAL" ? "1" : "3"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

        await client.SendAsync(request);
    }
}