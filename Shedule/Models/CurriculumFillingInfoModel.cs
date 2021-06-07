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

        public new int AllocatedHours
        {
            get
            {
                return allocatedHours;
            }
            set
            {
                allocatedHours = value;
                OnPropertyChanged("AllocatedHours");
            }
        }

        //public new int NotAllocatedHours //TODO заменить на вычитание AllocatedHours из PlannedHours
        //{
        //    get
        //    {
        //        return allocatedHours;
        //    }
        //    set
        //    {
        //        allocatedHours = value;
        //        OnPropertyChanged("AllocatedHours");
        //    }
        //}


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