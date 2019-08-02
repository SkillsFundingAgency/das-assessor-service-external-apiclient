namespace SFA.DAS.AssessorService.ExternalApi.Examples
{
    using CsvHelper;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Learners;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ProgramCsv
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

            ProgramCsv p = new ProgramCsv(learnerApiClient, epaApiClient, certificateApiClient, standardsApiClient);
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

        public ProgramCsv(LearnerApiClient learnerApiClient, EpaApiClient epaApiClient, CertificateApiClient certificateApiClient, StandardsApiClient standardsApiClient)
        {
            _LearnerApiClient = learnerApiClient;
            _EpaApiClient = epaApiClient;
            _CertificateApiClient = certificateApiClient;
            _StandardsApiClient = standardsApiClient;
        }

        public async Task GetLearnerExample()
        {
            const string filePath = @"CsvFiles\getLearners.csv";

            IEnumerable<GetLearnerRequest> learners;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    learners = csv.GetRecords<GetLearnerRequest>().ToList();
                }
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = learners.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                // NOTE: The External API does not have an batch delete (for safety reasons). You'll have to loop.
                foreach (var request in learners)
                {
                    var response = await _LearnerApiClient.GetLearner(request);

                    if (response is null)
                    {
                        // NOTE: You may want to deal with bad records separately
                    }
                }
            }
        }

        public async Task CreateEpaRecordsExample()
        {
            const string filePath = @"CsvFiles\createEpaRecords.csv";

            IEnumerable<CreateEpaRequest> epaRecordsToCreate;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    epaRecordsToCreate = csv.GetRecords<CreateEpaRequest>().ToList();
                }
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = epaRecordsToCreate.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                var response = (await _EpaApiClient.CreateEpaRecords(epaRecordsToCreate)).ToList();

                // NOTE: You may want to deal with good & bad records separately
                var goodEpaRecords = response.Where(c => c.EpaReference != null && !c.ValidationErrors.Any());
                var badEpaRecords = response.Except(goodEpaRecords);


                Console.WriteLine($"Good Certificates: {goodEpaRecords.Count()}, Bad Certificates: {badEpaRecords.Count()} ");
            }
        }

        public async Task UpdateEpaRecordsExample()
        {
            const string filePath = @"CsvFiles\updateEpaRecords.csv";

            IEnumerable<UpdateEpaRequest> epaRecordsToUpdate;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    epaRecordsToUpdate = csv.GetRecords<UpdateEpaRequest>().ToList();
                }
            }

            // Let's pretend the first and last apprentices have now passed their EPA
            epaRecordsToUpdate.First().EpaDetails.Epas.First().EpaOutcome = "Pass";
            epaRecordsToUpdate.First().EpaDetails.Epas.First().EpaDate = DateTime.UtcNow;
            epaRecordsToUpdate.Last().EpaDetails.Epas.First().EpaOutcome = "Pass";
            epaRecordsToUpdate.Last().EpaDetails.Epas.First().EpaDate = DateTime.UtcNow;

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = epaRecordsToUpdate.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                var response = (await _EpaApiClient.UpdateEpaRecords(epaRecordsToUpdate)).ToList();

                // NOTE: You may want to deal with good & bad records separately
                var goodEpaRecords = response.Where(c => c.EpaReference != null && !c.ValidationErrors.Any());
                var badEpaRecords = response.Except(goodEpaRecords);

                Console.WriteLine($"Good Certificates: {goodEpaRecords.Count()}, Bad Certificates: {badEpaRecords.Count()} ");
            }

        }

        public async Task DeleteEpaRecordExample()
        {
            const string filePath = @"CsvFiles\deleteEpaRecords.csv";

            IEnumerable<DeleteEpaRequest> epaRecordsToDelete;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    epaRecordsToDelete = csv.GetRecords<DeleteEpaRequest>().ToList();
                }
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = epaRecordsToDelete.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                // NOTE: The External API does not have an batch delete (for safety reasons). You'll have to loop.
                foreach (var request in epaRecordsToDelete)
                {
                    var response = await _EpaApiClient.DeleteEpaRecord(request);

                    if (response.Error != null)
                    {
                        // NOTE: You may want to deal with bad records separately
                    }
                }
            }
        }

        public async Task CreateCertificatesExample()
        {
            const string filePath = @"CsvFiles\createCertificates.csv";

            IEnumerable<CreateCertificateRequest> certificatesToCreate;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    certificatesToCreate = csv.GetRecords<CreateCertificateRequest>().ToList();
                }
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificatesToCreate.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                var response = (await _CertificateApiClient.CreateCertificates(certificatesToCreate)).ToList();

                // NOTE: You may want to deal with good & bad records separately
                var goodCertificates = response.Where(c => c.Certificate != null && !c.ValidationErrors.Any());
                var badCertificates = response.Except(goodCertificates);

                Console.WriteLine($"Good Certificates: {goodCertificates.Count()}, Bad Certificates: {badCertificates.Count()} ");
            }
        }

        public async Task UpdateCertificatesExample()
        {
            const string filePath = @"CsvFiles\updateCertificates.csv";

            IEnumerable<UpdateCertificateRequest> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    certificates = csv.GetRecords<UpdateCertificateRequest>().ToList();
                }
            }

            // Let's pretend the first and last apprentices got better grades
            certificates.First().LearningDetails.AchievementOutcome = "MERIT";
            certificates.Last().LearningDetails.AchievementOutcome = "PASS";

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificates.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                var response = (await _CertificateApiClient.UpdateCertificates(certificates)).ToList();

                // NOTE: You may want to deal with good & bad records separately
                var goodCertificates = response.Where(c => c.Certificate != null && !c.ValidationErrors.Any());
                var badCertificates = response.Except(goodCertificates);

                Console.WriteLine($"Good Certificates: {goodCertificates.Count()}, Bad Certificates: {badCertificates.Count()} ");
            }

        }

        public async Task SubmitCertificatesExample()
        {
            const string filePath = @"CsvFiles\submitCertificates.csv";

            IEnumerable<SubmitCertificateRequest> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    certificates = csv.GetRecords<SubmitCertificateRequest>().ToList();
                }
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificates.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                var response = (await _CertificateApiClient.SubmitCertificates(certificates)).ToList();

                // NOTE: You may want to deal with good & bad records separately
                var goodCertificates = response.Where(c => c.Certificate != null && !c.ValidationErrors.Any());
                var badCertificates = response.Except(goodCertificates);

                Console.WriteLine($"Good Certificates: {goodCertificates.Count()}, Bad Certificates: {badCertificates.Count()} ");
            }
        }

        public async Task DeleteCertificateExample()
        {
            const string filePath = @"CsvFiles\deleteCertificates.csv";

            IEnumerable<DeleteCertificateRequest> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    certificates = csv.GetRecords<DeleteCertificateRequest>().ToList();
                }
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificates.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                // NOTE: The External API does not have an batch delete (for safety reasons). You'll have to loop.
                foreach (var request in certificates)
                {
                    var response = await _CertificateApiClient.DeleteCertificate(request);

                    if (response.Error != null)
                    {
                        // NOTE: You may want to deal with bad records separately
                    }
                }
            }
        }

        public async Task GetCertificateExample()
        {
            const string filePath = @"CsvFiles\getCertificates.csv";

            IEnumerable<GetCertificateRequest> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                using (CsvReader csv = new CsvReader(textReader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    certificates = csv.GetRecords<GetCertificateRequest>().ToList();
                }
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificates.Any(c => !c.IsValid(out _));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                // NOTE: The External API does not have an batch delete (for safety reasons). You'll have to loop.
                foreach (var request in certificates)
                {
                    var response = await _CertificateApiClient.GetCertificate(request);

                    if (response is null)
                    {
                        // NOTE: You may want to deal with bad records separately
                    }
                }
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
        }
    }
}
