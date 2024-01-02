
namespace Access_Models.ViewModels
{
    public class CatalogVM
    {
        public IEnumerable<IndividualProductVM> IndividualProductVMs { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<ProductAttribute> ProductAttributes { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
        public IEnumerable<AttributeType> AttributeTypes { get; set; }
        public IEnumerable<AttributeValue> AttributeValues { get; set; }
        public Category CurrentCategory { get; set; }
        public string SearchText { get; set; }
    }
}
