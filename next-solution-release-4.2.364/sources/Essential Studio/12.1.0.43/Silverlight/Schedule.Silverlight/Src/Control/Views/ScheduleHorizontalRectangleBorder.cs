#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls.Schedule
{
#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class for horizontal border in Schedule view
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleHorizontalRectangleBorder : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleBorder"/>
        /// class.
        /// </summary>
        public ScheduleHorizontalRectangleBorder()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalRectangleBorder);
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LeftBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LeftBrushProperty = DependencyProperty.Register("LeftBrush", typeof(Brush), typeof(ScheduleHorizontalRectangleBorder), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets brush value for LeftBrush
        /// </summary>
        public Brush LeftBrush
        {
            get
            {
                return (Brush)this.GetValue(ScheduleHorizontalRectangleBorder.LeftBrushProperty);
            }

            set
            {
                this.SetValue(ScheduleHorizontalRectangleBorder.LeftBrushProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for RightBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RightBrushProperty = DependencyProperty.Register("RightBrush", typeof(Brush), typeof(ScheduleHorizontalRectangleBorder), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets brush value for right brush
        /// </summary>
        public Brush RightBrush
        {
            get
            {
                return (Brush)this.GetValue(ScheduleHorizontalRectangleBorder.RightBrushProperty);
            }

            set
            {
                this.SetValue(ScheduleHorizontalRectangleBorder.RightBrushProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TopBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TopBrushProperty = DependencyProperty.Register("TopBrush", typeof(Brush), typeof(ScheduleHorizontalRectangleBorder), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets brush value for top brush
        /// </summary>
        public Brush TopBrush
        {
            get
            {
                return (Brush)this.GetValue(ScheduleHorizontalRectangleBorder.TopBrushProperty);
            }

            set
            {
                this.SetValue(ScheduleHorizontalRectangleBorder.TopBrushProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for BottomBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BottomBrushProperty = DependencyProperty.Register("BottomBrush", typeof(Brush), typeof(ScheduleHorizontalRectangleBorder), new PropertyMetadata(null));
        /// <summary>
        /// gets and sets brush value for bottom brush
        /// </summary>
        public Brush BottomBrush
        {
            get
            {
                return (Brush)this.GetValue(ScheduleHorizontalRectangleBorder.BottomBrushProperty);
            }

            set
            {
                this.SetValue(ScheduleHorizontalRectangleBorder.BottomBrushProperty, value);
            }
        }
    }

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Class that holds external border for scheduleView
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleHorizontalRectangleBorderExt : ScheduleHorizontalRectangleBorder
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleBorderExt"/>
        /// class.
        /// </summary>
        public ScheduleHorizontalRectangleBorderExt()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalRectangleBorderExt);
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ScheduleHorizontalRectangleBorderExt), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var rectangle = dpo as ScheduleHorizontalRectangleBorderExt;
            rectangle.GotoSelectedState();
        }

        private void GotoSelectedState()
        {
            if (this.IsSelected)
            {
                VisualStateManager.GoToState(this, "Selected", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }


        /// <summary>
        /// Gets / Sets if this rectangle is selected under a particular time slot.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(ScheduleHorizontalRectangleBorderExt.IsSelectedProperty);
            }

            set
            {
                this.SetValue(ScheduleHorizontalRectangleBorderExt.IsSelectedProperty, value);
            }
        }


        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public Brush SelectionBackground
        {
            get { return (Brush)GetValue(SelectionBackgroundProperty); }
            set { SetValue(SelectionBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SelectionBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBackgroundProperty = DependencyProperty.Register("SelectionBackground",
            typeof(Brush), typeof(ScheduleHorizontalRectangleBorderExt), new PropertyMetadata(null));


        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsMarked.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMarkedProperty = DependencyProperty.Register("IsMarked", typeof(bool), typeof(ScheduleHorizontalRectangleBorderExt), new PropertyMetadata(false, OnIsMarkChanged));

        private static void OnIsMarkChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var rectangle = dpo as ScheduleHorizontalRectangleBorderExt;
            rectangle.GotoMarkState();
        }

        private void GotoMarkState()
        {
            if (this.IsMarked)
            {
                VisualStateManager.GoToState(this, "Marked", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        /// <summary>
        /// Gets / Sets the mark state if this rectangle is under a particular Schedule Appointment.
        /// </summary>
        public bool IsMarked
        {
            get
            {
                return (bool)this.GetValue(ScheduleHorizontalRectangleBorderExt.IsMarkedProperty);
            }

            set
            {
                this.SetValue(ScheduleHorizontalRectangleBorderExt.IsMarkedProperty, value);
            }
        }

    }
}

