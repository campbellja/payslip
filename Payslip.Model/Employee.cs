using System;

namespace Payslip.Model
{
    public sealed class Employee 
    {     
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public decimal AnnualSalary{get;set;}
        
        /**0% - 50% inclusive  */         
        public double SuperRate{get;set;}
        public DateTime PaymentStartDate{get;set;}
    }
}
