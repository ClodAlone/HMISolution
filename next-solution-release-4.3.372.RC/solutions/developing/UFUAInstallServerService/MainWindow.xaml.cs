using Utilities;
using UFUAServiceLibrary;
using DevExpress.Xpf.Core;
using WPFUtilities;

namespace UFUAInstallServerService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
    {
        #region Declarations
        internal CommadLineOptions cmdOptions;
        bool bLoaded;
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            ShowIcon = false;
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    ApplicationPropertiesHelper.SetProperty("CurrentSkin", cmdOptions.CurrentSkin);
                    ThemeHelper.SetTheme(this);

                    if (!string.IsNullOrEmpty(cmdOptions.Title))
                        Title = cmdOptions.Title;

                    serviceControl.cmdOptions = cmdOptions;
                }
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                }
            };        
        }
        #endregion
    }
}
