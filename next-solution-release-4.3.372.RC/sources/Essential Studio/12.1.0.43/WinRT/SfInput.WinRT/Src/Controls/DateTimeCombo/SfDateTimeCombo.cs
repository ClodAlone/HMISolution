// <copyright file="SfDateTimeCombo.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a control that allows the user to select and edit a date by using a drop-down
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeItem"/> control for each DateTime part.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfDateTimeCombo : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeItem"/>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Input">Syncfusion.UI.Xaml.Controls.Input
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfDateTimeCombo()
        {
            DefaultStyleKey = typeof(SfDateTimeCombo);
            Loaded += SfDateTimeCombo_Loaded;
        }

        void SfDateTimeCombo_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormatString != null)
            {
                UpdateDateTime(FormatString);
            }
            Validate((DateTime)Formatdate(Value));
        }

        #endregion

        #region Members

        internal DateTimeItemsControl m_itemscontrol;

        internal char[] m_StandardFormats = new[] { 
            'd', 'D', 'h','H', 'm', 'M', 's', 't', 'T',  'y', 'Y'
         };

        #endregion

        #region Dependency Properties
        
        /// <summary>
        /// Gets or sets the value of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/> that hold the currently
        /// selected date.
        /// </summary>
        /// <value>
        /// The default value is <see cref="P:System.DateTime.Now">DateTime.Now</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeItem"/>
        [ClassReference(IsReviewed = false)]
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDateTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SfDateTimeCombo), new PropertyMetadata(DateTime.Now,OnValueChanged));

        /// <summary>
        /// Gets or sets the data that is used as a format for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeItem"/>
        [ClassReference(IsReviewed = false)]
        public string FormatString
        {
            get { return (string)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FormatString.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FormatStringProperty =
            DependencyProperty.Register("FormatString", typeof(string), typeof(SfDateTimeCombo), new PropertyMetadata("d", OnFormatStringChanged));
        
        /// <summary>
        /// Gets or sets the data that is used as a style for the DayCombo in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public Style DayComboStyle
        {
            get { return (Style)GetValue(DayComboStyleProperty); }
            set { SetValue(DayComboStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayComboStyle  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayComboStyleProperty =
            DependencyProperty.Register("DayComboStyle", typeof(Style), typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data that is used as a style for the MonthCombo in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public Style MonthComboStyle  
        {
            get { return (Style)GetValue(MonthComboStyleProperty); }
            set { SetValue(MonthComboStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthComboStyle  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthComboStyleProperty =
            DependencyProperty.Register("MonthComboStyle", typeof(Style), typeof(SfDateTimeCombo), new PropertyMetadata(null));

         /// <summary>
        /// Gets or sets the data that is used as a style for the DisplayMinDate in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public DateTime DisplayMinDate
        {
            get { return (DateTime)GetValue(DisplayMinDateProperty); }
            set { SetValue(DisplayMinDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayMinDate  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayMinDateProperty =
            DependencyProperty.Register("DisplayMinDate", typeof(DateTime), typeof(SfDateTimeCombo), new PropertyMetadata(DateTime.MinValue,OnDisplayMinDateChanged));
        
        /// <summary>
        /// Gets or sets the data that is used as a style for the DisplayMaxDate in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public DateTime DisplayMaxDate
        {
            get { return (DateTime)GetValue(DisplayMaxDateProperty); }
            set { SetValue(DisplayMaxDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayMaxDate  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayMaxDateProperty =
            DependencyProperty.Register("DisplayMaxDate", typeof(DateTime), typeof(SfDateTimeCombo), new PropertyMetadata(DateTime.MaxValue,OnDisplayMaxDateChanged));

        /// <summary>
        /// Gets or sets the data that is used as a style for the YearCombo in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public Style YearComboStyle  
        {
            get { return (Style)GetValue(YearComboStyleProperty); }
            set { SetValue(YearComboStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearComboStyle  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearComboStyleProperty =
            DependencyProperty.Register("YearComboStyle", typeof(Style), typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data that is used as a style for the TwelveHourCombo in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public Style TwelveHourComboStyle  
        {
            get { return (Style)GetValue(TwelveHourComboStyleProperty); }
            set { SetValue(TwelveHourComboStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TwelveHourComboStyle  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TwelveHourComboStyleProperty =
            DependencyProperty.Register("TwelveHourComboStyle", typeof(Style), typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data that is used as a style for the TwentyFourHourCombo in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public Style TwentyFourHourComboStyle  
        {
            get { return (Style)GetValue(TwentyFourHourComboStyleProperty); }
            set { SetValue(TwentyFourHourComboStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TwentyFourHourComboStyle  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TwentyFourHourComboStyleProperty =
            DependencyProperty.Register("TwentyFourHourComboStyle", typeof(Style), typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data that is used as a style for the MinuteCombo in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public Style MinuteComboStyle  
        {
            get { return (Style)GetValue(MinuteComboStyleProperty); }
            set { SetValue(MinuteComboStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteComboStyle  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinuteComboStyleProperty =
            DependencyProperty.Register("MinuteComboStyle", typeof(Style), typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data that is used as a style for the SecondCombo in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public Style SecondComboStyle  
        {
            get { return (Style)GetValue(SecondComboStyleProperty); }
            set { SetValue(SecondComboStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SecondComboStyle  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondComboStyleProperty =
            DependencyProperty.Register("SecondComboStyle", typeof(Style), typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data that is used as a style for the AMPMDesignatorCombo in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        /// </summary>
        public Style AMPMDesignatorComboStyle  
        {
            get { return (Style)GetValue(AMPMDesignatorComboStyleProperty); }
            set { SetValue(AMPMDesignatorComboStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AMPMDesignatorComboStyle  .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AMPMDesignatorComboStyleProperty =
            DependencyProperty.Register("AMPMDesignatorComboStyle", typeof(Style), typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Day ItemTemplate
        /// </summary>
        public DataTemplate DayItemTemplate
        {
            get { return (DataTemplate)GetValue(DayItemTemplateProperty); }
            set { SetValue(DayItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayItemTemplateProperty =
            DependencyProperty.Register("DayItemTemplate", typeof(DataTemplate), typeof(SfDateTimeCombo),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the TemplateSelector for Day ItemTemplate
        /// </summary>
        public DataTemplateSelector DayItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DayItemTemplateSelectorProperty); }
            set { SetValue(DayItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayItemTemplateSelectorProperty =
            DependencyProperty.Register("DayItemTemplateSelector", typeof(DataTemplateSelector),
                                        typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Month ItemTemplate
        /// </summary>
        public DataTemplate MonthItemTemplate
        {
            get { return (DataTemplate)GetValue(MonthItemTemplateProperty); }
            set { SetValue(MonthItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthItemTemplateProperty =
            DependencyProperty.Register("MonthItemTemplate", typeof(DataTemplate), typeof(SfDateTimeCombo),
                                        new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the TemplateSelector for Month ItemTemplate
        /// </summary>
        public DataTemplateSelector MonthItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(MonthItemTemplateSelectorProperty); }
            set { SetValue(MonthItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthItemTemplateSelectorProperty =
            DependencyProperty.Register("MonthItemTemplateSelector", typeof(DataTemplateSelector),
                                        typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Year ItemTemplate
        /// </summary>
        public DataTemplate YearItemTemplate
        {
            get { return (DataTemplate)GetValue(YearItemTemplateProperty); }
            set { SetValue(YearItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearItemTemplateProperty =
            DependencyProperty.Register("YearItemTemplate", typeof(DataTemplate), typeof(SfDateTimeCombo),
                                        new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the TemplateSelector for Year ItemTemplate
        /// </summary>
        public DataTemplateSelector YearItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(YearItemTemplateSelectorProperty); }
            set { SetValue(YearItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearItemTemplateSelectorProperty =
            DependencyProperty.Register("YearItemTemplateSelector", typeof(DataTemplateSelector),
                                        typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the TwelveHour ItemTemplate
        /// </summary>
        public DataTemplate TwelveHourItemTemplate
        {
            get { return (DataTemplate)GetValue(TwelveHourItemTemplateProperty); }
            set { SetValue(TwelveHourItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TwelveHourItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TwelveHourItemTemplateProperty =
            DependencyProperty.Register("TwelveHourItemTemplate", typeof(DataTemplate), typeof(SfDateTimeCombo),
                                        new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the TemplateSelector for TwelveHour ItemTemplate
        /// </summary>
        public DataTemplateSelector TwelveHourItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TwelveHourItemTemplateSelectorProperty); }
            set { SetValue(TwelveHourItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TwelveHourItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TwelveHourItemTemplateSelectorProperty =
            DependencyProperty.Register("TwelveHourItemTemplateSelector", typeof(DataTemplateSelector),
                                        typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the TwentyFourHour ItemTemplate
        /// </summary>
        public DataTemplate TwentyFourHourItemTemplate
        {
            get { return (DataTemplate)GetValue(TwentyFourHourItemTemplateProperty); }
            set { SetValue(TwentyFourHourItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TwentyFourHourItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TwentyFourHourItemTemplateProperty =
            DependencyProperty.Register("TwentyFourHourItemTemplate", typeof(DataTemplate), typeof(SfDateTimeCombo),
                                        new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the TemplateSelector for TwentyFourHour ItemTemplate
        /// </summary>
        public DataTemplateSelector TwentyFourHourItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TwentyFourHourItemTemplateSelectorProperty); }
            set { SetValue(TwentyFourHourItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TwentyFourHourItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TwentyFourHourItemTemplateSelectorProperty =
            DependencyProperty.Register("TwentyFourHourItemTemplateSelector", typeof(DataTemplateSelector),
                                        typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the MinuteItem Template
        /// </summary>
        public DataTemplate MinuteItemTemplate
        {
            get { return (DataTemplate)GetValue(MinuteItemTemplateProperty); }
            set { SetValue(MinuteItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinuteItemTemplateProperty =
            DependencyProperty.Register("MinuteItemTemplate", typeof(DataTemplate), typeof(SfDateTimeCombo),
                                        new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the TemplateSelector for Minute ItemTemplate
        /// </summary>
        public DataTemplateSelector MinuteItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(MinuteItemTemplateSelectorProperty); }
            set { SetValue(MinuteItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinuteItemTemplateSelectorProperty =
            DependencyProperty.Register("MinuteItemTemplateSelector", typeof(DataTemplateSelector),
                                        typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Second ItemTemplate
        /// </summary>
        public DataTemplate SecondItemTemplate
        {
            get { return (DataTemplate)GetValue(SecondItemTemplateProperty); }
            set { SetValue(SecondItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SecondItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondItemTemplateProperty =
            DependencyProperty.Register("SecondItemTemplate", typeof(DataTemplate), typeof(SfDateTimeCombo),
                                        new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the TemplateSelector for Second ItemTemplate
        /// </summary>
        public DataTemplateSelector SecondItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SecondItemTemplateSelectorProperty); }
            set { SetValue(SecondItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SecondItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondItemTemplateSelectorProperty =
            DependencyProperty.Register("SecondItemTemplateSelector", typeof(DataTemplateSelector),
                                        typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the AMPMDesignator ItemTemplate
        /// </summary>
        public DataTemplate AMPMDesignatorItemTemplate
        {
            get { return (DataTemplate)GetValue(AMPMDesignatorItemTemplateProperty); }
            set { SetValue(AMPMDesignatorItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AMPMDesignatorItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AMPMDesignatorItemTemplateProperty =
            DependencyProperty.Register("AMPMDesignatorItemTemplate", typeof(DataTemplate), typeof(SfDateTimeCombo),
                                        new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the TemplateSelector for AMPMDesignator ItemTemplate
        /// </summary>
        public DataTemplateSelector AMPMDesignatorItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(AMPMDesignatorItemTemplateSelectorProperty); }
            set { SetValue(AMPMDesignatorItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for AMPMDesignatorItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AMPMDesignatorItemTemplateSelectorProperty =
            DependencyProperty.Register("AMPMDesignatorItemTemplateSelector", typeof(DataTemplateSelector),
                                        typeof(SfDateTimeCombo), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets Minute interval
        /// </summary>
        public int MinuteInterval
        {
            get { return (int)GetValue(MinuteIntervalProperty); }
            set { SetValue(MinuteIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinuteIntervalProperty =
            DependencyProperty.Register("MinuteInterval", typeof(int), typeof(SfDateTimeCombo), new PropertyMetadata(1,new PropertyChangedCallback(OnMinuteIntervalChanged)));

        /// <summary>
        /// Gets or sets seconds interval
        /// </summary>
        public int SecondsInterval
        {
            get { return (int)GetValue(SecondsIntervalProperty); }
            set { SetValue(SecondsIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SecondsInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondsIntervalProperty =
            DependencyProperty.Register("SecondsInterval", typeof(int), typeof(SfDateTimeCombo), new PropertyMetadata(1,new PropertyChangedCallback(OnSecondsIntervalChanged)));
        
        #endregion

        #region override Methods
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_itemscontrol = GetTemplateChild("Part_Items") as DateTimeItemsControl;
            if (m_itemscontrol != null)
                m_itemscontrol.dateTimeCombo = this;
        }
        #endregion

        #region CallBack Methods

        internal void Validate(DateTime datevalue)
        {
            if (datevalue < DisplayMinDate)
                datevalue = DisplayMinDate;
            else if (datevalue > DisplayMaxDate)
                datevalue = DisplayMaxDate;
            int minuterange = (datevalue.Minute % MinuteInterval) == 0 ? datevalue.Minute / MinuteInterval : (int)datevalue.Minute / MinuteInterval + 1;
            int secondrange = (datevalue.Second % SecondsInterval) == 0 ? datevalue.Second / SecondsInterval : (int)datevalue.Second / SecondsInterval + 1;
            if (!(minuterange * MinuteInterval).Equals(datevalue.Minute) && minuterange * MinuteInterval < 60)
                datevalue = new DateTime(datevalue.Year, datevalue.Month, datevalue.Day, datevalue.Hour,
                                         minuterange * MinuteInterval, datevalue.Second);
            if (!(secondrange * SecondsInterval).Equals(datevalue.Second) && secondrange * SecondsInterval < 60)
                datevalue = new DateTime(datevalue.Year, datevalue.Month, datevalue.Day, datevalue.Hour,
                                         datevalue.Minute, secondrange * SecondsInterval);
            if (!Formatdate(Value).Equals(datevalue))
                Value = datevalue;
        }

        private static void OnDisplayMaxDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeCombo dateTimeCombo = d as SfDateTimeCombo;
            dateTimeCombo.Validate((DateTime) dateTimeCombo.Formatdate(dateTimeCombo.Value));
        }

        private static void OnDisplayMinDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeCombo dateTimeCombo = d as SfDateTimeCombo;
            dateTimeCombo.Validate((DateTime)dateTimeCombo.Formatdate(dateTimeCombo.Value));
        }

        private static void OnMinuteIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeCombo dateTimeCombo = d as SfDateTimeCombo;
            if (e.NewValue.Equals(0))
                dateTimeCombo.MinuteInterval = 1;
            else if ((int)e.NewValue > 59)
                dateTimeCombo.MinuteInterval = 59;
            dateTimeCombo.Validate((DateTime)dateTimeCombo.Formatdate(dateTimeCombo.Value));
        }

        private static void OnSecondsIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeCombo dateTimeCombo = d as SfDateTimeCombo;
            if (e.NewValue.Equals(0))
                dateTimeCombo.SecondsInterval = 1;
            else if ((int) e.NewValue > 59)
                dateTimeCombo.SecondsInterval = 59;
            dateTimeCombo.Validate((DateTime)dateTimeCombo.Formatdate(dateTimeCombo.Value));
        }

        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/> control value has changed. 
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            Validate((DateTime) Formatdate(args.NewValue));

            bool canexecute = args.NewValue != null ? args.NewValue.Equals(Value) : Value == null ? true : false;

            if (canexecute && ValueChanged != null)
            {
                var valueArgs = new ValueChangedEventArgs {NewValue = args.NewValue, OldValue = args.OldValue};
                ValueChanged(this, valueArgs);
            }
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = (SfDateTimeCombo)obj;
            if (control != null)
            {
                control.OnValueChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/> control FormatString has changed. 
        /// </summary>
        /// <param name="args"></param>
        protected void OnFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                UpdateDateTime(args.NewValue.ToString());
            }
        }

        private static void OnFormatStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = (SfDateTimeCombo)obj;
            if (control != null)
            {
                control.OnFormatStringChanged(args);
            }
        }

        #endregion

        #region Public Methods
        
        internal DateTimeItem SetDateTimePart(object item, DateTimeItem datetimeitem)
        {
            string part = item.ToString();
            var binding = new Binding();
            PropertyPath pathItemStyle;
            PropertyPath pathItemTemplate;
            PropertyPath pathItemTemplateSelector;
            DataTemplateSelector itemTemplateselector;
            binding.Source = this;
            if (part.Equals("M"))
            {
                datetimeitem.DateTimePart = DateTimePart.Month;
                pathItemTemplate = new PropertyPath("MonthItemTemplate");
                pathItemTemplateSelector = new PropertyPath("MonthItemTemplateSelector");
                itemTemplateselector = MonthItemTemplateSelector;
                pathItemStyle = new PropertyPath("MonthComboStyle");
            }
            else if (part.ToLower().Equals("y"))
            {
                datetimeitem.DateTimePart = DateTimePart.Year;
                pathItemTemplate = new PropertyPath("YearItemTemplate");
                pathItemTemplateSelector = new PropertyPath("YearItemTemplateSelector");
                itemTemplateselector = YearItemTemplateSelector;
                pathItemStyle = new PropertyPath("YearComboStyle");
            }
            else if (part.ToLower().Equals("d"))
            {
                datetimeitem.DateTimePart = DateTimePart.Day;
                pathItemTemplate = new PropertyPath("DayItemTemplate");
                pathItemTemplateSelector = new PropertyPath("DayItemTemplateSelector");
                itemTemplateselector = DayItemTemplateSelector;
                pathItemStyle = new PropertyPath("DayComboStyle");
            }
            else if (part.ToLower().Equals("h"))
            {
                if (!DateTimeWrapper.CurrentCultureUsesTwentyFourHourClock())
                {
                    datetimeitem.DateTimePart = DateTimePart.TwelveHour;
                    pathItemTemplate = new PropertyPath("TwelveHourItemTemplate");
                    pathItemTemplateSelector = new PropertyPath("TwelveHourItemTemplateSelector");
                    itemTemplateselector = TwelveHourItemTemplateSelector;
                    pathItemStyle = new PropertyPath("TwelveHourComboStyle");
                }
                else
                {
                    datetimeitem.DateTimePart = DateTimePart.TwentyFourHour;
                    pathItemTemplate = new PropertyPath("TwentyFourHourItemTemplate");
                    pathItemTemplateSelector = new PropertyPath("TwentyFourHourItemTemplateSelector");
                    itemTemplateselector = TwentyFourHourItemTemplateSelector;
                    pathItemStyle = new PropertyPath("TwentyFourHourComboStyle");
                }
            }
            else if (part.Equals("m"))
            {
                datetimeitem.DateTimePart = DateTimePart.Minute;
                pathItemTemplate = new PropertyPath("MinuteItemTemplate");
                pathItemTemplateSelector = new PropertyPath("MinuteItemTemplateSelector");
                itemTemplateselector = MinuteItemTemplateSelector;
                pathItemStyle = new PropertyPath("MinuteComboStyle");
            }
            else if (part.ToLower().Equals("s"))
            {
                datetimeitem.DateTimePart = DateTimePart.Second;
                pathItemTemplate = new PropertyPath("SecondItemTemplate");
                pathItemTemplateSelector = new PropertyPath("SecondItemTemplateSelector");
                itemTemplateselector = SecondItemTemplateSelector;
                pathItemStyle = new PropertyPath("SecondComboStyle");
            }
            else
            {
                datetimeitem.DateTimePart = DateTimePart.AMPMDesignator;
                pathItemTemplate = new PropertyPath("AMPMDesignatorItemTemplate");
                pathItemTemplateSelector = new PropertyPath("AMPMDesignatorItemTemplateSelector");
                itemTemplateselector = AMPMDesignatorItemTemplateSelector;
                pathItemStyle = new PropertyPath("AMPMDesignatorComboStyle");
            }
            binding.Mode = BindingMode.TwoWay;
            binding.Path = pathItemTemplate;
            datetimeitem.SetBinding(ItemsControl.ItemTemplateProperty, binding);
            if (itemTemplateselector != null)
            {
                datetimeitem.ItemTemplate = itemTemplateselector.SelectTemplate(item, datetimeitem);
                binding = new Binding { Source = this, Mode = BindingMode.TwoWay, Path = pathItemTemplateSelector };
                datetimeitem.SetBinding(ItemsControl.ItemTemplateSelectorProperty, binding);
            }
            binding = new Binding {Source = this, Mode = BindingMode.TwoWay, Path = pathItemStyle};
            datetimeitem.SetBinding(DateTimeItem.StyleProperty, binding);
            return datetimeitem;
        }

        internal List<DateTimeWrapper> GetDateTimePartItems(DateTimePart datepart, object date,out DataSource source)
        {
            var items = new List<DateTimeWrapper>();
            var datetime = (DateTime)date;
            source = null;
            switch (datepart)
            {
                case DateTimePart.Day:
                    source = new DayDataSource();
                    break;
                case DateTimePart.Month:
                    source = new MonthDataSource();
                    break;
                case DateTimePart.Year:
                    source = new YearDataSource();
                    break;
                case DateTimePart.TwelveHour:
                    source = new TwelveHourDataSource();
                    break;
                case DateTimePart.TwentyFourHour:
                    source = new TwentyFourHourDataSource();
                    break;
                case DateTimePart.Minute:
                    source = new MinuteDataSource(MinuteInterval);
                    break;
                case DateTimePart.Second:
                    source = new SecondDataSource(SecondsInterval);
                    break;
                case DateTimePart.AMPMDesignator:
                    source = new AmPmDataSource();
                    break;
            }
            if (source != null)
                items = source.GetDatePartCollection(datetime, MinuteInterval, SecondsInterval);
            return items;
        }

        internal List<string> SplitDateParts(string formatstring)
        {
            var datepart = string.Empty;
            var collection = new List<string>();
            for (int i = 0; i < formatstring.Length; i++)
            {
                if (!string.IsNullOrEmpty(datepart) && !datepart.Contains(formatstring[i].ToString()))
                {
                    collection.Add(datepart);
                    datepart = string.Empty;
                }
                if(formatstring[i].ToString().IndexOfAny(m_StandardFormats) >= 0)
                    datepart = formatstring[i].ToString();
                if (!string.IsNullOrEmpty(datepart) && i == formatstring.Length - 1)
                    collection.Add(datepart);
                if ((formatstring[i] == 't' || formatstring[i] == 'T') && collection.Contains(datepart) && !DateTime.Now.Date.ToString().Contains("AM") && !DateTime.Now.Date.ToString().Contains("PM"))
                    collection.Remove(datepart);
            }
            return collection;
        }

        internal object Formatdate(object date)
        {
            DateTime datetime = DateTime.Now;
            if (date == null || string.IsNullOrEmpty(date.ToString()))
                date = datetime;
            else
            {
                if (DateTime.TryParse(date.ToString(), out datetime))
                    date = datetime;
                else
                    throw new InvalidCastException("Input is not in DateTime format");
            }
            return date;
        }

        internal void UpdateDateTime(string formatstring)
        {
            if (m_itemscontrol != null)
            {
                Grid grid = m_itemscontrol.GetItemsPanelGrid();
                if (grid != null)
                    grid.ColumnDefinitions.Clear();
                var dateparts = SplitDateParts(formatstring);
                if (dateparts.Count == 0)
                    dateparts.Add("d");
                m_itemscontrol.ItemsSource = dateparts;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when current <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo.Value"/> changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event ValueChangedEventHandler ValueChanged;

        #endregion

    }

    /// <summary>
    /// Represents a class for selecting the disable item template
    /// </summary>
    public class DisableItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Returns the Type of template
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is DateTimeComboItem)
            {
                DateTimeComboItem comboItem = container as DateTimeComboItem;
                DateTimeWrapper wrapper = item as DateTimeWrapper;
                if (wrapper != null && comboItem.ParentDateTimeItem != null && comboItem.ParentDateTimeItem.DateTimeCombo != null)
                {
                    DateTime minDate = comboItem.ParentDateTimeItem.DateTimeCombo.DisplayMinDate;
                    DateTime maxDate = comboItem.ParentDateTimeItem.DateTimeCombo.DisplayMaxDate;
                    comboItem.IsEnabled = true;
                    switch (comboItem.ParentDateTimeItem.DateTimePart)
                    {
                        case DateTimePart.Day:
                        case DateTimePart.Month:
                        case DateTimePart.Minute:
                        case DateTimePart.AMPMDesignator:
                        case DateTimePart.TwelveHour:
                        case DateTimePart.TwentyFourHour:
                        case DateTimePart.Second:
                            if (wrapper.DateTime < minDate || wrapper.DateTime > maxDate)
                                comboItem.IsEnabled = false;
                            break;
                        case DateTimePart.Year:
                            if (wrapper.DateTime.Year < minDate.Year || wrapper.DateTime.Year > maxDate.Year)
                                comboItem.IsEnabled = false;
                            break;
                    }
                }
                return (container as DateTimeComboItem).ContentTemplate;
            }
            else if (container is DateTimeItem)
            {
                return (container as DateTimeItem).ItemTemplate;
            }
            return null;
        }
    }
}
