using System.Collections.Generic;
using System.Linq;

namespace Payslip.Model
{
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