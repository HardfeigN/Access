using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class StatusController : Controller
    {
        private readonly IStatusRepository _statusRepos;
        [BindProperty]
        public StatusVM StatusVM { get; set; }

        public StatusController(IStatusRepository statusRepos)
        {
            _statusRepos = statusRepos;
        }

        public IActionResult Index()
        {
            ICollection<Status> objList = _statusRepos.GetAll();
            return View(objList);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            StatusVM = new StatusVM()
            {
                Status = new Status(),
                StatusSelectList = _statusRepos.GetAllDropdownList(nameof(Status), true, true)
            };

            if (id == null)
            {
                //for create
                return View(StatusVM);
            }
            else
            {
                StatusVM.Status = _statusRepos.Find(id.GetValueOrDefault());
                if (StatusVM.Status == null)
                {
                    return NotFound();
                }
                StatusVM.StatusSelectList = StatusVM.StatusSelectList.Where(u => u.Value != StatusVM.Status.Id.ToString());
                return View(StatusVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (StatusVM.Status.Id == 0)
                {
                    //create
                    _statusRepos.Add(StatusVM.Status);
                    TempData[WebConstants.Success] = "Status created successfully";
                }
                else
                {
                    //update
                    _statusRepos.Update(StatusVM.Status);
                    TempData[WebConstants.Success] = "Status updated successfully";
                }
                _statusRepos.Save();
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating or updating Status";

            StatusVM.StatusSelectList = _statusRepos.GetAllDropdownList(nameof(Status), true, true);
            if (_statusRepos.Find(StatusVM.Status.Id) != null)
            {
                StatusVM.StatusSelectList = StatusVM.StatusSelectList.Where(u => u.Value != StatusVM.Status.Id.ToString());
            }
            return View(StatusVM);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _statusRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            StatusVM = new StatusVM()
            {
                Status = obj,
                StatusSelectList = _statusRepos.GetAllDropdownList(nameof(Status), true, true)
            };
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _statusRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WebConstants.Error] = "Error while deleting Status";
                return NotFound();
            }

            _statusRepos.Remove(obj);
            _statusRepos.Save();
            TempData[WebConstants.Success] = "Status deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
