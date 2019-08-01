namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls
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
    /// Interaction logic for GetWindow.xaml
    /// </summary>
    public partial class GetWindow : Window
    {
        private ViewModels.GetViewModel _ViewModel => DataContext as ViewModels.GetViewModel;

        public GetWindow()
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

                var items = CsvFileHelper<GetCertificateRequest>.GetFromFile(_ViewModel.FilePath);

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
                        _ViewModel.Certificates.Add(item);
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.Certificates.Clear();
            _ViewModel.FilePath = string.Empty;
        }

        private async void btnGet_Click(object sender, RoutedEventArgs e)
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

            await GetCertificates();
        }

        private async Task GetCertificates()
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

                var notYetCreatedCertificates = new List<GetCertificateResponse>();
                var validCertificates = new List<GetCertificateResponse>();
                var invalidCertificates = new List<GetCertificateResponse>();

                try
                {
                    BusyIndicator.IsBusy = true;

                    foreach (var certificate in _ViewModel.Certificates)
                    {
                        var response = await certificateApiClient.GetCertificate(certificate);

                        if (response.Error != null)
                        {
                            invalidCertificates.Add(response);
                        }
                        else if (response.Certificate != null)
                        {
                            validCertificates.Add(response);
                        }
                        else
                        {
                            notYetCreatedCertificates.Add(response);
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

                if (notYetCreatedCertificates.Any())
                {
                    SaveNotYetCertificates(notYetCreatedCertificates);
                }

                if (validCertificates.Any())
                {
                    SaveCertificates(validCertificates.Select(r => r.Certificate.CertificateData));
                }
            }
        }

        private void SaveNotYetCertificates(IEnumerable<GetCertificateResponse> invalidCertificates)
        {
            string sMessageBoxText = "There are certificates that are not yet created. Do you want to save these for later use?";
            string sCaption = "Not Yet Created Certificates";

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
                var certificatesToSave = invalidCertificates.Select(ic => new { ic.Uln, ic.FamilyName, ic.Standard, Message = "This certificate is not yet created" });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, certificatesToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveInvalidCertificates(IEnumerable<GetCertificateResponse> invalidCertificates)
        {
            string sMessageBoxText = "There were invalid requests. Do you want to save these to a new file to amend?";
            string sCaption = "Invalid Requests";

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
                var certificatesToSave = invalidCertificates.Select(ic => new { ic.Uln, ic.FamilyName, ic.Standard, Errors = ic.Error.Message });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, certificatesToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveCertificates(IEnumerable<CertificateData> certificates)
        {
            string sMessageBoxText = "Do you want to save the found certificates?";
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
