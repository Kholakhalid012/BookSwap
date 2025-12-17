using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BookSwap.Models;
using BookSwap.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System;
using System.Threading.Tasks;

namespace BookSwap.Controllers
{
    public class SellerController : Controller
    {
        private readonly IBookRepository _bookRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public SellerController(IBookRepository bookRepo, IOrderRepository orderRepo, UserManager<ApplicationUser> userManager)
        {
            _bookRepo = bookRepo;
            _orderRepo = orderRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var seller = await _userManager.GetUserAsync(User);
            if (seller == null) return RedirectToAction("Login", "Account");

            var books = _bookRepo.GetBooksBySeller(seller.Id);
            ViewBag.TotalBooks = books.Count;
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookCount()
        {
            var seller = await _userManager.GetUserAsync(User);
            if (seller == null) return Json(new { count = 0 });

            int count = _bookRepo.GetBooksBySeller(seller.Id).Count;
            return Json(new { count });
        }

        [HttpGet]
        public IActionResult AddBook() => View();

        [HttpPost]
        public async Task<IActionResult> AddBook(string title, string author, string category, decimal price, IFormFile BookImage)
        {
            string imagePath = "/images/default-book.svg";

            if (BookImage != null && BookImage.Length > 0)
            {
                var ext = Path.GetExtension(BookImage.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" , "svg", "webp"};

                if (!allowed.Contains(ext))
                {
                    TempData["Error"] = "Only JPG, JPEG, PNG, SVG, WEBP images are allowed.";
                    return RedirectToAction("AddBook");
                }

                var uploadsFolder = Path.Combine( Directory.GetCurrentDirectory(),  "wwwroot", "uploads", "books"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + ext;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await BookImage.CopyToAsync(fileStream);

                imagePath = "/uploads/books/" + fileName;
            }

            var seller = await _userManager.GetUserAsync(User);

            var book = new Book
            {
                Title = title,
                Author = author,
                Category = category,
                Price = price,
                ImagePath = imagePath,
                SellerId = seller.Id,
                SellerName = seller.UserName,
                SellerContact = seller.PhoneNumber
            };

                _bookRepo.Add(book);
                TempData["Success"] = "Book added successfully!";
                return RedirectToAction("MyBooks");
            }

            public async Task<IActionResult> MyBooks()
            {
                var seller = await _userManager.GetUserAsync(User);
                if (seller == null) return RedirectToAction("Login", "Account");

                var books = _bookRepo.GetBooksBySeller(seller.Id);
                return View(books);
            }

            [HttpGet]
            public IActionResult EditBook(int id)
            {
                var book = _bookRepo.GetById(id);
                if (book == null) return NotFound();
                return View(book);
            }

            [HttpPost]
            public IActionResult EditBook(int id, string title, string author, string category, decimal price, IFormFile BookImage)
            {
                var book = _bookRepo.GetById(id);
            if (book == null) return NotFound();

            string imagePath = book.ImagePath ?? "/images/default-book.svg";

            if (BookImage != null && BookImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BookImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BookImage.CopyTo(fileStream);
                }

                imagePath = "/uploads/" + fileName;
            }

            book.Title = title;
            book.Author = author;
            book.Category = category;
            book.Price = price;
            book.ImagePath = imagePath;

            _bookRepo.Update(book);
            TempData["Success"] = "Book updated successfully!";
            return RedirectToAction("MyBooks");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBook(int id)
        {
            var book = _bookRepo.GetById(id);
            if (book == null) return NotFound();

            _bookRepo.Delete(id);
            TempData["Success"] = "Book deleted successfully!";
            return RedirectToAction("MyBooks");
        }

        public IActionResult Orders()
        {
            var sellerId = _userManager.GetUserId(User);
            var orders = _orderRepo.getAllOrders()
                .Where(o => o.Book?.SellerId == sellerId)
                .ToList();
            return View(orders);
        }
    }
}
