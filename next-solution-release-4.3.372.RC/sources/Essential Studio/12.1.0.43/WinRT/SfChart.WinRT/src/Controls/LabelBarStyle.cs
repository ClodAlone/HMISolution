#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.Foundation;
#endif
using System.Reflection;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows;

namespace Syncfusion.UI.Xaml.Charts
{
    public class LabelBarStyle : DependencyObject
    {
        public LabelBarStyle()
        {

        }

        /// <summary>
        /// Gets or Sets the HorizontalAlignment of the labels inside the label bar
        /// </summary>
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelHorizontalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
            DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(LabelBarStyle), new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Gets or Sets the Background the label bar
        /// </summary>
        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(LabelBarStyle), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or Sets the color of the labels inside the selected region.
        /// </summary>
        public SolidColorBrush SelectedLabelBrush
        {
            get { return (SolidColorBrush)GetValue(SelectedLabelBrushProperty); }
            set { SetValue(SelectedLabelBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedLabelBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedLabelBrushProperty =
            DependencyProperty.Register("SelectedLabelBrush", typeof(SolidColorBrush), typeof(LabelBarStyle), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        
        

        

    }
}
