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
            var records = _payslipService.GeneratePayslipsFromStream(model.InputFile.OpenReadStream(), validationContext);
            if (!validationContext.IsValid)
            {
                model.Errors = $"Validation Error: {String.Join(",", validationContext.ValidationErrors)}";
                return View(model);
            }
            
            model.Results = records;
            return View(model);
        }

        //[HttpPost]
        //public IActionResult GeneratePayslips([FromForm] SubmissionModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        model.Errors = "Some errors were detected in your submission. Please correct any field errors and re-submit.";
        //        return View( "Index", model);
        //    }

        //    var inputFile = model.InputFile;
        //    var records = _payslipRepository.ReadRecordsFromStream<Employee>(inputFile.OpenReadStream());

        //    var employees = records.Select(x => new Employee(x.FirstName, x.LastName,
        //        x.AnnualSalary,
        //        x.SuperRate,
        //        x.PaymentStartDate)
        //    );
        //    var results = new Calculator(TaxRates).Calculate(employees);

        //    string fileName = "payslips.csv";
        //    var bytes = _payslipRepository.WriteRecordsToBytes(results);
        //    byte[] fileBytes = bytes;
        //    return File(fileBytes, "text/csv", fileName);
        //}


        //public async Task<FileResult> DownloadFile()
        //{
        //    string fileName = "payslips.csv";
        //    byte[] fileBytes = CountfileBytes();

        //    return File(fileBytes, "text/csv", fileName);
        //}

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
