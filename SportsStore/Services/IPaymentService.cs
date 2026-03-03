namespace SportsStore.Services {

    public record PaymentIntentResult(string PaymentIntentId, string ClientSecret);

    public interface IPaymentService {
        Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency = "eur");
        Task<string> GetPaymentStatusAsync(string paymentIntentId);
    }
}
