using System;
using System.Collections.Generic;
using System.Globalization;

namespace Payslip.Model
{
    public static class DateTime_Extensions{
        
        /// <summary>
        /// Months between two dates
        /// See: https://stackoverflow.com/questions/11930565/list-the-months-between-two-dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<string> MonthsBetween(
          this DateTime startDate,
          DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                yield return $"{dateTimeFormat.GetMonthName(iterator.Month)} {iterator.Year}";
                iterator = iterator.AddMonths(1);
            }
        }

    }
}