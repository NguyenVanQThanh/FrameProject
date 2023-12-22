using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;
using Utility;

namespace ProjectFrame.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_User_Cust)]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }
    
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult AddCart(int ProductId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Cart cart = new Cart {
                UserId = userId,
                ProductId = ProductId,
                Quantity = 1,
            };
            _unitOfWork.Cart.Add(cart);
            _unitOfWork.Cart.Save();
            string returnUrl = Request.Headers["Referer"].ToString();
            return Redirect(returnUrl);
        }
    }
}
