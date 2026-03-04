using Microsoft.AspNetCore.Mvc;
using SportsStore.Infrastructure;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Services;
using Stripe;

namespace SportsStore.Controllers {

    public class OrderController : Controller {
        private const string PendingOrderKey = "PendingOrder";

        private readonly IOrderRepository repository;
        private readonly Cart cart;
        private readonly IPaymentService paymentService;
        private readonly string publishableKey;
        private readonly bool useMock;
        private readonly ILogger<OrderController> logger;

        public OrderController(
            IOrderRepository repoService,
            Cart cartService,
            IPaymentService paymentSvc,
            IConfiguration config,
            ILogger<OrderController> loggerService) {
            repository = repoService;
            cart = cartService;
            paymentService = paymentSvc;
            publishableKey = config["Stripe:PublishableKey"];
            if (string.IsNullOrWhiteSpace(publishableKey))
                throw new InvalidOperationException(
                    "Stripe:PublishableKey is not configured. Set it via dotnet user-secrets.");
            useMock = config.GetValue<bool>("Stripe:UseMock");
            logger = loggerService;
        }

        public ViewResult Checkout() {
            logger.LogInformation("Checkout page accessed by {UserName}",
                User.Identity?.Name ?? "Anonymous");
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order) {
            logger.LogInformation(
                "Checkout submitted for {CustomerName}, {City}, {Country}",
                order.Name, order.City, order.Country);

            if (!cart.Lines.Any()) {
                logger.LogWarning(
                    "Checkout attempted with empty cart by {CustomerName}", order.Name);
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }

            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                logger.LogWarning(
                    "Checkout validation failed for {CustomerName}. Errors: {ValidationErrors}",
                    order.Name, errors);
                return View(order);
            }

            try {
                decimal total = cart.ComputeTotalValue();
                PaymentIntentResult intent =
                    await paymentService.CreatePaymentIntentAsync(total);

                order.PaymentIntentId = intent.PaymentIntentId;
                HttpContext.Session.SetJson(PendingOrderKey, order);

                logger.LogInformation(
                    "PaymentIntent {PaymentIntentId} created for {CustomerName}, amount {Amount}",
                    intent.PaymentIntentId, order.Name, total);

                return View("Payment", new PaymentViewModel {
                    ClientSecret = intent.ClientSecret,
                    PublishableKey = publishableKey,
                    Amount = total,
                    IsMock = useMock
                });
            }
            catch (StripeException ex) {
                logger.LogError(ex,
                    "Stripe error creating PaymentIntent for {CustomerName}", order.Name);
                ModelState.AddModelError("", "Unable to initiate payment. Please try again.");
                return View(order);
            }
        }

        public async Task<IActionResult> PaymentReturn(
            string? payment_intent,
            string? redirect_status) {

            if (string.IsNullOrEmpty(payment_intent) ||
                string.IsNullOrEmpty(redirect_status)) {
                logger.LogWarning("PaymentReturn called with missing parameters");
                return RedirectToAction(nameof(Checkout));
            }

            logger.LogInformation(
                "PaymentReturn: PaymentIntent {PaymentIntentId}, status {Status}",
                payment_intent, redirect_status);

            if (redirect_status != "succeeded") {
                logger.LogWarning(
                    "Payment {PaymentIntentId} did not succeed (redirect_status: {Status})",
                    payment_intent, redirect_status);
                TempData["PaymentError"] = redirect_status == "canceled"
                    ? "Your payment was cancelled."
                    : "Your payment failed. Please check your details and try again.";
                return RedirectToAction(nameof(PaymentFailed));
            }

            try {
                string stripeStatus =
                    await paymentService.GetPaymentStatusAsync(payment_intent);

                if (stripeStatus != "succeeded") {
                    logger.LogWarning(
                        "PaymentIntent {PaymentIntentId} status from Stripe is {Status}",
                        payment_intent, stripeStatus);
                    TempData["PaymentError"] = "Payment could not be confirmed. Please try again.";
                    return RedirectToAction(nameof(PaymentFailed));
                }

                var order = HttpContext.Session.GetJson<Order>(PendingOrderKey);
                if (order is null) {
                    logger.LogWarning(
                        "No pending order in session for PaymentIntent {PaymentIntentId}",
                        payment_intent);
                    TempData["PaymentError"] = "Session expired. Please place your order again.";
                    return RedirectToAction(nameof(PaymentFailed));
                }

                order.Lines = cart.Lines.ToArray();
                order.PaymentStatus = "succeeded";

                repository.SaveOrder(order);
                cart.Clear();
                HttpContext.Session.Remove(PendingOrderKey);

                logger.LogInformation(
                    "Order {OrderId} saved for {CustomerName} — {ItemCount} line(s), " +
                    "PaymentIntent {PaymentIntentId}",
                    order.OrderID, order.Name, order.Lines.Count, payment_intent);

                return RedirectToPage("/Completed", new { orderId = order.OrderID });
            }
            catch (StripeException ex) {
                logger.LogError(ex,
                    "Stripe error verifying PaymentIntent {PaymentIntentId}", payment_intent);
                TempData["PaymentError"] = "Unable to verify payment status. Please contact support.";
                return RedirectToAction(nameof(PaymentFailed));
            }
            catch (Exception ex) {
                logger.LogError(ex,
                    "Unexpected error processing PaymentIntent {PaymentIntentId}", payment_intent);
                TempData["PaymentError"] = "An unexpected error occurred. Please contact support.";
                return RedirectToAction(nameof(PaymentFailed));
            }
        }

        public ViewResult PaymentFailed() {
            logger.LogWarning("PaymentFailed page displayed. Error: {PaymentError}",
                TempData["PaymentError"]);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmMockPayment() {
            var order = HttpContext.Session.GetJson<Order>(PendingOrderKey);
            if (order is null) {
                logger.LogWarning("[MOCK] ConfirmMockPayment called but no pending order in session");
                return RedirectToAction(nameof(Checkout));
            }

            order.Lines = cart.Lines.ToArray();
            order.PaymentStatus = "succeeded";

            repository.SaveOrder(order);
            cart.Clear();
            HttpContext.Session.Remove(PendingOrderKey);

            logger.LogInformation(
                "[MOCK] Order {OrderId} saved for {CustomerName} — {ItemCount} line(s)",
                order.OrderID, order.Name, order.Lines.Count);

            return RedirectToPage("/Completed", new { orderId = order.OrderID });
        }
    }
}
