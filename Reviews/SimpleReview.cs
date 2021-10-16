using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek.Reviews
{
    public class SimpleReview : IReview
    {
        public int Overall { get; set; }
        public string Print()
        {
            return $"People say: {Overall}";
        }
    }
}
