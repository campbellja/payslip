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
        public void Ctor_InvalidIncomeRanges_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TaxRate.Create(-1M, 0M, 0M, 0M));
            Assert.Throws<ArgumentOutOfRangeException>(() => TaxRate.Create(0M, -1M, 0M, 0M));
            Assert.Throws<ArgumentOutOfRangeException>(() => TaxRate.Create(0M, -1M, -1M, 0M));
        }

        [Theory]
        [MemberData(nameof(IsWithinRangeData))]
        public void IsWithinIncomeRange_SalaryWithinRange_ReturnsTrue(decimal minIncome, decimal? maxIncome, decimal income, bool result)
        {
            TaxRate.Create(minIncome, maxIncome).IsWithinIncomeRange(income).ShouldBe(result);
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
        public void CalculateTaxAmountOverMinIncome_RateValueSet_ReturnsTaxOverMinIncome()
        {
            const decimal salary = 60050M;//$60,050
            //(60,050 - 37,000) x 0.325
            //
            var taxRate = TaxRate.Create(37001M, 87000M, rateValue: 0.325M);
            taxRate.CalculateTaxAmountOverMinIncome(salary)
                .ShouldBe(7491.25M);
        }

        [Fact]
        public void CalculateIncomeTax_TaxRateWithNilTaxApplicable_ReturnsZero()
        {
            TaxRate.NilTaxRate(37001M, 87000M).CalculateIncomeTax(60050M).ShouldBe(0.0M);
        }

        [Fact]
        public void CalculateIncomeTax_NoBaseTaxAmountSpecified_ReturnsTaxAmountOverMinIncome()
        {
            //= 7,491.25 / 12 months 
            //= 624.27083333333333333333333333333
            // arrange, act & assert
            TaxRate.Create(37001M, maxIncome:87000M, rateValue:0.325M)
                .CalculateIncomeTax(60050M).ShouldBe(624M);
        }

        [Fact]
        public void CalculateIncomeTax()
        {
            //$87,001 - $180,000 $19,822 plus 37c for each $1 over $87,000
            TaxRate.Create(87001M, 180000M, 0.37M , 19822M  ).CalculateIncomeTax(120000M).ShouldBe(2669M);
            TaxRate.Create(37001M, 87000M, 0.325M, 3572M   ).CalculateIncomeTax(60050M).ShouldBe(922M);
        }

        [Fact]
        public void CalculateTaxAmountOverMinIncome_RateNotSpecified_ReturnsZero()
        {            
            // arrange, act & assert
            TaxRate.NilTaxRate(37001M, 87000M).CalculateTaxAmountOverMinIncome(60050M).ShouldBe(0.0M);
        }

        [Fact]
        public void CalculateIncomeTax_RateForDollarsOverMinIncomeNotSpecified_ReturnsBaseTaxAmount()
        {
            // arrange
            const decimal wholeBaseTaxAmountPerMonth = 298M;
            // act & assert
            TaxRate.Create(37001M, 87000M, baseTaxAmount: 3572M)
                .CalculateIncomeTax(60050M)
                .ShouldBe(wholeBaseTaxAmountPerMonth);
        }
    }
}
