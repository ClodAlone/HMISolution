#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    public enum Direction
    {
        Right = 0,
        Left,
        Up,
        Down
    }

    [ToolboxItem(false)]
    public class Splitter : SplitContainer
    {
        public Splitter()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.KeyPress += new KeyPressEventHandler(Splitter_KeyPress);
            this.KeyUp += new KeyEventHandler(Splitter_KeyUp);
        }
       

        #region private properties

        private System.Drawing.Color hotColor = GetAlphaBlendColor(SystemColors.Highlight, SystemColors.Window, 70);

        #endregion

        private void Splitter_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.Invalidate();
            e.Handled = false;
        }

        private void Splitter_KeyUp(object sender, KeyEventArgs e)
        {
            this.Invalidate();
            e.Handled = false;
        }


        /// <summary>
        /// // solid color obtained as a result of alpha-blending
        /// </summary>
        /// <param name="front">front color</param>
        /// <param name="back">back color</param>
        /// <param name="alpha">alpha blend value</param>
        /// <returns>color</returns>
        private static Color GetAlphaBlendColor(Color front, Color back, int alpha)
        {
            Color frontColor = Color.FromArgb(255, front);
            Color backColor = Color.FromArgb(255, back);

            float frontRed = frontColor.R;
            float frontGreen = frontColor.G;
            float frontBlue = frontColor.B;
            float backRed = backColor.R;
            float backGreen = backColor.G;
            float backBlue = backColor.B;

            float fRed = frontRed * alpha / 255 + backRed * ((float)(255 - alpha) / 255);
            byte newRed = (byte)fRed;
            float fGreen = frontGreen * alpha / 255 + backGreen * ((float)(255 - alpha) / 255);
            byte newGreen = (byte)fGreen;
            float fBlue = frontBlue * alpha / 255 + backBlue * ((float)(255 - alpha) / 255);
            byte newBlue = (byte)fBlue;

            return Color.FromArgb(255, newRed, newGreen, newBlue);
        }

        /// <summary>
        /// returns an array of points
        /// </summary>
        private Point[] ArrayPoint(int x, int y, Direction direction)
        {
            Point[] point = new Point[3];

            // decide which direction the arrow will point
            if (direction == Direction.Right)
            {
                // right arrow
                point[0] = new Point(x, y);
                point[1] = new Point(x + 3, y + 3);
                point[2] = new Point(x, y + 6);
            }
            if (direction == Direction.Left)
            {
                // left arrow
                point[0] = new Point(x + 3, y);
                point[1] = new Point(x, y + 3);
                point[2] = new Point(x + 3, y + 6);
            }
            if (direction == Direction.Up)
            {
                // up arrow
                point[0] = new Point(x + 3, y);
                point[1] = new Point(x + 6, y + 4);
                point[2] = new Point(x, y + 4);
            }
            if (direction == Direction.Down)
            {
                // down arrow
                point[0] = new Point(x, y);
                point[1] = new Point(x + 2, y + 3);
                point[2] = new Point(x + 5, y);
            }
            return point;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            Rectangle r = this.ClientRectangle;
            g.FillRectangle(new SolidBrush(this.BackColor), r);


            if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                this.SplitterWidth = 9;
                int recWidth = SplitterRectangle.Width / 3;
                Rectangle split_rect = new Rectangle(SplitterRectangle.X + recWidth, SplitterRectangle.Y, SplitterRectangle.Width - recWidth, SplitterRectangle.Height);

                int x = split_rect.X;
                int y = split_rect.Y + 3;

                g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + 2);
                g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + recWidth, y);
                g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 2, x + recWidth, y + 2);
                g.DrawLine(new Pen(SystemColors.ControlDark), x + recWidth, y, x + recWidth, y + 2);
            }
            else
            {
                this.SplitterWidth = 9;
                int recHeight = SplitterRectangle.Height / 3;
                Rectangle split_rect = new Rectangle(SplitterRectangle.X, SplitterRectangle.Y + recHeight, SplitterRectangle.Width, SplitterRectangle.Height - 2 * recHeight);

                int x = split_rect.X + 3;
                int y = split_rect.Y;

                g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 2, y);
                g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + recHeight);
                g.DrawLine(new Pen(SystemColors.ControlDark), x + 2, y, x + 2, y + recHeight);
                g.DrawLine(new Pen(SystemColors.ControlDark), x, y + recHeight, x + 2, y + recHeight);
            }
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }
    }
}