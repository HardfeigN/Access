
namespace Access_Models.ViewModels
{
    public class InquiryVM 
    {
        public InquiryHeader InquiryHeader { get; set; }
        public IEnumerable<InquiryDetail> InquiryDetail { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
    }
}
