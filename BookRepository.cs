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

        public async Task<List<string>> GetBooksTitlesAsync()
        {
            var result = await _collection
                .Aggregate()
                .Project(x => new
                {
                    x.Title
                })
                .ToListAsync();
            return result.Select(x => x.Title).ToList();
        }

        public async Task<(List<BookCountByDateStart> Centuries, List<BookCountByDateStart> Decades)> GetBooksCountInCenturiesAndDecadesAsync()
        {
            var centuriesDates = new List<DateTime>();
            for (var i = 1400; i <= 2100; i += 100)
                centuriesDates.Add(new DateTime(i, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            var decadesDates = new List<DateTime>();
            for (var i = 1940; i <= 2030; i += 10)
                decadesDates.Add(new DateTime(i, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            var result = await _collection
                .Aggregate()
                .Facet(AggregateFacet.Create("Centuries",
                        new EmptyPipelineDefinition<BookModel>().Bucket(
                            x => x.ReleaseDate,
                            centuriesDates,
                            x => new
                            {
                                _id = default(DateTime),  // musi być _id
                                count = x.Count()
                            },
                            new AggregateBucketOptions<DateTime>
                            {
                                DefaultBucket = new DateTime(1, 1, 1)
                            }
                        )
                    ),
                    AggregateFacet.Create("Decades",
                        new EmptyPipelineDefinition<BookModel>().Bucket(
                            x => x.ReleaseDate,
                            decadesDates,
                            x => new
                            {
                                _id = default(DateTime),
                                count = x.Count()
                            },
                            new AggregateBucketOptions<DateTime>
                            {
                                DefaultBucket = new DateTime(1, 1, 1)
                            }
                        )
                    )
                ).SingleAsync();

            var result1 = FacetOutput(new { _id = default(DateTime), count = default(int) }, result.Facets[0]);
            var result2 = FacetOutput(new { _id = default(DateTime), count = default(int) }, result.Facets[1]);

            var centuries = result1.Select(x => new BookCountByDateStart
            {
                DateStart = x._id,
                Count = x.count
            }).ToList();
            var decades = result2.Select(x => new BookCountByDateStart
            {
                DateStart = x._id,
                Count = x.count
            }).ToList();

            return (centuries, decades);
        }

        // to jest trudne :O
        private static IReadOnlyList<T> FacetOutput<T>(T _, AggregateFacetResult result) => result.Output<T>();


        // opcja prosta, ale mało wydajna
        public async Task<List<AuthorAverageOverallOfExpertReviews>> AverageOverallOfExpertReviewsByAuthorAsync()
        {
            var result = await _collection.Aggregate()
                .Unwind<BookModel, BookAuthorUnwindReviewModel>(x => x.Reviews)
                .Match(Builders<BookAuthorUnwindReviewModel>.Filter.OfType<IReview, ExpertReview>(x => x.Reviews))
                .Group(x => x.Author, grouping => new AuthorAverageOverallOfExpertReviews
                {
                    Author = grouping.Key,
                    Average = grouping.Average(x => ((ExpertReview)x.Reviews).Overall)
                })
                .ToListAsync();

            return result;
        }

        // OPCJA EXPERT

        //    public async Task<List<AuthorAverageOverallOfExperts>> GetAverageOverallOfExpertReviewsByAuthorAsync()
        //    {
        //        var result = await _collection
        //            .Aggregate()
        //            .Project(
        //                (ProjectionDefinition<BookModel, BookAuthorFilteredReviewModel>)
        //                $@"{{
        //    'author' : '$author',
        //    'reviews' : {{
        //        '$filter' : {{
        //            'input' : '$reviews',
        //            'as' : 'item',
        //            'cond' : {{
        //                '$eq' : ['$$item._t', 'Expert']
        //            }}
        //        }}
        //    }}
        //}}")
        //            .Unwind<BookAuthorFilteredReviewModel, BookAuthorReviewUnwindModel>(x => x.Reviews)
        //            .Group(x => x.Author, grouping => new AuthorAverageOverallOfExperts
        //            {
        //                Author = grouping.Key,
        //                Average = grouping.Average(x => ((ExpertReview)x.Reviews).Overall)
        //            })
        //            .ToListAsync();
        //        return result;
        //    }


    }



}
