namespace Payslip.Model
{
    public sealed class Employee
    {
        public Employee() { }
        public Employee(string firstName, string lastName, decimal annualSalary, decimal superRate, PaymentPeriod paymentPeriod)
        {
            //if (annualSalary < 0M)
            //{
            //    throw new ArgumentOutOfRangeException($"{nameof(AnnualSalary)} must not be a negative decimal", nameof(AnnualSalary));
            //}
            //if (!annualSalary.IsWholeNumber())
            //{
            //    throw new ArgumentException($"{nameof(AnnualSalary)} must be a whole number", nameof(AnnualSalary));
            //}

            //FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            //LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            //AnnualSalary = annualSalary;
            //SuperRate = superRate;
            //PaymentStartDate = paymentPeriod ?? throw new ArgumentNullException(nameof(paymentPeriod));
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal AnnualSalary { get; set; }

        public decimal SuperRate { get; set; }
        public PaymentPeriod PaymentStartDate { get; set; }

    }
}
