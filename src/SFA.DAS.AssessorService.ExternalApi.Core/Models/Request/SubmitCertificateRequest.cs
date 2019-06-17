namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Request
{
    using SFA.DAS.AssessorService.ExternalApi.Core.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class SubmitCertificateRequest : IEquatable<SubmitCertificateRequest>
    {
        public string RequestId { get; set; }

        [Range(1000000000, 9999999999, ErrorMessage = "The apprentice's ULN should contain exactly 10 numbers")]
        public long Uln { get; set; }

        [StandardCodeValidation(nameof(StandardReference), ErrorMessage = "A standard should be selected")]
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the apprentice's last name")]
        public string FamilyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the certificate reference")]
        public string CertificateReference { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ Uln.GetHashCode();
                hash = (hash * multiplier) ^ StandardCode.GetHashCode();
                hash = (hash * multiplier) ^ (StandardReference is null ? 0 : StandardReference.GetHashCode());
                hash = (hash * multiplier) ^ (FamilyName is null ? 0 : FamilyName.GetHashCode());
                hash = (hash * multiplier) ^ (CertificateReference is null ? 0 : CertificateReference.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((SubmitCertificateRequest)obj);
        }

        public bool Equals(SubmitCertificateRequest other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(SubmitCertificateRequest other)
        {
            return Equals(Uln, other.Uln)
                && Equals(StandardCode, other.StandardCode)
                && string.Equals(StandardReference, other.StandardReference)
                && string.Equals(FamilyName, other.FamilyName)
                && string.Equals(CertificateReference, other.CertificateReference);
        }

        public static bool operator ==(SubmitCertificateRequest left, SubmitCertificateRequest right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(SubmitCertificateRequest left, SubmitCertificateRequest right)
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
