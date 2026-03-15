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

	#region EditorBaseCultureChangedCommand
	// EditorBaseCultureChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseCultureChangedCommand : ControlCommandBase<EditorBaseCultureChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseCultureChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseCultureChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseCultureChangedCommandBehavior<T> : EditorBaseCultureChangedCommandBehavior
    { }
	#endregion

	#region EditorBaseNumberFormatChangedCommand
	// EditorBaseNumberFormatChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseNumberFormatChangedCommand : ControlCommandBase<EditorBaseNumberFormatChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseNumberFormatChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseNumberFormatChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseNumberFormatChangedCommandBehavior<T> : EditorBaseNumberFormatChangedCommandBehavior
    { }
	#endregion

	#region EditorBaseWaterMarkTemplateChangedCommand
	// EditorBaseWaterMarkTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseWaterMarkTemplateChangedCommand : ControlCommandBase<EditorBaseWaterMarkTemplateChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseWaterMarkTemplateChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseWaterMarkTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseWaterMarkTemplateChangedCommandBehavior<T> : EditorBaseWaterMarkTemplateChangedCommandBehavior
    { }
	#endregion

	#region EditorBaseWaterMarkTextChangedCommand
	// EditorBaseWaterMarkTextChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseWaterMarkTextChangedCommand : ControlCommandBase<EditorBaseWaterMarkTextChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseWaterMarkTextChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseWaterMarkTextChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseWaterMarkTextChangedCommandBehavior<T> : EditorBaseWaterMarkTextChangedCommandBehavior
    { }
	#endregion

	#region EditorBaseIsUndoEnabledChangedCommand
	// EditorBaseIsUndoEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseIsUndoEnabledChangedCommand : ControlCommandBase<EditorBaseIsUndoEnabledChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseIsUndoEnabledChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseIsUndoEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseIsUndoEnabledChangedCommandBehavior<T> : EditorBaseIsUndoEnabledChangedCommandBehavior
    { }
	#endregion

	#region EditorBaseTextSelectionOnFocusChangedCommand
	// EditorBaseTextSelectionOnFocusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseTextSelectionOnFocusChangedCommand : ControlCommandBase<EditorBaseTextSelectionOnFocusChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseTextSelectionOnFocusChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseTextSelectionOnFocusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseTextSelectionOnFocusChangedCommandBehavior<T> : EditorBaseTextSelectionOnFocusChangedCommandBehavior
    { }
	#endregion

	#region EditorBaseNegativeForegroundChangedCommand
	// EditorBaseNegativeForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseNegativeForegroundChangedCommand : ControlCommandBase<EditorBaseNegativeForegroundChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseNegativeForegroundChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseNegativeForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseNegativeForegroundChangedCommandBehavior<T> : EditorBaseNegativeForegroundChangedCommandBehavior
    { }
	#endregion

	#region EditorBaseIsValueNegativeChangedCommand
	// EditorBaseIsValueNegativeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseIsValueNegativeChangedCommand : ControlCommandBase<EditorBaseIsValueNegativeChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseIsValueNegativeChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseIsValueNegativeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseIsValueNegativeChangedCommandBehavior<T> : EditorBaseIsValueNegativeChangedCommandBehavior
    { }
	#endregion

	#region EditorBaseEnterToMoveNextChangedCommand
	// EditorBaseEnterToMoveNextChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class EditorBaseEnterToMoveNextChangedCommand : ControlCommandBase<EditorBaseEnterToMoveNextChangedCommandBehavior, EditorBase>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseEnterToMoveNextChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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

	// EditorBaseEnterToMoveNextChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseEnterToMoveNextChangedCommandBehavior<T> : EditorBaseEnterToMoveNextChangedCommandBehavior
    { }
	#endregion
}


