#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Any infringement will be prosecuted under
//  applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.ComponentModel;
using Syncfusion.Drawing;

namespace Syncfusion.GridHelperClasses
{
	/// <summary>
	/// Implements the data/model part for a Xhtml cell.
	/// </summary>
	/// <remarks>
	/// You typically access cell models through the <see cref="GridModel.CellModels"/>
	/// property of the <see cref="GridModel"/> class.<para/>
	/// A <see cref="XhtmlCellModel"/> can serve as model for several <see cref="XhtmlCellRenderer"/>
	/// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
	/// <para/>
	/// See <see cref="XhtmlCellRenderer"/> for more detailed information about this cell type.
	/// </remarks>
	[Serializable]
	public class XhtmlCellModel: GridCellModelBase
	{
		/// <overload>
		/// Initializes a new <see cref="XhtmlCellModel"/> object.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="XhtmlCellModel"/> object 
		/// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
		/// </summary>
		/// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>	
		/// <remarks>
		/// You typically access cell models through the <see cref="GridModel.CellModels"/>
		/// property of the <see cref="GridModel"/> class.
		/// </remarks>
		public XhtmlCellModel(GridModel grid)
			: base(grid)
		{
			ButtonBarSize = new Size(21, 0);
		}

		/// <summary>
		/// Initializes a new <see cref="XhtmlCellModel"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
		protected XhtmlCellModel(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			TraceUtil.TraceCurrentMethodInfoIf(true, info.FullTypeName, info.MemberCount);
			ButtonBarSize = new Size(21, 0);
		}

		/// <override/>
		public override GridCellRendererBase CreateRenderer(GridControlBase control)
		{
			return new XhtmlCellRenderer(control, this);
		}
	}


	
	/// <summary>
	/// Implements the renderer part of a Xhtml cell.
	/// </summary>
	/// <remarks>
	/// Use "XhtmlCell" as identifier in CellType of a cells <see cref="GridStyleInfo"/>
	///  to associate this cell type with a cell. 
	/// <para/>
	/// This renderer supports editing the contents of the Xhtml with a dropdown
	/// panel. When the user drops the panel a <see cref="XhtmlEntryPanel"/> is shown
	/// and the user can format the text and then accept changes by pressing "Save" button.
	/// </remarks>
	public class XhtmlCellRenderer: GridCellRendererBase
	{
		Syncfusion.GridHelperClasses.XhtmlEntryPanel panel = null;
		RichTextBox richTextBox = new RichTextBox();

		/// <summary>
		/// Initializes a new XhtmlCellRenderer object for the given GridControlBase
		/// and GridCellModelBase.
		/// </summary>
		/// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
		/// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
		/// be shared among views.</param>
		/// <remarks>References to GridControlBase, 
		/// and GridCellModelBase will be saved.</remarks>
		public XhtmlCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
			: base(grid, cellModel)
		{
			DropDownPart = new GridDropDownCellImp(this);
			DropDownButton = new GridCellComboBoxButton(this);
		}

		/// <override/>
		protected override void InitializeDropDownContainer()
		{
			base.InitializeDropDownContainer();

			panel = new XhtmlEntryPanel();
			panel.KeyDown += new KeyEventHandler(PanelKeyDown);
			panel.RichTextBox.Text = this.ControlText;

			((GridDropDownContainer)this.DropDownContainer).Controls.Add(panel);
		}

		/// <override/>
		protected override void OnInitialize(int rowIndex, int colIndex)
		{
			GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
			ControlValue = style.CellValue;
			ControlText = style.Text;
			base.OnInitialize(rowIndex, colIndex);
		}

		/// <summary>
		/// Event handler for the KeyDown event of the <see cref="XhtmlEntryPanel"/>
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A KeyEventArgs that contains the event data. </param>
		protected virtual void PanelKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && Control.ModifierKeys == Keys.None
				|| e.KeyCode == Keys.Down && Control.ModifierKeys == Keys.Alt
				|| e.KeyCode == Keys.F2 && Control.ModifierKeys == Keys.None
				|| e.KeyCode == Keys.F4 && Control.ModifierKeys == Keys.None)
			{
				CurrentCell.CloseDropDown(PopupCloseType.Done);
			}
		}

		/// <summary>
		/// Event handler for the <see cref="XhtmlEntryPanel.Save"/> event of the <see cref="XhtmlEntryPanel"/>
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Event data. </param>
		protected virtual void PanelSave(object sender, EventArgs e)
		{
			CurrentCell.CloseDropDown(PopupCloseType.Done);
		}

		/// <summary>
		/// Event handler for the <see cref="XhtmlEntryPanel.Cancel"/> event of the <see cref="XhtmlEntryPanel"/>
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Event data. </param>
		protected virtual void PanelCancel(object sender, EventArgs e)
		{
			CurrentCell.CloseDropDown(PopupCloseType.Canceled);
		}
		
		/// <override/>
		public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
		{
			GridDropDownContainer dropdown = (GridDropDownContainer)this.DropDownContainer;
			dropdown.PopupHost.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			dropdown.PopupHost.Size = new Size(370, 240);
			dropdown.Dock = DockStyle.Fill;
			GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
			panel.RichTextBox.Font = style.GdipFont;
			if (this.HasControlText)
				panel.RichTextBox.Text = ControlText;
			else
				panel.RichTextBox.Text = style.Text;
			panel.RichTextBox.BackColor = style.BackColor;
			panel.RichTextBox.ForeColor = style.TextColor;
			panel.Dock = DockStyle.Fill;
			panel.Save += new EventHandler(PanelSave);
			panel.Cancel +=new EventHandler(PanelCancel);
			base.DropDownContainerShowingDropDown(sender, e);
		}

		/// <override/>
		public override void DropDownContainerShowedDropDown(object sender, EventArgs e)
		{
			panel.RichTextBox.Focus();
		}

		/// <override/>
		public override void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e)  
		{
			if (e.PopupCloseType == PopupCloseType.Done)
			{
				if (this.NotifyCurrentCellChanging())
				{
					ControlText = panel.RichTextBox.Text;
					this.NotifyCurrentCellChanged();
				}
			}
			Grid.InvalidateRange(GridRangeInfo.Cell(RowIndex, ColIndex), GridRangeOptions.MergeCoveredCells); // Merge all cells
			base.DropDownContainerCloseDropDown(sender, e);
		}

		/// <override/>
		protected override bool OnSaveChanges()
		{
			if (HasControlText)
				Grid.Model[RowIndex, ColIndex].CellValue = ControlText;
			return true;
		}

		/// <override/>
		protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style) 
		{
			TraceUtil.TraceCurrentMethodInfoIf(true, clientRectangle, rowIndex, colIndex);
			Rectangle textRectangle;

			int h1 = Grid.Model.RowHeights.GetTotal(rowIndex, Grid.ViewLayout.LastVisibleRow-1);
			int h2 = Grid.Model.RowHeights.GetTotal(rowIndex, Grid.ViewLayout.LastVisibleRow);

			int b1 = clientRectangle.Top + h1;
			int b2 = clientRectangle.Top + h2;
		
			// inactive cell
			textRectangle = RemoveMargins(clientRectangle, style);
			richTextBox.BackColor = style.Interior.BackColor;
			string xhtml;
			
			if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && CurrentCell.IsModified && HasControlText)
				xhtml = ControlText;
			else
				xhtml = style.Text;

			XmlTranslator xmlTranslator;

			// Sending a string of XHTML to the XmlTranslator constructor
			// creates a DOM of the XHTML in the XmlTranslator.
			xmlTranslator = new XmlTranslator(xhtml);

			// The XmlTranslator translates the XHTML into RTF code
			// wrapped in an RtfDocument object.
			string rtf = xmlTranslator.ToRtfDocument().ToString();

			// Any errors are passed from the translator to this text box.
			//ArrayList errors = xmlTranslator.Errors;

			Rectangle clipBounds = GetCellBoundsCore(rowIndex, colIndex, true);
//			richTextBox.BackColor = style.BackColor; 
			RichTextPaint.DrawRichText(g, richTextBox, rtf, Grid.PrintingMode, Grid.GridBounds, textRectangle, clipBounds, style.BackColor, style.WrapText, 100);
		}

	}


}
