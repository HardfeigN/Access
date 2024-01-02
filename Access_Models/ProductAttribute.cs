using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Models
{
    public class ProductAttribute
    {
        [Key]
        public int Id { get; set; }
        public int AttributeValueId { get; set; }
        [ForeignKey("AttributeValueId")]
        public virtual AttributeValue? AttributeValue { get; set; }
        public int AttributeTypeId { get; set; }
        [ForeignKey("AttributeTypeId")]
        public virtual AttributeType? AttributeType { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        public bool IsInStock { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; }
        [NotMapped]
        public bool ExistInCart { get; set; }
        [NotMapped]
        [Range(1, 1000, ErrorMessage = "Quantity must be greater than 0.")]
        public int TempQuantity { get; set; }

        public ProductAttribute()
        {
            ProductImage = new List<ProductImage>();
            ExistInCart = false;
            TempQuantity = 1;
        }
    }
}
