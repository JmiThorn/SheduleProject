using LearningProcessesAPIClient.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models
{
    public class MainScheduleLiveModel : MainSchedule, INotifyPropertyChanged
    {
        private int? teachingId;
        private int? classroomId;

        public new int? TeachingId
        {
            get
            {
                return teachingId;
            }
            set
            {
                teachingId = value;
                OnPropertyChanged("TeachingId");
            }
        }

        public new int? ClassroomId
        {
            get
            {
                return classroomId;
            }
            set
            {
                classroomId = value;
                OnPropertyChanged("ClassroomId");
            }
        }

        public void setMainSchedule(MainSchedule mainSchedule)
        {
            Id = mainSchedule.Id;
            SemesterId = mainSchedule.SemesterId;
            DayOfWeekId = mainSchedule.DayOfWeekId;
            TeachingId = mainSchedule.TeachingId;
            ClassNumber = mainSchedule.ClassNumber;
            IsRedWeek = mainSchedule.IsRedWeek;
            ClassroomId = mainSchedule.ClassroomId;
            AlteredSchedules = mainSchedule.AlteredSchedules;
            Classroom = mainSchedule.Classroom;
            DaysOfWeek = mainSchedule.DaysOfWeek;
            Semester = mainSchedule.Semester;
            Teaching = mainSchedule.Teaching;
            OnPropertyChanged("Id");
            OnPropertyChanged("SemesterId");
            OnPropertyChanged("DayOfWeekId");
            OnPropertyChanged("TeachingId");
            OnPropertyChanged("ClassNumber");
            OnPropertyChanged("IsRedWeek");
            OnPropertyChanged("ClassroomId");
            OnPropertyChanged("AlteredSchedules");
            OnPropertyChanged("Classroom");
            OnPropertyChanged("DaysOfWeek");
            OnPropertyChanged("Semester");
            OnPropertyChanged("Teaching");
        }

        public MainSchedule getMainSchedule()
        {
            return new MainSchedule()
            {
                Id = this.Id,
                SemesterId = this.SemesterId,
                DayOfWeekId = this.DayOfWeekId,
                TeachingId = this.TeachingId,
                ClassNumber = this.ClassNumber,
                IsRedWeek = this.IsRedWeek,
                ClassroomId = this.ClassroomId,
                AlteredSchedules = this.AlteredSchedules,
                Classroom = this.Classroom,
                DaysOfWeek = this.DaysOfWeek,
                Semester = this.Semester,
                Teaching = this.Teaching
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