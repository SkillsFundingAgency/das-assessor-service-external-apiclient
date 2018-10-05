namespace SFA.DAS.AssessorService.ExternalApi.Client.ViewModels
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    public class GetViewModel : INotifyPropertyChanged
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

        public ObservableCollection<GetCertificate> Certificates { get; private set; }

        public CollectionViewSource ValidCertificates { get; private set; }

        public CollectionViewSource InvalidCertificates { get; private set; }

        public GetViewModel()
        {
            Certificates = new ObservableCollection<GetCertificate>();

            ValidCertificates = new CollectionViewSource();
            ValidCertificates.Filter += ValidCertificates_Filter;
            ValidCertificates.Source = Certificates;

            InvalidCertificates = new CollectionViewSource();
            InvalidCertificates.Filter += InvalidCertificates_Filter;
            InvalidCertificates.Source = Certificates;
        }

        private static void ValidCertificates_Filter(object sender, FilterEventArgs e)
        {
            GetCertificate certificate = e.Item as GetCertificate;
            e.Accepted = (certificate != null && certificate.IsValid(out var validationResults));
        }

        private static void InvalidCertificates_Filter(object sender, FilterEventArgs e)
        {
            GetCertificate certificate = e.Item as GetCertificate;
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
