using Access_Models;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IAttributeValueRepository : IRepository<AttributeValue>
    {
        void Update(AttributeValue obj);
    }
}
