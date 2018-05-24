using System;
using Xunit;
using Shouldly;
using System.Collections.Generic;
using Payslip.Model;
using System.Linq;

namespace Payslip.UnitTests
{
    public class CalculatorTests
    {
        [Fact]
        public void Employee_NegativeAnnualSalary_ThrowsArgumentOutOfRangeException(){
            // arrange
            var sut = BuildCalculator();
            // act & assert
            Assert.Throws<ArgumentOutOfRangeException>(()=>
                sut.Calculate(new[] {
                    new Employee{
                        AnnualSalary = -1M
                    }
                })
            );                        
        }

        [Fact]
        public void Ctor_TaxRatesNotInitialised_ThrowsArgumentException(){
            // arrange, act & assert
            Assert.Throws<ArgumentException>(()=>new Calculator(new TaxRate[0]));
        }

        [Fact]
        public void Calculate_TaxRatesInitialised_ReturnsPayslip()
        {
            // arrange
            var input = new[] {
                new Employee{
                    AnnualSalary = 60050M,
                    PaymentPeriodStartDate = new DateTime(2018,03,01),
                    PaymentPeriodEndDate = new DateTime(2018, 03, 31),
                    SuperAnnuationRatePercentage  = 0.09M,      
                },
                new Employee{
                    AnnualSalary = 60050M,
                    PaymentPeriodStartDate = new DateTime(2018,03,01),
                    PaymentPeriodEndDate = new DateTime(2018, 06, 30),
                },
            };
            // act
            var payslips = BuildCalculator().Calculate(input).ToArray();
            // assert   
            var payslip = payslips[0];
            payslip.GrossIncome.ShouldBe(5004);            
            payslip.IncomeTax.ShouldBe(922M);
            payslip.NetIncome.ShouldBe(4082M);
            payslip.Super.ShouldBe(450);
        }

        private Calculator BuildCalculator()
        {
            return new Calculator(TaxRates);
        }

        [Fact]
        public void PaymentFrequencyTotalPeriods_DateRange_ReturnsTotalPeriods()
        {

        }

        Employee BuildValidEmployee()
        {
            return new Employee{AnnualSalary = 95000M};
        }

        // Supplied 2018 Tax Table:
        // $0 - $18,200         Nil Nil
        // $18,201 - $37,000    19c for each $1 over $18,200
        // $37,001 - $87,000    $3,572 plus 32.5c for each $1 over $37,000
        // $87,001 - $180,000   $19,822 plus 37c for each $1 over $87,000
        // $180,001 and over    $54,232 plus 45c for each $1 over $180,000 
        TaxRate[] TaxRates => new[]{
            TaxRate.NilTaxRate( 0M, 18_200M),
            TaxRate.Create( 18_201M, 37_000M, 0.19M),
            TaxRate.Create( 37_001M, 87_000M, 0.325M,  3_572M ),
            TaxRate.Create( 87_001M, 180_000M, 0.37M, 19_822M),
            TaxRate.TopTierRate( 180_001M, 0.45M, 54_232M ),
        };
                        
        [Fact]
        public void RoundUp_DecimalGreaterThanEqualTo50_RoundToNextDollar()
        {
            0.50M.RoundUpToNearestDollar().ShouldBe(1M);
            0.50M.RoundDownToNearestDollar().ShouldBe(0M);
        }
    }
    
}
