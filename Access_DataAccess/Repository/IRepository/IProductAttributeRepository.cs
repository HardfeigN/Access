using Access_Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IProductAttributeRepository : IRepository<ProductAttribute>
    {
        void Update(ProductAttribute obj);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}
