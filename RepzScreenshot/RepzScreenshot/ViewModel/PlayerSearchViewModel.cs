using RepzScreenshot.DataAccess;
using RepzScreenshot.Error;
using RepzScreenshot.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RepzScreenshot.ViewModel
{
    class PlayerSearchViewModel : WorkspaceViewModel, IDataErrorInfo, IDisposable
    {
        private RepzDataAccess RepzDataAccess;
        private string searchQuery = String.Empty;

        #region properties

        public override string Title
        {
            get
            {
                return "Search Player";
            }

        }

        public string SearchQuery {
            get
            {
                return searchQuery;
            }
            set
            {
                if(searchQuery != value)
                {
                    searchQuery = value;
                    NotifyPropertyChanged("SearchQuery");
                }
            }
        }

        public ObservableCollection<PlayerViewModel> Players { get; private set; }



        #endregion //properties


        #region Commands

        public Command SearchCommand{ get; private set; }

        

        #endregion //commands


        #region Constructor

        public PlayerSearchViewModel() : base(false)
        {
            Players = new ObservableCollection<PlayerViewModel>();
            RepzDataAccess = new RepzDataAccess();

            InitCommands();

            this.PropertyChanged += PlayerSearchViewModel_PropertyChanged;
        }

        
        #endregion //constructor


        #region methods

        private void InitCommands()
        {
            SearchCommand = new Command(CmdSearch, CanSearch);
        }

        private void AddPlayer(Player p)
        {
            Players.Add(new PlayerViewModel(p));
        }

        private void RemovePlayer(Player p)
        {
            Players.Remove(Players.First(x => x.Player.OriginalName.Equals(p.OriginalName)));
        }

        public void Dispose()
        {
            RepzDataAccess.Dispose();
            RepzDataAccess = null;
        }

        #endregion //methods


        #region command methods

        private bool CanSearch()
        {
            return !IsLoading && SearchQuery != String.Empty;
        }

        private async void CmdSearch()
        {
            IsLoading = true;
            try
            {
                await RepzDataAccess.UpdateCollection<PlayerViewModel, Player>(Players, GetData, x => x.Player, x => x.OriginalName, AddPlayer);
            }
            catch(ExceptionBase ex)
            {
                SetError(ex, CmdSearch);
            }
            IsLoading = false;
        }

        private async Task<List<Player>> GetData()
        {
            return await RepzDataAccess.FindPlayers(SearchQuery);
        }

        #endregion //command methods

        #region events

        private void PlayerSearchViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "SearchQuery":
                case "IsLoading":
                    SearchCommand.NotifyCanExecuteChanged();
                    break;
            }
        }


        #endregion //events



        #region errors

        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case "SearchQuery":
                        if(SearchQuery == String.Empty || SearchQuery == null)
                        {
                            return "Name must not be empty";
                        }
                        break;
                }
                return null;
            }
        }

        #endregion
    }
}
