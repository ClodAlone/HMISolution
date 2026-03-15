#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
using System.Windows.Forms.VisualStyles;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;

#endif

// This file contains the exact logic as the NumericUpDownExt.
namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Extends the DomainUpDown to provide XP Look and Feel.
    /// </summary>
    /// <seealso cref="NumericUpDownExt"/>
    /// <remarks>
    /// Take a look at the <see cref="DomainUpDownExt.ThemesEnabled"/> and
    /// <see cref="DomainUpDownExt.ThemedBorder"/> properties.
    /// </remarks>
    [
    Designer(
        typeof(Syncfusion.Windows.Forms.Tools.DomainUpDownExtDesigner),
        typeof(System.ComponentModel.Design.IDesigner)),
    ToolboxBitmap(typeof(DomainUpDownExt), "ToolboxIcons.DomainUpDownExt.bmp"),
    Description("Extends DomainUpDown control to provide Themed Look and Feel.")
    ]
    public class DomainUpDownExt :
        DomainUpDown,
        IThemedControl,
        INonClientPaintingSupport,
        IUpDownButtonsOrientable,
        ISupportOffice2007Theme,
        IVisualStyle 
    {
        private static readonly Color c_defualtBorderColor = Color.Black;
        private ThemedSpinButtonDrawing themedSpinButtonDrawing = null;
        private ThemedEditDrawing themedEditDrawing = null;
        private TextBox upDownEdit = null;
        private Control upDownButtons = null;
        private bool themesEnabled = false;
        private bool _themedBorder = true;
        private Border3DSide borderSides = Border3DSide.All;
        private Border3DStyle border3DStyle = Border3DStyle.Sunken;
        private Color m_borderColor = Color.Black;
        private ControlDrawing cd;
        private Orientation m_spinOrientation = Orientation.Vertical;
        private Rectangle m_rTopButton;
        private Rectangle m_rBottomButton;
        private bool m_bNativeButtonPress;
        private Timer m_timer;
        private VisualStyle m_visualStyle = VisualStyle.Default;
        private Office2007Theme m_colorScheme = Office2007Theme.Blue;
        private Office2010Theme m_office2010colorScheme = Office2010Theme.Blue;
        private IUpDownRenderer m_renderer;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        #region PROPERTIES
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
                    {
                        ApplyScaleToControl(1.5f);
                    }
                    else
                    {
                        ApplyScaleToControl(1.0f);
                    }
                }
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (!EnableTouchMode && this.DesignMode)
                CTRLSIZE = this.Size;
            base.OnSizeChanged(e);
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
            //this.m_rTopButton.Size = new Size(30,m_rBottomButton.Height);
            //this.m_rBottomButton.Size = new Size(30,m_rBottomButton.Height);
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        /// <summary>
        /// Gets or sets the border sides of the control that will be drawn in 3D mode.
        /// </summary>
        [Description("Indicates the border sides of the control to be drawn in 3D mode.")]
        [Category("Appearance")]
        [DefaultValue(Border3DSide.All)]
        public Border3DSide BorderSides
        {
            get 
            { 
                return borderSides; 
            }
            set
            {
                if (borderSides != value)
                {
                    borderSides = value;
                    this.OnBorderSidesChanged(EventArgs.Empty);
                    this.InvalidateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the 2D border.
        /// </summary>
        [Description("Indicates the color of the 2D border.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get 
            {
                return m_borderColor; 
            }
            set
            {
                if (m_borderColor != value && this.VisualStyle != VisualStyle.Office2007 && this.VisualStyle != VisualStyle.Office2010)
                {
                    m_borderColor = value;
                    this.OnBorderColorChanged(EventArgs.Empty);
                    this.InvalidateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the 3D border.
        /// </summary>
        /// <remarks>
        /// This style is used when the BorderStyle is Fixed3D and when XP Themed drawing is disabled.
        /// </remarks>
        [Description("Indicates the style of the 3D border.")]
        [Category("Appearance")]
        [DefaultValue(Border3DStyle.Sunken)]
        public Border3DStyle Border3DStyle
        {
            get 
            { 
                return border3DStyle; 
            }
            set
            {
                if (border3DStyle != value)
                {
                    border3DStyle = value;
                    this.OnBorder3DStyleChanged(EventArgs.Empty);
                    this.InvalidateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum length of the text that can be entered into the editable portion of the control.
        /// </summary>
        [Description("Gets or Sets the maximum length of the text that can be entered into the editable portion of the control.")]
        [Category("Behavior")]
        [DefaultValue(32767)]
        public int MaxLength
        {
            get
            { 
                return this.upDownEdit.MaxLength; 
            }
            set
            {
                if (this.upDownEdit.MaxLength != value)
                {
                    this.upDownEdit.MaxLength = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets UpDownButton's orientation.
        /// </summary>
        [Description("Gets or sets UpDownButton's orientation.")]
        [Category("Behavior")]
        [DefaultValue(typeof(Orientation), "Vertical")]
        public Orientation SpinOrientation
        {
            get
            {
                return m_spinOrientation;
            }
            set
            {
                if (m_spinOrientation != value)
                {
                    m_spinOrientation = value;
                    OnSpinOrientationChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets visual style that is used for drawing a control.
        /// </summary>
        [Description("Specifies visual style that is used for drawing a control.")]
        [Category("Appearance")]
        [DefaultValue(VisualStyle.Default)]
        [TypeConverter(typeof(UpDownVisualStyleEnumFilter))]
        public VisualStyle VisualStyle
        {
            get
            {
                return m_visualStyle;
            }
            set
            {
                if (m_visualStyle != value)
                {
                    m_visualStyle = value;
                    OnVisualStyleChanged();
                }
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;

                if (value == "Office2007Blue")
                {
                    VisualStyle = VisualStyle.Office2007;
                    ColorScheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    VisualStyle = VisualStyle.Office2007;
                    ColorScheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    VisualStyle = VisualStyle.Office2007;
                    ColorScheme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    VisualStyle = VisualStyle.Office2007;
                    ColorScheme = Office2007Theme.Managed;
                }
                else if (value == "Metro")
                    VisualStyle = VisualStyle.Metro;
                else if (value == "VS2010")
                    VisualStyle = VisualStyle.VS2010;
                else if (value == "Default")
                    VisualStyle = VisualStyle.Default;

            }
        }
        /// <summary>
        /// Gets or sets color scheme for control.
        /// </summary>
        [Description("Gets or sets color scheme for control.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Office2007Theme), "Blue")]
        public Office2007Theme ColorScheme
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
                    this.OnColorSchemeChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets color scheme for control.
        /// </summary>
        [Description("Gets or sets color scheme for control.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Office2010Theme), "Blue")]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return m_office2010colorScheme;
            }
            set
            {
                if (m_office2010colorScheme != value)
                {
                    m_office2010colorScheme = value;
                    this.OnColorSchemeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color. (overridden property)
        /// </summary>
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                if (base.BackColor != value)
                {
                    base.BackColor = value;
                    this.Refresh();
                }
            }
        }
        #endregion PROPERTIES

        #region INIT
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainUpDownExt"/> class.
        /// </summary>
        public DomainUpDownExt()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(DomainUpDownExt));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            Control buttons = this.GetButtons();
            buttons.Paint += new PaintEventHandler(this.Buttons_Paint);
            buttons.Resize += new EventHandler(this.Buttons_Resize);
            this.Buttons_Resize(null, EventArgs.Empty);
#if SyncfusionFramework1_0 ||SyncfusionFramework1_1
			buttons.MouseLeave += new EventHandler(Buttons_MouseLeave);
#endif

            this.cd = new ControlDrawing();

            if (XPThemes.IsThemedOS)
            {
                this.themedSpinButtonDrawing = new ThemedSpinButtonDrawing(this);
                this.themedEditDrawing = new ThemedEditDrawing(this);
            }

            this.Height += 1;
            CTRLSIZE = this.Size;

            Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied += new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
        }

        /// <summary>
        /// Overrides the <see cref="System.Windows.Forms.Control.CreateParams"></see> property.
        /// </summary>
        protected override /*ContainerControl*/ CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle &= ~0x200 /*WS_EX_CLIENTEDGE*/;
                cp.Style &= ~0x800000 /*WS_BORDER*/;
                if (!(XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled))
                {
                    if (this.VSBorderStyle == BorderStyle.FixedSingle)
                    {
                        cp.Style |= 0x800000;
                    }
                    if (this.VSBorderStyle == BorderStyle.Fixed3D)
                    {
                        cp.ExStyle |= 0x200;
                    }
                }
                cp.Style = cp.Style | (int)ControlStyles.AllPaintingInWmPaint | (int)ControlStyles.UserPaint | (int)WhidbeyCompatibleControlStyles.DoubleBuffer;
                return cp;
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.StopTimer();
            Control buttons = this.GetButtons();
            buttons.Paint -= new PaintEventHandler(this.Buttons_Paint);
            buttons.Resize -= new EventHandler(this.Buttons_Resize);
#if SyncfusionFramework1_0 ||SyncfusionFramework1_1
			buttons.MouseLeave -= new EventHandler(Buttons_MouseLeave);
#endif
            Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);

            if (this.themedSpinButtonDrawing != null)
            {
                this.themedSpinButtonDrawing.Dispose();
                this.themedSpinButtonDrawing = null;
            }

            if (this.themedEditDrawing != null)
            {
                this.themedEditDrawing.Dispose();
                this.themedEditDrawing = null;
            }

            upDownEdit = null;
            upDownButtons = null;
            m_renderer = null;
            cd = null;

            base.Dispose(disposing);
        }

        private TextBox GetTextBox()
        {
            if (this.upDownEdit != null)
                return this.upDownEdit;

            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    this.upDownEdit = control as TextBox;
                    break;
                }
            }
            return this.upDownEdit;
        }

        private Control GetButtons()
        {
            if (this.upDownButtons != null)
                return this.upDownButtons;

            foreach (Control control in this.Controls)
            {
                if (!(control is TextBox))
                {
                    this.upDownButtons = control;
                }
            }
            return this.upDownButtons;
        }
        #endregion INIT

        #region BORDER_DRAWING_STUFF
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            this.InvalidateWindow();
        }
        private void InvalidateWindow()
        {
            int redrawFlags = NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE;
            NativeMethodsHelper.RedrawWindow(this.Handle, redrawFlags);
        }
        private IntPtr cachedRgn = IntPtr.Zero;
        IntPtr INonClientPaintingSupport.NonClientPaint(PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = displayRect;

            // This is not good for the following reasons:
            // 1) When dragging a hidden tree into the visible desktop range, clipping
            // is not proper and as a result the BG is drawn over the whole control and stays there!
            // 2) Even in other scenarios, we can see the BG color being drawn first
            // followed by the tree. Causing a flicker effect.
            // So, instead fill the bg only in the border area.
            // Without this some 3D styles (SunkenOuter) will leave a 1 pixel transparent area.
            // g.FillRectangle(new SolidBrush(this.BackColor),bounds);
            int w = 2;
            if (this.VSBorderStyle == BorderStyle.FixedSingle)
                w = 1;

            // The borders as 4 rectangles
            Rectangle[] clipRects = new Rectangle[] { new Rectangle(bounds.Location, new Size(w, bounds.Height)), new Rectangle(bounds.Location, new Size(bounds.Width, w)), new Rectangle(bounds.Width - w, bounds.Y, w, bounds.Height), new Rectangle(bounds.X, bounds.Height - w, bounds.Width, w) };

            // Fill the border-rectangles with the bg brush, since some of the 
            // 3d border types are only 1 pixel wide.
            for (int i = 0; i < 4; i++)
            {
                g.FillRectangle(new SolidBrush(this.BackColor), clipRects[i]);
            }

            if (this.BorderSides != Border3DSide.All)
            {
                if (this.BorderSides != Border3DSide.Middle)
                    cd.DrawBorder(g, bounds, this.VSBorderStyle, this.border3DStyle, ButtonBorderStyle.Solid, this.m_borderColor, this.borderSides);
            }
            else
                cd.DrawBorder(g, bounds, this.VSBorderStyle, this.border3DStyle, ButtonBorderStyle.Solid, this.m_borderColor);

            // return a region excluding where you just drew.
            return NativeMethods.CreateRectRgn(windowRectInScreen.Left + w, windowRectInScreen.Top + w, windowRectInScreen.Right - w, windowRectInScreen.Bottom - w);
        }
        #endregion BORDER_DRAWING_STUFF

        #region OVERRIDES

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Resize"></see> event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        /// <remarks> Overridden.Sets the PreferredHeight and position controls based on settings</remarks>
        protected override void OnTextBoxResize(object source, EventArgs e)
        {
            this.UpdateHeight();
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Layout"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.LayoutEventArgs"></see> that contains the event data.</param>
        /// <remarks >Positions child controls based on settings.</remarks>
        protected override /*ScrollableControl*/ void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            this.PositionControls();
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.HandleCreated"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        /// <remarks > Overridden</remarks>
        protected override /*Control*/ void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.FontChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        /// <remarks > Overridden.</remarks>
        protected override /*Control*/ void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.UpdateHeight();

        }

        /// <summary>
        /// Gets or sets the alignment of the up and down buttons on the control.
        /// </summary>
        public new LeftRightAlignment UpDownAlign
        {
            get 
            { 
                return base.UpDownAlign; 
            }
            set
            {
                base.UpDownAlign = value;
                this.PositionControls();
            }
        }

        /// <summary>
        /// Gets or sets the border style for control.
        /// </summary>
        /// <remarks >Overridden.Cannot set BorderStyle to None when ThemesEnabled. Use ThemedBorder property to disable borders when themed.</remarks>
        public new BorderStyle BorderStyle
        {
            get
            { 
                return base.BorderStyle; 
            }
            set
            {
                if (base.BorderStyle != value)
                {
                    if (value == BorderStyle.None && this.ThemesEnabled)
                    {
                        if (this.DesignMode)
                            MessageBox.Show("Cannot set BorderStyle to None when ThemesEnabled. Use ThemedBorder property to disable borders when themed.");
                    }
                    else
                        base.BorderStyle = value;
                }
            }
        }

        private BorderStyle VSBorderStyle
        {
            get
            {
                return ((this.VisualStyle == VisualStyle.Office2007||this.VisualStyle == VisualStyle.Office2010) && !this.ThemesEnabled)
                    ? BorderStyle.FixedSingle
                    : this.BorderStyle;
            }
        }

        /// <summary>
        /// Updates the height of the control to the preferred height based on settings.
        /// </summary>
        protected virtual void UpdateHeight()
        {
            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
            {
                base.Height = this.PreferredHeight;
                this.PositionControls();
            }
        }

        /// <summary>
        /// Displays the previous item in the collection.
        /// </summary>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override void UpButton()
        {
            if (m_spinOrientation == Orientation.Vertical || m_bNativeButtonPress)
            {
                base.UpButton();
                m_bNativeButtonPress = false;
            }
        }

        /// <summary>
        /// Displays the next item in the object collection.
        /// </summary>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override void DownButton()
        {
            if (m_spinOrientation == Orientation.Vertical || m_bNativeButtonPress)
            {
                base.DownButton();
                m_bNativeButtonPress = false;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            m_bNativeButtonPress = true;
            base.OnMouseWheel(e);
        }
        protected override void OnTextBoxKeyDown(object source, KeyEventArgs e)
        {
            m_bNativeButtonPress = true;
            base.OnTextBoxKeyDown(source, e);
        }
        #endregion OVERRIDES

        #region THEMED_PAINTING
        private bool positioningControls = false;

        /// <summary>
        /// Positions child controls based on settings.
        /// </summary>
        protected virtual void PositionControls()
        {
            if (positioningControls)
                return;
            try
            {
                this.positioningControls = true;

                Size clientSize = base.ClientSize;
                LeftRightAlignment leftRightAlignment = base.RtlTranslateLeftRight(this.UpDownAlign);

                TextBox upDownEdit = this.GetTextBox();
                Control upDownButtons = this.GetButtons();

                Size btnSize = new Size(16, this.ClientRectangle.Height - 2);
                if (upDownButtons != null)
                {
                    btnSize = this.upDownButtons.Size;
                }

                if (this.VSBorderStyle == BorderStyle.None)
                {
                    base.Height -= 4;
                }
                else
                {
                    base.Height += 4;
                }

                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
                {
                    if (upDownEdit != null)
                    {
                        SizeF editSize;
                        using (Graphics editGfx = upDownEdit.CreateGraphics())
                        {
                            editSize = editGfx.MeasureString("Wg", upDownEdit.Font);
                        }

                        int top = (int)Math.Round(((float)clientSize.Height - editSize.Height) / 2);
                        int editTop = (top >= 0) ? top : 1;

                        upDownEdit.Size = new Size(clientSize.Width - btnSize.Width - 2, clientSize.Height - editTop - 2);
                        if (leftRightAlignment == LeftRightAlignment.Left)
                            upDownEdit.Location = new Point(btnSize.Width + 1, editTop);
                        else
                            upDownEdit.Location = new Point(1, editTop);
                    }

                    if (upDownButtons != null)
                    {
                        if (leftRightAlignment == LeftRightAlignment.Left)
                            upDownButtons.Location = new Point(1, 1);
                        else
                            upDownButtons.Location = new Point(clientSize.Width - btnSize.Width - 1, 1);

                        upDownButtons.Size = new Size(btnSize.Width, clientSize.Height - 2);
                        upDownButtons.Invalidate();
                    }
                }
            }
            finally
            {
                this.positioningControls = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see>  that contains the event data.</param>
        /// <remarks >Overridden and activate the themes </remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled
                && this.ThemedBorder)
            {
                this.themedEditDrawing.DrawEditBoxBackground(g, this.ClientRectangle, this.Enabled);
            }
            else
            {
                Rectangle rect = this.ClientRectangle;
                using (Brush brush = new SolidBrush(this.BackColor))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }

        private void Buttons_Resize(object sender, EventArgs e)
        {
            Control button = GetButtons();
            Size clientSize = button.ClientSize;
            int n;
            if (m_spinOrientation == Orientation.Vertical)
            {
                n = clientSize.Height / 2;
                m_rTopButton = new Rectangle(0, 0, clientSize.Width, n);
                m_rBottomButton = new Rectangle(0, n, clientSize.Width, n);
            }
            else
            {
                n = clientSize.Width / 2;
                m_rTopButton = new Rectangle(n, 0, n, clientSize.Height);
                m_rBottomButton = new Rectangle(0, 0, n, clientSize.Height);
            }
        }

        private void Buttons_Paint(object sender, PaintEventArgs e)
        {
            bool themed = XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled;
            if (m_visualStyle != VisualStyle.Default && m_renderer != null && !themed)
            {
                ButtonState upState = GetButtonStateForDrawing(ButtonID.Up);
                ButtonState downState = GetButtonStateForDrawing(ButtonID.Down);
                m_renderer.Render(e.Graphics, upState, downState);
            }
            else if (themed)
            {
                ScrollButtonAppearance orientation;
                if (m_spinOrientation == Orientation.Vertical)
                {
                    orientation = ScrollButtonAppearance.Vertical;
                }
                else
                {
                    orientation = ScrollButtonAppearance.Horizontal;
                }

                this.themedSpinButtonDrawing.DrawScrollButton(e.Graphics, m_rTopButton, ButtonID.Up, orientation, this.GetButtonStateForDrawing(ButtonID.Up));

                this.themedSpinButtonDrawing.DrawScrollButton(e.Graphics, m_rBottomButton, ButtonID.Down, orientation, this.GetButtonStateForDrawing(ButtonID.Down));
            }
            else
            {
                if (m_spinOrientation == Orientation.Horizontal)
                {
                    this.DrawHorizontalUpDownButtons(e.Graphics);
                }
                else
                {
                    this.DrawVerticalUpDownButtons(e.Graphics);
                }
            }
        }

        private void DrawVerticalUpDownButtons(Graphics g)
        {
            Size buttonsSize = this.upDownButtons.ClientSize;
            int width = buttonsSize.Width / 2;
            ButtonState upButtonState = GetButtonStateForDrawing(ButtonID.Up);
            ButtonState downButtonState = GetButtonStateForDrawing(ButtonID.Down);

#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
            if (Application.RenderWithVisualStyles)
            {
                VisualStyleElement element = VisualStyleElement.Spin.Up.Normal;
                VisualStyleRenderer upRenderer = new VisualStyleRenderer(element);

                element = VisualStyleElement.Spin.Down.Normal;
                VisualStyleRenderer downRenderer = new VisualStyleRenderer(element);

                if (!this.Enabled)
                {
                    upRenderer.SetParameters(VisualStyleElement.Spin.Up.Disabled);
                }
                else if (ButtonState.Pushed == upButtonState)
                {
                    upRenderer.SetParameters(VisualStyleElement.Spin.Up.Pressed);
                }
                else if (ButtonState.Normal == upButtonState)
                {
                    upRenderer.SetParameters(VisualStyleElement.Spin.Up.Hot);
                }

                upRenderer.DrawBackground(g, m_rTopButton);

                if (!this.Enabled)
                {
                    downRenderer.SetParameters(VisualStyleElement.Spin.Down.Disabled);
                }
                else if (ButtonState.Pushed == downButtonState)
                {
                    downRenderer.SetParameters(VisualStyleElement.Spin.Down.Pressed);
                }
                else if (ButtonState.Normal == downButtonState)
                {
                    downRenderer.SetParameters(VisualStyleElement.Spin.Down.Hot);
                }

                downRenderer.DrawBackground(g, m_rBottomButton);
            }
            else
            {
                DrawClassicScrollButtons(g, upButtonState, downButtonState);
            }
#else
			DrawClassicScrollButtons(g, upButtonState, downButtonState);
#endif
        }

        private void DrawHorizontalUpDownButtons(Graphics g)
        {
            Size buttonsSize = this.upDownButtons.ClientSize;
            int width = buttonsSize.Width / 2;
            ButtonState upButtonState = GetButtonStateForDrawing(ButtonID.Up);
            ButtonState downButtonState = GetButtonStateForDrawing(ButtonID.Down);

#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
            if (Application.RenderWithVisualStyles)
            {
                VisualStyleElement element = VisualStyleElement.Spin.UpHorizontal.Normal;
                VisualStyleRenderer upRenderer = new VisualStyleRenderer(element);

                element = VisualStyleElement.Spin.DownHorizontal.Normal;
                VisualStyleRenderer downRenderer = new VisualStyleRenderer(element);

                if (!this.Enabled)
                {
                    upRenderer.SetParameters(VisualStyleElement.Spin.UpHorizontal.Disabled);
                }
                else if (ButtonState.Pushed == upButtonState)
                {
                    upRenderer.SetParameters(VisualStyleElement.Spin.UpHorizontal.Pressed);
                }
                else if (ButtonState.Normal == upButtonState)
                {
                    upRenderer.SetParameters(VisualStyleElement.Spin.UpHorizontal.Hot);
                }

                upRenderer.DrawBackground(g, m_rTopButton);

                if (!this.Enabled)
                {
                    downRenderer.SetParameters(VisualStyleElement.Spin.DownHorizontal.Disabled);
                }
                else if (ButtonState.Pushed == downButtonState)
                {
                    downRenderer.SetParameters(VisualStyleElement.Spin.DownHorizontal.Pressed);
                }
                else if (ButtonState.Normal == downButtonState)
                {
                    downRenderer.SetParameters(VisualStyleElement.Spin.DownHorizontal.Hot);
                }

                downRenderer.DrawBackground(g, m_rBottomButton);
            }
            else
            {
                DrawClassicScrollButtons(g, upButtonState, downButtonState);
            }
#else
			DrawClassicScrollButtons(g, upButtonState, downButtonState);
#endif
        }

        private void DrawClassicScrollButtons(Graphics g, ButtonState upButtonState, ButtonState downButtonState)
        {
            if (this.SpinOrientation == Orientation.Horizontal)
            {
                ControlPaint.DrawButton(g, m_rTopButton, ButtonState.Flat == upButtonState ? ButtonState.Normal : upButtonState);
                DrawArrow(g, m_rTopButton, true);

                ControlPaint.DrawButton(g, m_rBottomButton, ButtonState.Flat == downButtonState ? ButtonState.Normal : downButtonState);
                DrawArrow(g, m_rBottomButton, false);
            }
            else
            {
                ControlPaint.DrawScrollButton(g, m_rTopButton, System.Windows.Forms.ScrollButton.Up, ButtonState.Flat == upButtonState ? ButtonState.Normal : upButtonState);
                ControlPaint.DrawScrollButton(g, m_rBottomButton, System.Windows.Forms.ScrollButton.Down, ButtonState.Flat == downButtonState ? ButtonState.Normal : downButtonState);
            }
        }

        private void DrawArrow(Graphics g, Rectangle rectangle, bool isRight)
        {
            int sign = 1;
            if (!isRight)
            {
                sign = -1;
            }
            Point p1 = new Point(rectangle.Width / 2 + sign + rectangle.X, (rectangle.Height - 1) / 2 + rectangle.Y);
            Point p2 = new Point(p1.X - sign * 2, p1.Y - 2);
            Point p3 = new Point(p1.X - sign * 2, p1.Y + 2);

            g.FillPolygon(new SolidBrush(Color.Black), new Point[] { p1, p2, p3 });
        }

        private ButtonState GetButtonStateForDrawing(ButtonID button)
        {
            ButtonState buttonState = ButtonState.Flat;
            if (!this.Enabled)
            {
                buttonState = ButtonState.Inactive;
            }
            else if (this.CapturedButton == button)
            {
                buttonState = ButtonState.Pushed;
            }
            else if (this.hitButton == button)
            {
                buttonState = ButtonState.Normal;
            }
            return buttonState;
        }
		/// <summary>
		///Metrocolor
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#D1D3D4");
		/// <summary>
		///Gets or Sets the metrocolor
		/// </summary>
        public Color MetroColor
        {
            get
            {
                return metroColor;
            }
            set
            {
                metroColor = value;
                if (m_visualStyle == VisualStyle.Metro)
                {
                    this.OnVisualStyleChanged();
                }
            }
        }
        private void OnVisualStyleChanged()
        {
            if (!this.ThemesEnabled)
            {
                if (m_visualStyle == VisualStyle.Office2007)
                {
                    m_renderer = new UpDownOffice2007Renderer(this, m_colorScheme);
                    this.m_borderColor = (m_renderer as UpDownOffice2007Renderer).BorderColor;
                }
                else if (m_visualStyle == VisualStyle.Office2010)
                {
                    m_renderer = new UpDownOffice2010Renderer(this, m_office2010colorScheme);
                    this.m_borderColor = (m_renderer as UpDownOffice2010Renderer).BorderColor;
                }
                else if (m_visualStyle == VisualStyle.Metro)
                {
                    if(this.DesignMode)
                    this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                    m_renderer = new Metrorender(this, metroColor);
                    this.m_borderColor = (m_renderer as Metrorender).BorderColor;
                }
                else
                {
                    m_renderer = null;
                    ResetBackColor();
                    this.BorderColor = c_defualtBorderColor;
                }
                if (IsHandleCreated)
                {
                    RecreateHandle();
                }
            }
        }
        private void OnColorSchemeChanged()
        {
            if (!this.ThemesEnabled)
            {
                if (VisualStyle == VisualStyle.Office2007)
                {
                    (m_renderer as UpDownOffice2007Renderer).ColorSheme = m_colorScheme;
                    this.m_borderColor = (m_renderer as UpDownOffice2007Renderer).BorderColor;
                    if (IsHandleCreated)
                    {
                        RecreateHandle();
                    }
                }
                else if (VisualStyle == VisualStyle.Office2010)
                {
                    (m_renderer as UpDownOffice2010Renderer).ColorSheme = m_office2010colorScheme;
                    this.m_borderColor = (m_renderer as UpDownOffice2010Renderer).BorderColor;
                    if (IsHandleCreated)
                    {
                        RecreateHandle();
                    }
                }
            }
        }
        #endregion THEMED_PAINTING

        #region THEMES_RELATED
        /// <summary>
        /// Fired when the ThemesEnabled property is changed.
        /// </summary>
        [Description("Fired when the ThemesEnabled property is changed.")]
        public event EventHandler ThemeChanged;

        /// <summary>
        /// Gets or sets a value indicating whether XP Themes (visual styles) should be used for this control when
        /// available.
        /// </summary>
        [
        DefaultValue(false),
        Category(@"Appearance"),
        Description("Specifies whether XP Themes (visual styles) should be used for this control when available.")
        ]
        public bool ThemesEnabled
        {
            get
            {
                return this.themesEnabled;
            }
            set
            {
                if (this.themesEnabled != value)
                {
                    this.themesEnabled = value;
                    this.OnThemeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether you want themed border around the control when themes are enabled.
        /// </summary>
        /// <remarks>
        /// This property is used only when the <see cref="ThemesEnabled"/> property is set.
        /// </remarks>
        [
        DefaultValue(true),
        Category(@"Appearance"),
        Description("Specifies whether or not you want themed border around the control when themes are enabled.")
        ]
        public bool ThemedBorder
        {
            get 
            { 
                return this._themedBorder;
            }
            set
            {
                if (this._themedBorder != value)
                {
                    this._themedBorder = value;
                    this.Invalidate();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (this.cachedRgn != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(this.cachedRgn);
                this.cachedRgn = IntPtr.Zero;
            }

            if (m.Msg == Syncfusion.Runtime.InteropServices.NativeMethods.WM_NCPAINT
                && !(XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled))
            {
                this.cachedRgn = DrawingUtils.NCPaintHelper(this, this, ref m);
            }

            base.WndProc(ref m);
        }

        private void OnThemeChanged()
        {
            if (this.VisualStyle != VisualStyle.Default)
            {
                if (themesEnabled)
                {
                    ResetBackColor();
                    this.BorderColor = c_defualtBorderColor;
                }
                else
                {
                    this.OnVisualStyleChanged();
                }
            }

            if (this.BorderStyle == BorderStyle.None)
                this.BorderStyle = BorderStyle.Fixed3D;

            this.RaiseThemeChangedEvent(EventArgs.Empty);

            if (IsHandleCreated)
            {
                this.RecreateHandle();
                this.UpdateHeight();
                this.Invalidate(true);
            }

            // NOTE: Workaround. Done for proper repositioning of upDownEdit
            // after theme style.
            this.upDownEdit.Width += 1;
        }

        /// <summary>
        /// Raises the ThemeChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks>
        /// <para>The RaiseThemeChangedEvent method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding RaiseThemeChangedEvent in a derived
        /// class, be sure to call the base class's RaiseThemeChangedEvent method so that
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void RaiseThemeChangedEvent(EventArgs e)
        {
            if (this.ThemeChanged != null)
            {
                try
                {
                    this.ThemeChanged(this, e);
                }
                catch 
                {
                }
            }
        }
        #endregion THEMES_RELATED
        #region EVENTS
        /// <summary>
        /// Occurs when border's 3D style is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when border's 3D style is changed.")]
        public event EventHandler Border3DStyleChanged;

        /// <summary>
        /// Occurs when border's color is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when border's color is changed.")]
        public event EventHandler BorderColorChanged;

        /// <summary>
        /// Occurs when border's sides are changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when border's sides are changed.")]
        public event EventHandler BorderSidesChanged;

        /// <summary>
        /// Occurs when the SpinOrientation property has changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when the SpinOrientation property has changed.")]
        public event EventHandler SpinOrientationChanged;

        /// <summary>
        /// Raises the Border3DStyleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBorder3DStyleChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnBorder3DStyleChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnBorder3DStyleChanged method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnBorder3DStyleChanged(EventArgs e)
        {
            if (Border3DStyleChanged != null) 
            { 
                Border3DStyleChanged(this, e); 
            }
        }

        /// <summary>
        /// Raises the BorderColorChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBorderColorChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnBorderColorChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnBorderColorChanged method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnBorderColorChanged(EventArgs e)
        {
            if (BorderColorChanged != null)
            {
                BorderColorChanged(this, e); 
            }
        }

        /// <summary>
        /// Raises the BorderSidesChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBorderSidesChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OmBorderSidesChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnBorderSidesChanged method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnBorderSidesChanged(EventArgs e)
        {
            if (BorderSidesChanged != null) 
            { 
                BorderSidesChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the SpinOrientationChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnSpinOrientationChanged(EventArgs e)
        {
            Control buttons = GetButtons();
            Buttons_Resize(buttons, EventArgs.Empty);
            buttons.Invalidate();
            this.StopTimer();
            if (SpinOrientationChanged != null)
            {
                SpinOrientationChanged(this, e);
            }
        }

        #endregion EVENTS

        #region BUTTON_HIT_STATE
        private ButtonID hitButton = ButtonID.None;
        private ButtonID HitButton
        {
            get
            { 
                return this.hitButton;
            }
            set
            {
                if (this.hitButton != value
                    &&
                    (value == ButtonID.None || this.capturedButton == ButtonID.None))
                {
                    this.hitButton = value;
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
					this.GetButtons().Invalidate();
#endif
                }
            }
        }

#if SyncfusionFramework1_0 ||SyncfusionFramework1_1
		private void Buttons_MouseLeave(object sender, EventArgs e)
		{
			this.HitButton = ButtonID.None;
			this.CapturedButton = ButtonID.None;
		}
#endif

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.MouseDown"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        /// <remarks >Gets the CapturedButton according to MousePosition</remarks>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point ptScreen = Control.MousePosition;
            Point ptButtonsClient = this.GetButtons().PointToClient(ptScreen);

            if (this.GetButtons().ClientRectangle.Contains(ptButtonsClient))
            {
                this.CapturedButton = this.HitButton;
                if (m_spinOrientation == Orientation.Horizontal)
                {
                    ProcessMouseDown();
                    this.StartTimer();
                }
            }
            base.OnMouseDown(e);
        }

        private void ProcessMouseDown()
        {
            m_bNativeButtonPress = true;
            if (this.capturedButton == ButtonID.Down ^ this.RightToLeft == RightToLeft.Yes)
            {
                this.UpButton();
            }
            else
            {
                this.DownButton();
            }
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (m_spinOrientation == Orientation.Horizontal)
            {
                this.StopTimer();
            }
            base.OnMouseUp(mevent);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.MouseMove"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        /// <remarks >Overridden and captures the Up or Down  buttons based on MousePosition.</remarks>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Control is very buggy!
            // This could get called by the button control when it hasn't captured the Mouse.
            // But it gets called with the Button's co-ords!
            // So ignoring the incoming bounds and finding it directly.
            Point ptScreen = Control.MousePosition;
            Point ptButtonsClient = this.GetButtons().PointToClient(ptScreen);

            if (Control.MouseButtons == MouseButtons.None)
                this.CapturedButton = ButtonID.None;

            if (this.GetButtons().ClientRectangle.Contains(ptButtonsClient))
            {
                this.HitButton = GetMouseHoverButton(ptButtonsClient);
            }
            else
            {
                this.HitButton = ButtonID.None;
            }
            base.OnMouseMove(e);
        }

#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
        protected override void OnMouseLeave(EventArgs e)
        {
            this.CapturedButton = ButtonID.None;
            this.HitButton = ButtonID.None;
            base.OnMouseLeave(e);
        }
#endif

        protected virtual ButtonID GetMouseHoverButton(Point ptButtonsClient)
        {
            ButtonID hitButton = ButtonID.None;
            if (m_rTopButton.Contains(ptButtonsClient))
            {
                hitButton = ButtonID.Up;
            }
            else if (m_rBottomButton.Contains(ptButtonsClient))
            {
                hitButton = ButtonID.Down;
            }
            return hitButton;
        }

        private ButtonID capturedButton = ButtonID.None;
        private ButtonID CapturedButton
        {
            get
            { 
                return this.capturedButton;
            }
            set
            {
                if (this.capturedButton != value)
                {
                    this.capturedButton = value;
                }
            }
        }
        #endregion BUTTON_HIT_STATE

        #region TIMER
        private void TimerTick(object sender, EventArgs e)
        {
            if (!GetButtons().Capture)
            {
                capturedButton = ButtonID.None;
                hitButton = ButtonID.None;
                this.StopTimer();
            }
            else
            {
                ProcessMouseDown();

                int timerInterval = m_timer.Interval;
                timerInterval *= 7;
                timerInterval /= 10;
                if (timerInterval < 1)
                {
                    timerInterval = 1;
                }
                m_timer.Interval = timerInterval;
            }
        }
        private void StartTimer()
        {
            if (m_timer == null)
            {
                m_timer = new Timer();
                m_timer.Tick += new EventHandler(this.TimerTick);
            }
            m_timer.Interval = 500;
            m_timer.Start();
        }
        private void StopTimer()
        {
            if (m_timer != null)
            {
                m_timer.Tick -= new EventHandler(this.TimerTick);
                m_timer.Stop();
                m_timer.Dispose();
                m_timer = null;
            }
        }
        #endregion

        #region ISupportOffice2007Theme implementation

        Office2007Theme ISupportOffice2007Theme.Office2007ColorTheme
        {
            get
            {
                return this.ColorScheme;
            }
            set
            {
                this.ColorScheme = value;
            }
        }

        void ISupportOffice2007Theme.EnableOffice2007Style()
        {
            this.VisualStyle = VisualStyle.Office2007;
        }

        #endregion

        #region Implementation

       public void Office2007ManagedColorsApplied(Office2007Colors.ManagedColorsAppliedEventArgs args)
        {
            if (this.VisualStyle == VisualStyle.Office2007)
            {
                Color borderColor = Office2007Colors.GetColorTable(this.ColorScheme).ComboBoxAdvNormalBorderColor;

                if (m_borderColor != borderColor)
                {
                    m_borderColor = borderColor;

                    Invalidate();
                }
            }
        }

       public void Office2010ManagedColorsApplied(Office2010Colors.ManagedColorsAppliedEventArgs args)
       {
           if (this.VisualStyle == VisualStyle.Office2010)
           {
               Color borderColor = Office2010Colors.GetColorTable(this.Office2010ColorScheme).ComboBoxAdvNormalBorderColor;

               if (m_borderColor != borderColor)
               {
                   m_borderColor = borderColor;

                   Invalidate();
               }
           }
       }

        #endregion
    }

    public class DomainUpDownExtDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public DomainUpDownExtDesigner()
            : base()
        {
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
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
                        new DomainUpDownExtActionList(this.Component));
                }
                return actionLists;
            }
        }

#endif
        public override System.Windows.Forms.Design.SelectionRules SelectionRules
        {
            get
            {
                return base.SelectionRules & ~(System.Windows.Forms.Design.SelectionRules.BottomSizeable | System.Windows.Forms.Design.SelectionRules.TopSizeable);
            }
        }
    }
}

