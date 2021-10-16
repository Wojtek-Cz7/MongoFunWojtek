﻿using MongoDB.Bson;
using MongoDB.Driver;
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

        public async Task<bool> RemoveBookAsync(ObjectId bookId)
        {
            var filter = Builders<BookModel>.Filter.Eq(x => x.Id, bookId);  // to jest filtr wskazujący na rekord do usunięcia
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

        public async Task<bool> RemoveBooksAsync(List<ObjectId> booksIds)
        {
            var filter = Builders<BookModel>.Filter.In(x => x.Id, booksIds); 
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<List<BookModel>> GetBooksNewerThanAsync(int year)
        {
            var filter = Builders<BookModel>.Filter.Gt(x => x.ReleaseDate, new DateTime(year,1,1));
            return await _collection.Find(filter).ToListAsync();
        }
    }
}