#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
using System.Windows;
#if !(WINDOWS_PHONE_7||WPF||SILVERLIGHT||WINDOWS_PHONE)
using Syncfusion.UI.Xaml.Converters;
using Syncfusion.UI.Xaml.Utils;
using Windows.System;
using Windows.UI.Core;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Data;
using Syncfusion.WP.Converters;
using Syncfusion.WP.Utils;
using System.Windows.Media;
using Syncfusion.WP.Primitives;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.WP.Controls.Input

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using System.Collections;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a control that displays the day, months and years in given format.
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/>
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarView"/>
    public class SfCalendar : Control,IDisposable
    {
        private const double CManipulationThreshold = 3.0;

        /// <summary>
        /// Invokes an event when the Previous scroll button is clicked.
        /// </summary>
        public event SelectionChangedEventHandler PreviousScrollButtonClicked;

        /// <summary>
        /// Invokes an event when the Next scroll button is clicked.
        /// </summary>
        public event SelectionChangedEventHandler NextScrollButtonClicked;

        /// <summary>
        /// Invokes an event when the selection is changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        internal object SelectionStartDate, SelectionEndDate;
		
		internal bool IsManipulated = false;
#endif
        internal DateTime displayDate;
        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/> class.
        /// </summary>
        public SfCalendar()
        {
            DefaultStyleKey = typeof(SfCalendar);
            this.Loaded += CalendarLoaded;
            this.Unloaded += SfCalendar_Unloaded;
            BlackOutDates = new DateTimeCollection();
            VisibleDates = new DateTimeCollection();
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
            SelectedDates=new ObservableCollection<DateRange>();
#endif
        }

        void SfCalendar_Unloaded(object sender, RoutedEventArgs e)
        {
            if(BlackOutDates!=null)
                this.BlackOutDates.CollectionChanged -= OnBlackOutDatesChanged;
            if(VisibleDates!=null)
                this.VisibleDates.CollectionChanged -= OnVisibleDatesChanged;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
            this.SelectedDates.CollectionChanged -= SelectedDates_CollectionChanged;
            this.SelectedDates.Clear();
            if (PART_CalendarView != null)
            PART_CalendarView.UpdateSelectionStates(true);
#endif
        }

        private static void OnCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {       
            var Calendar = sender as SfCalendar;
            if (Calendar != null)
            {
                if (Calendar.BlackOutDates != null)
                    Calendar.OnCollectionChanged(e);
                else if(e.NewValue==null && e.OldValue!=null && (e.OldValue as DateTimeCollection).Count>0)
                   Calendar.BlackOutDates =e.OldValue as DateTimeCollection;
                Calendar.Refresh();
            }
        }
        /// <summary>
        /// occurs when BlackoutDates given as new collection.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PART_CalendarView != null && PART_CalendarView.PART_LayoutRoot != null)
            {
                ValidateBlackOutDates();
            }
        }

        private void OnBlackOutDatesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PART_CalendarView != null && PART_CalendarView.PART_LayoutRoot != null)
            {
                ValidateBlackOutDates();
                PART_CalendarView.DrawCurrentMonthCells(displayDate, false);
            }
        }
        private static void OnBlackOutCellTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfCalendar instance = sender as SfCalendar;
            if (instance != null)
                instance.Refresh();
        }

        private static void OnDayNameCellTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfCalendar instance = sender as SfCalendar;
            if (instance != null)
                instance.RenderWeekDays();
        }
        void OnVisibleDatesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PART_CalendarView != null && PART_CalendarView.PART_LayoutRoot != null)
            {
                ValidateBlackOutDates();
                PART_CalendarView.DrawCurrentMonthCells(displayDate, false);
            }
        }

        internal void ValidateBlackOutDates()
        {
            foreach (var item in PART_CalendarView.PART_LayoutRoot.Children)
            {
                var button = item as CalendarDayButton;
                if (button != null)
                {
                    var content = button.Content;
                    if (content != null)
                    {
                        var dateTime = DateTime.Parse(content.ToString());
                        if (BlackOutDates != null && BlackOutDates.ContainsDate(dateTime))
                        {
                            button.IsDateBlocked = true;                           
                        }
                        else if(ValidateDate(dateTime) && VisibleDates.Count>0)
                        {
                            if (VisibleDates!=null && VisibleDates.ContainsDate(dateTime))
                            {
                                button.IsDateBlocked = false;
                            }
                            else
                            {
                                button.IsDateBlocked = true;
                            }
                        }
                        else if (ValidateDate(dateTime))
                        {
                            button.IsDateBlocked = false;
                        }
                    }
                }
            }
        }

        void CalendarLoaded(object sender, RoutedEventArgs e)
        {
            displayDate = DateTime.Parse(DisplayDate.ToString());       
            if(BlackOutDates!=null)
                this.BlackOutDates.CollectionChanged += OnBlackOutDatesChanged;
            if(VisibleDates!=null)
                this.VisibleDates.CollectionChanged += OnVisibleDatesChanged;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
            this.SelectedDates.CollectionChanged -= SelectedDates_CollectionChanged;
            this.SelectedDates.CollectionChanged += SelectedDates_CollectionChanged;
#endif
        }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        void SelectedDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                List<object> oldItems = new List<object>();
                List<object> newItems = new List<object>();
                oldItems.Add(e.OldItems);
                newItems = SelectedDates.ToList<object>();
                SelectionChangedEventArgs selectionargs = new SelectionChangedEventArgs(oldItems, newItems);
                SelectionChanged(this, selectionargs);
            }
            Refresh();
        }
#endif
        internal CalendarView PART_CalendarView;

        internal CalendarDayButton PART_Month;

        private Grid PART_WeekDays;

        private Button PART_Previous, PART_Next;

        private Point? manipulationStartPoint;

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_CalendarView = GetTemplateChild("PART_CalendarView") as CalendarView;
            PART_WeekDays = GetTemplateChild("PART_WeekDays") as Grid;
            PART_Previous = GetTemplateChild("PART_Previous") as Button;
            PART_Next = GetTemplateChild("PART_Next") as Button;
            PART_Month = GetTemplateChild("Part_Month") as CalendarDayButton;
            
            if(PART_CalendarView != null)
            {
                PART_CalendarView.parentCalendar = this;
            }
            RenderWeekDays();

            if (PART_Previous != null)
            {
                PART_Previous.Command=ScrollPreviousCommand;
            }
            if (PART_Next != null)
            {
                PART_Next.Command=ScrollNextCommand;
            }
            if (PART_Month != null)
            {
                Binding binding =new Binding(){Source=DisplayDate,Converter=new CalendarHeaderFormatter(),ConverterParameter=Culture,Mode=BindingMode.TwoWay}; 
                PART_Month.SetBinding(CalendarDayButton.ContentProperty,binding);
            }
        }

        #region Commands

        private ICommand scrollNextCommand;

        private ICommand scrollPreviousCommand;

        /// <summary>
        /// Executes the command when ScrollNext Button is pressed.
        /// </summary>
        public ICommand ScrollNextCommand
        {
            get
            {
                if (scrollNextCommand == null)
                {
                    scrollNextCommand = new DelegateCommand(param => NextMonth());
                }
                return scrollNextCommand;
            }
        }

        /// <summary>
        /// Executes the command when ScrollPrevious Button is pressed.
        /// </summary>
        public ICommand ScrollPreviousCommand
        {
            get
            {
                if (scrollPreviousCommand == null)
                {
                    scrollPreviousCommand = new DelegateCommand(param => PreviousMonth());
                }
                return scrollPreviousCommand;
            }
        }
        #endregion  

        /// <summary>
        /// Invoked when the next month is clicked
        /// </summary>
        public void NextMonth()
        {
            if (VisibleMaxDate==null || VisibleMaxDate!=null && ValidateMonth(displayDate.AddMonths(1)))
            {
                if (NextScrollButtonClicked != null)
                {
                    List<object> oldItems = new List<object>();
                    List<object> newItems = new List<object>();
                    oldItems.Add(displayDate);
                    newItems.Add(displayDate.AddMonths(1));
                    SelectionChangedEventArgs selectionargs = new SelectionChangedEventArgs(oldItems, newItems);
                    NextScrollButtonClicked(this, selectionargs);
                }
                DisplayDate = displayDate.AddMonths(1);
            }
        }

        /// <summary>
        /// Invoked when the previous month is clicked
        /// </summary>
        public void PreviousMonth()
        {
            if (VisibleMinDate==null || VisibleMinDate!=null && ValidateMonth(displayDate.AddMonths(-1)))
            {
                if (PreviousScrollButtonClicked != null)
                {
                    List<object> oldItems = new List<object>();
                    List<object> newItems = new List<object>();
                    oldItems.Add(displayDate);
                    newItems.Add(displayDate.AddMonths(-1));
                    SelectionChangedEventArgs selectionargs = new SelectionChangedEventArgs(oldItems, newItems);
                    PreviousScrollButtonClicked(this, selectionargs);
                }
                DisplayDate = displayDate.AddMonths(-1);
            }
        }

        /// <summary>
        /// Occurs when the focus is obtained
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if(e.Delta>0)
#else
        protected override void OnPointerWheelChanged(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)        
        {
            if (e.GetCurrentPoint(this).Properties.MouseWheelDelta > 0 &&(VisibleMinDate==null || VisibleMinDate!=null && ValidateMonth(displayDate.AddMonths(-1))))
#endif
            {
                DisplayDate = displayDate.AddMonths(-1);
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            else if (e.Delta <= 0 && (VisibleMaxDate == null || VisibleMaxDate != null && ValidateMonth(displayDate.AddMonths(1))))
#else
            else if(e.GetCurrentPoint(this).Properties.MouseWheelDelta <= 0 && (VisibleMaxDate==null || VisibleMaxDate!=null && ValidateMonth(displayDate.AddMonths(1))))
#endif
            {
                DisplayDate = displayDate.AddMonths(1);
            }
            e.Handled = true;
        }
        /// <summary>
        /// Occurs when the manipulation is started
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnManipulationStarted(System.Windows.Input.ManipulationStartedEventArgs e)
#else
        protected override void OnManipulationStarted(Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
#endif
        {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            manipulationStartPoint = e.ManipulationOrigin; 
#else
            manipulationStartPoint = e.Position;
#endif
            base.OnManipulationStarted(e);
        }

        /// <summary>
        /// Manipulates the Display date of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/>
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e)
#else
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            IsManipulated = false;
            base.OnManipulationCompleted(e);
        }

        /// <summary>
        /// Manipulates the Display date of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationDelta(Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
#endif
        {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            Point currentPoint = e.DeltaManipulation.Translation;
            if (manipulationStartPoint.HasValue)
            {
#else
            Point currentPoint = e.Position;

            if (manipulationStartPoint.HasValue && !PART_CalendarView.IsLongPress&& Math.Abs(currentPoint.X- manipulationStartPoint.Value.X)>50)
            {
#endif
#if (WINDOWS_PHONE||WINDOWS_PHONE_7)
                if (currentPoint.Y<0)
                {
                    if (manipulationStartPoint.Value.Y - currentPoint.Y > CManipulationThreshold && (VisibleMaxDate == null || VisibleMaxDate != null && ValidateMonth(displayDate.AddMonths(1))))
                    {
                        this.NextMonth();
                        manipulationStartPoint = null;
                    }
                }
                else if (currentPoint.Y - manipulationStartPoint.Value.Y > CManipulationThreshold && (VisibleMinDate == null || VisibleMinDate != null && ValidateMonth(displayDate.AddMonths(-1))))
                {
                    this.PreviousMonth();
                    manipulationStartPoint = null;
                }
#else
                if (currentPoint.X < manipulationStartPoint.Value.X)
                {
                    if (manipulationStartPoint.Value.X - currentPoint.X > CManipulationThreshold && (VisibleMaxDate==null || VisibleMaxDate!=null && ValidateMonth(displayDate.AddMonths(1))))
                    {
                        this.NextMonth();
                        manipulationStartPoint = null;
                    }
                }
                else if (currentPoint.X- manipulationStartPoint.Value.X > CManipulationThreshold && (VisibleMinDate==null || VisibleMinDate!=null && ValidateMonth(displayDate.AddMonths(-1))))
                {
                    this.PreviousMonth();
                    manipulationStartPoint = null;
                }
               IsManipulated = true;
                PART_CalendarView.timer.Stop();
#endif
            }
            base.OnManipulationDelta(e);
        }
       
        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
#else
        protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
#endif
        {
            object  resultdate=null;
            if (SelectionMode != SelectionMode.None && FocusManager.GetFocusedElement() is CalendarDayButton)
            {
                DateTime selectedDateTime;
                if (SelectedDate == null)
                {
                    selectedDateTime = DateTime.Parse((FocusManager.GetFocusedElement() as CalendarDayButton).Content.ToString());
                }
                else
                {
                    selectedDateTime = DateTime.Parse(SelectedDate.ToString());
                }
                var prevDate = selectedDateTime;
                
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    if (e.Key == Key.Up)
#else
                    if (e.Key == VirtualKey.Up)
#endif
                    {
                        if (FindNextValidDate(selectedDateTime, -7, e.Key, out resultdate))
                            SelectedDate = resultdate;
                        else if (VisibleMinDate == null || ValidateMinMaxDate(selectedDateTime.AddDays(-7),VisibleMinDate.ToDateTime()))
                            SelectedDate = selectedDateTime.AddDays(-7);
                    }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    else if (e.Key == Key.Down)
#else
                    else if (e.Key == VirtualKey.Down)
#endif
                    {        
                        if (FindNextValidDate(selectedDateTime, 7, e.Key, out resultdate))
                            SelectedDate = resultdate;
                        else if (VisibleMaxDate == null || ValidateMinMaxDate(selectedDateTime.AddDays(7),VisibleMaxDate.ToDateTime()))
                            SelectedDate = selectedDateTime.AddDays(7);
                    }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    else if (e.Key == Key.Right)
#else
                    else if (e.Key == VirtualKey.Right)
#endif
                    {
                        if (FindNextValidDate(selectedDateTime, 1, e.Key, out resultdate))
                            SelectedDate=resultdate;
                        else if (VisibleMaxDate == null || ValidateMinMaxDate(selectedDateTime.AddDays(1),VisibleMaxDate.ToDateTime()))
                            SelectedDate = selectedDateTime.AddDays(1);
                    }
#if WINDOWS_PHONE || WINDOWS_PHONE_7
                    else if (e.Key == Key.Left)
#else
                    else if (e.Key == VirtualKey.Left)
#endif
                    {
                       
                        if (FindNextValidDate(selectedDateTime, -1, e.Key, out resultdate))
                            SelectedDate = resultdate;
                        else if (VisibleMinDate == null || ValidateMinMaxDate(selectedDateTime.AddDays(-1), VisibleMinDate.ToDateTime()))
                            SelectedDate = selectedDateTime.AddDays(-1);
                    }
#if !(WINDOWS_PHONE || WINDOWS_PHONE_7)

                    else if (e.Key == VirtualKey.PageUp)

                    {
                        if (VisibleMinDate == null || ValidateMinMaxDate(selectedDateTime.AddMonths(-1), VisibleMinDate.ToDateTime()))
                            SelectedDate = selectedDateTime.AddMonths(-1);
                    }


                    else if (e.Key == VirtualKey.PageDown)

                    {
                        if (VisibleMaxDate == null || ValidateMinMaxDate(selectedDateTime.AddMonths(1), VisibleMaxDate.ToDateTime()))
                            SelectedDate = selectedDateTime.AddMonths(1);
                    }
                    else if (e.Key == VirtualKey.Home)
                    {
                            if (VisibleMinDate!=null && selectedDateTime.FirstDay() <= VisibleMinDate.ToDateTime())
                                SelectedDate = VisibleMinDate.ToDateTime();
                            else
                                SelectedDate = selectedDateTime.FirstDay();
                    }
                    else if (e.Key == VirtualKey.End)
                    {
                            if (VisibleMaxDate!=null && selectedDateTime.LastDay() >= VisibleMaxDate.ToDateTime())
                                SelectedDate = VisibleMaxDate.ToDateTime();
                            else
                                SelectedDate = selectedDateTime.LastDay();
                    }
#endif
                    DateTime  currentDate;
                if (SelectedDate != null)
                    currentDate = DateTime.Parse(SelectedDate.ToString());
                else
                    currentDate = DateTime.Parse((FocusManager.GetFocusedElement() as CalendarDayButton).Content.ToString());
                if (prevDate.Month != currentDate.Month)
                {
                    DisplayDate = currentDate;
                }
            }
        }

        /// <summary>
        /// Used to get the visual container for the corresponding DateTime object.
        /// </summary>
        /// <param name="dateTime">The DateTime for which the container to get.</param>
        /// <returns>CalendarDayButton, the container.</returns>
        /// <remarks></remarks>
        public CalendarDayButton GetContainerFromDateTime(DateTime dateTime)
        {
            if (PART_CalendarView != null && PART_CalendarView.PART_LayoutRoot != null)
            {
                return (from button in PART_CalendarView.PART_LayoutRoot.Children.OfType<CalendarDayButton>() 
                        where button.Content != null 
                        let datetime = DateTime.Parse(button.Content.ToString()) 
                        where datetime == dateTime 
                        select button).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the template for the data used as header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(SfCalendar), new PropertyMetadata(null,OnTemplateChanged));

        /// <summary>
        /// Gets or sets the template for the PreviousScrollButton.
        /// </summary>
        public ControlTemplate PreviousScrollButtonTemplate
        {
            get { return (ControlTemplate)GetValue(PreviousScrollButtonTemplateProperty); }
            set { SetValue(PreviousScrollButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PreviousScrollButtonTemplateProperty =
            DependencyProperty.Register("PreviousScrollButtonTemplate", typeof(ControlTemplate), typeof(SfCalendar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template for the NextScrollButton.
        /// </summary>
        public ControlTemplate NextScrollButtonTemplate
        {
            get { return (ControlTemplate)GetValue(NextScrollButtonTemplateProperty); }
            set { SetValue(NextScrollButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NextScrollButtonTemplateProperty =
            DependencyProperty.Register("NextScrollButtonTemplate", typeof(ControlTemplate), typeof(SfCalendar), new PropertyMetadata(null));

        /// <summary>
        /// Returns a value when set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNavigationButton
        {
            get { return (bool)GetValue(ShowNavigationButtonProperty); }
            set { SetValue(ShowNavigationButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowNavigationButtonProperty =
            DependencyProperty.Register("ShowNavigationButton", typeof(bool), typeof(SfCalendar), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the template for the BlackOutCell.
        /// </summary>
        public DataTemplate BlackOutCellTemplate
        {
            get { return (DataTemplate)GetValue(BlackOutCellTemplateProperty); }
            set { SetValue(BlackOutCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BlackOutCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BlackOutCellTemplateProperty =
            DependencyProperty.Register("BlackOutCellTemplate", typeof(DataTemplate), typeof(SfCalendar), new PropertyMetadata(null, OnBlackOutCellTemplateChanged));


        /// <summary>
        /// Gets or sets the template for the Cell <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarDayButton"/>
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarVieew"/>
        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(SfCalendar), new PropertyMetadata(null,OnTemplateChanged));

        /// <summary>
        /// Gets or sets the TemplateSelector for the Cell <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarDayButton"/>
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarVieew"/>
        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(SfCalendar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the dates used as BlackOutDates
        /// </summary>
        public DateTimeCollection BlackOutDates
        {
            get { return (DateTimeCollection)GetValue(BlackOutDatesProperty); }
            set { SetValue(BlackOutDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BlackOutDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BlackOutDatesProperty =
            DependencyProperty.Register("BlackOutDates", typeof(DateTimeCollection), typeof(SfCalendar), new PropertyMetadata(null,OnCollectionChanged));

        /// <summary>
        /// Gets or sets the SelectionMode of the calendar
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.SelectionMode"/>
        /// <value>
        /// The default value is <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SelectionMode.Single"/>
        /// </value>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectionMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(SfCalendar), new PropertyMetadata(SelectionMode.Single, OnSelectionModeChanged));

        private static void OnSelectionModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as SfCalendar;
#if !(WINDOWS_PHONE || WINDOWS_PHONE_7)
            if (calendar.SelectionMode != SelectionMode.Multiple && calendar.SelectedDates.Count > 0)
            {
                calendar.SelectedDates.Clear();
            }
#endif
            if (calendar != null)            
            {
                if(calendar.PART_CalendarView != null&& calendar.SelectedDate!=null)  
                    calendar.PART_CalendarView.ValidateSelectionStates(DateTime.Parse(calendar.SelectedDate.ToString()));
                calendar.Refresh();
            }
        }
       
        /// <summary>
        /// Gets or sets the date that has been selected by the user.
        /// </summary>
        public object SelectedDate
        {
            get { return (object)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(object), typeof(SfCalendar), new PropertyMetadata(null, OnSelectedDateChanged));

#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        /// <summary>
        /// Gets or sets the date that has been selected by the user.
        /// </summary>
        public ObservableCollection<DateRange> SelectedDates
        {
            get { return (ObservableCollection<DateRange>)GetValue(SelectedDatesProperty); }
            set { SetValue(SelectedDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateRange>), typeof(SfCalendar), new PropertyMetadata(null));
#endif

        private static void OnSelectedDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as SfCalendar;
            if (calendar != null)
            {
                calendar.OnSelectedDateChanged(e);
            }
        }

#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        internal bool ContainsDateinSelection(DateTime startdate,DateTime endDate)
        {
            var query = from DateRange dateRange in SelectedDates
                       where dateRange.StartDate == startdate.Date && dateRange.EndDate == endDate.Date
                       select dateRange;

            return query.Any();
        }

        internal bool ContainsDateinStartSelection(DateTime startdate)
        {
            var query = from DateRange dateRange in SelectedDates
                        where dateRange.StartDate == startdate.Date
                        select dateRange;

            return query.Any();
        }

        internal DateRange ContainsDate(DateTime startDate,DateTime endDate)
        {
            var query = from DateRange dateRange in SelectedDates
                        where dateRange.StartDate == startDate.Date && dateRange.EndDate == endDate.Date
                        select dateRange;

            return query.SingleOrDefault() as DateRange;
        }

        internal DateRange ContainsDate(DateTime startDate)
        {
            var query = from DateRange dateRange in SelectedDates
                        where dateRange.StartDate == startDate.Date
                        select dateRange;

            return query.SingleOrDefault() as DateRange;
        }
#endif
        /// <summary>
        /// Occurs when the SelectedDate has changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedDateChanged(DependencyPropertyChangedEventArgs e)
        {
#if !(WINDOWS_PHONE || WINDOWS_PHONE_7)
            if (SelectionMode != SelectionMode.Multiple && SelectedDates.Count > 0)
            {
               SelectedDates.Clear();
            }
#endif
            if (PART_CalendarView != null)
            {
                PART_CalendarView.ValidateSelectionStates(DateTime.Parse(e.NewValue.ToString()));
            }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
            if (SelectionChanged != null && SelectionMode != SelectionMode.Multiple)
#else
            if (SelectionChanged != null)
#endif
            {
                List<object> oldItems = new List<object>();
                List<object> newItems = new List<object>();
                oldItems.Add(e.OldValue);
                newItems.Add(e.NewValue);
                SelectionChangedEventArgs selectionargs = new SelectionChangedEventArgs(oldItems, newItems);
                SelectionChanged(this, selectionargs);
            }
        }

        /// <summary>
        /// Gets or sets the date to be displayed.
        /// </summary>
        public object DisplayDate
        {
            get { return (object)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register("DisplayDate", typeof(object), typeof(SfCalendar), new PropertyMetadata(DateTime.Today, OnDisplayDateChanged));

        private static void OnDisplayDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var view = sender as SfCalendar;
            if (view != null)
                view.OnDisplayDateChanged(args);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayNameCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayNameCellTemplateProperty =
            DependencyProperty.Register("DayNameCellTemplate", typeof(DataTemplate), typeof(SfCalendar), new PropertyMetadata(default(DataTemplate), OnDayNameCellTemplateChanged));

        /// <summary>
        /// Gets or sets the Template for the DayName cell
        /// </summary>
        public DataTemplate DayNameCellTemplate
        {
            get { return (DataTemplate) GetValue(DayNameCellTemplateProperty); }
            set { SetValue(DayNameCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayNameDisplayMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayNameDisplayModeProperty =
            DependencyProperty.Register("DayNameDisplayMode", typeof (DayNameDisplayMode), typeof (SfCalendar), new PropertyMetadata(DayNameDisplayMode.AbbreviatedDayNames));

        /// <summary>
        /// Gets or sets the DayNameDisplayMode <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DayNameDisplayMode"/>
        /// </summary>
        /// <value>
        /// The default value is <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DayNameDisplayMode.AbbreviatedDayNames"/>
        /// </value>
        public DayNameDisplayMode DayNameDisplayMode
        {
            get { return (DayNameDisplayMode) GetValue(DayNameDisplayModeProperty); }
            set { SetValue(DayNameDisplayModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FirstDayofWeek <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.FirstDayofWeek"/>
        /// </summary>
        /// <value>
        /// The default value is <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.FirstDayofWeek.AbbreviatedDayNames"/>
        /// </value>
        public DayOfWeek FirstDayofWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayofWeekProperty); }
            set { SetValue(FirstDayofWeekProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayNameDisplayMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FirstDayofWeekProperty =
            DependencyProperty.Register("FirstDayofWeek", typeof(DayOfWeek), typeof(SfCalendar), new PropertyMetadata(DayOfWeek.Sunday,OnFirstDayofWeekChanged));

        private static void OnFirstDayofWeekChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var view = sender as SfCalendar;
            if (view != null)
                view.OnFirstDayofWeekChanged(args);
        }

        /// <summary>
        /// Gets or sets the FirstDayofWeek <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Culture"/>
        /// </summary>
        /// <value>
        /// The default value is <see 
        /// cref="T:System.Globalization.CultureInfo"/>
        /// </value>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Culture.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(SfCalendar), new PropertyMetadata(CultureInfo.CurrentUICulture, new PropertyChangedCallback(OnCultureChanged)));


        private static void OnCultureChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfCalendar instance = obj as SfCalendar;
            instance.OnCultureChanged(args);
           
        }

        private static void OnTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfCalendar instance = obj as SfCalendar;
            if(instance!=null)
             instance.Refresh();
        }
        private void OnCultureChanged(DependencyPropertyChangedEventArgs args)
        {
           
                if (PART_Month != null)
                {
                    Binding binding = new Binding()
                        {
                            Source = DisplayDate,
                            Converter = new CalendarHeaderFormatter(),
                            ConverterParameter = Culture,
                            Mode = BindingMode.TwoWay
                        };
                    PART_Month.SetBinding(CalendarDayButton.ContentProperty, binding);
                }
                RenderWeekDays();
                Refresh();
                UpdateLayout();
                
        }

        private void OnFirstDayofWeekChanged(DependencyPropertyChangedEventArgs args)
        {
            Refresh();
            RenderWeekDays();
            UpdateLayout();
        }

        /// <summary>
        /// Gets or sets the minimum date
        /// </summary>
        public object VisibleMinDate
        {
            get { return (object)GetValue(VisibleMinDateProperty); }
            set { SetValue(VisibleMinDateProperty, value); }
        }
      
        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleMinDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleMinDateProperty =
            DependencyProperty.Register("VisibleMinDate", typeof(object), typeof(SfCalendar), new PropertyMetadata(null, new PropertyChangedCallback(OnVisibleMinDateChanged)));

        /// <summary>
        /// Gets or sets the maximum date
        /// </summary>
        public object VisibleMaxDate
        {
            get { return (object)GetValue(VisibleMaxDateProperty); }
            set { SetValue(VisibleMaxDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleMaxDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleMaxDateProperty =
            DependencyProperty.Register("VisibleMaxDate", typeof(object), typeof(SfCalendar), new PropertyMetadata(null,new PropertyChangedCallback(OnVisibleMaxDateChanged)));

        /// <summary>
        /// Gets or sets the template for the disabled date.
        /// </summary>
        public DataTemplate DisabledCellTemplate
        {
            get { return (DataTemplate)GetValue(DisabledCellTemplateProperty); }
            set { SetValue(DisabledCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisabledCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisabledCellTemplateProperty =
            DependencyProperty.Register("DisabledCellTemplate", typeof(DataTemplate), typeof(SfCalendar), new PropertyMetadata(null,OnTemplateChanged));

        /// <summary>
        /// Gets or sets the collection for the visible dates.
        /// </summary>
        public DateTimeCollection  VisibleDates
        {
            get { return (DateTimeCollection )GetValue(VisibleDatesProperty); }
            set { SetValue(VisibleDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleDatesProperty =
            DependencyProperty.Register("VisibleDates", typeof(DateTimeCollection), typeof(SfCalendar), new PropertyMetadata(null,OnTemplateChanged));

        /// <summary>
        /// Invoked when VisibleMinDate changed
        /// </summary>
        private static void OnVisibleMinDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfCalendar control = (sender as SfCalendar);
            if (control != null && args.NewValue != null)
            {
                if (control.VisibleMaxDate!=null && args.NewValue.ToDateTime() > control.VisibleMaxDate.ToDateTime())
                {
                    throw new InvalidOperationException("Invalid VisibleMinDate");
                }
                else if (args.NewValue.ToDateTime() > control.DisplayDate.ToDateTime())
                {
                    DateTime date = args.NewValue.ToDateTime();
                    control.DisplayDate = new DateTime(date.Year, date.Month, control.displayDate.Day);
                }
                else
                {
                    control.Refresh();
                }
            }
        }

        /// <summary>
        /// Invoked when VisibleMaxDate changed
        /// </summary>
        private static void OnVisibleMaxDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfCalendar control = (sender as SfCalendar);
            if (control != null && args.NewValue != null)
            {
                if (control.VisibleMinDate!=null && args.NewValue.ToDateTime() < control.VisibleMinDate.ToDateTime())
                {
                    throw new InvalidOperationException("Invalid VisibleMaxDate");
                }
                else if (args.NewValue.ToDateTime() < control.DisplayDate.ToDateTime())
                {
                    DateTime date = args.NewValue.ToDateTime();
                    control.DisplayDate = new DateTime(date.Year, date.Month, control.displayDate.Day);
                }
                else
                    control.Refresh();
            }
        }

#if WINDOWS_PHONE ||NETFX_CORE
        private async void OnDisplayDateChanged(DependencyPropertyChangedEventArgs args)
#else
        private void OnDisplayDateChanged(DependencyPropertyChangedEventArgs args)
#endif
        {
            if(args.NewValue!=null)
                displayDate = DateTime.Parse(args.NewValue.ToString());
            if(PART_CalendarView != null)
            {
#if WINDOWS_PHONE
                await Task.Run(() =>
                                   {
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                   {
#else
#if !NETFX_CORE
                 Deployment.Current.Dispatcher.BeginInvoke(()=>
                    {
#else              
           await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,()=>
              {
#endif
#endif
                PART_CalendarView.OnRender(displayDate, false);
                if (SelectedDate != null)
                    PART_CalendarView.ValidateSelectionStates(DateTime.Parse(SelectedDate.ToString()));
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                if (SelectedDates.Count > 0)
                    PART_CalendarView.ValidateSelectionStates(displayDate);
#endif
                                 
#if !WINDOWS_PHONE

                   });
#else
                    });
                                   });
#endif 
                if((VisibleMinDate!=null || VisibleMaxDate!=null) && ShowNavigationButton)
                {
                    UpdateNavigationButtonState(DisplayDate.ToDateTime());
                }
            }
        }

        /// <summary>
        /// Updates the layout of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/> control.
        /// </summary>
        public void Refresh()
        {
            if (PART_CalendarView != null && PART_CalendarView.PART_LayoutRoot != null)
            {
                PART_CalendarView.ClearCellData();
                PART_CalendarView.DrawPreviousMonthCells(displayDate, true);
                PART_CalendarView.DrawCurrentMonthCells(displayDate, true);
                PART_CalendarView.DrawNextMonthCells(displayDate, true);
                UpdateNavigationButtonState(displayDate);
            }
        }

        private void RenderWeekDays()
        {
            string[] dayNames;
            if(DayNameDisplayMode == DayNameDisplayMode.AbbreviatedDayNames)
               dayNames= Culture.DateTimeFormat.AbbreviatedDayNames;
            else
            {
                dayNames = Culture.DateTimeFormat.DayNames;
            }

            if (PART_WeekDays != null)
            {
                PART_WeekDays.Children.Clear();
                int startDay;
                if (FirstDayofWeek == DayOfWeek.Sunday)
                    startDay = (int) Culture.DateTimeFormat.FirstDayOfWeek;
                else
                    startDay = (int) FirstDayofWeek;
                int j = 0;
                for (int i = startDay; i <CalendarView.CWeekDays ; i++,j++)
                {
                    var dayNameContent = new ContentPresenter(){Margin = new Thickness(2), Content = dayNames[i],ContentTemplate = DayNameCellTemplate};
                    Grid.SetColumn(dayNameContent, j);
                    PART_WeekDays.Children.Add(dayNameContent);
                }
                for (int i = 0 ; i < startDay; j++ , i++)
                {
                    var dayNameContent = new ContentPresenter() { Margin = new Thickness(2), Content = dayNames[i], ContentTemplate = DayNameCellTemplate };
                    Grid.SetColumn(dayNameContent, j);
                    PART_WeekDays.Children.Add(dayNameContent);
                }
            }
        }

        /// <summary>
        /// Validate month from the given date and retun the boolean
        /// </summary>
        /// <param name="_dateTime"></param>
        internal bool ValidateMonth(DateTime _dateTime)
        {            
            if(ValidateDate(_dateTime.FirstDay()) || ValidateDate(_dateTime.LastDay()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validate the given date and retun the boolean
        /// </summary>
        /// <param name="_dateTime"></param>
        internal bool ValidateDate(DateTime _dateTime)
        {
            if ((VisibleMinDate == null && VisibleMaxDate == null)
                || (VisibleMinDate == null && VisibleMaxDate != null && DateTime.Compare(_dateTime.Date, VisibleMaxDate.ToDateTime().Date) <= 0)
                || (VisibleMaxDate == null && VisibleMinDate != null && DateTime.Compare(_dateTime.Date, VisibleMinDate.ToDateTime().Date) >= 0)
                || (VisibleMinDate != null && VisibleMaxDate != null &&
                DateTime.Compare(_dateTime.Date, VisibleMaxDate.ToDateTime().Date) <= 0 && DateTime.Compare(_dateTime.Date, VisibleMinDate.ToDateTime().Date) >= 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    
        internal bool IsValidateMinMax(DateTime _dateTime)
        {
            if ((VisibleMinDate == null && VisibleMaxDate == null) || ValidateDate(_dateTime))
                return true;
            else
                return false;
        }
        private void UpdateNavigationButtonState(DateTime _dateTime)
        {
            if (PART_Next != null && _dateTime!=null)
            {
                if ((VisibleMinDate!=null || VisibleMaxDate!=null) && ValidateMonth(_dateTime.AddMonths(1)))
                {
                    PART_Next.IsEnabled = true;
                }
                else
                {
                    PART_Next.IsEnabled = false;
                }
            }
            if (PART_Previous!=null && _dateTime!=null)
            {
                if ((VisibleMinDate != null || VisibleMaxDate != null) && ValidateMonth(_dateTime.AddMonths(-1)))
                {
                    PART_Previous.IsEnabled = true;
                }
                else
                {
                    PART_Previous.IsEnabled = false;
                }
            }
        }

        private bool FindNextValidBlackedOutDate(DateTime _dateTime,int determinent,out object resultdate)
        {
            if (BlackOutDates != null && BlackOutDates.ContainsDate(_dateTime))
            {
                bool IsdateFound = FindNextValidBlackedOutDate(_dateTime.AddDays(determinent), determinent, out resultdate);
                if(IsdateFound && ValidateDate(resultdate.ToDateTime()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                resultdate = _dateTime;
                if (ValidateDate(_dateTime)&& (VisibleDates.Count==0 || VisibleDates.ContainsDate(_dateTime)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        private bool FindNextValidVisibleDate(DateTime _dateTime,out object resultdate,System.Windows.Input.Key  key)
#else
        private bool FindNextValidVisibleDate(DateTime _dateTime,out object resultdate,VirtualKey key)
#endif
        {
            bool result = false;
            List<DateTime> visibleDates = (from DateTime datetime in VisibleDates
                                           select datetime.Date).ToList<DateTime>();
            visibleDates = visibleDates.OrderBy(datetime => datetime.Date).ToList<DateTime>();
            resultdate = _dateTime;
            int index = visibleDates.IndexOf(_dateTime);

            switch(key)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                case Key.Up:
#else
                case VirtualKey.Up:
#endif
                    if (index - 1 >= 0)
                    {
                        for (int i = index - 1; i >= 0; i--)
                        {
                            if (visibleDates[i].DayOfWeek == _dateTime.DayOfWeek)
                            {
                                resultdate = visibleDates[i];
                                result = true;
                                break;
                            }
                        }
                    }
                    break;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                case Key.Down:
#else
                case VirtualKey.Down:
#endif                    
                    if (index + 1 <= visibleDates.Count-1)
                    {
                        for (int i = index + 1; i <= visibleDates.Count - 1; i++)
                        {
                            if (visibleDates[i].DayOfWeek == _dateTime.DayOfWeek)
                            {
                                resultdate = visibleDates[i];
                                result = true;
                                break;
                            }
                        }
                    }
                    break;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
                case Key.Right:
#else
                case VirtualKey.Right:
#endif
                    if (index + 1 <= visibleDates.Count-1)
                    {
                        resultdate = visibleDates[index + 1];
                        result = true;
                    }
                    else
                        result = false;
                    break;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
                case Key.Left:
#else
                case VirtualKey.Left:
#endif
                    if (index - 1 >= 0)
                    {
                        resultdate = visibleDates[index - 1];
                        result = true;
                    }
                    else
                        result = false;
                    break;
            }
            bool isValidDate = ValidateDate(resultdate.ToDateTime());
            if (result && !isValidDate || (isValidDate && BlackOutDates != null && BlackOutDates.ContainsDate(resultdate.ToDateTime())))
            {
                if (BlackOutDates != null && BlackOutDates.ContainsDate(resultdate.ToDateTime()))
                {
                    result = FindNextValidVisibleDate(resultdate.ToDateTime(), out resultdate, key);
                }
                else
                {
                    result = false;
                }
            }
            return result;   
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7        
        private bool FindNextValidDate(DateTime _dateTime, int determinent, System.Windows.Input.Key key, out Object resultdate)
#else
        private bool FindNextValidDate(DateTime _dateTime,int determinent,VirtualKey key,out Object resultdate)
#endif
        {
            if((BlackOutDates!=null && BlackOutDates.Count > 0 && FindNextValidBlackedOutDate(_dateTime.AddDays(determinent), determinent, out resultdate)) || 
                            (VisibleDates!=null && VisibleDates.Count>0 && FindNextValidVisibleDate(_dateTime,out resultdate,key)))
            {
                return true;
            }
            else
            {
                resultdate = _dateTime;
                return false;
            }
            
        }
   
        private bool ValidateMinMaxDate(DateTime _dateTime,DateTime MinMaxDate)
        {
            if(MinMaxDate != null && ValidateDate(_dateTime) 
                            && (BlackOutDates!=null && (BlackOutDates.Count==0 || !BlackOutDates.ContainsDate(_dateTime))) 
                            && (VisibleDates!=null && (VisibleDates.Count==0 || VisibleDates.ContainsDate(_dateTime))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Invoked to dispose the collections
        /// </summary>
        public void Dispose()
        {
            if (BlackOutDates != null)
            {
                BlackOutDates.Clear();
                BlackOutDates = null;
            }
            if(VisibleDates!=null)
            {
                VisibleDates.Clear();
                VisibleDates = null;
            }
        }
    }

    /// <summary>
    /// Creates a list for the mode of Displaying dayname
    /// </summary>
    public enum DayNameDisplayMode
    {
        /// <summary>
        /// DyaNames in abbreviated form
        /// </summary>
        AbbreviatedDayNames,
        /// <summary>
        /// Default form
        /// </summary>
        DayNames
    }
 
}
