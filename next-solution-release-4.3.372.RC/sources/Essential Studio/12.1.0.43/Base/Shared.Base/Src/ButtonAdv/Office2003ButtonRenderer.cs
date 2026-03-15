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

using System.Drawing;

using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Office2003 button style rendering.
	/// </summary>
	internal class Office2003ButtonRenderer : ButtonRenderer
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
		/// <summary></summary>
		private Pen focusBorderPen;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="button"/>
		public Office2003ButtonRenderer( ButtonAdv button ) : base( button )
		{
		}
		/// <summary></summary>
		/// <param name="disposing"></param>
		protected override void OnDispose( bool disposing )
		{
			if( disposing )
			{
				DisposeDrawingObjects();
			}

			base.OnDispose( disposing );
		}

		/// <summary></summary>
		private void DisposeDrawingObjects()
		{
			// dispose objects
			if( grayBrush != null )
			{
				DisposeHelper.Dispose( ref orangeBrush );
				DisposeHelper.Dispose( ref redBrush );
				DisposeHelper.Dispose( ref borderPen );
				DisposeHelper.Dispose( ref focusBorderPen );
				DisposeHelper.Dispose( ref grayBrush );
			}
		}
		#endregion

		#region Class utility methods
		/// <summary></summary>
		private void CreateDrawingObjects()
		{
			grayBrush = new SolidBrush( this.Button.BackColor );
			orangeBrush = new SolidBrush( ButtonOffice2003Colors.MouseOverColor );
			redBrush = new SolidBrush( ButtonOffice2003Colors.PressedColor );

			borderPen = new Pen( ButtonOffice2003Colors.BorderColor, 1 );
			focusBorderPen = new Pen( ButtonOffice2003Colors.FocusBorderColor, 1 );
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
				if( ( this.Button.IsMouseDown ) && IsMouseOver( this.Button ) )
				{
					// Draw an red background
					g.FillRectangle( redBrush, this.bounds );
				}
				else if( IsMouseOver( this.Button ) )
				{
					// Draw an orange background
					g.FillRectangle( orangeBrush, this.bounds );
				}
				else
				{
					// Draw a red background
					g.FillRectangle( redBrush, this.bounds );
				}
			}
			else if( IsDefault( this.Button ) )
			{
				// Draw an gray background
				g.FillRectangle( grayBrush, this.bounds );
			}
			else if( IsMouseOver( this.Button ) )
			{
				// Draw an orange background
				g.FillRectangle( orangeBrush, this.bounds );
			}

			if( this.Button.BorderStyleAdv == ButtonAdvBorderStyle.Default )
			{
				g.DrawRectangle( focusBorderPen, 0, 0, this.bounds.Width - 1, this.bounds.Height - 1 );
			}

			DisposeDrawingObjects();

			DrawTextAndImage( g );
		}
		#endregion
	}
}