#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Drawing;
using Syncfusion.Runtime.InteropServices;
using System.Windows.Forms;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms
{
    public abstract class TabsRendererBase :
        Disposable
    {
        #region Class constans

        private const int c_drawTextFlags = DrawTextFormats.DT_SINGLELINE;
        private const string c_measureString = "X";
        private const int c_imageTextPadding = 3;

        #endregion

        #region Class members

        /// <summary>
        /// 
        /// </summary>
        private InternalTab m_parent;
        /// <summary>
        /// 
        /// </summary>
        private Rectangle m_bounds;
        /// <summary>
        /// Special graphics for text measuring.
        /// </summary>
        private Graphics m_measure;
        /// <summary>
        /// Default tab font.
        /// </summary>
        private Font m_defaultFont = new Font("Microsoft Sans Serif", 8.25f);
        /// <summary>
        /// Indent from borders to the text.
        /// </summary>
        protected int m_imageTextIndent = 8;
        #endregion

        #region Class initialize
        /// <summary>
        /// Creates an instance of the <see cref="Syncfusion.Windows.Forms.Workbook.TabsRendererBase"/>.
        /// <summary>
        public TabsRendererBase(InternalTab parent)
        {
            m_parent = parent;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Returns the parent.
        /// </summary>
        public InternalTab TabBar
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Bounds of the tab.
        /// </summary>
        public virtual Rectangle Bounds
        {
            get
            {
                return m_bounds;
            }
            set
            {
                m_bounds = value;
            }
        }

        /// <summary>
        /// Gets the region which contains tab bounds.
        /// </summary>
        public virtual Region GetTabRegion
        {
            get
            {
                return new Region(this.Bounds);
            }
        }

        /// <summary>
        /// Gets the tab.
        /// </summary>
        protected InternalTab Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Text of the tab.
        /// </summary>
        public string TabText
        {
            get
            {
                TabBarPage page = m_parent.Cookie as TabBarPage;
                if (page != null)
                {
                    return page.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Font of the active tab.
        /// </summary>
        protected virtual Font InactiveTabFont
        {
            get
            {
                TabBarPage page = m_parent.Cookie as TabBarPage;
                if (page != null)
                {
                    return page.Font;
                }
                else
                {
                    return m_defaultFont;
                }
            }
        }

        /// <summary>
        /// Font of the active tab.
        /// </summary>
        protected virtual Font ActiveTabFont
        {
            get
            {
                TabBarPage page = m_parent.Cookie as TabBarPage;
                if (page != null)
                {
                    return page.Font;
                }
                else
                {
                    return m_defaultFont;
                }
            }
        }

        /// <summary>
        /// Color used to draw the text of the tab.
        /// </summary>
        protected virtual Color ForeColor
        {
            get
            {
                TabBarPage page = m_parent.Cookie as TabBarPage;
                if (page != null)
                {
                    return page.ForeColor;
                }
                else
                {
                    return Color.Black;
                }
            }
        }

        /// <summary>
        /// Get special measure graphics that allowing measuring without control creation.
        /// </summary>
        private Graphics MeasureGraphics
        {
            get
            {
                if (m_measure == null)
                {
                    m_measure = Graphics.FromImage(new Bitmap(1, 1));
                }

                return m_measure;
            }
            set
            {
                if (m_measure != null)
                {
                    m_measure.Dispose();
                }

                m_measure = value;
            }
        }
        #endregion

        #region Class overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        public virtual void DrawTab(Graphics g)
        {
            this.DrawBorders(g);

            this.DrawBackground(g);

            this.DrawTextAndImage(g, this.Bounds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        public virtual void DrawBorders(Graphics g)
        {
            using (Pen pen = new Pen(SystemColors.ControlDarkDark))
            {
                g.DrawLine(pen, this.Bounds.Left, this.Bounds.Top, this.Bounds.Left, this.Bounds.Bottom);
                g.DrawLine(pen, this.Bounds.Right, this.Bounds.Top, this.Bounds.Right, this.Bounds.Bottom);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        public virtual void DrawBackground(Graphics g)
        {
            if (m_parent.Pushed)
            {
                using (Brush brush = new SolidBrush(SystemColors.Window))
                {
                    g.FillRectangle(brush, this.Bounds);
                }
            }
            else
            {
                using (Brush brush = new SolidBrush(SystemColors.Control))
                {
                    g.FillRectangle(brush, this.Bounds);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectTextAndImage"></param>
        protected virtual void DrawTextAndImage(Graphics g, Rectangle rectTextAndImage)
        {
            TabBarPage page = m_parent.Cookie as TabBarPage;
            if (page != null)
            {
                Rectangle rectImage = Rectangle.Empty;
                Rectangle rectText = Rectangle.Empty;

                IInternalTabParent container = m_parent.Owner as IInternalTabParent;
                if (container != null && container.ImageList != null && m_parent.ImageIndex >= 0)
                {
                    int imageWidth = container.ImageList.ImageSize.Width;
                    int imageHeight = container.ImageList.ImageSize.Height;
                    rectImage = new Rectangle(rectTextAndImage.X + m_imageTextIndent, rectTextAndImage.Y, imageWidth, imageHeight);

                    int textWidth = rectTextAndImage.Width - imageWidth - m_imageTextIndent * 2 - c_imageTextPadding - this.GetOverlappedWidth() / 2;
                    int textHeight = rectTextAndImage.Height;
                    rectText = new Rectangle(rectImage.Right + c_imageTextPadding, rectTextAndImage.Y, textWidth, textHeight);

                    g.DrawImage(container.ImageList.Images[m_parent.ImageIndex], rectImage);
                }
                else
                {
                    rectText = rectTextAndImage;
                    rectText.Width -= this.GetOverlappedWidth() / 2;
                    rectText.X -= 2;
                }

                Font font = this.InactiveTabFont;
                if (m_parent.Pushed)
                {
                    font = this.ActiveTabFont;
                }

                this.DrawText(g, this.TabText, font, this.ForeColor, rectText, page.TabEnabled);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="foreColor"></param>
        /// <param name="rectText"></param>
        /// <param name="enabled"></param>
        protected virtual void DrawText(Graphics g, string text, Font font, Color foreColor, Rectangle rectText, bool enabled)
        {
            this.DrawTextInternal(g, text, font, foreColor, rectText, enabled);
        }

        /// <summary>
        /// Gets tab preferred size.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual Size GetItemPreferredSize()
        {
            Size preferredSize = Size.Empty;

            string measuredString = (this.TabText == string.Empty) ? c_measureString : this.TabText;

            Size activeTabSize = Size.Ceiling(this.MeasureText(measuredString, this.ActiveTabFont));
            Size inactiveTabSize = Size.Ceiling(this.MeasureText(measuredString, this.InactiveTabFont));

            preferredSize.Width += Math.Max(activeTabSize.Width, inactiveTabSize.Width);
            preferredSize.Height += Math.Max(activeTabSize.Height, inactiveTabSize.Height);

            IInternalTabParent container = m_parent.Owner as IInternalTabParent;
            if (container.ImageList != null && m_parent.ImageIndex >= 0 && m_parent.ImageIndex < container.ImageList.Images.Count)
            {
                int imageWidth = container.ImageList.ImageSize.Width + c_imageTextPadding;
                preferredSize.Width += imageWidth;
                preferredSize.Height = Math.Max(container.ImageList.ImageSize.Height, preferredSize.Height);
            }

            preferredSize.Width += m_imageTextIndent * 2;

            return preferredSize;
        }

        /// <summary>
        /// Returns the overlapped size of the tabs.
        /// </summary>
        /// <returns></returns>
        public virtual int GetOverlappedWidth()
        {
            return 0;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Component and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>See the documentation for the <see cref="System.ComponentModel.Component"/> class and its Dispose member.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_parent = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Class utility methods

        /// <summary>
        /// Measures the text.
        /// </summary>
        /// <param name="text">The text to be measured.</param>
        /// <param name="font">The font of the text.</param>
        /// <returns>Size of the text.</returns>
        private SizeF MeasureText(string text, Font font)
        {
            SizeF sizeRect = SizeF.Empty;

            Graphics graphics = this.MeasureGraphics; // Graphics for measuring text.
            sizeRect = ControlDrawing.MeasureDisplayStringSize(graphics, text, font, false);

            return sizeRect;
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        private void DrawTextInternal(Graphics g, string text, Font font, Color foreColor, RectangleF rectText, bool enabled)
        {
            if (enabled)
            {
                this.DrawTextNative(g, text, font, foreColor, rectText);
            }
            else
            {
                rectText.Offset(1, 1);
                Color color = ControlPaint.LightLight(SystemColors.Control);
                DrawTextNative(g, text, font, color, rectText);

                rectText.Offset(-1, -1);
                color = ControlPaint.Dark(color);
                DrawTextNative(g, text, font, color, rectText);
            }
        }

        /// <summary>
        /// Draw text by native GDI API.
        /// </summary>
        private void DrawTextNative(Graphics g, string text, Font f, Color color, RectangleF textRect)
        {
            int nFlags = c_drawTextFlags;
            nFlags |= DrawTextFormats.DT_CENTER;
            nFlags |= DrawTextFormats.DT_VCENTER;

            IntPtr clipRgn = g.Clip.GetHrgn(g);
            IntPtr hdc = g.GetHdc();
            IntPtr hFont = f.ToHfont();
            IntPtr prevFont = NativeMethods.SelectObject(hdc, hFont);

            NativeMethods.SelectClipRgn(hdc, clipRgn);
            Color invertedColor = Color.FromArgb(0, color.B, color.G, color.R);
            NativeMethods.SetTextColor(hdc, invertedColor.ToArgb() & 0xFFFFFF);
            NativeMethods.SetBkMode(hdc, 1); // TRANSPARENT

            Rectangle r = Rectangle.Ceiling(textRect);
            NativeMethods.RECT rect = new NativeMethods.RECT(r);

            NativeMethods.DrawText(hdc, text, text.Length, ref rect, nFlags);

            prevFont = NativeMethods.SelectObject(hdc, prevFont);

            NativeMethods.DeleteObject(hFont);
            NativeMethods.DeleteObject(clipRgn);

            NativeMethods.SelectClipRgn(hdc, IntPtr.Zero);

            g.ReleaseHdc(hdc);
        }
        #endregion
    }
}