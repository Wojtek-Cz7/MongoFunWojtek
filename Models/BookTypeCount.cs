namespace MongoFunWojtek.Models
{
	public class BookTypeCount
	{
		public BookType Type  { get; set; }
		public long     Count { get; set; }

		public override string ToString() => $"{Type} - {Count}";
	}
}