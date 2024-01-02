using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_DataAccess.Repository
{
    public class StatusRepository : Repository<Status>, IStatusRepository
    {
        private readonly ApplicationDbContext _db;

        public StatusRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Status obj)
        {
            _db.Status.Update(obj);
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj, bool addId, bool addNoParent)
        {
            if (obj == nameof(Status))
            {
                List<SelectListItem> listItem = new List<SelectListItem>();
                if (addNoParent)
                {
                    listItem = new List<SelectListItem>
                    {
                        new SelectListItem()
                        {
                            Text = "No parent",
                            Value = DBNull.Value.ToString()
                        }
                    };
                }

                listItem.AddRange(_db.Status.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = addId ? i.Id.ToString() : i.Name
                }));
                return listItem;
            }
            return null;
        }

        public IEnumerable<Status> GetNextStatus(int currentId)
        {
            Status current = _db.Status.FirstOrDefault(u => u.Id == currentId);
            List<Status> next = ( current == null ) ? new List<Status>() : _db.Status.Where(u => u.ParentId == current.Id).ToList();
            return next;
        }
    }
}
