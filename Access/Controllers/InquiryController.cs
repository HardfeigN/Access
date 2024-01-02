using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace Access.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository _inqDRepos;
        private readonly IInquiryHeaderRepository _inqHRepos;
        private readonly IInquiryToOrderRepository _inqToOrdRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IProductAttributeRepository _prodAttrRepos;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryDetailRepository inqDRepos, IInquiryHeaderRepository inqHRepos, IProductImageRepository prodImgRepos,
            IProductAttributeRepository prodAttrRepos, IInquiryToOrderRepository inqToOrdRepos)
        {
            _inqDRepos = inqDRepos;
            _inqHRepos = inqHRepos;
            _prodImgRepos = prodImgRepos;
            _prodAttrRepos = prodAttrRepos;
            _inqToOrdRepos = inqToOrdRepos;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inqHRepos.FirstOrDefault(u => u.Id == id),
                InquiryDetail = _inqDRepos.GetAll(u => u.InquiryHeaderId == id, includeProperties:nameof(ProductAttribute)),//_inqDRepos.GetAll(u => u.InquiryHeaderId == id, includeProperties:$"{nameof(ProductAttribute)},{nameof(Product)}")
            };
            foreach (InquiryDetail inqD in InquiryVM.InquiryDetail)
            {
                inqD.ProductAttribute = _prodAttrRepos.FirstOrDefault(u => u.Id == inqD.ProductAttributeId, includeProperties: nameof(Product));
            }

            IList<ProductImage> images = new List<ProductImage>();
            foreach (InquiryDetail idetail in InquiryVM.InquiryDetail)
            {
                images.AddRange(_prodImgRepos.GetAll( u => u.AttributeId == idetail.ProductAttributeId));
            }
            InquiryVM.ProductImages = images;
            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            InquiryVM.InquiryDetail = _inqDRepos.GetAll(u =>u.InquiryHeaderId == InquiryVM.InquiryHeader.Id, includeProperties: nameof(ProductAttribute));

            List<ProductImage> images = new List<ProductImage>();
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (InquiryDetail idetail in InquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = idetail.ProductAttribute.ProductId,
                    ProductAttributeId = idetail.ProductAttribute.Id,
                    Quantity = idetail.Quantity
                };
                shoppingCartList.Add(shoppingCart);
                images.AddRange(_prodImgRepos.GetAll(u => u.AttributeId == idetail.ProductAttribute.Id));
            }
            InquiryVM.ProductImages = images;

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WebConstants.SessionInquiryId, InquiryVM.InquiryHeader.Id);
            TempData[WebConstants.Success] = "Inquiry converted to cart successfully";
            return RedirectToAction("Index","Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inqHRepos.FirstOrDefault(u => u.Id == InquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inqDRepos.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            _inqDRepos.RemoveRange(inquiryDetails);
            _inqHRepos.Remove(inquiryHeader);
            _inqHRepos.Save();

            TempData[WebConstants.Success] = "Inquiry deleted to cart successfully";
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            List<InquiryHeader> inquiryHeaders = _inqHRepos.GetAll().ToList();
            foreach(InquiryHeader inquiryHeader in inquiryHeaders)
            {
                InquiryToOrder inquiryToOrder = _inqToOrdRepos.FirstOrDefault(u => u.InquiryHeaderId == inquiryHeader.Id);
                if (inquiryToOrder != null) inquiryHeader.OrderHeaderId = inquiryToOrder.OrderHeaderId;
                inquiryHeader.ShortInquiryDate = inquiryHeader.InquiryDate.ToShortDateString();
            }
            return Json(new { data = inquiryHeaders });
        }

        #endregion
    }
}
