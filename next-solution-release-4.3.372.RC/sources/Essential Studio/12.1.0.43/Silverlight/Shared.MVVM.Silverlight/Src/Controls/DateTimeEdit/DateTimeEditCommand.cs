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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{

    #region DateTimeEditCalendarPopupOpenedCommand
    /// <summary>
    /// DateTimeEditCalendarPopupOpenedCommand
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

    /// <summary>
    /// DateTimeEditCalendarPopupOpenedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditCalendarPopupOpenedCommandBehavior<T> : DateTimeEditCalendarPopupOpenedCommandBehavior
    { }
    #endregion

    #region DateTimeEditDateTimeChangedCommand
    /// <summary>
    /// DateTimeEditDateTimeChangedCommand
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

    /// <summary>
    /// DateTimeEditDateTimeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditDateTimeChangedCommandBehavior<T> : DateTimeEditDateTimeChangedCommandBehavior
    { }
    #endregion

    #region DateTimeEditMaxDateTimeChangedCommand
    /// <summary>
    /// DateTimeEditMaxDateTimeChangedCommand
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

    /// <summary>
    /// DateTimeEditMaxDateTimeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditMaxDateTimeChangedCommandBehavior<T> : DateTimeEditMaxDateTimeChangedCommandBehavior
    { }
    #endregion

    #region DateTimeEditMinDateTimeChangedCommand
    /// <summary>
    /// DateTimeEditMinDateTimeChangedCommand
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

    /// <summary>
    /// DateTimeEditMinDateTimeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeEditMinDateTimeChangedCommandBehavior<T> : DateTimeEditMinDateTimeChangedCommandBehavior
    { }
    #endregion
   
}


