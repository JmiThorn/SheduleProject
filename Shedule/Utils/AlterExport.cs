using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;


namespace Shedule.Utils
{
    class AlterExport
    {

        public static async Task ExportAlterShedule(List<AlteredSchedule> alter, DateTime date)
        {
            var application = new Word.Application();
            Word.Document document = application.Documents.Add();

            Word.Paragraph paragraph = document.Paragraphs.Add();
            Word.Range range = paragraph.Range;
            range.Text = "Изменения расписания на " + date.ToShortDateString();
            //paragraph.set_Style("Title");
            range.InsertParagraphAfter();

            var result = alter.OrderBy(u => u.Teaching.GroupId);
            var count = alter.GroupBy(a => a.MainSchedule.SemesterId, a => a, (semesterId, alters) => new
            {
                SemesterId = semesterId,
                AlteredSchedules = alters
            }).Count();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table alterTable = document.Tables.Add(tableRange, count + 1, 7);
            //alterTable.Range.Rows.Add
            alterTable.Borders.InsideLineStyle = alterTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

            Word.Range cellRange;
            Word.Range cellGroup;

            cellRange = alterTable.Cell(1, 1).Range;
            cellRange.Text = "";
            cellRange = alterTable.Cell(1, 2).Range;
            cellRange.Text = "1";
            cellRange = alterTable.Cell(1, 3).Range;
            cellRange.Text = "2";
            cellRange = alterTable.Cell(1, 4).Range;
            cellRange.Text = "3";
            cellRange = alterTable.Cell(1, 5).Range;
            cellRange.Text = "4";
            cellRange = alterTable.Cell(1, 6).Range;
            cellRange.Text = "5";
            cellRange = alterTable.Cell(1, 7).Range;
            cellRange.Text = "6";

            alterTable.Rows[1].Range.Bold = 1;
            alterTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;


            int indexStart = 0;
            int indexEnd = 1;
            int groupIndex = 2;
            foreach (var alt in alter)
            {
                cellRange = alterTable.Cell(groupIndex, 1).Range;
                cellRange.Text = alt.Teaching.Group.Codename;
                groupIndex++;
            }

            for (int i = 2; i <= alterTable.Range.Rows.Count; i++)
            {
                foreach (var alt in alter)
                {
                    if (alt.Teaching.SpecialitySubject.Subject == null)
                    {
                        alt.Teaching.SpecialitySubject.Subject = await LearningProcessesAPI.getSubject(alt.Teaching.SpecialitySubject.SubjectId);
                    }
                    if (alterTable.Cell(i, 1).Range.Text.Contains($"{alt.Teaching.Group.Codename}"))
                    {
                        switch (alt.MainSchedule.ClassNumber)
                        {
                            case 0:
                                cellRange = alterTable.Cell(i, 2).Range;
                                cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                        + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                        + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
                                break;
                            case 1:
                                cellRange = alterTable.Cell(i, 3).Range;
                                cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                        + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                        + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
                                break;
                            case 2:
                                cellRange = alterTable.Cell(i, 4).Range;
                                cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                        + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                        + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
                                break;
                            case 3:
                                cellRange = alterTable.Cell(i, 5).Range;
                                cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                        + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                        + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
                                break;
                            case 4:
                                cellRange = alterTable.Cell(i, 6).Range;
                                cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                        + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                        + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
                                break;
                            case 5:
                                cellRange = alterTable.Cell(i, 7).Range;
                                cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                        + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                        + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
                                break;
                        }
                    }
                }
            }

            //    foreach (var alt in alter)
            //    {
            //    if (alt.Teaching.SpecialitySubject.Subject == null)
            //    {
            //        alt.Teaching.SpecialitySubject.Subject = await LearningProcessesAPI.getSubject(alt.Teaching.SpecialitySubject.SubjectId);
            //    }

            //    switch (alt.MainSchedule.ClassNumber)
            //    {
            //        case 0:
            //            cellRange = alterTable.Cell(2, 2).Range;
            //            cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
            //                    + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
            //                    + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
            //            break;
            //        case 1:
            //            cellRange = alterTable.Cell(2, 3).Range;
            //            cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
            //                    + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
            //                    + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
            //            break;
            //        case 2:
            //            cellRange = alterTable.Cell(2, 4).Range;
            //            cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
            //                    + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
            //                    + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
            //            break;
            //        case 3:
            //            cellRange = alterTable.Cell(2, 5).Range;
            //            cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
            //                    + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
            //                    + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
            //            break;
            //        case 4:
            //            cellRange = alterTable.Cell(2, 6).Range;
            //            cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
            //                    + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
            //                    + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
            //            break;
            //        case 5:
            //            cellRange = alterTable.Cell(2, 7).Range;
            //            cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
            //                    + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
            //                    + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number;
            //            break;
            //    }
            //    //if()
            //}
            ////////////////////////////////////////////// - Сохранение файла
            var ExportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = new FileInfo(ExportPath + $"\\Изменения расписания на {date.ToShortDateString()}.docx");
            int n = 1;
            while (path.Exists)
            {
                path = new FileInfo(ExportPath + "\\Изменения расписания на " + date.ToShortDateString() + "(" + n++.ToString() + ")" + ".docx");
            }
            string path2 = path.ToString();
            document.SaveAs2(path2);
            document.Close();
        }


    }
}
