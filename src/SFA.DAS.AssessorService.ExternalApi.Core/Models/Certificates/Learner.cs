namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class Learner : IEquatable<Learner>
    {
        [Range(1000000000, 9999999999, ErrorMessage = "The apprentice's ULN should contain exactly 10 numbers")]
        public long Uln { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the apprentice's first name")]
        public string GivenNames { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the apprentice's last name")]
        public string FamilyName { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ Uln.GetHashCode();
                hash = (hash * multiplier) ^ (GivenNames is null ? 0 : GivenNames.GetHashCode());
                hash = (hash * multiplier) ^ (FamilyName is null ? 0 : FamilyName.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Learner)obj);
        }

        public bool Equals(Learner other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Learner other)
        {
            return Equals(Uln, other.Uln)
                && string.Equals(GivenNames, other.GivenNames)
                && string.Equals(FamilyName, other.FamilyName);
        }

        public static bool operator ==(Learner left, Learner right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Learner left, Learner right)
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
