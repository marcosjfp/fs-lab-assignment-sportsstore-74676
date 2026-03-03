using Stripe;

namespace SportsStore.Services {

    public class StripePaymentService : IPaymentService {
        private readonly StripeClient stripeClient;
        private readonly ILogger<StripePaymentService> logger;

        public StripePaymentService(IConfiguration config, ILogger<StripePaymentService> logger) {
            var secretKey = config["Stripe:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException(
                    "Stripe:SecretKey is not configured. Set it via dotnet user-secrets.");
            stripeClient = new StripeClient(secretKey);
            this.logger = logger;
        }

        public async Task<PaymentIntentResult> CreatePaymentIntentAsync(
            decimal amount, string currency = "eur") {

            var options = new PaymentIntentCreateOptions {
                Amount = (long)(amount * 100),
                Currency = currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions {
                    Enabled = true
                }
            };

            var service = new PaymentIntentService(stripeClient);
            PaymentIntent intent = await service.CreateAsync(options);

            logger.LogInformation(
                "PaymentIntent {PaymentIntentId} created for {Amount} {Currency}",
                intent.Id, amount, currency.ToUpper());

            return new PaymentIntentResult(intent.Id, intent.ClientSecret!);
        }

        public async Task<string> GetPaymentStatusAsync(string paymentIntentId) {
            var service = new PaymentIntentService(stripeClient);
            PaymentIntent intent = await service.GetAsync(paymentIntentId);

            logger.LogInformation(
                "PaymentIntent {PaymentIntentId} retrieved with status {Status}",
                paymentIntentId, intent.Status);

            return intent.Status;
        }
    }
}
