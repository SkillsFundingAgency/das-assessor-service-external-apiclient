namespace SFA.DAS.AssessorService.ExternalApi.Client.ViewModels.Learners
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Learners;
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

        public ObservableCollection<GetLearnerRequest> Requests { get; private set; }

        public CollectionViewSource ValidRequests { get; private set; }

        public CollectionViewSource InvalidRequests { get; private set; }

        public GetViewModel()
        {
            Requests = new ObservableCollection<GetLearnerRequest>();

            ValidRequests = new CollectionViewSource();
            ValidRequests.Filter += ValidRequests_Filter;
            ValidRequests.Source = Requests;

            InvalidRequests = new CollectionViewSource();
            InvalidRequests.Filter += InvalidRequests_Filter;
            InvalidRequests.Source = Requests;
        }

        private static void ValidRequests_Filter(object sender, FilterEventArgs e)
        {
            var request = e.Item as GetLearnerRequest;
            e.Accepted = (request != null && request.IsValid(out _));
        }

        private static void InvalidRequests_Filter(object sender, FilterEventArgs e)
        {
            var request = e.Item as GetLearnerRequest;
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
