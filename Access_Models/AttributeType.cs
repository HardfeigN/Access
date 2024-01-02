using System.ComponentModel.DataAnnotations;

namespace Access_Models
{
    public class AttributeType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(7, ErrorMessage = "Attribute Value must not be greater then 7.")]
        public string Name { get; set; }
    }
}
