using System;

namespace FinancialDates
{
    // Extend existing DateTime class to include excel date conversion
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Returns a DateTime from an Excel Julian date
        /// </summary>
        /// <param name="dt">The current date</param>
        /// <param name="excelDate">The current date</param>
        /// <returns></returns>
        public static DateTime FromExcel(this DateTime dt, int excelDate)
        {
            if (excelDate < 1)
                throw new ArgumentException("Excel dates cannot be smaller than 0.");

            excelDate = (excelDate > 60) ? excelDate - 2 : excelDate - 1;

            return new DateTime(1900, 1, 1).AddDays(excelDate);
        }

        /// <summary>
        ///     Returns an Excel Julian date from a DateTime
        /// </summary>
        /// <param name="dt">The current date</param>
        /// <returns></returns>
        public static int ToExcel(this DateTime dt)
        {
            var serialDate = (dt - new DateTime(1899, 12, 30)).Days;
            return serialDate < 60 ? serialDate - 1 : serialDate;
        }

        /// <summary>
        ///     Returns a DateTime representing the first day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime First(this DateTime current)
        {
            var first = current.AddDays(1 - current.Day);
            return first;
        }

        /// <summary>
        ///     Returns a DateTime representing the first specified day in the current month
        /// </summary>
        /// <param name="dt">The current day</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime First(this DateTime dt, DayOfWeek dayOfWeek)
        {
            var first = dt.First();

            if (first.DayOfWeek != dayOfWeek)
            {
                first = first.Next(dayOfWeek);
            }

            return first;
        }

        /// <summary>
        ///     Returns a DateTime representing the last day in the current month
        /// </summary>
        /// <param name="dt">The current date</param>
        /// <returns></returns>
        public static DateTime Last(this DateTime dt)
        {
            var daysInMonth = DateTime.DaysInMonth(dt.Year, dt.Month);

            var last = dt.First().AddDays(daysInMonth - 1);
            return last;
        }

        /// <summary>
        ///     Returns a DateTime representing the last specified day in the current month
        /// </summary>
        /// <param name="dt">The current date</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime Last(this DateTime dt, DayOfWeek dayOfWeek)
        {
            DateTime last = dt.Last();

            last = last.AddDays(Math.Abs(dayOfWeek - last.DayOfWeek)*-1);
            return last;
        }

        /// <summary>
        ///     Returns a DateTime representing the first date following the current date which falls on the given day of the week
        /// </summary>
        /// <param name="dt">The current date</param>
        /// <param name="dayOfWeek">The day of week</param>
        public static DateTime Next(this DateTime dt, DayOfWeek dayOfWeek)
        {
            var offsetDays = dayOfWeek - dt.DayOfWeek;

            if (offsetDays <= 0)
            {
                offsetDays += 7;
            }

            var result = dt.AddDays(offsetDays);
            return result;
        }

        /// <summary>
        ///     Returns a DateTime representing the Nth day of the week in the month
        /// </summary>
        /// <param name="dt">The current date</param>
        /// <param name="num">N'th day</param>
        /// <param name="dayOfWeek">The day of week</param>
        public static DateTime Find(this DateTime dt, int num, DayOfWeek dayOfWeek)
        {
            if (num < 1 || num > 4)
                throw new ArgumentException("Day of week index must be in the range 1...4");

            var tempDate = new DateTime(dt.Year, dt.Month, 1);

            for (var i = 0; i < num; i++)
                tempDate = tempDate.Next(dayOfWeek);

            return tempDate;
        }
    }
}