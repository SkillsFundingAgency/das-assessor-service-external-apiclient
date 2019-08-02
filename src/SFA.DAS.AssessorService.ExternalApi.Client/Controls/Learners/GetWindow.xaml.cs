namespace SFA.DAS.AssessorService.ExternalApi.Client.Controls.Learners
{
    using Microsoft.Win32;
    using SFA.DAS.AssessorService.ExternalApi.Client.Helpers;
    using SFA.DAS.AssessorService.ExternalApi.Client.Properties;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Learners;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Learners;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners;
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
        private ViewModels.Learners.GetViewModel _ViewModel => DataContext as ViewModels.Learners.GetViewModel;

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
                _ViewModel.Requests.Clear();

                var items = CsvFileHelper<GetLearnerRequest>.GetFromFile(_ViewModel.FilePath);

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

        private async void btnGet_Click(object sender, RoutedEventArgs e)
        {
            if (!_ViewModel.InvalidRequests.View.IsEmpty)
            {
                string sMessageBoxText = "Do you want to continue?";
                string sCaption = "Invalid Learners";

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

                LearnerApiClient learnerApiClient = new LearnerApiClient(httpClient);

                var notFoundLearners = new List<GetLearnerResponse>();
                var validLearners = new List<GetLearnerResponse>();
                var invalidLearners = new List<GetLearnerResponse>();

                try
                {
                    BusyIndicator.IsBusy = true;

                    foreach (var learner in _ViewModel.Requests)
                    {
                        var response = await learnerApiClient.GetLearner(learner);

                        if (response.Error != null)
                        {
                            invalidLearners.Add(response);
                        }
                        else if (response.Learner != null)
                        {
                            validLearners.Add(response);
                        }
                        else
                        {
                            notFoundLearners.Add(response);
                        }
                    }
                }
                finally
                {
                    BusyIndicator.IsBusy = false;
                }

                if (invalidLearners.Any())
                {
                    SaveInvalidLearners(invalidLearners);
                }

                if (notFoundLearners.Any())
                {
                    SaveNotFoundLearners(notFoundLearners);
                }

                if (validLearners.Any())
                {
                    SaveLearners(validLearners.Select(r => r.Learner));
                }
            }
        }

        private void SaveNotFoundLearners(IEnumerable<GetLearnerResponse> invalidLearners)
        {
            string sMessageBoxText = "There are learners which are not found. Do you want to save these for later use?";
            string sCaption = "Not Found Learners";

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
                var learnersToSave = invalidLearners.Select(ic => new { ic.Uln, ic.FamilyName, ic.Standard, Message = "This learner is not found" });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, learnersToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveInvalidLearners(IEnumerable<GetLearnerResponse> invalidLearners)
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
                var learnersToSave = invalidLearners.Select(ic => new { ic.Uln, ic.FamilyName, ic.Standard, Errors = ic.Error.Message });

                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, learnersToSave);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void SaveLearners(IEnumerable<Learner> learners)
        {
            string sMessageBoxText = "Do you want to save the found learners?";
            string sCaption = "Save Learners";

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
                CsvFileHelper<Learner>.SaveToFile(saveFileDialog.FileName, learners);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
