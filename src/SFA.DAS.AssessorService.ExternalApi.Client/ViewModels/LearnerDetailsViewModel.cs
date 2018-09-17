namespace SFA.DAS.AssessorService.ExternalApi.Client.ViewModels
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Search;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class LearnerDetailsViewModel : INotifyPropertyChanged
    {
        private long? _Uln;
        public long? Uln
        {
            get { return _Uln; }
            set
            {
                _Uln = value;
                OnPropertyChanged();
            }
        }

        private string _FamilyName;
        public string FamilyName
        {
            get { return _FamilyName; }
            set
            {
                _FamilyName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SearchResult> Results { get; private set; }

        public LearnerDetailsViewModel()
        {
            Results = new ObservableCollection<SearchResult>();
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
