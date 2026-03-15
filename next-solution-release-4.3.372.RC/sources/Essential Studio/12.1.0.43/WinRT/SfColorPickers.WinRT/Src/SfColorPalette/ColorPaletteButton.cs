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
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.WP.Controls.Media
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Tools.Controls.Media
#elif WPF
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Windows.Controls.Media
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a control that enables the user to select values by clicking it in the
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPalette"/> control.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSwatches"/>
    [ClassReference(IsReviewed = false)]
    public class ColorPaletteButton:Button
    {
#if!WINRT
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
#else
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="Syncfusion.UI.Xaml.Controls.Media.ColorPaletteButton"/> class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
#endif
        public ColorPaletteButton()
        {
            DefaultStyleKey = typeof (ColorPaletteButton);
           
        }

        internal FrameworkElement Apex, Hardcover, Metro, Module, Office, Paper, Pushpin, Solstice, Urban, Waveform;

        /// <summary>
        /// Initializes all the child elements of the
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorPaletteButton"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            Apex = GetTemplateChild("Apex") as FrameworkElement;
            Hardcover = GetTemplateChild("Hardcover") as FrameworkElement;
            Metro = GetTemplateChild("Metro") as FrameworkElement;
            Module = GetTemplateChild("Module") as FrameworkElement;
            Office = GetTemplateChild("Office") as FrameworkElement;
            Paper = GetTemplateChild("Paper") as FrameworkElement;
            Pushpin = GetTemplateChild("Pushpin") as FrameworkElement;
            Solstice = GetTemplateChild("Solstice") as FrameworkElement;
            Urban = GetTemplateChild("Urban") as FrameworkElement;
            Waveform = GetTemplateChild("Waveform") as FrameworkElement;
            Apex.Visibility = Visibility.Visible;
            base.OnApplyTemplate();
        }

    }
}
