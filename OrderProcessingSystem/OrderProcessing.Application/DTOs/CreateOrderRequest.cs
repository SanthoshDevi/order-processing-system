using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Application.DTOs
{
    public record CreateOrderRequest(
    List<CreateOrderItemDto> Items
);
}
