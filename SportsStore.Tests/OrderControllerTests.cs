using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Services;
using System.Threading.Tasks;
using Xunit;

namespace SportsStore.Tests {

    public class OrderControllerTests {

        [Fact]
        public async Task Cannot_Checkout_Empty_Cart() {
            // Arrange - create a mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Mock<IPaymentService> mockPayment = new Mock<IPaymentService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            // Arrange - create an empty cart
            Cart cart = new Cart();
            // Arrange - create the order
            Order order = new Order();
            // Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart,
                mockPayment.Object, mockConfig.Object);

            // Act
            ViewResult? result = (await target.Checkout(order)) as ViewResult;

            // Assert - check that the order hasn't been stored 
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            // Assert - check that I am passing an invalid model to the view
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Cannot_Checkout_Invalid_ShippingDetails() {

            // Arrange - create a mock order repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Mock<IPaymentService> mockPayment = new Mock<IPaymentService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            // Arrange - create a cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart,
                mockPayment.Object, mockConfig.Object);
            // Arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");

            // Act - try to checkout
            ViewResult? result = (await target.Checkout(new Order())) as ViewResult;

            // Assert - check that the order hasn't been passed stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            // Assert - check that I am passing an invalid model to the view
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Can_Checkout_And_Submit_Order() {
            // Arrange - create a mock order repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Mock<IPaymentService> mockPayment = new Mock<IPaymentService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockPayment
                .Setup(p => p.CreatePaymentIntentAsync(It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentIntentResult("pi_test_id", "pi_test_secret"));
            // Arrange - create a cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product { Price = 10 }, 1);
            // Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart,
                mockPayment.Object, mockConfig.Object);

            // Act - try to checkout
            RedirectToActionResult? result =
                (await target.Checkout(new Order())) as RedirectToActionResult;

            // Assert - check that the order has been stored (once for initial save, once to attach payment intent)
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Exactly(2));
            // Assert - check that the method is redirecting to the Payment action
            Assert.Equal("Payment", result?.ActionName);
        }
    }
}
