namespace EcommerceApi.Services;

public interface IPaymentGatewayService
{
    string ProviderName { get; }
    Task<PaymentSessionResponse> CreatePaymentSessionAsync(CheckoutRequest request);
}

public record PaymentSessionResponse(string SessionId, string Url);
public record CheckoutRequest(int ProductId, string ProductName, decimal Price);