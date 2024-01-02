using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string FullAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly DateOfBirthOnly { get; set; }
    }
}
