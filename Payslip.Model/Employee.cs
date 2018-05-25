using System;

namespace Payslip.Model
{
    public sealed class Employee 
    {     
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public decimal AnnualSalary{get;set;}
        
        /**0% - 50% inclusive  */         
        public decimal SuperAnnuationRatePercentage {get;set;}
        public PaymentPeriod PaymentPeriod { get; set; }
        public bool IsWholeNumber { get; set; }

        internal Guid Id { get; set; }
    }

    public class PaymentPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
