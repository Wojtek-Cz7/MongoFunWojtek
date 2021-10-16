using MongoDB.Bson;
using MongoFunWojtek.Models;
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
        Task<bool> RemoveBookAsync(string bookId);

        Task<bool> AddBooksAsync(List<BookModel> bookModels);
        Task<List<BookModel>> GetBooksByAuthorAsync(string author);

        Task<bool> RemoveBooksAsync(List<string> booksIds);
        Task<List<BookModel>> GetBooksNewerThanAsync(int year);

        Task<bool> AddReviewToBookAsync(IReview review, string bookId);

        Task<List<BookModel>> GetBooksWithSimpleReviewsAsync();

        Task<List<BookModel>> GetBooksWithGradeReviewsAsync();

        Task RemoveAllBooks();

        Task<long> CountBooksAsync();

        Task<long> CountBooksNewerThanAsync(DateTime dateTime);
        Task<long> CountBooksWithAtLeastOneReviewAsync();

        Task<List<BookTypeCount>> GroupByTypesAsync();

        Task<List<AuthorBookCountWithNewestDate>> GroupByAuthorsWithAtLeast1BookAsync();



    }
}
