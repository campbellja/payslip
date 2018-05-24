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
        public DateTime PaymentPeriodStartDate{get;set;}
        public DateTime PaymentPeriodEndDate { get; set; }
    }
}
