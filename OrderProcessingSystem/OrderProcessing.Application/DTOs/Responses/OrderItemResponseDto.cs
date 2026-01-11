using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Application.DTOs.Responses
{
    public record OrderItemResponseDto(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal Price
);
}
