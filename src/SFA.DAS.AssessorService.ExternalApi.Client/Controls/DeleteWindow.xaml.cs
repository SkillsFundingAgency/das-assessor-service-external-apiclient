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
    /// Interaction logic for DeleteWindow.xaml
    /// </summary>
    public partial class DeleteWindow : Window
    {
        private ViewModels.DeleteViewModel _ViewModel => DataContext as ViewModels.DeleteViewModel;

        public DeleteWindow()
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

                foreach (var item in CsvFileHelper<DeleteCertificate>.GetFromFile(_ViewModel.FilePath))
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

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
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

            await DeleteCertificates();
        }

        private async Task DeleteCertificates()
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

                List<DeleteBatchCertificateResponse> invalidCertificates = new List<DeleteBatchCertificateResponse>();

                try
                {
                    BusyIndicator.IsBusy = true;

                    foreach (var certificate in _ViewModel.Certificates)
                    {
                        var response = await certificateApiClient.DeleteCertificate(certificate);

                        if (response.Error != null)
                        {
                            invalidCertificates.Add(response);
                        }
                    }
                }
                finally
                {
                    BusyIndicator.IsBusy = false;
                }

                if (invalidCertificates.Any())
                {
                    SaveInvalidCertificates(invalidCertificates);
                }
                else
                {
                    string sMessageBoxText = "Certificates are now deleted.";
                    string sCaption = "Delete Certificates";

                    MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void SaveInvalidCertificates(IEnumerable<DeleteBatchCertificateResponse> invalidCertificates)
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
                var certificatesToSave = invalidCertificates.Select(ic => new { ic.Uln, ic.FamilyName, ic.StandardCode, Errors = ic.Error?.Message });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, certificatesToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
