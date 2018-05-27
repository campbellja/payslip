using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Payslip.DataAccess;
using Payslip.Model;

namespace Payslip.Service
{
    public sealed class PayslipService : IPayslipService
    {
        private readonly IPayslipRepository _payslipRepository;
        private readonly ILogger<PayslipService> _logger;
        private readonly Calculator _calculator;

        public PayslipService(IPayslipRepository payslipRepository, ILogger<PayslipService> logger)
        {
            _payslipRepository = payslipRepository ?? throw new ArgumentNullException(nameof(payslipRepository));
            _calculator = new Calculator(Constants.TaxRates);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<EmployeePayslip> GeneratePayslipsFromStream(Stream stream, IValidationContext validationContext)
        {
            IEnumerable<Employee> employees;
            try
            {
                employees = _payslipRepository.ReadRecordsFromStream<Employee>(stream);
            }
            catch (IOException ioe)
            {
                _logger.LogError(ioe, $"{nameof(IOException)} thrown reading employee records from file stream");
                throw;
            }
            _logger.LogInformation($"Processing {employees.Count()} records...");
            return _calculator.Calculate(employees, validationContext);
        }
    }
}