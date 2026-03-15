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

#region file using directives

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Documentation;
using System;

#endregion

namespace Syncfusion.Windows.Forms.Chart.Scrolling
{
	/// <summary>
	/// Represents the horizontal scrollbar.
	/// </summary>
	[ToolboxItem(false), DocumentationExclude()]
	public sealed class ChartHScrollBar : ChartScrollBar
	{
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartHScrollBar"/> class.
		/// </summary>
		/// <internalonly/>
		public ChartHScrollBar()
			: base()
		{
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Creates the scroll bar.
		/// </summary>
		/// <returns></returns>
		/// <internalonly/>
		protected override ScrollBar CreateScrollBar()
		{
			return new HScrollBar();
		}
		/// <summary>
		/// Arranges controls.
		/// </summary>
		protected override void OnLayout()
		{
			if (m_zoomButton.Visible)
			{
				m_zoomButton.Bounds = new Rectangle(0, 0, m_zoomButton.Width, this.Height);
				m_scrollBar.Bounds = new Rectangle(m_zoomButton.Right, 0, this.Width - m_zoomButton.Width, this.Height);
			}
			else
			{
				m_scrollBar.Bounds = this.ClientRectangle;
			}
		}
		/// <summary>
		/// Resets the scrollbar.
		/// </summary>
		/// <internalonly/>
		[Obsolete( "This method isn't used anymore and might be deleted." )]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ResetScrollBar()
		{
			if (this.Visible && this.ShouldPosition)
			{
				int dimension = this.Dimension;
				RectangleF axisRect = this.ChartAxis.Rect;
				Rectangle hScrollBounds = new Rectangle((int)axisRect.X, ChartArea.Bounds.Bottom - Dimension,
					(int)axisRect.Width, Dimension);
				this.SetPosition(hScrollBounds);
			}
		}
		#endregion
	}
}