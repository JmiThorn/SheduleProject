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
    
    public class ExtendedTeaching : Teaching, INotifyPropertyChanged
    {
        private int allocatedFromMainScheduleHoursPlan = 0;
        private int usedHoursFact = 0;
        private int usedHoursPlan = 0;
        private int plannedHours = 0;
        //private int allocatedFromCurrentAlteredScheduleHours = 0;

        public int AllocatedFromMainScheduleHoursPlan
        {
            get
            {
                return allocatedFromMainScheduleHoursPlan;
            }
            set
            {
                allocatedFromMainScheduleHoursPlan = value;
                OnPropertyChanged("AllocatedFromMainScheduleHoursPlan");
                OnPropertyChanged("AllocatedHoursFact");
                OnPropertyChanged("NotAllocatedHoursFact");
            }
        }

        //public int AllocatedFromCurrentAlteredScheduleHours
        //{
        //    get
        //    {
        //        return allocatedFromCurrentAlteredScheduleHours;
        //    }
        //    set
        //    {
        //        allocatedFromCurrentAlteredScheduleHours = value;
        //        OnPropertyChanged("AllocatedFromCurrentAlteredScheduleHours");
        //        OnPropertyChanged("AllocatedHoursFact");
        //        OnPropertyChanged("NotAllocatedHoursFact");
        //    }
        //}

        //базовое значение на учебный день назад
        public int AllocatedHoursFact
        {
            get
            {
                //AllocatedFromMainScheduleHoursPlan + diff(used fact - used plan) + AllocatedFromCurrentAlteredScheduleHours
                //return AllocatedFromMainScheduleHoursPlan + (UsedHoursFact - UsedHoursPlan) + AllocatedFromCurrentAlteredScheduleHours;
                return AllocatedFromMainScheduleHoursPlan + (UsedHoursFact - UsedHoursPlan);
            }
        }

        public int UsedHoursFact
        {
            get
            {
                return usedHoursFact;
            }
            set
            {
                usedHoursFact = value;
                OnPropertyChanged("UsedHoursFact");
                OnPropertyChanged("AllocatedHoursFact");
                OnPropertyChanged("NotAllocatedHoursFact");
            }
        }

        public int UsedHoursPlan
        {
            get
            {
                return usedHoursPlan;
            }
            set
            {
                usedHoursPlan = value;
                OnPropertyChanged("UsedHoursPlan");
                OnPropertyChanged("AllocatedHoursFact");
            }
        }

        public int PlannedHours
        {
            get
            {
                return plannedHours;
            }
            set
            {
                plannedHours = value;
                OnPropertyChanged("PlannedHours");
                OnPropertyChanged("NotAllocatedHoursFact");
            }
        }

        public int NotAllocatedHoursFact
        {
            get
            {
                return PlannedHours - AllocatedHoursFact;
            }
        }

        //Предотвращаем потенциально некорректное создание
        private ExtendedTeaching() : base()
        {

        }

        public static ExtendedTeaching fromTeaching(Teaching teaching)
        {
            return new ExtendedTeaching()
            {
                AlteredSchedules = teaching.AlteredSchedules,
                Group = teaching.Group,
                GroupId = teaching.GroupId,
                Id = teaching.Id,
                MainSchedules = teaching.MainSchedules,
                SpecialitySubject = teaching.SpecialitySubject,
                SpecialitySubjectId = teaching.SpecialitySubjectId,
                Teacher = teaching.Teacher,
                TeacherId = teaching.TeacherId
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

}
