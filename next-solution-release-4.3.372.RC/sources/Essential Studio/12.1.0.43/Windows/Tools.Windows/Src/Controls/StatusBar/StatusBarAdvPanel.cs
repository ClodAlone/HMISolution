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
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The StatusBarAdvPanel is used with the StatusBarAdv to show information like key states date and time.
    /// </summary>
    [
    System.Drawing.ToolboxBitmap(typeof(StatusBarAdvPanel), "ToolboxIcons.StatusBarAdvPanel.bmp"),
    Description("Represents a control used with StatusBarAdv to show information like key states date and time")
    ]
    public class StatusBarAdvPanel : GradientPanel
    {
        private const int VK_INSERT = 0x2D;

        /// <summary>
        /// Gap between image and text.
        /// </summary>
        private const int DEF_IMAGE_GAP = 2;

        /// <summary>
        /// Default delay for aniamtion.
        /// </summary>
        private const int DEF_ANIMATION_DELAY = 5;

        /// <summary>
        /// Default animation speed.
        /// </summary>
        private const int DEF_ANIMATION_SPEED = 1;

        private System.ComponentModel.IContainer components;

        private string text;
        private string customOnText = "Custom ON";
        private string customOffText = "Custom OFF";
        /// <summary>
        ///Alignment
        /// </summary>
        private HorizontalAlignment align = HorizontalAlignment.Left;
        private StatusBarAdvPanelType type = StatusBarAdvPanelType.Custom;
        private Icon icon = null;
        private System.Windows.Forms.Timer timer;
        private KeyListener keyListener;
        private FlowLayoutConstraints constraints;
        private Size preferredSize = Size.Empty;
        private Size minimumSize = Size.Empty;
        private Color oldBackColor = SystemColors.Control;
        private bool sizeToContent = false;

        // private bool hasPreferredSize = true;
        private string toolTip = string.Empty;
        private System.Windows.Forms.ToolTip m_toolTip;
        private ThemedControlDrawing tcd = null;

        /// <summary>
        /// Uses for animation displayed text.
        /// </summary>
        private AnimationHelper m_animator = null;

        /// <summary>
        /// A value indicating whether control uses 
        /// marquee style for displayed text.
        /// </summary>
        private bool m_bIsMarquee = false;

        /// <summary>
        /// A value indicating whether control uses 
        /// custom text feature for displaying the CAPS/SCROLL/NUMLOCK/INSERTKEY
        /// </summary>
        private bool m_showCustomText = false;

        /// <summary>
        /// Animation speed.
        /// </summary>
        private int m_animationSpeed = DEF_ANIMATION_SPEED;

        /// <summary>
        /// Delay for animation.
        /// </summary>
        private int m_animationDelay = DEF_ANIMATION_DELAY;

        /// <summary>
        /// Point which uses for animation.
        /// </summary>
        private Point m_animationPoint = Point.Empty;

        /// <summary>
        /// Direction of animation.
        /// </summary>
        private MarqueeDirection m_enAnimationDirection = MarqueeDirection.Left;

        /// <summary>
        /// Style of animation.
        /// </summary>
        private MarqueeStyle m_enAnimationStyle = MarqueeStyle.Scroll;

        /// <summary>
        /// Indicates whether animation can be started.
        /// </summary>
        private bool m_bAnimationStarted = false;

        /// <summary>
        /// Currently direnction for animation.
        /// </summary>
        private MarqueeDirection m_enPerformingDirection = MarqueeDirection.Left;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>

        /// Event is triggered when panel has changed the preferred size 
        /// </summary>
        [Category("Property Changed")]
        [Description("Event is triggered when panel has changed the preferred size.")]
        public event EventHandler PreferredSizeChanged;
        [Category("Property Changed")]
        internal event EventHandler MinimumSizeChanged;

        /// <summary>
        /// Event is raised if the list of constraints has changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Event is raised if the list of constraints has changed.")]
        public event EventHandler ConstraintsChanged;

        /// <summary>
        /// Event  is thrown at any time, if the alignment of panel changes.
        /// </summary>
        [Category("Property Changed")]
        [Description("Event  is thrown at any time, if the alignment of panel changes.")]
        public event EventHandler AlignChanged;

        /// <summary>
        /// Event is triggered when PanelType changes.
        /// </summary>
        [Category("Property Changed")]
        [Description("Event is triggered when PanelType changes.")]
        public event EventHandler TypeChanged;

        /// <summary>
        /// Event is raised when icon of the panel changes.
        /// </summary>
        [Category("Property Changed")]
        [Description("Event is raised when icon of the panel changes.")]
        public event EventHandler IconChanged;

        /// <summary>
        /// Raised when the IsMarquee property is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Raised when the IsMarquee property is changed.")]
        public event EventHandler IsMarqueeChanged;

        /// <summary>
        /// Raised when the Animation speed is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Raised when the Animation speed property is changed.")]
        public event EventHandler AnimationSpeedChanged;

        /// <summary>
        /// Raised when the Animation delay is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Raised when the Animation delay property is changed.")]
        public event EventHandler AnimationDelayChanged;

        /// <summary>
        /// Raised when the Animation direction is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Raised when the Animation direction property is changed.")]
        public event EventHandler AnimationDirectionChanged;

        /// <summary>
        /// Raised when the Animation style is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Raised when the Animation style is changed.")]
        public event EventHandler AnimationStyleChanged;

        /// <summary>
        /// Method is called when PreferredSizeChanged Event is triggered.
        /// </summary>
        protected void OnPreferredSizeChanged()
        {
            if (PreferredSizeChanged != null) PreferredSizeChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method is called when MinimumSizeChanged Event is triggered.
        /// </summary>
        protected void OnMinimumSizeChanged()
        {
            if (MinimumSizeChanged != null) MinimumSizeChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method is called when ConstraintsChanged Event is triggered.
        /// </summary>
        protected void OnConstraintsChanged()
        {
            if (ConstraintsChanged != null) ConstraintsChanged(this, EventArgs.Empty);
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

        /// <summary>
        /// Method is called when AlignChanged Event is triggered.
        /// </summary>
        protected void OnAlignChanged()
        {
            if (AlignChanged != null) AlignChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method is called when TypeChanged Event is triggered.
        /// </summary>
        protected void OnTypeChanged()
        {
            if (TypeChanged != null) TypeChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method is called when IconChanged Event is triggered.
        /// </summary>
        protected void OnIconChanged()
        {
            if (IconChanged != null) IconChanged(this, EventArgs.Empty);
        }

        private void RaiseIsMarqueeChanged()
        {
            if (this.IsMarqueeChanged != null)
            {
                this.IsMarqueeChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseAnimationSpeedChanged()
        {
            if (this.AnimationSpeedChanged != null)
            {
                this.AnimationSpeedChanged(this, EventArgs.Empty);
            }
        }
        private void RaiseAnimationDelayChanged()
        {
            if (this.AnimationDelayChanged != null)
            {
                this.AnimationDelayChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseAnimationDirectionChanged()
        {
            if (this.AnimationDirectionChanged != null)
            {
                this.AnimationDirectionChanged(this, EventArgs.Empty);
            }
        }
        private void RaiseAnimationStyleChanged()
        {
            if (this.AnimationStyleChanged != null)
            {
                this.AnimationStyleChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnAnimationStyleChanged()
        {
            RefreshAnimation();

            RaiseAnimationStyleChanged();
        }
        protected virtual void OnAnimationDirectionChanged()
        {
            RefreshAnimation();

            RaiseAnimationDirectionChanged();
        }
        protected virtual void OnIsMarqueeChanged()
        {
            if (!this.IsMarquee)
            {
                StopAnimation();
            }

            RaiseIsMarqueeChanged();
        }

        protected virtual void OnAnimationSpeedChanged()
        {
            RefreshAnimation();

            RaiseAnimationSpeedChanged();
        }
        protected virtual void OnAnimationDelayChanged()
        {
            RefreshAnimation();

            RaiseAnimationDelayChanged();
        }
        
        private bool KeyState(short state)
        {
            bool stateValue = (StatusBarAdvNativeMethods.LOWORD(state) == 1 || StatusBarAdvNativeMethods.LOWORD(state) == 65409);
            return stateValue;
        }


        /// <summary>
        /// Gets or sets the tool tip of the panel.
        /// </summary>
        [Description("Indicates the tool tip of the panel.")]
        [Category("Appearance")]
        [DefaultValue("")]
        public string ToolTip
        {
            get
            {
                return toolTip;
            }
            set
            {
                if (toolTip != value)
                {
                    toolTip = value;

                    m_toolTip.RemoveAll();
                    m_toolTip.SetToolTip(this, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the size of the panel will be automatically calculated by the size of it`s contents.
        /// </summary>
        [Description("Indicates if the size of the panel will be automatically calculated by the size of it`s contents.")]
        [Category("Layout")]
        [DefaultValue(false)]
        public bool SizeToContent
        {
            get 
            { 
                return sizeToContent; 
            }
            set
            {
                if (value) this.Alignment = HorizontalAlignment.Left;
                if (this.sizeToContent != value)
                {
                    this.sizeToContent = value;
                    RecalculateSize();
                    Update();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum size of the panel in the FlowLayout.
        /// </summary>
        [Description("Indicates the minimum size of the panel in the FlowLayout.")]
        [Category("Layout")]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		internal Size MinimumSize
#else
        internal new Size MinimumSize
#endif
        {
            get 
            { 
                return minimumSize; 
            }
            set 
            {
                minimumSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the preferred size of the panel in the FlowLayout.
        /// </summary>
        [Description("Indicates the preferred size of the panel in the FlowLayout.")]
        [Category("Layout")]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public Size PreferredSize
#else
        public new Size PreferredSize
#endif
        {
            get
            {
                if (preferredSize == Size.Empty)
                {
                    return Size;
                }
                return preferredSize;
            }
            set
            {
                if (preferredSize != value && !this.sizeToContent)
                {
                    preferredSize = value;
                    OnPreferredSizeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the constraints in the FlowLayout.
        /// </summary>
        [Description("Indicates the constraints in the FlowLayout.")]
        [Category("Layout")]
        internal FlowLayoutConstraints Constraints
        {
            get
            { 
                return constraints; 
            }
            set 
            { 
                constraints = value; 
                this.OnConstraintsChanged();
            }
        }

        /// <summary>
        /// Gets or sets the HorizontalAlignment in the FlowLayout.
        /// </summary>
        /// <remarks>
        /// Set this property if the parent StatusBar has the Alignment property set to ChildConstraints.
        /// Otherwise this setting will not be taken into account.
        /// </remarks>
        [Description("Indicates the HorizontalAlignment in the FlowLayout.")]
        [Category("Layout")]
        [DefaultValue(HorzFlowAlign.Left)]
        public HorzFlowAlign HAlign
        {
            get 
            {
                return constraints.HAlign; 
            }
            set
            {
                if (this.HAlign != value)
                {
                    constraints.HAlign = value;

                    // set halign.
                    if (this.HAlign == HorzFlowAlign.Justify)
                    {
                        // Actualize preferred size for Justify(if preferred size not already set).
                        if (!this.ShouldSerializePreferredSize())
                            this.PreferredSize = new Size(Width - 1, Height);
                    }
                    OnConstraintsChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon of the panel.
        /// </summary>
        /// <remarks>
        /// This icon represents the icon that appears in the panel.
        /// It is a 16x16 icon.
        /// </remarks>
        [Description("Indicates the icon of the panel.")]
        [Category("Appearance")]
        [DefaultValue(null)]
        public Icon Icon
        {
            get 
            { 
                return icon; 
            }
            set
            {
                if (icon != value)
                {
                    if (value == null)
                    {
                        icon = null;
                    }
                    else
                    {
                        icon = new Icon(value, new Size(16, 16));
                    }
                    this.OnIconChanged();
                    RecalculateSize();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the panel.
        /// </summary>
        /// <remarks>
        /// Set this property if you want the panel to display a predefined text representing key states, date/time information or culture information.
        /// </remarks>
        [Description("Indicates the type of the panel.")]
        [Category("Appearance")]
        [DefaultValue(StatusBarAdvPanelType.Custom)]
        public StatusBarAdvPanelType PanelType
        {
            get 
            { 
                return type; 
            }
            set
            {
                if (type != value)
                {
                    timer.Enabled = false;
                    type = value;
                    this.OnTypeChanged();
                    if (type == StatusBarAdvPanelType.LongDate || type == StatusBarAdvPanelType.LongTime
                        || type == StatusBarAdvPanelType.ShortDate || type == StatusBarAdvPanelType.ShortTime
                        || type == StatusBarAdvPanelType.CurrentCulture || type == StatusBarAdvPanelType.ShortTime24Format || type == StatusBarAdvPanelType.LongTime24Format )
                    {
                        if (this.DesignMode)
                        {
                            if (this.ShowCustomText)
                                throw new ArgumentException("Custom text entry has to be disabled for the PanelType " + this.PanelType.ToString()
                                    + ". The custom text entry is enabled only when the selected PanelType is CapsLockState/InsertKeyState/NumLockState/ScrollLockState. Please disable the custom Text property");
                        }
                        timer.Enabled = true;
                    }
                    RecalculateSize();
                    Invalidate();
                }
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
        [Browsable(true),DefaultValue(false),
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
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }


        /// <summary>
        /// Gets or sets the alignment type of the text and icon of the panel.
        /// </summary>
        [Description("Indicates the alignment type of the text and icon of the panel.")]
        [Category("Appearance")]
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment Alignment
        {
            get 
            {
                return align; 
            }
            set
            {
                if (align != value && !this.sizeToContent)
                {
                    align = value;
                    this.OnAlignChanged();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text of the panel.
        /// </summary>
        /// <remarks>
        /// This property will be ignored if the PanelType property is set to a value different than Custom.
        /// </remarks>
        [Browsable(true)]
        [DefaultValue("")]
        public new string Text
        {
            get
            { 
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether custom text should be enabled.
        /// </summary>
        /// <value><c>true</c> if custom text should be enabled; otherwise, <c>false</c>.</value>
        /// <summary>
        [
        DefaultValue(false),
        Description("A value indicating whether control uses custom texts for displaying text."),
        Category("Appearance")
        ]
        public bool ShowCustomText
        {
            get
            {
                return m_showCustomText;
            }
            set
            {
                if (value != m_showCustomText)
                {
                    if (this.DesignMode)
                    {
                        if (this.PanelType == StatusBarAdvPanelType.CapsLockState ||
                           this.PanelType == StatusBarAdvPanelType.InsertKeyState ||
                           this.PanelType == StatusBarAdvPanelType.NumLockState ||
                           this.PanelType == StatusBarAdvPanelType.ScrollLockState)
                        {
                            m_showCustomText = value;
                            this.Invalidate();
                        }
                        else
                            throw new ArgumentException("Custom text entry is disabled for the PanelType " + this.PanelType.ToString() 
                                + ". The custom text entry is enabled only when the selected PanelType is CapsLockState/InsertKeyState/NumLockState/ScrollLockState.");
                    }

                    m_showCustomText = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the custom  ON text.
        /// </summary>
        /// <value>The custom on text.</value>
        /// <remarks>
        /// This value represents the CustomOnText of the StatusBarAdvPanel. 
        /// The CustomOnText will be set only if EnableCustomText is set to True.
        /// </remarks>
        [Category("Appearance")]
        [DefaultValue("Custom ON")]
        [Description("Determines the Custom Text for StatusBarAdvPanel instead of default Caps-ON/Scroll-ON/Insert/Num-ON Text. This will be applied only if EnableCustomText is set to True.")]
        public string CustomONText
        {
            get
             {
                 return customOnText;
            }
            set 
            {
                if (customOnText != value)
                {
                    customOnText = value;
                    Invalidate();
                 }
            }
        }

        /// <summary>
        /// Gets or sets the custom OFF text.
        /// </summary>
        /// <value>The custom OFF text.</value>
        /// <remarks>
        /// This value represents the CustomOFFText of the StatusBarAdvPanel. The CustomOnText is will be set only 
        /// if EnableCustomText is set to True.
        /// </remarks>
        [Category("Appearance")]
        [DefaultValue("Custom OFF")]
        [Description("Determines the Custom OFF Text for StatusBarAdvPanel instead of default Caps-OFF/Scroll-OFF/NUM-OFF/Overwrite Text. This will be applied only if EnableCustomText is set to True")]
        public string CustomOFFText
        {
            get
             {
                 return customOffText;
            }
            set 
            {
                if (customOffText != value)
                {
                    customOffText = value;
                    Invalidate();
                 }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether control uses 
        /// marquee style for displaying text.
        /// </summary>
        [
        DefaultValue(false),
        Description("A value indicating whether control uses marquee style for displaying text."),
        Category("Appearance")
        ]
        public bool IsMarquee
        {
            get
            {
                return m_bIsMarquee;
            }
            set
            {
                if (value != m_bIsMarquee)
                {
                    m_bIsMarquee = value;
                    OnIsMarqueeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets animation speed of marquee style.
        /// </summary>
        [
        DefaultValue(DEF_ANIMATION_SPEED),
        Description("Animation speed of marquee style."),
        Category("Behavior")
        ]
        public int AnimationSpeed
        {
            get
            {
                return m_animationSpeed;
            }
            set
            {
                if (value != m_animationSpeed)
                {
                    m_animationSpeed = value;
                    OnAnimationSpeedChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets delay for animation of marquee style.
        /// </summary>
        [
        DefaultValue(DEF_ANIMATION_DELAY),
        Description("Delay for animation of marquee style."),
        Category("Behavior")
        ]
        public int AnimationDelay
        {
            get
            {
                return m_animationDelay;
            }
            set
            {
                if (value != m_animationDelay)
                {
                    m_animationDelay = value;
                    OnAnimationDelayChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets direction of animation for marquee style.
        /// </summary>
        [
        DefaultValue(MarqueeDirection.Left),
        Description("Direction of animation for marquee style."),
        Category("Behavior")
        ]
        public MarqueeDirection AnimationDirection
        {
            get
            {
                return m_enAnimationDirection;
            }
            set
            {
                if (value != m_enAnimationDirection)
                {
                    m_enAnimationDirection = value;
                    OnAnimationDirectionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets style of animation for marquee style.
        /// </summary>
        [
        DefaultValue(MarqueeStyle.Scroll),
        Description("Style of animation for marquee style."),
        Category("Behavior")
        ]
        public MarqueeStyle AnimationStyle
        {
            get
            {
                return m_enAnimationStyle;
            }
            set
            {
                if (value != m_enAnimationStyle)
                {
                    m_enAnimationStyle = value;
                    OnAnimationStyleChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the StatusBarAdvPanel class.
        /// </summary>
        public StatusBarAdvPanel()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(StatusBarAdvPanel));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            constraints = new FlowLayoutConstraints(true, HorzFlowAlign.Left, VertFlowAlign.Top, false, false, false);
            keyListener = new KeyListener(this);
            MessageFilterEntryHelper.AddMessageFilter(keyListener, false);
            this.preferredSize = Size.Empty;
            this.ThemedBorder = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            RecalculateSize();

            m_toolTip = new ToolTip();
            m_toolTip.InitialDelay = 0;
            m_toolTip.AutomaticDelay = 0;
            m_toolTip.AutoPopDelay = 0;
            m_toolTip.ReshowDelay = 0;
            m_toolTip.ShowAlways = true;
            m_toolTip.SetToolTip(this, toolTip);

            // TODO: Add any initialization after the InitForm call
            m_initialInsertState = StatusBarAdvNativeMethods.GetKeyState(VK_INSERT);

            InitializeAnimation();

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
            {
                tcd = new ThemedControlDrawing(ThemedControls.STATUS, this);
            }
            CTRLSIZE = this.Size;

#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
            this.Margin = new Padding(0);
#endif
        }

        private void InitializeAnimation()
        {
            m_animator = new AnimationHelper();
            m_animator.AnimationPositionChanged += new EventHandler(Animator_AnimationPositionChanged);
            m_animator.AnimationDone += new EventHandler(Animator_AnimationDone);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.Parent is IThemedControl)
                this.ThemesEnabled = (this.Parent as IThemedControl).ThemesEnabled;
        }

        /// <summary>
        /// Insert key KeyCode.
        /// </summary>
        /// <summary>
        /// Used to determine, has insert state changed from initial,
        /// when Insert key is pressed.
        /// </summary>
        private short m_initialInsertState = 0;

        private bool ShouldSerializePreferredSize()
        {
            if (this.preferredSize == Size.Empty)
                return false;
            else
                return true;
        }
        private void ResetPreferredSize()
        {
            if (HAlign != HorzFlowAlign.Justify)
            {
                this.PreferredSize = Size.Empty;
            }
            else
            {
                if (DesignMode) MessageBox.Show("You cannot reset this property when the HAlign is Justify");
            }
        }

        /// <summary> 
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Booll disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (m_toolTip != null)
                {
                    m_toolTip.RemoveAll();
                    m_toolTip.Dispose();
                }

                MessageFilterEntryHelper.RemoveMessageFilter(keyListener);

                if (this.tcd != null)
                {
                    this.tcd.Dispose();
                    this.tcd = null;
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
            this.timer = new System.Windows.Forms.Timer(this.components);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // StatusBarAdvPanel
            // 
            this.Size = new System.Drawing.Size(100, 16);
            this.SizeChanged += new System.EventHandler(this.StatusBarAdvPanel_SizeChanged);
            this.ThemeChanged += new System.EventHandler(this.StatusBarAdvPanel_ThemeChanged);

        }
        #endregion

        /// <summary>
        /// Starts animation for marquee style.
        /// </summary>
        public void StartAnimation()
        {
            m_bAnimationStarted = true;
            m_enPerformingDirection = this.AnimationDirection;
            BeginAnimation();
        }

        /// <summary>
        /// Stops animation.
        /// </summary>
        public void StopAnimation()
        {
            m_bAnimationStarted = false;
            EndAnimation();
            Invalidate();
        }

        /// <summary>
        /// Begin animation.
        /// </summary>
        private void BeginAnimation()
        {
            if (!m_bAnimationStarted || !this.IsMarquee || this.AnimationSpeed == 0 || this.AnimationDelay == 0) return;

            this.m_animationPoint = GetStartAnimationPoint(m_enPerformingDirection);
            Size textSize = GetTextSize();
            int positions = 0;
            int offsetIcon = (icon != null) ? this.icon.Width + DEF_IMAGE_GAP : 0;

            // calculates count position of animation
            switch (this.AnimationStyle)
            {
                case MarqueeStyle.Scroll:
                    {
                        positions = (textSize.Width + this.Size.Width + offsetIcon) / this.AnimationSpeed;
                        break;
                    }
                case MarqueeStyle.Slide:
                    {
                        switch (this.Alignment)
                        {
                            case HorizontalAlignment.Left:
                                {
                                    positions = (m_enPerformingDirection == MarqueeDirection.Left) ?
                                        this.ClientRectangle.Width : textSize.Width + offsetIcon;
                                    break;
                                }
                            case HorizontalAlignment.Right:
                                {
                                    positions = (m_enPerformingDirection == MarqueeDirection.Left) ?
                                        textSize.Width + offsetIcon : this.ClientRectangle.Width;
                                    break;
                                }
                            case HorizontalAlignment.Center:
                                {
                                    positions = (this.ClientRectangle.Width - textSize.Width - offsetIcon) / 2;
                                    positions += textSize.Width + offsetIcon;
                                    break;
                                }
                        }

                        positions = positions / this.AnimationSpeed;

                        break;
                    }
                case MarqueeStyle.Alternate:
                    {
                        positions = (this.ClientRectangle.Width > textSize.Width) ?
                            this.Size.Width - textSize.Width - offsetIcon :
                            textSize.Width + offsetIcon - this.Size.Width;

                        positions = positions / this.AnimationSpeed;

                        break;
                    }
            }

            // starts animation
            m_animator.StartAnimation(positions, true, this.AnimationDelay);
        }

        /// <summary>
        /// Ends animation.
        /// </summary>
        private void EndAnimation()
        {
            if (m_animator.AnimationOn)
            {
                this.m_animationPoint = Point.Empty;
                m_animator.StopAnimation();
            }
        }

        /// <summary>
        /// Restarts animation.
        /// </summary>
        private void RefreshAnimation()
        {
            if (m_animator.AnimationOn)
            {
                this.EndAnimation();
            }

            if (this.IsMarquee)
            {
                m_enPerformingDirection = this.AnimationDirection;
                this.BeginAnimation();
            }
        }

        /// <summary>
        /// Gets size displayed text.
        /// </summary>
        /// <returns>Return Size</returns>
        private Size GetTextSize()
        {
            string txt = GetText();
            Graphics g = this.CreateGraphics();
            Size size = g.MeasureString(txt, this.Font).ToSize();

            return size;
        }

        /// <summary>
        /// Gets start point of animation.
        /// </summary>
        /// <param name="direction">Marquee Direction</param>
        /// <returns>Return start point</returns>
        private Point GetStartAnimationPoint(MarqueeDirection direction)
        {
            Size textSize = GetTextSize();

            int offsetIcon = (this.icon != null) ? this.icon.Width + DEF_IMAGE_GAP : 0;

            int x = 0;

            switch (this.AnimationStyle)
            {
                case MarqueeStyle.Scroll:
                    {
                        x = (direction == MarqueeDirection.Left) ?
                            this.ClientRectangle.Right :
                            this.ClientRectangle.Left - textSize.Width - offsetIcon;

                        break;
                    }
                case MarqueeStyle.Slide:
                    {
                        x = (direction == MarqueeDirection.Left) ?
                            this.ClientRectangle.Right :
                            this.ClientRectangle.Left - textSize.Width - offsetIcon;

                        break;
                    }
                case MarqueeStyle.Alternate:
                    {
                        int offset = this.Size.Width - textSize.Width - offsetIcon;

                        if (this.ClientRectangle.Width > textSize.Width)
                        {
                            x = (direction == MarqueeDirection.Left) ? offset : 0;
                        }
                        else
                        {
                            x = (direction == MarqueeDirection.Left) ? 0 : offset;
                        }

                        break;
                    }
            }

            int y = (this.ClientRectangle.Height - textSize.Height) / 2;

            Point point = new Point(x, y);

            return point;
        }

        private void RecalculateSize()
        {
            if (!this.sizeToContent) return;
            int width = 0;
            int offset = 0;
            if (icon != null)
            {
                width += icon.Width + 2;
            }
            Graphics g = CreateGraphics();
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            offset = Convert.ToInt32(Math.Ceiling(g.MeasureString(" ", this.Font).Width));
            width += g.MeasureString(GetText(), Font).ToSize().Width + offset;
            this.ClientSize = new Size(width, this.ClientSize.Height);
            this.MinimumSize = this.Size;
            this.PreferredSize = this.Size;
            g.Dispose();
        }

        /// <summary>
        /// Method returns text according to the KeyState.
        /// </summary>
        /// <returns>returns string</returns>
        public string GetText()
        {
            short capsState = StatusBarAdvNativeMethods.GetKeyState(0x14 /*VK_CAPTIAL*/);
            short numState = StatusBarAdvNativeMethods.GetKeyState(0x90 /*VK_NUMLOCK*/);
            short scrollState = StatusBarAdvNativeMethods.GetKeyState(0x91 /*VK_SCROLL*/);
            short insertState = StatusBarAdvNativeMethods.GetKeyState(0x2D /*VK_INSERT*/);

            bool capsKeyON = StatusBarAdvNativeMethods.LOWORD(capsState) != 0 && KeyState(capsState);
            bool numKeyON = StatusBarAdvNativeMethods.LOWORD(numState) != 0 && KeyState(numState);
            bool scrollKeyON = StatusBarAdvNativeMethods.LOWORD(scrollState) != 0 && KeyState(scrollState);
            bool insertKeyON = StatusBarAdvNativeMethods.LOWORD(insertState) != 0 && KeyState(insertState);

            string txt = text;
            switch (type)
            {
                case StatusBarAdvPanelType.Custom: txt = text; 
                    break;
                case StatusBarAdvPanelType.CapsLockState:
                    if (!m_showCustomText)
                    {
                        txt = capsKeyON ? "CapsLock ON" : "CapsLock OFF";
                    }
                    else 
                    {
                        txt = capsKeyON ? customOnText : customOffText;
                    }
                    break;
                case StatusBarAdvPanelType.NumLockState:
                    if (!m_showCustomText)
                    {
                        txt = numKeyON ? "NumLock ON" : "NumLock OFF";
                    }
                    else 
                    {
                        txt = numKeyON ? customOnText : customOffText;
                    }
                    break;
                case StatusBarAdvPanelType.ScrollLockState:
                    if (!m_showCustomText)
                    {
                        txt = scrollKeyON ? "ScrollLock ON" : "ScrollLock OFF";
                    }
                    else
                    {
                        txt = scrollKeyON ? customOnText : customOffText;
                    }
                    break;
                case StatusBarAdvPanelType.LongDate: txt = DateTime.Now.ToLongDateString();
                    break;
                case StatusBarAdvPanelType.ShortDate: txt = DateTime.Now.ToShortDateString(); 
                    break;
                case StatusBarAdvPanelType.LongTime: txt = DateTime.Now.ToLongTimeString(); 
                    break;
                case StatusBarAdvPanelType.ShortTime: txt = DateTime.Now.ToShortTimeString(); 
                    break;
				case StatusBarAdvPanelType.ShortTime24Format: txt = DateTime.Now.ToString("HH:mm");
					break;
				case StatusBarAdvPanelType.LongTime24Format: txt = DateTime.Now.ToString("HH:mm:ss");
					break;
                case StatusBarAdvPanelType.CurrentCulture: txt = System.Globalization.CultureInfo.CurrentCulture.DisplayName; 
                    break;
                case StatusBarAdvPanelType.InsertKeyState:
                    if (!m_showCustomText)
                    {
                        txt = insertKeyON ? "Insert" : "Overwrite";
                    }
                    else
                    {
                        txt = insertKeyON ? customOnText : customOffText;
                    }
                    break;
            }
            return txt;
        }

        private bool themesEnabled = false;

        /// <summary>
        /// Gets or sets a value indicating whether the background color will be set to Transparent. Indicated settings: BorderStyle:Fixed3D, Border3DStyle = Etched.
        /// </summary>
        [Description("Indicates if the background color will be set to Transparent. Indicated settings:BorderSides = Right, BorderStyle = Fixed3D, Border3DStyle = Etched.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public new bool ThemesEnabled
        {
            get 
            { 
                return themesEnabled; 
            }
            set
            {
                base.ThemesEnabled = false;
                if (themesEnabled != value)
                {
                    themesEnabled = value;
                    if (value)
                    {
                        base.BackColor = Color.Transparent;
                    }
                    else
                    {
                        base.BackColor = this.oldBackColor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control will ignore the theme's background color and draw the backcolor instead. 
        /// </summary>
        [Browsable(false)]
        public new bool IgnoreThemeBackground
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets the backcolor of the panel.
        /// </summary>
        public new Color BackColor
        {
            get 
            { 
                return oldBackColor; 
            }
            set
            {
                if (oldBackColor != value)
                {
                    oldBackColor = value;
                    if (!ThemesEnabled)
                    {
                        base.BackColor = value;
                    }
                }
            }
        }

        /// <summary>
        /// Raises the paint event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            // if(this.ThemesEnabled && !this.IgnoreThemeBackground)
            base.OnPaint(e);

            if (null != tcd)
                tcd.DrawMirrored = this.RightToLeft == RightToLeft.Yes;

            string txt = GetText();

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

            Size sz = e.Graphics.MeasureString(txt, Font).ToSize();
            Point pt = new Point(0, 0);
            switch (align)
            {
                case HorizontalAlignment.Left: pt = new Point(2, (ClientRectangle.Height - sz.Height) / 2); 
                    break;
                case HorizontalAlignment.Center: pt = new Point((ClientRectangle.Width - sz.Width - (icon == null ? 0 : icon.Width)) / 2, (ClientRectangle.Height - sz.Height) / 2); 
                    break;
                case HorizontalAlignment.Right: pt = new Point(ClientRectangle.Width - sz.Width - 3 - (icon == null ? 0 : icon.Width), (ClientRectangle.Height - sz.Height) / 2); 
                    break;
            }

            Point location = (this.m_animationPoint == Point.Empty) ? pt : this.m_animationPoint;

            if (icon != null)
            {
                e.Graphics.DrawIcon(icon, location.X, (ClientRectangle.Height - icon.Height) / 2);
                location.X += icon.Width + 2;
            }

            if (this.Enabled)
            {
               using(Brush brush= new SolidBrush(ForeColor))
                   e.Graphics.DrawString(txt, Font, brush, location);
            }
            else
            {
                Rectangle rectText = new Rectangle(location, sz);
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				ControlPaint.DrawStringDisabled( e.Graphics, txt, Font, BackColor, rectText, StringFormat.GenericTypographic );
#else
                ControlPaint.DrawStringDisabled(e.Graphics, txt, Font, BackColor, rectText, TextFormatFlags.Default);
#endif
            }
        }

        protected override void ThemedPaintBackground(Graphics graphics, Rectangle rect, Rectangle clip)
        {
            tcd.DrawThemeBackground(graphics, 1, 0, ClientRectangle);
        }

        private void StatusBarAdvPanel_SizeChanged(object sender, System.EventArgs e)
        {
            RecalculateSize();
            if (this.ShouldSerializePreferredSize() == false)
                this.OnPreferredSizeChanged();
            Invalidate();
        }
        internal void KeyListener_KeyEvent(Keys key)
        {
            if (PanelType == StatusBarAdvPanelType.CapsLockState && key == Keys.Capital)
                Invalidate();
            if (PanelType == StatusBarAdvPanelType.NumLockState && key == Keys.NumLock)
                Invalidate();
            if (PanelType == StatusBarAdvPanelType.ScrollLockState && key == Keys.Scroll)
                Invalidate();
            if (PanelType == StatusBarAdvPanelType.InsertKeyState && key == Keys.Insert)
                Invalidate();
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            Invalidate();
        }

      

        private void StatusBarAdvPanel_ThemeChanged(object sender, System.EventArgs e)
        {
            if (this.ThemesEnabled)
            {
                if (this.BackColor != Color.Transparent)
                {
                    this.oldBackColor = this.BackColor;
                    this.BackColor = Color.Transparent;
                }
            }
            else
            {
                this.BackColor = this.oldBackColor;
            }

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
            {
                tcd = new ThemedControlDrawing(ThemedControls.STATUS);
            }
        }

        private void Animator_AnimationPositionChanged(object sender, EventArgs e)
        {
            Point point = this.m_animationPoint;

            switch (m_enPerformingDirection)
            {
                case MarqueeDirection.Left:
                    {
                        point.Offset(-this.AnimationSpeed, 0);
                        break;
                    }
                case MarqueeDirection.Right:
                    {
                        point.Offset(this.AnimationSpeed, 0);
                        break;
                    }
            }

            this.m_animationPoint = point;
            Invalidate();
        }

        private void Animator_AnimationDone(object sender, EventArgs e)
        {
            if (this.IsMarquee && m_bAnimationStarted)
            {
                switch (this.AnimationStyle)
                {
                    case MarqueeStyle.Scroll:
                        {
                            BeginAnimation();
                            break;
                        }
                    case MarqueeStyle.Slide:
                        {
                            EndAnimation();
                            break;
                        }
                    case MarqueeStyle.Alternate:
                        {
                            if (m_enPerformingDirection == MarqueeDirection.Left)
                            {
                                m_enPerformingDirection = MarqueeDirection.Right;
                            }
                            else
                            {
                                m_enPerformingDirection = MarqueeDirection.Left;
                            }
                            BeginAnimation();

                            break;
                        }
                }
            }
            else if (!m_bAnimationStarted)
            {
                EndAnimation();
            }
        }
    }
    internal class KeyListener : IMessageFilter, IKeyboardProcHookClient
    {
       private StatusBarAdvPanel owner;

        public KeyListener(StatusBarAdvPanel own)
        {
            owner = own;
        }
        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0100:
                case 0x0101:
                    owner.KeyListener_KeyEvent((Keys)(int)m.WParam & Keys.KeyCode);

                    break;
            }
            return false;
        }

        // This will be called when hosted in a native app.
        bool IKeyboardProcHookClient.KeyboardHookProc(int wParam, int lParam)
        {
            Keys keys = (Keys)wParam;
            owner.KeyListener_KeyEvent(keys & Keys.KeyCode);
            return false;
        }
    }

    public enum StatusBarAdvPanelType
    {
        /// <summary>
        /// Represents custom
        /// </summary>
        Custom,

        /// <summary>
        ///  Num Lock state
        /// </summary>
        NumLockState, 
        
        /// <summary>
        /// Represents Caps Lock state
        /// </summary>
        CapsLockState, 
        
        /// <summary>
        /// Represents Scroll lock state
        /// </summary>
        ScrollLockState, 
        
        /// <summary>
        /// Represents Long date
        /// </summary>
        LongDate, 
        
        /// <summary>
        /// Represents Shortdate
        /// </summary>
        ShortDate, 

        /// <summary>
        /// Represents Longtime
        /// </summary>
        LongTime, 

        /// <summary>
        /// Represents Shorttime
        /// </summary>
        ShortTime, 
        
        /// <summary>        
        /// Represents Longtime in 24 Hours format
        /// </summary>
        LongTime24Format,

        /// <summary>
        /// Represents Shorttime in 24 Hours format
        /// </summary>
        ShortTime24Format,

        /// <summary>
        /// Represents Currentculture
        /// </summary>
        CurrentCulture,
 
        /// <summary>
        /// Represents InsertKeyState
        /// </summary>
        InsertKeyState
    }
    [ComVisibleAttribute(false), SuppressUnmanagedCodeSecurityAttribute()]
    internal class StatusBarAdvNativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff/*=~0x0000*/;
        }
        public static int LOWORD(int n)
        {
            return n & 0xffff/*=~0x0000*/;
        }
    }

    /// <summary>
    /// Direction of the displaying text for marquee style.
    /// </summary>
    public enum MarqueeDirection
    {
        /// <summary>
        /// Represents Left
        /// </summary>
        Left = 0,

        /// <summary>
        /// Represents Right
        /// </summary>
        Right = 1
    }

    /// <summary>
    /// Behaviour of the displayed text for marquee style.
    /// </summary>
    public enum MarqueeStyle
    {
        /// <summary>
        /// Represents scroll
        /// </summary>
        Scroll = 0,

        /// <summary>
        /// Represents Slide
        /// </summary>
        Slide = 1,
       
        /// <summary>
        /// Represent Alternate
        /// </summary>
        Alternate = 2
    }
}
