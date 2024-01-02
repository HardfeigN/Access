using Access_Models;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IInquiryToOrderRepository : IRepository<InquiryToOrder>
    {
        void Update(InquiryToOrder obj);
    }
}
