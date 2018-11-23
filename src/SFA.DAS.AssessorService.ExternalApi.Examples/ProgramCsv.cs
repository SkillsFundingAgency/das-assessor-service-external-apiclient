namespace SFA.DAS.AssessorService.ExternalApi.Examples
{
    using CsvHelper;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
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

            ProgramCsv p = new ProgramCsv(certificateApiClient);
            p.CreateCertificatesExample().GetAwaiter().GetResult();
            p.UpdateCertificatesExample().GetAwaiter().GetResult();
            p.SubmitCertificatesExample().GetAwaiter().GetResult();
            p.DeleteCertificateExample().GetAwaiter().GetResult();
            p.GetCertificateExample().GetAwaiter().GetResult();
        }


        private readonly CertificateApiClient _CertificateApiClient;

        public ProgramCsv(CertificateApiClient certificateApiClient)
        {
            _CertificateApiClient = certificateApiClient;
        }

        public async Task CreateCertificatesExample()
        {
            const string filePath = @"CsvFiles\createCertificates.csv";

            IEnumerable<CreateCertificate> certificatesToCreate;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificatesToCreate = csv.GetRecords<CreateCertificate>().ToList();
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificatesToCreate.Any(c => !c.IsValid(out ICollection<ValidationResult> validationResults));

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

            IEnumerable<UpdateCertificate> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificates = csv.GetRecords<UpdateCertificate>().ToList();
            }

            // Let's pretend the first and last apprentices got better grades
            certificates.First().LearningDetails.AchievementOutcome = "MERIT";
            certificates.Last().LearningDetails.AchievementOutcome = "PASS";

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificates.Any(c => !c.IsValid(out ICollection<ValidationResult> validationResults));

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

            IEnumerable<SubmitCertificate> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificates = csv.GetRecords<SubmitCertificate>().ToList();
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificates.Any(c => !c.IsValid(out ICollection<ValidationResult> validationResults));

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

            IEnumerable<DeleteCertificate> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificates = csv.GetRecords<DeleteCertificate>().ToList();
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificates.Any(c => !c.IsValid(out ICollection<ValidationResult> validationResults));

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

            IEnumerable<GetCertificate> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificates = csv.GetRecords<GetCertificate>().ToList();
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificates.Any(c => !c.IsValid(out ICollection<ValidationResult> validationResults));

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
    }
}
