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

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    ///  HexagonColorControl class.
    /// </summary>
    [ToolboxItem(false)]
    public class HexagonColorControl : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Label label1;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private ColorCellCollection m_items = null;

        private ColorCell m_selectedCell = null;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorCell SelectedCell
        {
            get 
            {
                return m_selectedCell; 
            }
            set
            {
                if (m_selectedCell != value)
                    m_selectedCell = value;
            }
        }

        [Browsable(false)]
        public ColorCellCollection Items
        {
            get { return m_items; }
            set { m_items = value; }
        }

        private Color m_selectedColor = Color.Empty;
        public Color SelectedColor
        {
            get 
            { 
                return m_selectedColor; 
            }
            set
            {
                if (m_selectedColor != value)
                    m_selectedColor = value;
            }
        }

        public event System.EventHandler Picked;

        protected virtual void OnPicked(EventArgs e)
        {
            if (this.Picked != null)
                this.Picked(this, e);
        }

        public HexagonColorControl()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            InitializeComponent();

            Init();
        }

        private void Init()
        {
            this.BackColor = Color.FromArgb(255, 251, 255);

            if (m_items == null)
                m_items = new ColorCellCollection(this);

            m_items.Add(new ColorCell(Color.FromArgb(0, 0, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(8, 0, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(24, 0, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(49, 0, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(74, 0, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(99, 0, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 0, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 16, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 0, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(24, 0, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(57, 0, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(107, 0, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(156, 0, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 0, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 0, 90)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 36, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 32, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 12, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(49, 4, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(123, 4, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(198, 4, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 4, 247)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 0, 148)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 0, 74)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 56, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 69, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 60, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(66, 69, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(123, 65, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(198, 65, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 65, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 4, 181)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 0, 99)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 0, 49)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 77, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 113, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 130, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(66, 130, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(140, 142, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(198, 138, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 138, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 65, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 4, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 0, 57)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 0, 24)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 97, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 154, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 195, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(66, 199, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(140, 199, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(214, 211, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 211, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 138, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 65, 123)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 4, 49)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 0, 24)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 0, 8)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 117, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 190, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 255, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(66, 255, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(140, 255, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(214, 255, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 255, 255)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 211, 214)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 138, 140)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 65, 66)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 4, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 0, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 0, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 117, 99)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 190, 156)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 255, 198)));
            m_items.Add(new ColorCell(Color.FromArgb(66, 255, 198)));
            m_items.Add(new ColorCell(Color.FromArgb(140, 255, 198)));
            m_items.Add(new ColorCell(Color.FromArgb(214, 255, 214)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 255, 214)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 190, 140)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 121, 66)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 52, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 24, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 12, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 117, 74)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 190, 115)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 255, 132)));
            m_items.Add(new ColorCell(Color.FromArgb(66, 255, 132)));
            m_items.Add(new ColorCell(Color.FromArgb(140, 255, 140)));
            m_items.Add(new ColorCell(Color.FromArgb(198, 255, 140)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 255, 140)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 186, 66)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 113, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 60, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 28, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 117, 57)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 190, 66)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 255, 57)));
            m_items.Add(new ColorCell(Color.FromArgb(66, 255, 66)));
            m_items.Add(new ColorCell(Color.FromArgb(123, 255, 66)));
            m_items.Add(new ColorCell(Color.FromArgb(198, 255, 66)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 251, 66)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 182, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 101, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 48, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 117, 33)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 190, 33)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 255, 8)));
            m_items.Add(new ColorCell(Color.FromArgb(49, 255, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(123, 255, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(198, 255, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(255, 247, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 146, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 73, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 117, 16)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 190, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(24, 190, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(57, 190, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(107, 190, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(156, 194, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 186, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 93, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(0, 117, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(8, 117, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(24, 117, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(49, 117, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(74, 117, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(99, 117, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(115, 113, 0)));
            m_items.Add(new ColorCell(Color.FromArgb(222, 223, 222)));
            m_items.Add(new ColorCell(Color.FromArgb(206, 207, 206)));
            m_items.Add(new ColorCell(Color.FromArgb(189, 190, 189)));
            m_items.Add(new ColorCell(Color.FromArgb(173, 174, 173)));
            m_items.Add(new ColorCell(Color.FromArgb(156, 154, 156)));
            m_items.Add(new ColorCell(Color.FromArgb(140, 142, 140)));
            m_items.Add(new ColorCell(Color.FromArgb(123, 125, 123)));
            m_items.Add(new ColorCell(Color.FromArgb(107, 109, 107)));
            m_items.Add(new ColorCell(Color.FromArgb(90, 93, 90)));
            m_items.Add(new ColorCell(Color.FromArgb(74, 77, 74)));
            m_items.Add(new ColorCell(Color.FromArgb(57, 60, 57)));
            m_items.Add(new ColorCell(Color.FromArgb(49, 48, 49)));
            m_items.Add(new ColorCell(Color.FromArgb(33, 36, 33)));
            m_items.Add(new ColorCell(Color.FromArgb(16, 20, 16)));
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            for (int i = 0; i < this.Items.Count; i++)
            {
                ColorCell cell = this.Items[i];

                using (SolidBrush brush = new SolidBrush(cell.Color))
                {
                    g.FillRegion(brush, cell.Bounds);
                }
            }

            if (m_selectedCell != null && m_selectedCell.State == ColorCellState.Selected)
            {
                using (Pen pen = new Pen(Color.Black, 4))
                    g.DrawPath(pen, m_selectedCell.Path);
                using (Pen pen = new Pen(Color.White, 2))
                    g.DrawPath(pen, m_selectedCell.Path);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int i = this.HitTest(e.X, e.Y);

            if (e.Button == MouseButtons.Left && i != -1)
                this.ChangeSelection(i);
        }

        private int HitTest(int x, int y)
        {
            Point p = new Point(x, y);
            int index = -1;

            foreach (ColorCell cell in this.Items)
            {
                if (cell.Bounds.IsVisible(p))
                {
                    index = cell.Index;
                    break;
                }
            }

            return index;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int i = this.HitTest(e.X, e.Y);

            if (e.Button == MouseButtons.Left && i != -1)
                this.ChangeSelection(i);
        }

        internal void ChangeSelection(int index)
        {
            Rectangle rect = Rectangle.Empty;

            ColorCell cell = this.Items[index];

            if (m_selectedCell != cell)
            {
                if (m_selectedCell != null)
                {
                    m_selectedCell.State = ColorCellState.Normal;

                    this.Invalidate();
                }

                cell.State = ColorCellState.Selected;
                m_selectedCell = cell;

                this.SelectedColor = m_selectedCell.Color;

                this.Invalidate();

                this.OnPicked(EventArgs.Empty);
            }
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

            int index = -1;

            if (m_selectedCell == null)
            {
                m_selectedCell = this.Items[0];
                m_selectedCell.State = ColorCellState.Selected;

                this.Invalidate();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (m_selectedCell.VerticalPosition >= 6)
                        index = m_selectedCell.Index + Math.Abs(6 - m_selectedCell.VerticalPosition + 13);
                    else if (m_selectedCell.VerticalPosition < 6)
                        index = m_selectedCell.Index + Math.Abs(-(6 - m_selectedCell.VerticalPosition) + 14);

                    if (index > this.Items.Count - 1)
                        index = m_selectedCell.Index;

                    if (index != m_selectedCell.Index)
                        this.ChangeSelection(index);

                    return;
                case Keys.Up:
                    if (m_selectedCell.VerticalPosition > 6)
                        index = m_selectedCell.Index - Math.Abs(6 - m_selectedCell.VerticalPosition + 14);
                    else
                        index = m_selectedCell.Index - Math.Abs(-(6 - m_selectedCell.VerticalPosition) + 13);

                    if (index < 0)
                        index = m_selectedCell.Index;

                    if (index != m_selectedCell.Index)
                        this.ChangeSelection(index);

                    return;
                case Keys.Right:
                    if (m_selectedCell.Index < this.Items.Count - 1)
                        index = m_selectedCell.Index + 1;
                    else
                        index = 0;

                    this.ChangeSelection(index);
                    return;
                case Keys.Left:
                    if (m_selectedCell.Index > 0)
                        index = m_selectedCell.Index - 1;
                    else
                        index = this.Items.Count - 1;

                    this.ChangeSelection(index);
                    return;
            }
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = SR.GetString(SR.ColorEditorColorsLabel);
            // 
            // HexagonColorControl
            // 
            this.Controls.Add(this.label1);
            this.Name = "HexagonColorControl";
            this.Size = new System.Drawing.Size(252, 294);
            this.ResumeLayout(false);

        }
        #endregion
    }

    public enum ColorCellState
    {
        /// <summary>
        /// Represents Normal Color cell state.
        /// </summary>
        Normal,

        /// <summary>
        /// Represents selected Color cell state.
        /// </summary>
        Selected
    }

    [ToolboxItem(false)]
    public class ColorCell :
        Component
    {
        public ColorCell(Color color)
        {
            m_color = color;
        }

        private Region m_bounds = null;
        public Region Bounds
        {
            get { return m_bounds; }
            set { m_bounds = value; }
        }

        private Color m_color = Color.Empty;
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        private int m_index = -1;
        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        private ColorCellState m_state = ColorCellState.Normal;
        [DefaultValue(ColorCellState.Normal)]
        public ColorCellState State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        private int m_cellWidth = 18;
        private int m_cellHeight = 14;

        private int xPos = 0;
        public int HorizontalPosition
        {
            get { return xPos; }
        }

        private int yPos = 0;
        public int VerticalPosition
        {
            get { return yPos; }
        }

        public void CalculateBounds()
        {
            if (m_index < 7)
            {
                xPos = m_index;
            }
            else if (m_index < 15)
            {
                xPos = m_index - 7;
                yPos = 1;
            }
            else if (m_index < 24)
            {
                xPos = m_index - 15;
                yPos = 2;
            }
            else if (m_index < 34)
            {
                xPos = m_index - 24;
                yPos = 3;
            }
            else if (m_index < 45)
            {
                xPos = m_index - 34;
                yPos = 4;
            }
            else if (m_index < 57)
            {
                xPos = m_index - 45;
                yPos = 5;
            }
            else if (m_index < 70)
            {
                xPos = m_index - 57;
                yPos = 6;
            }
            else if (m_index < 82)
            {
                xPos = m_index - 70;
                yPos = 7;
            }
            else if (m_index < 93)
            {
                xPos = m_index - 82;
                yPos = 8;
            }
            else if (m_index < 103)
            {
                xPos = m_index - 93;
                yPos = 9;
            }
            else if (m_index < 112)
            {
                xPos = m_index - 103;
                yPos = 10;
            }
            else if (m_index < 120)
            {
                xPos = m_index - 112;
                yPos = 11;
            }
            else if (m_index < 127)
            {
                xPos = m_index - 120;
                yPos = 12;
            }
            else if (m_index < 134)
            {
                xPos = m_index - 127;
                yPos = 13;
            }
            else if (m_index < 141)
            {
                xPos = m_index - 134;
                yPos = 14;
            }

            Point location = this.CalculateLocation(xPos, yPos);
            this.CalculateCellBounds(location);
        }

        private Point CalculateLocation(int xPos, int yPos)
        {
            Point defOffcet = new Point(60, 42);

            int xOffcet = 0;
            int yOffcet = 0;
            int defXOffcet = m_cellWidth / 2;

            if (yPos <= 12)
            {
                defXOffcet += Math.Abs(6 - yPos) * m_cellWidth / 2 + m_cellWidth / 2;
            }
            else if (yPos == 13)
            {
                defXOffcet += (int)(m_cellWidth * 4.5);
            }
            else if (yPos == 14)
            {
                defXOffcet += m_cellWidth * 4;
            }

            xOffcet = defXOffcet + m_cellWidth * xPos;
            yOffcet = defOffcet.Y + m_cellHeight * yPos;

            if (yPos > 12)
                yOffcet += m_cellHeight;

            return new Point(xOffcet, yOffcet);
        }

        private void CalculateCellBounds(Point loc)
        {
            GraphicsPath path = new GraphicsPath();
            Point[] points = new Point[]
            {
                    new Point( loc.X + 3, loc.Y ),
                    new Point( loc.X + 3, loc.Y + 1 ),
                    new Point( loc.X + 6, loc.Y + 1 ),
                    new Point( loc.X + 6, loc.Y + 2 ),
                    new Point( loc.X + 9, loc.Y + 2 ),
                    new Point( loc.X + 9, loc.Y + 14 ),
                    new Point( loc.X + 6, loc.Y + 14 ),
                    new Point( loc.X + 6, loc.Y + 15 ),
                    new Point( loc.X + 3, loc.Y + 15 ),
                    new Point( loc.X + 3, loc.Y + 16 ),
                    new Point( loc.X - 3, loc.Y + 16 ),
                    new Point( loc.X - 3, loc.Y + 15 ),
                    new Point( loc.X - 6, loc.Y + 15 ),
                    new Point( loc.X - 6, loc.Y + 14 ),
                    new Point( loc.X - 9, loc.Y + 14 ),
                    new Point( loc.X - 9, loc.Y + 2 ),
                    new Point( loc.X - 6, loc.Y + 2 ),
                    new Point( loc.X - 6, loc.Y + 1),
                    new Point( loc.X - 3, loc.Y + 1),
                    new Point( loc.X - 3, loc.Y ),
                };

            path.AddLines(points);
            path.CloseFigure();

            m_path = path;

            this.Bounds = new Region(path);
        }

        private GraphicsPath m_path = new GraphicsPath();
        public GraphicsPath Path
        {
            get { return m_path; }
            set { m_path = value; }
        }
    }

    [Serializable]
    public class ColorCellCollection : CollectionBase
    {
        #region Members

        private HexagonColorControl m_colorControl = null;

        #endregion

        #region Events

        public event EventHandler CollectionChanged;

        #endregion
        protected void OnCollectionChanged()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, EventArgs.Empty);
            }
        }

        #region Constructors

        public ColorCellCollection(HexagonColorControl control)
        {
            if (control == null)
            {
                throw new NullReferenceException("ColorControl can't be NULL");
            }

            m_colorControl = control;
        }

        #endregion

        #region Indexer

        public ColorCell this[int index]
        {
            get
            {
                return (ColorCell)this.List[index];
            }
            set
            {
                if (index < 0 || index >= this.List.Count)
                {
                    throw new IndexOutOfRangeException("index");
                }
                if (value == null)
                {
                    throw new NullReferenceException("value can't be NULL");
                }

                if (this.List[index] != value)
                {
                    this.List[index] = value;
                }
            }
        }
        #endregion

        #region Methods

        public void Add(ColorCell cell)
        {
            if (cell == null)
            {
                throw new NullReferenceException("Cell can't be NULL");
            }

            this.List.Add(cell);
        }

        public bool Contains(ColorCell cell)
        {
            if (cell == null)
            {
                throw new NullReferenceException("Cell can't be NULL");
            }

            return this.List.Contains(cell);
        }

        public void Remove(ColorCell cell)
        {
            if (cell == null)
            {
                throw new NullReferenceException("cell can't be NULL");
            }

            if (!this.Contains(cell))
            {
                throw new NullReferenceException("Cell doesn't exist in collection");
            }

            this.List.Remove(cell);
        }

        public int IndexOf(ColorCell cell)
        {
            if (cell == null)
            {
                throw new NullReferenceException("Cell can't be NULL");
            }

            return this.List.IndexOf(cell);
        }

        public void Insert(int index, ColorCell cell)
        {
            if (cell == null)
            {
                throw new NullReferenceException("cell can't be NULL");
            }

            if (index < 0 || index >= this.List.Count)
            {
                throw new IndexOutOfRangeException("index");
            }

            this.List.Insert(index, cell);
        }

        #endregion

        #region Overrides

        protected override void OnInsert(int index, object value)
        {
            ColorCell cell = (ColorCell)value;

            if (!this.Contains(cell))
            {
                base.OnInsert(index, value);

                if (cell.Index == -1)
                {
                    cell.Index = this.Count;
                }

                this.OnCollectionChanged();
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);

            ColorCell cell = value as ColorCell;
            cell.CalculateBounds();
        }

        #endregion
    }
}