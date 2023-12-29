using Azure;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Models.ViewModels;
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
        private readonly ILogger<CartController> _logger;
        public CartViewModel cartViewModel {get; set;}

        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<CartController> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [Route("/Customer/Cart")]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Cart> carts = _unitOfWork.Cart.GetAll(u=> u.UserId == userId, includeProperties: "Product").ToList();
            //_logger.LogInformation($"Cart found: {carts != null}");
            //_logger.LogInformation($"Product found: {carts[0].Product != null}");
            double total= 0;
            if (carts !=null){
                total = _unitOfWork.Cart.Total(carts);
            } 
            cartViewModel = new CartViewModel(){
                cartsList = carts,
                total= total
            };
            return View("/Areas/Customer/Views/Home/Cart.cshtml", cartViewModel);
        }
        [Route("/Product/{productId}/addCart")]
        [HttpPost]
        public IActionResult AddCart(int productId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Product productInDb = _unitOfWork.Product.Get(u=>u.Id==productId);
            if (productInDb.Quantity<=0){
                var data = new {
                status = "Success",
                message = "Thêm thành công"
            };
                return Json(data);
            }else {
            Cart cart = _unitOfWork.Cart.Get(u=>u.ProductId==productId && u.UserId==userId);
            if (cart == null){
                cart = new Cart {
                UserId = userId,
                ProductId = productId,
                Quantity = int.Parse(Request.Form["quantity"])
            };
            _unitOfWork.Cart.Add(cart);
            _unitOfWork.Cart.Save();

            } else {
                cart.Quantity += int.Parse(Request.Form["quantity"]);
                _unitOfWork.Cart.Save();
            }
            _unitOfWork.Cart.Save();
            string returnUrl = Request.Headers["Referer"].ToString();
            var data = new {
                status = "Success",
                message = "Thêm thành công"
            };
            return Json(data);
            }

        }
        [Route("/Cart/update-quantity")]
        [HttpPost]
        public IActionResult updateQuantity(){
            var cartId = int.Parse(Request.Form["cartId"]);
            var quantity = int.Parse(Request.Form["quantity"]);
            Cart cart = _unitOfWork.Cart.Get(u=>u.Id == cartId);
            if (cart == null){
                var data = new {
                    status = "error",
                    message = "Có lỗi xảy ra"
                };
                return Json(data);
            }else {                
            cart.Quantity = quantity;
            _unitOfWork.Cart.Update(cart);
            _unitOfWork.Cart.Save();
                _logger.LogInformation($"Cart quantity {cart.Id} la : {cart.Quantity}");
            var data = new {
                status = "success",
                message = "Chỉnh sửa thành công"
            };
            return Json(data);
            }
        }

        [Route("/Cart/clear")]
        public IActionResult clearCart(){
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var carts = _unitOfWork.Cart.FindCartOfUser(userId).ToList();
            _unitOfWork.Cart.DeleteRange(carts);
            var data = new {
                status = "success"
            };
            return Json(data);
        }

        [Route("/Cart/{cartId}/Remove")]
        public IActionResult removeCart(int cartId){
            Cart cart = _unitOfWork.Cart.Get(u=>u.Id == cartId);
            _unitOfWork.Cart.Remove(cart);
            _unitOfWork.Cart.Save();
            return Redirect("Index");
        }
    }
}
