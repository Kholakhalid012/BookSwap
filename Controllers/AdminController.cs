using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BookSwap.Models;
using BookSwap.Models.Interfaces;
using BookSwap.ViewModels;
using BookSwap.Models.Services;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace BookSwap.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IBookRepository _bookRepo;
        private readonly IOrderRepository _orderRepo;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            IBookRepository bookRepo,
            INotificationService notificationService,
            IOrderRepository orderRepo)
        {
            _userManager = userManager;
            _bookRepo = bookRepo;
            _orderRepo = orderRepo;
            _notificationService = notificationService;
        }

        public IActionResult AdminDashboard()
        {
            ViewBag.TotalUsers = _userManager.Users.Count();
            ViewBag.TotalBooks = _bookRepo.GetAll().Count(b => !b.IsDeleted);
            ViewBag.TotalOrders = _orderRepo.getAllOrders().Count();

            return View();
        }

        public IActionResult Books()
        {
            var books = _bookRepo.GetAll()
                                 .Where(b => !b.IsDeleted)
                                 .ToList();
            return View(books);
        }

        [HttpGet]
        public IActionResult EditBook(int id)
        {
            var book = _bookRepo.GetById(id);
            if (book == null || book.IsDeleted)
                return NotFound();

            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

            public async Task<IActionResult> EditBook(
                int id,
                string title,
                string author,
                string category,
                decimal price,
                int stock,
                IFormFile BookImage,
                string ExistingImagePath)
            {
                var book = _bookRepo.GetById(id);
                if (book == null) return NotFound();

                string imagePath = ExistingImagePath;

                if (BookImage != null && BookImage.Length > 0)
                {
                    var ext = Path.GetExtension(BookImage.FileName).ToLower();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".svg", ".webp", ".gif" };

                    if (!allowed.Contains(ext))
                    {
                        TempData["Error"] = "Invalid image format.";
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

                book.Title = title;
                book.Author = author;
                book.Category = category;
                book.Price = price;
                book.Stock = stock;
                book.ImagePath = imagePath;

                _bookRepo.Update(book);

                TempData["Success"] = $"Book{book.Title} updated successfully By Admin!";
                return RedirectToAction("Books","Admin");
            }

       [Authorize(Roles = "Admin")]
       [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = _bookRepo.GetById(id);
            if (book == null)
                return NotFound();

            // 1️⃣ Soft delete
            _bookRepo.SoftDelete(id);
            // 2️⃣ Mark pending orders
            var orders = _orderRepo.getAllOrders()
                .Where(o => o.BookId == id && o.Status == "Pending")
                .ToList();

            foreach (var order in orders)
            {
                order.BookRemoved = true;
                _orderRepo.updateStatus(order.OrderId, order.Status!);
            }

            // 3️⃣ Notify seller
            var seller = !string.IsNullOrEmpty(book.SellerId) 
                ? await _userManager.FindByIdAsync(book.SellerId) 
                : null;
            if (seller != null && !string.IsNullOrEmpty(seller.Email))
            {
                await _notificationService.NotifyAsync(
                    seller.Email,
                    $"Your book '{book.Title}' was removed by admin."
                );
            }

            TempData["Success"] = "Book removed  successfully and seller notified.";
            return RedirectToAction("Books"); 
        }


        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

            public IActionResult Reports()
           {
            var users = _userManager.Users.ToList();
            var books = _bookRepo.GetAll().ToList();
            var orders = _orderRepo.getAllOrders().ToList();

            var userDict = users.ToDictionary(
                u => u.Id,
                u => (string?)(u.UserName ?? "Unknown")
            );

            ViewBag.TotalOrders = orders.Count;
            ViewBag.CompletedOrders = orders.Count(o => o.Status == "Completed");
            ViewBag.PendingOrders = orders.Count(o => o.Status == "Pending");
            ViewBag.TotalRevenue = orders.Sum(o => o.TotalPrice);
            ViewBag.TotalBooks = books.Count(b => !b.IsDeleted);
            ViewBag.TotalUsers = users.Count;

            // Buyer / Seller count
            ViewBag.BuyerCount = users.Count(u => _userManager.IsInRoleAsync(u, "Buyer").Result);
            ViewBag.SellerCount = users.Count(u => _userManager.IsInRoleAsync(u, "Seller").Result);

            // TOP BUYER
            var topBuyerGroup = orders
                .Where(o => !string.IsNullOrEmpty(o.BuyerId))
                .GroupBy(o => o.BuyerId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            ViewBag.TopBuyerId = topBuyerGroup?.Key;
            ViewBag.TopBuyerName =
                topBuyerGroup != null && userDict.ContainsKey(topBuyerGroup.Key!)
                    ? userDict[topBuyerGroup.Key!]
                    : "N/A";

            // TOP SELLER
            var topSellerGroup = orders
                .Join(books,
                    o => o.BookId,
                    b => b.Id,
                    (o, b) => b.SellerId)
                .Where(id => !string.IsNullOrEmpty(id))
                .GroupBy(id => id)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            ViewBag.TopSellerId = topSellerGroup?.Key;
            ViewBag.TopSellerName =
                topSellerGroup != null && userDict.ContainsKey(topSellerGroup.Key!)
                    ? userDict[topSellerGroup.Key!]
                    : "N/A";

            ViewBag.RecentOrders = orders
                .OrderByDescending(o => o.OrderId)
                .Take(5)
                .ToList();

            ViewBag.UserDict = userDict;

            return View();
        }


        public async Task<IActionResult> DownloadReportPdf()
        {
            var orders = _orderRepo.getAllOrders().OrderByDescending(o => o.OrderId).ToList();
            var users = _userManager.Users.ToList();
            var books = _bookRepo.GetAll().ToList();

            var userDict = users.ToDictionary(u => u.Id, u => (string?)(u.UserName ?? "Unknown"));
            var bookDict = books.ToDictionary(b => b.Id, b => (Book?)b);

            int buyerCount = 0, sellerCount = 0;
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Buyer")) buyerCount++;
                if (roles.Contains("Seller")) sellerCount++;
            }

            var model = new AdminReportViewModel
            {
                RecentOrders = orders!,
                UserDict = userDict,
                BookDict = bookDict,
                TotalUsers = users.Count,
                TotalBooks = books.Count(b => !b.IsDeleted),
                TotalOrders = orders.Count,
                BuyerCount = buyerCount,
                SellerCount = sellerCount
            };

            return new ViewAsPdf("ReportsPdf", model)
            {
                FileName = "AdminReport.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait
            };
        }
    }
}
