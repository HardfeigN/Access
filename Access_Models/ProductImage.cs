using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int AttributeId { get; set; }
        [ForeignKey("AttributeId")]
        public virtual Attribute Attribute { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Category Level must be equal or greater than 0")]
        public int ImageNumber { get; set; }

    }
}
