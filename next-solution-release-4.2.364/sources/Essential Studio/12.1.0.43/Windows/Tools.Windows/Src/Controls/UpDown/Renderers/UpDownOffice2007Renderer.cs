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
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Renders UpDownBase control with Office2007 style.
    /// </summary>
    public class UpDownOffice2007Renderer : UpDownRenderer
    {
        /// <summary>
        /// Initializes a new instance of the UpDownOffice2007Renderer class.
        /// </summary>
        /// <param name="upDownControl">Control to render.</param>
        /// <param name="scheme">Color scheme of Office2007 style.</param>
        public UpDownOffice2007Renderer(UpDownBase upDownControl, Office2007Theme scheme)
            : base(upDownControl)
        {
            this.ColorSheme = scheme;
            
            this.UpDownTextBox.MouseMove += new MouseEventHandler(UpDownTextBox_MouseMove);
            this.UpDownTextBox.MouseLeave += new EventHandler(UpDownTextBox_MouseLeave);
            this.UpDownTextBox.Validated += new EventHandler(UpDownTextBox_ValidatedEnter);
            this.UpDownTextBox.Enter += new EventHandler(UpDownTextBox_ValidatedEnter);
            this.UpDownTextBox.MouseEnter += new EventHandler(UpDownTextBox_ValidatedEnter);
        }

        #region EventHandlers
        private void UpDownTextBox_ValidatedEnter(object sender, EventArgs e)
        {
            this.UpDownControl.Invalidate(true);
        }

        private void UpDownTextBox_MouseLeave(object sender, EventArgs e)
        {
            m_mouseOver = false;
            this.UpDownControl.Invalidate(true);
        }

        private void UpDownTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            m_mouseOver = true;
        }
        #endregion

        #region Members
        private Office2007Theme m_colorSheme = Office2007Theme.Blue;
        private Office2007Colors m_colorTable = Office2007Colors.GetColorTable(Office2007Theme.Blue);
        private bool m_mouseOver = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color scheme to render with.
        /// </summary>
        public Office2007Theme ColorSheme
        {
            get
            {
                return m_colorSheme;
            }
            set
            {
                if (value != m_colorSheme)
                {
                    m_colorSheme = value;
                    this.OnColorSchemeChanged();
                }
            }
        }
       
        /// <summary>
        /// Gets border color for UpDownControl.
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return m_colorTable.UpDownBorderNormalColor;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Changes the color table according color scheme.
        /// </summary>
        protected virtual void OnColorSchemeChanged()
        {
            m_colorTable = Office2007Colors.GetColorTable(m_colorSheme);
        }
        private Point[] GetArrowPoints(Rectangle rectangle, bool upper)
        {
            Point p1, p2, p3;
            int sign = 1;
            if (!upper)
            {
                sign = -sign;
            }
            if (this.UpDownSpinOrientation == Orientation.Horizontal)
            {
                p1 = new Point(rectangle.Width / 2 + sign + rectangle.X, rectangle.Height / 2 + rectangle.Y);
                p2 = new Point(p1.X - sign * 3, p1.Y - 3);
                p3 = new Point(p1.X - sign * 3, p1.Y + 3);
            }
            else
            {
                p1 = new Point(rectangle.Width / 2 + rectangle.X, rectangle.Height / 2 - 2 * sign + rectangle.Y);
                p2 = new Point(p1.X - 3, p1.Y + sign * 4);
                p3 = new Point(p1.X + 3, p1.Y + sign * 3);
            }
            return new Point[] { p1, p2, p3 };
        }
        private Pen GetBorderPen(ButtonState buttonState)
        {
            Pen pen;
            switch (buttonState)
            {
                case ButtonState.Flat:
                    pen = new Pen(m_colorTable.UpDownBorderNormalColor);
                    break;
                case ButtonState.Pushed:
                    pen = new Pen(m_colorTable.UpDownBorderPressedColor);
                    break;
                case ButtonState.Inactive:
                    pen = new Pen(m_colorTable.UpDownBorderDisabledColor);
                    break;
                default:
                    pen = new Pen(m_colorTable.UpDownBorderHotColor);
                    break;
            }
            return pen;
        }

        private void DrawHotBackground(Graphics g, Rectangle buttonRectangle)
        {
            using (Brush brush = new LinearGradientBrush(buttonRectangle, m_colorTable.UpDownInnerBorderHotStartColor, m_colorTable.UpDownInnerBorderHotEndColor, LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(brush, buttonRectangle);
            }
            buttonRectangle.Inflate(-1, -1);
            int h = buttonRectangle.Height / 2;
            Rectangle topR = new Rectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, h);
            Rectangle botR = new Rectangle(buttonRectangle.X, buttonRectangle.Y + h, buttonRectangle.Width, buttonRectangle.Height - h);
            using (Brush brush = new LinearGradientBrush(topR, m_colorTable.UpDownBackgroundHotTopStartColor, m_colorTable.UpDownBackgroundHotTopEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, topR);
            }
            using (Brush brush = new LinearGradientBrush(botR, m_colorTable.UpDownBackgroundHotBottomStartColor, m_colorTable.UpDownBackgroundHotBottomEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, botR);
            }
        }
        private void DrawPressedBackground(Graphics g, Rectangle buttonRectangle)
        {
            using (Brush brush = new LinearGradientBrush(buttonRectangle, m_colorTable.UpDownInnerBorderPressedStartColor, m_colorTable.UpDownInnerBorderPressedEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, buttonRectangle);
            }
            buttonRectangle.Inflate(-1, -1);
            int h = buttonRectangle.Height / 2;
            Rectangle topR = new Rectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, h);
            Rectangle botR = new Rectangle(buttonRectangle.X, buttonRectangle.Y + h, buttonRectangle.Width, buttonRectangle.Height - h);
            using (Brush brush = new LinearGradientBrush(topR, m_colorTable.UpDownBackgroundPressedTopStartColor, m_colorTable.UpDownBackgroundPressedTopEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, topR);
            }
            using (Brush brush = new LinearGradientBrush(botR, m_colorTable.UpDownBackgroundPressedBottomStartColor, m_colorTable.UpDownBackgroundPressedBottomEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, botR);
            }
        }
        private void DrawDisabledBackground(Graphics g, Rectangle buttonRectangle)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(buttonRectangle, m_colorTable.UpDownBackgroundDisabledStartColor, m_colorTable.UpDownBackgroundDisabledEndColor, LinearGradientMode.Vertical))
            {
                // brush.Blend.Positions = new float[] { 0.0f, 0.45f, 0.5f, 1.0f };
                // brush.Blend.Factors = new float[] { 0.0f, 0.4f, 1.0f, 0.6f };
                g.FillRectangle(brush, buttonRectangle);
            }
        }

        private void DrawButtonsNoBorder(Graphics g, Brush brush, ButtonState upButtonState, ButtonState downButtonState)
        {
            g.FillRectangle(brush, this.UpDownButtons.ClientRectangle);
            this.DrawArrow(g, this.UpButtonRectangle, upButtonState, true);
            this.DrawArrow(g, this.DownButtonRectangle, downButtonState, false);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Draws arrow with Office2007 style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle where the arrow will be drawn.</param>
        /// <param name="buttonState">State of button where the arrow will be drawn.</param>
        /// <param name="upper">Indicates whether the arrow is upper.</param>
        public override void DrawArrow(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, bool upper)
        {
            Point[] points = GetArrowPoints(buttonRectangle, upper);

            GraphicsPath path = new GraphicsPath();
            path.AddLines(points);
            RectangleF rect = path.GetBounds();

            if (!this.UpDownControl.Enabled)
            {
                using (Brush arrowBrush = new SolidBrush(m_colorTable.UpDownBorderDisabledColor))
                {
                    g.FillPolygon(arrowBrush, points);
                }
            }
            else
            {
                using (Brush arrowBrush = new LinearGradientBrush(rect, m_colorTable.UpDownArrowStartColor, m_colorTable.UpDownArrowEndColor, LinearGradientMode.Vertical))
                {
                    g.FillPolygon(arrowBrush, points);
                }
            }
            path.Dispose();
        }
      
        /// <summary>
        /// Draws button's border with Office2007 style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        public override void DrawScrollButtonBorder(Graphics g, Rectangle buttonRectangle, ButtonState buttonState)
        {
            buttonRectangle.Width--;
            buttonRectangle.Height--;

            Pen borderPen = GetBorderPen(buttonState);
            g.DrawRectangle(borderPen, buttonRectangle);
            borderPen.Dispose();
        }
      
        /// <summary>
        /// Draws button's background with Office2007 style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        public override void DrawScrollButtonBackground(Graphics g, Rectangle buttonRectangle, ButtonState buttonState)
        {
            buttonRectangle.Inflate(-1, -1);
            switch (buttonState)
            {
                case ButtonState.Inactive:
                    DrawDisabledBackground(g, buttonRectangle);
                    break;
                case ButtonState.Normal:
                    DrawHotBackground(g, buttonRectangle);
                    break;
                case ButtonState.Pushed:
                    DrawPressedBackground(g, buttonRectangle);
                    break;
            }
        }
       
        /// <summary>
        /// Draws button's background, border and arrow with Office2007 style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="upButtonState">State of up button.</param>
        /// <param name="downButtonState">State of down button.</param>
        public override void Render(Graphics g, ButtonState upButtonState, ButtonState downButtonState)
        {
            bool buttonsNormal = upButtonState == ButtonState.Flat && downButtonState == ButtonState.Flat;
            if (!m_mouseOver && ((!this.UpDownTextBox.Focused && buttonsNormal) || !this.UpDownControl.Enabled))
            {
                // draw with UpDownBackgroundNormalColor
                if (this.UpDownControl.BackColor != m_colorTable.UpDownBackgroundNormalColor)
                {
                    this.UpDownControl.BackColor = m_colorTable.UpDownBackgroundNormalColor;
                }

                if (this.UpDownButtons.Enabled)
                {
                    using (Brush brush = new SolidBrush(m_colorTable.UpDownBackgroundNormalColor))
                    {
                        this.DrawButtonsNoBorder(g, brush, upButtonState, downButtonState);
                    }
                }
            }
            else
            { // mouse is over
                if (!this.UpDownControl.ReadOnly)
                    this.UpDownControl.BackColor = SystemColors.Window;
                if (this.UpDownButtons.Enabled)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                            this.UpDownButtons.ClientRectangle,
                            m_colorTable.UpDownBackgroundNormalStartColor,
                            m_colorTable.UpDownBackgroundNormalEndColor,
                            LinearGradientMode.Vertical))
                    {
                        Blend blend = new Blend(4);
                        blend.Factors = new float[] { 0.9F, 0.4F, 0.0F, 1.0F };
                        blend.Positions = new float[] { 0.0F, 0.49F, 0.5F, 1.0F };
                        brush.Blend = blend;
                        this.DrawButtonsNoBorder(g, brush, upButtonState, downButtonState);
                    }

                    this.DrawScrollButtonBorder(g, this.UpDownButtons.ClientRectangle, ButtonState.Flat);

                    if (!buttonsNormal)
                    {
                        base.Render(g, upButtonState, downButtonState);
                    }
                }
            }
        }
     
        /// <summary>
        /// Draws button's background, border and arrow.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        /// <param name="buttonID">Indicates whether the arrow is up-button or down-button.</param>
        public override void DrawScrollButton(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, ButtonID buttonID)
        {
            if (buttonState != ButtonState.Flat)
            {
                base.DrawScrollButton(g, buttonRectangle, buttonState, buttonID);
            }
        }
        #endregion
    }

    /// <summary>
    /// Renders UpDownBase control with Office2010 style.
    /// </summary>
    public class UpDownOffice2010Renderer : UpDownRenderer
    {
        /// <summary>
        /// Initializes a new instance of the UpDownOffice2010Renderer class.
        /// </summary>
        /// <param name="upDownControl">Control to render.</param>
        /// <param name="scheme">Color scheme of Office2010 style.</param>
        public UpDownOffice2010Renderer(UpDownBase upDownControl, Office2010Theme scheme)
            : base(upDownControl)
        {
            this.ColorSheme = scheme;

            this.UpDownTextBox.MouseMove += new MouseEventHandler(UpDownTextBox_MouseMove);
            this.UpDownTextBox.MouseLeave += new EventHandler(UpDownTextBox_MouseLeave);
            this.UpDownTextBox.Validated += new EventHandler(UpDownTextBox_ValidatedEnter);
            this.UpDownTextBox.Enter += new EventHandler(UpDownTextBox_ValidatedEnter);
            this.UpDownTextBox.MouseEnter += new EventHandler(UpDownTextBox_ValidatedEnter);
        }

        #region EventHandlers
        private void UpDownTextBox_ValidatedEnter(object sender, EventArgs e)
        {
            this.UpDownControl.Invalidate(true);
        }

        private void UpDownTextBox_MouseLeave(object sender, EventArgs e)
        {
            m_mouseOver = false;
            this.UpDownControl.Invalidate(true);
        }

        private void UpDownTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            m_mouseOver = true;
        }
        #endregion

        #region Members
        private Office2010Theme m_colorSheme = Office2010Theme.Blue;
        private Office2010Colors m_colorTable = Office2010Colors.GetColorTable(Office2010Theme.Blue);
        private bool m_mouseOver = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color scheme to render with.
        /// </summary>
        public Office2010Theme ColorSheme
        {
            get
            {
                return m_colorSheme;
            }
            set
            {
                if (value != m_colorSheme)
                {
                    m_colorSheme = value;
                    this.OnColorSchemeChanged();
                }
            }
        }

        /// <summary>
        /// Gets border color for UpDownControl.
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return m_colorTable.UpDownBorderNormalColor;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Changes the color table according color scheme.
        /// </summary>
        protected virtual void OnColorSchemeChanged()
        {
            m_colorTable = Office2010Colors.GetColorTable(m_colorSheme);
        }
        private Point[] GetArrowPoints(Rectangle rectangle, bool upper)
        {
            Point p1, p2, p3;
            int sign = 1;
            if (!upper)
            {
                sign = -sign;
            }
            if (this.UpDownSpinOrientation == Orientation.Horizontal)
            {
                p1 = new Point(rectangle.Width / 2 + sign + rectangle.X, rectangle.Height / 2 + rectangle.Y);
                p2 = new Point(p1.X - sign * 3, p1.Y - 3);
                p3 = new Point(p1.X - sign * 3, p1.Y + 3);
            }
            else
            {
                p1 = new Point(rectangle.Width / 2 + rectangle.X, rectangle.Height / 2 - 2 * sign + rectangle.Y);
                p2 = new Point(p1.X - 3, p1.Y + sign * 4);
                p3 = new Point(p1.X + 3, p1.Y + sign * 3);
            }
            return new Point[] { p1, p2, p3 };
        }
        private Pen GetBorderPen(ButtonState buttonState)
        {
            Pen pen;
            switch (buttonState)
            {
                case ButtonState.Flat:
                    pen = new Pen(m_colorTable.UpDownBorderNormalColor);
                    break;
                case ButtonState.Pushed:
                    pen = new Pen(m_colorTable.UpDownBorderPressedColor);
                    break;
                case ButtonState.Inactive:
                    pen = new Pen(m_colorTable.UpDownBorderDisabledColor);
                    break;
                default:
                    pen = new Pen(m_colorTable.UpDownBorderHotColor);
                    break;
            }
            return pen;
        }

        private void DrawHotBackground(Graphics g, Rectangle buttonRectangle)
        {
            using (Brush brush = new LinearGradientBrush(buttonRectangle, m_colorTable.UpDownInnerBorderHotStartColor, m_colorTable.UpDownInnerBorderHotEndColor, LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(brush, buttonRectangle);
            }
            buttonRectangle.Inflate(-1, -1);
            int h = buttonRectangle.Height / 2;
            Rectangle topR = new Rectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, h);
            Rectangle botR = new Rectangle(buttonRectangle.X, buttonRectangle.Y + h, buttonRectangle.Width, buttonRectangle.Height - h);
            using (Brush brush = new LinearGradientBrush(topR, m_colorTable.UpDownBackgroundHotTopStartColor, m_colorTable.UpDownBackgroundHotTopEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, topR);
            }
            using (Brush brush = new LinearGradientBrush(botR, m_colorTable.UpDownBackgroundHotBottomStartColor, m_colorTable.UpDownBackgroundHotBottomEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, botR);
            }
        }
        private void DrawPressedBackground(Graphics g, Rectangle buttonRectangle)
        {
            using (Brush brush = new LinearGradientBrush(buttonRectangle, m_colorTable.UpDownInnerBorderPressedStartColor, m_colorTable.UpDownInnerBorderPressedEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, buttonRectangle);
            }
            buttonRectangle.Inflate(-1, -1);
            int h = buttonRectangle.Height / 2;
            Rectangle topR = new Rectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, h);
            Rectangle botR = new Rectangle(buttonRectangle.X, buttonRectangle.Y + h, buttonRectangle.Width, buttonRectangle.Height - h);
            using (Brush brush = new LinearGradientBrush(topR, m_colorTable.UpDownBackgroundPressedTopStartColor, m_colorTable.UpDownBackgroundPressedTopEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, topR);
            }
            using (Brush brush = new LinearGradientBrush(botR, m_colorTable.UpDownBackgroundPressedBottomStartColor, m_colorTable.UpDownBackgroundPressedBottomEndColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, botR);
            }
        }
        private void DrawDisabledBackground(Graphics g, Rectangle buttonRectangle)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(buttonRectangle, m_colorTable.UpDownBackgroundDisabledStartColor, m_colorTable.UpDownBackgroundDisabledEndColor, LinearGradientMode.Vertical))
            {
                // brush.Blend.Positions = new float[] { 0.0f, 0.45f, 0.5f, 1.0f };
                // brush.Blend.Factors = new float[] { 0.0f, 0.4f, 1.0f, 0.6f };
                g.FillRectangle(brush, buttonRectangle);
            }
        }

        private void DrawButtonsNoBorder(Graphics g, Brush brush, ButtonState upButtonState, ButtonState downButtonState)
        {
            g.FillRectangle(brush, this.UpDownButtons.ClientRectangle);
            this.DrawArrow(g, this.UpButtonRectangle, upButtonState, true);
            this.DrawArrow(g, this.DownButtonRectangle, downButtonState, false);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Draws arrow with Office2010 style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle where the arrow will be drawn.</param>
        /// <param name="buttonState">State of button where the arrow will be drawn.</param>
        /// <param name="upper">Indicates whether the arrow is upper.</param>
        public override void DrawArrow(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, bool upper)
        {
            Point[] points = GetArrowPoints(buttonRectangle, upper);

            GraphicsPath path = new GraphicsPath();
            path.AddLines(points);
            RectangleF rect = path.GetBounds();

            if (!this.UpDownControl.Enabled)
            {
                using (Brush arrowBrush = new SolidBrush(m_colorTable.UpDownBorderDisabledColor))
                {
                    g.FillPolygon(arrowBrush, points);
                }
            }
            else
            {
                using (Brush arrowBrush = new LinearGradientBrush(rect, m_colorTable.UpDownArrowStartColor, m_colorTable.UpDownArrowEndColor, LinearGradientMode.Vertical))
                {
                    g.FillPolygon(arrowBrush, points);
                }
            }
            path.Dispose();
        }

        /// <summary>
        /// Draws button's border with Office2010 style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        public override void DrawScrollButtonBorder(Graphics g, Rectangle buttonRectangle, ButtonState buttonState)
        {
            buttonRectangle.Width--;
            buttonRectangle.Height--;

            Pen borderPen = GetBorderPen(buttonState);
            g.DrawRectangle(borderPen, buttonRectangle);
            borderPen.Dispose();
        }

        /// <summary>
        /// Draws button's background with Office2010 style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        public override void DrawScrollButtonBackground(Graphics g, Rectangle buttonRectangle, ButtonState buttonState)
        {
            buttonRectangle.Inflate(-1, -1);
            switch (buttonState)
            {
                case ButtonState.Inactive:
                    DrawDisabledBackground(g, buttonRectangle);
                    break;
                case ButtonState.Normal:
                    DrawHotBackground(g, buttonRectangle);
                    break;
                case ButtonState.Pushed:
                    DrawPressedBackground(g, buttonRectangle);
                    break;
            }
        }

        /// <summary>
        /// Draws button's background, border and arrow with Office2010 style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="upButtonState">State of up button.</param>
        /// <param name="downButtonState">State of down button.</param>
        public override void Render(Graphics g, ButtonState upButtonState, ButtonState downButtonState)
        {
            bool buttonsNormal = upButtonState == ButtonState.Flat && downButtonState == ButtonState.Flat;
            if (!m_mouseOver && ((!this.UpDownTextBox.Focused && buttonsNormal) || !this.UpDownControl.Enabled))
            {
                // draw with UpDownBackgroundNormalColor
                if (this.UpDownControl.BackColor != m_colorTable.UpDownBackgroundNormalColor)
                {
                    this.UpDownControl.BackColor = m_colorTable.UpDownBackgroundNormalColor;
                }

                if (this.UpDownButtons.Enabled)
                {
                    using (Brush brush = new SolidBrush(m_colorTable.UpDownBackgroundNormalColor))
                    {
                        this.DrawButtonsNoBorder(g, brush, upButtonState, downButtonState);
                    }
                }
            }
            else
            { // mouse is over
                if (!this.UpDownControl.ReadOnly)
                    this.UpDownControl.BackColor = m_colorTable.UpDownBackgroundNormalColor;
                if (this.UpDownButtons.Enabled)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                            this.UpDownButtons.ClientRectangle,
                            m_colorTable.UpDownBackgroundNormalStartColor,
                            m_colorTable.UpDownBackgroundNormalEndColor,
                            LinearGradientMode.Vertical))
                    {
                        Blend blend = new Blend(4);
                        blend.Factors = new float[] { 0.9F, 0.4F, 0.0F, 1.0F };
                        blend.Positions = new float[] { 0.0F, 0.49F, 0.5F, 1.0F };
                        brush.Blend = blend;
                        this.DrawButtonsNoBorder(g, brush, upButtonState, downButtonState);
                    }

                    this.DrawScrollButtonBorder(g, this.UpDownButtons.ClientRectangle, ButtonState.Flat);

                    if (!buttonsNormal)
                    {
                        base.Render(g, upButtonState, downButtonState);
                    }
                }
            }
        }

        /// <summary>
        /// Draws button's background, border and arrow.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        /// <param name="buttonID">Indicates whether the arrow is up-button or down-button.</param>
        public override void DrawScrollButton(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, ButtonID buttonID)
        {
            if (buttonState != ButtonState.Flat)
            {
                base.DrawScrollButton(g, buttonRectangle, buttonState, buttonID);
            }
        }
        #endregion
    }
    public class Metrorender : UpDownRenderer 
    {
        /// <summary>
        /// Initializes a new instance of the Metrorender class.
        /// </summary>
        /// <param name="upDownControl">Control to render.</param>
        /// <param name="scheme">Color scheme of Metro style.</param>
        public Metrorender(UpDownBase upDownControl, Color _metrocolor)
            : base(upDownControl)
       {
            this.UpDownTextBox.MouseMove += new MouseEventHandler(UpDownTextBox_MouseMove);
            this.UpDownTextBox.MouseLeave += new EventHandler(UpDownTextBox_MouseLeave);
            this.UpDownTextBox.Validated += new EventHandler(UpDownTextBox_ValidatedEnter);
            this.UpDownTextBox.Enter += new EventHandler(UpDownTextBox_ValidatedEnter);
            this.UpDownTextBox.MouseEnter += new EventHandler(UpDownTextBox_ValidatedEnter);
            metroColor = _metrocolor;
        }

        #region EventHandlers
        private void UpDownTextBox_ValidatedEnter(object sender, EventArgs e)
        {
            m_mouseOver = true;
            this.UpDownControl.Invalidate(true);
        }

        private void UpDownTextBox_MouseLeave(object sender, EventArgs e)
        {
            m_mouseOver = true;
            this.UpDownControl.Invalidate(true);
        }

        private void UpDownTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            m_mouseOver = true;
       }
        #endregion

        #region Members

        private bool m_mouseOver = true;
        #endregion
        private Office2007Colors m_colorTable = Office2007Colors.GetColorTable(Office2007Theme.Blue);
        #region Properties
        /// <summary>
        /// Gets or sets the color scheme to render with.
   /// </summary>

        public Color metroColor = Color.Black;    
        /// <summary>
       /// Gets border color for UpDownControl.+        /// </summary>
        public Color BorderColor
        {
            get
            {
                return metroColor;    
                
            }       }
        #endregion

        #region Methods
        /// <summary>
        /// Changes the color table according color scheme.
        /// </summary>
        
        private Point[] GetArrowPoints(Rectangle rectangle, bool upper)
        {
            Point p1, p2, p3;
            int sign = 1;
            if (!upper)
            {
                sign = -sign;            }
            if (this.UpDownSpinOrientation == Orientation.Horizontal)
            {
                p1 = new Point(rectangle.Width / 2 + sign + rectangle.X, rectangle.Height / 2 + rectangle.Y);
                p2 = new Point(p1.X - sign * 3, p1.Y - 3);
                p3 = new Point(p1.X - sign * 3, p1.Y + 3);
            }
            else
            {
                p1 = new Point(rectangle.Width / 2 + rectangle.X, rectangle.Height / 2 - 2 * sign + rectangle.Y);
                p2 = new Point(p1.X - 3, p1.Y + sign * 4);
                p3 = new Point(p1.X + 3, p1.Y + sign * 3);
            }
            return new Point[] { p1, p2, p3 };
        }
        private Pen GetBorderPen(ButtonState buttonState)
       {
            Pen pen;
            switch (buttonState)
            {
                case ButtonState.Flat:
                    pen = new Pen(metroColor);
                    break;
                case ButtonState.Pushed:
                    pen = new Pen(metroColor);
                    break;
                case ButtonState.Inactive:
                    pen = new Pen(metroColor);
                    break;
                default:
                    pen = new Pen(metroColor);
                    break;
            }
            return pen;
        }

   
        private void DrawPressedBackground(Graphics g, Rectangle buttonRectangle)
        {
            SolidBrush brushcolor  = new SolidBrush(metroColor);
            g.FillRectangle(brushcolor, buttonRectangle);
            buttonRectangle.Inflate(-1, -1);
            int h = buttonRectangle.Height / 2;
            Rectangle topR = new Rectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, h);
            Rectangle botR = new Rectangle(buttonRectangle.X, buttonRectangle.Y + h , buttonRectangle.Width, buttonRectangle.Height - h );
            g.FillRectangle(brushcolor, topR);
            g.FillRectangle(brushcolor, botR);
            brushcolor.Dispose ();
        }
        private void DrawDisabledBackground(Graphics g, Rectangle buttonRectangle)
       {
                SolidBrush brush = new SolidBrush(metroColor);
                g.FillRectangle(brush, buttonRectangle);
                brush.Dispose();
        }

        private void DrawButtonsNoBorder(Graphics g, Brush brush, ButtonState upButtonState, ButtonState downButtonState)
        {
            g.FillRectangle(brush, this.UpDownButtons.ClientRectangle);
            this.DrawArrow(g, this.UpButtonRectangle, upButtonState, true);
            this.DrawArrow(g, this.DownButtonRectangle, downButtonState, false);   
        }
        #endregion

       #region Overrides
        /// <summary>
        /// Draws arrow with Metro style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle where the arrow will be drawn.</param>
        /// <param name="buttonState">State of button where the arrow will be drawn.</param>
        /// <param name="upper">Indicates whether the arrow is upper.</param>
        public override void DrawArrow(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, bool upper)
        {
            Point[] points = GetArrowPoints(buttonRectangle, upper);
            GraphicsPath path = new GraphicsPath();
            path.AddLines(points);
            RectangleF rect = path.GetBounds();
            if (!this.UpDownControl.Enabled)
            {
               SolidBrush brush = new SolidBrush(metroColor);              
                g.FillPolygon(brush, points);
               brush.Dispose();
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.Black);
                g.FillPolygon(brush, points);
                brush.Dispose();

            }
            if (buttonState == ButtonState.Pushed)
            {
                SolidBrush brush = new SolidBrush(Color.White);
                g.FillPolygon(brush, points);
                brush.Dispose();
            }
            path.Dispose();

        }
      
        /// <summary>
        /// Draws button's border with Metro style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
       /// <param name="buttonState">State of button.</param>
        public override void DrawScrollButtonBorder(Graphics g, Rectangle buttonRectangle, ButtonState buttonState)
        {
            //buttonRectangle.Width--;
            //buttonRectangle.Height--;
            //Pen borderPen = GetBorderPen(buttonState);
            //int h = buttonRectangle.Height / 2;
            //Pen brushcolor = new Pen(metroColor);
            //Rectangle topR = new Rectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, h+1 );
            //Rectangle botR = new Rectangle(buttonRectangle.X, buttonRectangle.Y + h, buttonRectangle.Width, buttonRectangle.Height - h);
            //g.DrawRectangle(brushcolor, topR);
            //g.DrawRectangle(brushcolor, botR);
            //SolidBrush brushcolor1 = new SolidBrush(Color.FromArgb(40, Color.Gray)); 
            //g.FillRectangle(brushcolor1, topR);
            //g.FillRectangle(brushcolor1, botR);
        }
      
        /// <summary>
        /// Draws button's background with Metro style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        public override void DrawScrollButtonBackground(Graphics g, Rectangle buttonRectangle, ButtonState buttonState)
        {
            buttonRectangle.Inflate(-1, -1);
            switch (buttonState)
            {
                case ButtonState.Inactive:
                    DrawDisabledBackground(g, buttonRectangle);
                    break;

                case ButtonState.Pushed:
                    DrawPressedBackground(g, buttonRectangle);
                    break;
            }
        }
      
        /// <summary>
       /// Draws button's background, border and arrow with Metro style.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="upButtonState">State of up button.</param>
        /// <param name="downButtonState">State of down button.</param>
        public override void Render(Graphics g, ButtonState upButtonState, ButtonState downButtonState)
        {
            bool buttonsNormal = upButtonState == ButtonState.Flat && downButtonState == ButtonState.Flat;
            if (!m_mouseOver && ((!this.UpDownTextBox.Focused && buttonsNormal) || !this.UpDownControl.Enabled))
           {
                if (this.UpDownButtons.Enabled)
                {
                    SolidBrush brush1 = new SolidBrush(Color.White);
                    this.DrawButtonsNoBorder(g, brush1, upButtonState, downButtonState);
                    brush1.Dispose();
                }
            }
            else
            {
                if (!this.UpDownControl.ReadOnly)
                    this.UpDownControl.BackColor = base.UpDownControl.BackColor;

                if (this.UpDownButtons.Enabled)
                {
                    Blend blend = new Blend(4);
                    blend.Factors = new float[] { 0.9F, 0.4F, 0.0F, 1.0F };
                    blend.Positions = new float[] { 0.0F, 0.49F, 0.5F, 1.0F };
                    SolidBrush s = new SolidBrush(Color.White);
                    this.DrawButtonsNoBorder(g, s, upButtonState, downButtonState);
                    this.DrawScrollButtonBorder(g, this.UpDownButtons.ClientRectangle, ButtonState.Flat);
                    if (!buttonsNormal)
                    {
                        base.Render(g, upButtonState, downButtonState);
                    }
                    s.Dispose();
                }
            }
       }
     
        /// <summary>
        /// Draws button's background, border and arrow.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        /// <param name="buttonID">Indicates whether the arrow is up-button or down-button.</param>
       public override void DrawScrollButton(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, ButtonID buttonID)
        {
            if (buttonState != ButtonState.Flat)
            {
                base.DrawScrollButton(g, buttonRectangle, buttonState, buttonID);
            }
           
            //SolidBrush brushcolor = new SolidBrush(Color.Blue);
            //int h = buttonRectangle.Height / 2;
            //Rectangle topR = new Rectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, h);
            //Rectangle botR = new Rectangle(buttonRectangle.X, buttonRectangle.Y + h, buttonRectangle.Width, buttonRectangle.Height - h);
            //g.FillRectangle(brushcolor, topR);
            //g.FillRectangle(brushcolor, botR);
        }
        #endregion
    }
   
 }
