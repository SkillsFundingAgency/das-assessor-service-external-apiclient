namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models.Certificates
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Linq;

    [TestFixture(Category = "Models")]
    public class CertificateDataTests
    {
        [Test]
        public void LearnerMissing()
        {
            // arrange
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = null)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificateData.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Learner is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void StandardMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = null)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificateData.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Standard is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void LearningDetailsMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = null)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificateData.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("LearningDetails is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void PostalContactMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = null).Build();

            // act
            bool isValid = certificateData.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("PostalContact is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificateData.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();

            var certificateData1 = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificateData2 = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool areEqual = certificateData1 == certificateData2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();

            var certificateData1 = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.Learner.FamilyName = "1")
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificateData2 = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.Learner.FamilyName = "2")
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool areNotEqual = certificateData1 != certificateData2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
