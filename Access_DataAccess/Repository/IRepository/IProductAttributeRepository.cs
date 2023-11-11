using Access_Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IProductAttributeRepository : IRepository<ProductAttribute>
    {
        void Update(ProductAttribute obj);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}
