
namespace Access_Models.ViewModels
{
    public class IndividualProductVM
    {
        public Product Product { get; set; }
        public int SliderImageIndex { get; set; }
        public bool ExistInCart { get; set; }
        public IEnumerable<ProductAttribute> ProductAttributes { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
        public IEnumerable<IndividualProductVM> IndividualProductVMs { get; set; }

        public IndividualProductVM()
        {
            SliderImageIndex = 0;
            ExistInCart = false;
        }
    }
}
