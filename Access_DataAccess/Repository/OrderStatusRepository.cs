using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;

namespace Access_DataAccess.Repository
{
    public class OrderStatusRepository : Repository<OrderStatus>, IOrderStatusRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderStatusRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderStatus obj)
        {
            _db.OrderStatus.Update(obj);
        }
    }
}
