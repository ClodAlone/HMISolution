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

    #region CurrencyTextBoxMinValueChangedCommand
    /// <summary>
    /// CurrencyTextBoxMinValueChangedCommand
    /// </summary>
    public class CurrencyTextBoxMinValueChangedCommand : ControlCommandBase<CurrencyTextBoxMinValueChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxMinValueChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxMinValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxMinValueChangedCommandBehavior<T> : CurrencyTextBoxMinValueChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCurrencySymbolPositionChangedCommand
    /// <summary>
    /// CurrencyTextBoxCurrencySymbolPositionChangedCommand
    /// </summary>
    public class CurrencyTextBoxCurrencySymbolPositionChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencySymbolPositionChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior<T> : CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxValueChangedCommand
    /// <summary>
    /// CurrencyTextBoxValueChangedCommand
    /// </summary>
    public class CurrencyTextBoxValueChangedCommand : ControlCommandBase<CurrencyTextBoxValueChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxValueChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxValueChangedCommandBehavior<T> : CurrencyTextBoxValueChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxMaxValueChangedCommand
    /// <summary>
    /// CurrencyTextBoxMaxValueChangedCommand
    /// </summary>
    public class CurrencyTextBoxMaxValueChangedCommand : ControlCommandBase<CurrencyTextBoxMaxValueChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxMaxValueChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxMaxValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxMaxValueChangedCommandBehavior<T> : CurrencyTextBoxMaxValueChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCurrencyDecimalDigitsChangedCommand
    /// <summary>
    /// CurrencyTextBoxCurrencyDecimalDigitsChangedCommand
    /// </summary>
    public class CurrencyTextBoxCurrencyDecimalDigitsChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyDecimalDigitsChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior<T> : CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCurrencyDecimalSeparatorChangedCommand
    /// <summary>
    /// CurrencyTextBoxCurrencyDecimalSeparatorChangedCommand
    /// </summary>
    public class CurrencyTextBoxCurrencyDecimalSeparatorChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyDecimalSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior<T> : CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCurrencyGroupSeparatorChangedCommand
    /// <summary>
    /// CurrencyTextBoxCurrencyGroupSeparatorChangedCommand
    /// </summary>
    public class CurrencyTextBoxCurrencyGroupSeparatorChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyGroupSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior<T> : CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCurrencyGroupSizesChangedCommand
    /// <summary>
    /// CurrencyTextBoxCurrencyGroupSizesChangedCommand
    /// </summary>
    public class CurrencyTextBoxCurrencyGroupSizesChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyGroupSizesChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior<T> : CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCurrencyNegativePatternChangedCommand
    /// <summary>
    /// CurrencyTextBoxCurrencyNegativePatternChangedCommand
    /// </summary>
    public class CurrencyTextBoxCurrencyNegativePatternChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyNegativePatternChanged += OnEventRaised;
        }
    }

    
    /// <summary>
    /// CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior<T> : CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCurrencyPositivePatternChangedCommand
     
    /// <summary>
    /// CurrencyTextBoxCurrencyPositivePatternChangedCommand
    /// </summary>
    public class CurrencyTextBoxCurrencyPositivePatternChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyPositivePatternChanged += OnEventRaised;
        }
    }

    
    /// <summary>
    /// CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior<T> : CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCurrencySymbolChangedCommand
    
    /// <summary>
    /// CurrencyTextBoxCurrencySymbolChangedCommand
    /// </summary>
    public class CurrencyTextBoxCurrencySymbolChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencySymbolChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencySymbolChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencySymbolChanged += OnEventRaised;
        }
    }

    
    /// <summary>
    /// CurrencyTextBoxCurrencySymbolChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencySymbolChangedCommandBehavior<T> : CurrencyTextBoxCurrencySymbolChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxCultureChangedCommand
    /// <summary>
    /// CurrencyTextBoxCultureChangedCommand
    /// </summary>
    public class CurrencyTextBoxCultureChangedCommand : ControlCommandBase<CurrencyTextBoxCultureChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCultureChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxCultureChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCultureChangedCommandBehavior<T> : CurrencyTextBoxCultureChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxTextSelectionOnFocusChangedCommand
    /// <summary>
    /// CurrencyTextBoxTextSelectionOnFocusChangedCommand
    /// </summary>
    public class CurrencyTextBoxTextSelectionOnFocusChangedCommand : ControlCommandBase<CurrencyTextBoxTextSelectionOnFocusChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxTextSelectionOnFocusChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxTextSelectionOnFocusChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxTextSelectionOnFocusChangedCommandBehavior<T> : CurrencyTextBoxTextSelectionOnFocusChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxEnterToMoveNextChangedCommand
    /// <summary>
    /// CurrencyTextBoxEnterToMoveNextChangedCommand
    /// </summary>
    public class CurrencyTextBoxEnterToMoveNextChangedCommand : ControlCommandBase<CurrencyTextBoxEnterToMoveNextChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxEnterToMoveNextChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxEnterToMoveNextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxEnterToMoveNextChangedCommandBehavior<T> : CurrencyTextBoxEnterToMoveNextChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxIsUndoEnabledChangedCommand
    /// <summary>
    /// CurrencyTextBoxIsUndoEnabledChangedCommand
    /// </summary>
    public class CurrencyTextBoxIsUndoEnabledChangedCommand : ControlCommandBase<CurrencyTextBoxIsUndoEnabledChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxIsUndoEnabledChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxIsUndoEnabledChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxIsUndoEnabledChangedCommandBehavior<T> : CurrencyTextBoxIsUndoEnabledChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxIsValueNegativeChangedCommand
    /// <summary>
    /// CurrencyTextBoxIsValueNegativeChangedCommand
    /// </summary>
    public class CurrencyTextBoxIsValueNegativeChangedCommand : ControlCommandBase<CurrencyTextBoxIsValueNegativeChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxIsValueNegativeChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxIsValueNegativeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxIsValueNegativeChangedCommandBehavior<T> : CurrencyTextBoxIsValueNegativeChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxNegativeForegroundChangedCommand
    /// <summary>
    /// CurrencyTextBoxNegativeForegroundChangedCommand
    /// </summary>
    public class CurrencyTextBoxNegativeForegroundChangedCommand : ControlCommandBase<CurrencyTextBoxNegativeForegroundChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxNegativeForegroundChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxNegativeForegroundChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxNegativeForegroundChangedCommandBehavior<T> : CurrencyTextBoxNegativeForegroundChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxNumberFormatChangedCommand
    /// <summary>
    /// CurrencyTextBoxNumberFormatChangedCommand
    /// </summary>
    public class CurrencyTextBoxNumberFormatChangedCommand : ControlCommandBase<CurrencyTextBoxNumberFormatChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxNumberFormatChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxNumberFormatChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxNumberFormatChangedCommandBehavior<T> : CurrencyTextBoxNumberFormatChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxWaterMarkTemplateChangedCommand
    /// <summary>
    /// CurrencyTextBoxWaterMarkTemplateChangedCommand
    /// </summary>
    public class CurrencyTextBoxWaterMarkTemplateChangedCommand : ControlCommandBase<CurrencyTextBoxWaterMarkTemplateChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxWaterMarkTemplateChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxWaterMarkTemplateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxWaterMarkTemplateChangedCommandBehavior<T> : CurrencyTextBoxWaterMarkTemplateChangedCommandBehavior
    { }
    #endregion


    #region CurrencyTextBoxWaterMarkTextChangedCommand
    /// <summary>
    /// CurrencyTextBoxWaterMarkTextChangedCommand
    /// </summary>
    public class CurrencyTextBoxWaterMarkTextChangedCommand : ControlCommandBase<CurrencyTextBoxWaterMarkTextChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxWaterMarkTextChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxWaterMarkTextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxWaterMarkTextChangedCommandBehavior<T> : CurrencyTextBoxWaterMarkTextChangedCommandBehavior
    { }
    #endregion

    #region CurrencyTextBoxSelectionChangedCommand
    /// <summary>
    /// CurrencyTextBoxSelectionChangedCommand
    /// </summary>
    public class CurrencyTextBoxSelectionChangedCommand : ControlCommandBase<CurrencyTextBoxSelectionChangedCommandBehavior, CurrencyTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxSelectionChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
    /// CurrencyTextBoxSelectionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxSelectionChangedCommandBehavior<T> : CurrencyTextBoxSelectionChangedCommandBehavior
    { }
    #endregion








}


