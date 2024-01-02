using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;

namespace Access_DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<ProductImage> GetProductAttributeImages(int id)
        {
            return _db.ProductImage.Where(x => x.AttributeId == id);
        }

        public IEnumerable<ProductImage> GetProductImages(int id)
        {
            return _db.ProductImage.Where(x => x.ProductId == id);
        }

        public void Update(ProductImage obj)
        {
            _db.ProductImage.Update(obj);
        }
    }
}
