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

using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using System.Windows.Controls;


namespace Syncfusion.Windows.Tools.MVVM
{   

	#region AutoCompleteSourceChangedCommand
	// AutoCompleteSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteSourceChangedCommand : ControlCommandBase<AutoCompleteSourceChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class AutoCompleteSourceChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.SourceChanged += OnEventRaised;
        }
    }

	// AutoCompleteSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoCompleteSourceChangedCommandBehavior<T> : AutoCompleteSourceChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteSelectedValueChangedCommand
	// AutoCompleteSelectedValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteSelectedValueChangedCommand : ControlCommandBase<AutoCompleteSelectedValueChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class AutoCompleteSelectedValueChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.SelectedValueChanged += OnEventRaised;
        }
    }

	// AutoCompleteSelectedValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoCompleteSelectedValueChangedCommandBehavior<T> : AutoCompleteSelectedValueChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteIsDropDownOpenChangedCommand
	// AutoCompleteIsDropDownOpenChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteIsDropDownOpenChangedCommand : ControlCommandBase<AutoCompleteIsDropDownOpenChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteIsDropDownOpenChangedCommandBehavior<T> : AutoCompleteIsDropDownOpenChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteCustomSourceChangedCommand
	// AutoCompleteCustomSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteCustomSourceChangedCommand : ControlCommandBase<AutoCompleteCustomSourceChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteCustomSourceChangedCommandBehavior<T> : AutoCompleteCustomSourceChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteIsAutoAppendChangedCommand
	// AutoCompleteIsAutoAppendChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteIsAutoAppendChangedCommand : ControlCommandBase<AutoCompleteIsAutoAppendChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteIsAutoAppendChangedCommandBehavior<T> : AutoCompleteIsAutoAppendChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteMaxDropHeightChangedCommand
	// AutoCompleteMaxDropHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteMaxDropHeightChangedCommand : ControlCommandBase<AutoCompleteMaxDropHeightChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteMaxDropHeightChangedCommandBehavior<T> : AutoCompleteMaxDropHeightChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteIsFilterChangedCommand
	// AutoCompleteIsFilterChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteIsFilterChangedCommand : ControlCommandBase<AutoCompleteIsFilterChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteIsFilterChangedCommandBehavior<T> : AutoCompleteIsFilterChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteIsAutoCompleteItemChangedCommand
	// AutoCompleteIsAutoCompleteItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteIsAutoCompleteItemChangedCommand : ControlCommandBase<AutoCompleteIsAutoCompleteItemChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteIsAutoCompleteItemChangedCommandBehavior<T> : AutoCompleteIsAutoCompleteItemChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteIsAsyncAddContentChangedCommand
	// AutoCompleteIsAsyncAddContentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteIsAsyncAddContentChangedCommand : ControlCommandBase<AutoCompleteIsAsyncAddContentChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteIsAsyncAddContentChangedCommandBehavior<T> : AutoCompleteIsAsyncAddContentChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteTextChangedCommand
	// AutoCompleteTextChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteTextChangedCommand : ControlCommandBase<AutoCompleteTextChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteTextChangedCommandBehavior<T> : AutoCompleteTextChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteIsHistoryChangedCommand
	// AutoCompleteIsHistoryChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteIsHistoryChangedCommand : ControlCommandBase<AutoCompleteIsHistoryChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class AutoCompleteIsHistoryChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.IsHistoryChanged += OnEventRaised;
        }
    }

	// AutoCompleteIsHistoryChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoCompleteIsHistoryChangedCommandBehavior<T> : AutoCompleteIsHistoryChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteIsHistoryDropDownOpenChangedCommand
	// AutoCompleteIsHistoryDropDownOpenChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteIsHistoryDropDownOpenChangedCommand : ControlCommandBase<AutoCompleteIsHistoryDropDownOpenChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class AutoCompleteIsHistoryDropDownOpenChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.IsHistoryDropDownOpenChanged += OnEventRaised;
        }
    }

	// AutoCompleteIsHistoryDropDownOpenChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoCompleteIsHistoryDropDownOpenChangedCommandBehavior<T> : AutoCompleteIsHistoryDropDownOpenChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteHistoryListHeightChangedCommand
	// AutoCompleteHistoryListHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteHistoryListHeightChangedCommand : ControlCommandBase<AutoCompleteHistoryListHeightChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class AutoCompleteHistoryListHeightChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.HistoryListHeightChanged += OnEventRaised;
        }
    }

	// AutoCompleteHistoryListHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoCompleteHistoryListHeightChangedCommandBehavior<T> : AutoCompleteHistoryListHeightChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteFileStorageNameChangedCommand
	// AutoCompleteFileStorageNameChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteFileStorageNameChangedCommand : ControlCommandBase<AutoCompleteFileStorageNameChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class AutoCompleteFileStorageNameChangedCommandBehavior : CommandBehaviorBase<AutoComplete>
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
            TargetObject.FileStorageNameChanged += OnEventRaised;
        }
    }

	// AutoCompleteFileStorageNameChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoCompleteFileStorageNameChangedCommandBehavior<T> : AutoCompleteFileStorageNameChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteSelectionModeChangedCommand
	// AutoCompleteSelectionModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteSelectionModeChangedCommand : ControlCommandBase<AutoCompleteSelectionModeChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteSelectionModeChangedCommandBehavior<T> : AutoCompleteSelectionModeChangedCommandBehavior
    { }
	#endregion

	#region AutoCompleteSelectionChangedCommand
	// AutoCompleteSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class AutoCompleteSelectionChangedCommand : ControlCommandBase<AutoCompleteSelectionChangedCommandBehavior, AutoComplete>
	{ }
    /// <summary>
    /// 
    /// </summary>
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
    public class AutoCompleteSelectionChangedCommandBehavior<T> : AutoCompleteSelectionChangedCommandBehavior
    { }
	#endregion
}


