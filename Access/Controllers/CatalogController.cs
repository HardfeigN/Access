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
        public IActionResult Index(int? id, int? page = null)
        {
            if(id == null)
            {

                if (page != null)
                {
                    return PartialView("_CatalogItemsBlock", GetItemsPage(page.GetValueOrDefault()));
                }

                CatalogVM = new CatalogVM()
                {
                    Products = _prodRepos.GetAll(includeProperties: $"{nameof(Category)}"),
                    Categories = _catRepos.GetAll(),
                    ProductAttributes = _prodAttrRepos.GetAll(includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"),
                    ProductImages = _prodImgRepos.GetAll(),
                    AttributeTypes = _attrTypeRepos.GetAll(),
                    AttributeValues = _attrValRepos.GetAll(),
                    IndividualProductVMs = new List<IndividualProductVM>(),
                    CurrentCategory = null,
                };
                List<IndividualProductVM> list = new List<IndividualProductVM>();
                foreach (var product in CatalogVM.Products)
                {
                    IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                    list.Add(prodVM);
                }
                //CatalogVM.IndividualProductVMs = list;
                CatalogVM.IndividualProductVMs = GetItemsPage(0);
            }
            else
            {
                CatalogVM = new CatalogVM()
                {
                    Products = _prodRepos.GetAll(u => u.CategoryId == id, includeProperties: $"{nameof(Category)}"),
                    Categories = _catRepos.GetAll(),
                    AttributeTypes = _attrTypeRepos.GetAll(),
                    AttributeValues = _attrValRepos.GetAll(),
                    IndividualProductVMs = new List<IndividualProductVM>(),
                    CurrentCategory = _catRepos.FirstOrDefault(u => u.Id == id)
                };
                List<IndividualProductVM> individualProduct = new List<IndividualProductVM>();
                List<ProductAttribute> productAttributes = new List<ProductAttribute>();
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (var product in CatalogVM.Products)
                {
                    productAttributes.AddRange(_prodAttrRepos.GetAll(u => u.ProductId == product.Id, includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"));
                    productImages.AddRange(_prodImgRepos.GetAll(u => u.ProductId == product.Id));

                    IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                    individualProduct.Add(prodVM);
                }
                CatalogVM.IndividualProductVMs = individualProduct;
                CatalogVM.ProductAttributes = productAttributes;
                CatalogVM.ProductImages = productImages;
            }
            return View(CatalogVM);
        }

        [HttpGet]
        public IActionResult NewArrivals(int? id)
        {
            if(id == null)
            {
                CatalogVM = new CatalogVM()
                {
                    Products = _prodRepos.GetAll(includeProperties: $"{nameof(Category)}").TakeLast(8),
                    Categories = _catRepos.GetAll(),
                    ProductAttributes = _prodAttrRepos.GetAll(includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"),
                    ProductImages = _prodImgRepos.GetAll(),
                    AttributeTypes = _attrTypeRepos.GetAll(),
                    AttributeValues = _attrValRepos.GetAll(),
                    IndividualProductVMs = new List<IndividualProductVM>(),
                    CurrentCategory = null
                };
                List<IndividualProductVM> list = new List<IndividualProductVM>();
                foreach (var product in CatalogVM.Products)
                {
                    IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                    list.Add(prodVM);
                }
                CatalogVM.IndividualProductVMs = list;
            }
            else
            {
                CatalogVM = new CatalogVM()
                {
                    Products = _prodRepos.GetAll(u => u.CategoryId == id, includeProperties: $"{nameof(Category)}").TakeLast(8),
                    Categories = _catRepos.GetAll(),
                    AttributeTypes = _attrTypeRepos.GetAll(),
                    AttributeValues = _attrValRepos.GetAll(),
                    IndividualProductVMs = new List<IndividualProductVM>(),
                    CurrentCategory = _catRepos.FirstOrDefault(u => u.Id == id)
                };
                List<IndividualProductVM> individualProduct = new List<IndividualProductVM>();
                List<ProductAttribute> productAttributes = new List<ProductAttribute>();
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (var product in CatalogVM.Products)
                {
                    productAttributes.AddRange(_prodAttrRepos.GetAll(u => u.ProductId == product.Id, includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"));
                    productImages.AddRange(_prodImgRepos.GetAll(u => u.ProductId == product.Id));

                    IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                    individualProduct.Add(prodVM);
                }
                CatalogVM.IndividualProductVMs = individualProduct;
                CatalogVM.ProductAttributes = productAttributes;
                CatalogVM.ProductImages = productImages;
            }
            return View(CatalogVM);
        }

        [HttpGet]
        public IActionResult Search(string? searchText, int? id)
        {
            if(searchText == null)
            {
                CatalogVM = new CatalogVM()
                {
                    Products = new List<Product>(),
                    Categories = _catRepos.GetAll(),
                    IndividualProductVMs = new List<IndividualProductVM>(),
                    CurrentCategory = null
                };
                List<IndividualProductVM> list = new List<IndividualProductVM>();
                foreach (var product in CatalogVM.Products)
                {
                    IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                    list.Add(prodVM);
                }
                CatalogVM.IndividualProductVMs = list;
            }
            else
            {
                if (id == null)
                {
                    CatalogVM = new CatalogVM()
                    {
                        Products = _prodRepos.GetAll(u => u.Name.Contains(searchText) || u.Description.Contains(searchText) || u.ShortDesc.Contains(searchText) ,includeProperties: $"{nameof(Category)}"),
                        Categories = _catRepos.GetAll(),
                        ProductAttributes = _prodAttrRepos.GetAll(includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"),
                        ProductImages = _prodImgRepos.GetAll(),
                        AttributeTypes = _attrTypeRepos.GetAll(),
                        AttributeValues = _attrValRepos.GetAll(),
                        IndividualProductVMs = new List<IndividualProductVM>(),
                        CurrentCategory = null
                    };
                    List<IndividualProductVM> list = new List<IndividualProductVM>();
                    foreach (var product in CatalogVM.Products)
                    {
                        IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                        list.Add(prodVM);
                    }
                    CatalogVM.IndividualProductVMs = list;
                }
                else
                {
                    CatalogVM = new CatalogVM()
                    {
                        Products = _prodRepos.GetAll(u => u.CategoryId == id && (u.Name.Contains(searchText) || u.Description.Contains(searchText) || u.ShortDesc.Contains(searchText)), includeProperties: $"{nameof(Category)}").TakeLast(8),
                        Categories = _catRepos.GetAll(),
                        AttributeTypes = _attrTypeRepos.GetAll(),
                        AttributeValues = _attrValRepos.GetAll(),
                        IndividualProductVMs = new List<IndividualProductVM>(),
                        CurrentCategory = _catRepos.FirstOrDefault(u => u.Id == id)
                    };
                    List<IndividualProductVM> individualProduct = new List<IndividualProductVM>();
                    List<ProductAttribute> productAttributes = new List<ProductAttribute>();
                    List<ProductImage> productImages = new List<ProductImage>();
                    foreach (var product in CatalogVM.Products)
                    {
                        productAttributes.AddRange(_prodAttrRepos.GetAll(u => u.ProductId == product.Id, includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}"));
                        productImages.AddRange(_prodImgRepos.GetAll(u => u.ProductId == product.Id));

                        IndividualProductVM prodVM = _prodRepos.GetIndividualProductVM(product.Id);
                        individualProduct.Add(prodVM);
                    }
                    CatalogVM.IndividualProductVMs = individualProduct;
                    CatalogVM.ProductAttributes = productAttributes;
                    CatalogVM.ProductImages = productImages;
                }
            }
            CatalogVM.SearchText = searchText;
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

        private List<IndividualProductVM> GetItemsPage(int page = 1)
        {
            var itemsToSkip = page * _pageSize;

            List<Product> products = _prodRepos.GetAll().OrderBy(t => t.Id).Skip(itemsToSkip).Take(_pageSize).ToList();
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
