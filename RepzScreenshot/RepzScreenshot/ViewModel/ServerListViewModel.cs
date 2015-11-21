using RepzScreenshot.DataAccess;
using RepzScreenshot.Error;
using RepzScreenshot.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;

namespace RepzScreenshot.ViewModel
{
    class ServerListViewModel : WorkspaceViewModel, IDisposable
    {
        
        private Timer RefreshTimer = new Timer(10000);
        private bool autoRefresh = true;

        #region properties
        public ObservableCollection<ServerViewModel> Servers { get; private set; }
        
        public override string Title
        {
            get
            {
                return "Server Browser";
            }
            
        }

        public bool AutoRefresh
        {
            get
            {
                return autoRefresh;
            }
            set
            {
                if (autoRefresh != value)
                {
                    autoRefresh = value;
                    NotifyPropertyChanged("AutoRefresh");
                }
            }
        }




        #endregion // properties


        #region commands

        public Command RefreshCommand { get; private set; }
        #endregion commands


        #region constructor
        public ServerListViewModel():base(false)
        {
            Servers = new ObservableCollection<ServerViewModel>();

            

            RefreshCommand = new Command(CmdRefresh, CanRefresh);

            RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            RefreshTimer.AutoReset = false;

            this.PropertyChanged += ServerListViewModel_PropertyChanged;

            UpdateServers();
        }

        
        #endregion //constructor


        #region command methods

        private bool CanRefresh()
        {
            return !IsLoading;
        }

        private void CmdRefresh()
        {
            UpdateServers();
        }
        
        #endregion //command methods


        #region methods


        private async void UpdateServers()
        {
              
            IsLoading = true;
           
            try
            {
                await RepzDataAccess.UpdateCollection(Servers, RepzDataAccess.GetServersAsync, x => x.Server, x => Add(x), false);
                
                if(AutoRefresh)
                {
                    RefreshTimer.Start();
                }
                
            }
            catch(ExceptionBase ex)
            {
                SetError(ex, UpdateServers);
                
            }
            catch (Exception)
            {
                SetError(new Exception("Unknown error"), UpdateServers);
                
            }
            finally
            {
                IsLoading = false;
            }
            

        }
        private void Add(Server s)
        {
            ServerViewModel vm = new ServerViewModel(s);
            Servers.Add(vm);
        }

        public void Dispose()
        {
            RefreshTimer.Dispose();
            RefreshTimer = null;
        }

        private void AutoRefreshChanged()
        {
            RefreshTimer.Enabled = AutoRefresh;
        }

        #endregion //methods


        #region event handler methods

        void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                UpdateServers();
            });
            
        }

        void ServerListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "IsLoading":
                    RefreshCommand.NotifyCanExecuteChanged();
                    break;
                case "AutoRefresh":
                    AutoRefreshChanged();
                    break;
            }
        }

        #endregion //event handler methods


    }
}
