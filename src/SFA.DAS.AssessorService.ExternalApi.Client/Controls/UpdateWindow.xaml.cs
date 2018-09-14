namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls
{
    using Microsoft.Win32;
    using SFA.DAS.AssessorService.ExternalApi.Client.Helpers;
    using SFA.DAS.AssessorService.ExternalApi.Client.Properties;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private ViewModels.UpdateViewModel _ViewModel => DataContext as ViewModels.UpdateViewModel;

        public UpdateWindow()
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

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!_ViewModel.InvalidCertificates.View.IsEmpty)
            {
                string sMessageBoxText = "Do you want to continue?";
                string sCaption = "Invalid Certificates";

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (rsltMessageBox == MessageBoxResult.No)
                {
                    return;
                }
            }

            await UpdateCertificates();

        }

        private async Task UpdateCertificates()
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

                var results = await certificateApiClient.UpdateCertificates(_ViewModel.Certificates);

                var validCertificates = results.Where(r => r.Certificate != null);
                var invalidCertificates = results.Except(validCertificates);

                if (invalidCertificates.Any())
                {
                    SaveInvalidCertificates(invalidCertificates);
                }

                if (validCertificates.Any())
                {
                    SaveCertificates(validCertificates.Select(r => r.Certificate.CertificateData));
                }
            }
        }

        private void SaveInvalidCertificates(IEnumerable<BatchCertificateResponse> invalidCertificates)
        {
            string sMessageBoxText = "There were invalid certificates. Do you want to save these to a new file to amend?";
            string sCaption = "Invalid Certificates";

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (rsltMessageBox == MessageBoxResult.No)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var certificatesToSave = invalidCertificates.Select(ic => new { ic.ProvidedCertificateData, Errors = string.Join(", ", ic.ValidationErrors) });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, certificatesToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveCertificates(IEnumerable<CertificateData> certificates)
        {
            string sMessageBoxText = "Do you want to save the updated certificates?";
            string sCaption = "Save Certificates";

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (rsltMessageBox == MessageBoxResult.No)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                CsvFileHelper<CertificateData>.SaveToFile(saveFileDialog.FileName, certificates);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
