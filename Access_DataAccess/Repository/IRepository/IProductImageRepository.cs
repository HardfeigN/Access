using Access_Models;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        void Update(ProductImage obj);
        IEnumerable<ProductImage> GetProductImages(int id);
        IEnumerable<ProductImage> GetProductAttributeImages(int id);
    }
}
