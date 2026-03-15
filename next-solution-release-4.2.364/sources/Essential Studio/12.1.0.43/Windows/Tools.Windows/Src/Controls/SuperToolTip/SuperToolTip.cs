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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel.Design.Serialization;
using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using System.Diagnostics;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
    #region Events definition
    public enum ToolTipPropertyID
    {
        Unspecified,
        ItemHidden,
        ItemTextChanged,
        ItemImageChanged,
        ItemImageScaleChanged,
        ItemFontChanged,
        TextMarginsChanged,
        SeparatorChanged,
        ItemSizeChanged
    };

    /// <summary>
    /// Specifies the way image and text are situated on tool tip.
    /// </summary>
    public enum ToolTipTextImageRelation
    {
        /// <summary>
        /// Image is situated before text.
        /// </summary>
        ImageBeforeText,
        /// <summary>
        /// Text is situated before image.
        /// </summary>
        TextBeforeImage,
    }

    public delegate void ToolTipPropertyChangedEventHandler(object sender, ToolTipPropertyID id);
    public delegate void PopupHandler(Component component, ref Message m);
    public delegate void PopupToolTipHandler(Component component, ref Rectangle rc);
    public delegate void UpdateToolTipHandler(Component component, ref ToolTipInfo info);
    #endregion

    #region Constants
    /// <summary>
    /// 
    /// </summary>
    public enum TTF
    {
        TTF_IDISHWND = 0x0001,
        TTF_SUBCLASS = 0x0010,
        TTF_TRACK = 0x0020,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum TTM
    {
        TTM_ACTIVATE = Msg.WM_USER + 1,
        TTM_SETDELAYTIME = Msg.WM_USER + 3,
        TTM_RELAYEVENT = Msg.WM_USER + 7,
        TTM_WINDOWFROMPOINT = Msg.WM_USER + 16,
        TTM_GETDELAYTIME = Msg.WM_USER + 21,
        TTM_POP = Msg.WM_USER + 28,
        TTM_POPUP = Msg.WM_USER + 34,
        TTM_ADDTOOLW = Msg.WM_USER + 50,
        TTM_DELTOOLW = Msg.WM_USER + 51,
        TTM_NEWTOOLRECTW = Msg.WM_USER + 52,
        TTM_GETTOOLINFOW = Msg.WM_USER + 53,
        TTM_SETTOOLINFOW = Msg.WM_USER + 54,
        TTM_GETCURRENTTOOLW = Msg.WM_USER + 59,

        TTM_TRACKACTIVATE = Msg.WM_USER + 17,  // wParam = TRUE/FALSE start end  lparam = LPTOOLINFO
        TTM_TRACKPOSITION = Msg.WM_USER + 18,  // lParam = dwPos
    }
    /// <summary>
    /// 
    /// </summary>
    public enum TTN
    {
        TTN_FIRST = -520,
        TTN_SHOW = TTN_FIRST - 1,
        TTN_POP = TTN_FIRST - 2,
        TTN_GETDISPINFOW = TTN_FIRST - 10,
    }
    /// <summary>
    /// 
    /// </summary>
    abstract class TXTFORMAT
    {
        public const TextFormatFlags COMMON =
            TextFormatFlags.ExpandTabs |
            TextFormatFlags.WordBreak |
            TextFormatFlags.WordEllipsis |
            TextFormatFlags.NoPadding;
    }
    #endregion

    #region IToolTipService
   public interface IToolTipService : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        void AddComponent(object component);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        void RemoveComponent(object component);

        event PopupHandler Popup;
        event EventHandler Pop;
    }
    #endregion

    #region SuperToolTip
    [ProvideProperty("ToolTip", typeof(Component))]
    [ProvideProperty("MenuButtonToolTip", typeof(RibbonControlAdv))]
    [ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Tools.SuperToolTip), "ToolboxIcons.ToolTip.bmp")]
    [TypeConverter(typeof(Design.SuperToolTipTypeConverter))]
    [Description("Provides options to set Office 2007 Style ScreenTips.")]
   public class SuperToolTip : Component, IExtenderProvider, INativeMessageFilter, IMessageFilter
    {
        #region Constants
        public enum FadingType
        {
            System = 0,
            Blend = 1,
        }
        public enum SuperToolTipStyle
        {
            Normal = 0,
            Balloon = 1,
            Office2013Style
        }
        /// <summary>
        /// SuperToolTip Style
        /// </summary>
        public enum Appearance
        {
            /// <summary>
            /// Default appearance.
            /// </summary>
            Default,
            /// <summary>
            /// Metro-like appearance.
            /// </summary>
            Metro
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        static SuperToolTip()
        {
            m_tooltips = new ArrayList();
        }
        /// <summary>
        /// Specifies an advanced appearance this control.
        /// </summary>
        private Appearance appearance = Appearance.Default;
        /// <summary></summary>
        private Color metroColor = Color.Empty;      
        /// <summary>
        /// 
        /// </summary>
        public SuperToolTip()
            : this(null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public SuperToolTip(Control owner)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SuperToolTip));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            hook = new WindowsAPI.HookProc(MouseHookProc);
            m_tooltips.Add(this);

            m_tipsInfo = new Dictionary<Component, ToolTipInfo>();
            m_owner = owner;
            m_control = new ToolTipControl(this);
            m_trackControl = new ToolTipControl(this);

            m_handler = new NativeMessageHandler();
            m_handler.MessageFilter = this;

            m_control.Popup += new EventHandler(OnPopup);
            m_control.HandleCreated += new EventHandler(OnToolTipControlHandleCreated);
            m_control.HandleDestroyed += new EventHandler(OnToolTipControlHandleDestroyed);

            if (m_owner != null)
            {
                m_owner.HandleCreated += new EventHandler(OnOwnerHandleCreated);
                m_owner.HandleDestroyed += new EventHandler(OnOwnerHandleDestroyed);
                m_owner.Disposed += new EventHandler(m_owner_Disposed);
                if (m_owner.IsHandleCreated)
                {
                    OnOwnerHandleCreated(m_owner, EventArgs.Empty);
                }
            }
            m_control.BackColor = m_control.Info.BackColor;
        }

        void m_owner_Disposed(object sender, EventArgs e)
        {
            this.Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Shows the tooltip at a specified location.
        /// </summary>
        /// <param name="ttInfo">The ToolTipInfo created.</param>
        /// <param name="position">The position to be displayed.</param>
        public void Show(ToolTipInfo ttInfo, Point position)
        {
            Show(ttInfo, position, m_control.DisplayTime);
        }
        /// <summary>
        /// Shows the tooltip at a specified location for a particular amount of time.
        /// </summary>
        /// <param name="ttInfo">The ToolTipInfo created.</param>
        /// <param name="position">The position to be displayed.</param>
        /// <param name="displayTime">Duration (in milliseconds) to display the ToolTip (-1 to display infinitely)</param>
        public void Show(ToolTipInfo ttInfo, Point position, int displayTime)
        {
            if (ttInfo != null && !m_trackControl.IsDisposed)
            {
                Hide();
                m_trackControl.MaxWidth = this.MaxWidth;
                m_trackControl.UseFading = this.UseFading;
                m_trackControl.Style = this.Style;
                m_trackControl.ShowToolTip(ttInfo, position, displayTime);
            }
        }
        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        public void Hide()
        {
            m_control.HideToolTip();
            m_trackControl.HideToolTip();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="component"></param>
        static internal void SetToolTips(Component target, Component source)
        {
            foreach (SuperToolTip toolTip in m_tooltips)
            {
                toolTip.Hide();
                toolTip.SetToolTip(target, toolTip.GetToolTip(source));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        static internal void RemoveToolTips(Component target)
        {
            foreach (SuperToolTip toolTip in m_tooltips)
            {
                toolTip.Hide();
                toolTip.SetToolTip(target, null);
            }
        }
        #endregion

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_tipsInfo != null)
                {
                    m_tipsInfo.Clear();
                    m_tipsInfo = null;
                }

                if (m_control != null)
                {
                    m_control.Popup -= new EventHandler(OnPopup);

                    m_control.Dispose();
                    m_control = null;
                }

                if (m_trackControl != null)
                {
                    m_trackControl.Dispose();
                    m_trackControl = null;
                }

                if (m_handler != null)
                {
                    m_handler.MessageFilter = null;
                    m_handler = null;
                }

                if (m_owner != null)
                {
                    m_owner.HandleCreated -= new EventHandler(OnOwnerHandleCreated);
                    m_owner.HandleDestroyed -= new EventHandler(OnOwnerHandleDestroyed);
                    m_owner.Disposed -= new EventHandler(m_owner_Disposed);
                }
            }

            m_tooltips.Remove(this);

            base.Dispose(disposing);
        }
        #endregion

        #region IExtenderProvider Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extendee"></param>
        /// <returns></returns>
        bool IExtenderProvider.CanExtend(object extendee)
        {
            Component tool = Cast(extendee);

            if (tool != null)
            {
                return
                    tool is Control && !(tool is XPToolBar) ||
                    tool is Syncfusion.Windows.Forms.Tools.XPMenus.BarItem ||
                    m_control.GetToolTipService(tool.GetType()) != null;
            }

            return false;
        }
        /// <summary>
        /// Gets SuperToolTip associated with a component.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        //[DefaultValue(null)]
        [ExtenderProvidedProperty(), Description("Gets or sets SuperToolTip associated with a component.")]
        public ToolTipInfo GetToolTip(Component component)
        {
            Component tool = Cast(component);

            if (tool != null && m_tipsInfo.ContainsKey(tool))
            {
                ToolTipInfo tti = m_tipsInfo[tool];

                tti.RightToLeft = this.RightToLeft;

                return tti;
            }
            return null;
        }
        /// <summary>
        /// Sets SuperToolTip associated with a component.
        /// </summary>
        /// <param name="component">The component to which the tooltip should be assigned.</param>
        /// <param name="value">The toolTipInfo created.</param>
        [ExtenderProvidedProperty(), Description("Gets or sets SuperToolTip associated with a component.")]
        public void SetToolTip(Component component, ToolTipInfo value)
        {
            Component tool = Cast(component);

            if (tool != null)
            {
                if (value != null)
                {
                    if (!m_tipsInfo.ContainsKey(tool))
                    {
                        m_control.AddTool(tool);
                    }
                    m_tipsInfo[tool] = value;
                }
                else
                {
                    if (m_tipsInfo.ContainsKey(tool))
                    {
                        m_control.DelTool(tool);
                        m_tipsInfo.Remove(tool);
                    }
                }
            }
        }
        /// <summary>
        /// Indicates whether the current value of ToolTip is to be serialized.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private bool ShouldSerializeToolTip(Component component)
        {
            Component tool = Cast(component);

            return tool != null && m_tipsInfo.ContainsKey(tool);
        }
        /// <summary>
        /// Resets the ToolTip to its default value.
        /// </summary>
        /// <param name="component"></param>
        void ResetToolTip(Component component)
        {
            SetToolTip(component, null);
        }
        /// <summary>
        /// Gets the tooltip from the MenuButton on RibbonControlAdv
        /// </summary>
        /// <param name="ribbon"> The RibbonConrolAdv with MenuButton from which the tooltip should be get. </param>
        /// <returns></returns>
        [ExtenderProvidedProperty()]
        [Category("Menu Button")]
        public ToolTipInfo GetMenuButtonToolTip(RibbonControlAdv ribbon)
        {
            if (ribbon != null && ribbon.HeaderInternal != null)
            {
                return GetToolTip(ribbon.HeaderInternal.MenuButton);
            }

            return null;
        }
        /// <summary>
        /// Assigns the tooltip to a MenuButton instance on RibbonControlAdv.
        /// </summary>
        /// <param name="ribbon"> The RibbonConrolAdv with MenuButton to which the tooltip should be assigned. </param>
        /// <param name="value"> The toolTipInfo created. </param>
        [ExtenderProvidedProperty()]
        [Category("Menu Button")]
        public void SetMenuButtonToolTip(RibbonControlAdv ribbon, ToolTipInfo value)
        {
            if (ribbon != null && ribbon.HeaderInternal != null)
            {
                SetToolTip(ribbon.HeaderInternal.MenuButton, value);
            }
        }
        /// <summary>
        /// Indicates whether the current value of MenuButtonToolTip is to be serialized.
        /// </summary>
        /// <param name="ribbon"></param>
        /// <returns></returns>
        private bool ShouldSerializeMenuButtonToolTip(RibbonControlAdv ribbon)
        {
            if (ribbon != null && ribbon.HeaderInternal != null)
            {
                return ShouldSerializeToolTip(ribbon.HeaderInternal.MenuButton);
            }

            return false;
        }
        /// <summary>
        /// Resets the MenuToolTip to its default value.
        /// </summary>
        /// <param name="ribbon"></param>
        private void ResetMenuButtonToolTip(RibbonControlAdv ribbon)
        {
            SetMenuButtonToolTip(ribbon, null);
        }
        #endregion

        #region IMessageFilter Members
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            switch ((Msg)m.Msg)
            {
                case Msg.WM_LBUTTONDOWN:
                case Msg.WM_LBUTTONUP:
                case Msg.WM_MBUTTONDOWN:
                case Msg.WM_MBUTTONUP:
                case Msg.WM_MOUSEMOVE:
                case Msg.WM_RBUTTONDOWN:
                case Msg.WM_RBUTTONUP:
                    if (m_control.IsHandleCreated)
                    {
                        Control tool = GetTool(ref m);
                        if (tool != null)
                        {
                            POINT pt = new POINT(m.LParam);
                            WindowsAPI.MapWindowPoints(m.HWnd, tool.Handle, ref pt, 1);

                            MSG msg = new MSG();
                            msg.hwnd = tool.Handle;
                            msg.message = m.Msg;
                            msg.wParam = m.WParam;
                            msg.lParam = (IntPtr)WindowsAPI.MAKELONG(pt.x, pt.y);

                            WindowsAPI.SendMessage(m_control.Handle, (int)TTM.TTM_RELAYEVENT, 0, ref msg);
                        }
                    }
                    break;
            }
            return false;
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MOUSEHOOKSTRUCT mouseHookStruct = (MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MOUSEHOOKSTRUCT));
            switch((int)wParam)
            {
                case (int) Msg.WM_MOUSEMOVE:
                case (int) Msg.WM_LBUTTONDOWN:
                case (int) Msg.WM_LBUTTONUP:
                case (int) Msg.WM_MBUTTONDOWN:
                case (int) Msg.WM_MBUTTONUP:
                case (int) Msg.WM_RBUTTONDOWN:
                case (int) Msg.WM_RBUTTONUP:
                    if (m_control.IsHandleCreated && ! this.DesignMode)
                    {
                        Point point = Cursor.Position;
                        Control tool = GetTool(mouseHookStruct.hwnd);
                        
                            if (tool != null)
                            {
                                UserControl userControl = tool.Parent as UserControl;
                                if ( userControl != null && userControl.Parent == null )
                                {
                                    POINT pt = mouseHookStruct.pt;
                                    WindowsAPI.MapWindowPoints(mouseHookStruct.hwnd, tool.Handle, ref pt, 1);

                                    MSG msg = new MSG();
                                    msg.hwnd = tool.Handle;
                                    msg.message = (int)wParam;
                                    msg.wParam = wParam;
                                    msg.lParam = (IntPtr)WindowsAPI.MAKELONG(pt.x, pt.y);

                                    WindowsAPI.SendMessage(m_control.Handle, (int)TTM.TTM_RELAYEVENT, 0, ref msg);
                                }
                            }
                    }
                    break;
            }
            return WindowsAPI.CallNextHookEx(nhook, nCode, wParam, lParam);
        }
        #endregion

        #region INativeMessageFilter Members
        bool INativeMessageFilter.ProcessMessage(ref Message m)
        {
            bool bResult = false;
            
            switch ((Msg)m.Msg)
            {
                case Msg.WM_WINDOWPOSCHANGING:
                    {
                        m_trackControl.HideToolTip();
                    }
                    break;
                case Msg.WM_NCACTIVATE:
                    if ((int)m.WParam == (int)ActivateState.WA_INACTIVE)
                    {
                        m_trackControl.HideToolTip();
                    }
                    break;
            }

            return bResult;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPopup(object sender, EventArgs e)
        {
            m_trackControl.HideToolTip();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnToolTipControlHandleCreated(object sender, EventArgs e)
        {
            Application.AddMessageFilter(this);
            nhook = WindowsAPI.SetWindowsHookEx((int)WindowsHookCodes.WH_MOUSE, hook, (IntPtr)0, Process.GetCurrentProcess().Threads[0].Id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnToolTipControlHandleDestroyed(object sender, EventArgs e)
        {
            Application.RemoveMessageFilter(this);
            WindowsAPI.UnhookWindowsHookEx(nhook);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnOwnerHandleCreated(object sender, EventArgs e)
        {
            m_handler.Assign((sender as Control).Handle);
        }

        /// <summary>
        /// Called when owner's handle is destroyed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnOwnerHandleDestroyed(object sender, EventArgs e)
        {
            m_handler.Unassign();
        }

        #endregion

        #region Implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        private Control GetTool(ref Message m)
        {
            POINT pt = new POINT(m.LParam);

            WindowsAPI.MapWindowPoints(m.HWnd, IntPtr.Zero, ref pt, 1);
            IntPtr hWnd = (IntPtr)WindowsAPI.SendMessage(m_control.Handle, (int)TTM.TTM_WINDOWFROMPOINT, 0, ref pt);
            IntPtr hActualWnd = WindowsAPI.WindowFromPoint(pt);

            Control tool = null;

            if (hWnd == hActualWnd && m_control.Contains(hWnd))
            {
                tool = Control.FromHandle(hWnd);
            }

            return tool;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private Control GetTool(IntPtr handle)
        {
            Control tool = null;

            if ( m_control.Contains(handle))
            {
                tool = Control.FromHandle(handle);
            }

            return tool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        static Component Cast(object tool)
        {
            if (tool is ToolStripControlHost)
            {
                return (tool as ToolStripControlHost).Control;
            }
            return tool as Component;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets, the duration of the ToolTip (in sec) when mouse hovers over a control.
        /// </summary>
        [DefaultValue(0)]
        [Description("Indicates the duration of the ToolTip (in sec) when mouse hovers over a control.")]
        public int ToolTipDuration
        {
            get
            {
                return m_control.ToolTipDuration;
            }
            set
            {
                m_control.ToolTipDuration = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        [Description("Indicates the time (in msec) before the tooltip to be shown.")]
        public int InitialDelay
        {
            get
            {
                return m_control.InitialDelay;
            }
            set
            {
                m_control.InitialDelay = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum width of SuperToolTip.
        /// </summary>
        [DefaultValue(0)]
        [Description("Indicates the maximum width of SuperToolTip.")]
        public int MaxWidth
        {
            get
            {
                return m_control.MaxWidth;
            }
            set
            {
                m_control.MaxWidth = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Description("Gets or sets fading type")]
        [DefaultValue(typeof(FadingType), "Blend")]
        public FadingType UseFading
        {
            get
            {
                return m_control.UseFading;
            }
            set
            {
                m_control.UseFading = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Description("Gets or sets a value indicating whether tooltip info is displayed from right to left")]
        [DefaultValue(typeof(RightToLeft), "No")]
        public RightToLeft RightToLeft
        {
            get
            {                
                Control ctrl = m_owner;
                if (null != ctrl)
                {
                    m_rightToLeft = ctrl.RightToLeft;
                }
                return m_rightToLeft;
            }
            set
            {
                if (m_rightToLeft != value)
                {
                    m_rightToLeft = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Description("Gets or sets style of the SuperToolTip")]
        [DefaultValue(typeof(SuperToolTipStyle), "Normal")]
        public SuperToolTipStyle Style
        {
            get
            {
                return m_control.Style;
            }
            set
            {
                m_control.Style = value;
            }
        }
        /// <summary>
        /// Gets or sets the theme color of the ButtonAdv
        /// </summary>
        [
        Browsable(true),
        Category("MetroColor"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("Gets or sets the pressed background color of the control.")
        ]
        public Color MetroColor
        {
            get { return metroColor; }
            set
            {
                metroColor = value;
            }
        }
        private bool ShouldSerializeMetroColor()
        {
            if (this.MetroColor == Color.Empty)
                return false;
            else
                return true;
        }

        private void ResetMetroColor()
        {
            this.MetroColor = Color.Empty;
        }
        /// <summary>
        /// Gets or sets an advanced appearance for the RangeSliderAdv.
        /// </summary>
        [Description("Gets or sets an advanced appearance for the RangeSliderAdv.")]
        [Category("Appearance")]
        [DefaultValue(Appearance.Default)]
        public Appearance VisualStyle
        {
            get
            {
                return this.appearance;
            }

            set
            {
                if (this.appearance != value)
                {
                    this.appearance = value;
                    //this.Invalidate();
                }
            }
        }
        private bool ShouldSerializeVisualStyle()
        {
            if (this.VisualStyle != Appearance.Default)
                this.VisualStyle = Appearance.Default;
            return true;
        }

        private void ResetVisualStyle()
        {
            this.VisualStyle = Appearance.Default;
        }
        [Description("Gets or sets Gradient Background of the SuperToolTip")]
        [DefaultValue(true )]
        public bool GradientBackGround
        {
            get
            {
                return m_control.GradientBackGround;
            }
            set
            {
                if (m_control.GradientBackGround != value)
                   m_control.GradientBackGround  = value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the tool tip pops up.
        /// </summary>
        [Description("Occurs when the tool tip pops up.")]
        public event PopupToolTipHandler PopupToolTip
        {
            add { m_control.PopupToolTip += value; }
            remove { m_control.PopupToolTip -= value; }
        }
        /// <summary>
        /// Occurs when the tool tip is updated.
        /// </summary>
        [Description("Occurs when the tool tip is updated.")]
        public event UpdateToolTipHandler UpdateToolTip
        {
            add { m_control.UpdateToolTip += value; }
            remove { m_control.UpdateToolTip -= value; }
        }
        #endregion

        #region Fields
        Dictionary<Component, ToolTipInfo> m_tipsInfo;

        Control m_owner;
        ToolTipControl m_control;
        ToolTipControl m_trackControl;

        NativeMessageHandler m_handler;
        IntPtr nhook = IntPtr.Zero;
        RightToLeft m_rightToLeft = RightToLeft.No;
        private WindowsAPI.HookProc hook;
        static ArrayList m_tooltips;
        #endregion
    }
    #endregion

    #region ToolTipInfo
    [TypeConverter(typeof(Design.ToolTipTypeConverter))]
    [Editor(typeof(Design.ToolTipEditor), typeof(UITypeEditor))]
    public class ToolTipInfo
    {
        #region *** ToolTipItem
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ToolTipItem
        {
            #region Constructors
            /// <summary>
            /// 
            /// </summary>
            static ToolTipItem()
            {
                defaultMagrin = new Padding(2);
                minimumSize = new Size(60, 15);

                m_graphics = Graphics.FromImage(new Bitmap(1, 1));
                m_graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            }

            public ToolTipItem(ToolTipInfo owner, ToolTipItem template)
            {
                m_owner = owner;

                if (template != null)
                {
                    m_bHidden = template.m_bHidden;
                    m_text = template.m_text;
                    m_font = template.m_font;
                    m_foreColor = template.m_foreColor;
                    m_textAlign = template.m_textAlign;
                    m_textImageRelation = template.m_textImageRelation;
                    m_image = template.m_image;
                    m_imageAlign = template.m_imageAlign;
                    m_imageTransparentColor = template.m_imageTransparentColor;
                    m_imageScalingSize = template.m_imageScalingSize;
                    m_pTextMargin = template.m_pTextMargin;
                    renderHtml = template.renderHtml;
                    itemSize = template.itemSize;
                }
            }
            #endregion

            #region Methods
            public Size GetPreferredSize(int nMaxWidth, bool bDesignMode)
            {
                Size size = Size.Empty;

                if (!this.Hidden && !this.RenderHtml)
                {
                    int nAvailableWidth = nMaxWidth;

                    Size szImage = GetImageSize();

                    if (!szImage.IsEmpty)
                    {
                        size.Width = Math.Min(szImage.Width + defaultMagrin.Horizontal, nMaxWidth);
                        size.Height = szImage.Height + defaultMagrin.Vertical;

                        nAvailableWidth -= size.Width;
                    }

                    Padding textMargin = defaultMagrin + this.TextMargin;
                    if (nAvailableWidth > textMargin.Horizontal)
                    {
                        Size szText = GetTextSize(nAvailableWidth - textMargin.Horizontal);

                        if (!szText.IsEmpty)
                        {
                            size.Width += szText.Width + textMargin.Horizontal;
                            size.Height = Math.Max(size.Height, szText.Height + textMargin.Vertical);
                        }
                    }

                    if (bDesignMode)
                    {
                        int nMinWidth = Math.Min(minimumSize.Width, nMaxWidth);

                        if (size.Width < nMinWidth)
                            size.Width = nMinWidth;
                        if (size.Height < minimumSize.Height)
                            size.Height = minimumSize.Height;
                    }
                }
                else if (!this.Hidden && RenderHtml)
                {
                    size = this.itemSize;
                    if (bDesignMode)
                    {
                        int nMinWidth = Math.Min(minimumSize.Width, nMaxWidth);

                        if (size.Width < nMinWidth)
                            size.Width = nMinWidth;
                        if (size.Height < minimumSize.Height)
                            size.Height = minimumSize.Height;
                    }
                }
                return size;
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets or Sets whether a ToolTipItem should be hidden.
            /// </summary>
            [Category("Behavior"), DefaultValue(false), Description("Indicates whether a ToolTipItem should be hidden.")]
            public bool Hidden
            {
                get
                {
                    return m_bHidden;
                }
                set
                {
                    if (m_bHidden != value)
                    {
                        m_bHidden = value;

                        m_owner.OnPropertyChanged(this, ToolTipPropertyID.ItemHidden);
                    }
                }
            }

            [DefaultValue(false), Description("Indicates whether to render html or plain text")]
            public bool RenderHtml
            {
                get
                {
                    return renderHtml;
                }
                set
                {
                    if (renderHtml != value)
                    {
                        renderHtml = value;
                        m_owner.OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                    }
                }
            }

            [Description("Size of the item that renders html, if RenderHtml is set to true")]
            public Size Size
            {
                get
                {
                    return itemSize;
                }
                set
                {
                    itemSize = value;


                    m_owner.OnPropertyChanged(this, ToolTipPropertyID.ItemSizeChanged);
                }
            }

            /// <summary>
            /// Gets or Sets the text to be displayed in the SuperTooltip.
            /// </summary>
            [Category("Appearance"), DefaultValue(null), Description("Indicates the text to be displayed in the SuperTooltip.")]
            [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor)), Localizable(true)]
            public string Text
            {
                get
                {
                    return m_text;
                }
                set
                {
                    if (m_text != value)
                    {
                        m_text = value;

                        m_owner.OnPropertyChanged(this, ToolTipPropertyID.ItemTextChanged);
                    }
                }
            }
            /// <summary>
            /// Gets or Sets the font for the toolTip text.
            /// </summary>
            [Category("Appearance"), AmbientValue(null), Description("Indicates the font for the toolTip text.")]
            public Font Font
            {
                get
                {
                    if (m_font == null)
                    {
                        return Control.DefaultFont;
                    }
                    return m_font;
                }
                set
                {
                    if (m_font != value)
                    {
                        m_font = value;

                        m_owner.OnPropertyChanged(this, ToolTipPropertyID.ItemFontChanged);
                    }
                }
            }
            /// <summary>
            /// Gets or Sets the forecolor of the SuperTooltip text.
            /// </summary>
            [Category("Appearance"), AmbientValue(null), Description("Indicates the forecolor of the SuperTooltip text.")]
            public Color ForeColor
            {
                get
                {
                    if (m_foreColor == Color.Empty)
                    {
                        return m_owner.ForeColor;
                    }
                    return m_foreColor;
                }
                set
                {
                    m_foreColor = value;

                    m_owner.OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                }
            }
            /// <summary>
            /// Gets or Sets, the alignment of the SuperTooltip text.
            /// </summary>
            [Category("Appearance"), DefaultValue(typeof(ContentAlignment), "TopLeft")]
            [Description("Indicates the alignment of the tooltip text.")]
            public ContentAlignment TextAlign
            {
                get
                {
                    return m_textAlign;
                }
                set
                {
                    m_textAlign = value;

                    m_owner.OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                }
            }
            /// <summary>
            /// Gets or sets value specifying the way image and text are situated.
            /// </summary>
            [Category("Appearance"), DefaultValue(typeof(ToolTipTextImageRelation), "ImageBeforeText")]
            [Description("Indicates the way image and text are situated.")]
            public ToolTipTextImageRelation TextImageRelation
            {
                get
                {
                    return m_textImageRelation;
                }
                set
                {
                    if (m_textImageRelation != value)
                    {
                        m_textImageRelation = value;

                        m_owner.OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                    }
                }
            }
            /// <summary>
            /// Gets or Sets, the image to be shown in the tooltip.
            /// </summary>
            [Category("Appearance"), DefaultValue(typeof(Image), "")]
            [Description("Indicates the image to be shown in the tooltip.")]
            public Image Image
            {
                get
                {
                    return m_image;
                }
                set
                {
                    if (m_image != value)
                    {
                        m_image = value;

                        if (m_imageTransparentColor != Color.Empty)
                        {
                            Bitmap bmp = value as Bitmap;
                            if (bmp != null)
                            {
                                bmp.MakeTransparent(m_imageTransparentColor);
                            }
                        }

                        m_owner.OnPropertyChanged(this, ToolTipPropertyID.ItemImageChanged);
                    }
                }
            }
            /// <summary>
            /// Gets or Sets, the alignment of the image.
            /// </summary>
            [Category("Appearance"), DefaultValue(typeof(ContentAlignment), "MiddleLeft")]
            [Description("Indicates the alignment of the image.")]
            public ContentAlignment ImageAlign
            {
                get
                {
                    return m_imageAlign;
                }
                set
                {
                    m_imageAlign = value;

                    m_owner.OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                }
            }
            /// <summary>
            /// Gets or Sets, the transparency color for the image.
            /// </summary>
            [Category("Appearance"), Description("Indicates the transparency color for the image.")]
            public Color ImageTransparentColor
            {
                get
                {
                    return m_imageTransparentColor;
                }
                set
                {
                    if (m_imageTransparentColor != value)
                    {
                        m_imageTransparentColor = value;

                        if (value != Color.Empty)
                        {
                            Bitmap bmp = this.Image as Bitmap;
                            if (bmp != null)
                            {
                                bmp.MakeTransparent(value);

                                m_owner.OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Gets or Sets, the image scaling size.
            /// </summary>
            [Category("Appearance"), Description("Indicates the size of the image.")]
            public Size ImageScalingSize
            {
                get
                {
                    if (m_imageScalingSize.IsEmpty && m_image != null)
                    {
                        return m_image.Size;
                    }
                    return m_imageScalingSize;
                }
                set
                {
                    if (m_imageScalingSize != value)
                    {
                        m_imageScalingSize = value;

                        m_owner.OnPropertyChanged(this, ToolTipPropertyID.ItemImageScaleChanged);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            [Category("Appearance"), Description("Gets or sets the space between the text and item bounds")]
            public Padding TextMargin
            {
                get
                {
                    return m_pTextMargin;
                }
                set
                {
                    if (m_pTextMargin != value)
                    {
                        m_pTextMargin = value;
                        m_owner.OnPropertyChanged(this, ToolTipPropertyID.TextMarginsChanged);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            internal Rectangle Bounds
            {
                get
                {
                    return m_bounds;
                }
                set
                {
                    if (m_bounds != value)
                    {
                        if (!this.renderHtml)
                        {
                            m_bounds = value;
                        }
                        else
                        {
                            m_bounds = value;
                            m_bounds.Size = this.itemSize;
                        }                      

                        m_bounds = value;
                    }
                }
            }
            /// <summary>
            /// Gets the bounds of the image.
            /// </summary>
            [Browsable(false)]
            internal Rectangle ImageBounds
            {
                get
                {
                    Size szImage = GetImageSize();
                    Rectangle rcItem = this.Bounds;

                    Rectangle rcImage = Rectangle.Empty;

                    if ((m_textImageRelation == ToolTipTextImageRelation.ImageBeforeText) ^ (m_owner.RightToLeft == RightToLeft.Yes))
                    {
                        Point pt = new Point(rcItem.X + defaultMagrin.Left, rcItem.Y);
                        rcImage = new Rectangle(pt, szImage);
                    }
                    else
                    {
                        Padding textMargin = defaultMagrin + this.TextMargin;

                        if (szImage.Width + defaultMagrin.Horizontal + textMargin.Horizontal < this.Bounds.Width)
                        {
                            Point pt = new Point(this.Bounds.Width - (szImage.Width + defaultMagrin.Right), rcItem.Y);
                            rcImage = new Rectangle(pt, szImage);
                        }
                    }

                    if (!rcImage.IsEmpty)
                    {
                        switch (this.ImageAlign)
                        {
                            case ContentAlignment.TopCenter:
                                rcImage.Y += defaultMagrin.Top;
                                rcImage.X = (rcItem.Width - szImage.Width) / 2;
                                break;
                            case ContentAlignment.TopLeft:
                            case ContentAlignment.TopRight:
                                rcImage.Y += defaultMagrin.Top;
                                break;
                            case ContentAlignment.BottomCenter:
                                rcImage.X = (rcItem.Width - szImage.Width) / 2;
                                rcImage.Y += rcItem.Height - (szImage.Height + defaultMagrin.Bottom);
                                break;
                            case ContentAlignment.BottomLeft:
                            case ContentAlignment.BottomRight:
                                rcImage.Y += rcItem.Height - (szImage.Height + defaultMagrin.Bottom);
                                break;
                            case ContentAlignment.MiddleCenter:
                                rcImage.Y += (rcItem.Height - szImage.Height) / 2;
                                rcImage.X = (rcItem.Width - szImage.Width) / 2;
                                break;
                            default:
                                rcImage.Y += (rcItem.Height - szImage.Height) / 2;
                                break;
                        }
                    }

                    return rcImage;
                }
            }
            /// <summary>
            /// Gets the bounds of the toolTip text.
            /// </summary>
            [Browsable(false)]
            internal Rectangle TextBounds
            {
                get
                {
                    Rectangle rc = this.Bounds;
                    Padding textMargins = defaultMagrin + this.TextMargin;

                    rc.X += textMargins.Left;
                    rc.Width -= textMargins.Horizontal;

                    rc.Y += textMargins.Top;
                    rc.Height -= textMargins.Vertical;

                    Size szImage = GetImageSize();

                    if (szImage.Width > 0)
                    {
                        int imageWidth = szImage.Width + defaultMagrin.Horizontal;

                        if ((m_textImageRelation == ToolTipTextImageRelation.ImageBeforeText) ^ (m_owner.RightToLeft == RightToLeft.Yes))
                        {
                            rc.X += imageWidth;
                        }
                        rc.Width -= imageWidth;
                    }

                    return rc;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            internal ContentAlignment TextAlignInternal
            {
                get
                {
                    ContentAlignment textAlign = m_textAlign;

                    if (m_owner.RightToLeft == RightToLeft.Yes)
                    {
                        switch (textAlign)
                        {
                            case ContentAlignment.TopLeft:
                                textAlign = ContentAlignment.TopRight;
                                break;
                            case ContentAlignment.TopRight:
                                textAlign = ContentAlignment.TopLeft;
                                break;
                            case ContentAlignment.MiddleLeft:
                                textAlign = ContentAlignment.MiddleRight;
                                break;
                            case ContentAlignment.MiddleRight:
                                textAlign = ContentAlignment.MiddleLeft;
                                break;
                            case ContentAlignment.BottomLeft:
                                textAlign = ContentAlignment.BottomRight;
                                break;
                            case ContentAlignment.BottomRight:
                                textAlign = ContentAlignment.BottomLeft;
                                break;
                        }
                    }
                    return textAlign;
                }
            }

            #endregion

            #region Implementation
            /// <summary>
            /// Indicates whether the current value of the Font property is to be serialized.
            /// </summary>
            /// <returns></returns>
            bool ShouldSerializeFont()
            {
                return m_font != null;
            }
            /// <summary>
            /// Indicates whether the current value of the ForeColor property is to be serialized.
            /// </summary>
            /// <returns></returns>
            bool ShouldSerializeForeColor()
            {
                return m_foreColor != Color.Empty;
            }
            /// <summary>
            /// Indicates whether the current value of the Image Transparency color property is to be serialized.
            /// </summary>
            /// <returns></returns>
            bool ShouldSerializeImageTransparentColor()
            {
                return m_imageTransparentColor != Color.Empty;
            }
            /// <summary>
            /// Indicates whether the current value of the ImageScaling Size property is to be serialized.
            /// </summary>
            /// <returns></returns>
            bool ShouldSerializeImageScalingSize()
            {
                return !m_imageScalingSize.IsEmpty;
            }
            /// <summary>
            /// 
            /// </summary>
            bool ShouldSerializeTextMargin()
            {
                return m_pTextMargin != Padding.Empty;
            }

            /// <summary>
            /// Gets the size of the image.
            /// </summary>
            /// <returns></returns>
            Size GetImageSize()
            {
                if (m_image != null)
                {
                    if (m_imageScalingSize.IsEmpty)
                    {
                        return m_image.Size;
                    }
                    return m_imageScalingSize;
                }
                return Size.Empty;
            }
            /// <summary>
            /// Gets the maximum size (width) of the text.
            /// </summary>
            /// <param name="nMaxWidth"></param>
            /// <returns></returns>
            Size GetTextSize(int nMaxWidth)
            {
                Size szText = Size.Empty;

                if (this.Text != null)
                {
                    szText = TextRenderer.MeasureText(m_graphics, this.Text, this.Font, new Size(nMaxWidth, 0), TXTFORMAT.COMMON);

                    if (szText.Width > nMaxWidth)
                    {
                        szText.Width = nMaxWidth;
                    }
                }
                return szText;
            }
            #endregion

            #region Fields
            ToolTipInfo m_owner = null;

            bool m_bHidden = false;

            Image m_image = null;
            string m_text = null;

            bool renderHtml = false;
            Size itemSize = new Size(20, 20);

            Font m_font = null;
            Color m_foreColor = Color.Empty;
            Color m_imageTransparentColor = Color.Empty;

            ContentAlignment m_imageAlign = ContentAlignment.MiddleLeft;
            ContentAlignment m_textAlign = ContentAlignment.TopLeft;

            /// <summary>
            /// Specifies the way image and text are situated.
            /// </summary>
            ToolTipTextImageRelation m_textImageRelation = ToolTipTextImageRelation.ImageBeforeText;

            Rectangle m_bounds;
            Size m_imageScalingSize = Size.Empty;
            Padding m_pTextMargin = Padding.Empty;

            static Padding defaultMagrin;
            static Size minimumSize;
            static Graphics m_graphics;
            #endregion
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public ToolTipInfo()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        public ToolTipInfo(ToolTipInfo template)
        {
            if (template != null)
            {
                m_backColor = template.m_backColor;
                m_foreColor = template.m_foreColor;
                m_borderColor = template.m_borderColor;
                m_bSeparator = template.m_bSeparator;
                m_rightToLeft = template.m_rightToLeft;

                m_ttHeader = new ToolTipItem(this, template.Header);
                m_ttBody = new ToolTipItem(this, template.Body);
                m_ttFooter = new ToolTipItem(this, template.Footer);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets backColor of SuperToolTip.
        /// </summary>
        [Category("Appearance"), DefaultValue(typeof(Color), "LightBlue")]
        [Description("Indicates the backColor of SuperToolTip")]
        public Color BackColor
        {
            get
            {
                return m_backColor;
            }
            set
            {
                if (m_backColor != value)
                {
                    m_backColor = value;
                    OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                }
            }
        }
        /// <summary>
        /// Gets or Sets forecolor of SuperToolTip.
        /// </summary>
        [Category("Appearance"), Description("Indicates the forecolor of SuperToolTip.")]
        public Color ForeColor
        {
            get
            {
                if (m_foreColor == Color.Empty)
                {
                    return Control.DefaultForeColor;
                }
                return m_foreColor;
            }
            set
            {
                if (m_foreColor != value)
                {
                    m_foreColor = value;
                    OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Category("Appearance"), Description("Indicates the border color of SuperToolTip.")]
        public Color BorderColor
        {
            get
            {
                if (m_borderColor == Color.Empty)
                {
                    return SystemColors.ControlDark;
                }
                return m_borderColor;
            }
            set
            {
                if (m_borderColor != value)
                {
                    m_borderColor = value;
                    OnPropertyChanged(this, ToolTipPropertyID.Unspecified);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Category("Appearance"), Description("Indicates the appearance of the Footer's separator.")]
        [DefaultValue(true)]
        public bool Separator
        {
            get
            {
                if (!this.Footer.Hidden)
                {
                    return m_bSeparator;
                }
                return false;
            }
            set
            {
                if (m_bSeparator != value)
                {
                    m_bSeparator = value;
                    OnPropertyChanged(this, ToolTipPropertyID.SeparatorChanged);
                }
            }
        }
        /// <summary>
        /// Gets the Header ToolTipItem.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolTipItem Header
        {
            get
            {
                if (m_ttHeader == null)
                {
                    m_ttHeader = new ToolTipItem(this, null);
                }
                return m_ttHeader;
            }
        }
        /// <summary>
        /// Gets the Body (Description) ToolTipItem.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolTipItem Body
        {
            get
            {
                if (m_ttBody == null)
                {
                    m_ttBody = new ToolTipItem(this, null);
                }
                return m_ttBody;
            }
        }
        /// <summary>
        /// Gets or Sets Footer ToolTipItem
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolTipItem Footer
        {
            get
            {
                if (m_ttFooter == null)
                {
                    m_ttFooter = new ToolTipItem(this, null);
                }
                return m_ttFooter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal RightToLeft RightToLeft
        {
            get
            {
                return m_rightToLeft;
            }
            set
            {
                if (m_rightToLeft != value)
                {
                    m_rightToLeft = value;
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Indicates whether the current value of the ForeColor property is to be serialized.
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeForeColor()
        {
            return m_foreColor != Color.Empty;
        }

        bool ShouldSerializeBorderColor()
        {
            return m_borderColor != Color.Empty;
        }

        void OnPropertyChanged(object sender, ToolTipPropertyID id)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, id);
            }
        }
        #endregion

        #region Events
        public event ToolTipPropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Fields

        Color m_backColor = Color.LightBlue;
        Color m_foreColor = Color.Empty;
        Color m_borderColor = Color.Empty;
        bool m_bSeparator = true;

        ToolTipItem m_ttHeader = null;
        ToolTipItem m_ttFooter = null;
        ToolTipItem m_ttBody = null;

        RightToLeft m_rightToLeft = RightToLeft.No;

        #endregion
    }
    #endregion

    #region ToolTipControl
    [Designer(typeof(Design.ToolTipDesigner), typeof(IRootDesigner))]
    [ToolboxItem(false)]
   public class ToolTipControl : Control
    {
        #region *** ToolTipLayout
        class ToolTipLayout : LayoutEngine
        {
            #region Constructors
            protected ToolTipLayout()
            {
            }
            #endregion

            #region Overrides
            /// <summary>
            /// 
            /// </summary>
            /// <param name="child"></param>
            /// <param name="specified"></param>
            public override void InitLayout(object child, BoundsSpecified specified)
            {
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="container"></param>
            /// <param name="layoutEventArgs"></param>
            /// <returns></returns>
            public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
            {   

                ToolTipControl toolTip = container as ToolTipControl;
                if (toolTip != null)
                {
                    int nWidth = 0;
                    int nHeight = 0;

                    if (toolTip.MaxWidth == 0 || toolTip.MaxWidth > 4 * VertexRadius)
                    {
                        int nMaxWidth = toolTip.MaxWidth == 0 ? Int32.MaxValue : toolTip.MaxWidth - 4 * VertexRadius;

                        ToolTipInfo toolTipInfo = toolTip.Info;

                        Size szBody = toolTipInfo.Body.GetPreferredSize(nMaxWidth, toolTip.m_bDesignMode);
                        Size szHeader = toolTipInfo.Header.GetPreferredSize(nMaxWidth, toolTip.m_bDesignMode);
                        Size szFooter = toolTipInfo.Footer.GetPreferredSize(nMaxWidth, toolTip.m_bDesignMode);

                        nWidth = Math.Max(szBody.Width, Math.Max(szHeader.Width, szFooter.Width));

                        int x = VertexRadius;
                        int y = VertexRadius;

                        int nItemHeight = szHeader.Height;
                        if (nItemHeight > 0)
                        {
                            toolTipInfo.Header.Bounds = new Rectangle(x, y, nWidth, nItemHeight);
                            nHeight += nItemHeight;
                        }

                        nItemHeight = szBody.Height;
                        if (nItemHeight > 0)
                        {
                            toolTipInfo.Body.Bounds = new Rectangle(x, y + nHeight, nWidth, nItemHeight);
                            nHeight += nItemHeight;
                        }

                        if (toolTipInfo.Separator)
                        {
                            nHeight += DEF_SEP_HEIGHT;
                        }

                        nItemHeight = szFooter.Height;
                        if (nItemHeight > 0)
                        {
                            toolTipInfo.Footer.Bounds = new Rectangle(x, y + nHeight, nWidth, nItemHeight);
                            nHeight += nItemHeight;
                        }
                    }

                    if (toolTip.Style == SuperToolTip.SuperToolTipStyle.Normal)
                    {

                        toolTip.Size = new Size(nWidth + 2 * VertexRadius, nHeight + 2 * VertexRadius);

                    }
                    else
                    {
                        toolTip.contentSize = new Size(nWidth + 2 * VertexRadius, nHeight + 2 * VertexRadius);
                        toolTip.balloonPointerHeight = ((nHeight + 2 * VertexRadius) * 20) / 100;
                        toolTip.balloonPointerWidth = ((nWidth + 2 * VertexRadius) * 10) / 100;
                        if (toolTip.balloonPointerHeight < 6)
                            toolTip.balloonPointerHeight = 6;
                        if (toolTip.balloonPointerWidth < 5)
                            toolTip.balloonPointerWidth = 5;
                        if (toolTip.balloonPointerHeight < toolTip.balloonPointerWidth)
                            toolTip.balloonPointerHeight = toolTip.balloonPointerWidth + 3;
                        Point pt = Cursor.Position;
                        Screen scr = Screen.FromPoint(pt);
                        Rectangle rect = scr.Bounds;
                        toolTip.Size = new Size(nWidth + 2 * VertexRadius, nHeight + 2 * VertexRadius + toolTip.balloonPointerHeight);
                        if (rect.Contains(pt))
                        {
                            if (pt.Y - toolTip.Height < rect.Top)
                            {
                                toolTip.Info.Header.Bounds = new Rectangle(toolTip.Info.Header.Bounds.X, toolTip.Info.Header.Bounds.Y + toolTip.balloonPointerHeight, toolTip.Info.Header.Bounds.Width, toolTip.Info.Header.Bounds.Height);
                                toolTip.Info.Body.Bounds = new Rectangle(toolTip.Info.Body.Bounds.X, toolTip.Info.Body.Bounds.Y + toolTip.balloonPointerHeight, toolTip.Info.Body.Bounds.Width, toolTip.Info.Body.Bounds.Height);
                                toolTip.Info.Footer.Bounds = new Rectangle(toolTip.Info.Footer.Bounds.X, toolTip.Info.Footer.Bounds.Y + toolTip.balloonPointerHeight, toolTip.Info.Footer.Bounds.Width, toolTip.Info.Footer.Bounds.Height);
                            }
                        }
                    }
                    toolTip.Invalidate();
                }
                return false;
            }
            #endregion

            #region Fields
            public static ToolTipLayout Instance = new ToolTipLayout();
            #endregion
        }
        #endregion

        #region Constants

        const int VertexRadius = 2;
        const int CursorOffset = 4;
        const int DEF_SEP_HEIGHT = 2;

        const int TTDT_AUTOMATIC = 0;
        const int TTDT_RESHOW = 1;
        const int TTDT_AUTOPOP = 2;
        const int TTDT_INITIAL = 3;

        const SetWindowPosFlags SWP_MOVE = SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER;
        const SetWindowPosFlags SWP_ZORDER = SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE;
        const int FADING_DELAY = 500;

        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        static ToolTipControl()
        {
            m_formatFlags = new Dictionary<ContentAlignment, TextFormatFlags>();
            m_formatFlags[ContentAlignment.TopLeft] = TextFormatFlags.Default;
            m_formatFlags[ContentAlignment.TopCenter] = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
            m_formatFlags[ContentAlignment.TopRight] = TextFormatFlags.Top | TextFormatFlags.Right;
            m_formatFlags[ContentAlignment.MiddleLeft] = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
            m_formatFlags[ContentAlignment.MiddleCenter] = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
            m_formatFlags[ContentAlignment.MiddleRight] = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
            m_formatFlags[ContentAlignment.BottomLeft] = TextFormatFlags.Bottom | TextFormatFlags.Left;
            m_formatFlags[ContentAlignment.BottomCenter] = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
            m_formatFlags[ContentAlignment.BottomRight] = TextFormatFlags.Bottom | TextFormatFlags.Right;
        }
        /// <summary>
        /// Creates instance of tooltip control
        /// </summary>
        public ToolTipControl(SuperToolTip tipsProvider)
        {
            this.DoubleBuffered = true;

            m_timer = new Timer();

            m_ttInfo = null;
            m_ttProvider = tipsProvider;
            m_popupHandler = new PopupHandler(OnPopup);
            m_popHandler = new EventHandler(OnPop);
            m_tickHandler = new EventHandler(OnTick);

            m_tools = new Dictionary<IntPtr, Component>();
            m_services = new Dictionary<Type, IToolTipService>();

            m_bDesignMode = false;

            m_toolInfo = new TOOLINFO();
            m_toolInfo.cbSize = (uint)Marshal.SizeOf(m_toolInfo);
            m_toolInfo.uFlags = (uint)(TTF.TTF_IDISHWND);
            m_toolInfo.hwnd = IntPtr.Zero;
            m_toolInfo.uId = IntPtr.Zero; //tool's handle;
            m_toolInfo.rect = new RECT();
            m_toolInfo.hinst = IntPtr.Zero;
            m_toolInfo.lpszText = Marshal.StringToHGlobalAuto("tooltip");
            m_toolInfo.lParam = IntPtr.Zero;

            AddToolTipService(typeof(ToolStripItem), new ToolStripItemsToolTipService(this));
        }
        /// <summary>
        /// Creates instance of child control for designer
        /// </summary>
        public ToolTipControl()
            : this(null)
        {
            m_bDesignMode = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        public void AddTool(Component tool)
        {
            if (tool != null)
            {
                if (tool is Control)
                {
                    Control toolControl = (Control)tool;

                    toolControl.HandleCreated += new EventHandler(OnToolHandleCreated);
                    toolControl.HandleDestroyed += new EventHandler(OnToolHandleDestroyed);

                    if (toolControl.IsHandleCreated)
                    {
                        OnToolHandleCreated(toolControl, EventArgs.Empty);
                    }
                }
                else
                {
                    IToolTipService svc = GetToolTipService(tool.GetType());

                    if (svc != null)
                    {
                        svc.AddComponent(tool);
                    }
                }
            }
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (this.Style == SuperToolTip.SuperToolTipStyle.Office2013Style)
                this.Location =new Point( loc.X + locX , this.loc.Y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        public void DelTool(Component tool)
        {
            if (tool != null)
            {
                if (tool is Control)
                {
                    Control toolControl = (Control)tool;

                    toolControl.HandleCreated -= new EventHandler(OnToolHandleCreated);
                    toolControl.HandleDestroyed -= new EventHandler(OnToolHandleDestroyed);

                    if (toolControl.IsHandleCreated)
                    {
                        OnToolHandleDestroyed(toolControl, EventArgs.Empty);
                    }
                }
                else
                {
                    IToolTipService svc = GetToolTipService(tool.GetType());

                    if (svc != null)
                    {
                        svc.RemoveComponent(tool);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public bool Contains(IntPtr hWnd)
        {
            return m_tools.ContainsKey(hWnd);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttInfo"></param>
        public void ShowToolTip(ToolTipInfo ttInfo, Point position, int displayTime)
        {
            if (!this.IsDisposed)
            {
                this.Info = ttInfo;

                IntPtr hWnd = this.Handle;

                TOOLINFO ti = m_toolInfo;
                ti.uFlags = (uint)TTF.TTF_TRACK;

                position = AdjustTooltipLocation(position);
                IntPtr dwPos = (IntPtr)WindowsAPI.MAKELONG(position.X, position.Y);

                WindowsAPI.SendMessage(hWnd, (int)TTM.TTM_TRACKPOSITION, 0, dwPos);
                WindowsAPI.SendMessage(hWnd, (int)TTM.TTM_ADDTOOLW, 0, ref ti);
                WindowsAPI.SendMessage(hWnd, (int)TTM.TTM_TRACKACTIVATE, 1, ref ti);
                if (this.Style == SuperToolTip.SuperToolTipStyle.Balloon)
                {
                    this.SetBalloonLocation(null);
                    this.Invalidate();
                }
                if (displayTime > 0)
                {
                    m_timer.Interval = displayTime;
                    m_timer.Tick += m_tickHandler;
                    m_timer.Start();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Point AdjustTooltipLocation(Point position)
        {
            Rectangle Bounds = Screen.FromPoint(position).WorkingArea;

            this.PerformLayout();

            if (position.X + this.Bounds.Width > Bounds.Right)
                position.X = Bounds.Right - (this.Bounds.Width + 10);
 
            if (position.Y + this.Bounds.Height > Bounds.Bottom) 
                position.Y = Bounds.Bottom - (this.Bounds.Height + 10 );

            return position;
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideToolTip()
        {
            m_timer.Stop();
            m_timer.Tick -= m_tickHandler;

            if (this.IsHandleCreated)
            {
                IntPtr hWnd = this.Handle;

                TOOLINFO ti = new TOOLINFO();
                ti.cbSize = (uint)Marshal.SizeOf(ti);

                if (WindowsAPI.SendMessage(hWnd, (int)TTM.TTM_GETCURRENTTOOLW, 0, ref ti) != 0)
                {
                    if ((ti.uFlags & (uint)TTF.TTF_TRACK) != 0)
                    {
                        WindowsAPI.SendMessage(hWnd, (int)TTM.TTM_TRACKACTIVATE, 0, ref ti);
                        WindowsAPI.SendMessage(hWnd, (int)TTM.TTM_DELTOOLW, 0, ref ti);
                    }
                    else
                    {
                        WindowsAPI.SendMessage(hWnd, (int)TTM.TTM_POP, 0, ref ti);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="service"></param>
        public void AddToolTipService(Type componentType, IToolTipService service)
        {
            if (!m_services.ContainsKey(componentType))
            {
                m_services[componentType] = service;

                service.Popup += m_popupHandler;
                service.Pop += m_popHandler;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public IToolTipService GetToolTipService(Type componentType)
        {
            IToolTipService svc = null;

            foreach (Type type in m_services.Keys)
            {
                if (type.IsAssignableFrom(componentType))
                {
                    svc = m_services[type];
                    break;
                }
            }

            return svc;
        }
        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)Msg.WM_NOTIFY:
                    {
                        NMHDR hdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
                        switch (hdr.code)
                        {       
                            case (int)TTN.TTN_SHOW:
                                OnTtnShow(GetTool(hdr.hwndFrom, (IntPtr)hdr.idFrom), ref m);
                                return;
                            case (int)TTN.TTN_POP:
                                OnTtnClose(GetTool(hdr.hwndFrom, (IntPtr)hdr.idFrom));
                                return;
                        }
                    }
                    break;
                case (int)TTM.TTM_WINDOWFROMPOINT:
                    OnTtmWindowFromPoint(ref m);
                    return;
                case NativeMethods.WM_WINDOWPOSCHANGED:
                    OnWmWindowPosChanged(ref m);
                    return;
            }

            base.WndProc(ref m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                HideToolTip();

                foreach (IToolTipService svc in m_services.Values)
                {
                    svc.Popup -= m_popupHandler;
                    svc.Pop -= m_popHandler;

                    svc.Dispose();
                }

                m_services.Clear();
                m_services = null;

                Marshal.FreeHGlobal(m_toolInfo.lpszText);
            }

            base.Dispose(disposing);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!m_bDesignMode)
            {
                int style = WindowsAPI.GetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_STYLE);
                WindowsAPI.SetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_STYLE, style & ~(int)WindowStyles.WS_BORDER);
            }

            UpdateRegion();
            UpdateDelayTime();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle rc = this.ClientRectangle;

            ToolTipInfo info = this.Info;
            Color backColor = Color.Empty;
            if (m_ttProvider != null && m_ttProvider.VisualStyle == SuperToolTip.Appearance.Metro)
                backColor = m_ttProvider.MetroColor;
            else
                backColor = this.Info.BackColor;
            if (m_bGradientBackGround)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rc, Color.White, backColor, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, e.ClipRectangle);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brush, e.ClipRectangle);
                }
            }

            // Draw border
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(info.BorderColor))
            {
                if (this.style == SuperToolTip.SuperToolTipStyle.Normal)
                {
                    e.Graphics.DrawPolygon(pen, GetRoundedPolygon(rc));
                }
                else if (this.Style == SuperToolTip.SuperToolTipStyle.Balloon)
                {
                    e.Graphics.DrawPolygon(pen, GetBalloonPolygon(new Rectangle(new Point(0, 0), contentSize)));
                }
                else
                {
                    using (Pen borderPen = new Pen(Color.LightGray))
                    {
                        Rectangle rect = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
                        {
                            e.Graphics.DrawRectangle(borderPen, rect);
                        }
                    }
                }

                if (info.Separator)
                {
                    int y = info.Footer.Bounds.Y - DEF_SEP_HEIGHT;

                    using (Pen sepPen1 = new Pen(Color.FromArgb(32, Color.Black)))
                    {
                        using (Pen sepPen2 = new Pen(Color.FromArgb(160, Color.White)))
                        {
                            e.Graphics.DrawLine(sepPen1, info.Footer.Bounds.Left, y, info.Footer.Bounds.Right, y);
                            e.Graphics.DrawLine(sepPen2, info.Footer.Bounds.Left, y + 1, info.Footer.Bounds.Right, y + 1);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            GraphicsState gState = e.Graphics.Save();

            bool bRtl = (this.Info.RightToLeft == RightToLeft.Yes);

            PaintItem(e, this.Info.Header, bRtl, ToolTipRegion.Header);
            PaintItem(e, this.Info.Body, bRtl, ToolTipRegion.Body);
            PaintItem(e, this.Info.Footer, bRtl, ToolTipRegion.Footer);

            e.Graphics.Restore(gState);
        }
        /// <summary>
        /// 
        /// </summary>
        public override void ResetBackColor()
        {
            this.BackColor = Color.LightBlue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public ToolTipInfo Info
        {
            get
            {
                if (m_ttInfo == null)
                {
                    SetToolTipInfo(new ToolTipInfo(null));
                }
                return m_ttInfo;
            }
            set
            {
                SetToolTipInfo(value);
            }
        }
        /// <summary>
        /// Gets/sets delay time in seconds
        /// </summary>
        public int ToolTipDuration
        {
            get
            {
                return m_nDelayTime;
            }
            set
            {
                if (value < 0) value = 0;
                if (value > 32) value = 32;

                if (m_nDelayTime != value)
                {
                    m_nDelayTime = value;
                    UpdateDelayTime();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int InitialDelay
        {
            get
            {
                return m_nInitialDelay;
            }
            set
            {
                if (m_nInitialDelay != value)
                {
                    m_nInitialDelay = value;
                    UpdateDelayTime();
                }
            }
        }
        /// <summary>
        /// Gets delay time in milliseconds
        /// </summary>
        public int DisplayTime
        {
            get
            {
                if (m_nDelayTime <= 0 && this.IsHandleCreated)
                {
                    return (int)WindowsAPI.SendMessage(this.Handle, (int)TTM.TTM_GETDELAYTIME, TTDT_AUTOPOP, IntPtr.Zero);
                }
                return m_nDelayTime * 1000;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int MaxWidth
        {
            get
            {
                return m_maxWidth;
            }
            set
            {
                m_maxWidth = value;
            }
        }
        public bool GradientBackGround
        {
            get
            {
                return m_bGradientBackGround;
            }
            set
            {
                if (m_bGradientBackGround!=value )
                  m_bGradientBackGround = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public SuperToolTip.FadingType UseFading
        {
            get
            {
                return m_UseFading;
            }
            set
            {
                m_UseFading = value;
            }
        }
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public SuperToolTip.SuperToolTipStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override LayoutEngine LayoutEngine
        {
            get
            {
                return ToolTipLayout.Instance;
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
                if (!m_bDesignMode)
                {
                    result.ClassName = "tooltips_class32";
                    result.Style = unchecked((int)WindowStyles.WS_POPUP) | (int)ToolTipStyles.TTS_NOFADE | (int)ToolTipStyles.TTS_NOANIMATE | (int)ToolTipStyles.TTS_ALWAYSTIP;
                    result.ExStyle |= (int)WindowExStyles.WS_EX_TOPMOST;
                }
                return result;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// 
        /// </summary>
        void UpdateDelayTime()
        {
            if (this.IsHandleCreated)
            {
                int delayTime = m_nDelayTime > 0 ? m_nDelayTime * 1000 : -1;
                WindowsAPI.SendMessage(this.Handle, (int)TTM.TTM_SETDELAYTIME, TTDT_AUTOPOP, new IntPtr(WindowsAPI.MAKELONG(delayTime, 0)));

                int initialDelay = m_nInitialDelay > 0 ? m_nInitialDelay : -1;
                WindowsAPI.SendMessage(this.Handle, (int)TTM.TTM_SETDELAYTIME, TTDT_INITIAL, new IntPtr(WindowsAPI.MAKELONG(initialDelay, 0)));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        void UpdateRegion()
        {
            if (this.style == SuperToolTip.SuperToolTipStyle.Normal)
            {
                this.Region = GetRoundedRegion(this.ClientRectangle);
            }
            else if (this.Style == SuperToolTip.SuperToolTipStyle.Balloon)
            {
                this.Region = GetBalloonRegion(this.ClientRectangle, new Rectangle(new Point(0, 0), contentSize));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        Region GetRoundedRegion(Rectangle rc)
        {
            Region rgn = new Region(rc);

            if (rc.Width > 2 * VertexRadius && rc.Height > 2 * VertexRadius)
            {
                int iLeft = rc.X;
                int iTop = rc.Y;
                int iRight = rc.Right;
                int iBottom = rc.Bottom;

                Region rgnResult = new Region(rc);

                for (int i = 0; i < VertexRadius; i++)
                {
                    rgn.Exclude(new Rectangle(iLeft + i, iTop, 1, VertexRadius - i));
                    rgn.Exclude(new Rectangle(iLeft + i, iBottom, 1, i - VertexRadius));
                    rgn.Exclude(new Rectangle(iRight - i, iTop, -1, VertexRadius - i));
                    rgn.Exclude(new Rectangle(iRight - i, iBottom, -1, i - VertexRadius));
                }
            }

            return rgn;
        }

        Region GetBalloonRegion(Rectangle clientRc, Rectangle rc)
        {
            Region rgn = new Region(clientRc);
            if (rc.Width > 2 * VertexRadius && rc.Height > 2 * VertexRadius)
            {
                int iLeft = rc.X;
                int iTop = rc.Y;
                int iRight = rc.Right;
                int iBottom = rc.Bottom;

                Region rgnResult = new Region(rc);

                for (int i = 0; i < VertexRadius; i++)
                {
                    rgn.Exclude(new Rectangle(iLeft + i, iTop, 1, VertexRadius - i));
                    rgn.Exclude(new Rectangle(iLeft + i, iBottom, 1, i - VertexRadius));
                    rgn.Exclude(new Rectangle(iRight - i, iTop, -1, VertexRadius - i));
                    rgn.Exclude(new Rectangle(iRight - i, iBottom, -1, i - VertexRadius));
                }
                if (bottomLeft)
                {
                    bottomLeft = false;
                    rgn.Exclude(new Rectangle(clientRc.X, clientRc.Top, clientRc.Width - (2 * balloonPointerWidth), balloonPointerHeight));
                    rgn.Exclude(new Rectangle(clientRc.X + (this.Width - balloonPointerWidth), clientRc.Top, balloonPointerWidth, balloonPointerHeight));
                    for (int i = 0; i < balloonPointerHeight - 1; i++)
                    {
                        rgn.Exclude(new Rectangle(clientRc.X + (this.Width - (2 * balloonPointerWidth)) + i, clientRc.Top - 1, 1, balloonPointerHeight - i));
                    }
                }
                else if (bottomRight)
                {
                    bottomRight = false;
                    rgn.Exclude(new Rectangle(clientRc.X, clientRc.Top, balloonPointerWidth, balloonPointerHeight));
                    rgn.Exclude(new Rectangle(clientRc.X + (2 * balloonPointerWidth), clientRc.Top, this.Width - (2 * balloonPointerWidth), balloonPointerHeight));
                    for (int i = 0; i < balloonPointerHeight - 1; i++)
                    {
                        rgn.Exclude(new Rectangle(clientRc.X + ((2 * balloonPointerWidth) - i), clientRc.Top, 1, balloonPointerHeight - i));
                    }
                }
                else if (topLeft)
                {
                    topLeft = false;
                    rgn.Exclude(new Rectangle(clientRc.X, clientRc.Bottom - balloonPointerHeight, this.Width - (2 * balloonPointerWidth), balloonPointerHeight));
                    rgn.Exclude(new Rectangle(clientRc.X + (this.Width - balloonPointerWidth), clientRc.Bottom - balloonPointerHeight, balloonPointerWidth, balloonPointerHeight));
                    for (int i = 0; i < balloonPointerHeight - 1; i++)
                    {
                        rgn.Exclude(new Rectangle(clientRc.Right - (2 * balloonPointerWidth) + i, (rc.Bottom + 1) + i, 1, balloonPointerHeight - i));
                    }
                }
                else
                {
                    rgn.Exclude(new Rectangle(clientRc.X, clientRc.Bottom - balloonPointerHeight, balloonPointerWidth, balloonPointerHeight));
                    rgn.Exclude(new Rectangle(clientRc.X + (2 * balloonPointerWidth), clientRc.Bottom - balloonPointerHeight, clientRc.Right - clientRc.X - (2 * balloonPointerWidth), balloonPointerHeight));
                    for (int i = 0; i < balloonPointerHeight - 1; i++)
                    {
                        rgn.Exclude(new Rectangle(clientRc.X + ((2 * balloonPointerWidth) - i), clientRc.Bottom - (balloonPointerHeight - i), 1, balloonPointerHeight - i));
                    }
                }
            }
            return rgn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        Point[] GetRoundedPolygon(Rectangle rc)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right - 1;
            int iBottom = rc.Bottom - 1;

            Point[] points = new Point[]
				{
					new Point(iLeft, iTop+VertexRadius),
					new Point(iLeft+VertexRadius, iTop),
					new Point(iRight-VertexRadius, iTop),
					new Point(iRight, iTop+VertexRadius),
					new Point(iRight, iBottom-VertexRadius),
					new Point(iRight-VertexRadius, iBottom),
					new Point(iLeft+VertexRadius, iBottom),
					new Point(iLeft, iBottom-VertexRadius),
				};

            return points;
        }

        Point[] GetBalloonPolygon(Rectangle rc)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right - 1;
            int iBottom = rc.Bottom - 1;
            Screen scr = Screen.FromPoint(cursorPoint);
            Rectangle scrRect = scr.Bounds;
            Point[] points = new Point[] { Point.Empty };
            if (scrRect.Contains(cursorPoint) && cursorPoint.Y - this.Height > scrRect.Top && cursorPoint.X + this.Width > scrRect.Right)
            {
                points = new Point[]
				{
					new Point(iLeft, iTop+VertexRadius),
					new Point(iLeft+VertexRadius, iTop),
					new Point(iRight-VertexRadius, iTop),
					new Point(iRight, iTop+VertexRadius),
					new Point(iRight, iBottom-VertexRadius),
					new Point(iRight-VertexRadius, iBottom),
                    new Point(iRight-balloonPointerWidth,iBottom),
                    new Point(iRight-balloonPointerWidth,iBottom+balloonPointerWidth),
                    new Point(iRight-(2*balloonPointerWidth),iBottom),
    				new Point(iLeft+VertexRadius, iBottom),
					new Point(iLeft, iBottom-VertexRadius),
				};
            }
            else if (scrRect.Contains(cursorPoint) && cursorPoint.Y - this.Height < scrRect.Top && cursorPoint.X + this.Width > scrRect.Right)
            {
                iTop = iTop + balloonPointerHeight;
                iBottom = iBottom + balloonPointerHeight;
                points = new Point[]
				{
					new Point(iLeft, iTop+VertexRadius),
					new Point(iLeft+VertexRadius, iTop),
                    new Point(iRight-(2*balloonPointerWidth),iTop),
                    new Point(iRight-balloonPointerWidth,iTop-balloonPointerWidth),
                    new Point(iRight-balloonPointerWidth,iTop),
					new Point(iRight-VertexRadius, iTop),
					new Point(iRight, iTop+VertexRadius),
					new Point(iRight, iBottom-VertexRadius),
					new Point(iRight-VertexRadius, iBottom),
    				new Point(iLeft+VertexRadius, iBottom),
					new Point(iLeft, iBottom-VertexRadius),
				};
            }
            else if (scrRect.Contains(cursorPoint) && cursorPoint.Y - this.Height < scrRect.Top && cursorPoint.X + this.Width < scrRect.Right)
            {
                iTop = iTop + balloonPointerHeight;
                iBottom = iBottom + balloonPointerHeight;
                points = new Point[]
				{
					new Point(iLeft, iTop+VertexRadius),
					new Point(iLeft+VertexRadius, iTop),
                    new Point(iLeft+balloonPointerWidth,iTop),
                    new Point(iLeft+balloonPointerWidth,iTop-balloonPointerWidth),
                    new Point(iLeft+(2*balloonPointerWidth),iTop),
					new Point(iRight-VertexRadius, iTop),
					new Point(iRight, iTop+VertexRadius),
					new Point(iRight, iBottom-VertexRadius),
					new Point(iRight-VertexRadius, iBottom),
    				new Point(iLeft+VertexRadius, iBottom),
					new Point(iLeft, iBottom-VertexRadius),
				};
            }
            else
            {
                points = new Point[]
				{
					new Point(iLeft, iTop+VertexRadius),
					new Point(iLeft+VertexRadius, iTop),
					new Point(iRight-VertexRadius, iTop),
					new Point(iRight, iTop+VertexRadius),
					new Point(iRight, iBottom-VertexRadius),
					new Point(iRight-VertexRadius, iBottom),
                    new Point(iLeft+(2*balloonPointerWidth),iBottom),
                    new Point(iLeft+balloonPointerWidth,iBottom+balloonPointerWidth),
                    new Point(iLeft+balloonPointerWidth,iBottom),
    				new Point(iLeft+VertexRadius, iBottom),
					new Point(iLeft, iBottom-VertexRadius),
				};
            }

            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void PaintItem(PaintEventArgs e, ToolTipInfo.ToolTipItem item, bool bRtl , ToolTipRegion ToolTipR)
        {
            if (!item.Hidden)
            {
                e.Graphics.SetClip(item.Bounds, CombineMode.Replace);

                PaintImage(e, item);
                PaintText(e, item, bRtl , ToolTipR);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void PaintImage(PaintEventArgs e, ToolTipInfo.ToolTipItem item)
        {
            if (item.Image != null)
            {
                Rectangle imageRect = item.ImageBounds;

                if (!imageRect.IsEmpty)
                {
                    e.Graphics.DrawImage(item.Image, imageRect);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void PaintText(PaintEventArgs e, ToolTipInfo.ToolTipItem item, bool bRtl , ToolTipRegion toolRegion)
        {
            if (item.Text != null && !item.RenderHtml)
            {
                Rectangle rc = item.TextBounds;
                if (rc.Width > 0 && rc.Height > 0)
                {
                    e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    TextFormatFlags tf = m_formatFlags[item.TextAlignInternal] | TXTFORMAT.COMMON;

                    if (bRtl)
                    {
                        tf |= TextFormatFlags.RightToLeft;
                    }
                    if (this.Style == SuperToolTip.SuperToolTipStyle.Office2013Style)
                    {
                        Font font = item.Font;
                        if (toolRegion == ToolTipRegion.Header)
                        {
                            font = new System.Drawing.Font(item.Font.Name, item.Font.Size, FontStyle.Bold);
                        }
                        TextRenderer.DrawText(e.Graphics, item.Text, font, rc, item.ForeColor, tf);
                    }
                    else
                    {
                        TextRenderer.DrawText(e.Graphics, item.Text, item.Font, rc, item.ForeColor, tf);
                    }
                }
            }
            else if (item.Text != null && item.RenderHtml)
            {
                HtmlRootBox rootBox = new HtmlRootBox(item.Text, item.Bounds);
                rootBox.CalcBoxBounds(e.Graphics);
                rootBox.Paint(e.Graphics);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        void SetToolTipInfo(ToolTipInfo info)
        {
            if (m_ttInfo != null)
            {
                m_ttInfo.PropertyChanged -= new ToolTipPropertyChangedEventHandler(OnPropertyChanged);
            }

            m_ttInfo = info;

            if (m_ttInfo != null)
            {
                m_ttInfo.PropertyChanged += new ToolTipPropertyChangedEventHandler(OnPropertyChanged);
            }

            if (this.IsHandleCreated)
            {
                PerformLayout();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        ToolTipInfo GetToolTipInfo(Component tool)
        {
            if (m_ttProvider != null)
            {
                return m_ttProvider.GetToolTip(tool);
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Component GetTool(IntPtr hWnd, IntPtr id)
        {
            if (this.Handle == hWnd && m_tools.ContainsKey(id))
            {
                return m_tools[id];
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool GetIsCompositeControl(Control control)
        {
            return control is ComboBox || control is ButtonEdit;
        }

        /// <summary>
        /// Sets the balloon location.
        /// </summary>
        /// <param name="component">The component.</param>
        private void SetBalloonLocation(Component component)
        {
            cursorPoint = Cursor.Position;
            Screen scr = Screen.FromPoint(cursorPoint);
            Rectangle scrRect = scr.Bounds;
            if (scrRect.Contains(cursorPoint) && cursorPoint.Y - this.Height < scrRect.Top && cursorPoint.X + this.Width > scrRect.Right)
            {
                bottomLeft = true;
                this.Location = new Point(cursorPoint.X - (this.Width - balloonPointerWidth), cursorPoint.Y + 5);
                Rectangle rc = new Rectangle(0, balloonPointerHeight, this.contentSize.Width, this.contentSize.Height);
                this.Region = GetBalloonRegion(this.ClientRectangle, rc);
            }
            else if (scrRect.Contains(cursorPoint) && cursorPoint.Y - this.Height < scrRect.Top && cursorPoint.X + this.Width < scrRect.Right)
            {
                bottomRight = true;
                this.Location = new Point(cursorPoint.X - balloonPointerWidth, cursorPoint.Y + 5);
                Rectangle rc = new Rectangle(0, balloonPointerHeight, this.contentSize.Width, this.contentSize.Height);
                this.Region = GetBalloonRegion(this.ClientRectangle, rc);
            }
            else if (scrRect.Contains(cursorPoint) && cursorPoint.Y - this.Height > scrRect.Top && cursorPoint.X + this.Width > scrRect.Right)
            {
                topLeft = true;
                this.Location = new Point(cursorPoint.X - (this.Width - balloonPointerWidth), cursorPoint.Y - this.Height);
                this.Region = GetBalloonRegion(this.ClientRectangle, new Rectangle(new Point(0, 0), this.contentSize));
            }
            else
            {
                this.Location = new Point(cursorPoint.X - balloonPointerWidth, cursorPoint.Y - this.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool SetAdjustedLocation(Component component)
        {
            bool bResult = false;

            Point pt = GetAdjustedLocation(component);
            if (!pt.IsEmpty)
            {
                WindowsAPI.SetWindowPos(this.Handle, IntPtr.Zero, pt.X, pt.Y, 0, 0, SWP_MOVE);
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Point GetAdjustedLocation(Component component)
        {
            Point ptResult = Point.Empty;
            Point ptCursor = Cursor.Position;

            Screen scrCurrent = Screen.FromPoint(ptCursor);

            Rectangle rcScreen = scrCurrent.WorkingArea;
            Rectangle rcToolTip = this.Bounds;

            if (!rcScreen.Contains(rcToolTip) || rcToolTip.Contains(ptCursor))
            {
                int nWidth = rcToolTip.Width;
                int nHeight = rcToolTip.Height;

                //If cursor is placed in secondary monitor
                if (rcToolTip.X > rcScreen.Width)
                {
                   
                    if (nHeight < rcScreen.Height - (ptCursor.Y + CursorOffset))
                    {
                        // Place ToolTip below the cursor
                        if (rcToolTip.Y < ptCursor.Y + CursorOffset)
                        {
                            rcToolTip.Y = ptCursor.Y + CursorOffset;
                        }
                        if (nWidth > rcScreen.Width - ptCursor.X)
                        {
                            rcToolTip.X = rcToolTip.X-nWidth;
                        }
                        else rcToolTip.X = ptCursor.X;
                    }
                    else if ((rcToolTip.Y+ nHeight+ CursorOffset) > ptCursor.Y )
                    {
                        // Place ToolTip above the cursor
                        rcToolTip.Y = rcToolTip.Y - nHeight;
                        if ((rcToolTip.X + nWidth + CursorOffset) > ptCursor.X)
                        {
                            rcToolTip.X = rcToolTip.X - nWidth;
                        }                    
                        else rcToolTip.X = ptCursor.X;
                    }
                    else if (nWidth < rcScreen.Width - (ptCursor.X + CursorOffset) || nWidth > ptCursor.X - CursorOffset)
                    {
                        // ToolTip can be placed after the cursor
                        if (rcToolTip.X < ptCursor.X + CursorOffset)
                        {
                            rcToolTip.X = rcToolTip.X - nWidth; 
                        }
                        if (nHeight > rcScreen.Height - ptCursor.Y)
                        {
                            rcToolTip.Y = rcScreen.Height > nHeight ? rcScreen.Height - nHeight : 0;
                        }
                        else rcToolTip.Y = ptCursor.Y;
                    }
                    else
                    {
                        // ToolTip can be placed before the cursor
                        rcToolTip.X = ptCursor.X - nWidth - CursorOffset;

                        if (nHeight > rcScreen.Height - ptCursor.Y)
                        {
                            rcToolTip.Y = rcScreen.Height > nHeight ? rcScreen.Height - nHeight : 0;
                        }
                        else rcToolTip.Y = ptCursor.Y;
                    }
                    ptResult = rcToolTip.Location;


                }
                else
                {
                    if (nHeight < rcScreen.Height - (ptCursor.Y + CursorOffset))
                    {
                        // Place ToolTip below the cursor
                        if (rcToolTip.Y < ptCursor.Y + CursorOffset)
                        {
                            rcToolTip.Y = ptCursor.Y + CursorOffset;
                        }
                        if (nWidth > rcScreen.Width - ptCursor.X)
                        {
                            rcToolTip.X = rcScreen.Width > nWidth ? rcScreen.Width - nWidth : 0;
                        }
                        else rcToolTip.X = ptCursor.X;
                    }
                    else if (nHeight < ptCursor.Y - CursorOffset)
                    {
                        // Place ToolTip above the cursor
                        rcToolTip.Y = ptCursor.Y - CursorOffset - nHeight;

                        if (nWidth > rcScreen.Width - ptCursor.X)
                        {
                            rcToolTip.X = rcScreen.Width > nWidth ? rcScreen.Width - nWidth : 0;
                        }
                        else rcToolTip.X = ptCursor.X;
                    }
                    else if (nWidth < rcScreen.Width - (ptCursor.X + CursorOffset) || nWidth > ptCursor.X - CursorOffset)
                    {
                        // ToolTip can be placed after the cursor
                        if (rcToolTip.X < ptCursor.X + CursorOffset)
                        {
                            rcToolTip.X = ptCursor.X + CursorOffset;
                        }
                        if (nHeight > rcScreen.Height - ptCursor.Y)
                        {
                            rcToolTip.Y = rcScreen.Height > nHeight ? rcScreen.Height - nHeight : 0;
                        }
                        else rcToolTip.Y = ptCursor.Y;
                    }
                    else
                    {
                        // ToolTip can be placed before the cursor
                        rcToolTip.X = ptCursor.X - nWidth - CursorOffset;

                        if (nHeight > rcScreen.Height - ptCursor.Y)
                        {
                            rcToolTip.Y = rcScreen.Height > nHeight ? rcScreen.Height - nHeight : 0;
                        }
                        else rcToolTip.Y = ptCursor.Y;
                    }
                    ptResult = rcToolTip.Location;
                }
            }

            if (PopupToolTip != null)
            {
                Rectangle rc = rcToolTip;
                PopupToolTip(component, ref rc);

                if (rc.Location != rcToolTip.Location)
                {
                    ptResult = rc.Location;
                }
            }

            return ptResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="m"></param>
        void OnTtnShow(Component tool, ref Message m)
        {
            OnPopup(tool, ref m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        void OnTtnClose(Component tool)
        {
            Control control = tool as Control;

            if (control != null)
            {
                RECT rcTool = new RECT();
                WindowsAPI.GetWindowRect(control.Handle, ref rcTool);

                if (((Rectangle)rcTool).Contains(Cursor.Position))
                {
                    control.MouseLeave += new EventHandler(OnToolMouseLeave);
                }
            }

            OnPop(this, EventArgs.Empty);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private void OnWmWindowPosChanged(ref Message m)
        {
            UpdateRegion();
            base.WndProc(ref m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        void OnTtmWindowFromPoint(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Result == IntPtr.Zero || !m_tools.ContainsKey(m.Result))
            {
                Point ptScreen = (Point)(POINT)m.GetLParam(typeof(POINT));
                foreach (KeyValuePair<IntPtr, Component> de in m_tools)
                {
                    Control control = de.Value as Control;
                    if (control.Visible && this.GetIsCompositeControl(control))
                    {
                        Point p = control.PointToClient(ptScreen);
                        if (control.ClientRectangle.Contains(p))
                        {
                            m.Result = de.Key;
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool UseSystemFading()
        {
            return this.UseFading == SuperToolTip.FadingType.System || !OSFeature.Feature.IsPresent(OSFeature.LayeredWindows);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bShow"></param>
        void Animate(bool bShow)
        {
            AnimateWindowFlags flags = AnimateWindowFlags.AW_BLEND;

            if (bShow)
            {
                WindowsAPI.SetWindowPos(this.Handle, new IntPtr((int)SetWindowPosZOrder.HWND_TOPMOST), 0, 0, 0, 0, SWP_ZORDER);
            }
            else flags |= AnimateWindowFlags.AW_HIDE;

            if (!AnimateWindow(this.Handle, FADING_DELAY, flags))
            {
                Exception e = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                System.Diagnostics.Debug.WriteLine("Animate(" + bShow.ToString() + ") failed :" + e.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="dwTime"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool AnimateWindow(IntPtr hwnd, int dwTime, AnimateWindowFlags dwFlags);
        #endregion

        #region Event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="id"></param>
        void OnPropertyChanged(object sender, ToolTipPropertyID id)
        {
            if (this.IsHandleCreated)
            {
                switch (id)
                {
                    case ToolTipPropertyID.Unspecified:
                        Invalidate();
                        break;
                    case ToolTipPropertyID.ItemSizeChanged:
                        PerformLayout();
                        Invalidate();
                        break;
                    default:
                        PerformLayout();
                        break;
                }
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, id);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnToolHandleCreated(object sender, EventArgs e)
        {
            Control tool = sender as Control;

            if (tool != null)
            {
                m_tools[tool.Handle] = tool;

                m_toolInfo.hwnd = this.Handle;
                m_toolInfo.uId = tool.Handle;

                WindowsAPI.SendMessage(this.Handle, (int)TTM.TTM_ADDTOOLW, 0, ref m_toolInfo);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnToolHandleDestroyed(object sender, EventArgs e)
        {
            Control tool = sender as Control;

            if (tool != null && tool.IsHandleCreated)
            {
                m_tools.Remove(tool.Handle);

                if (this.IsHandleCreated)
                {
                    m_toolInfo.hwnd = this.Handle;
                    m_toolInfo.uId = tool.Handle;

                    WindowsAPI.SendMessage(this.Handle, (int)TTM.TTM_DELTOOLW, 0, ref m_toolInfo);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnToolMouseLeave(object sender, EventArgs e)
        {
            Control tool = sender as Control;
            if (tool != null)
            {
                tool.MouseLeave -= new EventHandler(OnToolMouseLeave);

                WindowsAPI.SendMessage(this.Handle, (int)TTM.TTM_ACTIVATE, 0, IntPtr.Zero);
                WindowsAPI.SendMessage(this.Handle, (int)TTM.TTM_ACTIVATE, 1, IntPtr.Zero);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="m"></param>
        void OnPopup(Component component, ref Message m)
        {
            ToolTipInfo info = new ToolTipInfo(GetToolTipInfo(component));
            if (component is ToolStripItem)
            {
                if ((component as ToolStripItem).GetCurrentParent().FindForm() != null)
                {
                    foreach (Control ctrl in (component as ToolStripItem).GetCurrentParent().FindForm().Controls)
                    {
                        if (ctrl is RibbonControlAdv)
                        {
                            ribbonControl = true;
                            loc = new Point((component as ToolStripItem).GetCurrentParent().FindForm().Bounds.X, (component as ToolStripItem).GetCurrentParent().FindForm().Bounds.Y + (ctrl as RibbonControlAdv).Bounds.Bottom);
                            locX = 0;
                            foreach (ToolStripItem quickitem in (ctrl as RibbonControlAdv).HeaderInternal.QuickItems)
                            {
                                if (quickitem == (component as ToolStripItem))
                                {
                                    loc = new Point((component as ToolStripItem).GetCurrentParent().FindForm().Bounds.X + quickitem.Bounds.Left, (component as ToolStripItem).GetCurrentParent().FindForm().Bounds.Y + (ctrl as RibbonControlAdv).HeaderInternal.QuickPanelHeight + RibbonForm.BORDER_WIDTH);
                                }
                            }
                            foreach (ToolStripTabItem tab in (ctrl as RibbonControlAdv).HeaderInternal.MainItems)
                            {
                                foreach (ToolStripEx toolstripex in tab.Panel.Controls)
                                {
                                    int count = 0;
                                    foreach (ToolStripItem toolitem in toolstripex.Items)
                                    {
                                        if (toolstripex.Items[count] is ToolStripPanelItem)
                                        {
                                            foreach (ToolStripItem item in (toolstripex.Items[count] as ToolStripPanelItem).Items)
                                            {
                                                if (item == (component as ToolStripItem))
                                                {
                                                    locX = item.Bounds.Left + (toolstripex.Items[count] as ToolStripPanelItem).Bounds.Left;
                                                }
                                                if (item is ToolStripPanelItem)
                                                {
                                                    ToolStripItemsLocation(item as ToolStripPanelItem, component, toolstripex);
                                                }
                                            }
                                        }
                                        if (toolitem == (component as ToolStripItem))
                                        {
                                            locX = toolitem.Bounds.Left + toolstripex.Bounds.Left;
                                        }
                                        count++;
                                    }
                                }
                            }
                            if (this.Style == SuperToolTip.SuperToolTipStyle.Office2013Style)
                                this.Location = new Point(loc.X + locX, loc.Y);
                            break;
                        }
                    }
                }
            }
            if (UpdateToolTip != null)
            {
                UpdateToolTip(component, ref info);
            }

            this.Info = info;

            if (this.style == SuperToolTip.SuperToolTipStyle.Balloon)
            {
                SetBalloonLocation(component);
                m.Result = new IntPtr(-1);
            }
             if (this.style == SuperToolTip.SuperToolTipStyle.Normal)
             {
                 if (SetAdjustedLocation(component))
                 {
                     m.Result = new IntPtr(-1);
                 }
             }

            if (Popup != null)
            {
                Popup(this, EventArgs.Empty);
            }

            if (!UseSystemFading())
            {
                Animate(true);
            }
        }
        int locX = 0;
        private void ToolStripItemsLocation(ToolStripPanelItem panel, Component component, ToolStripEx toolstripex)
        {
            foreach (ToolStripItem item in panel.Items)
            {
                if (item == (component as ToolStripItem))
                {
                    locX = toolstripex.Bounds.Left + item.Bounds.Left;
                    break;
                }
                if (item is ToolStripPanelItem)
                {
                    ToolStripItemsLocation(item as ToolStripPanelItem, component, toolstripex);
                }
            }
        }
        Point loc = new Point(0, 0);
        private bool ribbonControl = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPop(object sender, EventArgs e)
        {
            if (!UseSystemFading())
            {
                this.Visible = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTick(object sender, EventArgs e)
        {
            HideToolTip();
        }
        #endregion

        #region Events
        public event ToolTipPropertyChangedEventHandler PropertyChanged;
        public event EventHandler Popup;
        public event PopupToolTipHandler PopupToolTip;
        public event UpdateToolTipHandler UpdateToolTip;
        #endregion

        #region Fields
        Timer m_timer;
        ToolTipInfo m_ttInfo;
        SuperToolTip m_ttProvider;
        PopupHandler m_popupHandler;
        EventHandler m_popHandler;
        EventHandler m_tickHandler;

        Dictionary<IntPtr, Component> m_tools;
        Dictionary<Type, IToolTipService> m_services;

        bool m_bDesignMode;
        TOOLINFO m_toolInfo;
        int m_nDelayTime = 0;
        int m_nInitialDelay = 0;
        int m_maxWidth = 0;
        int balloonPointerHeight = 0;
        int balloonPointerWidth = 0;
        bool bottomLeft = false;
        bool bottomRight = false;
        bool topLeft = false;
        public bool m_bGradientBackGround = true;
        Point cursorPoint = Point.Empty;
        Size contentSize = new Size();
        SuperToolTip.FadingType m_UseFading = SuperToolTip.FadingType.Blend;
        SuperToolTip.SuperToolTipStyle style = SuperToolTip.SuperToolTipStyle.Normal;
        static Dictionary<ContentAlignment, TextFormatFlags> m_formatFlags;
        #endregion
    }
    public enum ToolTipRegion
    {
        Header,
        Footer,
        Body
    }
    #endregion
}

#endif