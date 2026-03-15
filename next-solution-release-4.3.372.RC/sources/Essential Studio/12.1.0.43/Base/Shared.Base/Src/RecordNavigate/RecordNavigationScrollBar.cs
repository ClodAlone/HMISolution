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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// RecordNavigationScrollBar is a <see cref="RecordNavigationBar"/> with a scrollbar contained in one control.<para/>
	/// <see cref="RecordNavigationControl"/> displays this control in the bottom left corner of the frame.
	/// </summary>
	[ToolboxItem(false)]
	public class RecordNavigationScrollBar : RecordNavigationBar, IScrollBarContainer
	{

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal Control scrollBar = null; // shared Control

		int prefWidth = 216;
		private Rectangle fillBounds;

		/// <summary>
		/// Initializes a new <see cref="RecordNavigationScrollBar"/> with a shared scroll bar control.
		/// </summary>
		/// <param name="scrollBar">The control that either hosts a scrollbar (a <see cref="IScrollBarContainer"/>) or is itself a scrollbar.</param>
		public RecordNavigationScrollBar(Control scrollBar)
		{
			this.scrollBar = scrollBar;
		}

		/// <override/>
		protected override void OnHandleCreated(EventArgs e)
		{
			if (scrollBar != null)
			{
				if (!Controls.Contains(scrollBar))
					Controls.Add(scrollBar);
				if (!scrollBar.IsHandleCreated)
					scrollBar.CreateControl();
			}
#if DEBUG
			if (Switches.RecordNavigationBarEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Handle);
#endif

			base.OnHandleCreated(e);
		}

		/// <override/>
		protected override Rectangle ComputeButtonBarChildBounds()
		{
			Rectangle r = this.ClientRectangle;
			r.Width = Math.Min(r.Width, prefWidth);
			if (this.RightToLeft == RightToLeft.Yes)
				r.X = ClientRectangle.Right - r.Right;
			return r;
		}

		/// <override/>
		protected override void OnPaint(PaintEventArgs pe)
		{
#if DEBUG
			if (Switches.RecordNavigationBarEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(pe.ClipRectangle);
#endif

			base.OnPaint(pe);
			if (Parent != null)
			{
				Brush parentBrush = new SolidBrush(Color.White);//Parent.BackColor);
				Rectangle r = ClientRectangle;
				pe.Graphics.FillRectangle(parentBrush, fillBounds);
				parentBrush.Dispose();
			}
		}

		/// <summary>
		/// Gets / sets the preferred width of this control.
		/// </summary>
		public int PrefWidth
		{
			get
			{
				return prefWidth;
			}
			set
			{
				prefWidth = value;
			}
		}


		/// <override/>
		protected override void OnLayout(LayoutEventArgs levent)
		{
#if DEBUG
			if (Switches.RecordNavigationBarEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(levent.AffectedProperty);
#endif

			base.OnLayout(levent);

			if (this.scrollBar != null)
			{
				Rectangle tbr = ButtonBarChild.Bounds;
				this.fillBounds = new Rectangle(tbr.Right, tbr.Top, 10, tbr.Height);
				tbr.Width += 10;
				Rectangle r = new Rectangle(tbr.Right, tbr.Top, this.Width-tbr.Right, tbr.Height);
				if (this.RightToLeft == RightToLeft.Yes)
				{
					tbr.X -= 10;
					r = new Rectangle(tbr.Width, tbr.Top, this.Width-tbr.Width, tbr.Height);
					r.X = this.ClientRectangle.Right - r.Right;
				}
				this.scrollBar.Bounds = r;
				this.Invalidate();
			}
		}

		Control IScrollBarContainer.ScrollBar
		{
			get
			{
                return this.scrollBar;
			}
			set
			{
				if (value != this.scrollBar)
				{
					Controls.Remove(this.scrollBar);
					this.scrollBar = value;
					Controls.Add(this.scrollBar);
				}
			}
		}


	}
}
