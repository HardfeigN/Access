using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_Models.ViewModels
{
    public class ProductAttributeVM
    {
        public ProductAttribute ProductAttribute { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> AttributeTypeSelectList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> AttributeValueSelectList { get; set; }
        [ValidateNever]
        public IEnumerable<ProductImage> ProductImages { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ProductSelectList { get; set; }
        [ValidateNever]
        public bool DeleteOldImages { get; set; }

        public ProductAttributeVM() 
        {
            DeleteOldImages = false;
        }
    }
}
