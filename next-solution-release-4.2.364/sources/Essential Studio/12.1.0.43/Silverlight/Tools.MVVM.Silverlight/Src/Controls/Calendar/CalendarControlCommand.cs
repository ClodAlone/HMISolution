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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools.MVVM
{

    #region CalendarControlAllowMultipleSelectionChangedCommand
    // CalendarControlAllowMultipleSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlAllowMultipleSelectionChangedCommand : ControlCommandBase<CalendarControlAllowMultipleSelectionChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlAllowMultipleSelectionChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.AllowMultipleSelectionChanged += OnEventRaised;
        }
    }

    // CalendarControlAllowMultipleSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlAllowMultipleSelectionChangedCommandBehavior<T> : CalendarControlAllowMultipleSelectionChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlBackgroundPropertyChangedCommand
    // CalendarControlBackgroundPropertyChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlBackgroundPropertyChangedCommand : ControlCommandBase<CalendarControlBackgroundPropertyChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlBackgroundPropertyChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.BackgroundPropertyChanged += OnEventRaised;
        }
    }

    // CalendarControlBackgroundPropertyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlBackgroundPropertyChangedCommandBehavior<T> : CalendarControlBackgroundPropertyChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlCalendarChangedCommand
    // CalendarControlCalendarChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlCalendarChangedCommand : ControlCommandBase<CalendarControlCalendarChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlCalendarChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.CalendarChanged += OnEventRaised;
        }
    }

    // CalendarControlCalendarChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlCalendarChangedCommandBehavior<T> : CalendarControlCalendarChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlCalendarStyleChangedCommand
    // CalendarControlCalendarStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlCalendarStyleChangedCommand : ControlCommandBase<CalendarControlCalendarStyleChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlCalendarStyleChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.CalendarStyleChanged += OnEventRaised;
        }
    }

    // CalendarControlCalendarStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlCalendarStyleChangedCommandBehavior<T> : CalendarControlCalendarStyleChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlCultureChangedCommand
    // CalendarControlCultureChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlCultureChangedCommand : ControlCommandBase<CalendarControlCultureChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlCultureChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.CultureChanged += OnEventRaised;
        }
    }

    // CalendarControlCultureChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlCultureChangedCommandBehavior<T> : CalendarControlCultureChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlDateChangedCommand
    // CalendarControlDateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlDateChangedCommand : ControlCommandBase<CalendarControlDateChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlDateChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.DateChanged += OnEventRaised;
        }
    }

    // CalendarControlDateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlDateChangedCommandBehavior<T> : CalendarControlDateChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlDateForegroundPropertyChangedCommand
    // CalendarControlDateForegroundPropertyChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlDateForegroundPropertyChangedCommand : ControlCommandBase<CalendarControlDateForegroundPropertyChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlDateForegroundPropertyChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.DateForegroundPropertyChanged += OnEventRaised;
        }
    }

    // CalendarControlDateForegroundPropertyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlDateForegroundPropertyChangedCommandBehavior<T> : CalendarControlDateForegroundPropertyChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlDayForegroundChangedCommand
    // CalendarControlDayForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlDayForegroundChangedCommand : ControlCommandBase<CalendarControlDayForegroundChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlDayForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.DayForegroundChanged += OnEventRaised;
        }
    }

    // CalendarControlDayForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlDayForegroundChangedCommandBehavior<T> : CalendarControlDayForegroundChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlDaysAbbreviationLengthChangedCommand
    // CalendarControlDaysAbbreviationLengthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlDaysAbbreviationLengthChangedCommand : ControlCommandBase<CalendarControlDaysAbbreviationLengthChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlDaysAbbreviationLengthChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.DaysAbbreviationLengthChanged += OnEventRaised;
        }
    }

    // CalendarControlDaysAbbreviationLengthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlDaysAbbreviationLengthChangedCommandBehavior<T> : CalendarControlDaysAbbreviationLengthChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlIsDayNameAbbreviatedChangedCommand
    // CalendarControlIsDayNameAbbreviatedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlIsDayNameAbbreviatedChangedCommand : ControlCommandBase<CalendarControlIsDayNameAbbreviatedChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlIsDayNameAbbreviatedChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.IsDayNameAbbreviatedChanged += OnEventRaised;
        }
    }

    // CalendarControlIsDayNameAbbreviatedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlIsDayNameAbbreviatedChangedCommandBehavior<T> : CalendarControlIsDayNameAbbreviatedChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlIsMonthNameAbbreviatedChangedCommand
    // CalendarControlIsMonthNameAbbreviatedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlIsMonthNameAbbreviatedChangedCommand : ControlCommandBase<CalendarControlIsMonthNameAbbreviatedChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlIsMonthNameAbbreviatedChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.IsMonthNameAbbreviatedChanged += OnEventRaised;
        }
    }

    // CalendarControlIsMonthNameAbbreviatedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlIsMonthNameAbbreviatedChangedCommandBehavior<T> : CalendarControlIsMonthNameAbbreviatedChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlIsShowWeekNumbersChangedCommand
    // CalendarControlIsShowWeekNumbersChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlIsShowWeekNumbersChangedCommand : ControlCommandBase<CalendarControlIsShowWeekNumbersChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlIsShowWeekNumbersChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.IsShowWeekNumbersChanged += OnEventRaised;
        }
    }

    // CalendarControlIsShowWeekNumbersChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlIsShowWeekNumbersChangedCommandBehavior<T> : CalendarControlIsShowWeekNumbersChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlMonthChangeDirectionChangedCommand
    // CalendarControlMonthChangeDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMonthChangeDirectionChangedCommand : ControlCommandBase<CalendarControlMonthChangeDirectionChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMonthChangeDirectionChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.MonthChangeDirectionChanged += OnEventRaised;
        }
    }

    // CalendarControlMonthChangeDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlMonthChangeDirectionChangedCommandBehavior<T> : CalendarControlMonthChangeDirectionChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlMouseHoverBorderBrushChangedCommand
    // CalendarControlMouseHoverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverBorderBrushChangedCommand : ControlCommandBase<CalendarControlMouseHoverBorderBrushChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.MouseHoverBorderBrushChanged += OnEventRaised;
        }
    }

    // CalendarControlMouseHoverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverBorderBrushChangedCommandBehavior<T> : CalendarControlMouseHoverBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlMouseHoverCellBackgroundBrushChangedCommand
    // CalendarControlMouseHoverCellBackgroundBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellBackgroundBrushChangedCommand : ControlCommandBase<CalendarControlMouseHoverCellBackgroundBrushChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellBackgroundBrushChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.MouseHoverCellBackgroundBrushChanged += OnEventRaised;
        }
    }

    // CalendarControlMouseHoverCellBackgroundBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellBackgroundBrushChangedCommandBehavior<T> : CalendarControlMouseHoverCellBackgroundBrushChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlMouseHoverCellBorderThicknessChangedCommand
    // CalendarControlMouseHoverCellBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellBorderThicknessChangedCommand : ControlCommandBase<CalendarControlMouseHoverCellBorderThicknessChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellBorderThicknessChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.MouseHoverCellBorderThicknessChanged += OnEventRaised;
        }
    }

    // CalendarControlMouseHoverCellBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellBorderThicknessChangedCommandBehavior<T> : CalendarControlMouseHoverCellBorderThicknessChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlMouseHoverCellCornerRadiusChangedCommand
    // CalendarControlMouseHoverCellCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellCornerRadiusChangedCommand : ControlCommandBase<CalendarControlMouseHoverCellCornerRadiusChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellCornerRadiusChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.MouseHoverCellCornerRadiusChanged += OnEventRaised;
        }
    }

    // CalendarControlMouseHoverCellCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverCellCornerRadiusChangedCommandBehavior<T> : CalendarControlMouseHoverCellCornerRadiusChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlMouseHoverForegroundPropertyChangedCommand
    // CalendarControlMouseHoverForegroundPropertyChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverForegroundPropertyChangedCommand : ControlCommandBase<CalendarControlMouseHoverForegroundPropertyChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverForegroundPropertyChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.MouseHoverForegroundPropertyChanged += OnEventRaised;
        }
    }

    // CalendarControlMouseHoverForegroundPropertyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlMouseHoverForegroundPropertyChangedCommandBehavior<T> : CalendarControlMouseHoverForegroundPropertyChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlMouseWheelCommand
    // CalendarControlMouseWheelCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseWheelCommand : ControlCommandBase<CalendarControlMouseWheelCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlMouseWheelCommandBehavior : CommandBehaviorBase<CalendarControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, Syncfusion.Windows.Tools.Controls.MouseWheelEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.MouseWheel += OnEventRaised;
        }
    }

    // CalendarControlMouseWheelCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlMouseWheelCommandBehavior<T> : CalendarControlMouseWheelCommandBehavior
    { }
    #endregion

    #region CalendarControlNextMonthDaysForegroundChangedCommand
    // CalendarControlNextMonthDaysForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlNextMonthDaysForegroundChangedCommand : ControlCommandBase<CalendarControlNextMonthDaysForegroundChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlNextMonthDaysForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.NextMonthDaysForegroundChanged += OnEventRaised;
        }
    }

    // CalendarControlNextMonthDaysForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlNextMonthDaysForegroundChangedCommandBehavior<T> : CalendarControlNextMonthDaysForegroundChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlPreviousMonthDaysForegroundChangedCommand
    // CalendarControlPreviousMonthDaysForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlPreviousMonthDaysForegroundChangedCommand : ControlCommandBase<CalendarControlPreviousMonthDaysForegroundChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlPreviousMonthDaysForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.PreviousMonthDaysForegroundChanged += OnEventRaised;
        }
    }

    // CalendarControlPreviousMonthDaysForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlPreviousMonthDaysForegroundChangedCommandBehavior<T> : CalendarControlPreviousMonthDaysForegroundChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlScrollButtonFillPropertyChangedCommand
    // CalendarControlScrollButtonFillPropertyChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlScrollButtonFillPropertyChangedCommand : ControlCommandBase<CalendarControlScrollButtonFillPropertyChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlScrollButtonFillPropertyChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.ScrollButtonFillPropertyChanged += OnEventRaised;
        }
    }

    // CalendarControlScrollButtonFillPropertyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlScrollButtonFillPropertyChangedCommandBehavior<T> : CalendarControlScrollButtonFillPropertyChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlSelectedCellBackgroundChangedCommand
    // CalendarControlSelectedCellBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellBackgroundChangedCommand : ControlCommandBase<CalendarControlSelectedCellBackgroundChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.SelectedCellBackgroundChanged += OnEventRaised;
        }
    }

    // CalendarControlSelectedCellBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellBackgroundChangedCommandBehavior<T> : CalendarControlSelectedCellBackgroundChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlSelectedCellBorderThicknessChangedCommand
    // CalendarControlSelectedCellBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellBorderThicknessChangedCommand : ControlCommandBase<CalendarControlSelectedCellBorderThicknessChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellBorderThicknessChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.SelectedCellBorderThicknessChanged += OnEventRaised;
        }
    }

    // CalendarControlSelectedCellBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellBorderThicknessChangedCommandBehavior<T> : CalendarControlSelectedCellBorderThicknessChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlSelectedCellCornerRadiusChangedCommand
    // CalendarControlSelectedCellCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellCornerRadiusChangedCommand : ControlCommandBase<CalendarControlSelectedCellCornerRadiusChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellCornerRadiusChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.SelectedCellCornerRadiusChanged += OnEventRaised;
        }
    }

    // CalendarControlSelectedCellCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellCornerRadiusChangedCommandBehavior<T> : CalendarControlSelectedCellCornerRadiusChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlSelectedCellForegroundPropertyChangedCommand
    // CalendarControlSelectedCellForegroundPropertyChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellForegroundPropertyChangedCommand : ControlCommandBase<CalendarControlSelectedCellForegroundPropertyChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellForegroundPropertyChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.SelectedCellForegroundPropertyChanged += OnEventRaised;
        }
    }

    // CalendarControlSelectedCellForegroundPropertyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlSelectedCellForegroundPropertyChangedCommandBehavior<T> : CalendarControlSelectedCellForegroundPropertyChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlSelectedDateChangedCommand
    // CalendarControlSelectedDateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedDateChangedCommand : ControlCommandBase<CalendarControlSelectedDateChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedDateChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.SelectedDateChanged += OnEventRaised;
        }
    }

    // CalendarControlSelectedDateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlSelectedDateChangedCommandBehavior<T> : CalendarControlSelectedDateChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlSelectedDatesChangedCommand
    // CalendarControlSelectedDatesChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedDatesChangedCommand : ControlCommandBase<CalendarControlSelectedDatesChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectedDatesChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.SelectedDatesChanged += OnEventRaised;
        }
    }

    // CalendarControlSelectedDatesChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlSelectedDatesChangedCommandBehavior<T> : CalendarControlSelectedDatesChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlSelectionBorderBrushChangedCommand
    // CalendarControlSelectionBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectionBorderBrushChangedCommand : ControlCommandBase<CalendarControlSelectionBorderBrushChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectionBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.SelectionBorderBrushChanged += OnEventRaised;
        }
    }

    // CalendarControlSelectionBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlSelectionBorderBrushChangedCommandBehavior<T> : CalendarControlSelectionBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlSelectionRangeModeChangedCommand
    // CalendarControlSelectionRangeModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectionRangeModeChangedCommand : ControlCommandBase<CalendarControlSelectionRangeModeChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlSelectionRangeModeChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.SelectionRangeModeChanged += OnEventRaised;
        }
    }

    // CalendarControlSelectionRangeModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlSelectionRangeModeChangedCommandBehavior<T> : CalendarControlSelectionRangeModeChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlShowNextMonthDaysChangedCommand
    // CalendarControlShowNextMonthDaysChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlShowNextMonthDaysChangedCommand : ControlCommandBase<CalendarControlShowNextMonthDaysChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlShowNextMonthDaysChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.ShowNextMonthDaysChanged += OnEventRaised;
        }
    }

    // CalendarControlShowNextMonthDaysChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlShowNextMonthDaysChangedCommandBehavior<T> : CalendarControlShowNextMonthDaysChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlShowPreviousMonthDaysChangedCommand
    // CalendarControlShowPreviousMonthDaysChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlShowPreviousMonthDaysChangedCommand : ControlCommandBase<CalendarControlShowPreviousMonthDaysChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlShowPreviousMonthDaysChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.ShowPreviousMonthDaysChanged += OnEventRaised;
        }
    }

    // CalendarControlShowPreviousMonthDaysChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlShowPreviousMonthDaysChangedCommandBehavior<T> : CalendarControlShowPreviousMonthDaysChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlTodayRowIsVisibleChangedCommand
    // CalendarControlTodayRowIsVisibleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlTodayRowIsVisibleChangedCommand : ControlCommandBase<CalendarControlTodayRowIsVisibleChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlTodayRowIsVisibleChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.TodayRowIsVisibleChanged += OnEventRaised;
        }
    }

    // CalendarControlTodayRowIsVisibleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlTodayRowIsVisibleChangedCommandBehavior<T> : CalendarControlTodayRowIsVisibleChangedCommandBehavior
    { }
    #endregion

    #region CalendarControlVisibleDataChangedCommand
    // CalendarControlVisibleDataChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlVisibleDataChangedCommand : ControlCommandBase<CalendarControlVisibleDataChangedCommandBehavior, CalendarControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CalendarControlVisibleDataChangedCommandBehavior : CommandBehaviorBase<CalendarControl>
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
            TargetObject.VisibleDataChanged += OnEventRaised;
        }
    }

    // CalendarControlVisibleDataChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CalendarControlVisibleDataChangedCommandBehavior<T> : CalendarControlVisibleDataChangedCommandBehavior
    { }
    #endregion
}

