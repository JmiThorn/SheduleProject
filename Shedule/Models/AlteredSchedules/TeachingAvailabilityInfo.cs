using LearningProcessesAPIClient.model;
using Shedule.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models.AlteredSchedules
{
    public class TeachingAvailabilityInfo : AlteredSchedulesObserver, INotifyPropertyChanged
    {
        private List<Tuple<Classroom, Teaching>> cachedTeacherParallels = new List<Tuple<Classroom, Teaching>>();
        private int cachedTodayExtraHours = 0;
        public ExtendedTeaching ExtendedTeaching { get; private set; }

        public int ClassNumber { get; private set; }
        public int DayOfWeekId { get; private set; }
        public bool IsRedWeek { get; private set; }
        public DateTime Date { get; private set; }

        public List<Tuple<Classroom, Teaching>> TeacherParallels { 
            get 
            {
                return cachedTeacherParallels;
            } 
        }

        public bool TeacherIsFree { 
            get
            {
                return TeacherParallels.Count == 0;
            } 
        }

        //Положительные значения - предмет не вместился в семестр,
        //отрицательные - предмет будет образовывать окна в конце семестра,
        //ноль - предмет идельно вписан в семестр
        public int RelativeNotAllocatedHoursFact
        {
            get
            {
                return ExtendedTeaching.PlannedHours - (ExtendedTeaching.AllocatedHoursFact + cachedTodayExtraHours);
            }
        }

        //Вернет число от 0
        //Чем меньше - тем выше уровень рекомендации
        //Числа 0 и 1 считать показанием к замене
        public int RecommendationLevel
        {
            get
            {
                if(TeacherIsFree && RelativeNotAllocatedHoursFact > 0)
                {
                    return 0;
                }else if(!TeacherIsFree && RelativeNotAllocatedHoursFact > 0)
                {
                    return TeacherParallels.Count;
                }else
                {
                    return 10;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TeachingAvailabilityInfo(ExtendedTeaching extendedTeaching,DateTime date, int classNumber, int dayOfWeekId, bool isRedWeek) : base()
        {
            ExtendedTeaching = extendedTeaching;
            Date = date;
            ClassNumber = classNumber;
            DayOfWeekId = dayOfWeekId;
            IsRedWeek = isRedWeek;
        }

        private void updateCaches()
        {
            updateCachedTeacherParallels();
            updateCachedTodayPrevHours();
        }

        private void updateCachedTodayPrevHours()
        {
            var alters = AlteredScheduleRow.AllAlteredSchedules.Where(a =>
                a.Date == Date
                && (a.MainSchedule.TeachingId == ExtendedTeaching.Id
                    || a.NewTeachingId == ExtendedTeaching.Id));
            cachedTodayExtraHours = 0;
            alters.ToList().ForEach(a =>
            {
                if (a.NewTeachingId == ExtendedTeaching.Id
                && a.MainSchedule.TeachingId != ExtendedTeaching.Id)
                    cachedTodayExtraHours++;
                if (a.NewTeachingId != ExtendedTeaching.Id
                && a.MainSchedule.TeachingId == ExtendedTeaching.Id)
                    cachedTodayExtraHours--;
            });
            cachedTodayExtraHours *= 2;
        }

        private void updateCachedTeacherParallels()
        {
            List<Tuple<Classroom, Teaching>> parallels = new List<Tuple<Classroom, Teaching>>();
            AlteredScheduleRow.AllMainSchedules.Where(m =>
                m.ClassNumber == ClassNumber
                && m.DayOfWeekId == DayOfWeekId
                && m.IsRedWeek == IsRedWeek
            ).ToList().ForEach(m => {
                AlteredSchedule alteredSchedule = AlteredScheduleRow.AllAlteredSchedules.FirstOrDefault(a => a.MainScheduleId == m.Id);
                if(alteredSchedule == null)
                {
                    if(m.Teaching?.TeacherId == ExtendedTeaching.TeacherId && m.TeachingId != ExtendedTeaching.Id)
                    {
                        parallels.Add(new Tuple<Classroom, Teaching>(m.Classroom, m.Teaching));
                    }
                }
                else
                {
                    if(alteredSchedule.Teaching?.TeacherId == ExtendedTeaching.TeacherId && alteredSchedule.NewTeachingId != ExtendedTeaching.Id)
                    {
                        parallels.Add(new Tuple<Classroom, Teaching>(alteredSchedule.Classroom, alteredSchedule.Teaching));
                    }
                }
            });
            cachedTeacherParallels = parallels;
        }

        public List<ClassroomAvailabilityInfo> getClassroomModels()
        {
            var list = new List<ClassroomAvailabilityInfo>();
            AlteredScheduleRow.AllClassrooms.ForEach(c => {
                int dayOfWeekId = WeeksColoringUtils.convertDayOfWeekToInt(Date.DayOfWeek);
                bool isRedWeek = WeeksColoringUtils.getWeekColor(Date) == WeeksColoringUtils.WeekColors.RED;
                var model = new ClassroomAvailabilityInfo(c, ExtendedTeaching, ClassNumber, DayOfWeekId, IsRedWeek, Date);
                AlteredScheduleRow.OnAlteredSchedulesUpdated += model.onAlteredSchedulesUpdated;
                list.Add(model);
            });
            return list;
        }

        public override void onAlteredSchedulesUpdated()
        {
            updateCaches();
            OnPropertyChanged("TeacherParallels");
            OnPropertyChanged("TeacherIsFree");
            OnPropertyChanged("RecommendationLevel");
            OnPropertyChanged("RelativeNotAllocatedHoursFact");
            OnPropertyChanged("ExtendedTeaching"); //TODO проверить необходимость
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
