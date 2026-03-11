using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SportsStore.Controllers;
using SportsStore.Infrastructure;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Services;
using Xunit;

namespace SportsStore.Tests {

    internal class FakeSession : ISession {
        private readonly Dictionary<string, byte[]> store = new();
        public string Id => "fake-session-id";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => store.Keys;
        public void Clear() => store.Clear();
        public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken ct = default) => Task.CompletedTask;
        public void Remove(string key) => store.Remove(key);
        public void Set(string key, byte[] value) => store[key] = value;
        public bool TryGetValue(string key, out byte[] value) =>
            store.TryGetValue(key, out value!);
    }

    public class OrderControllerTests {

        private static OrderController BuildController(
            IOrderRepository repo,
            Cart cart,
            IPaymentService? paymentService = null) {

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Stripe:PublishableKey"]).Returns("pk_test_fake");

            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s.Value).Returns("false");
            mockConfig.Setup(c => c.GetSection("Stripe:UseMock")).Returns(mockSection.Object);

            var mockLogger = new Mock<ILogger<OrderController>>();
            var mockPayment = paymentService ?? new Mock<IPaymentService>().Object;

            var controller = new OrderController(
                repo, cart, mockPayment, mockConfig.Object, mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new FakeSession();
            controller.ControllerContext = new ControllerContext {
                HttpContext = httpContext
            };
            controller.TempData =
                new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            return controller;
        }

        [Fact]
        public async Task Cannot_Checkout_Empty_Cart() {
            // Arrange
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            Order order = new Order();
            OrderController target = BuildController(mock.Object, cart);

            // Act
            ViewResult? result = await target.Checkout(order) as ViewResult;

            // Assert - check that the order hasn't been stored 
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            // Assert - check that I am passing an invalid model to the view
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Cannot_Checkout_Invalid_ShippingDetails() {
            // Arrange
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            OrderController target = BuildController(mock.Object, cart);
            // Arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");

            // Act - try to checkout
            ViewResult? result = await target.Checkout(new Order()) as ViewResult;

            // Assert - check that the order hasn't been stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            // Assert - check that I am passing an invalid model to the view
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Checkout_With_Valid_Cart_Initiates_Payment() {
            // Arrange
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product { Price = 10 }, 1);

            Mock<IPaymentService> mockPayment = new Mock<IPaymentService>();
            mockPayment
                .Setup(p => p.CreatePaymentIntentAsync(It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentIntentResult("pi_test_123", "secret_test_123"));

            OrderController target = BuildController(mock.Object, cart, mockPayment.Object);

            // Act - try to checkout
            ViewResult? result = await target.Checkout(new Order()) as ViewResult;

            // Assert - PaymentIntent was created
            mockPayment.Verify(
                p => p.CreatePaymentIntentAsync(It.IsAny<decimal>(), It.IsAny<string>()),
                Times.Once);
            // Assert - order is NOT saved yet (happens after payment confirmation)
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Assert - Payment view is returned with correct model
            Assert.Equal("Payment", result?.ViewName);
            Assert.IsType<PaymentViewModel>(result?.Model);
        }

        [Fact]
        public async Task PaymentReturn_Saves_Order_When_Payment_Succeeds() {
            // Arrange
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product { Price = 10 }, 2);

            Mock<IPaymentService> mockPayment = new Mock<IPaymentService>();
            mockPayment
                .Setup(p => p.GetPaymentStatusAsync("pi_test_123"))
                .ReturnsAsync("succeeded");

            OrderController target = BuildController(mock.Object, cart, mockPayment.Object);

            // Pre-populate session with a pending order (as Checkout would have done)
            var pendingOrder = new Order {
                Name = "Test User", Line1 = "1 Test St",
                City = "London", State = "England", Country = "UK",
                PaymentIntentId = "pi_test_123"
            };
            target.HttpContext.Session.SetJson("PendingOrder", pendingOrder);

            // Act
            IActionResult result =
                await target.PaymentReturn("pi_test_123", "succeeded");

            // Assert - order was saved after payment confirmation
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);
            // Assert - redirect to Completed page
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Completed", ((RedirectToPageResult)result).PageName);
        }

        [Fact]
        public async Task PaymentReturn_Does_Not_Save_Order_When_Payment_Fails() {
            // Arrange
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product { Price = 10 }, 1);
            OrderController target = BuildController(mock.Object, cart);

            // Act
            IActionResult result =
                await target.PaymentReturn("pi_test_123", "failed");

            // Assert - order was NOT saved
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Assert - redirect to PaymentFailed action
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(OrderController.PaymentFailed), redirect.ActionName);
        }
    }
}
