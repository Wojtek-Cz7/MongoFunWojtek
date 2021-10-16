using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    public class BookModel
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime ReleaseDate { get; set; }

        public override string ToString() => $"{Id.ToString()} - {Title} - {Author} - {ReleaseDate.Year}";
    }

    
}
