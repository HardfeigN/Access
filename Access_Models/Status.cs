using System.ComponentModel.DataAnnotations;

namespace Access_Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Status must not be greater then 30.")]
        public string Name { get; set; }
        [StringLength(50, ErrorMessage = "Visible name not be greater then 50.")]
        public string? VisibleName { get; set; }
        [Range(0, 30, ErrorMessage = "Status Number must be between 0 and 30.")]
        public int? ParentId { get; set; }
    }
}
