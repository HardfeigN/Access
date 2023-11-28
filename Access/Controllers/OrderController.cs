using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace Access.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderDetailRepository _orderDRepos;
        private readonly IOrderHeaderRepository _orderHRepos;
        private readonly IOrderStatusRepository _orderSRepos;
        private readonly IProductImageRepository _prodImgRepos;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IOrderHeaderRepository orderHRepos, IOrderDetailRepository orderDRepos, IOrderStatusRepository orderSRepos, IProductImageRepository prodImgRepos)
        {
            _orderHRepos = orderHRepos;
            _orderDRepos = orderDRepos;
            _orderSRepos = orderSRepos;
            _prodImgRepos = prodImgRepos;
        }

        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status_Name = null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHList = _orderHRepos.GetAll(),
                OrderStatusList = _orderSRepos.GetAllDropdownList(nameof(OrderStatus), false) //_orderSRepos.GetAllDropdownList(nameof(OrderStatus), true) for search by id
            };
            foreach (var ordH in orderListVM.OrderHList)
            {
                ordH.OrderStatus = _orderSRepos.FirstOrDefault(u => u.Id == ordH.OrderStatusId);
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.FullName.ToLower().Contains(searchEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.FullName.ToLower().Contains(searchPhone.ToLower()));
            }

            if (!string.IsNullOrEmpty(Status_Name) && Status_Name != "--Order Status--")
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.OrderStatus.Name.ToLower().Contains(Status_Name.ToLower()));
            }

            return View(orderListVM);
        }

        public IActionResult Details(int id)
        {
            OrderVM OrderVM = new OrderVM()
            {
                OrderHeader = _orderHRepos.FirstOrDefault(u => u.Id == id, includeProperties: nameof(OrderStatus)),
                OrderDetail = _orderDRepos.GetAll(u => u.OrderHeaderId == id, includeProperties: nameof(Product))
            };
            IList<ProductImage> images = new List<ProductImage>();
            foreach (OrderDetail odetail in OrderVM.OrderDetail)
            {
                images.AddRange(_prodImgRepos.GetProductImages(odetail.ProductId));
            }
            OrderVM.ProductImages = images;

            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHRepos.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: nameof(OrderStatus));
            orderHeader.OrderStatus = _orderSRepos.FirstOrDefault(u => u.Name == WebConstants.StatusProcessing);
            _orderHRepos.Save();

            TempData[WebConstants.Success] = "Order in process.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHRepos.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: nameof(OrderStatus));
            orderHeader.OrderStatus = _orderSRepos.FirstOrDefault(u => u.Name == WebConstants.StatusShipped);
            orderHeader.ShippingDate = DateTime.Now;
            _orderHRepos.Save();

            TempData[WebConstants.Success] = "Order shipped successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHRepos.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: nameof(OrderStatus));
            orderHeader.OrderStatus = _orderSRepos.FirstOrDefault(u => u.Name == WebConstants.StatusRefunded);
            _orderHRepos.Save();

            TempData[WebConstants.Success] = "Order cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHRepos.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: nameof(OrderStatus));
            orderHeaderFromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.Email = OrderVM.OrderHeader.Email;
            orderHeaderFromDb.FullAddress = OrderVM.OrderHeader.FullAddress;

            _orderHRepos.Save();

            TempData[WebConstants.Success] = "Order Details updated successfully.";
            return RedirectToAction("Details", "Order", new { id = orderHeaderFromDb.Id });
        }
    }
}
