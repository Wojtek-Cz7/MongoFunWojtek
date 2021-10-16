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

        public override string ToString() => $"{Idek.ToString()} - {Title} - {Author} - {ReleaseDate.Year}";
    }


}
