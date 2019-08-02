namespace SFA.DAS.AssessorService.ExternalApi.Client.ViewModels.Epa
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    public class CreateViewModel : INotifyPropertyChanged
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

        public ObservableCollection<CreateEpaRequest> Requests { get; private set; }

        public CollectionViewSource ValidRequests { get; private set; }

        public CollectionViewSource InvalidRequests { get; private set; }

        public CreateViewModel()
        {
            Requests = new ObservableCollection<CreateEpaRequest>();

            ValidRequests = new CollectionViewSource();
            ValidRequests.Filter += ValidRequests_Filter;
            ValidRequests.Source = Requests;

            InvalidRequests = new CollectionViewSource();
            InvalidRequests.Filter += InvalidRequests_Filter;
            InvalidRequests.Source = Requests;
        }

        private static void ValidRequests_Filter(object sender, FilterEventArgs e)
        {
            var request = e.Item as CreateEpaRequest;
            e.Accepted = (request != null && request.IsValid(out _));
        }

        private static void InvalidRequests_Filter(object sender, FilterEventArgs e)
        {
            var request = e.Item as CreateEpaRequest;
            e.Accepted = (request != null && !request.IsValid(out _));
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
