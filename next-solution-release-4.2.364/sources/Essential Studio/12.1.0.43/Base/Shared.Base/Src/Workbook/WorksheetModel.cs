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
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	///    Provides the data / model part for a worksheet in an Excel-like Workbook display. A <see cref="WorksheetModel"/> is a member of the <see cref="WorkbookModel.Worksheets"/>
	///    collection of a <see cref="WorkbookModel"/>.
	/// </summary>
	/// <remarks>
	/// This class can be serialized into a serialization stream. <para/>
	/// Multiple <see cref="WorkbookView"/> controls can share the same <see cref="WorkbookModel"/>. Each <see cref="WorksheetView"/>
	/// that is displayed in a <see cref="WorkbookView"/> is associated with a <see cref="WorksheetModel"/> from the <see cref="WorkbookModel.Worksheets"/>
	/// collection in a <see cref="WorkbookModel"/>.
	/// </remarks>
	[Serializable]
	[ToolboxItem(false)]
	public class WorksheetModel : Component, ISerializable //: ICloneable
	{
		// Events
		/// <summary>
		/// Occurs when the <see cref="Name"/> is changed.
		/// </summary>
		public event EventHandler NameChanged;

		/// <summary>
		/// Occurs when the <see cref="ToolTipText"/> is changed.
		/// </summary>
		public event EventHandler ToolTipTextChanged;

		/// <summary>
		/// Occurs when the <see cref="Visible"/> property is changed.
		/// </summary>
		public event EventHandler VisibleChanged;

//		/// <summary>
//		/// Occurs when the <see cref="ActiveView"/> is changed.
//		/// </summary>
//		public event EventHandler ActiveViewChanged;

		/// <summary>
		/// Occurs when the <see cref="Content"/> property is changed.
		/// </summary>
		public event EventHandler ContentChanged;

		// Fields
		internal WorkbookModel workbook;
		private string name = "Worksheet";
		private string toolTipText = "";
		private bool visible = true; 
		private object content = null;
		//private object activeView = null;
		// int bitmapIndex;

		/// <overload>
		/// Initializes a new <see cref="WorksheetModel"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="WorksheetModel"/> and associates it with a <see cref="WorkbookModel"/>.
		/// </summary>
		/// <param name="workbook">The <see cref="WorkbookModel"/> this sheet belongs to.</param>
		/// <param name="name">The name of this sheet.</param>
		public WorksheetModel(WorkbookModel workbook, string name)
		{
			this.workbook = workbook;
			this.name = name;
		}

		/// <summary>
		/// Initializes a new <see cref="WorksheetModel"/> and associates it with a <see cref="WorkbookModel"/>.
		/// </summary>
		/// <param name="workbook">The <see cref="WorkbookModel"/> this sheet belongs to.</param>
		/// <param name="name">The name of this sheet.</param>
		/// <param name="content">The object with data to be displayed in this control, e.g. a Syncfusion.Windows.Forms.Grid.GridModel
		/// with grid data. The object should implement <see cref="ICreateControl"/> the interface and be able to create a <see cref="Control"/> 
		/// object to be displayed as pane in a <see cref="WorkbookView"/>. <see langword="Syncfusion.Windows.Forms.Grid.GridModel"/> will create a
		///  <see langword="Syncfusion.Windows.Forms.Grid.GridControlBase"/> for example.
		/// </param>
		public WorksheetModel(WorkbookModel workbook, string name, object content)
		{
			this.workbook = workbook;
			this.name = name;
			this.content = content;
		}

		/// <summary>
		/// Initializes a new <see cref="WorksheetModel"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
		protected WorksheetModel(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#else
			;
#endif


			name = info.GetString("Name");
			toolTipText = info.GetString("TooltipText");
			content = info.GetValue("Content", typeof(object));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#else
			;
#endif


			info.AddValue("Name", name); // String
			info.AddValue("TooltipText", toolTipText); // String
			info.AddValue("Content", content); // Value
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Creates the <see cref="Control"/> that knows how to display the <see cref="content"/> in a form.
		/// </summary>
		/// <returns>The <see cref="Control"/> to be displayed in the <see cref="WorksheetView"/>
		/// </returns>
		/// <remarks>
		/// The <see cref="content"/> object should implement <see cref="ICreateControl"/> interface and be able to create a <see cref="Control"/> 
		/// object to be displayed as pane in a <see cref="WorkbookView"/>. <see langword="Syncfusion.Windows.Forms.Grid.GridModel"/> will create a
		///  <see langword="Syncfusion.Windows.Forms.Grid.GridControlBase"/> for example.
		/// </remarks>
		public Control CreateControl()
		{
			if (content is ICreateControl)
				return ((ICreateControl) content).CreateControl();
			return null;
		}

		/// <summary>
		/// Gets / sets the name of this worksheet.
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name != value)
				{
					this.name = value;
					OnNameChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="NameChanged"/> event.
		/// </summary>
		/// <param name="e">EventArgs.Empty.</param>
		protected virtual void OnNameChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name);
#else
			;
#endif

			if (NameChanged != null)
				NameChanged(this, e);
		}

		/// <summary>
		/// Gets / sets the object with data to be displayed in this control, e.g. a Syncfusion.Windows.Forms.Grid.GridModel
		/// with grid data. The object should implement <see cref="ICreateControl"/> interface and be able to create a <see cref="Control"/> 
		/// object to be displayed as pane in a <see cref="WorkbookView"/>. <see langword="Syncfusion.Windows.Forms.Grid.GridModel"/> will create a
		///  <see langword="Syncfusion.Windows.Forms.Grid.GridControlBase"/> for example.
		/// </summary>
		public object Content
		{
			get
			{
				return this.content;
			}
			set
			{
				if (this.content != value)
				{
					this.content = value;
					OnContentChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="content"/> property is changed.
		/// </summary>
		/// <param name="e">EventArgs.Empty.</param>
		protected virtual void OnContentChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.Content);
#else
			;
#endif

			if (ContentChanged != null)
				ContentChanged(this, e);
		}

		/// <summary>
		///     Gets / sets the ToolTip text for the tab that will appear when the mouse hovers
		///     over the tab and the TabBarSplitterControl's showToolTips property is True.
		/// </summary>
		[
		DefaultValueAttribute(""),
		]
		public string ToolTipText
		{
			get
			{
				return this.toolTipText;
			}
			set
			{
				if (this.toolTipText != value)
				{
					this.toolTipText = value;
					OnToolTipTextChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="ToolTipTextChanged"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnToolTipTextChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.toolTipText);
#else
			;
#endif

			if (ToolTipTextChanged != null)
				ToolTipTextChanged(this, e);
		}


		/// <summary>
		/// Indicates whether this worksheet is visible in the parent workbook.
		/// </summary>
		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				if (this.visible != value)
				{
					this.visible = value;
					OnVisibleChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="VisibleChanged"/> event.
		/// </summary>
		/// <param name="e">EventArgs.Empty.</param>
		protected virtual void OnVisibleChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.Visible);
#else
			;
#endif

			if (VisibleChanged != null)
				VisibleChanged(this, e);
		}

//		/// <summary>
//		/// The active view for this worksheet model.
//		/// </summary>
//		public object ActiveView
//		{
//			get
//			{
//				return this.activeView;
//			}
//			set
//			{
//				if (value != this.activeView)
//				{
//					this.activeView = value;
//					OnActiveViewChanged(EventArgs.Empty);
//				}
//			}
//		}

//		protected virtual void OnActiveViewChanged(EventArgs e)
//		{
//			if (ActiveViewChanged != null)
//				ActiveViewChanged(this, e);
//		}

		/// <summary>
		/// Returns the <see cref="WorkbookModel"/> this sheet is associated with.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkbookModel Workbook
		{
			get
			{
				return this.workbook;
			}
		}

	}
}
