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
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
namespace Syncfusion.Windows.Controls.Navigation
#else
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a List that allows the user to select an item in
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RadialList : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialList"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Navigation">Syncfusion.UI.Xaml.Controls.Navigation
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public RadialList()
        {
            SizeChanged += RadialList_SizeChanged;
        }

        #endregion

        void RadialList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Radius = e.NewSize.Width / 2;
        }

        #region Dependency Properties

        /// <summary>
        /// Gets and sets the Radius of the <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialList"/>
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(RadialList), new PropertyMetadata(0d));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ListHost.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ListHostProperty =
            DependencyProperty.Register("ListHost", typeof (object), typeof (RadialList), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the Host of the <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialList"/>
        /// </summary>
        public object ListHost
        {
            get { return GetValue(ListHostProperty); }
            set { SetValue(ListHostProperty, value); }
        }

        #endregion

    }
}
