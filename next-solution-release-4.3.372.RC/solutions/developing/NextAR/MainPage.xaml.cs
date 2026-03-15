using DevExpress.UI.Xaml.Layout;
using System;
using Windows.UI.Xaml.Data;
namespace NextAR
{
    public sealed partial class MainPage : DXPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Tile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Windows.UI.ApplicationSettings.SettingsPane.Show();
        }
    }
}

