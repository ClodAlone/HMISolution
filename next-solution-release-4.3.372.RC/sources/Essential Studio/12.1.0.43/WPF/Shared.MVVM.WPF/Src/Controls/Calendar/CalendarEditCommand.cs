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

	#region CalendarEditDateChangedCommand
	// CalendarEditDateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDateChangedCommand : ControlCommandBase<CalendarEditDateChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDateChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditDateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDateChangedCommandBehavior<T> : CalendarEditDateChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditMonthChangedCommand
	// CalendarEditMonthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditMonthChangedCommand : ControlCommandBase<CalendarEditMonthChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditMonthChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EventArgs e)
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

	// CalendarEditMonthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditMonthChangedCommandBehavior<T> : CalendarEditMonthChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditCultureChangedCommand
	// CalendarEditCultureChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditCultureChangedCommand : ControlCommandBase<CalendarEditCultureChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditCultureChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditCultureChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditCultureChangedCommandBehavior<T> : CalendarEditCultureChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditCalendarChangedCommand
	// CalendarEditCalendarChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditCalendarChangedCommand : ControlCommandBase<CalendarEditCalendarChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditCalendarChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditCalendarChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditCalendarChangedCommandBehavior<T> : CalendarEditCalendarChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditCalendarStyleChangedCommand
	// CalendarEditCalendarStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditCalendarStyleChangedCommand : ControlCommandBase<CalendarEditCalendarStyleChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditCalendarStyleChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditCalendarStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditCalendarStyleChangedCommandBehavior<T> : CalendarEditCalendarStyleChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDisableDateSelectionChangedCommand
	// CalendarEditDisableDateSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDisableDateSelectionChangedCommand : ControlCommandBase<CalendarEditDisableDateSelectionChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDisableDateSelectionChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditDisableDateSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDisableDateSelectionChangedCommandBehavior<T> : CalendarEditDisableDateSelectionChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditAllowSelectionChangedCommand
	// CalendarEditAllowSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditAllowSelectionChangedCommand : ControlCommandBase<CalendarEditAllowSelectionChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditAllowSelectionChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.AllowSelectionChanged += OnEventRaised;
        }
    }

	// CalendarEditAllowSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditAllowSelectionChangedCommandBehavior<T> : CalendarEditAllowSelectionChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditAllowMultiplySelectionChangedCommand
	// CalendarEditAllowMultiplySelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditAllowMultiplySelectionChangedCommand : ControlCommandBase<CalendarEditAllowMultiplySelectionChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditAllowMultiplySelectionChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.AllowMultiplySelectionChanged += OnEventRaised;
        }
    }

	// CalendarEditAllowMultiplySelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditAllowMultiplySelectionChangedCommandBehavior<T> : CalendarEditAllowMultiplySelectionChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditIsDayNamesAbbreviatedChangedCommand
	// CalendarEditIsDayNamesAbbreviatedChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditIsDayNamesAbbreviatedChangedCommand : ControlCommandBase<CalendarEditIsDayNamesAbbreviatedChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditIsDayNamesAbbreviatedChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.IsDayNamesAbbreviatedChanged += OnEventRaised;
        }
    }

	// CalendarEditIsDayNamesAbbreviatedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditIsDayNamesAbbreviatedChangedCommandBehavior<T> : CalendarEditIsDayNamesAbbreviatedChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditIsMonthNameAbbreviatedChangedCommand
	// CalendarEditIsMonthNameAbbreviatedChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditIsMonthNameAbbreviatedChangedCommand : ControlCommandBase<CalendarEditIsMonthNameAbbreviatedChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditIsMonthNameAbbreviatedChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditIsMonthNameAbbreviatedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditIsMonthNameAbbreviatedChangedCommandBehavior<T> : CalendarEditIsMonthNameAbbreviatedChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditSelectionRangeModeChangedCommand
	// CalendarEditSelectionRangeModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditSelectionRangeModeChangedCommand : ControlCommandBase<CalendarEditSelectionRangeModeChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditSelectionRangeModeChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditSelectionRangeModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditSelectionRangeModeChangedCommandBehavior<T> : CalendarEditSelectionRangeModeChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditSelectionBorderBrushChangedCommand
	// CalendarEditSelectionBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditSelectionBorderBrushChangedCommand : ControlCommandBase<CalendarEditSelectionBorderBrushChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditSelectionBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditSelectionBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditSelectionBorderBrushChangedCommandBehavior<T> : CalendarEditSelectionBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditMouseOverBorderBrushChangedCommand
	// CalendarEditMouseOverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditMouseOverBorderBrushChangedCommand : ControlCommandBase<CalendarEditMouseOverBorderBrushChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditMouseOverBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.MouseOverBorderBrushChanged += OnEventRaised;
        }
    }

	// CalendarEditMouseOverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditMouseOverBorderBrushChangedCommandBehavior<T> : CalendarEditMouseOverBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditMouseOverBackgroundChangedCommand
	// CalendarEditMouseOverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditMouseOverBackgroundChangedCommand : ControlCommandBase<CalendarEditMouseOverBackgroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditMouseOverBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.MouseOverBackgroundChanged += OnEventRaised;
        }
    }

	// CalendarEditMouseOverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditMouseOverBackgroundChangedCommandBehavior<T> : CalendarEditMouseOverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditMouseOverForegroundChangedCommand
	// CalendarEditMouseOverForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditMouseOverForegroundChangedCommand : ControlCommandBase<CalendarEditMouseOverForegroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditMouseOverForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.MouseOverForegroundChanged += OnEventRaised;
        }
    }

	// CalendarEditMouseOverForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditMouseOverForegroundChangedCommandBehavior<T> : CalendarEditMouseOverForegroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditSelectedDayCellBorderBrushChangedCommand
	// CalendarEditSelectedDayCellBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditSelectedDayCellBorderBrushChangedCommand : ControlCommandBase<CalendarEditSelectedDayCellBorderBrushChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditSelectedDayCellBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.SelectedDayCellBorderBrushChanged += OnEventRaised;
        }
    }

	// CalendarEditSelectedDayCellBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditSelectedDayCellBorderBrushChangedCommandBehavior<T> : CalendarEditSelectedDayCellBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditTodayCellBorderBrushChangedCommand
	// CalendarEditTodayCellBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditTodayCellBorderBrushChangedCommand : ControlCommandBase<CalendarEditTodayCellBorderBrushChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditTodayCellBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.TodayCellBorderBrushChanged += OnEventRaised;
        }
    }

	// CalendarEditTodayCellBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditTodayCellBorderBrushChangedCommandBehavior<T> : CalendarEditTodayCellBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditTodayCellForegroundChangedCommand
	// CalendarEditTodayCellForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditTodayCellForegroundChangedCommand : ControlCommandBase<CalendarEditTodayCellForegroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditTodayCellForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.TodayCellForegroundChanged += OnEventRaised;
        }
    }

	// CalendarEditTodayCellForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditTodayCellForegroundChangedCommandBehavior<T> : CalendarEditTodayCellForegroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditTodayCellBackgroundChangedCommand
	// CalendarEditTodayCellBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditTodayCellBackgroundChangedCommand : ControlCommandBase<CalendarEditTodayCellBackgroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditTodayCellBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.TodayCellBackgroundChanged += OnEventRaised;
        }
    }

	// CalendarEditTodayCellBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditTodayCellBackgroundChangedCommandBehavior<T> : CalendarEditTodayCellBackgroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditTodayCellSelectedBorderBrushChangedCommand
	// CalendarEditTodayCellSelectedBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditTodayCellSelectedBorderBrushChangedCommand : ControlCommandBase<CalendarEditTodayCellSelectedBorderBrushChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditTodayCellSelectedBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.TodayCellSelectedBorderBrushChanged += OnEventRaised;
        }
    }

	// CalendarEditTodayCellSelectedBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditTodayCellSelectedBorderBrushChangedCommandBehavior<T> : CalendarEditTodayCellSelectedBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditTodayCellSelectedBackgroundChangedCommand
	// CalendarEditTodayCellSelectedBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditTodayCellSelectedBackgroundChangedCommand : ControlCommandBase<CalendarEditTodayCellSelectedBackgroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditTodayCellSelectedBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.TodayCellSelectedBackgroundChanged += OnEventRaised;
        }
    }

	// CalendarEditTodayCellSelectedBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditTodayCellSelectedBackgroundChangedCommandBehavior<T> : CalendarEditTodayCellSelectedBackgroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditSelectedDayCellHoverBackgroundChangedCommand
	// CalendarEditSelectedDayCellHoverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditSelectedDayCellHoverBackgroundChangedCommand : ControlCommandBase<CalendarEditSelectedDayCellHoverBackgroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditSelectedDayCellHoverBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.SelectedDayCellHoverBackgroundChanged += OnEventRaised;
        }
    }

	// CalendarEditSelectedDayCellHoverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditSelectedDayCellHoverBackgroundChangedCommandBehavior<T> : CalendarEditSelectedDayCellHoverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditSelectedDayCellBackgroundChangedCommand
	// CalendarEditSelectedDayCellBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditSelectedDayCellBackgroundChangedCommand : ControlCommandBase<CalendarEditSelectedDayCellBackgroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditSelectedDayCellBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.SelectedDayCellBackgroundChanged += OnEventRaised;
        }
    }

	// CalendarEditSelectedDayCellBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditSelectedDayCellBackgroundChangedCommandBehavior<T> : CalendarEditSelectedDayCellBackgroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditSelectedDayCellForegroundChangedCommand
	// CalendarEditSelectedDayCellForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditSelectedDayCellForegroundChangedCommand : ControlCommandBase<CalendarEditSelectedDayCellForegroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditSelectedDayCellForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.SelectedDayCellForegroundChanged += OnEventRaised;
        }
    }

	// CalendarEditSelectedDayCellForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditSelectedDayCellForegroundChangedCommandBehavior<T> : CalendarEditSelectedDayCellForegroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditNotCurrentMonthForegroundChangedCommand
	// CalendarEditNotCurrentMonthForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditNotCurrentMonthForegroundChangedCommand : ControlCommandBase<CalendarEditNotCurrentMonthForegroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditNotCurrentMonthForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.NotCurrentMonthForegroundChanged += OnEventRaised;
        }
    }

	// CalendarEditNotCurrentMonthForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditNotCurrentMonthForegroundChangedCommandBehavior<T> : CalendarEditNotCurrentMonthForegroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditSelectionForegroundChangedCommand
	// CalendarEditSelectionForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditSelectionForegroundChangedCommand : ControlCommandBase<CalendarEditSelectionForegroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditSelectionForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.SelectionForegroundChanged += OnEventRaised;
        }
    }

	// CalendarEditSelectionForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditSelectionForegroundChangedCommandBehavior<T> : CalendarEditSelectionForegroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberSelectionBorderBrushChangedCommand
	// CalendarEditWeekNumberSelectionBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberSelectionBorderBrushChangedCommand : ControlCommandBase<CalendarEditWeekNumberSelectionBorderBrushChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberSelectionBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberSelectionBorderBrushChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberSelectionBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberSelectionBorderBrushChangedCommandBehavior<T> : CalendarEditWeekNumberSelectionBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberSelectionBorderThicknessChangedCommand
	// CalendarEditWeekNumberSelectionBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberSelectionBorderThicknessChangedCommand : ControlCommandBase<CalendarEditWeekNumberSelectionBorderThicknessChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberSelectionBorderThicknessChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberSelectionBorderThicknessChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberSelectionBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberSelectionBorderThicknessChangedCommandBehavior<T> : CalendarEditWeekNumberSelectionBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberSelectionBorderCornerRadiusChangedCommand
	// CalendarEditWeekNumberSelectionBorderCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberSelectionBorderCornerRadiusChangedCommand : ControlCommandBase<CalendarEditWeekNumberSelectionBorderCornerRadiusChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberSelectionBorderCornerRadiusChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberSelectionBorderCornerRadiusChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberSelectionBorderCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberSelectionBorderCornerRadiusChangedCommandBehavior<T> : CalendarEditWeekNumberSelectionBorderCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberBackgroundChangedCommand
	// CalendarEditWeekNumberBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberBackgroundChangedCommand : ControlCommandBase<CalendarEditWeekNumberBackgroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberBackgroundChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberBackgroundChangedCommandBehavior<T> : CalendarEditWeekNumberBackgroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberForegroundChangedCommand
	// CalendarEditWeekNumberForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberForegroundChangedCommand : ControlCommandBase<CalendarEditWeekNumberForegroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberForegroundChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberForegroundChangedCommandBehavior<T> : CalendarEditWeekNumberForegroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberHoverForegroundChangedCommand
	// CalendarEditWeekNumberHoverForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberHoverForegroundChangedCommand : ControlCommandBase<CalendarEditWeekNumberHoverForegroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberHoverForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberHoverForegroundChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberHoverForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberHoverForegroundChangedCommandBehavior<T> : CalendarEditWeekNumberHoverForegroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberSelectionForegroundChangedCommand
	// CalendarEditWeekNumberSelectionForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberSelectionForegroundChangedCommand : ControlCommandBase<CalendarEditWeekNumberSelectionForegroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberSelectionForegroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberSelectionForegroundChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberSelectionForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberSelectionForegroundChangedCommandBehavior<T> : CalendarEditWeekNumberSelectionForegroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberSelectionBackgroundChangedCommand
	// CalendarEditWeekNumberSelectionBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberSelectionBackgroundChangedCommand : ControlCommandBase<CalendarEditWeekNumberSelectionBackgroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberSelectionBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberSelectionBackgroundChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberSelectionBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberSelectionBackgroundChangedCommandBehavior<T> : CalendarEditWeekNumberSelectionBackgroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberHoverBackgroundChangedCommand
	// CalendarEditWeekNumberHoverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberHoverBackgroundChangedCommand : ControlCommandBase<CalendarEditWeekNumberHoverBackgroundChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberHoverBackgroundChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberHoverBackgroundChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberHoverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberHoverBackgroundChangedCommandBehavior<T> : CalendarEditWeekNumberHoverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberHoverBorderBrushChangedCommand
	// CalendarEditWeekNumberHoverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberHoverBorderBrushChangedCommand : ControlCommandBase<CalendarEditWeekNumberHoverBorderBrushChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberHoverBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberHoverBorderBrushChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberHoverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberHoverBorderBrushChangedCommandBehavior<T> : CalendarEditWeekNumberHoverBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberBorderBrushChangedCommand
	// CalendarEditWeekNumberBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberBorderBrushChangedCommand : ControlCommandBase<CalendarEditWeekNumberBorderBrushChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberBorderBrushChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberBorderBrushChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberBorderBrushChangedCommandBehavior<T> : CalendarEditWeekNumberBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberBorderThicknessChangedCommand
	// CalendarEditWeekNumberBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberBorderThicknessChangedCommand : ControlCommandBase<CalendarEditWeekNumberBorderThicknessChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberBorderThicknessChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberBorderThicknessChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberBorderThicknessChangedCommandBehavior<T> : CalendarEditWeekNumberBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditWeekNumberCornerRadiusChangedCommand
	// CalendarEditWeekNumberCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditWeekNumberCornerRadiusChangedCommand : ControlCommandBase<CalendarEditWeekNumberCornerRadiusChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditWeekNumberCornerRadiusChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.WeekNumberCornerRadiusChanged += OnEventRaised;
        }
    }

	// CalendarEditWeekNumberCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditWeekNumberCornerRadiusChangedCommandBehavior<T> : CalendarEditWeekNumberCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditSelectionBorderCornerRadiusChangedCommand
	// CalendarEditSelectionBorderCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditSelectionBorderCornerRadiusChangedCommand : ControlCommandBase<CalendarEditSelectionBorderCornerRadiusChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditSelectionBorderCornerRadiusChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.SelectionBorderCornerRadiusChanged += OnEventRaised;
        }
    }

	// CalendarEditSelectionBorderCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditSelectionBorderCornerRadiusChangedCommandBehavior<T> : CalendarEditSelectionBorderCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditFrameMovingTimeChangedCommand
	// CalendarEditFrameMovingTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditFrameMovingTimeChangedCommand : ControlCommandBase<CalendarEditFrameMovingTimeChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditFrameMovingTimeChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.FrameMovingTimeChanged += OnEventRaised;
        }
    }

	// CalendarEditFrameMovingTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditFrameMovingTimeChangedCommandBehavior<T> : CalendarEditFrameMovingTimeChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditChangeModeTimeChangedCommand
	// CalendarEditChangeModeTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditChangeModeTimeChangedCommand : ControlCommandBase<CalendarEditChangeModeTimeChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditChangeModeTimeChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.ChangeModeTimeChanged += OnEventRaised;
        }
    }

	// CalendarEditChangeModeTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditChangeModeTimeChangedCommandBehavior<T> : CalendarEditChangeModeTimeChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditMonthChangeDirectionChangedCommand
	// CalendarEditMonthChangeDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditMonthChangeDirectionChangedCommand : ControlCommandBase<CalendarEditMonthChangeDirectionChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditMonthChangeDirectionChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditMonthChangeDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditMonthChangeDirectionChangedCommandBehavior<T> : CalendarEditMonthChangeDirectionChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDayNameCellsDataTemplateChangedCommand
	// CalendarEditDayNameCellsDataTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDayNameCellsDataTemplateChangedCommand : ControlCommandBase<CalendarEditDayNameCellsDataTemplateChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDayNameCellsDataTemplateChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.DayNameCellsDataTemplateChanged += OnEventRaised;
        }
    }

	// CalendarEditDayNameCellsDataTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDayNameCellsDataTemplateChangedCommandBehavior<T> : CalendarEditDayNameCellsDataTemplateChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditPreviousScrollButtonTemplateChangedCommand
	// CalendarEditPreviousScrollButtonTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditPreviousScrollButtonTemplateChangedCommand : ControlCommandBase<CalendarEditPreviousScrollButtonTemplateChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditPreviousScrollButtonTemplateChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.PreviousScrollButtonTemplateChanged += OnEventRaised;
        }
    }

	// CalendarEditPreviousScrollButtonTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditPreviousScrollButtonTemplateChangedCommandBehavior<T> : CalendarEditPreviousScrollButtonTemplateChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditNextScrollButtonTemplateChangedCommand
	// CalendarEditNextScrollButtonTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditNextScrollButtonTemplateChangedCommand : ControlCommandBase<CalendarEditNextScrollButtonTemplateChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditNextScrollButtonTemplateChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.NextScrollButtonTemplateChanged += OnEventRaised;
        }
    }

	// CalendarEditNextScrollButtonTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditNextScrollButtonTemplateChangedCommandBehavior<T> : CalendarEditNextScrollButtonTemplateChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDayCellsDataTemplateChangedCommand
	// CalendarEditDayCellsDataTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDayCellsDataTemplateChangedCommand : ControlCommandBase<CalendarEditDayCellsDataTemplateChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDayCellsDataTemplateChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.DayCellsDataTemplateChanged += OnEventRaised;
        }
    }

	// CalendarEditDayCellsDataTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDayCellsDataTemplateChangedCommandBehavior<T> : CalendarEditDayCellsDataTemplateChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDayCellsStyleChangedCommand
	// CalendarEditDayCellsStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDayCellsStyleChangedCommand : ControlCommandBase<CalendarEditDayCellsStyleChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDayCellsStyleChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.DayCellsStyleChanged += OnEventRaised;
        }
    }

	// CalendarEditDayCellsStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDayCellsStyleChangedCommandBehavior<T> : CalendarEditDayCellsStyleChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDayNameCellsStyleChangedCommand
	// CalendarEditDayNameCellsStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDayNameCellsStyleChangedCommand : ControlCommandBase<CalendarEditDayNameCellsStyleChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDayNameCellsStyleChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.DayNameCellsStyleChanged += OnEventRaised;
        }
    }

	// CalendarEditDayNameCellsStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDayNameCellsStyleChangedCommandBehavior<T> : CalendarEditDayNameCellsStyleChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDayCellsDataTemplateSelectorChangedCommand
	// CalendarEditDayCellsDataTemplateSelectorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDayCellsDataTemplateSelectorChangedCommand : ControlCommandBase<CalendarEditDayCellsDataTemplateSelectorChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDayCellsDataTemplateSelectorChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.DayCellsDataTemplateSelectorChanged += OnEventRaised;
        }
    }

	// CalendarEditDayCellsDataTemplateSelectorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDayCellsDataTemplateSelectorChangedCommandBehavior<T> : CalendarEditDayCellsDataTemplateSelectorChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDayNameCellsDataTemplateSelectorChangedCommand
	// CalendarEditDayNameCellsDataTemplateSelectorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDayNameCellsDataTemplateSelectorChangedCommand : ControlCommandBase<CalendarEditDayNameCellsDataTemplateSelectorChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDayNameCellsDataTemplateSelectorChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.DayNameCellsDataTemplateSelectorChanged += OnEventRaised;
        }
    }

	// CalendarEditDayNameCellsDataTemplateSelectorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDayNameCellsDataTemplateSelectorChangedCommandBehavior<T> : CalendarEditDayNameCellsDataTemplateSelectorChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDateDataTemplatesChangedCommand
	// CalendarEditDateDataTemplatesChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDateDataTemplatesChangedCommand : ControlCommandBase<CalendarEditDateDataTemplatesChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDateDataTemplatesChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.DateDataTemplatesChanged += OnEventRaised;
        }
    }

	// CalendarEditDateDataTemplatesChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDateDataTemplatesChangedCommandBehavior<T> : CalendarEditDateDataTemplatesChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditDateStylesChangedCommand
	// CalendarEditDateStylesChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditDateStylesChangedCommand : ControlCommandBase<CalendarEditDateStylesChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditDateStylesChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.DateStylesChanged += OnEventRaised;
        }
    }

	// CalendarEditDateStylesChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditDateStylesChangedCommandBehavior<T> : CalendarEditDateStylesChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditScrollToDateEnabledChangedCommand
	// CalendarEditScrollToDateEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditScrollToDateEnabledChangedCommand : ControlCommandBase<CalendarEditScrollToDateEnabledChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditScrollToDateEnabledChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.ScrollToDateEnabledChanged += OnEventRaised;
        }
    }

	// CalendarEditScrollToDateEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditScrollToDateEnabledChangedCommandBehavior<T> : CalendarEditScrollToDateEnabledChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditVisualModeChangedCommand
	// CalendarEditVisualModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditVisualModeChangedCommand : ControlCommandBase<CalendarEditVisualModeChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditVisualModeChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.VisualModeChanged += OnEventRaised;
        }
    }

	// CalendarEditVisualModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditVisualModeChangedCommandBehavior<T> : CalendarEditVisualModeChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditTodayRowIsVisibleChangedCommand
	// CalendarEditTodayRowIsVisibleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditTodayRowIsVisibleChangedCommand : ControlCommandBase<CalendarEditTodayRowIsVisibleChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditTodayRowIsVisibleChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditTodayRowIsVisibleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditTodayRowIsVisibleChangedCommandBehavior<T> : CalendarEditTodayRowIsVisibleChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditMinMaxHiddenChangedCommand
	// CalendarEditMinMaxHiddenChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditMinMaxHiddenChangedCommand : ControlCommandBase<CalendarEditMinMaxHiddenChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditMinMaxHiddenChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.MinMaxHiddenChanged += OnEventRaised;
        }
    }

	// CalendarEditMinMaxHiddenChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditMinMaxHiddenChangedCommandBehavior<T> : CalendarEditMinMaxHiddenChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditIsShowWeekNumbersChangedCommand
	// CalendarEditIsShowWeekNumbersChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditIsShowWeekNumbersChangedCommand : ControlCommandBase<CalendarEditIsShowWeekNumbersChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditIsShowWeekNumbersChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditIsShowWeekNumbersChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditIsShowWeekNumbersChangedCommandBehavior<T> : CalendarEditIsShowWeekNumbersChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditIsShowWeekNumbersGridChangedCommand
	// CalendarEditIsShowWeekNumbersGridChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditIsShowWeekNumbersGridChangedCommand : ControlCommandBase<CalendarEditIsShowWeekNumbersGridChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditIsShowWeekNumbersGridChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.IsShowWeekNumbersGridChanged += OnEventRaised;
        }
    }

	// CalendarEditIsShowWeekNumbersGridChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditIsShowWeekNumbersGridChangedCommandBehavior<T> : CalendarEditIsShowWeekNumbersGridChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditIsAllowYearSelectionChangedCommand
	// CalendarEditIsAllowYearSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditIsAllowYearSelectionChangedCommand : ControlCommandBase<CalendarEditIsAllowYearSelectionChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditIsAllowYearSelectionChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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
            TargetObject.IsAllowYearSelectionChanged += OnEventRaised;
        }
    }

	// CalendarEditIsAllowYearSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditIsAllowYearSelectionChangedCommandBehavior<T> : CalendarEditIsAllowYearSelectionChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditShowPreviousMonthDaysChangedCommand
	// CalendarEditShowPreviousMonthDaysChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditShowPreviousMonthDaysChangedCommand : ControlCommandBase<CalendarEditShowPreviousMonthDaysChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditShowPreviousMonthDaysChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditShowPreviousMonthDaysChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditShowPreviousMonthDaysChangedCommandBehavior<T> : CalendarEditShowPreviousMonthDaysChangedCommandBehavior
    { }
	#endregion

	#region CalendarEditShowNextMonthDaysChangedCommand
	// CalendarEditShowNextMonthDaysChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CalendarEditShowNextMonthDaysChangedCommand : ControlCommandBase<CalendarEditShowNextMonthDaysChangedCommandBehavior, CalendarEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CalendarEditShowNextMonthDaysChangedCommandBehavior : CommandBehaviorBase<CalendarEdit>
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

	// CalendarEditShowNextMonthDaysChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CalendarEditShowNextMonthDaysChangedCommandBehavior<T> : CalendarEditShowNextMonthDaysChangedCommandBehavior
    { }
	#endregion
}


