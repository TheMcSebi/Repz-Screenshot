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

        #region properties
        public ObservableCollection<ServerViewModel> Servers { get; private set; }
        
        public override string Title
        {
            get
            {
                return "Server Browser";
            }
            
        }

        private RepzDataAccess RepzDataAccess { get; set; }



        #endregion // properties


        #region commands

        #endregion commands


        #region constructor
        public ServerListViewModel():base(false)
        {
            RepzDataAccess = new RepzDataAccess();
            Servers = new ObservableCollection<ServerViewModel>();

            LoadServers();

            RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            RefreshTimer.AutoReset = false;
        }

        
        #endregion //constructor


        #region command methods

        
        #endregion //command methods


        #region methods

        
        private async void LoadServers()
        {
            IsLoading = true;
            try
            {
                List<Server> servers = await RepzDataAccess.GetServersAsync();
                Servers.Clear();

                foreach (Server s in servers)
                {
                    Servers.Add(new ServerViewModel(s));
                }
                RefreshTimer.Start();
            }
            catch(ExceptionBase ex)
            {
                SetError(ex, LoadServers);
            }
            catch(Exception)
            {
                SetError(new Exception("Unknown error"), LoadServers);
            }
            finally
            {
                IsLoading = false;
            }
            
        }

        private async void UpdateServers()
        {
              
            IsLoading = true;
           
            try
            {
                await RepzDataAccess.UpdateCollection(Servers, RepzDataAccess.GetServersAsync, x => x.Server, x => x.Npid, x => Add(x));
                RefreshTimer.Start();
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

            RepzDataAccess.Dispose();
            RepzDataAccess = null;
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

        #endregion //event handler methods


    }
}
