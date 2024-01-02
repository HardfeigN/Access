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
    public class OrderController : Controller
    {
        private readonly IOrderDetailRepository _orderDRepos;
        private readonly IOrderHeaderRepository _orderHRepos;
        private readonly IOrderStatusRepository _orderSRepos;
        private readonly IStatusRepository _statusRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IProductAttributeRepository _prodAttrRepos;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IOrderHeaderRepository orderHRepos, IOrderDetailRepository orderDRepos, IOrderStatusRepository orderSRepos, IProductImageRepository prodImgRepos, IStatusRepository statusRepos, IProductAttributeRepository prodAttrRepos)
        {
            _orderHRepos = orderHRepos;
            _orderDRepos = orderDRepos;
            _orderSRepos = orderSRepos;
            _prodImgRepos = prodImgRepos;
            _statusRepos = statusRepos;
            _prodAttrRepos = prodAttrRepos;
        }

        [HttpGet]
        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status_Name = null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHList = _orderHRepos.GetAll(),
                StatusList = _statusRepos.GetAllDropdownList(nameof(Status), false, false), //_statusRepos.GetAllDropdownList(nameof(OrderStatus), true) for search by id
                OrderStatus = _orderSRepos.GetAll(includeProperties: nameof(Status))
            };
            List<OrderStatus> orderStatuses = new List<OrderStatus>();
            foreach (OrderHeader ordH in orderListVM.OrderHList)
            {
                //OrderStatus orderStatus = _orderSRepos.GetAll(u => u.OrderHeaderId == ordH.Id, includeProperties: nameof(Status)).MaxBy(u => u.Date);
                OrderStatus orderStatus = orderListVM.OrderStatus.Where(u => u.OrderHeaderId == ordH.Id).MaxBy(u => u.Date);
                orderStatuses.Add(orderStatus);
                ordH.OrderStatusName = orderStatus.Status.Name;
                ordH.CreationDate = orderListVM.OrderStatus.Where(u => u.OrderHeaderId == ordH.Id).MinBy(u => u.Date).Date;
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }

            if (!string.IsNullOrEmpty(Status_Name) && Status_Name != "--Order Status--")
            {
                //orderListVM.OrderStatus = orderListVM.OrderStatus.Where(u => u.Status.Name.ToLower().Contains(Status_Name.ToLower()));
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.OrderStatusName.ToLower().Contains(Status_Name.ToLower()));
            }

            return View(orderListVM);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            OrderVM OrderVM = new OrderVM()
            {
                OrderHeader = _orderHRepos.FirstOrDefault(u => u.Id == id),
                OrderDetail = _orderDRepos.GetAll(u => u.OrderHeaderId == id, includeProperties: nameof(ProductAttribute)),
                Status = _statusRepos.GetAll()
            };
            OrderVM.OrderStatus = _orderSRepos.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: nameof(Status));
            OrderVM.OrderHeader.OrderStatusName = OrderVM.OrderStatus.MaxBy(u => u.Date).Status.Name;
            foreach (OrderDetail ordD in OrderVM.OrderDetail)
            {
                ordD.ProductAttribute = _prodAttrRepos.FirstOrDefault(u => u.Id == ordD.ProductAttributeId, includeProperties: nameof(Product));
            }

            IList<ProductImage> images = new List<ProductImage>();
            foreach (OrderDetail odetail in OrderVM.OrderDetail)
            {
                images.AddRange(_prodImgRepos.GetAll(u => u.AttributeId == odetail.ProductAttribute.Id));
            }
            OrderVM.ProductImages = images;

            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult NextStatus()
        {
            OrderStatus currentOrderStatus = _orderSRepos.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: nameof(Status)).MaxBy(u => u.Date);
            if (_statusRepos.FirstOrDefault(u => u.ParentId == currentOrderStatus.StatusId) == new Status())
            {
                TempData[WebConstants.Success] = "Current Order status is the last.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                OrderStatus orderStatus = new OrderStatus()
                {
                    OrderHeader = _orderHRepos.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id),
                    Date = DateTime.Now,
                    Status = _statusRepos.FirstOrDefault(u => u.ParentId == currentOrderStatus.StatusId)
                };
                _orderSRepos.Add(orderStatus);
                _orderSRepos.Save();

                TempData[WebConstants.Success] = $"Order status changed to {orderStatus.Status.Name}.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderStatus orderStatus = new OrderStatus()
            {
                OrderHeader = _orderHRepos.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id),
                Date = DateTime.Now,
                Status = _statusRepos.FirstOrDefault(u => u.Name == WebConstants.StatusCancelled)
            };
            _orderSRepos.Add(orderStatus);
            _orderSRepos.Save();

            TempData[WebConstants.Success] = "Order cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHRepos.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
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
