namespace Payslip.Model
{
    /// <summary>
    /// A payslip for a specific Employee payment period.
    /// </summary>
    public class EmployeePayslip
    {
        public string Name { get; set; }
        public string PayPeriod { get; set; }
        public decimal GrossIncome { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal NetIncome { get; set; }
        public decimal Super { get; set; }
    }
}