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

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Documentation;
using System;

namespace Syncfusion.Windows.Forms.Chart.Scrolling
{
	/// <summary>
	/// Represents the vertical scrollbar.
	/// </summary>
	/// <internalonly/>
	[ ToolboxItem( false ), DocumentationExclude() ]
	public class ChartVScrollBar : ChartScrollBar
	{
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartVScrollBar"/> class.
		/// </summary>
		/// <internalonly/>
		public ChartVScrollBar()
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
			return new VScrollBar();
		}
		/// <summary>
		/// Arranges controls.
		/// </summary>
		protected override void OnLayout()
		{
			if (m_zoomButton.Visible)
			{
				m_zoomButton.Bounds = new Rectangle(0, 0, this.Width, m_zoomButton.Height);
				m_scrollBar.Bounds = new Rectangle(0, m_zoomButton.Bottom, this.Width, this.Height - m_zoomButton.Height);
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
		[Obsolete("This method isn't used anymore and might be deleted.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ResetScrollBar()
		{
			if( this.Visible && this.ShouldPosition )
			{
				int dimension = this.Dimension;
        RectangleF axisRect = this.ChartAxis.Rect;
				Rectangle vScrollBounds = new Rectangle(ChartArea.Bounds.Right - Dimension, (int)axisRect.Top,
          this.Dimension, (int)axisRect.Height);
        this.SetPosition( vScrollBounds );
			}
		}
		#endregion
	}
}