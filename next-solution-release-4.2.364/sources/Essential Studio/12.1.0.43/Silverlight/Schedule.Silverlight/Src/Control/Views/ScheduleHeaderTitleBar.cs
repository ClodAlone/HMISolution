#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Collections.Specialized;

    /// <summary>
    /// Represents Schedule's HeaderTitle Bar.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
 
    public class ScheduleHeaderTitleBar : Control, IScheduleCalendarViewModelHost
    {
        private static DateTime MonthViewNavigatorDate;
        bool MonthSelection = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleHeaderTitleBar"/> class.
        /// </summary>
        public ScheduleHeaderTitleBar()
        {
            this.DefaultStyleKey = typeof(ScheduleHeaderTitleBar);
            this.DataContext = this;
        }

        #region Text (DependencyProperty)

        /// <summary>
        /// Gets / sets the Text property.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Text.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ScheduleHeaderTitleBar),
              new PropertyMetadata(string.Empty));

        #endregion

        #region SelectedDates (DependencyProperty)

        /// <summary>
        /// Gets / sets the SelectedDates property.
        /// </summary>
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            set { SetValue(SelectedDatesProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SelectedDates.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleHeaderTitleBar),
              new PropertyMetadata(null));

        #endregion

        #region SelectedDatesConverter (DependencyProperty)

        /// <summary>
        /// Gets / sets the Text property.
        /// </summary>
        public IValueConverter SelectedDatesConverter
        {
            get { return (IValueConverter)GetValue(SelectedDatesConverterProperty); }
            set { SetValue(SelectedDatesConverterProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SelectedDatesConverter.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesConverterProperty =
            DependencyProperty.Register("SelectedDatesConverter", typeof(IValueConverter), typeof(ScheduleHeaderTitleBar),
              new PropertyMetadata(new ScheduleTextToSelectedDatesConverter(), SelectedDatesConverterChanged));

        private static void SelectedDatesConverterChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scheduleHeaderTitleBar = dpo as ScheduleHeaderTitleBar;
            scheduleHeaderTitleBar.UpdateConverter((IValueConverter)args.NewValue);
        }

        private void UpdateConverter(IValueConverter newValue)
        {
            Binding TextBinding = new Binding() { Source = this.SelectedDates, Converter = newValue };
            SetBinding(ScheduleHeaderTitleBar.TextProperty, TextBinding);           
        }

        #endregion 

        #region IScheduleCalendarViewModelHost Members

        private ScheduleCalendarViewModel model;
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ScheduleCalendarViewModel Model
        {
            get { return this.model; }
        }

        /// <summary>
        /// Sets the calendar view model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);
            }
            this.model = model;
            this.SelectedDates = model.SelectedDates;            
            this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Model control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDates" || e.PropertyName == "TitleBarTextConverter")
            {
                this.SelectedDates = Model.SelectedDates;
               // this.SelectedDatesConverter = this.Model.TitleBarTextConverter;
                Binding TextBinding = new Binding() { Source = this.SelectedDates, Converter = this.SelectedDatesConverter };
                SetBinding(ScheduleHeaderTitleBar.TextProperty, TextBinding);  
            }
        }
        #endregion

        private Button prevButton;
        private Button nextButton;
        private TextBlock dateText;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.prevButton = this.GetTemplateChild("PART_PrevButton") as Button;
            this.nextButton = this.GetTemplateChild("PART_NextButton") as Button;
            this.dateText = this.GetTemplateChild("PART_DateText") as TextBlock;

            this.prevButton.Click += new System.Windows.RoutedEventHandler(prevButton_Click);
            this.nextButton.Click += new System.Windows.RoutedEventHandler(nextButton_Click);


            Binding TextBinding = new Binding() { Source = this.SelectedDates, Converter = this.SelectedDatesConverter };
            SetBinding(ScheduleHeaderTitleBar.TextProperty, TextBinding);   
        }


#if Test
        internal void nextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.Model == null)
            {
                return;
            }

            var endDate = this.Model.SelectedDates[this.Model.SelectedDates.Count - 1];
            var selectedDates = new ObservableCollection<DateTime>();
            switch (this.Model.CurrentScheduleType)
            {
                case ScheduleType.Day:
                    {
                        MonthSelection = false;
                        endDate = endDate.AddDays(1);
                        selectedDates.Add(endDate);
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.Week:
                    {
                        MonthSelection = false;

                        for (int i = 1; i <= Model.SelectedDates.Count; i++)
                        {
                            selectedDates.Add(endDate.AddDays(i));
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.WorkWeek:
                    {
                        MonthSelection = false;

                        for (int i = 0; i < this.Model.WorkingDays.Count; i++)
                        {
                            selectedDates.Add(endDate.AddDays(7).StartOfWeek(this.Model.WorkingDays[i]));
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.Month:
                    {
                        int selectedDayscount = this.Model.SelectedDates.Count;
                        if (MonthSelection == false)
                        {
                            MonthViewNavigatorDate = this.Model.SelectedDates[0];
                            MonthSelection = true;
                        }
                        MonthViewNavigatorDate = MonthViewNavigatorDate.AddMonths(1);
                        var firstDate = MonthViewNavigatorDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(selectedDayscount - 7);
                        for (int i = 0; i < selectedDayscount; i++)
                        {
                            selectedDates.Add(firstDate);
                            firstDate = firstDate.AddDays(1);
                        }
                        this.Model.SelectedDates = selectedDates;


                    }
                    break;
            }

            // we clear the selection states here
            this.Model.SelectedStartTimeSpan = DateTime.MinValue;
            this.Model.SelectedEndTimeSpan = DateTime.MinValue;
            this.UpdateCurrentDateText();
        }

        internal void prevButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.Model == null)
            {
                return;
            }

            var endDate = this.Model.SelectedDates[0];
            var selectedDates = new ObservableCollection<DateTime>();
            switch (this.Model.CurrentScheduleType)
            {
                case ScheduleType.Day:
                    {
                        selectedDates.Add(endDate.SubractDays(1));
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.Week:
                    {

                        for (int i = 1; i <= this.Model.SelectedDates.Count; i++)
                        {

                            selectedDates.Add(endDate.SubractDays(i));
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.WorkWeek:
                    {
                        var date = this.Model.SelectedDates[this.Model.SelectedDates.Count - 1];
                        for (int i = 0; i < this.Model.WorkingDays.Count; i++)
                        {
                            selectedDates.Add(date.SubractDays(7).StartOfWeek(this.Model.WorkingDays[i]));
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.Month:
                    {
                        int selectedDayscount = this.Model.SelectedDates.Count;
                        if (MonthSelection == false)
                        {
                            MonthViewNavigatorDate = this.Model.SelectedDates[0];
                            MonthSelection = true;
                        }
                        MonthViewNavigatorDate = MonthViewNavigatorDate.AddMonths(-1);
                        var firstDate = MonthViewNavigatorDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                        for (int i = 0; i < selectedDayscount; i++)
                        {
                            selectedDates.Add(firstDate);
                            firstDate = firstDate.AddDays(1);
                        }
                        this.Model.SelectedDates = selectedDates;

                    }

                    break;
            }

            this.UpdateCurrentDateText();
        }
#else

        /// <summary>
        /// Handles the Click event of the nextButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void nextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.Model == null)
            {
                return;
            }

            
            var selectedDates = new ObservableCollection<DateTime>();
            switch (this.Model.CurrentScheduleType)
            {
                case ScheduleType.Day:
                    {
                        MonthSelection = false;
                        selectedDates.Add(this.Model.SelectedDates[0].AddDays(1));
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.ScheduleView:
                case ScheduleType.Week:
                    {
                        MonthSelection = false; var endDate = this.Model.SelectedDates[this.Model.SelectedDates.Count - 1];
                        for (int i = 1; i <= Model.SelectedDates.Count; i++)
                        {
                            selectedDates.Add(endDate.AddDays(i));
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.WorkWeek:
                    {
                        MonthSelection = false; 
                        var endofweekdate = this.Model.SelectedDates[0].AddDays(7).StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(6);
                        var newDates = new ObservableCollection<DateTime>();
                        for (int i = 0; i < this.Model.WorkingDays.Count; i++)
                        {
                            var date = endofweekdate.StartOfWeek(this.Model.WorkingDays[i]);
                            selectedDates.Add(date);
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.Month:
                    {
                        int selectedDayscount = this.Model.SelectedDates.Count;
                        if (selectedDayscount == 35)
                        {
                            var firstDate = this.Model.SelectedDates[7];
                            firstDate = firstDate.AddDays(30);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < selectedDayscount; i++)
                            {

                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);

                            }
                        }
                        else
                        {
                            if (MonthSelection == false)
                            {
                                MonthViewNavigatorDate = this.Model.SelectedDates[(this.Model.SelectedDates.Count - 1)];
                                MonthSelection = true;
                            }
                            MonthViewNavigatorDate = MonthViewNavigatorDate.AddDays(selectedDayscount);
                            var firstDate = MonthViewNavigatorDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(selectedDayscount - 7);                                                      
                            for (int i = 0; i < selectedDayscount; i++)
                            {
                                selectedDates.Add(firstDate);
                                firstDate = firstDate.AddDays(1);

                            }
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
            }

            // we clear the selection states here
            this.Model.SelectedStartTimeSpan = DateTime.MinValue;
            this.Model.SelectedEndTimeSpan = DateTime.MinValue;
            this.SelectedDates = Model.SelectedDates;           
        }

        /// <summary>
        /// Handles the Click event of the prevButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void prevButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.Model == null)
            {
                return;
            }

            var endDate = this.Model.SelectedDates[0];
            var selectedDates = new ObservableCollection<DateTime>();
            switch (this.Model.CurrentScheduleType)
            {
                case ScheduleType.Day:
                    {
                        selectedDates.Add(endDate.SubractDays(1));
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.ScheduleView:
                case ScheduleType.Week:
                    {
                        for (int i = 1;i <= this.Model.SelectedDates.Count;i++)
                        {

                            selectedDates.Add(endDate.SubractDays(i));
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.WorkWeek:
                    {
                        var date = this.Model.SelectedDates[this.Model.SelectedDates.Count - 1];
                        for (int i = 0;i < this.Model.WorkingDays.Count;i++)
                        {
                            selectedDates.Add(date.SubractDays(7).StartOfWeek(this.Model.WorkingDays[i]));
                        }
                        this.Model.SelectedDates = selectedDates;
                    }
                    break;
                case ScheduleType.Month:
                    {
                        int selectedDayscount = this.Model.SelectedDates.Count;
                        if (selectedDayscount == 35)
                        {
                            var firstDate = this.Model.SelectedDates[7];
                            firstDate = firstDate.AddDays(-30);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < selectedDayscount; i++)
                            {
                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                        }
                        else
                        {
                            if (MonthSelection == false)
                            {
                                MonthViewNavigatorDate = this.Model.SelectedDates[0];
                                MonthSelection = true;
                            }
                            MonthViewNavigatorDate = MonthViewNavigatorDate.AddMonths(-1);
                            var firstDate = MonthViewNavigatorDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                            //while ((((firstDate.AddDays(selectedDayscount).Month) - (firstDate.Month)) != 1) && (((firstDate.AddDays(selectedDayscount).Year) - (firstDate.Year)) == 0))
                            //{
                            //    firstDate = firstDate.AddDays(-1);
                            //}
                            for (int i = 0; i < selectedDayscount; i++)
                            {
                                selectedDates.Add(firstDate);
                                firstDate = firstDate.AddDays(1);
                            }
                        }
                        this.Model.SelectedDates = selectedDates;

                    }
                    break;
            }
            this.SelectedDates = Model.SelectedDates;
            
        }
#endif

    }


   

}


