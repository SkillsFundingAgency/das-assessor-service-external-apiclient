namespace SFA.DAS.AssessorService.ExternalApi.Client.ViewModels
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    public class UpdateViewModel : INotifyPropertyChanged
    {
        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CertificateData> Certificates { get; private set; }

        public CollectionViewSource ValidCertificates { get; private set; }

        public CollectionViewSource InvalidCertificates { get; private set; }

        public UpdateViewModel()
        {
            Certificates = new ObservableCollection<CertificateData>();

            ValidCertificates = new CollectionViewSource();
            ValidCertificates.Filter += ValidCertificates_Filter;
            ValidCertificates.Source = Certificates;

            InvalidCertificates = new CollectionViewSource();
            InvalidCertificates.Filter += InvalidCertificates_Filter;
            InvalidCertificates.Source = Certificates;
        }

        private static void ValidCertificates_Filter(object sender, FilterEventArgs e)
        {
            CertificateData certificate = e.Item as CertificateData;
            e.Accepted = (certificate != null && certificate.IsValid(out var validationResults));
        }

        private static void InvalidCertificates_Filter(object sender, FilterEventArgs e)
        {
            CertificateData certificate = e.Item as CertificateData;
            e.Accepted = (certificate != null && !certificate.IsValid(out var validationResults));
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion  
    }
}
