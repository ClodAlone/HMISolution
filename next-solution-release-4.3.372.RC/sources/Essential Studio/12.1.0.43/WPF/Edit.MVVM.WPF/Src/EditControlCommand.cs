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
using Syncfusion.Windows.Edit;


namespace Syncfusion.Windows.Edit.MVVM
{   

	#region EditControlSelectedTextChangedCommand
	// EditControlSelectedTextChangedCommand
    /// <summary>
    /// EditControlSelectedTextChangedCommand class
    /// </summary>
	public class EditControlSelectedTextChangedCommand : ControlCommandBase<EditControlSelectedTextChangedCommandBehavior, EditControl>
	{ }
    /// <summary>
    /// EditControlSelectedTextChangedCommandBehavior class
    /// </summary>
    public class EditControlSelectedTextChangedCommandBehavior : CommandBehaviorBase<EditControl>
    {
        /// <summary>
        /// This method is called when any event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// This method is called when the target is attached.
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedTextChanged += OnEventRaised;
        }
    }

	// EditControlSelectedTextChangedCommandBehavior
    /// <summary>
    /// EditControlSelectedTextChangedCommandBehavior class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditControlSelectedTextChangedCommandBehavior<T> : EditControlSelectedTextChangedCommandBehavior
    { }
	#endregion

	#region EditControlTextChangedCommand
	// EditControlTextChangedCommand
    /// <summary>
    /// EditControlTextChangedCommand class
    /// </summary>
	public class EditControlTextChangedCommand : ControlCommandBase<EditControlTextChangedCommandBehavior, EditControl>
	{ }

    /// <summary>
    /// EditControlTextChangedCommandBehavior class
    /// </summary>
    public class EditControlTextChangedCommandBehavior : CommandBehaviorBase<EditControl>
    {
        /// <summary>
        /// This method is called when any event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// This method is called when the target is attached.
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TextChanged += OnEventRaised;
        }
    }

	// EditControlTextChangedCommandBehavior
    /// <summary>
    /// EditControlTextChangedCommandBehavior class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditControlTextChangedCommandBehavior<T> : EditControlTextChangedCommandBehavior
    { }
	#endregion

	#region EditControlDocumentSourceChangedCommand
	// EditControlDocumentSourceChangedCommand
    /// <summary>
    /// EditControlDocumentSourceChangedCommand class
    /// </summary>
	public class EditControlDocumentSourceChangedCommand : ControlCommandBase<EditControlDocumentSourceChangedCommandBehavior, EditControl>
	{ }

    /// <summary>
    /// EditControlDocumentSourceChangedCommandBehavior class
    /// </summary>
    public class EditControlDocumentSourceChangedCommandBehavior : CommandBehaviorBase<EditControl>
    {
        /// <summary>
        /// This method is called when any event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// This method is called when the target is attached.
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DocumentSourceChanged += OnEventRaised;
        }
    }

	// EditControlDocumentSourceChangedCommandBehavior
    /// <summary>
    /// EditControlDocumentSourceChangedCommandBehavior class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditControlDocumentSourceChangedCommandBehavior<T> : EditControlDocumentSourceChangedCommandBehavior
    { }
	#endregion

	#region EditControlIntellisenseBoxOpeningCommand
	// EditControlIntellisenseBoxOpeningCommand
    /// <summary>
    /// EditControlIntellisenseBoxOpeningCommand class
    /// </summary>
	public class EditControlIntellisenseBoxOpeningCommand : ControlCommandBase<EditControlIntellisenseBoxOpeningCommandBehavior, EditControl>
	{ }
    /// <summary>
    /// EditControlIntellisenseBoxOpeningCommandBehavior class
    /// </summary>
    public class EditControlIntellisenseBoxOpeningCommandBehavior : CommandBehaviorBase<EditControl>
    {
        /// <summary>
        /// This method is called when any event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EditIntellisenseArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// This method is called when the target is attached.
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IntellisenseBoxOpening += OnEventRaised;
        }
    }

	// EditControlIntellisenseBoxOpeningCommandBehavior
    /// <summary>
    /// EditControlIntellisenseBoxOpeningCommandBehavior class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditControlIntellisenseBoxOpeningCommandBehavior<T> : EditControlIntellisenseBoxOpeningCommandBehavior
    { }
	#endregion

	#region EditControlIntellisenseDrillDownCommand
	// EditControlIntellisenseDrillDownCommand
    /// <summary>
    /// EditControlIntellisenseDrillDownCommand class
    /// </summary>
	public class EditControlIntellisenseDrillDownCommand : ControlCommandBase<EditControlIntellisenseDrillDownCommandBehavior, EditControl>
	{ }
    /// <summary>
    /// EditControlIntellisenseDrillDownCommandBehavior class
    /// </summary>
    public class EditControlIntellisenseDrillDownCommandBehavior : CommandBehaviorBase<EditControl>
    {
        /// <summary>
        /// This method is called when any event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EditIntellisenseArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// This method is called when the target is attached.
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IntellisenseDrillDown += OnEventRaised;
        }
    }

	// EditControlIntellisenseDrillDownCommandBehavior
    /// <summary>
    /// EditControlIntellisenseDrillDownCommandBehavior class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditControlIntellisenseDrillDownCommandBehavior<T> : EditControlIntellisenseDrillDownCommandBehavior
    { }
	#endregion

	#region EditControlSelectionChangedCommand
	// EditControlSelectionChangedCommand
    /// <summary>
    /// EditControlSelectionChangedCommand class
    /// </summary>
	public class EditControlSelectionChangedCommand : ControlCommandBase<EditControlSelectionChangedCommandBehavior, EditControl>
	{ }
    /// <summary>
    /// EditControlSelectionChangedCommandBehavior class
    /// </summary>
    public class EditControlSelectionChangedCommandBehavior : CommandBehaviorBase<EditControl>
    {
        /// <summary>
        /// This method is called when any event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// This method is called when the target is attached.
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

	// EditControlSelectionChangedCommandBehavior
    /// <summary>
    /// EditControlSelectionChangedCommandBehavior class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditControlSelectionChangedCommandBehavior<T> : EditControlSelectionChangedCommandBehavior
    { }
	#endregion
}


