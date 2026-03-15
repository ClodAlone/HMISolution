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

namespace Syncfusion.Windows.Forms.Chart.Scrolling
{

	/// <internalonly/>
	[ ToolboxItem( false ), DocumentationExclude() ]
	public class ChartZoomButton : Button
	{

		/// <internalonly/>
		public ChartZoomButton()
		{
		}

		/// <internalonly/>
		protected override void OnPaint( PaintEventArgs args )
		{
			base.OnPaint( args );
			Pen pen = new Pen( this.ForeColor );
			Rectangle rc = this.ClientRectangle;
			rc.Inflate( -4, -4 );
			args.Graphics.DrawEllipse( pen, rc );
			args.Graphics.DrawLine( pen, rc.Right, rc.Top, rc.Left, rc.Bottom );
		}
	}
}