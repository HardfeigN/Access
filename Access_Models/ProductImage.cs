using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? AttributeId { get; set; }
        public virtual ProductAttribute? Attribute { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Image number must be equal or greater than 0")]
        public int ImageNumber { get; set; }
    }
}
