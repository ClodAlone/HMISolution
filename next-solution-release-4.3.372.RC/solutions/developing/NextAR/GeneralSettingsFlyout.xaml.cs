using NextAR.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace NextAR
{
    public sealed partial class GeneralSettingsFlyout : SettingsFlyout
    {
        SettingsViewModel model = new SettingsViewModel();

        public GeneralSettingsFlyout()
        {
            this.InitializeComponent();
            model.WebUrl = Settings.WebUrl;
            DataContext = model;

            Loaded += GeneralSettingsFlyout_Loaded;
            Unloaded += GeneralSettingsFlyout_Unloaded;
        }

        void GeneralSettingsFlyout_Unloaded(object sender, RoutedEventArgs e)
        {
            Settings.WebUrl = model.WebUrl;
            Settings.SaveSettigs();
        }

        void GeneralSettingsFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            var processor = WindowsStoreSystemInformation.GetProcessorArchitecture();
            if (processor == ProcessorArchitecture.ARM)
            {

            }
        }
    }
}
