using System;

namespace Payslip.Model
{
    public sealed class TaxRate
    {
        private decimal MinIncome { get; }
        private decimal? MaxIncome { get; }
        private decimal? BaseTaxAmount { get; }
        private decimal? RateValue { get; }
        
        public static TaxRate NilTaxRate(decimal minIncome, decimal maxIncome)
        {
            return new TaxRate(minIncome, maxIncome);
        }
        
        public static TaxRate TopTierRate(decimal minIncome, decimal? rateValue = null, decimal? baseTaxAmount = null)
        {
            return new TaxRate(minIncome, null, rateValue, baseTaxAmount);
        }

        public static TaxRate Create(decimal minIncome, decimal? maxIncome = null, decimal? rateValue = null, decimal? baseTaxAmount = null)
        {
            return new TaxRate(minIncome, maxIncome, rateValue, baseTaxAmount);
        }
        private TaxRate(decimal minIncome, decimal? maxIncome = null,  decimal? rateValue = null, decimal? baseTaxAmount = null)
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
            RateValue = rateValue;
        }

        public bool IsWithinIncomeRange(decimal salary) => salary >= MinIncome && (!MaxIncome.HasValue || (salary <= MaxIncome));
        
        /// <summary>
        /// Calculation: taxAmountOverMinIncome = (Income - MinIncomeThresholdAmount) x rateForEachDollarOverMinIncome;
        /// </summary>
        /// <param name="salary"></param>
        /// <returns></returns>
        public decimal CalculateTaxAmountOverMinIncome(decimal salary)
        {            
            if(!RateValue.HasValue)
            {
                return 0.0M;
            }

            var rate = RateValue.Value;
            decimal minIncomeThresholdAmount;
            if (MinIncome > 0)
            {
                minIncomeThresholdAmount = MinIncome - 1M;
            }
            else
            {
                minIncomeThresholdAmount = MinIncome;
            }
            
            return (salary - minIncomeThresholdAmount) * rate;            
        }

        private const int TotalMonthsInOneYear = 12;

        /// <summary>
        /// Calculates the income tax applicable to a specified salary for this TaxRate's income range.
        /// Calculation: incomeTax = (BaseTaxAmount + taxAmountOverMinIncome) / TotalMonthsInOneYear = incomeTax (rounded)
        /// </summary>
        /// <param name="salary"></param>
        /// <returns></returns>
        public decimal CalculateIncomeTax(decimal salary)
        {
            if (!BaseTaxAmount.HasValue && !RateValue.HasValue)
            {
                return 0.0M;
            }
            
            var baseTax = BaseTaxAmount.GetValueOrDefault();
            var taxAmountOverMinIncome = CalculateTaxAmountOverMinIncome(salary);
            var incomeTax = (baseTax + taxAmountOverMinIncome) / TotalMonthsInOneYear;
            return incomeTax.RoundToNearestDollar();
        }
    }
}
