using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Services;

namespace SportsStore.Controllers {

    public class OrderController : Controller {
        private readonly IOrderRepository repository;
        private readonly Cart cart;
        private readonly IPaymentService paymentService;
        private readonly IConfiguration config;
        private readonly ILogger<OrderController> logger;

        public OrderController(IOrderRepository repoService, Cart cartService,
            IPaymentService paymentService, IConfiguration config,
            ILogger<OrderController> logger) {
            repository = repoService;
            cart = cartService;
            this.paymentService = paymentService;
            this.config = config;
            this.logger = logger;
        }

        public ViewResult Checkout() => View(new Order());

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order) {
            if (cart.Lines.Count() == 0) {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (!ModelState.IsValid) {
                logger.LogWarning("Checkout attempted with invalid state for {ItemCount} cart items",
                    cart.Lines.Count());
                return View();
            }

            order.Lines = cart.Lines.ToArray();
            order.PaymentStatus = "Pending";
            repository.SaveOrder(order);

            logger.LogInformation(
                "Order {OrderId} created for {ItemCount} items totalling {Total:C}",
                order.OrderID, order.Lines.Count, cart.ComputeTotalValue());

            try {
                PaymentIntentResult intent =
                    await paymentService.CreatePaymentIntentAsync(cart.ComputeTotalValue());

                order.PaymentIntentId = intent.PaymentIntentId;
                order.PaymentClientSecret = intent.ClientSecret;
                repository.SaveOrder(order);

                logger.LogInformation(
                    "PaymentIntent {PaymentIntentId} attached to Order {OrderId}",
                    order.PaymentIntentId, order.OrderID);

                return RedirectToAction("Payment", new { orderId = order.OrderID });
            } catch (Exception ex) {
                logger.LogError(ex,
                    "Failed to create PaymentIntent for Order {OrderId}", order.OrderID);
                ModelState.AddModelError("", "Payment initialisation failed. Please try again.");
                return View();
            }
        }

        public IActionResult Payment(int orderId) {
            Order? order = repository.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order == null || string.IsNullOrEmpty(order.PaymentClientSecret))
                return RedirectToAction("Checkout");

            var vm = new PaymentViewModel {
                OrderId = orderId,
                ClientSecret = order.PaymentClientSecret,
                PublishableKey = config["Stripe:PublishableKey"] ?? string.Empty,
                Amount = order.Lines.Sum(l => l.Product.Price * l.Quantity),
                ReturnUrl = Url.Action("PaymentReturn", "Order",
                    new { orderId }, Request.Scheme)!
            };
            return View(vm);
        }

        public async Task<IActionResult> PaymentReturn(int orderId, string? payment_intent) {
            if (string.IsNullOrEmpty(payment_intent))
                return RedirectToAction("Checkout");

            Order? order = repository.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order == null) return NotFound();

            try {
                string status = await paymentService.GetPaymentStatusAsync(payment_intent);
                order.PaymentStatus = status;
                repository.SaveOrder(order);

                if (status == "succeeded") {
                    logger.LogInformation(
                        "Payment succeeded for Order {OrderId}, PaymentIntent {PaymentIntentId}",
                        orderId, payment_intent);
                    cart.Clear();
                    return RedirectToPage("/Completed", new { orderId = order.OrderID });
                }

                logger.LogWarning(
                    "Payment {Status} for Order {OrderId}, PaymentIntent {PaymentIntentId}",
                    status, orderId, payment_intent);
                return View("PaymentFailed", order);
            } catch (Exception ex) {
                logger.LogError(ex,
                    "Payment status check failed for Order {OrderId}, PaymentIntent {PaymentIntentId}",
                    orderId, payment_intent);
                order.PaymentStatus = "error";
                repository.SaveOrder(order);
                return View("PaymentFailed", order);
            }
        }
    }
}
