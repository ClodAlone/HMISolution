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

    #region DateTimeBaseIsDropDownOpenChangedCommand
    /// <summary>
    /// DateTimeBaseIsDropDownOpenChangedCommand
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

    /// <summary>
    /// DateTimeBaseIsDropDownOpenChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsDropDownOpenChangedCommandBehavior<T> : DateTimeBaseIsDropDownOpenChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseUnderlyingDateTimeChangedCommand
    /// <summary>
    /// DateTimeBaseUnderlyingDateTimeChangedCommand
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

    /// <summary>
    /// DateTimeBaseUnderlyingDateTimeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseUnderlyingDateTimeChangedCommandBehavior<T> : DateTimeBaseUnderlyingDateTimeChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseIsScrollingOnCircleChangedCommand
    /// <summary>
    /// DateTimeBaseIsScrollingOnCircleChangedCommand
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

    /// <summary>
    /// DateTimeBaseIsScrollingOnCircleChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsScrollingOnCircleChangedCommandBehavior<T> : DateTimeBaseIsScrollingOnCircleChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseCustomPatternChangedCommand
    /// <summary>
    /// DateTimeBaseCustomPatternChangedCommand
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

    /// <summary>
    /// DateTimeBaseCustomPatternChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseCustomPatternChangedCommandBehavior<T> : DateTimeBaseCustomPatternChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBasePatternChangedCommand
    /// <summary>
    /// DateTimeBasePatternChangedCommand
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

    /// <summary>
    /// DateTimeBasePatternChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBasePatternChangedCommandBehavior<T> : DateTimeBasePatternChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseIsEmptyDateEnabledChangedCommand
    /// <summary>
    /// DateTimeBaseIsEmptyDateEnabledChangedCommand
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

    /// <summary>
    /// DateTimeBaseIsEmptyDateEnabledChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsEmptyDateEnabledChangedCommandBehavior<T> : DateTimeBaseIsEmptyDateEnabledChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseIsButtonPopUpEnabledChangedCommand
    /// <summary>
    /// DateTimeBaseIsButtonPopUpEnabledChangedCommand
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

    /// <summary>
    /// DateTimeBaseIsButtonPopUpEnabledChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsButtonPopUpEnabledChangedCommandBehavior<T> : DateTimeBaseIsButtonPopUpEnabledChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseIsCalendarEnabledChangedCommand
    /// <summary>
    /// DateTimeBaseIsCalendarEnabledChangedCommand
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

    /// <summary>
    /// DateTimeBaseIsCalendarEnabledChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsCalendarEnabledChangedCommandBehavior<T> : DateTimeBaseIsCalendarEnabledChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseIsVisibleRepeatButtonChangedCommand
    /// <summary>
    /// DateTimeBaseIsVisibleRepeatButtonChangedCommand
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

    /// <summary>
    /// DateTimeBaseIsVisibleRepeatButtonChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseIsVisibleRepeatButtonChangedCommandBehavior<T> : DateTimeBaseIsVisibleRepeatButtonChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseRepeatButtonBackgroundChangedCommand
    /// <summary>
    /// DateTimeBaseRepeatButtonBackgroundChangedCommand
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

    /// <summary>
    /// DateTimeBaseRepeatButtonBackgroundChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseRepeatButtonBackgroundChangedCommandBehavior<T> : DateTimeBaseRepeatButtonBackgroundChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseRepeatButtonBorderBrushChangedCommand
    /// <summary>
    /// DateTimeBaseRepeatButtonBorderBrushChangedCommand
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

    /// <summary>
    /// DateTimeBaseRepeatButtonBorderBrushChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseRepeatButtonBorderBrushChangedCommandBehavior<T> : DateTimeBaseRepeatButtonBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseRepeatButtonBorderThicknessChangedCommand
    /// <summary>
    /// DateTimeBaseRepeatButtonBorderThicknessChangedCommand
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

    /// <summary>
    /// DateTimeBaseRepeatButtonBorderThicknessChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseRepeatButtonBorderThicknessChangedCommandBehavior<T> : DateTimeBaseRepeatButtonBorderThicknessChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseUpRepeatButtonMarginChangedCommand
    /// <summary>
    /// DateTimeBaseUpRepeatButtonMarginChangedCommand
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

    /// <summary>
    /// DateTimeBaseUpRepeatButtonMarginChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseUpRepeatButtonMarginChangedCommandBehavior<T> : DateTimeBaseUpRepeatButtonMarginChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseDownRepeatButtonMarginChangedCommand
    /// <summary>
    /// DateTimeBaseDownRepeatButtonMarginChangedCommand
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

    /// <summary>
    /// DateTimeBaseDownRepeatButtonMarginChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseDownRepeatButtonMarginChangedCommandBehavior<T> : DateTimeBaseDownRepeatButtonMarginChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseUpRepeatButtonTemplateChangedCommand
    /// <summary>
    /// DateTimeBaseUpRepeatButtonTemplateChangedCommand
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

    /// <summary>
    /// DateTimeBaseUpRepeatButtonTemplateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseUpRepeatButtonTemplateChangedCommandBehavior<T> : DateTimeBaseUpRepeatButtonTemplateChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseDownRepeatButtonTemplateChangedCommand
    /// <summary>
    /// DateTimeBaseDownRepeatButtonTemplateChangedCommand
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

    /// <summary>
    /// DateTimeBaseDownRepeatButtonTemplateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseDownRepeatButtonTemplateChangedCommandBehavior<T> : DateTimeBaseDownRepeatButtonTemplateChangedCommandBehavior
    { }
    #endregion

    #region DateTimeBaseCultureInfoChangedCommand
    /// <summary>
    /// DateTimeBaseCultureInfoChangedCommand
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

    /// <summary>
    /// DateTimeBaseCultureInfoChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateTimeBaseCultureInfoChangedCommandBehavior<T> : DateTimeBaseCultureInfoChangedCommandBehavior
    { }
    #endregion      
}


