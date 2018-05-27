using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Payslip.Model
{
    public interface IPayslipService
    {
        IEnumerable<EmployeePayslip> GeneratePayslipsFromStream(Stream stream, IValidationContext validationContext);
    }


    public interface IValidationContext
    {
        bool IsValid { get; }
        IEnumerable<string> ValidationErrors { get; }
        void AddError(string message);
    }

    public sealed class ValidationContext : IValidationContext
    {
        private readonly List<string> _validationErrors = new List<string>();
        public IEnumerable<string> ValidationErrors => _validationErrors;
        public bool IsValid => !ValidationErrors.Any();


        public void AddError(string message)
        {
            _validationErrors.Add(message);
        }
    }
}