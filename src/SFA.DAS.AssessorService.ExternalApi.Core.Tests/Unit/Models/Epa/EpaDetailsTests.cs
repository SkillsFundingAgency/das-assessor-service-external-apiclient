namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models.Epa
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Epa;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture(Category = "Models")]
    public class EpaDetailsTests
    {
        [Test]
        public void EpasMissing()
        {
            // arrange
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = null).Build();

            // act
            bool isValid = epaDetails.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Epas is required", validationResults.First().ErrorMessage);
        }


        [Test]
        public void WhenValid()
        {
            // arrange
            var epaRecord = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            // act
            bool isValid = epaDetails.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var epaRecord = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").Build();

            var epaDetails1 = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();
            var epaDetails2 = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            // act
            bool areEqual = epaDetails1 == epaDetails2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var epaRecord1 = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").Build();
            var epaRecord2 = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Fail").Build();

            var epaDetails1 = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord1 }).Build();
            var epaDetails2 = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord2 }).Build();

            // act
            bool areNotEqual = epaDetails1 != epaDetails2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
