using System;
using CsvHelper.Configuration;

namespace Payslip.DataAccess
{
    public sealed class RecordClassMapping : ClassMap<Record>
    {
        // ReSharper disable once UnusedMember.Global
        public RecordClassMapping()
        {
            AutoMap();
            Map(x => x.SuperRate)
                .ConvertUsing(r =>
                {
                    var value = r.GetField(3);
                    var trimmed = value.Trim(new[] {'%', ' '});
                    if (Decimal.TryParse(trimmed, out decimal result))
                    {
                        return result / 100;
                    }

                    throw new InvalidOperationException(
                        $"Unable to parse super rate - Unrecognised percentage value: {value}");
                });
        }
    }
}