using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.ViewModels;
using Utility;

namespace ProjectFrame.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_User_Cust)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<OrderController> _logger;
        public CheckoutViewModel checkoutViewModel {get; set;}

        public OrderController(ILogger<OrderController> logger,IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Route("/check-out")]
        public IActionResult CheckoutView()
        {
            //if (HttpContext.User.Identity.IsAuthenticated)
            //{     
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            //User user = _unitOfWork.User.Get(u=>u.Id == userId);
            //checkoutViewModel = new CheckoutViewModel{
            //    user = user,
            //    total = total
            //};
            //return View("/Areas/Customer/Views/Home/Cart.cshtml", checkoutViewModel);
            //}
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _unitOfWork.User.Get(u => u.Id == userId);
            List<Cart> carts = _unitOfWork.Cart.GetAll(u=>u.UserId == userId, includeProperties: "Product").ToList();
            double total = _unitOfWork.Cart.Total(carts);
            checkoutViewModel = new CheckoutViewModel
            {
                user = user,
                total = total
            };
            return View("/Areas/Customer/Views/Home/Checkout.cshtml", checkoutViewModel);

        }

        [Route("/check-out/cod")]
        [HttpPost]
        public IActionResult OrderCOD(){
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var name = Request.Form["ten"];
            var diaChi = Request.Form["address"];
            var phone = Request.Form["sdt"];
            var statusOrder = "Đã đặt hàng";
            var enable = true;
            var total = decimal.Parse(Request.Form["total"]);
            var created = DateTime.Now;
            
            List<Cart> carts = _unitOfWork.Cart.GetAll(u=>u.UserId == userId, includeProperties: "Product").ToList();
            Order order = new Order{
                Name = name,
                Address = diaChi,
                Phone = phone,
                Status = statusOrder,
                Enable = enable,
                Total = total,
                Created = created,
                CustomerId = userId
            };
            _unitOfWork.Order.Add(order);
            _unitOfWork.Save();
            User user = _unitOfWork.User.Get(u=>u.Id == userId);
            if (user.Total == null){
                user.Total = total;
            } else {
            user.Total += total;
            }
            _unitOfWork.User.Update(user);
            _unitOfWork.Save();
            _unitOfWork.Order.SaveAndClear(order.Id, carts);
            var status = "success";
            var redirect_url = "~/Customer/check-out/success";
            var data = new {
                status = status,
                redirectUrl = redirect_url
            };
            TempData["orderId"] = order.Id;
            return Json(data);


        }

        [Route("check-out/success")]
        public IActionResult SuccessView(){
            return View("/Areas/Customer/Views/Home/Success.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}