namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public sealed class LearningDetails : IEquatable<LearningDetails>
    {
        public DateTime StandardPublicationDate { get; set; }
        public string CourseOption { get; set; }

        [CustomValidation(typeof(OverallGradeValidator), "ValidateOverallGrade")]
        public string OverallGrade { get; set; }
        public string AchievementOutcome { get; set; }

        [CustomValidation(typeof(AchievementDateValidator), "ValidateAchievementDate")]
        public DateTime? AchievementDate { get; set; }
        public DateTime LearningStartDate { get; set; }
        public string ProviderName { get; set; }
        public int ProviderUkPrn { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ StandardPublicationDate.GetHashCode();
                hash = (hash * multiplier) ^ (CourseOption is null ? 0 : CourseOption.GetHashCode());
                hash = (hash * multiplier) ^ (OverallGrade is null ? 0 : OverallGrade.GetHashCode());
                hash = (hash * multiplier) ^ (AchievementOutcome is null ? 0 : AchievementOutcome.GetHashCode());
                hash = (hash * multiplier) ^ (AchievementDate is null ? 0 : AchievementDate.GetHashCode());
                hash = (hash * multiplier) ^ LearningStartDate.GetHashCode();
                hash = (hash * multiplier) ^ (ProviderName is null ? 0 : ProviderName.GetHashCode());
                hash = (hash * multiplier) ^ ProviderUkPrn.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((LearningDetails)obj);
        }

        public bool Equals(LearningDetails other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(LearningDetails other)
        {
            return Equals(StandardPublicationDate, other.StandardPublicationDate)
                && string.Equals(CourseOption, other.CourseOption)
                && string.Equals(OverallGrade, other.OverallGrade)
                && string.Equals(AchievementOutcome, other.AchievementOutcome)
                && Equals(AchievementDate, other.AchievementDate)
                && Equals(LearningStartDate, other.LearningStartDate)
                && string.Equals(ProviderName, other.ProviderName)
                && Equals(ProviderUkPrn, other.ProviderUkPrn);
        }

        public static bool operator ==(LearningDetails left, LearningDetails right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(LearningDetails left, LearningDetails right)
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

        public static class AchievementDateValidator
        {
            public static ValidationResult ValidateAchievementDate(DateTime? achievementDate, ValidationContext validationContext)
            {
                if (validationContext.MemberName != "AchievementDate")
                    throw new InvalidOperationException("This Validator is exclusive to AchievementDate");

                if (!achievementDate.HasValue)
                {
                    return new ValidationResult("Enter the achievement date", new List<string> { "AchievementDate" });
                }
                else if (achievementDate.Value < new DateTime(2017, 1, 1))
                {
                    return new ValidationResult("An achievement date cannot be before 01 01 2017", new List<string> { "AchievementDate" });
                }
                else if (achievementDate.Value > DateTime.UtcNow)
                {
                    return new ValidationResult("An achievement date cannot be in the future", new List<string> { "AchievementDate" });
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }

        public static class OverallGradeValidator
        {
            public static ValidationResult ValidateOverallGrade(string overallGrade, ValidationContext validationContext)
            {
                if (validationContext.MemberName != "OverallGrade")
                    throw new InvalidOperationException("This Validator is exclusive to OverallGrade");

                var grades = new string[] { "Pass", "Credit", "Merit", "Distinction", "Pass with excellence", "No grade awarded" };

                if (string.IsNullOrWhiteSpace(overallGrade))
                {
                    return new ValidationResult($"Select the grade the apprentice achieved", new List<string> { "OverallGrade" });
                }
                else if (!grades.Any(g => g == overallGrade))
                {
                    string gradesString = string.Join(", ", grades);
                    return new ValidationResult($"Invalid grade. Must one of the following: {gradesString}", new List<string> { "OverallGrade" });
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }
        #endregion
    }
}
