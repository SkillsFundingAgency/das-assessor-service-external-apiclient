﻿namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class DeleteEpaRequest : IEquatable<DeleteEpaRequest>
    {
        [Range(1000000000, 9999999999, ErrorMessage = "The apprentice's ULN should contain exactly 10 numbers")]
        public long Uln { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "A standard should be selected")]
        public string Standard { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the apprentice's last name")]
        public string FamilyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the EPA reference")]
        public string EpaReference { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ Uln.GetHashCode();
                hash = (hash * multiplier) ^ (Standard is null ? 0 : Standard.GetHashCode());
                hash = (hash * multiplier) ^ (FamilyName is null ? 0 : FamilyName.GetHashCode());
                hash = (hash * multiplier) ^ (EpaReference is null ? 0 : EpaReference.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((DeleteEpaRequest)obj);
        }

        public bool Equals(DeleteEpaRequest other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(DeleteEpaRequest other)
        {
            return Equals(Uln, other.Uln)
                && string.Equals(Standard, other.Standard)
                && string.Equals(FamilyName, other.FamilyName)
                && string.Equals(EpaReference, other.EpaReference);
        }

        public static bool operator ==(DeleteEpaRequest left, DeleteEpaRequest right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(DeleteEpaRequest left, DeleteEpaRequest right)
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
