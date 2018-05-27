using System;
using CsvHelper.Configuration;
using Payslip.Model;

namespace Payslip.DataAccess
{
    public sealed class EmployeeClassMapping : ClassMap<Employee>
    {
        private const string PaymentStartDateField = "payment_start_date";
        private const string SuperRateField = "super_rate";

        // ReSharper disable once UnusedMember.Global
        public EmployeeClassMapping()
        {
            AutoMap();
            Map(x => x.SuperRate)
                .ConvertUsing(r =>
                {
                    var value = r.GetField(SuperRateField);
                    var trimmed = value.Trim(new[] {'%', ' '});
                    if (Decimal.TryParse(trimmed, out decimal result))
                    {
                        return result / 100;
                    }

                    throw new InvalidOperationException(
                        $"Unable to parse {SuperRateField} - Unrecognised percentage value: {value}");
                });
            Map(x => x.PaymentStartDate)
                .ConvertUsing(x =>
                {
                    var dateRange = x.GetField(PaymentStartDateField);
                    PaymentPeriod.TryParseFromDateRangeString(dateRange, out var paymentPeriod);
                    return paymentPeriod;
                });
        }
    }
}