using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public ProductVM ProductVM { get; set; }

        public ProductController(IProductRepository prodRepos, IWebHostEnvironment webHostEnvironment, IProductImageRepository prodImgRepos)
        {
            _prodRepos = prodRepos;
            _webHostEnvironment = webHostEnvironment;
            _prodImgRepos = prodImgRepos;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRepos.GetAll(includeProperties: $"{nameof(Category)}");

            return View(objList);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM = new ProductVM()
            {
                Product = new Product()
                {
                    Category = new Category()
                },
                
                CategorySelectList = _prodRepos.GetAllDropdownList(nameof(Category)),
                ProductImages = new List<ProductImage>()
            };

            if (id == null)
            {
                //for create
                return View(ProductVM);
            }
            else
            {
                ProductVM.Product = _prodRepos.Find(id.GetValueOrDefault());
                if (ProductVM.Product == null)
                {
                    return NotFound();
                }
                ProductVM.ProductImages = _prodImgRepos.GetProductImages(ProductVM.Product.Id);
                return View(ProductVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (ProductVM.Product.Id == 0)
                {

                    _prodRepos.Add(ProductVM.Product);
                    TempData[WebConstants.Success] = "Pruduct created successfully";
                }
                else
                {
                    //update
                    var objFromDb = _prodRepos.FirstOrDefault(u => u.Id == ProductVM.Product.Id, isTracking:false);
                    _prodRepos.Update(ProductVM.Product);
                    TempData[WebConstants.Success] = "Pruduct updated successfully";
                }
                _prodRepos.Save();
                return RedirectToAction("Index");
            }
            ProductVM.CategorySelectList = _prodRepos.GetAllDropdownList(nameof(Category));
            return View(ProductVM);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product product = _prodRepos.FirstOrDefault(u => u.Id == id, includeProperties: $"{nameof(Category)}");
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WebConstants.Error] = "Error deleting Product";
                return NotFound();
            }

            _prodRepos.Remove(obj);
            _prodRepos.Save();
            TempData[WebConstants.Success] = "Pruduct deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
