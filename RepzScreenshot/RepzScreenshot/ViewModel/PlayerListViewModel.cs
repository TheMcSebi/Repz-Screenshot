using RepzScreenshot.DataAccess;
using RepzScreenshot.Error;
using RepzScreenshot.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RepzScreenshot.ViewModel
{
    class PlayerListViewModel : WorkspaceViewModel, IDisposable
    {
        private bool autoRefresh;

        #region properties

        public override string Title
        {
            get
            {
                return "Players";
            }
        }

        public ServerViewModel ServerVM { get; private set; }

        public ObservableCollection<PlayerViewModel> Players
        {
            get
            {
                return ServerVM.Players;
            }
        }

        public Timer RefreshTimer { get; private set; }

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

        #endregion //properties

        #region commands

        public ParameterCommand<PlayerViewModel> OpenPlayerCommand { get; private set; }
        public Command RefreshCommand { get; private set; }
        public Command GetAllCommand { get; private set; }

        #endregion //commands


        #region constructor

        public PlayerListViewModel(ServerViewModel server):base(false)
        {
            ServerVM = server;

            OpenPlayerCommand = new ParameterCommand<PlayerViewModel>(CmdOpenPlayer, CanOpenPlayer);
            RefreshCommand = new Command(CmdRefresh, CanRefresh);
            GetAllCommand = new Command(CmdGetAll, CanGetAll);

            RefreshTimer = new Timer(5000);

            RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            RefreshTimer.AutoReset = false;

            ServerVM.OpenWorkspace +=ServerVM_OpenWorkspace;
            ServerVM.RequestClose += ServerVM_RequestClose;
            this.OpenWorkspace += PlayerListViewModel_OpenWorkspace;
            this.PropertyChanged += PlayerListViewModel_PropertyChanged;
        }

        
        #endregion //constructor


        #region command methods

        private bool CanOpenPlayer(PlayerViewModel pvm)
        {
            return !ServerVM.Tabs.Contains(pvm);
        }

        private void CmdOpenPlayer(PlayerViewModel pvm)
        {
            ServerVM.Tabs.Add(pvm);
            pvm.RequestClose += PlayerViewModel_RequestClose;
            pvm.OnOpen();
            OpenPlayerCommand.NotifyCanExecuteChanged();
        }

        private bool CanRefresh()
        {
            return !IsLoading;
        }

        private void CmdRefresh()
        {
            UpdatePlayers();
        }

        private bool CanGetAll()
        {
            return Players.Any(x => x.Screenshot == null && !ServerVM.Tabs.Contains(x) && x.Error == null);

        }

        private void CmdGetAll()
        {
            List<PlayerViewModel> toDo = Players.Where(x => x.Screenshot == null && !ServerVM.Tabs.Contains(x) && x.Error == null).ToList();
            foreach (PlayerViewModel p in toDo)
            {
                ServerVM.Tabs.Add(p);
                p.OnOpen();
                p.RequestClose += PlayerViewModel_RequestClose;
                OpenPlayerCommand.NotifyCanExecuteChanged();
            }
        }

        #endregion //command methods
        
        #region methods

        private async void UpdatePlayers()
        {

            IsLoading = true;
            try
            {
                await ServerDataAccess.UpdateCollection<PlayerViewModel, Player>(Players, ServerVM.ServerDataAccess.GetPlayersAsync, x => x.Player, Add, true);

            }
            catch (ExceptionBase ex)
            {
                SetError(ex, UpdatePlayers);

            }
            catch (Exception ex)
            {
                SetError(new Exception("Unknown error"), UpdatePlayers);
                throw ex;
            }
            finally
            {
                IsLoading = false;
                GetAllCommand.NotifyCanExecuteChanged();
            }
        }

        private void Add(Player p)
        {
            Players.Add(RepzDataAccess.GetPlayerVM(p));
        }

        public void Dispose()
        {
            RefreshTimer.Dispose();
            RefreshTimer = null;
        }

        #endregion //methods

        #region event handler methods

        private void PlayerViewModel_RequestClose(object sender, EventArgs e)
        {
            if (sender is PlayerViewModel)
            {
                PlayerViewModel pvm = (PlayerViewModel)sender;
                ServerVM.Tabs.Remove(pvm);
                pvm.RequestClose -= PlayerViewModel_RequestClose;
                OpenPlayerCommand.NotifyCanExecuteChanged();
            }

        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                UpdatePlayers();
                if (Error == null && AutoRefresh)
                    RefreshTimer.Start();
            });

        }

        private void ServerVM_OpenWorkspace(object sender, EventArgs e)
        {
            this.OnOpen();
        }

        private void ServerVM_RequestClose(object sender, EventArgs e)
        {
            AutoRefresh = false;
        }

        private void PlayerListViewModel_OpenWorkspace(object sender, EventArgs e)
        {
            UpdatePlayers();
            AutoRefresh = true;
        }

        private void PlayerListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Error":
                    if (Error == null && !RefreshTimer.Enabled && AutoRefresh)
                        RefreshTimer.Start();
                    break;
                case "IsLoading":
                    RefreshCommand.NotifyCanExecuteChanged();
                    break;
                case "AutoRefresh":
                    RefreshTimer.Enabled = AutoRefresh;
                    break;
            }
        }


        #endregion //event handler methods
    }
}
