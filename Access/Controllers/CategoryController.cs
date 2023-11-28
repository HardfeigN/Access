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

        public async Task<IActionResult> Index()
        {
            ICollection<Category> objList = await _catRepos.GetAllAsync();
            return View(objList);
        }

        //GET - Upsert
        public IActionResult Upsert(int? id)
        {

            CategoryVM categoryVM = new CategoryVM()
            {
                Category = new Category(),
                CategorySelectList = _catRepos.GetAllDropdownList(nameof(Category))
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
                    TempData[WebConstants.Success] = "Category created successfully";
                }
                else
                {
                    //update
                    _catRepos.Update(categoryVM.Category);
                    TempData[WebConstants.Success] = "Category updated successfully";
                }
                _catRepos.Save();
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating or updating category";

            categoryVM.CategorySelectList = _catRepos.GetAllDropdownList(nameof(Category));
            
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

            CategoryVM categoryVM = new CategoryVM()
            {
                Category = obj,
                CategorySelectList = _catRepos.GetAllDropdownList(nameof(Category))
            };
            return View(categoryVM);
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
                TempData[WebConstants.Error] = "Error while deketing category";
                return NotFound();
            }

            _catRepos.Remove(obj);
            _catRepos.Save();
            TempData[WebConstants.Success] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
