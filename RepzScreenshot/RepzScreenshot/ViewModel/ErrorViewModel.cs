using System;

namespace RepzScreenshot.ViewModel
{
    class ErrorViewModel : ViewModelBase
    {
        private string error;

        #region properties

        public String ErrorMessage
        {
            get
            {
                return error;
            }
            set
            {
                if(error != value)
                {
                    error = value;
                    NotifyPropertyChanged("ErrorMessage");
                }
            }
        }

        #endregion //properties

        #region commands

        public Command RetryCommand { get; private set; }

        #endregion //commands


        #region constructor
        

        public ErrorViewModel(Exception ex, Action retry)
        {
            RetryCommand = new Command(()=>CmdRetry(retry), ()=>true);
            ErrorMessage = ex.Message;
            RetryCommand.NotifyCanExecuteChanged();
            
        }

        #endregion constructor
        
        private void CmdRetry(Action retry)
        {
            ErrorMessage = null;
            retry();
        }
    }
}
