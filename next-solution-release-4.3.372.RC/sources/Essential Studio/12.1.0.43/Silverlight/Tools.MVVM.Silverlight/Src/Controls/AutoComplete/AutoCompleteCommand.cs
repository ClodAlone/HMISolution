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

    #region AutoCompleteSelectionModeChangedCommand
    // AutoCompleteSelectionModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteSelectionModeChangedCommand : ControlCommandBase<AutoCompleteSelectionModeChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteSelectionModeChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.SelectionModeChanged += OnEventRaised;
        }
    }

    // AutoCompleteSelectionModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteSelectionModeChangedCommandBehavior<T> : AutoCompleteSelectionModeChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteDropDownButtonVisibilityChangedCommand
    // AutoCompleteDropDownButtonVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteDropDownButtonVisibilityChangedCommand : ControlCommandBase<AutoCompleteDropDownButtonVisibilityChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteDropDownButtonVisibilityChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.DropDownButtonVisibilityChanged += OnEventRaised;
        }
    }

    // AutoCompleteDropDownButtonVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteDropDownButtonVisibilityChangedCommandBehavior<T> : AutoCompleteDropDownButtonVisibilityChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteIsHistoryEnabledChangedCommand
    // AutoCompleteIsHistoryEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsHistoryEnabledChangedCommand : ControlCommandBase<AutoCompleteIsHistoryEnabledChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsHistoryEnabledChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.IsHistoryEnabledChanged += OnEventRaised;
        }
    }

    // AutoCompleteIsHistoryEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteIsHistoryEnabledChangedCommandBehavior<T> : AutoCompleteIsHistoryEnabledChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteIsDropDownOpenChangedCommand
    // AutoCompleteIsDropDownOpenChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsDropDownOpenChangedCommand : ControlCommandBase<AutoCompleteIsDropDownOpenChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsDropDownOpenChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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

    // AutoCompleteIsDropDownOpenChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteIsDropDownOpenChangedCommandBehavior<T> : AutoCompleteIsDropDownOpenChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteCustomSourceChangedCommand
    // AutoCompleteCustomSourceChangedCommand

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteCustomSourceChangedCommand : ControlCommandBase<AutoCompleteCustomSourceChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteCustomSourceChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.CustomSourceChanged += OnEventRaised;
        }
    }

    // AutoCompleteCustomSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteCustomSourceChangedCommandBehavior<T> : AutoCompleteCustomSourceChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteIsAutoAppendChangedCommand
    // AutoCompleteIsAutoAppendChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsAutoAppendChangedCommand : ControlCommandBase<AutoCompleteIsAutoAppendChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsAutoAppendChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.IsAutoAppendChanged += OnEventRaised;
        }
    }

    // AutoCompleteIsAutoAppendChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteIsAutoAppendChangedCommandBehavior<T> : AutoCompleteIsAutoAppendChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteMaxDropHeightChangedCommand
    // AutoCompleteMaxDropHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteMaxDropHeightChangedCommand : ControlCommandBase<AutoCompleteMaxDropHeightChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteMaxDropHeightChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.MaxDropHeightChanged += OnEventRaised;
        }
    }

    // AutoCompleteMaxDropHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteMaxDropHeightChangedCommandBehavior<T> : AutoCompleteMaxDropHeightChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteIsFilterChangedCommand
    // AutoCompleteIsFilterChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsFilterChangedCommand : ControlCommandBase<AutoCompleteIsFilterChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsFilterChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.IsFilterChanged += OnEventRaised;
        }
    }

    // AutoCompleteIsFilterChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteIsFilterChangedCommandBehavior<T> : AutoCompleteIsFilterChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteIsAutoCompleteItemChangedCommand
    // AutoCompleteIsAutoCompleteItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsAutoCompleteItemChangedCommand : ControlCommandBase<AutoCompleteIsAutoCompleteItemChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsAutoCompleteItemChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.IsAutoCompleteItemChanged += OnEventRaised;
        }
    }

    // AutoCompleteIsAutoCompleteItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteIsAutoCompleteItemChangedCommandBehavior<T> : AutoCompleteIsAutoCompleteItemChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteIsAsyncAddContentChangedCommand
    // AutoCompleteIsAsyncAddContentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsAsyncAddContentChangedCommand : ControlCommandBase<AutoCompleteIsAsyncAddContentChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteIsAsyncAddContentChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.IsAsyncAddContentChanged += OnEventRaised;
        }
    }

    // AutoCompleteIsAsyncAddContentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteIsAsyncAddContentChangedCommandBehavior<T> : AutoCompleteIsAsyncAddContentChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteTextChangedCommand
    // AutoCompleteTextChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteTextChangedCommand : ControlCommandBase<AutoCompleteTextChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteTextChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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

    // AutoCompleteTextChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteTextChangedCommandBehavior<T> : AutoCompleteTextChangedCommandBehavior
    { }
    #endregion

    #region AutoCompleteSelectionChangedCommand
    // AutoCompleteSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteSelectionChangedCommand : ControlCommandBase<AutoCompleteSelectionChangedCommandBehavior, AutoComplete>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AutoCompleteSelectionChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, SelectionChangedEventArgs e)
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

    // AutoCompleteSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class AutoCompleteSelectionChangedCommandBehavior<T> : AutoCompleteSelectionChangedCommandBehavior
    { }
    #endregion
}

