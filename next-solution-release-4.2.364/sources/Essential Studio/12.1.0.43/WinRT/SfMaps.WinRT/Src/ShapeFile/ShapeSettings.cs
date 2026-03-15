#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
#if WINRT
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using System.Threading.Tasks;
#else
    using System.Windows;
    using System.Windows.Media;
#endif

    using System.Collections.ObjectModel;
    

    public class ShapeSettings : DependencyObject
    {

        #region InternalFields

        internal ShapeFileLayer layer=null;

        #endregion

        #region Constructor


        public ShapeSettings()
        {
            if (this.CustomColors == null)
            {
                this.CustomColors = new ObservableCollection<MapColorPalette>();
            }

        }

        #endregion

        #region Properties

        #region CustomColors



        public ObservableCollection<MapColorPalette> CustomColors
        {
            get { return (ObservableCollection<MapColorPalette>)GetValue(CustomColorsProperty); }
            set { SetValue(CustomColorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomColors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomColorsProperty =
            DependencyProperty.Register("CustomColors", typeof(ObservableCollection<MapColorPalette>), typeof(ShapeSettings), new PropertyMetadata(null));



        #endregion

        #region ShapeColorMode



        public ShapeColorMode ShapeColorMode
        {
            get { return (ShapeColorMode)GetValue(ShapeColorModeProperty); }
            set { SetValue(ShapeColorModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeColorMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeColorModeProperty =
            DependencyProperty.Register("ShapeColorMode", typeof(ShapeColorMode), typeof(ShapeSettings), new PropertyMetadata(ShapeColorMode.Default, new PropertyChangedCallback(OnShapeColorModeChanged)));

        private static void OnShapeColorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeSettings settings = d as ShapeSettings;
            if (settings.layer != null)
            {
                if (e.NewValue != null)
                {
                    foreach (MapShape shp in settings.layer.MapShapes)
                    {
                        if ((ShapeColorMode)e.NewValue == ShapeColorMode.Default || (ShapeColorMode)e.NewValue == ShapeColorMode.HeatMap)
                        {
                            if (shp.value != null)
                            {
                                settings.layer.FillColors(shp.Shape, shp.ColorValue, -1);
                            }
                        }
                        else if ((ShapeColorMode)e.NewValue == ShapeColorMode.ColorPalette)
                        {
                        }
                    }
                }
            }
        }



        #endregion

        #region ShapeFill



        public Brush ShapeFill
        {
            get { return (Brush)GetValue(ShapeFillProperty); }
            set { SetValue(ShapeFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeFill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeFillProperty =
            DependencyProperty.Register("ShapeFill", typeof(Brush), typeof(ShapeSettings), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 250, 225, 200)), new PropertyChangedCallback(OnShapeFillChanged)));

        private static void OnShapeFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeSettings setting = d as ShapeSettings;
            if (setting.layer != null)
            {
                foreach (MapShape shp in setting.layer.MapShapes)
                {
                    if (! shp.isSelected)
                    {
                        setting.layer.FillColors(shp.Shape, shp.ColorValue, -1);
                    }
                }
            }
        }




        #endregion

        #region ShapeStroke



        public Brush ShapeStroke
        {
            get { return (Brush)GetValue(ShapeStrokeProperty); }
            set { SetValue(ShapeStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeStrokeProperty =
            DependencyProperty.Register("ShapeStroke", typeof(Brush), typeof(ShapeSettings), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        #endregion

        #region ShapeStrokeThickness



        public double ShapeStrokeThickness
        {
            get { return (double)GetValue(ShapeStrokeThicknessProperty); }
            set { SetValue(ShapeStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeStrokeThicknessProperty =
            DependencyProperty.Register("ShapeStrokeThickness", typeof(double), typeof(ShapeSettings), new PropertyMetadata(1d, new PropertyChangedCallback(OnShapeStrokeThicknessChanged)));

        private static void OnShapeStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var settings = d as ShapeSettings;
            if (e.NewValue != null)
            {
                if (settings.layer != null)
                {
                    settings.TempShapeStrokeThickness = settings.ShapeStrokeThickness / settings.layer.ZoomTransform.ScaleX;
                }
                else
                {
                    settings.TempShapeStrokeThickness = settings.ShapeStrokeThickness;
                }
            }
        }



        #endregion

        #region TempShapeStrokeThicknessProperty



        internal double TempShapeStrokeThickness
        {
            get { return (double)GetValue(TempShapeStrokeThicknessProperty); }
            set { SetValue(TempShapeStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TempShapeStrokeThickness.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TempShapeStrokeThicknessProperty =
            DependencyProperty.Register("TempShapeStrokeThickness", typeof(double), typeof(ShapeSettings), new PropertyMetadata(0.5d));



        #endregion

        #region MaxColorValue



        public SolidColorBrush MaxColorValue
        {
            get { return (SolidColorBrush)GetValue(MaxColorValueProperty); }
            set { SetValue(MaxColorValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxColorValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxColorValueProperty =
            DependencyProperty.Register("MaxColorValue", typeof(SolidColorBrush), typeof(ShapeSettings), new PropertyMetadata(null));




        #endregion

        #region ShapeValuePath

        public string ShapeValuePath
        {
            get { return (string)GetValue(ShapeValuePathProperty); }
            set { SetValue(ShapeValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeValuePathProperty =
            DependencyProperty.Register("ShapeValuePath", typeof(string), typeof(ShapeSettings), new PropertyMetadata(string.Empty));

        #endregion

        #region ShapeColorValuePath

        public string ShapeColorValuePath
        {
            get { return (string)GetValue(ShapeColorValuePathProperty); }
            set { SetValue(ShapeColorValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeColorValuePathProperty =
            DependencyProperty.Register("ShapeColorValuePath", typeof(string), typeof(ShapeSettings), new PropertyMetadata(string.Empty));

        #endregion

        #region ShapeValuePath

        public Brush SelectedShapeColor
        {
            get { return (Brush)GetValue(SelectedShapeColorProperty); }
            set { SetValue(SelectedShapeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedShapeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedShapeColorProperty =
            DependencyProperty.Register("SelectedShapeColor", typeof(Brush), typeof(ShapeSettings), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        #endregion

        #region ColorPalette



        public ColorPalettes ColorPalette
        {
            get { return (ColorPalettes)GetValue(ColorPaletteProperty); }
            set { SetValue(ColorPaletteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorPalette.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorPaletteProperty =
            DependencyProperty.Register("ColorPalette", typeof(ColorPalettes), typeof(ShapeSettings), new PropertyMetadata(ColorPalettes.Metro, new PropertyChangedCallback(OnColorPaletteChanged)));

        private static void OnColorPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeSettings settings = d as ShapeSettings;
            if (settings.layer != null)
            {
                if (e.NewValue != null)
                {
                    settings.layer.shapeCount = 0;
                    settings.layer.SetColorPalette((ColorPalettes)e.NewValue);
                    if ((ColorPalettes)e.NewValue == ColorPalettes.CustomPalette || (e.OldValue != null && (ColorPalettes)e.OldValue == ColorPalettes.CustomPalette))
                    {
                        foreach (MapShape shp in settings.layer.MapShapes)
                        {
                            settings.layer.FillColors(shp.Shape, null, -1);
                        }
                    }
                }
            }
        }




        #endregion

        #endregion
    }
}
