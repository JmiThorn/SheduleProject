using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model.parsing;
using OfficeOpenXml;
using Shedule.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shedule.Models.Parsing
{
    public class ParsingUtils
    {
        public enum TABLE_TYPE
        {
            TABLE_TYPE_OLD,
            TABLE_TYPE_NEW,
            TABLE_TYPE_UNKNOWN
        }

        private static String alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static void ParseFile(string fileName, int Id)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(fileName))) //РУКАМИ ПРИВОДИТЕ К НОВОМУ ФОРМАТУ
            {
                TABLE_TYPE type;
                ExcelWorksheet worksheet = getWorksheet(package, out type);
                GroupModel model = null;
                switch (type)
                {
                    case TABLE_TYPE.TABLE_TYPE_OLD:
                        model = parseOldTable(worksheet);
                        break;
                    case TABLE_TYPE.TABLE_TYPE_NEW:
                        model = parseNewTable(worksheet);
                        break;
                    case TABLE_TYPE.TABLE_TYPE_UNKNOWN:
                        Console.WriteLine("Таблица не найдена");
                        return;
                }

                if (model != null)
                {
                    List<ParsedSubject> parsedSubjects = new List<ParsedSubject>();
                    foreach (int semesterNum in model.semesters.Keys)
                    {
                        SemesterModel semester = model.semesters[semesterNum];
                        foreach (SubjectModel subject in semester.subjectHours.Keys)
                        {
                            parsedSubjects.Add(
                                new ParsedSubject()
                                {
                                    Semester = semesterNum,
                                    SemesterWeeks = semester.weeks,
                                    Name = subject.name,
                                    Index = subject.index,
                                    GroupId = Id,
                                    IsPractise = subject.isPractise,
                                    SemesterStart = DateTime.MinValue, //TODO просить пользователя заполнить их в разделе семестров
                                    PractiseStart = null, //TODO просить пользователя заполнить их в разделе курриколумов
                                    PractiseEnd = null, //TODO просить пользователя заполнить их в разделе курриколумов
                                    Hours = semester.subjectHours[subject]
                                }
                            );
                        }
                    }
                    AppUtils.ProcessClientLibraryRequest(async () =>
                    {
                        LearningProcessesAPI.addParsedSubjects(parsedSubjects);
                    });
                }
            }

        }

       
        private static GroupModel parseNewTable(ExcelWorksheet worksheet)
        {
            GroupModel group = new GroupModel();
            //Курсы начинаются с J
            int aIndex = alphabet.IndexOf("J");
            //Все предметы
            Dictionary<int, SubjectModel> subjects = getSubjects(worksheet);
            //Флаг конца
            bool isLastCourse = false;
            //Идем по курсам
            for (char symb = alphabet[aIndex]; aIndex < alphabet.Length; aIndex++, symb = alphabet[aIndex])
            {
                ExcelRange courseCell = worksheet.Cells[$"{symb}3"];
                ExcelRange semesterCell = worksheet.Cells[$"{symb}4"];
                ExcelRange weeksCell = worksheet.Cells[$"{symb}5"];
                ExcelRange range;
                //Скорее всего, это объединеная в название курса ячейка, но может и не объединенная
                if (courseCell.Value != null)
                {
                    group.totalCourses++;
                    //Если у нас ячейка названия курса не объеденена, то мы на столбце последнего учитываемого семестра
                    if (!courseCell.Merge)
                        isLastCourse = true;
                }

                //Семестры закончились
                if (semesterCell.Value == null && courseCell.Value == null)
                    break;
                //Еслои есть номер семестра, то и кол-во недель на ячейку ниже присутствует
                if (semesterCell.Value != null)
                {
                    //Что-то в духе "2 сем. 23 нед." или "7 сем. 30 нед. (20 нед. АЗ)"
                    Regex regex = new Regex(@"\w*(?<numeric>[0-9]{1,}).*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    //Смогли идентифицировать семестр
                    if (regex.IsMatch(semesterCell.Value.ToString()))
                    {
                        Match matchSemester = regex.Match(semesterCell.Value.ToString());
                        //Смогли идентифицировать число недель. точно наш клиент
                        if (regex.IsMatch(weeksCell.Value.ToString()))
                        {
                            Match matchWeeks = regex.Match(weeksCell.Value.ToString());
                            SemesterModel semester = new SemesterModel();
                            semester.course = group.totalCourses;
                            semester.weeks = Convert.ToInt32(matchWeeks.Groups["numeric"].Value);
                            group.semesters[Convert.ToInt32(matchSemester.Groups["numeric"].Value)] = semester;
                            semester.subjectHours = scanHours(worksheet, symb, semester, subjects);
                        }
                    }
                }
                if (isLastCourse)
                    break;
            }

            return group;
        }

        private static GroupModel parseOldTable(ExcelWorksheet worksheet)
        {
            GroupModel group = new GroupModel();
            //Курсы начинаются с J
            int aIndex = alphabet.IndexOf("J");
            //Все предметы
            Dictionary<int, SubjectModel> subjects = getSubjects(worksheet);
            //Идем по курсам
            for (char symb = alphabet[aIndex]; aIndex < alphabet.Length; aIndex++, symb = alphabet[aIndex])
            {
                ExcelRange range;
                //Скорее всего, это объединеная в название курса ячейка, но может и не объединенная
                if (worksheet.Cells[$"{symb}4"].Value != null)
                    group.totalCourses++;
                //Семестры закончились
                if (worksheet.Cells[$"{symb}5"].Value == null && worksheet.Cells[$"{symb}4"].Value == null)
                    break;
                if ((range = worksheet.Cells[$"{symb}5"]).Value != null)
                {
                    //Что-то в духе "2 сем. 23 нед." или "7 сем. 30 нед. (20 нед. АЗ)"
                    Regex regex = new Regex(@"\w*(?<semeseter>[0-9]{1,})[^1-9]+(?<weeks>[0-9]{1,}).*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    //Смогли идентифицировать семестр
                    if (regex.IsMatch(range.Value.ToString()))
                    {
                        Match match = regex.Match(range.Value.ToString());
                        SemesterModel semester = new SemesterModel();
                        semester.course = group.totalCourses;
                        semester.weeks = Convert.ToInt32(match.Groups["weeks"].Value);
                        group.semesters[Convert.ToInt32(match.Groups["semeseter"].Value)] = semester;
                        semester.subjectHours = scanHours(worksheet, symb, semester, subjects);
                    }
                }
            }

            return group;
        }
        //<номер строки, предмет>
        private static Dictionary<int, SubjectModel> getSubjects(ExcelWorksheet worksheet)
        {
            Dictionary<int, SubjectModel> subjects = new Dictionary<int, SubjectModel>();
            int row = 3;
            while (true)
            {
                row++;
                ExcelRange indexCell = worksheet.Cells[$"A{row}"];
                ExcelRange nameCell = worksheet.Cells[$"B{row}"];
                //Тут есть много вариантов в зависимости от типа таблицы
                if (indexCell.Value == null)
                {
                    //Конец необходимой к парсингу инфы означается полностью пустой строкой
                    //Также мы предполагаем, что эти таблицы достаточно длинные, и весь их контент занимает более 10 строк
                    if (indexCell.Value == null && nameCell.Value == null && row > 10)
                    {
                        break;
                    }
                    //АА если н\она не полностью пуста, то значит еще есть шанс отрыть инфы
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    //Строка с номерами столбцов
                    if (indexCell.Value.ToString() == "1")
                        continue;
                    //Конец необходимой к парсингу инфы означается словом всего или пустым названием дисциплины
                    if ((indexCell.Value.ToString().ToLower() == "всего" && nameCell.Value == null))
                        break;
                }
                //Сто пудов какая-то категория, НО БУДЬ АККУРАТЕН И НИКОМУ НЕ ВЕРЬ
                //ДОБАВЬ КУДА-НИБУДЬ ДАБЛ ЧЕК ИЛИ ГАЛОЧКУ О БОЖЕ ОНИ ТЕБЯ ОБМАНУТ
                if (indexCell.Style.Font.Bold)
                    continue;
                //Хммм, ну тут имени нет, маркера конца нет, сто пудов не предмет
                if (nameCell.Value == null)
                    continue;
                int column = alphabet.IndexOf("A");
                bool streak = true;
                //8 семестр у нас в Q столбце
                for (char symb = alphabet[column]; column < alphabet.IndexOf("Q"); column++, symb = alphabet[column])
                {
                    //Строки категорий имеют суммы по всем столбцам, в то время как норм предметы имеют значения только там, где надо
                    if (worksheet.Cells[$"{symb}{row}"].Value == null)
                    {
                        streak = false;
                        break;
                    }
                }
                //Это заголовок, что пудов (ну, с вероятностью в 99% точно, мы не встречали предметов на все 8 семестров)
                if (streak)
                    continue;
                //Сохраняем чистые от лишних пробелов значения
                String name = nameCell.Value.ToString();
                bool isPractise = false;
                //Определяем практики и приводим их к унифицированному виду
                if (name.ToLower().Contains("практика"))
                {
                    if (name.ToLower().Contains("учебная"))
                    {
                        name = "Учебная практика";
                        isPractise = true;
                    }
                    else if (name.ToLower().Contains("производственная"))
                    {
                        name = "Производственная практика";
                        isPractise = true;
                    }
                }
                Regex regex = new Regex("\\s{2,}");
                var matchesEnum = regex.Matches(name).GetEnumerator();
                while (matchesEnum.MoveNext())
                {
                    name = name.Replace((matchesEnum.Current as Match).Value, " ");
                }
                SubjectModel subject = new SubjectModel
                {
                    index = indexCell.Value.ToString().Trim(),
                    name = name.Trim(),
                    isPractise = isPractise
                };
                subjects[row] = subject;
            }
            return subjects;
        }

        //<предмет, часы>
        private static Dictionary<SubjectModel, int> scanHours(ExcelWorksheet worksheet, char column, SemesterModel semester, Dictionary<int, SubjectModel> subjects)
        {
            Dictionary<SubjectModel, int> result = new Dictionary<SubjectModel, int>();
            //Перебираем все известные строки
            foreach (int row in subjects.Keys)
            {
                ExcelRange hoursCell = worksheet.Cells[$"{column}{row}"];
                //Пустые нас не интересуют, а тк на входе только подтвержденные - лишние данны ене пройдут    
                if (hoursCell.Value == null)
                    continue;
                int hours = Convert.ToInt32(hoursCell.Value.ToString());
                result[subjects[row]] = hours;
            }
            return result;
        }

        private static ExcelWorksheet getWorksheet(ExcelPackage package, out TABLE_TYPE type)
        {
            type = TABLE_TYPE.TABLE_TYPE_UNKNOWN;

            Dictionary<TABLE_TYPE, String> sheetMarkers = new Dictionary<TABLE_TYPE, string>();
            sheetMarkers.Add(TABLE_TYPE.TABLE_TYPE_OLD, "план учебного процесса");
            sheetMarkers.Add(TABLE_TYPE.TABLE_TYPE_NEW, "сводные данные по бюджету времени");
            foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
            {
                //Для тбч только тбч (2)
                if (worksheet.Name.Contains("тбч") && !worksheet.Name.Contains("(2)"))
                    continue;
                //Console.WriteLine(worksheet.Name); OK TESTED
                //TODO Ищем заголовок листа
                foreach (char symb in alphabet.ToCharArray())
                {
                    //TODO границы в переменные
                    for (int row = 1; row <= 2; row++)
                    {
                        //Бывают пустые строки
                        if (worksheet.Cells[symb + "" + row].Value == null)
                            continue;
                        String value = worksheet.Cells[symb + "" + row].Value.ToString().ToLower();

                        //Ищем ключевую фразу
                        foreach (TABLE_TYPE tableType in sheetMarkers.Keys)
                        {
                            String marker = sheetMarkers[tableType];
                            //такая строка может быть только в тбч
                            //За хардкод сорян, придумай что получше
                            if (marker.ToLower().Equals("сводные данные по бюджету времени") && !worksheet.Name.Contains("тбч"))
                                continue;
                            //Нашли листок
                            if (value.Contains(marker.ToLower()))
                            {
                                //Console.WriteLine($"_{tableType}_+{fileName}+ [{worksheet.Name}] {value}: {symb}{row}");
                                type = tableType;
                                return worksheet;
                            }
                        }
                    }
                }
            }
            return null;
        }

    }
}
