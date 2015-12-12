using System;

namespace RepzScreenshot.Model
{
    class Player : ModelBase, IEquatable<Player>
    {

        private int id;
        private string name;
        private int score;
        private int ping;
        private string hostname;
        private string game;
        private string country;
        private bool screenshotTaken;


        #region properties
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(value != id)
                {
                    id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if (value != score)
                {
                    score = value;
                    NotifyPropertyChanged("Score");
                }
            }
        }

        public int Ping
        {
            get
            {
                return ping;
            }
            set
            {
                if (value != ping)
                {
                    ping = value;
                    NotifyPropertyChanged("Ping");
                }
            }
        }

        public string Hostname
        {
            get
            {
                return hostname;
            }
            set
            {
                if (hostname != value)
                {
                    hostname = value;
                    NotifyPropertyChanged("Hostname");
                }
            }
        }

        public string Game
        {
            get
            {
                return game;
            }
            set
            {
                if (game != value)
                {
                    game = value;
                    NotifyPropertyChanged("Game");
                }
            }
        }

        public string Country
        {
            get
            {
                return country;
            }
            set
            {
                if (country != value)
                {
                    country = value;
                    NotifyPropertyChanged("Country");
                }
            }
        }

        public bool ScreenshotTaken 
        {
            get
            {
                return screenshotTaken;
            }
            set
            {
                if(screenshotTaken != value)
                {
                    screenshotTaken = value;
                    NotifyPropertyChanged("ScreenshotTaken");
                }
            }
        }

        public string OriginalName { get; private set; }

        #endregion //properties


        #region constructor

        public Player(string name, int score, int ping)
        {
            Name = name;
            Score = score;
            Ping = ping;

            OriginalName = name;
        }

        public Player(int id, string name):this(name, 0, 0)
        {
            Id = id;
        }

        #endregion //constructor


        #region methods

        public override bool Update(ModelBase p)
        {
            if(p is Player)
            {
                return (UpdateProperty("Score", ((Player)p).Score) || UpdateProperty("Ping", ((Player)p).Ping));
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion //methods

        #region interfaces

        public bool Equals(Player p)
        {
            if(this.Id != 0 && p.Id != 0)
            {
                return (this.Id.Equals(p.Id));
            }
            else
            {
                return (this.OriginalName.Equals(p.OriginalName));
            }
        }

        #endregion //interfaces
    }
}
