using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shedule.Utils
{
    public class SheduleExport
    {
        public static async Task Export(List<MainSchedule> shedule, DateTime startDate)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var ExportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // MessageBox.Show();
            var file = new FileInfo(ExportPath + "\\Экспорт Расписания.xlsx");
            int n = 1;
            while (file.Exists)
            {
                file = new FileInfo(ExportPath + "\\Экспорт Расписания" + "(" + n++.ToString() + ")" + ".xlsx");
            }
            using (var package = new ExcelPackage(file))
            {
                DateTime ToNow = DateTime.Today;
                var sheet = package.Workbook.Worksheets.Add("Расписание на "+startDate.ToShortDateString());
                var result = shedule.OrderBy(u => u.SemesterId);
                int redIndex = 30;
                int blueIndex = 30;
                int groupRedIndex = 1;
                int groupBlueIndex = 1;
                foreach (var m in result)
                {
                    if (m.Semester.Group == null)
                    {
                        m.Semester.Group = await LearningProcessesAPI.getGroup(m.Semester.GroupId);
                    }
                    if (m.IsRedWeek == true && m.Semester.StartDate == startDate)
                    {

                        if (redIndex == 30)
                        {
                            sheet.Cells[$"A{groupRedIndex}"].Value = m.Semester.Group.Codename;
                            sheet.Cells[$"B{groupRedIndex}"].Value = m.Semester.Number;
                            sheet.Cells[$"C{groupRedIndex}"].Value = "Красная";
                            sheet.Cells[$"A{groupRedIndex + 1}"].Value = "Пн";
                            sheet.Cells[$"B{groupRedIndex + 1}"].Value = "Вт";
                            sheet.Cells[$"C{groupRedIndex + 1}"].Value = "Ср";
                            sheet.Cells[$"D{groupRedIndex + 1}"].Value = "Чт";
                            sheet.Cells[$"E{groupRedIndex + 1}"].Value = "Пт";
                            redIndex = 0;
                        }

                        switch (m.DaysOfWeek.Name)
                        {
                            case "Понедельник":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"A{groupRedIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;

                                    redIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"A{groupRedIndex + m.ClassNumber + 2}"].Value = " ";
                                    redIndex++;
                                }
                                break;
                            case "Вторник":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"B{groupRedIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;
                                    redIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"B{groupRedIndex + m.ClassNumber + 2}"].Value = " ";
                                    redIndex++;
                                }
                                break;
                            case "Среда":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"C{groupRedIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;
                                    redIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"C{groupRedIndex + m.ClassNumber + 2}"].Value = " ";
                                    redIndex++;
                                }
                                break;
                            case "Четверг":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"D{groupRedIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;
                                    redIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"D{groupRedIndex + m.ClassNumber + 2}"].Value = " ";
                                    redIndex++;
                                }
                                break;
                            case "Пятница":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"E{groupRedIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;
                                    redIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"E{groupRedIndex + m.ClassNumber + 2}"].Value = " ";
                                    redIndex++;
                                }
                                break;

                        }
                        if (redIndex == 30)
                        {
                            groupRedIndex += 8;
                        }
                    }
                    if (m.IsRedWeek == false && m.Semester.StartDate == startDate)
                    {
                        if (blueIndex == 30)
                        {
                            sheet.Cells[$"G{groupBlueIndex}"].Value = m.Semester.Group.Codename;
                            sheet.Cells[$"H{groupBlueIndex}"].Value = m.Semester.Number;
                            if (!m.IsRedWeek)
                            {
                                sheet.Cells[$"I{groupBlueIndex}"].Value = "Синяя";
                            }
                            sheet.Cells[$"G{groupBlueIndex + 1}"].Value = "Пн";
                            sheet.Cells[$"H{groupBlueIndex + 1}"].Value = "Вт";
                            sheet.Cells[$"I{groupBlueIndex + 1}"].Value = "Ср";
                            sheet.Cells[$"J{groupBlueIndex + 1}"].Value = "Чт";
                            sheet.Cells[$"K{groupBlueIndex + 1}"].Value = "Пт";
                            blueIndex = 0;
                        }

                        switch (m.DaysOfWeek.Name)
                        {
                            case "Понедельник":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"G{groupBlueIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;

                                    blueIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"G{groupBlueIndex + m.ClassNumber + 2}"].Value = " ";
                                    blueIndex++;
                                }
                                break;
                            case "Вторник":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"H{groupBlueIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;
                                    blueIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"H{groupBlueIndex + m.ClassNumber + 2}"].Value = " ";
                                    blueIndex++;
                                }
                                break;
                            case "Среда":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"I{groupBlueIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;
                                    blueIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"I{groupBlueIndex + m.ClassNumber + 2}"].Value = " ";
                                    blueIndex++;
                                }
                                break;
                            case "Четверг":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"J{groupBlueIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;
                                    blueIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"J{groupBlueIndex + m.ClassNumber + 2}"].Value = " ";
                                    blueIndex++;
                                }
                                break;
                            case "Пятница":
                                if (m.Teaching != null)
                                {
                                    sheet.Cells[$"K{groupBlueIndex + m.ClassNumber + 2}"].Value = m.Teaching.SpecialitySubject.Subject.Name + " "
                                        + m.Teaching.Teacher.Surname + " " + m.Teaching.Teacher.Name[0] + "."
                                        + m.Teaching.Teacher.Patronymic[0] + "." + " " + m.Classroom.Number;
                                    blueIndex++;
                                }
                                else
                                {
                                    sheet.Cells[$"K{groupBlueIndex + m.ClassNumber + 2}"].Value = " ";
                                    blueIndex++;
                                }
                                break;

                        }
                        if (blueIndex == 30)
                        {
                            groupBlueIndex += 8;
                        }
                    }
                };
                package.Save();

            }
        }

    }

}
