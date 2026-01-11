using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Services;
using OrderProcessing.Domain.Enums;
using OrderProcessing.Infrastructure.Data;
using Microsoft.Extensions.Options;
using OrderProcessing.Application.Configuration;
using NUnit.Framework;


namespace OrderProcessing.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderDbContext _context = null!;
        private OrderService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderDbContext(options);
            var rulesOptions = Options.Create(new OrderRulesOptions
            {
                MaxItemQuantity = 100,
                MaxItemsPerOrder = 10
            });

            _service = new OrderService(_context, rulesOptions);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void CreateOrder_ShouldFail_WhenQuantityExceedsLimit()
        {
            var request = new CreateOrderRequest(
    [
        new CreateOrderItemDto("Laptop",1000, 50000)
    ]);

            Assert.ThrowsAsync<ArgumentException>(
                async () => await _service.CreateOrderAsync(request));
        }

        [Test]
        public async Task CreateOrder_Should_Set_Status_As_Pending()
        {
            var request = new CreateOrderRequest(
    [
        new CreateOrderItemDto("Laptop", 1, 50000)
    ]);

            var orderId = await _service.CreateOrderAsync(request);
            var order = await _context.Orders.FindAsync(orderId);

            Assert.That(order, Is.Not.Null);
            Assert.That(order!.Status, Is.EqualTo(OrderStatus.Pending));
        }

        [Test]
        public async Task CancelOrder_Should_Succeed_When_Pending()
        {
            var orderId = await CreateSampleOrder();

            var result = await _service.CancelOrderAsync(orderId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CancelOrder_Should_Fail_When_Not_Pending()
        {
            var orderId = await CreateSampleOrder();
            await _service.UpdateOrderStatusAsync(orderId, OrderStatus.Processing);

            var result = await _service.CancelOrderAsync(orderId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateOrderStatus_Should_Allow_Valid_Transition()
        {
            var orderId = await CreateSampleOrder();

            var result = await _service.UpdateOrderStatusAsync(orderId, OrderStatus.Processing);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UpdateOrderStatus_Should_Block_Invalid_Transition()
        {
            var orderId = await CreateSampleOrder();

            var result = await _service.UpdateOrderStatusAsync(orderId, OrderStatus.Delivered);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetOrders_Should_Return_All_When_Status_Not_Provided()
        {
            await CreateSampleOrder();
            await CreateSampleOrder();

            var orders = await _service.GetAllOrdersAsync(null);

            Assert.That(orders.Count(), Is.EqualTo(2));
        }

        private async Task<Guid> CreateSampleOrder()
        {
            var request = new CreateOrderRequest(new()
    {
        new CreateOrderItemDto("Phone", 1, 30000)
    });

            return await _service.CreateOrderAsync(request);
        }
    }
}