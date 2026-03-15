using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Charts;
using System.Windows.Media;

namespace ChartsDemo {
    public class PaletteItem : RadioButton {
        public static readonly DependencyProperty PaletteNameProperty = DependencyProperty.Register("PaletteName", typeof(string), typeof(PaletteItem), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty Brush1Property = DependencyProperty.Register("Brush1", typeof(SolidColorBrush), typeof(PaletteItem), new PropertyMetadata(null));
        public static readonly DependencyProperty Brush2Property = DependencyProperty.Register("Brush2", typeof(SolidColorBrush), typeof(PaletteItem), new PropertyMetadata(null));
        public static readonly DependencyProperty Brush3Property = DependencyProperty.Register("Brush3", typeof(SolidColorBrush), typeof(PaletteItem), new PropertyMetadata(null));
        public static readonly DependencyProperty Brush4Property = DependencyProperty.Register("Brush4", typeof(SolidColorBrush), typeof(PaletteItem), new PropertyMetadata(null));
        public static readonly DependencyProperty Brush5Property = DependencyProperty.Register("Brush5", typeof(SolidColorBrush), typeof(PaletteItem), new PropertyMetadata(null));
        public static readonly DependencyProperty Brush6Property = DependencyProperty.Register("Brush6", typeof(SolidColorBrush), typeof(PaletteItem), new PropertyMetadata(null));

        Palette palette;

        public string PaletteName {
            get { return (string)GetValue(PaletteNameProperty); }
            set { SetValue(PaletteNameProperty, value); }
        }
        public SolidColorBrush Brush1 {
            get { return (SolidColorBrush)GetValue(Brush1Property); }
            set { SetValue(Brush1Property, value); }
        }
        public SolidColorBrush Brush2 {
            get { return (SolidColorBrush)GetValue(Brush2Property); }
            set { SetValue(Brush2Property, value); }
        }
        public SolidColorBrush Brush3 {
            get { return (SolidColorBrush)GetValue(Brush3Property); }
            set { SetValue(Brush3Property, value); }
        }
        public SolidColorBrush Brush4 {
            get { return (SolidColorBrush)GetValue(Brush4Property); }
            set { SetValue(Brush4Property, value); }
        }
        public SolidColorBrush Brush5 {
            get { return (SolidColorBrush)GetValue(Brush5Property); }
            set { SetValue(Brush5Property, value); }
        }
        public SolidColorBrush Brush6 {
            get { return (SolidColorBrush)GetValue(Brush6Property); }
            set { SetValue(Brush6Property, value); }
        }
        public Palette Palette {
            get { return palette; }
            set {
                palette = value;
                PaletteName = palette.PaletteName;
                Brush1 = new SolidColorBrush(palette[0]);
                Brush2 = new SolidColorBrush(palette[1]);
                Brush3 = new SolidColorBrush(palette[2]);
                Brush4 = new SolidColorBrush(palette[3]);
                Brush5 = new SolidColorBrush(palette[4]);
                Brush6 = new SolidColorBrush(palette[5]);
            }
        }

        public PaletteItem() {
            DefaultStyleKey = typeof(PaletteItem);
        }
    }
}
