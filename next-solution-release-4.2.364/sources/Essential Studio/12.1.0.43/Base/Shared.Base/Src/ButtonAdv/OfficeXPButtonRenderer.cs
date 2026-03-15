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
using System.Drawing;

using Syncfusion.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Summary description for OfficeXPButtonRenderer.
	/// </summary>
	internal class OfficeXPButtonRenderer : ButtonRenderer
	{
		#region Class members
		/// <summary></summary>
		private Brush grayBrush;
		/// <summary></summary>
		private Brush orangeBrush;
		/// <summary></summary>
		private Brush redBrush;
		/// <summary></summary>
		private Pen borderPen;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="button"/>
		public OfficeXPButtonRenderer( ButtonAdv button ) : base( button )
		{
			grayBrush = new SolidBrush( button.BackColor );
			orangeBrush = new SolidBrush( Color.FromArgb( 233, 234, 237 ) );
			redBrush = new SolidBrush( Color.FromArgb( 132, 146, 181 ) );
			borderPen = new Pen( ButtonOfficeXPColors.BorderColor, 1 );
		}
		/// <summary></summary>
		/// <param name="disposing"></param>
		protected override void OnDispose( bool disposing )
		{
			if( disposing )
			{
				DisposeHelper.Dispose( ref grayBrush );
				DisposeHelper.Dispose( ref orangeBrush );
				DisposeHelper.Dispose( ref redBrush );
				DisposeHelper.Dispose( ref borderPen );
			}

			base.OnDispose( disposing );
		}
		#endregion

        #region Class utility methods
        /// <summary></summary>
        private void CreateDrawingObjects()
        {
            grayBrush = new SolidBrush( this.Button.BackColor );
            orangeBrush = new SolidBrush( Color.FromArgb( 233, 234, 237 ) );
            redBrush = new SolidBrush( Color.FromArgb( 132, 146, 181 ) );
            borderPen = new Pen( ButtonOfficeXPColors.BorderColor, 1 );
        }
        #endregion

		#region Class overrides
		/// <summary></summary>
		/// <param name="g"/>
		public override void Render( Graphics g )
		{
            this.CreateDrawingObjects();

			if( IsPressed( this.Button ) )
			{
				if( IsMouseOver( this.Button ) )
				{
					// Draw a orangered background
					using(Brush brush = new SolidBrush( Color.OrangeRed ))
						g.FillRectangle(brush, this.bounds);
				}
				else
				{
					// Draw a red background
					g.FillRectangle( redBrush, this.bounds );
				}
			}
			else if( IsDefault( this.Button ) )
			{
				// Draw a gray background
				g.FillRectangle( grayBrush, this.bounds );
			}
			else if( IsMouseOver( this.Button ) )
			{
				// Draw an orange background
				g.FillRectangle( orangeBrush, this.bounds );
			}

			if( this.Button.BorderStyleAdv == ButtonAdvBorderStyle.Default )
			{
				g.DrawRectangle( borderPen, 0, 0, this.bounds.Width - 1, this.bounds.Height - 1 );
			}

			DrawTextAndImage( g );
		}
		#endregion
	}
}