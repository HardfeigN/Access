using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public int OrderStatusId { get; set; }
        [ForeignKey("OrderStatusId")]
        public virtual OrderStatus OrderStatus { get; set; }
        public string CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedBy { get; set; }
        public float OrderPrice;
        public DateTime CreateDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime ComplationDate { get; set; }
    }
}
