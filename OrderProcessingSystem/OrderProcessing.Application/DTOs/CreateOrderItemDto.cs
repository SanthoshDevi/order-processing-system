using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Application.DTOs
{
    public record CreateOrderItemDto(
     string ProductName,
     int Quantity,
     decimal Price
 );
}
