using Access_Models;
using Access_Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj, bool addAllCategory = false);
        IndividualProductVM GetIndividualProductVM(int id);
    }
}
