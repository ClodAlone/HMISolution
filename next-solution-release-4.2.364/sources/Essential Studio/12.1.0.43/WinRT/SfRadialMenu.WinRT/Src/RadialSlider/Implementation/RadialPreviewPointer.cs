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
using Windows.UI.Xaml.Controls;
namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a preview for the pointer that allows the user to select a value in
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class RadialPreviewPointer : Control
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialPreviewPointer"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Navigation">Syncfusion.UI.Xaml.Controls.Navigation
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public RadialPreviewPointer()
        {
            DefaultStyleKey = typeof(RadialPreviewPointer);
        }

        #endregion
    }
}
