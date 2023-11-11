﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int OrderStatusId { get; set; }
        [ForeignKey("OrderStatusId")]
        public virtual OrderStatus OrderStatus { get; set; }
        public string CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public ApplicationUser CreatedBy { get; set; }
        public int OrderPrice;
        public DateTime CreateDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime ComplationDate { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
