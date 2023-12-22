using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace ProjectFrame.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(string? category)
        {  
            if (category == null || category.Equals(""))
            {
                return View(_unitOfWork.Product.GetAll());
            }
            List<Product> products = _unitOfWork.Product.GetForCategory(category);
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product, IFormFile? file)
        {
            product.Id = _unitOfWork.Product.newId();
            product.ImageUrl = "";
            if (ModelState.IsValid)
            {
                product.Enable = true;
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    product.ImageUrl = @"\images\product\" + fileName;

                }
                _unitOfWork.Product.Add(product);
                _unitOfWork.Product.Save();
                TempData["success"] = "Product created successfully!!";
            }
            else
            {
                var message = new List<string>();
                foreach (var key in ModelState.Keys)
                {
                    var value = ModelState[key];
                    foreach (var item in value.Errors)
                    {
                        message.Add(item.ErrorMessage);
                    }
                    TempData["error"] = "error";
                }
            }
            return RedirectToAction("Index", "");
        }
        public IActionResult Edit(int id)
        {
            Product product = _unitOfWork.Product.Get(u=>u.Id==id);
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? file) {
            if (ModelState.IsValid)
            {
                product.Enable = true;
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    product.ImageUrl = @"\images\product\" + fileName;

                }
                _unitOfWork.Product.Update(product);
                _unitOfWork.Product.Save();
                TempData["success"] = "Product created successfully!!";
            }
            else
            {
                var message = new List<string>();
                foreach (var key in ModelState.Keys)
                {
                    var value = ModelState[key];
                    foreach (var item in value.Errors)
                    {
                        message.Add(item.ErrorMessage);
                    }
                    TempData["error"] = "error";
                }
            }
            return RedirectToAction("Index", new { category = product.Category });
        }
            //public IActionResult UpSert(int? id)
            //{
            //    if (id == null || id == 0) {
            //        return View();
            //    }
            //    else
            //    {
            //        Product product = _unitOfWork.Product.Get(u=>u.Id==id);
            //        return View(product);
            //    }
            //}

            //[HttpPost]
            //public IActionResult UpSert(Product product, IFormFile? file)
            //{
            //    Boolean insert = false;
            //    if (product == null || product.Id == 0 )
            //    {
            //        insert = true;
            //        product.Id = _unitOfWork.Product.newId();

            //    }
            //    if (ModelState.IsValid)
            //    {
            //        string wwwRootPath = _webHostEnvironment.WebRootPath;
            //        if (file != null)
            //        {
            //            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            //            string productPath = Path.Combine(wwwRootPath, @"images\product");
//                        if (!string.IsNullOrEmpty(product.ImageUrl))
//                        {
//                            var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
//                            if(System.IO.File.Exists(oldImagePath))
//                            {
//                                System.IO.File.Delete(oldImagePath);
//                            }
//}
//            using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
//            {
//                file.CopyTo(fileStream);
//            }
//            product.ImageUrl = @"\images\product\" + fileName;
//        }
//        if (insert) {
//            _unitOfWork.Product.Add(product);
//            TempData["success"] = "Product created successfully!";
//        }
//        else
//        {
//            _unitOfWork.Product.Update(product);
//            TempData["success"] = "Product updated successfully!";

//        }
//        _unitOfWork.Product.Save();
//    }else
//    {
//var message = new List<string>();
//        foreach (var key in ModelState.Keys)
//        {
//            var value = ModelState[key];
//            foreach (var item in value.Errors)
//            {
//                message.Add(item.ErrorMessage);
//            }
//        }
//        TempData["error"] = "error";
//    }
//    return RedirectToAction("Index", new {category = product.Category});
//}
public IActionResult Enable(int id)
        {
            string refererUrl = HttpContext.Request.Headers["Referer"].ToString();
            _unitOfWork.Product.Enable(id);
            return Redirect(refererUrl);
        }
    }
}
