using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LearningProcessesAPIClient.model;

namespace Shedule.Models
{
    public class CurriculumFillingInfoModel : Curriculum
    {
        private int allocatedHours = 0;
        private DateTime ?lastDay = null;

        public int AllocatedHours
        {
            get
            {
                return allocatedHours;
            }
            set
            {
                allocatedHours = value;
                OnPropertyChanged("AllocatedHours");
                OnPropertyChanged("AbsoluteNotAllocatedHours");
            }
        }

        public int AbsoluteNotAllocatedHours //TODO сделать относительную версию с UsedHours?
        {
            get
            {
                return PlannedHours - AllocatedHours;
            }
        }

        public DateTime ?LastDayFact {
            get {
                return lastDay;
            }
            set
            {
                lastDay = value;
                OnPropertyChanged("LastDay");
                OnPropertyChanged("LastDayFormatted");
            }
        }

        public string LastDayFactFormatted
        {
            get
            {
                return LastDayFact?.ToShortDateString() ?? "Не определено";
            }
        }


        public void setCurriculum(Curriculum curriculum)
        {

            Id = curriculum.Id;
            SpecialitySubjectId = curriculum.SpecialitySubjectId;
            PlannedHours = curriculum.PlannedHours;
            SemesterId = curriculum.SemesterId;
            UsedHours = curriculum.UsedHours;
            Semester = curriculum.Semester;
            SpecialitySubject = curriculum.SpecialitySubject;
            PracticeSchedule = curriculum.PracticeSchedule;

            AllocatedHours = 0;
            LastDayFact = null;

            OnPropertyChanged("Id");               
            OnPropertyChanged("SpecialitySubjectId");
            OnPropertyChanged("PlannedHours");
            OnPropertyChanged("SemesterId");
            OnPropertyChanged("UsedHours");
            OnPropertyChanged("Semester");
            OnPropertyChanged("SpecialitySubject");
            OnPropertyChanged("PracticeSchedule");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}