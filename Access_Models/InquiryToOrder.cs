using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Models
{
    public class InquiryToOrder
    {
        [Key]
        public int Id { get; set; }
        public int? OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public virtual OrderHeader? OrderHeader { get; set; }
        public int? InquiryHeaderId { get; set; }
        [ForeignKey("InquiryHeaderId")]
        public virtual InquiryHeader? InquiryHeader { get; set; }
    }
}
