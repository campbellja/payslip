using System;

namespace Payslip.Model
{
    static public class Decimal_Extensions
    {
        public static decimal RoundUpToNearestDollar(this decimal @decimal)
        {
            return Math.Ceiling(@decimal);
            //return Math.Round(@decimal, 0, MidpointRounding.AwayFromZero);
        }
        public static decimal RoundDownToNearestDollar(this decimal @decimal)
        {
            return Math.Floor(@decimal);
        }

        public static decimal RoundToNearestDollar(this decimal @decimal)
        {
            if ((@decimal % 1) >= 0.5M)
            {
                return @decimal.RoundUpToNearestDollar();
            }
            return @decimal.RoundDownToNearestDollar();
        }
    }
}