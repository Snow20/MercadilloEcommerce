using Stripe;
using Stripe.Checkout;

namespace EcommerceApi.Services;

public class StripePaymentService : IPaymentGatewayService
{
    private readonly IConfiguration _config;

    public StripePaymentService(IConfiguration config)
    {
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public string ProviderName => "Stripe";

    public async Task<PaymentSessionResponse> CreatePaymentSessionAsync(CheckoutRequest request)
    {
        var domain = "http://localhost";

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(request.Price * 100),
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = request.ProductName,
                            Description = "Produto artesanal da Feira Gallega"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = domain + "/?status=success",
            CancelUrl = domain + "/?status=cancel",
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return new PaymentSessionResponse(session.Id, session.Url);
    }
}