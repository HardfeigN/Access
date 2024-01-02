using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepos;
        [BindProperty]
        public CategoryVM CategoryVM { get; set; }

        public CategoryController(ICategoryRepository catRepos)
        {
            _catRepos = catRepos;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ICollection<Category> objList = _catRepos.GetAll();
            return View(objList);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            CategoryVM = new CategoryVM()
            {
                Category = new Category(),
                CategorySelectList = _catRepos.GetAllDropdownList(nameof(Category))
            };

            if (id == null)
            {
                //for create
                return View(CategoryVM);
            }
            else
            {

                CategoryVM.Category = _catRepos.Find(id.GetValueOrDefault());
                if (CategoryVM.Category == null)
                {
                    return NotFound();
                }
                CategoryVM.CategorySelectList = CategoryVM.CategorySelectList.Where(u => u.Value != CategoryVM.Category.Id.ToString());
                return View(CategoryVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (CategoryVM.Category.Id == 0)
                {
                    //create
                    _catRepos.Add(CategoryVM.Category);
                    TempData[WebConstants.Success] = "Category created successfully";
                }
                else
                {
                    //update
                    _catRepos.Update(CategoryVM.Category);
                    TempData[WebConstants.Success] = "Category updated successfully";
                }
                _catRepos.Save();
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating or updating category";

            CategoryVM.CategorySelectList = _catRepos.GetAllDropdownList(nameof(Category));
            
            if (_catRepos.Find(CategoryVM.Category.Id) != null)
            {
                CategoryVM.CategorySelectList = CategoryVM.CategorySelectList.Where(u => u.Value != CategoryVM.Category.Id.ToString());
            }
            return View(CategoryVM);
        }

        [HttpGet]
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

            CategoryVM = new CategoryVM()
            {
                Category = obj,
                CategorySelectList = _catRepos.GetAllDropdownList(nameof(Category))
            };
            return View(CategoryVM);
        }

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
