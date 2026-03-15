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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	///  Provides functionality for displaying several <see cref="WorksheetView"/> controls in an Excel-like workbook. 
	/// </summary>
	/// <remarks>
	/// A <see cref="WorkbookView"/> is associated with a <see cref="WorkbookModel"/>. The <see cref="WorkbookModel"/>
	/// has a <see cref="WorkbookModel.Worksheets"/> collection. For each of the <see cref="WorksheetModel"/> objects in
	/// the <see cref="WorkbookModel.Worksheets"/> collection of the <see cref="WorkbookModel"/>, a <see cref="WorksheetView"/>
	/// is created and displayed in this <see cref="WorkbookView"/>.
	/// </remarks>
	///	<example>
	///	<code lange="C#">
	///	public class NewWorkbookFile : BasicAction
	///	{
	///		int windowCount = 0;
	///		WorkbookModel workbook;
	///		public override void InvokeAction(object sender, EventArgs e)
	///		{
	///			windowCount++;
	///			workbook = new WorkbookModel("Workbook");
	///			GridModel sheet1 = new GridModel();
	///			SampleGrid.SetupGridModel(sheet1);
	///			GridModel sheet2 = new GridModel();
	///			SampleGrid.SetupGridModel(sheet2);
	///
	///			workbook.Worksheets.Add(new WorksheetModel(workbook, "Sheet 1", sheet1));
	///			workbook.Worksheets.Add(new WorksheetModel(workbook, "Sheet 2", sheet2));
	///
	///			WorkbookForm doc = new WorkbookForm(workbook);
	///			doc.Text = workbook.Name + windowCount;
	///			doc.MdiParent = MainWindow;
	///			doc.Show();
	///		}
	///	}
	/// </code>
	/// </example>
	[ToolboxItem(false)]
	public class WorkbookView : TabBarSplitterControl
	{
		WorkbookModel workbook;
		//ArrayList worksheetViews;
		//object activeWorksheetView;
		Form parentForm;

		/// <summary>
		/// Occurs when the view is activated.
		/// </summary>
		public event EventHandler Activated;

		/// <summary>
		/// Initializes a new <see cref="WorkbookView"/> for the specified <see cref="WorkbookModel"/>.
		/// </summary>
		/// <param name="workbook">The <see cref="WorkbookModel"/> which holds data to be displayed in this view.</param>
		public WorkbookView(WorkbookModel workbook)
		{
			if (workbook == null)
				throw new ArgumentNullException("workbook");

			this.workbook = workbook;
			FetchModel();
			WireWorkbook();
		}

		/// <override/>
		protected override/*Control*/ void OnHandleCreated(EventArgs e)  
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.Handle);
#else
			;
#endif

			base.OnHandleCreated(e);

			// TODO: WorkbookView.Activated 
			// this is fine if we are a DockStyle.Fill in a MDI child but what about other scenarios?
			parentForm = this.Parent as Form;
			if (parentForm != null)
				parentForm.Activated += new EventHandler(OnParentFormActived);
		}

		void OnParentFormActived(object sender, EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.Name);
#else
			;
#endif

			if (Activated != null)
				Activated(this, e);

			workbook.ActiveView = this;
		}

		void FetchModel()
		{
			this.Name = workbook.Name;
			int count = workbook.Worksheets.Count;
			WorksheetView[] views = new WorksheetView[count]; 
			for (int n = 0; n < count; n++)
				views[n] = new WorksheetView(workbook.Worksheets[n], this);
			//worksheetViews = new ArrayList(views);
			Controls.AddRange(views);
		}

		void WireWorkbook()
		{
			workbook.NameChanged += new EventHandler(OnWorkbookNameChanged);
			workbook.Worksheets.SheetMoved += new SheetMovedEventHandler(OnWorksheetMoved);
		}

		void UnwireWorkbook()
		{
			workbook.NameChanged -= new EventHandler(OnWorkbookNameChanged);
			workbook.Worksheets.SheetMoved -= new SheetMovedEventHandler(OnWorksheetMoved);
			if (parentForm != null)
				parentForm.Activated -= new EventHandler(OnParentFormActived);
		}

		void OnWorkbookNameChanged(object sender, EventArgs e) 
		{
			this.Name = workbook.Name;
		}

		/// <override/>
		protected override void TabsMoving(object sender, TabMovedEventArgs e)
		{
			if (this.Validate())
				this.Workbook.Worksheets.Move(e.Tab, e.DestTab);
			e.Cancel = true;
		}

		void OnWorksheetMoved(object sender, SheetMovedEventArgs e) 
		{
			SuspendLayout();
			TabBarPage currentPage = this.ActivePage;
			if (e.Reason == SheetMovedReason.ClearAll)
				TabBarPages.Clear();
			else if (e.Reason == SheetMovedReason.InsertSheet)
			{
				if (e.Index == -1)
					TabBarPages.Add(new WorksheetView(workbook.Worksheets[e.Destination], this));
				else
					TabBarPages.Insert(e.Index, new WorksheetView(workbook.Worksheets[e.Index], this));
			}
			else if (e.Reason == SheetMovedReason.RemoveSheet)
			{
				TabBarPage page = TabBarPages[e.Index];
				TabBarPages.Remove(page);
				Controls.Remove(page);
			}
			else if (e.Reason == SheetMovedReason.MoveSheet)
			{
				TabBarPage page = TabBarPages[e.Index];
				TabBarPages.Remove(page);
				int n = e.Destination;
				if (e.Destination > e.Index)
					n--;
				TabBarPages.Insert(n, page);
			}
			if (TabBarPages.Contains(currentPage))
			{
				this.ActivePage = currentPage;
				Bar.TabBarChild.CurrentTab = TabBarPages.IndexOf(currentPage);
			}
			ResumeLayout(true);
		}


		/// <summary>
		/// Returns the <see cref="WorkbookModel"/> which holds data to be displayed in this view.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkbookModel Workbook
		{
			get
			{
				return workbook;
			}
		}

		/// <summary>
		/// Creates the <see cref="WorksheetViewCollection"/>.
		/// </summary>
		/// <returns>The new <see cref="WorksheetViewCollection"/>.</returns>
		public override TabBarPageCollection OnCreateTabBarPageCollection()
		{
			return new WorksheetViewCollection(this);
		}

		/// <override/>
		protected override Control.ControlCollection CreateControlsInstance()  
		{
			return new ControlCollection(this);
		}

		internal new class ControlCollection : TabBarSplitterControl.ControlCollection
		{
			// Fields
			private WorkbookView owner;

			// Constructors
			public ControlCollection(WorkbookView owner) 
				: base (owner)
			{
				this.owner = owner;
			}

			// Methods
			public override void Add(Control value)  
			{
				WorksheetView view = value as WorksheetView;
				if (view != null)
				{
				}
				base.Add(value);
			}

			public override void Remove(Control value)  
			{
				WorksheetView view = value as WorksheetView;
				if (view != null)
				{
				}
				base.Remove(value);
			}
		}
	}

	/// <summary>
	/// A collection of <see cref="WorksheetView"/> items.
	/// </summary>
	/// <remarks>
	/// You access this collection with the <see cref="TabBarSplitterControl.TabBarPages"/> property of 
	/// a <see cref="WorkbookView"/>.
	/// </remarks>
	public class WorksheetViewCollection: TabBarPageCollection
	{
		WorkbookView owner;

		// Constructors
		/// <summary>
		/// Initializes a new <see cref="WorksheetViewCollection"/> and
		/// associates it with a <see cref="WorkbookView"/>.
		/// </summary>
		/// <param name="owner">The <see cref="WorkbookView"/> that manages this collection.</param>
		public WorksheetViewCollection(WorkbookView owner)  
			: base(owner)
		{
			this.owner = owner;
		}

		// Methods
		/// <override/>
		public override void Add(TabBarPage value)  
		{
			WorksheetView view = value as WorksheetView;
			if (view != null)
			{
			}
			base.Add(value);
		}
	}

}
