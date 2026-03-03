namespace SportsStore.Services {

    public class MockPaymentService : IPaymentService {

        public Task<PaymentIntentResult> CreatePaymentIntentAsync(
            decimal amount, string currency = "eur") {
            return Task.FromResult(new PaymentIntentResult("pi_mock_id", "pi_mock_secret"));
        }

        public Task<string> GetPaymentStatusAsync(string paymentIntentId) {
            return Task.FromResult("succeeded");
        }
    }
}

