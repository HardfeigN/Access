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
        private readonly IOrderDetailRepository _orderDRepos;
        private readonly IOrderHeaderRepository _orderHRepos;
        private readonly IOrderStatusRepository _orderSRepos;
        private readonly IStatusRepository _statusRepos;
        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IApplicationUserRepository userRepos, IProductRepository prodRepos, IWebHostEnvironment webHostEnvironment,
            IOrderHeaderRepository orderHRepos, IOrderDetailRepository orderDRepos, IOrderStatusRepository orderSRepos, IStatusRepository statusRepos, IProductAttributeRepository prodAttrRepos,
            IProductImageRepository prodImgRepos)
        {
            _userRepos = userRepos;
            _prodRepos = prodRepos;
            _orderHRepos = orderHRepos;
            _orderDRepos = orderDRepos;
            _orderSRepos = orderSRepos;
            _webHostEnvironment = webHostEnvironment;
            _statusRepos = statusRepos;
            _prodAttrRepos = prodAttrRepos;
            _prodImgRepos = prodImgRepos;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(WebConstants.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WebConstants.SessionInquiryId) != 0)
                {
                    OrderHeader orderHeader = _orderHRepos.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WebConstants.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        Id = orderHeader.CustomerUserId,
                        Email = orderHeader.Email,
                        FullName = orderHeader.FullName,
                        FullAddress = orderHeader.FullAddress,
                        PhoneNumber = orderHeader.PhoneNumber
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
                images.Add(_prodImgRepos.GetAll(u => u.AttributeId == prodAttr.Id).MinBy(u => u.ImageNumber));
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
                if (HttpContext.Session.Get<int>(WebConstants.SessionInquiryId) != 0)
                {
                    OrderHeader orderHeader = _orderHRepos.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WebConstants.SessionInquiryId));
                    OrderStatus currentOrderStatus = _orderSRepos.GetAll(u => u.OrderHeaderId == orderHeader.Id, includeProperties: nameof(Status)).MaxBy(u => u.Date);
                    if (currentOrderStatus.Status.Name == WebConstants.StatusPending)
                    {
                        float orderPrice = 0;
                        foreach (ProductAttribute prodAttr in ProductUserVM.ProductAttributeList)
                        {
                            orderPrice += (float)Math.Round((ProductUserVM.ProductList.FirstOrDefault(u => u.Id == prodAttr.ProductId).Price * prodAttr.TempQuantity), 2);
                        }

                        orderHeader.ApproveUserId = claim.Value;
                        orderHeader.OrderCost = orderPrice;
                        orderHeader.FullName = ProductUserVM.ApplicationUser.FullName;
                        orderHeader.FullAddress = ProductUserVM.ApplicationUser.FullAddress;
                        orderHeader.Email = ProductUserVM.ApplicationUser.Email;
                        orderHeader.PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber;

                        OrderStatus orderStatus = new OrderStatus()
                        {
                            OrderHeader = _orderHRepos.FirstOrDefault(u => u.Id == orderHeader.Id),
                            Date = DateTime.Now,
                            Status = _statusRepos.FirstOrDefault(u => u.ParentId == currentOrderStatus.StatusId)
                        };
                        _orderSRepos.Add(orderStatus);
                        _orderSRepos.Save();

                        _orderDRepos.RemoveRange(_orderDRepos.GetAll(u => u.OrderHeaderId == orderHeader.Id));
                        foreach (var prodAttr in ProductUserVM.ProductAttributeList)
                        {
                            if( prodAttr.TempQuantity > 0)
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
                        }
                        _orderDRepos.Save();

                        OrderVM orderVM = new OrderVM()
                        {
                            OrderHeader = new OrderHeader()
                            {
                                Id = orderHeader.Id
                            }
                        };
                        TempData[WebConstants.Success] = "Order was approved successfully";
                        return RedirectToAction("Index", "Order");
                    }
                    else
                    {
                        //status is not pending
                        TempData[WebConstants.Success] = $"Error. Order status is not \"{WebConstants.StatusPending}\"";
                        return View(ProductUserVM);
                    }
                }
                TempData[WebConstants.Success] = "Error. Order not found";
                return RedirectToAction("Index", "Order");
            }
            else
            {
                float orderPrice = 0;
                foreach (ProductAttribute prodAttr in ProductUserVM.ProductAttributeList)
                {
                    orderPrice += (float)Math.Round((ProductUserVM.ProductList.FirstOrDefault(u => u.Id == prodAttr.ProductId).Price * prodAttr.TempQuantity), 2);
                }

                OrderHeader orderHeader = new OrderHeader()
                {
                    CustomerUserId = claim.Value,
                    OrderCost = orderPrice,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    FullAddress = ProductUserVM.ApplicationUser.FullAddress,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                };
                _orderHRepos.Add(orderHeader);
                _orderHRepos.Save();

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
                    if (prodAttr.TempQuantity > 0)
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
                }
                _orderDRepos.Save();

                /*
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
                TempData[WebConstants.Success] = "Inquiry submitted successfully";
                _inqDRepos.Save();
                */
                TempData[WebConstants.Success] = "Order was created successfully";
            }
            return RedirectToAction("Index", "Home");
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
            TempData[WebConstants.Success] = "Pruduct was removed from your cart successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Clear(int id)
        {
            HttpContext.Session.Clear();
            TempData[WebConstants.Success] = "Cart was cleared successfully";
            return RedirectToAction("Index","Catalog");
        }
    }
}
