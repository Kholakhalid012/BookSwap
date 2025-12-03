using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using BookSwap.Models;
using BookSwap.Models.Interfaces;

namespace BookSwap.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public BooksController(IBookRepository bookRepo, UserManager<IdentityUser> userManager)
        {
            _bookRepo = bookRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            ViewBag.Categories = _bookRepo.GetAllCategories();
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(
            string title,
            string author,
            string category,
            decimal price,
            IFormFile BookImage)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            string imagePath =null;

            if (BookImage != null && BookImage.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}_{BookImage.FileName}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    BookImage.CopyTo(stream);
                }

                imagePath = "/uploads/" + fileName;
            }

            var book = new Book
            {
                Title = title,
                Author = author,
                Category = category,
                Price = price,
                ImagePath = imagePath,
                SellerId = userId   
            };

            _bookRepo.Add(book);

            TempData["Success"] = "Book added successfully!";
            return RedirectToAction("MyBooks");
        }
        public IActionResult EditBook(int id)
        {
            var book = _bookRepo.GetById(id);
            if (book == null)
                return NotFound();
            

            ViewBag.Categories = _bookRepo.GetAllCategories();
            return View(book);
        }
        public IActionResult MyBooks()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var books = _bookRepo.GetBooksBySeller(userId);
            return View(books);
        }
    }
}
