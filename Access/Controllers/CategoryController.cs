using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepos;

        public CategoryController(ICategoryRepository catRepos)
        {
            _catRepos = catRepos;
        }

        public IActionResult Index()
        {
            ICollection<Category> objList = _catRepos.GetAll();
            return View(objList);
        }

        //GET - Upsert
        public IActionResult Upsert(int? id)
        {

            CategoryVM categoryVM = new CategoryVM()
            {
                Category = new Category(),
                CategorySelectList = _catRepos.GetAllDropdownList(WebConstants.CategoryName)
            };

            if (id == null)
            {
                //for create
                return View(categoryVM);
            }
            else
            {

                categoryVM.Category = _catRepos.Find(id.GetValueOrDefault());
                if (categoryVM.Category == null)
                {
                    return NotFound();
                }
                categoryVM.CategorySelectList = categoryVM.CategorySelectList.Where(u => u.Value != categoryVM.Category.Id.ToString());
                return View(categoryVM);
            }
        }

        //POST - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CategoryVM categoryVM)
        {
            if (ModelState.IsValid)
            {
                if (categoryVM.Category.Id == 0)
                {
                    //create
                    _catRepos.Add(categoryVM.Category);
                }
                else
                {
                    //update
                    var objFromDb = _catRepos.FirstOrDefault(u => u.Id == categoryVM.Category.Id, isTracking: false);
                    _catRepos.Update(categoryVM.Category);
                }
                _catRepos.Save();
                return RedirectToAction("Index");
            }
            categoryVM.CategorySelectList = _catRepos.GetAllDropdownList(WebConstants.CategoryName);
            
            if (_catRepos.Find(categoryVM.Category.Id) != null)
            {
                categoryVM.CategorySelectList = categoryVM.CategorySelectList.Where(u => u.Value != categoryVM.Category.Id.ToString());
            }
            return View(categoryVM);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _catRepos.Find(id.GetValueOrDefault());
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
            var obj = _catRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            _catRepos.Remove(obj);
            _catRepos.Save();
            return RedirectToAction("Index");
        }
    }
}
