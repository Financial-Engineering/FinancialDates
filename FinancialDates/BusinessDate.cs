using System;
using System.Runtime.InteropServices;

namespace FinancialDates
{
    // Business date roll behaviours 
    public enum BusinessConvention
    {
        FOLLOWING,
        FRN,
        MODFOLLOWING,
        PRECEDING,
        MODPRECEDING,
        NONE
    }

    // Summary:
    //     Represents a business date with 0..n holiday calendars
    // 
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class BusinessDate : IComparable, IComparable<BusinessDate>, IEquatable<BusinessDate>
    {
        //private Calendar cal;
        private DateTime _date;

        public BusinessDate()
        {
            Calendar = new Calendar();
            _date = new DateTime();
            Convention = BusinessConvention.NONE;
        }

        public BusinessDate(BusinessDate bd)
        {
            Calendar = bd.Calendar;
            _date = bd._date;
            Convention = bd.Convention;
        }

        public BusinessDate(DateTime dt)
        {
            Create(dt, new Calendar());
        }

        public BusinessDate(DateTime dt, Calendar cal, BusinessConvention conv = BusinessConvention.NONE)
        {
            Create(dt, cal, conv);
        }

        public BusinessDate(DateTime dt, string cal, BusinessConvention conv = BusinessConvention.NONE)
        {
            Create(dt, cal, conv);
        }

        public BusinessDate(int year, int month, int day, Calendar cal,
            BusinessConvention conv = BusinessConvention.NONE)
        {
            Create(new DateTime(year, month, day), cal, conv);
        }

        public BusinessDate(int year, int month, int day, string cal, BusinessConvention conv = BusinessConvention.NONE)
        {
            Create(new DateTime(year, month, day), cal, conv);
        }

        public BusinessDate(DateTime dt, params string[] cals)
        {
            Create(dt, new Calendar(cals));
        }

        public Calendar Calendar { get; set; }
        public BusinessConvention Convention { get; set; }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                _date = AdjustDate().Date;
            }
        }

        [ComVisible(false)]
        public int CompareTo(object value)
        {
            return _date.CompareTo(value);
        }

        public int CompareTo(BusinessDate value)
        {
            return _date.CompareTo(value);
        }

        public bool Equals(BusinessDate value)
        {
            return _date.Equals(value);
        }

        public static implicit operator DateTime(BusinessDate bd)
        {
            return bd.Date;
        }

        public static implicit operator BusinessDate(DateTime dt)
        {
            return new BusinessDate(dt);
        }

        public void Create(DateTime dt, Calendar cal = null, BusinessConvention conv = BusinessConvention.NONE)
        {
            Calendar = cal;
            _date = dt;
            Convention = conv;

            _date = AdjustDate().Date;
        }

        public void Create(DateTime dt, string cal, BusinessConvention conv = BusinessConvention.NONE)
        {
            Create(dt, new Calendar(cal), conv);
        }

        private BusinessDate AddBusinessDays(int incr)
        {
            var ndt = new BusinessDate(this);

            while (!Calendar.IsBusinessDay(ndt.Date))
                ndt._date = ndt._date.AddDays(incr);

            return ndt;
        }

        public BusinessDate AdjustDate()
        {
            var ndt = new BusinessDate(this);

            if (!Calendar.IsBusinessDay(ndt.Date))
            {
                switch (Convention)
                {
                    case BusinessConvention.FOLLOWING:
                        ndt = AddBusinessDays(1);
                        break;
                    case BusinessConvention.MODFOLLOWING:
                        ndt = AddBusinessDays(1);
                        if (ndt.Date.Month != Date.Month)
                        {
                            goto case BusinessConvention.PRECEDING;
                        }
                        break;
                    case BusinessConvention.PRECEDING:
                        ndt = AddBusinessDays(-1);
                        break;
                    case BusinessConvention.MODPRECEDING:
                        ndt = AddBusinessDays(-1);
                        if (ndt.Date.Month != Date.Month)
                        {
                            goto case BusinessConvention.FOLLOWING;
                        }
                        break;
                }
            }

            return ndt;
        }

        private BusinessDate PrevBusinessDay()
        {
            BusinessDate pdt = AddBusinessDays(-1);

            return pdt;
        }

        public override int GetHashCode()
        {
            return _date.GetHashCode();
        }

        [ComVisible(false)]
        public override bool Equals(object value)
        {
            return _date.Equals(value);
        }

        public static bool operator ==(BusinessDate d1, BusinessDate d2)
        {
            return d2 != null && (d1 != null && d1._date == d2._date);
        }

        public static bool operator ==(BusinessDate d1, DateTime d2)
        {
            return d1 != null && d1._date == d2;
        }

        public static bool operator ==(DateTime d1, BusinessDate d2)
        {
            return d2 != null && d1 == d2._date;
        }

        public static bool operator !=(BusinessDate d1, BusinessDate d2)
        {
            return d2 != null && (d1 != null && d1._date != d2._date);
        }

        public static bool operator !=(BusinessDate d1, DateTime d2)
        {
            return d1 != null && d1._date != d2;
        }

        public static bool operator !=(DateTime d1, BusinessDate d2)
        {
            return d2 != null && d1 != d2.Date;
        }

        public bool lt(BusinessDate d1)
        {
            return _date < d1._date;
        }

        public bool lt(DateTime d1)
        {
            return _date < d1;
        }

        public bool le(BusinessDate d1)
        {
            return _date <= d1._date;
        }

        public bool le(DateTime d1)
        {
            return _date <= d1;
        }

        public bool gt(BusinessDate d1)
        {
            return _date > d1._date;
        }

        public bool gt(DateTime d1)
        {
            return _date > d1;
        }

        public bool ge(BusinessDate d1)
        {
            return _date >= d1._date;
        }

        public bool ge(DateTime d1)
        {
            return _date >= d1;
        }

        public bool ne(BusinessDate d1)
        {
            return _date != d1._date;
        }

        public bool ne(DateTime d1)
        {
            return _date != d1;
        }

        public static bool operator <(BusinessDate d1, BusinessDate d2)
        {
            return d1._date < d2._date;
        }

        public static bool operator <(BusinessDate d1, DateTime d2)
        {
            return d1._date < d2;
        }

        public static bool operator <(DateTime d1, BusinessDate d2)
        {
            return d1 < d2._date;
        }

        public static bool operator <=(BusinessDate d1, BusinessDate d2)
        {
            return d1._date <= d2._date;
        }

        public static bool operator <=(BusinessDate d1, DateTime d2)
        {
            return d1._date <= d2;
        }

        public static bool operator <=(DateTime d1, BusinessDate d2)
        {
            return d1 <= d2._date;
        }

        public static bool operator >(BusinessDate d1, BusinessDate d2)
        {
            return d1._date > d2._date;
        }

        public static bool operator >(BusinessDate d1, DateTime d2)
        {
            return d1._date > d2;
        }

        public static bool operator >(DateTime d1, BusinessDate d2)
        {
            return d1 > d2._date;
        }

        public static bool operator >=(BusinessDate d1, BusinessDate d2)
        {
            return d1._date >= d2._date;
        }

        public static bool operator >=(BusinessDate d1, DateTime d2)
        {
            return d1._date >= d2;
        }

        public static bool operator >=(DateTime d1, BusinessDate d2)
        {
            return d1 >= d2._date;
        }

        public static TimeSpan operator -(BusinessDate d1, BusinessDate d2)
        {
            return d1._date - d2._date;
        }

        public static TimeSpan operator -(BusinessDate d1, DateTime d2)
        {
            return d1._date - d2;
        }

        public static TimeSpan operator -(DateTime d1, BusinessDate d2)
        {
            return d1 - d2._date;
        }

        public static BusinessDate operator +(BusinessDate d1, TimeSpan ts)
        {
            return new BusinessDate(d1._date + ts, d1.Calendar);
        }

        public BusinessDate AddDays(int days)
        {
            return new BusinessDate(Date.AddDays(days), Calendar, Convention);
        }

        public BusinessDate AddMonths(int months)
        {
            return new BusinessDate(Date.AddMonths(months), Calendar, Convention);
        }

        public BusinessDate AddYears(int years)
        {
            return new BusinessDate(Date.AddYears(years), Calendar, Convention);
        }

        public int Subtract(BusinessDate d1, BusinessDate d2)
        {
            return (d1 - d2).Days;
        }

        public int Subtract(BusinessDate d1, DateTime d2)
        {
            return (d1 - d2).Days;
        }

        public int Subtract(DateTime d1, BusinessDate d2)
        {
            return (d1 - d2).Days;
        }

        [ComVisible(false)]
        public string ToString(IFormatProvider provider)
        {
            return _date.ToString(provider);
        }

        public string ToString(string format)
        {
            return _date.ToString(format);
        }

        [ComVisible(false)]
        public string ToString(string format, IFormatProvider provider)
        {
            return _date.ToString(format, provider);
        }

        public override string ToString()
        {
            return _date.ToString("yyyy-MM-dd");
        }

        public int ToExcel()
        {
            return _date.ToExcel();
        }

        public BusinessDate FromExcel(int excelDate)
        {
            return new BusinessDate(_date.FromExcel(excelDate), Calendar);
        }

        public BusinessDate Next(DayOfWeek dayOfWeek)
        {
            return new BusinessDate(_date.Next(dayOfWeek), Calendar);
        }

        public BusinessDate Find(int num, DayOfWeek dayOfWeek)
        {
            return new BusinessDate(_date.Find(num, dayOfWeek), Calendar);
        }
    }
}