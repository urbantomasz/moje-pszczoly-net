using Microsoft.OpenApi.Writers;

namespace MojePszczoly.Models
{
    public class OrdersDataDto
    {
        public List<DateTime> Dates { get; set; }
        public List<OrderDto> Orders { get; set; }
    }
}
