using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek.Reviews
{
    class ExpertReview : IReview
    {
        public ExpertReview()
        {
        }

        public ExpertReview(int overall, string additionalWord)
        {
            Overall = overall;
            AdditionalWord = additionalWord;
        }


        public int Overall { get; set; }
        public string AdditionalWord { get; set; }
        public string Print()
        {
            return $"Expert says: {Overall} - {AdditionalWord}";
        }
    }
}
