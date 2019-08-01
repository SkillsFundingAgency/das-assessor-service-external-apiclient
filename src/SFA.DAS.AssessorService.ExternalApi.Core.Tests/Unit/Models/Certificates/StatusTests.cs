namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models.Certificates
{

    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Linq;

    [TestFixture(Category = "Models")]
    public class StatusTests
    {
        [Test]
        public void CurrentStatusMissing()
        {
            // arrange
            var status = Builder<Status>.CreateNew().With(c => c.CurrentStatus = null).Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("CurrentStatus is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenStatusValid()
        {
            // arrange
            var status = Builder<Status>.CreateNew().With(s => s.CurrentStatus = "Draft").Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenStatusEqual()
        {
            // arrange
            var status1 = Builder<Status>.CreateNew().Build();
            var status2 = Builder<Status>.CreateNew().Build();

            // act
            bool areEqual = status1 == status2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenStatusNotEqual()
        {
            // arrange
            var status1 = Builder<Status>.CreateNew().With(s => s.CurrentStatus = "Draft").Build();
            var status2 = Builder<Status>.CreateNew().With(s => s.CurrentStatus = "Submitted").Build();

            // act
            bool areNotEqual = status1 != status2;

            // assert
            Assert.IsTrue(areNotEqual);
        }

        [Test]
        public void CreatedByMissing()
        {
            // arrange
            var status = Builder<Created>.CreateNew().With(c => c.CreatedBy = null).Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("CreatedBy is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenCreatedValid()
        {
            // arrange
            var status = Builder<Created>.CreateNew().With(s => s.CreatedBy = "Test").Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenCreatedEqual()
        {
            // arrange
            var status1 = Builder<Created>.CreateNew().Build();
            var status2 = Builder<Created>.CreateNew().Build();

            // act
            bool areEqual = status1 == status2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenCreatedNotEqual()
        {
            // arrange
            var status1 = Builder<Created>.CreateNew().With(s => s.CreatedBy = "Test 1").Build();
            var status2 = Builder<Created>.CreateNew().With(s => s.CreatedBy = "Test 2").Build();

            // act
            bool areNotEqual = status1 != status2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
