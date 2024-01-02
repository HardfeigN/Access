using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Access.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IApplicationUserRepository _userRepos;
        private readonly IOrderHeaderRepository _ordHRepos;
        private readonly IOrderDetailRepository _ordDRepos;
        private readonly IOrderStatusRepository _ordSRepos;
        private readonly IStatusRepository _statusRepos;
        private readonly IProductRepository _prodRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IProductAttributeRepository _prodAttrRepos;

        public AccountController(IApplicationUserRepository userRepos, IOrderHeaderRepository ordHRepos, IOrderDetailRepository ordDRepos, IOrderStatusRepository ordSRepos,
            IStatusRepository statusRepos, IProductRepository prodRepos, IProductImageRepository prodImgRepos, IProductAttributeRepository prodAttrRepos, SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userRepos = userRepos;
            _ordHRepos = ordHRepos;
            _ordDRepos = ordDRepos;
            _ordSRepos = ordSRepos;
            _statusRepos = statusRepos;
            _prodRepos = prodRepos;
            _prodImgRepos = prodImgRepos;
            _prodAttrRepos = prodAttrRepos;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser user = _userRepos.FirstOrDefault(u => u.Id == claim.Value);
            user.DateOfBirthOnly = DateOnly.FromDateTime(user.DateOfBirth);
            return View(user);
        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost(ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                return View(_userRepos.FirstOrDefault(u => u.Id == claim.Value));
            }
            applicationUser.DateOfBirth = new DateTime(applicationUser.DateOfBirthOnly.Year, applicationUser.DateOfBirthOnly.Month, applicationUser.DateOfBirthOnly.Day);
            _userRepos.Update(applicationUser);
            _userRepos.Save();
            
            return View(applicationUser);
        }

        [HttpGet]
        public IActionResult Orders()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = _userRepos.FirstOrDefault(u => u.Id == claim.Value);

            if (User.IsInRole(WebConstants.AdminRole))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                UserOrderList userOrderList = new UserOrderList()
                {
                    OrderHeader = _ordHRepos.GetAll(u => u.CustomerUserId == applicationUser.Id),
                };
                List<OrderDetail> orderDList = new List<OrderDetail>();
                List<OrderStatus> orderSList = new List<OrderStatus>();
                List<ProductAttribute> prodAttrList = new List<ProductAttribute>();
                List<Product> prodList = new List<Product>();
                List<ProductImage> imgList = new List<ProductImage>();

                foreach (OrderHeader ordH in userOrderList.OrderHeader)
                {
                    orderDList.AddRange(_ordDRepos.GetAll(u => u.OrderHeaderId == ordH.Id));
                    orderSList.Add(_ordSRepos.GetAll(u => u.OrderHeaderId == ordH.Id).MaxBy(s => s.Date));
                    ordH.CreationDate = _ordSRepos.GetAll(u => u.OrderHeaderId == ordH.Id).MinBy(s => s.Date).Date;
                    ordH.OrderStatusName = _ordSRepos.GetAll(u => u.OrderHeaderId == ordH.Id, includeProperties: nameof(Status)).MaxBy(s => s.Date).Status.Name;
                }
                foreach (OrderDetail ordD in orderDList)
                {
                    ProductAttribute productAttribute = _prodAttrRepos.FirstOrDefault(u => u.Id == ordD.ProductAttributeId, includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)}");
                    prodList.Add(_prodRepos.FirstOrDefault(u => u.Id == productAttribute.ProductId));
                    if(_prodImgRepos.GetAll(u => u.AttributeId == productAttribute.Id).Count() > 0)
                        imgList.Add(_prodImgRepos.FirstOrDefault(u => u.AttributeId == productAttribute.Id));
                    prodAttrList.Add(productAttribute);
                }
                userOrderList.OrderHeader = userOrderList.OrderHeader.OrderByDescending(u => u.CreationDate);
                userOrderList.OrderStatus = orderSList;
                userOrderList.OrderDetail = orderDList;
                userOrderList.ProductAttribute = prodAttrList;
                userOrderList.Product = prodList;
                userOrderList.ProductImage = imgList;

                return View(userOrderList);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
