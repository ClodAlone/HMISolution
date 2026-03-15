#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{   

	#region RichTextBoxAdvFileOpeningCommand
	// RichTextBoxAdvFileOpeningCommand
	public class RichTextBoxAdvFileOpeningCommand : ControlCommandBase<RichTextBoxAdvFileOpeningCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvFileOpeningCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
		protected virtual void OnEventRaised(object sender, FileOpeningEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.FileOpening += OnEventRaised;
        }
    }

	// RichTextBoxAdvFileOpeningCommandBehavior
    public class RichTextBoxAdvFileOpeningCommandBehavior<T> : RichTextBoxAdvFileOpeningCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvFileSavingCommand
	// RichTextBoxAdvFileSavingCommand
	public class RichTextBoxAdvFileSavingCommand : ControlCommandBase<RichTextBoxAdvFileSavingCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvFileSavingCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
		protected virtual void OnEventRaised(object sender, FileSavingEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.FileSaving += OnEventRaised;
        }
    }

	// RichTextBoxAdvFileSavingCommandBehavior
    public class RichTextBoxAdvFileSavingCommandBehavior<T> : RichTextBoxAdvFileSavingCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvStyleChangedCommand
	// RichTextBoxAdvStyleChangedCommand
	public class RichTextBoxAdvStyleChangedCommand : ControlCommandBase<RichTextBoxAdvStyleChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvStyleChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.StyleChanged += OnEventRaised;
        }
    }

	// RichTextBoxAdvStyleChangedCommandBehavior
    public class RichTextBoxAdvStyleChangedCommandBehavior<T> : RichTextBoxAdvStyleChangedCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvPrintingCommand
	// RichTextBoxAdvPrintingCommand
	public class RichTextBoxAdvPrintingCommand : ControlCommandBase<RichTextBoxAdvPrintingCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvPrintingCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
		protected virtual void OnEventRaised(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.Printing += OnEventRaised;
        }
    }

	// RichTextBoxAdvPrintingCommandBehavior
    public class RichTextBoxAdvPrintingCommandBehavior<T> : RichTextBoxAdvPrintingCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvPrintCompletedCommand
	// RichTextBoxAdvPrintCompletedCommand
	public class RichTextBoxAdvPrintCompletedCommand : ControlCommandBase<RichTextBoxAdvPrintCompletedCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvPrintCompletedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.PrintCompleted += OnEventRaised;
        }
    }

	// RichTextBoxAdvPrintCompletedCommandBehavior
    public class RichTextBoxAdvPrintCompletedCommandBehavior<T> : RichTextBoxAdvPrintCompletedCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvTextChangedCommand
	// RichTextBoxAdvTextChangedCommand
	public class RichTextBoxAdvTextChangedCommand : ControlCommandBase<RichTextBoxAdvTextChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvTextChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
        protected virtual void OnEventRaised(object sender, Syncfusion.Windows.Tools.Controls.TextChangedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.TextChanged += OnEventRaised;
        }
    }

	// RichTextBoxAdvTextChangedCommandBehavior
    public class RichTextBoxAdvTextChangedCommandBehavior<T> : RichTextBoxAdvTextChangedCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvSelectionChangedCommand
	// RichTextBoxAdvSelectionChangedCommand
	public class RichTextBoxAdvSelectionChangedCommand : ControlCommandBase<RichTextBoxAdvSelectionChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvSelectionChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
        protected virtual void OnEventRaised(object sender, Syncfusion.Windows.Tools.Controls.SelectionChangedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

	// RichTextBoxAdvSelectionChangedCommandBehavior
    public class RichTextBoxAdvSelectionChangedCommandBehavior<T> : RichTextBoxAdvSelectionChangedCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvHyperlinkClickedCommand
	// RichTextBoxAdvHyperlinkClickedCommand
	public class RichTextBoxAdvHyperlinkClickedCommand : ControlCommandBase<RichTextBoxAdvHyperlinkClickedCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvHyperlinkClickedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.HyperlinkClicked += OnEventRaised;
        }
    }

	// RichTextBoxAdvHyperlinkClickedCommandBehavior
    public class RichTextBoxAdvHyperlinkClickedCommandBehavior<T> : RichTextBoxAdvHyperlinkClickedCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvIsReadOnlyChangedCommand
	// RichTextBoxAdvIsReadOnlyChangedCommand
	public class RichTextBoxAdvIsReadOnlyChangedCommand : ControlCommandBase<RichTextBoxAdvIsReadOnlyChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvIsReadOnlyChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.IsReadOnlyChanged += OnEventRaised;
        }
    }

	// RichTextBoxAdvIsReadOnlyChangedCommandBehavior
    public class RichTextBoxAdvIsReadOnlyChangedCommandBehavior<T> : RichTextBoxAdvIsReadOnlyChangedCommandBehavior
    { }
	#endregion

	#region RichTextBoxAdvZoomFactorChangedCommand
	// RichTextBoxAdvZoomFactorChangedCommand
	public class RichTextBoxAdvZoomFactorChangedCommand : ControlCommandBase<RichTextBoxAdvZoomFactorChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
	{ }

    public class RichTextBoxAdvZoomFactorChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.RichTextBoxAdv>
    {
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.ZoomFactorChanged += OnEventRaised;
        }
    }

	// RichTextBoxAdvZoomFactorChangedCommandBehavior
    public class RichTextBoxAdvZoomFactorChangedCommandBehavior<T> : RichTextBoxAdvZoomFactorChangedCommandBehavior
    { }
	#endregion
}


