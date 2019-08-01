namespace SFA.DAS.AssessorService.ExternalApi.Client.ViewModels
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Certificates;
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

        public ObservableCollection<GetCertificateRequest> Certificates { get; private set; }

        public CollectionViewSource ValidCertificates { get; private set; }

        public CollectionViewSource InvalidCertificates { get; private set; }

        public GetViewModel()
        {
            Certificates = new ObservableCollection<GetCertificateRequest>();

            ValidCertificates = new CollectionViewSource();
            ValidCertificates.Filter += ValidCertificates_Filter;
            ValidCertificates.Source = Certificates;

            InvalidCertificates = new CollectionViewSource();
            InvalidCertificates.Filter += InvalidCertificates_Filter;
            InvalidCertificates.Source = Certificates;
        }

        private static void ValidCertificates_Filter(object sender, FilterEventArgs e)
        {
            GetCertificateRequest certificate = e.Item as GetCertificateRequest;
            e.Accepted = (certificate != null && certificate.IsValid(out _));
        }

        private static void InvalidCertificates_Filter(object sender, FilterEventArgs e)
        {
            GetCertificateRequest certificate = e.Item as GetCertificateRequest;
            e.Accepted = (certificate != null && !certificate.IsValid(out _));
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
