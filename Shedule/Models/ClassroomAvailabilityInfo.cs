using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningProcessesAPIClient.model;

namespace Shedule.Models
{
    public class ClassroomAvailabilityInfo
    {
        public Classroom Classroom { get; set; }
        public bool IsRecommended { get; set; }
        public int GroupId { get; set; } //?
        public int TeacherId { get; set; }
        public int ClassNumber { get; set; }
        public int DayOfWeekId { get; set; }
        public bool IsRedWeek { get; set; }
        public DateTime SemesterStart { get; set; }
        public DateTime SemesterEnd { get; set; }
        public List<MainSchedule> MainSchedulesInTheSameClassroom { get; set; } = new List<MainSchedule>();

        //Вернет число от 0 до 3 (меньше - лучше)
        public int RecommendationLevel
        {
            get
            {
                if (!IsRecommended)
                {
                    if (MainSchedulesInTheSameClassroom.Count == 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    if (MainSchedulesInTheSameClassroom.Count == 0)
                    {
                        return 0;
                    }
                    else if (MainSchedulesInTheSameClassroom.Count == 1)
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
            }

            set
            {

            }
        }
    }
}
