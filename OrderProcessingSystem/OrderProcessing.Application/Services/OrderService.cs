using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderProcessing.Application.Configuration;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.DTOs.Responses;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Enums;
using OrderProcessing.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly OrderRulesOptions _rules;

        public OrderService(
        OrderDbContext context,
        IOptions<OrderRulesOptions> rules)
        {
            _context = context;
            _rules = rules.Value;
        }

        public async Task<Guid> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            if (request.Items.Count > _rules.MaxItemsPerOrder)
            {
                throw new ArgumentException(
                    $"An order cannot contain more than {_rules.MaxItemsPerOrder} items.");
            }

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0 || item.Quantity > _rules.MaxItemQuantity)
                {
                    throw new ArgumentException(
                        $"Item quantity must be between 1 and {_rules.MaxItemQuantity}.");
                }

                if (item.Price <= 0)
                {
                    throw new ArgumentException("Item price must be greater than zero.");
                }
            }

            var order = new Order
            {
                Status = OrderStatus.Pending,
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order.Id;
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Id == orderId)
                .Select(o => new OrderResponseDto(
                    o.Id,
                    o.CreatedAt,
                    o.Status,
                    o.Items.Select(i => new OrderItemResponseDto(
                        i.Id,
                        i.ProductName,
                        i.Quantity,
                        i.Price
                    )).ToList()
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string? status)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<OrderStatus>(status, true, out var parsed))
            {
                query = query.Where(o => o.Status == parsed);
            }

            return await query
                .Select(o => new OrderResponseDto(
                    o.Id,
                    o.CreatedAt,
                    o.Status,
                    o.Items.Select(i => new OrderItemResponseDto(
                        i.Id,
                        i.ProductName,
                        i.Quantity,
                        i.Price
                    )).ToList()
                ))
                .ToListAsync();
        }

        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending)
                return false;

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return false;

            // Cannot update cancelled or delivered orders
            if (order.Status == OrderStatus.Cancelled ||
                order.Status == OrderStatus.Delivered)
                return false;

            // Allowed transitions
            var valid = order.Status switch
            {
                OrderStatus.Pending => newStatus == OrderStatus.Processing,
                OrderStatus.Processing => newStatus == OrderStatus.Shipped,
                OrderStatus.Shipped => newStatus == OrderStatus.Delivered,
                _ => false
            };

            if (!valid)
                return false;

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
