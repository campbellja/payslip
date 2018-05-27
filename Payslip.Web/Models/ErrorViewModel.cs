using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Payslip.DataAccess;
using Payslip.Model;


namespace Payslip.Web.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }


    public class SubmissionModel
    {
        [Required]
        [Display(Description = "CSV File")]
        public IFormFile EmployeeInputFile { get; set; }
        public IEnumerable<EmployeePayslip> Results { get; set; }
        public string Errors { get; set; }
    }

}