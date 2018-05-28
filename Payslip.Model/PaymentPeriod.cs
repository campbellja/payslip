using System;

namespace Payslip.Model
{
    /// <summary>
    /// A payment period represented as a date range of two DateTime objects.
    /// </summary>
    public sealed class PaymentPeriod
    {
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }

        internal PaymentPeriod() { }
        public PaymentPeriod(DateTime startDateTime, DateTime endDateTime)
        {
            StartDate = startDateTime;
            EndDate = endDateTime;
        }

        public static bool TryParseFromDateRangeString(string dateRange, out PaymentPeriod paymentPeriod)
        {
            var parts = dateRange.Split(new []{ '-' }, StringSplitOptions.RemoveEmptyEntries);
            if(DateTime.TryParse(parts[0], out var startDateTime)){
                if (DateTime.TryParse(parts[1], out var endDateTime))
                {
                    paymentPeriod = new PaymentPeriod(startDateTime, endDateTime);
                    return true;
                }
            }

            paymentPeriod = default(PaymentPeriod);
            return false;
        }

        public override string ToString()
        {
            return $"{StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}";
        }
    }
}