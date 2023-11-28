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
    public class OrderStatusRepository : Repository<OrderStatus>, IOrderStatusRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderStatusRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderStatus obj)
        {
            _db.OrderStatus.Update(obj);
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj, bool addId)
        {
            if (obj == nameof(OrderStatus))
            {
                List<SelectListItem> listItem = new List<SelectListItem>();
                listItem.AddRange(_db.OrderStatus.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = addId ? i.Id.ToString() : i.Name
                }));
                return listItem;
            }
            return null;
        }
    }
}
