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
    using System.Threading.Tasks;
    using Windows.UI;
    using Windows.UI.Text;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;


    public class MapAnnotations : DependencyObject
    {
        public MapAnnotations()
        {
        }
        #region Properties

        #region Symbol(Dependency Property)

        public UIElement AnnotationSymbol
        {
            get { return (UIElement)GetValue(AnnotationSymbolProperty); }
            set { SetValue(AnnotationSymbolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationSymbolProperty =
            DependencyProperty.Register("AnnotationSymbol", typeof(UIElement), typeof(MapAnnotations), new PropertyMetadata(null));

        #endregion    

        #region Latitude(Dependency Property)

        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Latitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(MapAnnotations), new PropertyMetadata(0d, new PropertyChangedCallback(OnLatitudeChanged)));

        private static void OnLatitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }
       #endregion

        #region Longitude(Dependency Property)

        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Longitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(MapAnnotations), new PropertyMetadata(0d, new PropertyChangedCallback(OnLongitudeChanged)));

        private static void OnLongitudeChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
           
        }
        #endregion

        #region AnnotationLabel

        public string AnnotationLabel
        {
            get { return (string)GetValue(AnnotationLabelProperty); }
            set { SetValue(AnnotationLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelProperty =
            DependencyProperty.Register("AnnotationLabel", typeof(string), typeof(MapAnnotations), new PropertyMetadata(""));

        #endregion

        #region LabelForeground(Dependency Property)

        /// <summary>
        /// Gets or sets the Foreground Color for the Labels on the Map.
        /// </summary>
        /// <value>Foreground Color.</value>
        public Brush AnnotationLabelForeground
        {
            get { return (Brush)GetValue(AnnotationLabelForegroundProperty); }
            set { SetValue(AnnotationLabelForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelForegroundProperty =
            DependencyProperty.Register("AnnotationLabelForeground", typeof(Brush), typeof(MapAnnotations), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region AnnotationLabelFontFamily(Dependency Property)

        /// <summary>
        /// Gets or sets the font family of the Labels in the Map.
        /// </summary>
        /// <value>Font Family</value>
        public FontFamily AnnotationLabelFontFamily
        {
            get { return (FontFamily)GetValue(AnnotationLabelFontFamilyProperty); }
            set { SetValue(AnnotationLabelFontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelFontFamilyProperty =
            DependencyProperty.Register("AnnotationLabelFontFamily", typeof(FontFamily), typeof(MapAnnotations), new PropertyMetadata(new FontFamily("Times New Roman")));


        #endregion

        #region AnnotationLabelBackground(Dependency Property)

        /// <summary>
        /// Gets or sets Background color of the Label on the Map.
        /// </summary>
        /// <value>The Background Color.</value>
        public Brush AnnotationLabelBackground
        {
            get { return (Brush)GetValue(AnnotationLabelBackgroundProperty); }
            set { SetValue(AnnotationLabelBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelBackgroundProperty =
            DependencyProperty.Register("AnnotationLabelBackground", typeof(Brush), typeof(MapAnnotations), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        #endregion

        #region AnnotationLabelFontStyle(Dependency Property)

        /// <summary>
        /// Gets or sets Font Style of the Label in the Map.
        /// </summary>
        /// <value>Font Style of the Label.</value>
        public FontStyle AnnotationLabelFontStyle
        {
            get { return (FontStyle)GetValue(AnnotationLabelFontStyleProperty); }
            set { SetValue(AnnotationLabelFontStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelFontStyleProperty =
            DependencyProperty.Register("AnnotationLabelFontStyle", typeof(FontStyle), typeof(MapAnnotations), new PropertyMetadata(FontStyle.Normal));

        #endregion          

        #region AnnotationLabelFontSize(Dependency Property)

        /// <summary>
        /// Gets or sets the font size of the label in the Map.
        /// </summary>
        /// <value>The size of the label font.</value>
        public double AnnotationLabelFontSize
        {
            get { return (double)GetValue(AnnotationLabelFontSizeProperty); }
            set { SetValue(AnnotationLabelFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelFontSizeProperty =
            DependencyProperty.Register("AnnotationLabelFontSize", typeof(double), typeof(MapAnnotations), new PropertyMetadata(12d));

        #endregion      

        #region AnnotationMargin



        internal Thickness AnnotationMargin
        {
            get { return (Thickness)GetValue(AnnotationMarginProperty); }
            set { SetValue(AnnotationMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationMarginProperty =
            DependencyProperty.Register("AnnotationMargin", typeof(Thickness), typeof(MapAnnotations), new PropertyMetadata(new Thickness(0, 0, 0, 0)));




        #endregion

        #region SymbolTemplate



        public DataTemplate AnnotationTemplate
        {
            get { return (DataTemplate)GetValue(AnnotationTemplateProperty); }
            set { SetValue(AnnotationTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationTemplateProperty =
            DependencyProperty.Register("SymbolTemplate", typeof(DataTemplate), typeof(MapAnnotations), new PropertyMetadata(null));




        #endregion

        #endregion

    }
}
