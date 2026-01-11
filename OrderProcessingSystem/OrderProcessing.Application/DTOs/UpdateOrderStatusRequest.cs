using OrderProcessing.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Application.DTOs
{
    public record UpdateOrderStatusRequest(
    OrderStatus Status
);
}
