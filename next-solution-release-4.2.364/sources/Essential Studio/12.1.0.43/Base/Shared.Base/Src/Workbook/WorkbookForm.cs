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
	///    A form that hosts a <see cref="WorkbookView"/>. The <see cref="WorkbookView"/> fills the whole form.
	/// </summary>
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
	public class WorkbookForm : Form,
		IThemedControl
	{
		WorkbookView workbookView;
		private System.ComponentModel.Container components;

		/// <summary>
		/// Overloaded. Initializes a new <see cref="WorkbookForm"/> and associates it with a <see cref="WorkbookView"/>.
		/// </summary>
		/// <param name="workbookView">The view to be displayed in the form.</param>
		public WorkbookForm(WorkbookView workbookView)
		{
			if (workbookView == null)
				throw new ArgumentNullException("workbookView");

			this.workbookView = workbookView;
			InitializeWorkbookView();
		}

		/// <summary>
		/// Initializes a new <see cref="WorkbookForm"/> and associates it with a new default <see cref="WorkbookView"/>
		/// that is created for the specified <see cref="WorkbookModel"/>.
		/// </summary>
		/// <param name="workbookModel">The model for the workbook view to be displayed in the form.</param>
		public WorkbookForm(WorkbookModel workbookModel)
		{
			if (workbookModel == null)
				throw new ArgumentNullException("workbookModel");

			this.workbookView = new WorkbookView(workbookModel);
			InitializeWorkbookView();
		}

		/// <summary>
		/// Initializes a new <see cref="WorkbookForm"/>.
		/// </summary>
		protected WorkbookForm()
		{
		}

		/// <summary>
		///    Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		void InitializeWorkbookView()
		{
			this.components = new System.ComponentModel.Container ();
			workbookView.DisabledColor = System.Drawing.SystemColors.GrayText;
			workbookView.Size = this.Size;
			workbookView.RelativeWidth = 64;
			workbookView.ButtonLook = ButtonLook.Flat;
			workbookView.Dock = System.Windows.Forms.DockStyle.Fill;
			workbookView.Resizable = true;
			workbookView.ScrollButtons = DisplayArrowButtons.All;
			workbookView.RepeatClickDelay = 10;
			workbookView.TabIndex = 0;
			workbookView.ShowToolTips = true;
			workbookView.EnabledColor = System.Drawing.SystemColors.WindowText;
			workbookView.ThemesEnabled = this.ThemesEnabled;
			this.Controls.Add(workbookView);
		}

		/// <summary>
		/// Gets / sets the view that is displayed in the form.
		/// </summary>
		public WorkbookView WorkbookView
		{
			get
			{
				return workbookView;
			}
			set
			{
				workbookView = value;
			}
		}

		/// <summary>
		/// Raises the ThemeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnThemeChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnThemeChanged in a derived
		/// class, be sure to call the base class's OnThemeChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemeChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.ThemesEnabled);
#else
			;
#endif

			if(this.ThemeChanged != null)
			{
				try
				{
					this.ThemeChanged(this, e);
				}
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;
				}
			}
			if (workbookView != null)
				workbookView.ThemesEnabled = this.ThemesEnabled;
		}

		/// <summary>
		/// Indicates whether themes are enabled for this control.
		/// </summary>
		public virtual bool ThemesEnabled
		{
			get{return this.themesEnabled;}
			set
			{
				if(this.themesEnabled != value)
				{
					this.themesEnabled = value;
					this.OnThemeChanged(EventArgs.Empty);
				}
			}
		}

		private bool themesEnabled = false;

		/// <summary>
		/// Fired when the ThemesEnabled property changes.
		/// </summary>
		public event EventHandler ThemeChanged;

	}
}
