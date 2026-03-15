#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
namespace Syncfusion.Windows.Grid.Olap
#else
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// light weight custom type for defining the olap grid cell style
    /// </summary>
    public class OlapGridCellStyle : DependencyObject, INotifyPropertyChanged
    {
        #region Dependency Property Declarations

        public static readonly DependencyProperty BackgroundProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("Background", typeof(Brush), typeof(OlapGridCellStyle), new UIPropertyMetadata(null, OlapGridCellStyle.OnBackgroundChanged));
#else
            DependencyProperty.Register("Background", typeof(Brush), typeof(OlapGridCellStyle), new PropertyMetadata(null, OlapGridCellStyle.OnBackgroundChanged));
#endif

        public static readonly DependencyProperty FontFamilyProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(OlapGridCellStyle), new UIPropertyMetadata(new FontFamily("Segoe UI"), OlapGridCellStyle.OnFontFamilyChanged));
#else
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(OlapGridCellStyle), new PropertyMetadata(new FontFamily("Segoe UI"), OlapGridCellStyle.OnFontFamilyChanged));
#endif

        public static readonly DependencyProperty FontSizeProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("FontSize", typeof(int), typeof(OlapGridCellStyle), new UIPropertyMetadata(12, OlapGridCellStyle.OnFontSizeChanged));
#else
            DependencyProperty.Register("FontSize", typeof(int), typeof(OlapGridCellStyle), new PropertyMetadata(12, OlapGridCellStyle.OnFontSizeChanged));
#endif

        public static readonly DependencyProperty FontWeightProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(OlapGridCellStyle), new UIPropertyMetadata(FontWeights.Normal, OlapGridCellStyle.OnFontWeightChanged));
#else
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(OlapGridCellStyle), new PropertyMetadata(FontWeights.Normal, OlapGridCellStyle.OnFontWeightChanged));
#endif

        public static readonly DependencyProperty ForegroundProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(OlapGridCellStyle), new UIPropertyMetadata(Brushes.Black, OlapGridCellStyle.OnForegroundChanged));
#else
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(OlapGridCellStyle), new PropertyMetadata(new SolidColorBrush(Colors.Black), OlapGridCellStyle.OnForegroundChanged));
#endif

        public static readonly DependencyProperty IsHyperlinkCellProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(OlapGridCellStyle), new UIPropertyMetadata(false, OlapGridCellStyle.OnIsHyperlinkCellChanged));
#else
            DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(OlapGridCellStyle), new PropertyMetadata(false, OlapGridCellStyle.OnIsHyperlinkCellChanged));
#endif

        public static readonly DependencyProperty StyleProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("Style", typeof(Style), typeof(OlapGridCellStyle), new UIPropertyMetadata(null, OlapGridCellStyle.OnStyleChanged));
#else
            DependencyProperty.Register("Style", typeof(Style), typeof(OlapGridCellStyle), new PropertyMetadata(null, OlapGridCellStyle.OnStyleChanged));
#endif

        public static readonly DependencyProperty TextWrappingProperty =

#if !SILVERLIGHT
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(OlapGridCellStyle), new UIPropertyMetadata(TextWrapping.NoWrap));
#else
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(OlapGridCellStyle), new PropertyMetadata(TextWrapping.NoWrap));
#endif




        #endregion

        #region Initilize/Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridCellStyle"/> class.
        /// </summary>
        public OlapGridCellStyle()
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>The foreground.</value>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hyperlink cell.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is hyperlink cell; otherwise, <c>false</c>.
        /// </value>
        public bool IsHyperlinkCell
        {
            get { return (bool)GetValue(IsHyperlinkCellProperty); }
            set { SetValue(IsHyperlinkCellProperty, value); }
        }

        [Browsable(false)]
        public Style Style
        {
            get { return (Style)GetValue(StyleProperty); }
            set { SetValue(StyleProperty, value); }
        }



        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the internal source style.
        /// </summary>
        /// <value>The internal source style.</value>
        internal OlapGridCellStyle InternalSourceStyle
        {
            get;
            set;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="info">The info.</param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        #region Dependency Properties Implementation

        /// <summary>
        /// Called when [background changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnBackgroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGridCellStyle gridCellStyle = dependencyObject as OlapGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("Background");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.Background = gridCellStyle.Background;
            }
        }

        /// <summary>
        /// Called when [font family changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFontFamilyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGridCellStyle gridCellStyle = dependencyObject as OlapGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("FontFamily");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.FontFamily = gridCellStyle.FontFamily;
            }
        }

        /// <summary>
        /// Called when [font size changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFontSizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGridCellStyle gridCellStyle = dependencyObject as OlapGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("FontSize");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.FontSize = gridCellStyle.FontSize;
            }
        }

        /// <summary>
        /// Called when [font weight changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFontWeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGridCellStyle gridCellStyle = dependencyObject as OlapGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("FontWeight");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.FontWeight = gridCellStyle.FontWeight;
            }
        }

        /// <summary>
        /// Called when [foreground changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnForegroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGridCellStyle gridCellStyle = dependencyObject as OlapGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("Foreground");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.Foreground = gridCellStyle.Foreground;
            }
        }

        /// <summary>
        /// Called when [is hyperlink cell changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsHyperlinkCellChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGridCellStyle gridCellStyle = dependencyObject as OlapGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("IsHyperlinkCell");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.IsHyperlinkCell = gridCellStyle.IsHyperlinkCell;
            }
        }

        /// <summary>
        /// Called when [style changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGridCellStyle gridCellStyle = dependencyObject as OlapGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("Style");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.Style = gridCellStyle.Style;
            }
        }

        #endregion
    }
}
