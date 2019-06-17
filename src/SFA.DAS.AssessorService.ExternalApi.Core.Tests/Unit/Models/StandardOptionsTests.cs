namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Standards;

    [TestFixture(Category = "Model")]
    public class StandardOptionsTests
    {
        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var standard1 = Builder<StandardOptions>.CreateNew().With(s => s.StandardCode = 1).Build();
            var standard2 = Builder<StandardOptions>.CreateNew().With(s => s.StandardCode = 1).Build();

            // act
            bool areEqual = standard1 == standard2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var standard1 = Builder<StandardOptions>.CreateNew().With(s => s.StandardCode = 1).Build();
            var standard2 = Builder<StandardOptions>.CreateNew().With(s => s.StandardCode = 2).Build();

            // act
            bool areNotEqual = standard1 != standard2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
