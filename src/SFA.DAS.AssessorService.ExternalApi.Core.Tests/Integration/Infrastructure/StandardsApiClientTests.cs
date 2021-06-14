namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Integration.Infrastructure
{
    using FizzWare.NBuilder;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Standards;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    [TestFixture(Category = "Infrastructure")]
    public sealed class StandardsApiClientTests : IDisposable
    {
        private const string apiBaseAddress = "http://localhost";
        private const string subscriptionKey = "test";

        private StandardsApiClient _ApiClient;
        private MockHttpMessageHandler _MockHttp;

        [SetUp]
        public void Setup()
        {
            _MockHttp = new MockHttpMessageHandler();

            HttpClient httpClient = _MockHttp.ToHttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            httpClient.BaseAddress = new Uri(apiBaseAddress);

            _ApiClient = new StandardsApiClient(httpClient);
        }

        [TearDown]
        public void Dispose()
        {
            _MockHttp?.Dispose();
        }

        [Test]
        public async Task GetOptionsForAllStandards()
        {
            // arrange 
            var standard1 = Builder<StandardOptions>.CreateNew().With(so => so.StandardCode = 1)
                                                                        .With(so => so.StandardReference = "ST0127")
                                                                        .With(cd => cd.CourseOption = new List<string> { "English", "French", "German" }).Build();

            var standard2 = Builder<StandardOptions>.CreateNew().With(so => so.StandardCode = 6)
                                                                        .With(so => so.StandardReference = "ST0156")
                                                                        .With(cd => cd.CourseOption = new List<string> { "Overhead lines", "Underground cables", "Substation fitting" }).Build();

            var expectedResponse = new List<StandardOptions>
            {
               standard1, standard2
            };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/standards/options")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.GetOptionsForAllStandards();

            // assert
            Assert.That(actual, Has.Count.EqualTo(2));
            Assert.That(actual.FirstOrDefault(so => so.StandardCode == 1), Is.Not.Null);
        }

        [Test]
        public async Task GetOptionsForStandard_Valid_StandardCode()
        {
            // arrange 
            var standard = Builder<StandardOptions>.CreateNew().With(so => so.StandardCode = 1)
                                                                        .With(so => so.StandardReference = "ST0127")
                                                                        .With(cd => cd.CourseOption = new List<string> { "English", "French", "German" }).Build();

            var expectedResponse = standard;

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/standards/options/{standard.StandardCode}")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.GetOptionsForStandard("1");

            // assert
            Assert.That(actual, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task GetOptionsForStandard_Valid_StandardReference()
        {
            // arrange 
            var standard = Builder<StandardOptions>.CreateNew().With(so => so.StandardCode = 1)
                                                                        .With(so => so.StandardReference = "ST0127")
                                                                        .With(cd => cd.CourseOption = new List<string> { "English", "French", "German" }).Build();

            var expectedResponse = standard;

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/standards/options/{standard.StandardReference}")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.GetOptionsForStandard("ST0127");

            // assert
            Assert.That(actual, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task GetOptionsForStandard_Standard_No_Options()
        {
            // arrange 
            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/standards/options/NO_OPTIONS")
                .Respond(HttpStatusCode.NoContent, "application/json", string.Empty);

            // act
            var actual = await _ApiClient.GetOptionsForStandard("NO_OPTIONS");

            // assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public async Task GetOptionsForStandard_Standard_Not_Found()
        {
            // arrange 
            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/standards/options/INVALID")
                .Respond(HttpStatusCode.NotFound, "application/json", string.Empty);

            // act
            var actual = await _ApiClient.GetOptionsForStandard("INVALID");

            // assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public async Task GetOptionsForStandardVersion_VersionFound()
        {
            var standard = Builder<StandardOptions>.CreateNew().With(so => so.StandardCode = 1)
                                                                        .With(so => so.StandardReference = "ST0127")
                                                                        .With(so => so.Version = "1.0")
                                                                        .With(cd => cd.CourseOption = new List<string> { "English", "French", "German" })
                                                                        .Build();

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/standards/options/{standard.StandardReference}/{standard.Version}")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(standard));

            var actual = await _ApiClient.GetOptionsForStandardVersion("ST0127", "1.0");

            Assert.That(actual, Is.EqualTo(standard));
        }

        [Test]
        public async Task GetOptionsForStandardVerion_VersionNotFound()
        {
            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/standards/options/INVALID/INVALID")
                .Respond(HttpStatusCode.NotFound, "application/json", string.Empty);

            var actual = await _ApiClient.GetOptionsForStandardVersion("INVALID","INVALID");

            Assert.That(actual, Is.Null);
        }
    }
}
