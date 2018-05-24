using Payslip.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payslip.UnitTests
{
    public class Calculator
    {
        private const decimal MonthsInYear = 12M;

        private readonly IEnumerable<TaxRate> _taxRates;
        public Calculator(IEnumerable<TaxRate> taxRates)
        {
            if (taxRates == null)
            {
                throw new ArgumentNullException(nameof(taxRates));
            }
            if (!taxRates.Any())
            {
                throw new ArgumentException($"At least one tax rate must be specified", nameof(taxRates));
            }
            _taxRates = taxRates;
        }


        decimal CalculateSuperForGrossIncome(decimal grossIncome, decimal ratePercentage)
        {
            var result = grossIncome * ratePercentage;
            return result.RoundToNearestDollar();
        }


        static public decimal AmountByPaymentFrequencyRounded(decimal amount)
        {
            var result = amount / 12;
            return result.RoundToNearestDollar();
        }

        public IEnumerable<EmployeePayslip> Calculate(IEnumerable<Employee> employees)
        {
            if(employees.Any(x=>x.AnnualSalary < 0M))
            {
                throw new ArgumentOutOfRangeException($"{nameof(Employee.AnnualSalary)} must not be a negative decimal", nameof(Employee.AnnualSalary));
            }
            var result = (from e in employees
                          // from period in e.PaymentPeriods
                          let salary = e.AnnualSalary
                          let rate = _taxRates.Single(t => t.IsWithinIncomeRange(salary))
                          let superRate = e.SuperAnnuationRatePercentage
                          let incomeTax = rate.CalculateIncomeTax(salary).RoundToNearestDollar()                              
                          let grossIncome = AmountByPaymentFrequencyRounded(salary)
                          let netIncome = (grossIncome - incomeTax).RoundToNearestDollar()
                          let super = CalculateSuperForGrossIncome(grossIncome, superRate)
                          select new EmployeePayslip
                         {
                              GrossIncome = grossIncome,
                              IncomeTax = incomeTax,
                              NetIncome = netIncome,
                             Super = super
                         }
             );
            return result.ToList();

            //var result = new List<EmployeePayslip>();
            //foreach (var employee in employees)
            //{
            //    if (employee.AnnualSalary < 0M)
            //    {
            //        throw new ArgumentOutOfRangeException($"{nameof(employee.AnnualSalary)} must not be a negative decimal", nameof(employee.AnnualSalary));
            //    }
            //    var taxRate = _taxRates.FirstOrDefault(x => x.IsWithinIncomeRange(employee.AnnualSalary));
            //    var incomeTax = taxRate.CalculateIncomeTax(employee.AnnualSalary);
            //    var superRate = employee.SuperRate;


            //    var grossIncome = GrossIncomePerMonth(employee);
            //    result.Add(new EmployeePayslip
            //    {
            //        IncomeTax = incomeTax,
            //        GrossIncome = grossIncome,
            //        Super = CalculateSuperForGrossIncome(grossIncome, superRate).RoundDownToNearestDollar()
            //    });
            //}
            //return result;
        }

        private static decimal GrossIncomePerMonth(Employee employee)
        {
            return (employee.AnnualSalary / MonthsInYear).RoundDownToNearestDollar();
        }

    }
}