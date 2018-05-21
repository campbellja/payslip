using System;
using Xunit;
using Shouldly;

namespace payslip.tests
{
    public class CalculatorTests
    {
        [Fact]
        public void AnnualSalary_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            var account = new Account{AnnualSalary = -0.01M};
            Assert.Throws<ArgumentOutOfRangeException>(()=>Calculate(account));            
        }

        [Fact]
        public void AnnualSalary_Calculate_GrossIncomeRounded()
        {
            var result = Calculate(new Account{AnnualSalary = 60050M});
            
            result.GrossIncome.ShouldBe(5004M);
        }
        class Calculation
        {
            public decimal GrossIncome { get; internal set; }
            public decimal IncomeTax{get; internal set;}
        }
        private Calculation Calculate(Account account)
        {
            if(account.AnnualSalary < 0M){
                throw new ArgumentOutOfRangeException();
            }
            var grossIncome = account.AnnualSalary / 12;
            
            var grossIncomeRounded = Math.Round(grossIncome, 0);
            return new Calculation{GrossIncome = grossIncomeRounded};
        }
    }


    class Account{
         public string FirstName{get;set;}
         public string LastName{get;set;}
         public Decimal AnnualSalary{get;set;}
         public double SuperRate{get;set;}
    }
}
