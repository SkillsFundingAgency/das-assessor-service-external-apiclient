﻿namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls.Certificates
{
    using Microsoft.Win32;
    using SFA.DAS.AssessorService.ExternalApi.Client.Helpers;
    using SFA.DAS.AssessorService.ExternalApi.Client.Properties;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Certificates;
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
        private ViewModels.Certificates.SubmitViewModel _ViewModel => DataContext as ViewModels.Certificates.SubmitViewModel;

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
                _ViewModel.Requests.Clear();

                var items = CsvFileHelper<SubmitCertificateRequest>.GetFromFile(_ViewModel.FilePath);

                if (items is null || !items.Any())
                {
                    string sMessageBoxText = "The file you selected has invalid data or is empty";
                    string sCaption = "Invalid File Selected";

                    MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    foreach (var item in items)
                    {
                        _ViewModel.Requests.Add(item);
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.Requests.Clear();
            _ViewModel.FilePath = string.Empty;
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!_ViewModel.InvalidRequests.View.IsEmpty)
            {
                string sMessageBoxText = "Do you want to continue?";
                string sCaption = "Invalid Certificates";

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (rsltMessageBox == MessageBoxResult.No)
                {
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
                IEnumerable<SubmitCertificateResponse> results;

                try
                {
                    BusyIndicator.IsBusy = true;
                    results = await certificateApiClient.SubmitCertificates(_ViewModel.Requests);
                }
                finally
                {
                    BusyIndicator.IsBusy = false;
                }

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

        private void SaveInvalidCertificates(IEnumerable<SubmitCertificateResponse> invalidCertificates)
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
                var certificatesToSave = invalidCertificates.Select(ic => new { ic.RequestId, Errors = string.Join(", ", ic.ValidationErrors) });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, certificatesToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveCertificates(IEnumerable<CertificateData> certificates)
        {
            string sMessageBoxText = "Do you want to save the newly submitted certificates?";
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
