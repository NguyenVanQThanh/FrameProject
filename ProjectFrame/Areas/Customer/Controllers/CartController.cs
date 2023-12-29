using Azure;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;
using Utility;
using System.Web;

namespace ProjectFrame.Areas.Customer.Controllers
{
    [Area("Customer")]
    // [Authorize(Roles = SD.Role_User_Cust)]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CartController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public class Data(){
            public string status {set; get;}
            public string message {set; get;}
            public string? product_price {set; get;}
        }
        public static Data dataFault = new Data(){
            status = "Error",
            message = "Có lỗi xảy ra",
        };
        public CartViewModel cartViewModel { get; set; }

        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<CartController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("/Customer/Cart")]
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                List<Cart> carts = _unitOfWork.Cart.GetAll(u => u.UserId == userId, includeProperties: "Product").ToList();
                //_logger.LogInformation($"Cart found: {carts != null}");
                //_logger.LogInformation($"Product found: {carts[0].Product != null}");
                double total = 0;
                if (carts != null)
                {
                    total = _unitOfWork.Cart.Total(carts);
                }
                cartViewModel = new CartViewModel()
                {
                    cartsList = carts,
                    total = total
                };
                return View("/Areas/Customer/Views/Home/Cart.cshtml", cartViewModel);
            }
            else
            {
                string cartData = _httpContextAccessor.HttpContext.Request.Cookies["Cart"];
                string productData = _httpContextAccessor.HttpContext.Request.Cookies["Product"];
                List<Cart> carts;
                List<Product> products;
                if (!string.IsNullOrEmpty(cartData))
                {
                    carts = JsonConvert.DeserializeObject<List<Cart>>(cartData);
                    products = JsonConvert.DeserializeObject<List<Product>>(productData);
                }
                else
                {
                    carts = new List<Cart>();
                    products = new List<Product>();
                }

                // Create a view model with the cart data
                var cartViewModel = new CartViewModel()
                {
                    cartsList = carts,
                    productList = products,
                    total = _unitOfWork.Cart.Total(carts) // Calculate total if needed
                };

                // Set the cookie with CookieOptions
                //var cookieOptions = new CookieOptions
                //{
                //    Expires = DateTime.Now.AddDays(7),
                //    HttpOnly = true,
                //    Secure = true,
                //    SameSite = SameSiteMode.Lax
                //};
                //_httpContextAccessor.HttpContext.Response.Cookies.Append("Cart", JsonConvert.SerializeObject(carts), cookieOptions);
                //_httpContextAccessor.HttpContext.Response.Cookies.Append("Product", JsonConvert.SerializeObject(products), cookieOptions);

                return View("/Areas/Customer/Views/Home/Cart.cshtml", cartViewModel);

                // Trường hợp sử dụng Cookie


            }
        }
        [Route("/Product/{productId}/addCart")]
        [HttpPost]
        // [Authorize(Roles = SD.Role_User_Cust)]
        public IActionResult AddCart(int productId)
        {
            Data data = new Data{
                status = "error",
                message = "Có lỗi xảy ra"
            };
            Product productInDb = _unitOfWork.Product.Get(u => u.Id == productId);
            if (productInDb.Quantity < int.Parse(Request.Form["quantity"]) || productInDb == null)
            {
                return Json(data);
            }
            else
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                    Cart cart = _unitOfWork.Cart.Get(u => u.ProductId == productId && u.UserId == userId);
                    if (cart == null)
                    {
                        cart = new Cart
                        {
                            UserId = userId,
                            ProductId = productId,
                            Quantity = int.Parse(Request.Form["quantity"])
                        };
                        _unitOfWork.Cart.Add(cart);
                        _unitOfWork.Cart.Save();

                    }
                    else
                    {
                        cart.Quantity += int.Parse(Request.Form["quantity"]);
                        if (productInDb.Quantity < cart.Quantity)
                        {
                            return Json(data);
                        }
                        _unitOfWork.Cart.Update(cart);
                        _unitOfWork.Cart.Save();
                    }
                    _unitOfWork.Cart.Save();
                    string returnUrl = Request.Headers["Referer"].ToString();
                    data.status = "Success";
                    data.message = "Thêm thành công";
                    return Json(data);
                }
                else
                {
                    //Thêm vào Cookie
                    // Thêm sản phẩm vào Cookie
                    string cartData = _httpContextAccessor.HttpContext.Request.Cookies["Cart"];
                    string productData = _httpContextAccessor.HttpContext.Request.Cookies["Product"];
                    List<Cart> carts;
                    List<Product> products;
                    if (!string.IsNullOrEmpty(cartData))
                    {
                        carts = JsonConvert.DeserializeObject<List<Cart>>(cartData);
                        products = JsonConvert.DeserializeObject<List<Product>>(productData);
                    }
                    else
                    {
                        carts = new List<Cart>();
                        products = new List<Product>();
                    }

                    var cartItem = carts.FirstOrDefault(c => c.ProductId == productId);
                    if (cartItem != null)
                    {
                        // Nếu sản phẩm đã tồn tại trong giỏ hàng, chỉ cập nhật số lượng
                        cartItem.Quantity += int.Parse(Request.Form["quantity"]);
                        if (productInDb.Quantity < cartItem.Quantity)
                        {
                            return Json(data);
                        }
                    }
                    else
                    {
                        // Nếu sản phẩm chưa tồn tại, thêm mới vào giỏ hàng
                        cartItem = new Cart
                        {
                            ProductId = productId,
                            Quantity = int.Parse(Request.Form["quantity"])
                        };
                        carts.Add(cartItem);
                        Product product = _unitOfWork.Product.Get(u => u.Id == productId);
                        products.Add(product);
                    }

                    // Lưu danh sách mới vào Cookie
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    };
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("Cart", JsonConvert.SerializeObject(carts), cookieOptions);
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("Product", JsonConvert.SerializeObject(products), cookieOptions);


                    data.status = "Success";
                    data.message = "Thêm thành công";
                    return Json(data);

                }
            }

        }
        [Route("/Cart/update-quantity")]
        [HttpPost]
        public IActionResult updateQuantity()
        {
            Data data = new Data{
                status = "error",
                message = "Có lỗi xảy ra"
            };
            var cartId = int.Parse(Request.Form["cartId"]);
            var quantity = int.Parse(Request.Form["quantity"]);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Cart cart = _unitOfWork.Cart.Get(u => u.Id == cartId, includeProperties: "Product");
                if (cart == null)
                {
                    return Json(data);
                }
                else
                {
                    if (cart.Product.Quantity < quantity)
                        {
                            return Json(dataFault);
                        }
                    cart.Quantity = quantity;
                    _unitOfWork.Cart.Update(cart);
                    _unitOfWork.Cart.Save();
                    _logger.LogInformation($"Cart quantity {cart.Id} la : {cart.Quantity}");
                    data.status = "success";
                    data.message = "Chỉnh sửa thành công";
                    data.product_price = cart.Product.Price.ToString();
                    return Json(data);
                }
            }
            else
            {
                // Cookie
                string cartData = _httpContextAccessor.HttpContext.Request.Cookies["Cart"];
                string productData = _httpContextAccessor.HttpContext.Request.Cookies["Product"];
                List<Cart> carts;
                List<Product> products;
                if (!string.IsNullOrEmpty(cartData))
                {
                    carts = JsonConvert.DeserializeObject<List<Cart>>(cartData);
                    products = JsonConvert.DeserializeObject<List<Product>>(productData);
                }
                else
                {
                    carts = new List<Cart>();
                    products = new List<Product>();
                }

                var cartItem = carts.FirstOrDefault(c => c.Id == cartId);
                var productItem = products.FirstOrDefault(p=>p.Id == cartItem.ProductId);
                if (cartItem != null)
                {
                    if (productItem.Quantity < quantity)
                        {
                            return Json(dataFault);
                        }
                    cartItem.Quantity = int.Parse(Request.Form["quantity"]);
                }

                // Lưu danh sách đã cập nhật vào Cookie
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Cart", JsonConvert.SerializeObject(carts), cookieOptions);
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Product", JsonConvert.SerializeObject(products), cookieOptions);
                data.status = "success";
                data.message = "Chỉnh sửa số lượng sản phẩm thành công thông qua Cookie.";
                data.product_price=productItem.Price.ToString();
                return Json(data);
            }
        }

        [Route("/Cart/clear")]
        public IActionResult clearCart()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var carts = _unitOfWork.Cart.FindCartOfUser(userId).ToList();
                _unitOfWork.Cart.DeleteRange(carts);
                var data = new
                {
                    status = "success"
                };
                return Json(data);
            }
            else
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1),  // Đặt thời gian hết hạn của Cookie là một ngày trước đây
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Cart", "", cookieOptions);  // Ghi đè Cookie hiện tại với thời gian hết hạn
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Product", "", cookieOptions);  // Ghi đè Cookie hiện tại với thời gian hết hạn
                var data = new
                {
                    status = "success"
                };
                return Json(data);
            }
        }

        [Route("/Cart/{cartId}/Remove")]
        public IActionResult removeCart(int cartId)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Cart cart = _unitOfWork.Cart.Get(u => u.Id == cartId);
                _unitOfWork.Cart.Remove(cart);
                _unitOfWork.Cart.Save();
                return RedirectToAction("Index");
            }
            else
            {
                string cartData = _httpContextAccessor.HttpContext.Request.Cookies["Cart"];
                string productData = _httpContextAccessor.HttpContext.Request.Cookies["Product"];
                List<Cart> carts;
                List<Product> products;
                if (!string.IsNullOrEmpty(cartData))
                {
                    carts = JsonConvert.DeserializeObject<List<Cart>>(cartData);
                    products = JsonConvert.DeserializeObject<List<Product>>(productData);
                }
                else
                {
                    carts = new List<Cart>();
                    products = new List<Product>();
                }
                var cartItemToRemove = carts.FirstOrDefault(c => c.Id == cartId);
                if (cartItemToRemove != null)
                {
                    Product product = products.FirstOrDefault(p => p.Id == cartItemToRemove.ProductId);
                    carts.Remove(cartItemToRemove);
                    products.Remove(product);
                    // Lưu danh sách đã cập nhật vào Cookie
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    };
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("Cart", JsonConvert.SerializeObject(carts), cookieOptions);
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("Product", JsonConvert.SerializeObject(products), cookieOptions);
                }

                return RedirectToAction("Index"); // Hoặc chuyển hướng đến trang giỏ hàng của người dùng
            }
        }
    }
}

