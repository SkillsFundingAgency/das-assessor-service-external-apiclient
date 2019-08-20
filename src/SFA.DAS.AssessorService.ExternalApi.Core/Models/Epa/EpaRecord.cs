namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Epa
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public sealed class EpaRecord : IEquatable<EpaRecord>
    {
        [CustomValidation(typeof(EpaDateValidator), "ValidateEpaDate")]
        public DateTime EpaDate { get; set; }
        [CustomValidation(typeof(EpaOutcomeValidator), "ValidateEpaOutcome")]
        public string EpaOutcome { get; set; }
        public bool? Resit { get; set; }
        public bool? Retake { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ EpaDate.GetHashCode();
                hash = (hash * multiplier) ^ (EpaOutcome is null ? 0 : EpaOutcome.GetHashCode());
                hash = (hash * multiplier) ^ (Resit is null ? 0 : Resit.GetHashCode());
                hash = (hash * multiplier) ^ (Retake is null ? 0 : Retake.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((EpaRecord)obj);
        }

        public bool Equals(EpaRecord other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(EpaRecord other)
        {
            return Equals(EpaDate, other.EpaDate)
                && string.Equals(EpaOutcome, other.EpaOutcome)
                && Equals(Resit, other.Resit)
                && Equals(Retake, other.Retake);
        }

        public static bool operator ==(EpaRecord left, EpaRecord right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(EpaRecord left, EpaRecord right)
        {
            return !(left == right);
        }
        #endregion

        #region DataAnnotations 
        public bool IsValid(out ICollection<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();

            ValidationContext validationContent = new ValidationContext(this);
            return Validator.TryValidateObject(validationContent.ObjectInstance, validationContent, validationResults, true);
        }

        public static class EpaDateValidator
        {
            public static ValidationResult ValidateEpaDate(DateTime epaDate, ValidationContext validationContext)
            {
                if (validationContext.MemberName != "EpaDate")
                    throw new InvalidOperationException("This Validator is exclusive to EpaDate");

                if (epaDate > DateTime.UtcNow)
                {
                    return new ValidationResult("An EPA date cannot be in the future", new List<string> { "EpaDate" });
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }

        public static class EpaOutcomeValidator
        {
            public static ValidationResult ValidateEpaOutcome(string epaOutcome, ValidationContext validationContext)
            {
                if (validationContext.MemberName != "EpaOutcome")
                    throw new InvalidOperationException("This Validator is exclusive to EpaOutcome");

                var outcomes = new string[] { "Pass", "Fail" };

                if (string.IsNullOrWhiteSpace(epaOutcome))
                {
                    return new ValidationResult($"Select the outcome the apprentice achieved", new List<string> { "EpaOutcome" });
                }
                else if (!outcomes.Any(g => g == epaOutcome))
                {
                    string outcomesString = string.Join(", ", outcomes);
                    return new ValidationResult($"Invalid outcome. Must one of the following: {outcomesString}", new List<string> { "EpaOutcome" });
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }
        #endregion
    }
}
