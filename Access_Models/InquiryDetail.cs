using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Models
{
    public class InquiryDetail
    {
        [Key]
        public int Id { get; set; }
        public int InquiryHeaderId { get; set; }
        [ForeignKey("InquiryHeaderId")]
        public virtual InquiryHeader? InquiryHeader { get; set; }
        public int ProductAttributeId { get; set; }
        [ForeignKey("ProductAttributeId")]
        public virtual ProductAttribute? ProductAttribute { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public InquiryDetail()
        {
            Quantity = 1;
        }
    }
}
