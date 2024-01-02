using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Models
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public virtual OrderHeader? OrderHeader { get; set; }
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
