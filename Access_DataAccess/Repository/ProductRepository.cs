using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == nameof(Category))
            {
                List<SelectListItem> listItem = new List<SelectListItem>();
                listItem.AddRange(_db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }));
                return listItem;
            }
            return null;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllDropdownListAsync(string obj)
        {
            if (obj == nameof(Category))
            {
                List<SelectListItem> listItem = new List<SelectListItem>();
                listItem.AddRange(await _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }).ToListAsync());
                return listItem;
            }
            return null;
        }

        public async Task<IndividualProductVM> GetIndividualProductVMAsync(int id)
        {
            try
            {
                IndividualProductVM IndividualProductVM = new IndividualProductVM()
                {
                    Product = await _db.Product.FindAsync(id),
                    ProductAttributes = await _db.ProductAttribute.Where(i => i.ProductId == id).ToListAsync(),
                    ProductImages = await _db.ProductImage.Where(i => i.ProductId == id).ToListAsync()
                };
                return IndividualProductVM;
            }
            catch
            {

            }

            return null;
        }

        public IndividualProductVM GetIndividualProductVM(int id)
        {
            try
            {
                IndividualProductVM IndividualProductVM = new IndividualProductVM()
                {
                    Product = _db.Product.Find(id),
                    ProductAttributes = _db.ProductAttribute.Where(i => i.ProductId == id).ToList(),
                    ProductImages = _db.ProductImage.Where(i => i.ProductId == id).ToList()
                };
                return IndividualProductVM;
            }
            catch
            {

            }

            return new IndividualProductVM();
        }

        public void Update(Product obj)
        {
            _db.Product.Update(obj);
        }
    }
}
