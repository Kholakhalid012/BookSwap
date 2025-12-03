using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookSwap.Models;
using BookSwap.Models.Interfaces;
using BookSwap.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BookSwap.Controllers
{
    [Authorize(Roles = "Buyer")]
    public class BuyerController : Controller
    {
        private readonly IBookRepository _bookRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public BuyerController(IBookRepository bookRepo, IOrderRepository orderRepo, UserManager<ApplicationUser> userManager)
        {
            _bookRepo = bookRepo;
            _orderRepo = orderRepo;
            _userManager = userManager;
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

                bookid = bookId,
                buyerid = user.Id, 
                quantity = quantity,
                totalprice = (double)book.Price * quantity  ,
                status = "Pending"
            };

            _orderRepo.placeOrder(order);

            TempData["Success"] = "Order placed successfully!";
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
