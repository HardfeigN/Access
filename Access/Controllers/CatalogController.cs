using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

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
        private const int _pageSize = 10;

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

        [HttpGet]
        public IActionResult Index(int? id = null, int? page = null, string? searchText = null, CatalogSection ? section = CatalogSection.Index)
        {
            if(page != null)
            {
                if ((section != CatalogSection.Search) || (section == CatalogSection.Search && searchText != null))
                {
                    return PartialView("_CatalogItemsBlock", GetItemsPage(id: id, page: page, searchText: (section == CatalogSection.Search)? searchText : null, 
                        byDecs: (section == CatalogSection.NewArrivals)));                    
                }
                return PartialView("_CatalogItemsBlock", null);
            }

            CatalogVM = new CatalogVM()
            {
                CategoryList = _prodRepos.GetAllDropdownList(nameof(Category), addAllCategory: true),
                CurrentCategory = (id == null)? null : _catRepos.Find(id.GetValueOrDefault()),
                CatalogSection = section.GetValueOrDefault(),
                PageSize = _pageSize
            };

            if (section == CatalogSection.Search && searchText != null)
            {
                CatalogVM.SearchText = searchText;
            }
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
                ProductAttributes = _prodAttrRepos.GetAll(u => u.ProductId == id, includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"),
                ExistInCart = false
            };

            if(individualProductVM.Product == null) 
            {
                return NotFound();
            }

            foreach(ProductAttribute attr in individualProductVM.ProductAttributes.Where(u => u.AttributeType.Name == WebConstants.Color))
            {
                if (shoppingCartList.Where(u => u.ProductAttributeId == attr.Id).Count() > 0) attr.ExistInCart = true;
            }

            List<Product> prodsIdCat = _prodRepos.GetAll(u => u.CategoryId == individualProductVM.Product.CategoryId && u.Id != id).Take(4).ToList();
            List<IndividualProductVM> individualProduct = new List<IndividualProductVM>();
            List<ProductAttribute> productAttributes = new List<ProductAttribute>();
            List<ProductImage> productImages = new List<ProductImage>();

            foreach (var product in prodsIdCat)
            {
                productAttributes.AddRange(_prodAttrRepos.GetAll(u => u.ProductId == product.Id, includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"));
                productImages.AddRange(_prodImgRepos.GetAll(u => u.ProductId == product.Id));

                IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                individualProduct.Add(prodVM);
            }
            individualProductVM.IndividualProductVMs = individualProduct;

            return View(individualProductVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, int prodColor)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id, Quantity = 1, ProductAttributeId = prodColor });
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id, int prodColor)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id && r.ProductAttributeId == prodColor);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            TempData[WebConstants.Success] = "Pruduct removed from your cart successfully";
            return RedirectToAction(nameof(Index));
        }

        private List<IndividualProductVM> GetItemsPage(int? id = null, int? page = 0, string? searchText = null, bool? byDecs = false)
        {
            int itemsToSkip = page.GetValueOrDefault() * _pageSize;
            List<Product> products = new List<Product>();
            Expression<Func<Product, bool>> filter = null;
            if(searchText != null)
            {
                if (id != null)
                    filter = u => (u.Name.Contains(searchText) || u.Description.Contains(searchText) || u.ShortDesc.Contains(searchText)) && u.CategoryId == id;
                else filter = u => (u.Name.Contains(searchText) || u.Description.Contains(searchText) || u.ShortDesc.Contains(searchText));
            }
            else
            {
                if (id != null) filter = u => u.CategoryId == id;
            }

            if (!byDecs.GetValueOrDefault()) products = _prodRepos.GetAll(filter).OrderBy(t => t.Id).Skip(itemsToSkip).Take(_pageSize).ToList();
            else products = _prodRepos.GetAll(filter).OrderByDescending(t => t.Id).Skip(itemsToSkip).Take(_pageSize).ToList();

            List<IndividualProductVM> individualProductVMs = new List<IndividualProductVM>();

            foreach (Product prod in products)
            {
                individualProductVMs.Add(_prodRepos.GetIndividualProductVM(prod.Id));
            }
            return individualProductVMs;
        }

        #region API CALLS

        [HttpGet, Route("/Catalog/IsInCart/{attrId?}")]
        public IActionResult IsInCart(int? attrId = null)
        {
            if ( attrId == null) return Json(new { data = false });
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            
            bool isInCart = false;
            foreach(var item in shoppingCartList)
            {
                if (item.ProductAttributeId == attrId) isInCart = true;
            }

            return Json(new { data = isInCart });
        }

        #endregion
    }
}
