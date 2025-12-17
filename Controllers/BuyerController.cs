using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookSwap.Models;
using BookSwap.Models.Interfaces;
using BookSwap.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using BookSwap.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BookSwap.Controllers
{
    public class BuyerController : Controller
    {
        private readonly IBookRepository _bookRepo;
        private readonly IHubContext<StockHub> _hubContext;
        private readonly IOrderRepository _orderRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public BuyerController(IBookRepository bookRepo, IOrderRepository orderRepo, UserManager<ApplicationUser> userManager, IHubContext<StockHub> hubContext)
        {
            _bookRepo = bookRepo;
            _orderRepo = orderRepo;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Index()
        {
            var books = _bookRepo.GetAll() ?? new List<Book>();
            return View(books);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int bookId, int quantity)
        {
            var book = _bookRepo.GetById(bookId);
            if (book == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var order = new Order
            {
                BookId = bookId,
                BuyerId = user.Id,
                Quantity = quantity,
                TotalPrice = (double)book.Price * quantity,
                Status = "Pending"
            };

            bool success = _orderRepo.placeOrder(order, out string message);

            if (!success)
            {
                TempData["Error"] = message;
                return RedirectToAction("Index"); 
            }

            TempData["Success"] = message;
            return RedirectToAction("MyOrders");
        }


        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var orders = _orderRepo.getOrdersByBuyer(user.Id); // string
            return View(orders);
        }
    }

}
