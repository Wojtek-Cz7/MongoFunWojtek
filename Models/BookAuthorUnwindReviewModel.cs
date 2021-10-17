using MongoFunWojtek.Reviews;

namespace MongoFunWojtek.Models
{
	public class BookAuthorUnwindReviewModel
	{
		public string  Author  { get; set; }
		public IReview Reviews { get; set; }
	}
}