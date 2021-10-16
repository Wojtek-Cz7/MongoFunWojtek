using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    public interface IBookRepository
    {
        Task<bool> AddBookAsync(BookModel bookModel);
        Task<List<BookModel>> GetBooksAsync();
        Task<bool> RemoveBookAsync(ObjectId bookId);

        Task<bool> AddBooksAsync(List<BookModel> bookModels);
        Task<List<BookModel>> GetBooksByAuthorAsync(string author);

        Task<bool> RemoveBooksAsync(List<ObjectId> booksIds);
        Task<List<BookModel>> GetBooksNewerThanAsync(int year);

    }
}
