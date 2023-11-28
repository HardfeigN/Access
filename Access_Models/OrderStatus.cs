using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Access_Models
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(25, ErrorMessage = "Order Status must not be greater then 25.")]
        public string Name { get; set; }
        [Range(0, 30, ErrorMessage = "Order Status Number must be between 0 and 30.")]
        public int StatusNumber { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<OrderHeader> OrderHeader { get; set; }
        public OrderStatus()
        {
            OrderHeader = new List<OrderHeader>();
            StatusNumber = 0;
        }
    }
}
