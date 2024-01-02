using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Access_Models.ViewModels
{
    public class ProductUserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]
        public IList<Product> ProductList { get; set; }
        [ValidateNever]
        public IList<ProductAttribute> ProductAttributeList { get; set; }
        [ValidateNever]
        public IList<ProductImage> ProductImageList { get; set; }
    }
}
