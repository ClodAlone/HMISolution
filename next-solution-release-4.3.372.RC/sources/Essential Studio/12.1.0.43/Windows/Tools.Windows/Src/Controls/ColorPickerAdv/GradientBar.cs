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

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// GradientBar class.
    /// </summary>
    [ToolboxItem(false)]
    public class GradientBar : System.Windows.Forms.UserControl
    {
        public const int DEF_OFFCET = 4;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private Bitmap m_bitmap = null;

        private Color m_middleColor = Color.Gray;
        public Color MiddleColor
        {
            get
            { 
                return m_middleColor; 
            }
            set
            {
                m_middleColor = value;

                this.Invalidate();
            }
        }

        private int m_position = 142;
        public int Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        private int m_bitmapHeight = 142;
        public int BitmapHeight
        {
            get { return m_bitmapHeight; }
        }

        private int m_maxValue = 255;
        public int MaxValue
        {
            get { return m_maxValue; }
            set { m_maxValue = value; }
        }

        public event System.EventHandler PositionChanged;

        protected virtual void OnPositionChanged(EventArgs e)
        {
            if (this.PositionChanged != null)
                this.PositionChanged(this, e);
        }

        public GradientBar()
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool value disposing</param>
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // GradientBar
            // 
            this.Name = "GradientBar";
            this.Size = new System.Drawing.Size(20, 150);
            this.SizeChanged += new EventHandler(GradientBar_SizeChanged);

        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_bitmap == null)
                m_bitmap = new Bitmap(ClientRectangle.Width - 10, m_bitmapHeight);

            Color[] colors = new Color[] { Color.Black, this.MiddleColor, Color.White };
            BrushInfo brushInfo = new BrushInfo(GradientStyle.Vertical, colors);

            Graphics g = Graphics.FromImage(m_bitmap);

            Rectangle bmRect = new Rectangle(0, 0, this.Size.Width - 10, m_bitmapHeight);
            BrushPaint.FillRectangle(g, bmRect, brushInfo);

            g.Dispose();

            Rectangle rect = new Rectangle(new Point(0, DEF_OFFCET), new Size(this.Size.Width - 10, m_bitmapHeight));
            e.Graphics.DrawImage(m_bitmap, rect);

            this.DrawPointer(e.Graphics);
        }

        private void DrawPointer(Graphics g)
        {
            int xOffcet = this.ClientRectangle.Right - (2 * DEF_OFFCET - 1);
            int yOffcet = this.Position + (DEF_OFFCET - 1);

            using (GraphicsPath path = new GraphicsPath())
            {
                Point[] points = new Point[]
            {
                    new Point( xOffcet, yOffcet ),
                    new Point( xOffcet + 1, yOffcet ),
                    new Point( xOffcet + 1, yOffcet - 1 ),
                    new Point( xOffcet + 3, yOffcet - 1 ),
                    new Point( xOffcet + 3, yOffcet - 2 ),
                    new Point( xOffcet + 4, yOffcet - 2 ),
                    new Point( xOffcet + 4, yOffcet - 3 ),
                    new Point( xOffcet + 6, yOffcet - 3 ),
                    new Point( xOffcet + 6, yOffcet - 4 ),
                    new Point( xOffcet + 7, yOffcet - 4 ),
                    new Point( xOffcet + 7, yOffcet + 5 ),
                    new Point( xOffcet + 6, yOffcet + 5 ),
                    new Point( xOffcet + 6, yOffcet + 4 ),
                    new Point( xOffcet + 4, yOffcet + 4 ),
                    new Point( xOffcet + 4, yOffcet + 3 ),
                    new Point( xOffcet + 3, yOffcet + 3 ),
                    new Point( xOffcet + 3, yOffcet + 2 ),
                    new Point( xOffcet + 1, yOffcet + 2 ),
                    new Point( xOffcet + 1, yOffcet + 1 ),
                    new Point( xOffcet + 1, yOffcet + 1 )
                };

                path.AddLines(points);
                path.CloseFigure();
                using(Brush brush =new SolidBrush(Color.Black))
                g.FillPath(brush, path);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.ChangePosition(e.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
                this.ChangePosition(e.Y);
        }

        internal void ChangePosition(int position)
        {
            if (position >= this.ClientRectangle.Height - DEF_OFFCET)
                position = this.ClientRectangle.Height - DEF_OFFCET * 2;
            else if (position < DEF_OFFCET)
                position = 0;
            else
                position -= DEF_OFFCET - 1;

            if (this.Position != position)
            {
                this.Position = position;

                this.Invalidate();

                this.OnPositionChanged(EventArgs.Empty);
            }
        }

        private void GradientBar_SizeChanged(object sender, EventArgs e)
        {
            if (this.Size.Width < 20)
                this.Size = new Size(20, this.Size.Height);

            m_bitmap = new Bitmap(this.ClientRectangle.Width - 10, m_bitmapHeight);

            m_bitmapHeight = this.Height - DEF_OFFCET * 2;
            m_position = this.Height - DEF_OFFCET * 2;

            this.Invalidate();
        }
    }
}