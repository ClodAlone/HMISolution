#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a border for date in month view.
    /// </summary>
    public class ScheduleRectangleBorder : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleRectangleBorder">ScheduleRectangleBorder</see>
        /// class.
        /// </summary>
        public ScheduleRectangleBorder()
        {
            DefaultStyleKey = typeof(ScheduleRectangleBorder);
        }

        #endregion

        #region Dependency Properties

        #region LeftBrush
        /// <summary>
        /// Gets or sets the color of left border for date in month view
        /// </summary>
        public Brush LeftBrush
        {
            get { return (Brush)GetValue(LeftBrushProperty); }
            set { SetValue(LeftBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LeftBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LeftBrushProperty =
            DependencyProperty.Register("LeftBrush", typeof(Brush), typeof(ScheduleRectangleBorder), new PropertyMetadata(null));
        #endregion

        #region RightBrush
        /// <summary>
        /// Gets or sets the color of right border for date in month view
        /// </summary>
        public Brush RightBrush
        {
            get { return (Brush)GetValue(RightBrushProperty); }
            set { SetValue(RightBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RightBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RightBrushProperty =
            DependencyProperty.Register("RightBrush", typeof(Brush), typeof(ScheduleRectangleBorder), new PropertyMetadata(null));
        #endregion

        #region TopBrush
        /// <summary>
        /// Gets or sets the color of top border for date in month view
        /// </summary>
        public Brush TopBrush
        {
            get { return (Brush)GetValue(TopBrushProperty); }
            set { SetValue(TopBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TopBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TopBrushProperty =
            DependencyProperty.Register("TopBrush", typeof(Brush), typeof(ScheduleRectangleBorder), new PropertyMetadata(null));
        #endregion

        #region BottomBrush
        /// <summary>
        /// Gets or sets the color of bottom border for date in month view
        /// </summary>
        public Brush BottomBrush
        {
            get { return (Brush)GetValue(BottomBrushProperty); }
            set { SetValue(BottomBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BottomBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BottomBrushProperty = 
            DependencyProperty.Register("BottomBrush", typeof(Brush), typeof(ScheduleRectangleBorder), new PropertyMetadata(null));
        #endregion

        #endregion
    }
}
