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
    public class AttributeValueRepository : Repository<AttributeValue>, IAttributeValueRepository
    {
        private readonly ApplicationDbContext _db;

        public AttributeValueRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AttributeValue obj)
        {
            var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Value = obj.Value;
            }
        }
    }
}
