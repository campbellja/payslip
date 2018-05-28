using System;

namespace Payslip.Model
{
    public static class Decimal_Extensions
    {
        private static decimal RoundUpToNearestDollar(this decimal @decimal) => Math.Ceiling(@decimal);
        private static decimal RoundDownToNearestDollar(this decimal @decimal) => Math.Floor(@decimal);

        public static decimal RoundToNearestDollar(this decimal @decimal)
        {
            if ((@decimal % 1) >= 0.5M)
            {
                return @decimal.RoundUpToNearestDollar();
            }
            return @decimal.RoundDownToNearestDollar();
        }

        public static bool IsWholeNumber(this decimal @decimal) => (@decimal % 1M) == 0M;
    }
}