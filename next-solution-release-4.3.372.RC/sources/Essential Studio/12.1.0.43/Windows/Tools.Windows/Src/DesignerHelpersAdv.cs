#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface ITabControlAdvDesigner
	{
		void InitTypeLoaderComponent();
		void RemoveUnserializableChildControls();
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IBarManagerDesigner
	{
		void SetDirty();
		object GetService(Type type);
		IDesignerHost DesignerHost{get;}
		IntPtr MdiClientWnd{get;}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IListenForMainFormVisibilityChange
	{
		void OnChangedVisibility(bool visible);
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IToolbarDesigners
	{
		void SetDirty();
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface ICommandBarDesignerComponentInvoke
	{
		void RaiseComponentChanged();
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface ICommandBarDesignerInvoke
	{
		void SetChildControlBounds();
		CommandDockBar GetCommandDockBarParent();
		bool CommandBarBaseVisibility
		{
			get;
			set;
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface ICBControllerDesignerInvoke
	{
		CommandDockBar GetCommandDockBarL();
		CommandDockBar GetCommandDockBarT();
		CommandDockBar GetCommandDockBarR();
		CommandDockBar GetCommandDockBarB();
		void InitializeCBController();
	}
}
