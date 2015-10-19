using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace RepzScreenshot.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {

        public static ObservableCollection<WorkspaceViewModel> Workspaces { get; private set; }
        
        public MainWindowViewModel()
        {
            Workspaces = new ObservableCollection<WorkspaceViewModel>();

            
            var w1 = new PlayerSearchViewModel();
            var w2 = new ServerListViewModel();
            AddWorkspace(w1);
            AddWorkspace(w2);

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

        }
        

        static void Workspace_RequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel ws = sender as WorkspaceViewModel;

            Workspaces.Remove(ws);

        }

        public static void AddWorkspace(WorkspaceViewModel ws)
        {

            ws.RequestClose += Workspace_RequestClose;
            Workspaces.Add(ws);
        }
        
        #region event handler methods

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                e.Handled = false;
                return;
            }

            string msg = String.Format("An unexpected error occurred!\n\nInfo:\n{0}", e.Exception.Message);

            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;

        }


        #endregion//event handler methods

    }
}
