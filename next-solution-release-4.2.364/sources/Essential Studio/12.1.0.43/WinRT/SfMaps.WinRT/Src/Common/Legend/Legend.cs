#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{

#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Shapes;
#else
    using System.Windows;
    using System.Windows.Shapes;
#endif

    public class Legend : DependencyObject
    {
        #region Constructor

        public Legend()
        {
            LegendIcon = new Ellipse();
        }

        #endregion

        #region properties

        #region LegendLabel

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LegendLabelProperty =
            DependencyProperty.Register("LegendLabel", typeof(string), typeof(Legend), new PropertyMetadata(string.Empty));
        internal string LegendLabel
        {
            get { return (string)GetValue(LegendLabelProperty); }
            set { SetValue(LegendLabelProperty, value); }
        }


        #endregion

        #region LegendIcon
        internal FrameworkElement LegendIcon
        {
            get { return (FrameworkElement)GetValue(LegendIconProperty); }
            set { SetValue(LegendIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIcon.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LegendIconProperty =
            DependencyProperty.Register("LegendIcon", typeof(FrameworkElement), typeof(Legend), new PropertyMetadata(null));
        #endregion

        #endregion
    }
}


