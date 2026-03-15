#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Renders the UpDownBase control.
    /// </summary>
    public interface IUpDownRenderer
    {
        /// <summary>
        /// Draws arrow.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle where the arrow will be drawn.</param>
        /// <param name="buttonState">State of button where the arrow will be drawn.</param>
        /// <param name="upper">Indicates whether the arrow is upper.</param>
        void DrawArrow(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, bool upper);
  
        /// <summary>
        /// Draws button's background, border and arrow.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        /// <param name="buttonID">Indicates whether the arrow is up-button or down-button.</param>
        void DrawScrollButton(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, ButtonID buttonID);
  
        /// <summary>
        /// Draws button's background.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        void DrawScrollButtonBackground(Graphics g, Rectangle buttonRectangle, ButtonState buttonState);
    
        /// <summary>
        /// Draws up and down buttons.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="upButtonState">State of up-button.</param>
        /// <param name="downButtonState">State of down-button.</param>
        void Render(Graphics g, ButtonState upButtonState, ButtonState downButtonState);
     
        /// <summary>
        /// Draws button's border.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        void DrawScrollButtonBorder(Graphics g, Rectangle buttonRectangle, ButtonState buttonState);
      
        /// <summary>
        /// Recalculates and sets button's rectangles.
        /// </summary>
        void Layout();
    }

    public interface IUpDownButtonsOrientable
    {
        /// <summary>
        /// Gets Up-Down button's orientation.
        /// </summary>
        Orientation SpinOrientation 
        { 
            get; 
        }
        event EventHandler SpinOrientationChanged;
    }
}
