using Access_Models;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IOrderStatusRepository : IRepository<OrderStatus>
    {
        void Update(OrderStatus obj);
    }
}
