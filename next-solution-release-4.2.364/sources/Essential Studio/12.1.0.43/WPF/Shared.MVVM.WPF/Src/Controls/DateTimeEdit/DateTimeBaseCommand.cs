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

	#region DateTimeBaseIsDropDownOpenChangedCommand
	// DateTimeBaseIsDropDownOpenChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseIsDropDownOpenChangedCommand : ControlCommandBase<DateTimeBaseIsDropDownOpenChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseIsDropDownOpenChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.IsDropDownOpenChanged += OnEventRaised;
        }
    }

	// DateTimeBaseIsDropDownOpenChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsDropDownOpenChangedCommandBehavior<T> : DateTimeBaseIsDropDownOpenChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseUnderlyingDateTimeChangedCommand
	// DateTimeBaseUnderlyingDateTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseUnderlyingDateTimeChangedCommand : ControlCommandBase<DateTimeBaseUnderlyingDateTimeChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseUnderlyingDateTimeChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.UnderlyingDateTimeChanged += OnEventRaised;
        }
    }

	// DateTimeBaseUnderlyingDateTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseUnderlyingDateTimeChangedCommandBehavior<T> : DateTimeBaseUnderlyingDateTimeChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseIsScrollingOnCircleChangedCommand
	// DateTimeBaseIsScrollingOnCircleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseIsScrollingOnCircleChangedCommand : ControlCommandBase<DateTimeBaseIsScrollingOnCircleChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseIsScrollingOnCircleChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.IsScrollingOnCircleChanged += OnEventRaised;
        }
    }

	// DateTimeBaseIsScrollingOnCircleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsScrollingOnCircleChangedCommandBehavior<T> : DateTimeBaseIsScrollingOnCircleChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseCustomPatternChangedCommand
	// DateTimeBaseCustomPatternChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseCustomPatternChangedCommand : ControlCommandBase<DateTimeBaseCustomPatternChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseCustomPatternChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.CustomPatternChanged += OnEventRaised;
        }
    }

	// DateTimeBaseCustomPatternChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseCustomPatternChangedCommandBehavior<T> : DateTimeBaseCustomPatternChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBasePatternChangedCommand
	// DateTimeBasePatternChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBasePatternChangedCommand : ControlCommandBase<DateTimeBasePatternChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBasePatternChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.PatternChanged += OnEventRaised;
        }
    }

	// DateTimeBasePatternChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBasePatternChangedCommandBehavior<T> : DateTimeBasePatternChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseIsWatchEnabledChangedCommand
	// DateTimeBaseIsWatchEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseIsWatchEnabledChangedCommand : ControlCommandBase<DateTimeBaseIsWatchEnabledChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseIsWatchEnabledChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.IsWatchEnabledChanged += OnEventRaised;
        }
    }

	// DateTimeBaseIsWatchEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsWatchEnabledChangedCommandBehavior<T> : DateTimeBaseIsWatchEnabledChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseIsEmptyDateEnabledChangedCommand
	// DateTimeBaseIsEmptyDateEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseIsEmptyDateEnabledChangedCommand : ControlCommandBase<DateTimeBaseIsEmptyDateEnabledChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseIsEmptyDateEnabledChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.IsEmptyDateEnabledChanged += OnEventRaised;
        }
    }

	// DateTimeBaseIsEmptyDateEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsEmptyDateEnabledChangedCommandBehavior<T> : DateTimeBaseIsEmptyDateEnabledChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseIsButtonPopUpEnabledChangedCommand
	// DateTimeBaseIsButtonPopUpEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseIsButtonPopUpEnabledChangedCommand : ControlCommandBase<DateTimeBaseIsButtonPopUpEnabledChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseIsButtonPopUpEnabledChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.IsButtonPopUpEnabledChanged += OnEventRaised;
        }
    }

	// DateTimeBaseIsButtonPopUpEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsButtonPopUpEnabledChangedCommandBehavior<T> : DateTimeBaseIsButtonPopUpEnabledChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseIsCalendarEnabledChangedCommand
	// DateTimeBaseIsCalendarEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseIsCalendarEnabledChangedCommand : ControlCommandBase<DateTimeBaseIsCalendarEnabledChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseIsCalendarEnabledChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.IsCalendarEnabledChanged += OnEventRaised;
        }
    }

	// DateTimeBaseIsCalendarEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsCalendarEnabledChangedCommandBehavior<T> : DateTimeBaseIsCalendarEnabledChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseIsVisibleRepeatButtonChangedCommand
	// DateTimeBaseIsVisibleRepeatButtonChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseIsVisibleRepeatButtonChangedCommand : ControlCommandBase<DateTimeBaseIsVisibleRepeatButtonChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseIsVisibleRepeatButtonChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.IsVisibleRepeatButtonChanged += OnEventRaised;
        }
    }

	// DateTimeBaseIsVisibleRepeatButtonChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsVisibleRepeatButtonChangedCommandBehavior<T> : DateTimeBaseIsVisibleRepeatButtonChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseRepeatButtonBackgroundChangedCommand
	// DateTimeBaseRepeatButtonBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseRepeatButtonBackgroundChangedCommand : ControlCommandBase<DateTimeBaseRepeatButtonBackgroundChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseRepeatButtonBackgroundChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.RepeatButtonBackgroundChanged += OnEventRaised;
        }
    }

	// DateTimeBaseRepeatButtonBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseRepeatButtonBackgroundChangedCommandBehavior<T> : DateTimeBaseRepeatButtonBackgroundChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseRepeatButtonBorderBrushChangedCommand
	// DateTimeBaseRepeatButtonBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseRepeatButtonBorderBrushChangedCommand : ControlCommandBase<DateTimeBaseRepeatButtonBorderBrushChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseRepeatButtonBorderBrushChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.RepeatButtonBorderBrushChanged += OnEventRaised;
        }
    }

	// DateTimeBaseRepeatButtonBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseRepeatButtonBorderBrushChangedCommandBehavior<T> : DateTimeBaseRepeatButtonBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseRepeatButtonBorderThicknessChangedCommand
	// DateTimeBaseRepeatButtonBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseRepeatButtonBorderThicknessChangedCommand : ControlCommandBase<DateTimeBaseRepeatButtonBorderThicknessChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseRepeatButtonBorderThicknessChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.RepeatButtonBorderThicknessChanged += OnEventRaised;
        }
    }

	// DateTimeBaseRepeatButtonBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseRepeatButtonBorderThicknessChangedCommandBehavior<T> : DateTimeBaseRepeatButtonBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseUpRepeatButtonMarginChangedCommand
	// DateTimeBaseUpRepeatButtonMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseUpRepeatButtonMarginChangedCommand : ControlCommandBase<DateTimeBaseUpRepeatButtonMarginChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseUpRepeatButtonMarginChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.UpRepeatButtonMarginChanged += OnEventRaised;
        }
    }

	// DateTimeBaseUpRepeatButtonMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseUpRepeatButtonMarginChangedCommandBehavior<T> : DateTimeBaseUpRepeatButtonMarginChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseDownRepeatButtonMarginChangedCommand
	// DateTimeBaseDownRepeatButtonMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseDownRepeatButtonMarginChangedCommand : ControlCommandBase<DateTimeBaseDownRepeatButtonMarginChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseDownRepeatButtonMarginChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.DownRepeatButtonMarginChanged += OnEventRaised;
        }
    }

	// DateTimeBaseDownRepeatButtonMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseDownRepeatButtonMarginChangedCommandBehavior<T> : DateTimeBaseDownRepeatButtonMarginChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseUpRepeatButtonTemplateChangedCommand
	// DateTimeBaseUpRepeatButtonTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseUpRepeatButtonTemplateChangedCommand : ControlCommandBase<DateTimeBaseUpRepeatButtonTemplateChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseUpRepeatButtonTemplateChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.UpRepeatButtonTemplateChanged += OnEventRaised;
        }
    }

	// DateTimeBaseUpRepeatButtonTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseUpRepeatButtonTemplateChangedCommandBehavior<T> : DateTimeBaseUpRepeatButtonTemplateChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseDownRepeatButtonTemplateChangedCommand
	// DateTimeBaseDownRepeatButtonTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseDownRepeatButtonTemplateChangedCommand : ControlCommandBase<DateTimeBaseDownRepeatButtonTemplateChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseDownRepeatButtonTemplateChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.DownRepeatButtonTemplateChanged += OnEventRaised;
        }
    }

	// DateTimeBaseDownRepeatButtonTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseDownRepeatButtonTemplateChangedCommandBehavior<T> : DateTimeBaseDownRepeatButtonTemplateChangedCommandBehavior
    { }
	#endregion

	#region DateTimeBaseCultureInfoChangedCommand
	// DateTimeBaseCultureInfoChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DateTimeBaseCultureInfoChangedCommand : ControlCommandBase<DateTimeBaseCultureInfoChangedCommandBehavior, DateTimeBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeBaseCultureInfoChangedCommandBehavior : CommandBehaviorBase<DateTimeBase>
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
            TargetObject.CultureInfoChanged += OnEventRaised;
        }
    }

	// DateTimeBaseCultureInfoChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseCultureInfoChangedCommandBehavior<T> : DateTimeBaseCultureInfoChangedCommandBehavior
    { }
	#endregion
}


