namespace SFA.DAS.AssessorService.ExternalApi.Client.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class StandardOptionsViewModel : INotifyPropertyChanged
    {
        private string _Standard;
        public string Standard
        {
            get { return _Standard; }
            set
            {
                _Standard = value;
                OnPropertyChanged();
            }
        }

        public StandardOptionsViewModel()
        {
            Standard = string.Empty;
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
