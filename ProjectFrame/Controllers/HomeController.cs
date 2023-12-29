using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using ProjectFrame.Models;
using System.Diagnostics;

namespace ProjectFrame.Controllers
{
    public class HomeController : Controller
    {
        private ProductVM _productVM;
        private readonly ILogger<HomeController> _logger;
        private readonly UnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, UnitOfWork unitOfWork, ProductVM productVM)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _productVM = productVM;
        }

        public IActionResult Index()
        {
            //ProductVM productVM = new ProductVM(_unitOfWork);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
