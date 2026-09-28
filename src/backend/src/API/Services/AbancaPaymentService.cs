namespace EcommerceApi.Services;

public class AbancaPaymentService : IPaymentGatewayService
{
    private readonly IConfiguration _config;

    public AbancaPaymentService(IConfiguration config)
    {
        _config = config;
    }

    public string ProviderName => "Abanca";

    public async Task<PaymentSessionResponse> CreatePaymentSessionAsync(CheckoutRequest request)
    {
        var merchantCode = _config["Abanca:MerchantCode"] ?? "999008881";
        var orderId = $"ABANCA-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var redirectUrl = $"https://sis-t.redsys.es/sis/realizarPago?order={orderId}&amount={(long)(request.Price * 100)}";

        return await Task.FromResult(new PaymentSessionResponse(orderId, redirectUrl));
    }
}