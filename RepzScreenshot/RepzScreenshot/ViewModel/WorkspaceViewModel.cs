using System;

namespace RepzScreenshot.ViewModel
{
    abstract class WorkspaceViewModel : ViewModelBase
    {
        private ErrorViewModel error;

        public event EventHandler RequestClose;

        public Command CloseCommand { get; private set; }

        public virtual string Title { get; protected set; }

        public bool IsClosable { get; protected set; }

        public ErrorViewModel Error
        {
            get
            {
                return error;
            }
            set
            {
                if (error != value)
                {
                    error = value;
                    NotifyPropertyChanged("Error");
                }
            }
        }




        public WorkspaceViewModel():this(true)
        {}

        public WorkspaceViewModel(bool closable)
        {
            CloseCommand = new Command(this.OnRequestClose, CanClose);
            IsClosable = closable;

            this.PropertyChanged += WorkspaceViewModel_PropertyChanged;
        }

        

        private bool CanClose()
        {
            return IsClosable;
        }

        private void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }


        protected void SetError(Exception ex)
        {
            SetError(ex, null);
        }

        protected void SetError(Exception ex, Action retry)
        {
            if (ex == null)
            {
                Error = null;
            }
            else
            {
                Error = new ErrorViewModel(ex, retry);
            }
        }

        protected void RemoveError()
        {
            SetError(null);
        }

        #region event handler methods

        private void WorkspaceViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Error")
            {
                if(Error is ErrorViewModel)
                {
                    Error.PropertyChanged += Error_PropertyChanged;
                }
            }
        }

        private void Error_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ErrorMessage")
            {
                if(Error.ErrorMessage == String.Empty || Error.ErrorMessage == null)
                {
                    RemoveError();
                }
            }
        }

        #endregion //event handler methods


    }
}
