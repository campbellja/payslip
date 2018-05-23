using System;
using Xunit;
using Shouldly;
using System.Collections.Generic;
using Payslip.Model;

namespace Payslip.UnitTests
{
    public class TaxRateTests
    {

        [Fact]
        public void Ctor_InvalidateIncomeRanges_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TaxRate(-1M, 0M, 0M, 0M));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TaxRate(0M, -1M, 0M, 0M));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TaxRate(0M, -1M, -1M, 0M));
        }

        [Theory]
        [MemberData(nameof(IsWithinRangeData))]
        public void IsWithinRange_IsWithinRange_ReturnsTrue(decimal minIncome, decimal? maxIncome, decimal income, bool result)
        {
            new TaxRate(minIncome, maxIncome, 1M, 1M).IsWithinIncomeRange(income).ShouldBe(result);
        }

        public static IEnumerable<object[]> IsWithinRangeData =>
          new List<object[]>
          {
                new object[] { 37001M, 87000M, 60050M, true },
                new object[] { 0M, null, 1M, true },
                new object[] { 0M, 60050M, 60050M, true },
                new object[] { 60050M, 182000M, 60050M, true },

                new object[] { 37001M, 87000M, 37000M, false },
                new object[] { 0M, 18200M, 60050M, false },
          };

        
        [Fact]
        public void CalculateTaxAmountOverMinIncome_ReturnsTaxForTaxRate()
        {
            const decimal salary = 60050M;//$60,050
            //(60,050 - 37,000) x 0.325
            //
            var taxRate = new TaxRate(37001M, 87000M, null, 0.325M);
            taxRate.CalculateTaxAmountOverMinIncome(salary)
                .ShouldBe(7491.25M);
        }

        [Fact]
        public void CalculateIncomeTax_TaxRateWithNilTaxApplicable_ReturnsZero()
        {
            new TaxRate(37001M, 87000M).CalculateIncomeTax(60050M).ShouldBe(0.0M);
        }

        [Fact]
        public void CalculateIncomeTax_NoBaseTaxAmountSpecified_ReturnsTaxAmountOverMinIncome()
        {
            //= 7,491.25 / 12 months 
            //= 624.27083333333333333333333333333
            // arrange, act & assert
            new TaxRate(37001M, maxIncome:87000M, rateForDollarsOverMinIncome:0.325M)
                .CalculateIncomeTax(60050M).ShouldBe(624M);
        }

        [Fact]
        public void CalculateIncomeTax()
        {
            //$87,001 - $180,000 $19,822 plus 37c for each $1 over $87,000
            new TaxRate(87001M, 180000M, 19822M, 0.37M).CalculateIncomeTax(120000M).ShouldBe(2669M);
            new TaxRate(37001M, 87000M, 3572M, 0.325M).CalculateIncomeTax(60050M).ShouldBe(922M);
        }

        [Fact]
        public void CalculateTaxAmountOverMinIncome_RateForDollarsOverMinIncomeNotSpecified_ReturnsZero()
        {
            // arrange, act & assert
            new TaxRate(37001M, 87000M, 3572M).CalculateTaxAmountOverMinIncome(60050M).ShouldBe(0.0M);
        }

        [Fact]
        public void CalculateIncomeTax_RateForDollarsOverMinIncomeNotSpecified_ReturnsBaseTaxAmount()
        {
            // arrange
            const decimal wholeBaseTaxAmountPerMonth = 298M;
            // act & assert
            new TaxRate(37001M, 87000M, 3572M).CalculateIncomeTax(60050M)
                .ShouldBe(wholeBaseTaxAmountPerMonth);
        }
    }
}
