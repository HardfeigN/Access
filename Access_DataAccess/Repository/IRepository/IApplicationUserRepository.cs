using Access_Models;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser obj);
    }
}
