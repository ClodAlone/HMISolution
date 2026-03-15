#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{   

	#region DateTimeEditCalendarPopupOpenedCommand
	// DateTimeEditCalendarPopupOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeEditCalendarPopupOpenedCommand : ControlCommandBase<DateTimeEditCalendarPopupOpenedCommandBehavior, DateTimeEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEditCalendarPopupOpenedCommandBehavior : CommandBehaviorBase<DateTimeEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.CalendarPopupOpened += OnEventRaised;
        }
    }

	// DateTimeEditCalendarPopupOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditCalendarPopupOpenedCommandBehavior<T> : DateTimeEditCalendarPopupOpenedCommandBehavior
    { }
	#endregion

	#region DateTimeEditClockPopupOpenedEventCommand
	// DateTimeEditClockPopupOpenedEventCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeEditClockPopupOpenedEventCommand : ControlCommandBase<DateTimeEditClockPopupOpenedEventCommandBehavior, DateTimeEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEditClockPopupOpenedEventCommandBehavior : CommandBehaviorBase<DateTimeEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ClockPopupOpenedEvent += OnEventRaised;
        }
    }

	// DateTimeEditClockPopupOpenedEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditClockPopupOpenedEventCommandBehavior<T> : DateTimeEditClockPopupOpenedEventCommandBehavior
    { }
	#endregion

	
	#region DateTimeEditDateTimeChangedCommand
	// DateTimeEditDateTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeEditDateTimeChangedCommand : ControlCommandBase<DateTimeEditDateTimeChangedCommandBehavior, DateTimeEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEditDateTimeChangedCommandBehavior : CommandBehaviorBase<DateTimeEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DateTimeChanged += OnEventRaised;
        }
    }

	// DateTimeEditDateTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditDateTimeChangedCommandBehavior<T> : DateTimeEditDateTimeChangedCommandBehavior
    { }
	#endregion

	#region DateTimeEditMaxDateTimeChangedCommand
	// DateTimeEditMaxDateTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeEditMaxDateTimeChangedCommand : ControlCommandBase<DateTimeEditMaxDateTimeChangedCommandBehavior, DateTimeEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEditMaxDateTimeChangedCommandBehavior : CommandBehaviorBase<DateTimeEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.MaxDateTimeChanged += OnEventRaised;
        }
    }

	// DateTimeEditMaxDateTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditMaxDateTimeChangedCommandBehavior<T> : DateTimeEditMaxDateTimeChangedCommandBehavior
    { }
	#endregion

	#region DateTimeEditMinDateTimeChangedCommand
	// DateTimeEditMinDateTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeEditMinDateTimeChangedCommand : ControlCommandBase<DateTimeEditMinDateTimeChangedCommandBehavior, DateTimeEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEditMinDateTimeChangedCommandBehavior : CommandBehaviorBase<DateTimeEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.MinDateTimeChanged += OnEventRaised;
        }
    }

	// DateTimeEditMinDateTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditMinDateTimeChangedCommandBehavior<T> : DateTimeEditMinDateTimeChangedCommandBehavior
    { }
	#endregion

	#region DateTimeEditDisableDateSelectionChangedCommand
	// DateTimeEditDisableDateSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeEditDisableDateSelectionChangedCommand : ControlCommandBase<DateTimeEditDisableDateSelectionChangedCommandBehavior, DateTimeEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEditDisableDateSelectionChangedCommandBehavior : CommandBehaviorBase<DateTimeEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DisableDateSelectionChanged += OnEventRaised;
        }
    }

	// DateTimeEditDisableDateSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditDisableDateSelectionChangedCommandBehavior<T> : DateTimeEditDisableDateSelectionChangedCommandBehavior
    { }
	#endregion

	#region DateTimeEditDropDownViewChangedCommand
	// DateTimeEditDropDownViewChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeEditDropDownViewChangedCommand : ControlCommandBase<DateTimeEditDropDownViewChangedCommandBehavior, DateTimeEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEditDropDownViewChangedCommandBehavior : CommandBehaviorBase<DateTimeEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DropDownViewChanged += OnEventRaised;
        }
    }

	// DateTimeEditDropDownViewChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditDropDownViewChangedCommandBehavior<T> : DateTimeEditDropDownViewChangedCommandBehavior
    { }
	#endregion

	#region DateTimeEditMonthChangedCommand
	// DateTimeEditMonthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeEditMonthChangedCommand : ControlCommandBase<DateTimeEditMonthChangedCommandBehavior, DateTimeEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEditMonthChangedCommandBehavior : CommandBehaviorBase<DateTimeEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.MonthChanged += OnEventRaised;
        }
    }

	// DateTimeEditMonthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditMonthChangedCommandBehavior<T> : DateTimeEditMonthChangedCommandBehavior
    { }
	#endregion
}


