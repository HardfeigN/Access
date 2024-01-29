using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == nameof(Category))
            {
                List<SelectListItem> listItem = new List<SelectListItem>
                {
                    new SelectListItem()
                    {
                        Text = "No parent",
                        Value = DBNull.Value.ToString()
                    }
                };
                listItem.AddRange(_db.Category.Select(i => new SelectListItem
                {
                    Text = i.VisibleName,
                    Value = i.Id.ToString()
                }));
                return listItem;
            }
            return null;
        }

        public void Update(Category obj)
        {
            _db.Category.Update(obj);
        }
    }
}
