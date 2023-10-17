using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Microsoft.AspNetCore.Mvc;

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

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepos.Add(obj);
                _catRepos.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET - Edit
        public IActionResult Edit(int? id)
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

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepos.Update(obj);
                _catRepos.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
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
