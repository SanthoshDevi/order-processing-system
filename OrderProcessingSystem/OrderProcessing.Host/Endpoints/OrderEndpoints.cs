using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Host.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this WebApplication app)
        {
            app.MapPost("/api/orders", async (
                CreateOrderRequest request,
                IOrderService service) =>
            {
                var orderId = await service.CreateOrderAsync(request);
                return Results.Ok(orderId);
            })
            .WithName("CreateOrder")
            .WithTags("Orders");

            app.MapGet("/api/orders/{id:guid}", async (
                Guid id,
                IOrderService service) =>
            {
                var order = await service.GetOrderByIdAsync(id);
                if (order is null)
                {
                    return Results.Problem(
                        title: "Order not found",
                        detail: $"Order with ID '{id}' does not exist.",
                        statusCode: StatusCodes.Status404NotFound);
                }

                return Results.Ok(order);
            })
            .WithName("GetOrderById")
            .WithTags("Orders");

            app.MapGet("/api/orders", async (
                string? status,
                IOrderService service) =>
            {
                var orders = await service.GetAllOrdersAsync(status);
                if (!orders.Any())
                {
                    return Results.Ok(new
                    {
                        message = "No orders found for the given criteria.",
                        data = orders
                    });
                }

                return Results.Ok(new
                {
                    data = orders
                });
            })
            .WithName("GetOrders")
            .WithTags("Orders");

            app.MapPut("/api/orders/{id:guid}/status", async (
                Guid id,
                UpdateOrderStatusRequest request,
                IOrderService service) =>
            {
                var updated = await service.UpdateOrderStatusAsync(id, request.Status);

                if (!updated)
                {
                    return Results.Problem(
                        title: "Invalid status update",
                        detail: "The requested status transition is not allowed.",
                        statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.Ok();
            })
            .WithName("UpdateOrderStatus")
            .WithTags("Orders");

            app.MapDelete("/api/orders/{id:guid}", async (
                Guid id,
                IOrderService service) =>
            {
                var cancelled = await service.CancelOrderAsync(id);

                if (!cancelled)
                {
                    return Results.Problem(
                        title: "Order cannot be cancelled",
                        detail: "Only orders in PENDING status can be cancelled.",
                        statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.Ok();
            })
            .WithName("CancelOrder")
            .WithTags("Orders");
        }
    }
}
