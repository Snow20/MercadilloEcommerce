using System.Text;
using System.Text.Json;

namespace EcommerceApi.Services;

public class PaypalPaymentService : IPaymentGatewayService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public PaypalPaymentService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public string ProviderName => "PayPal";

    public async Task<PaymentSessionResponse> CreatePaymentSessionAsync(CheckoutRequest request)
    {
        var clientId = _config["Paypal:ClientId"] ?? "PAYPAL_CLIENT_ID_MOCK";
        var clientSecret = _config["Paypal:ClientSecret"] ?? "PAYPAL_CLIENT_SECRET_MOCK";

        // MOCK / Integración REST para entorno sandbox de PayPal
        var sessionId = $"PAYPAL-ORDER-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var approvalUrl = $"https://www.sandbox.paypal.com/checkoutnow?token={sessionId}";

        return await Task.FromResult(new PaymentSessionResponse(sessionId, approvalUrl));
    }
}