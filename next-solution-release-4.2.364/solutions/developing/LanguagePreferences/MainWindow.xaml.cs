using DevExpress.Xpf.Core;
using Utilities;
using WPFUtilities;

namespace LanguagePreferences
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
    {
        public MainWindow()
        {
            ShowIcon = false;
            InitializeComponent();
            Loaded += (o, e) =>
            {
                ApplicationPropertiesHelper.SetProperty("CurrentSkin", (DataContext as LanguageViewModel).CurrentSkin);
                ThemeHelper.SetTheme(this);
            };
        }
    }
}
