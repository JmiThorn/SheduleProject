using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models.Parsing
{
    class SubjectModel
    {
        public String index;
        public String name;
        public bool isPractise;

        public override String ToString()
        {
            return index + " " + name;
        }
    }
}
