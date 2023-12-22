using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace ProjectFrame.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CustomerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<User> objUsersList = _unitOfWork.User.GetByRole(SD.Role_User_Cust);
            return View(objUsersList);
        }
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            User? userFromDb = _unitOfWork.User.Get(u => u.Id == id);
            if (userFromDb == null)
            {
                return NotFound();
            }
            return View(userFromDb);
        }
        [HttpPost]
        public IActionResult Edit(User user) {
            if (ModelState.IsValid) {
                _unitOfWork.User.Update(user);
                _unitOfWork.User.Save();
                TempData["success"] = "Customer updated successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Enable(string  id)
        {
            _unitOfWork.User.Enable(id);
            return RedirectToAction("Index");
        }
    }
}
