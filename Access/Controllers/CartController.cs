using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace Access.Controllers
{
    [Authorize]

    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IApplicationUserRepository _userRepos;
        private readonly IProductRepository _prodRepos;
        private readonly IInquiryDetailRepository _inqDRepos;
        private readonly IInquiryHeaderRepository _inqHRepos;
        private readonly IOrderDetailRepository _orderDRepos;
        private readonly IOrderHeaderRepository _orderHRepos;
        private readonly IOrderStatusRepository _orderSRepos;
        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IApplicationUserRepository userRepos, IProductRepository prodRepos, IInquiryDetailRepository inqDRepos, IInquiryHeaderRepository inqHRepos, IWebHostEnvironment webHostEnvironment,
            IOrderHeaderRepository orderHRepos, IOrderDetailRepository orderDRepos, IOrderStatusRepository orderSRepos)
        {
            _userRepos = userRepos;
            _prodRepos = prodRepos;
            _inqDRepos = inqDRepos;
            _inqHRepos = inqHRepos;
            _orderHRepos = orderHRepos;
            _orderDRepos = orderDRepos;
            _orderSRepos = orderSRepos;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                //session exists
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodListTemp = _prodRepos.GetAll(u => prodInCart.Contains(u.Id));
            IList<Product> prodList = new List<Product>();

            foreach (var cartObj in shoppingCartList)
            {
                Product prodTemp = prodListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempQuantity = cartObj.Quantity;
                prodList.Add(prodTemp);
            }


            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach(Product prod in prodList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Quantity = prod.TempQuantity});
            }
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }

        [HttpGet]
        public IActionResult Summary()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(WebConstants.AdminRole))
            {
                if(HttpContext.Session.Get<int>(WebConstants.SessionInquiryId) != 0)
                {
                    InquiryHeader inquiryHeader = _inqHRepos.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WebConstants.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
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
            IEnumerable<Product> prodList = _prodRepos.GetAll(u => prodInCart.Contains(u.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser
            };
            foreach (var cartobj in shoppingCartList)
            {
                Product prodTemp = _prodRepos.FirstOrDefault(u => u.Id == cartobj.ProductId);
                prodTemp.TempQuantity = cartobj.Quantity;
                ProductUserVM.ProductList.Add(prodTemp);
            }

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ProductUserVM ProductUserVM)
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

                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    OrderPrice = ProductUserVM.ProductList.Sum(x => x.TempQuantity * x.Price),
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    FullAddress = ProductUserVM.ApplicationUser.FullAddress,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    CreateDate = DateTime.Now,
                    OrderStatusId = _orderSRepos.FirstOrDefault(u => u.StatusNumber == 0).Id
                };
                _orderHRepos.Add(orderHeader);
                _orderHRepos.Save();

                foreach (var prod in ProductUserVM.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerPiece = prod.Price,
                        Quantity = prod.TempQuantity,
                        ProductId = prod.Id
                    };
                    _orderDRepos.Add(orderDetail);

                }
                _orderDRepos.Save();
                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });

                
                TempData[WebConstants.Success] = "Order created successfully";
            }
            else
            {
                //we need to create an inquiry
                /*
                var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
               + "templates" + Path.DirectorySeparatorChar.ToString() +
               "Inquiry.html";

                var subject = "New Inquiry";
                string HtmlBody = "";
                using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }
                //Name: { 0}
                //Email: { 1}
                //Phone: { 2}
                //Products: {3}

                StringBuilder productListSB = new StringBuilder();
                foreach (var prod in ProductUserVM.ProductList)
                {
                    productListSB.Append($" - Name: {prod.Name} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
                }

                string messageBody = string.Format(HtmlBody,
                    ProductUserVM.ApplicationUser.FullName,
                    ProductUserVM.ApplicationUser.Email,
                    ProductUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());
                */
                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    ApplicationUserId = claim.Value,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now

                };

                _inqHRepos.Add(inquiryHeader);
                _inqHRepos.Save();

                foreach (var prod in ProductUserVM.ProductList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = prod.Id,

                    };
                    _inqDRepos.Add(inquiryDetail);

                }
                _inqDRepos.Save();
                TempData[WebConstants.Success] = "Inquiry submitted successfully";
            }
            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _orderHRepos.FirstOrDefault(u => u.Id == id);

            HttpContext.Session.Clear();
            return View(orderHeader);
        }

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

        public IActionResult Clear(int id)
        {
            HttpContext.Session.Clear();
            TempData[WebConstants.Success] = "Cart cleared successfully";
            return RedirectToAction("Index","Home");
        }
    }
}
