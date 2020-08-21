namespace SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class PostalContact : IEquatable<PostalContact>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a contact name")]
        public string ContactName { get; set; }

        public string Department { get; set; }

        public string Organisation { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter an address")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a city or town")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a postcode")]
        [RegularExpression("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$", ErrorMessage = "Enter a valid UK postcode")]
        public string PostCode { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (ContactName is null ? 0 : ContactName.GetHashCode());
                hash = (hash * multiplier) ^ (Department is null ? 0 : Department.GetHashCode());
                hash = (hash * multiplier) ^ (Organisation is null ? 0 : Organisation.GetHashCode());
                hash = (hash * multiplier) ^ (AddressLine1 is null ? 0 : AddressLine1.GetHashCode());
                hash = (hash * multiplier) ^ (AddressLine2 is null ? 0 : AddressLine2.GetHashCode());
                hash = (hash * multiplier) ^ (AddressLine3 is null ? 0 : AddressLine3.GetHashCode());
                hash = (hash * multiplier) ^ (City is null ? 0 : City.GetHashCode());
                hash = (hash * multiplier) ^ (PostCode is null ? 0 : PostCode.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((PostalContact)obj);
        }

        public bool Equals(PostalContact other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(PostalContact other)
        {
            return string.Equals(ContactName, other.ContactName)
                && string.Equals(Department, other.Department)
                && string.Equals(Organisation, other.Organisation)
                && string.Equals(AddressLine1, other.AddressLine1)
                && string.Equals(AddressLine2, other.AddressLine2)
                && string.Equals(AddressLine3, other.AddressLine3)
                && string.Equals(City, other.City)
                && string.Equals(PostCode, other.PostCode);
        }

        public static bool operator ==(PostalContact left, PostalContact right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(PostalContact left, PostalContact right)
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
