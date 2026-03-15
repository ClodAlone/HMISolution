using DevExpress.Xpf.Core;
using log4net;
using System;
using System.Windows;

namespace MovNextLogViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog _log = LogManager.GetLogger(Properties.Settings.Default.LoggerName);
        bool bLoaded, bClosed = false;

        public MainWindow()
        {
            InitializeComponent();
            
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bClosed)
                {
                    bLoaded = true;
                    mainWindow.Title = Properties.Settings.Default.MainTitle;
                    logViewer.FileOpened += OnFileOpened;
                    //logViewer.ToolbarBackground = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));
                    //_log.Info("Logging test succeeded!!");
                    LoadTheme();
                }
            };
        }

        void LoadTheme()
        {
            ThemeManager.SetThemeName(mainWindow, Properties.Settings.Default.ThemeName);
            //SkinStorage.SetVisualStyle(mainWindow, Properties.Settings.Default.VisualStyle);
        }

        void OnFileOpened(object sender, EventArgs e)
        {
            mainWindow.Title = string.Format("{0} ({1})", Properties.Settings.Default.MainTitle, logViewer.LogFileName);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            bClosed = true;
            logViewer.FileOpened -= OnFileOpened;
            logViewer.Dispose();
        }
    }
}
