using LearningProcessesAPIClient.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models.AlteredSchedules
{
    public class ClassroomAvailabilityInfo : AlteredSchedulesObserver, INotifyPropertyChanged
    {
        private List<Teaching> cachedTeachingsAtTheSameTime = new List<Teaching>();

        public Classroom Classroom { get; private set; }
        public Teaching Teaching { get; private set; }
        public int ClassNumber { get; private set; }
        public int DayOfWeekId { get; private set; }
        public bool IsRedWeek { get; private set; }
        public DateTime Date { get; private set; }

        public bool IsOwnedByTeacher
        {
            get
            {
                return Classroom.TeacherId == Teaching.TeacherId;
            }
        }

        public List<Teaching> TeachingsAtTheSameTime
        {
            get
            {
                return cachedTeachingsAtTheSameTime;
            }
        }

        //Вернет число от 0 до 3
        //0 и 1 считать показаниями к рекомендации
        public int RecommendationLevel
        {
            get
            {
                if (!IsOwnedByTeacher)
                {
                    if (TeachingsAtTheSameTime.Count == 0)
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
                    if (TeachingsAtTheSameTime.Count == 0)
                    {
                        return 0;
                    }
                    else if (TeachingsAtTheSameTime.Count == 1)
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ClassroomAvailabilityInfo(Classroom classroom, Teaching teaching, int classNumber, int dayOfWeekId, bool isRedWeek, DateTime date) : base()
        {
            Classroom = classroom;
            Teaching = teaching;
            ClassNumber = classNumber;
            DayOfWeekId = dayOfWeekId;
            IsRedWeek = isRedWeek;
            Date = date;
        }

        public override void onAlteredSchedulesUpdated()
        {
            updateCache();
            OnPropertyChanged("TeachingsAtTheSameTime");
            OnPropertyChanged("RecommendationLevel");
        }

        private void updateCache()
        {
            List<Teaching> classes = new List<Teaching>();
            AlteredScheduleRow.AllMainSchedules.Where(m =>
                m.ClassNumber == ClassNumber
                && m.DayOfWeekId == DayOfWeekId
                && m.IsRedWeek == IsRedWeek
            ).ToList().ForEach(m => {
                if (m.ClassroomId == Classroom.Id)
                {
                    if (AlteredScheduleRow.AllAlteredSchedules.Count(a => a.MainScheduleId == m.Id && a.NewClassroomId != m.ClassroomId && a.NewTeachingId != Teaching.Id && a.Date == Date) == 0)
                    {
                        classes.Add(m.Teaching);
                    }
                }
                else
                {
                    AlteredSchedule alteredSchedule;
                    if ((alteredSchedule = AlteredScheduleRow.AllAlteredSchedules.Find(a => a.MainScheduleId == m.Id && a.NewClassroomId == Classroom.Id && a.NewTeachingId != Teaching.Id && a.Date == Date)) != null)
                    {
                        classes.Add(alteredSchedule.Teaching);
                    }
                }
            });
            cachedTeachingsAtTheSameTime = classes;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
