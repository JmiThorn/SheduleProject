using LearningProcessesAPIClient.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models
{
    public class TeachingModel : Teaching
    {
        //Основа
        public Teaching Teaching { get; set; }

        public TeachingModel(Teaching teaching)
        {
            Id = teaching.Id;
            SpecialitySubjectId = teaching.SpecialitySubjectId;
            GroupId = teaching.GroupId;
            TeacherId = teaching.TeacherId;
            AlteredSchedules = teaching.AlteredSchedules;
            Group = teaching.Group;
            MainSchedules = teaching.MainSchedules;
            SpecialitySubject = teaching.SpecialitySubject;
            Teacher = teaching.Teacher;
            Teaching = teaching;
        }

        public string FullNameSubjectTeacher
        {
            get
            {
                string name = null;
                string surname = null;
                string patronymic = null;
                string sbjName = null;
                if (Teacher != null)
                {
                    name = Teacher.Name.Substring(0, 1).ToUpper();
                    surname = Teacher.Surname;
                    patronymic = Teacher.Patronymic.Substring(0, 1)?.ToUpper();
                }
                if (SpecialitySubject != null)
                {
                    sbjName = SpecialitySubject.SubjectIndex;
                    if (SpecialitySubject.Subject != null)
                    {
                        sbjName = SpecialitySubject.Subject.Name;
                    }
                }
                //return sbjName +" - "+ surname +" "+ name +" "+ patronymic;
                return $"{sbjName ?? "Незаданный предмет"} - " + (name == null ? "Незаданный преподаватель" : $"{surname} {name}.{patronymic}.");
            }
        }
    }
}
