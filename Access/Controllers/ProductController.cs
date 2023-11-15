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
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IProductRepository prodRepos, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepos = prodRepos;
            _webHostEnvironment = webHostEnvironment;
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
                
                CategorySelectList = _prodRepos.GetAllDropdownList(nameof(Category))
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
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                string upload = webRootPath + WebConstants.ProductImagePath;
                string fileName = Guid.NewGuid().ToString();

                if (productVM.Product.Id == 0)
                {
                    //create
                    /*
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStrem = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStrem);
                    };
                    productVM.Product.Image = fileName + extension;
                    */
                    _prodRepos.Add(productVM.Product);
                }
                else
                {
                    //update
                    var objFromDb = _prodRepos.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking:false);
                    /*
                    if (files.Count > 0)
                    {
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStrem = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStrem);
                        };

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    */
                    _prodRepos.Update(productVM.Product);
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
                return NotFound();
            }
            /*
            string upload = _webHostEnvironment.WebRootPath + WebConstants.ProductImagePath;

            var oldFile = Path.Combine(upload, obj.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            */
            _prodRepos.Remove(obj);
            _prodRepos.Save();
            return RedirectToAction("Index");
        }
    }
}
