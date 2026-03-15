#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
    public class MetroStatusStripExRenderer : Office2013ToolStripRenderer
    {
        #region Constants

        private const int SEPARATOR_LEFT = 27;
        #endregion

        #region Constructors
        static MetroStatusStripExRenderer()
        {
        }

        public MetroStatusStripExRenderer(Color MetroColor, Color BackColor)
        {
            base.MenuColor = MetroColor;
            metroColor = MetroColor;
            backColor = BackColor;
        }
        #endregion

        #region Destructors
        ~MetroStatusStripExRenderer()
        {
            metroColor = Color.Empty;
        }
        #endregion

        #region Methods

        public void DrawBackground(StatusStripEx statusStrip, PaintEventArgs pe)
        {
        }
        #endregion

        #region Overrides
        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            if (SystemInfo.IsVisualStyleEnabled)
            {
                if (e.ToolStrip is StatusStripEx)
                {

                    bResult = PaintToolstripBackground(e);
                }
            }

            if (!bResult)
            {
                base.OnRenderToolStripBackground(e);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintImageMargin(e))
            {
                base.OnRenderImageMargin(e);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintSeparators(e))
            {
                base.OnRenderSeparator(e);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (!PaintMenuItemText(e))
            {
                base.OnRenderItemText(e);
            }
        }
        #endregion

        #region Implementation

        private Brush GetBackgroundBrush(int nHeight)
        {
            SolidBrush brush = new SolidBrush(backColor);          
            return brush;
        }

        private Brush GetBackgroundBrushDark(int nHeight)
        {
            SolidBrush brush = new SolidBrush(backColor);
            return brush;
        }

        private bool PaintToolstripBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            RECT rect = new RECT();

            StatusStripEx statusStrip = e.ToolStrip as StatusStripEx;

            if (statusStrip != null)
            {
                if (WindowsAPI.GetWindowRect(statusStrip.Handle, ref rect))
                {
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Graphics g = e.Graphics;

                        PanelItemRenderEventArgs pie = e as PanelItemRenderEventArgs;

                        Rectangle rectLight = statusStrip.NotificationslArea;
                        Rectangle rectDark = statusStrip.StatusControlsArea;

                        if (pie != null)
                        {
                            Point offset = pie.PanelStrip.Location;
                            rectLight.Offset(-offset.X, -offset.Y);
                            rectDark.Offset(-offset.X, -offset.Y);
                        }

                        if (statusStrip.BackColor == SystemColors.Control)
                        {

                            if (rectLight.Width > 0)
                            {
                                using (Brush brush = GetBackgroundBrush(rectLight.Height))
                                {

                                    g.FillRectangle(brush, rectLight);
                                }
                            }

                            if (rectDark.Width > 0)
                            {
                                using (Brush brush = GetBackgroundBrushDark(rectDark.Height))
                                {

                                    g.FillRectangle(brush, rectDark);
                                }
                            }
                        }
                        else
                        {
                            using (Brush brush = new SolidBrush(statusStrip.BackColor))
                            {
                                g.FillRectangle(brush, rectDark);
                            }
                        }

                        int iSeparatorsCount = statusStrip.Separators.Count;

                        // Draw separators between items in left side.
                        if (iSeparatorsCount > 0 && pie == null)
                        {
                            // Draw separators
                            using (Pen penDark = new Pen(backColor))
                            {
                                using (Pen penLight = new Pen(backColor))
                                {
                                    for (int i = 0; i < iSeparatorsCount; i++)
                                    {
                                        if (statusStrip.RightToLeft == RightToLeft.Yes)
                                        {
                                            g.DrawLine(penDark, statusStrip.Separators[i], 0, statusStrip.Separators[i], rectDark.Height);
                                            g.DrawLine(penLight, statusStrip.Separators[i] + 1, 0, statusStrip.Separators[i] + 1, rectDark.Height);
                                        }
                                        else
                                        {
                                            g.DrawLine(penLight, statusStrip.Separators[i], 0, statusStrip.Separators[i], rectDark.Height);
                                            g.DrawLine(penDark, statusStrip.Separators[i] + 1, 0, statusStrip.Separators[i] + 1, rectDark.Height);
                                        }
                                    }
                                }
                            }
                        }

                        // Draw main separator.
                        if (statusStrip.SeparatorPosition != -1 && pie == null)
                        {
                            using (Pen penBegin = new Pen(Color.DarkGray))
                            {                               
                                    g.DrawLine(penBegin, statusStrip.SeparatorPosition, 0, statusStrip.SeparatorPosition, rectDark.Height);                               
                            }
                        }

                        if (statusStrip.Dock == DockStyleEx.BottomMost && pie == null)
                        {
                            using (Pen penBorder = new Pen(Color.DarkGray))
                            {
                                //g.DrawLine(penBorder, 0, 0, statusStrip.Width, 0);
                            }
                        }

                        int iLeft = statusStrip.Width - statusStrip.DisplayRectangle.Width;

                        bResult = true;
                    }
                }
            }

            return bResult;
        }

        private bool PaintImageMargin(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            if (e.ToolStrip is StatusStripEx)
            {
                Graphics g = e.Graphics;
                Rectangle rc = Rectangle.Inflate(e.AffectedBounds, -1, -2);

                using (Brush brush = new SolidBrush(ColorTable.ImageMarginGradientBegin))
                {
                    g.FillRectangle(brush, rc);
                }

                g.DrawLine(Pens.White, rc.Left, rc.Top, rc.Left, rc.Bottom - 1);
                g.DrawLine(this.MenuItemBorder, rc.Right, rc.Top, rc.Right, rc.Bottom - 1);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintSeparators(ToolStripSeparatorRenderEventArgs e)
        {
            bool bResult = false;

            Graphics g = e.Graphics;

            ToolStripSeparator item = e.Item as ToolStripSeparator;

            if (item != null)
            {
                Rectangle rc = new Rectangle(Point.Empty, item.Size);
                int iTop = (rc.Bottom - rc.Top) / 2;
                g.DrawLine(this.MenuItemBorder, SEPARATOR_LEFT, iTop, rc.Right, iTop);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintMenuItemText(ToolStripItemTextRenderEventArgs e)
        {
            bool bResult = false;

            if (e.ToolStrip is StatusContextMenuStrip)
            {
                Graphics g = e.Graphics;

                Size szItem = e.Item.Size;
                Padding pd = e.ToolStrip.Padding;

                Rectangle rcText = e.TextRectangle;
                Rectangle rcStatus = new Rectangle(0, rcText.Y, szItem.Width - pd.Horizontal - rcText.Width, rcText.Height);

                TextFormatFlags tf = TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;

                if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
                {
                    tf |= TextFormatFlags.Right | TextFormatFlags.RightToLeft;
                    rcStatus.X = pd.Left;
                    rcText.X = rcStatus.Right;
                }
                else
                {
                    rcStatus.X = rcText.Right;
                }

                Font textFont = e.TextFont;
                Color textColor = e.Item.ForeColor;

                TextRenderer.DrawText(g, e.Text, textFont, rcText, textColor, tf);

                StatusContextMenuStrip.StatusMenuItem statusItem = e.Item as StatusContextMenuStrip.StatusMenuItem;

                if (statusItem != null)
                {
                    string sStatus = statusItem.Status;

                    if (!string.IsNullOrEmpty(sStatus))
                    {
                        TextRenderer.DrawText(g, sStatus, textFont, rcStatus, textColor, tf ^ TextFormatFlags.Right);
                    }
                }
                bResult = true;
            }
            return bResult;
        }

        #endregion

        #region Fields

        private Color metroColor;
        private Color backColor;

        #endregion
    }
}
#endif
