using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    public static class BookModelSetup
    {
        public static void Setup()
        {
            BsonClassMap.RegisterClassMap<BookModel>(map =>
            {
                map.MapIdProperty(x => x.Idek)
                .SetSerializer(new StringSerializer(BsonType.ObjectId))
                .SetIdGenerator(StringObjectIdGenerator.Instance);

                map.MapProperty(x => x.Title).SetElementName("title");

                map.MapProperty(x => x.Author)
                .SetElementName("author")
                .SetDefaultValue(BookModel.DefaultAuthor)
                .SetIgnoreIfDefault(true);

                map.MapProperty(x => x.ReleaseDate)
                    .SetElementName("releaseDate");
                //.SetSerializer(new DateTimeSerializer(true));

                map.MapProperty(x => x.Type)
                .SetElementName("type")
                .SetSerializer(new EnumSerializer<BookType>(BsonType.String));
            }
            );
        }
    }
}
