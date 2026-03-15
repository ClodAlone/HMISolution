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

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows.Data;
using Syncfusion.WP.Converters;
using Syncfusion.WP.Utils;
using System.Windows;
using System.Windows.Media;
using Syncfusion.WP.Primitives;


namespace Syncfusion.WP.Controls.Input

#else
using Syncfusion.UI.Xaml.Converters;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a button control for the calendar dates <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarView"/> control.
    /// </summary>
    public class CalendarDayButton : Button
    {
        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarDayButton"/> class.
        /// </summary>
        public CalendarDayButton()
        {
            DefaultStyleKey = typeof (CalendarDayButton);
        }

        /// <summary>
        /// Gets or sets state of the button
        /// </summary>
        internal bool IsDateBlocked
        {
            get { return (bool)GetValue(IsDateBlockedProperty); }
            set {SetValue(IsDateBlockedProperty, value);}
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDateBlocked.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty IsDateBlockedProperty =
            DependencyProperty.Register("IsDateBlocked", typeof(bool), typeof(CalendarDayButton), new PropertyMetadata(false));


      private object GetParentItem(DependencyObject obj)
        {
            var item = obj;
            while (VisualTreeHelper.GetParent(item) != null && !(VisualTreeHelper.GetParent(item) is SfCalendar))
            {
                item =VisualTreeHelper.GetParent(item);
            }
            if (VisualTreeHelper.GetParent(item) is SfCalendar)
                return VisualTreeHelper.GetParent(item);
            return item;
        }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
      internal Ellipse Part_Circle = null;

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
         {
             Part_Circle = GetTemplateChild("Part_Circle") as Ellipse;
             base.OnApplyTemplate();
         }

        internal bool IsPointerReleased = false;

        /// <summary>
        /// Occurs when Pointer is released <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/> has changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var control = GetParentItem(this as DependencyObject);
            if (control is SfCalendar && (!this.IsDateBlocked))
            {
                var parent = control as SfCalendar;
                if (parent.SelectionMode == SelectionMode.Multiple && !parent.IsManipulated && parent.PART_CalendarView.IsLongPress)
                {
                   parent.PART_CalendarView.popup.Child = null;
                   
                    if (parent.SelectionStartDate != null && parent.SelectionEndDate != null &&
                        (DateTime) parent.SelectionStartDate == (DateTime) parent.SelectionEndDate)
                    {
                        parent.PART_CalendarView.IsLongPress = false;

                        if (!parent.ContainsDateinStartSelection((DateTime) parent.SelectionStartDate))
                        {
                            parent.SelectedDates.Add(new DateRange((DateTime) parent.SelectionStartDate));
                        }
                        parent.PART_CalendarView.ValidateSelectionStates((DateTime)parent.SelectionStartDate);
                        parent.SelectionStartDate = null;
                        parent.SelectionEndDate = null;
                        parent.PART_CalendarView.currentButton = null;
                        IsPointerReleased = true;
                    }
                    parent.SelectedDate = (DateTime)this.Content;
                }
            }
            base.OnPointerReleased(e);
        }
#endif
        /// <summary>
        /// Invoked when the content is changed
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (oldContent != null && newContent != null)
            {
                var olddate = oldContent.ToDateTime();
                var newdate = newContent.ToDateTime();

                if (newdate != olddate && newdate.Year != 0001 && olddate.Year != 0001)
                {
                    if (newdate > olddate)
                    {
#if WINRT
                        Direction = SlideDirection.Left;
#else
                    Direction = SlideDirection.Up;
#endif
                    }
                    else
                    {
#if WINRT
                        Direction = SlideDirection.Right;
#else
                    Direction = SlideDirection.Down;
#endif
                    }
                }
                if (IsHitTestVisible)
                {
                    if (newdate.Month == DateTime.Now.Month && newdate.Day == DateTime.Now.Day &&
                        newdate.Year == DateTime.Now.Year)
                    {
                        VisualStateManager.GoToState(this, "Today", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "Other", true);
                    }
                }
                SfCalendar parent = GetParentItem(this as DependencyObject) as SfCalendar;
                if (parent!=null && parent.PART_Month != null)
                {
                    Binding binding = new Binding()
                    {
                        Source = parent.DisplayDate,
                        Converter = new CalendarHeaderFormatter(),
                        ConverterParameter =parent.Culture,
                        Mode = BindingMode.TwoWay
                    };
                    parent.PART_Month.SetBinding(CalendarDayButton.ContentProperty, binding);
                }
            }

            base.OnContentChanged(oldContent, newContent);
        }

        private TransitionContentControl PART_Content;

        /// <summary>
        /// Gets or sets the direction
        /// </summary>
        public SlideDirection Direction
        {
            get
            {
                if(PART_Content == null)
                {
                    PART_Content = GetTemplateChild("PART_Content") as TransitionContentControl;
                }
                var transition = PART_Content.Transition as SlideTransition;

                if (transition != null)
                    return transition.Direction;
                else
                    return SlideDirection.Default;
            }

            set
            {
                if(PART_Content == null)
                {
                    PART_Content = GetTemplateChild("PART_Content") as TransitionContentControl;
                }
                var transition = PART_Content.Transition as SlideTransition;

                if (transition != null)
                    transition.Direction = value;
            }
        }

        internal bool UpdateActiveVisualState(bool isactive)
        {
            bool returnflag = false;
            if (isactive)
            {
                returnflag = VisualStateManager.GoToState(this, "Active", true);
            }
            else
            {
                returnflag = VisualStateManager.GoToState(this, "InActive", true);
            }
            return returnflag;
        }

        internal bool UpdateSelectionState(bool isselected)
        {
            bool returnflag = false;
            if (isselected)
            {
                returnflag = VisualStateManager.GoToState(this, "Selected", true);
            }
            else
            {
                returnflag = VisualStateManager.GoToState(this, "Unselected", true);
            }
            return returnflag;
        }

#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        internal bool UpdateMultiSelectionState(bool isselected)
        {
            bool returnflag = false;
            if (isselected)
            {
                returnflag = VisualStateManager.GoToState(this, "Multiple", true);
            }
            else
            {
                returnflag = VisualStateManager.GoToState(this, "Unselected", true);
            }
            return returnflag;
        }

        internal bool UpdateLongPressState(bool isselected)
        {
            bool returnflag = false;
            if (isselected)
            {
                returnflag = VisualStateManager.GoToState(this, "LongPress", true);
            }
            else
            {
                returnflag = VisualStateManager.GoToState(this, "Unselected", true);
            }
            return returnflag;
        }

     
#endif      
        /// <summary>
        /// Occurs when the pointer is entered
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if(!this.IsDateBlocked)
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                base.OnMouseEnter(e);
#else
                base.OnPointerEntered(e);
#endif
        }

        
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        /// <summary>
        /// Occurs when the pointer is pressed
        /// </summary>
        protected override void OnClick()
#else
        /// <summary>
        /// Occurs when the pointer is pressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if(!this.IsDateBlocked)
                
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                 base.OnClick();
#else
		VisualStateManager.GoToState(this, "Pressed", true);
                base.OnPointerPressed(e);
#endif
        }
    }
}
