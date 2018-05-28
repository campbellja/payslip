using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Payslip.Model
{
    public interface IValidationContext
    {
        bool IsValid { get; }
        IEnumerable<string> ValidationErrors { get; }
        void AddError(string message);
    }
}