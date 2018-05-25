using System;
using Microsoft.AspNetCore.Http;

namespace Payslip.Web.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }


    public class Submission
    {
        public IFormFile InputFile { get; set; }
    }
}