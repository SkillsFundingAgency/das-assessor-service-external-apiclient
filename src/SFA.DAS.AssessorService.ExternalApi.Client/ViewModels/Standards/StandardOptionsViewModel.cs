namespace SFA.DAS.AssessorService.ExternalApi.Client.ViewModels.Standards
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class StandardOptionsViewModel : INotifyPropertyChanged
    {
        private string _Standard;
        private string _Version;

        public string Standard
        {
            get { return _Standard; }
            set
            {
                _Standard = value;
                OnPropertyChanged();
            }
        }
        
        public string Version
        {
            get { return _Version; }
            set
            {
                _Version = value;
                OnPropertyChanged();
            }
        }

        public StandardOptionsViewModel()
        {
            Standard = string.Empty;
            Version = string.Empty;
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
