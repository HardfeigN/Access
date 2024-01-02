using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_Models.ViewModels
{
    public class StatusVM
    {
        public Status Status { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> StatusSelectList { get; set; }
    }
}
