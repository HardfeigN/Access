using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class AttributeValueController : Controller
    {
        private readonly IAttributeValueRepository _attrValueRepos;

        public AttributeValueController(IAttributeValueRepository attrValueRepos)
        {
            _attrValueRepos = attrValueRepos;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ICollection<AttributeValue> objList = _attrValueRepos.GetAll();
            return View(objList);
        }


        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            if (id == null)
            {
                //for create
                return View(new AttributeValue());
            }
            else
            {
                var obj = _attrValueRepos.Find(id.GetValueOrDefault());
                if (obj == null)
                {
                    return NotFound();
                }
                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(AttributeValue attributeValue)
        {
            if (ModelState.IsValid)
            {
                if (attributeValue.Id == 0)
                {
                    //create
                    _attrValueRepos.Add(attributeValue);
                    TempData[WebConstants.Success] = "Attribute Value created successfully";
                }
                else
                {
                    //update
                    _attrValueRepos.Update(attributeValue);
                    TempData[WebConstants.Success] = "Attribute Value updated successfully";
                }
                _attrValueRepos.Save();
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating or updating Attribute Value";
            return View(attributeValue);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _attrValueRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _attrValueRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WebConstants.Error] = "Error while deleting Attribute Value";
                return NotFound();
            }

            _attrValueRepos.Remove(obj);
            _attrValueRepos.Save();
            TempData[WebConstants.Success] = "Attribute Type deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
