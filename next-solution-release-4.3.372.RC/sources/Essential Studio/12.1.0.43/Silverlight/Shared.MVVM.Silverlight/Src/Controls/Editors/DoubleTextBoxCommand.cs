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

    #region DoubleTextBoxMinValueChangedCommand
    /// <summary>
    /// DoubleTextBoxMinValueChangedCommand
    /// </summary>
    public class DoubleTextBoxMinValueChangedCommand : ControlCommandBase<DoubleTextBoxMinValueChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxMinValueChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

    /// <summary>
    /// DoubleTextBoxMinValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxMinValueChangedCommandBehavior<T> : DoubleTextBoxMinValueChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxValueChangedCommand
    /// <summary>
    /// DoubleTextBoxValueChangedCommand
    /// </summary>
    public class DoubleTextBoxValueChangedCommand : ControlCommandBase<DoubleTextBoxValueChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxValueChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

    /// <summary>
    /// DoubleTextBoxValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxValueChangedCommandBehavior<T> : DoubleTextBoxValueChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxValueChangingCommand
    /// <summary>
    /// DoubleTextBoxValueChangingCommand
    /// </summary>
    public class DoubleTextBoxValueChangingCommand : ControlCommandBase<DoubleTextBoxValueChangingCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxValueChangingCommandBehavior : CommandBehaviorBase<DoubleTextBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, ValueChangingEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ValueChanging += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxValueChangingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxValueChangingCommandBehavior<T> : DoubleTextBoxValueChangingCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxMaxValueChangedCommand
    /// <summary>
    /// DoubleTextBoxMaxValueChangedCommand
    /// </summary>
    public class DoubleTextBoxMaxValueChangedCommand : ControlCommandBase<DoubleTextBoxMaxValueChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxMaxValueChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

    /// <summary>
    /// DoubleTextBoxMaxValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxMaxValueChangedCommandBehavior<T> : DoubleTextBoxMaxValueChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxNumberDecimalDigitsChangedCommand
    /// <summary>
    /// DoubleTextBoxNumberDecimalDigitsChangedCommand
    /// </summary>
    public class DoubleTextBoxNumberDecimalDigitsChangedCommand : ControlCommandBase<DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberDecimalDigitsChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior<T> : DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxNumberDecimalSeparatorChangedCommand
    /// <summary>
    /// DoubleTextBoxNumberDecimalSeparatorChangedCommand
    /// </summary>
    public class DoubleTextBoxNumberDecimalSeparatorChangedCommand : ControlCommandBase<DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberDecimalSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior<T> : DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxNumberGroupSizesChangedCommand
    /// <summary>
    /// DoubleTextBoxNumberGroupSizesChangedCommand
    /// </summary>
    public class DoubleTextBoxNumberGroupSizesChangedCommand : ControlCommandBase<DoubleTextBoxNumberGroupSizesChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberGroupSizesChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberGroupSizesChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxNumberGroupSizesChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberGroupSizesChangedCommandBehavior<T> : DoubleTextBoxNumberGroupSizesChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxNumberGroupSeparatorChangedCommand
    /// <summary>
    /// DoubleTextBoxNumberGroupSeparatorChangedCommand
    /// </summary>
    public class DoubleTextBoxNumberGroupSeparatorChangedCommand : ControlCommandBase<DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberGroupSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior<T> : DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxMinimumNumberDecimalDigitsChangedCommand
    /// <summary>
    /// DoubleTextBoxMinimumNumberDecimalDigitsChangedCommand
    /// </summary>
    public class DoubleTextBoxMinimumNumberDecimalDigitsChangedCommand : ControlCommandBase<DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.MinimumNumberDecimalDigitsChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior<T> : DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxMaximumNumberDecimalDigitsChangedCommand
    /// <summary>
    /// DoubleTextBoxMaximumNumberDecimalDigitsChangedCommand
    /// </summary>
    public class DoubleTextBoxMaximumNumberDecimalDigitsChangedCommand : ControlCommandBase<DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.MaximumNumberDecimalDigitsChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior<T> : DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxCultureChangedCommand
    /// <summary>
    /// DoubleTextBoxCultureChangedCommand
    /// </summary>
    public class DoubleTextBoxCultureChangedCommand : ControlCommandBase<DoubleTextBoxCultureChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxCultureChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

    /// <summary>
    /// DoubleTextBoxCultureChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxCultureChangedCommandBehavior<T> : DoubleTextBoxCultureChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxTextSelectionOnFocusChangedCommand
    /// <summary>
    /// DoubleTextBoxTextSelectionOnFocusChangedCommand
    /// </summary>
    public class DoubleTextBoxTextSelectionOnFocusChangedCommand : ControlCommandBase<DoubleTextBoxTextSelectionOnFocusChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxTextSelectionOnFocusChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.TextSelectionOnFocusChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxTextSelectionOnFocusChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxTextSelectionOnFocusChangedCommandBehavior<T> : DoubleTextBoxTextSelectionOnFocusChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxEnterToMoveNextChangedCommand
    /// <summary>
    /// DoubleTextBoxEnterToMoveNextChangedCommand
    /// </summary>
    public class DoubleTextBoxEnterToMoveNextChangedCommand : ControlCommandBase<DoubleTextBoxEnterToMoveNextChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxEnterToMoveNextChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.EnterToMoveNextChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxEnterToMoveNextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxEnterToMoveNextChangedCommandBehavior<T> : DoubleTextBoxEnterToMoveNextChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxIsUndoEnabledChangedCommand
    /// <summary>
    /// DoubleTextBoxIsUndoEnabledChangedCommand
    /// </summary>
    public class DoubleTextBoxIsUndoEnabledChangedCommand : ControlCommandBase<DoubleTextBoxIsUndoEnabledChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxIsUndoEnabledChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.IsUndoEnabledChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxIsUndoEnabledChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxIsUndoEnabledChangedCommandBehavior<T> : DoubleTextBoxIsUndoEnabledChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxIsValueNegativeChangedCommand
    /// <summary>
    /// DoubleTextBoxIsValueNegativeChangedCommand
    /// </summary>
    public class DoubleTextBoxIsValueNegativeChangedCommand : ControlCommandBase<DoubleTextBoxIsValueNegativeChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxIsValueNegativeChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.IsValueNegativeChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxIsValueNegativeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxIsValueNegativeChangedCommandBehavior<T> : DoubleTextBoxIsValueNegativeChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxNegativeForegroundChangedCommand
    /// <summary>
    /// DoubleTextBoxNegativeForegroundChangedCommand
    /// </summary>
    public class DoubleTextBoxNegativeForegroundChangedCommand : ControlCommandBase<DoubleTextBoxNegativeForegroundChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNegativeForegroundChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

    /// <summary>
    /// DoubleTextBoxNegativeForegroundChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNegativeForegroundChangedCommandBehavior<T> : DoubleTextBoxNegativeForegroundChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxNumberFormatChangedCommand
    /// <summary>
    /// DoubleTextBoxNumberFormatChangedCommand
    /// </summary>
    public class DoubleTextBoxNumberFormatChangedCommand : ControlCommandBase<DoubleTextBoxNumberFormatChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberFormatChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberFormatChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxNumberFormatChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberFormatChangedCommandBehavior<T> : DoubleTextBoxNumberFormatChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxWaterMarkTemplateChangedCommand
    /// <summary>
    /// DoubleTextBoxWaterMarkTemplateChangedCommand
    /// </summary>
    public class DoubleTextBoxWaterMarkTemplateChangedCommand : ControlCommandBase<DoubleTextBoxWaterMarkTemplateChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxWaterMarkTemplateChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.WaterMarkTemplateChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxWaterMarkTemplateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxWaterMarkTemplateChangedCommandBehavior<T> : DoubleTextBoxWaterMarkTemplateChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxWaterMarkTextChangedCommand
    /// <summary>
    /// DoubleTextBoxWaterMarkTextChangedCommand
    /// </summary>
    public class DoubleTextBoxWaterMarkTextChangedCommand : ControlCommandBase<DoubleTextBoxWaterMarkTextChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxWaterMarkTextChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.WaterMarkTextChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxWaterMarkTextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxWaterMarkTextChangedCommandBehavior<T> : DoubleTextBoxWaterMarkTextChangedCommandBehavior
    { }
    #endregion

    #region DoubleTextBoxSelectionChangedCommand
    /// <summary>
    /// DoubleTextBoxSelectionChangedCommand
    /// </summary>
    public class DoubleTextBoxSelectionChangedCommand : ControlCommandBase<DoubleTextBoxSelectionChangedCommandBehavior, DoubleTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxSelectionChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DoubleTextBoxSelectionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxSelectionChangedCommandBehavior<T> : DoubleTextBoxSelectionChangedCommandBehavior
    { }
    #endregion








}


