using System;
using System.Collections.Generic;
using System.Linq;
using Payslip.Model;

namespace Payslip.Model
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

        public static decimal AmountByPaymentFrequencyRounded(decimal amount)
        {
            var result = amount / MonthsInYear;
            return result.RoundToNearestDollar();
        }


        private static IEnumerable<string> PaymentPeriods(Employee e)
        {
            return e.PaymentStartDate.StartDate.MonthsBetween(e.PaymentStartDate.EndDate);
        }

        public IEnumerable<EmployeePayslip> Calculate(IEnumerable<Employee> employees, IValidationContext validationContext)
        {
            List<EmployeePayslip> result = null;
            Validate(employees, validationContext);

            if (validationContext.IsValid)
            {
                result = (from e in employees
                        let salary = e.AnnualSalary
                        let rate = _taxRates.Single(t => t.IsWithinIncomeRange(salary))
                        let superRate = e.SuperRate
                        let incomeTax = rate.CalculateIncomeTax(salary)
                        let grossIncome = AmountByPaymentFrequencyRounded(salary)
                        let netIncome = (grossIncome - incomeTax).RoundToNearestDollar()
                        let super = CalculateSuperForGrossIncome(grossIncome, superRate)
                        from payPeriod in PaymentPeriods(e)
                        select new EmployeePayslip
                        {
                            Name = $"{e.FirstName} {e.LastName}",
                            PayPeriod = payPeriod,
                            GrossIncome = grossIncome,
                            IncomeTax = incomeTax,
                            NetIncome = netIncome,
                            Super = super
                        }
                    ).ToList();
            }

            return result;
        }

        private void Validate(IEnumerable<Employee> employees, IValidationContext validationContext)
        {
            foreach (var e in employees)
            {
                var annualSalary = e.AnnualSalary;
                if (annualSalary < 0M)
                {
                    validationContext.AddError($"{nameof(annualSalary)} must not be a negative decimal, was: {annualSalary}");
                }
                if (!annualSalary.IsWholeNumber())
                {
                    validationContext.AddError($"{nameof(annualSalary)} must be a whole number, was: {annualSalary}");
                }

                if (String.IsNullOrWhiteSpace(e.FirstName))
                {
                    validationContext.AddError($"{nameof(e.FirstName)} must not be empty");
                }
                if (String.IsNullOrWhiteSpace(e.LastName))
                {
                    validationContext.AddError($"{nameof(e.LastName)} must not be empty");
                }

                if (e.PaymentStartDate == null)
                {
                    validationContext.AddError($"{nameof(e.PaymentStartDate)} payment period must be specified and use the following date range format: DD MM - DD MM");
                }
                
                if (e.SuperRate < 0M || e.SuperRate > 0.50M)
                {
                    validationContext.AddError($"{nameof(e.SuperRate)} must be between 0% and 50% inclusive");
                }
            }
        }
    }
}