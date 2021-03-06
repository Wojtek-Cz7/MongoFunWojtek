using System;

namespace MongoFunWojtek.Models
{
	public class BookCountByDateStart
	{
		public long     Count     { get; set; }
		public DateTime DateStart { get; set; }

		public override string ToString() => $"{DateStart} - {Count}";
	}
}