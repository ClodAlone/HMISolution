#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// ChartTitle is lets you define a custom title for the chart control.
    /// </summary>
    [ToolboxItem(false)]
    [Designer(typeof(ComponentDesigner)), DesignTimeVisible(false)]
    public sealed class ChartTitle : ChartDockControl
    {
        #region Members
        private bool m_autoSize = true;
        private bool m_showBorder = false;
        private LineInfo m_borderInfo = new LineInfo();
        private SizeF m_textSize = SizeF.Empty;
        private int m_margin = 4;
        private bool m_isWindowLess;
        #endregion

        #region Events
        #endregion

        #region Properties
        /// <summary>
        /// This member override <see cref="ChartDockControl.Position"/> property.
        /// </summary>
        public override ChartDock Position
        {
            get
            {
                return base.Position;
            }

            set
            {
                if (base.Position != value)
                {
                    base.Position = value;
                    SetOrientationByPosition();
                    ReCalcSize();
                }
            }
        }

        /// <summary>
        /// This member override <see cref="ChartDockControl.Orientation"/> property.
        /// </summary>
        public override ChartOrientation Orientation
        {
            get
            {
                return base.Orientation;
            }

            set
            {
                base.Orientation = value;

                if (Position == ChartDock.Floating)
                {
                    ReCalcSize();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [show border].
        /// </summary>
        /// <value><c>true</c> if [show border]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ShowBorder
        {
            get
            {
                return m_showBorder;
            }

            set
            {
                if (m_showBorder != value)
                {
                    m_showBorder = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the value to true or false. If set as true then the size is determined automatically based on the text of the title.
        /// </summary>
        [DefaultValue(true)]
        public override bool AutoSize
        {
            get
            {
                return m_autoSize;
            }

            set
            {
                if (m_autoSize != value)
                {
                    m_autoSize = value;
                }
            }
        }

        /// <summary>
        /// Gets the Configuration information for the border.
        /// </summary>
        [Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineInfo Border
        {
            get
            {
                return m_borderInfo;
            }
        }

        /// <summary>
        /// Gets or sets margin around the text. Default is 4.
        /// </summary>
        [DefaultValue(4)]
        public new int Margin
        {
            get
            {
                return m_margin;
            }

            set
            {
                if (m_margin != value)
                {
                    m_margin = value;
                    ReCalcSize();
                }
            }
        }

        /// <summary>
        /// Get or set background color of title. Default is Transparent.
        /// </summary>
        [DefaultValue(typeof(Color), "Transparent")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                base.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font of the text displayed by the control.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Drawing.Font"></see> to apply to the text displayed by the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultFont"></see> property.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [DefaultValue(typeof(Font), "Verdana, 14pt")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                base.Font = value;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed by the control.
        /// </summary>
        [NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public new string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// Gets or Sets location of title.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Point Location
        {
            get
            {
                return base.Location;
            }

            set
            {
                base.Location = value;
            }
        }

        /// <summary>
        /// Specifies the position and manner in which a ChartTitle is docked.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get
            {
                return DockStyle.None;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control and all its parent controls are displayed.
        /// </summary>
        /// <value></value>
        /// <returns>true if the control and all its parent controls are displayed; otherwise, false. The default is true.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Visible
        {
            get
            {
                return base.Visible;
            }

            set
            {
                base.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the tab order of the control within its container.
        /// </summary>
        /// <value></value>
        /// <returns>The index value of the control within the set of controls within its container. The controls in the container are included in the tab order.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }

            set
            {
                base.TabIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the height and width of the control.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Drawing.Size"></see> that represents the height and width of the control in pixels.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size Size
        {
            get
            {
                return base.Size;
            }

            set
            {
                base.Size = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is window less.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is window less; otherwise, <c>false</c>.
        /// </value>
        internal bool IsWindowLess
        {
            get { return m_isWindowLess; }
            set { m_isWindowLess = value; }
        }
        #endregion

        #region Class initialize/finalize methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTitle"/> class.
        /// </summary>
        public ChartTitle()
        {
            this.Visible = false;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor
                | ControlStyles.DoubleBuffer
                | ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, false);
            this.BackColor = Color.Transparent;
            this.Font = new Font("Verdana", 14f);

            m_borderInfo.SettingsChanged += new EventHandler(OnBorderInfoChanged);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and optionally releases the managed
        /// resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ChartControl chart = this.Parent as ChartControl;

                if (chart != null && chart.Titles != null)
                {
                    chart.Titles.Remove(this);
                }

                if (m_borderInfo != null)
                {
                    m_borderInfo.SettingsChanged -= new EventHandler(OnBorderInfoChanged);
                    m_borderInfo.Pen.Dispose();
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Measure size of control.
        /// </summary>
        /// <param name="size"></param>
        /// <returns>Returns Size of control.</returns>
        public override SizeF Measure(SizeF size)
        {
            Graphics g = null;
            ChartControl chart = this.Parent as ChartControl;

            if (m_isWindowLess && chart != null)
            {
                g = chart.GetGraphics();
            }
            else
            {
                g = this.CreateGraphics();
            }

            SizeF result = this.Measure(g, size);

            g.Dispose();

            return result;
        }

        /// <summary>
        /// Measures the specified Graphics.
        /// </summary>
        /// <param name="g">The Graphics.</param>
        /// <param name="size">The size.</param>
        /// <returns>Returns the control's size.</returns>
        internal SizeF Measure(Graphics g, SizeF size)
        {
            SizeF result = this.Size;

            if (m_autoSize)
            {
                if (Orientation == ChartOrientation.Vertical)
                {
                    m_textSize = Size.Ceiling(g.MeasureString(Text, Font, (int)size.Height));
                    result = Size.Ceiling(new SizeF(m_textSize.Height + 2 * m_margin, m_textSize.Width + 2 * m_margin));
                }
                else
                {
                    m_textSize = Size.Ceiling(g.MeasureString(Text, Font, (int)size.Width));
                    result = new SizeF(m_textSize.Width + 2 * m_margin, m_textSize.Height + 2 * m_margin);
                }
            }
            else
            {
                if (Orientation == ChartOrientation.Vertical)
                {
                    m_textSize = Size.Round(g.MeasureString(Text, Font, this.Size.Height));
                }
                else
                {
                    m_textSize = Size.Round(g.MeasureString(Text, Font, this.Size.Width));
                }
            }

            return this.Size = Size.Ceiling(result);
        }

        /// <summary>
        /// Prints the specified g.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect">The rect.</param>
        internal void Print(Graphics g, RectangleF rect)
        {
            #region Draw background
            using (SolidBrush sb = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(sb, rect);
            }

            if (this.BackgroundImage != null)
            {
                switch (this.BackgroundImageLayout)
                {
                    case ImageLayout.Center:
                        g.DrawImage(this.BackgroundImage, rect.X + 0.5f * (rect.Width - this.BackgroundImage.Width), rect.Y + 0.5f * (rect.Height - this.BackgroundImage.Height));
                        break;

                    case ImageLayout.None:
                        g.DrawImage(this.BackgroundImage, rect.Location);
                        break;

                    case ImageLayout.Stretch:
                        g.DrawImage(this.BackgroundImage, rect);
                        break;

                    case ImageLayout.Tile:
                        {
                            using (TextureBrush brush = new TextureBrush(this.BackgroundImage))
                            {
                                g.FillRectangle(brush, rect);
                            }
                        }

                        break;

                    case ImageLayout.Zoom:
                        {
                            float dx = rect.Width / this.BackgroundImage.Width;
                            float dy = rect.Height / this.BackgroundImage.Height;

                            if (dx < dy)
                            {
                                float height = dx * this.BackgroundImage.Height;
                                g.DrawImage(this.BackgroundImage, rect.X, rect.Y + 0.5f * (rect.Height - height), rect.Width, height);
                            }
                            else
                            {
                                float width = dy * this.BackgroundImage.Width;
                                g.DrawImage(this.BackgroundImage, rect.X + 0.5f * (rect.Width - width), rect.Y, width, rect.Height);
                            }
                        }

                        break;
                }
            }
            #endregion

            this.Draw(g, rect);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Should the serialize text.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        private bool ShouldSerializeSize()
        {
            return !this.AutoSize;
        }

        /// <summary>
        /// Should the serialize text.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        private bool ShouldSerializeText()
        {
            return !string.IsNullOrEmpty(this.Name)
                && this.Name != ChartControl.c_defaultTitleName;
        }

        /// <summary>
        /// This method called when settings of border was changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Styles.StyleChangedEventArgs"/> instance containing the event data.</param>
        private void OnBorderInfoChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        /// <summary>
        /// This method override <see cref="Control.OnPaint"/> method.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            ChartControl chart = this.Parent as ChartControl;

            if (chart != null)
            {
                e.Graphics.TextRenderingHint = chart.TextRenderingHint;
                e.Graphics.SmoothingMode = chart.SmoothingMode;
            }
            else
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            this.Draw(e.Graphics, new RectangleF(0, 0, this.Width, this.Height));

            base.OnPaint(e);
        }

        /// <summary>
        /// This method override <see cref="Control.OnFontChanged"/> method.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnFontChanged(EventArgs e)
        {
            ReCalcSize();
            base.OnFontChanged(e);
        }

        /// <summary>
        /// This method override <see cref="Control.OnTextChanged"/> method.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            this.Visible = !string.IsNullOrEmpty(this.Text);
            this.ReCalcSize();

            base.OnTextChanged(e);
        }

        /// <summary>
        /// This method override <see cref="Control.OnDoubleClick"/> method.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            if (Position == ChartDock.Floating)
            {
                Orientation = Orientation == ChartOrientation.Vertical ?
                    ChartOrientation.Horizontal : ChartOrientation.Vertical;
            }

            base.OnDoubleClick(e);
        }

        /// <summary>
        /// Calculate a new size of title.
        /// </summary>
        private void ReCalcSize()
        {
            if (Parent != null)
            {
                this.Measure(Parent.Bounds.Size);
            }
        }

        /// <summary>
        /// Draws to the specified <see cref="System.Drawing.Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/>.</param>
        /// <param name="rect">The bounds of title.</param>
        private void Draw(Graphics g, RectangleF rect)
        {
            string text = Text;
            RectangleF textRect = new RectangleF(rect.X + m_margin, rect.Y + m_margin, m_textSize.Width, m_textSize.Height);
            StringFormat strFormat = new StringFormat();

            switch (this.Alignment)
            {
                case ChartAlignment.Near:
                    strFormat.Alignment = StringAlignment.Near;
                    break;
                case ChartAlignment.Center:
                    strFormat.Alignment = StringAlignment.Center;
                    break;
                case ChartAlignment.Far:
                    strFormat.Alignment = StringAlignment.Far;
                    break;
            }

            if (text != null && text != string.Empty)
            {
                using (SolidBrush sb = new SolidBrush(ForeColor))
                {
                    if (Orientation == ChartOrientation.Vertical)
                    {
                        Matrix oldTransform = g.Transform;

                        if (Position == ChartDock.Right)
                        {
                            PointF pt1 = new PointF(textRect.X + textRect.Height, textRect.Y);
                            PointF pt2 = new PointF(textRect.X + textRect.Height, textRect.Y + textRect.Width);
                            PointF pt3 = new PointF(textRect.X, textRect.Y);

                            g.MultiplyTransform(new Matrix(textRect, new PointF[] { pt1, pt2, pt3 }));
                            g.DrawString(text, Font, sb, textRect, strFormat);
                        }
                        else
                        {
                            PointF pt1 = new PointF(textRect.X, textRect.Y + textRect.Width);
                            PointF pt2 = new PointF(textRect.X, textRect.Y);
                            PointF pt3 = new PointF(textRect.X + textRect.Height, textRect.Y + textRect.Width);

                            g.MultiplyTransform(new Matrix(textRect, new PointF[] { pt1, pt2, pt3 }));
                            g.DrawString(text, Font, sb, textRect, strFormat);
                        }

                        g.Transform = oldTransform;
                    }
                    else
                    {
                        g.DrawString(text, Font, sb, textRect, strFormat);
                    }
                }
            }

            if (m_showBorder)
            {
                g.DrawRectangle(m_borderInfo.Pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }
        #endregion
    }
}
