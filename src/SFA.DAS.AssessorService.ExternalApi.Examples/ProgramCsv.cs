namespace SFA.DAS.AssessorService.ExternalApi.Examples
{
    using CsvHelper;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Certificates;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ProgramCsv
    {
        public static void Main(string[] args)
        {
            const string subscriptionKey = ""; // insert your key here
            const string apiBaseAddress = ""; // insert the API address here

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            httpClient.BaseAddress = new Uri(apiBaseAddress);

            CertificateApiClient certificateApiClient = new CertificateApiClient(httpClient);
            StandardsApiClient standardsApiClient = new StandardsApiClient(httpClient);

            ProgramCsv p = new ProgramCsv(certificateApiClient, standardsApiClient);
            p.CreateCertificatesExample().GetAwaiter().GetResult();
            p.UpdateCertificatesExample().GetAwaiter().GetResult();
            p.SubmitCertificatesExample().GetAwaiter().GetResult();
            p.DeleteCertificateExample().GetAwaiter().GetResult();
            p.GetCertificateExample().GetAwaiter().GetResult();
            p.GetGradesExample().GetAwaiter().GetResult();
            p.GetOptionsForAllStandardsExample().GetAwaiter().GetResult();
            p.GetOptionsForStandardExample().GetAwaiter().GetResult();
        }


        private readonly CertificateApiClient _CertificateApiClient;
        private readonly StandardsApiClient _StandardsApiClient;

        public ProgramCsv(CertificateApiClient certificateApiClient, StandardsApiClient standardsApiClient)
        {
            _CertificateApiClient = certificateApiClient;
            _StandardsApiClient = standardsApiClient;
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
