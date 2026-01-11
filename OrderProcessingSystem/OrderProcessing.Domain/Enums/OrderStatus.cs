
using System.Text.Json.Serialization;

namespace OrderProcessing.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4
    }
}
