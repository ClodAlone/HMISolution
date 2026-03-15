#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Controls.StatusBar;

namespace Syncfusion.Windows.Forms.Tools
{
    public class StatusBarAdvDesigner : ParentControlDesigner
    {
        /// <summary>
        /// Initializes the designer with the specified component.
        /// </summary>
        /// <param name="component">The <see cref="System.ComponentModel.IComponent"></see> to associate with the designer.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            IDesignerHost host = base.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host != null)
            {
                IComponentChangeService componentChangeService = host.GetService(typeof(IComponentChangeService))
                    as IComponentChangeService;

                if (componentChangeService != null)
                {
                    componentChangeService.ComponentAdding += new ComponentEventHandler(OnComponentAdding);
                    componentChangeService.ComponentAdded += new ComponentEventHandler(ComponentChangeService_ComponentAdded);
                }
            }
        }

       public void ComponentChangeService_ComponentAdded(object sender, ComponentEventArgs e)
        {
            StatusBarAdv statusBar = this.Component as StatusBarAdv;

            if (statusBar != null)
            {
                statusBar.UnlockRedraw();
            }
        }

        private void OnComponentAdding(object sender, ComponentEventArgs e)
        {
            StatusBarAdv statusBar = this.Component as StatusBarAdv;

            if (statusBar != null)
            {
                statusBar.LockRedraw();
            }
        }
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

       private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    actionLists.Add(
                        new StatusBarAdvActionList(this.Component));
                }
                return actionLists;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDesignerHost host = base.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (host != null)
                {
                    IComponentChangeService componentChangeService = host.GetService(typeof(IComponentChangeService))
                        as IComponentChangeService;
                    if (componentChangeService != null)
                    {
                        componentChangeService.ComponentAdding -= new ComponentEventHandler(OnComponentAdding);
                        componentChangeService.ComponentAdded -= new ComponentEventHandler(ComponentChangeService_ComponentAdded);
                    }
                }
            }
            base.Dispose(disposing);
        }
#endif
    }

    /// <summary>
    /// The Essential Tools StatusBarAdv is an extension to the Windows Forms StatusBar.
    /// It supports different border and background styles and can contain other
    /// controls besides StatusBarAdvPanel.
    /// </summary>
    /// <example>
    /// The StatusBarAdv control can be used programmatically as detailed below:
    /// To create an instance of the StatusBarAdv class,
    /// <code lang="C#">
    /// StatusBarAdv statusBarAdv1;
    /// Create the StatusBarAdv control
    /// this.statusBarAdv1 = new StatusBarAdv();
    /// </code>
    /// <code lang="VB.NET">
    /// Dim statusBarAdv1 As StatusBarAdv
    /// � Create the StatusBarAdv control
    /// Me.statusBarAdv1 = New StatusBarAdv()
    /// </code>
    /// Set the properties of the StatusBarAdv control.
    /// <code lang="C#">
    /// this.statusBarAdv1.Alignment = Syncfusion.Windows.Forms.Tools.FlowAlignment.Far;
    /// this.statusBarAdv1.Border3DStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
    /// this.statusBarAdv1.BorderColor = System.Drawing.Color.Black;
    /// this.statusBarAdv1.BorderSides = System.Windows.Forms.Border3DSide.All;
    /// this.statusBarAdv1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
    /// this.statusBarAdv1.CustomLayoutBounds = new System.Drawing.Rectangle(0, 0, 0, 0);
    /// this.statusBarAdv1.Dock = System.Windows.Forms.DockStyle.Bottom;
    /// this.statusBarAdv1.GradientColors = new System.Drawing.Color[0];
    /// this.statusBarAdv1.Location = new System.Drawing.Point(0, 318);
    /// this.statusBarAdv1.Name = "statusBarAdv1";
    /// this.statusBarAdv1.Size = new System.Drawing.Size(856, 24);
    /// this.statusBarAdv1.Spacing = new System.Drawing.Size(5, 2);
    /// this.statusBarAdv1.TabIndex = 12;
    /// </code>
    /// Add StatusBarAdvPanel controls to the StatusBarAdv control.
    /// <code lang="C#">
    /// StatusBarAdvPanel statuBarAdvPanel1 = new StatusBarAdvPanel()
    /// StatusBarAdvPanel statuBarAdvPanel2 = new StatusBarAdvPanel();
    /// StatusBarAdvPanel statuBarAdvPanel3 = new StatusBarAdvPanel();
    /// this.statusBarAdv1.Panels = new StatusBarAdvPanel[]
    /// {
    /// this.statusBarAdvPanel1,
    /// this.statusBarAdvPanel2,
    /// this.statusBarAdvPanel3});
    /// statusBarAdvPanel1
    /// this.statusBarAdvPanel1.BorderColor = System.Drawing.Color.Black;
    /// this.statusBarAdvPanel1.BorderSides = System.Windows.Forms.Border3DSide.All;
    /// this.statusBarAdvPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
    /// this.statusBarAdvPanel1.GradientColors = new System.Drawing.Color[0];
    /// this.statusBarAdvPanel1.Location = new System.Drawing.Point(541, 2);
    /// this.statusBarAdvPanel1.Name = "statusBarAdvPanel1";
    /// this.statusBarAdvPanel1.Size = new System.Drawing.Size(100, 20);
    /// this.statusBarAdvPanel1.TabIndex = 1;
    /// this.statusBarAdvPanel1.Text = "statusBarAdvPanel1";
    /// this.statusBarAdvPanel1.Type = StatusBarAdvPanelType.CapsLockState;
    /// statusBarAdvPanel2
    /// this.statusBarAdvPanel2.BorderColor = System.Drawing.Color.Black;
    /// this.statusBarAdvPanel2.BorderSides = System.Windows.Forms.Border3DSide.All;
    /// this.statusBarAdvPanel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
    /// this.statusBarAdvPanel2.GradientColors = new System.Drawing.Color[0];
    /// this.statusBarAdvPanel2.Location = new System.Drawing.Point(646, 2);
    /// this.statusBarAdvPanel2.Name = "statusBarAdvPanel2";
    /// this.statusBarAdvPanel2.Size = new System.Drawing.Size(100, 20);
    /// this.statusBarAdvPanel2.TabIndex = 2;
    /// this.statusBarAdvPanel2.Text = "statusBarAdvPanel2";
    /// this.statusBarAdvPanel2.Type = StatusBarAdvPanelType.NumLockState;
    /// statusBarAdvPanel3
    /// this.statusBarAdvPanel3.BorderColor = System.Drawing.Color.Black;
    /// this.statusBarAdvPanel3.BorderSides = System.Windows.Forms.Border3DSide.All;
    /// this.statusBarAdvPanel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
    /// this.statusBarAdvPanel3.GradientColors = new System.Drawing.Color[0];
    /// this.statusBarAdvPanel3.Location = new System.Drawing.Point(751, 2);
    /// this.statusBarAdvPanel3.Name = "statusBarAdvPanel3";
    /// this.statusBarAdvPanel3.Size = new System.Drawing.Size(100, 20);
    /// this.statusBarAdvPanel3.TabIndex = 3;
    /// this.statusBarAdvPanel3.Text = "statusBarAdvPanel3";
    /// this.statusBarAdvPanel3.Type = StatusBarAdvPanelType.LongTime;
    /// </code>
    /// Add the StatusBarAdv control to the form
    /// <code lang="C#">
    /// Add the StatusBarAdv control to the form
    /// this.Controls.Add(this.statusBarAdv1);
    /// </code>
    /// <code lang="VB.NET">
    /// �Add the StatusBarAdv control to the form
    /// Me.Controls.Add(Me.statusBarAdv1)
    /// </code>
    /// </example>
    [Designer(typeof(StatusBarAdvDesigner))]
    [
    System.Drawing.ToolboxBitmap(typeof(StatusBarAdv), "ToolboxIcons.StatusBarAdv.bmp"),
    Description("Represents advanced StatusBar with different border and background styles.")
    ]
    public class StatusBarAdv : GradientPanel, ICanCancel, IStatusBarAdv
    {
        #region Fields

        private System.ComponentModel.IContainer components;
        private bool size = false;
        private Syncfusion.Windows.Forms.Tools.FlowLayout flowLayout;
        private bool autoHeightControls = true;
        private bool sizingGrip = true;
        private int sizingWidth = 14;
        private Size parentSize = Size.Empty;
        private Size originalSize = Size.Empty;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);


        // private CancelListener cl = null;
        private ThemedControlDrawing tcd = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the Statusbar will draw a themed background. Indicated settings: BorderStyle = None.
        /// </summary>
        [Description("Indicates if the Statusbar will draw a themed background. Indicated settings: BorderStyle = None")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public new bool ThemesEnabled
        {
            get 
            {
                return base.ThemesEnabled;
            }
            set
            {
                base.ThemesEnabled = value;

                foreach (StatusBarAdvPanel panel in this.Panels)
                    panel.ThemesEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Sizing grip is visible.
        /// </summary>
        /// <remarks>
        /// If SizingGrip is false the statusbar will not resize it`s parent.
        /// </remarks>
        [Description("Indicates if the Sizing grip is visible.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool SizingGrip
        {
            get
            { 
                return sizingGrip; 
            }
            set
            {
                if (sizingGrip != value)
                {
                    sizingGrip = value;
                    this.flowLayout.HorzFarMargin = value ? this.sizingWidth + 3 : 7;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a custom rectangle that the layout will use to display the panels.
        /// </summary>
        /// <remarks>Set this property to specify new boundaries for the layout of the panels.</remarks>
        [Description("Indicates a custom rectangle that the layout will use to display the panels.")]
        [Category("Appearance")]
        public Rectangle CustomLayoutBounds
        {
            get 
            { 
                return flowLayout.CustomLayoutBounds; 
            }
            set
            {
                flowLayout.CustomLayoutBounds = value;
            }
        }
        bool isScaling = false;

        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        /// <summary>
        /// Gets or sets the alignment of the panels.
        /// </summary>
        /// <remarks>
        /// This property determines the location of the panels.
        /// If set to ChildConstraints the Panels` HAlign property will be used to position and size them.
        /// </remarks>
        [Description("Determines the alignment of the panels.")]
        [Category("Appearance")]
        [DefaultValue(FlowAlignment.ChildConstraints)]
        public FlowAlignment Alignment
        {
            get 
            {
                return this.flowLayout.Alignment; 
            }
            set
            {
                this.flowLayout.Alignment = value;
                RecalculateConstraints();
            }
        }

        /// <summary>
        /// Gets or sets the spacing between the panels.
        /// </summary>
        [Description("Determines the spacing between the panels.")]
        [Category("Appearance")]
        public Size Spacing
        {
            get 
            { 
                return new Size(this.flowLayout.HGap, this.flowLayout.VGap); 
            }
            set
            {
                this.flowLayout.HGap = value.Width;
                this.flowLayout.VGap = value.Height;
                this.flowLayout.TopMargin = value.Height;
                this.OnSizeChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the StatusBar will resize the height of the panels according to it`s height.
        /// </summary>
        /// <remarks>
        /// The default value is true, indicating that when the StatusBar`s height changes the panels inside will also change their height.
        /// </remarks>
        [Description("Determines if the StatusBar will resize the Height of the panels according to it`s Height.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool AutoHeightControls
        {
            get 
            { 
                return autoHeightControls;
            }
            set
            {
                if (autoHeightControls != value)
                {
                    autoHeightControls = value;
                    this.OnSizeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the StatusBarAdvPanel controls contained in the StatusBarAdv.
        /// </summary>
        /// <remarks>
        /// Use this property to Add/Remove panels from the status bar.
        /// </remarks>
        [Description("Indicates the StatusBarAdvPanel controls contained in the StatusBarAdv.")]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StatusBarAdvPanel[] Panels
        {
            get
            {
                ArrayList panels = new ArrayList();
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i] is StatusBarAdvPanel) panels.Add(Controls[i]);
                }
                return (StatusBarAdvPanel[])panels.ToArray(typeof(StatusBarAdvPanel));
            }
            set
            {
                ArrayList alCopy = new ArrayList();
                foreach (Control control in Controls)
                {
                    if (control is StatusBarAdvPanel)
                    {
                        alCopy.Add(control);
                    }
                }

                foreach (Control ctrl in alCopy)
                {
                    Controls.Remove(ctrl);
                }

                for (int i = 0; i < value.Length; i++)
                {
                    if (!Controls.Contains(value[i]))
                        Controls.Add(value[i]);
                }
            }
        }

        #endregion

        #region ISupportInitialize methods
        /// <summary>
        /// Begins initialization of the control.
        /// </summary>
        public override void BeginInit()
        {
            base.BeginInit();
        }

        /// <summary>
        /// Ends initialization of the control.
        /// </summary>
        public override void EndInit()
        {
            RecalculateConstraints();

            base.EndInit();
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the StatusBarAdv class.
        /// </summary>
        public StatusBarAdv()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(StatusBarAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            this.ThemedBorder = false;

            // This should be called only when the drag begins...
            // cl = new CancelListener(this);
            if (DesignMode)
                this.AllowDrop = true;

            this.ControlAdded += new ControlEventHandler(Control_Added);
            this.ControlRemoved += new ControlEventHandler(Control_Removed);

            XPThemes.ThemeChanged += new EventHandler(XPThemes_ThemeChanged);

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
            {
                tcd = new ThemedControlDrawing(ThemedControls.STATUS, this);
            }
            CTRLSIZE = this.Size;

            // TODO: Add any initialization after the InitForm call
        }

        private void Control_Added(object sender, ControlEventArgs e)
        {
            if (autoHeightControls)
            {
                e.Control.Height = ClientRectangle.Height - this.flowLayout.VGap * 2/*-2*/;
            }
            e.Control.SizeChanged += new EventHandler(ControlSizeChanged);

            this.flowLayout.SetConstraints(e.Control, new FlowLayoutConstraints(true, HorzFlowAlign.Left, VertFlowAlign.Top, false, false, false));
            this.flowLayout.SetPreferredSize(e.Control, e.Control.Size);

            if (e.Control is StatusBarAdvPanel)
            {
                this.flowLayout.SetConstraints(e.Control, ((StatusBarAdvPanel)e.Control).Constraints);
                this.flowLayout.SetPreferredSize(e.Control, ((StatusBarAdvPanel)e.Control).PreferredSize);
                ((StatusBarAdvPanel)e.Control).ConstraintsChanged += new EventHandler(Panel_ConstraintsChanged);
                ((StatusBarAdvPanel)e.Control).MinimumSizeChanged += new EventHandler(Panel_MinimumSizeChanged);
                ((StatusBarAdvPanel)e.Control).PreferredSizeChanged += new EventHandler(Panel_PreferredSizeChanged);
            }
            this.RecalculateConstraints();
            e.Control.SendToBack();
        }

        private bool m_bIsRedrawLocked = false;

        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal void LockRedraw()
        {
            if (!m_bIsRedrawLocked)
            {
                NativeMethodsHelper.SetRedrawWindow(this.Handle, false, false);
            }

            m_bIsRedrawLocked = true;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal void UnlockRedraw()
        {
            if (m_bIsRedrawLocked)
            {
                NativeMethodsHelper.SetRedrawWindow(this.Handle, true, true);
            }
            m_bIsRedrawLocked = false;
        }

        private void Control_Removed(object sender, ControlEventArgs e)
        {
            e.Control.SizeChanged -= new EventHandler(ControlSizeChanged);
        }

        private void Panel_MinimumSizeChanged(object sender, EventArgs e)
        {
            this.flowLayout.SetMinimumSize(((Control)sender), ((StatusBarAdvPanel)sender).MinimumSize);
        }
        private void Panel_PreferredSizeChanged(object sender, EventArgs e)
        {
            this.flowLayout.SetPreferredSize(((Control)sender), ((StatusBarAdvPanel)sender).PreferredSize);
        }
        private void Panel_ConstraintsChanged(object sender, EventArgs e)
        {
            this.flowLayout.SetConstraints(((Control)sender), ((StatusBarAdvPanel)sender).Constraints);
            RecalculateConstraints();
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">True if disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DragDrop -= new System.Windows.Forms.DragEventHandler(this.StatusBarAdv_DragDrop);
                this.ThemeChanged -= new System.EventHandler(this.StatusBarAdv_ThemeChanged);
                this.ControlAdded -= new ControlEventHandler(Control_Added);
                this.ControlRemoved -= new ControlEventHandler(Control_Removed);
                XPThemes.ThemeChanged -= new EventHandler(XPThemes_ThemeChanged);

                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
                if (this.tcd != null)
                {
                    this.tcd.Dispose();
                    this.tcd = null;
                }
                if (this.flowLayout != null)
                {
                    this.flowLayout.Dispose();
                    this.flowLayout = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.flowLayout = new Syncfusion.Windows.Forms.Tools.FlowLayout(this.components);
            //
            // flowLayout
            //
            this.flowLayout.Alignment = Syncfusion.Windows.Forms.Tools.FlowAlignment.ChildConstraints;
            this.flowLayout.ContainerControl = this;
            this.flowLayout.HGap = 2;
            this.flowLayout.HorzFarMargin = 14;
            this.flowLayout.TopMargin = 2;
            this.flowLayout.VGap = 2;
            //
            // StatusBarAdv
            //
            this.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DockPadding.All = 3;
            this.Size = new System.Drawing.Size(500, 22);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.StatusBarAdv_DragDrop);
            this.ThemeChanged += new System.EventHandler(this.StatusBarAdv_ThemeChanged);

        }
        #endregion

        private void ControlSizeChanged(object sender, EventArgs e)
        {
            if (autoHeightControls)
            {
                UpdateChildHeight((Control)sender);
            } 
            RecalculateConstraints();
        }

        private void UpdateChildHeight(Control c)
        {
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
            if (!this.AutoSize)
#endif
                c.Height = ClientRectangle.Height - this.flowLayout.VGap * 2;
        }

        #region Overrides

        protected override void ThemedPaintBackground(System.Drawing.Graphics graphics, Rectangle rect, Rectangle clip)
        {
            tcd.DrawMirrored = GetIsMirrored();
            tcd.DrawThemeBackground(graphics, 4, 1, rect, clip);
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            int width = ClientRectangle.Width;
            int height = ClientRectangle.Height;
            base.OnPaint(e);

            if (null != tcd)
                tcd.DrawMirrored = GetIsMirrored();

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed && ThemesEnabled && !this.IgnoreThemeBackground)
            {
                tcd.DrawThemeBackground(e.Graphics, 4, 1, ClientRectangle);
            }
            else
            {
                // base.OnPaint(e);
            }
            if (!sizingGrip) return;
            int x = width - sizingWidth;
            /*
            for(int i=0;i<5;i++)
            {
                e.Graphics.DrawLine(new Pen(SystemColors.ControlLightLight),x,Height+i*4,Width,Height+i*4-sizingWidth);
                e.Graphics.DrawLine(new Pen(SystemColors.ControlDark),x,Height+i*4+1,Width,Height+i*4-sizingWidth+1);
                e.Graphics.DrawLine(new Pen(SystemColors.ControlDark),x,Height+i*4+2,Width,Height+i*4-sizingWidth+2);
            }
            */
            bool bIsMirrored = GetIsMirrored();
            if (!(Parent is Form && ((Form)Parent).WindowState == FormWindowState.Maximized))
            {
                int nGripWidth = this.sizingWidth + 3;
                if (bIsMirrored)
                {
                    this.flowLayout.HorzNearMargin = nGripWidth;
                }
                else
                {
                    this.flowLayout.HorzFarMargin = nGripWidth;
                }

                Rectangle rectGripper = new Rectangle(bIsMirrored ? 0 : x, 0, sizingWidth, height);
                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed && ThemesEnabled)
                {
                    if(!(this is StatusBarExt))
                        tcd.DrawThemeBackground(e.Graphics, 3, 1, rectGripper);
                }
                else
                {
                    Graphics gfx = e.Graphics;

                    using (CMirroredDrawer mdDrawer = new CMirroredDrawer(gfx, rectGripper, bIsMirrored))
                    {
                        Graphics gfxCanvas = mdDrawer.VirtualGfx;
                        Rectangle rectCanvas = mdDrawer.VirtualBounds;

                        ControlPaint.DrawSizeGrip(gfxCanvas, Color.Transparent, rectCanvas);
                    }
                }
            }
            else
            {
                if (bIsMirrored)
                {
                    this.flowLayout.HorzNearMargin = 3;
                }
                else
                {
                    this.flowLayout.HorzFarMargin = 3;
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
            if (autoHeightControls)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    UpdateChildHeight(Controls[i]);
                }
            }

            RecalculateConstraints();
            Invalidate(true);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCHITTEST:
                    if (ShouldEnableSizing())
                    {
                        Rectangle rcGrip = this.ClientRectangle;

                        rcGrip.X = rcGrip.Right - sizingWidth;
                        rcGrip.Width = sizingWidth;

                        int x = NativeMethods.LOWORD(m.LParam);
                        int y = NativeMethods.HIWORD(m.LParam);

                        if (rcGrip.Contains(PointToClient(new Point(x, y))))
                        {
                            m.Result = (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                            return;
                        }
                    }
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion

        void ICanCancel.CancelOperation()
        {
            if (size)
            {
                size = false;
                Parent.Size = originalSize;
            }
        }

        /// <summary>
        /// Indicates whether sizing should be enabled.
        /// </summary>
        /// <returns>True to enable sizing by the user; false otherwise.</returns>
        /// <remarks>
        /// This method indicates whether the StatusBarAdv's <see cref="SizingGrip"/> is on,
        /// is docked to the bottom, the right-bottom corner aligns with the parent's right-bottom
        /// and that the Parent is not maximized.
        /// </remarks>
        protected virtual bool ShouldEnableSizing()
        {
            if (this.sizingGrip && Dock == DockStyle.Bottom &&
                (this.Bottom == Parent.DisplayRectangle.Bottom) &&
                (this.Right == Parent.DisplayRectangle.Right) &&
                !ParentMaximized)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the parent control is maximized.
        /// </summary>
        protected bool ParentMaximized
        {
            get
            {
                if (Parent is Form && ((Form)Parent).WindowState == FormWindowState.Maximized) return true;
                return false;
            }
        }

        private void StatusBarAdv_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            ISelectionService ss = this.GetService(typeof(ISelectionService)) as ISelectionService;
            if (!(ss.PrimarySelection is Control)) return;
            int index = Controls.IndexOf((Control)ss.PrimarySelection);
            if (index > 0)
            {
                Point pt = this.PointToClient(new Point(e.X, e.Y));
                int replace = -1;
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i == index) continue;
                    if (Controls[i].Location.X > pt.X)
                    {
                        replace = i;
                        break;
                    }
                }
                if (replace < index)
                {
                    Controls.SetChildIndex(Controls[index], replace); 
                }
                else
                {
                    Controls.SetChildIndex(Controls[index], replace - 1);
                }
            }
            IComponentChangeService changeService = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (changeService != null)
            {
                changeService.OnComponentChanging(this, null);
                changeService.OnComponentChanged(this, null, null, null);
            }
            Parent.Update();
            Invalidate(true);
        }

        private void RecalculateConstraints()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (!(Controls[i] is StatusBarAdvPanel))
                {
                    this.flowLayout.SetPreferredSize(Controls[i], Controls[i].Size);
                    this.flowLayout.SetMinimumSize(Controls[i], Controls[i].Size);
                }
                if (Controls[i] is StatusBarAdvPanel)
                {
                    this.flowLayout.SetPreferredSize(Controls[i], ((StatusBarAdvPanel)Controls[i]).PreferredSize);
                    this.flowLayout.SetMinimumSize(Controls[i], new Size(0, ((StatusBarAdvPanel)Controls[i]).Height));
                    /*
                    if(Alignment!=FlowAlignment.ChildConstraints)
                    {
                        if(((StatusBarAdvPanel)Controls[i]).HasPreferredSize )
                        {
                            ((StatusBarAdvPanel)Controls[i]).PreferredSize = ((StatusBarAdvPanel)Controls[i]).Size;
                            ((StatusBarAdvPanel)Controls[i]).MinimumSize = new Size(0,((StatusBarAdvPanel)Controls[i]).Height);
                        }
                        else
                        {
                            ((StatusBarAdvPanel)Controls[i]).PreferredSize = ((StatusBarAdvPanel)Controls[i]).Size;
                            ((StatusBarAdvPanel)Controls[i]).MinimumSize = ((StatusBarAdvPanel)Controls[i]).Size;
                        }
                    }
                    else
                    {
                            if( ((StatusBarAdvPanel)Controls[i]).Constraints.HAlign != HorzFlowAlign.Justify)
                            {
                                ((StatusBarAdvPanel)Controls[i]).PreferredSize = ((StatusBarAdvPanel)Controls[i]).Size;
                                ((StatusBarAdvPanel)Controls[i]).MinimumSize = ((StatusBarAdvPanel)Controls[i]).Size;

                            }
                            else
                            {
                                ((StatusBarAdvPanel)Controls[i]).PreferredSize = new Size(0,((StatusBarAdvPanel)Controls[i]).Height);
                                ((StatusBarAdvPanel)Controls[i]).MinimumSize = new Size(0,((StatusBarAdvPanel)Controls[i]).Height);
                            }

                    }
                }
                else
                {
                    if(Alignment!=FlowAlignment.ChildConstraints)
                    {
                        flowLayout.SetPreferredSize(Controls[i],Controls[i].Size);
                        flowLayout.SetMinimumSize(Controls[i],Controls[i].Size);
                    }
                    else
                    {
                        if( flowLayout.GetConstraints(Controls[i]).HAlign != HorzFlowAlign.Justify)
                        {
                            flowLayout.SetPreferredSize(Controls[i],Controls[i].Size);
                            flowLayout.SetMinimumSize(Controls[i],Controls[i].Size);

                        }
                        else
                        {
                            flowLayout.SetPreferredSize(Controls[i],new Size(0,Controls[i].Height));
                            flowLayout.SetMinimumSize(Controls[i],new Size(0,Controls[i].Height));
                        }
                    }*/
                }
            }
        }

        /// <summary>
        /// Sets the preferred size in the layout of the specified control.
        /// </summary>
        /// <remarks>
        /// Use this method to set the preferred size of a control inside the StatusBar.
        /// </remarks>
        /// <param name="control">The control whose preferred size is to be set.</param>
        /// <param name="size">The size.</param>
        public void SetPreferredSize(Control control, Size size)
        {
            this.flowLayout.SetMinimumSize(control, size);
        }

        /// <summary>
        /// Sets the horizontal alignment options for the specified control.
        /// </summary>
        /// <remarks>
        /// If the StatusBar`s Alignment property is set to ChildContstraints, the StatusBar will use this option in the positioning and resizing of the control.
        /// </remarks>
        /// <param name="control">The control whose HAlign is to be set.</param>
        /// <param name="align">The alignment option to be set to the specified control.</param>
        public void SetHAlign(Control control, HorzFlowAlign align)
        {
            this.flowLayout.GetConstraintsRef(control).HAlign = align;
        }

        /// <summary>
        /// Returns the preferred size of the specified control.
        /// </summary>
        /// <param name="control">The control whose preferred size is to be returned.</param>
        /// <returns>The preferred size of the control.</returns>
        public Size GetPreferredSize(Control control)
        {
            return this.flowLayout.GetPreferredSize(control);
        }

        private void StatusBarAdv_ThemeChanged(object sender, System.EventArgs e)
        {
            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
            {
                tcd = new ThemedControlDrawing(ThemedControls.STATUS);
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            if (sizingGrip)
            {
                int nRiteMarg = this.flowLayout.HorzNearMargin;
                this.flowLayout.HorzNearMargin = this.flowLayout.HorzFarMargin;
                this.flowLayout.HorzFarMargin = nRiteMarg;
            }

            PerformLayout();
        }

        /// <summary>
        /// Indicates whether the control should be drawn right-to-left.
        /// </summary>
        /// <returns>True if the control is to be drawn right-to-left; false otherwise.</returns>
        protected bool GetIsMirrored()
        {
            return RightToLeft == RightToLeft.Yes;
        }

        private void XPThemes_ThemeChanged(object sender, EventArgs e)
        {
            if (tcd == null && XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
            {
                tcd = new ThemedControlDrawing(ThemedControls.STATUS);
            }
        }
    }
}
