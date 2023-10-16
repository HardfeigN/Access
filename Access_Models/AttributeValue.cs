using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_Models
{
    public class AttributeValue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(7, ErrorMessage = "Attribute Value must not be greater then 7.")]
        public string Value { get; set; }
    }
}
