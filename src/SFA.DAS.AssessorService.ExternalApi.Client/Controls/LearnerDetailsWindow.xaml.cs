namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls
{
    using Microsoft.Win32;
    using SFA.DAS.AssessorService.ExternalApi.Client.Helpers;
    using SFA.DAS.AssessorService.ExternalApi.Client.Properties;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Search;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for LearnerDetailsWindow.xaml
    /// </summary>
    public partial class LearnerDetailsWindow : Window
    {
        private ViewModels.LearnerDetailsViewModel _ViewModel => DataContext as ViewModels.LearnerDetailsViewModel;

        public LearnerDetailsWindow()
        {
            InitializeComponent();

            DataObject.AddPastingHandler(txtUln, txtUln_PastingEvent);
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.Results.Clear();

            if (_ViewModel.Uln < 1000000000 && _ViewModel.Uln > 9999999999)
            {
                string sMessageBoxText = "The apprentice's ULN should contain exactly 10 numbers";
                string sCaption = "Invalid Input Detected";

                MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrWhiteSpace(_ViewModel.FamilyName))
            {
                string sMessageBoxText = "Please enter the apprentice's last name";
                string sCaption = "Invalid Input Detected";

                MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                await GetLearnerDetails();
            }
        }

        private async Task GetLearnerDetails()
        {
            string subscriptionKey = Settings.Default["SubscriptionKey"].ToString();
            string apiBaseAddress = Settings.Default["ApiBaseAddress"].ToString();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseAddress);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                SearchApiClient searchApiClient = new SearchApiClient(httpClient);
                IEnumerable<SearchResult> results;

                try
                {
                    BusyIndicator.IsBusy = true;
                    results = await searchApiClient.Search(_ViewModel.Uln.Value, _ViewModel.FamilyName);
                }
                finally
                {
                    BusyIndicator.IsBusy = false;
                }

                if (results.Any())
                {
                    foreach (var item in results)
                    {
                        _ViewModel.Results.Add(item);
                    }

                    SaveCertificates(results);
                }
                else
                {
                    string sMessageBoxText = "We were unable to find any results.";
                    string sCaption = "No Results";

                    MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void SaveCertificates(IEnumerable<SearchResult> results)
        {
            string sMessageBoxText = "Do you want to save the retrieved details?";
            string sCaption = "Save Learner Details";

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
                CsvFileHelper<SearchResult>.SaveToFile(saveFileDialog.FileName, results);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void txtUln_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt64(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void txtUln_PastingEvent(object sender, DataObjectPastingEventArgs e)
        {
            string clipboard = e.DataObject.GetData(typeof(string)) as string;
            try
            {
                Convert.ToInt64(clipboard);
            }
            catch
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }
    }

}
