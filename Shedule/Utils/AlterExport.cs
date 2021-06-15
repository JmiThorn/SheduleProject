using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Microsoft.Win32;
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
            range.Bold = 1;
            range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            range.InsertParagraphAfter();

            var result = alter.OrderBy(u => u.Teaching.GroupId);
            var count = alter.GroupBy(a => a.MainSchedule.SemesterId, a => a, (semesterId, alters) => new
            {
                SemesterId = semesterId,
                AlteredSchedules = alters
            }).Count();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table alterTable = document.Tables.Add(tableRange, count + 2, 7);
            alterTable.Borders.InsideLineStyle = alterTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

            Word.Range cellRange;

            object begCell = alterTable.Cell(1, 2).Range.Start;
            object endCell = alterTable.Cell(1, 7).Range.End;
           var wordcellrange = document.Range(ref begCell, ref endCell);
            wordcellrange.Select();
            application.Selection.Cells.Merge();
            wordcellrange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
 

            cellRange = alterTable.Cell(2, 1).Range;
            cellRange.Text = "";
            cellRange = alterTable.Cell(2, 2).Range;
            cellRange.Text = "1";
            cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            cellRange.ParagraphFormat.SpaceAfter = 0;
            cellRange = alterTable.Cell(2, 3).Range;
            cellRange.Text = "2";
            cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            cellRange.ParagraphFormat.SpaceAfter = 0;
            cellRange = alterTable.Cell(2, 4).Range;
            cellRange.Text = "3";
            cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            cellRange.ParagraphFormat.SpaceAfter = 0;
            cellRange = alterTable.Cell(2, 5).Range;
            cellRange.Text = "4";
            cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            cellRange.ParagraphFormat.SpaceAfter = 0;
            cellRange = alterTable.Cell(2, 6).Range;
            cellRange.Text = "5";
            cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            cellRange.ParagraphFormat.SpaceAfter = 0;
            cellRange = alterTable.Cell(2, 7).Range;
            cellRange.Text = "6";
            cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            cellRange.ParagraphFormat.SpaceAfter = 0;
            alterTable.Rows[1].Range.Bold = 1;
            alterTable.Rows[2].Range.Bold = 1;
            alterTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            alterTable.Rows[2].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            int groupIndex = 3;


            List<String> group = new List<string>();

            foreach (var alt in alter)
            {
                if(alt.Teaching != null)
                {
                group.Add(alt.Teaching.Group.Codename);
                }
                else
                {
                    continue;
                }
            }

            var distinct = group.Distinct();

            foreach (var alt in distinct)
            {
                cellRange = alterTable.Cell(groupIndex, 1).Range;
                cellRange.Text = alt;
                groupIndex++;
                cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cellRange.ParagraphFormat.SpaceAfter = 0;
                cellRange.Font.Bold = 1;
            }
            alterTable.Cell(1, 2).Range.Text = "Пары";
            alterTable.Cell(2, 1).Range.Text = "ГРУППЫ";
            alterTable.Cell(1, 2).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            alterTable.Cell(1, 2).Range.ParagraphFormat.SpaceAfter = 0;
            alterTable.Cell(2, 1).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            alterTable.Cell(2, 1).Range.ParagraphFormat.SpaceAfter = 8;
            alterTable.Cell(2, 1).Range.ParagraphFormat.SpaceBefore = 8;
            var color = WeeksColoringUtils.getWeekColor(date);
            if (color == WeeksColoringUtils.WeekColors.BLUE)
            {
                alterTable.Cell(2, 1).Range.Shading.ForegroundPatternColor =
                   Word.WdColor.wdColorLightBlue;
              alterTable.Cell(1,2).Range.Shading.ForegroundPatternColor = Word.WdColor.wdColorLightBlue;
            }
            else if (color == WeeksColoringUtils.WeekColors.RED)
            {
                alterTable.Cell(2, 1).Range.Shading.ForegroundPatternColor =
                  Word.WdColor.wdColorRed;
                alterTable.Cell(1, 2).Range.Shading.ForegroundPatternColor = Word.WdColor.wdColorLightBlue;
            }
            else
            {
                alterTable.Cell(2, 1).Range.Shading.ForegroundPatternColor =
                    Word.WdColor.wdColorGray05;
                alterTable.Cell(1, 2).Range.Shading.ForegroundPatternColor = Word.WdColor.wdColorLightBlue;
            }


          

            for (int i = 3; i <= alterTable.Range.Rows.Count; i++)
            {
                foreach (var alt in alter)
                {
                    if (alt.NewTeachingId != null)
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
                                            + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number + " ауд.";
                                    break;
                                case 1:
                                    cellRange = alterTable.Cell(i, 3).Range;
                                    cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                            + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                            + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number + " ауд.";
                                    break;
                                case 2:
                                    cellRange = alterTable.Cell(i, 4).Range;
                                    cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                            + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                            + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number + " ауд.";
                                    break;
                                case 3:
                                    cellRange = alterTable.Cell(i, 5).Range;
                                    cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                            + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                            + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number + " ауд.";
                                    break;
                                case 4:
                                    cellRange = alterTable.Cell(i, 6).Range;
                                    cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                            + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                            + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number + " ауд.";
                                    break;
                                case 5:
                                    cellRange = alterTable.Cell(i, 7).Range;
                                    cellRange.Text = alt.Teaching.SpecialitySubject.Subject.Name + " "
                                            + alt.Teaching.Teacher.Surname + " " + alt.Teaching.Teacher.Name[0] + "."
                                            + alt.Teaching.Teacher.Patronymic[0] + "." + " " + alt.Classroom.Number + " ауд.";
                                    break;
                            }
                        }
                        
                    }
                    else
                    {
                        if (alterTable.Cell(i, 1).Range.Text.Contains($"{alt.MainSchedule.Teaching.Group.Codename}"))
                        {
                            switch (alt.MainSchedule.ClassNumber)
                            {
                                case 0:
                                    cellRange = alterTable.Cell(i, 2).Range;
                                    cellRange.Text = "ОТМЕНА";
                                    cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                    cellRange.ParagraphFormat.SpaceAfter = 8;
                                    cellRange.ParagraphFormat.SpaceBefore = 8;
                                    break;
                                case 1:
                                    cellRange = alterTable.Cell(i, 3).Range;
                                    cellRange.Text = "ОТМЕНА";
                                    cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                    cellRange.ParagraphFormat.SpaceAfter = 8;
                                    cellRange.ParagraphFormat.SpaceBefore = 8;
                                    break;
                                case 2:
                                    cellRange = alterTable.Cell(i, 4).Range;
                                    cellRange.Text = "ОТМЕНА";
                                    cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                    cellRange.ParagraphFormat.SpaceAfter = 8;
                                    cellRange.ParagraphFormat.SpaceBefore = 8;
                                    break;
                                case 3:
                                    cellRange = alterTable.Cell(i, 5).Range;
                                    cellRange.Text = "ОТМЕНА";
                                    cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                    cellRange.ParagraphFormat.SpaceAfter = 8;
                                    cellRange.ParagraphFormat.SpaceBefore = 8;
                                    break;
                                case 4:
                                    cellRange = alterTable.Cell(i, 6).Range;
                                    cellRange.Text = "ОТМЕНА";
                                    cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                    cellRange.ParagraphFormat.SpaceAfter = 8;
                                    cellRange.ParagraphFormat.SpaceBefore = 8;
                                    break;
                                case 5:
                                    cellRange = alterTable.Cell(i, 7).Range;
                                    cellRange.Text = "ОТМЕНА";
                                    cellRange.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                    cellRange.ParagraphFormat.SpaceAfter = 8;
                                    cellRange.ParagraphFormat.SpaceBefore = 8;
                                    break;
                            }
                        }
                    }
                }
            }
            ////////////////////////////////////////////// - Сохранение файла
            //var ExportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //var path = new FileInfo(ExportPath + $"\\Изменения расписания на {date.ToShortDateString()}.docx");
            //int n = 1;
            //while (path.Exists)
            //{
            //    path = new FileInfo(ExportPath + "\\Изменения расписания на " + date.ToShortDateString() + "(" + n++.ToString() + ")" + ".docx");
            //}
          //  string path2 = path.ToString();
            //document.SaveAs2(path2);
            //document.Close();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Document Word files(*.docx)|*.docx";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.FileName = "Изменения на " + date.ToShortDateString();
            if (saveFileDialog.ShowDialog() == true)
            {

                var ExportPath = saveFileDialog.FileName;
                document.SaveAs2(ExportPath);
                document.Close();
            }
            }


    }
}
