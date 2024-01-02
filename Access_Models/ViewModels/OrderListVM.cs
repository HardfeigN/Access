using Microsoft.AspNetCore.Mvc.Rendering;

namespace Access_Models.ViewModels
{
    public class OrderListVM
    {
        public IEnumerable<OrderHeader> OrderHList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public IEnumerable<OrderStatus> OrderStatus { get; set; }
    }
}
