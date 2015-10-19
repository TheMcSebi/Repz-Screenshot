namespace RepzScreenshot.Model
{
    class Player : ModelBase
    {

        private int id;
        private string name;
        private int score;
        private int ping;
        private string hostname;


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

        public string OriginalName { get; private set; }

        #endregion //properties

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

        public override void Update(ModelBase p)
        {
            if(p is Player)
            {
                Score = ((Player)p).Score;
                Ping = ((Player)p).Ping;
            }
            
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
