﻿namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls.Epa
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
    /// Interaction logic for CreateWindow.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        private ViewModels.Epa.CreateViewModel _ViewModel => DataContext as ViewModels.Epa.CreateViewModel;

        public CreateWindow()
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

                var items = CsvFileHelper<CreateEpaRequest>.GetFromFile(_ViewModel.FilePath);

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

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
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

            await CreateEpaRecords();
        }

        private async Task CreateEpaRecords()
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
                IEnumerable<CreateEpaResponse> results;

                try
                {
                    BusyIndicator.IsBusy = true;
                    results = await epaApiClient.CreateEpaRecords(_ViewModel.Requests);
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

        private void SaveInvalidEpaRecords(IEnumerable<CreateEpaResponse> invalidEpaRecords)
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
                var epasToSave = invalidEpaRecords.Select(ier => new { ier.RequestId, Errors = string.Join(", ", ier.ValidationErrors) });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, epasToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveEpaRecords(IEnumerable<CreateEpaResponse> epaRecords)
        {
            string sMessageBoxText = "Do you want to save the newly created EPA Records?";
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
                var epasToSave = epaRecords.Select(er => new { er.RequestId, er.EpaReference });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, epasToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
