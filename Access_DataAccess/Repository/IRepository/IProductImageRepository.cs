using Access_Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        void Update(ProductImage obj);

        IEnumerable<ProductImage> GetProductImages(int id);
        IEnumerable<ProductImage> GetProductAttributeImages(int id);
    }
}
