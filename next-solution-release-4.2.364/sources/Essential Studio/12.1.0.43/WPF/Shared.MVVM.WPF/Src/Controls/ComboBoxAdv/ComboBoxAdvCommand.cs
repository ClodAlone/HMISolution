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

	#region ComboBoxAdvDropDownClosedCommand
	// ComboBoxAdvDropDownClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ComboBoxAdvDropDownClosedCommand : ControlCommandBase<ComboBoxAdvDropDownClosedCommandBehavior, ComboBoxAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ComboBoxAdvDropDownClosedCommandBehavior : CommandBehaviorBase<ComboBoxAdv>
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
            TargetObject.DropDownClosed += OnEventRaised;
        }
    }

	// ComboBoxAdvDropDownClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComboBoxAdvDropDownClosedCommandBehavior<T> : ComboBoxAdvDropDownClosedCommandBehavior
    { }
	#endregion

	#region ComboBoxAdvDropDownOpenedCommand
	// ComboBoxAdvDropDownOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ComboBoxAdvDropDownOpenedCommand : ControlCommandBase<ComboBoxAdvDropDownOpenedCommandBehavior, ComboBoxAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ComboBoxAdvDropDownOpenedCommandBehavior : CommandBehaviorBase<ComboBoxAdv>
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
            TargetObject.DropDownOpened += OnEventRaised;
        }
    }

	// ComboBoxAdvDropDownOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComboBoxAdvDropDownOpenedCommandBehavior<T> : ComboBoxAdvDropDownOpenedCommandBehavior
    { }
	#endregion
    #region ComboBoxAdvSelectionChangedCommand
    // ComboBoxAdvSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class ComboBoxAdvSelectionChangedCommand : ControlCommandBase<ComboBoxAdvSelectionChangedCommandBehavior, ComboBoxAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class ComboBoxAdvSelectionChangedCommandBehavior : CommandBehaviorBase<ComboBoxAdv>
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
            TargetObject.SelectionChanged  += OnEventRaised;
        }
    }

    // ComboBoxAdvSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComboBoxAdvSelectionChangedCommandBehavior<T> : ComboBoxAdvSelectionChangedCommandBehavior
    { }
    #endregion
}


