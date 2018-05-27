namespace Payslip.DataAccess
{
    public class Record
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal AnnualSalary { get; set; }
        public decimal SuperRate { get; set; }
        public string PaymentStartDate { get; set; }
    }
}