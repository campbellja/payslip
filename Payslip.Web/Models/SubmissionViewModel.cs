using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Payslip.Model;

namespace Payslip.Web.Models
{
    public class SubmissionViewModel
    {
        [Required(ErrorMessage = "An employee file is required")]
        [Display(Description = "CSV File")]
        public IFormFile EmployeeInputFile { get; set; }
        public IEnumerable<EmployeePayslip> Results { get; set; }
        public IEnumerable<EmployeeViewModel> Employees { get; set; }
        public string Errors { get; set; }
    }
}