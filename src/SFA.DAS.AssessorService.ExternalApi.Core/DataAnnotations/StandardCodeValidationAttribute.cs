namespace SFA.DAS.AssessorService.ExternalApi.Core.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class StandardCodeValidationAttribute : ValidationAttribute
    {
        public string StandardReferenceProperty { get; private set; }

        public StandardCodeValidationAttribute(string standardReferenceProperty) : base()
        {
            if (standardReferenceProperty is null)
            {
                throw new ArgumentNullException("standardReferenceProperty");
            }

            StandardReferenceProperty = standardReferenceProperty;
        }

        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var standardReferencePropertyInfo = validationContext.ObjectType.GetProperty(StandardReferenceProperty);

            if (standardReferencePropertyInfo is null)
            {
                return new ValidationResult($"Could not find a property named {StandardReferenceProperty}");
            }

            var standardReference = standardReferencePropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (value is null && standardReference is null)
            {
                return new ValidationResult(ErrorMessage);
            }
            else if (value is int?)
            {
                var standardCode = value as int?;

                if (standardCode.HasValue && standardCode < 1)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
