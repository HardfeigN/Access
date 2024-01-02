using Access_Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IStatusRepository : IRepository<Status>
    {
        void Update(Status obj);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj, bool addId, bool addNoParent);
        IEnumerable<Status> GetNextStatus(int currentId);
    }
}
