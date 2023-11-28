using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    public class AttributeTypeController : Controller
    {
        private readonly IAttributeTypeRepository _attrTypeRepos;

        public AttributeTypeController(IAttributeTypeRepository attrTypeRepos)
        {
            _attrTypeRepos = attrTypeRepos;
        }

        public IActionResult Index()
        {
            ICollection<AttributeType> objList = _attrTypeRepos.GetAll();
            return View(objList);
        }


        //GET - Upsert
        public IActionResult Upsert(int? id)
        {

            if (id == null)
            {
                //for create
                return View(new AttributeType());
            }
            else
            {
                var obj = _attrTypeRepos.Find(id.GetValueOrDefault());
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
        public IActionResult Upsert(AttributeType attributeType)
        {
            if (ModelState.IsValid)
            {
                if (attributeType.Id == 0)
                {
                    //create
                    _attrTypeRepos.Add(attributeType);
                    TempData[WebConstants.Success] = "Attribute Type created successfully";
                }
                else
                {
                    //update
                    _attrTypeRepos.Update(attributeType);
                    TempData[WebConstants.Success] = "Attribute Type updated successfully";
                }
                _attrTypeRepos.Save();
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating or updating Attribute Type";
            return View(attributeType);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _attrTypeRepos.Find(id.GetValueOrDefault());
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
            var obj = _attrTypeRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WebConstants.Error] = "Error while deleting Attribute Type";
                return NotFound();
            }

            _attrTypeRepos.Remove(obj);
            _attrTypeRepos.Save();
            TempData[WebConstants.Success] = "Attribute Type deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
