using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ProjectFrame.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork){
            _unitOfWork = unitOfWork;
        }
        [Route("/Product/Category/{category}")]
        public IActionResult Index(string? category)
        {
            List<Product> products = new List<Product>();
            if (category == null){
                products = _unitOfWork.Product.GetAll().ToList();
            } else {
                products = _unitOfWork.Product.GetForCategory(category).ToList();
            }
            return View(products);
        }

        [Route("/Product/{productId}")]
        public IActionResult Detail(int productId){
            Product product = _unitOfWork.Product.Get(u=>u.Id==productId);
            return View("Detail",product);
        }
    }
}
