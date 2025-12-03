using System.Collections.Generic;

namespace BookSwap.Models.Interfaces
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAll();
        Book GetById(int id);
        void Add(Book book);
        void Update(Book book);
        void Delete(int id);
        List<Book> GetBooksBySeller(string sellerId);
        List<string> GetAllCategories();           
        void AddCategory(string categoryName);     
    }
}
