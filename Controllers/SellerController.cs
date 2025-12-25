using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BookSwap.Models;
using BookSwap.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System;
using System.Threading.Tasks;

namespace BookSwap.Controllers;
     
    [Authorize(Policy = "SellerOnly")]
 
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

         [Authorize(Roles = "Seller")]
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
        public async Task<IActionResult> AddBook(string title, string author, string category, decimal price, int stock, IFormFile BookImage)
        {
            string imagePath = "/images/default-book.svg";

            if (BookImage != null && BookImage.Length > 0)
            {
                var ext = Path.GetExtension(BookImage.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" , ".svg", ".webp"};

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
                Stock= stock,
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
          public async Task<IActionResult> EditBook(int id, string title, string author, string category, decimal price, int stock, IFormFile BookImage,string ExistingImagePath)
         {
            string imagePath = ExistingImagePath; 

            if (BookImage != null && BookImage.Length > 0)
            {
                var ext = Path.GetExtension(BookImage.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".svg", ".webp" , ".gif"};

                if (!allowed.Contains(ext))
                {
                    TempData["Error"] = "Only JPG, JPEG, PNG, SVG, WEBP , GIFimages are allowed.";
                    return RedirectToAction("EditBook", new { id });
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "books"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + ext;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await BookImage.CopyToAsync(stream);

                imagePath = "/uploads/books/" + fileName;
            }

            var seller = await _userManager.GetUserAsync(User);

            var book = new Book
            {
                Id = id,
                Title = title,
                Author = author,
                Category = category,
                Price = price,
                Stock = stock,
                ImagePath = imagePath,
                SellerId = seller.Id
            };

            _bookRepo.Update(book);

            TempData["Success"] = "Book updated successfully!";
            return RedirectToAction("MyBooks");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBook(int id)
        {
            bool deleted = _bookRepo.Delete(id);

            if (!deleted)
            {
                TempData["Error"] = "This book cannot be deleted because it has orders.";
                return RedirectToAction("MyBooks");
            }

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