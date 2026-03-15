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

    #region MaskedTextBoxMaskChangedCommand
    /// <summary>
    /// MaskedTextBoxMaskChangedCommand
    /// </summary>
    public class MaskedTextBoxMaskChangedCommand : ControlCommandBase<MaskedTextBoxMaskChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxMaskChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.MaskChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxMaskChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxMaskChangedCommandBehavior<T> : MaskedTextBoxMaskChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxValidationStringChangedCommand
    /// <summary>
    /// MaskedTextBoxValidationStringChangedCommand
    /// </summary>
    public class MaskedTextBoxValidationStringChangedCommand : ControlCommandBase<MaskedTextBoxValidationStringChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxValidationStringChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.ValidationStringChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxValidationStringChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxValidationStringChangedCommandBehavior<T> : MaskedTextBoxValidationStringChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxMaskCompletedChangedCommand
    /// <summary>
    /// MaskedTextBoxMaskCompletedChangedCommand
    /// </summary>
    public class MaskedTextBoxMaskCompletedChangedCommand : ControlCommandBase<MaskedTextBoxMaskCompletedChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxMaskCompletedChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.MaskCompletedChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxMaskCompletedChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxMaskCompletedChangedCommandBehavior<T> : MaskedTextBoxMaskCompletedChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxInvalidValueBehaviorChangedCommand
    /// <summary>
    /// MaskedTextBoxInvalidValueBehaviorChangedCommand
    /// </summary>
    public class MaskedTextBoxInvalidValueBehaviorChangedCommand : ControlCommandBase<MaskedTextBoxInvalidValueBehaviorChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxInvalidValueBehaviorChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.InvalidValueBehaviorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxInvalidValueBehaviorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxInvalidValueBehaviorChangedCommandBehavior<T> : MaskedTextBoxInvalidValueBehaviorChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxDateSeparatorChangedCommand
    /// <summary>
    /// MaskedTextBoxDateSeparatorChangedCommand
    /// </summary>
    public class MaskedTextBoxDateSeparatorChangedCommand : ControlCommandBase<MaskedTextBoxDateSeparatorChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxDateSeparatorChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.DateSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxDateSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxDateSeparatorChangedCommandBehavior<T> : MaskedTextBoxDateSeparatorChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxTimeSeparatorChangedCommand
    /// <summary>
    /// MaskedTextBoxTimeSeparatorChangedCommand
    /// </summary>
    public class MaskedTextBoxTimeSeparatorChangedCommand : ControlCommandBase<MaskedTextBoxTimeSeparatorChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxTimeSeparatorChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.TimeSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxTimeSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxTimeSeparatorChangedCommandBehavior<T> : MaskedTextBoxTimeSeparatorChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxDecimalSeparatorChangedCommand
    /// <summary>
    /// MaskedTextBoxDecimalSeparatorChangedCommand
    /// </summary>
    public class MaskedTextBoxDecimalSeparatorChangedCommand : ControlCommandBase<MaskedTextBoxDecimalSeparatorChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxDecimalSeparatorChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.DecimalSeparatorChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxDecimalSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxDecimalSeparatorChangedCommandBehavior<T> : MaskedTextBoxDecimalSeparatorChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxNumberGroupSeparatorChangedCommand
    /// <summary>
    /// MaskedTextBoxNumberGroupSeparatorChangedCommand
    /// </summary>
    public class MaskedTextBoxNumberGroupSeparatorChangedCommand : ControlCommandBase<MaskedTextBoxNumberGroupSeparatorChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxNumberGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
    /// MaskedTextBoxNumberGroupSeparatorChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxNumberGroupSeparatorChangedCommandBehavior<T> : MaskedTextBoxNumberGroupSeparatorChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxCurrencySymbolChangedCommand
    /// <summary>
    /// MaskedTextBoxCurrencySymbolChangedCommand
    /// </summary>
    public class MaskedTextBoxCurrencySymbolChangedCommand : ControlCommandBase<MaskedTextBoxCurrencySymbolChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxCurrencySymbolChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
    /// MaskedTextBoxCurrencySymbolChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxCurrencySymbolChangedCommandBehavior<T> : MaskedTextBoxCurrencySymbolChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxPromptCharChangedCommand
    /// <summary>
    /// MaskedTextBoxPromptCharChangedCommand
    /// </summary>
    public class MaskedTextBoxPromptCharChangedCommand : ControlCommandBase<MaskedTextBoxPromptCharChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxPromptCharChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.PromptCharChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxPromptCharChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxPromptCharChangedCommandBehavior<T> : MaskedTextBoxPromptCharChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxWatermarkTextModeChangedCommand
    /// <summary>
    /// MaskedTextBoxWatermarkTextModeChangedCommand
    /// </summary>
    public class MaskedTextBoxWatermarkTextModeChangedCommand : ControlCommandBase<MaskedTextBoxWatermarkTextModeChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxWatermarkTextModeChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.WatermarkTextModeChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxWatermarkTextModeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxWatermarkTextModeChangedCommandBehavior<T> : MaskedTextBoxWatermarkTextModeChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxWatermarkTextIsVisibleChangedCommand
    /// <summary>
    /// MaskedTextBoxWatermarkTextIsVisibleChangedCommand
    /// </summary>
    public class MaskedTextBoxWatermarkTextIsVisibleChangedCommand : ControlCommandBase<MaskedTextBoxWatermarkTextIsVisibleChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxWatermarkTextIsVisibleChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.WatermarkTextIsVisibleChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxWatermarkTextIsVisibleChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxWatermarkTextIsVisibleChangedCommandBehavior<T> : MaskedTextBoxWatermarkTextIsVisibleChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxValidatingCommand
    /// <summary>
    /// MaskedTextBoxValidatingCommand
    /// </summary>
    public class MaskedTextBoxValidatingCommand : ControlCommandBase<MaskedTextBoxValidatingCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxValidatingCommandBehavior : CommandBehaviorBase<MaskedTextBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Validating += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxValidatingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxValidatingCommandBehavior<T> : MaskedTextBoxValidatingCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxValidatedCommand
    /// <summary>
    /// MaskedTextBoxValidatedCommand
    /// </summary>
    public class MaskedTextBoxValidatedCommand : ControlCommandBase<MaskedTextBoxValidatedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxValidatedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.Validated += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxValidatedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxValidatedCommandBehavior<T> : MaskedTextBoxValidatedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxStringValidationCompletedCommand
    /// <summary>
    /// MaskedTextBoxStringValidationCompletedCommand
    /// </summary>
    public class MaskedTextBoxStringValidationCompletedCommand : ControlCommandBase<MaskedTextBoxStringValidationCompletedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxStringValidationCompletedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, StringValidationEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.StringValidationCompleted += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxStringValidationCompletedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxStringValidationCompletedCommandBehavior<T> : MaskedTextBoxStringValidationCompletedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxTextSelectionOnFocusChangedCommand
    /// <summary>
    /// MaskedTextBoxTextSelectionOnFocusChangedCommand
    /// </summary>
    public class MaskedTextBoxTextSelectionOnFocusChangedCommand : ControlCommandBase<MaskedTextBoxTextSelectionOnFocusChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxTextSelectionOnFocusChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
    /// MaskedTextBoxTextSelectionOnFocusChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxTextSelectionOnFocusChangedCommandBehavior<T> : MaskedTextBoxTextSelectionOnFocusChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxEnterToMoveNextChangedCommand
    /// <summary>
    /// MaskedTextBoxEnterToMoveNextChangedCommand
    /// </summary>
    public class MaskedTextBoxEnterToMoveNextChangedCommand : ControlCommandBase<MaskedTextBoxEnterToMoveNextChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxEnterToMoveNextChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
    /// MaskedTextBoxEnterToMoveNextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxEnterToMoveNextChangedCommandBehavior<T> : MaskedTextBoxEnterToMoveNextChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxMinLengthChangedCommand
    /// <summary>
    /// MaskedTextBoxMinLengthChangedCommand
    /// </summary>
    public class MaskedTextBoxMinLengthChangedCommand : ControlCommandBase<MaskedTextBoxMinLengthChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxMinLengthChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.MinLengthChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxMinLengthChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxMinLengthChangedCommandBehavior<T> : MaskedTextBoxMinLengthChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxWatermarkTemplateChangedCommand
    /// <summary>
    /// MaskedTextBoxWatermarkTemplateChangedCommand
    /// </summary>
    public class MaskedTextBoxWatermarkTemplateChangedCommand : ControlCommandBase<MaskedTextBoxWatermarkTemplateChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxWatermarkTemplateChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.WatermarkTemplateChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxWatermarkTemplateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxWatermarkTemplateChangedCommandBehavior<T> : MaskedTextBoxWatermarkTemplateChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxWatermarkTextChangedCommand
    /// <summary>
    /// MaskedTextBoxWatermarkTextChangedCommand
    /// </summary>
    public class MaskedTextBoxWatermarkTextChangedCommand : ControlCommandBase<MaskedTextBoxWatermarkTextChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxWatermarkTextChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
            TargetObject.WatermarkTextChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// MaskedTextBoxWatermarkTextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxWatermarkTextChangedCommandBehavior<T> : MaskedTextBoxWatermarkTextChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxValueChangedCommand
    /// <summary>
    /// MaskedTextBoxValueChangedCommand
    /// </summary>
    public class MaskedTextBoxValueChangedCommand : ControlCommandBase<MaskedTextBoxValueChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxValueChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
    /// MaskedTextBoxValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxValueChangedCommandBehavior<T> : MaskedTextBoxValueChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxSelectionChangedCommand
    /// <summary>
    /// MaskedTextBoxSelectionChangedCommand
    /// </summary>
    public class MaskedTextBoxSelectionChangedCommand : ControlCommandBase<MaskedTextBoxSelectionChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxSelectionChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
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
    /// MaskedTextBoxSelectionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxSelectionChangedCommandBehavior<T> : MaskedTextBoxSelectionChangedCommandBehavior
    { }
    #endregion

    #region MaskedTextBoxTextChangedCommand
    /// <summary>
    /// MaskedTextBoxTextChangedCommand
    /// </summary>
    public class MaskedTextBoxTextChangedCommand : ControlCommandBase<MaskedTextBoxTextChangedCommandBehavior, MaskedTextBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class MaskedTextBoxTextChangedCommandBehavior : CommandBehaviorBase<MaskedTextBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TextChangedEventArgs e)
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

    /// <summary>
    /// MaskedTextBoxTextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxTextChangedCommandBehavior<T> : MaskedTextBoxTextChangedCommandBehavior
    { }
    #endregion











}


