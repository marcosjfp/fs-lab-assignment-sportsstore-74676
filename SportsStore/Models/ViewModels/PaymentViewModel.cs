namespace SportsStore.Models.ViewModels {

    public class PaymentViewModel {
        public string ClientSecret { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsMock { get; set; }
    }
}
