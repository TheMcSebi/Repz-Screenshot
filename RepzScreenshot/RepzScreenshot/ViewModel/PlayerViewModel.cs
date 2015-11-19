using Microsoft.Win32;
using RepzScreenshot.DataAccess;
using RepzScreenshot.Error;
using RepzScreenshot.Helper;
using RepzScreenshot.Model;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RepzScreenshot.ViewModel
{
    class PlayerViewModel : WorkspaceViewModel
    {

        private BitmapImage screenshot;
        private BitmapImage infoScreenshot;
        private string screenshotUrl;
        private DateTime? screenshotDate;
        private bool isScreenUploaded;
        private bool drawUserInfo = true;

        #region properties
        public Player Player { get; private set; }


        private static ReportListViewModel ReportListViewModel { get; set; }

        private ReportViewModel Report { get; set; }

        public override string Title
        {
            get
            {
                return CleanName;
            }
        }

        public int PlayerId
        {
            get
            {
                return Player.Id;
            }
            set
            {
                if (value != Player.Id)
                {
                    Player.Id = value;
                    NotifyPropertyChanged("PlayerId");
                }
            }
        }

        public string PlayerName 
        { 
            get 
            {
                return Player.Name;
            }
            set 
            { 
                if(value != Player.Name)
                {
                    Player.Name = value;
                    NotifyPropertyChanged("PlayerName");
                }
            }
        }

        public string CleanName
        {
            get
            {
                return UIHelper.RemoveColor(PlayerName);
            }
        }

        public int PlayerScore
        {
            get
            {
                return Player.Score;
            }
            set
            {
                if (value != Player.Score)
                {
                    Player.Score = value;
                    NotifyPropertyChanged("PlayerScore");
                }
            }
        }

        public int PlayerPing
        {
            get
            {
                return Player.Ping;
            }
            set
            {
                if (value != Player.Ping)
                {
                    Player.Ping = value;
                    NotifyPropertyChanged("PlayerPing");
                }
            }
        }

        public string PlayerGame
        {
            get
            {
                return UIHelper.GetGameName(Player.Game);
            }
        }

        public string PlayerCountry
        {
            get
            {
                return Player.Country;
            }
        }

        public bool DrawUserInfo
        {
            get
            {
                return drawUserInfo;
            }
            set
            {
                if (value != drawUserInfo)
                {
                    drawUserInfo = value;
                    NotifyPropertyChanged("DrawUserInfo");
                }
            }
        }
        
        public BitmapImage Screenshot
        {
            get
            {
                return DrawUserInfo ? infoScreenshot : screenshot;
            }
            set
            {
                if(screenshot != value)
                {
                    screenshot = value;
                    NotifyPropertyChanged("Screenshot");
                    
                }
            }
        }

        public string ScreenshotUrl
        {
            get
            {
                return screenshotUrl;
            }
            set
            {
                if (screenshotUrl != value)
                {
                    screenshotUrl = value;
                    NotifyPropertyChanged("ScreenshotUrl");
                    
                }
            }
        }

        public bool IsScreenUploaded
        {
            get
            {
                return isScreenUploaded;
            }
            set
            {
                if (isScreenUploaded != value)
                {
                    isScreenUploaded = value;
                    NotifyPropertyChanged("IsScreenUploaded");

                }
            }
        }

        
        public DateTime? ScreenshotDate
        {
            get
            {
                return screenshotDate;
            }
            private set
            {
                if (screenshotDate != value)
                {
                    screenshotDate = value;
                    NotifyPropertyChanged("ScreenshotDate");
                }
            }
        }

        
        public string ServerHostname
        {
            get
            {
                return UIHelper.RemoveColor(Player.Hostname);
            }
            set
            {
                if (Player.Hostname != value)
                {
                    Player.Hostname = value;
                    NotifyPropertyChanged("ServerHostname");
                }
            }
        }

        public Brush StatusBrush {
            get
            {
                if (Error is ErrorViewModel && Error.ErrorMessage != String.Empty)
                    return Brushes.Red;
                else if (IsLoading)
                    return Brushes.Yellow;
                else if (Screenshot is BitmapImage)
                    return MainWindowViewModel.Workspaces.Contains(this) ? Brushes.Green : Brushes.LightGreen;
                
                else
                    return null;
            }
        }

        #endregion //properties


        #region Commands

        public Command ScreenshotCommand { get; protected set; }
        public Command ReloadCommand { get; protected set; }
        public Command SaveImageCommand { get; protected set; }

        public Command UploadImageCommand { get; protected set; }

        public Command AddReportCommand { get; protected set; }


        private void InitCommands()
        {
            ScreenshotCommand = new Command(CmdGetScreenshot, CanGetScreenshot);
            ReloadCommand = new Command(CmdReload, CanReload);
            SaveImageCommand = new Command(CmdSaveImage, CanSaveImage);
            UploadImageCommand = new Command(CmdUploadImage, CanUploadImage);
            AddReportCommand = new Command(CmdAddReport, CanAddReport);
        }
        #endregion //Commands


        #region constructor

        public PlayerViewModel(Player player)
        {
            Player = player;

            Player.PropertyChanged += Player_PropertyChanged;
            this.PropertyChanged += PlayerViewModel_PropertyChanged;
            InitCommands();

            Report = new ReportViewModel(this);
        }

        static PlayerViewModel()
        {
            ReportListViewModel = new ReportListViewModel();
        }

        #endregion


        #region Command methods

        private bool CanGetScreenshot()
        {
            return !(MainWindowViewModel.Workspaces.Contains(this));
        }

        private void CmdGetScreenshot()
        {
            Open();
        }

        private bool CanReload()
        {
            return (!IsLoading && Error == null);
        }

        private void CmdReload()
        {
            GetScreenshot();
        }

        private bool CanSaveImage()
        {
            return (Screenshot is BitmapImage);
        }

        private void CmdSaveImage()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save Screenshot";
            saveFileDialog1.FileName = Player.Name;
            if (saveFileDialog1.ShowDialog() == true)
            {

                if (saveFileDialog1.FileName != "")
                {
                    using (System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile())
                    {

                        BitmapEncoder encoder;
                        switch (saveFileDialog1.FilterIndex)
                        {
                            default:
                                encoder = new JpegBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(Screenshot));
                                break;

                            case 2:
                                encoder = new BmpBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(Screenshot));
                                break;

                            case 3:
                                encoder = new GifBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(Screenshot));
                                break;
                        }
                        encoder.Save(fs);
                       
                    }
                }
            }
        }

        private bool CanUploadImage()
        {
            return CanSaveImage() && !IsScreenUploaded && !IsLoading;
        }

        private async void CmdUploadImage()
        {
            IsLoading = true;
            try
            {
                string url = await ImgurDataAccess.UploadImage(Player, Screenshot, ScreenshotDate);
                ScreenshotUrl = url;
                IsScreenUploaded = true;
            }
            catch (ExceptionBase ex)
            {
                SetError(ex, CmdUploadImage);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanAddReport()
        {
            return IsScreenUploaded && !this.Report.ImageUrls.Contains(ScreenshotUrl);
        }

        private void CmdAddReport()
        {
            if(!MainWindowViewModel.Workspaces.Contains(ReportListViewModel))
                MainWindowViewModel.AddWorkspace(ReportListViewModel);

            ReportListViewModel.AddReport(Report);
            Report.AddImage(ScreenshotUrl);
            
            AddReportCommand.NotifyCanExecuteChanged();
        }

        #endregion


        #region methods

        public void Open()
        {
            MainWindowViewModel.AddWorkspace(this);
            NotifyPropertyChanged("StatusBrush");
            this.RequestClose += PlayerViewModel_RequestClose;
            ScreenshotCommand.NotifyCanExecuteChanged();

            if (Screenshot == null && Error == null)
                GetScreenshot();
        }


        private async void GetScreenshot()
        {
            IsLoading = true;
            try
            {
                
                if(Player.Id == 0)
                {
                    await RepzDataAccess.GetIdAsync(Player);
                }
           
                await RepzDataAccess.GetPresenceDataAsync(Player);
                Screenshot = await RepzDataAccess.GetScreenshotAsync(Player);
                ScreenshotDate = DateTime.Now;
                IsScreenUploaded = false;
            }
            catch (ExceptionBase ex)
            {
                SetError(ex, CmdReload);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsLoading = false;
            }
           
        }

        private BitmapImage DrawInfo(BitmapImage bitmap)
        {
            if (bitmap == null || ScreenshotDate == null)
                return bitmap;
            FormattedText text = new FormattedText(
                String.Format("ID: {0}\nName: {1}\nDate: {2}", Player.Id, Player.Name, ScreenshotDate.Value.ToString("yyyy-MM-dd HH:mm")),
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segeo UI"),
                20,
                Brushes.Red);


            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawImage(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                dc.DrawRectangle(Brushes.Gray, new Pen(), new Rect(0, 0, text.Width+5, 80));
                dc.DrawText(text, new Point(2,2));
                
            }

            RenderTargetBitmap target = new RenderTargetBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, PixelFormats.Default);
            target.Render(visual);

            BitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(target));

            var newimage = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                newimage.BeginInit();
                newimage.CacheOption = BitmapCacheOption.OnLoad;
                newimage.StreamSource = stream;
                newimage.EndInit();
            }
            return newimage;
        }
        

        #endregion //methods


        #region event handler methods

        void PlayerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "IsLoading":
                    ReloadCommand.NotifyCanExecuteChanged();
                    NotifyPropertyChanged("StatusBrush");
                    UploadImageCommand.NotifyCanExecuteChanged();
                    break;
                case "Error":
                    NotifyPropertyChanged("StatusBrush");
                    ReloadCommand.NotifyCanExecuteChanged();
                    break;
                case "PlayerName":
                    NotifyPropertyChanged("Title");
                    NotifyPropertyChanged("CleanName");
                    break;
                case "IsScreenUploaded":
                    UploadImageCommand.NotifyCanExecuteChanged();
                    AddReportCommand.NotifyCanExecuteChanged();
                    break;
                case "DrawUserInfo":
                    NotifyPropertyChanged("Screenshot");
                    break;
                case "ScreenshotDate":
                    infoScreenshot = DrawInfo(screenshot);
                    NotifyPropertyChanged("Screenshot");
                    break;
                case "Screenshot":
                    SaveImageCommand.NotifyCanExecuteChanged();
                    UploadImageCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        void PlayerViewModel_RequestClose(object sender, EventArgs e)
        {
            ScreenshotCommand.NotifyCanExecuteChanged();
            NotifyPropertyChanged("StatusBrush");
        }

        void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string property = null;
            switch(e.PropertyName)
            {
                case "Id":
                    property = "PlayerId";
                    break;
                case "Name":
                    property = "PlayerName";
                    break;
                case "Score":
                    property = "PlayerScore";
                    break;
                case "Ping":
                    property = "PlayerPing";
                    break;
                case "Hostname":
                    property = "ServerHostname";
                    break;
                case "Game":
                    property = "PlayerGame";
                    break;
                case "Country":
                    property = "PlayerCountry";
                    break;
                    
            }
            if (property != null)
                NotifyPropertyChanged(property);
        }

        #endregion

        
    }
}
