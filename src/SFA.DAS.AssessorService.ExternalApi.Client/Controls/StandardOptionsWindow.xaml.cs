namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls
{
    using Microsoft.Win32;
    using SFA.DAS.AssessorService.ExternalApi.Client.Helpers;
    using SFA.DAS.AssessorService.ExternalApi.Client.Properties;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Standards;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for StandardOptionsWindow.xaml
    /// </summary>
    public partial class StandardOptionsWindow : Window
    {
        private ViewModels.StandardOptionsViewModel _ViewModel => DataContext as ViewModels.StandardOptionsViewModel;

        public StandardOptionsWindow()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.Standard = string.Empty;
        }

        private async void btnGet_Click(object sender, RoutedEventArgs e)
        {
            await GetStandardOptions();
        }

        private async Task GetStandardOptions()
        {
            string subscriptionKey = Settings.Default["SubscriptionKey"].ToString();
            string apiBaseAddress = Settings.Default["ApiBaseAddress"].ToString();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseAddress);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                StandardsApiClient standardsApiClient = new StandardsApiClient(httpClient);

                List<StandardOptions> options = new List<StandardOptions>();
                try
                {
                    BusyIndicator.IsBusy = true;

                    if (string.IsNullOrWhiteSpace(_ViewModel.Standard))
                    {
                        var response = await standardsApiClient.GetOptionsForAllStandards();
                        if (response != null)
                        {
                            options.AddRange(response);
                        }
                    }
                    else
                    {
                        var response = await standardsApiClient.GetOptionsForStandard(_ViewModel.Standard);
                        if (response != null)
                        {
                            options.Add(response);
                        }
                    }
                }
                finally
                {
                    BusyIndicator.IsBusy = false;
                }

                if (options.Any())
                {
                    SaveStandardOptions(options);
                }
                else
                {
                    string sMessageBoxText = "There are no options for the specified standard";
                    string sCaption = "No Options Found";

                    MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SaveStandardOptions(IEnumerable<StandardOptions> options)
        {
            string sMessageBoxText = "Do you want to save the standard options?";
            string sCaption = "Save Standard Options?";

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
                CsvFileHelper<StandardOptions>.SaveToFile(saveFileDialog.FileName, options);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
