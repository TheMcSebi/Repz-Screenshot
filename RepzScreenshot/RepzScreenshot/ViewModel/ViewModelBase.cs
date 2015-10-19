using System.ComponentModel;

namespace RepzScreenshot.ViewModel
{
    abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isLoading;
        

        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            protected set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        



        protected void NotifyPropertyChanged(string propertyName)
        {

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
