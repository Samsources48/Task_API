using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Utilidades
{
    public static class DateTimeExtensions
    {
        private const int minYear = 2000;
        public static bool ValidateDateTime(this DateTime dateTime)
        {
            return dateTime.Year >= minYear;
        }

        public static string ToDateFormatISOString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }
        public static string ToDateFormatTracklinkString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToDateFormatString(this DateTime dateTime)
        {
            return dateTime.ToString("ddMMyy");
        }

        public static string ToDateFormat2String(this DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy");
        }

        public static string ToDateFormatISOString(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("dd/MM/yyyy HH:mm") : DateTime.MinValue.ToString("dd/MM/yyyy HH:mm");
        }

        public static string ToDateFormatISOEmptyString(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("dd/MM/yyyy HH:mm") : "";
        }
        public static int ToDayOfWeek(this DateTime? dateTime)
        {
            return dateTime.HasValue ? (dateTime.Value.DayOfWeek == 0 ? 7 : (int)dateTime.Value.DayOfWeek) : 0;
        }
        public static int ToDayOfWeek(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == 0 ? 7 : (int)dateTime.DayOfWeek;
        }
        public static(DateTime, DateTime) GetWeekRange(DateTime currentDate)
        {
            DayOfWeek dayOfWeek = currentDate.DayOfWeek;

            if (dayOfWeek == DayOfWeek.Sunday)
            {
                dayOfWeek = DayOfWeek.Monday;
            }

            DateTime startOfWeek = currentDate.AddDays(-(int)dayOfWeek + (int)DayOfWeek.Monday);

            DateTime endOfWeek = startOfWeek.AddDays(6);

            return (startOfWeek, endOfWeek);
        }
    }
}
