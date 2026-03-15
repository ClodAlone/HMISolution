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

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Documentation;
using Syncfusion.Runtime.InteropServices;

using InteropServices_NativeMethods = Syncfusion.Runtime.InteropServices.NativeMethods;
using Runtime_InteropServices_NativeMethods = Syncfusion.Runtime.InteropServices.NativeMethods;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
    [DocumentationExclude()]
    public class MdiSysMenuProvider : DisposableWithDisposedProp
    {
        #region Class constants
        protected internal const string ICO_MINIMIZE = "Syncfusion.Windows.Forms.Tools.FrameworkComponents.XPMenus.bmps.minimizemc.ico";
        protected internal const string ICO_RESTORE = "Syncfusion.Windows.Forms.Tools.FrameworkComponents.XPMenus.bmps.restoremc.ico";
        protected internal const string ICO_CLOSE = "Syncfusion.Windows.Forms.Tools.FrameworkComponents.XPMenus.bmps.closemc.ico";
        /// <summary>
        /// WM_APP + 3
        /// </summary>
        protected internal const int UM_REMOVE_MAIN_MENU = 0x8003;
        /// <summary>
        /// WM_APP + 4
        /// </summary>
        protected internal const int UM_RESTORE_MAIN_MENU = 0x8004;

        [DocumentationExclude()]
        protected enum SysButtons
        {
            Minimize = 0,
            Restore,
            Close,
            None
        }
        #endregion

        #region Class static members
        private static Icon _icoRestore = null;
        private static Icon _icoMinimize = null;
        private static Icon _icoClose = null;
        #endregion

        #region Class members
        private bool m_bNeedInvalidate = true;
        /// <summary>
        ///
        /// </summary>
        private MdiClientNativeWnd m_nwMdiClient = null;
        /// <summary>
        ///
        /// </summary>
        protected internal MdiClient m_wndMdiClient = null;
        /// <summary>
        ///
        /// </summary>
        protected internal IntPtr m_mnuActiveChild = IntPtr.Zero;
        /// <summary>
        ///
        /// </summary>
        private Form m_wndActiveChild = null;
        /// <summary>
        ///
        /// </summary>
        private SysButtons m_sbHilight = SysButtons.None;
        /// <summary>
        ///
        /// </summary>
        private SysButtons m_sbMouseClick = SysButtons.None;
        /// <summary>
        ///
        /// </summary>
        private MdiPopupMenu m_systemMenu;
        /// <summary>
        ///
        /// </summary>
        private bool m_bShowControlBox = true;
        /// <summary>
        ///
        /// </summary>
        private GCHandle m_ProcHandle;
        /// <summary>
        ///
        /// </summary>
        private IntPtr m_MsgHookProc;
        /// <summary>
        ///
        /// </summary>
        private bool m_bChildMaximized = false;
        /// <summary>
        ///
        /// </summary>
        private IntPtr m_hwndChild;
        /// <summary>
        ///
        /// </summary>
        private Form m_frmMain = null;
        /// <summary>
        ///
        /// </summary>
        private Control m_ctrlCanvas = null;
        /// <summary>
        ///
        /// </summary>
        private Rectangle m_rcSysIcon = Rectangle.Empty;
        /// <summary>
        ///
        /// </summary>
        private Rectangle m_rcControlBox = Rectangle.Empty;
        /// <summary>
        /// Hash that stores to each window handle its menu handle.
        /// key		- handle to MdiChild window.
        /// value - handle to MdiChild menu.
        /// </summary>
        protected internal Hashtable m_hashMenus = new Hashtable();
        private VisualStyle m_style = VisualStyle.Default;
        private ImageList imageList1;
        private IContainer components;

        /// <summary>
        /// Tooltip for MDIChild ControlBox when Maximized.
        /// </summary>
        /// <remarks></remarks>
        private ToolTip m_ctrlBoxToolTip = new ToolTip();

        /// <summary>
        /// To toggle the ToolTip appearance for the System Buttons in the Bar when MDI child is maximized.
        /// </summary>
        /// <remarks></remarks>
        private bool m_showSysBtnToolTipOnMDIChildMax = true;

        /// <summary>
        /// Indicating whether the PopupSystemMenu need paint.
        /// </summary>
        private bool m_bNeedPaintPopupSystemMenu = false;
        #endregion

        #region Class properties
        public bool NeedMenuButtons
        {
            get
            {
                // m_mnuActiveChild != IntPtr.Zero ||
                return (this.IsChildMaximized);
            }
        }
        public Form WndActiveChild
        {
            get
            {
                return m_wndActiveChild;
            }
            set
            {
                if (m_wndActiveChild != value)
                {
                    Form prevActiveChild = m_wndActiveChild;

                    m_wndActiveChild = value;

                    if (prevActiveChild == null || m_wndActiveChild == null)
                    {
                        this.OnNeedMenuButtonsChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the System Button ToolTips on the Bar when MDI Child Form is maximized.
        /// </summary>
        /// <value><see langword="true"/> Display System Button ToolTip; otherwise, do not display System Button ToolTip<see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool ShowToolTipOnMDIChildMax
        {
            get { return m_showSysBtnToolTipOnMDIChildMax; }
            set { m_showSysBtnToolTipOnMDIChildMax = value; }
        }

        public bool NeedUpdateHostedForm
        {
            get
            {
                return m_bNeedInvalidate;
            }
            set
            {
                if (value != m_bNeedInvalidate)
                {
                    m_bNeedInvalidate = value;
                }
            }
        }
        //protected internal const int nButtonW = 15;
        [
        DocumentationExclude(),
        Browsable(false)
        ]
        public static int ButtonHeight
        {
            get
            {
                return (SystemInformation.MenuFont.Height + 2);
            }
        }

        /// <summary>
        /// Gets / sets the Visual Style.
        /// </summary>
        public VisualStyle Style
        {
            get
            {
                return m_style;
            }
            set
            {
                m_style = value;
            }
        }

        /// <summary>
        /// Indicates whether the control box should be drawn with the minimize, maximize and close buttons.
        /// </summary>
        /// <value>True to show the control box; false otherwise. Default is true.</value>
        /// <remarks>
        /// <p>If a child form's ControlBox property is set to false, then the control box buttons will be drawn
        /// inactive rather than hidden, in accordance with the .NET menus behavior.</p>
        /// <p>If you want to alter this behavior and instead want the control box to be hidden, set this property
        /// to false.</p>
        /// </remarks>
        public bool ShowControlBox
        {
            get
            {
                return m_bShowControlBox;
            }
            set
            {
                if (m_bShowControlBox != value)
                {
                    m_bShowControlBox = value;

                    if (m_ctrlCanvas != null)
                    {
                        m_ctrlCanvas.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public PopupMenu SystemMenu
        {
            get
            {
                return m_systemMenu;
            }
        }
        /// <summary>
        ///
        /// </summary>
        public Rectangle SystemIconRect
        {
            get
            {
                return m_rcSysIcon;
            }
            set
            {
                int nbuttonht = this.ShowIcon ? MdiSysMenuProvider.ButtonHeight : 0;
                m_rcSysIcon = new Rectangle(value.X, value.Y, nbuttonht, nbuttonht);

                if (this.IsVertical)
                {
                    m_rcSysIcon = Transform(m_rcSysIcon);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Rectangle ControlBoxRect
        {
            get
            {
                return m_rcControlBox;
            }
            set
            {
                // Center the rectangle vertically for 15 pxls and assign it to rcControlBox
                int nbuttonht = MdiSysMenuProvider.ButtonHeight;
                m_rcControlBox = new Rectangle(value.X, (int)Math.Ceiling((float)(value.Height - nbuttonht) / (float)2), value.Width, nbuttonht);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Control Canvas
        {
            get
            {
                return m_ctrlCanvas;
            }
            set
            {
                if (m_ctrlCanvas != value)
                {
                    if (m_ctrlCanvas != null)
                    {
                        m_ctrlCanvas.Resize -= new EventHandler(this.ControlCanvas_Resize);
                    }

                    m_ctrlCanvas = value;

                    if (m_ctrlCanvas != null)
                    {
                        m_ctrlCanvas.Resize += new EventHandler(this.ControlCanvas_Resize);
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Form MainForm
        {
            get
            {
                return m_frmMain;
            }
            set
            {
                if (m_frmMain != value)
                {
                    if (m_frmMain != null)
                    {
                        this.UnInitializeProvider();
                    }

                    m_frmMain = value;

                    if (m_frmMain != null)
                    {
                        this.InitializeProvider(value);
                        this.InitSystemMenu();
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected Rectangle rcMinButton
        {
            get
            {
                Rectangle rect = Rectangle.Empty;

                if (null == m_wndActiveChild || !this.MinimizeButtonHidden)
                {
                    rect = new Rectangle(0, m_rcControlBox.Top,
                        MdiSysMenuProvider.ButtonHeight, m_rcControlBox.Height);

                    if (this.IsCanvasRTL)
                    {
                        rect.X = m_rcControlBox.Left + 2 * MdiSysMenuProvider.ButtonHeight;
                    }
                    else
                    {
                        rect.X = m_rcControlBox.Left;
                    }
                }

                if (this.IsVertical)
                {
                    rect = Transform(rect);
                }

                return rect;
            }
        }

        protected virtual void OnPerformingTransform(TransformEventArgs e)
        {
            if (PerformingTransform != null)
            {
                PerformingTransform(this, e);
            }
        }

        private Rectangle Transform(Rectangle rect)
        {
            TransformEventArgs e = new TransformEventArgs();

            OnPerformingTransform(e);

            Rectangle transformedRect = new Rectangle(rect.Top, rect.Left, rect.Width, rect.Height);

            if (e.TransformOffsetY != 0)
            {
                transformedRect.X = e.TransformOffsetY - transformedRect.Height;
            }

            return transformedRect;
        }

        /// <summary>
        ///
        /// </summary>
        protected Rectangle rcRestoreButton
        {
            get
            {
                Rectangle rect = Rectangle.Empty;

                if (null == m_wndActiveChild || !this.RestoreButtonHidden)
                {
                    int nbuttonht = MdiSysMenuProvider.ButtonHeight;
                    rect = new Rectangle(m_rcControlBox.Left + nbuttonht, m_rcControlBox.Top,
                        nbuttonht, m_rcControlBox.Height);
                }

                if (IsVertical)
                {
                    rect = Transform(rect);
                }

                return rect;
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected Rectangle rcCloseButton
        {
            get
            {
                Rectangle rect = Rectangle.Empty;

                if (null == m_wndActiveChild || !this.CloseButtonHidden)
                {
                    int nbuttonht = MdiSysMenuProvider.ButtonHeight;

                    rect = new Rectangle(0, m_rcControlBox.Top, nbuttonht, m_rcControlBox.Height);

                    if (this.IsCanvasRTL)
                    {
                        rect.X = m_rcControlBox.Left;
                    }
                    else
                    {
                        int displacement = this.MinimizeButtonHidden ? 0 : (2 * nbuttonht + 2);

                        rect.X = m_rcControlBox.Left + displacement;
                    }
                }

                if (IsVertical)
                {
                    rect = Transform(rect);
                }

                return rect;
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected bool IsCanvasRTL
        {
            get
            {
                return (null != m_ctrlCanvas) && (RightToLeft.Yes == m_ctrlCanvas.RightToLeft);
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected internal bool MinimizeButtonHidden
        {
            get
            {
                return (m_wndActiveChild.MinimizeBox == false &&
                    m_wndActiveChild.MaximizeBox == false) ||
                    this.ShowControlBox == false;
            }
        }
        /// <summary>
        ///
        /// </summary>
        protected internal bool RestoreButtonHidden
        {
            get
            {
                return this.MinimizeButtonHidden;
            }
        }
        /// <summary>
        ///
        /// </summary>
        protected internal bool CloseButtonHidden
        {
            get
            {
                return this.ShowControlBox == false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected bool MinimizeButtonDisabled
        {
            get
            {
                return m_wndActiveChild.MinimizeBox == false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected bool RestoreButtonDisabled
        {
            get
            {
                return m_wndActiveChild.MaximizeBox == false;
            }
        }

        public bool ShowIcon
        {
            get
            {
                return m_wndActiveChild.ShowIcon == true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected bool IsChildMaximized
        {
            get
            {
                return m_bChildMaximized;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Throws an event when the NeedMenuButtons property changes.
        /// </summary>
        public event EventHandler NeedMenuButtonsChanged;

        public event TransformEventHandler PerformingTransform;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Static constrcutor - initialize static members: Icons.
        /// </summary>
        static MdiSysMenuProvider()
        {
            _icoMinimize = GetIcon(ICO_MINIMIZE);
            _icoRestore = GetIcon(ICO_RESTORE);
            _icoClose = GetIcon(ICO_CLOSE);
        }

        /// <summary>
        /// Extracts icon from resources.
        /// </summary>
        /// <param name="resourceName">resource name</param>
        /// <returns>Instance of recovered icon</returns>
        protected static Icon GetIcon(string resourceName)
        {
            Type type = typeof(MdiSysMenuProvider);
            Assembly assembly = type.Module.Assembly;
            //string[] resourceNames = assembly.GetManifestResourceNames();
            // cursorNS + cursorName
            Stream stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new ArgumentException("Could not find specified resource. resorce name: " + resourceName);
            }

            return new Icon(stream);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MdiSysMenuProvider()
        {
            this.InitializeComponent();
            this.AttachHook();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (m_ctrlBoxToolTip != null)
                    m_ctrlBoxToolTip.Dispose();

                this.DetachHook();
                m_nwMdiClient.ReleaseHandle();

                m_wndMdiClient = null;
                m_wndActiveChild = null;
                m_frmMain = null;

                if (null != m_hashMenus)
                {
                    m_hashMenus.Clear();
                    m_hashMenus = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            ResourceManager resources = new ResourceManager(typeof(MdiSysMenuProvider));
            this.imageList1 = new ImageList(this.components);
            //
            // imageList1
            //
            this.imageList1.ColorDepth = ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new Size(16, 16);
            this.imageList1.ImageStream = ((ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = Color.Transparent;

        }

        protected void InitializeProvider(Form mainfrm)
        {
            if (mainfrm.IsMdiContainer == false)
            {
                Trace.Assert(false, "Error: Form is not an mdi container.");
                return;
            }
            m_frmMain = mainfrm;

            foreach (Control ctrl in mainfrm.Controls)
            {
                if (ctrl is MdiClient)
                {
                    m_wndMdiClient = ctrl as MdiClient;
                    break;
                }
            }

            m_nwMdiClient = new MdiClientNativeWnd(this);
        }

        public void ActivateProvider(Control canvas)
        {
            if (this.MainForm == null)
            {
                return;
            }

            this.Canvas = canvas;
        }

        public void DeactivateProvider()
        {
            if (this.MainForm == null)
            {
                return;
            }

            this.Canvas = null;
        }

        protected void UnInitializeProvider()
        {
            if (m_frmMain == null)
            {
                return;
            }

            this.DeactivateProvider();

            m_wndMdiClient = null;
            m_frmMain = null;
        }
        #endregion

        #region Class Public Methods
        private bool m_bIsVertical = false;

        public bool IsVertical
        {
            get
            {
                return m_bIsVertical;
            }
            set
            {
                if (value != m_bIsVertical)
                {
                    m_bIsVertical = value;
                }
            }
        }

        public void CorrectLayout()
        {
        }

        public void HandlePaint(Graphics gph)
        {
            if ((m_wndActiveChild == null) || (m_mnuActiveChild == IntPtr.Zero) || (m_wndActiveChild.WindowState != FormWindowState.Maximized))
            {
                return;
            }

            Icon icsystem = (m_rcSysIcon.Height < 32) ?
                (new Icon(m_wndActiveChild.Icon, 16, 16)) :
                (new Icon(m_wndActiveChild.Icon, 32, 32));

            if (ShowIcon)
            {
                if (icsystem.Size == m_rcSysIcon.Size)
                {
                    gph.DrawIconUnstretched(icsystem, m_rcSysIcon);
                }
                else
                {
                    gph.DrawIcon(icsystem, m_rcSysIcon);
                }
            }

            this.DrawMinimizeButton(gph, false);
            this.DrawRestoreButton(gph, false);
            this.DrawCloseButton(gph, false);

            icsystem.Dispose();
        }

        /// <summary>
        /// To show the tooltips for the System Buttons in the Bar when the MDI Child is maximixed.
        /// </summary>
        /// <param name="ctrlButton">Either Close or Minimize or Maximize button representation in HandleMouseMove method.</param>
        /// <remarks></remarks>
        private void ShowControlBoxToolTips(Rectangle ctrlButton)
        {
            if (m_showSysBtnToolTipOnMDIChildMax)
            {
                m_ctrlBoxToolTip.InitialDelay = 2000; //in ms.
                int tpDelay = 750; //in ms.
                string tooltipText = string.Empty;

                if (ctrlButton != null && this.MainForm != null)
                {
                    if (ctrlButton == this.rcMinButton)
                    {
                        tooltipText = "Minimize";
                    }

                    else if (ctrlButton == this.rcCloseButton)
                    {
                        tooltipText = "Close";
                    }

                    else if (ctrlButton == this.rcRestoreButton)
                    {
                        tooltipText = "Restore Down";
                    }

                    m_ctrlBoxToolTip.Show(tooltipText, this.MainForm, new Point(ctrlButton.Location.X + ctrlButton.Width, ctrlButton.Location.Y + (ctrlButton.Height * 4)), tpDelay);
                }
            }
        }

        public void HandleMouseMove(Point ptclient)
        {
            if ((m_wndActiveChild == null) || (m_mnuActiveChild == IntPtr.Zero))
            {
                return;
            }

            if (this.rcMinButton.Contains(ptclient) == true
                && !this.MinimizeButtonDisabled)
            {
                if (m_sbMouseClick != SysButtons.None)
                {
                    if (m_sbMouseClick == SysButtons.Minimize)
                    {
                        this.DrawButtonHilight(m_sbMouseClick, true);
                        m_sbHilight = SysButtons.Minimize;
                    }
                    else
                    {
                        this.DrawButtonHilight(m_sbMouseClick, false);
                        m_sbHilight = SysButtons.None;
                    }
                }
                else
                {
                    if ((m_sbHilight != SysButtons.None) && (m_sbHilight != SysButtons.Minimize))
                    {
                        this.EraseButtonHilight(m_sbHilight);
                        m_sbHilight = SysButtons.None;
                    }
                    if ((m_wndActiveChild.MinimizeBox == true) && (m_sbHilight == SysButtons.None))
                    {
                        m_sbHilight = SysButtons.Minimize;
                        this.DrawButtonHilight(m_sbHilight, false);
                        ShowControlBoxToolTips(this.rcMinButton);
                    }
                }
            }
            else if (this.rcRestoreButton.Contains(ptclient) == true
                && !this.RestoreButtonDisabled)
            {
                if (m_sbMouseClick != SysButtons.None)
                {
                    if (m_sbMouseClick == SysButtons.Restore)
                    {
                        this.DrawButtonHilight(m_sbMouseClick, true);
                        m_sbHilight = SysButtons.Restore;
                    }
                    else
                    {
                        this.DrawButtonHilight(m_sbMouseClick, false);
                        m_sbHilight = SysButtons.None;
                    }
                }
                else
                {
                    if ((m_sbHilight != SysButtons.None) && (m_sbHilight != SysButtons.Restore))
                    {
                        this.EraseButtonHilight(m_sbHilight);
                        m_sbHilight = SysButtons.None;
                    }
                    if (m_sbHilight == SysButtons.None)
                    {
                        m_sbHilight = SysButtons.Restore;
                        this.DrawButtonHilight(m_sbHilight, false);
                        ShowControlBoxToolTips(this.rcRestoreButton);
                    }
                }
            }
            else if (this.rcCloseButton.Contains(ptclient) == true)
            {
                if (m_sbMouseClick != SysButtons.None)
                {
                    if (m_sbMouseClick == SysButtons.Close)
                    {
                        this.DrawButtonHilight(m_sbMouseClick, true);
                        m_sbHilight = SysButtons.Close;
                    }
                    else
                    {
                        this.DrawButtonHilight(m_sbMouseClick, false);
                        m_sbHilight = SysButtons.None;
                    }
                }
                else
                {
                    if ((m_sbHilight != SysButtons.None) && (m_sbHilight != SysButtons.Close))
                    {
                        this.EraseButtonHilight(m_sbHilight);
                        m_sbHilight = SysButtons.None;
                    }
                    if ((m_wndActiveChild.ControlBox == true) && (m_sbHilight == SysButtons.None)
                        && (CloseButtonEnabled))
                    {
                        m_sbHilight = SysButtons.Close;
                        this.DrawButtonHilight(m_sbHilight, false);
                        ShowControlBoxToolTips(this.rcCloseButton);
                    }
                }
            }
            else if (m_sbHilight != SysButtons.None)
            {
                if (m_sbMouseClick != SysButtons.None)
                {
                    this.DrawButtonHilight(m_sbMouseClick, false);
                }
                else
                {
                    this.EraseButtonHilight(m_sbHilight);
                }
                m_sbHilight = SysButtons.None;
            }

        }

        public void HandleMouseDoubleClick(Point ptclient)
        {
            if ((m_wndActiveChild == null) || (m_mnuActiveChild == IntPtr.Zero) ||
                (m_systemMenu.IsShowing()))
            {
                return;
            }

            // Close active child on double click.
            if (m_rcSysIcon.Contains(ptclient) == true)
            {
                this.DestroyMDIActiveChild();
            }
        }

        public void HandleMouseDown(Point ptclient)
        {
            if ((m_wndActiveChild == null) || (m_mnuActiveChild == IntPtr.Zero))
            {
                return;
            }

            m_bNeedPaintPopupSystemMenu = !m_bNeedPaintPopupSystemMenu;

            if (m_sbHilight != SysButtons.None)
            {
                this.DrawButtonHilight(m_sbHilight, true);
                m_sbMouseClick = m_sbHilight;
            }

            if (m_rcSysIcon.Contains(ptclient) == true && m_bNeedPaintPopupSystemMenu)
            {
                Point pt = new Point(this.IsCanvasRTL ? m_rcSysIcon.Right : m_rcSysIcon.Left, m_rcSysIcon.Bottom);
                this.PopupSystemMenu(pt);

                m_bNeedPaintPopupSystemMenu = false;
            }
            else
            {
                m_systemMenu.Hide();
            }
        }

        public void HandleMouseUp(Point ptclient)
        {
            if ((m_wndActiveChild == null) || (m_mnuActiveChild == IntPtr.Zero))
            {
                return;
            }

            if (m_sbMouseClick != SysButtons.None)
            {
                if (m_sbHilight == m_sbMouseClick)
                {
                    this.DrawButtonHilight(m_sbMouseClick, false);
                }
                else
                {
                    this.EraseButtonHilight(m_sbMouseClick);
                }
            }
            if ((m_sbMouseClick != SysButtons.None) && (m_sbMouseClick == m_sbHilight))
            {
                switch (m_sbMouseClick)
                {
                    case SysButtons.Minimize:
                        this.MinimizeMDIActiveChild();
                        break;
                    case SysButtons.Restore:
                        this.RestoreMDIActiveChild();
                        break;
                    case SysButtons.Close:
                        this.DestroyMDIActiveChild();
                        break;
                }
            }
            m_sbMouseClick = SysButtons.None;
        }

        public void HandleMouseLeave()
        {
            if ((m_wndActiveChild == null) || (m_mnuActiveChild == IntPtr.Zero))
            {
                return;
            }

            if (m_sbHilight != SysButtons.None)
            {
                this.EraseButtonHilight(m_sbHilight);
                m_sbHilight = SysButtons.None;
            }
        }
        #endregion

        #region Class drawing
        protected void OnNeedMenuButtonsChanged(EventArgs e)
        {
            if (this.NeedMenuButtonsChanged != null)
            {
                this.NeedMenuButtonsChanged(this, e);
            }
        }

        protected void ControlCanvas_Resize(object sender, EventArgs arg)
        {
            if (m_wndActiveChild != null && m_mnuActiveChild != IntPtr.Zero && null != this.Canvas)
            {
                this.Canvas.Invalidate(true);
            }
        }

        protected void DrawMinimizeButton(Graphics gph, bool btndown)
        {
            if (this.MinimizeButtonHidden)
            {
                return;
            }

            ImageAttributes imgattr = new ImageAttributes();
            ColorMap[] clrmap = new ColorMap[1] { new ColorMap() };

            if (this.MinimizeButtonDisabled)
            {
                clrmap[0].OldColor = SystemColors.ControlText;
                clrmap[0].NewColor = SystemColors.GrayText;
                imgattr.SetRemapTable(clrmap);
            }
            else if (btndown == true) // Map the ControlText syscolor to ActiveCaptionText syscolor.
            {
                clrmap[0].OldColor = SystemColors.ControlText;
                clrmap[0].NewColor = SystemColors.ActiveCaptionText;
                imgattr.SetRemapTable(clrmap);
            }

            Icon icoMinimize = new Icon(_icoMinimize, 18, 18);
            Bitmap bmpMinimize = icoMinimize.ToBitmap();
            Point ptdest = new Point(this.rcMinButton.Left + ((this.rcMinButton.Width - 18) / 2), this.rcMinButton.Top + ((this.rcMinButton.Height - 18) / 2));
            gph.DrawImage(bmpMinimize, new Rectangle(ptdest.X, ptdest.Y, 18, 18), 0, 0, 18, 18, GraphicsUnit.Pixel, imgattr);
            icoMinimize.Dispose();
            bmpMinimize.Dispose();
        }

        protected void DrawRestoreButton(Graphics gph, bool btndown)
        {
            if (this.RestoreButtonHidden)
            {
                return;
            }

            Icon icoRestore = new Icon(_icoRestore, 18, 18);
            Bitmap bmpRestore = icoRestore.ToBitmap();
            Point ptdest = new Point(this.rcRestoreButton.Left + ((this.rcRestoreButton.Width - 18) / 2) + 1, this.rcRestoreButton.Top + ((this.rcRestoreButton.Height - 18) / 2) + 1);
            if (this.RestoreButtonDisabled)
            {
                ImageAttributes imgattr = new ImageAttributes();
                ColorMap[] clrmap = new ColorMap[1] { new ColorMap() };
                clrmap[0].OldColor = SystemColors.ControlText;
                clrmap[0].NewColor = SystemColors.GrayText;
                imgattr.SetRemapTable(clrmap);
                gph.DrawImage(bmpRestore, new Rectangle(ptdest.X, ptdest.Y, 18, 18), 0, 0, 18, 18, GraphicsUnit.Pixel, imgattr);
            }
            else if (btndown == true) // Map the ControlText syscolor to ActiveCaptionText syscolor.
            {
                ImageAttributes imgattr = new ImageAttributes();
                ColorMap[] clrmap = new ColorMap[1] { new ColorMap() };
                clrmap[0].OldColor = SystemColors.ControlText;
                clrmap[0].NewColor = SystemColors.ActiveCaptionText;
                imgattr.SetRemapTable(clrmap);
                gph.DrawImage(bmpRestore, new Rectangle(ptdest.X, ptdest.Y, 18, 18), 0, 0, 18, 18, GraphicsUnit.Pixel, imgattr);
            }
            else
            {
                gph.DrawImage(bmpRestore, ptdest);
            }
            icoRestore.Dispose();
            bmpRestore.Dispose();
        }


        protected bool CloseButtonEnabled
        {
            get
            {
                if ((m_wndActiveChild == null) || !m_wndActiveChild.IsHandleCreated ||
                    (!m_wndActiveChild.ControlBox))
                {
                    return false;
                }

                int styles = NativeMethods.GetClassLong(m_wndActiveChild.Handle, NativeMethods.GCL_STYLE);
                if ((styles & NativeMethods.CS_NOCLOSE) == NativeMethods.CS_NOCLOSE)
                {
                    return false;
                }
                return true;
            }
        }

        protected void DrawCloseButton(Graphics gph, bool btndown)
        {
            if (this.CloseButtonHidden)
            {
                return;
            }

            ImageAttributes imgattr = new ImageAttributes();
            ColorMap[] clrmap = new ColorMap[1] { new ColorMap() };

            if ((m_wndActiveChild.ControlBox == false) || !CloseButtonEnabled)
            {
                clrmap[0].OldColor = SystemColors.ControlText;
                clrmap[0].NewColor = SystemColors.GrayText;
                imgattr.SetRemapTable(clrmap);
            }
            else if (btndown == true) // Map the ControlText syscolor to ActiveCaptionText syscolor.
            {
                clrmap[0].OldColor = SystemColors.ControlText;
                clrmap[0].NewColor = SystemColors.ActiveCaptionText;
                imgattr.SetRemapTable(clrmap);
            }

            Icon icoClose = new Icon(_icoClose, 18, 18);
            Bitmap bmpClose = icoClose.ToBitmap();
            Point ptdest = new Point(this.rcCloseButton.Left + ((this.rcCloseButton.Width - 18) / 2), this.rcCloseButton.Top + ((this.rcCloseButton.Height - 18) / 2));
            gph.DrawImage(bmpClose, new Rectangle(ptdest.X, ptdest.Y, 18, 18), 0, 0, 18, 18, GraphicsUnit.Pixel, imgattr);
            icoClose.Dispose();
            bmpClose.Dispose();
        }

        protected void DrawButtonHilight(SysButtons button, bool btndown)
        {
            Color clrborder = SystemColors.Highlight;
            Color clrshade = GetHighlightButtonColor(this.Style, btndown);
            Graphics gph = m_ctrlCanvas.CreateGraphics();
            Rectangle rcfill = Rectangle.Empty;

            switch (button)
            {
                case SysButtons.Minimize:
                    rcfill = this.rcMinButton;
                    break;
                case SysButtons.Restore:
                    rcfill = this.rcRestoreButton;
                    break;
                case SysButtons.Close:
                    rcfill = this.rcCloseButton;
                    break;
            }

            Pen borderpen = new Pen(clrborder, 1);
            gph.DrawRectangle(borderpen, rcfill);
            borderpen.Dispose();

            SolidBrush fillbrush = new SolidBrush(clrshade);
            gph.FillRectangle(fillbrush, new Rectangle(rcfill.Left + 1, rcfill.Top + 1, rcfill.Width - 1, rcfill.Height - 1));

            switch (button)
            {
                case SysButtons.Minimize:
                    this.DrawMinimizeButton(gph, btndown);
                    break;
                case SysButtons.Restore:
                    this.DrawRestoreButton(gph, btndown);
                    break;
                case SysButtons.Close:
                    this.DrawCloseButton(gph, btndown);
                    break;
            }

            gph.Dispose();
        }

        /// <summary>
        /// Gets color for highlight button.
        /// </summary>
        private Color GetHighlightButtonColor(VisualStyle style, bool bDown)
        {
            Color color = Color.Empty;

            switch (style)
            {
                case VisualStyle.Office2003:
                    {
                        color = (bDown) ? Office2003Colors.PressedSelColor : Office2003Colors.SelColor;
                        break;
                    }
                case VisualStyle.VS2005:
                    {
                        color = (bDown) ? VS2005Colors.BarItemPressLightColor : VS2005Colors.MenuSelectedItemColor;
                        break;
                    }
                default:
                    {
                        color = (bDown) ? Color.FromArgb(148, 148, 184) : Color.FromArgb(173, 173, 209);
                        break;
                    }
            }

            return color;
        }

        protected void EraseButtonHilight(SysButtons button)
        {
            Rectangle rcbutton = Rectangle.Empty;
            switch (button)
            {
                case SysButtons.Minimize:
                    rcbutton = this.rcMinButton;
                    break;
                case SysButtons.Restore:
                    rcbutton = this.rcRestoreButton;
                    break;
                case SysButtons.Close:
                    rcbutton = this.rcCloseButton;
                    break;
            }

            m_ctrlCanvas.Invalidate(Rectangle.Inflate(rcbutton, 1, 1), false);
            m_ctrlCanvas.Update();
        }
        #endregion

        #region MSG Hook
        /// <summary>
        /// Attaches Hook which allows easy catch of: MDI child maximization,
        /// MDI Child restore, MDI Child switching and etc.
        /// </summary>
        protected void AttachHook()
        {
            NativeMethods.HookProc hookProc = new NativeMethods.HookProc(MsgHook);
            m_ProcHandle = GCHandle.Alloc(hookProc);

            m_MsgHookProc = NativeMethods.SetWindowsHookEx(
                (int)Runtime_InteropServices_NativeMethods.WindowsHookCodes.WH_GETMESSAGE,
                hookProc, IntPtr.Zero, NativeMethods.GetCurrentThreadId());
        }

        /// <summary>
        /// Detaches hook on class destroy.
        /// </summary>
        protected void DetachHook()
        {
            NativeMethods.UnhookWindowsHookEx(m_MsgHookProc);
            m_ProcHandle.Free();
            m_MsgHookProc = IntPtr.Zero;
        }

        /// <summary>
        /// Hook function that catches MDI child's actions.
        /// </summary>
        protected IntPtr MsgHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode != NativeMethods.WM_MDIGETACTIVE &&
                nCode != NativeMethods.WM_MDISETMENU)
            {
                IntPtr child;
                bool bOldMax = m_bChildMaximized;
                bool bMax = GetMdiActiveChild(out child);

                m_bChildMaximized = (child != IntPtr.Zero && bMax);

                if (m_bChildMaximized)
                {
                    if (m_hwndChild != child || bOldMax != m_bChildMaximized)
                    {
                        m_hwndChild = child;
                        SetToolbarButtons(FindChildByHandle(m_hwndChild));
                    }
                }

                // if we return child to normal state from Maiximized then
                // remove command bar buttons
                if (bOldMax != m_bChildMaximized &&
                    !m_bChildMaximized)
                {
                    RemoveToolbarButtons();
                }
            }

            return NativeMethods.CallNextHookEx(m_MsgHookProc, nCode, wParam, lParam);
        }

        /// <summary>
        /// Finds by Handle the corresponding MDI Child.
        /// </summary>
        /// <param name="child">Handle to check.</param>
        /// <returns>Null - if nothing found; otherwise MDI Child form</returns>
        protected Form FindChildByHandle(IntPtr child)
        {
            if (child == IntPtr.Zero || m_frmMain == null || !m_frmMain.IsHandleCreated)
            {
                return null;
            }

            foreach (Form form in this.MainForm.MdiChildren)
            {
                if (form.IsHandleCreated && form.Handle == child)
                {
                    return form;
                }
            }

            return null;
        }
        #endregion

        #region Class helper methods
        protected internal void PopupSystemMenu(Point pt)
        {
            m_systemMenu.Hide();
            this.SetSysMenuCaptions();
            m_systemMenu.Show(m_ctrlCanvas, pt);
        }

        protected void ApplyMenuCaption(IntPtr hsysmenu, long commandID, BarItem itemDest)
        {
            String caption = new String('t', 30);
            int len = NativeMethods.GetMenuString(hsysmenu, (uint)commandID, caption, 30, 0);

            // Restore caption
            if (len > 0) //0 for MF_BYCOMMAND
            {
                int shortcutBeginning = caption.IndexOf('\t');

                if (shortcutBeginning != -1)
                {
                    len = shortcutBeginning;
                }

                caption = caption.Substring(0, len);
                itemDest.Text = caption;
            }
        }

        protected void SetSysMenuCaptions()
        {
            if (m_wndActiveChild != null && m_wndActiveChild.Handle != IntPtr.Zero)
            {
                IntPtr hsysmenu = NativeMethods.GetSystemMenu(m_wndActiveChild.Handle, false);
                bool enabled = !this.RestoreButtonHidden && !this.RestoreButtonDisabled;

                // Restore caption
                this.ApplyMenuCaption(hsysmenu, 0xF120, m_systemMenu.ParentBarItem.Items[0]);
                m_systemMenu.ParentBarItem.Items[0].Enabled = enabled;

                // Minimize caption
                this.ApplyMenuCaption(hsysmenu, 0xF020, m_systemMenu.ParentBarItem.Items[1]);
                m_systemMenu.ParentBarItem.Items[1].Enabled = enabled;

                // Close caption
                this.ApplyMenuCaption(hsysmenu, 0xF060, m_systemMenu.ParentBarItem.Items[2]);
                m_systemMenu.ParentBarItem.Items[2].Enabled = this.CloseButtonEnabled &&
                    !this.CloseButtonHidden;

                // Next caption
                this.ApplyMenuCaption(hsysmenu, 0xF040, m_systemMenu.ParentBarItem.Items[3]);

                m_systemMenu.ActiveBounds = m_rcSysIcon;
            }
        }

        protected virtual void InitSystemMenu()
        {
            m_systemMenu = new MdiPopupMenu();
            m_systemMenu.ActiveBounds = this.m_rcSysIcon;
            m_systemMenu.ParentBarItem = new ParentBarItem();

            // The captions will be replaced by localized strings in ApplyMenuCaption.
            m_systemMenu.ParentBarItem.Items.Add(new BarItem("&Restore", new EventHandler(this.SystemMenuClicked)));
            m_systemMenu.ParentBarItem.Items.Add(new BarItem("Mi&nimize", new EventHandler(this.SystemMenuClicked)));
            m_systemMenu.ParentBarItem.Items.Add(new BarItem("&Close", new EventHandler(this.SystemMenuClicked), Shortcut.CtrlF4));
            m_systemMenu.ParentBarItem.Items.Add(new BarItem("Next", new EventHandler(this.SystemMenuClicked), Shortcut.CtrlF6));

            m_systemMenu.ParentBarItem.BeginGroupAt(m_systemMenu.ParentBarItem.Items[2]);
            m_systemMenu.ParentBarItem.BeginGroupAt(m_systemMenu.ParentBarItem.Items[3]);

            m_systemMenu.ParentBarItem.Items[0].ImageList = this.imageList1;
            m_systemMenu.ParentBarItem.Items[0].ImageIndex = 2;
            m_systemMenu.ParentBarItem.Items[1].ImageList = this.imageList1;
            m_systemMenu.ParentBarItem.Items[1].ImageIndex = 1;
            m_systemMenu.ParentBarItem.Items[2].ImageList = this.imageList1;
            m_systemMenu.ParentBarItem.Items[2].ImageIndex = 0;
        }

        protected virtual void SystemMenuClicked(object sender, EventArgs e)
        {
            int index = m_systemMenu.ParentBarItem.Items.IndexOf(sender);

            switch (index)
            {
                case 0:
                    this.RestoreMDIActiveChild();
                    break;

                case 1:
                    this.MinimizeMDIActiveChild();
                    break;

                case 2:
                    this.DestroyMDIActiveChild();
                    break;

                case 3:
                    this.NextWindowMDIActiveChild(false);
                    break;
            }
        }

        protected void MinimizeMDIActiveChild()
        {
            if (m_frmMain == null || m_frmMain.ActiveMdiChild == null)
            {
                return;
            }
            //this.frmMain.ActiveMdiChild.WindowState = FormWindowState.Minimized;
            // Users prefer to receive the messages - in thier WndProc override - instead of us calling the Close method.
            // This way they can distinguish if form closing was initiated programmatically or by the user.
            //this.frmMain.ActiveMdiChild.Close();
            NativeMethods.SendMessage(m_frmMain.ActiveMdiChild.Handle,
                NativeMethods.WM_SYSCOMMAND,
                (IntPtr)NativeMethods.SC_MINIMIZE,
                IntPtr.Zero);
        }

        protected void NextWindowMDIActiveChild(bool bprevious)
        {
            if (m_frmMain == null ||
                m_frmMain.ActiveMdiChild == null ||
                m_wndMdiClient == null)
            {
                return;
            }

            NativeMethods.SendMessage(m_wndMdiClient.Handle,
                NativeMethods.WM_MDINEXT,
                m_frmMain.ActiveMdiChild.Handle,
                (bprevious) ? (IntPtr)1 : IntPtr.Zero);
        }

        protected void MDIChildActivate(IntPtr wndHandle)
        {
            if (m_wndMdiClient == null)
            {
                return;
            }

            NativeMethods.SendMessage(m_wndMdiClient.Handle,
                NativeMethods.WM_MDIACTIVATE,
                wndHandle, IntPtr.Zero);
        }

        protected void DestroyMDIActiveChild()
        {
            if (m_frmMain == null || m_frmMain.ActiveMdiChild == null)
            {
                return;
            }

            // Users prefer to receive the messages - in thier WndProc override - instead of us calling the Close method.
            // This way they can distinguish if form closing was initiated programmatically or by the user.
            //this.frmMain.ActiveMdiChild.Close();
            NativeMethods.PostMessage(m_frmMain.ActiveMdiChild.Handle,
                NativeMethods.WM_SYSCOMMAND, (IntPtr)NativeMethods.SC_CLOSE, IntPtr.Zero);
        }

        protected void RestoreMDIActiveChild()
        {
            if (m_frmMain == null ||
                m_frmMain.ActiveMdiChild == null ||
                m_wndMdiClient == null)
            {
                return;
            }

            NativeMethods.SendMessage(m_wndMdiClient.Handle,
                NativeMethods.WM_MDIRESTORE,
                m_frmMain.ActiveMdiChild.Handle, IntPtr.Zero);

            foreach (Control ctrl in m_frmMain.Controls)
            {
                if (ctrl is RibbonControlAdv)
                {
                    RibbonControlAdv ribbon = (RibbonControlAdv)ctrl;
                    ribbon.HeaderInternal.UpdateSystemButtons();
                    break;
                }
            }           
        }
        #endregion

        #region Class Utility methods
        protected internal void InvalidateHostedForm()
        {
            if (NeedUpdateHostedForm && m_frmMain != null)
            {
                m_frmMain.Invalidate(true);
            }
        }
        /// <summary>
        /// Locks repainting of MainForm. After Locking window stops redrawing
        /// until UnLock action done. You can safely call BeginUpdate several
        /// times (but always must be corresponding code which will call EndUpdate),
        /// because we have internal counter which accumulates calls. Several
        /// calls simply increase counter for UnLock method EndUpdate.
        /// </summary>
        protected internal void BeginUpdate()
        {
            if (!NeedUpdateHostedForm || m_frmMain == null || m_frmMain.Handle == IntPtr.Zero)
            {
                return;
            }

            // NOTE: if we called twice and Locked set before then start to
            // calculate how much UnLock required for proper window redrawing
            NativeMethodsHelper.SuspendRedrawWindow(m_frmMain.Handle);
        }

        /// <summary>
        /// UnLocks repainting of MainForm. After UnLocking form Invalidation
        /// will work. We have internal counter that is why each call of BeginUpdate
        /// must have corresponding EndUpdate method call, otherwise window redrawing
        /// will be in locked state till end of time.
        /// </summary>
        protected internal void EndUpdate()
        {
            if (!NeedUpdateHostedForm || m_frmMain == null || m_frmMain.Handle == IntPtr.Zero)
            {
                return;
            }

            // Unlock window if counter is set to Zero
            NativeMethodsHelper.ResumeRedrawWindow(m_frmMain.Handle, false);
        }

        /// <summary>
        /// Force window invalidation. Window will redraw itself even if
        /// repainting is locked.
        /// </summary>
        protected void ForceRedrawCompletly()
        {
            if (m_frmMain == null || m_frmMain.Handle == IntPtr.Zero)
            {
                return;
            }

            // 0687h == 1671
            const int flags =
                NativeMethods.RDW_INVALIDATE |
                NativeMethods.RDW_INTERNALPAINT |
                NativeMethods.RDW_ERASE |
                NativeMethods.RDW_ALLCHILDREN |
                NativeMethods.RDW_FRAME |
                NativeMethods.RDW_ERASENOW |
                NativeMethods.RDW_UPDATENOW;

            IntPtr handle = m_frmMain.Handle;
            NativeMethodsHelper.RedrawWindow(handle, flags);
        }

        /// <summary>
        /// Invalidates only part of form - it's Caption NonClient area.
        /// </summary>
        protected void ForceRedrawCaptionOnly()
        {
            const int flags = NativeMethods.RDW_INVALIDATE |
                NativeMethods.RDW_FRAME |
                NativeMethods.RDW_UPDATENOW;

            NativeMethodsHelper.RedrawWindow(this.MainForm.Handle, flags);
        }

        /// <summary>
        /// Updates toolbar and set variables which indicate toolbar to show
        /// control box buttons and sys menu icon.
        /// </summary>
        /// <param name="activeChild">Active child reference.</param>
        protected void SetToolbarButtons(Form activeChild)
        {
            if (activeChild != null)
            {
                // try to extract mdi child menu if mdiClient can not help us in this
                if (m_mnuActiveChild == IntPtr.Zero)
                {
                    m_mnuActiveChild = NativeMethods.GetMenu(activeChild.Handle);
                }

                // store mdi child menu handle if exists
                if (m_mnuActiveChild != IntPtr.Zero)
                {
                    m_hashMenus[activeChild.Handle] = m_mnuActiveChild;
                }

                // NOTE: recover child menu. someone kill menu and I cannot find where.
                // here is workaround which try easily fix problem without deep digging
                // in other libraries. If problems appears againe good fix will require
                // near 2-3 weeks of time.
                if (m_mnuActiveChild == IntPtr.Zero && m_hashMenus.Count > 0)
                {
                    if (m_hashMenus[activeChild.Handle] != null)
                    {
                        m_mnuActiveChild = (IntPtr)m_hashMenus[activeChild.Handle];

                        if (m_mnuActiveChild != IntPtr.Zero)
                        {
                            NativeMethods.SetMenu(activeChild.Handle, m_mnuActiveChild);
                        }
                    }
                }
            }

            // WARNING: do not change order of variables assignment, otherwise
            // toolbar will not detect properly how to show control box
            this.ShowControlBox = true;
            this.WndActiveChild = activeChild;

            if (null != this.Canvas)
            {
                this.Canvas.Invalidate(true);
                if (this.Canvas is BarControlInternal)
                {
                    (this.Canvas as BarControlInternal).Renderer.UpdateRenderers();
                }
            }
        }
        /// <summary>
        /// Resets variables to state in which toolbar will be drawn with only barItems and
        /// without system menu and caption buttons.
        /// </summary>
        protected void RemoveToolbarButtons()
        {
            // WARNING: do not change order of variables assignment, otherwise
            // toolbar will not detect properly how to show control box
            m_mnuActiveChild = IntPtr.Zero;
            this.WndActiveChild = null;
            this.ShowControlBox = false;

            if (null != this.Canvas)
            {
                this.Canvas.Invalidate(true);
            }
        }

        /// <summary>
        /// Returns the handle of active MDI Child.
        /// </summary>
        /// <param name="handle">Handle of active MDI Childs.</param>
        /// <returns>True if child is maximized; false otherwise.</returns>
        protected bool GetMdiActiveChild(out IntPtr handle)
        {
            bool bMaximized = false;
            handle = IntPtr.Zero;

            if (m_wndMdiClient != null && m_wndMdiClient.IsHandleCreated && !m_wndMdiClient.IsDisposed)
            {
                handle = NativeMethods.SendMessage(m_wndMdiClient.Handle,
                    NativeMethods.WM_MDIGETACTIVE, IntPtr.Zero, out bMaximized);
            }

            return bMaximized;
        }
        #endregion
    }

    public delegate void TransformEventHandler(object sender, TransformEventArgs e);

    public class TransformEventArgs : EventArgs
    {
        private int m_transformOffsetY = 0;

        private static TransformEventArgs _empty = null;

        public int TransformOffsetY
        {
            get
            {
                return m_transformOffsetY;
            }
            set
            {
                if (value != m_transformOffsetY)
                {
                    m_transformOffsetY = value;
                }
            }
        }

        public static new TransformEventArgs Empty
        {
            get
            {
                return _empty;
            }
        }

        static TransformEventArgs()
        {
            _empty = new TransformEventArgs();
        }

        public TransformEventArgs()
        {
        }

        public TransformEventArgs(int offfsetY)
        {
            m_transformOffsetY = offfsetY;
        }
    }

    public class MdiClientNativeWnd : NativeWindowSubclass
    {
        #region Class members
        /// <summary>
        ///
        /// </summary>
        private MdiSysMenuProvider m_owner;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor. Set parent reference and Attach
        /// NativeWindow subclassing to MDIClient.
        /// </summary>
        /// <param name="owner">Reference on owner.</param>
        public MdiClientNativeWnd(MdiSysMenuProvider owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            m_owner = owner;

            this.AssignHandleCustom(owner.m_wndMdiClient);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Subclassing WndProc for MDIClient window. We skip WM_MDISETMENU
        /// and WM_MDIREFRESHMENU messages for disabling MDI menus. On
        /// WM_MDIACTIVATE and WM_MDINEXT we are locking redrawing to disable
        /// flicking of child forms.
        /// </summary>
        /// <param name="m">Message class</param>
        protected override void WndProc(ref Message m)
        {
            BarManager barMan = MainFrameBarManager.GetManagerFromForm(m_owner.MainForm);

            if (barMan != null)
            {
                if (m.Msg == NativeMethods.WM_MDISETMENU)
                {
                    m_owner.m_mnuActiveChild = m.WParam;
                    return;
                }
                else if (m.Msg == NativeMethods.WM_MDIREFRESHMENU)
                {
                    return;
                }
                else if (m.Msg == NativeMethods.WM_MDIDESTROY)
                {
                    // clean our workaround hash
                    m_owner.m_hashMenus.Remove(m.WParam);
                }
                else if (m.Msg == NativeMethods.WM_KILLFOCUS)
                {
                    if (m_owner.MainForm.Handle == m.WParam)
                    {
                        bool bMax = false;
                        IntPtr child = NativeMethods.SendMessage(m_owner.m_wndMdiClient.Handle, NativeMethods.WM_MDIGETACTIVE, IntPtr.Zero, out bMax);
                        Form childForm = Control.FromHandle(child) as Form;
                        if (!bMax && childForm != null && childForm.WindowState == FormWindowState.Minimized)
                        {
                            m_owner.m_wndMdiClient.Focus();
                        }
                    }
                }
            }

            base.WndProc(ref m);
        }
        #endregion
    }

    [DocumentationExclude()]
    public class MdiSysMenuManager
    {
        private static Hashtable htFormsVsProvider = new Hashtable();

        public static MdiSysMenuProvider GetProviderForForm(Form form)
        {
            if (form.IsMdiContainer)
            {
                if (htFormsVsProvider[form] == null)
                {
                    MdiSysMenuProvider provider = new MdiSysMenuProvider();
                    provider.MainForm = form;
                    form.Closed += new EventHandler(MdiContainer_Closed);
                    htFormsVsProvider[form] = provider;
                }
                return htFormsVsProvider[form] as MdiSysMenuProvider;
            }
            return null;
        }

        private static void MdiContainer_Closed(object sender, EventArgs e)
        {
            Form form = sender as Form;
            form.Closed -= new EventHandler(MdiContainer_Closed);
            if (htFormsVsProvider[form] != null)
            {
                MdiSysMenuProvider provider = htFormsVsProvider[form] as MdiSysMenuProvider;
                provider.MainForm = null;
                htFormsVsProvider.Remove(form);
                provider.Dispose();
            }
        }
    }

    [ToolboxItem(false)]
    public class MdiPopupMenu : PopupMenu
    {
        #region Class Members

        private Rectangle m_activeBounds = Rectangle.Empty;

        #endregion

        #region Class Properties

        public Rectangle ActiveBounds
        {
            get
            {
                return m_activeBounds;
            }
            set
            {
                m_activeBounds = value;
            }
        }

        #endregion

        #region Class Overrides

        public override bool IsRelatedControl(Control control, bool askParent)
        {
            if (control is BarControlInternal)
            {
                Point pt = control.PointToClient(Control.MousePosition);

                if (this.ActiveBounds.Contains(pt) == true)
                {
                    return true;
                }
            }

            return base.IsRelatedControl(control, askParent);
        }

        #endregion
    }
}
