namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class CertificateData : IEquatable<CertificateData>
    {
        public string CertificateReference { get; set; }

        [Required(ErrorMessage = "Learner is required")]
        public Learner Learner { get; set; }

        [Required(ErrorMessage = "LearningDetails is required")]
        public LearningDetails LearningDetails { get; set; }

        [Required(ErrorMessage = "PostalContact is required")]
        public PostalContact PostalContact { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (CertificateReference is null ? 0 : CertificateReference.GetHashCode());
                hash = (hash * multiplier) ^ (Learner is null ? 0 : Learner.GetHashCode());
                hash = (hash * multiplier) ^ (LearningDetails is null ? 0 : LearningDetails.GetHashCode());
                hash = (hash * multiplier) ^ (PostalContact is null ? 0 : PostalContact.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((CertificateData)obj);
        }

        public bool Equals(CertificateData other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(CertificateData other)
        {
            return string.Equals(CertificateReference, other.CertificateReference)
                && Equals(Learner, other.Learner)
                && Equals(LearningDetails, other.LearningDetails)
                && Equals(PostalContact, other.PostalContact);
        }

        public static bool operator ==(CertificateData left, CertificateData right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(CertificateData left, CertificateData right)
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
