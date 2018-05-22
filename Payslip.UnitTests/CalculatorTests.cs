using System;
using Xunit;
using Shouldly;
using Payslip.Model;
using System.Collections.Generic;
using System.Linq;

namespace Payslip.UnitTests
{
    public class CalculatorTests
    {
        [Fact]
        public void Employee_AnnualSalary_ThrowsArgumentOutOfRangeException(){
            // arrange
            var sut = BuildPayslipCalculator();
            // act & assert
            Assert.Throws<ArgumentOutOfRangeException>(()=>
                sut.Calculate(new Employee{
                    AnnualSalary = -1M
                })
            );                        
        }

        [Fact]
        public void Ctor_TaxRatesNotInitialised_ThrowsArgumentException(){
            // arrange, act & assert
            Assert.Throws<ArgumentException>(()=>new Calculator(new TaxRate[0]));
        }

        [Fact]
        public void Calculate_TaxRatesInitialised_ReturnsPayslip(){
            
            // arrange & act
            var payslip = new Calculator(new[]{
                new TaxRate(37001M, 87000M, 3572M, 0.325M)
            }).Calculate(new Employee{
                AnnualSalary = 60050M
            });
            // assert   
            payslip.IncomeTax.ShouldBe(922M);
            
        }

        Employee BuildValidEmployee()
        {
            return new Employee{AnnualSalary = 95000M};
        }

        private IEnumerable<TaxRate> _taxRates = new[]{
            new TaxRate(37001M, 87000M, 3572M, 0.325M)
        };

        Calculator BuildPayslipCalculator(){
            var result = new Calculator(_taxRates);
            return result;
        }

        public class Calculator{
            private IEnumerable<TaxRate> _taxRates;
            public Calculator(IEnumerable<TaxRate> taxRates){
                if(taxRates == null)
                {
                    throw new ArgumentNullException(nameof(taxRates));
                }
                if( !taxRates.Any())
                {
                    throw new ArgumentException($"At least one tax rate must be specified", nameof(taxRates));
                }
                _taxRates = taxRates;
            }
            public EmployeePayslip Calculate(Employee employee){                                
                if(employee.AnnualSalary < 0M)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(employee.AnnualSalary)} must not be a negative decimal", nameof(employee.AnnualSalary));
                }   
                var taxRate = _taxRates.FirstOrDefault(  x=>x.IsWithinIncomeRange(employee.AnnualSalary) );
                var incomeTax = taxRate.CalculateIncomeTax(employee.AnnualSalary);
                return new EmployeePayslip
                {
                    IncomeTax = incomeTax
                };
            }
        }

        public class EmployeePayslip
        {
            public string Name{get;set;}
            public string PayPeriod{get;set;}
            public decimal GrossIncome{get;set;}
            public decimal IncomeTax{get;set;}
            public decimal NetIncome{get;set;}
            public decimal Super{get;set;}
            
        }

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
