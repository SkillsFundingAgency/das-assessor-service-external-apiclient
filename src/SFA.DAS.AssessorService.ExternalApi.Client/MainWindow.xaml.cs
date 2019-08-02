using Microsoft.Win32;
using SFA.DAS.AssessorService.ExternalApi.Client.Controls;
using SFA.DAS.AssessorService.ExternalApi.Client.Helpers;
using SFA.DAS.AssessorService.ExternalApi.Client.Properties;
using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace SFA.DAS.AssessorService.ExternalApi.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreateCertificates_Click(object sender, RoutedEventArgs e)
        {
            Controls.Certificates.CreateWindow window = new Controls.Certificates.CreateWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnUpdateCertificates_Click(object sender, RoutedEventArgs e)
        {
            Controls.Certificates.UpdateWindow window = new Controls.Certificates.UpdateWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnSubmitCertificates_Click(object sender, RoutedEventArgs e)
        {
            Controls.Certificates.SubmitWindow window = new Controls.Certificates.SubmitWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnDeleteCertificates_Click(object sender, RoutedEventArgs e)
        {
            Controls.Certificates.DeleteWindow window = new Controls.Certificates.DeleteWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnGetCertificates_Click(object sender, RoutedEventArgs e)
        {
            Controls.Certificates.GetWindow window = new Controls.Certificates.GetWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            Controls.Standards.StandardOptionsWindow window = new Controls.Standards.StandardOptionsWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private async void btnGrades_Click(object sender, RoutedEventArgs e)
        {
            await GetGrades();
        }

        private async Task GetGrades()
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

                var grades = new List<string>();

                try
                {
                    BusyIndicator.IsBusy = true;
                    var response = await certificateApiClient.GetGrades();
                    if (response != null)
                    {
                        grades.AddRange(response);
                    }
                }
                finally
                {
                    BusyIndicator.IsBusy = false;
                }

                if (grades.Any())
                {
                    SaveGrades(grades);
                }
            }
        }

        private void SaveGrades(IEnumerable<string> grades)
        {
            string sMessageBoxText = " Do you want to save the grades for later use?";
            string sCaption = "Save Grades";

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
                CsvFileHelper<dynamic>.SaveToFile(saveFileDialog.FileName, grades);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
