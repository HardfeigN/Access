using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Access_Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string? ApproveUserId { get; set; }
        [ForeignKey("ApproveUserId")]
        public virtual ApplicationUser? CreatedBy { get; set; }
        public string? CustomerUserId { get; set; }
        [ForeignKey("CustomerUserId")]
        public virtual ApplicationUser? CustomerUser { get; set; }
        [Required]
        public string FullAddress { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public float OrderCost { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<OrderStatus> Statuses { get; set; }
        [NotMapped]
        public string OrderStatusName { get; set; }
        [NotMapped]
        public DateTime CreationDate { get; set; }
        [NotMapped]
        public string ShortOrderDate { get; set; }

        public OrderHeader()
        {
            Statuses = new List<OrderStatus>();
        }
    }
}
