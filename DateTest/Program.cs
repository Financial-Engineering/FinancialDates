using System;

using FinancialDates;

namespace DateTest
{
    class Program
    {
        static void Main()
        {
            var d1 = new DateTime(2012, 2, 29);
            var d2 = new DateTime(2012, 3, 29);

            DayCount dc = new Act365();
            var fract = dc.YearFraction(d1, d2);

            var c1 = new Calendar("GBP");

            var a = c1.IsBusinessDay(d1);
            var b = c1.IsBusinessDay(d2);

            string[] cals = { "USD", "GBP" };

            var c2 = new Calendar("USD", "GBP");
            var c3 = new Calendar(new []{ "USD", "GBP" });

            a = c2.IsBusinessDay(d1);
            b = c2.IsBusinessDay(d2);
          
            d1 = new DateTime(2011, 5, 28); // Saturday before Memorial Day
            var e1 = d1.ToExcel();

            var bd = new BusinessDate(d1, c2);
            BusinessDate bd3 = d1;

            var edt = bd.ToExcel();

            if (bd == new DateTime(2011, 5, 31))
                System.Console.WriteLine("Dates are equal");

            if (new DateTime(2011, 5, 31) == bd)
                System.Console.WriteLine("Dates are equal");

            var b1 = new BusinessDate(2011, 09, 29, c3, BusinessConvention.MODFOLLOWING);
            var b2 = new BusinessDate(2012, 09, 29, c3, BusinessConvention.MODFOLLOWING);

            var d3 = d2.Find(3, System.DayOfWeek.Wednesday);
            var d4 = d2.Next(System.DayOfWeek.Monday);

            var s1 = new Schedule(b1, b2, Frequency.SEMIANNUAL);

            b1 = new BusinessDate(2011, 10, 15, c3, BusinessConvention.MODFOLLOWING);
            b2 = new BusinessDate(2012, 09, 29, c3, BusinessConvention.MODFOLLOWING);

            var s2 = new Schedule(b1, b2, Frequency.SEMIANNUAL, RollConvention.NONE, Stub.SHORT_FIRST);
        }
    }
}
