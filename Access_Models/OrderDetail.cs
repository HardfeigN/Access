using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public virtual OrderHeader? OrderHeader { get; set; }
        public int ProductAttributeId { get; set; }
        [ForeignKey("ProductAttributeId")]
        public virtual ProductAttribute? ProductAttribute { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
        public double PricePerPiece { get; set; }

        public OrderDetail()
        {
            Quantity = 1;
        }
    }
}
