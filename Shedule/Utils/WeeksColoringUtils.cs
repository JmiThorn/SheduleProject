using LearningProcessesAPIClient.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Utils
{
    public class WeeksColoringUtils
    {
        public enum WeekColors
        {
            RED,
            BLUE,
            NOT_DETERMINED //За пределами учебного года (зимние или летние каникулы)
        }

        //Длительность зимних каникул
        public const int H2_DAYS_OFFSET = 10;

        //Цвет первой недели В УЧЕБНОМ ГОДУ
        public static WeekColors FIRST_COLOR = WeekColors.RED;

        //0 - первая неделя, 1 - вторая неделя и т.п.
        public static int getCurrentWeekNumberFromStart(Semester semester)
        {
            return getWeekNumberFromStart(semester, DateTime.Now);
        }

        //0 - первая неделя, 1 - вторая неделя и т.п.
        //TODO перерпроверить корректность
        public static int getWeekNumberFromStart(Semester semester, DateTime comparedDate)
        {
            TimeSpan diff = comparedDate.Subtract(semester.StartDate);
            return (int)Math.Floor((double)diff.Days / 7.0);
        }

        public static int getRedWeeksCount(Semester semester)
        {
            WeekColors startColor = getWeekColor(semester.StartDate);
            int firstTypeWeeks;
            int secondTypeWeeks;
            countSemesterWeeks(semester,out firstTypeWeeks,out secondTypeWeeks);
            if (startColor == WeekColors.RED)
                return firstTypeWeeks;
            else
                return secondTypeWeeks;
        }

        public static int getBlueWeeksCount(Semester semester)
        {
            WeekColors startColor = getWeekColor(semester.StartDate);
            int firstTypeWeeks;
            int secondTypeWeeks;
            countSemesterWeeks(semester, out firstTypeWeeks, out secondTypeWeeks);
            if (startColor == WeekColors.BLUE)
                return firstTypeWeeks;
            else
                return secondTypeWeeks;
        }

        private static void countSemesterWeeks(Semester semester, out int firstTypeWeeks, out int secondTypeWeeks)
        {
            firstTypeWeeks = (int)Math.Ceiling((double)semester.WeeksCount / 2);
            secondTypeWeeks = semester.WeeksCount - firstTypeWeeks;
        }

        public static WeekColors getWeekColor(DateTime date)
        {
            //Летние каникулы
            if (date.Month == 7 || date.Month == 8)
                return WeekColors.NOT_DETERMINED;
            //Сентябрьские выходные
            if (date.Month == 9 && date < getFirstWorkDayH1(date.Year))
                return WeekColors.NOT_DETERMINED;
            //Зимние каникулы
            if (date.Month == 1 && date < getFirstWorkDayH2(date.Year))
                return WeekColors.NOT_DETERMINED;

            //Разница относительно первого учебного ПОНЕДЕЛЬНИКА в учебном году
            TimeSpan dateDiff;
            if (date.Month < 9)
            {
                dateDiff = date.Subtract(getFirstWorkDayH1(date.Year-1))
                    //Каникулы не учитываются при смене цвета недели
                    //Tакже предполагается, что учеба идет до 31 декабря
                    .Subtract(TimeSpan.FromDays(H2_DAYS_OFFSET));
                if(getFirstWorkDayH1(date.Year - 1).DayOfWeek != DayOfWeek.Monday)
                {
                    dateDiff = dateDiff.Add(
                        TimeSpan.FromDays(
                            getDaysOfWeekDiff(
                                getFirstWorkDayH1(date.Year - 1).DayOfWeek, DayOfWeek.Monday
                    )));
                }
            }
            else
            {
                dateDiff = date.Subtract(getFirstWorkDayH1(date.Year));
                if (getFirstWorkDayH1(date.Year).DayOfWeek != DayOfWeek.Monday)
                {
                    dateDiff = dateDiff.Add(
                        TimeSpan.FromDays(
                            getDaysOfWeekDiff(
                                getFirstWorkDayH1(date.Year).DayOfWeek, DayOfWeek.Monday
                    )));
                }
            }

            //Четное значение - значит неделя красная
            if(Math.Floor((double)dateDiff.Days/7.0) % 2 == 0)
            {
                return WeekColors.RED;
            }
            else
            {
                return WeekColors.BLUE;
            }

        }

        //Для первого полугодия
        public static DateTime getFirstWorkDayH1(int year)
        {
            DateTime date = new DateTime(year, 9, 1); //sept 1 of year
            while(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }
            return date;
        }

        //Для второго полугодия
        public static DateTime getFirstWorkDayH2(int year)
        {
            DateTime date = new DateTime(year, 1, H2_DAYS_OFFSET); //jan H2_DAYS_OFFSET of year
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }
            return date;
        }

        //разница в днях между днями недели
        public static int getDaysOfWeekDiff(DayOfWeek furtherDay, DayOfWeek previousDay)
        {
            return convertDayOfWeekToInt(furtherDay) - convertDayOfWeekToInt(previousDay);
        }

        //Вернет от 1 до 7 в соответствии с днями недели
        private static int convertDayOfWeekToInt(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Sunday)
                return 7;
            else
            {
                return (int)dayOfWeek;
            }
        }

    }
}
