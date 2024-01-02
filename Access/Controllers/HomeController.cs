using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IProductRepository _prodRepos;
        private readonly ICategoryRepository _catRepos;
        
        public HomeController(IProductImageRepository prodImgRepos, IProductRepository prodRepos, ICategoryRepository catRepos)
        {
            _prodImgRepos = prodImgRepos;
            _prodRepos = prodRepos;
            _catRepos = catRepos;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _catRepos.GetAll();
            List<ProductImage> images = new List<ProductImage>();
            foreach(Category category in categories)
            {
                Product product = _prodRepos.FirstOrDefault(u => u.CategoryId == category.Id);
                if (product != null) images.Add(_prodImgRepos.FirstOrDefault(u => u.ProductId == product.Id));
            }

            return View(images);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
    }
}
