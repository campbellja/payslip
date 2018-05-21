using System;

namespace payslip.tests
{


    // $0 - $18,200         Nil Nil
    // $18,201 - $37,000    19c for each $1 over $18,200
    // $37,001 - $87,000    $3,572 plus 32.5c for each $1 over $37,000
    // $87,001 - $180,000   $19,822 plus 37c for each $1 over $87,000
    // $180,001 and over    $54,232 plus 45c for each $1 over $180,000        

    public sealed class TaxRate
    {
        public decimal MinIncome { get; set; }
        public decimal? MaxIncome { get; set; }
        public decimal? BaseTaxAmount { get; set; }
        public decimal? RateForDollarsOverMinIncome { get; set; }

        public TaxRate(decimal minIncome, decimal maxIncome) : this(minIncome, maxIncome, null, null) { }

        public TaxRate(decimal minIncome, decimal? maxIncome = null, decimal? baseTaxAmount = null, decimal? rateForDollarsOverMinIncome = null)
        {
            if (minIncome < 0M)
            {
                throw new ArgumentOutOfRangeException(nameof(minIncome), $"{nameof(minIncome)} must not be a negative decimal");
            }
            if (maxIncome.HasValue && maxIncome.Value < 0M)
            {
                throw new ArgumentOutOfRangeException(nameof(maxIncome), $"{nameof(maxIncome)} must not be a negative decimal");
            }
            if (maxIncome < minIncome)
            {
                throw new ArgumentOutOfRangeException(nameof(maxIncome), $"{nameof(maxIncome)} must be greater than {nameof(minIncome)}");
            }
            if (baseTaxAmount.HasValue && baseTaxAmount < 0M)
            {
                throw new ArgumentOutOfRangeException(nameof(baseTaxAmount), $"{nameof(baseTaxAmount)} must not be a negative decimal");
            }
            MinIncome = minIncome;
            MaxIncome = maxIncome;
            BaseTaxAmount = baseTaxAmount;
            RateForDollarsOverMinIncome = rateForDollarsOverMinIncome;
        }

        public bool IsWithinIncomeRange(decimal salary) => salary >= MinIncome && (!MaxIncome.HasValue || (salary <= MaxIncome));


        // taxAmountOverMinIncome = (Income - MinIncomeThresholdAmount) x rateForEachDollarOverMinIncome;
        public decimal CalculateTaxAmountOverMinIncome(decimal salary)
        {
            decimal minIncomeThresholdAmount;
            if (MinIncome > 0)
            {
                minIncomeThresholdAmount = MinIncome - 1M;
            }
            else
            {
                minIncomeThresholdAmount = MinIncome;
            }
            var rate = RateForDollarsOverMinIncome.Value;
            var taxAmountOverMinIncome = (salary - minIncomeThresholdAmount) * rate;
            return taxAmountOverMinIncome;
        }

        //incomeTax = (BaseTaxAmount + taxAmountOverMinIncome) / monthsInOneYear = incomeTax (rounded up)
        internal decimal CalculateIncomeTax(decimal salary)
        {
            if (!BaseTaxAmount.HasValue && !RateForDollarsOverMinIncome.HasValue)
            {
                return 0.0M;
            }
            const int monthsInOneYear = 12;
            var taxAmountOverMinIncome = CalculateTaxAmountOverMinIncome(salary);
            var incomeTax = (BaseTaxAmount.Value + taxAmountOverMinIncome) / ((Decimal)monthsInOneYear);
            var incomeTaxRounded = Math.Round(incomeTax, 0);
            return incomeTaxRounded;
        }
    }
}
