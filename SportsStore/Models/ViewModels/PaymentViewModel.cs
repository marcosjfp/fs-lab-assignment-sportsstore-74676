namespace SportsStore.Models.ViewModels {

    public class PaymentViewModel {
        public int OrderId { get; set; }
        public string ClientSecret { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ReturnUrl { get; set; } = string.Empty;
    }
}

