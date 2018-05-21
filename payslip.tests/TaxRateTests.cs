using System;
using Xunit;
using Shouldly;

namespace payslip.tests
{
    public class TaxRateTests
    {
     
        [Fact]
        public void Ctor_InvalidateIncomeRanges_ThrowsArgumentOutOfRangeException(){
               Assert.Throws<ArgumentOutOfRangeException>(()=>new TaxRate( -1M,  0M,  0M, 0M));
               Assert.Throws<ArgumentOutOfRangeException>(()=>new TaxRate( 0M,  -1M,  0M, 0M));
               Assert.Throws<ArgumentOutOfRangeException>(()=>new TaxRate( 0M,  -1M,  -1M, 0M));
        }

        [Fact]
        public void IsWithinRange_IsWithinRange_ReturnsTrue(){
            new TaxRate(37001M, 87000M, 1M, 1M).IsWithinIncomeRange(60050M).ShouldBeTrue();
            new TaxRate(0M, 18200M, 1M, 1M).IsWithinIncomeRange(60050M).ShouldBeFalse();
            new TaxRate(0M, null, 1M, 1M).IsWithinIncomeRange(1M).ShouldBeTrue();
            new TaxRate(0M, 60050M, 1M, 1M).IsWithinIncomeRange(60050M).ShouldBeTrue();
            new TaxRate(60050M, 182000M, 1M, 1M).IsWithinIncomeRange(60050M).ShouldBeTrue();
        }


        [Fact]
        public void test(){
            
        }


// $0 - $18,200         Nil Nil
// $18,201 - $37,000    19c for each $1 over $18,200
// $37,001 - $87,000    $3,572 plus 32.5c for each $1 over $37,000
// $87,001 - $180,000   $19,822 plus 37c for each $1 over $87,000
// $180,001 and over    $54,232 plus 45c for each $1 over $180,000        

        class TaxRate{
            private decimal _minIncome;
            private decimal? _maxIncome;
            private decimal _baseTaxAmount;
            private decimal _rateForEachDollarOver;

            public TaxRate(decimal minIncome, decimal? maxIncome, decimal baseTaxAmount, decimal rateForEachDollarOver)
            {
                if(minIncome < 0M)
                {
                    throw new ArgumentOutOfRangeException(nameof(minIncome), $"{nameof(minIncome)} must not be a negative decimal");
                }
                if(maxIncome.HasValue && maxIncome.Value < 0M)
                {
                    throw new ArgumentOutOfRangeException(nameof(maxIncome), $"{nameof(maxIncome)} must not be a negative decimal");
                }
                if(maxIncome < minIncome)
                {
                    throw new ArgumentOutOfRangeException(nameof(maxIncome), $"{nameof(maxIncome)} must be greater than {nameof(minIncome)}");
                }
                if(baseTaxAmount < 0M)
                {
                    throw new ArgumentOutOfRangeException(nameof(baseTaxAmount), $"{nameof(baseTaxAmount)} must not be a negative decimal");
                }
                this._minIncome = minIncome;
                this._maxIncome = maxIncome;
                _baseTaxAmount = baseTaxAmount;
                this._rateForEachDollarOver = rateForEachDollarOver;
            }

            public bool IsWithinIncomeRange(decimal income){
                return income >= _minIncome && (!_maxIncome.HasValue || (income <= _maxIncome));
            }

        }
    }


}
