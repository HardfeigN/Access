namespace Access_Models.ViewModels
{
    public class UserOrderList
    {
        public IEnumerable<OrderHeader> OrderHeader { get; set; }
        public IEnumerable<OrderStatus> OrderStatus { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
        public IEnumerable<ProductImage> ProductImage { get; set; }
        public IEnumerable<Product> Product { get; set; }
        public IEnumerable<ProductAttribute> ProductAttribute { get; set; }
    }
}
