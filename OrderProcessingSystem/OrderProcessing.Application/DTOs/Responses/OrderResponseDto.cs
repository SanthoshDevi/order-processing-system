using OrderProcessing.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Application.DTOs.Responses
{
    public record OrderResponseDto(
    Guid Id,
    DateTime CreatedAt,
    OrderStatus Status,
    List<OrderItemResponseDto> Items
);
}
