using Access_DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository _inqDRepos;
        private readonly IInquiryHeaderRepository _inqHRepos;
        
        public InquiryController(IInquiryDetailRepository inqDRepos, IInquiryHeaderRepository inqHRepos)
        {
            _inqDRepos = inqDRepos;
            _inqHRepos = inqHRepos;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
