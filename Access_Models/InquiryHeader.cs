using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Models
{
    public class InquiryHeader
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public DateTime InquiryDate { get; set; }
        [Required]
        public string PhoneNumber { get; set;}
        [Required]
        public string FullName { get; set;}
        [Required]
        public string Email { get; set;}
        [Required]
        public string FullAddress { get; set;}
        [NotMapped]
        public int? OrderHeaderId { get; set; }
        [NotMapped]
        public string ShortInquiryDate { get; set; }
    }
}
