using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IWebHostEnvironment _webHostEnvironment;


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

        //GET - Upsert
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new ProductVM()
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
                return View(productVM);
            }
            else
            {
                productVM.Product = _prodRepos.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                productVM.ProductImages = _prodImgRepos.GetProductImages(productVM.Product.Id);
                return View(productVM);
            }
        }

        //POST - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {

                    _prodRepos.Add(productVM.Product);
                    TempData[WebConstants.Success] = "Pruduct created successfully";
                }
                else
                {
                    //update
                    var objFromDb = _prodRepos.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking:false);
                    _prodRepos.Update(productVM.Product);
                    TempData[WebConstants.Success] = "Pruduct updated successfully";
                }
                _prodRepos.Save();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectList = _prodRepos.GetAllDropdownList(nameof(Category));
            return View(productVM);
        }

        //GET - Delete
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

        //POST - Delete
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
