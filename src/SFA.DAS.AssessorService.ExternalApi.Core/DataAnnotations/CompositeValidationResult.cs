namespace SFA.DAS.AssessorService.ExternalApi.Core.DataAnnotations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CompositeValidationResult : ValidationResult
    {
        public IEnumerable<ValidationResult> Results { get; private set; }

        public CompositeValidationResult(IEnumerable<ValidationResult> results, string errorMessage) : base(errorMessage) { Results = results; }
        public CompositeValidationResult(IEnumerable<ValidationResult> results, string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { Results = results; }
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }
    }
}
