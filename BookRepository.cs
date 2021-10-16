using MongoDB.Bson;
using MongoDB.Driver;
using MongoFunWojtek.Models;
using MongoFunWojtek.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    public class BookRepository : IBookRepository
    {
        private string CollectionName = "books";
        private IMongoCollection<BookModel> _collection;


        public BookRepository(IMongoDatabase mongoDatabase)
        {
            // opcjonalne - dodwanie indexu
            var options = new CreateIndexOptions
            {
                Name = "AuthorIndex"
            };

            //var model = new CreateIndexModel<BookModel>(Builders<BookModel>.IndexKeys.Ascending(x => x.Author), options);
            //_collection.Indexes.CreateOneAsync(model);

            _collection = mongoDatabase.GetCollection<BookModel>(CollectionName);
        }


        public async Task<bool> AddBookAsync(BookModel bookModel)
        {
            try
            {
                await _collection.InsertOneAsync(bookModel);
                return true;
            }
            catch (MongoWriteException)
            {
                return false;
            }
        }

        public async Task<List<BookModel>> GetBooksAsync()
        {
            var filter = Builders<BookModel>.Filter.Empty;
            return await _collection.Find(filter).ToListAsync();

        }

        public async Task<bool> RemoveBookAsync(string bookId)
        {
            var filter = Builders<BookModel>.Filter.Eq(x => x.Idek, bookId);  // to jest filtr wskazujący na rekord do usunięcia
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<bool> AddBooksAsync(List<BookModel> bookModels)
        {
            try
            {
                await _collection.InsertManyAsync(bookModels);
                return true;
            }
            catch (MongoWriteException)
            {
                return false;
            }
        }

        public async Task<List<BookModel>> GetBooksByAuthorAsync(string author)
        {
            var filter = Builders<BookModel>.Filter.Eq(x => x.Author, author);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> RemoveBooksAsync(List<string> booksIds)
        {
            var filter = Builders<BookModel>.Filter.In(x => x.Idek, booksIds); 
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<List<BookModel>> GetBooksNewerThanAsync(int year)
        {
            var filter = Builders<BookModel>.Filter.Gt(x => x.ReleaseDate, new DateTime(year,1,1));
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> AddReviewToBookAsync(IReview review, string bookId)
        {
            var filter = Builders<BookModel>.Filter.Eq(x => x.Idek, bookId.ToString());
            var update = Builders<BookModel>.Update.Push(x => x.Reviews, review);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount == 1;
        }

        public async Task<List<BookModel>> GetBooksWithSimpleReviewsAsync()
        {
            var filter = Builders<BookModel>.Filter.ElemMatch(x => x.Reviews,
                Builders<IReview>.Filter.OfType<SimpleReview>());
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<BookModel>> GetBooksWithGradeReviewsAsync()
        {
            var filter = Builders<BookModel>.Filter.ElemMatch(x => x.Reviews,                
                    Builders<IReview>.Filter.OfType<GradeReview>());
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task RemoveAllBooks()
        {
            var filter = Builders<BookModel>.Filter.Empty;
            await _collection.DeleteManyAsync(filter);
        }

        public async Task<long> CountBooksAsync()
        {
            var result = await _collection
                .Aggregate()
                .Count()
                .SingleOrDefaultAsync();

            return result?.Count ?? 0;
        }

        public async Task<long> CountBooksNewerThanAsync(DateTime dateTime)
        {
            var countResult = await _collection
                .Aggregate()
                .Match(Builders<BookModel>.Filter.Gt(x => x.ReleaseDate, dateTime))
                .Count()
                .SingleOrDefaultAsync();
            return countResult?.Count ?? 0;
        }

        public async Task<long> CountBooksWithAtLeastOneReviewAsync()
        {
            var countResult = await _collection
                .Aggregate()
                .Match(Builders<BookModel>.Filter.SizeGt(x => x.Reviews, 0))
                .Count()
                .SingleOrDefaultAsync();
            return countResult?.Count ?? 0;
        }

        public async Task<List<BookTypeCount>> GroupByTypesAsync()
        {
            //var filter = Builders<BookModel>.Filter.Empty;
            var result = await _collection
                .Aggregate()
                //.Match(filter)
                .Group(x => x.Type, grouping => new BookTypeCount
                {
                    Type = grouping.Key,
                    Count = grouping.Count()
                })
                .ToListAsync();
            return result;
        }

        public async Task<List<AuthorBookCountWithNewestDate>> GroupByAuthorsWithAtLeast1BookAsync()
        {
            var result = await _collection
                .Aggregate()
                .Group(x => x.Author, grouping => new AuthorBookCountWithNewestDate
                {
                    Author = grouping.Key,
                    Count = grouping.Count(),
                    NewestDate = grouping.Max(x => x.ReleaseDate)
                })
                .Match(Builders<AuthorBookCountWithNewestDate>.Filter.Gt(x => x.Count, 1))
                .ToListAsync();
            return result;
        }
    }
}
