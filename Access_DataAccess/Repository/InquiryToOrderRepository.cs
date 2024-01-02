using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;

namespace Access_DataAccess.Repository
{
    public class InquiryToOrderRepository : Repository<InquiryToOrder>, IInquiryToOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryToOrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InquiryToOrder obj)
        {
            _db.InquiryToOrder.Update(obj);
        }
    }
}
