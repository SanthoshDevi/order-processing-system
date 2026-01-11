using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.DTOs.Responses;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string? status);
        Task<bool> CancelOrderAsync(Guid orderId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);

    }
}
