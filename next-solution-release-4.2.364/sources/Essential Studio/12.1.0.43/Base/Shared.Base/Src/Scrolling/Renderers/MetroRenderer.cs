#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region Directives
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Renderers;
#endregion

namespace Syncfusion.Windows.Forms
{
    /// <summary>
    /// The metro renderer for scrollbar
    /// </summary>
    public class MetroRenderer : ClassicRenderer
    {
        #region Class Members
        private MetroColorTable m_colorTable;
        private Bitmaps m_bitmaps;
        #endregion

        #region Class Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="MetroRenderer"/> class.
        /// </summary>
        /// <param name="parent">The parent control</param>
        public MetroRenderer(ScrollBarCustomDraw parent)
            : base(parent)
        {
            m_colorTable = new MetroColorTable();
            m_bitmaps = new Bitmaps((int)EIMAGE.MAX);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetroRenderer"/> class.
        /// </summary>
        /// <param name="parent">The parent control.</param>
        /// <param name="colorTable">The metro color table.</param>
        public MetroRenderer(ScrollBarCustomDraw parent, MetroColorTable colorTable)
            : base(parent)
        {
            m_colorTable = colorTable;
            m_bitmaps = new Bitmaps((int)EIMAGE.MAX);
        }
        #endregion

        #region Background
        /// <summary>
        /// Draws background of scroll. If theme is disabled than draw classic scroll.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="rcBackground">The bounds of background.</param>
        /// <param name="state">The scroll button state.</param>
        public override void DrawBackground(Graphics gfx, Rectangle rcBackground, ButtonState state)
        {
            if (null == gfx)
                throw new ArgumentNullException("g");

            if (!m_parent.ThemeEnabled)
                base.DrawBackground(gfx, rcBackground, state);
            else
            {
                if (rcBackground.Width > 0 && rcBackground.Height > 0)
                {
                    if (state == ButtonState.Normal)
                        DrawBackground(gfx, rcBackground);
                    else if (state == ButtonState.Pushed)
                        DrawPushedBackground(gfx, rcBackground);
                }
            }
        }

        /// <summary>
        /// Draws pushed background.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="bounds">The bounds of background.</param>
        private void DrawPushedBackground(Graphics gfx, Rectangle bounds)
        {
            using (SolidBrush brush = new SolidBrush(m_colorTable.ScrollerBackground))
            {
                gfx.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Draws scroll background.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="bounds">The scroll bounds.</param>
        private void DrawBackground(Graphics gfx, Rectangle bounds)
        {
            using (SolidBrush brush = new SolidBrush(m_colorTable.ScrollerBackground))
            {
                gfx.FillRectangle(brush, bounds);
            }
        }
        #endregion

        #region Thumb
        /// <summary>
        /// Draws scroll thumb. If theme is disabled than draw classic scroll. 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="rcThumb"></param>
        /// <param name="state"></param>
        public override void DrawThumb(Graphics gfx, Rectangle rcThumb, ButtonState state)
        {
            if (null == gfx)
                throw new ArgumentNullException("g");

            if (!m_parent.ThemeEnabled)
            {
                base.DrawThumb(gfx, rcThumb, state);
            }
            else
            {
                DrawThumbBackground(gfx, rcThumb, state);
            }
        }

        /// <summary>
        /// Draws scroll thumb background.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="rect">The bounds of thumb.</param>
        /// <param name="state">The scroll button state.</param>
        private void DrawThumbBackground(Graphics gfx, Rectangle rect, ButtonState state)
        {
            Color color = Color.Empty;

            switch (state)
            {
                case ButtonState.Normal:
                    color = m_colorTable.ThumbNormal;
                    break;
                case ButtonState.Checked:
                    color = m_colorTable.ThumbChecked;
                    break;
                case ButtonState.Pushed:
                    color = m_colorTable.ThumbPushed;
                    break;
                case ButtonState.Inactive:
                    color = m_colorTable.ThumbInActive;
                    break;
                default:
                    color = m_colorTable.ThumbNormal;
                    break;
            }
            using (SolidBrush brush = new SolidBrush(color))
            {
                using (Brush br = new SolidBrush(this.m_colorTable.ScrollerBackground))
                    gfx.FillRectangle(br, rect);
                if (IsVerticalScrollBar)
                {
                    rect.X += 3;
                    rect.Width -= 6;
                }
                else
                {
                    rect.Y += 3;
                    rect.Height -= 6;
                }
                gfx.FillRectangle(brush, rect);
            }
        }
        #endregion

        #region Arrow
       /// <summary>
        /// Draws arrow button of scroll. If theme is disabled than draw classic scroll. 
       /// </summary>
       /// <param name="gfx">The graphics.</param>
       /// <param name="rcArrow">The bounds of arrow.</param>
       /// <param name="type">The scroll button type.</param>
       /// <param name="state">The scroll button state.</param>
        public override void DrawArrowButton(Graphics gfx, Rectangle rcArrow, ScrollButton type, ButtonState state)
        {
            if (null == gfx)
                throw new ArgumentNullException("graphics");

            if (!m_parent.ThemeEnabled)
                base.DrawArrowButton(gfx, rcArrow, type, state);
            else
                DrawArrowBackground(gfx, rcArrow, type, state);
        }

        /// <summary>
        /// Draws arrow background.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="rect">The arrow backgroud bounds.</param>
        /// <param name="type">The scroll button type.</param>
        /// <param name="state">The scroll button state.</param>
        private void DrawArrowBackground(Graphics gfx, Rectangle rect, ScrollButton type, ButtonState state)
        {
            using (SolidBrush brush = new SolidBrush(m_colorTable.ScrollerBackground))
                gfx.FillRectangle(brush, rect);

            DrawArrow(gfx, rect, type, state);
        }

        /// <summary>
        /// Draws scrollbar arrow
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="rect">The bounds of the arrow.</param>
        /// <param name="type">The scroll button type.</param>
        /// <param name="state">The scroll button state.</param>
        private void DrawArrow(Graphics gfx, Rectangle rect, ScrollButton type, ButtonState state)
        {
            int iImageWidth = rect.Width;
            int iImageHeight = rect.Height;
            int iArrowHeight = 0;
            int iArrowWidth = 0;
            Point p = Point.Empty;
            Bitmap bmpArrow;

            switch (type)
            {
                case ScrollButton.Down:
                case ScrollButton.Up:
                    iArrowWidth = (int)((iImageHeight) / 2) + 1;
                    iArrowHeight = (int)(iArrowWidth / 2) + 1;

                    if (type == ScrollButton.Down)
                    {
                        if (state == ButtonState.Pushed || state == ButtonState.Checked)
                            bmpArrow = GetOfficeDownArrow(iArrowWidth, iArrowHeight);
                        else
                            bmpArrow = GetDownArrowNormal(iArrowWidth, iArrowHeight);
                    }
                    else
                    {
                        if (state == ButtonState.Pushed || state == ButtonState.Checked)
                            bmpArrow = GetOfficeUpArrow(iArrowWidth, iArrowHeight);
                        else
                            bmpArrow = GetUpArrowNormal(iArrowWidth, iArrowHeight);
                    }

                    p.X = rect.Left + (iImageWidth - iArrowWidth) / 2;
                    p.Y = rect.Top + (iImageHeight - iArrowHeight) / 2;

                    gfx.DrawImage(bmpArrow, p);
                    break;

                case ScrollButton.Right:
                case ScrollButton.Left:
                    iArrowHeight = (int)(iImageWidth / 2) + 1;
                    iArrowWidth = (int)(iArrowHeight / 2) + 1;

                    if (type == ScrollButton.Right)
                    {
                        if (state == ButtonState.Pushed || state == ButtonState.Checked)
                            bmpArrow = GetOfficeRightArrow(iArrowWidth, iArrowHeight);
                        else
                            bmpArrow = GetRightArrowNormal(iArrowWidth, iArrowHeight);
                    }
                    else
                    {
                        if (state == ButtonState.Pushed || state == ButtonState.Checked)
                            bmpArrow = GetOfficeLeftArrow(iArrowWidth, iArrowHeight);
                        else
                            bmpArrow = GetLeftArrowNormal(iArrowWidth, iArrowHeight);
                    }

                    p.X = rect.Left + (iImageWidth - iArrowWidth) / 2;
                    p.Y = rect.Top + (iImageHeight - iArrowHeight) / 2;

                    gfx.DrawImage(bmpArrow, p);
                    break;
            }
        }

        /// <summary>
        /// Returns the pushed or checked state right arrow.
        /// </summary>
        /// <param name="width">The width of the arrow.</param>
        /// <param name="height">The height of the arrow.</param>
        /// <returns>The bitmap.</returns>
        protected override Bitmap GetOfficeRightArrow(int width, int height)
        {
            Bitmap bitmap = m_bitmaps[EIMAGE.eiRightArrow] as Bitmap;

            if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    gfx.Clear(Color.Transparent);

                    Rectangle rcRightArrow = new Rectangle(0, 0, 1, height);

                    using (Region region = new Region(rcRightArrow))
                    {
                        region.Union(rcRightArrow);

                        for (int i = 0; i < width; i++)
                        {
                            rcRightArrow.Inflate(0, -1);
                            rcRightArrow.X += 1;
                            region.Union(rcRightArrow);
                        }               

                        using (Brush brush = new SolidBrush(m_colorTable.ArrowChecked))
                        {
                            gfx.FillRegion(brush, region);
                        }
                    }
                }

                m_bitmaps[EIMAGE.eiRightArrow] = bitmap;
            }

            return bitmap;
        }

        /// <summary>
        /// Returns the pushed or checked state left arrow.
        /// </summary>
        /// <param name="width">The width of the arrow.</param>
        /// <param name="height">The height of the arrow.</param>
        /// <returns>The bitmap.</returns>
        protected override Bitmap GetOfficeLeftArrow(int width, int height)
        {
            Bitmap bitmap = m_bitmaps[EIMAGE.eiLeftArrow] as Bitmap;

            if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    gfx.Clear(Color.Transparent);

                    Rectangle rcLeftArrow = new Rectangle(width - 1, 0, 1, height);

                    using (Region region = new Region(rcLeftArrow))
                    {
                        region.Union(rcLeftArrow);

                        for (int i = 0; i < width; i++)
                        {
                            rcLeftArrow.Inflate(0, -1);
                            rcLeftArrow.X -= 1;
                            region.Union(rcLeftArrow);
                        }

                        using (Brush brush = new SolidBrush(m_colorTable.ArrowChecked))
                        {
                            gfx.FillRegion(brush, region);
                        }
                    }
                }

                m_bitmaps[EIMAGE.eiLeftArrow] = bitmap;
            }

            return bitmap;
        }

        /// <summary>
        /// Returns the pushed or checked state down arrow.
        /// </summary>
        /// <param name="width">The width of the arrow.</param>
        /// <param name="height">The height of the arrow.</param>
        /// <returns>The bitmap.</returns>
        protected override Bitmap GetOfficeDownArrow(int width, int height)
        {
            Bitmap bitmap = m_bitmaps[EIMAGE.eiDownArrow] as Bitmap;

            if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    gfx.Clear(Color.Transparent);

                    Rectangle rcDownArrow = new Rectangle(0, 0, width, 1);

                    using (Region region = new Region(rcDownArrow))
                    {
                        region.Union(rcDownArrow);

                        for (int i = 0; i < height; i++)
                        {
                            rcDownArrow.Inflate(-1, 0);
                            rcDownArrow.Y += 1;
                            region.Union(rcDownArrow);
                        }

                        using (Brush brush = new SolidBrush(m_colorTable.ArrowChecked))
                        {
                            gfx.FillRegion(brush, region);
                        }
                    }
                }

                m_bitmaps[EIMAGE.eiDownArrow] = bitmap;
            }

            return bitmap;
        }

        /// <summary>
        /// Returns the pushed or checked state up arrow.
        /// </summary>
        /// <param name="width">The width of the arrow.</param>
        /// <param name="height">The height of the arrow.</param>
        /// <returns>The bitmap.</returns>
        protected override Bitmap GetOfficeUpArrow(int width, int height)
        {
            Bitmap bitmap = m_bitmaps[EIMAGE.eiUpArrow] as Bitmap;

            if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    gfx.Clear(Color.Transparent);

                    Rectangle rcUpArrow = new Rectangle(0, height - 1, width, 1);

                    using (Region region = new Region(rcUpArrow))
                    {
                        region.Union(rcUpArrow);

                        for (int i = 0; i < height; i++)
                        {
                            rcUpArrow.Inflate(-1, 0);
                            rcUpArrow.Y -= 1;
                            region.Union(rcUpArrow);
                        }

                        using (Brush brush = new SolidBrush(m_colorTable.ArrowChecked))
                        {
                            gfx.FillRegion(brush, region);
                        }
                    }
                }

                m_bitmaps[EIMAGE.eiUpArrow] = bitmap;
            }

            return bitmap;
        }

        /// <summary>
        /// Returns the normal right arrow.
        /// </summary>
        /// <param name="width">The width of the arrow.</param>
        /// <param name="height">The height of the arrow.</param>
        /// <returns>The bitmap.</returns>
        protected Bitmap GetRightArrowNormal(int width, int height)
        {
            Bitmap bitmap = m_bitmaps[EIMAGE.eiRightArrowNormal] as Bitmap;

            if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    gfx.Clear(Color.Transparent);

                    Rectangle rcRightArrow = new Rectangle(0, 0, 1, height);

                    using (Region region = new Region(rcRightArrow))
                    {
                        region.Union(rcRightArrow);

                        for (int i = 0; i < width; i++)
                        {
                            rcRightArrow.Inflate(0, -1);
                            rcRightArrow.X += 1;
                            region.Union(rcRightArrow);
                        }
              
                        using (Brush brush = new SolidBrush(m_colorTable.ArrowNormal))
                        {
                            gfx.FillRegion(brush, region);
                        }
                    }
                }

                m_bitmaps[EIMAGE.eiRightArrowNormal] = bitmap;
            }

            return bitmap;
        }

        /// <summary>
        /// Returns the normal left arrow.
        /// </summary>
        /// <param name="width">The width of the arrow.</param>
        /// <param name="height">The height of the arrow.</param>
        /// <returns>The bitmap.</returns>
        protected Bitmap GetLeftArrowNormal(int width, int height)
        {
            Bitmap bitmap = m_bitmaps[EIMAGE.eiLeftArrowNormal] as Bitmap;

            if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    gfx.Clear(Color.Transparent);

                    Rectangle rcLeftArrow = new Rectangle(width - 1, 0, 1, height);

                    using (Region region = new Region(rcLeftArrow))
                    {
                        region.Union(rcLeftArrow);

                        for (int i = 0; i < width; i++)
                        {
                            rcLeftArrow.Inflate(0, -1);
                            rcLeftArrow.X -= 1;
                            region.Union(rcLeftArrow);
                        }
          
                        using (Brush brush = new SolidBrush(m_colorTable.ArrowNormal))
                        {
                            gfx.FillRegion(brush, region);
                        }
                    }
                }

                m_bitmaps[EIMAGE.eiLeftArrowNormal] = bitmap;
            }

            return bitmap;
        }

        /// <summary>
        /// Returns the normal down arrow.
        /// </summary>
        /// <param name="width">The width of the arrow.</param>
        /// <param name="height">The height of the arrow.</param>
        /// <returns>The bitmap.</returns>
        protected Bitmap GetDownArrowNormal(int width, int height)
        {
            Bitmap bitmap = m_bitmaps[EIMAGE.eiDownArrowNormal] as Bitmap;

            if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    gfx.Clear(Color.Transparent);

                    Rectangle rcDownArrow = new Rectangle(0, 0, width, 1);

                    using (Region region = new Region(rcDownArrow))
                    {
                        region.Union(rcDownArrow);

                        for (int i = 0; i < height; i++)
                        {
                            rcDownArrow.Inflate(-1, 0);
                            rcDownArrow.Y += 1;
                            region.Union(rcDownArrow);
                        }

                        using (Brush brush = new SolidBrush(m_colorTable.ArrowNormal))
                        {
                            gfx.FillRegion(brush, region);
                        }
                    }
                }

                m_bitmaps[EIMAGE.eiDownArrowNormal] = bitmap;
            }

            return bitmap;
        }

       /// <summary>
       /// Returns the normal up arrow.
       /// </summary>
       /// <param name="width">The width of the arrow.</param>
       /// <param name="height">The height of the arrow.</param>
       /// <returns>The bitmap.</returns>
        protected Bitmap GetUpArrowNormal(int width, int height)
        {
            Bitmap bitmap = m_bitmaps[EIMAGE.eiUpArrowNormal] as Bitmap;

            if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    gfx.Clear(Color.Transparent);

                    Rectangle rcUpArrow = new Rectangle(0, height - 1, width, 1);

                    using (Region region = new Region(rcUpArrow))
                    {
                        region.Union(rcUpArrow);

                        for (int i = 0; i < height; i++)
                        {
                            rcUpArrow.Inflate(-1, 0);
                            rcUpArrow.Y -= 1;
                            region.Union(rcUpArrow);
                        }

                        using (Brush brush = new SolidBrush(m_colorTable.ArrowNormal))
                        {
                            gfx.FillRegion(brush, region);
                        }
                    }
                }

                m_bitmaps[EIMAGE.eiUpArrowNormal] = bitmap;
            }
            return bitmap;
        }
        #endregion
    }

    /// <summary>
    /// Metro color table.
    /// </summary>
    public partial class MetroColorTable
    {
        #region Class Members
        private Color m_cScrollerBackground;
        private Color m_cThumbNormal;
        private Color m_cThumbChecked;
        private Color m_cThumbPushed;
        private Color m_cThumbInActive;

        private Color m_cArrowNormal;
        private Color m_cArrowChecked;
        private Color m_cArrowPushed;
        private Color m_cArrowInActive;
        #endregion

        #region Class Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="MetroColorTable"/> class.
        /// </summary>
        public MetroColorTable()
        {
            //initialize defaults.
            m_cScrollerBackground = Color.White;
            m_cThumbNormal = Color.FromArgb(198,198,198);
            m_cThumbInActive = Color.White;
            m_cThumbPushed = Color.FromArgb(88, 89, 90);
            m_cThumbChecked = Color.FromArgb(147,149,152);

            m_cArrowNormal = Color.FromArgb(198, 198, 198);
            m_cArrowInActive = Color.White;
            m_cArrowPushed = Color.FromArgb(88, 89, 90);
            m_cArrowChecked = Color.FromArgb(147, 149, 152);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the scroll bar background color.
        /// </summary>
        public Color ScrollerBackground
        {
            get
            {
                return m_cScrollerBackground;
            }
            set
            {
                if (value != m_cScrollerBackground)
                    m_cScrollerBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets the normal state thumb color.
        /// </summary>
        public Color ThumbNormal
        {
            get
            {
                return m_cThumbNormal;
            }
            set
            {
                if (value != m_cThumbNormal)
                    m_cThumbNormal = value;
            }
        }

        /// <summary>
        /// Gets or sets the checked state thumb color.
        /// </summary>
        public Color ThumbChecked
        {
            get
            {
                return m_cThumbChecked;
            }
            set
            {
                if (value != m_cThumbChecked)
                    m_cThumbChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets the pushed state thumb color. 
        /// </summary>
        public Color ThumbPushed
        {
            get
            {
                return m_cArrowPushed;
            }
            set
            {
                if (value != m_cArrowPushed)
                    m_cArrowPushed = value;
            }
        }

        /// <summary>
        /// Gets or sets the inactive state thumb color.
        /// </summary>
        public Color ThumbInActive
        {
            get
            {
                return m_cThumbInActive;
            }
            set
            {
                if (value != m_cThumbInActive)
                    m_cThumbInActive = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the normal state arrow color.
        /// </summary>
        public Color ArrowNormal
        {
            get
            {
                return m_cArrowNormal;
            }
            set
            {
                if (value != m_cArrowNormal)
                    m_cArrowNormal = value;
            }
        }

        /// <summary>
        /// Gets or sets the checked state arrow color.
        /// </summary>
        public Color ArrowChecked
        {
            get
            {
                return m_cArrowChecked;
            }
            set
            {
                if (value != m_cArrowChecked)
                    m_cArrowChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets the pushed state arrow color.
        /// </summary>
        public Color ArrowPushed
        {
            get
            {
                return m_cArrowPushed;
            }
            set
            {
                if (value != m_cArrowPushed)
                    m_cArrowPushed = value;
            }
        }

        /// <summary>
        /// Gets or sets the inactive state arrow color.
        /// </summary>
        public Color ArrowInActive
        {
            get
            {
                return m_cArrowInActive;
            }
            set
            {
                if (value != m_cArrowInActive)
                    m_cArrowInActive = value;
            }
        }
        #endregion
    }
}
