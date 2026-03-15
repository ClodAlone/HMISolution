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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    [ToolboxItem(false)]
    internal partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            InitializeComponent();
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(this.searchPreviousBtn, "Previous");
            tooltip.SetToolTip(this.searchNextBtn, "Next");
            tooltip.SetToolTip(this.searchCloseBtn, "Close search bar");
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, 275, 30, specified);
        }

        #region events
        private void SearchBox_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath graphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
            float curveValue = 10F;
            RectangleF leftRectangle = new RectangleF(0, this.Height - curveValue, curveValue, curveValue);
            RectangleF rightRectangle = new RectangleF(this.Width - curveValue + 1, this.Height - curveValue, curveValue, curveValue);
            graphicsPath.AddRectangle(new RectangleF(0, 0, this.Width, this.Height - curveValue / 2));

            graphicsPath.AddArc(leftRectangle, -270F, 90F);
            graphicsPath.AddArc(rightRectangle, 360F, 90F);
            this.BackColor = Color.FromArgb(255, 180, 203, 255);
            this.Region = new Region(graphicsPath);
        }

        private void allButton_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle != Rectangle.Empty)
            {
                Button paintBtn = sender as Button;
                if (paintBtn.Name == "searchNextBtn")
                {
                    Brush borderBrush = new SolidBrush(Color.FromArgb(225, 228, 230));
                    e.Graphics.DrawLine(new Pen(borderBrush), new Point(e.ClipRectangle.Left, e.ClipRectangle.Top), new Point(e.ClipRectangle.Left, e.ClipRectangle.Bottom));
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(179, 179, 179)), new Point(e.ClipRectangle.Left, e.ClipRectangle.Top), new Point(e.ClipRectangle.Right, e.ClipRectangle.Top));
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(179, 179, 179)), new Point(e.ClipRectangle.Right, e.ClipRectangle.Top), new Point(e.ClipRectangle.Right, e.ClipRectangle.Bottom));
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 233, 239), 3), new Point(e.ClipRectangle.Left, e.ClipRectangle.Bottom), new Point(e.ClipRectangle.Right, e.ClipRectangle.Bottom));
                }
                else if (paintBtn.Name == "searchPreviousBtn")
                {
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(179, 179, 179)), new Point(e.ClipRectangle.Left, e.ClipRectangle.Top), new Point(e.ClipRectangle.Right, e.ClipRectangle.Top));
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 233, 239), 3), new Point(e.ClipRectangle.Left, e.ClipRectangle.Bottom), new Point(e.ClipRectangle.Right, e.ClipRectangle.Bottom));
                }
                Brush transBrush = Brushes.Transparent;
                e.Graphics.FillRectangle(transBrush, e.ClipRectangle);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        #endregion
    }

    class SearchBoxButton : Button
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Focused)
            {
                if (this.Name == "searchPreviousBtn" || this.Name == "searchNextBtn")
                {
                    e.Graphics.DrawImage(this.Image, new PointF(-1F, -1F));
                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(179, 179, 179)), e.ClipRectangle);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(180, 203, 255)), e.ClipRectangle);
                    e.Graphics.DrawImage(this.Image, new PointF(3, 6.3F));
                }
                return;
            }
            base.OnPaint(e);
        }
    }
}
