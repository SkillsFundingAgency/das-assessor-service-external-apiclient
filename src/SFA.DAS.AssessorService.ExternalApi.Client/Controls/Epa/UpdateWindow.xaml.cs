namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls.Epa
{
    using Microsoft.Win32;
    using SFA.DAS.AssessorService.ExternalApi.Client.Helpers;
    using SFA.DAS.AssessorService.ExternalApi.Client.Properties;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Epa;
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
        private ViewModels.Epa.UpdateViewModel _ViewModel => DataContext as ViewModels.Epa.UpdateViewModel;

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
                _ViewModel.Requests.Clear();

                var items = CsvFileHelper<UpdateEpaRequest>.GetFromFile(_ViewModel.FilePath, new Helpers.CsvClassMaps.UpdateEpaRequestMap());

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

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!_ViewModel.InvalidRequests.View.IsEmpty)
            {
                string sMessageBoxText = "Do you want to continue?";
                string sCaption = "Invalid Epa Records";

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (rsltMessageBox == MessageBoxResult.No)
                {
                    return;
                }
            }

            await UpdateEpaRecords();

        }

        private async Task UpdateEpaRecords()
        {
            string subscriptionKey = Settings.Default["SubscriptionKey"].ToString();
            string apiBaseAddress = Settings.Default["ApiBaseAddress"].ToString();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseAddress);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                EpaApiClient epaApiClient = new EpaApiClient(httpClient);
                IEnumerable<UpdateEpaResponse> results;

                try
                {
                    BusyIndicator.IsBusy = true;
                    results = await epaApiClient.UpdateEpaRecords(_ViewModel.Requests);
                }
                finally
                {
                    BusyIndicator.IsBusy = false;
                }

                var validEpaRecords = results.Where(r => r.EpaReference != null);
                var invalidEpaRecords = results.Except(validEpaRecords);

                if (invalidEpaRecords.Any())
                {
                    SaveInvalidEpaRecords(invalidEpaRecords);
                }

                if (validEpaRecords.Any())
                {
                    SaveEpaRecords(validEpaRecords);
                }
            }
        }

        private void SaveInvalidEpaRecords(IEnumerable<UpdateEpaResponse> invalidEpaRecords)
        {
            string sMessageBoxText = "There were invalid Epa Records. Do you want to save these to a new file to amend?";
            string sCaption = "Invalid Epa Records";

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
                var epasToSave = invalidEpaRecords.Select(ic => new { ic.RequestId, Errors = string.Join(", ", ic.ValidationErrors) });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, epasToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveEpaRecords(IEnumerable<UpdateEpaResponse> epaRecords)
        {
            string sMessageBoxText = "Do you want to save the updated Epa Records?";
            string sCaption = "Save Epa Records";

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
                var epasToSave = epaRecords.Select(ic => new { ic.RequestId, Errors = string.Join(", ", ic.ValidationErrors) });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, epasToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
