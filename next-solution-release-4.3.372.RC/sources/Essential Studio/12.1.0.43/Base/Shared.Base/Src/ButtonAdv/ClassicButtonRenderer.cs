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
	/// Render button in classic style
	/// </summary>
	internal class ClassicButtonRenderer : ButtonRenderer
	{
		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="button"/>
		public ClassicButtonRenderer( ButtonAdv button ) : base( button )
		{
		}

		#endregion

		#region Class overrides
		/// <summary></summary>
		/// <param name="g"/>
		public override void Render( Graphics g )
		{
			Rectangle buttonRect = this.Button.ClientRectangle;
            if (!buttonRect.IsEmpty)
            {
                if (IsPressed(this.Button))
                {
                    if (IsMouseOver(this.Button))
                    {
                        ControlPaint.DrawButton(g, buttonRect, ButtonState.Normal);

                        ControlPaint.DrawSelectionFrame(g, true,
                            new Rectangle(1, 1, Button.Bounds.Width - 1, Button.Bounds.Height - 1),
                            new Rectangle(2, 2, Button.Bounds.Width - 3, Button.Bounds.Height - 3),
                            this.Button.BackColor);
                    }

                    ControlPaint.DrawButton(g, buttonRect, ButtonState.Pushed);
                }
                else
                {
                    ControlPaint.DrawButton(g, buttonRect, ButtonState.Normal);
                }
                DrawTextAndImage(g);
            }
		}
		#endregion
	}
}