using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    public class BookModel
    {

        public const string DefaultAuthor = "Unknown";

        //[BsonId] 
        public string Idek { get; set; }



        //[BsonElement("title")] 
        public string Title { get; set; }

        //[BsonElement("author")]
        //[BsonIgnoreIfNull]
        //[BsonDefaultValue(DefaultAuthor)] // definiuje defaultową wartość
        //[BsonIgnoreIfDefault]
        public string Author { get; set; } = DefaultAuthor;



        //[BsonElement("releaseDate")] 
       // [BsonDateTimeOptions(DateOnly = true)]          
        public DateTime ReleaseDate { get; set; }

        public BookType Type { get; set; }

        public List<IReview> Reviews { get; set; } = new();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Idek} - {Title} - {Author} - {ReleaseDate.Year} - {Type}");
            foreach (var review in Reviews)
                sb.AppendLine($"\t{review.Print()}");

            return sb.ToString();
        }
    }


}
