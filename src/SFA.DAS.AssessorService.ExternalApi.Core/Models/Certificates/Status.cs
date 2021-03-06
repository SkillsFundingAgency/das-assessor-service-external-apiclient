﻿namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class Status : IEquatable<Status>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "CurrentStatus is required")]
        public string CurrentStatus { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (CurrentStatus is null ? 0 : CurrentStatus.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Status)obj);
        }

        public bool Equals(Status other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Status other)
        {
            return string.Equals(CurrentStatus, other.CurrentStatus);
        }

        public static bool operator ==(Status left, Status right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Status left, Status right)
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
