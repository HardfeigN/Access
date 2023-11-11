using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    public class ProductAttributeController : Controller
    {
        private readonly IProductAttributeRepository _prodAttrRepos;

        public ProductAttributeController(IProductAttributeRepository prodAttrRepos)
        {
            _prodAttrRepos = prodAttrRepos;
        }

        public IActionResult Index()
        {
            ICollection<ProductAttribute> objList = _prodAttrRepos.GetAll(includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)},{nameof(Product)}");
            return View(objList);
        }


        //GET - Upsert
        public IActionResult Upsert(int? id)
        {
            ProductAttributeVM productAttributeVM = new ProductAttributeVM()
            {
                ProductAttribute = new ProductAttribute(),
                AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType)),
                AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue))
            };

            if (id == null)
            {
                //for create
                return View(productAttributeVM);
            }
            else
            {
                productAttributeVM.ProductAttribute = _prodAttrRepos.Find(id.GetValueOrDefault());
                if (productAttributeVM.ProductAttribute == null)
                {
                    return NotFound();
                }
                return View(productAttributeVM);
            }
        }

        //POST - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductAttributeVM productAttributeVM)
        {
            if (ModelState.IsValid)
            {
                if (productAttributeVM.ProductAttribute.Id == 0)
                {
                    //create
                    _prodAttrRepos.Add(productAttributeVM.ProductAttribute);
                }
                else
                {
                    //update
                    var objFromDb = _prodAttrRepos.FirstOrDefault(u => u.Id == productAttributeVM.ProductAttribute.Id, isTracking: false);
                    _prodAttrRepos.Update(productAttributeVM.ProductAttribute);
                }
                _prodAttrRepos.Save();
                return RedirectToAction("Index");
            }
            productAttributeVM.AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType));
            productAttributeVM.AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue));

            if (_prodAttrRepos.Find(productAttributeVM.ProductAttribute.Id) != null)
            {
                productAttributeVM.AttributeTypeSelectList = productAttributeVM.AttributeTypeSelectList.Where(u => u.Value == productAttributeVM.ProductAttribute.Id.ToString());
                productAttributeVM.AttributeValueSelectList = productAttributeVM.AttributeValueSelectList.Where(u => u.Value == productAttributeVM.ProductAttribute.Id.ToString());
            }
            return View(productAttributeVM);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _prodAttrRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            ProductAttributeVM productAttributeVM = new ProductAttributeVM()
            {
                ProductAttribute = obj,
                AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType)),
                AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue))
            };

            return View(productAttributeVM);
        }

        //POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodAttrRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            _prodAttrRepos.Remove(obj);
            _prodAttrRepos.Save();
            return RedirectToAction("Index");
        }
    }
}
