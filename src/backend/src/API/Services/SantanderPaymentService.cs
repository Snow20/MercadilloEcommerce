namespace EcommerceApi.Services;

public class SantanderPaymentService : IPaymentGatewayService
{
    private readonly IConfiguration _config;

    public SantanderPaymentService(IConfiguration config)
    {
        _config = config;
    }

    public string ProviderName => "Santander";

    public async Task<PaymentSessionResponse> CreatePaymentSessionAsync(CheckoutRequest request)
    {
        var orderId = $"SAN-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var redirectUrl = $"https://sandbox.santander.es/checkout?order={orderId}";

        return await Task.FromResult(new PaymentSessionResponse(orderId, redirectUrl));
    }
}