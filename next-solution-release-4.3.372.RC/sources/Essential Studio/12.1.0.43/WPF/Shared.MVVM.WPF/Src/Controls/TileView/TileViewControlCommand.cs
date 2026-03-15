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
using System.Windows.Controls;


namespace Syncfusion.Windows.Tools.MVVM
{
    #region TileViewControlSelectionChangedPropertyChangedCommand
    // TileViewControlSelectionChangedPropertyChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlSelectionChangedPropertyChangedCommand : ControlCommandBase<TileViewControlSelectionChangedPropertyChangedCommandBehavior, TileViewControl>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlSelectionChangedPropertyChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void TargetObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged +=new SelectionChangedEventHandler(TargetObject_SelectionChanged);
        }
    }

    // TileViewControlSelectionChangedPropertyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlSelectionChangedCommandBehavior<T> : TileViewControlSelectionChangedPropertyChangedCommandBehavior
    { }
    #endregion

	#region TileViewControlIsClickHeaderToMaximizePropertyChangedCommand
	// TileViewControlIsClickHeaderToMaximizePropertyChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlIsClickHeaderToMaximizePropertyChangedCommand : ControlCommandBase<TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.IsClickHeaderToMaximizePropertyChanged += OnEventRaised;
        }
    }

	// TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior<T> : TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommand
	// TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommand : ControlCommandBase<TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.IsMinMaxButtonOnMouseOverOnlyChanged += OnEventRaised;
        }
    }

	// TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior<T> : TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlIsSplitterVisibilityChangedCommand
	// TileViewControlIsSplitterVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlIsSplitterVisibilityChangedCommand : ControlCommandBase<TileViewControlIsSplitterVisibilityChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlIsSplitterVisibilityChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.IsSplitterVisibilityChanged += OnEventRaised;
        }
    }

	// TileViewControlIsSplitterVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlIsSplitterVisibilityChangedCommandBehavior<T> : TileViewControlIsSplitterVisibilityChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlRowCountChangedCommand
	// TileViewControlRowCountChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlRowCountChangedCommand : ControlCommandBase<TileViewControlRowCountChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlRowCountChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.RowCountChanged += OnEventRaised;
        }
    }

	// TileViewControlRowCountChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlRowCountChangedCommandBehavior<T> : TileViewControlRowCountChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlColumnCountChangedCommand
	// TileViewControlColumnCountChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlColumnCountChangedCommand : ControlCommandBase<TileViewControlColumnCountChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlColumnCountChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.ColumnCountChanged += OnEventRaised;
        }
    }

	// TileViewControlColumnCountChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlColumnCountChangedCommandBehavior<T> : TileViewControlColumnCountChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlAllowItemRepositioningChangedCommand
	// TileViewControlAllowItemRepositioningChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlAllowItemRepositioningChangedCommand : ControlCommandBase<TileViewControlAllowItemRepositioningChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlAllowItemRepositioningChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.AllowItemRepositioningChanged += OnEventRaised;
        }
    }

	// TileViewControlAllowItemRepositioningChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlAllowItemRepositioningChangedCommandBehavior<T> : TileViewControlAllowItemRepositioningChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlMinimizedItemsOrientationChangedCommand
	// TileViewControlMinimizedItemsOrientationChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlMinimizedItemsOrientationChangedCommand : ControlCommandBase<TileViewControlMinimizedItemsOrientationChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMinimizedItemsOrientationChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.MinimizedItemsOrientationChanged += OnEventRaised;
        }
    }

	// TileViewControlMinimizedItemsOrientationChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMinimizedItemsOrientationChangedCommandBehavior<T> : TileViewControlMinimizedItemsOrientationChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlMinimizedItemsPercentageChangedCommand
	// TileViewControlMinimizedItemsPercentageChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlMinimizedItemsPercentageChangedCommand : ControlCommandBase<TileViewControlMinimizedItemsPercentageChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMinimizedItemsPercentageChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.MinimizedItemsPercentageChanged += OnEventRaised;
        }
    }

	// TileViewControlMinimizedItemsPercentageChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMinimizedItemsPercentageChangedCommandBehavior<T> : TileViewControlMinimizedItemsPercentageChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlSplitterThicknessChangedCommand
	// TileViewControlSplitterThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlSplitterThicknessChangedCommand : ControlCommandBase<TileViewControlSplitterThicknessChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlSplitterThicknessChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.SplitterThicknessChanged += OnEventRaised;
        }
    }

	// TileViewControlSplitterThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlSplitterThicknessChangedCommandBehavior<T> : TileViewControlSplitterThicknessChangedCommandBehavior
    { }
	#endregion

	#region TileViewControlRepositionedCommand
	// TileViewControlRepositionedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlRepositionedCommand : ControlCommandBase<TileViewControlRepositionedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlRepositionedCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Repositioned += OnEventRaised;
        }
    }

	// TileViewControlRepositionedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlRepositionedCommandBehavior<T> : TileViewControlRepositionedCommandBehavior
    { }
	#endregion

	#region TileViewControlRepositioningCommand
	// TileViewControlRepositioningCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlRepositioningCommand : ControlCommandBase<TileViewControlRepositioningCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlRepositioningCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewCancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Repositioning += OnEventRaised;
        }
    }

	// TileViewControlRepositioningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlRepositioningCommandBehavior<T> : TileViewControlRepositioningCommandBehavior
    { }
	#endregion

	#region TileViewControlMinimizedCommand
	// TileViewControlMinimizedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlMinimizedCommand : ControlCommandBase<TileViewControlMinimizedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMinimizedCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Minimized += OnEventRaised;
        }
    }

	// TileViewControlMinimizedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMinimizedCommandBehavior<T> : TileViewControlMinimizedCommandBehavior
    { }
	#endregion

	#region TileViewControlMinimizingCommand
	// TileViewControlMinimizingCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlMinimizingCommand : ControlCommandBase<TileViewControlMinimizingCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMinimizingCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Minimizing += OnEventRaised;
        }
    }

	// TileViewControlMinimizingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMinimizingCommandBehavior<T> : TileViewControlMinimizingCommandBehavior
    { }
	#endregion

	#region TileViewControlMaximizingCommand
	// TileViewControlMaximizingCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlMaximizingCommand : ControlCommandBase<TileViewControlMaximizingCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMaximizingCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewCancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Maximizing += OnEventRaised;
        }
    }

	// TileViewControlMaximizingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMaximizingCommandBehavior<T> : TileViewControlMaximizingCommandBehavior
    { }
	#endregion

	#region TileViewControlMaximizedCommand
	// TileViewControlMaximizedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlMaximizedCommand : ControlCommandBase<TileViewControlMaximizedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMaximizedCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Maximized += OnEventRaised;
        }
    }

	// TileViewControlMaximizedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMaximizedCommandBehavior<T> : TileViewControlMaximizedCommandBehavior
    { }
	#endregion

	#region TileViewControlRestoringCommand
	// TileViewControlRestoringCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlRestoringCommand : ControlCommandBase<TileViewControlRestoringCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlRestoringCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewCancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Restoring += OnEventRaised;
        }
    }

	// TileViewControlRestoringCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlRestoringCommandBehavior<T> : TileViewControlRestoringCommandBehavior
    { }
	#endregion

	#region TileViewControlRestoredCommand
	// TileViewControlRestoredCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlRestoredCommand : ControlCommandBase<TileViewControlRestoredCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlRestoredCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Restored += OnEventRaised;
        }
    }

	// TileViewControlRestoredCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlRestoredCommandBehavior<T> : TileViewControlRestoredCommandBehavior
    { }
	#endregion

	#region TileViewControlMaximizedItemChangedCommand
	// TileViewControlMaximizedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewControlMaximizedItemChangedCommand : ControlCommandBase<TileViewControlMaximizedItemChangedCommandBehavior, TileViewControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMaximizedItemChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.MaximizedItemChanged += OnEventRaised;
        }
    }

	// TileViewControlMaximizedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMaximizedItemChangedCommandBehavior<T> : TileViewControlMaximizedItemChangedCommandBehavior
    { }
	#endregion
}


