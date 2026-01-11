using OrderProcessing.Domain.Enums;
using OrderProcessing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderProcessing.Host.BackgroundJobs
{
    public class OrderStatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderStatusUpdater(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                var pendingOrders = await context.Orders
                    .Where(o => o.Status == OrderStatus.Pending)
                    .ToListAsync(stoppingToken);

                foreach (var order in pendingOrders)
                {
                    order.Status = OrderStatus.Processing;
                }

                await context.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
