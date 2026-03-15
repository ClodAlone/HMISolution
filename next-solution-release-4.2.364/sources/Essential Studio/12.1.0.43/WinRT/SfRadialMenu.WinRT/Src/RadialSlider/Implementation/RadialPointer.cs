#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a pointer that allows the user to select a value in
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class RadialPointer : Control
    {
        #region Constructor
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        /// <summary>
        /// Declaraation of a framework element
        /// </summary>
        [Browsable(false)]
        public FrameworkElement _partRoot;
#else
        private FrameworkElement _partRoot;
#endif
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialPointer"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Navigation">Syncfusion.UI.Xaml.Controls.Navigation
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public RadialPointer()
        {
            DefaultStyleKey = typeof(RadialPointer);
        }

        /// <summary>
        /// Initializes the variables on applying template.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            _partRoot = GetTemplateChild("PART_Root") as FrameworkElement;
            base.OnApplyTemplate();
        }
        #endregion
    }
}
