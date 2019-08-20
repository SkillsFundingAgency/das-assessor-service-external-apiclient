namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Epa
{
    using SFA.DAS.AssessorService.ExternalApi.Core.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public sealed class EpaDetails : IEquatable<EpaDetails>
    {
        public DateTime? LatestEpaDate { get; set; }
        public string LatestEpaOutcome { get; set; }
        [Required(ErrorMessage = "Epas is required"), ValidateObject]
        public List<EpaRecord> Epas { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (LatestEpaDate is null ? 0 : LatestEpaDate.GetHashCode());
                hash = (hash * multiplier) ^ (LatestEpaOutcome is null ? 0 : LatestEpaOutcome.GetHashCode());
                hash = (hash * multiplier) ^ (Epas is null ? 0 : Epas.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((EpaDetails)obj);
        }

        public bool Equals(EpaDetails other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(EpaDetails other)
        {
            var epasEqual = Epas is null ? other.Epas is null : Epas.SequenceEqual(other.Epas);

            return Equals(LatestEpaDate, other.LatestEpaDate)
                && string.Equals(LatestEpaOutcome, other.LatestEpaOutcome)
                && epasEqual;
        }

        public static bool operator ==(EpaDetails left, EpaDetails right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(EpaDetails left, EpaDetails right)
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
