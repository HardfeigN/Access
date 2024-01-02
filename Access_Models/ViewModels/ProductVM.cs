using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
        [ValidateNever]
        public IEnumerable<ProductImage> ProductImages { get; set; }
    }
}
