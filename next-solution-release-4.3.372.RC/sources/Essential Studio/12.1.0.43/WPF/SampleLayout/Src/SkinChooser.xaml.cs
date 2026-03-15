#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Interaction logic for SkinChooser.xaml
    /// </summary>
    public partial class SkinChooser : UserControl
    {
        public SkinChooser()
        {
            InitializeComponent();
            this.DataContext = new SkinViewModel(SkinBand);
        }       

        public static readonly DependencyProperty SkinBandProperty = DependencyProperty.Register(
   "SkinBand",
   typeof(string),
   typeof(SkinChooser), new FrameworkPropertyMetadata("Office2007", OnSkinBandChanged));

        public string SkinBand
        {
            get
            {
                return (string)this.GetValue(SkinChooser.SkinBandProperty);
            }

            set
            {
                this.SetValue(SkinChooser.SkinBandProperty, value);
            }
        }

        private static void OnSkinBandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            SkinChooser sc = d as SkinChooser;
            switch (args.NewValue.ToString())
            {
                case "Office14-2003": sc.DataContext = new SkinViewModel(sc.SkinBand); break;
                case "Office2007": sc.DataContext = new SkinViewModel(sc.SkinBand); break;
                case "ShineyBureau": sc.DataContext = new SkinViewModel(sc.SkinBand); break;
                case "GlassyDefault": sc.DataContext = new SkinViewModel(sc.SkinBand); break;
            }
        }

        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register(
            "Skin",
            typeof(string),
            typeof(SkinChooser));

        public string Skin
        {
            get
            {
                return (string)this.GetValue(SkinChooser.SkinProperty);
            }

            set
            {
                this.SetValue(SkinChooser.SkinProperty, value);
            }
        }
    }

    internal class SkinSelectorPanel : Panel
    {

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            foreach (UIElement el in this.InternalChildren)
            {
                el.Measure(new Size(100, 100));
            }

            availableSize.Width = 100;

            return base.MeasureOverride(availableSize);
        }

        Point actualChildPoint;
        int count = 0;
        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = base.ArrangeOverride(finalSize);

            if (this.Children.Count == 0)
            {
                return finalSize;
            }

            double _angle = 0;
            //Degrees converted to Radian by multiplying with PI/180
            double _incrementalAngularSpace = 90;
            //An approximate radii based on the avialable size , obviusly a better approach is needed here.
            double radiusX = 0;
            double radiusY = 0;
            double rotateAngle = -90d;
            foreach (UIElement elem in Children)
            {
                //Calculate the point on the circle for the element
                Point childPoint = new Point(Math.Cos(_angle) * radiusX, -Math.Sin(_angle) * radiusY);
                //Offsetting the point to the Avalable rectangular area which is FinalSize.
                    actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2, finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);

                elem.RenderTransform = new RotateTransform(rotateAngle);
                rotateAngle += 25;
                //Call Arrange method on the child element by giving the calculated point as the placementPoint.
                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y-0.2, elem.DesiredSize.Width, elem.DesiredSize.Height));
                //Calculate the new _angle for the next element
                _angle += _incrementalAngularSpace;
            }

            return finalSize;
        }
    }

    internal class SkinSelector : ListBox
    {
        public SkinSelector()
        {
        }
       
        protected override DependencyObject GetContainerForItemOverride()
        {
            var newSkin = new SkinListItem();
            return newSkin;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is SkinListItem);
        }
    }

    internal class SkinListItem : ListBoxItem
    {
        public SkinListItem()
        {
            this.DefaultStyleKey = typeof(SkinListItem);
        }
    }

    internal class SkinViewModel : INotifyPropertyChanged
    {
        public class Skin
        {
            public string Name { get; private set; }
            public Brush Background { get; private set; }
            public Skin(string name, Brush bg)
            {
                this.Name = name;
                this.Background = bg;
            }
        }

        public VisualStyle CurrentSkin { get; private set; }

        private ObservableCollection<Skin> skinCollection;
        public ObservableCollection<Skin> SkinCollection
        {
            get { return skinCollection; }
            set { skinCollection = value; }
        }

        public SkinViewModel(string band)
        {
            switch (band)
            {
                case "Office14-2003": MockGetBand1DataFromModel(); break;
                case "Office2007": MockGetBand2DataFromModel(); break;
                case "ShineyBureau": MockGetBand3DataFromModel(); break;
                case "GlassyDefault": MockGetBand4DataFromModel(); break;
            }
        }

        private void MockGetBand1DataFromModel()
        {
            string[] values = new[] {"Office14Blue", "Office14Silver", "Office14Black", "Office2003"};
            skinCollection = new ObservableCollection<Skin>();
            for (int i = 0; i < 4; i++)
            {
                CurrentSkin = (VisualStyle)Enum.Parse(typeof(VisualStyle), values[i]);
                skinCollection.Add(new Skin(values[i], GetSkinBrush(this.CurrentSkin)));
            }
        }

        private void MockGetBand2DataFromModel()
        {
            string[] values = new[] { "DefaultOffice2007Blue", "DefaultOffice2007Silver", "DefaultOffice2007Black", "Office2007Blue", "Office2007Silver", "Office2007Black" };
            skinCollection = new ObservableCollection<Skin>();
            for (int i = 0; i < 6; i++)
            {
                CurrentSkin = (VisualStyle)Enum.Parse(typeof(VisualStyle), values[i]);
                skinCollection.Add(new Skin(values[i], GetSkinBrush(this.CurrentSkin)));
            }
        }

        private void MockGetBand3DataFromModel()
        {
            string[] values = new[] { "ShinyBlue", "ShinyRed", "BureauBlue", "BureauBlack" };
            skinCollection = new ObservableCollection<Skin>();
            for (int i = 0; i < 4; i++)
            {
                CurrentSkin = (VisualStyle)Enum.Parse(typeof(VisualStyle), values[i]);
                skinCollection.Add(new Skin(values[i], GetSkinBrush(this.CurrentSkin)));
            }
        }

        private void MockGetBand4DataFromModel()
        {
            string[] values = new[] { "Default", "Blend", "SunBlack", "GlassyGreen", "TwilightBlue" };
            skinCollection = new ObservableCollection<Skin>();
            for (int i = 0; i < 5; i++)
            {
                CurrentSkin = (VisualStyle)Enum.Parse(typeof(VisualStyle), values[i]);
                skinCollection.Add(new Skin(values[i], GetSkinBrush(this.CurrentSkin)));
            }
        }

        internal Brush GetSkinBrush(VisualStyle style)
        {
            switch (style)
            {
                case VisualStyle.Default:
                    return new GridDataDefaultGridVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.Office2007Blue:
                    return new GridDataBlueVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.Office2007Silver:
                    return new GridDataSilverVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.Office2007Black:
                    return new GridDataBlackVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.Office2003:
                    return new GridDataSyncBlueVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.Blend:
                    return new GridDataBlendVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.GlassyGreen:
                    return new GridDataGlassyGreenVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.SunBlack:
                    return new GridDataSunBlackVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.ShinyRed:
                    return new GridDataShinyRedVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.ShinyBlue:
                    return new GridDataShinyBlueVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.BureauBlue:
                    return new GridDataBureauBlueVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.BureauBlack:
                    return new GridDataBureauBlackVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.TwilightBlue:
                    return new GridDataTwilightBlueVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.DefaultOffice2007Blue:
                    return new GridDataOffice2007BlueVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.DefaultOffice2007Black:
                    return new GridDataOffice2007BlackVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.DefaultOffice2007Silver:
                    return new GridDataOffice2007SilverVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.Office14Blue:
                    return new GridDataOffice14BlueVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.Office14Black:
                    return new GridDataOffice14BlackVisualStyle().HeaderBackgroundBrush;
                case VisualStyle.Office14Silver:
                    return new GridDataOffice14SilverVisualStyle().HeaderBackgroundBrush;
            }

            return null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    internal class GridDataObjectToStringConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            if (value.ToString().StartsWith("System.Windows.Controls.ComboBoxItem: "))
            {
                value = value.ToString().Remove(0, 38);
                return value.ToString();
            }
            else
            {
                return (value as SkinViewModel.Skin).Name;
            }
        }
        #endregion
    }

    
}
