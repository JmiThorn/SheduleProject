using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models.Parsing
{
    class GroupModel
    {
        public int totalCourses = 0;
        public Dictionary<int, SemesterModel> semesters = new Dictionary<int, SemesterModel>();
    }
}
