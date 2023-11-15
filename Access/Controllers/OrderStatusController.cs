using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    public class OrderStatusController : Controller
    {
        private readonly IOrderStatusRepository _ordStRepos;

        public OrderStatusController(IOrderStatusRepository ordStRepos)
        {
            _ordStRepos = ordStRepos;
        }

        public IActionResult Index()
        {
            ICollection<OrderStatus> objList = _ordStRepos.GetAll();
            return View(objList);
        }


        //GET - Upsert
        public IActionResult Upsert(int? id)
        {

            if (id == null)
            {
                //for create
                return View(new OrderStatus());
            }
            else
            {
                var obj = _ordStRepos.Find(id.GetValueOrDefault());
                if (obj == null)
                {
                    return NotFound();
                }
                return View(obj);
            }
        }

        //POST - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(OrderStatus orderStatus)
        {
            if (ModelState.IsValid)
            {
                if (orderStatus.Id == 0)
                {
                    //create
                    _ordStRepos.Add(orderStatus);
                }
                else
                {
                    //update
                    _ordStRepos.Update(orderStatus);
                }
                _ordStRepos.Save();
                return RedirectToAction("Index");
            }
            return View(orderStatus);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _ordStRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _ordStRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            _ordStRepos.Remove(obj);
            _ordStRepos.Save();
            return RedirectToAction("Index");
        }

    }
}
