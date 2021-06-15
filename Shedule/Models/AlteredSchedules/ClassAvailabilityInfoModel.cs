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
    public class ClassAvailabilityInfoModel : AlteredSchedulesObserver, INotifyPropertyChanged
    {
        private AlteredSchedule alteredSchedule = null;
        private readonly Curriculum practiceCurriculum;
        private readonly List<ExtendedTeaching> extendedTeachings;
        private bool cacheIsSubjectOver = false;

        private Group Group { get; set; }
        public DateTime Date { get; private set; }

        public MainSchedule MainSchedule { get; private set; }
        public Curriculum MainCurriculum { get; private set; }
        public AlteredSchedule AlteredSchedule
        {
            get => alteredSchedule;
            set
            {
                alteredSchedule = value;
                OnPropertyChanged("AlteredSchedule");
                //OnPropertyChanged("DailyHoursAreOk"); обновляются через onAlteredSchedulesUpdated
            }
        }


        #region Важнейшие условия
        public bool IsPracticeAtTheSameTime { get => practiceCurriculum != null; }

        //Может быть для одной и той же пары в один день разным, т.е. может быть две пары, и последней будет первая пара, а вторая уже должна заменяться
        public bool IsSubjectOver { 
            get => cacheIsSubjectOver;
        }

        //Рекомендуем не перегружать учеников
        public bool DailyHoursAreOk { 
            get
            {
                if (MainSchedule == null)
                    return true;
                var relatedMainSchedules = AlteredScheduleRow.AllMainSchedules.Where(m =>
                    m.SemesterId == MainSchedule.SemesterId
                    && m.DayOfWeekId == MainSchedule.DayOfWeekId
                    && m.IsRedWeek == MainSchedule.IsRedWeek
                    && AlteredScheduleRow.AllAlteredSchedules.Count(a => a.MainScheduleId == m.Id && a.NewTeachingId == null) == 0);
                var alteredSchedules = AlteredScheduleRow.AllAlteredSchedules.Where(a => a.Date == Date && a.MainSchedule.SemesterId == MainSchedule.SemesterId && a.NewTeachingId != null);
                int alteredHours = alteredSchedules.Count(a => a.NewTeachingId != null && a.MainSchedule.TeachingId == null) * 2;
                int mainHours = relatedMainSchedules.Count(r => r.TeachingId != null) * 2;

                return mainHours+alteredHours <= Group.Speciality.MaxDailyHours;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public ClassAvailabilityInfoModel(Group group, DateTime date, MainSchedule mainSchedule, Curriculum mainCurriculum, Curriculum practiceCurriculum, ref List<ExtendedTeaching> extendedTeachings) : base()
        {
            Group = group;
            Date = date;
            MainSchedule = mainSchedule;
            MainCurriculum = mainCurriculum;
            this.practiceCurriculum = practiceCurriculum;
            this.extendedTeachings = extendedTeachings;
            onAlteredSchedulesUpdated();
        }

        private void updateCache()
        {
            if (MainSchedule == null || MainCurriculum == null)
            {
                cacheIsSubjectOver =  false;
                return;
            }

            var prevMains = AlteredScheduleRow.AllMainSchedules.Where(m =>
                m.SemesterId == MainSchedule.SemesterId
                && m.DayOfWeekId == MainSchedule.DayOfWeekId
                && m.IsRedWeek == MainSchedule.IsRedWeek
                && m.ClassNumber < MainSchedule.ClassNumber
                && m.TeachingId != null
                && m.TeachingId == MainSchedule.TeachingId);
            var prevAlters = AlteredScheduleRow.AllAlteredSchedules.Where(a =>
                a.Date == Date
                && a.MainSchedule.ClassNumber < MainSchedule.ClassNumber
                && a.MainSchedule.SemesterId == MainSchedule.SemesterId);
            int todayPrevHours = 0;
            prevAlters.ToList().ForEach(a =>
            {
                if (a.NewTeachingId != null
                && a.NewTeachingId == MainSchedule.TeachingId
                && a.MainSchedule.TeachingId != MainSchedule.TeachingId)
                    todayPrevHours++;
                if (a.NewTeachingId == null
                && a.MainSchedule.TeachingId == MainSchedule.TeachingId)
                    todayPrevHours--;
            });
            todayPrevHours += prevMains.Count();
            todayPrevHours *= 2;
            cacheIsSubjectOver =  MainCurriculum.UsedHours + todayPrevHours >= MainCurriculum.PlannedHours;
        }

        public List<TeachingAvailabilityInfo> getTeachingModels()
        {
            var list = new List<TeachingAvailabilityInfo>();
            if (MainSchedule != null)
                extendedTeachings.ForEach(e => {
                    int dayOfWeekId = WeeksColoringUtils.convertDayOfWeekToInt(Date.DayOfWeek);
                    bool isRedWeek = WeeksColoringUtils.getWeekColor(Date) == WeeksColoringUtils.WeekColors.RED;
                    var model = new TeachingAvailabilityInfo(e, Date, MainSchedule.ClassNumber, dayOfWeekId, isRedWeek);
                    AlteredScheduleRow.OnAlteredSchedulesUpdated += model.onAlteredSchedulesUpdated;
                    list.Add(model);
                });
            return list;
        }

        public override void onAlteredSchedulesUpdated()
        {
            updateCache();  
            OnPropertyChanged("AlteredSchedule");
            OnPropertyChanged("DailyHoursAreOk");
            OnPropertyChanged("IsSubjectOver");
            OnPropertyChanged("FeaturedTeachings");
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        
    }
}
