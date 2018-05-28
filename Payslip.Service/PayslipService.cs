using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Payslip.DataAccess;
using Payslip.Model;

namespace Payslip.Service
{
    public sealed class PayslipService : IPayslipService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<PayslipService> _logger;
        private readonly Calculator _calculator;

        public PayslipService(IEmployeeRepository employeeRepository, ILogger<PayslipService> logger)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _calculator = new Calculator(Constants.TaxRates);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Employee> ReadRecordsFromStream(Stream stream)
        {
            try
            {
                return _employeeRepository.ReadRecordsFromStream<Employee>(stream);
            }
            catch (IOException ioe)
            {
                _logger.LogError(ioe, $"{nameof(IOException)} thrown reading {nameof(Employee)} records from file stream");
                throw;
            }
            catch (ReaderException re)
            {
                _logger.LogError(re, $"{nameof(ReaderException)} thrown reading {nameof(Employee)} records from file stream");
                throw;
            }
        }

        public IEnumerable<EmployeePayslip> GeneratePayslipsFromStream(IEnumerable<Employee> employees, IValidationContext validationContext)
        {
            var employeeList = employees.ToList();
            _logger.LogInformation($"Processing {employeeList.Count} records...");
            return _calculator.Calculate(employeeList, validationContext);
        }

        public byte[] WritePayslipsToByteArray(IEnumerable<EmployeePayslip> payslips)
        {
            return _employeeRepository.WriteRecordsToBytes(payslips);
        }
    }
}