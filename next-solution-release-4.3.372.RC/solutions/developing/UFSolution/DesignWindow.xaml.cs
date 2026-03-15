using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Utilities;

namespace UFSolution
{
    /// <summary>
    /// Interaction logic for DesignWindow.xaml
    /// </summary>
    public partial class DesignWindow : Window
    {
        public DesignWindow()
        {
            InitializeComponent();

            try
            {
                var list = FindAndLoadDLL.LoadDLLs<UserControl>(String.Format("{0}Splashes\\", AppDomain.CurrentDomain.BaseDirectory),
                                                    "DesignSplashControl.dll", false);

                if (list.Count > 0)
                    splashContent.Content = list[0];
                versionLabel.Content = String.Format(Properties.Resources.Splash_VersionLabel, Utilities.AssemblyInfo.FileFormatVersion);
                revisionLabel.Content = String.Format(Properties.Resources.Splash_RevisionLabel, Utilities.AssemblyInfo.FilePrivatePart);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
