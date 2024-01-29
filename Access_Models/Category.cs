using System.ComponentModel.DataAnnotations;

namespace Access_Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Attribute Value must not be greater then 30.")]
        public string Name { get; set; }
        [StringLength(50, ErrorMessage = "Visible name must not be greater then 50.")]
        public string? VisibleName { get; set; }
        public int? ParentId { get; set; }
    }
}
