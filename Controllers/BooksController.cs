using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // For IFormFile
using System.IO;

public class BooksController : Controller
{
[HttpPost]

    
public IActionResult AddBook(string title, string author, string category, decimal price, IFormFile BookImage)
{
    string imagePath = null;

    if (BookImage != null && BookImage.Length > 0)
    {
        // Ensure wwwroot/uploads folder exists
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        if (!Directory.Exists(uploads))
        {
            Directory.CreateDirectory(uploads);
        }

        // Save the file
        var fileName = Path.GetFileName(BookImage.FileName);
        var filePath = Path.Combine(uploads, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            BookImage.CopyTo(fileStream);
        }

        imagePath = "/uploads/" + fileName; // Path to store in database
    }

    // TODO: Save book info to database
    // title, author, category, price, imagePath

    TempData["Success"] = "Book added successfully!";
    return RedirectToAction("MyBooks");
}
}
