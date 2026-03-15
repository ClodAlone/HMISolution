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

    #region EditorBaseCultureChangedCommand
    /// <summary>
    /// EditorBaseCultureChangedCommand
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

    /// <summary>
    /// EditorBaseCultureChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseCultureChangedCommandBehavior<T> : EditorBaseCultureChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseNumberFormatChangedCommand
    /// <summary>
    /// EditorBaseNumberFormatChangedCommand
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

    /// <summary>
    /// EditorBaseNumberFormatChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseNumberFormatChangedCommandBehavior<T> : EditorBaseNumberFormatChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseWaterMarkTemplateChangedCommand
    /// <summary>
    /// EditorBaseWaterMarkTemplateChangedCommand
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

    /// <summary>
    /// EditorBaseWaterMarkTemplateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseWaterMarkTemplateChangedCommandBehavior<T> : EditorBaseWaterMarkTemplateChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseWaterMarkTextChangedCommand
    /// <summary>
    /// EditorBaseWaterMarkTextChangedCommand
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

    /// <summary>
    /// EditorBaseWaterMarkTextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseWaterMarkTextChangedCommandBehavior<T> : EditorBaseWaterMarkTextChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseIsUndoEnabledChangedCommand
    /// <summary>
    /// EditorBaseIsUndoEnabledChangedCommand
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

    /// <summary>
    /// EditorBaseIsUndoEnabledChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseIsUndoEnabledChangedCommandBehavior<T> : EditorBaseIsUndoEnabledChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseTextSelectionOnFocusChangedCommand
    /// <summary>
    /// EditorBaseTextSelectionOnFocusChangedCommand
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

    /// <summary>
    /// EditorBaseTextSelectionOnFocusChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseTextSelectionOnFocusChangedCommandBehavior<T> : EditorBaseTextSelectionOnFocusChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseNegativeForegroundChangedCommand
    /// <summary>
    /// EditorBaseNegativeForegroundChangedCommand
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

    /// <summary>
    /// EditorBaseNegativeForegroundChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseNegativeForegroundChangedCommandBehavior<T> : EditorBaseNegativeForegroundChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseIsValueNegativeChangedCommand
    /// <summary>
    /// EditorBaseIsValueNegativeChangedCommand
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

    /// <summary>
    /// EditorBaseIsValueNegativeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseIsValueNegativeChangedCommandBehavior<T> : EditorBaseIsValueNegativeChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseEnterToMoveNextChangedCommand
    /// <summary>
    /// EditorBaseEnterToMoveNextChangedCommand
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

    /// <summary>
    /// EditorBaseEnterToMoveNextChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseEnterToMoveNextChangedCommandBehavior<T> : EditorBaseEnterToMoveNextChangedCommandBehavior
    { }
    #endregion

    #region EditorBaseSelectionChangedCommand
    /// <summary>
    /// EditorBaseSelectionChangedCommand
    /// </summary>
    public class EditorBaseSelectionChangedCommand : ControlCommandBase<EditorBaseSelectionChangedCommandBehavior, EditorBase>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class EditorBaseSelectionChangedCommandBehavior : CommandBehaviorBase<EditorBase>
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
    /// EditorBaseSelectionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorBaseSelectionChangedCommandBehavior<T> : EditorBaseSelectionChangedCommandBehavior
    { }
    #endregion


}


