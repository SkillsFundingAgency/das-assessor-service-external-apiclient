namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Integration.Infrastructure
{
    using FizzWare.NBuilder;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Learners;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Learners;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    [TestFixture(Category = "Infrastructure")]
    public sealed class LearnerApiClientTests : IDisposable
    {
        private const string apiBaseAddress = "http://localhost";
        private const string subscriptionKey = "test";

        private LearnerApiClient _ApiClient;
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

            _ApiClient = new LearnerApiClient(httpClient);
        }

        [TearDown]
        public void Dispose()
        {
            _MockHttp?.Dispose();
        }

        [Test]
        public async Task GetLearner()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            int standardCode = 1;

            var standard = Builder<Models.Certificates.Standard>.CreateNew().With(s => s.StandardCode = standardCode).Build();
            var learner = Builder<Models.Certificates.Learner>.CreateNew().With(l => l.Uln = uln).With(l => l.FamilyName = lastname).Build();

            var learnerData = Builder<LearnerData>.CreateNew().With(cd => cd.Standard = standard)
                                                            .With(cd => cd.Learner = learner)
                                                            .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                            .Build();

            var status = new Status { CompletionStatus = 1 };

            var expectedResponse = new Learner { LearnerData = learnerData, Status = status, Certificate = null, EpaDetails = null};

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/learner/{uln}/{lastname}/{standardCode}")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new GetLearnerRequest { Uln = uln, FamilyName = lastname, Standard = standardCode.ToString() };
            var actual = await _ApiClient.GetLearner(request);

            // assert
            Assert.That(actual.Error, Is.Null);
            Assert.That(actual.Learner, Is.Not.Null);
        }

        [Test]
        public async Task GetLearner_LearnerNotFound()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "NOT_FOUND";
            int standardCode = 4321;

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Cannot find apprentice with the specified Uln, FamilyName & Standard"
            };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/learner/{uln}/{lastname}/{standardCode}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new GetLearnerRequest { Uln = uln, FamilyName = lastname, Standard = standardCode.ToString() };
            var actual = await _ApiClient.GetLearner(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
            Assert.That(actual.Learner, Is.Null);
        }
    }
}
