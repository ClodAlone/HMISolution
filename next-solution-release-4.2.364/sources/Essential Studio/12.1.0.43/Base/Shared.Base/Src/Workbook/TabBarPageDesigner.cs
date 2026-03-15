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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	///       Provides design-time functionality for <see cref="TabBarPage"/>
	///       controls.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class TabBarPageDesigner : ScrollableControlDesigner 
	{
		/// <override/>
		public override bool CanBeParentedTo(IDesigner parentDesigner)  
		{ 
			return 
				parentDesigner is TabBarSplitterControlDesigner;
		} 

		/// <override/>
		public override SelectionRules SelectionRules 
		{
			get 
			{
				SelectionRules sr = base.SelectionRules;
				Control c = Control;
				if (
					c.Parent is TabBarSplitterControl)
				{
					sr = sr & ~SelectionRules.AllSizeable;
				}
				return sr;
			}
		}

		private void DrawBorder(Graphics graphics)  
		{
			Control control = (Control) this.Component;
			Rectangle bounds = control.ClientRectangle;
			Color backColor = control.BackColor;

			Color color;
			if (((double) backColor.GetBrightness()) < 0.5) 
				color = ControlPaint.Light(control.BackColor);
			else
				color = ControlPaint.Dark(control.BackColor);

			Pen pen = new Pen(color);
			pen.DashStyle = DashStyle.Dash;
			
			bounds.Width = (bounds.Width - 1);
			bounds.Height = (bounds.Height - 1);
			
			graphics.DrawRectangle(pen,bounds);
			pen.Dispose();
		}


		/// <override/>
		protected override/*ParentControlDesigner*/ void OnPaintAdornments(PaintEventArgs pe)  
		{
			this.DrawBorder(pe.Graphics);
			base.OnPaintAdornments(pe);
		}

	}
}
