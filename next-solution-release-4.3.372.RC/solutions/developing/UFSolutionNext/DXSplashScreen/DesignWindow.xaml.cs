using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Utilities;

namespace UFSolutionNext
{
    /// <summary>
    /// Interaction logic for DesignWindow.xaml
    /// </summary>
    public partial class DesignWindow : UserControl
    {
        public DesignWindow()
        {
            InitializeComponent();

            try
            {
                var filename = String.Format("{0}Splashes\\{1}", AppDomain.CurrentDomain.BaseDirectory,
                                             "DesignSplashControl.jpg");
                if (File.Exists(filename))
                {
                    splashContent.Content = new Image() { Source = new BitmapImage(new Uri(filename)) };

                    versionLabel.Visibility = Visibility.Collapsed;
                    revisionLabel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    var list = FindAndLoadDLL.LoadDLLs<UserControl>(String.Format("{0}Splashes\\", AppDomain.CurrentDomain.BaseDirectory),
                                                        "DesignSplashControl.dll", false);

                    if (list.Count > 0)
                        splashContent.Content = list[0];
                    versionLabel.Content = String.Format(Properties.Resources.Splash_VersionLabel, Utilities.AssemblyInfo.FileFormatVersion);
                    revisionLabel.Content = String.Format(Properties.Resources.Splash_RevisionLabel, Utilities.AssemblyInfo.FilePrivatePart);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
