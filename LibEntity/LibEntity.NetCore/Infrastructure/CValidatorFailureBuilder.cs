using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibEntity.NetCore.Exceptions;

namespace LibEntity.NetCore.Infrastructure
{
    /// <summary>
    /// A builder class for creating validation failures.
    /// Neatly handles validation message collection and formatting of multiple failures.
    /// Introduced for (later) use coherently translating errors to users on the web frontend.
    /// </summary>
    public class CValidatorFailureBuilder
    {
        private readonly List<CValidationFailure> _failures = new();

        public void AddFailure(EnumValidationFailure failure, string message)
        {
            // if there's a failure of the same EnumValidationFailure type, append the corresponding message.
            if (_failures.Any(f => f.Failure == failure))
            {
                var existingFailure = _failures.First(f => f.Failure == failure);
                existingFailure.Add(message);
            }
            else
            {
                _failures.Add(new CValidationFailure(failure, message));
            }
        }

        public List<CValidationFailure> Get() => _failures;
        public void AddFailure(CValidationFailure failure) => _failures.Add(failure);

        // make an error on each line for each type of failure.
        public string Build()
        {
            if (_failures.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var failure in _failures)
            {
                sb.Append($"Validation Failure: {failure.Failure}: {failure.Message}\n");
            }
            return sb.ToString();
        }
        public bool ShouldThrow()
        {
            // if there are any failures, return true.
            return _failures.Count > 0;
        }

        public CEntityValidationException ValidationException() => new(this);
    }

    public class CValidationFailure
    {
        public EnumValidationFailure Failure { get; }
        public string Message { get; set; } = string.Empty;
        public CValidationFailure(EnumValidationFailure failure, string message)
        {
            Failure = failure;
            Message = message;
        }
        public void Add(string message)
        {
            Message += (message.Trim() + "; ");
        }
    }
}
