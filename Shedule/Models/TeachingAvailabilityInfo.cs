using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningProcessesAPIClient.model;

namespace Shedule.Models
{
    public class TeachingAvailabilityInfo
    {
        public Teaching Teaching { get; set; }
        public int GroupId { get; set; }
        public int TeacherId { get; set; }
        public int DayOfWeekId { get; set; }
        public int ClassNumber { get; set; }
        public bool IsRedWeek { get; set; }
        public DateTime SemesterStart { get; set; }
        public DateTime SemesterEnd { get; set; }
        public List<MainSchedule> MainSchedulesInTheSameTime { get; set; } = new List<MainSchedule>();

        //Вернет число от 0 (меньше - лучше)
        //TODO учитывать отчитанные потенциально/фактически часы?
        public int RecommendationLevel
        {
            get
            {
                return MainSchedulesInTheSameTime.Count;
            }

            set
            {

            }
        }
    }
}
