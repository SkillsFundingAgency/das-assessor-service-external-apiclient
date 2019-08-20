namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa
{
    using SFA.DAS.AssessorService.ExternalApi.Core.DataAnnotations;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class UpdateEpaRequest : IEquatable<UpdateEpaRequest>
    {
        public string RequestId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the EPA reference")]
        public string EpaReference { get; set; }

        [Required(ErrorMessage = "Standard is required"), ValidateObject]
        public Standard Standard { get; set; }

        [Required(ErrorMessage = "Learner is required"), ValidateObject]
        public Learner Learner { get; set; }

        [Required(ErrorMessage = "EpaDetails is required"), ValidateObject]
        public Models.Epa.EpaDetails EpaDetails { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (EpaReference is null ? 0 : EpaReference.GetHashCode());
                hash = (hash * multiplier) ^ (Standard is null ? 0 : Standard.GetHashCode());
                hash = (hash * multiplier) ^ (Learner is null ? 0 : Learner.GetHashCode());
                hash = (hash * multiplier) ^ (EpaDetails is null ? 0 : EpaDetails.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((UpdateEpaRequest)obj);
        }

        public bool Equals(UpdateEpaRequest other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(UpdateEpaRequest other)
        {
            return string.Equals(EpaReference, other.EpaReference)
                && Equals(Standard, other.Standard)
                && Equals(Learner, other.Learner)
                && Equals(EpaDetails, other.EpaDetails);
        }

        public static bool operator ==(UpdateEpaRequest left, UpdateEpaRequest right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(UpdateEpaRequest left, UpdateEpaRequest right)
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
        #endregion
    }
}
