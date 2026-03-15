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

    #region NumericUpDownSelectionChangedCommand
    // NumericUpDownSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownSelectionChangedCommand : ControlCommandBase<NumericUpDownSelectionChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownSelectionChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void TargetObject_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged+=new RoutedEventHandler(TargetObject_SelectionChanged);
        }
    }

    // NumericUpDownSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownSelectionChangedCommandBehavior<T> : NumericUpDownSelectionChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownAllowEditChangedCommand
    // NumericUpDownAllowEditChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownAllowEditChangedCommand : ControlCommandBase<NumericUpDownAllowEditChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownAllowEditChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.AllowEditChanged += OnEventRaised;
        }
    }

    // NumericUpDownAllowEditChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownAllowEditChangedCommandBehavior<T> : NumericUpDownAllowEditChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownCornerRadiusChangedCommand
    // NumericUpDownCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownCornerRadiusChangedCommand : ControlCommandBase<NumericUpDownCornerRadiusChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownCornerRadiusChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.CornerRadiusChanged += OnEventRaised;
        }
    }

    // NumericUpDownCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownCornerRadiusChangedCommandBehavior<T> : NumericUpDownCornerRadiusChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownDownButtonTemplateChangedCommand
    // NumericUpDownDownButtonTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownDownButtonTemplateChangedCommand : ControlCommandBase<NumericUpDownDownButtonTemplateChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownDownButtonTemplateChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.DownButtonTemplateChanged += OnEventRaised;
        }
    }

    // NumericUpDownDownButtonTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownDownButtonTemplateChangedCommandBehavior<T> : NumericUpDownDownButtonTemplateChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownFlowDirectionChangedCommand
    // NumericUpDownFlowDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownFlowDirectionChangedCommand : ControlCommandBase<NumericUpDownFlowDirectionChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownFlowDirectionChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.FlowDirectionChanged += OnEventRaised;
        }
    }

    // NumericUpDownFlowDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownFlowDirectionChangedCommandBehavior<T> : NumericUpDownFlowDirectionChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownForegroundChangedCommand
    // NumericUpDownForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownForegroundChangedCommand : ControlCommandBase<NumericUpDownForegroundChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownForegroundChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.ForegroundChanged += OnEventRaised;
        }
    }

    // NumericUpDownForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownForegroundChangedCommandBehavior<T> : NumericUpDownForegroundChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownIntervalChangedCommand
    // NumericUpDownIntervalChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownIntervalChangedCommand : ControlCommandBase<NumericUpDownIntervalChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownIntervalChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.IntervalChanged += OnEventRaised;
        }
    }

    // NumericUpDownIntervalChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownIntervalChangedCommandBehavior<T> : NumericUpDownIntervalChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownIsFocusedChangedCommand
    // NumericUpDownIsFocusedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownIsFocusedChangedCommand : ControlCommandBase<NumericUpDownIsFocusedChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownIsFocusedChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.IsFocusedChanged += OnEventRaised;
        }
    }

    // NumericUpDownIsFocusedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownIsFocusedChangedCommandBehavior<T> : NumericUpDownIsFocusedChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownMaxValueChangedCommand
    // NumericUpDownMaxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownMaxValueChangedCommand : ControlCommandBase<NumericUpDownMaxValueChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownMaxValueChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.MaxValueChanged += OnEventRaised;
        }
    }

    // NumericUpDownMaxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownMaxValueChangedCommandBehavior<T> : NumericUpDownMaxValueChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownMinValueChangedCommand
    // NumericUpDownMinValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownMinValueChangedCommand : ControlCommandBase<NumericUpDownMinValueChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownMinValueChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.MinValueChanged += OnEventRaised;
        }
    }

    // NumericUpDownMinValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownMinValueChangedCommandBehavior<T> : NumericUpDownMinValueChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownNegativeForegroundChangedCommand
    // NumericUpDownNegativeForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownNegativeForegroundChangedCommand : ControlCommandBase<NumericUpDownNegativeForegroundChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownNegativeForegroundChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.NegativeForegroundChanged += OnEventRaised;
        }
    }

    // NumericUpDownNegativeForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownNegativeForegroundChangedCommandBehavior<T> : NumericUpDownNegativeForegroundChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownTextAlignmentChangedCommand
    // NumericUpDownTextAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownTextAlignmentChangedCommand : ControlCommandBase<NumericUpDownTextAlignmentChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownTextAlignmentChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.TextAlignmentChanged += OnEventRaised;
        }
    }

    // NumericUpDownTextAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownTextAlignmentChangedCommandBehavior<T> : NumericUpDownTextAlignmentChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownValueChangedCommand
    // NumericUpDownValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownValueChangedCommand : ControlCommandBase<NumericUpDownValueChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownValueChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.ValueChanged += OnEventRaised;
        }
    }

    // NumericUpDownValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownValueChangedCommandBehavior<T> : NumericUpDownValueChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownTextChangedCommand
    // NumericUpDownTextChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownTextChangedCommand : ControlCommandBase<NumericUpDownTextChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownTextChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.TextChanged += OnEventRaised;
        }
    }

    // NumericUpDownTextChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownTextChangedCommandBehavior<T> : NumericUpDownTextChangedCommandBehavior
    { }
    #endregion

    #region NumericUpDownUpButtonTemplateChangedCommand
    // NumericUpDownUpButtonTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownUpButtonTemplateChangedCommand : ControlCommandBase<NumericUpDownUpButtonTemplateChangedCommandBehavior, NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class NumericUpDownUpButtonTemplateChangedCommandBehavior : CommandBehaviorBase<NumericUpDown>
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
            TargetObject.UpButtonTemplateChanged += OnEventRaised;
        }
    }

    // NumericUpDownUpButtonTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class NumericUpDownUpButtonTemplateChangedCommandBehavior<T> : NumericUpDownUpButtonTemplateChangedCommandBehavior
    { }
    #endregion
}
