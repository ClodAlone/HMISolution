using System;
using System.Windows;
using System.Windows.Controls;
using Utilities.WPF;
using System.Collections.Generic;
using System.Threading;
using ScreenManager.ComponentService;
using UFProjectManager.ComponentService;
using System.ComponentModel;
using DeployServer.Processes;

namespace UFProjectManager.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProcessOutput : UserControl, INotifyPropertyChanged
    {
        #region Declarations
        bool bLoaded;
        public event PropertyChangedEventHandler PropertyChanged;
        Window wnd;
        #endregion
        
        #region Properties
        string processOut;
        public string ProcessOut
        {
            get
            {
                return processOut;
            }
            set
            {
                if (value != processOut)
                {
                    processOut = value;
                    OnPropertyChanged("ProcessOut");
                }
            }
        }
        #endregion

        #region Ctor
        public ProcessOutput(string processOutput)
        {
            InitializeComponent();

            DataContext = this;
            ProcessOut = processOutput;
            
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    wnd = this.FindParent<Window>();
                    //if (wnd != null)
                    //{
                    //    wnd.ContentRendered += OnContentRendered;
                    //    wnd.Closing += (s, ea) =>
                    //    {
                    //        wnd.ContentRendered -= OnContentRendered;
                    //        projectManagerComponent.Workspace.ProgressStateChanged -= OnProgressStateChanged;
                    //    };
                    //}
                }
            };
        }
        #endregion
        #region Methods
        

        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
