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

    #region PercentTextBoxPercentageSymbolChangedCommand
    /// <summary>
    /// PercentTextBoxPercentageSymbolChangedCommand
    /// </summary>
    public class PercentTextBoxPercentageSymbolChangedCommand : ControlCommandBase<PercentTextBoxPercentageSymbolChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentageSymbolChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentageSymbolChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// PercentTextBoxPercentageSymbolChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentageSymbolChangedCommandBehavior<T> : PercentTextBoxPercentageSymbolChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxPercentEditModeChangedCommand
    /// <summary>
    /// PercentTextBoxPercentEditModeChangedCommand
    /// </summary>
    public class PercentTextBoxPercentEditModeChangedCommand : ControlCommandBase<PercentTextBoxPercentEditModeChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentEditModeChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentEditModeChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// PercentTextBoxPercentEditModeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentEditModeChangedCommandBehavior<T> : PercentTextBoxPercentEditModeChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxPercentValueChangedCommand
    /// <summary>
    /// PercentTextBoxPercentValueChangedCommand
    /// </summary>
    public class PercentTextBoxPercentValueChangedCommand : ControlCommandBase<PercentTextBoxPercentValueChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentValueChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentValueChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// PercentTextBoxPercentValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentValueChangedCommandBehavior<T> : PercentTextBoxPercentValueChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxMinValueChangedCommand
    /// <summary>
    /// PercentTextBoxMinValueChangedCommand
    /// </summary>
    public class PercentTextBoxMinValueChangedCommand : ControlCommandBase<PercentTextBoxMinValueChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxMinValueChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxMinValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxMinValueChangedCommandBehavior<T> : PercentTextBoxMinValueChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxMaxValueChangedCommand
    /// <summary>
    /// PercentTextBoxMaxValueChangedCommand
    /// </summary>
    public class PercentTextBoxMaxValueChangedCommand : ControlCommandBase<PercentTextBoxMaxValueChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxMaxValueChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxMaxValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxMaxValueChangedCommandBehavior<T> : PercentTextBoxMaxValueChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxPercentDecimalDigitsChangedCommand
    /// <summary>
    /// PercentTextBoxPercentDecimalDigitsChangedCommand
    /// </summary>
    public class PercentTextBoxPercentDecimalDigitsChangedCommand : ControlCommandBase<PercentTextBoxPercentDecimalDigitsChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentDecimalDigitsChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// PercentTextBoxPercentDecimalDigitsChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentDecimalDigitsChangedCommandBehavior<T> : PercentTextBoxPercentDecimalDigitsChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxPercentDecimalSeparatorChangedCommand
    /// <summary>
    /// PercentTextBoxPercentDecimalSeparatorChangedCommand
    /// </summary>
    public class PercentTextBoxPercentDecimalSeparatorChangedCommand : ControlCommandBase<PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentDecimalSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior<T> : PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxPercentGroupSeparatorChangedCommand
    /// <summary>
    /// PercentTextBoxPercentGroupSeparatorChangedCommand
    /// </summary>
    public class PercentTextBoxPercentGroupSeparatorChangedCommand : ControlCommandBase<PercentTextBoxPercentGroupSeparatorChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentGroupSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// PercentTextBoxPercentGroupSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentGroupSeparatorChangedCommandBehavior<T> : PercentTextBoxPercentGroupSeparatorChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxPercentGroupSizesChangedCommand
    /// <summary>
    /// PercentTextBoxPercentGroupSizesChangedCommand
    /// </summary>
    public class PercentTextBoxPercentGroupSizesChangedCommand : ControlCommandBase<PercentTextBoxPercentGroupSizesChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentGroupSizesChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentGroupSizesChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// PercentTextBoxPercentGroupSizesChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentGroupSizesChangedCommandBehavior<T> : PercentTextBoxPercentGroupSizesChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxValidationOnLostFocusChangedCommand
    /// <summary>
    /// PercentTextBoxValidationOnLostFocusChangedCommand
    /// </summary>
    public class PercentTextBoxValidationOnLostFocusChangedCommand : ControlCommandBase<PercentTextBoxValidationOnLostFocusChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxValidationOnLostFocusChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.ValidationOnLostFocusChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// PercentTextBoxValidationOnLostFocusChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxValidationOnLostFocusChangedCommandBehavior<T> : PercentTextBoxValidationOnLostFocusChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxCultureChangedCommand
    /// <summary>
    /// PercentTextBoxCultureChangedCommand
    /// </summary>
    public class PercentTextBoxCultureChangedCommand : ControlCommandBase<PercentTextBoxCultureChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxCultureChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxCultureChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxCultureChangedCommandBehavior<T> : PercentTextBoxCultureChangedCommandBehavior
    { }
    #endregion


    #region PercentTextBoxTextSelectionOnFocusChangedCommand
    /// <summary>
    /// PercentTextBoxTextSelectionOnFocusChangedCommand
    /// </summary>
    public class PercentTextBoxTextSelectionOnFocusChangedCommand : ControlCommandBase<PercentTextBoxTextSelectionOnFocusChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxTextSelectionOnFocusChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxTextSelectionOnFocusChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxTextSelectionOnFocusChangedCommandBehavior<T> : PercentTextBoxTextSelectionOnFocusChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxEnterToMoveNextChangedCommand
    /// <summary>
    /// PercentTextBoxEnterToMoveNextChangedCommand
    /// </summary>
    public class PercentTextBoxEnterToMoveNextChangedCommand : ControlCommandBase<PercentTextBoxEnterToMoveNextChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxEnterToMoveNextChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxEnterToMoveNextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxEnterToMoveNextChangedCommandBehavior<T> : PercentTextBoxEnterToMoveNextChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxIsUndoEnabledChangedCommand
    /// <summary>
    /// PercentTextBoxIsUndoEnabledChangedCommand
    /// </summary>
    public class PercentTextBoxIsUndoEnabledChangedCommand : ControlCommandBase<PercentTextBoxIsUndoEnabledChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxIsUndoEnabledChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxIsUndoEnabledChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxIsUndoEnabledChangedCommandBehavior<T> : PercentTextBoxIsUndoEnabledChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxIsValueNegativeChangedCommand
    /// <summary>
    /// PercentTextBoxIsValueNegativeChangedCommand
    /// </summary>
    public class PercentTextBoxIsValueNegativeChangedCommand : ControlCommandBase<PercentTextBoxIsValueNegativeChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxIsValueNegativeChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxIsValueNegativeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxIsValueNegativeChangedCommandBehavior<T> : PercentTextBoxIsValueNegativeChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxNegativeForegroundChangedCommand
    /// <summary>
    /// PercentTextBoxNegativeForegroundChangedCommand
    /// </summary>
    public class PercentTextBoxNegativeForegroundChangedCommand : ControlCommandBase<PercentTextBoxNegativeForegroundChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxNegativeForegroundChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxNegativeForegroundChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxNegativeForegroundChangedCommandBehavior<T> : PercentTextBoxNegativeForegroundChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxNumberFormatChangedCommand
    /// <summary>
    /// PercentTextBoxNumberFormatChangedCommand
    /// </summary>
    public class PercentTextBoxNumberFormatChangedCommand : ControlCommandBase<PercentTextBoxNumberFormatChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxNumberFormatChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxNumberFormatChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxNumberFormatChangedCommandBehavior<T> : PercentTextBoxNumberFormatChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxWaterMarkTemplateChangedCommand
    /// <summary>
    /// PercentTextBoxWaterMarkTemplateChangedCommand
    /// </summary>
    public class PercentTextBoxWaterMarkTemplateChangedCommand : ControlCommandBase<PercentTextBoxWaterMarkTemplateChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxWaterMarkTemplateChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxWaterMarkTemplateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxWaterMarkTemplateChangedCommandBehavior<T> : PercentTextBoxWaterMarkTemplateChangedCommandBehavior
    { }
    #endregion


    #region PercentTextBoxWaterMarkTextChangedCommand
    /// <summary>
    /// PercentTextBoxWaterMarkTextChangedCommand
    /// </summary>
    public class PercentTextBoxWaterMarkTextChangedCommand : ControlCommandBase<PercentTextBoxWaterMarkTextChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxWaterMarkTextChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxWaterMarkTextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxWaterMarkTextChangedCommandBehavior<T> : PercentTextBoxWaterMarkTextChangedCommandBehavior
    { }
    #endregion

    #region PercentTextBoxSelectionChangedCommand
    /// <summary>
    /// PercentTextBoxSelectionChangedCommand
    /// </summary>
    public class PercentTextBoxSelectionChangedCommand : ControlCommandBase<PercentTextBoxSelectionChangedCommandBehavior, PercentTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxSelectionChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
    /// PercentTextBoxSelectionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxSelectionChangedCommandBehavior<T> : PercentTextBoxSelectionChangedCommandBehavior
    { }
    #endregion






}


