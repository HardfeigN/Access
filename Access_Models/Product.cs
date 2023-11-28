using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public float Price { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductAttribute> Attribute { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }
        [NotMapped]
        [Range(1, 1000, ErrorMessage = "Quantity must be greater than 0.")]
        public int TempQuantity { get; set; }

        public Product()
        {
            Attribute = new List<ProductAttribute>();
            ProductImage = new List<ProductImage>();
            TempQuantity = 1;
        }

    }
}
