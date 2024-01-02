
namespace Access_Models.ViewModels
{
    public class OrderVM 
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderStatus> OrderStatus { get; set; }
        public IEnumerable<Status> Status { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
    }
}
