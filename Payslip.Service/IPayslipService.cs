using System.Collections.Generic;
using System.IO;
using Payslip.Model;

namespace Payslip.Service
{
    public interface IPayslipService
    {
        IEnumerable<EmployeePayslip> GeneratePayslipsFromStream(Stream stream, IValidationContext validationContext);
    }
}