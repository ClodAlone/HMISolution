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

    #region IntegerTextBoxMinValueChangedCommand
    /// <summary>
    /// IntegerTextBoxMinValueChangedCommand
    /// </summary>
    public class IntegerTextBoxMinValueChangedCommand : ControlCommandBase<IntegerTextBoxMinValueChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxMinValueChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxMinValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxMinValueChangedCommandBehavior<T> : IntegerTextBoxMinValueChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxValueChangedCommand
    /// <summary>
    /// IntegerTextBoxValueChangedCommand
    /// </summary>
    public class IntegerTextBoxValueChangedCommand : ControlCommandBase<IntegerTextBoxValueChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxValueChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxValueChangedCommandBehavior<T> : IntegerTextBoxValueChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxMaxValueChangedCommand
    /// <summary>
    /// IntegerTextBoxMaxValueChangedCommand
    /// </summary>
    public class IntegerTextBoxMaxValueChangedCommand : ControlCommandBase<IntegerTextBoxMaxValueChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxMaxValueChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxMaxValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxMaxValueChangedCommandBehavior<T> : IntegerTextBoxMaxValueChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxNumberGroupSizesChangedCommand
    /// <summary>
    /// IntegerTextBoxNumberGroupSizesChangedCommand
    /// </summary>
    public class IntegerTextBoxNumberGroupSizesChangedCommand : ControlCommandBase<IntegerTextBoxNumberGroupSizesChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxNumberGroupSizesChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxNumberGroupSizesChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxNumberGroupSizesChangedCommandBehavior<T> : IntegerTextBoxNumberGroupSizesChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxNumberGroupSeparatorChangedCommand
    /// <summary>
    /// IntegerTextBoxNumberGroupSeparatorChangedCommand
    /// </summary>
    public class IntegerTextBoxNumberGroupSeparatorChangedCommand : ControlCommandBase<IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior<T> : IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxCultureChangedCommand
    /// <summary>
    /// IntegerTextBoxCultureChangedCommand
    /// </summary>
    public class IntegerTextBoxCultureChangedCommand : ControlCommandBase<IntegerTextBoxCultureChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxCultureChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxCultureChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxCultureChangedCommandBehavior<T> : IntegerTextBoxCultureChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxTextSelectionOnFocusChangedCommand
    /// <summary>
    /// IntegerTextBoxTextSelectionOnFocusChangedCommand
    /// </summary>
    public class IntegerTextBoxTextSelectionOnFocusChangedCommand : ControlCommandBase<IntegerTextBoxTextSelectionOnFocusChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxTextSelectionOnFocusChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxTextSelectionOnFocusChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxTextSelectionOnFocusChangedCommandBehavior<T> : IntegerTextBoxTextSelectionOnFocusChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxEnterToMoveNextChangedCommand
    /// <summary>
    /// IntegerTextBoxEnterToMoveNextChangedCommand
    /// </summary>
    public class IntegerTextBoxEnterToMoveNextChangedCommand : ControlCommandBase<IntegerTextBoxEnterToMoveNextChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxEnterToMoveNextChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxEnterToMoveNextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxEnterToMoveNextChangedCommandBehavior<T> : IntegerTextBoxEnterToMoveNextChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxIsUndoEnabledChangedCommand
    /// <summary>
    /// IntegerTextBoxIsUndoEnabledChangedCommand
    /// </summary>
    public class IntegerTextBoxIsUndoEnabledChangedCommand : ControlCommandBase<IntegerTextBoxIsUndoEnabledChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxIsUndoEnabledChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxIsUndoEnabledChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxIsUndoEnabledChangedCommandBehavior<T> : IntegerTextBoxIsUndoEnabledChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxIsValueNegativeChangedCommand
    /// <summary>
    /// IntegerTextBoxIsValueNegativeChangedCommand
    /// </summary>
    public class IntegerTextBoxIsValueNegativeChangedCommand : ControlCommandBase<IntegerTextBoxIsValueNegativeChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxIsValueNegativeChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxIsValueNegativeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxIsValueNegativeChangedCommandBehavior<T> : IntegerTextBoxIsValueNegativeChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxNegativeForegroundChangedCommand
    /// <summary>
    /// IntegerTextBoxNegativeForegroundChangedCommand
    /// </summary>
    public class IntegerTextBoxNegativeForegroundChangedCommand : ControlCommandBase<IntegerTextBoxNegativeForegroundChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxNegativeForegroundChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxNegativeForegroundChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxNegativeForegroundChangedCommandBehavior<T> : IntegerTextBoxNegativeForegroundChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxNumberFormatChangedCommand
    /// <summary>
    /// IntegerTextBoxNumberFormatChangedCommand
    /// </summary>
    public class IntegerTextBoxNumberFormatChangedCommand : ControlCommandBase<IntegerTextBoxNumberFormatChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxNumberFormatChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxNumberFormatChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxNumberFormatChangedCommandBehavior<T> : IntegerTextBoxNumberFormatChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxWaterMarkTemplateChangedCommand
    /// <summary>
    /// IntegerTextBoxWaterMarkTemplateChangedCommand
    /// </summary>
    public class IntegerTextBoxWaterMarkTemplateChangedCommand : ControlCommandBase<IntegerTextBoxWaterMarkTemplateChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxWaterMarkTemplateChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxWaterMarkTemplateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxWaterMarkTemplateChangedCommandBehavior<T> : IntegerTextBoxWaterMarkTemplateChangedCommandBehavior
    { }
    #endregion


    #region IntegerTextBoxWaterMarkTextChangedCommand
    /// <summary>
    /// IntegerTextBoxWaterMarkTextChangedCommand
    /// </summary>
    public class IntegerTextBoxWaterMarkTextChangedCommand : ControlCommandBase<IntegerTextBoxWaterMarkTextChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxWaterMarkTextChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxWaterMarkTextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxWaterMarkTextChangedCommandBehavior<T> : IntegerTextBoxWaterMarkTextChangedCommandBehavior
    { }
    #endregion

    #region IntegerTextBoxSelectionChangedCommand
    /// <summary>
    /// IntegerTextBoxSelectionChangedCommand
    /// </summary>
    public class IntegerTextBoxSelectionChangedCommand : ControlCommandBase<IntegerTextBoxSelectionChangedCommandBehavior, IntegerTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxSelectionChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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
    /// IntegerTextBoxSelectionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxSelectionChangedCommandBehavior<T> : IntegerTextBoxSelectionChangedCommandBehavior
    { }
    #endregion

   









}


