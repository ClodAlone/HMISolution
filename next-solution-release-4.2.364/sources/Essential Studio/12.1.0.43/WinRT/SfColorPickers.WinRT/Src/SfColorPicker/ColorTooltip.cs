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


#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.WP.Controls.Media
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a control that highlights the color selected by the user in the <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSelectionBox"/> control or the 
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSlider"/>
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPicker"/>
    [ClassReference(IsReviewed = false)]
    public sealed class ColorTooltip : Control
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public ColorTooltip()
        {
            this.DefaultStyleKey = typeof(ColorTooltip);
        }

        /// <summary>
        /// Returns a color from the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPicker"/> control that has been selected.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorTooltip), new PropertyMetadata(Colors.Transparent));


    }
}
