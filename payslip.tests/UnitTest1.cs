using System;
using Xunit;
using Shouldly;

namespace payslip.tests
{
    public class UnitTest1
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

        [Fact]
        public void TaxRate_ValidatesIncomeRanges(){
               Assert.Throws<ArgumentOutOfRangeException>(new TaxRate(0M,1M, 1M));
        }



        TaxRate SelectTaxRate( decimal income ){
            //(3,572 + (60,050 - 37,000) x 0.325) / 12 

        }

// $0 - $18,200         Nil Nil
// $18,201 - $37,000    19c for each $1 over $18,200
// $37,001 - $87,000    $3,572 plus 32.5c for each $1 over $37,000
// $87,001 - $180,000   $19,822 plus 37c for each $1 over $87,000
// $180,001 and over    $54,232 plus 45c for each $1 over $180,000        

        class TaxRate{
            private decimal _minIncome;
            private decimal _maxIncome;
            private decimal v3;

            public TaxRate(decimal minIncome, decimal maxIncome, decimal v3)
            {
                this._minIncome = minIncome;
                this._maxIncome = maxIncome;
                this.v3 = v3;
            }

            public decimal MinIncome{get;set;}
            public decimal MaxIncome{get;set;}
            public decimal Amount{get;set;}
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
