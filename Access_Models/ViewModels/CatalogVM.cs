using Microsoft.AspNetCore.Mvc.Rendering;
using Access_Utility;

namespace Access_Models.ViewModels
{
    public class CatalogVM
    {
        public IEnumerable<IndividualProductVM> IndividualProductVMs { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public Category CurrentCategory { get; set; }
        public string SearchText { get; set; }
        public int PageSize { get; set; }
        public CatalogSection CatalogSection { get; set; }
    }
}
