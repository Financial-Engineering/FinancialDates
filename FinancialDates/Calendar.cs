using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Runtime.InteropServices;

namespace FinancialDates
{
    using CalDictionary = Dictionary<string, HashSet<DateTime>>;

    #region UnknownCurrencyException Class

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class UnknownCurrencyException : Exception
    {
        public UnknownCurrencyException()
        {
        }

        public UnknownCurrencyException(string message)
            : base(message)
        {
        }

        public UnknownCurrencyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    #endregion

    #region Calendar Class

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class Calendar
    {
        private static readonly CalDictionary Cals;

        static Calendar()
        {
            Cals = new CalDictionary();

            try
            {
                const string connstr =
                    "metadata=res://*/Calendar.csdl|res://*/Calendar.ssdl|res://*/Calendar.msl;provider=System.Data.SqlClient;provider connection string='Data Source=MIZAUSYD07;Initial Catalog=FXSystem;Integrated Security=True;MultipleActiveResultSets=False'";
                var fxContext = new FXSystemEntities(connstr);

                ObjectQuery<FXFCAL> calendars = fxContext.FXFCAL;

                // Ignore MEL entries and only consider SYD for AUD
                var calQuery = from cal in calendars
                    where !cal.BUSCENTRE.Contains(@"/MELBOURNE/")
                    select new {ccy = cal.CCYCODE, date = cal.DATE};

                foreach (var row in calQuery)
                {
                    if (!Cals.ContainsKey(row.ccy))
                        Cals.Add(row.ccy, new HashSet<DateTime>());

                    var dts = Cals[row.ccy];
                    dts.Add(row.date.Value);
                }

                fxContext.Connection.Close();
            }
            catch
            {
            }
        }

        public Calendar()
        {
            Holidays = new HashSet<DateTime>();
        }

        public Calendar(string ccy)
        {
            SetCalendar(ccy);
        }

        public Calendar(params string[] ccys)
        {
            SetCalendar(ccys);
        }

        [ComVisible(false)]
        public HashSet<DateTime> Holidays { get; set; }

        public void SetCalendar(string ccy)
        {
            if (!Cals.ContainsKey(ccy))
                throw new UnknownCurrencyException("Unsupported Currency: " + ccy);

            Holidays = Cals[ccy];
        }

        [ComVisible(false)]
        public void SetCalendar(params string[] ccys)
        {
            foreach (var ccy in ccys)
            {
                if (!Cals.ContainsKey(ccy))
                    throw new UnknownCurrencyException(ccy);

                if (Holidays == null)
                    Holidays = Cals[ccy];
                else
                    Holidays.UnionWith(Cals[ccy]);
            }
        }

        public void AddCalendar(string ccy)
        {
            if (!Cals.ContainsKey(ccy))
                throw new UnknownCurrencyException(ccy);

            if (Holidays == null)
                Holidays = Cals[ccy];
            else
                Holidays.UnionWith(Cals[ccy]);
        }

        // This function makes no provision for Israel or Islamic countries
        public bool IsWeekend(DateTime dt)
        {
            return (dt.DayOfWeek == DayOfWeek.Saturday) ||
                   (dt.DayOfWeek == DayOfWeek.Sunday);
        }

        public bool IsHoliday(DateTime dt)
        {
            return Holidays.Contains(dt);
        }

        public bool IsBusinessDay(DateTime dt)
        {
            return !(IsHoliday(dt) || IsWeekend(dt));
        }
    }

    #endregion
}