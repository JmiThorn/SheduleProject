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
    public class AlteredScheduleLiveModel : AlteredSchedule
    {

        private int? newTeachingId;
        private int? newClassroomId;

        public new int? NewTeachingId
        {
            get
            {
                return newTeachingId;
            }
            set
            {
                newTeachingId = value;
                OnPropertyChanged("NewTeachingId");
            }
        }

        public new int? NewClassroomId
        {
            get
            {
                return newClassroomId;
            }
            set
            {
                newClassroomId = value;
                OnPropertyChanged("NewClassroomId");
            }
        }

        public void setMainSchedule(AlteredSchedule alteredSchedule)
        {
            MainScheduleId = alteredSchedule.MainScheduleId;
            Classroom = alteredSchedule.Classroom;
            Date = alteredSchedule.Date;
            Event = alteredSchedule.Event;
            EventId = alteredSchedule.EventId;
            Id = alteredSchedule.Id;
            MainSchedule = alteredSchedule.MainSchedule;
            NewClassroomId = alteredSchedule.MainScheduleId;
            NewTeachingId = alteredSchedule.NewTeachingId;
            Teaching = alteredSchedule.Teaching;

            OnPropertyChanged("MainScheduleId");
            OnPropertyChanged("Classroom");
            OnPropertyChanged("Date");
            OnPropertyChanged("Event");
            OnPropertyChanged("EventId");
            OnPropertyChanged("Id");
            OnPropertyChanged("MainSchedule");
            OnPropertyChanged("NewClassroomId");
            OnPropertyChanged("NewTeachingId");
            OnPropertyChanged("Teaching");
            OnPropertyChanged("Semester");
            OnPropertyChanged("Teaching");
        }

        public AlteredSchedule geAlteredSchedule()
        {
            return new AlteredSchedule()
            {
                MainScheduleId = this.MainScheduleId,
                Classroom = this.Classroom,
                Date = this.Date,
                Event = this.Event,
                EventId = this.EventId,
                Id = this.Id,
                MainSchedule = this.MainSchedule,
                NewClassroomId = this.NewClassroomId,
                NewTeachingId = this.NewTeachingId,
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
