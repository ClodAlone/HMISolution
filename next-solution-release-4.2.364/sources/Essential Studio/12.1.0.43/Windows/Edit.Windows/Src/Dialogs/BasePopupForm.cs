#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Forms.Popup
{
    /// <summary>
    /// Common base form for all popup stuff.
    /// </summary>
    public class BasePopupForm
    : System.Windows.Forms.Form
    {
        #region Internal Classes
        /// <summary>
        /// Class for subclassing parent form and preventing it from blinking.
        /// </summary>
        internal class ParentFormSubClass
        : NativeWindow
        {
            #region PublicFields
            /// <summary>
            /// Indicates whether messages should be catched.
            /// </summary>
            public bool CatchMessage = false;
            #endregion

            #region Overrides
            /// <summary>
            /// Catches WM_NCACTIVATE message if needed.
            /// </summary>
            /// <param name="m">The message</param>
            protected override void WndProc(ref Message m)
            {
                bool bGoToBase = true;

                if (CatchMessage)
                {
                    switch (m.Msg)
                    {
                        case (int)Msg.WM_NCACTIVATE:
                            {
                                m.Result = IntPtr.Zero;
                                bGoToBase = false;
                                break;
                            }

                        case (int)Msg.WM_LBUTTONDOWN:
                        case (int)Msg.WM_NCLBUTTONDOWN:
                        case (int)Msg.WM_MENUSELECT:
                            {
                                if (null != ParentClicked)
                                {
                                    ParentClicked(this, EventArgs.Empty);
                                }
                                break;
                            }
                    }
                }

                if (bGoToBase)
                {
                    base.WndProc(ref m);
                }
            }
            #endregion

            #region Events
            /// <summary>
            /// Raised when most parent form is clicked (for example, main menu clicking).
            /// </summary>
            public event EventHandler ParentClicked;
            #endregion
        }
        #endregion

        #region Constants
        /// <summary>
        /// Tick interval of opacity timer.
        /// </summary>
        private const int DEF_TIMER_INTERVAL = 20;

        /// <summary>
        /// Step of opacity increasing.
        /// </summary>
        private const float DEF_OPACITY_STEP = 0.1f;
        #endregion

        #region Fields
        /// <summary>
        /// Timer for opacity increasing.
        /// </summary>
        private Timer m_opacityTimer;

        /// <summary>
        /// Indicates whether form should be faded in while shown.
        /// </summary>
        private bool m_bFadeIn = true;

        /// <summary>
        /// Specifies whether OnLoad was called.
        /// </summary>
        private bool m_bOnloadCalled = false;

        /// <summary>
        /// Specifies whether form is visible.
        /// </summary>
        private bool m_bVisible = false;

        /// <summary>
        /// Step of opacity incrementation while fading in.
        /// </summary>
        private float m_fOpacityStep = DEF_OPACITY_STEP;

        /// <summary>
        /// Parent control.
        /// </summary>
        private Control m_parent = null;

        /// <summary>
        /// Object for parent form subclassing.
        /// </summary>
        private ParentFormSubClass m_parentSubClass;

        /// <summary>
        /// Indicates whether form must be inactive.
        /// </summary>
        private bool m_bInactive = true;

        /// <summary>
        /// Indicates whether form should be closed when parent is clocked.
        /// </summary>
        private bool m_bCloseOnParentClick = true;

        /// <summary>
        /// Indicates whether parent's WM_NCACTIVATE message should be caught.
        /// When set to false, the focus is returned by simple giving it back. The parent's caption is blinking in this case.
        /// </summary>
        private bool m_bCatchParentActivation = true;

        /// <summary>
        /// Hook for catching windows activation.
        /// </summary>
        private CallWndProcHook m_callWndProcHook;

        /// <summary>
        /// Pen to draw single border.
        /// </summary>
        protected Pen m_borderPen = new Pen(Color.Black);

        /// <summary>
        /// Brush to draw backgrounds.
        /// </summary>
        protected BrushInfo m_backgroundBrush = new BrushInfo(Color.LemonChiffon);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether form should be faded in while shown.
        /// </summary>
        public bool FadeIn
        {
            get
            {
                return m_bFadeIn;
            }
            set
            {
                m_bFadeIn = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether form is visible.
        /// </summary>
        public new bool Visible
        {
            get
            {
                return m_bVisible;
            }
            set
            {
                if (m_bInactive)
                {
                    SetVisibleCore(value);
                }
                else
                {
                    base.SetVisibleCore(value);
                    m_bVisible = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets step of opacity incrementation while fading in.
        /// </summary>
        public float OpacityStep
        {
            get
            {
                return m_fOpacityStep;
            }
            set
            {
                m_fOpacityStep = value;
            }
        }

        /// <summary>
        /// Gets or sets color of single border.
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return m_borderPen.Color;
            }
            set
            {
                if (m_borderPen.Color != value)
                {
                    m_borderPen.Color = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets brush for drawing background.
        /// </summary>
        public BrushInfo BackgroundBrush
        {
            get
            {
                return m_backgroundBrush;
            }
            set
            {
                m_backgroundBrush = value;
                Invalidate();
            }
        }
        #endregion

        #region Initialization & Finalization
        /// <summary>
        /// Initializes a new instance of the BasePopupForm class.
        /// </summary>
        public BasePopupForm()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BasePopupForm class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public BasePopupForm(Control parent)
            : this(parent, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BasePopupForm class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="bFadeIn">Indicates whether form should be faded in while shown.</param>
        public BasePopupForm(Control parent, bool bFadeIn)
            : this(parent, bFadeIn, DEF_OPACITY_STEP)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BasePopupForm class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="bFadeIn">Indicates whether form should be faded in while shown.</param>
        /// <param name="fOpacityStep">Step of opacity incrementation while fading in.</param>
        public BasePopupForm(Control parent, bool bFadeIn, float fOpacityStep)
            : this(parent, bFadeIn, DEF_OPACITY_STEP, true, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BasePopupForm class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="bFadeIn">Indicates whether form should be faded in while shown.</param>
        /// <param name="fOpacityStep">Step of opacity incrementation while fading in.</param>
        /// <param name="bInactive">Indicates whether form must be inactive. True by default.</param>
        /// <param name="bCloseOnParentClick">Indicates whether form must be closed on parent click.</param>
        /// <param name="bCatchParentActivation">Indicates whether parent's WM_NCACTIVATE message should be caught.
        /// When set to false, the focus is returned by simple giving it back. The parent's caption is blinking in this case.</param>
        public BasePopupForm(Control parent, bool bFadeIn, float fOpacityStep, bool bInactive, bool bCloseOnParentClick, bool bCatchParentActivation)
        {
            this.FormBorderStyle = FormBorderStyle.None;

            m_opacityTimer = new Timer();
            m_opacityTimer.Interval = DEF_TIMER_INTERVAL;
            m_opacityTimer.Tick += new EventHandler(OnOpacityTimerTick);

            m_parentSubClass = new ParentFormSubClass();
            m_parentSubClass.ParentClicked += new EventHandler(OnParentSubClassParentClicked);

            if (null != parent)
            {
                m_parent = parent;
            }

            m_bFadeIn = bFadeIn;

            m_fOpacityStep = fOpacityStep;

            m_bCloseOnParentClick = bCloseOnParentClick;
            m_bInactive = bInactive;
            m_bCatchParentActivation = bCatchParentActivation;
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            m_backgroundBrush = null;
            UnregisterHooks();
            base.Dispose(disposing);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Shows the form with fading in when needed.
        /// </summary>
        [UIPermission(SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows)]
        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public new void Show()
        {
            if (m_bFadeIn)
            {
                this.Opacity = 0;
                this.Visible = true;
                m_opacityTimer.Enabled = true;
            }
            else
            {
                this.Opacity = 1;
                this.Visible = true;
            }
        }
        #endregion

        #region Nonpublic Methods
        /// <summary>
        /// Unregistering Hooks.
        /// </summary>
        private void UnregisterHooks()
        {
            if (IntPtr.Zero != m_parentSubClass.Handle)
            {
                m_parentSubClass.CatchMessage = false;
            }

            if (m_callWndProcHook != null)
            {
                m_callWndProcHook.Dispose();
                m_callWndProcHook = null;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Shows/hides window without focusing.
        /// </summary>
        /// <param name="value">Indicates whether window has to be set to visible or not.</param>
        protected override void SetVisibleCore(bool value)
        {
            if (m_bVisible != value)
            {
                m_bVisible = value;
                if (value)
                {
                    if (!m_bOnloadCalled)
                    {
                        this.OnLoad(EventArgs.Empty);
                        m_bOnloadCalled = true;
                    }

                    if (null != m_parent)
                    {
                        if (IntPtr.Zero == m_parentSubClass.Handle && m_bCatchParentActivation)
                        {
                            m_parentSubClass.AssignHandle(m_parent.TopLevelControl != null ? m_parent.TopLevelControl.Handle : m_parent.Handle);
                        }

                        m_parentSubClass.CatchMessage = true;

                        if (m_callWndProcHook == null)
                        {
                            m_callWndProcHook = new CallWndProcHook(new Callback.WindowProc(this.CallWndProc));
                        }
                    }
                }
                else
                {
                    if (IntPtr.Zero != m_parentSubClass.Handle)
                    {
                        m_parentSubClass.CatchMessage = false;
                    }

                    if (m_callWndProcHook != null)
                    {
                        m_callWndProcHook.Dispose();
                        m_callWndProcHook = null;
                    }
                }

                if ((this.IsHandleCreated && this.Handle != IntPtr.Zero) || value)
                {
                    Size size = this.Size;
                    WinAPI.ShowWindow(this.Handle, value ? 8 : 0);
                    this.Size = size;
                }

                if (this.Parent != null)
                {
                    this.Parent.PerformLayout(this, "Visible");
                }
            }
        }

        /// <summary>
        /// Performs closing-related operations.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnClosed(EventArgs e)
        {
            m_bVisible = false;

            base.OnClosed(e);

            UnregisterHooks();
        }

        /// <summary>
        /// Prevents base form from blinking.
        /// </summary>
        /// <param name="m">The Windows Message to process.</param>
        protected override void WndProc(ref Message m)
        {
            if ((int)Msg.WM_MOUSEACTIVATE == m.Msg && m_bInactive)
            {
                m.Result = (IntPtr)3 /*MA_NOACTIVATE*/;
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Performs activation-related operations.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!m_bCatchParentActivation)
            {
                m_parent.Focus();
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Increases opacity of the form.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void OnOpacityTimerTick(object sender, EventArgs e)
        {
            if (!this.IsDisposed)
            {
                if (this.Opacity < 1 - m_fOpacityStep)
                {
                    this.Opacity += m_fOpacityStep;
                }
                else
                {
                    this.Opacity = 1;
                    m_opacityTimer.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Closes popup forms when parent form is clicked.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void OnParentSubClassParentClicked(object sender, EventArgs e)
        {
            if (m_bCloseOnParentClick)
            {
                Close();
            }
        }

        private int CallWndProc(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam)
        {
            switch ((Msg)nMsg)
            {
                case Msg.WM_SHOWWINDOW:
                    if (wParam != IntPtr.Zero)
                    {
                        IntPtr parentHandle = m_parent.TopLevelControl != null ? m_parent.TopLevelControl.Handle : m_parent.Handle;
                        if (hWnd != parentHandle && hWnd != this.Handle)
                        {
                            Control c = Control.FromHandle(hWnd);
                            if (!(c is BasePopupForm) && (c is Form || c == null))
                            {
                                Hide();
                            }
                        }
                    }
                    break;
            }
            return 0;
        }
        #endregion
    }
}