using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek.Reviews
{
    class GradeReview : IReview
    {


        public GradeReview()
        {
        }

        public GradeReview(Grade grade)
        {
            Grade = grade;
        }




        public Grade Grade { get; set; }
        public string Print()
        {
            return $"GRADE: {Grade}";
        }
    }
}
