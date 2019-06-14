﻿namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Standards
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public sealed class StandardOptions : IEquatable<StandardOptions>
    {
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public IEnumerable<string> CourseOption { get; set; }

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
                hash = (hash * multiplier) ^ (CourseOption is null ? 0 : CourseOption.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((StandardOptions)obj);
        }

        public bool Equals(StandardOptions other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(StandardOptions other)
        {
            var courseOptionsEqual = CourseOption is null ? other.CourseOption is null : CourseOption.SequenceEqual(other.CourseOption);
            return Equals(StandardCode, other.StandardCode)
                && string.Equals(StandardReference, other.StandardReference)
                && courseOptionsEqual;
        }

        public static bool operator ==(StandardOptions left, StandardOptions right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(StandardOptions left, StandardOptions right)
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
