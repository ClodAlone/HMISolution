#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region File Using

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Represents NumericUpDown for MonthCalendarAdv.
    /// </summary>
    [ToolboxItem(false)]
    public class YearNumericUpDown : NumericUpDownExt
    {
        #region Class Initialize/Finalize Methods
        public YearNumericUpDown()
            : base()
        {
        }
        #endregion

        #region Class Overrides

        /// <override/>
        /// <summary>
        /// Draws background of the up/down button for MonthCalendarAdv with Office2007 visual style.
        /// </summary>
        /// <param name="g"> Graphics object</param>
        /// <param name="rect">Rectangle object</param>
        /// <param name="button">Button ID</param>
        /// <param name="state">Button State</param>
        protected override void DrawOffice2007ButtonBackground(Graphics g, Rectangle rect, ButtonID button, ButtonState state)
        {
            base.DrawOffice2007ButtonBackground(g, rect, button, state);

            Color startColor = Color.Empty;
            Color endColor = Color.Empty;

            Brush fillBrush = new SolidBrush(SystemColors.Window);

            if (state == ButtonState.Normal)
            {
                startColor = (button == ButtonID.Up) ? Office2007Colors.Default.MenuItemLightColor :
                    Office2007Colors.Default.MenuItemDarkColor;
                endColor = (button == ButtonID.Up) ? Office2007Colors.Default.MenuItemDarkColor :
                    Office2007Colors.Default.MenuItemLightColor;

                fillBrush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Vertical);
            }
            else if (state == ButtonState.Pushed)
            {
                startColor = (button == ButtonID.Up) ? Office2007Colors.Default.BarItemPressLightColor :
                    Office2007Colors.Default.BarItemPressDarkColor;
                endColor = (button == ButtonID.Up) ? Office2007Colors.Default.BarItemPressDarkColor :
                    Office2007Colors.Default.BarItemPressLightColor;

                fillBrush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Vertical);
            }

            using (fillBrush)
            {
                g.FillRectangle(fillBrush, rect);
            }
        }

        /// <override/>
        /// <summary>
        /// Draws border of the up/down button for MonthCalendarAdv with Office2007 visual style.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="rect"> Rectangle object</param>
        /// <param name="button"> Button ID</param>
        /// <param name="state"> Button State</param>
        protected override void DrawOffice2007ButtonBorder(Graphics g, Rectangle rect, ButtonID button, ButtonState state)
        {
            base.DrawOffice2007ButtonBorder(g, rect, button, state);

            Color borderColor = Office2007Colors.Default.NumericUpDownBorderColor;

            if (state == ButtonState.Normal)
            {
                borderColor = Office2007Colors.Default.BarItemHighlightBorderColor;
            }
            else if (state == ButtonState.Pushed)
            {
                borderColor = Office2007Colors.Default.BarItemPressBorderColor;
            }

            using (Pen borderPen = new Pen(borderColor))
            {
                g.DrawRectangle(borderPen, rect);
            }
        }

        protected override void Buttons_Paint(object sender, PaintEventArgs e)
        {
            if (this.VisualStyle == VisualStyle.Office2007)
            {
                this.DrawOffice2007Buttons(e.Graphics);
            }
            else
            {
                base.Buttons_Paint(sender, e);
            }
        }
        #endregion
    }
}
