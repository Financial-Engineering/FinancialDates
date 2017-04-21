using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FinancialDates
{
    public enum Frequency
    {
        DAY,
        WEEK,
        FORTNIGHT,
        MONTH,
        QUARTER,
        SEMIANNUAL,
        YEAR
    }

    public enum RollConvention
    {
        SUN = 0,
        MON = 1,
        TUE = 2,
        WED = 3,
        THU = 4,
        FRI = 5,
        SAT = 6,
        EOM,
        FRN,
        IMM,
        IMMCAD,
        SFE,
        TBILL,
        NONE
    }

    public enum Stub
    {
        NONE,
        SHORT_FIRST,
        SHORT_LAST,
        LONG_FIRST,
        LONG_LAST
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Period
    {
        public Period(BusinessDate startDate, BusinessDate endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public BusinessDate StartDate { get; set; }
        public BusinessDate EndDate { get; set; }
    }

    // Define an interface to be exported through COM
    public interface ISchedule
    {
        IList Dates { get; }
        IList Periods { get; }

        int Create(BusinessDate startDate, BusinessDate endDate,
            Frequency freq, RollConvention roll, Stub stub);

        int Create(BusinessDate startDate, BusinessDate endDate,
            int duration, Frequency unit, RollConvention roll, Stub stub);
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComDefaultInterface(typeof (ISchedule))]
    public class Schedule : ISchedule
    {
        public Schedule()
        {
            StartDate = new BusinessDate();
            EndDate = new BusinessDate();
            Frequency = new Frequency();
            Duration = 1;
            Roll = RollConvention.NONE;
            Stub = Stub.NONE;

            Dates = new List<BusinessDate>();
            Periods = new List<Period>();
        }

        public Schedule(BusinessDate startDate, BusinessDate endDate,
            Frequency freq, RollConvention roll = RollConvention.NONE, Stub stub = Stub.NONE)
        {
            Create(startDate, endDate, 1, freq, roll, stub);
        }

        public BusinessDate StartDate { get; set; }
        public BusinessDate EndDate { get; set; }
        public int Duration { get; set; }
        public Frequency Frequency { get; set; }
        public RollConvention Roll { get; set; }
        public Stub Stub { get; set; }

        [ComVisible(false)]
        public List<BusinessDate> Dates { get; set; }

        [ComVisible(false)]
        public List<Period> Periods { get; set; }

        public BusinessDate AdjustEndDate(BusinessDate bd)
        {
            var rd = new BusinessDate(bd);

            switch (Roll)
            {
                case RollConvention.EOM:
                    rd.Date = bd.Date.Last();
                    break;
                case RollConvention.FRN:
                    rd.Convention = BusinessConvention.MODFOLLOWING;
                    rd = rd.AdjustDate();
                    break;
                case RollConvention.IMM:
                    rd.Date = bd.Find(3, DayOfWeek.Wednesday);
                    break;
                case RollConvention.IMMCAD:
                    rd.Date = new BusinessDate(bd.Find(3, DayOfWeek.Monday), new Calendar("GBP", "CAD"));
                    break;
                case RollConvention.SFE:
                    rd.Date = bd.Find(2, DayOfWeek.Friday);
                    break;
                case RollConvention.TBILL:
                    rd = new BusinessDate(bd.Next(DayOfWeek.Monday), new Calendar("USD"));
                    break;
                case RollConvention.SUN:
                case RollConvention.MON:
                case RollConvention.TUE:
                case RollConvention.WED:
                case RollConvention.THU:
                case RollConvention.FRI:
                case RollConvention.SAT:
                    rd.Date = bd.Date.Next((DayOfWeek) Roll);
                    break;
            }

            return rd;
        }

        public void Create(BusinessDate startDate, BusinessDate endDate,
            int duration, Frequency freq,
            RollConvention roll = RollConvention.NONE, Stub stub = Stub.NONE)
        {
            StartDate = startDate;
            EndDate = endDate;
            Duration = duration;
            Frequency = freq;
            Roll = roll;
            Stub = stub;

            Dates = new List<BusinessDate>();
            Periods = new List<Period>();
            Create();
        }

        private void Create()
        {
            int duration = Duration;
            int sign = 1;
            Del compare = (a, b) => a < b;
            var ndt = new BusinessDate(StartDate);
            var pdt = new BusinessDate(StartDate);
            BusinessDate beginDate = StartDate;
            BusinessDate endDate = EndDate;

            if ((Stub == Stub.SHORT_FIRST || Stub == Stub.LONG_FIRST))
            {
                sign = -1;
                compare = (a, b) => a > b;
                ndt = new BusinessDate(EndDate);
                pdt = new BusinessDate(EndDate);
                beginDate = EndDate;
                endDate = StartDate;
            }

            int n = 1;

            Dates.Add(ndt);
            while (compare(ndt, endDate))
            {
                switch (Frequency)
                {
                    case Frequency.DAY:
                        ndt = beginDate.AddDays(sign*duration*n++);
                        break;
                    case Frequency.WEEK:
                        duration = 7;
                        goto case Frequency.DAY;
                    case Frequency.FORTNIGHT:
                        duration = 14;
                        goto case Frequency.DAY;
                    case Frequency.MONTH:
                        ndt = beginDate.AddMonths(sign*duration*n++);
                        break;
                    case Frequency.QUARTER:
                        duration = 3;
                        goto case Frequency.MONTH;
                    case Frequency.SEMIANNUAL:
                        duration = 6;
                        goto case Frequency.MONTH;
                    case Frequency.YEAR:
                        ndt = beginDate.AddYears(sign*duration*n++);
                        break;
                }

                Dates.Add(ndt);
                Periods.Add(new Period(pdt, ndt));
                pdt = ndt;
            }

            if ((Stub == Stub.SHORT_FIRST || Stub == Stub.LONG_FIRST))
            {
                Dates[0] = StartDate;
            }
            else if ((Stub == Stub.SHORT_LAST || Stub == Stub.LONG_LAST))
            {
                Dates[Dates.Count - 1] = EndDate;
            }
        }

        [ComVisible(false)]
        private void Generate(BusinessDate startDate, BusinessDate endDate,
            int duration, Frequency freq, RollConvention roll, Stub stub)
        {
            StartDate = startDate;
            EndDate = endDate;
            Duration = duration;
            Frequency = freq;
            Roll = roll;
            Stub = stub;

            Dates = new List<BusinessDate>();
            Periods = new List<Period>();
            Create();
        }

        #region ISchedule Members

        IList ISchedule.Dates
        {
            get { return Dates; }
        }

        IList ISchedule.Periods
        {
            get { return Periods; }
        }

        int ISchedule.Create(BusinessDate startDate, BusinessDate endDate,
            int duration, Frequency unit, RollConvention roll, Stub stub)
        {
            Generate(startDate, endDate, duration, unit, roll, stub);
            return Dates.Count;
        }

        int ISchedule.Create(BusinessDate startDate, BusinessDate endDate,
            Frequency freq, RollConvention roll, Stub stub)
        {
            Generate(startDate, endDate, 1, freq, roll, stub);
            return Dates.Count;
        }

        #endregion

        private delegate bool Del(BusinessDate a, BusinessDate b);
    }
}