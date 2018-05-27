using System.Collections.Generic;

namespace Payslip.Model
{
    public static class Constants
    {
        // Supplied 2018 Tax Table:
        // $0 - $18,200         Nil Nil
        // $18,201 - $37,000    19c for each $1 over $18,200
        // $37,001 - $87,000    $3,572 plus 32.5c for each $1 over $37,000
        // $87,001 - $180,000   $19,822 plus 37c for each $1 over $87,000
        // $180,001 and over    $54,232 plus 45c for each $1 over $180,000 
        public static IEnumerable<TaxRate> TaxRates => new[]{
            TaxRate.NilTaxRate( 0M, 18_200M),
            TaxRate.Create( 18_201M, 37_000M, 0.19M),
            TaxRate.Create( 37_001M, 87_000M, 0.325M,  3_572M ),
            TaxRate.Create( 87_001M, 180_000M, 0.37M, 19_822M),
            TaxRate.TopTierRate( 180_001M, 0.45M, 54_232M ),
        };

    }
}