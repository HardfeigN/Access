using Access_Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}
