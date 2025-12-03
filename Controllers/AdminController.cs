using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BookSwap.Models;
using BookSwap.Models.Interfaces;

namespace BookSwap.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookRepository _bookRepo;
        private readonly IOrderRepository _orderRepo;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            IBookRepository bookRepo,
            IOrderRepository orderRepo)
        {
            _userManager = userManager;
            _bookRepo = bookRepo;
            _orderRepo = orderRepo;
        }

        // -------------------- Dashboard --------------------
        public IActionResult AdminDashboard()
        {
            var users = _userManager.Users.ToList();
            var books = _bookRepo.GetAll().ToList();
            var orders = _orderRepo.getAllOrders().ToList();

            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalBooks = books.Count;
            ViewBag.TotalOrders = orders.Count;

            return View();
        }

        // -------------------- Users --------------------
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // -------------------- Books --------------------
        public IActionResult Books()
        {
            var books = _bookRepo.GetAll().ToList();
            return View(books);
        }

        // -------------------- Categories --------------------
        public IActionResult Categories()
        {
            var categories = _bookRepo.GetAllCategories();  
            return View(categories);
        }

        [HttpPost]
        public IActionResult AddCategory(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
                _bookRepo.AddCategory(categoryName);

            return RedirectToAction("Categories");
        }

        // -------------------- Reports --------------------
        public IActionResult Reports()
        {
            var users = _userManager.Users.ToList();
            var books = _bookRepo.GetAll().ToList();
            var orders = _orderRepo.getAllOrders().ToList();

            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalBooks = books.Count;
            ViewBag.TotalOrders = orders.Count;

            return View();
        }
    }
}
