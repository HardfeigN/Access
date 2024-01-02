using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;

namespace Access_DataAccess.Repository
{
    public class AttributeValueRepository : Repository<AttributeValue>, IAttributeValueRepository
    {
        private readonly ApplicationDbContext _db;

        public AttributeValueRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AttributeValue obj)
        {
            _db.AttributeValue.Update(obj);
        }
    }
}
