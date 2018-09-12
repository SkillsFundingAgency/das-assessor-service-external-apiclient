namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls
{
    using Microsoft.Win32;
    using SFA.DAS.AssessorService.ExternalApi.Client.Helpers;
    using SFA.DAS.AssessorService.ExternalApi.Client.Properties;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for SubmitWindow.xaml
    /// </summary>
    public partial class SubmitWindow : Window
    {
        private ViewModels.SubmitViewModel _ViewModel => DataContext as ViewModels.SubmitViewModel;

        public SubmitWindow()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _ViewModel.FilePath = openFileDialog.FileName;
                _ViewModel.Certificates.Clear();

                foreach (var item in CsvFileHelper<CertificateData>.GetFromFile(_ViewModel.FilePath))
                {
                    _ViewModel.Certificates.Add(item);
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.Certificates.Clear();
            _ViewModel.FilePath = string.Empty;
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!_ViewModel.InvalidCertificates.View.IsEmpty)
            {
                string sMessageBoxText = "Do you want to continue?";
                string sCaption = "Invalid Certificates";

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.No:
                        return;
                }
            }

            await SubmitCertificates();

        }

        private async Task SubmitCertificates()
        {
            string subscriptionKey = Settings.Default["SubscriptionKey"].ToString();
            string apiBaseAddress = Settings.Default["ApiBaseAddress"].ToString();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseAddress);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                CertificateApiClient certificateApiClient = new CertificateApiClient(httpClient);

                var certsToSubmit = _ViewModel.Certificates.Select(sc =>
                                        new SubmitCertificate
                                        {
                                            Uln = sc.Learner.Uln,
                                            FamilyName = sc.Learner.FamilyName,
                                            StandardCode = sc.LearningDetails.StandardCode
                                        });

                var results = await certificateApiClient.SubmitCertificates(certsToSubmit);

                var validCertificates = results.Where(r => r.Certificate != null);
                var invalidCertificates = results.Except(validCertificates);

                if (invalidCertificates.Any())
                {
                    List<CertificateData> certificatesToSave = new List<CertificateData>();

                    // This is a bit horrendous but easier to read than LINQ
                    foreach(var cert in invalidCertificates)
                    {
                        CertificateData certificateToSave = _ViewModel.Certificates.FirstOrDefault(vmc => vmc.Learner.Uln == cert.Uln
                                                                                                && vmc.Learner.FamilyName == cert.FamilyName
                                                                                                && vmc.LearningDetails.StandardCode == cert.StandardCode);

                        if (certificateToSave != null) certificatesToSave.Add(certificateToSave);
                    }

                    SaveInvalidCertificates(certificatesToSave);
                }

                SaveCertificates(validCertificates.Select(r => r.Certificate));
            }
        }

        private void SaveInvalidCertificates(IEnumerable<CertificateData> invalidCertificates)
        {
            string sMessageBoxText = "There were invalid certificates. Do you want to save these to a new file to amend?";
            string sCaption = "Invalid Certificates";

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Error);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.No:
                    return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                CsvFileHelper<CertificateData>.SaveToFile(saveFileDialog.FileName, invalidCertificates);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveCertificates(IEnumerable<Certificate> certificates)
        {
            string sMessageBoxText = "Do you want to save the newly submitted certificates?";
            string sCaption = "Save Certificates";

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.No:
                    return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                CsvFileHelper<Certificate>.SaveToFile(saveFileDialog.FileName, certificates);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
