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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
    public delegate void GetMinSizeHandler(ref Size szProposal);
    public delegate void ColorSchemeChanged(Object sender, RibbonForm.ColorSchemeType colorScheme);
    public delegate void RibbonStyleChanged(Object sender, RibbonStyle ribbonStyle);
    public delegate void MenuColorChanged(Object sender, Color menuColor);
    internal delegate void GetMarginsHandler(ref Padding pdMargings);

    #region DockStyleEx
    public enum DockStyleEx
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        Fill,
        TopMost,
        BottomMost
    }
    #endregion

    #region IDockExtended
    public interface IDockExtended
    {
        DockStyleEx Dock
        {
            get;
        }
    }
    #endregion

    #region
    
    internal interface IRibbonStyleNotifier
    {
        ToolStripEx.ColorScheme OfficeColorScheme { get; }

        RibbonStyle RibbonStyle { get; }
        Color MenuColor { get; }
        event ColorSchemeChanged ColorSchemeChanged;

        event RibbonStyleChanged RibbonStyleChanged;
        event MenuColorChanged MenuColorChanged;
    }

    #endregion

    #region RibbonForm
    [ProvideProperty("Shortcut", typeof(ToolStripItem))]
    public class RibbonForm : Form, IExtenderProvider, IMessageFilter
    {
        [DllImport("UxTheme")]
        public   static extern bool IsThemeActive();
        #region Constants
        public enum ColorSchemeType
        {
            Managed,
            Silver,
            Blue,
            Black
        }
        public enum AppearanceType
        {
            Normal = 0,
            Office2007
        }

        internal const int BORDER_WIDTH = 6;
        internal const int DPI_125_BORDER_WIDTH = 8;
        internal const int DPI_150_BORDER_WIDTH = 11;
        internal const int VERTEX_RADIUS = 8;

        const int WMSZ_LEFT = 1;
        const int WMSZ_RIGHT = 2;
        const int WMSZ_TOP = 3;
        const int WMSZ_TOPLEFT = 4;
        const int WMSZ_TOPRIGHT = 5;
        const int WMSZ_BOTTOM = 6;
        const int WMSZ_BOTTOMLEFT = 7;
        const int WMSZ_BOTTOMRIGHT = 8;

        /// <summary>
        /// This message is used to perform asynchronous invalidate of the form.
        /// To invalidate child controls the WParam parameter is TRUE.
        /// </summary>
        const int WMU_INVALIDATE = (int)Msg.WM_USER + 1;
        const int WMU_SYSCOMMAND = (int)Msg.WM_USER + 2;

        const SystemCommand SC_NONE = (SystemCommand)0;

        const int CS_NOCLOSE = 0x200;

        #endregion

        #region Constructors
        static RibbonForm()
        {
            m_blTitle = new Blend();
            m_blTitle.Positions = new float[] { 0.0F, 0.27F, 0.27F, 1.0F };
            m_blTitle.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.0F };

            m_blFrameButton = new Blend();
            m_blFrameButton.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
            m_blFrameButton.Factors = new float[] { 0.2F, 0.0F, 1.0F, 0.5F };

            m_blFrameButtonBorder = new Blend();
            m_blFrameButtonBorder.Positions = new float[] { 0.0f, 0.5f, 1.0f };
            m_blFrameButtonBorder.Factors = new float[] { 0.5f, 1.0f, 0.5f };
        }
        /// <summary>
        /// 
        /// </summary>
        public RibbonForm()
        {
            m_hShortcuts = new Hashtable();
            using (Graphics g = Graphics.FromImage(new Bitmap ( 10,10)))
            {
                if (g.DpiX > 120)
                    m_Borders = new Padding(DPI_150_BORDER_WIDTH, 1, DPI_150_BORDER_WIDTH, DPI_150_BORDER_WIDTH);
                else if (g.DpiX > 96)
                    m_Borders = new Padding(DPI_125_BORDER_WIDTH, 1, DPI_125_BORDER_WIDTH, DPI_125_BORDER_WIDTH);
                else
                    m_Borders = new Padding(BORDER_WIDTH, 1, BORDER_WIDTH, BORDER_WIDTH);
            }
            m_DwmFrame = new Padding(0, 0, 0, 0);

            m_bCompositionEnabled = DwmAPI.IsCompositionEnabled;

            this.DoubleBuffered = true;

            this.RibbonStyle = RibbonStyle.Office2007;

            UpdateColorScheme();
        }
        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_MenuStrip.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnAppearanceChanged()
        {
            this.Region = null;
            RecreateHandle();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this.MainMenuStrip == null)
            {
                this.MainMenuStrip = m_MenuStrip;
                this.MainMenuStrip.Visible = false;
            }
            if (this.Appearance == AppearanceType.Office2007)
            {
                int mask = base.CreateParams.Style & (int)WindowStyles.WS_CAPTION;
                if (mask != 0)
                {
                    //
                    // Call default WM_NCCALCSIZE handler to enable windows tile/cascade. Window should have WS_CAPTION style.
                    //
                    IntPtr hWnd = this.Handle;
                    RECT rc = (RECT)(this.Bounds);

                    int style = WindowsAPI.GetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_STYLE);

                    WindowsAPI.SetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_STYLE, (IntPtr)(style | mask));

                    WindowsAPI.DefWindowProc(hWnd, Msg.WM_NCCALCSIZE, IntPtr.Zero, ref rc);

                    WindowsAPI.SetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_STYLE, (IntPtr)(style));
                }

                UpdateRegion();
            }

            Application.AddMessageFilter(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.MainMenuStrip == m_MenuStrip)
            {
                this.MainMenuStrip = null;
            }
            Application.RemoveMessageFilter(this);

            base.OnHandleDestroyed(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            ToolStripItem item = GetShortcutTarget(keyData) as ToolStripItem;

            if (item != null)
            {
                ProcessShortcut(item);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            IDockExtended ide = e.Control as IDockExtended;
            if (ide != null)
            {
                switch (ide.Dock)
                {
                    case DockStyleEx.TopMost:
                        m_nTopHeight += e.Control.Height;
                        break;
                    case DockStyleEx.BottomMost:
                        m_nBottomHeight += e.Control.Height;
                        break;
                }
            }

            IRibbonStyleNotifier iRibbon = e.Control as IRibbonStyleNotifier;

            if (iRibbon != null)
            {
                this.ColorScheme = RibbonControlAdv.GetColorSchemeType(iRibbon.OfficeColorScheme);
                this.RibbonStyle = iRibbon.RibbonStyle;
                this.MenuColor = iRibbon.MenuColor;
                iRibbon.ColorSchemeChanged += new ColorSchemeChanged(OnRibbonControlColorSchemeChanged);
                iRibbon.RibbonStyleChanged += new RibbonStyleChanged(OnRibbonControlStyleChanged);
                iRibbon.MenuColorChanged += new MenuColorChanged(iRibbon_MenuColorChanged);
            }

        }

        void iRibbon_MenuColorChanged(object sender, Color menuColor)
        {
            MenuColor = menuColor;
        }

        private void OnRibbonControlStyleChanged(object sender, RibbonStyle style)
        {
            this.RibbonStyle = style;
        }

        private void OnRibbonControlColorSchemeChanged(Object sender,RibbonForm.ColorSchemeType colorScheme)
        {
            this.ColorScheme = colorScheme;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            IDockExtended ide = e.Control as IDockExtended;
            if (ide != null)
            {
                switch (ide.Dock)
                {
                    case DockStyleEx.TopMost:
                        m_nTopHeight -= e.Control.Height;
                        break;
                    case DockStyleEx.BottomMost:
                        m_nBottomHeight -= e.Control.Height;
                        break;
                }
            }

            IRibbonStyleNotifier iRibbon = e.Control as IRibbonStyleNotifier;
            if (iRibbon != null)
            {
                iRibbon.ColorSchemeChanged -= new ColorSchemeChanged(OnRibbonControlColorSchemeChanged);
                iRibbon.RibbonStyleChanged -= new RibbonStyleChanged(OnRibbonControlStyleChanged);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="levent"></param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (this.IsHandleCreated)
            {
                //
                // Extended layout is not supported when AutoSize==true;
                //
                if (!this.AutoSize)
                {
                    PerformExtendedLayout();
                }

                base.OnLayout(levent);

                if (this.CompositionEnabled)
                {
                    Padding margins = this.Borders;

                    Padding sysframe = this.SystemFrame;

                    margins.Left = Math.Max(0, margins.Left - sysframe.Left);
                    margins.Right = Math.Max(0, margins.Right - sysframe.Right);
                    margins.Top = Math.Max(0, margins.Top - sysframe.Top);
                    margins.Bottom = Math.Max(0, margins.Bottom - sysframe.Bottom);

                    if (this.GetMargins != null)
                    {
                        this.GetMargins(ref margins);
                    }

                    this.DwmFrame = margins;
                    if (this.RibbonStyle == Tools.RibbonStyle.Office2013)
                    {
                        margins.Left = Math.Max(0, 0);
                        margins.Right = Math.Max(0, 0);
                        margins.Top = Math.Max(2, 0);
                        margins.Bottom = Math.Max(0, 0);
                        this.DwmFrame = margins;
                    }
                }
                WindowsAPI.PostMessage(this.Handle, WMU_INVALIDATE, 0, 0);
            }
        }
        internal bool designmode = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode)
                designmode = true;
            else
                designmode = false;
            base.OnPaint(e);
            if (this.Appearance == AppearanceType.Office2007)
            {
                DrawBorders(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)Msg.WM_NCPAINT:
                    if (OnWmNcPaint(ref m))
                        return;
                    break;
                case (int)Msg.WM_NCCALCSIZE:
                    if (OnNcCalcSize(ref m))
                        return;
                    break;
                case (int)Msg.WM_NCACTIVATE:
                    if (OnNcActivate(ref m))
                        return;
                    break;
                case (int)Msg.WM_NCHITTEST:
                    if (OnNcHitTest(ref m))
                        return;
                    break;
                case (int)Msg.WM_NCMOUSEMOVE:
                    if (OnWmNcMouseMove(ref m))
                        return;
                    break;
                case (int)Msg.WM_NCMOUSELEAVE:
                    if (OnWmNcMouseLeave(ref m))
                        return;
                    break;
                case (int)Msg.WM_NCLBUTTONDOWN:
                    if (OnWmNcLButtonDown(ref m))
                        return;
                    break;
                case (int)Msg.WM_MOUSEMOVE:
                    if (OnWmMouseMove(ref m))
                        return;
                    break;
                case (int)Msg.WM_LBUTTONUP:
                    if (OnWmLButtonUp(ref m))
                        return;
                    break;
                case (int)Msg.WM_CAPTURECHANGED:
                    if (OnWmCaptureChanged(ref m))
                        return;
                    break;
                case (int)Msg.WM_GETMINMAXINFO:
                    if (OnGetMinMaxInfo(ref m))
                        return;
                    break;
                case (int)Msg.WM_WINDOWPOSCHANGING:
                    if (OnWmWindowPosChanging(ref m))
                        return;
                    break;
                case (int)Msg.WM_WINDOWPOSCHANGED:
                    if (OnWmWindowPosChanged(ref m))
                        return;
                    break;
                case (int)Msg.WM_SETCURSOR:
                    if (OnWmSetCursor(ref m))
                        return;
                    break;
                case (int)Msg.WM_SYSCOMMAND:
                    if (OnWmSysSommand(ref m))
                        return;
                    break;
                case (int)Msg.WM_SETICON:
                case (int)Msg.WM_SETTEXT:
                    if (!this.CompositionEnabled)
                    {
                        BaseWndProc(ref m);
                        return;
                    }
                    break;
                case DwmAPI.WM_DWMCOMPOSITIONCHANGED:
                    OnDwmCompositionChanged(ref m);
                    break;
                case WMU_INVALIDATE:
                    Invalidate(m.WParam != IntPtr.Zero);
                    return;
                case WMU_SYSCOMMAND:
                    WindowsAPI.SendMessage(this.Handle, (int)Msg.WM_SYSCOMMAND, m.WParam, m.LParam);
                    return;
            }
            base.WndProc(ref m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            if (this.Appearance == AppearanceType.Office2007)
            {
                UpdateRegion();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            this.ActiveState = true;
            base.OnActivated(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDeactivate(EventArgs e)
        {
            this.ActiveState = false;
            base.OnDeactivate(e);
        }
        /// <summary>
        /// In Office2007 mode form's Size and ClientSize are equal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void SetClientSizeCore(int x, int y)
        {
            if (this.Appearance == AppearanceType.Office2007)
            {
                this.Size = new Size(x, y);
                UpdateBounds(this.Left, this.Top, x, y, x, y);
            }
            else base.SetClientSizeCore(x, y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="specified"></param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!m_bSuppressSizing)
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }
        #endregion

        #region Message handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmNcPaint(ref Message m)
        {
            if (this.Appearance == AppearanceType.Office2007)
            {
                if (!this.CompositionEnabled)
                {
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        IntPtr hdc = WindowsAPI.GetWindowDC(this.Handle);
                        try
                        {
                            if (hdc != IntPtr.Zero)
                            {
                                IntPtr bufferDC = WindowsAPI.CreateCompatibleDC(hdc);
                                if (bufferDC != IntPtr.Zero)
                                {
                                    RECT rc = new RECT();
                                    WindowsAPI.GetWindowRect(this.Handle, ref rc);

                                    IntPtr hBmp = WindowsAPI.CreateCompatibleBitmap(hdc, rc.Width, rc.Height);
                                    if (hBmp != IntPtr.Zero)
                                    {
                                        IntPtr oldBmp = WindowsAPI.SelectObject(bufferDC, hBmp);

                                        using (Graphics bufferedGraphics = Graphics.FromHdc(bufferDC))
                                        {
                                            DrawFrame(bufferedGraphics, new Rectangle(0, 0, rc.Width, rc.Height));

                                            WindowsAPI.BitBlt(hdc, 0, 0, rc.Width, rc.Height, bufferDC, 0, 0, (uint)PatBltTypes.SRCCOPY);
                                        }
                                        WindowsAPI.DeleteObject(hBmp);
                                    }
                                    WindowsAPI.DeleteDC(bufferDC);
                                }                                
                            }
                        }
                        finally 
                        {
                            WindowsAPI.ReleaseDC(this.Handle, hdc);
                        }
                    }
                    m.Result = IntPtr.Zero;
                    return true;
                }
                else
                {
                    IntPtr hdc = WindowsAPI.GetWindowDC(this.Handle);
                    try
                    {
                        if (hdc != IntPtr.Zero)
                        {
                            RECT rcWnd = new RECT();
                            if (WindowsAPI.GetWindowRect(this.Handle, ref rcWnd))
                            {
                                IntPtr brush = WindowsAPI.CreateSolidBrush(0);
                                if (brush != IntPtr.Zero)
                                {
                                    int w = rcWnd.Width;
                                    int h = rcWnd.Height;

                                    Padding pd = this.Borders;
                                    WindowsAPI.ExcludeClipRect(hdc, pd.Left, 0, w - pd.Right, h - pd.Bottom);

                                    RECT rcFill = new RECT(0, 0, w, h);

                                    WindowsAPI.FillRect(hdc, ref rcFill, brush);

                                    WindowsAPI.DeleteObject(brush);
                                }
                            }                            
                        }
                    }
                    finally 
                    {
                        WindowsAPI.ReleaseDC(this.Handle, hdc);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnNcHitTest(ref Message m)
        {
            bool bResult = false;

            if (this.Appearance != AppearanceType.Normal)
            {
                HitTest hitTest = HitTest.HTNOWHERE;

                if (this.CompositionEnabled)
                {
                    IntPtr dwmResult = IntPtr.Zero;
                    DwmAPI.DwmDefWindowProc(this.Handle, m.Msg, m.WParam, m.LParam, ref dwmResult);

                    switch ((HitTest)dwmResult)
                    {
                        case HitTest.HTMINBUTTON:
                        case HitTest.HTMAXBUTTON:
                        case HitTest.HTCLOSE:
                        case HitTest.HTHELP:
                            hitTest = (HitTest)dwmResult;
                            break;
                    }
                }

                if (hitTest == HitTest.HTNOWHERE)
                {
                    Point pt = WindowsAPI.GetPointFromLPARAM((int)m.LParam);

                    hitTest = GetHitTest(pt);
                }


                if (hitTest != HitTest.HTNOWHERE)
                {
                    m.Result = (IntPtr)hitTest;
                    bResult = true;
                }
            }
            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnNcCalcSize(ref Message m)
        {
            bool bResult = false;
            if (!IsMinimized())
            {
                if (this.Appearance == AppearanceType.Office2007)
                {
                    if (this.CompositionEnabled)
                    {
                        Padding borders = this.Borders;

                        RECT rc = (RECT)m.GetLParam(typeof(RECT));

                        rc.left += borders.Left;
                        rc.right -= borders.Right;
                        rc.bottom -= borders.Bottom;

                        Marshal.StructureToPtr(rc, m.LParam, false);
                    }
                    m.Result = IntPtr.Zero;
                    if (this.IsMdiChild && this.WindowState == FormWindowState.Minimized)
                        return bResult;
                    else
                        bResult = true;
                }
            }
            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnNcActivate(ref Message m)
        {
            bool bResult = false;

            if (this.Appearance == AppearanceType.Office2007)
            {
                if (m.WParam == IntPtr.Zero)
                {
                    m_highlightedButton = SC_NONE;
                }

                if (IsMinimized())
                {
                    InvalidateFrame();
                }

                m.Result = (IntPtr)1;
                if (this.MdiParent != null && this.TopMost == false)
                {
                    return bResult;
                }
                else
                {
                    bResult = !this.CompositionEnabled;
                }
            }
            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmNcMouseMove(ref Message m)
        {
            if (this.Appearance == AppearanceType.Office2007 && IsMinimized())
            {
                this.SelectedButton = GetButtonId(m.LParam);

                if (!m_bMouseIsTracked)
                {
                    TRACKMOUSEEVENTS tme = new TRACKMOUSEEVENTS();

                    tme.cbSize = (uint)Marshal.SizeOf(tme);

                    tme.hWnd = this.Handle;
                    tme.dwFlags = (uint)(TrackerEventFlags.TME_LEAVE | TrackerEventFlags.TME_NONCLIENT);

                    WindowsAPI.TrackMouseEvent(ref tme);

                    m_bMouseIsTracked = true;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmNcMouseLeave(ref Message m)
        {
            this.SelectedButton = SC_NONE;

            m_bMouseIsTracked = false;

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmNcLButtonDown(ref Message m)
        {
            if (this.Appearance == AppearanceType.Office2007)
            {
                if (!IsMinimized())
                {
                    switch ((HitTest)m.WParam)
                    {
                        case HitTest.HTCAPTION:
                            if (this.WindowState != FormWindowState.Maximized)
                            {
                                MoveForm(m.LParam);
                            }
                            break;
                        case HitTest.HTLEFT:
                            ResizeForm(WMSZ_LEFT, m.LParam);
                            break;
                        case HitTest.HTTOP:
                            ResizeForm(WMSZ_TOP, m.LParam);
                            break;
                        case HitTest.HTRIGHT:
                            ResizeForm(WMSZ_RIGHT, m.LParam);
                            break;
                        case HitTest.HTBOTTOM:
                            ResizeForm(WMSZ_BOTTOM, m.LParam);
                            break;
                        case HitTest.HTTOPLEFT:
                            ResizeForm(WMSZ_TOPLEFT, m.LParam);
                            break;
                        case HitTest.HTTOPRIGHT:
                            ResizeForm(WMSZ_TOPRIGHT, m.LParam);
                            break;
                        case HitTest.HTBOTTOMLEFT:
                            ResizeForm(WMSZ_BOTTOMLEFT, m.LParam);
                            break;
                        case HitTest.HTBOTTOMRIGHT:
                            ResizeForm(WMSZ_BOTTOMRIGHT, m.LParam);
                            break;
                        case HitTest.HTMINBUTTON:
                        case HitTest.HTMAXBUTTON:
                        case HitTest.HTCLOSE:
                        case HitTest.HTHELP:
                            base.WndProc(ref m);
                            break;
                    }
                }
                else
                {
                    SystemCommand buttonId = GetButtonId(m.LParam);
                    if (buttonId != SC_NONE)
                    {
                        if (IsButtonEnabled(buttonId))
                        {
                            this.PressedButton = buttonId;
                            this.Capture = true;
                        }
                    }
                    else
                    {
                        WindowsAPI.SendMessage(m.HWnd, (int)Msg.WM_SYSCOMMAND, (int)SystemCommand.SC_MOVE | 2, m.LParam);
                    }
                }

                m.Result = IntPtr.Zero;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmMouseMove(ref Message m)
        {
            if (this.Appearance == AppearanceType.Office2007 && IsMinimized())
            {
                if (this.Capture)
                {
                    Point pt = new Point(WindowsAPI.LOW_ORDER(m.LParam), WindowsAPI.HIGH_ORDER(m.LParam));
                    this.SelectedButton = GetButtonId(this.PointToScreen(pt));
                }
            }

            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmLButtonUp(ref Message m)
        {
            if (this.Appearance == AppearanceType.Office2007 && IsMinimized())
            {
                if (this.PressedButton != SC_NONE)
                {
                    Point pt = new Point(WindowsAPI.LOW_ORDER(m.LParam), WindowsAPI.HIGH_ORDER(m.LParam));
                    SystemCommand button = GetButtonId(this.PointToScreen(pt));

                    if (button == this.PressedButton)
                    {
                        WindowsAPI.PostMessage(this.Handle, WMU_SYSCOMMAND, (int)button, 0);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmCaptureChanged(ref Message m)
        {
            this.PressedButton = SC_NONE;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnGetMinMaxInfo(ref Message m)
        {
            base.WndProc(ref m);

            if (this.TopLevel)
            {
                Rectangle rc;

                Screen scr = Screen.FromHandle(m.HWnd);
                if (scr != null && scr.Primary)
                {
                    if (this.FormBorderStyle == FormBorderStyle.None)
                    {
                        rc = scr.Bounds;
                    }
                    else
                    {
                        rc = scr.WorkingArea;
                    }
                    AdjustWorkingArea(ref rc);
                }
                else
                {
                    rc = Screen.PrimaryScreen.Bounds;
                }

                MINMAXINFO mmInfo = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO));

                mmInfo.ptMaxPosition.x = 0;
                mmInfo.ptMaxPosition.y = 0;

                mmInfo.ptMaxSize.x = rc.Width;
                mmInfo.ptMaxSize.y = rc.Height;

                Marshal.StructureToPtr(mmInfo, m.LParam, false);
                m.Result = IntPtr.Zero;
            }
            else if (this.IsMdiChild)
            {
                MINMAXINFO mmInfo = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO));

                Rectangle rc = GetMdiMaxBounds();

                mmInfo.ptMaxPosition.x = rc.Left;
                mmInfo.ptMaxPosition.y = rc.Top;

                mmInfo.ptMaxSize.x = rc.Width;
                mmInfo.ptMaxSize.y = rc.Height;

                Marshal.StructureToPtr(mmInfo, m.LParam, false);
                m.Result = IntPtr.Zero;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmWindowPosChanging(ref Message m)
        {
            bool bResult = false;

            if (this.Appearance == AppearanceType.Office2007)
            {
                if (this.IsMdiChild && this.IsMaximized())
                {
                    SetWindowPosFlags mask = SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE;

                    WINDOWPOS wpos = (WINDOWPOS)m.GetLParam(typeof(WINDOWPOS));

                    if ((wpos.flags & mask) != mask)
                    {
                        base.WndProc(ref m);

                        Rectangle rc = GetMdiMaxBounds();

                        wpos.x = rc.Left;
                        wpos.y = rc.Top;

                        wpos.cx = rc.Width;
                        wpos.cy = rc.Height;

                        Marshal.StructureToPtr(wpos, m.LParam, false);
                        m.Result = IntPtr.Zero;

                        bResult = true;
                    }
                }
            }
            return bResult;
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                foreach (Control ctr in this.Controls)
                {
                    if (ctr is RibbonControlAdv)
                    {
                        if ((ctr as RibbonControlAdv).HeaderInternal.AutoHide && this.RibbonStyle == Tools.RibbonStyle.Office2013)
                        {
                            (ctr as RibbonControlAdv).RibbonStatus = false;
                            foreach (ToolStripTabItem tab in (ctr as RibbonControlAdv).HeaderInternal.VisibleTabItem)
                            {
                                tab.Visible = false;
                            }
                                (ctr as RibbonControlAdv).QuickPanelVisible = false;
                                (ctr as RibbonControlAdv).MinimizePanel = false;
                                this.ShowIcon = false;
                                this.MinimizeBox = false;
                                this.MaximizeBox = false;
                        }
                        break;
                    }
                }
            }
            base.OnSizeChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmWindowPosChanged(ref Message m)
        {
            bool bResult = false;

            if (this.Appearance == AppearanceType.Office2007)
            {
                WINDOWPOS wpos = (WINDOWPOS)m.GetLParam(typeof(WINDOWPOS));
                if ((wpos.flags & SetWindowPosFlags.SWP_NOSIZE) != SetWindowPosFlags.SWP_NOSIZE)
                {
                    m_bSuppressSizing = true;

                    base.WndProc(ref m);
                    Invalidate();

                    m_bSuppressSizing = false;

                    UpdateRegion();

                    m.Result = IntPtr.Zero;
                    bResult = true;
                }
            }
            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmSetCursor(ref Message m)
        {
            if (this.Appearance == AppearanceType.Office2007)
            {
                IntPtr hCursor = Cursors.Default.Handle;

                HitTest ht = (HitTest)WindowsAPI.LOW_ORDER((int)m.LParam);
                if (ht != HitTest.HTCLIENT)
                {
                    if (this.IsSizeable())
                    {
                        switch (ht)
                        {
                            case HitTest.HTTOP:
                            case HitTest.HTBOTTOM:
                                hCursor = Cursors.SizeNS.Handle;
                                break;
                            case HitTest.HTLEFT:
                            case HitTest.HTRIGHT:
                                hCursor = Cursors.SizeWE.Handle;
                                break;
                            case HitTest.HTTOPLEFT:
                            case HitTest.HTBOTTOMRIGHT:
                                hCursor = Cursors.SizeNWSE.Handle;
                                break;
                            case HitTest.HTBOTTOMLEFT:
                            case HitTest.HTTOPRIGHT:
                                hCursor = Cursors.SizeNESW.Handle;
                                break;
                            default:
                                return false;
                        }
                    }
                }
                else hCursor = this.Cursor.Handle;

                WindowsAPI.SetCursor(hCursor);
                m.Result = (IntPtr)1;

                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool OnWmSysSommand(ref Message m)
        {
            if (this.Appearance == AppearanceType.Office2007)
            {
                switch ((int)m.WParam & 0xfff0)
                {
                    case (int)SystemCommand.SC_KEYMENU:
                        if ((int)m.LParam == ' ' && this.ShowSystemMenu != null)
                        {
                            this.ShowSystemMenu(this, EventArgs.Empty);
                        }
                        m.Result = IntPtr.Zero;
                        return true;
                    case (int)SystemCommand.SC_MOUSEMENU:
                        m.Result = IntPtr.Zero;
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        private void OnDwmCompositionChanged(ref Message m)
        {
            m_bCompositionEnabled = DwmAPI.IsCompositionEnabled;

            this.Region = null;

            RecreateHandle();
        }

        #endregion

        #region IExtenderProvider Members
        public bool CanExtend(object extendee)
        {
            return extendee is ToolStripItem;
        }
        #endregion

        #region IMessageFilter Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            bool bResult = false;
            switch ((Msg)m.Msg)
            {
                case Msg.WM_NCLBUTTONDOWN:
                    if (this.Appearance == AppearanceType.Office2007 && WindowsAPI.GetParent(m.HWnd) == this.Handle)
                    {
                        if ((HitTest)m.WParam == HitTest.HTBOTTOMRIGHT)
                        {
                            if (this.Focused || Control.FromHandle(m.HWnd).FindForm() == this)
                            {
                                WindowsAPI.SendMessage(this.Handle, (int)Msg.WM_SYSCOMMAND, (int)SystemCommand.SC_SIZE | 8 /*WMSZ_BOTTOMRIGHT*/, m.LParam);

                                m.Result = IntPtr.Zero;
                                bResult = true;
                            }
                        }
                    }
                    break;
            }

            return bResult;
        }

        #endregion

        #region Implementation
        private void InvalidateBorder()
        {
            if (this.Appearance == AppearanceType.Office2007)
            {
                Rectangle rc = this.ClientRectangle;

                rc.Y += m_nTopHeight;
                rc.Height -= m_nTopHeight + m_nBottomHeight;

                using (Region rg = new Region(rc))
                {
                    Padding borders = this.BordersInternal;

                    rc.X += borders.Left;
                    rc.Y += borders.Top;
                    rc.Width -= borders.Horizontal;
                    rc.Height -= borders.Vertical;

                    rg.Exclude(rc);

                    this.Invalidate(rg, false);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateFrame()
        {
            if (this.IsHandleCreated && this.CompositionEnabled)
            {
                DwmAPI.MARGINS dwm_margins = new DwmAPI.MARGINS();

                dwm_margins.cxLeftWidth = m_DwmFrame.Left;
                dwm_margins.cxRightWidth = m_DwmFrame.Right;
                dwm_margins.cyTopHeight = m_DwmFrame.Top;
                dwm_margins.cyBottomHeight = m_DwmFrame.Bottom;
                using (Graphics g = this.CreateGraphics())
                {
                    if (g.DpiX > 96 && this.CompositionEnabled && this.RibbonStyle == Tools.RibbonStyle.Office2010)
                    {
                        dwm_margins.cyTopHeight = m_DwmFrame.Top + RibbonForm.BORDER_WIDTH;
                    }
                }
                DwmAPI.DwmExtendFrameIntoClientArea(this.Handle, ref dwm_margins);
            }
        }
        private bool PaddingUpdated = false;
        Padding AdjustPadding = new Padding(0);

        /// <summary>
        /// 
        /// </summary>
        private void UpdateRegion()
        {

            if (this.IsHandleCreated && !this.CompositionEnabled)
            {
                Region region = null;

                RECT rc = new RECT();
                IntPtr hWnd = this.Handle;

                if (WindowsAPI.GetWindowRect(hWnd, ref rc))
                {
                    if (!this.IsMaximized())
                    {
                        region = GetFormRegion(new Rectangle(Point.Empty, rc.Size));
                    }
                    else
                    {
                        Screen scr = Screen.FromHandle(hWnd);
                        if (scr != null)
                        {
                            Rectangle rcWnd = new Rectangle(rc.left, rc.top, rc.Width, rc.Height);
                            Rectangle rcRgn = scr.WorkingArea;

                            rcRgn.Intersect(rcWnd);
                            rcRgn.Offset(-rcWnd.X, -rcWnd.Y);

                            region = new Region(rcRgn);
                        }
                    }
                }

                this.Region = region;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateColorScheme()
        {
            switch (m_ColorScheme)
            {
                case ColorSchemeType.Managed:
                    m_ColorTable = Office12ColorTable.ManagedColors;
                    colorTable = new Office2010ColorTable(Office2010ColorScheme.Blue);
                    break;
                case ColorSchemeType.Silver:
                    m_ColorTable = new Office12ColorTable();
                    colorTable = new Office2010ColorTable(Office2010ColorScheme.Silver);
                    break;
                case ColorSchemeType.Blue:
                    m_ColorTable = new OfficeBlue();
                    colorTable = new Office2010ColorTable(Office2010ColorScheme.Blue);
                    break;
                case ColorSchemeType.Black:
                    m_ColorTable = new OfficeBlack();
                    colorTable = new Office2010ColorTable(Office2010ColorScheme.Black);
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        private GraphicsPath GetFormPath(Rectangle rc)
        {
            GraphicsPath path = null;

            if (this.IsHandleCreated)
            {
                path = new GraphicsPath();

                int nTopLeftRadius = this.WindowState == FormWindowState.Minimized ? VERTEX_RADIUS : this.TopLeftRadius;

                if (RightToLeft == RightToLeft.No)
                {
                    int VERTEX_DIAMETER = 2 * VERTEX_RADIUS;
                    int x = rc.Left;
                    int y = rc.Top;

                    path.AddArc(x - 1, y - 1, 2 * nTopLeftRadius, 2 * nTopLeftRadius, 180f, 90f);
                    path.AddLine(x + nTopLeftRadius, y, rc.Right - VERTEX_RADIUS, 0);
                    path.AddArc(rc.Right - VERTEX_DIAMETER, y - 1, VERTEX_DIAMETER, VERTEX_DIAMETER, -90f, 90f);
                    path.AddLine(rc.Right, VERTEX_RADIUS, rc.Right, rc.Bottom - VERTEX_RADIUS);
                    path.AddArc(rc.Right - VERTEX_DIAMETER, rc.Bottom - VERTEX_DIAMETER, VERTEX_DIAMETER, VERTEX_DIAMETER, 0f, 90f);
                    path.AddLine(rc.Right - VERTEX_RADIUS, rc.Bottom, VERTEX_RADIUS, rc.Bottom);
                    path.AddArc(x - 1, rc.Bottom - VERTEX_DIAMETER, VERTEX_DIAMETER, VERTEX_DIAMETER, 90f, 90f);
                    path.AddLine(x, rc.Bottom - VERTEX_DIAMETER, x, y + nTopLeftRadius);

                    path.CloseFigure();
                }
                else
                {
                    int VERTEX_DIAMETER = 2 * VERTEX_RADIUS;
                    int x = rc.Right;
                    int y = rc.Top;

                    path.AddArc(x - 2 * nTopLeftRadius, y - 1, 2 * nTopLeftRadius, 2 * nTopLeftRadius, 0f, -90f);
                    path.AddLine(x - nTopLeftRadius, y, rc.Left + VERTEX_RADIUS, 0);
                    path.AddArc(rc.Left, y - 1, VERTEX_DIAMETER, VERTEX_DIAMETER, 270f, -90f);
                    path.AddLine(rc.X, VERTEX_RADIUS, rc.X, rc.Bottom - VERTEX_RADIUS);
                    path.AddArc(rc.X - 1, rc.Bottom - VERTEX_DIAMETER, VERTEX_DIAMETER, VERTEX_DIAMETER, 180f, -90f);
                    path.AddLine(rc.Left + VERTEX_RADIUS, rc.Bottom, rc.Right - VERTEX_RADIUS, rc.Bottom);
                    path.AddArc(rc.Right - VERTEX_DIAMETER, rc.Bottom - VERTEX_DIAMETER, VERTEX_DIAMETER, VERTEX_DIAMETER, 90f, -90f);
                    path.AddLine(x, rc.Bottom - VERTEX_DIAMETER, x, y + nTopLeftRadius);

                    path.CloseFigure();
                }
            }

            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Region GetFormRegion(Rectangle rc)
        {
            Region region = null;

            if (this.IsHandleCreated)
            {
                GraphicsPath path = GetFormPath(rc);

                if (path != null)
                {
                    region = new Region(path);
                    path.Dispose();
                }
            }

            return region;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool IsMaximized()
        {
            WindowStyles style = (WindowStyles)WindowsAPI.GetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_STYLE);
            return (style & WindowStyles.WS_MAXIMIZE) != 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsMinimized()
        {
            WindowStyles style = (WindowStyles)WindowsAPI.GetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_STYLE);
            return (style & WindowStyles.WS_MINIMIZE) != 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsSizeable()
        {
            bool bResult = false;
            if (this.WindowState == FormWindowState.Normal)
            {
                WindowStyles ws = (WindowStyles)WindowsAPI.GetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_STYLE);

                bResult = ((ws & WindowStyles.WS_SIZEBOX) == WindowStyles.WS_SIZEBOX);
            }
            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void ProcessShortcut(ToolStripItem item)
        {
            if (item is IShortcutSupport)
            {
                ((IShortcutSupport)item).ProcessShortcut();
            }
            else if (item is ToolStripControlHost)
            {
                ((ToolStripControlHost)item).Control.Select();
            }
            else
            {
                item.PerformClick();
            }
        }
        /// <summary>
        /// Updates the renderers.
        /// </summary>
        private void UpdateRenderers()
        {
            foreach (Control c in this.Controls)
            {
                // Update renderers for RibbonControlAdv.
                RibbonControlAdv control = c as RibbonControlAdv;

                if (control != null)
                {
                    control.UpdateRenderers(false);
                }

                // Update renderer for StatusStrip.
                StatusStripEx statusControl = c as StatusStripEx;

                if (statusControl != null)
                {
                    statusControl.UpdateRenderer();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal object GetShortcutTarget(object key)
        {
            if (m_hShortcuts.Contains(key))
            {
                return m_hShortcuts[key];
            }
            return null;
        }
        RibbonControlAdv RibbonControl;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        internal HitTest GetHitTest(Point pt)
        {
            HitTest ht = HitTest.HTNOWHERE;

            if (this.IsSizeable())
            {
                RECT rc = new RECT();
                WindowsAPI.GetWindowRect(this.Handle, ref rc);

                Rectangle bounds = (Rectangle)rc;
                if (bounds.Contains(pt))
                {
                    int iFromBottom = bounds.Bottom - pt.Y;
                    int iFromRight = bounds.Right - pt.X;
                    int iFromLeft = pt.X - bounds.Left;
                    int iFromTop = pt.Y - bounds.Top;

                    if (iFromTop <= TopLeftRadius && iFromLeft <= TopLeftRadius && RightToLeft == RightToLeft.No)
                    {
                        int distance1 = TopLeftRadius - iFromLeft;
                        int disctance2 = TopLeftRadius - iFromTop;

                        int distance = (int)Math.Sqrt(distance1 * distance1 + disctance2 * disctance2);

                        if (TopLeftRadius - distance < 2 * BORDER_WIDTH)
                        {
                            ht = HitTest.HTTOPLEFT;    // Move Top-Left corner
                        }
                    }
                    else if (iFromTop <= TopLeftRadius && iFromRight <= TopLeftRadius && RightToLeft == RightToLeft.Yes)
                    {
                        int distance1 = TopLeftRadius - iFromRight;
                        int disctance2 = TopLeftRadius - iFromTop;

                        int distance = (int)Math.Sqrt(distance1 * distance1 + disctance2 * disctance2);

                        if (TopLeftRadius - distance < 2 * BORDER_WIDTH)
                        {
                            ht = HitTest.HTTOPRIGHT;    // Move Top-Right corner
                        }
                    }
                    else if (iFromTop <= VERTEX_RADIUS)
                    {
                        if (iFromLeft <= VERTEX_RADIUS && RightToLeft == RightToLeft.Yes)
                        {
                            ht = HitTest.HTTOPLEFT;   // Move Top-Left corner
                        }
                        else if (iFromRight <= VERTEX_RADIUS && RightToLeft == RightToLeft.No)
                        {
                            ht = HitTest.HTTOPRIGHT;   // Move Top-Right corner
                        }
                        else if (iFromTop <= 0)
                        {
                            ht = HitTest.HTTOP;        // Move Top border
                        }
                    }
                    else if (iFromBottom <= VERTEX_RADIUS)
                    {
                        if (iFromLeft <= VERTEX_RADIUS)
                        {
                            ht = HitTest.HTBOTTOMLEFT;  // Move Bottom-Left corner 
                        }
                        else if (iFromRight <= VERTEX_RADIUS)
                        {
                            ht = HitTest.HTBOTTOMRIGHT; // Move Bottom-Right corner
                        }
                        else if (iFromBottom <= 2 * BORDER_WIDTH)
                        {
                            ht = HitTest.HTBOTTOM;      // Move Bottom border
                        }
                    }
                    else
                    {
                        if (iFromLeft <= 2 * BORDER_WIDTH)
                        {
                            ht = HitTest.HTLEFT;        // Move Left border
                        }
                        else if (iFromRight <= 2 * BORDER_WIDTH)
                        {
                            ht = HitTest.HTRIGHT;       // Move Right border
                        }
                    }
                }
            }
            return ht;
        }
        /// <summary>
        /// 
        /// </summary>
        internal void PerformExtendedLayout()
        {
            bool bCompositionEnabled = this.CompositionEnabled;

            Size szClient = this.ClientSize;
            Padding borders = this.BordersInternal;

            int topHeight = 0;
            int bottomHeight = 0;
            int bottomOffset = szClient.Height - borders.Bottom;

            int topLeft = bCompositionEnabled ? -(m_Borders.Left - 3) : 1;
            int topWidth = szClient.Width - 2 * topLeft;

            int bottomLeft = borders.Left;
            int bottomWidth = szClient.Width - borders.Horizontal;

            ArrayList list = new ArrayList();

            foreach (Control c in this.Controls)
            {
                IDockExtended iDockExtendedControl = c as IDockExtended;

                if (iDockExtendedControl != null)
                {
                    switch (iDockExtendedControl.Dock)
                    {
                        case DockStyleEx.TopMost:
                            {
                                int height = (c.AutoSize) ? c.GetPreferredSize(Size.Empty).Height : c.Height;
                                switch (this.WindowState)
                                {
                                    case FormWindowState.Normal:
                                        if (this.RibbonStyle == Tools.RibbonStyle.Office2013)
                                            if ((c as RibbonControlAdv) != null && (c as RibbonControlAdv).BackStageView != null && (c as RibbonControlAdv).BackStageView.IsVisible && (c as RibbonControlAdv).RibbonStyle == Tools.RibbonStyle.Office2013) 
                                            {
                                                if ((c as RibbonControlAdv).HeaderInternal.MainItems.Count == 0)
                                                    c.SetBounds(1, topHeight - 2, topWidth + 4, height + 10);
                                                else
                                                    c.SetBounds(1, topHeight-2, topWidth +4, height);
                                            }
                                            else 
                                                c.SetBounds(1, topHeight+1 , topWidth + 4, height);
                                        else
                                            c.SetBounds(topLeft, topHeight, topWidth, height);
                                        break;
                                    case FormWindowState.Maximized:
                                        if (this.RibbonStyle == Tools.RibbonStyle.Office2013)
                                        {
                                            if ((c as RibbonControlAdv).HeaderInternal.AutoHide)
                                            {
                                                if ((c as RibbonControlAdv).BackStageView != null && ((c as RibbonControlAdv).BackStageView.IsVisible))
                                                {
                                                    c.SetBounds(7, topHeight, topWidth, 55);
                                                }
                                                else
                                                {
                                                    if ((c as RibbonControlAdv).RibbonStatus)
                                                        c.SetBounds(7, topHeight + 2, topWidth, height);
                                                    else
                                                    {
                                                        if ((c as RibbonControlAdv).RibbonTouchModeEnabled)
                                                            c.SetBounds(7, topHeight, topWidth, 37);
                                                        else
                                                            c.SetBounds(7, topHeight, topWidth, 25);
                                                    }
                                                }
                                            }
                                            else
                                                if (this.CompositionEnabled)
                                                {
                                                    c.SetBounds(RibbonForm.BORDER_WIDTH + 1, topHeight + RibbonForm.BORDER_WIDTH, topWidth - 9, height);
                                                }
                                                else
                                                {
                                                    if (IsThemeActive())
                                                        c.SetBounds(9, topHeight + RibbonForm.BORDER_WIDTH, topWidth - 9, height);
                                                    else
                                                        c.SetBounds(topLeft, topHeight, topWidth, height);
                                                }
                                        }
                                        else
                                            c.SetBounds(topLeft, topHeight, topWidth, height);
                                        break;
                                }
                                topHeight += height;

                                list.Add(c);
                            }
                            break;
                        case DockStyleEx.BottomMost:
                            {
                                int height = (c.AutoSize) ? c.GetPreferredSize(Size.Empty).Height : c.Height;

                                bottomOffset -= height;
                                bottomHeight += height;
                                int Ribbon2013StyleBorderWidth = 1;
                                int Ribbon2013StyleBorderHeight = 1;
                                int DPIAdjust = 0;
                                using (Graphics g = this.CreateGraphics())
                                {
                                    if (g.DpiX > 96 && !this.CompositionEnabled)
                                        DPIAdjust = 3;
                                    else if(g.DpiX > 96)
                                        Ribbon2013StyleBorderWidth = 0;
                                }
                                if (this.RibbonStyle == Tools.RibbonStyle.Office2013)
                                {
                                    switch (this.WindowState)
                                    {
                                        case FormWindowState.Maximized:
                                            if (!this.CompositionEnabled)
                                                c.SetBounds(bottomLeft + (RibbonForm.BORDER_WIDTH) / 2 - Ribbon2013StyleBorderWidth, bottomOffset - RibbonForm.BORDER_WIDTH, bottomWidth - (RibbonForm.BORDER_WIDTH) / 2, height);
                                            else
                                                c.SetBounds(bottomLeft + (RibbonForm.BORDER_WIDTH) / 2 - Ribbon2013StyleBorderWidth, bottomOffset - DPIAdjust, bottomWidth - (RibbonForm.BORDER_WIDTH) / 2, height);
                                            break;
                                        case FormWindowState.Normal:
                                            c.SetBounds(bottomLeft + Ribbon2013StyleBorderWidth, bottomOffset - Ribbon2013StyleBorderHeight, bottomWidth + 5, height);
                                            break;
                                    }
                                }
                                else
                                {
                                    c.SetBounds(bottomLeft, bottomOffset, bottomWidth, height);
                                }
                                list.Add(c);
                            }
                            break;
                    }
                }
            }

                    m_nBottomHeight = bottomHeight;
            // Set values to TopHeight and BottomHeight.
                    if (this.RibbonStyle == Tools.RibbonStyle.Office2013 && this.WindowState == FormWindowState.Maximized)
                    {
                        if (IsThemeActive())
                            m_nTopHeight = topHeight + 5;
                        else
                            m_nTopHeight = topHeight;
                        if (!this.CompositionEnabled)
                            m_nBottomHeight = bottomHeight + RibbonForm.BORDER_WIDTH;
                    }
                    else
                        m_nTopHeight = topHeight;

            // Apply DisplayRectangle's changes.
            if (list.Count > 0)
            {
                LayoutEngine le = this.LayoutEngine;
                if (le != null)
                {
                    foreach (Control c in list)
                    {
                        le.InitLayout(c, BoundsSpecified.All);
                    }
                }
                list.Clear();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void UpdateStartupBounds(ref CreateParams cp)
        {
            int width = this.Width;
            int height = this.Height;

            // Apply constraints
            Size szMax = this.MaximumSize;
            Size szMin = this.MinimumSize;

            if (szMax.Width > 0 && width > szMax.Width)
                width = szMax.Width;
            if (szMax.Height > 0 && height > szMax.Height)
                height = szMax.Height;
            if (width < szMin.Width)
                width = szMin.Width;
            if (height < szMin.Height)
                height = szMin.Height;

            if (cp.Width != width || cp.Height != height)
            {
                if (this.StartPosition == FormStartPosition.CenterScreen)
                {
                    cp.X += (cp.Width - width) / 2;
                    cp.Y += (cp.Height - height) / 2;
                }
                cp.Width = width;
                cp.Height = height;
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="rc"></param>
        private void AdjustWorkingArea(ref Rectangle rc)
        {
            APPBARDATA barData = new APPBARDATA();
            barData.cbSize = (uint)Marshal.SizeOf(barData);

            ABState state = (ABState)WindowsAPI.SHAppBarMessage((int)Msg.ABM_GETSTATE, ref barData);

            if ((state & ABState.ABS_AUTOHIDE) == ABState.ABS_AUTOHIDE)
            {
                // Workaround for auto hide problem
                rc.Height -= 1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lParam"></param>
        private void MoveForm(IntPtr lParam)
        {
            WindowsAPI.SendMessage(this.Handle, Msg.WM_SYSCOMMAND, (int)SystemCommand.SC_MOVE | 2, (int)lParam);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        private void ResizeForm(int direction, IntPtr lParam)
        {
            if (this.IsSizeable())
            {
                WindowsAPI.SendMessage(this.Handle, Msg.WM_SYSCOMMAND, (int)SystemCommand.SC_SIZE | direction, (int)lParam);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void DrawBorders(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            RECT rcWnd = new RECT();
            WindowsAPI.GetClientRect(this.Handle, ref rcWnd);

            Padding borders = this.BordersInternal;
            if (this.RibbonStyle == RibbonStyle.Office2013)
            {
                borders = new Padding(2, 1, 2, 2);
            }
            Rectangle rcForm = new Rectangle(0, 0, rcWnd.Width, rcWnd.Height);

            Rectangle rcInner = new Rectangle
            (
                rcForm.X + borders.Left,
                rcForm.Y + borders.Top + m_nTopHeight,
                rcForm.Width - borders.Horizontal,
                rcForm.Height - borders.Vertical - m_nTopHeight
            );

            Color clBorder = Color.Empty;

            if(this.RibbonStyle == RibbonStyle.Office2007)
                clBorder = this.ActiveState ? m_ColorTable.RibbonBorder : m_ColorTable.RibbonBorderInactive;
            else
                clBorder = this.ActiveState ? colorTable.ActiveHeaderBackground : colorTable.InActiveHeaderBackground;

            using (Region rg = new Region(rcForm))
            {
                GraphicsState state = g.Save();

                rg.Exclude(Rectangle.Inflate(rcInner, 1, 1));

                //if (!this.IsMaximized() && (!(this.RibbonStyle == Tools.RibbonStyle.Office2013 && this.CompositionEnabled)))
                {
                    using (GraphicsPath path = GetFormPath(Rectangle.Inflate(rcForm, -1, -1)))
                    {
                        if (this.RibbonStyle == Tools.RibbonStyle.Office2013 && this.CompositionEnabled)
                        {
                            using (Pen pen = new Pen(MenuColor))
                            {
                                g.DrawRectangle(pen, new Rectangle(rcForm.X, rcForm.Y, rcForm.Width - 1, rcForm.Height - 1));
                            }
                        }
                        else if (this.RibbonStyle == Tools.RibbonStyle.Office2013 && !this.CompositionEnabled)
                        {
                            using (Brush brush = new SolidBrush(MenuColor))
                            {
                                g.SetClip(path, CombineMode.Exclude);
                                g.FillRegion(brush, rg);
                                g.SetClip(path, CombineMode.Replace);
                            }
                        }
                        else
                        {
                            using (Brush brush = new SolidBrush(m_ColorTable.ToolStripBorder))
                            {
                                g.SetClip(path, CombineMode.Exclude);
                                g.FillRegion(brush, rg);
                                g.SetClip(path, CombineMode.Replace);
                            }
                        }
                    }
                }
                if (!(this.RibbonStyle == Tools.RibbonStyle.Office2013 && this.CompositionEnabled ) )
                {
                    using (Brush brush = new SolidBrush(clBorder))
                    {
                        g.FillRegion(brush, rg);
                    }
                }

                g.Restore(state);
            }

            Point[] ptLight = new Point[]
		    {
			    new Point(rcInner.Right, rcInner.Top-1),
			    new Point(rcInner.Right, rcInner.Bottom),
			    new Point(rcInner.Left-1, rcInner.Bottom),
		    };

            Color clBorderLight = Office12ColorTable.GetAlphaBlendedColor(Color.White, clBorder, 160);
            using (Pen pBorderLight = new Pen(clBorderLight))
            {
                g.DrawLine(pBorderLight, ptLight[0], ptLight[1]);
                g.DrawLine(pBorderLight, ptLight[1], ptLight[2]);
            }

            Point[] ptShadow = new Point[]
		    {
			    new Point(rcInner.Left-1, rcInner.Bottom-1),
			    new Point(rcInner.Left-1, rcInner.Top-5),
			    new Point(rcInner.Right-1, rcInner.Top-5)
		    };
            Point[] pt2007Shadow = new Point[]
		    {
			    new Point(rcInner.Left-1, rcInner.Bottom-1),
			    new Point(rcInner.Left-1, rcInner.Top-1),
			    new Point(rcInner.Right-1, rcInner.Top-1)
		    };
            Color clBorderShadow = Office12ColorTable.GetAlphaBlendedColor(Color.Black, clBorder, 32);
            Pen pBorderShadow = new Pen(clBorderShadow);
            if (this.RibbonStyle == Tools.RibbonStyle.Office2007 || this.RibbonStyle == Tools.RibbonStyle.Office2010)
            {
                if (ColorScheme == ColorSchemeType.Silver)
                    pBorderShadow = new Pen(ColorTranslator.FromHtml("#E5E7E9"),1.0F);
                else if (ColorScheme == ColorSchemeType.Black)
                    pBorderShadow = new Pen(ColorTranslator.FromHtml("#717171"),1.0F);
                else if (ColorScheme == ColorSchemeType.Blue)
                    pBorderShadow = new Pen(ColorTranslator.FromHtml("#BBCEE6"),1.0F);
                g.DrawLine(pBorderShadow, pt2007Shadow[0], pt2007Shadow[1]);
                g.DrawLine(pBorderShadow, pt2007Shadow[1], pt2007Shadow[2]);
            }
            pBorderShadow.Dispose();
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void DrawFrame(Graphics g, Rectangle rc)
        {
            DrawFrameBackground(g, rc);

            DrawFrameCaption(g, rc);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void DrawFrameBackground(Graphics g, Rectangle rc)
        {
            // Paint Title background depending on form state.
            Color clBegin = this.ActiveState ? m_ColorTable.ActiveTitleGradientBegin : m_ColorTable.InActiveTitleGradientBegin;
            Color clEnd = this.ActiveState ? m_ColorTable.ActiveTitleGradientEnd : m_ColorTable.InActiveTitleGradientEnd;

            // Fix of RightToLeft painting problem
            int x = rc.Left - 1;
            int width = rc.Width + 1;
            if (this.RibbonStyle == RibbonStyle.Office2013)
            {
                using (SolidBrush brush = new SolidBrush(Color.Blue))
                {
                    g.FillRectangle(brush, new Rectangle(x, rc.Top, width, rc.Height));
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, rc.Top, 1, rc.Height), clBegin, clEnd, 90f))
                {
                    brush.Blend = m_blTitle;
                    g.FillRectangle(brush, new Rectangle(x, rc.Top, width, rc.Height));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void DrawFrameCaption(Graphics g, Rectangle rc)
        {
            FrameLayoutInfo fl = this.FrameLayout;

            DrawFrameIcon(g, fl.IconBox);

            DrawFrameButton(g, fl.CloseBox, SystemCommand.SC_CLOSE, this.CloseBox);
            DrawFrameButton(g, fl.MaximizeBox, SystemCommand.SC_MAXIMIZE, this.MaximizeBox);
            DrawFrameButton(g, fl.MinimizeBox, SystemCommand.SC_RESTORE, this.MinimizeBox);
            DrawFrameButton(g, fl.HelpButton, SystemCommand.SC_CONTEXTHELP, this.HelpButton);

            DrawFrameText(g, fl.TextBox);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void DrawFrameIcon(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                if (this.Icon != null)
                {
                    Icon smallIcon = new Icon(this.Icon, rc.Size);
                    if (smallIcon != null)
                    {
                        rc.X = FrameLayout.BorderWidth;
                        g.DrawIcon(smallIcon, rc);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <param name="rc"></param>
        /// <param name="bEnabled"></param>
        private void DrawFrameButton(Graphics g, Rectangle rc, SystemCommand buttonId, bool bEnabled)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Image img = GetButtonImage(buttonId);

                if (img != null)
                {
                    Size szImage = img.Size;

                    int x = rc.X + (rc.Width - szImage.Width) / 2 - (this.IsRightToLeft ? 1 : 0);
                    int y = rc.Y + (rc.Height - szImage.Height) / 2 + 1;

                    if (bEnabled)
                    {
                        if (this.HighlightedButton == buttonId)
                        {
                            if (this.PressedButton == buttonId)
                            {
                                DrawFrameButtonBackgroundPressed(g, ref rc);
                            }
                            else
                            {
                                DrawFrameButtonBackgroundSelected(g, ref rc);
                            }
                        }
                        g.DrawImage(img, x, y);
                    }
                    else
                    {
                        ControlPaint.DrawImageDisabled(g, img, x, y, Color.White);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void DrawFrameButtonBackgroundSelected(Graphics g, ref Rectangle rc)
        {
            SmoothingMode saveMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawFrameButtonGradient(g, ref rc, m_ColorTable.SystemButtonSelectedGradientBegin, m_ColorTable.SystemButtonSelectedGradientEnd);

            DrawFrameButtonBorder(g, ref rc, m_ColorTable.SystemButtonBorderSelected);

            g.SmoothingMode = saveMode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void DrawFrameButtonBackgroundPressed(Graphics g, ref Rectangle rc)
        {
            SmoothingMode saveMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawFrameButtonGradient(g, ref rc, m_ColorTable.SystemButtonPressedGradientBegin, m_ColorTable.SystemButtonPressedGradientEnd);

            DrawFrameButtonFlash(g, ref rc);

            DrawFrameButtonBorder(g, ref rc, m_ColorTable.SystemButtonBorderPressed);

            g.SmoothingMode = saveMode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        /// <param name="color"></param>
        /// <param name="color_4"></param>
        private void DrawFrameButtonGradient(Graphics g, ref Rectangle rc, Color clBegin, Color clEnd)
        {
            IntPtr hRgn = WindowsAPI.CreateRoundRectRgn(rc.X, rc.Y, rc.Right, rc.Bottom, 2, 2);
            if (hRgn != IntPtr.Zero)
            {
                using (Region region = Region.FromHrgn(hRgn))
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(rc.X, rc.Y, 1, rc.Height), clBegin, clEnd, 90f))
                    {
                        brush.Blend = m_blFrameButton;
                        brush.WrapMode = WrapMode.TileFlipY;

                        g.FillRegion(brush, region);
                    }
                }
                WindowsAPI.DeleteObject(hRgn);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void DrawFrameButtonFlash(Graphics g, ref Rectangle rc)
        {
            Rectangle rcBrush = new Rectangle(rc.X, rc.Y + rc.Height * 3 / 5, rc.Width, rc.Height);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(rcBrush);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.White;
                    brush.SurroundColors = new Color[] { Color.Transparent };

                    g.FillRectangle(brush, rc);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        /// <param name="color"></param>
        private void DrawFrameButtonBorder(Graphics g, ref Rectangle rc, Color color)
        {
            Rectangle rcBrush = new Rectangle(rc.X, rc.Y, 1, rc.Height);
            using (LinearGradientBrush brush = new LinearGradientBrush(rcBrush, Color.White, Color.Transparent, 90f))
            {
                brush.Blend = m_blFrameButtonBorder;

                using (Pen pen = new Pen(brush))
                {
                    g.DrawRectangle(pen, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
                }
            }
            using (Pen pen = new Pen(color))
            {
                g.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rc, 1));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void DrawFrameText(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                string sText = this.Text;

                if (sText != string.Empty)
                {
                    using (StringFormat sf = StringFormat.GenericDefault)
                    {
                        sf.FormatFlags |= StringFormatFlags.LineLimit;
                        sf.Trimming = StringTrimming.EllipsisCharacter;

                        sf.Alignment = this.IsRightToLeft || this.RightToLeft == RightToLeft.Yes ? StringAlignment.Far : StringAlignment.Near;

                        using (Brush brush = new SolidBrush(m_ColorTable.RibbonTitleText))
                        {
                            g.DrawString(sText, this.Font, brush, rc, sf);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonID"></param>
        private Image GetButtonImage(SystemCommand buttonId)
        {
            Image img = null;

            Color c = buttonId != this.SelectedButton ? m_ColorTable.SystemButtonForeground : m_ColorTable.SystemButtonForegroundSelected;

            switch (buttonId)
            {
                case SystemCommand.SC_CLOSE:
                    img = SystemImages.GetImageClose(c ,false);
                    break;
                case SystemCommand.SC_RESTORE:
                    img = SystemImages.GetImageRestore(c,false);
                    break;
                case SystemCommand.SC_MAXIMIZE:
                    img = SystemImages.GetImageMaximize(c,false);
                    break;
                case SystemCommand.SC_CONTEXTHELP:
                    img = SystemImages.GetImageHelp();
                    break;
            }
            return img;
        }
        
        private string mHelpButtonToolTip = SR.GetString(SR.ToolStripItemHelpButton);
        /// <summary>
        /// Gets or sets HelpButtonToolTip
        /// </summary>
		[Description("Gets or sets tooltip for help button")]
        public string HelpButtonToolTip
        {
            get
            {
                return this.mHelpButtonToolTip;
            }
            set
            {
                if (this.mHelpButtonToolTip != value)
                {
                    this.mHelpButtonToolTip = value;
                    this.Invalidate();
                }
            }
        }

        private Image mHelpButtonImage = null;

        /// <summary>
        /// Gets or sets HelpButtonImage
        /// </summary>
        [Description("Gets or sets image for Help Button")]
        public Image HelpButtonImage
        {
            get
            {
                if (mHelpButtonImage != null)
                    return mHelpButtonImage;
                else
                {
                    if (this._ribbonStyle == RibbonStyle.Office2007)
                        return SystemImages.GetImageHelp();
                    else if (this._ribbonStyle == Tools.RibbonStyle.Office2010)
                        return SystemImages.GetOffice2010ImageHelp();
                    else
                        return SystemImages.GetOffice2013ImageHelp();
                }
            }
            set
            {
                if (this.mHelpButtonImage != value)
                {
                    this.mHelpButtonImage = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private SystemCommand GetButtonId(IntPtr points)
        {
            int x = WindowsAPI.LOW_ORDER(points);
            int y = WindowsAPI.HIGH_ORDER(points);

            return GetButtonId(new Point(x, y));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private SystemCommand GetButtonId(Point pt)
        {
            RECT rc = new RECT();
            WindowsAPI.GetWindowRect(this.Handle, ref rc);

            int x = pt.X - rc.left;
            int y = pt.Y - rc.top;

            FrameLayoutInfo fl = this.FrameLayout;

            if (fl.CloseBox.Contains(x, y))
                return SystemCommand.SC_CLOSE;
            if (fl.MaximizeBox.Contains(x, y))
                return SystemCommand.SC_MAXIMIZE;
            if (fl.MinimizeBox.Contains(x, y))
                return SystemCommand.SC_RESTORE;
            if (fl.HelpButton.Contains(x, y))
                return SystemCommand.SC_CONTEXTHELP;

            return SC_NONE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private bool IsButtonEnabled(SystemCommand buttonId)
        {
            bool bResult = true;

            switch (buttonId)
            {
                case SystemCommand.SC_MAXIMIZE:
                    {
                        bResult = this.MaximizeBox;
                        break;
                    }
                case SystemCommand.SC_RESTORE:
                    {
                        bResult = this.MinimizeBox;
                        break;
                    }
                case SystemCommand.SC_CLOSE:
                    {
                        bResult = this.CloseBox;
                        break;
                    }
            }
            return bResult;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InvalidateFrame()
        {
            if (this.IsHandleCreated)
            {
                WindowsAPI.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_INVALIDATE);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        private void BaseWndProc(ref Message m)
        {
            using (CaptionManager cm = new CaptionManager(this, true))
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Rectangle GetMdiMaxBounds()
        {
            IntPtr hParent = this.Parent.Handle;

            int style = WindowsAPI.GetWindowLong(hParent, (int)SetWindowLongOffsets.GWL_STYLE);
            int styleEx = WindowsAPI.GetWindowLong(hParent, (int)SetWindowLongOffsets.GWL_EXSTYLE);

            RECT rcBorder = new RECT();
            WindowsAPI.AdjustWindowRectEx(ref rcBorder, style & ~(int)(WindowStyles.WS_HSCROLL | WindowStyles.WS_VSCROLL), false, styleEx);

            RECT rcParent = new RECT();
            WindowsAPI.GetWindowRect(this.Parent.Handle, ref rcParent);

            Padding m = this.Borders;

            if (this.GetMargins != null)
            {
                this.GetMargins(ref m);
            }
            if (this.RibbonStyle != RibbonStyle.Office2007)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is RibbonControlAdv)
                    {
                        RibbonControlAdv control = ctrl as RibbonControlAdv;
                        m.Top = control.HeaderInternal.TabItemsRectangle.Height + 8;
                    }
                }
            }
            return new Rectangle(-m.Left, -m.Top, rcParent.Width - rcBorder.Width + m.Horizontal, rcParent.Height - rcBorder.Height + m.Vertical);
        }

        #endregion

        #region ShouldSerialize & Reset members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeAppearance()
        {
            return m_Appearance != AppearanceType.Office2007;
        }
        /// <summary>
        /// 
        /// </summary>
        void ResetAppearance()
        {
            m_Appearance = AppearanceType.Office2007;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeColorScheme()
        {
            return m_ColorScheme != ColorSchemeType.Managed;
        }
        /// <summary>
        /// 
        /// </summary>
        void ResetColorScheme()
        {
            this.ColorScheme = ColorSchemeType.Managed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeMinimumSize()
        {
            return (base.MinimumSize != base.DefaultMinimumSize);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void ResetMinimumSize()
        {
            base.MinimumSize = base.DefaultMinimumSize;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeBorder()
        {
            return m_Borders != new Padding(BORDER_WIDTH, 1, BORDER_WIDTH, BORDER_WIDTH);
        }
        /// <summary>
        /// 
        /// </summary>
        void ResetBorder()
        {
            using (Graphics g = this.CreateGraphics())
            {
                if (g.DpiX > 120)
                    m_Borders = new Padding(DPI_150_BORDER_WIDTH, 1, DPI_150_BORDER_WIDTH, DPI_150_BORDER_WIDTH);
                else if (g.DpiX > 96)
                    m_Borders = new Padding(DPI_125_BORDER_WIDTH, 1, DPI_125_BORDER_WIDTH, DPI_125_BORDER_WIDTH);
                else
                    m_Borders = new Padding(BORDER_WIDTH, 1, BORDER_WIDTH, BORDER_WIDTH);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        bool ShouldSerializeShortcut(Component component)
        {
            foreach (Component c in m_hShortcuts.Values)
            {
                if (c.Equals(component))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        void ResetShortcut(Component component)
        {
            SetShortcut(component, Keys.None);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the office color scheme of the Ribbon Form.
        /// </summary>
        [Description("Specifies the office color scheme of the Ribbon Form.")]
        public ColorSchemeType ColorScheme
        {
            get
            {
                return m_ColorScheme;
            }
            set
            {
                if (m_ColorScheme != value)
                {
                    m_ColorScheme = value;

                    UpdateColorScheme();
                    UpdateRenderers();

                    Invalidate(false);
                }
            }
        }
        private Color menuColor = Color.Gray;
        internal Color MenuColor
        {
            get
            {
                return menuColor;
            }
            set
            {
                menuColor = value;
                this.Refresh();
            }
        }
        private RibbonStyle _ribbonStyle = RibbonStyle.Office2007;
        internal RibbonStyle RibbonStyle
        {
            get { return _ribbonStyle; }
            set
            {
                if (value != _ribbonStyle)
                {
                    _ribbonStyle = value;

                    if (RibbonStyle != RibbonStyle.Office2013)
                    {
                        this.ControlBox = true;
                        using (Graphics g = this.CreateGraphics())
                        {
                            if (g.DpiX > 120)
                            {
                                Borders = new Padding(DPI_150_BORDER_WIDTH, 1, DPI_150_BORDER_WIDTH, DPI_150_BORDER_WIDTH);
                            }
                            else if (g.DpiX > 96)
                            {
                                Borders = new Padding(DPI_125_BORDER_WIDTH, 1, DPI_125_BORDER_WIDTH, DPI_125_BORDER_WIDTH);
                            }
                            else
                            {
                                Borders = new Padding(BORDER_WIDTH, 1, BORDER_WIDTH, BORDER_WIDTH);
                            }
                        }
                        this.UpdateStyles();
                    }
                    else
                    {
                        this.ControlBox = true;
                        Borders = new Padding(0, 0, 0, 0);
                        this.Padding = new Padding(1, 0, 1, 0);
                        this.UpdateStyles();
                    }
                }
            }
        }
        
        /// <summary>
        /// Specifies the appearance of the form.
        /// </summary>
        [Description("Specifies the appearance of the form.")]
        public AppearanceType Appearance
        {
            get
            {
                return m_Appearance;
            }
            set
            {
                if (m_Appearance != value)
                {
                    m_Appearance = value;
                    OnAppearanceChanged();
                }
            }
        }
        /// <summary>
        /// Specifies the radius for the curved top left corner of the Ribbon Form.
        /// </summary>
        [DefaultValue(VERTEX_RADIUS)]
        [Description("Specifies the radius for the curved top left corner of the Ribbon Form.")]
        public int TopLeftRadius
        {
            get
            {
                return m_TopLeftRadius;
            }
            set
            {
                if (m_TopLeftRadius != value)
                {
                    m_TopLeftRadius = value;

                    if (this.Appearance == AppearanceType.Office2007)
                    {
                        UpdateRegion();
                    }
                }
            }
        }
        /// <summary>
        /// Office2007 borders.
        /// </summary>
        [Description("Office2007 borders")]
        public virtual Padding Borders
        {
            get
            {
                if (this.FormBorderStyle == FormBorderStyle.None)
                {
                    return new Padding(0);
                }
                return m_Borders;
            }
            set
            {
                m_Borders = value;

                if (this.Appearance == AppearanceType.Office2007)
                {
                    PerformLayout();
                }
            }
        }
        /// <summary>
        /// AutoScroll is not supported
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoScroll
        {
            get
            {
                return false;
            }
            set
            {
                base.AutoScroll = false;
            }
        }
        /// <summary>
        /// The minimum size the form can be resized to.
        /// </summary>
        public override Size MinimumSize
        {
            get
            {
                Size szMin = base.MinimumSize;

                if (this.Appearance != AppearanceType.Normal && GetMinSize != null)
                {
                    Size size = Size.Empty;
                    GetMinSize(ref size);

                    szMin.Width = Math.Max(szMin.Width, size.Width);
                    szMin.Height = Math.Max(szMin.Height, size.Height + this.BordersInternal.Vertical);
                }

                return szMin;
            }
            set
            {
                base.MinimumSize = value;
            }
        }
        /// <summary>
        /// Gets the bounds of the display rectangle.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rc = base.DisplayRectangle;

                rc.Y += m_nTopHeight;
                rc.Height -= m_nTopHeight + m_nBottomHeight;

                if (this.Appearance == AppearanceType.Office2007)
                {
                    Padding borders = this.BordersInternal;

                    rc.X += borders.Left;
                    rc.Y += borders.Top;
                    rc.Width -= borders.Horizontal;
                    rc.Height -= borders.Vertical;
                }

                return rc;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams result = base.CreateParams;

                if (this.Appearance == AppearanceType.Office2007)
                {
                    if (this.TopLevel || this.IsMdiChild)
                    {
                        UpdateStartupBounds(ref result);
                    }
                }

                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        [Editor(typeof(Design.RibbonFormShortcutEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Keys GetShortcut(Component component)
        {
            foreach (DictionaryEntry de in m_hShortcuts)
            {
                if (de.Value.Equals(component))
                    return (Keys)de.Key;
            }
            return Keys.None;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        [Editor(typeof(Design.RibbonFormShortcutEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public void SetShortcut(Component component, Keys value)
        {
            //if (CheckShortcut(component, value))
            {
                Keys key = GetShortcut(component);

                if (key != Keys.None)
                {
                    m_hShortcuts.Remove(key);
                }

                if (value != Keys.None)
                {
                    m_hShortcuts[value] = component;
                }
            }
        }
        /// <summary>
        /// Gets or sets parent form state.
        /// </summary>
        internal bool ActiveState
        {
            get
            {
                if (this.IsMdiChild)
                {
                    Form f = this.MdiParent;

                    if (f != null && f.MdiChildren.Length == 1)
                    {
                        return true;
                    }
                }
                return m_bActiveState;
            }
            set
            {
                if (m_bActiveState != value)
                {
                    m_bActiveState = value;
                    InvalidateBorder();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool CompositionEnabled
        {
            get
            {
                    return m_bCompositionEnabled && this.TopLevel;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Padding BordersInternal
        {
            get
            {
                Padding pd = this.Borders;

                if (this.Appearance == AppearanceType.Office2007 && this.CompositionEnabled)
                {
                    if (this.RibbonStyle != Tools.RibbonStyle.Office2013)
                    {
                        pd.Left = 0;
                        pd.Right = 0;
                        pd.Bottom = 0;
                    }
                    else
                    {
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            if (RibbonStyle == Tools.RibbonStyle.Office2013)
                            {
                                pd.Left = 6;
                                pd.Right = 3;
                                pd.Bottom = 6;
                            }
                            else
                            {
                                pd.Left = 6;
                                pd.Right = 3;
                                pd.Bottom = 3;
                            }
                        }
                        else
                        {
                            pd.Left = 0;
                            pd.Right = 0;
                            pd.Bottom = 0;
                        }
                    }
                }

                return pd;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Padding DwmFrame
        {
            get
            {
                return m_DwmFrame;
            }
            set
            {
                if (m_DwmFrame != value)
                {
                    m_DwmFrame = value;
                    UpdateFrame();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Padding SystemFrame
        {
            get
            {
                if (this.IsHandleCreated)
                {
                    RECT rc = new RECT(0, 0, 0, 0);

                    int style = WindowsAPI.GetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_STYLE);
                    int styleEx = WindowsAPI.GetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_EXSTYLE);

                    if (WindowsAPI.AdjustWindowRectEx(ref rc, style, false, styleEx) != 0)
                    {
                        return new Padding(-rc.left, -rc.top, rc.right, rc.bottom);
                    }
                }

                return Padding.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Rectangle DwmButtonBounds
        {
            get
            {
                if (this.IsHandleCreated && this.CompositionEnabled)
                {
                    RECT rc = new RECT();

                    DwmAPI.DwmGetWindowAttribute(this.Handle, DwmAPI.DWMWINDOWATTRIBUTE.DWMWA_CAPTION_BUTTON_BOUNDS, ref rc, Marshal.SizeOf(rc));

                    Rectangle rcResult = new Rectangle(rc.left, rc.top, rc.Width, rc.Height);

                    if (this.IsRightToLeft)
                    {
                        rcResult.X = this.Width - rcResult.Right;
                    }

                    return rcResult;
                }
                return Rectangle.Empty;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsRightToLeft
        {
            get
            {
                bool bResult = false;

                if (this.IsHandleCreated)
                {
                    int styleEx = WindowsAPI.GetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_EXSTYLE);
                    bResult = (styleEx & (int)WindowExStyles.WS_EX_LAYOUTRTL) != 0;
                }

                return bResult;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private SystemCommand SelectedButton
        {
            get
            {
                return m_selectedButton;
            }
            set
            {
                if (m_selectedButton != value)
                {
                    m_selectedButton = value;

                    if (m_pressedButton != SC_NONE && m_pressedButton != value)
                    {
                        value = SC_NONE;
                    }

                    if (m_highlightedButton != value)
                    {
                        m_highlightedButton = value;
                        InvalidateFrame();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private SystemCommand PressedButton
        {
            get
            {
                return m_pressedButton;
            }
            set
            {
                if (m_pressedButton != value)
                {
                    m_pressedButton = value;

                    if (value == SC_NONE)
                    {
                        value = m_selectedButton;
                    }

                    m_highlightedButton = value;
                    InvalidateFrame();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private SystemCommand HighlightedButton
        {
            get
            {
                return m_highlightedButton;
            }
            set
            {
                if (m_highlightedButton != value)
                {
                    m_highlightedButton = value;
                    InvalidateFrame();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private FrameLayoutInfo FrameLayout
        {
            get
            {
                if (m_frameLayout == null)
                {
                    m_frameLayout = new FrameLayoutInfo(this);
                    m_frameLayout.PerformLayout(this.Width, this.Height);
                }
                return m_frameLayout;
            }
        }

        internal bool CloseBox
        {
            get
            {
                return (0 == (this.CreateParams.ClassStyle & CS_NOCLOSE));
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// 
        /// </summary>
        public event GetMinSizeHandler GetMinSize;
        /// <summary>
        /// 
        /// </summary>
        internal event GetMarginsHandler GetMargins;
        /// <summary>
        /// 
        /// </summary>
        internal event EventHandler ShowSystemMenu;
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        AppearanceType m_Appearance = AppearanceType.Office2007;
        /// <summary>
        /// 
        /// </summary>
        ColorSchemeType m_ColorScheme = ColorSchemeType.Managed;
        /// <summary>
        /// 
        /// </summary>
        Office12ColorTable m_ColorTable;
        /// <summary>
        /// 
        /// </summary>
        private Office2010ColorTable colorTable = null;
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        int m_TopLeftRadius = VERTEX_RADIUS;
        /// <summary>
        /// 
        /// </summary>
        Padding m_Borders;
        /// <summary>
        /// 
        /// </summary>
        Padding m_DwmFrame;
        /// <summary>
        /// Height of all TopMost Ribbon controls
        /// </summary>
        int m_nTopHeight = 0;
        /// <summary>
        /// Height of all BottomMost Ribbon controls
        /// </summary>
        int m_nBottomHeight = 0;
        /// <summary>
        /// 
        /// </summary>
        Hashtable m_hShortcuts;
        /// <summary>
        /// Indicates parent form's state.
        /// </summary>
        private bool m_bActiveState = true;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bSuppressSizing = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bCompositionEnabled = false;
        /// <summary>
        /// Selected system button. (SC_NONE - no button is selected)
        /// </summary>
        private SystemCommand m_selectedButton = SC_NONE;
        /// <summary>
        /// Pressed system button. (SC_NONE - no button is pressed)
        /// </summary>
        private SystemCommand m_pressedButton = SC_NONE;
        /// <summary>
        /// Highlighted system button. (SC_NONE - no button is highlighted)
        /// </summary>
        private SystemCommand m_highlightedButton = SC_NONE;
        /// <summary>
        /// 
        /// </summary>
        private FrameLayoutInfo m_frameLayout;
        /// <summary>
        /// 
        /// </summary>
        internal MenuStrip m_MenuStrip=new MenuStrip();
        /// <summary>
        /// 
        /// </summary>
        private bool m_bMouseIsTracked = false;
        /// <summary>
        /// 
        /// </summary>
        static Blend m_blTitle;
        /// <summary>
        /// 
        /// </summary>
        static Blend m_blFrameButton;
        /// <summary>
        /// 
        /// </summary>
        static Blend m_blFrameButtonBorder;
        #endregion

        #region Nested classes
        /// <summary>
        /// 
        /// </summary>
        class FrameLayoutInfo
        {
            #region Constructors
            public FrameLayoutInfo(RibbonForm form)
            {
                m_form = form;
            }
            #endregion

            #region Methods
            public void PerformLayout(int width, int height)
            {
                m_rcIcon = Rectangle.Empty;
                m_rcMin = Rectangle.Empty;
                m_rcMax = Rectangle.Empty;
                m_rcClose = Rectangle.Empty;
                m_rcHelpButton = Rectangle.Empty;
                m_rcText = Rectangle.Empty;

                Size szIcon = SystemInformation.SmallIconSize;
                bool bRightToLeft = m_form.IsRightToLeft;

                int buttonWidth = (szIcon.Width & ~1) + 3;
                int buttonHeight = szIcon.Height + 2;

                int top = m_form.WindowState != FormWindowState.Minimized ? m_iBorderWidth + 1 : (height - buttonHeight) / 2;
                int left = m_iBorderWidth;
                int right = width - m_iBorderWidth;

                if (m_form.FormBorderStyle != FormBorderStyle.None)
                {
                    if (m_form.ControlBox)
                    {
                        if (m_form.ShowIcon)
                        {
                            m_rcIcon.Size = szIcon;
                            m_rcIcon.Y = top;

                            if (bRightToLeft)
                            {
                                right -= szIcon.Width;
                                m_rcIcon.X = right;
                            }
                            else
                            {
                                m_rcIcon.X = left;
                                left = m_rcIcon.Right;
                            }
                        }

                        if (m_form.CloseBox)
                        {
                            m_rcClose.Y = top;
                            m_rcClose.Width = buttonWidth;
                            m_rcClose.Height = buttonHeight;

                            if (bRightToLeft)
                            {
                                m_rcClose.X = left;
                                left = m_rcClose.Right;
                            }
                            else
                            {
                                right -= buttonWidth;
                                m_rcClose.X = right;
                            }
                        }

                        if (m_form.MaximizeBox || m_form.MinimizeBox)
                        {
                            m_rcMax = m_rcClose;
                            m_rcMin = m_rcClose;

                            if (bRightToLeft)
                            {
                                m_rcMax.X = left;
                                m_rcMin.X = m_rcMax.Right;
                                left = m_rcMin.Right;
                            }
                            else
                            {
                                m_rcMax.X = right - buttonWidth;
                                m_rcMin.X = m_rcMax.X - buttonWidth;
                                right = m_rcMin.X;
                            }
                        }
                        else if (m_form.HelpButton)
                        {
                            m_rcHelpButton = m_rcClose;

                            if (bRightToLeft)
                            {
                                m_rcHelpButton.X = left;
                                left = m_rcHelpButton.Right;
                            }
                            else
                            {
                                right -= buttonWidth;
                                m_rcHelpButton.X = right;
                            }
                        }
                    }

                    m_rcText = new Rectangle(left, top + 2, right - left, buttonHeight);
                }
            }
            #endregion

            #region Properties
            /// <summary>
            /// 
            /// </summary>
            public Rectangle TextBox
            {
                get { return m_rcText; }
            }
            /// <summary>
            /// 
            /// </summary>
            public Rectangle IconBox
            {
                get { return m_rcIcon; }
            }
            /// <summary>
            /// 
            /// </summary>
            public Rectangle MinimizeBox
            {
                get { return m_rcMin; }
            }
            /// <summary>
            /// 
            /// </summary>
            public Rectangle MaximizeBox
            {
                get { return m_rcMax; }
            }
            /// <summary>
            /// 
            /// </summary>
            public Rectangle CloseBox
            {
                get { return m_rcClose; }
            }
            /// <summary>
            /// 
            /// </summary>
            public Rectangle HelpButton
            {
                get { return m_rcHelpButton; }
            }
            /// <summary>
            /// 
            /// </summary>
            public int BorderWidth
            {
                get { return m_iBorderWidth; }
            }
            #endregion

            #region Fields
            /// <summary>
            /// 
            /// </summary>
            private RibbonForm m_form;
            /// <summary>
            /// 
            /// </summary>
            private Rectangle m_rcText = Rectangle.Empty;
            /// <summary>
            /// 
            /// </summary>
            public Rectangle m_rcIcon = Rectangle.Empty;
            /// <summary>
            /// 
            /// </summary>
            public Rectangle m_rcMin = Rectangle.Empty;
            /// <summary>
            /// 
            /// </summary>
            public Rectangle m_rcMax = Rectangle.Empty;
            /// <summary>
            /// 
            /// </summary>
            public Rectangle m_rcClose = Rectangle.Empty;
            /// <summary>
            /// 
            /// </summary>
            public Rectangle m_rcHelpButton = Rectangle.Empty;
            /// <summary>
            /// 
            /// </summary>
            private int m_iBorderWidth = BORDER_WIDTH;
            #endregion
        }

        class CaptionManager : IDisposable
        {
            const int MASK = (int)WindowStyles.WS_CAPTION;

            #region Constructors
            /// <summary>
            /// 
            /// </summary>
            /// <param name="c"></param>
            public CaptionManager(RibbonForm form, bool bHideCaption)
            {
                if (form != null && form.Appearance == AppearanceType.Office2007)
                {
                    if (form.IsHandleCreated)
                    {
                        IntPtr hWnd = form.Handle;

                        m_style = WindowsAPI.GetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_STYLE);

                        if (bHideCaption)
                        {
                            if ((m_style & MASK) != 0)
                            {
                                WindowsAPI.SetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_STYLE, (IntPtr)(m_style & ~MASK));

                                m_form = form;
                            }
                        }
                        else
                        {
                            if ((m_style & MASK) == 0)
                            {
                                WindowsAPI.SetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_STYLE, (IntPtr)(m_style | MASK));

                                m_form = form;
                            }
                        }
                    }
                }
            }
            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                if (m_form != null && m_form.IsHandleCreated)
                {
                    WindowsAPI.SetWindowLong(m_form.Handle, (int)SetWindowLongOffsets.GWL_STYLE, (IntPtr)(m_style));
                }
            }

            #endregion

            #region Fields

            private RibbonForm m_form = null;

            private int m_style;

            #endregion

        }
        #endregion
    }
    #endregion

    #region IShortcutSupport
    public interface IShortcutSupport
    {
        void ProcessShortcut();
    }
    #endregion

    #region DwmAPI
    /// <summary>
    /// 
    /// </summary>
    internal class DwmAPI
    {
        #region Constants
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        #endregion

        #region Enums
        /// <summary>
        /// 
        /// </summary>
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_LAST
        };
        /// <summary>
        /// 
        /// </summary>
        public enum DttFlags
        {
            DTT_CRTEXT = 1,
            DTT_GLOWSIZE = 2048,
            DTT_COMPOSITED = 8192
        };
        #endregion

        #region Structs

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct DTTOPTS
        {
            public int dwSize;
            public DttFlags dwFlags;
            public int crText;
            public int crBorder;
            public int crShadow;
            public int iTextShadowType;
            public POINT ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public bool fApplyOverlay;
            public int iGlowSize;
            public IntPtr pfnDrawTextCallback;
            IntPtr lParam;
        };
        #endregion

        #region Imports

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pfEnabled"></param>
        /// <returns></returns>
        [DllImport("dwmapi.dll")]
        public extern static Int32 DwmIsCompositionEnabled(ref bool pfEnabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        /// <param name="pMarInset"></param>
        /// <returns></returns>
        [DllImport("dwmapi.dll")]
        public extern static Int32 DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public extern static Int32 DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);

        [DllImport("dwmapi.dll")]
        public extern static Int32 DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref RECT rc, Int32 cbAttribute);

        [DllImport("uxtheme.dll")]
        public static extern IntPtr OpenThemeData(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszClassList);

        [DllImport("uxtheme.dll")]
        public static extern IntPtr CloseThemeData(IntPtr hTheme);

        [DllImport("uxtheme.dll")]
        public extern static Int32 DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, [MarshalAs(UnmanagedType.LPWStr)]string pszText, int iCharCount, DrawTextFormatFlags dwFlags, ref RECT pRect, ref DTTOPTS pOptions);

        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public static bool IsCompositionEnabled
        {
            get
            {
                bool bResult = false;

                if (Environment.OSVersion.Version.Major >= 6)
                {
                    DwmIsCompositionEnabled(ref bResult);
                }

                return bResult;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gph"></param>
        /// <param name="rgn"></param>
        public static void FillBlackRegion(Graphics g, Rectangle rc)
        {
            IntPtr destdc = g.GetHdc();
            if (destdc != IntPtr.Zero)
            {
                FillBlackRegion(destdc, rc);

                g.ReleaseHdc();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destdc"></param>
        /// <param name="rc"></param>
        public static void FillBlackRegion(IntPtr destdc, Rectangle rc)
        {
            IntPtr memdc = WindowsAPI.CreateCompatibleDC(destdc);
            if (memdc != IntPtr.Zero)
            {
                BITMAPINFO_FLAT dib = new BITMAPINFO_FLAT();
                dib.bmiHeader_biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                dib.bmiHeader_biHeight = -rc.Height;
                dib.bmiHeader_biWidth = rc.Width;
                dib.bmiHeader_biPlanes = 1;
                dib.bmiHeader_biBitCount = 32;
                dib.bmiHeader_biCompression = 0;

                IntPtr ppv = IntPtr.Zero;

                IntPtr bitmap = WindowsAPI.CreateDIBSection(memdc, ref dib, 0, ref ppv, IntPtr.Zero, 0);
                if (bitmap != IntPtr.Zero)
                {
                    IntPtr bitmapOld = WindowsAPI.SelectObject(memdc, bitmap);

                    WindowsAPI.BitBlt(destdc, rc.X, rc.Y, rc.Width, rc.Height, memdc, 0, 0, (uint)PatBltTypes.SRCCOPY);

                    WindowsAPI.SelectObject(memdc, bitmapOld);

                    WindowsAPI.DeleteObject(bitmap);
                }

                WindowsAPI.DeleteDC(memdc);
            }
        }
        #endregion

    }
    #endregion
}
#endif