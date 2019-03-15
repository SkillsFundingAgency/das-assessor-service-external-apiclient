﻿namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates
{
    using SFA.DAS.AssessorService.ExternalApi.Core.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class Standard : IEquatable<Standard>
    {
        [StandardCodeValidation(nameof(StandardReference), ErrorMessage = "A standard should be selected")]
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }

        public string StandardName { get; set; }
        public int Level { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ StandardCode.GetHashCode();
                hash = (hash * multiplier) ^ (StandardReference is null ? 0 : StandardReference.GetHashCode());
                hash = (hash * multiplier) ^ (StandardName is null ? 0 : StandardName.GetHashCode());
                hash = (hash * multiplier) ^ Level.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Standard)obj);
        }

        public bool Equals(Standard other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Standard other)
        {
            return Equals(StandardCode, other.StandardCode)
                && string.Equals(StandardReference, other.StandardReference)
                && string.Equals(StandardName, other.StandardName)
                && Equals(Level, other.Level);
        }

        public static bool operator ==(Standard left, Standard right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Standard left, Standard right)
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
