namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Linq;

    [TestFixture(Category = "Model")]
    public class PostalContactTests
    {
        [Test]
        public void PostCodeInvalid()
        {
            // arrange
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "INVALID").Build();

            // act
            bool isValid = postalContact.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter a valid UK postcode", validationResults.First().ErrorMessage);
        }

        [Test]
        public void PostCodeMissing()
        {
            // arrange
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = null).Build();

            // act
            bool isValid = postalContact.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter a postcode", validationResults.First().ErrorMessage);
        }

        [Test]
        public void ContactNameMissing()
        {
            // arrange
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").With(l => l.ContactName = null).Build();

            // act
            bool isValid = postalContact.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter a contact name", validationResults.First().ErrorMessage);
        }

        [Test]
        public void OrganisationMissing()
        {
            // arrange
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").With(l => l.Organisation = null).Build();

            // act
            bool isValid = postalContact.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter an organisation", validationResults.First().ErrorMessage);
        }

        [Test]
        public void AddressMissing()
        {
            // arrange
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").With(l => l.AddressLine1 = null).Build();

            // act
            bool isValid = postalContact.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter an address", validationResults.First().ErrorMessage);
        }

        [Test]
        public void CityMissing()
        {
            // arrange
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").With(l => l.City = null).Build();

            // act
            bool isValid = postalContact.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter a city or town", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            // act
            bool isValid = postalContact.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var postalContact1 = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();
            var postalContact2 = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            // act
            bool areEqual = postalContact1 == postalContact2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var postalContact1 = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();
            var postalContact2 = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "AA1 1AA").Build();

            // act
            bool areNotEqual = postalContact1 != postalContact2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
