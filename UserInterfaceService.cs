﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    class UserInterfaceService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IBookRepository _bookRepository;
        ILogger<UserInterfaceService> _logger;

        public UserInterfaceService(IHostApplicationLifetime hostApplicationLifetime, IBookRepository bookRepository, ILogger<UserInterfaceService> logger)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(500, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();

                Console.Write("Command: ");
                var command = Console.ReadLine();

                try
                {
                    switch (command)
                    {
                        case "hello":
                            Console.WriteLine("Hello Mongo command ;o");
                            break;
                        case "exit":
                            _hostApplicationLifetime.StopApplication();
                            break;
                        case "add":
                            await AddBook();
                            break;
                        case "get":
                            await GetBooks();
                            break;
                        case "remove":
                            await RemoveBook();
                            break;
                        case "adds":
                            await AddBooks();
                            break;
                        case "getauthor":
                            await GetBooksByAuthor();
                            break;
                        case "getnt":
                            await GetBooksNewerThan();
                            break;
                        case "removebooks":
                        await RemoveBooks();
                            break;



                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
                
            }
        }

        private async Task RemoveBooks()
        {
            Console.Write("Author: ");
            var author = Console.ReadLine();
           
        }

        private async Task GetBooksNewerThan()
        {
            Console.Write("Year: ");
            var year = int.Parse(Console.ReadLine()!);
            var books = await _bookRepository.GetBooksNewerThanAsync(new DateTime(year, 1, 1).Year);
            Console.WriteLine(string.Join("\n", books));
        }

        private async Task AddBook()
        {
            Console.Write("Title: ");
            var title = Console.ReadLine();
            Console.Write("Author: ");
            var author = Console.ReadLine();
            Console.Write("Year: ");
            var year = Console.ReadLine();
            var book = new BookModel
            {
                Title = title,
                Author = author,
                ReleaseDate = new DateTime(int.Parse(year!), 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            var added = await _bookRepository.AddBookAsync(book);
            Console.WriteLine($"Book{(added ? "" : " not")} added");
        }

        private async Task RemoveBook()
        {
            Console.Write("Id: ");
            var id = Console.ReadLine();
            var objectId = ObjectId.Parse(id);
            var removed = await _bookRepository.RemoveBookAsync(objectId);
            Console.WriteLine($"Book{(removed ? "" : " not")} removed");
        }

        private async Task GetBooks()
        {
            var books = await _bookRepository.GetBooksAsync();
            var booksStr = string.Join(Environment.NewLine, books);
            Console.WriteLine(booksStr);
        }

        private async Task AddBooks()
        {
            Console.Write("Author: ");
            var author = Console.ReadLine();
            Console.Write("Number of books: ");
            var num = int.Parse(Console.ReadLine()!);
            var books = new List<BookModel>();
            for (var i = 0; i < num; i++)
            {
                Console.WriteLine($"Adding book {i + 1}/{num}");
                Console.Write("Title: ");
                var title = Console.ReadLine();
                Console.Write("Year: ");
                var year = int.Parse(Console.ReadLine()!);
                books.Add(new BookModel
                {
                    Author = author,
                    Title = title,
                    ReleaseDate = new DateTime(year, 1, 1)
                });
            }

            var booksAdded = await _bookRepository.AddBooksAsync(books);
            Console.WriteLine($"Books {(booksAdded ? "" : "not ")} added");
        }

        private async Task GetBooksByAuthor()
        {
            Console.Write("Author: ");
            var author = Console.ReadLine();
            var books = await _bookRepository.GetBooksByAuthorAsync(author);
            Console.WriteLine(string.Join(", ", books));
        }
    }
}
