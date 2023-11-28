using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace Access.Controllers
{
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository _inqDRepos;
        private readonly IInquiryHeaderRepository _inqHRepos;
        private readonly IProductImageRepository _prodImgRepos;
        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryDetailRepository inqDRepos, IInquiryHeaderRepository inqHRepos, IProductImageRepository prodImgRepos)
        {
            _inqDRepos = inqDRepos;
            _inqHRepos = inqHRepos;
            _prodImgRepos = prodImgRepos;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inqHRepos.FirstOrDefault(u => u.Id == id),
                InquiryDetail = _inqDRepos.GetAll(u => u.InquiryHeaderId == id, includeProperties:nameof(Product)),
            };
            IList<ProductImage> images = new List<ProductImage>();
            foreach (InquiryDetail idetail in InquiryVM.InquiryDetail)
            {
                images.AddRange(_prodImgRepos.GetProductImages(idetail.ProductId));
            }
            InquiryVM.ProductImages = images;
            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            InquiryVM.InquiryDetail = _inqDRepos.GetAll(u =>u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            List<ProductImage> images = new List<ProductImage>();
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (InquiryDetail idetail in InquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = idetail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
                images.AddRange(_prodImgRepos.GetProductImages(idetail.ProductId));
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
            return Json(new { data =_inqHRepos.GetAll() });
        }

        #endregion
    }
}
