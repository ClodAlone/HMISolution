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
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Tools
{
    #region SuperAccelerator
    [ProvideProperty("Accelerator", typeof(ToolStripItem))]
    [ProvideProperty("CollapsedDropDownAccelerator", typeof(ToolStripEx))]
    [ProvideProperty("MenuButtonAccelerator", typeof(RibbonControlAdv))]
    [TypeConverter(typeof(Design.SuperAcceleratorTypeConverter))]
    [ToolboxBitmap(typeof(SuperAccelerator), "ToolboxIcons.SuperAccelerator.bmp")]
    [Description("Provides options to set Office 2007 Style Key Tips.")]
    public class SuperAccelerator : Component, IMessageFilter, IExtenderProvider
    {
        #region *** Accelerator
        class Accelerator : Control
        {
            #region Constants
            const SetWindowPosFlags SWP_SHOW =
                SetWindowPosFlags.SWP_SHOWWINDOW |
                SetWindowPosFlags.SWP_NOACTIVATE;

            const int VERTEX_RADIUS = 2;
            #endregion

            #region Constructors
            /// <summary>
            /// 
            /// </summary>
            static Accelerator()
            {
                m_graphics = Graphics.FromImage(new Bitmap(1, 1));
                m_blend = new Blend();
                m_blend.Positions = new float[] { 0.0F, 0.6F, 1.0F };
                m_blend.Factors = new float[] { 0.0F, 0.5F, 1.0F };

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="component"></param>
            /// <param name="szAccelerator"></param>
            public Accelerator(Component component, string szAccelerator)
                : base(szAccelerator)
            {
                m_Component = component;
            }
            #endregion
            const int scroll_button_width= 13;

            #region Properties
            /// <summary>
            /// Returns true if linked component is disabled
            /// </summary>
            public bool Disabled
            {
                get
                {
                    if (m_Component is BackStageButton || m_Component is BackStageTab || m_Component is ButtonAdv)
                    {
                        return false;
                    }
                 
                    ToolStripItem item = m_Component as ToolStripItem;

                    return item == null || !item.Enabled;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public int RefCount
            {
                get
                {
                    return m_refCount;
                }
                set
                {
                    m_refCount = value;
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

                    result.Style = unchecked((int)(WindowStyles.WS_POPUP));
                    result.ExStyle |= (int)WindowExStyles.WS_EX_TOOLWINDOW;

                    return result;
                }
            }
            #endregion

            #region Methods
            /// <summary>
            /// 
            /// </summary>
            public void ShowWindow()
            {
                IntPtr hWnd = this.Handle;

                if (OSFeature.Feature.IsPresent(OSFeature.LayeredWindows))
                {
                    WindowExStyles styleEx = (WindowExStyles)WindowsAPI.GetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_EXSTYLE);

                    if (this.Disabled)
                    {
                        WindowsAPI.SetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_EXSTYLE, (int)(styleEx | WindowExStyles.WS_EX_LAYERED));
                        WindowsAPI.SetLayeredWindowAttributes(hWnd, 0, 0xA0, LayeredWindowFlags.LWA_ALPHA);
                    }
                    else
                    {
                        WindowsAPI.SetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_EXSTYLE, (int)(styleEx & ~WindowExStyles.WS_EX_LAYERED));
                    }
                }

                Size s = GetSize();
                Point p = GetLocation(ref s);

                WindowsAPI.SetWindowPos(hWnd, (IntPtr)SetWindowPosZOrder.HWND_TOPMOST, p.X, p.Y, s.Width, s.Height, SWP_SHOW);
            }
            /// <summary>
            /// 
            /// </summary>
            public void DestroyWindow()
            {
                DestroyHandle();
            }
            /// <summary>
            /// 
            /// </summary>
            public void PerformClick()
            {
                ToolStripItem item = m_Component as ToolStripItem;
                ButtonAdv button = m_Component as BackStageButton;
                BackStageTab tab = m_Component as BackStageTab;
                ButtonAdv buttonadv = m_Component as ButtonAdv;
                if (tab != null)
                {
                    BackStageTab selectedTab = tab as BackStageTab;
                    if (selectedTab != null)
                    {
                        if (selectedTab.Parent != null && selectedTab.Parent is BackStage && selectedTab.Enabled)
                        {
                            (selectedTab.Parent as BackStage).SelectedTab = selectedTab;
                            OnTabDrillDown(selectedTab);
                        }
                    }
                }
                if (button != null)
                {
                   Button ClickItem = button  as BackStageButton;
                   if (ClickItem != null)
                   {
                       ClickItem.PerformClick();
                       if(OnBackStageButtonHitDown!=null)
                         OnBackStageButtonHitDown(ClickItem);
                   }
                }

                if (buttonadv != null)
                {
                    ButtonAdv selectedbtn = buttonadv as ButtonAdv;
                    if (selectedbtn != null && !(selectedbtn is BackStageButton))
                    {
                        selectedbtn.PerformClick();
                        if (OnBackStageTabChildDown != null)
                            OnBackStageTabChildDown(selectedbtn);
                        selectedbtn = null;
                        buttonadv = null;
                    }
                }
                if (item != null && item.Enabled)
                {
                    this.Hide();

                    ToolStripControlHost hostItem = item as ToolStripControlHost;
                    if (hostItem != null)
                    {
                        if (hostItem.Control != null)
                        {
                            ComboBox cb = hostItem.Control as ComboBox;
                            if (cb != null)
                            {
                                if (!cb.DroppedDown)
                                {
                                    cb.Focus();
                                    cb.DroppedDown = true;
                                }
                            }
                            else hostItem.Control.Focus();
                        }
                    }
                    else
                    {
                        ToolStripDropDownItem dropDown = item as ToolStripDropDownItem;
                        if (dropDown != null)
                        {
                            if (item is ToolStripMenuButton)
                            {
                                item.PerformClick();
                            }
                            dropDown.DropDown.Closed += new ToolStripDropDownClosedEventHandler(DropDown_Closed);
                            dropDown.ShowDropDown();
                            if (dropDown.Owner!=null)
                                dropDown.Owner.Invalidate();
                        }
                        else
                        {
                            if (item is ToolStripTabItem)
                            {
                                if ((item as ToolStripTabItem).Panel.Parent is RibbonControlAdv.RibbonControlPopup)
                                {
                                    (item as ToolStripTabItem).Checked = true;
                                    ((item as ToolStripTabItem).Panel.Parent as RibbonControlAdv.RibbonControlPopup).Owner.VisiblePanel = true;
                                }
                                else
                                    item.PerformClick();
                            }
                            else
                                item.PerformClick();
                        }

                        if (OnDrillDown != null)
                        {
                            OnDrillDown(item);
                        }
                    }
                }
            }

            void DropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
            {
                if (OnDropDownClosed != null)
                {
                    OnDropDownClosed(e.CloseReason);
                }
                (sender as ToolStripDropDown).Closed -= new ToolStripDropDownClosedEventHandler(DropDown_Closed);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public bool IsMnemonic(string str)
            {
                return string.Compare(this.Text, str, true) == 0;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public bool IsMatched(string str)
            {
                int nLen = str.Length;

                return nLen == 0 || string.Compare(this.Text, 0, str, 0, nLen, true) == 0;
            }
            bool IsInsideRibbon(Control ctrl)
            {
                while (ctrl != null)
                {
                    if (ctrl is RibbonControlAdv )
                        return true;
                    ctrl = ctrl.Parent;
                }
                return false;
            }
            #endregion

            #region Overrides
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                switch ((Msg)m.Msg)
                {
                    case Msg.WM_MOUSEACTIVATE:
                        m.Result = new IntPtr((int)MouseActivateFlags.MA_NOACTIVATE);
                        return;
                }
                base.WndProc(ref m);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                this.PerformClick();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);
                this.Region = RendererUtils.GetRoundedRegion(new Rectangle(Point.Empty, this.Size), VERTEX_RADIUS);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                GraphicsState gState = g.Save();

                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rc = new Rectangle(Point.Empty, this.Size);

                using (LinearGradientBrush brush = new LinearGradientBrush(rc, Color.White, this.BackColor, 90f))
                {
                    brush.Blend = m_blend;
                    g.FillRectangle(brush, rc);
                }

                TextRenderer.DrawText(g, this.Text, this.Font, rc, this.ForeColor);

                using (Pen pen = new Pen(this.AcceleratorBorderColor))
                {
                    g.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rc, VERTEX_RADIUS));
                }

                g.Restore(gState);
            }

            protected override void Dispose(bool disposing)
            {
                if (m_Component != null)
                    m_Component = null;

                base.Dispose(disposing);
            }

            private Color AcceleratorBorderColor
            {
                get { return Office12ColorTable.GetAlphaBlendedColor(Color.Black, this.BackColor, 32); }
            }
            #endregion

            #region Implementation
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            Point GetLocation(ref Size accSize)
            {
                ToolStripItem item = m_Component as ToolStripItem;
                if (item != null)
                {
                    Control control = item.GetCurrentParent();
                    if (control != null)
                    {

                        Rectangle rc = item.Bounds;
                        Point pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, rc.Height / 2);
                        if (item.Owner != null && item.Owner.Parent != null)
                        {
                            if (IsInsideRibbon(item.Owner.Parent))
                            {
                                if (item.Owner is ToolStripEx)
                                {
                                    if (item.Owner is ToolStripPanelItem.ToolStripInternal)
                                    {
                                        vtop = (item.Owner.Parent.Height - lancherheight) / 3;
                                    }
                                    else
                                    {
                                        vtop = (item.Owner.Height - lancherheight) / 3;
                                    }
                                }
                                if (item.Owner is ToolStripEx)
                                {
                                    ToolStripEx ts = item.Owner as ToolStripEx;
                                    vtop = vtop - 5;
                                    vmiddle = vtop * 2;
                                    vbottom = vtop * 3;
                                    if (ts is ToolStripPanelItem.ToolStripInternal)
                                    {
                                        if (item.Owner.Bounds.Y + item.Bounds.Y <= vtop)
                                        {
                                            pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, 0);
                                            pt.Y = -8;
                                        }
                                        if (item.Owner.Bounds.Y + item.Bounds.Y > vtop && item.Owner.Bounds.Y + item.Bounds.Y < vmiddle)
                                        {
                                            pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, 0);
                                            pt.Y = ((item.Owner.Parent.Bounds.Height / 2) - ts.Bounds.Y) - lancherheight;
                                        }
                                        if (item.Owner.Bounds.Y + item.Bounds.Y > vmiddle && item.Owner.Bounds.Y + item.Bounds.Y <= vbottom)
                                        {
                                            pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, 0);
                                            pt.Y = vbottom;
                                        }
                                    }
                                    else
                                    {
                                        if (item.Bounds.Y <= vtop)
                                        {
                                            pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, 0);
                                            pt.Y = -8;
                                        }
                                        if (item.Bounds.Y > vtop && item.Bounds.Y < vmiddle)
                                        {
                                            pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, 0);
                                            pt.Y = item.Owner.Height / 2 - lancherheight;
                                        }
                                        if (item.Bounds.Y > vmiddle && item.Bounds.Y <= vbottom)
                                        {
                                            pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, 0);
                                            pt.Y = vbottom;
                                        }
                                    }
                                }
                                if ((item as ToolStripMenuButton) != null)
                                {
                                    pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, rc.Height / 3);
                                }
                                else if ((item as QuickButtonReflectable) != null)
                                {
                                    pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, rc.Height - 6);
                                }
                                else if ((item as ToolStripTabItem) != null)
                                {
                                    pt = rc.Location + new Size((rc.Width - accSize.Width) / 2, rc.Height * 3 / 4);
                                }
                                else if ((item as ToolStripCheckBox) != null || (item as ToolStripDropDownButton) != null || (item as ToolStripButton) != null && item.Image != null || (item as ToolStripSplitButton) != null || (item as ToolStripPanelItem) != null)
                                {
                                    pt.X = rc.X + 10;
                                }

                                //When resize the form ,the SuperAccelerator will display outside the form .Subtact the FormWidth and ScrollButtonWidth from the RibbonBorder width.


                                if (!getBounds(control).Contains(control.PointToScreen(pt)))
                                    return new Point(-50, -50);
                                else
                                    return control.PointToScreen(pt);
                            }
                            
                        }
                    }
                }
                else
                {
                    Control btn = m_Component as Control;
                    Point pt = new Point(btn.Bounds.Location.X + (btn.Bounds.Width - accSize.Width) / 2, btn.Bounds.Location.Y);
                    
                    if (btn is TabPageAdv)
                    {
                        BackStage parentBackstage = ((btn as BackStageTab).Parent as BackStage);
                        int index = parentBackstage.TabPages.IndexOf(btn as TabPageAdv);
                        Point loc = parentBackstage.Renderer.GetTabBounds(index).Location;
                        pt = new Point(56, loc.Y);
                    }
                    return btn.Parent.PointToScreen(pt);
                }
                return Point.Empty;
            }

            private Rectangle  getBounds(Control ctrl)
            {
                if (ctrl.Parent != null)
                {
                    if (ctrl.Parent is Form)
                        return ctrl.Parent.Bounds;
                    else
                        return getBounds(ctrl.Parent);
                }
                else
                    return ctrl.Bounds;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            Size GetSize()
            {
                 Size s= TextRenderer.MeasureText(m_graphics, this.Text, this.Font) + new Size(2, 4);
                 if (s.Width < s.Height)
                     s.Width = s.Height ;
                 return s;
            }
            #endregion

            #region Fields
            int m_refCount = 0;
            internal  Component m_Component;
            int vtop = 0, vmiddle = 0, vbottom = 0, lancherheight = 14;
            static Graphics m_graphics;
            static Blend m_blend;

            #endregion

            //BackStageTab Child Selection
            public delegate void OnBackStageTabChildEventHandler(Button clickeditem);
            public event OnBackStageTabChildEventHandler OnBackStageTabChildDown;

            //BackStageButton Selection
            public delegate void OnBackStageButtonHitEventHandler(Button clickeditem);
            public event OnBackStageButtonHitEventHandler OnBackStageButtonHitDown;

            //BackStageTab Selection
            public delegate void OnTabDrillDownEventHandler(BackStageTab selectedTab);
            public event OnTabDrillDownEventHandler OnTabDrillDown;

            public delegate void OnDrillDownEventHandler(ToolStripItem itemClicked);
            public event OnDrillDownEventHandler OnDrillDown;
            public delegate void OnDropDownClosedEventHandler(ToolStripDropDownCloseReason reason);
            public event OnDropDownClosedEventHandler OnDropDownClosed;
        }
        #endregion

        #region *** Accelerators
        class Accelerators : ArrayList
        {
            #region Properties
            /// <summary>
            /// Returns true if all accelerators are disabled
            /// </summary>
            public bool Disabled
            {
                get
                {
                    for (int i = 0, count = this.Count; i < Count; i++)
                    {
                        Accelerator acc = this[i] as Accelerator;
                        if (acc != null && !acc.Disabled)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            #endregion

            #region Methods
            public void Update()
            {
                for (int i = 0, count = this.Count; i < count; i++)
                {
                    Accelerator acc = this[i] as Accelerator;
                    if (acc != null)
                    {
                        acc.Invalidate();
                    }
                }
            }
            #endregion

            #region Overrides
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public override int Add(object value)
            {
                Accelerator acc = value as Accelerator;

                if (acc != null)
                {
                    acc.RefCount++;
                    return base.Add(acc);
                }

                return -1;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <param name="value"></param>
            public override void Insert(int index, object value)
            {
                Accelerator acc = value as Accelerator;

                if (acc != null)
                {
                    acc.RefCount++;
                    base.Insert(index, acc);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public override void Clear()
            {
                for (int i = 0, count = this.Count; i < count; i++)
                {
                    Accelerator acc = this[i] as Accelerator;

                    if (acc != null)
                    {
                        acc.RefCount--;

                        if (acc.RefCount <= 0)
                        {
                            acc.DestroyWindow();
                        }
                    }
                }

                base.Clear();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public override bool Contains(object item)
            {
                if (item is IntPtr)
                {
                    IntPtr hwnd = (IntPtr)item;

                    for (int i = 0, count = this.Count; i < count; i++)
                    {
                        Accelerator acc = this[i] as Accelerator;

                        if (acc != null && acc.Handle == hwnd)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return base.Contains(item);
            }
            #endregion
        }
        #endregion

        #region Constants

        const int intialLevelIndex = 0;
        const int firstLevelIndex = 1;
        const int secondLevelIndex = 2;
        const int thirdLevelIndex = 3;

        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public SuperAccelerator()
            : this(null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public SuperAccelerator(Control owner)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SuperAccelerator));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            m_Owner = owner;
            m_bActive = true;

            m_components = new Hashtable();
            m_accelerators = new Accelerators();
            InitialLevelAccelerator = new Accelerators();
            AcceleratorsStack = new Hashtable();
            if (m_Owner != null)
            {
                m_Owner.HandleCreated += new EventHandler(OnOwnerHandleCreated);
                m_Owner.HandleDestroyed += new EventHandler(OnOwnerHandleDestroyed);

                if (m_Owner.IsHandleCreated)
                {
                    OnOwnerHandleCreated(m_Owner, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the font for the accelerator key.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the font for the accelerator key")]
        public Font Font
        {
            get
            {
                if (m_Font == null)
                {
                    return Control.DefaultFont;
                }
                return m_Font;
            }
            set
            {
                m_Font = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeFont()
        {
            return m_Font != null;
        }
        /// <summary>
        /// 
        /// </summary>
        void ResetFont()
        {
            m_Font = null;
        }
        /// <summary>
        /// Gets ot sets the backcolor of the accelerator key.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets ot sets the backcolor of the accelerator key.")]
        public Color BackColor
        {
            get
            {
                if (m_BackColor == Color.Empty)
                {
                    return Color.White;
                }
                return m_BackColor;
            }
            set
            {
                m_BackColor = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeBackColor()
        {
            return m_BackColor != Color.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        void ResetBackColor()
        {
            m_BackColor = Color.Empty;
        }
        /// <summary>
        /// Gets or sets the forecolor for the accelerator key text.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the forecolor for the accelerator key text.")]
        public Color ForeColor
        {
            get
            {
                if (m_ForeColor == Color.Empty)
                {
                    return SystemColors.ControlText;
                }
                return m_ForeColor;
            }
            set
            {
                m_ForeColor = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeForeColor()
        {
            return m_ForeColor != Color.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        void ResetForeColor()
        {
            m_ForeColor = Color.Empty;
        }
        /// <summary>
        /// Gets or sets whether the accelerator should be active or not.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        [
        Category("Behavior"), DefaultValue(true),
        Description("Gets or sets whether the accelerator should be active or not.")
        ]
        public bool Active
        {
            get
            {
                return m_bActive;
            }
            set
            {
                if (m_bActive != value)
                {
                    m_bActive = value;
                    OnActiveChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets whether an underline should be drawn under the accelerator text.
        /// </summary>
        [
        Category("Behavior"), DefaultValue(true),
        Description("Gets or sets whether an underline should be drawn under the accelerator text.")
        ]
        public bool DisplayShortcuts
        {
            get
            {
                return m_bDisplayShortcuts;
            }
            set
            {
                if (m_bDisplayShortcuts != value)
                {
                    m_bDisplayShortcuts = value;

                    if (m_accelerators.Count > 0)
                    {
                        m_accelerators.Update();
                    }
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        private bool IsActive
        {
            get
            {
                if (m_Owner != null && (m_Owner.Site == null || !m_Owner.Site.DesignMode))
                {
                    return m_bActive;
                }
                return false;
            }
        }
        #endregion

        #region IExtenderProvider implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extendee"></param>
        /// <returns></returns>
        bool IExtenderProvider.CanExtend(object extendee)
        {
            ToolStripEx ts = extendee as ToolStripEx;
            if (ts != null && !(ts is ToolStripPanelItem.ToolStripInternal))
            {
                return true;
            }

            if (extendee is ToolStripItem || extendee is RibbonControlAdv)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets accelerator associated with a component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        [ExtenderProvidedProperty(), Description("Gets or sets accelerator associated with the component.")]
        public string GetAccelerator(ToolStripItem component)
        {
            if (component != null && m_components.ContainsKey(component))
            {
                Accelerator acc = m_components[component] as Accelerator;

                return acc.Text;
            }
            return null;
        }
        /// <summary>
        /// Sets accelerator associated with a component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        [ExtenderProvidedProperty(), Description("Gets or sets accelerator associated with the component.")]
        public void SetAccelerator(ToolStripItem component, string value)
        {
            if (component != null)
            {
                string str = value;
                if (str != null)
                {
                    while (true)
                    {
                        int i = str.IndexOf('&');

                        if (i >= 0)
                        {
                            str = str.Remove(i, 1);
                        }
                        else break;
                    }
                }

                if (str != null && str.Length > 0)
                {
                    if (CanSetAccelerator(component, str.ToUpper()))
                    {
                        if (m_components.ContainsKey(component))
                        {
                            Accelerator acc = m_components[component] as Accelerator;

                            if (acc != null)
                            {
                                acc.Text = str.ToUpper();
                            }
                        }
                        else
                        {
                            Accelerator acc = new Accelerator(component, str.ToUpper());

                            acc.Click += new EventHandler(OnAcceleratorClick);
                            acc.Disposed += new EventHandler(OnAcceleratorDisposed);
                            acc.Paint += new PaintEventHandler(OnAcceleratorPaint);
                            acc.OnDrillDown += new Accelerator.OnDrillDownEventHandler(OnAcceleratorDrillDown);
                            acc.OnDropDownClosed += new Accelerator.OnDropDownClosedEventHandler(OnDropDownClosed);

                            m_components[component] = acc;
                        }
                    }
                    else if(this.DesignMode)
                    {
                        throw new ArgumentException("Specified accelerator value already exists or,\r\nThis value cannot be used at this scope.", "Accelerator");
                    }
                }
                else
                {
                    if (m_components.ContainsKey(component))
                    {
                        Accelerator acc = m_components[component] as Accelerator;

                        if (acc != null)
                        {
                            acc.Dispose();
                        }

                        m_components.Remove(component);
                    }
                }
            }
        }

        void OnDropDownClosed(ToolStripDropDownCloseReason reason)
        {
            m_accelerators.Clear();

            if (reason == ToolStripDropDownCloseReason.Keyboard)
                RevertAccelerators();
            else
                ClearAcceleratorsStack();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        bool ShouldSerializeMenuButtonAccelerator(ToolStripEx toolStrip)
        {
            if (toolStrip != null)
            {
                return m_components.ContainsKey(toolStrip.DropDownButton);
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        void ResetAccelerator(ToolStripEx toolStrip)
        {
            if (toolStrip != null)
            {
                m_components.Remove(toolStrip.DropDownButton);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        bool ShouldSerializeAccelerator(ToolStripItem component)
        {
            if (component != null)
            {
                return m_components.ContainsKey(component);
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        void ResetAccelerator(ToolStripItem component)
        {
            if (component != null)
            {
                m_components.Remove(component);
            }
        }

        /// <summary>
        /// Gets the accelerator of a MenuButton instance on RibbonControlAdv.
        /// </summary>
        /// <param name="ribbon"> The RibbonConrolAdv with MenuButton which tooltip should be get. </param>
        /// <returns> String concatenating accelerator text. </returns>
        [ExtenderProvidedProperty()]
        [Category("Menu Button")]
        [Description("Gets the accelerator of a MenuButton instance on RibbonControlAdv.")]
        public string GetMenuButtonAccelerator(RibbonControlAdv ribbon)
        {
            if (ribbon != null && ribbon.HeaderInternal != null)
            {
                return GetAccelerator(ribbon.HeaderInternal.MenuButton);
            }

            return null;
        }
        /// <summary>
        /// Assigns the accelerator to a MenuButton instance on RibbonControlAdv.
        /// </summary>
        /// <param name="ribbon"> The RibbonConrolAdv with MenuButton to which the tooltip should be assigned. </param>
        /// <param name="value"> The accelerator string. </param>
        [ExtenderProvidedProperty()]
        [Category("Menu Button")]
        [Description("Assigns the accelerator to a MenuButton instance on RibbonControlAdv.")]
        public void SetMenuButtonAccelerator(RibbonControlAdv ribbon, string value)
        {
            if (ribbon != null && ribbon.HeaderInternal != null)
            {
                SetAccelerator(ribbon.HeaderInternal.MenuButton, value);
            }
        }
		/// <summary>
        /// Assigns the accelerator to a CollapsedDropDownButton instance on ToolStripEx.
        /// </summary>
        /// <param name="toolStrip"> The ToolStripEx with CollapsedDropDownButton to which the accelerator should be assigned. </param>
        /// <param name="value"> The accelerator string. </param>
        [ExtenderProvidedProperty()]
        [Description("Assigns the accelerator to a MenuButton instance on RibbonControlAdv.")]
        public void SetCollapsedDropDownAccelerator(ToolStripEx toolStrip, string value)
        {
            if (toolStrip != null)
                toolStrip.DropDownButton.Tag = toolStrip;

            SetAccelerator(toolStrip.DropDownButton, value);
        }
        /// <summary>
        /// Gets the accelerator to a CollapsedDropDownButton instance on ToolStripEx.
        /// </summary>
        /// <param name="toolStrip"> The ToolStripEx with CollapsedDropDownButton to which the accelerator should be get. </param>
        [ExtenderProvidedProperty()]
        [Description("Assigns the accelerator to a MenuButton instance on RibbonControlAdv.")]
        public string GetCollapsedDropDownAccelerator(ToolStripEx toolStrip)
        {
            if (toolStrip != null && m_components.ContainsKey(toolStrip.DropDownButton))
            {
                Accelerator acc = m_components[toolStrip.DropDownButton] as Accelerator;

                return acc.Text;
            }
            return null;
        }

        /// <summary>
        /// Indicates whether the current value of MenuButtonAccelerator is to be serialized.
        /// </summary>
        /// <param name="ribbon"></param>
        /// <returns></returns>
        private bool ShouldSerializeMenuButtonAccelerator(RibbonControlAdv ribbon)
        {
            if (ribbon != null && ribbon.HeaderInternal != null)
            {
                return ShouldSerializeAccelerator(ribbon.HeaderInternal.MenuButton);
            }
            return false;
        }
        /// <summary>
        /// Resets the MenuAccelerator to its default value.
        /// </summary>
        /// <param name="ribbon"></param>
        private void ResetMenuButtonAccelerator(RibbonControlAdv ribbon)
        {
            SetMenuButtonAccelerator(ribbon, null);
        }

        #endregion

        #region IMessageFilter Members
        private bool firstTime = true;
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
                case Msg.WM_NCACTIVATE:
                    bResult = OnNcActivate(ref m);
                    break;
                case Msg.WM_WINDOWPOSCHANGING:
                    HideAccelerators();
                    break;
                case Msg.WM_SYSKEYUP:
                    firstTime = true;
                    break;
                case Msg.WM_SYSKEYDOWN:
                    Keys keyCode = (Keys)(int)m.WParam & Keys.KeyCode;
                    if (m.WParam != (IntPtr)0x20)
                    {
                        bResult = m_bOwnerIsActive && OnChar(ref m);
                        if (m.WParam == (IntPtr)0x12 || m.WParam == (IntPtr)0x79)
                        {
                            if (firstTime && m_bOwnerIsActive)
                            {
                                OnSysKeyUp(ref m);
                                firstTime = false;
                                bResult = false;
                            }
                        }
                    }
                    else if (m.WParam == (IntPtr)0x20)
                    {
                        this.HideAccelerators();
                    }
                    if (keyCode == Keys.Back)
                    {
                        this.HideAccelerators();
                        bResult = false;
                    }
                    break;
                    
                case Msg.WM_SYSCHAR:
                    bResult = m_bOwnerIsActive && m_accelerators.Count > 0;
                    break;
                case Msg.WM_CHAR:
                    bResult = m_bOwnerIsActive && OnChar(ref m);
                    break;
                case Msg.WM_KILLFOCUS:
                    firstTime = true;
                    break;
                case Msg.WM_NCLBUTTONDOWN:
                case Msg.WM_NCRBUTTONDOWN:
                case Msg.WM_NCMBUTTONDOWN:
                case Msg.WM_LBUTTONDOWN:
                case Msg.WM_RBUTTONDOWN:
                case Msg.WM_MBUTTONDOWN:
                    bResult = m_bOwnerIsActive && OnMouseDown(ref m);
                    firstTime = true;
                    break;
            }
            return bResult;
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
                Detach();

                if (m_Owner != null)
                {
                    m_Owner.HandleCreated -= new EventHandler(OnOwnerHandleCreated);
                    m_Owner.HandleDestroyed -= new EventHandler(OnOwnerHandleDestroyed);
                }

                foreach (DictionaryEntry entry in m_components)
                {
                    IDisposable idisposable = entry.Value as IDisposable;

                    if (idisposable != null)
                    {
                        idisposable.Dispose();
                    }
                }
                AcceleratorsStack.Clear();
                AcceleratorsStack = null;
                m_accelerators.Clear();
                m_accelerators = null;
                m_components.Clear();
                m_components = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnOwnerHandleCreated(object sender, EventArgs e)
        {
            m_window = new NativeWindowEx(m_Owner.Handle);

            if (this.IsActive)
            {
                Attach();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnOwnerHandleDestroyed(object sender, EventArgs e)
        {
            Detach();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAcceleratorClick(object sender, EventArgs e)
        {
            m_accelerators.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAcceleratorDisposed(object sender, EventArgs e)
        {
            Accelerator acc = sender as Accelerator;
            if (acc != null)
            {
                acc.Click -= new EventHandler(OnAcceleratorClick);
                acc.Disposed -= new EventHandler(OnAcceleratorDisposed);
                acc.Paint -= new PaintEventHandler(OnAcceleratorPaint);
                acc.OnDrillDown -= new Accelerator.OnDrillDownEventHandler(OnAcceleratorDrillDown);
                acc.OnTabDrillDown -= new Accelerator.OnTabDrillDownEventHandler(OnAcceleratorTabDrillDown);
                acc.OnDropDownClosed -= new Accelerator.OnDropDownClosedEventHandler(OnDropDownClosed);
                acc.OnBackStageButtonHitDown -= acc_OnBackStageButtonHitDown;
                acc.OnBackStageTabChildDown -= acc_OnBackStageTabChildDown;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAcceleratorPaint(object sender, PaintEventArgs e)
        {
            Accelerator acc = sender as Accelerator;
            if (acc != null)
            {
                string sText = acc.Text;

                if (this.DisplayShortcuts && m_sCommand.Length < sText.Length)
                {
                    sText = sText.Insert(m_sCommand.Length, "&");
                }

                Color clText = acc.Disabled ? SystemColors.GrayText : acc.ForeColor;

                TextRenderer.DrawText(e.Graphics, sText, acc.Font, acc.ClientRectangle, clText);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemClicked"></param>
        void OnAcceleratorDrillDown(ToolStripItem itemClicked)
        {
            if (itemClicked is ToolStripTabItem || itemClicked is ToolStripMenuButton || itemClicked is CollapsedDropDownButton)
            {
                this.m_sCommand = string.Empty;
                this.itemClicked = itemClicked;
                this.ShowAccelerators(false, true, false);
            }
            else
            {
                ClearAcceleratorsStack();
            }
        }

        /// <summary>
        /// Invokes BackstageTab child item visibility upon selection
        /// </summary>
        /// <param name="Selected BackStageTab"></param>
        void OnAcceleratorTabDrillDown(BackStageTab SelectedTab)
        {
            if (SelectedTab is BackStageTab)
            {
                this.m_BackStageTab = SelectedTab;
                this.ShowAccelerators(false, true, true);
            }
            else
            {
                ClearAcceleratorsStack();
            }
        }

        /// <summary>
        /// BackStageTab child level verification
        /// </summary>
        /// <param name="clickeditem"></param>
        void acc_OnBackStageTabChildDown(Button clickeditem)
        {
            if (clickeditem is ButtonAdv && AcceleratorsStack.Count > secondLevelIndex)
            {
                for (int i = 0; i < secondLevelIndex; i++)
                {
                    this.AcceleratorsStack.Remove(AcceleratorsStack.Count - firstLevelIndex);
                }
            }
        }

        /// <summary>
        /// Returns selected BackStageButton
        /// </summary>
        /// <param name="clickeditem"></param>
        void acc_OnBackStageButtonHitDown(Button clickeditem)
        {
            if (AcceleratorsStack != null && AcceleratorsStack.Count > intialLevelIndex)
            {
                if (clickeditem is BackStageButton)
                    this.AcceleratorsStack.Remove(AcceleratorsStack.Count - firstLevelIndex);
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// 
        /// </summary>
        void OnActiveChanged()
        {
            if (m_Owner != null && m_Owner.IsHandleCreated)
            {
                if (this.IsActive)
                {
                    Attach();
                }
                else
                {
                    HideAccelerators();
                    Detach();
                }
            }
        }
        /// <summary>
        /// Attaches to owner's window
        /// </summary>
        void Attach()
        {
            if (m_window != null)
            {
                m_window.MessageFilter = this;
            }
            Application.AddMessageFilter(this);
        }
        /// <summary>
        /// Detaches from owner's window
        /// </summary>
        void Detach()
        {
            if (m_window != null)
            {
                m_window.MessageFilter = null;
            }
            Application.RemoveMessageFilter(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        bool OnNcActivate(ref Message m)
        {
            m_bOwnerIsActive = (m.WParam != IntPtr.Zero);

            if (!m_bOwnerIsActive)
            {
                HideAccelerators();
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool OnSysKeyUp(ref Message m)
        {
            bool bResult = m_accelerators.Count > 0;

            if (m.WParam == (IntPtr)VirtualKeys.VK_MENU || m.WParam == (IntPtr)0x79)
            {
                if (m_accelerators.Count == 0)
                {
                    m_sCommand = string.Empty;
                    ShowAccelerators(true,false,false);

                    bResult = m_accelerators.Count > 0;
                }
                else
                {
                    m_accelerators.Clear();
                }
            }

            return bResult;
        }
        bool isClearedTemp = false;

        /// <summary>
        /// Provides revert functionality from BackStageView
        /// </summary>
        private void ClearBackStageView()
        {
            Control parent = null;
            ArrayList newaccelerators;

            // Initial level verification
            if (AcceleratorsStack.Count > intialLevelIndex) 
            {
                ArrayList excludeTabItem = (ArrayList)AcceleratorsStack[intialLevelIndex];
                foreach (Accelerator item in excludeTabItem)
                {
                    if (item.m_Component != null && item.m_Component is BackStageTab)
                    {
                        ArrayList excludeChildTabItem = (ArrayList)AcceleratorsStack[AcceleratorsStack.Count - firstLevelIndex];
                        foreach (Accelerator ChildTab in excludeChildTabItem)
                        {
                            if (ChildTab.m_Component is ButtonAdv && this.AcceleratorsStack.Count > firstLevelIndex)
                            {
                                AcceleratorsStack.Remove(AcceleratorsStack.Count - firstLevelIndex);
                                isClearedTemp = true;
                                return;
                            }
                        }
                        break;
                    }
                }
            }

            // BackStageTab level verification
            if (AcceleratorsStack.Count >= thirdLevelIndex) 
            {
                bool isAcceleratorCleared = false;
                ArrayList accelerators = (ArrayList)AcceleratorsStack[AcceleratorsStack.Count - firstLevelIndex];
                ArrayList secondaryaccelerators = (ArrayList)AcceleratorsStack[AcceleratorsStack.Count - secondLevelIndex];

                if (accelerators != null)
                {
                    foreach (Accelerator acc in accelerators)
                    {
                        if (acc.m_Component != null && acc.m_Component is BackStageButton || acc.m_Component is BackStageTab)
                        {
                            isAcceleratorCleared = false; 
                            break;
                        }
                        else if (acc.m_Component != null && acc.m_Component is ToolStripTabItem) 
                        {
                            for (int i = AcceleratorsStack.Count; i > intialLevelIndex; i--)
                            {
                                AcceleratorsStack.Remove(i - firstLevelIndex);
                            }
                            isClearedTemp = true;
                            return;
                        }
                    }
                }
                if (!isAcceleratorCleared)
                {
                    for (int i = AcceleratorsStack.Count; i > secondLevelIndex; i--) 
                    {
                        AcceleratorsStack.Remove(i - firstLevelIndex);
                    }
                    isClearedTemp = true;
                    if (!VerifyAccelerator(accelerators, secondaryaccelerators)) 
                        return;
                }
                isAcceleratorCleared = false;
            }

            // BackStageView verification
            else if (AcceleratorsStack.Count < thirdLevelIndex) 
            {
                newaccelerators = (ArrayList)AcceleratorsStack[AcceleratorsStack.Count - firstLevelIndex];
                if (newaccelerators != null)
                {
                    foreach (Accelerator acc in newaccelerators)
                    {
                        if (acc.m_Component != null && acc.m_Component is ToolStripTabItem)
                        {
                            for (int i = AcceleratorsStack.Count; i > intialLevelIndex; i--)
                            {
                                AcceleratorsStack.Remove(i - firstLevelIndex);
                            }
                            isClearedTemp = true;
                            return;
                        }
                    }
                }
            }

            foreach (DictionaryEntry entry in m_components)
            {
                ToolStripItem item = entry.Key as ToolStripItem;
                if (item != null)
                    parent = item.GetCurrentParent();
                if (parent != null && parent is RibbonControlAdvHeader && (parent as RibbonControlAdvHeader).BackStageView!=null && (parent as RibbonControlAdvHeader).BackStageView.BackStage.Visible)
                {
                    (parent as RibbonControlAdvHeader).BackStageView.BackStage.Visible = false;
                    if (AcceleratorsStack.Count > 1)
                    {
                        for (int i = AcceleratorsStack.Count; i > 1; i--)
                        {
                            AcceleratorsStack.Remove(i - 1);
                        }
                        isClearedTemp = true;
                        m_BackStageTab = null;
                    }
                    break;
                }
            }
        }
        #region Functions
        /// <summary>
        ///  Ensures BackStage visibilty
        /// </summary>
        private void UpdateBackStageVisibility()
        {
            Control parent = null;

            foreach (DictionaryEntry entry in m_components)
            {
                ToolStripItem item = entry.Key as ToolStripItem;
                if (item != null)
                    parent = item.GetCurrentParent();
                if (parent != null && parent is RibbonControlAdvHeader && (parent as RibbonControlAdvHeader).BackStageView != null
                    && (parent as RibbonControlAdvHeader).BackStageView.BackStage.Visible)
                {
                    (parent as RibbonControlAdvHeader).BackStageView.BackStage.Visible = false;
                }
             }
        }

        /// <summary>
        /// Verify display level status
        /// </summary>
        /// <param name="lastIndex">Accelerator invoked recently</param>
        /// <param name="secondLastIndex">Accelerator invoked in previous</param>
        /// <returns></returns>
        private bool VerifyAccelerator(ArrayList lastIndex, ArrayList secondLastIndex) 
        {
            foreach (Accelerator item in lastIndex)
            {
                foreach (Accelerator item1 in secondLastIndex)
                {
                    if (item.m_Component == item1.m_Component 
                        || (item.m_Component is BackStageTab && item1.m_Component is ButtonAdv) 
                        || (item.m_Component is ToolStripTabItem && item1.m_Component is ToolStripTabItem))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Hides accelerators upon keypress
        /// </summary>
        internal void ClearAccelerator()
        {
            if (m_accelerators.Count > 0)
                m_accelerators.Clear();
        }

        /// <summary>
        /// Returns accelerator count
        /// </summary>
        /// <returns>Accelerators Count </returns>
        internal int GetAcceleratorCount()
        {
            return m_accelerators.Count;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        internal bool OnChar(ref Message m)
        {
            if (m_accelerators.Count > 0)
            {
                switch ((VirtualKeys)m.WParam)
                {
                    case VirtualKeys.VK_ESCAPE:
                        {
                            ClearBackStageView();
                            m_accelerators.Clear();
                            RevertAccelerators();
                            isClearedTemp = false;
                        }
                        break;
                    case VirtualKeys.VK_BACK:
                        {
                            if (m_sCommand.Length > 0)
                            {
                                m_sCommand = m_sCommand.Substring(0, m_sCommand.Length - 1);
                                ShowAccelerators();
                            }
                        }
                        break;
                    case VirtualKeys.VK_RETURN: 
                        {
                            m_accelerators.Clear();
                        }
                        break;
                    case VirtualKeys.VK_SPACE:
                        {
                            m_accelerators.Clear();
                        }
                        break;
                    default:
                        {
                            string sCmd = m_sCommand + (char)m.WParam;

                            Accelerators list = GetAccelerators(sCmd);

                            if (!list.Disabled)
                            {
                                m_accelerators.Clear();

                                if (list.Count == 1)
                                {
                                    Accelerator acc = list[0] as Accelerator;
                                    if (acc != null && acc.IsMnemonic(sCmd))
                                    {
                                        list.Clear();

                                        acc.PerformClick();
                                        break;
                                    }
                                }

                                m_accelerators = list;
                                m_sCommand = sCmd;

                                if (this.DisplayShortcuts)
                                {
                                    list.Update();
                                }
                            }
                            else list.Clear();
                        }
                        break;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool OnMouseDown(ref Message m)
        {
            if (!m_accelerators.Contains(m.HWnd))
            {
                HideAccelerators();
            }
            return false;
        }

        // Holds the collection of items on the ToolStripInternal for faster processing
        private ToolStripPanelItem.ToolStripInternal currentParent;
        private ArrayList ItemsOnCurrentParent = new ArrayList();
        [Syncfusion.Documentation.DocumentationExclude()]
        internal ToolStripPanelItem.ToolStripInternal CurrentParent
        {
            get { return currentParent; }
            set 
            {
                if (currentParent != value)
                {
                    currentParent = value;
                    ItemsOnCurrentParent.Clear();
                }
            }
        }

        /// <summary>
        /// Shows the acceleraors associate with the ToolStripItem.
        /// </summary>
        /// <param name="parentOnly">indicates whether to display the accelerator for ToolStripTabItem and ToolStripMenuButton alone</param>
        /// <param name="drillDown">indicates if this call is caused by item clik of a parent item</param>
        /// <param name="backstagechildenabled">indicates if this call is caused by item clik of a BackStageTab</param>
        void ShowAccelerators(bool parentOnly, bool drillDown, bool backstagechildenabled)
        {
            foreach (DictionaryEntry entry in m_components)
            {
                Control ctrl=null;
                ToolStripItem item = entry.Key as ToolStripItem;
                TabPageAdv tabpageadv = entry.Key as TabPageAdv;
                Accelerator defaultAccelerator = entry.Value as Accelerator;

                if(item==null)
                 ctrl= entry.Key as Control;
                
                bool isRequired = true;
                if (item != null || tabpageadv != null)
                {
                    if (parentOnly)
                    {
                        if (item != null)
                           isRequired = item is ToolStripTabItem || item is ToolStripMenuButton || item is QuickButtonReflectable;
                        if (item != null && item.GetCurrentParent() != null && item.GetCurrentParent() is Syncfusion.Windows.Forms.Tools.RibbonControlAdvHeader && item is ToolStripButton)
                            isRequired = true;
                        if (item != null && item.GetCurrentParent() != null && item.GetCurrentParent() is Syncfusion.Windows.Forms.Tools.RibbonControlAdvHeader && (item.GetCurrentParent() as RibbonControlAdvHeader).BackStageView != null && item is ToolStripMenuButton)
                            isRequired = !(item.GetCurrentParent() as RibbonControlAdvHeader).BackStageView.IsVisible;
                        if (tabpageadv != null && tabpageadv.Parent is BackStageTab)
                            isRequired = true;
                    }
                    else if (m_BackStageTab != null && drillDown) // Here as we consider the BackStage Tab as second level parent this has been included
                    {
                        if (m_BackStageTab is BackStageTab)
                        {
                            isRequired = false;
                        }
                    }
                    else if (drillDown && itemClicked != null && item != null)
                    {
                        if (itemClicked is ToolStripMenuButton)
                            isRequired = item.IsOnDropDown;
                        else if (itemClicked is ToolStripTabItem)
                        {
                            isRequired = !(item is ToolStripTabItem || item is ToolStripMenuButton);
                            if (item.GetCurrentParent() != null && item.GetCurrentParent() is Syncfusion.Windows.Forms.Tools.RibbonControlAdvHeader && item is ToolStripButton)
                                isRequired = false;
                            if (item is CollapsedDropDownButton)
                            {
                                isRequired = item.Visible;
                            }

                            if (item.Owner != null && item.Owner is ToolStripEx)
                            {
                                isRequired &= item.Visible;
                            }
                        }
                        else if (itemClicked is CollapsedDropDownButton)
                        {
                            ToolStripEx tsExItem = item.GetCurrentParent() as ToolStripEx;

                            isRequired = false;

                            if (itemClicked == item)
                            {
                                isRequired = false;
                                continue;
                            }
                            if (tsExItem is ToolStripPanelItem.ToolStripInternal)
                            {
                                CurrentParent = tsExItem as ToolStripPanelItem.ToolStripInternal;

                                isRequired = IsInCollection(item, (itemClicked as CollapsedDropDownButton).Panel.Items);
                            }
                        }
                    }
                }

                if (item != null && item.Visible && isRequired || ctrl != null && isRequired)
                {
                    bool show=true;
                    Control parent = null;
                    if (item!=null)
                        parent=item.GetCurrentParent();
                    else if (ctrl.Parent != null)
                        parent = ctrl.Parent;
                    if (parent is RibbonControlAdvHeader && (parent as RibbonControlAdvHeader).BackStageView != null)
                    {
                        bool test = (parent as RibbonControlAdvHeader).BackStageView.IsVisible;
                        if (test && (parent as RibbonControlAdvHeader).MenuButton == item)
                            show = true;
                        else if (test)
                            show = false;
                    }
                    else if (parent is BackStage) 
                    {
                        if ((parent as BackStage).Visible)
                            show = true;
                        else
                            show = false;
                        if ((parent as BackStage).Visible && ctrl is BackStageButton && !backstagechildenabled) // to ensure that Backstage Tab is selected 
                            show = true;
                        else if (ctrl is BackStageButton && backstagechildenabled || !(parent as BackStage).Visible)
                            show = false;
                    }
                    else if (parent is TabPageAdv)
                    {
                        if ((parent as TabPageAdv).Visible)
                            show = true;
                        else
                            show = false;
                        if (ctrl != null && ctrl is ButtonAdv)
                        {
                            if (backstagechildenabled && (parent as TabPageAdv).Visible)
                                show = true;
                            else
                                show = false;
                        }
                    }


                    if (parent != null && parent is RibbonControlAdvHeader && !show && item is ToolStripTabItem)
                    {
                        Accelerator CustomAccelerator = entry.Value as Accelerator;

                        if (CustomAccelerator.IsMatched(m_sCommand))
                        {
                            if (!m_accelerators.Contains(CustomAccelerator))
                            {
                                CustomAccelerator.Font = this.Font;
                                CustomAccelerator.BackColor = this.BackColor;
                                CustomAccelerator.ForeColor = this.ForeColor;
                                InitialLevelAccelerator.Add(CustomAccelerator);
                            }
                        }
                    }


                    if (parent != null && parent.Created && show)
                    {
                        Accelerator accelerator = entry.Value as Accelerator;

                        if (accelerator.IsMatched(m_sCommand))
                        {
                            if (!m_accelerators.Contains(accelerator))
                            {
                                accelerator.Font = this.Font;
                                accelerator.BackColor = this.BackColor;
                                accelerator.ForeColor = this.ForeColor;
                                accelerator.ShowWindow();

                                m_accelerators.Add(accelerator);
                            }
                            else
                            {
                                if (this.DisplayShortcuts)
                                {
                                    accelerator.Invalidate();
                                }
                            }
                        }
                    }
                }
                else if (item != null && item is ToolStripMenuButton && item.Visible)
                {
                    Accelerator CustomAccelerator1 = entry.Value as Accelerator;

                    if (CustomAccelerator1.IsMatched(m_sCommand))
                    {
                        if (!m_accelerators.Contains(CustomAccelerator1))
                        {
                            CustomAccelerator1.Font = this.Font;
                            CustomAccelerator1.BackColor = this.BackColor;
                            CustomAccelerator1.ForeColor = this.ForeColor;
                            InitialLevelAccelerator.Add(CustomAccelerator1);
                        }
                    }
                }
            }

            if (InitialLevelAccelerator.Count > 0)
            {
                if (InitialLevelAccelerators())
                    AcceleratorsStack[AcceleratorsStack.Count] = InitialLevelAccelerator.Clone();
                InitialLevelAccelerator.Clear();
            }

            if (m_accelerators.Count > 0)
                AcceleratorsStack[AcceleratorsStack.Count] = m_accelerators.Clone();
            else
                ClearAcceleratorsStack();
        }

        /// <summary>
        /// Determine accelerator invoke level 
        /// </summary>
        /// <returns>Accelerator invoke status</returns>
        private bool InitialLevelAccelerators()
        {
            ArrayList accelerators = (ArrayList)AcceleratorsStack[0];
            if (accelerators != null)
            {
                foreach (Accelerator acc in accelerators)
                {
                    if (acc.m_Component is BackStageTab || acc.m_Component is BackStageButton)
                    {
                        return true;
                    }
                }
            }
            else if (AcceleratorsStack.Count == 0)
                return true;
            return false;
        }

        void RevertAccelerators()
        {
            m_sCommand = string.Empty;
            m_accelerators.Clear();

            if (AcceleratorsStack.Count > 0 && !isClearedTemp)
                AcceleratorsStack.Remove(AcceleratorsStack.Count - 1);
            
            ArrayList accelerators = (ArrayList)AcceleratorsStack[AcceleratorsStack.Count - 1];
            if (accelerators != null)
            {
                foreach (Accelerator acc in accelerators)
                {
                    if (acc.m_Component is ToolStripTabItem && AcceleratorsStack.Count == 1)
                    {
                        UpdateBackStageVisibility();
                    }
                    acc.Font = this.Font;
                    acc.BackColor = this.BackColor;
                    acc.ForeColor = this.ForeColor;
                    acc.ShowWindow();

                    m_accelerators.Add(acc);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ShowAccelerators()
        {
            ShowAccelerators(false, false, false);
        }
        /// <summary>
        /// 
        /// </summary>
        void HideAccelerators()
        {
            bool isSetFalse = false;
            if (m_accelerators.Count > 0)
            {
                Control parent = null;

                foreach (DictionaryEntry entry in m_components) 
                {
                    ToolStripItem item = entry.Key as ToolStripItem;

                    if (item != null)
                        parent = item.GetCurrentParent();

                    if (parent != null && parent is RibbonControlAdvHeader && (parent as RibbonControlAdvHeader).BackStageView != null && !(parent as RibbonControlAdvHeader).BackStageView.BackStage.Visible)
                    {
                        isSetFalse = true;
                    }
                }

                if (!isSetFalse)
                ClearAcceleratorsStack();
                m_accelerators.Clear();
                WindowsAPI.RedrawWindow(m_Owner.Handle, IntPtr.Zero, IntPtr.Zero, updateFlags);
                this.itemClicked = null;
            }
            isSetFalse = false;
        }
        /// <summary>
        /// Checks if the specified item is in the CollapsedDropDownButton.Panel.Items collection
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        bool IsInCollection(ToolStripItem sourceItem, ToolStripItemCollection collection)
        {
            if (ItemsOnCurrentParent.Contains(sourceItem))
                return true;

            if (collection.Contains(sourceItem))
            {
                ItemsOnCurrentParent.Add(sourceItem);
                return true;
            }

            foreach (ToolStripItem item in collection)
            {
                if(!ItemsOnCurrentParent.Contains(item))
                    ItemsOnCurrentParent.Add(sourceItem);

                if (item is ToolStripPanelItem)
                  return IsInCollection(sourceItem, (item as ToolStripPanelItem).Items);
            }

            return false;
        }

        private void ClearAcceleratorsStack()
        {
            AcceleratorsStack.Clear();
            this.itemClicked = null;
            this.m_BackStageTab = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        Accelerators GetAccelerators(string str)
        {
            Accelerators list = new Accelerators();

            for (int i = 0, count = m_accelerators.Count; i < count; i++)
            {
                Accelerator acc = m_accelerators[i] as Accelerator;

                if (acc != null && acc.IsMatched(str))
                {
                    list.Add(acc);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a value indicating if the specified component can be added to the SuperAccelerator.
        /// </summary>
        /// <param name="component">ToolStripItem to be added</param>
        /// <param name="value">String specifying the accelerator</param>
        /// <returns>True if this accelerator can be added</returns>
        bool CanSetAccelerator(ToolStripItem component, string value)
        {
            bool result = true;
            if (component is ToolStripTabItem || component is ToolStripMenuButton)
            {
                foreach (object item in m_components.Keys)
                {
                    if (item is BackStageButton)
                        return result;
                    if (item is BackStageTab)
                        return result;
                }

                foreach (ToolStripItem item in m_components.Keys)
                {
                    if ((item as ToolStripMenuButton) == null || (item as ToolStripTabItem) == null
                        || item == component)
                        continue;

                    result = IsValidAccelerator(item, value);

                    if (!result)
                        break;
                }
            }
            else if (component.IsOnDropDown)
            {
                foreach (Object item1 in this.m_components.Keys)
                {
                    if (item1 is ToolStripItem)
                    {
                        ToolStripItem item = item1 as ToolStripItem;
                        if (component is ToolStripTabItem || component is ToolStripMenuButton
                           || item is ToolStripTabItem || item is ToolStripMenuButton)
                            continue;
                        if (item == component)
                            continue;
                        if (item is RibbonControlAdvHeader.QuickItemsOverflowButton)
                            continue;
                        if (!item.IsOnDropDown)
                            continue;
                        else if (component.OwnerItem != null && component.OwnerItem == item.OwnerItem)
                        {
                            result = IsValidAccelerator(item, value);
                            if (!result)
                                break;
                        }
                    }
                }
            }
            else
            {
                foreach (object item in this.m_components.Keys)
                {
                    if (item is ToolStripItem)
                    {
                       
                        if (component is ToolStripTabItem || component is ToolStripMenuButton
                            || item is ToolStripTabItem || item is ToolStripMenuButton)
                            continue;
                        if (item is RibbonControlAdvHeader.QuickItemsOverflowButton
                            || item == component)
                            continue;

                        if (IsInSameScope(component, item as ToolStripItem))
                            result = IsValidAccelerator(item as ToolStripItem, value);
                        else
                            continue;

                        if (!result)
                            break;
                    }
                }
            }

            return result;
        }
        [ExtenderProvidedProperty(), Description("Gets or sets accelerator associated with the component.")]
        public string GetAccelerator(Control component)
        {
            if (component != null && m_components.ContainsKey(component))
            {
                Accelerator acc = m_components[component] as Accelerator;

                return acc.Text;
            }
            return null;
        }
        /// <summary>
        /// Sets accelerator associated with a component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        [ExtenderProvidedProperty(), Description("Gets or sets accelerator associated with the component.")]
        public void SetAccelerator(Control component, string value)
        {
            if (component != null)
            {
                string str = value;
                if (str != null)
                {
                    while (true)
                    {
                        int i = str.IndexOf('&');

                        if (i >= 0)
                        {
                            str = str.Remove(i, 1);
                        }
                        else break;
                    }
                }

                if (str != null && str.Length > 0)
                {
                    if (CanSetAccelerator(component, str.ToUpper()))
                    {
                        if (m_components.ContainsKey(component))
                        {
                            Accelerator acc = m_components[component] as Accelerator;

                            if (acc != null)
                            {
                                acc.Text = str.ToUpper();
                            }
                        }
                        else
                        {
                            Accelerator acc = new Accelerator(component, str.ToUpper());

                            acc.Click += new EventHandler(OnAcceleratorClick);
                            acc.Disposed += new EventHandler(OnAcceleratorDisposed);
                            acc.Paint += new PaintEventHandler(OnAcceleratorPaint);
                            acc.OnDrillDown += new Accelerator.OnDrillDownEventHandler(OnAcceleratorDrillDown);
                            acc.OnTabDrillDown += new Accelerator.OnTabDrillDownEventHandler(OnAcceleratorTabDrillDown);
                            acc.OnDropDownClosed += new Accelerator.OnDropDownClosedEventHandler(OnDropDownClosed);
                            acc.OnBackStageButtonHitDown += acc_OnBackStageButtonHitDown;
                            acc.OnBackStageTabChildDown += acc_OnBackStageTabChildDown;
                            m_components[component] = acc;
                        }
                    }
                    else if (this.DesignMode)
                    {
                        throw new ArgumentException("Specified accelerator value already exists or,\r\nThis value cannot be used at this scope.", "Accelerator");
                    }
                }
                else
                {
                    if (m_components.ContainsKey(component))
                    {
                        Accelerator acc = m_components[component] as Accelerator;

                        if (acc != null)
                        {
                            acc.Dispose();
                        }

                        m_components.Remove(component);
                    }
                }
            }
        }
        bool CanSetAccelerator(Control component, string value)
        {
            bool result = true;
            if (component is Button )
            {
                foreach (Object item in m_components.Keys)
                {
                    if ((item as Button) == null ||  item == component)
                        continue;
                    if (item is Button)
                        result = IsValidAccelerator((item as Button), value);

                    if (!result)
                        break;
                }
            }
            return result;
        }
        bool IsValidAccelerator(Control item, string value)
        {
            string acc = this.GetAccelerator(item);

            if (value == acc)
                return false;
            else if (value.Length < acc.Length && acc.StartsWith(value))
                return false;
            else if (value.Length > acc.Length && value.StartsWith(acc))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component">ToolStripItem to be added to the SuperToolTip</param>
        /// <param name="item">Existing item in SuperAccelerator against which component scope is to be decided.</param>
        /// <returns>True if both the ToolStripItems exists under the same ToolStripTabItem</returns>
        bool IsInSameScope(ToolStripItem component, ToolStripItem item)
        {
            Object cmpntOwner = component.Owner;
            Object itemOwner = item.Owner;

           
            if (component is CollapsedDropDownButton)
            {
                cmpntOwner = component.Tag as ToolStripEx;

                if (cmpntOwner == itemOwner)
                {
                    return false;
                }
            }
            if(item is CollapsedDropDownButton)
            {
                itemOwner = item.Tag as ToolStripEx;

                if (cmpntOwner == itemOwner)
                {
                    return false;
                }
            }
            if (component is CollapsedDropDownButton && item is CollapsedDropDownButton)
            {
                return (itemOwner as ToolStripEx).TabItem == (cmpntOwner as ToolStripEx).TabItem;
            }


            if (cmpntOwner == itemOwner)
            {
                return true;
            }

            if (itemOwner is ToolStripPanelItem.ToolStripInternal && cmpntOwner is ToolStripPanelItem.ToolStripInternal
                && (itemOwner as ToolStripPanelItem.ToolStripInternal).TabItem != null)
            {
                return (itemOwner as ToolStripPanelItem.ToolStripInternal).TabItem == (cmpntOwner as ToolStripPanelItem.ToolStripInternal).TabItem;
            }
            else if (itemOwner is ToolStripEx && (itemOwner as ToolStripEx).TabItem != null && cmpntOwner is ToolStripPanelItem.ToolStripInternal)
            {
                return (itemOwner as ToolStripEx).TabItem == (cmpntOwner as ToolStripPanelItem.ToolStripInternal).TabItem;
            }
            else if (itemOwner is ToolStripPanelItem.ToolStripInternal && cmpntOwner is ToolStripEx && (cmpntOwner as ToolStripEx).TabItem != null)
            {
                return (itemOwner as ToolStripPanelItem.ToolStripInternal).TabItem == (cmpntOwner as ToolStripEx).TabItem;
            }
            else if (itemOwner is ToolStripPanelItem.ToolStripInternal && cmpntOwner is ToolStripPanelItem.ToolStripInternal)
            {
                Object cmpntOwnerParent = (cmpntOwner as ToolStripPanelItem.ToolStripInternal).Parent;
                Object itemOwnerParent = (itemOwner as ToolStripPanelItem.ToolStripInternal).Parent;

                if (itemOwnerParent is ToolStripEx && cmpntOwnerParent is ToolStripEx)
                    return (itemOwnerParent as ToolStripEx).TabItem == (cmpntOwnerParent as ToolStripEx).TabItem;
            }
            else if (itemOwner is ToolStripEx && cmpntOwner is ToolStripEx)
            {
                if(!(item is CollapsedDropDownButton || component is CollapsedDropDownButton))
                    return (itemOwner as ToolStripEx).TabItem == (cmpntOwner as ToolStripEx).TabItem;
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="component">ToolStripItem against which the value is to be comapred</param>
        /// <param name="value">String that acts as the accelerator</param>
        /// <returns>True if accelerator value can exists in the given scope</returns>
        bool IsValidAccelerator(ToolStripItem item, string value)
        {
            string acc = this.GetAccelerator(item);

            if (value == acc)
                return false;
            else if (value.Length < acc.Length && acc.StartsWith(value))
                return false;
            else if (value.Length > acc.Length && value.StartsWith(acc))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Fields
        Control m_Owner;
        NativeWindowEx m_window;

        Font m_Font = null;

        Color m_BackColor = Color.Empty;
        Color m_ForeColor = Color.Empty;
        bool m_bDisplayShortcuts = true;

        bool m_bActive;
        bool m_bOwnerIsActive = true;

        string m_sCommand;

        Hashtable m_components;
        Hashtable AcceleratorsStack;
        Accelerators m_accelerators;
        Accelerators InitialLevelAccelerator;
        private ToolStripItem itemClicked;
        private BackStageTab m_BackStageTab;
        static RedrawWindowFlags updateFlags = RedrawWindowFlags.RDW_INVALIDATE | RedrawWindowFlags.RDW_ALLCHILDREN | RedrawWindowFlags.RDW_UPDATENOW;
        #endregion
    }
    #endregion
}
#endif
