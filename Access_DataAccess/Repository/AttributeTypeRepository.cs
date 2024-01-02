using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;

namespace Access_DataAccess.Repository
{
    public class AttributeTypeRepository : Repository<AttributeType>, IAttributeTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public AttributeTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AttributeType obj)
        {
            _db.AttributeType.Update(obj);
        }
    }
}
