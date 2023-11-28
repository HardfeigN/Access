using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IApplicationUserRepository _userRepos;
        private readonly IProductRepository _prodRepos;
        private readonly ICategoryRepository _catRepos;
        private readonly IProductAttributeRepository _prodAttrRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IAttributeTypeRepository _attrTypeRepos;
        private readonly IAttributeValueRepository _attrValRepos;

        public CatalogVM CatalogVM { get; set; }

        public CatalogController(IWebHostEnvironment webHostEnvironment, IApplicationUserRepository userRepos, IProductRepository prodRepos, ICategoryRepository catRepos,
            IProductAttributeRepository prodAttrRepos, IProductImageRepository prodImgRepos, IAttributeTypeRepository attrTypeRepos, IAttributeValueRepository attrValRepos, ILogger<CatalogController> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _userRepos = userRepos;
            _prodRepos = prodRepos;
            _catRepos = catRepos;
            _prodAttrRepos = prodAttrRepos;
            _attrTypeRepos = attrTypeRepos;
            _attrValRepos = attrValRepos;
            _prodImgRepos = prodImgRepos;
            _logger = logger;
        }

        public IActionResult Index()
        {
            CatalogVM = new CatalogVM()
            {
                Products = _prodRepos.GetAll(includeProperties: $"{nameof(Category)}"),
                Categories = _catRepos.GetAll(),
                ProductAttributes = _prodAttrRepos.GetAll(includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"),
                ProductImages = _prodImgRepos.GetAll(),
                AttributeTypes = _attrTypeRepos.GetAll(),
                AttributeValues = _attrValRepos.GetAll(),
                IndividualProductVMs = new List<IndividualProductVM>()
            };
            List<IndividualProductVM> list = new List<IndividualProductVM>();
            foreach (var product in CatalogVM.Products)
            {
                IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                list.Add(prodVM);
            }
            CatalogVM.IndividualProductVMs = list;
            return View(CatalogVM);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            IndividualProductVM individualProductVM = new IndividualProductVM
            {
                Product = _prodRepos.Find(id),
                ProductImages = _prodImgRepos.GetProductImages(id),
                ProductAttributes = _prodAttrRepos.GetAllProductAttributes(id, true),
                ExistInCart = false
            };

            if(individualProductVM.Product == null) 
            {
                return NotFound();
            }

            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id)
                {
                    individualProductVM.ExistInCart = true;
                }
            }

            return View(individualProductVM);
        }

        //POST - Details
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id, Quantity = 1 });
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            TempData[WebConstants.Success] = "Pruduct removed from your cart successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
