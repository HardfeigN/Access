using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj, bool addAllCategory = false)
        {
            if (obj == nameof(Category))
            {
                List<SelectListItem> listItem = new List<SelectListItem>();
                if (addAllCategory)
                {
                    listItem.Add(new SelectListItem
                    {
                        Text = "All categories",
                        Value = DBNull.Value.ToString()
                    });
                }
                listItem.AddRange(_db.Category.Select(i => new SelectListItem
                {
                    Text = i.VisibleName,
                    Value = i.Id.ToString()
                }));
                return listItem;
            }
            if (obj == nameof(Product))
            {
                List<SelectListItem> listItem = new List<SelectListItem>();
                listItem.AddRange(_db.Product.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }));
                return listItem;
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
                };
                List<ProductImage> images = new List<ProductImage>();
                foreach(ProductAttribute attr in IndividualProductVM.ProductAttributes)
                {
                    if (IndividualProductVM.ProductAttributes.Count() > 1)
                        images.AddRange(_db.ProductImage.Where(i => i.AttributeId == attr.Id && i.ImageNumber == 0).ToList());
                    else images.AddRange(_db.ProductImage.Where(i => i.AttributeId == attr.Id).ToList());
                }
                IndividualProductVM.ProductImages = images;
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
