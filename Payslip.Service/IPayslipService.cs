using System.Collections.Generic;
using System.IO;
using Payslip.Model;

namespace Payslip.Service
{
    public interface IPayslipService
    {
        IEnumerable<Employee> ReadRecordsFromStream(Stream stream);
        IEnumerable<EmployeePayslip> GeneratePayslipsFromStream(IEnumerable<Employee> employees, IValidationContext validationContext);
        byte[] WritePayslipsToByteArray(IEnumerable<EmployeePayslip> payslips);
    }
}