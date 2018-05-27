using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Payslip.Model;
using Payslip.Service;
using Payslip.Web.Models;

namespace Payslip.Web.Controllers
{
    public class HomeController : Controller
    {
        private const string FileDownloadName = "payslips.csv";
        private readonly IPayslipService _payslipService;

        public HomeController(IPayslipService payslipService)
        {
            _payslipService = payslipService ?? throw new ArgumentNullException(nameof(payslipService));
        }

        public IActionResult Index()
        {
            return View(new SubmissionModel());
        }

        [HttpPost]
        public IActionResult Index([FromForm] SubmissionModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Errors = "Some errors were detected in your submission. Please correct any field errors and re-submit.";
                return View(model);
            }
            var validationContext = new ValidationContext();
            var bytes = _payslipService.GeneratePayslipsFromStream(model.EmployeeInputFile.OpenReadStream(), validationContext);
            if (!validationContext.IsValid)
            {
                model.Errors = $"Validation Error: {String.Join(",", validationContext.ValidationErrors)}";
                return View(model);
            }

            return File(bytes, "text/csv", FileDownloadName);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
