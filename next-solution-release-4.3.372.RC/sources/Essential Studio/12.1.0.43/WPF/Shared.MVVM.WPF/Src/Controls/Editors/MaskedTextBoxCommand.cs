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

	#region MaskedTextBoxMaskChangedCommand
	// MaskedTextBoxMaskChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxMaskChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxMaskChangedCommandBehavior<T> : MaskedTextBoxMaskChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxValidationStringChangedCommand
	// MaskedTextBoxValidationStringChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxValidationStringChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxValidationStringChangedCommandBehavior<T> : MaskedTextBoxValidationStringChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxMaskCompletedChangedCommand
	// MaskedTextBoxMaskCompletedChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxMaskCompletedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxMaskCompletedChangedCommandBehavior<T> : MaskedTextBoxMaskCompletedChangedCommandBehavior
    { }
	#endregion

	
	#region MaskedTextBoxDateSeparatorChangedCommand
	// MaskedTextBoxDateSeparatorChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxDateSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxDateSeparatorChangedCommandBehavior<T> : MaskedTextBoxDateSeparatorChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxTimeSeparatorChangedCommand
	// MaskedTextBoxTimeSeparatorChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxTimeSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxTimeSeparatorChangedCommandBehavior<T> : MaskedTextBoxTimeSeparatorChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxDecimalSeparatorChangedCommand
	// MaskedTextBoxDecimalSeparatorChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxDecimalSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxDecimalSeparatorChangedCommandBehavior<T> : MaskedTextBoxDecimalSeparatorChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxNumberGroupSeparatorChangedCommand
	// MaskedTextBoxNumberGroupSeparatorChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxNumberGroupSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxNumberGroupSeparatorChangedCommandBehavior<T> : MaskedTextBoxNumberGroupSeparatorChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxCurrencySymbolChangedCommand
	// MaskedTextBoxCurrencySymbolChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxCurrencySymbolChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxCurrencySymbolChangedCommandBehavior<T> : MaskedTextBoxCurrencySymbolChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxPromptCharChangedCommand
	// MaskedTextBoxPromptCharChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxPromptCharChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxPromptCharChangedCommandBehavior<T> : MaskedTextBoxPromptCharChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxWatermarkTextModeChangedCommand
	// MaskedTextBoxWatermarkTextModeChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxWatermarkTextModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxWatermarkTextModeChangedCommandBehavior<T> : MaskedTextBoxWatermarkTextModeChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxWatermarkTextIsVisibleChangedCommand
	// MaskedTextBoxWatermarkTextIsVisibleChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxWatermarkTextIsVisibleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxWatermarkTextIsVisibleChangedCommandBehavior<T> : MaskedTextBoxWatermarkTextIsVisibleChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxValidatingCommand
	// MaskedTextBoxValidatingCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxValidatingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxValidatingCommandBehavior<T> : MaskedTextBoxValidatingCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxValidatedCommand
	// MaskedTextBoxValidatedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxValidatedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxValidatedCommandBehavior<T> : MaskedTextBoxValidatedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxStringValidationCompletedCommand
	// MaskedTextBoxStringValidationCompletedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxStringValidationCompletedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxStringValidationCompletedCommandBehavior<T> : MaskedTextBoxStringValidationCompletedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxTextSelectionOnFocusChangedCommand
	// MaskedTextBoxTextSelectionOnFocusChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxTextSelectionOnFocusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxTextSelectionOnFocusChangedCommandBehavior<T> : MaskedTextBoxTextSelectionOnFocusChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxEnterToMoveNextChangedCommand
	// MaskedTextBoxEnterToMoveNextChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxEnterToMoveNextChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxEnterToMoveNextChangedCommandBehavior<T> : MaskedTextBoxEnterToMoveNextChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxMinLengthChangedCommand
	// MaskedTextBoxMinLengthChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxMinLengthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxMinLengthChangedCommandBehavior<T> : MaskedTextBoxMinLengthChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxWatermarkTemplateChangedCommand
	// MaskedTextBoxWatermarkTemplateChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxWatermarkTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxWatermarkTemplateChangedCommandBehavior<T> : MaskedTextBoxWatermarkTemplateChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxWatermarkTextChangedCommand
	// MaskedTextBoxWatermarkTextChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxWatermarkTextChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxWatermarkTextChangedCommandBehavior<T> : MaskedTextBoxWatermarkTextChangedCommandBehavior
    { }
	#endregion

	#region MaskedTextBoxValueChangedCommand
	// MaskedTextBoxValueChangedCommand
    /// <summary>
    /// 
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

	// MaskedTextBoxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskedTextBoxValueChangedCommandBehavior<T> : MaskedTextBoxValueChangedCommandBehavior
    { }
	#endregion
}


