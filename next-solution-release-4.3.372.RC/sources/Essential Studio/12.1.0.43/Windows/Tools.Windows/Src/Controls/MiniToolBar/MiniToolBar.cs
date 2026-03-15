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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
    [Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.MiniToolBarDesigner))]
    [Description("Represents an Office 2007 Style MiniBar.")]
    [ToolboxBitmap(typeof(MiniToolBar), "ToolboxIcons.MiniToolBar.bmp")]
    public class MiniToolBar : ToolStripDropDown, IMessageFilter
    {
        #region Constructors
        static MiniToolBar()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(MiniToolBar));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            m_layout = new MiniToolBarLayout();

            m_dRenderes = new Dictionary<ToolStripEx.ColorScheme, Office12ToolStripRenderer>();

            m_dRenderes[ToolStripEx.ColorScheme.Managed] = new Office12ToolStripRenderer(Office12ColorTable.ManagedColors);
            m_dRenderes[ToolStripEx.ColorScheme.Silver] = new Office12ToolStripRenderer();
            m_dRenderes[ToolStripEx.ColorScheme.Blue] = new Office12ToolStripRenderer(new OfficeBlue());
            m_dRenderes[ToolStripEx.ColorScheme.Black] = new Office12ToolStripRenderer(new OfficeBlack());
        }

        public MiniToolBar()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(MiniToolBar));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.AllowTransparency = true;
            this.DropShadowEnabled = false;

            m_pHook = new WindowsAPI.HookProc(MouseProc);

            UpdateRenderer();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color scheme used for painting
        /// </summary>
        [Description("Gets or set the color scheme used for painting")]
        [DefaultValue(typeof(ToolStripEx.ColorScheme), "Managed")]
        public ToolStripEx.ColorScheme ColorScheme
        {
            get
            {
                return m_colorScheme;
            }
            set
            {
                if (m_colorScheme != value)
                {
                    m_colorScheme = value;
                    UpdateRenderer();
                }
            }
        }

        /// <summary>
        /// Gets a cached instance of the control's layout engine.
        /// </summary>
        public override LayoutEngine LayoutEngine
        {
            get
            {
                return m_layout;
            }
        }

        /// <summary>
        /// Gets or sets the hides unused property of the base class  
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ToolStripLayoutStyle LayoutStyle
        {
            get { return base.LayoutStyle; }
            set { base.LayoutStyle = value; }
        }

        /// <summary>
        /// Gets or sets the opacity of minitoolbar
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new double Opacity
        {
            get { return base.Opacity; }
            set { base.Opacity = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether hides unused property of the base class
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool DropShadowEnabled
        {
            get 
            { 
                return base.DropShadowEnabled;
            }
            set 
            {
                base.DropShadowEnabled = value; 
            }
        }

        /// <summary>
        /// Gets or sets the control which is associated with MiniToolBar
        /// </summary>
        [Description("Gets or sets the control which is associated with MiniToolBar")]
        [TypeConverter(typeof(Design.AssociatedControlTypeConverter))]
        [DefaultValue((string)null)]
        public Control AssociatedControl
        {
            get
            {
                return m_associatedControl;
            }
            set
            {
                if (m_associatedControl != value)
                {
                    if (null != m_associatedControl)
                    {
                        m_associatedControl.MouseCaptureChanged -= new EventHandler(AssociatedControl_MouseCaptureChanged);
                        Application.RemoveMessageFilter(this);
                    }

                    m_associatedControl = value;

                    if (null != m_associatedControl)
                    {
                        m_associatedControl.MouseCaptureChanged += new EventHandler(AssociatedControl_MouseCaptureChanged);
                        Application.AddMessageFilter(this);
                    }
                }
            }
        }

        private IntPtr Module
        {
            get
            {
                return Marshal.GetHINSTANCE(this.GetType().Module);
            }
        }

        /// <summary>
        /// Gets the transparency changes' range
        /// </summary>
        private float Range
        {
            get
            {
                if (m_range < 0 && this.IsHandleCreated)
                {
                    using (Graphics g = Graphics.FromHwnd(this.Handle))
                    {
                        // pixels per inch
                        m_range = g.DpiX;
                    }
                }
                return m_range;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Retrieves the size of a rectangular area into which a control can be fitted.
        /// </summary>
        /// <param name="proposedSize">Proposed Size</param>
        /// <returns>Returns the size of a rectangular area into which a control can be fitted.</returns>
        [Description("Retrieves the size of a rectangular area into which a control can be fitted.")]
        public override Size GetPreferredSize(Size proposedSize)
        {
            if (this.Items.Count > 0)
            {
                if (m_preferredSize.IsEmpty)
                {
                    foreach (ToolStripItem item in this.Items)
                    {
                        Padding margins = item.Margin;
                        Size szItem = GetItemSize(item);

                        szItem.Width += margins.Horizontal;
                        szItem.Height += margins.Vertical;

                        if (m_preferredSize.Height < szItem.Height)
                        {
                            m_preferredSize.Height = szItem.Height;
                        }
                        m_preferredSize.Width += szItem.Width;
                    }
                    m_preferredSize.Width += this.Padding.Horizontal;
                    m_preferredSize.Height += this.Padding.Vertical;
                }
                return m_preferredSize;
            }
            return new Size(24, 24);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            m_preferredSize = Size.Empty;

            base.OnLayout(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            HandledEventArgs args = new HandledEventArgs(false);

            base.OnVisibleChanged(args);

            if (!args.Handled)
            {
                if (this.Visible)
                {
                    m_distance = -1;
                    this.Opacity = 1.0;

                    m_hHook = WindowsAPI.SetWindowsHookEx((int)WindowsHookCodes.WH_MOUSE_LL, m_pHook, this.Module, 0);
                }
                else
                {
                    WindowsAPI.UnhookWindowsHookEx(m_hHook);
                }
            }
        }
        public void ShowMiniToolbarOnSelection(bool show)
        {
            this.Visible = show;
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            if (changeopacity)
                this.Opacity = 0.3f;
            
            base.AllowTransparency = true;
            base.DropShadowEnabled = false;
            base.OnOpening(e);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            bool bVisible = this.Visible;

            if (bVisible)
            {
                this.Visible = false;
            }

            base.OnRightToLeftChanged(e);

            if (bVisible)
            {
                this.Visible = true;
            }
        }

        #endregion

        #region Implementation
        
        private IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if ((int)wParam == (int)Msg.WM_MOUSEMOVE)
            {
                MOUSEHOOKSTRUCT mhst = (MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MOUSEHOOKSTRUCT));
                OnMouseMove(ref mhst.pt);       
            }
            return WindowsAPI.CallNextHookEx(m_hHook, nCode, wParam, lParam);
        }

        private void UpdateRenderer()
        {
            switch (m_colorScheme)
            {
                case ToolStripEx.ColorScheme.Default:
                    this.Renderer = m_dRenderes[ToolStripEx.ColorScheme.Managed];
                    break;
                default:
                    this.Renderer = m_dRenderes[m_colorScheme];
                    break;
            }
            Invalidate();
        }

        private Size GetItemSize(ToolStripItem item)
        {
            if (item.Available)
            {
                return item.AutoSize ? item.GetPreferredSize(Size.Empty) : item.Size;
            }
            return Size.Empty;
        }
        private void OnMouseMove(ref POINT pt)
        {
            if (this.IsHandleCreated && this.Handle == GetTopWindow())
            {
                Rectangle rc = this.Bounds;

                int dx = 0;

                if (pt.x < rc.X)
                {
                    dx = rc.X - pt.x;
                }
                else if (pt.x > rc.Right)
                {
                    dx = pt.x - rc.Right;
                }

                int dy = 0;

                if (pt.y < rc.Y)
                {
                    dy = rc.Y - pt.y;
                }
                else if (pt.y > rc.Bottom)
                {
                    dy = pt.y - rc.Bottom;
                }

                int distance = Math.Max(dx, dy);
                if (m_associatedControl!= null && m_associatedControl.ContextMenuStrip != null && m_associatedControl.ContextMenuStrip.Visible)
                {
                    this.Opacity = 1.0;
                }
                else
                {                   
                        if (m_distance >= 0 && distance > m_distance)
                        {
                            int range = distance - m_distance;

                            if (range < this.Range * 2)
                            {
                                this.Opacity = 0.4f - range / this.Range;
                            }
                            else
                                Hide();
                        }

                        else
                            m_distance = distance;
                   }
                if(this.Bounds.Contains(pt))
                {
                    this.Opacity = 1.0f;
                }
                
            }
        }

            private IntPtr GetTopWindow()
        {
            int thread = WindowsAPI.GetCurrentThreadId();
            IntPtr hWnd = WindowsAPI.GetTopWindow(IntPtr.Zero);

            while (WindowsAPI.IsWindow(hWnd) != 0 && (!WindowsAPI.IsWindowVisible(hWnd) || thread != GetWindowThreadProcessId(hWnd, IntPtr.Zero)))
            {
                hWnd = WindowsAPI.GetWindow(hWnd, GetWindowCmd.GW_HWNDNEXT);
            }
            return hWnd;
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        #endregion

        #region IMessageFilter Members
        private void AssociatedControl_MouseCaptureChanged(object sender, EventArgs args)
        {
            m_bCaptured = false;
        }
        private bool m_bCaptured = false;

           bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            switch ((Msg)m.Msg)
            {
                case Msg.WM_RBUTTONUP:
                    OnWmRButtonUp(ref m);
                    break;
                case Msg.WM_RBUTTONDOWN:
                    OnWmRButtonDown(ref m);
                    break;
                case Msg.WM_LBUTTONDOWN:
                    OnWmLButtonDown(ref m);
                    break;
            }
            return false;
        }

        private void OnWmLButtonDown(ref Message m)
        {
             if (m_associatedControl != null && m_associatedControl.IsHandleCreated && m_associatedControl.Handle == m.HWnd && m_associatedControl.ContextMenuStrip!=null)
             {                
                 m_associatedControl.ContextMenuStrip.Visible = false;
             }              
         }
        private void OnWmRButtonDown(ref Message m)
        {
            if (m_associatedControl != null)
            {
                if (m_associatedControl.ContextMenuStrip != null)
                {
                    m_associatedControl.ContextMenuStrip.Visible = false;                
                }
            }
            if (m_associatedControl != null && m_associatedControl.IsHandleCreated && m_associatedControl.Handle == m.HWnd)
            {
                m_bCaptured = true;
            }
        }

        private void OnWmRButtonUp(ref Message m)
        {
            if (m_bCaptured)
            {
                m_bCaptured = false;
                if (m_associatedControl != null && m_associatedControl.IsHandleCreated && m_associatedControl.Handle == m.HWnd)
                {
                    POINT pt = new POINT();
                    pt.x = WindowsAPI.LOW_ORDER((int)m.LParam);
                    pt.y = WindowsAPI.HIGH_ORDER((int)m.LParam);

                    if (m_associatedControl.ClientRectangle.Contains(pt.x, pt.y))
                    {
                        Size szToolbar = this.AutoSize ? this.GetPreferredSize(Size.Empty) : this.Size;
                        pt.y -= szToolbar.Height;

                        WindowsAPI.MapWindowPoints(m.HWnd, IntPtr.Zero, ref pt, 1);

                        Screen scr = Screen.FromPoint(new Point(pt.x, pt.y));
                        if (scr != null)
                        {
                            Rectangle rcScr = scr.Bounds;

                            if (pt.x + szToolbar.Width > rcScr.Right)
                            {
                                pt.x = rcScr.Right - szToolbar.Width;
                            }
                            if (pt.y < rcScr.Top)
                            {
                                pt.y = rcScr.Top;
                            }
                        }
                        
                        //if context menu appears with minitoolbar,
                        //set spacing as 15 px, 
                        //set opacity as 1.0
                        if (m_associatedControl.ContextMenuStrip != null)
                        {
                         
                            m_associatedControl.ContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);
                            this.Visible = true;               
                          
                            this.Show(pt.x, pt.y-15);
                            this.Opacity = 1.0f;                    
                         
                        }                                                  
                        else
                            this.Show(pt.x, pt.y);                       
                    }
                   
                }
            }
        }
                   
        void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            //Position contextmenu and minitoolbar  
            if (m_associatedControl.Bounds.Height < m_associatedControl.ContextMenuStrip.Bounds.Height + m_associatedControl.ContextMenuStrip.Bounds.Y)
            {
                m_associatedControl.ContextMenuStrip.Show(new Point(m_associatedControl.ContextMenuStrip.Bounds.X, Cursor.Position.Y - m_associatedControl.ContextMenuStrip.Bounds.Height));
                changeopacity = false;
                if ((m_associatedControl.ContextMenuStrip.Bounds.Y + 15 + this.Bounds.Height) > m_associatedControl.Bounds.Height)
                    this.Show(new Point(this.Bounds.X, Cursor.Position.Y - m_associatedControl.ContextMenuStrip.Bounds.Height-this.Bounds.Height - 15));
                else 
                    this.Show(new Point(this.Bounds.X, Cursor.Position.Y + 15));
                changeopacity = true;
            }
        
            //Set width of contextmenu equal to minitoolbar[as per office 2007 guidelines]
            m_associatedControl.ContextMenuStrip.AutoSize = false;
            m_associatedControl.ContextMenuStrip.Width = this.Width;

            for(int i = 0;i<m_associatedControl.ContextMenuStrip.Items.Count;i++)
            {
              m_associatedControl.ContextMenuStrip.Items[i].AutoSize =false;
              m_associatedControl.ContextMenuStrip.Items[i].Width = this.Width;
            }
          
        }
        #endregion

        #region Fields
        private ToolStripEx.ColorScheme m_colorScheme = ToolStripEx.ColorScheme.Managed;
        private Size m_preferredSize = Size.Empty;

        /// <summary>
        /// Current distance for 100% opacity ("-1" - undefined)
        /// </summary>
        private int m_distance = -1;

        /// <summary>
        /// Transparency changes' range
        /// </summary>
        private float m_range = -1;

        private WindowsAPI.HookProc m_pHook;
        private IntPtr m_hHook;

        private Control m_associatedControl = null;
        private bool changeopacity = true;
       private static MiniToolBarLayout m_layout;
       private static Dictionary<ToolStripEx.ColorScheme, Office12ToolStripRenderer> m_dRenderes;
        #endregion

        #region *** MiniToolBarLayout
        
       public class MiniToolBarLayout : LayoutEngine
        {
            #region Constructors
            public MiniToolBarLayout()
            {
            }
            #endregion

            #region Overrides
            public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
            {
                MiniToolBar toolBar = container as MiniToolBar;

                Rectangle rcDisplay = toolBar.DisplayRectangle;
                int displayHeight = rcDisplay.Height;

                RightToLeft rtl = toolBar.RightToLeft;
                Point pt = new Point(0, rcDisplay.Y);

                foreach (ToolStripItem item in toolBar.Items)
                {
                    if (item.Available)
                    {
                        Padding margins = item.Margin;

                        Size szItem = toolBar.GetItemSize(item);
                        szItem.Height = displayHeight - margins.Vertical;

                        item.Size = szItem;
                        SetItemLocationRTL(toolBar, item, rtl, pt);

                        pt.X += item.Width + margins.Horizontal;
                    }
                }
                return toolBar.AutoSize;
            }
            #endregion

            #region Implementation
            /// <summary> Set item location depending on RTL property. </summary>
            /// <param name="toolbar"> MiniToolbar on which items lay out.</param>
            /// <param name="item"> Item which must be positioned. </param>
            /// <param name="rtl"> MiniToolbar RightToLeft property. </param>
            /// <param name="location"> Position for item on MiniToolbar. </param>
            private void SetItemLocationRTL(MiniToolBar toolbar, ToolStripItem item, RightToLeft rtl, Point location)
            {
                if (rtl == RightToLeft.Yes)
                {
                    location.X = toolbar.DisplayRectangle.Right - location.X - item.Width - item.Margin.Right;
                }
                else
                {
                    location.X = toolbar.DisplayRectangle.X + location.X + item.Margin.Left;
                }

                location.Y = location.Y + item.Margin.Top;

                toolbar.SetItemLocation(item, location);
            }
            #endregion
        }
        #endregion
    }
}
#endif
