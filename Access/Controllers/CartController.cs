using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Access.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IApplicationUserRepository _userRepos;
        private readonly IProductRepository _prodRepos;
        private readonly IProductAttributeRepository _prodAttrRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IInquiryDetailRepository _inqDRepos;
        private readonly IInquiryHeaderRepository _inqHRepos;
        private readonly IInquiryToOrderRepository _inqToOrdRepos;
        private readonly IOrderDetailRepository _orderDRepos;
        private readonly IOrderHeaderRepository _orderHRepos;
        private readonly IOrderStatusRepository _orderSRepos;
        private readonly IStatusRepository _statusRepos;
        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IApplicationUserRepository userRepos, IProductRepository prodRepos, IInquiryDetailRepository inqDRepos, IInquiryHeaderRepository inqHRepos, IWebHostEnvironment webHostEnvironment,
            IOrderHeaderRepository orderHRepos, IOrderDetailRepository orderDRepos, IOrderStatusRepository orderSRepos, IStatusRepository statusRepos, IProductAttributeRepository prodAttrRepos,
            IProductImageRepository prodImgRepos, IInquiryToOrderRepository inqToOrdRepos)
        {
            _userRepos = userRepos;
            _prodRepos = prodRepos;
            _inqDRepos = inqDRepos;
            _inqHRepos = inqHRepos;
            _orderHRepos = orderHRepos;
            _orderDRepos = orderDRepos;
            _orderSRepos = orderSRepos;
            _webHostEnvironment = webHostEnvironment;
            _statusRepos = statusRepos;
            _prodAttrRepos = prodAttrRepos;
            _prodImgRepos = prodImgRepos;
            _inqToOrdRepos = inqToOrdRepos;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(WebConstants.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WebConstants.SessionInquiryId) != 0)
                {
                    InquiryHeader inquiryHeader = _inqHRepos.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WebConstants.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        Id = inquiryHeader.ApplicationUserId,
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        FullAddress = inquiryHeader.FullAddress,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                }
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                applicationUser = _userRepos.FirstOrDefault(u => u.Id == claim.Value);
            }


            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                //session exists
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            List<int> prodAttrInCart = shoppingCartList.Select(i => i.ProductAttributeId).ToList();
            List<ProductAttribute> prodAttrList = _prodAttrRepos.GetAll(u => prodAttrInCart.Contains(u.Id), includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}").ToList();
            List<Product> productList = _prodRepos.GetAll(u => prodInCart.Contains(u.Id)).ToList();
            List<ProductImage> images = new List<ProductImage>();
            foreach(ProductAttribute prodAttr in prodAttrList)
            {
                prodAttr.TempQuantity = shoppingCartList.FirstOrDefault(u => u.ProductAttributeId == prodAttr.Id).Quantity;
                images.AddRange(_prodImgRepos.GetAll(u => u.AttributeId == prodAttr.Id));
            }

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser
            };

            ProductUserVM.ProductList = productList;
            ProductUserVM.ProductAttributeList = prodAttrList;
            ProductUserVM.ProductImageList = images;

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            if (!ModelState.IsValid)
            {
                TempData[WebConstants.Error] = "Error. Please check the entered data.";
                return View(ProductUserVM);
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WebConstants.AdminRole))
            {
                //we need to create an order
                float orderPrice = 0;
                foreach (ProductAttribute prodAttr in ProductUserVM.ProductAttributeList)
                {
                    orderPrice += (float)Math.Round((ProductUserVM.ProductList.FirstOrDefault(u =>  u.Id == prodAttr.ProductId).Price * prodAttr.TempQuantity), 2);
                }

                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    CustomerUserId = ProductUserVM.ApplicationUser.Id,
                    OrderCost = orderPrice,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    FullAddress = ProductUserVM.ApplicationUser.FullAddress,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                };
                _orderHRepos.Add(orderHeader);
                _orderHRepos.Save();

                InquiryToOrder inquiryToOrder = new InquiryToOrder()
                {
                    InquiryHeaderId = HttpContext.Session.Get<int>(WebConstants.SessionInquiryId),
                    OrderHeaderId = orderHeader.Id
                };
                _inqToOrdRepos.Add(inquiryToOrder);
                _inqToOrdRepos.Save();

                OrderStatus orderStatus = new OrderStatus()
                {
                    OrderHeader = orderHeader,
                    Date = DateTime.Now,
                    Status = _statusRepos.FirstOrDefault(u => u.Name == WebConstants.StatusPending)
                };
                _orderSRepos.Add(orderStatus);
                _orderSRepos.Save();

                foreach (var prodAttr in ProductUserVM.ProductAttributeList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerPiece = _prodRepos.FirstOrDefault(u => u.Id == prodAttr.ProductId).Price,
                        Quantity = prodAttr.TempQuantity,
                        ProductAttributeId = prodAttr.Id
                    };
                    _orderDRepos.Add(orderDetail);

                }
                _orderDRepos.Save();
                TempData[WebConstants.Success] = "Order created successfully";
                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });
            }
            else
            {
                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    ApplicationUserId = claim.Value,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    FullAddress = ProductUserVM.ApplicationUser.FullAddress,
                    InquiryDate = DateTime.Now
                };

                _inqHRepos.Add(inquiryHeader);
                _inqHRepos.Save();

                foreach (var prodAttr in ProductUserVM.ProductAttributeList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductAttributeId = prodAttr.Id,
                        Quantity = prodAttr.TempQuantity
                    };
                    _inqDRepos.Add(inquiryDetail);

                }
                _inqDRepos.Save();
                TempData[WebConstants.Success] = "Inquiry submitted successfully";
            }
            //return RedirectToAction(nameof(InquiryConfirmation));
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _orderHRepos.FirstOrDefault(u => u.Id == id);

            HttpContext.Session.Clear();
            return View(orderHeader);
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                //session exists
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            TempData[WebConstants.Success] = "Pruduct removed from your cart successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Clear(int id)
        {
            HttpContext.Session.Clear();
            TempData[WebConstants.Success] = "Cart cleared successfully";
            return RedirectToAction("Index","Catalog");
        }
    }
}
