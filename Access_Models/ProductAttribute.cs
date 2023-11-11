using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_Models
{
    public class ProductAttribute
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AttributeValueId { get; set; }
        [ForeignKey("AttributeValueId")]
        public virtual AttributeValue AttributeValue { get; set; }
        [Required]
        public int AttributeTypeId { get; set; }
        [ForeignKey("AttributeTypeId")]
        public virtual AttributeType AttributeType { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public bool IsInStock { get; set; }
    }
}
