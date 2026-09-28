namespace EcommerceApi.Services;

public class PaymentGatewayFactory
{
    private readonly IEnumerable<IPaymentGatewayService> _services;

    public PaymentGatewayFactory(IEnumerable<IPaymentGatewayService> services)
    {
        _services = services;
    }

    public IPaymentGatewayService GetService(string provider)
    {
        var service = _services.FirstOrDefault(s => s.ProviderName.Equals(provider, StringComparison.OrdinalIgnoreCase));
        if (service == null)
        {
            throw new NotSupportedException($"A pasarela de pago '{provider}' non está soportada.");
        }
        return service;
    }
}