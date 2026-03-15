#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces;
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Control representing one tab group.
    /// </summary>
    [Designer(typeof(RibbonTabGroupDesigner))]
    [ToolboxItem(false)]
    public partial class RibbonTabGroup
        : ToolStrip
    {
        #region Constants
        /// <summary>
        /// Blend positions for gradient background.
        /// </summary>
        private static readonly float[] DEF_BACKGROUND_BLEND_POSITIONS = new float[] { 0.0F, 0.5F, 1.0F };
    
        /// <summary>
        /// Blend factors for gradient background.
        /// </summary>
        private static readonly float[] DEF_BACKGROUND_BLEND_FACTORS = new float[] { 0.0F, 1.0F, 0.0F };
      
        /// <summary>
        /// Number of pixels to be added to the height of the font when size of the caption is being calculated.
        /// </summary>
        private const int DEF_PIXELS_TO_INFLATE_CAPTION = 2;

        /// <summary>
        /// Default caption font color.
        /// </summary>
        internal static readonly Color DEF_INITIAL_CAPTION_FONT_COLOR = Color.Firebrick;

        /// <summary>
        /// Default first color of caption.
        /// </summary>
        internal static readonly Color DEF_INITIAL_CAPTION_COLOR1 = Color.White;

        /// <summary>
        /// Default second color of caption.
        /// </summary>
        internal static readonly Color DEF_INITIAL_CAPTION_COLOR2 = Color.Orange;
        #endregion

        #region Fields
        /// <summary>
        /// Indicates whether group stores single item.
        /// </summary>
        private bool m_bSingleItem;

        /// <summary>
        /// Color of caption font.
        /// </summary>
        private Color m_clrCaptionFont = DEF_INITIAL_CAPTION_FONT_COLOR;

        /// <summary>
        /// The first color of caption gradiend.
        /// </summary>
        private Color m_clrCaption1 = DEF_INITIAL_CAPTION_COLOR1;

        /// <summary>
        /// The second color of caption gradiend
        /// </summary>
        private Color m_clrCaption2 = DEF_INITIAL_CAPTION_COLOR2;
        #endregion

        #region Static Fields
        /// <summary>
        /// Blend for painting gradient background.
        /// </summary>
        private static Blend s_backgroundBlend = new Blend();
        
        /// <summary>
        /// Font for drawing caption titles.
        /// </summary>
        private static Font s_captionFont;

        /// <summary>
        /// Height of the caption.
        /// </summary>
        private static int s_captionHeight;
        #endregion

        #region Internal Fields
        /// <summary>
        /// Indicates whether group is active and should highlight clicked item in design time. For designer internal usage only.
        /// </summary>
        protected internal bool Active;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether group stores single item.
        /// </summary>
        [Browsable(false)]
        public bool SingleItem
        {
            get
            {
                return m_bSingleItem;
            }
            set
            {
                if (m_bSingleItem != value)
                {
                    m_bSingleItem = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color of caption font.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color CaptionFontColor
        {
            get
            {
                return m_clrCaptionFont;
            }
            set
            {
                if (m_clrCaptionFont == value) return;

                m_clrCaptionFont = value;

                NativeMethods.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE);                  
            }
        }

        /// <summary>
        /// Gets or sets the first color of caption gradiend.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color CaptionColor1
        {
            get
            {
                return m_clrCaption1;
            }
            set
            {
                if (m_clrCaption1 == value) return;

                m_clrCaption1 = value;

                NativeMethods.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE);                    
            }
        }

        /// <summary>
        /// Gets or sets the second color of caption gradiend.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color CaptionColor2
        {
            get
            {
                return m_clrCaption2;
            }
            set
            {
                if (m_clrCaption2 == value) return;

                m_clrCaption2 = value;

                NativeMethods.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE);                   
            }
        }
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets height of the caption.
        /// </summary>
        public static int CaptionHeight
        {
            get
            {
                return s_captionHeight;
            }
        }
        #endregion

        #region Initialization

        static RibbonTabGroup()
        {
            s_backgroundBlend.Positions = DEF_BACKGROUND_BLEND_POSITIONS;
            s_backgroundBlend.Factors = DEF_BACKGROUND_BLEND_FACTORS;

            s_captionFont = new Font(Control.DefaultFont.FontFamily, Control.DefaultFont.Size, FontStyle.Bold);

            s_captionHeight = (int)s_captionFont.GetHeight() + DEF_PIXELS_TO_INFLATE_CAPTION;
        }

        /// <summary>
        /// Initializes a new instance of the RibbonTabGroup class.
        /// </summary>
        public RibbonTabGroup()
        {
            this.GripStyle = ToolStripGripStyle.Hidden;
            this.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.AutoSize = true;
            this.CanOverflow = false;

            this.Text = this.Name;
        }
        #endregion

        #region Overrides
        
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCCALCSIZE:
                    OnNcCalcSize(ref m);
                    break;
                case NativeMethods.WM_NCPAINT:
                    OnNcPaint(ref m);
                    break;
            }

            base.WndProc(ref m);
        }
         
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            ToolStripProfessionalRenderer renderer = this.Renderer as ToolStripProfessionalRenderer;

            if (renderer != null)
            {
                Color clBegin = renderer.ColorTable.MenuStripGradientBegin;
                Color clEnd = renderer.ColorTable.MenuStripGradientEnd;

                Control parentToDraw = ((IRibbonHeaderControl)this.Parent).GetParentForChildsTransparentRendering();
                Size szParent = parentToDraw.ClientSize;

                if (szParent.Width > 0 && szParent.Height > 0)
                {
                    Rectangle rcBrush = new Rectangle(0, 0, szParent.Width, 1);

                    using (LinearGradientBrush brush = new LinearGradientBrush(rcBrush, clBegin, clEnd, LinearGradientMode.Horizontal))
                    {
                        Point loc = parentToDraw.PointToClient(this.PointToScreen(new Point(0, 0)));

                        brush.TranslateTransform(szParent.Width - loc.X, szParent.Height - loc.Y, MatrixOrder.Append);
                        brush.Blend = s_backgroundBlend;

                        e.Graphics.FillRectangle(brush, this.ClientRectangle);

                        InvalidateInnerControls();
                    }
                }
            }
            else base.OnPaintBackground(e);
        }
        #endregion

        #region New Default Values
        /// <summary>
        /// Gets or sets new default value to GripSize property.
        /// </summary>
        [DefaultValue(ToolStripGripStyle.Hidden)]
        public new ToolStripGripStyle GripStyle
        {
            get
            {
                return base.GripStyle;
            }
            set
            {
                base.GripStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets new default value to LayoutStyle property.
        /// </summary>
        [DefaultValue(ToolStripLayoutStyle.HorizontalStackWithOverflow)]
        public new ToolStripLayoutStyle LayoutStyle
        {
            get
            {
                return base.LayoutStyle;
            }
            set
            {
                base.LayoutStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether default value to AutoSize property.
        /// </summary>
        [DefaultValue(true)]
        public new bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether CanOverflow property is set or not.
        /// </summary>
        [DefaultValue(false)]
        public new bool CanOverflow
        {
            get
            {
                return base.CanOverflow;
            }
            set
            {
                base.CanOverflow = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Forces inner controls of hosting items to invalidate.
        /// </summary>
        public void InvalidateWithInnerControl()
        {
            if (!this.IsDisposed)
            {
                Invalidate();

                if (this.SingleItem)
                {
                    NativeMethods.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE);                    
                }
                InvalidateInnerControls();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Invalidates inner controls.
        /// </summary>
        private void InvalidateInnerControls()
        {
            foreach (ToolStripItem item in this.Items)
            {
                ToolStripControlHost hostItem = item as ToolStripControlHost;

                if (hostItem != null)
                {
                    // Redraw NC area.
                    NativeMethods.RedrawWindow(hostItem.Control.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE);
                    hostItem.Control.Invalidate(true);   
                }
            }
        }

        private void OnNcCalcSize(ref Message m)
        {
            if (true)
            {
                RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                rc.top += (int)Math.Min(CaptionHeight, rc.Height);
                Marshal.StructureToPtr(rc, m.LParam, false);
            }
        }

        private void OnNcPaint(ref Message m)
        {
            IntPtr hWnd = m.HWnd;
            IntPtr hdc = WindowsAPI.GetWindowDC(hWnd);
            try
            {
                if (hdc != IntPtr.Zero)
                {
                    RECT rcWindow = new RECT();
                    WindowsAPI.GetWindowRect(hWnd, ref rcWindow);

                    RECT rcUpdate = new RECT(0, 0, rcWindow.Width, rcWindow.Height);
                    WindowsAPI.DefWindowProc(hWnd, Msg.WM_NCCALCSIZE, IntPtr.Zero, ref rcUpdate);

                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        Rectangle rcCaption = (Rectangle)rcUpdate;
                        rcCaption.Height = CaptionHeight;

                        if (this.SingleItem)
                        {
                            ToolStripProfessionalRenderer profRenderer = this.Renderer as ToolStripProfessionalRenderer;

                            if (profRenderer != null)
                            {
                                Color clBegin = profRenderer.ColorTable.MenuStripGradientBegin;
                                Color clEnd = profRenderer.ColorTable.MenuStripGradientEnd;
                                Control parentToDraw = ((IRibbonHeaderControl)this.Parent).GetParentForChildsTransparentRendering();
                                Size szParent = parentToDraw.ClientSize;

                                if (szParent.Width > 0 && szParent.Height > 0)
                                {
                                    Rectangle rcParent = new Rectangle(Point.Empty, szParent);

                                    using (LinearGradientBrush brush = new LinearGradientBrush(rcParent, clBegin, clEnd, LinearGradientMode.Horizontal))
                                    {
                                        Point loc = parentToDraw.PointToClient(this.PointToScreen(new Point(0, 0)));
                                        brush.TranslateTransform(szParent.Width - loc.X, szParent.Height - loc.Y, MatrixOrder.Append);
                                        brush.Blend = s_backgroundBlend;
                                        g.FillRectangle(brush, rcCaption);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (Brush brush = new LinearGradientBrush(rcCaption, m_clrCaption1, m_clrCaption2, LinearGradientMode.Vertical))
                            {
                                g.FillRectangle(brush, rcCaption);
                            }

                            rcCaption.Inflate(-5, 0);
                            rcCaption.Offset(1, 1);

                            using (Brush brush = new SolidBrush(m_clrCaptionFont))
                            {
                                g.DrawString(this.Text, s_captionFont, brush, rcCaption);
                            }
                        }
                    }                   
                }
            }
            finally
            {
                WindowsAPI.ReleaseDC(hWnd, hdc);
            }
        }
        #endregion
    }
}
#endif