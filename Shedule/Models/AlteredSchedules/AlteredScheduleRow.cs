using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Shedule.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shedule.Models.AlteredSchedules
{
    public class AlteredScheduleRow : INotifyPropertyChanged
    {
        #region Общее
        private static List<AlteredScheduleRow> alteredScheduleRows = new List<AlteredScheduleRow>();
        public static List<Group> GroupsList { get; } = new List<Group>();
        public List<Group> AvailableGroupsList
        {
            get
            {
                return GroupsList.Except(UsedGroupsList.Where(u => u != SelectedGroup)).ToList();
            }
        }

        public static MyList<Group> UsedGroupsList { get; } = new MyList<Group>();
        public static List<Classroom> AllClassrooms { get; } = new List<Classroom>();
        public static List<MainSchedule> AllMainSchedules { get; } = new List<MainSchedule>();
        public static List<AlteredSchedule> AllAlteredSchedules { get; } = new List<AlteredSchedule>();

        public static event AlteredScheduleUpdateEventHandler OnAlteredSchedulesUpdated;
        #endregion

        #region Частное
        private Group selectedGroup;
        //private DateTime? selectedDate;
        private readonly DateTime currentDate;
        private readonly List<Curriculum> curriculums = new List<Curriculum>();
        private List<ExtendedTeaching> extendedTeachings = new List<ExtendedTeaching>();
        private readonly Dictionary<int,ClassAvailabilityInfoModel> classAvailabilityModels = new Dictionary<int, ClassAvailabilityInfoModel>();


        //Таск, который создается при установке новой группы
        //Пока он не окончен, нельзя работать с данными модели об учебных планах

        public Group SelectedGroup
        {
            get
            {
                return selectedGroup;
            }
            set
            {
                selectedGroup = value;
                if(value != null)
                {
                    Semester = selectedGroup.Semesters.First(s => s.StartDate <= currentDate && s.StartDate.AddDays(7 * s.WeeksCount) >= currentDate);
                }
                else
                {
                    Semester = null;
                    curriculums.Clear();
                    extendedTeachings.Clear();
                    classAvailabilityModels.Clear();
                }
                OnPropertyChanged("SelectedGroup");
            }
        }

        private Semester Semester { get; set; }
        private List<Curriculum> Curriculums
        {
            get
            {
                return curriculums;
            }
        }

        private List<ExtendedTeaching> ExtendedTeachings { 
            get 
            {
                return extendedTeachings;
            }
        }
        #endregion


        public AlteredScheduleRow(DateTime currentDate) : base()
        {
            alteredScheduleRows.Add(this);
            UsedGroupsList.OnChanged += updateAvailableGroups;
            this.currentDate = currentDate;
        }

        public static void clearEventsObservers()
        {
            OnAlteredSchedulesUpdated = null;
        }

        #region Загрузка и вычисление данных
        public async Task recalculateConfiguration()
        {
            await loadCurriulums();
            await loadTeachings();
        }

        private async Task loadCurriulums()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var curricula = await LearningProcessesAPI.getCurriculaForSemester(Semester.Id);
                curriculums.Clear();
                curriculums.AddRange(curricula);
            });
        }

        private async Task loadTeachings()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var teachings = await LearningProcessesAPI.getGroupTeachings(SelectedGroup.Id);
                //Отбираем только те, которые есть в текущих учебных планах
                teachings = teachings.Where(t => curriculums.Count(c => c.SpecialitySubjectId == t.SpecialitySubjectId) > 0).ToList();
                extendedTeachings.Clear();
                extendedTeachings.AddRange(teachings.Select(t =>
                {
                    var extend = ExtendedTeaching.fromTeaching(t);
                    var curriculum = curriculums.First(c => c.SpecialitySubjectId == t.SpecialitySubjectId);
                    extend.UsedHoursFact = curriculum.UsedHours; // const
                    extend.UsedHoursPlan = getUsedHoursPlan(t); // const (bcs mains does not change)
                    extend.AllocatedFromMainScheduleHoursPlan = getAllocatedHoursPlan(t); // const (bcs mains does not change)
                    extend.PlannedHours = curriculum.PlannedHours;
                    return extend;
                }));
            });
        }

        private int getUsedHoursPlan(Teaching teaching)
        {
            int count = 0;
            //До текущего дня (исключительно)
            for (DateTime date = Semester.StartDate; date < currentDate; date = date.AddDays(1))
            {
                bool isRedWeek = WeeksColoringUtils.getWeekColor(date) == WeeksColoringUtils.WeekColors.RED;
                int dayClasses = AllMainSchedules.Count(m =>
                m.TeachingId == teaching.Id
                && m.DayOfWeekId == WeeksColoringUtils.convertDayOfWeekToInt(date.DayOfWeek)
                && m.IsRedWeek == isRedWeek);
                count += dayClasses * 2;
            }
            return count;
        }

        private int getAllocatedHoursPlan(Teaching teaching)
        {
            int count = 0;
            //До последнего дня (включительно)
            for (DateTime date = Semester.StartDate; date <= Semester.StartDate.AddDays(7 * Semester.WeeksCount); date = date.AddDays(1))
            {
                bool isRedWeek = WeeksColoringUtils.getWeekColor(date) == WeeksColoringUtils.WeekColors.RED;
                int dayClasses = AllMainSchedules.Count(m =>
                m.TeachingId == teaching.Id
                && m.DayOfWeekId == WeeksColoringUtils.convertDayOfWeekToInt(date.DayOfWeek)
                && m.IsRedWeek == isRedWeek);
                count += dayClasses * 2;
            }
            return count;
        }
        #endregion

        #region Получение моделей
        public ClassAvailabilityInfoModel getClassModel(int classNumber)
        {
            if(SelectedGroup == null)
            {
                return null;
            }
            if (!classAvailabilityModels.ContainsKey(classNumber)) {
                var mainSch = AllMainSchedules.FirstOrDefault(m =>
                    m.SemesterId == Semester.Id
                    && m.IsRedWeek == (WeeksColoringUtils.getWeekColor(currentDate) == WeeksColoringUtils.WeekColors.RED)
                    && m.DayOfWeekId == WeeksColoringUtils.convertDayOfWeekToInt(currentDate.DayOfWeek)
                    && m.ClassNumber == classNumber);
                var curriculum = Curriculums.FirstOrDefault(c =>
                    c.SemesterId == Semester.Id
                    && c.SpecialitySubjectId == mainSch?.Teaching?.SpecialitySubjectId);
                var practiceCurriculum = Curriculums.FirstOrDefault(c =>
                    c.SemesterId == Semester.Id
                    && c.PracticeSchedule?.StartDate <= currentDate
                    && c.PracticeSchedule?.EndDate >= currentDate);
                var altered = AllAlteredSchedules.FirstOrDefault(a => 
                    a.MainScheduleId == mainSch?.Id
                    && a.Date == currentDate);
                var model = new ClassAvailabilityInfoModel(SelectedGroup, currentDate, mainSch, curriculum, practiceCurriculum, ref extendedTeachings);
                if(altered != null)
                {
                    model.AlteredSchedule = altered;
                }
                OnAlteredSchedulesUpdated += model.onAlteredSchedulesUpdated;
                classAvailabilityModels.Add(classNumber, model);
            }
            return classAvailabilityModels[classNumber];
        }
        #endregion

        #region Обновление Замен
        public static void createAlteredSchedule(ClassAvailabilityInfoModel model)
        {
            if(model.MainSchedule == null) {
                MessageBox.Show("Отсутствует основное расписание для выбранной пары!", "Ошибка наполнения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            AlteredSchedule alteredSchedule = new AlteredSchedule()
            {
                Date = model.Date,
                MainScheduleId = model.MainSchedule.Id,
                MainSchedule = model.MainSchedule
            };
            AllAlteredSchedules.Add(alteredSchedule);
            model.AlteredSchedule = alteredSchedule;
            //Обновляем все заинетерснованные компоненты
            OnAlteredSchedulesUpdated?.Invoke();
        }

        public static void updateAlteredSchedule()
        {
            //Обновляем все заинетерснованные компоненты
            OnAlteredSchedulesUpdated?.Invoke();
        }

        public static void deleteAlteredSchedule(ClassAvailabilityInfoModel model)
        {
            AllAlteredSchedules.RemoveAll(a => a.MainScheduleId == model.MainSchedule.Id);
            model.AlteredSchedule = null;
            //Обновляем все заинетерснованные компоненты
            OnAlteredSchedulesUpdated?.Invoke();
        }
        #endregion

        public void updateAvailableGroups()
        {
            alteredScheduleRows.ForEach(a => a.OnPropertyChanged("AvailableGroupsList"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public class MyList<T> : List<T>
        {
            public delegate void Notifier();
            public event Notifier OnChanged;

            public new void Add(T item)
            {
                base.Add(item);
                OnChanged?.Invoke();
            }

            public new void Remove(T item)
            {
                base.Remove(item);
                OnChanged?.Invoke();
            }

            public new void RemoveAt(int pos)
            {
                base.RemoveAt(pos);
                OnChanged?.Invoke();
            }
        }

    }
}
