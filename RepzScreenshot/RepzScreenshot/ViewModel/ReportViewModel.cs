using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RepzScreenshot.ViewModel
{
    class ReportViewModel : ViewModelBase
    {

        private string reportReason;
        

        #region properties

        public PlayerViewModel PlayerViewModel { get; private set; }

        public string ReportReason
        {
            get
            {
                return reportReason;
            }

            set
            {
                if(reportReason != value)
                {
                    reportReason = value;
                    NotifyPropertyChanged("ReportReason");
                }
            }
        }

        public ObservableCollection<string> ImageUrls { get; private set; }



        #endregion


        #region constructor

        public ReportViewModel(PlayerViewModel pvm)
        {
            PlayerViewModel = pvm;
            ImageUrls = new ObservableCollection<string>();
        }

        #endregion


        #region methods

        public void AddImage(string url)
        {
            if(!ImageUrls.Contains(url))
                ImageUrls.Add(url);
        }

        #endregion
    }
}
