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
   public class RadialLabel : ContentControl
   {      
       #region Constructor

       /// <summary>
       /// Initializes a new instance of the <see
       /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialLabel"/> class.
       /// </summary>
       /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
       /// <seealso
       /// cref="N:Syncfusion.UI.Xaml.Controls.Navigation">Syncfusion.UI.Xaml.Controls.Navigation
       /// Namespace</seealso>
       [ClassReference(IsReviewed = false)]
       public RadialLabel()
       {
           DefaultStyleKey = typeof(RadialLabel);
       }

       #endregion

       #region Dependency Properties

       /// <summary>
       /// Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(RadialLabel), new PropertyMetadata(0d));

        /// <summary>
        /// Gets and sets the Angle of 
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialLabel"/>
        /// </summary>
        /// <value>
        /// The default value is 0
        /// </value>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

       #endregion

   }
}
