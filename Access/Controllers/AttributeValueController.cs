using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    public class AttributeValueController : Controller
    {
        private readonly IAttributeValueRepository _attrRepos;

        public AttributeValueController(IAttributeValueRepository attrRepos)
        {
            _attrRepos = attrRepos;
        }

        public IActionResult Index()
        {
            ICollection<AttributeValue> objList = _attrRepos.GetAll();
            return View(objList);
        }


        //GET - Upsert
        public IActionResult Upsert(int? id)
        {

            if (id == null)
            {
                //for create
                return View(new AttributeValue());
            }
            else
            {
                var obj = _attrRepos.Find(id.GetValueOrDefault());
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
        public IActionResult Upsert(AttributeValue attributeValue)
        {
            if (ModelState.IsValid)
            {
                if (attributeValue.Id == 0)
                {
                    //create
                    _attrRepos.Add(attributeValue);
                }
                else
                {
                    //update
                    var objFromDb = _attrRepos.FirstOrDefault(u => u.Id == attributeValue.Id, isTracking: false);
                    _attrRepos.Update(attributeValue);
                }
                _attrRepos.Save();
                return RedirectToAction("Index");
            }
            return View(attributeValue);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _attrRepos.Find(id.GetValueOrDefault());
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
            var obj = _attrRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            _attrRepos.Remove(obj);
            _attrRepos.Save();
            return RedirectToAction("Index");
        }

    }
}
