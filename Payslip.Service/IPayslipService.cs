using System.Collections.Generic;
using System.IO;
using Payslip.Model;

namespace Payslip.Service
{
    public interface IPayslipService
    {
        byte[] GeneratePayslipsFromStream(Stream stream, IValidationContext validationContext);
    }
}