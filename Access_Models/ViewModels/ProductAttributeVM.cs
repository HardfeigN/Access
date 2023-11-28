using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
