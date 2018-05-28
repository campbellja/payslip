using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payslip.Model;
using Payslip.Service;
using Payslip.Web.Models;

namespace Payslip.Web.Controllers
{
    public class HomeController : Controller
    {
        private const string FileDownloadName = "payslips.csv";
        private const string PayslipDataSessionKey = "payslip_csv";
        private readonly IPayslipService _payslipService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IPayslipService payslipService, ILogger<HomeController> logger)
        {
            _payslipService = payslipService ?? throw new ArgumentNullException(nameof(payslipService));
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new SubmissionViewModel());
        }

        [HttpPost]
        public IActionResult Index([FromForm] SubmissionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Errors = "Some errors were detected in your submission. Please correct any field errors and re-submit.";
                return View(viewModel);
            }

            var employees = ReadEmployeesFromViewModel(viewModel);
            if (employees != null)
            {
                var employeesList = employees.ToList();
                try
                {
                    GeneratePayslips(employeesList, viewModel);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error generating Payslips from uploaded Employee file");
                    viewModel.Errors = $"Error generating Payslips from uploaded Employee file: {e.Message}";
                    return View(viewModel);
                }

                viewModel.Employees = employeesList.Select(e => new EmployeeViewModel(e));
            }
            return View(viewModel);
        }

        private void GeneratePayslips(IEnumerable<Employee> employees, SubmissionViewModel viewModel)
        {
            var validationContext = new ValidationContext();
            var result = _payslipService.GeneratePayslipsFromStream(employees, validationContext);

            if (validationContext.IsValid)
            {
                var payslips = result.ToList();
                viewModel.Results = payslips;
                StoreToSessionCache(payslips);
            }
            else
            {
                viewModel.Errors = $"Validation Error: {String.Join(",", validationContext.ValidationErrors)}";
            }
        }

        private IEnumerable<Employee> ReadEmployeesFromViewModel(SubmissionViewModel viewModel)
        {
            var stream = viewModel.EmployeeInputFile.OpenReadStream();
            IEnumerable<Employee> employees = null;
            try
            {
                employees = _payslipService.ReadRecordsFromStream(stream).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error reading Employees from CSV File");
                viewModel.Errors = $"Error reading Employees from CSV File: {e.Message}";
            }

            return employees;
        }

        private void StoreToSessionCache(IEnumerable<EmployeePayslip> payslips)
        {
            var bytes = _payslipService.WritePayslipsToByteArray(payslips);
            HttpContext.Session.Set(PayslipDataSessionKey, bytes);
        }

        [HttpGet]
        public IActionResult DownloadFile()
        {
            var bytes = HttpContext.Session.Get(PayslipDataSessionKey);
            return File(bytes, "text/csv", FileDownloadName);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
