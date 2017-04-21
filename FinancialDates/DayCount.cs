using System;
using System.Runtime.InteropServices;

// Add a comment

namespace FinancialDates
{
    public abstract class DayCount
    {
        public virtual int Days(DateTime a, DateTime b)
        {
            TimeSpan span = b - a;
            return span.Days;
        }

        public DateTime LastDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        }

        public virtual double YearFraction(DateTime a, DateTime b)
        {
            return 0.0;
        }
    }


    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class Act360 : DayCount
    {
        public override double YearFraction(DateTime a, DateTime b)
        {
            return Days(a, b)/360.0;
        }
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class Act365 : DayCount
    {
        public override double YearFraction(DateTime a, DateTime b)
        {
            return Days(a, b)/365.0;
        }
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class Act252 : DayCount
    {
        public override double YearFraction(DateTime a, DateTime b)
        {
            return Days(a, b)/252.0;
        }
    }

    public abstract class Thirty360 : DayCount
    {
        public new virtual int days(DateTime a, DateTime b)
        {
            return 360*(b.Year - a.Year) + 30*(b.Month - a.Month) + (b.Day - a.Day);
        }

        public override double YearFraction(DateTime a, DateTime b)
        {
            return days(a, b)/360.0;
        }
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class US30360 : Thirty360
    {
        public override int days(DateTime a, DateTime b)
        {
            var d1 = a.Day;
            var d2 = b.Day;

            if ((a.Month == 2) && (a.Day >= 27) && (b.Day >= 27))
                d2 = 30;
            if ((a.Month == 2) && (a.Day >= 27))
                d1 = 30;
            if ((d2 == 31) && (d1 >= 30))
                d2 = 30;
            if (d1 == 31)
                d1 = 30;

            return base.days(new DateTime(a.Year, a.Month, d1),
                new DateTime(b.Year, b.Month, d2));
        }
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class ISMA30E360 : Thirty360
    {
        public override int days(DateTime a, DateTime b)
        {
            var d1 = a.Day;
            var d2 = b.Day;

            if (d1 == 31)
                d1 = 30;

            if (d2 == 31)
                d2 = 30;

            return base.days(new DateTime(a.Year, a.Month, d1),
                new DateTime(b.Year, b.Month, d2));
        }
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class ISDA30E360 : Thirty360
    {
        public override int days(DateTime a, DateTime b)
        {
            var d1 = a.Day;
            var d2 = b.Day;

            if (a == LastDayOfMonth(a))
                d1 = 30;

            if (a == LastDayOfMonth(a))
                d2 = 30;

            return base.days(new DateTime(a.Year, a.Month, d1),
                new DateTime(b.Year, b.Month, d2));
        }
    }
}