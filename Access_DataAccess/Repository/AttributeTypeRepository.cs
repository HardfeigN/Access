using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
