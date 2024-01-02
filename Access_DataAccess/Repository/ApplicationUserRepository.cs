using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Microsoft.EntityFrameworkCore;

namespace Access_DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUser obj)
        {
            var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id, isTracking: false);
            if (objFromDb != null)
            {
                objFromDb.FullAddress = obj.FullAddress;
                objFromDb.FullName = obj.FullName;
                objFromDb.Email = obj.Email;
                objFromDb.PhoneNumber = obj.PhoneNumber;
                objFromDb.DateOfBirth = obj.DateOfBirth;
                _db.Entry(objFromDb).State = EntityState.Modified;
                _db.ApplicationUser.Update(objFromDb);
            }
        }
    }
}
