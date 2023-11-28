using Access_Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
        Task<IEnumerable<SelectListItem>> GetAllDropdownListAsync(string obj);
    }
}
