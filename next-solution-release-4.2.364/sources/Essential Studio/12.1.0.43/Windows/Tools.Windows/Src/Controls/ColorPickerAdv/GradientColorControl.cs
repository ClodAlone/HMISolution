#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
    [ToolboxItem(false)]
    public class GradientColorControl : System.Windows.Forms.UserControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private Bitmap m_bitmap = null;
        [Browsable(false)]
        public Bitmap Bitmap
        {
            get { return m_bitmap; }
        }

        private Point m_selectedPoint = new Point(-1, -1);
        public Point SelectedPoint
        {
            get { return m_selectedPoint; }
            set { m_selectedPoint = value; }
        }

        private Color m_color = Color.Empty;
        public Color SelectedColor
        {
            get { return m_color; }
            set { m_color = value; }
        }

        private int m_horisontalStep = 7;
        public int HStep
        {
            get { return m_horisontalStep; }
            set { m_horisontalStep = value; }
        }

        private int m_verticalStep = 9;
        public int VStep
        {
            get { return m_verticalStep; }
            set { m_verticalStep = value; }
        }

        public event System.EventHandler Picked;

        protected virtual void OnPicked(EventArgs e)
        {
            if (this.Picked != null)
                this.Picked(this, e);
        }

        public GradientColorControl()
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();

            if (m_bitmap == null)
                m_bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool Disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        { 
            this.Name = "GradientColorControl";
            this.Size = new System.Drawing.Size(210, 150);
            this.SizeChanged += new EventHandler(GradientColorControl_SizeChanged);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_bitmap == null)
                m_bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);

            Graphics g = Graphics.FromImage(m_bitmap);

            Color[] colors = new Color[] { Color.Red, Color.FromArgb(255, 0, 255), Color.Blue, Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.Yellow, Color.Red };
            BrushInfo brushInfo = new BrushInfo(GradientStyle.Horizontal, colors);

            BrushPaint.FillRectangle(g, this.ClientRectangle, brushInfo);

            Color c1 = Color.FromArgb(0, Color.Gray);
            Color c2 = Color.FromArgb(255, Color.Gray);

            using (LinearGradientBrush linearBrush = new LinearGradientBrush(this.ClientRectangle, c1, c2, LinearGradientMode.Vertical))
            {
                g.FillRectangle(linearBrush, this.ClientRectangle);
            }

            g.Dispose();

            e.Graphics.DrawImage(m_bitmap, 0, 0);

            this.DrawSelection(e.Graphics);
        }

        private void DrawSelection(Graphics g)
        {
            Color fillColor = Color.Black;

            if (!this.Focused)
                fillColor = Color.White;

            Rectangle left = new Rectangle(this.SelectedPoint.X - 9, this.SelectedPoint.Y - 1, 5, 3);
            Rectangle up = new Rectangle(this.SelectedPoint.X - 1, this.SelectedPoint.Y - 9, 3, 5);
            Rectangle right = new Rectangle(this.SelectedPoint.X + 4, this.SelectedPoint.Y - 1, 5, 3);
            Rectangle down = new Rectangle(this.SelectedPoint.X - 1, this.SelectedPoint.Y + 4, 3, 5);

            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                g.FillRectangle(fillBrush, left);
                g.FillRectangle(fillBrush, up);
                g.FillRectangle(fillBrush, right);
                g.FillRectangle(fillBrush, down);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.ChangeSelection(new Point(e.X, e.Y));

            base.OnMouseDown(e);
        }

        internal void ChangeSelection(Point newPoint)
        {
            if (newPoint.X > this.ClientRectangle.Width - 1)
                newPoint.X = this.ClientRectangle.Width - 1;
            else if (newPoint.X < 0)
                newPoint.X = 0;

            if (newPoint.Y > this.ClientRectangle.Bottom - 1)
                newPoint.Y = this.ClientRectangle.Bottom - 1;
            else if (newPoint.Y < 0)
                newPoint.Y = 0;

            if (this.SelectedPoint != newPoint)
            {
                this.Invalidate(new Rectangle(this.SelectedPoint.X - 10, this.SelectedPoint.Y - 10, 20, 20));

                this.SelectedPoint = newPoint;
                this.SelectedColor = m_bitmap.GetPixel(this.SelectedPoint.X, this.SelectedPoint.Y);

                this.Invalidate(new Rectangle(this.SelectedPoint.X - 10, this.SelectedPoint.Y - 10, 20, 20));

                this.OnPicked(EventArgs.Empty);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.ChangeSelection(new Point(e.X, e.Y));

            base.OnMouseMove(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            this.Invalidate(new Rectangle(this.SelectedPoint.X - 10, this.SelectedPoint.Y - 10, 20, 20));
        }

        protected override bool IsInputKey(Keys keyData)
        {
            System.Windows.Forms.Keys keys = keyData;

            if (keys >= Keys.Left && keys <= Keys.Down)
                return true;

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (this.SelectedPoint.X == -1 && this.SelectedPoint.Y == -1)
                this.SelectedPoint = new Point(0, 0);

            Point p = this.SelectedPoint;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (p.X > m_horisontalStep)
                        p.X -= m_horisontalStep;
                    else
                        p.X = 0;

                    this.ChangeSelection(p);
                    return;
                case Keys.Right:
                    if (p.X < this.ClientRectangle.Width - m_horisontalStep)
                        p.X += m_horisontalStep;
                    else
                        p.X = this.ClientRectangle.Width;

                    this.ChangeSelection(p);
                    return;
                case Keys.Down:
                    if (p.Y < this.ClientRectangle.Height - m_verticalStep)
                        p.Y += m_verticalStep;
                    else
                        p.Y = this.ClientRectangle.Height;

                    this.ChangeSelection(p);
                    return;
                case Keys.Up:
                    if (p.Y > m_verticalStep)
                        p.Y -= m_verticalStep;
                    else
                        p.Y = 0;

                    this.ChangeSelection(p);
                    return;
            }
        }

        private void GradientColorControl_SizeChanged(object sender, EventArgs e)
        {
            m_bitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);

            this.Invalidate();
        }
    }
}