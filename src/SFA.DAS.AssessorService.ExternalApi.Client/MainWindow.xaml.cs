using SFA.DAS.AssessorService.ExternalApi.Client.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateWindow window = new CreateWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateWindow window = new UpdateWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            SubmitWindow window = new SubmitWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteWindow window = new DeleteWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnGet_Click(object sender, RoutedEventArgs e)
        {
            GetWindow window = new GetWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LearnerDetailsWindow window = new LearnerDetailsWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }
    }
}
