﻿using RepzScreenshot.DataAccess;
using RepzScreenshot.Error;
using RepzScreenshot.Helper;
using RepzScreenshot.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Timers;

namespace RepzScreenshot.ViewModel
{
    class ServerViewModel : WorkspaceViewModel, IDisposable
    {

        Timer RefreshTimer = new Timer(5000);
        
        
        #region Properties

        public Server Server { get; private set; }

        private ServerDataAccess ServerDataAccess { get; set; }

        public override string Title
        {
            get
            {
                return Hostname;
            }

        }

        public string Hostname
        {
            get
            {
                return UIHelper.RemoveColor(Server.Hostname);
            }
            set
            {
                if (value != Server.Hostname)
                {
                    Server.Hostname = value;
                    NotifyPropertyChanged("Hostname");
                }
            }
        }

        public IPAddress Address 
        { 
            get 
            {
                return Server.Address;
            }
            set 
            {
                if (value != Server.Address)
                {
                    Server.Address = value;
                    NotifyPropertyChanged("Address");
                }
            }
        }

        public int Port
        {
            get
            {
                return Server.Port;
            }
            set
            {
                if (value != Server.Port)
                {
                    Server.Port = value;
                    NotifyPropertyChanged("Port");
                }
            }
        }

        public int Clients
        {
            get
            {
                return Server.Clients;
            }
        }
        public string ClientCount
        {
            get
            {
                return String.Format("{0} / {1}", Server.Clients.ToString(), Server.MaxClients.ToString());
            }
        }

        public string MapName
        {
            get
            {
                return UIHelper.GetMapName(Server.Map);
            }
            
        }

        public string GameType
        {
            get
            {

                return UIHelper.GetGameTypeName(Server.GameType);
            }
        }

        
        public ObservableCollection<PlayerViewModel> Players {get; private set; }
        
        
        #endregion //properties


        #region Commands
        public Command OpenCommand { get; private set; }
        
        #endregion //Commands


        #region constructor
        public ServerViewModel(Server server)
        {
            Server = server;

            Server.PropertyChanged += Server_PropertyChanged;

            ServerDataAccess = new ServerDataAccess(Server);

            OpenCommand = new Command(Open, CanOpen);
            Players = new ObservableCollection<PlayerViewModel>();

            RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            RefreshTimer.AutoReset = false;
        }


        #endregion //constructor


        #region command methods
        private bool CanOpen()
        {
            return !(MainWindowViewModel.Workspaces.Contains(this));
        }

        private void Open()
        {
            LoadPlayers();
            MainWindowViewModel.AddWorkspace(this);
            this.RequestClose += ServerViewModel_RequestClose;
            OpenCommand.NotifyCanExecuteChanged();
            RefreshTimer.Start();
        }

        #endregion //command methods


        #region methods
        private async void LoadPlayers()
        {
            IsLoading = true;
            try
            {
                List<Player> players = await ServerDataAccess.GetPlayersAsync();

                Players.Clear();
                foreach (Player p in players)
                {
                    Players.Add(new PlayerViewModel(p));
                }
            }
            catch (ExceptionBase ex)
            {
                SetError(ex, LoadPlayers);
            }
            catch (Exception)
            {
                SetError(new Exception("Unknown error"), LoadPlayers);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void UpdatePlayers()
        {
            
            IsLoading = true;
            try
            {
                await ServerDataAccess.UpdateCollection<PlayerViewModel, Player>(Players, ServerDataAccess.GetPlayersAsync, x => x.Player, x => x.OriginalName, Add);
                RefreshTimer.Start();
            }
            catch (ExceptionBase ex)
            {
                SetError(ex, UpdatePlayers);
                
            }
            catch (Exception)
            {
                SetError(new Exception("Unknown error"), UpdatePlayers);
                
            }
            finally
            {
                IsLoading = false;

            }
        }

        private void Add(Player p)
        {
            PlayerViewModel vm = new PlayerViewModel(p);
            Players.Add(vm);
        }

        public void Dispose()
        {
            RefreshTimer.Dispose();
            RefreshTimer = null;
        }

        #endregion //methods


        #region event handler methods

        void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                UpdatePlayers();
            });
            
        }

        void ServerViewModel_RequestClose(object sender, EventArgs e)
        {
            OpenCommand.NotifyCanExecuteChanged();
            RefreshTimer.Stop();
        }

        void Server_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string property = null;
            switch(e.PropertyName)
            {
                case "Hostname":
                    property = "Hostname";
                    break;
                case "Address":
                    property = "Address";
                    break;
                case "Port":
                    property = "Port";
                    break;
                case "Clients":
                case "MaxClients":
                    property = "ClientCount";
                    break;
                case "Map":
                    property = "Map";
                    break;
                case "GameType":
                    property = "GameType";
                    break;
            }
            if (property != null)
                NotifyPropertyChanged(property);
        }

        #endregion //event handler methods



       
    }
}