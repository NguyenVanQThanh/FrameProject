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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectFrame.Areas.Customer.MomoConfig;
using Utility;

namespace ProjectFrame.Areas.Customer.Controllers
{
    [Area("Customer")]
    // [Authorize(Roles = SD.Role_User_Cust)]
    public class OrderController : Controller
    {
        // --------------
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<OrderController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CheckoutViewModel checkoutViewModel { get; set; }

        public OrderController(ILogger<OrderController> logger, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public class Data
        {
            public string? status { get; set; }
            public string? message { get; set; }
            public string? redirect_url { get; set; }
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
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                User user = _unitOfWork.User.Get(u => u.Id == userId);
                List<Cart> carts = _unitOfWork.Cart.GetAll(u => u.UserId == userId, includeProperties: "Product").ToList();
                if (carts == null)
                {
                    TempData["Error"] = "Có lỗi xảy ra!";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                double total = _unitOfWork.Cart.Total(carts);
                checkoutViewModel = new CheckoutViewModel
                {
                    user = user,
                    total = total
                };
                return View("/Areas/Customer/Views/Home/Checkout.cshtml", checkoutViewModel);
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
                    TempData["Error"] = "Có lỗi xảy ra!";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                double total = _unitOfWork.Cart.Total(carts);
                checkoutViewModel = new CheckoutViewModel
                {
                    total = total
                };
                return View("/Areas/Customer/Views/Home/Checkout.cshtml", checkoutViewModel);
            }

        }

        [Route("/check-out/cod")]
        [HttpPost]
        public IActionResult OrderCOD()
        {
            var name = Request.Form["ten"];
            var diaChi = Request.Form["address"];
            var phone = Request.Form["sdt"];
            var statusOrder = "Đã đặt hàng";
            var enable = true;
            var total = decimal.Parse(Request.Form["total"]);
            var created = DateTime.Now;
            Data data = new Data();
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                List<Cart> carts = _unitOfWork.Cart.GetAll(u => u.UserId == userId, includeProperties: "Product").ToList();
                foreach (var cart in carts)
                {
                    if (cart.Product.Quantity < cart.Quantity)
                    {
                        data.status = "error";
                        data.message = "Có lỗi xảy ra!";

                        return Json(data);
                    }
                }
                Order order = new Order
                {
                    Name = name,
                    Address = diaChi,
                    Phone = phone,
                    Status = statusOrder,
                    Enable = enable,
                    Total = total,
                    Created = created,
                    CustomerId = userId,
                    Payment_method = "COD",
                    Payment_status = "Chưa thanh toán"
                };
                _unitOfWork.Order.Add(order);
                _unitOfWork.Save();
                User user = _unitOfWork.User.Get(u => u.Id == userId);
                if (user.Total == null)
                {
                    user.Total = total;
                }
                else
                {
                    user.Total += total;
                }
                _unitOfWork.User.Update(user);
                _unitOfWork.Save();
                _unitOfWork.Order.SaveAndClear(order.Id, carts);
                var redirect_url = "/check-out/success";
                data.status = "success";
                data.redirect_url = redirect_url;
                TempData["orderId"] = order.Id;
                return Json(data);

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
                    data.status = "error";
                    data.message = "Có lỗi xảy ra!";

                    return Json(data);
                }
                Order order = new Order
                {
                    Name = name,
                    Address = diaChi,
                    Phone = phone,
                    Status = statusOrder,
                    Enable = enable,
                    Total = total,
                    Created = created,
                    Payment_method = "COD",
                    Payment_status = "Chưa thanh toán"
                };
                _unitOfWork.Order.Add(order);
                _unitOfWork.Save();
                _unitOfWork.Order.SaveAndClear(order.Id, carts);
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1),  // Đặt thời gian hết hạn của Cookie là một ngày trước đây
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Cart", "", cookieOptions);  // Ghi đè Cookie hiện tại với thời gian hết hạn
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Product", "", cookieOptions);
                var redirect_url = "/check-out/success";
                data.status = "success";
                data.redirect_url = redirect_url;
                TempData["orderId"] = order.Id;
                return Json(data);
            }


        }
        [Route("/check-out/momo")]
        [HttpPost]
        public IActionResult checkoutMomo()
        {
            var name = Request.Form["ten"];
            var diaChi = Request.Form["diachi"];
            var phone = Request.Form["sdt"];
            var statusOrder = "Đã đặt hàng";
            var enable = true;
            var total = decimal.Parse(Request.Form["total"]);
            var created = DateTime.Now;
            Data data = new Data();
            Order order;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                List<Cart> carts = _unitOfWork.Cart.GetAll(u => u.UserId == userId, includeProperties: "Product").ToList();
                foreach (var cart in carts)
                {
                    if (cart.Product.Quantity < cart.Quantity)
                    {
                        data.status = "error";
                        data.message = "Có lỗi xảy ra!";

                        return Json(data);
                    }
                }
                order = new Order
                {
                    Name = name,
                    Address = diaChi,
                    Phone = phone,
                    Status = statusOrder,
                    Enable = enable,
                    Total = total,
                    Created = created,
                    CustomerId = userId,
                    Payment_method = "MOMO",
                    Payment_status = "Chưa thanh toán"
                };
                _unitOfWork.Order.Add(order);
                _unitOfWork.Save();
                User user = _unitOfWork.User.Get(u => u.Id == userId);
                if (user.Total == null)
                {
                    user.Total = total;
                }
                else
                {
                    user.Total += total;
                }
                _unitOfWork.User.Update(user);
                _unitOfWork.Save();
                _unitOfWork.Order.SaveAndClear(order.Id, carts);

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
                    data.status = "error";
                    data.message = "Có lỗi xảy ra!";

                    return Json(data);
                }
                order = new Order
                {
                    Name = name,
                    Address = diaChi,
                    Phone = phone,
                    Status = statusOrder,
                    Enable = enable,
                    Total = total,
                    Created = created,
                    Payment_method = "MOMO",
                    Payment_status = "Đã thanh toán"
                };
                _unitOfWork.Order.Add(order);
                _unitOfWork.Save();
                _unitOfWork.Order.SaveAndClear(order.Id, carts);
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1),  // Đặt thời gian hết hạn của Cookie là một ngày trước đây
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Cart", "", cookieOptions);  // Ghi đè Cookie hiện tại với thời gian hết hạn
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Product", "", cookieOptions);
            }

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMO5RGX20191128";
            string accessKey = "M8brj9K6E22vXoDB";
            string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
            string orderInfo = "test";
            // string redirectUrl = "http://localhost:5114/check-out/success";
            // string ipnUrl = "http://localhost:5114/check-out/success";
            string requestType = "captureWallet";
            string returnUrl = "http://localhost:5114/check-out/success-momo";
            string redirectUrl = "http://localhost:5114/check-out/success-momo";
            string notifyurl = "http://localhost:5114/check-out/notify";
            string ipnUrl = "http://localhost:5114/check-out/notify";

            string amount = total.ToString();
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = order.Id.ToString();

            //Before sign HMAC SHA256 signature

            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType
                ;

            // log.Debug("rawHash = "+ rawHash);

            MomoSecurity crypto = new MomoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);
            // log.Debug("Signature = " + signature);

            //build body json request
            // JObject message = new JObject
            // {
            //     { "partnerCode", partnerCode },
            //     { "accessKey", accessKey },
            //     { "requestId", requestId },
            //     { "amount", amount },
            //     { "orderId", orderId },
            //     { "orderInfo", orderInfo },
            //     { "returnUrl", returnUrl },
            //     { "notifyUrl", notifyurl },
            //     { "requestType", requestType },
            //     { "signature", signature }
            //     // { "redirectUrl", redirectUrl },
            //     // { "ipnUrl", ipnUrl },
            //     // { "lang", "en" },
            //     // { "extraData", extraData },

            // };
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };
            // log.Debug("Json request to MoMo: " + message.ToString());
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);
            data.status = "success";
            data.redirect_url = jmessage.GetValue("payUrl").ToString();
            // log.Debug("Return from MoMo: " + jmessage.ToString());
            // return Json(data);
            return Redirect(jmessage.GetValue("payUrl").ToString());

        }

        [Route("check-out/success")]
        public IActionResult SuccessView()
        {
            return View("/Areas/Customer/Views/Home/Success.cshtml");
        }
        [Route("check-out/success-momo")]
        public IActionResult SuccessMomoView(string message, string extraData)
        {
            if (!message.Equals("Successful."))
            {
                int orderId = int.Parse(extraData);
                List<OrderDetail> orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.IdOrder == orderId).ToList();
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    Order order = _unitOfWork.Order.Get(u => u.Id == orderId);
                    foreach (var orderDetail in orderDetails)
                    {
                        Cart cart = new Cart
                        {
                            UserId = order.CustomerId,
                            Quantity = orderDetail.Quantity,
                            ProductId = orderDetail.IdProduct
                        };
                        _unitOfWork.Cart.Add(cart);
                        _unitOfWork.OrderDetail.Remove(orderDetail);
                        _unitOfWork.Save();
                    }
                    _unitOfWork.Order.Remove(order);
                    _unitOfWork.Save();
                }
                else
                {
                    List<Cart> carts = new List<Cart>();
                    List<Product> products = new List<Product>();
                    Order order = _unitOfWork.Order.Get(u => u.Id == orderId);
                    foreach (var orderDetail in orderDetails)
                    {
                        Cart cart = new Cart
                        {
                            Quantity = orderDetail.Quantity,
                            ProductId = orderDetail.IdProduct
                        };
                        Product product = _unitOfWork.Product.Get(u => u.Id == orderDetail.IdProduct);
                        carts.Add(cart);
                        products.Add(product);
                        _unitOfWork.OrderDetail.Remove(orderDetail);
                        _unitOfWork.Save();
                    }
                    _unitOfWork.Order.Remove(order);
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),  // Đặt thời gian hết hạn của Cookie là một ngày trước đây
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    };
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("Cart",JsonConvert.SerializeObject(carts), cookieOptions);  // Ghi đè Cookie hiện tại với thời gian hết hạn
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("Product",JsonConvert.SerializeObject(products), cookieOptions);
                }
                    TempData["Error"] = "Mua hàng thất bại";
                    return View("/Areas/Customer/Views/Home/Success.cshtml");
            }else {
                // int orderId = int.Parse(extraData);
                TempData["orderId"] = extraData;
                return View("/Areas/Customer/Views/Home/Success.cshtml");
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}