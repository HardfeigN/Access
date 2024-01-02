namespace Access_Models
{
    public class ShoppingCart
    {
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public int Quantity { get; set; }

        public ShoppingCart()
        {
            Quantity = 1;
        }
    }
}
