namespace SFA.DAS.AssessorService.ExternalApi.Examples
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Learners;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Epa;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main()
        {
            const string subscriptionKey = ""; // insert your key here
            const string apiBaseAddress = ""; // insert the API address here

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            httpClient.BaseAddress = new Uri(apiBaseAddress);

            LearnerApiClient learnerApiClient = new LearnerApiClient(httpClient);
            EpaApiClient epaApiClient = new EpaApiClient(httpClient);
            CertificateApiClient certificateApiClient = new CertificateApiClient(httpClient);
            StandardsApiClient standardsApiClient = new StandardsApiClient(httpClient);

            Program p = new Program(learnerApiClient, epaApiClient, certificateApiClient, standardsApiClient);
            p.GetLearnerExample().GetAwaiter().GetResult();
            p.CreateEpaRecordsExample().GetAwaiter().GetResult();
            p.UpdateEpaRecordsExample().GetAwaiter().GetResult();
            p.DeleteEpaRecordExample().GetAwaiter().GetResult();
            p.CreateCertificatesExample().GetAwaiter().GetResult();
            p.UpdateCertificatesExample().GetAwaiter().GetResult();
            p.SubmitCertificatesExample().GetAwaiter().GetResult();
            p.DeleteCertificateExample().GetAwaiter().GetResult();
            p.GetCertificateExample().GetAwaiter().GetResult();
            p.GetGradesExample().GetAwaiter().GetResult();
            p.GetOptionsForAllStandardsExample().GetAwaiter().GetResult();
            p.GetOptionsForStandardExample().GetAwaiter().GetResult();
        }

        private readonly LearnerApiClient _LearnerApiClient;
        private readonly EpaApiClient _EpaApiClient;
        private readonly CertificateApiClient _CertificateApiClient;
        private readonly StandardsApiClient _StandardsApiClient;

        public Program(LearnerApiClient learnerApiClient, EpaApiClient epaApiClient, CertificateApiClient certificateApiClient, StandardsApiClient standardsApiClient)
        {
            _LearnerApiClient = learnerApiClient;
            _EpaApiClient = epaApiClient;
            _CertificateApiClient = certificateApiClient;
            _StandardsApiClient = standardsApiClient;
        }

        public async Task GetLearnerExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            string standard = "1";

            GetLearnerRequest learnerToGet = new GetLearnerRequest
            {
                Uln = uln,
                FamilyName = lastName,
                Standard = standard,
            };

            if (learnerToGet.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _LearnerApiClient.GetLearner(learnerToGet);
            }
        }

        public async Task CreateEpaRecordsExample()
        {
            long uln = 1234567890;
            string firstName = "Fred";
            string lastName = "Blogs";
            int standardCode = 1;
            string standardReference = "ST0127";
            string version = "1.0";
            string epaOutcome = "Fail";
            DateTime epaDate = DateTime.UtcNow;

            CreateEpaRequest newEpa = new CreateEpaRequest
            {
                Learner = new Learner { Uln = uln, GivenNames = firstName, FamilyName = lastName },
                LearningDetails = new LearningDetails { Version = version },
                Standard = new Standard { StandardCode = standardCode, StandardReference = standardReference },
                EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaOutcome = epaOutcome, EpaDate = epaDate } } }
            };

            if (newEpa.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _EpaApiClient.CreateEpaRecords(new List<CreateEpaRequest> { newEpa });
            }
        }

        public async Task UpdateEpaRecordsExample()
        {
            // NOTE: You will need to know the Epa Reference
            string epaReference = "1234567890";
            long uln = 1234567890;
            string firstName = "Fred";
            string lastName = "Blogs";
            int standardCode = 1;
            string standardReference = "ST0127";
            string version = "1.0";
            string epaOutcome = "Pass";
            DateTime epaDate = DateTime.UtcNow;

            // Let's pretend the apprentice has now passed their EPA
            UpdateEpaRequest updatedEpa = new UpdateEpaRequest
            {
                EpaReference = epaReference,
                Learner = new Learner { Uln = uln, GivenNames = firstName, FamilyName = lastName },
                LearningDetails = new LearningDetails { Version = version },
                Standard = new Standard { StandardCode = standardCode, StandardReference = standardReference },
                EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaOutcome = epaOutcome, EpaDate = epaDate } } }
            };

            if (updatedEpa.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _EpaApiClient.UpdateEpaRecords(new List<UpdateEpaRequest> { updatedEpa });
            }
        }

        public async Task DeleteEpaRecordExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            string standard = "1";
            string epaReference = "1234567890";

            DeleteEpaRequest epaToDelete = new DeleteEpaRequest
            {
                Uln = uln,
                FamilyName = lastName,
                Standard = standard,
                EpaReference = epaReference
            };

            if (epaToDelete.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _EpaApiClient.DeleteEpaRecord(epaToDelete);
            }
        }

        public async Task CreateCertificatesExample()
        {
            long uln = 1234567890;
            string firstName = "Fred";
            string lastName = "Blogs";
            int standardCode = 1;
            string standardReference = "ST0127";
            string version = "1.0";
            string overallGrade = "PASS";
            string contactName = "Shreya Smith";
            string organisation = "Contoso Ltd";
            string address = "123 Test Road";
            string city = "Townsville";
            string postcode = "ZY9 9ZY";

            CreateCertificateRequest newCertificate = new CreateCertificateRequest
            {
                Learner = new Learner { Uln = uln, GivenNames = firstName, FamilyName = lastName },
                Standard = new Standard { StandardCode = standardCode, StandardReference = standardReference },
                LearningDetails = new LearningDetails { OverallGrade = overallGrade, AchievementDate = DateTime.UtcNow, Version = version },
                PostalContact = new PostalContact { ContactName = contactName, Organisation = organisation, AddressLine1 = address, City = city, PostCode = postcode }
            };

            if (newCertificate.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.CreateCertificates(new List<CreateCertificateRequest> { newCertificate });
            }
        }

        public async Task UpdateCertificatesExample()
        {
            // NOTE: You will need to know what the certificate currently looks like
            Certificate currentCertificate = new Certificate
            {
                Status = new Status { CurrentStatus = "Draft" },
                Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Example" },
                CertificateData = new CertificateData
                {
                    CertificateReference = "00012001",
                    Learner = new Learner { Uln = 1234567890, GivenNames = "Fred", FamilyName = "Bloggs" },
                    Standard = new Standard { StandardCode = 1, StandardReference = "ST0127", Level = 1, StandardName = "Example Standard", },
                    LearningDetails = new LearningDetails
                    {
                        LearningStartDate = DateTime.UtcNow.AddYears(-1),
                        ProviderName = "Test Provider",
                        ProviderUkPrn = 123456,
                        OverallGrade = "Pass",
                        AchievementDate = DateTime.UtcNow,
                        Version = "1.0"
                    },
                    PostalContact = new PostalContact { ContactName = "Shreya Smith", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", City = "Townsville", PostCode = "ZY99ZZ" }
                }
            };

            // Let's pretend the apprentice got a better grade
            UpdateCertificateRequest updatedCertificate = new UpdateCertificateRequest
            {
                CertificateReference = currentCertificate.CertificateData.CertificateReference,
                Learner = currentCertificate.CertificateData.Learner,
                Standard = currentCertificate.CertificateData.Standard,
                LearningDetails = currentCertificate.CertificateData.LearningDetails,
                PostalContact = currentCertificate.CertificateData.PostalContact,
            };

            updatedCertificate.LearningDetails.OverallGrade = "Merit";

            if (updatedCertificate.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.UpdateCertificates(new List<UpdateCertificateRequest> { updatedCertificate });
            }
        }

        public async Task SubmitCertificatesExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            int standardCode = 1;
            string standardReference = "ST0127";
            string certificateReference = "00012001";

            SubmitCertificateRequest certificateToSubmit = new SubmitCertificateRequest
            {
                Uln = uln,
                FamilyName = lastName,
                StandardCode = standardCode,
                StandardReference = standardReference,
                CertificateReference = certificateReference
            };

            if (certificateToSubmit.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.SubmitCertificates(new List<SubmitCertificateRequest> { certificateToSubmit });
            }
        }

        public async Task DeleteCertificateExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            string standard = "1";
            string certificateReference = "00012001";

            DeleteCertificateRequest certificateToDelete = new DeleteCertificateRequest
            {
                Uln = uln,
                FamilyName = lastName,
                Standard = standard,
                CertificateReference = certificateReference
            };

            if (certificateToDelete.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.DeleteCertificate(certificateToDelete);
            }
        }

        public async Task GetCertificateExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            string standard = "1";

            GetCertificateRequest certificateToGet = new GetCertificateRequest
            {
                Uln = uln,
                FamilyName = lastName,
                Standard = standard,
            };

            if (certificateToGet.IsValid(out _))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.GetCertificate(certificateToGet);
            }
        }

        public async Task GetGradesExample()
        {
            await _CertificateApiClient.GetGrades();
        }

        public async Task GetOptionsForAllStandardsExample()
        {
            await _StandardsApiClient.GetOptionsForAllStandards();
        }

        public async Task GetOptionsForStandardExample()
        {
            string standardCode = 1.ToString();
            string standardReference = "ST0127";

            await _StandardsApiClient.GetOptionsForStandard(standardCode);
            await _StandardsApiClient.GetOptionsForStandard(standardReference);
            await _StandardsApiClient.GetOptionsForStandardVersion(standardCode, "1.0");
        }
    }
}
