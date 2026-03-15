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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{

    /// <summary>
    ///    WorksheetView implements a single page in a <see cref="WorkbookView"/>. It is essentially a panel that hosts a child control
    ///    that is created from the <see cref="WorksheetModel.Content"/> of a <see cref="WorksheetModel"/>.
    /// </summary>
    /// <remarks>
	/// Multiple <see cref="WorksheetView"/> controls can share the same <see cref="WorksheetModel"/>. Each <see cref="WorksheetView"/>
	/// that is displayed in a <see cref="WorkbookView"/> is associated with a <see cref="WorksheetModel"/> from the <see cref="WorkbookModel.Worksheets"/>
	/// collection in a <see cref="WorkbookModel"/> object.
	/// </remarks>
	[ToolboxItem(false)]
	public class WorksheetView : TabBarPage
    {
        private WorksheetModel worksheet;
        private WorkbookView workbookView;
               
		/// <summary>
		/// Initializes a new <see cref="WorksheetView"/> for an existing <see cref="WorksheetModel"/> and a <see cref="WorkbookView"/>
		/// that displays this sheet.
		/// </summary>
		/// <param name="worksheet">The <see cref="WorksheetModel"/> that manages the data for this view.</param>
		/// <param name="workbookView">The <see cref="WorkbookView"/> that displays this sheet as a page.</param>
        public WorksheetView(WorksheetModel worksheet, WorkbookView workbookView)
        {
            this.worksheet = worksheet;
            this.workbookView = workbookView;
			FetchWorksheet();
			WireEvents();
		}

		/// <override/>
		protected override void InitLayout()  
		{
			Control c = worksheet.CreateControl();
			if (c != null)
			{
				c.Visible = true;
				Controls.Add(c);
			}
		}
		
		void FetchWorksheet()
		{
			this.Text = worksheet.Name;
			this.ToolTipText = worksheet.ToolTipText;
			this.Active = false;
			BackColor = Color.FromArgb (255, 255, 128);
			Active = false;
			SplitBars = DynamicSplitBars.Both;
		}

		/// <override/>
		protected override void OnTextChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.Text);
#else
			;
#endif

			base.OnTextChanged(e);
			worksheet.Name = this.Text;
		}
		
		/// <override/>
		protected override void OnToolTipTextChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.ToolTipText);
#else
			;
#endif

			base.OnToolTipTextChanged(e);
			worksheet.ToolTipText = this.ToolTipText;
		}
		
		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				UnwireEvents();
			}
			base.Dispose(disposing);
		}

		void WireEvents()
		{
			worksheet.NameChanged += new EventHandler(OnWorksheetNameChanged);
			worksheet.ToolTipTextChanged += new EventHandler(OnWorksheetToolTipChanged);
			// workbook.ActiveViewChanged += ...
		}

		void UnwireEvents()
		{
			worksheet.NameChanged -= new EventHandler(OnWorksheetNameChanged);
			worksheet.ToolTipTextChanged -= new EventHandler(OnWorksheetToolTipChanged);
		}

		void OnWorksheetNameChanged(object sender, EventArgs e)
		{
			this.Text = worksheet.Name;
		}

		void OnWorksheetToolTipChanged(object sender, EventArgs e)
		{
			this.ToolTipText = worksheet.ToolTipText;
		}

		/// <summary>
		/// Returns the <see cref="WorkbookView"/> that displays this sheet as a page.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkbookView WorkbookView
        {
            get
            {
                return this.workbookView;
            }
        }

		/// <summary>
		/// Returns the <see cref="WorksheetModel"/> that manages the data for this view.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorksheetModel Worksheet
        {
            get
            {
                return this.worksheet;
            }
        }

    }
}
