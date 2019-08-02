namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class Learner : IEquatable<Learner>
    {
        public LearnerData LearnerData { get; set; }
        public Status Status { get; set; }
        public Epa.EpaDetails EpaDetails { get; set; }
        public Certificates.Certificate Certificate { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (LearnerData is null ? 0 : LearnerData.GetHashCode());
                hash = (hash * multiplier) ^ (Status is null ? 0 : Status.GetHashCode());
                hash = (hash * multiplier) ^ (EpaDetails is null ? 0 : EpaDetails.GetHashCode());
                hash = (hash * multiplier) ^ (Certificate is null ? 0 : Certificate.GetHashCode());
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
            return Equals(LearnerData, other.LearnerData)
                && Equals(Status, other.Status)
                && Equals(EpaDetails, other.EpaDetails)
                && Equals(Certificate, other.Certificate);
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
