﻿namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Messages.Requests.Epa
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Epa;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture(Category = "Requests")]
    public class CreateEpaTests
    {
        [Test]
        public void LearnerMissing()
        {
            // arrange
            var standard = Builder<Standard>.CreateNew().Build();
            var epaRecord = Builder<EpaRecord>.CreateNew().With(e => e.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var request = Builder<CreateEpaRequest>.CreateNew().With(r => r.Learner = null)
                                                                        .With(r => r.Standard = standard)
                                                                        .With(r => r.EpaDetails = epaDetails).Build();

            // act
            bool isValid = request.IsValid(out var validationResults);

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
            var epaRecord = Builder<EpaRecord>.CreateNew().With(e => e.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var request = Builder<CreateEpaRequest>.CreateNew().With(r => r.Learner = learner)
                                                                        .With(r => r.Standard = null)
                                                                        .With(r => r.EpaDetails = epaDetails).Build();

            // act
            bool isValid = request.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Standard is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void EpaDetailsMissing()
        {
            // arrange
            var standard = Builder<Standard>.CreateNew().Build();
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();

            var request = Builder<CreateEpaRequest>.CreateNew().With(r => r.Learner = learner)
                                                                        .With(r => r.Standard = standard)
                                                                        .With(r => r.EpaDetails = null).Build();

            // act
            bool isValid = request.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("EpaDetails is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var epaRecord = Builder<EpaRecord>.CreateNew().With(e => e.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var request = Builder<CreateEpaRequest>.CreateNew().With(r => r.Learner = learner)
                                                                        .With(r => r.Standard = standard)
                                                                        .With(r => r.EpaDetails = epaDetails).Build();

            // act
            bool isValid = request.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().Build();

            var request1 = Builder<CreateEpaRequest>.CreateNew().With(r => r.Learner = learner)
                                                                        .With(r => r.Standard = standard)
                                                                        .With(r => r.EpaDetails = epaDetails).Build();

            var request2 = Builder<CreateEpaRequest>.CreateNew().With(r => r.Learner = learner)
                                                                        .With(r => r.Standard = standard)
                                                                        .With(r => r.EpaDetails = epaDetails).Build();

            // act
            bool areEqual = request1 == request2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var standard = Builder<Standard>.CreateNew().Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().Build();

            var request1 = Builder<CreateEpaRequest>.CreateNew().With(r => r.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(r => r.Learner.FamilyName = "1")
                                                                        .With(r => r.Standard = standard)
                                                                        .With(r => r.EpaDetails = epaDetails).Build();

            var request2 = Builder<CreateEpaRequest>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(r => r.Learner.FamilyName = "2")
                                                                        .With(r => r.Standard = standard)
                                                                        .With(r => r.EpaDetails = epaDetails).Build();

            // act
            bool areNotEqual = request1 != request2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
