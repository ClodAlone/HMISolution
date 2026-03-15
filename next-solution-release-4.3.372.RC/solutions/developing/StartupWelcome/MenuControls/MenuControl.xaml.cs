using DevExpress.Xpf.Bars;
using StartupWelcome.ComponentService;
using StartupWelcome.View_Model;
using System;
using System.Collections.Generic;
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
using System.Resources;

namespace StartupWelcome
{
    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl : BarManager
    {
        bool bLoaded;
        RecentRepository Recent = StartupWelcomeComponent.startupWelcomeComponent.Recent;
        public MenuControl(bool bIsInEasyMode)
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (bIsInEasyMode || ApplicationPropertiesHelper.GetProperty("AutoLoadWorkspace") as bool? == true)
                    DisableLayoutToFile();
                else
                    EnableLayoutToFile();

                RecentMenuHelper.LoadRecentFileList(recentProjects, Recent);
            };
        }

        private void recentProjects_Popup(object sender, EventArgs e)
        {
            RecentMenuHelper.LoadRecentFileList(recentProjects, Recent);
        }

        public void EnableLayoutToFile()
        {
            useLayoutToFile.IsEnabled = true;
            useLayoutToFile.IsChecked = ApplicationPropertiesHelper.GetProperty("UseLayoutToFile") as bool? == true;
        }

        public void DisableLayoutToFile()
        {
            useLayoutToFile.IsChecked = false;
            useLayoutToFile.IsEnabled = false;
        }
    }
}
