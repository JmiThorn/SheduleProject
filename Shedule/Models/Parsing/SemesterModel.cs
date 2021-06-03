using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models.Parsing
{
    class SemesterModel
    {
        public int course;
        public int weeks;
        //<предмет, часы>
        public Dictionary<SubjectModel, int> subjectHours = new Dictionary<SubjectModel, int>();
    }
}
