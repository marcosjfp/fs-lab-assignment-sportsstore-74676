namespace SportsStore.Services {

    public class MockPaymentService : IPaymentService {
        private readonly ILogger<MockPaymentService> logger;

        public MockPaymentService(ILogger<MockPaymentService> logger) {
            this.logger = logger;
        }

        public Task<PaymentIntentResult> CreatePaymentIntentAsync(
            decimal amount, string currency = "eur") {

            var paymentIntentId = $"pi_mock_{Guid.NewGuid():N}";
            var clientSecret   = $"{paymentIntentId}_secret_mock";

            logger.LogWarning(
                "[MOCK] PaymentIntent {PaymentIntentId} created for {Amount} {Currency}",
                paymentIntentId, amount, currency.ToUpper());

            return Task.FromResult(new PaymentIntentResult(paymentIntentId, clientSecret));
        }

        public Task<string> GetPaymentStatusAsync(string paymentIntentId) {
            logger.LogWarning(
                "[MOCK] PaymentIntent {PaymentIntentId} status queried — returning 'succeeded'",
                paymentIntentId);

            return Task.FromResult("succeeded");
        }
    }
}
