using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoFunWojtek.Reviews;
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
			var allConventionPack = new ConventionPack
			{
				new CamelCaseElementNameConvention(),
				new StringIdStoredAsObjectIdConvention()
			};
			ConventionRegistry.Register("All", allConventionPack, t => true);

			var reviewType = typeof(IReview);
			var reviewsConventionPack = new ConventionPack
			{
				new ReviewDiscriminatorConvention()
			};
			ConventionRegistry.Register("Review", reviewsConventionPack,
				type => type.GetInterfaces().Contains(reviewType));

			BsonClassMap.RegisterClassMap<BookModel>(map =>
			{
				map.MapIdMember(x => x.Idek);
				map.AutoMap();
				map.GetMemberMap(x => x.Author)
					.SetSerializer(new AuthorStringSerializer())
					.SetDefaultValue(BookModel.DefaultAuthor)
					.SetIgnoreIfDefault(true);

				map.GetMemberMap(x => x.ReleaseDate);
					//.SetSerializer(new DateTimeSerializer(true));

				map.GetMemberMap(x => x.Type)
					.SetSerializer(new EnumSerializer<BookType>(BsonType.String));
			});

			BsonSerializer.RegisterDiscriminatorConvention(typeof(IReview), StandardDiscriminatorConvention.Scalar);
			BsonClassMap.RegisterClassMap<SimpleReview>();
			BsonClassMap.RegisterClassMap<ExpertReview>();
			BsonClassMap.RegisterClassMap<GradeReview>(map =>
			{
				map.AutoMap();
				map.GetMemberMap(x => x.Grade)
					.SetSerializer(new EnumSerializer<Grade>(BsonType.String));
			});
		}
	}
}
