using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Charts;

namespace ChartsDemo {
    public class PaletteSelectorHelper
    {
        Palette actualPalette = new OfficePalette();

        public Palette ActualPalette
        {
            get { return actualPalette; }
            set { actualPalette = value; }
        }
    }

    public class IsCheckedToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

    public partial class PaletteChooser : UserControl, IDisposable 
    {
        ChartControl chart;
        PaletteSelectorHelper actualPalette = new PaletteSelectorHelper();
        public PaletteChooser(ChartControl chart) {
            InitializeComponent();
            this.chart = chart;
            int count = 0;
            chart.Palette = actualPalette.ActualPalette;
            foreach (PaletteKind paletteKind in Palette.GetPredefinedKinds()) {
                RowDefinition rowDefenition = new RowDefinition();
                rowDefenition.Height = GridLength.Auto;
                grPalettes.RowDefinitions.Add(rowDefenition);
                PaletteItem paletteItem = new PaletteItem();
                paletteItem.Palette = Activator.CreateInstance(paletteKind.Type) as Palette;
                if (chart != null && chart.Palette.PaletteName == paletteItem.Palette.PaletteName)
                    paletteItem.IsChecked = true;
                paletteItem.Checked += new RoutedEventHandler(paletteItem_Checked);
                paletteItem.GotMouseCapture += new MouseEventHandler(PaletteItem_ReleaseMouseCapture);
                paletteItem.ClickMode = ClickMode.Press;
                Grid.SetRow(paletteItem, count);
                grPalettes.Children.Add(paletteItem);
                count++;
            }
        }
        void PaletteItem_ReleaseMouseCapture(object sender, MouseEventArgs e) {
            PaletteItem paletteItem = sender as PaletteItem;
            paletteItem.ReleaseMouseCapture();
        }
        void paletteItem_Checked(object sender, RoutedEventArgs e) {
            PaletteItem paletteItem = sender as PaletteItem;
            if (chart != null && paletteItem != null) {
                chart.Palette = paletteItem.Palette;
                actualPalette.ActualPalette = paletteItem.Palette;
            }
        }
        public void UpdateChart(ChartControl chart) {
            this.chart = chart;
            chart.Palette = actualPalette.ActualPalette;
        }

        #region IDisposable Members

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            foreach (var child in grPalettes.Children)
            {
                var paletteItem = child as PaletteItem;
                if (paletteItem != null)
                {
                    paletteItem.Checked -= paletteItem_Checked;
                    paletteItem.GotMouseCapture -= PaletteItem_ReleaseMouseCapture;
                }
            }
            grPalettes.Children.Clear();

            chart = null;
            actualPalette.ActualPalette = null;
        }

        #endregion
    }
}
