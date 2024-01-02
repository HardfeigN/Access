using Access_Models;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IAttributeTypeRepository : IRepository<AttributeType>
    {
        void Update(AttributeType obj);
    }
}
