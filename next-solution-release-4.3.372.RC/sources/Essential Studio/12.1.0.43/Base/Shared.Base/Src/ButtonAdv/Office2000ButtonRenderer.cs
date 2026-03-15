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
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Summary description for Office2000ButtonRenderer.
	/// </summary>
	internal class Office2000ButtonRenderer : ButtonRenderer
	{
		#region Class constants
		/// <summary></summary>
		private const int OutsidePadding = 1;
		/// <summary></summary>
		private const int InsidePadding = 2;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="button"/>
		public Office2000ButtonRenderer( ButtonAdv button ) : base( button )
		{
		}
		#endregion

		#region Class overrides
		/// <summary></summary>
		/// <param name="g"/>
		public override void Render( Graphics g )
		{
			Rectangle buttonRect = this.Button.ClientRectangle;

			// NOTE: potential BUG! ControlPaint class do not apply correctly 
			// Graphics transform matrixs.

			if( IsPressed( this.Button ) )
			{               
                if ( IsMouseOver( this.Button ) )
                {
                    ControlPaint.DrawButton( g, buttonRect, ButtonState.Flat );

                    ControlPaint.DrawSelectionFrame( g, true,
                        new Rectangle( OutsidePadding, OutsidePadding,
                            Button.Bounds.Width - OutsidePadding,
                            Button.Bounds.Height - OutsidePadding ),
                      new Rectangle( InsidePadding, InsidePadding,
                            Button.Bounds.Width - ( InsidePadding + OutsidePadding ),
                            Button.Bounds.Height - ( InsidePadding + OutsidePadding ) ),
                        this.Button.BackColor );
                }
                else
                {
                    ControlPaint.DrawButton( g, buttonRect, ButtonState.Pushed );
                }

			}
			else if( IsDefault( this.Button ) )
			{
				ControlPaint.DrawButton( g, buttonRect, ButtonState.Flat );
			}
			else if( IsMouseOver( this.Button ) )
			{
				ControlPaint.DrawButton( g, buttonRect, ButtonState.Normal );
			}

			DrawTextAndImage( g );
		}
		#endregion
	}
}