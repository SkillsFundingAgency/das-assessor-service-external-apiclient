namespace SFA.DAS.AssessorService.ExternalApi.Core.DataAnnotations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(value);

            Validator.TryValidateObject(value, context, results, true);

            if (results.Count != 0)
            {
                return new CompositeValidationResult(results, $"Validation for {validationContext.DisplayName} failed!");
            }

            return ValidationResult.Success;
        }
    }
}
