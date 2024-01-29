using System.ComponentModel.DataAnnotations;

namespace Access_Models
{
    public class AttributeValue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Attribute Value must not be greater then 7.")]
        public string Value { get; set; }
        [StringLength(50, ErrorMessage = "Visible name must not be greater then 50.")]
        public string? VisibleName { get; set; }
    }
}
