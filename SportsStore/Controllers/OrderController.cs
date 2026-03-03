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

        public OrderController(IOrderRepository repoService, Cart cartService,
            IPaymentService paymentService, IConfiguration config) {
            repository = repoService;
            cart = cartService;
            this.paymentService = paymentService;
            this.config = config;
        }

        public ViewResult Checkout() => View(new Order());

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order) {
            if (cart.Lines.Count() == 0) {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid) {
                order.Lines = cart.Lines.ToArray();
                order.PaymentStatus = "Pending";
                repository.SaveOrder(order);

                PaymentIntentResult intent =
                    await paymentService.CreatePaymentIntentAsync(cart.ComputeTotalValue());

                order.PaymentIntentId = intent.PaymentIntentId;
                order.PaymentClientSecret = intent.ClientSecret;
                repository.SaveOrder(order);

                return RedirectToAction("Payment", new { orderId = order.OrderID });
            }
            return View();
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

            string status = await paymentService.GetPaymentStatusAsync(payment_intent);
            order.PaymentStatus = status;
            repository.SaveOrder(order);

            if (status == "succeeded") {
                cart.Clear();
                return RedirectToPage("/Completed", new { orderId = order.OrderID });
            }

            return View("PaymentFailed", order);
        }
    }
}
