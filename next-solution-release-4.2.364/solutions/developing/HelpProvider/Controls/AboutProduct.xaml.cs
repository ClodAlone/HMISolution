using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;

namespace HelpProvider.Controls
{
    /// <summary>
    /// Interaction logic for AboutProduct.xaml
    /// </summary>
    public partial class AboutProduct : UserControl
    {
        public AboutProduct()
        {
            InitializeComponent();
            Background = ApplicationPropertiesHelper.GetProperty("CurrentSkinBackColor") as Brush;
            copyrightLabel.Foreground = companyRegistered.Foreground = hyperlink.Foreground = productVersion.Foreground = productBuild.Foreground = productInternalRelease.Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;
            productVersion.Content = String.Format(Properties.Resources.WPFAbout_VersionLabel, Utilities.AssemblyInfo.FileFormatMainVersion);
            productBuild.Content = String.Format(Properties.Resources.WPFAbout_BuildLabel, Utilities.AssemblyInfo.FileFormatBuild);
            productInternalRelease.Content = String.Format(Properties.Resources.WPFAbout_InternalReleaseLabel, Utilities.AssemblyInfo.FilePrivatePart);
            copyrightLabel.Content = Utilities.AssemblyInfo.Copyright;
            companyRegistered.Content = Utilities.AssemblyInfo.Trademark;
            hyperlink.NavigateUri = new Uri(Properties.Resources.MoreInfoLink, UriKind.RelativeOrAbsolute);
        }

        private void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.LocalPath) == false)
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }
    }
}
