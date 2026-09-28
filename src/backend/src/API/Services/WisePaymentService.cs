namespace EcommerceApi.Services;

public class WisePaymentService : IPaymentGatewayService
{
    private readonly IConfiguration _config;

    public WisePaymentService(IConfiguration config)
    {
        _config = config;
    }

    public string ProviderName => "Wise";

    public async Task<PaymentSessionResponse> CreatePaymentSessionAsync(CheckoutRequest request)
    {
        var profileId = _config["Wise:ProfileId"] ?? "WISE-MOCK-PROFILE";
        var quoteId = $"WISE-QUOTE-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var redirectUrl = $"https://sandbox.wise.com/pay/me/{quoteId}";

        return await Task.FromResult(new PaymentSessionResponse(quoteId, redirectUrl));
    }
}