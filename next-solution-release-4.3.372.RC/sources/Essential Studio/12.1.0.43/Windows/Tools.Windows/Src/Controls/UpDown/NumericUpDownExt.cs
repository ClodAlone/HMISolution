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
using Syncfusion.Licensing.util.encoders;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
#endif

// This file contains the exact logic as the DomainUpDownExt.
namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Extends the NumericUpDown to provide XP Look and Feel.
    /// </summary>
    /// <seealso cref="DomainUpDownExt"/>
    /// <remarks>
    /// Take a look at the <see cref="NumericUpDownExt.ThemesEnabled"/> and
    /// <see cref="NumericUpDownExt.ThemedBorder"/> properties.
    /// </remarks>
    [
    Designer(
        typeof(Syncfusion.Windows.Forms.Tools.NumericUpDownExtDesigner),
        typeof(System.ComponentModel.Design.IDesigner)),
    ToolboxBitmap(typeof(NumericUpDownExt), "ToolboxIcons.NumericUpDownExt.bmp"),
    Description("")
    ]
    public class NumericUpDownExt :
        NumericUpDown,
        IThemedControl,
        INonClientPaintingSupport,
        ISupportOffice2007Theme,
        IVisualStyle 
    {
        private static readonly Color c_defaultBorderColor = Color.Black;
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
        private VisualStyle m_vStyle = VisualStyle.Default;
        private Office2007Theme m_colorScheme = Office2007Theme.Blue;
        private Office2010Theme m_colorScheme2010 = Office2010Theme.Blue;
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
                if (m_borderColor != value && (this.VisualStyle != VisualStyle.Office2010||this.VisualStyle != VisualStyle.Office2007))
                {
                    m_borderColor = value;
                    this.OnBorderColorChanged(EventArgs.Empty);
                    this.InvalidateWindow();
                }
            }
        }

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
                if (this.upDownEdit == null)
                    return 32767;
                else
                    return this.upDownEdit.MaxLength;
            }
            set
            {
                if (this.upDownEdit == null)
                    this.upDownEdit = this.GetTextBox();

                if (this.upDownEdit.MaxLength != value)
                {
                    this.upDownEdit.MaxLength = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only.
        /// </summary>
        public new bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                if (value != base.ReadOnly)
                {
                    base.ReadOnly = value;
                    OnReadOnlyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the value in hexadecimal numeration.
        /// </summary>
        [Description("Gets the value in hexadecimal numeration.")]
        [Browsable(false)]
        public string HexValue
        {
            get
            {
                string retValue = string.Empty;
                if (Hexadecimal)
                {
                    retValue = upDownEdit.Text;
                }
                else
                {
                    long val = (long)Value;
                    retValue = val.ToString("X");
                }

                return retValue;
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
        /// Gets or sets Office2007Theme for Office2007 style.
        /// </summary>
        [Description("Gets or sets Office2007Theme for Office2007 style.")]
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
                    OnColorSchemeChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets Office2010Theme for Office2010 style.
        /// </summary>
        [Description("Gets or sets Office2010Theme for Office2010 style.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Office2010Theme), "Blue")]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return m_colorScheme2010;
            }
            set
            {
                if (m_colorScheme2010 != value)
                {
                    m_colorScheme2010 = value;
                    OnColorSchemeChanged();
                }
            }
        }
        #endregion PROPERTIES

        #region INIT

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericUpDownExt"/> class.
        /// </summary>
        public NumericUpDownExt()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(NumericUpDownExt));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            CTRLSIZE = this.Size;
            Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied += new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
        }

        private void Init()
        {
            Control buttons = this.GetButtons();

            buttons.Paint += new PaintEventHandler(this.Buttons_Paint);
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
			buttons.MouseLeave += new EventHandler(Buttons_MouseLeave);
#endif

            TextBox textBox = this.GetTextBox();

            textBox.MouseEnter += new EventHandler(TextBox_MouseEnter);

            if (null == this.cd)
            {
                this.cd = new ControlDrawing();
            }

            if (XPThemes.IsThemedOS && null == this.themedSpinButtonDrawing && null == this.themedEditDrawing)
            {
                this.themedSpinButtonDrawing = new ThemedSpinButtonDrawing();
                this.themedEditDrawing = new ThemedEditDrawing();

                SetupThemesSupport(true);
            }
        }

        private void TextBox_MouseEnter(object sender, EventArgs e)
        {
            if (this.hitButton != ButtonID.None)
                this.hitButton = ButtonID.None;
        }

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
                if (this.VisualStyle != Forms.VisualStyle.Default && !this.ThemesEnabled)
                    cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        /// <summary>
        /// Cleans up any resources being used.Disposes the themed drawing enabled for the control.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.themedEditDrawing != null)
                {
                    this.themedEditDrawing.Dispose();
                    this.themedEditDrawing = null;
                }
                if (this.themedSpinButtonDrawing != null)
                {
                    this.themedSpinButtonDrawing.Dispose();
                    this.themedSpinButtonDrawing = null;
                }

                Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
                Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
            }
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
            //
            // So, instead fill the bg only in the border area.
            // 
            // Without this some 3D styles (SunkenOuter) will leave a 1 pixel transparent area.
            // g.FillRectangle(new SolidBrush(this.BackColor),bounds)
            int w = 2;
            if (this.VSBorderStyle == BorderStyle.FixedSingle)
                w = 1;

            // The borders as 4 rectangles
            Rectangle[] clipRects = new Rectangle[] { new Rectangle(bounds.Location, new Size(w, bounds.Height)), new Rectangle(bounds.Location, new Size(bounds.Width, w)), new Rectangle(bounds.Width - w, bounds.Y, w, bounds.Height),  new Rectangle(bounds.X, bounds.Height - w, bounds.Width, w) };

            // Fill the border-rectangles with the bg brush, since some of the 
            // 3d border types are only 1 pixel wide.
            for (int i = 0; i < 4; i++)
            {
                using(Brush brush =new SolidBrush(this.BackColor))
                    g.FillRectangle(brush, clipRects[i]);
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
        /// Raises the <see cref="System.Windows.Forms.Control.Validated"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        protected override void OnValidated(EventArgs e)
        {
            base.OnValidated(e);

            // This will invoke the ValueChanged event if necessary
            decimal value = this.Value;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (this.upDownEdit != null)
            {
                this.upDownEdit.BackColor = this.BackColor;
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Resize"></see> event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        /// <remarks >Updates the height of the control to the preferred height based on settings.</remarks>
        protected override void OnTextBoxResize(object source, EventArgs e)
        {
            this.UpdateHeight();
        }
       
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Layout"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.LayoutEventArgs"></see> that contains the event data.</param>
        /// <remarks >Overridden</remarks>
        protected override /*ScrollableControl*/ void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (this.IsHandleCreated)
            {
                this.PositionControls();
            }
        }
      
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.HandleCreated"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        /// <override/>
        protected override /*Control*/ void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            Init();

            this.PositionControls();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            Uninit();
        }

        private void Uninit()
        {
            Control buttons = this.GetButtons();
            TextBox textBox = this.GetTextBox();

            if (null != buttons)
            {
                buttons.Paint -= new PaintEventHandler(this.Buttons_Paint);
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
				buttons.MouseLeave -= new EventHandler(Buttons_MouseLeave);
#endif
            }

            if (null != textBox)
            {
                textBox.MouseEnter -= new EventHandler(TextBox_MouseEnter);
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.FontChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        /// <remarks >Overridden.Updates the height of the control to the preferred height based on fontchange</remarks>
        protected override /*Control*/ void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.UpdateHeight();
        }

        /// <summary>
        /// Gets or sets the alignment of the up and down buttons .
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
        /// Gets or sets the border style for the control.
        /// </summary>
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

                    if (this.IsHandleCreated)
                    {
                        this.PositionControls();
                    }
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
            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled && this.Visible)
            {
                base.Height = this.PreferredHeight;
                this.PositionControls();
            }
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

                this.Height = this.PreferredHeight;

                Size clientSize = base.ClientSize;
                LeftRightAlignment leftRightAlignment = base.RtlTranslateLeftRight(this.UpDownAlign);

                TextBox upDownEdit = this.GetTextBox();
                Control upDownButtons = this.GetButtons();

                int buttonWidth = 0;

                if (upDownButtons != null)
                {
                    buttonWidth = upDownButtons.Width;
                    int x = leftRightAlignment == LeftRightAlignment.Left ? 1 : clientSize.Width - (buttonWidth + 1);

                    upDownButtons.SetBounds(x, 1, buttonWidth, clientSize.Height - 2);
                    upDownButtons.Invalidate();
                }

                if (upDownEdit != null)
                {
                    Size szEdit = upDownEdit.GetPreferredSize(Size.Empty);

                    int x = leftRightAlignment == LeftRightAlignment.Left ? buttonWidth + 1 : 1;
                    int y = Math.Max(1, (clientSize.Height - szEdit.Height) / 2);

                    int w = clientSize.Width - buttonWidth - 2;
                    int h = clientSize.Height - 2 * y;

                    upDownEdit.SetBounds(x, y, w, h);
                }
            }
            finally
            {
                this.positioningControls = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Paint"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.PaintEventArgs"></see>  that contains the event data.</param>
        /// <remarks >Enables ThemedEditDrawing</remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            bool bThemesEnabled = XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled;

            Graphics g = e.Graphics;
            Color backColor = BackColor;
            if (BackColor == SystemColors.HotTrack)
            {
                backColor = Color.FromArgb(49, 106, 197);
            }
            base.OnPaint(e);

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if (bThemesEnabled && !ThemedBorder)
            {
                using (Brush brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }
#endif
            if (bThemesEnabled && ThemedBorder)
            {
                this.themedEditDrawing.DrawEditBoxBackground(g, this.ClientRectangle, this.Enabled);

                Rectangle rect = this.ClientRectangle;
                rect.Inflate(-2, -2);

                using (Brush brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            if (!bThemesEnabled)
            {
                Rectangle rect = this.ClientRectangle;
                using (Brush brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }

        protected virtual void Buttons_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Control button = sender as Control;

            Size clientSize = button.ClientSize;
            int n = clientSize.Height / 2;

            Rectangle topRect = new Rectangle(0, 0, clientSize.Width, n);
            Rectangle bottomRect = new Rectangle(0, n, clientSize.Width, n);

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
            {
                this.themedSpinButtonDrawing.DrawScrollButton(g, topRect, ButtonID.Up, ScrollButtonAppearance.Vertical, this.GetButtonStateForDrawing(ButtonID.Up));

                this.themedSpinButtonDrawing.DrawScrollButton(g, bottomRect, ButtonID.Down, ScrollButtonAppearance.Vertical, this.GetButtonStateForDrawing(ButtonID.Down));
            }
            else if (this.VisualStyle != VisualStyle.Default && m_renderer != null)
            {
                ButtonState upButtonState = GetButtonStateForDrawing(ButtonID.Up);
                ButtonState downButtonState = GetButtonStateForDrawing(ButtonID.Down);
                m_renderer.Render(g, upButtonState, downButtonState);
            }
            else
            {
                this.DrawUpDownButtons(e.Graphics, topRect, bottomRect);
            }
        }

        private void DrawUpDownButtons(Graphics g, Rectangle topRect, Rectangle bottomRect)
        {
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

                upRenderer.DrawBackground(g, topRect);

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

                downRenderer.DrawBackground(g, bottomRect);
            }
            else
            {
                ControlPaint.DrawScrollButton(g, topRect, System.Windows.Forms.ScrollButton.Up, ButtonState.Flat == upButtonState ? ButtonState.Normal : upButtonState);
                ControlPaint.DrawScrollButton(g, bottomRect, System.Windows.Forms.ScrollButton.Down, ButtonState.Flat == downButtonState ? ButtonState.Normal : downButtonState);
            }
#else
			ControlPaint.DrawScrollButton( g, topRect, System.Windows.Forms.ScrollButton.Up, ButtonState.Flat == upButtonState ? ButtonState.Normal : upButtonState );
			ControlPaint.DrawScrollButton( g, bottomRect, System.Windows.Forms.ScrollButton.Down, ButtonState.Flat == downButtonState ? ButtonState.Normal : downButtonState );
#endif
        }

        private ButtonState GetButtonStateForDrawing(ButtonID button)
        {
            ButtonState btnState = ButtonState.Flat;
            if (!this.Enabled)
            {
                btnState = ButtonState.Inactive;
            }
            else if (capturedButton == button)
            {
                btnState = ButtonState.Pushed;
            }
            else if (hitButton == button)
            {
                btnState = ButtonState.Normal;
            }

            return btnState;
        }
        #endregion THEMED_PAINTING

        #region Office2007 drawing
        protected void DrawOffice2007Buttons(Graphics g)
        {
            Control button = upDownButtons;
            Size clientSize = button.ClientSize;
            int n = clientSize.Height / 2;

            Rectangle topRect = new Rectangle(0, 0, clientSize.Width, n);
            Rectangle bottomRect = new Rectangle(0, n, clientSize.Width, n);

            using (Brush fillBrush = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(fillBrush, button.ClientRectangle);
            }

            Rectangle rectUp = topRect;
            rectUp.Inflate(0, -1);
            rectUp.Width -= 2;

            Rectangle rectDown = bottomRect;
            rectDown.Height -= 2;
            rectDown.Width -= 2;

            ButtonState upButtonState = GetButtonStateForDrawing(ButtonID.Up);
            ButtonState downButtonState = GetButtonStateForDrawing(ButtonID.Down);

            // draw up button
            DrawOffice2007Button(g, rectUp, ButtonID.Up, upButtonState);

            // draw down button
            DrawOffice2007Button(g, rectDown, ButtonID.Down, downButtonState);
        }

        private void DrawOffice2007Button(Graphics g, Rectangle rect, ButtonID button, ButtonState state)
        {
            // draw background
            DrawOffice2007ButtonBackground(g, rect, button, state);

            // draw border
            DrawOffice2007ButtonBorder(g, rect, button, state);

            // draw arrow
            DrawOffice2007ButtonArrow(g, rect, button, state);
        }

        protected virtual void DrawOffice2007ButtonBorder(Graphics g, Rectangle rect, ButtonID button, ButtonState state)
        {
            using (Pen pen = GetButtonBorderPen(state))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        protected virtual void DrawOffice2007ButtonArrow(Graphics g, Rectangle rect, ButtonID button, ButtonState state)
        {
            Point loc = (button == ButtonID.Down) ?
                new Point(rect.X + rect.Width / 2 - 3, rect.Y + rect.Height / 2 - 2) :
                new Point(rect.X + rect.Width / 2 - 3, rect.Bottom - rect.Height / 2 - 3);
            Size size = new Size(5, 3);

            Rectangle rcArrow = new Rectangle(loc, size);
            Point[] ptsdropdown;

            if (button == ButtonID.Up)
            {
                ptsdropdown = new Point[] { new Point( rcArrow.X + 3, rcArrow.Y + 1 ), new Point( rcArrow.X + 6, rcArrow.Y + 5 ), new Point( rcArrow.X, rcArrow.Y + 5 ) };
            }
            else
            {
                ptsdropdown = new Point[] { new Point( rcArrow.X + 6, rcArrow.Y + 1 ), new Point( rcArrow.X + 3, rcArrow.Y + 4 ), new Point( rcArrow.X + 1, rcArrow.Y + 1 ) };
            }

            GraphicsPath path = new GraphicsPath();
            path.AddLines(ptsdropdown);

            Brush fillBrush;

            if (state == ButtonState.Inactive)
            {
                fillBrush = new SolidBrush(SystemColors.ControlDark);
            }
            else
            {
                if (button == ButtonID.Up)
                {
                    fillBrush = new LinearGradientBrush(rcArrow, Office2007Colors.Default.NumericUpDownArrowLightColor, Office2007Colors.Default.NumericUpDownArrowDarkColor,  LinearGradientMode.Vertical);
                }
                else
                {
                    fillBrush = new LinearGradientBrush(rcArrow, Office2007Colors.Default.NumericUpDownArrowLightColor, Office2007Colors.Default.NumericUpDownArrowDarkColor, LinearGradientMode.Vertical);
                }
            }

            using (fillBrush)
            {
                g.FillPath(fillBrush, path);
                path.Dispose();
            }
        }

        protected virtual void DrawOffice2007ButtonBackground(Graphics g, Rectangle rect, ButtonID button, ButtonState state)
        {
            Rectangle fillRect = rect;
            Brush fillBrush = new SolidBrush(SystemColors.Window);

            if (state == ButtonState.Pushed)
            {
                fillBrush = new SolidBrush(Color.FromArgb(198, 231, 247));
            }
            else if (state == ButtonState.Flat)
            {
                fillBrush = new LinearGradientBrush(fillRect, Color.White, Color.FromArgb(214, 239, 255), LinearGradientMode.Vertical);
            }

            // draw background
            using (fillBrush)
            {
                g.FillRectangle(fillBrush, fillRect);
            }

            // draw shadow
            if (state == ButtonState.Pushed)
            {
                Pen pen = new Pen(Color.FromArgb(107, 142, 156));
                Point[] buttonPoints = null;

                if (button == ButtonID.Down)
                {
                    buttonPoints = new Point[] { new Point( rect.Left + 1, rect.Bottom - 1 ), new Point( rect.Left + 1, rect.Top + 1 ), new Point( rect.Right - 1, rect.Top + 1 ) };
                }
                else
                {
                    buttonPoints = new Point[] { new Point( rect.Left + 1, rect.Top + 1 ), new Point( rect.Left + 1, rect.Bottom - 1 ), new Point( rect.Right - 1, rect.Bottom - 1 ) };
                }

                using (pen)
                {
                    g.DrawLines(pen, buttonPoints);
                }
            }
        }

        private Pen GetButtonBorderPen(ButtonState buttonState)
        {
            if (this.VisualStyle == VisualStyle.Office2007)
            {
                Color color = Color.Empty;

                if (buttonState == ButtonState.Pushed)
                {
                    color = Office2007Colors.Default.NumericUpDownSelectedBorderColor;
                }
                else if (buttonState == ButtonState.Normal)
                {
                    color = Office2007Colors.Default.NumericUpDownHighLightedBorderColor;
                }
                else
                    color = Office2007Colors.Default.NumericUpDownBorderColor;

                return new Pen(color);
            }
            else if (this.VisualStyle == VisualStyle.Office2010)
            {
                Color color = Color.Empty;

                if (buttonState == ButtonState.Pushed)
                {
                    color = Office2010Colors.Default.NumericUpDownSelectedBorderColor;
                }
                else if (buttonState == ButtonState.Normal)
                {
                    color = Office2010Colors.Default.NumericUpDownHighLightedBorderColor;
                }
                else
                    color = Office2010Colors.Default.NumericUpDownBorderColor;

                return new Pen(color);
            }
            else
                return new Pen(Color.Black);
        }

        private void Office2007ManagedColorsApplied(Office2007Colors.ManagedColorsAppliedEventArgs args)
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
        private void Office2010ManagedColorsApplied(Office2010Colors.ManagedColorsAppliedEventArgs args)
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
                    if (this.BorderStyle == BorderStyle.None)
                        this.BorderStyle = BorderStyle.Fixed3D;

                    this.OnThemeChanged();

                    SetupThemesSupport(false);
                }
            }
        }

        private void SetupThemesSupport(bool bFromInit)
        {
            if (this.IsHandleCreated)
            {
                this.UpdateHeight();

                if (!bFromInit)
                {
                    this.RecreateHandle();
                    this.Invalidate(true);
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

        /// <summary>
        /// Gets or sets visual style that is used for drawing a control.
        /// </summary>
        [
        DefaultValue(VisualStyle.Default),
        Category(@"Appearance"),
        Description("Specifies visual style that is used for drawing a control."),
        TypeConverter(typeof(UpDownVisualStyleEnumFilter))
        ]
        public VisualStyle VisualStyle
        {
            get 
            { 
                return m_vStyle;
            }
            set
            {
                if (m_vStyle != value)
                {
                    m_vStyle = value;

                    this.OnStyleChanged();
                }
            }
        }
		/// <summary>
		///Metrocolor
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#D1D3D4");
		/// <summary>
		///Gets or Sets the MetroColor
		/// </summary>
        public Color MetroColor
        {
            get {
                return metroColor;
                
            }
            set { 
                metroColor = value;
               
                if (this.VisualStyle == VisualStyle.Metro)
                {
                 
                   this.OnStyleChanged();

                 
                }
                
            }
        }
        private void OnStyleChanged()
        {
            if (!this.ThemesEnabled)
            {
                if (this.VisualStyle == VisualStyle.Office2007)
                {
                    m_renderer = new UpDownOffice2007Renderer(this, m_colorScheme);
                    this.m_borderColor = (m_renderer as UpDownOffice2007Renderer).BorderColor;
                }
                else if (this.VisualStyle == VisualStyle.Office2010)
                {
                    m_renderer = new UpDownOffice2010Renderer(this, m_colorScheme2010);
                    this.m_borderColor = (m_renderer as UpDownOffice2010Renderer).BorderColor;
                }
                else if (this.VisualStyle == VisualStyle.Metro)
                {
                    if(this.DesignMode)
                    this.BorderStyle = BorderStyle.FixedSingle;
                    m_renderer = new Metrorender(this,metroColor);
                    this.m_borderColor = (m_renderer as Metrorender).BorderColor;
                    
                }
                else
                {
                    m_renderer = null;
                    ResetBackColor();
                    this.BorderColor = c_defaultBorderColor;
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
                    (m_renderer as UpDownOffice2010Renderer).ColorSheme = m_colorScheme2010;
                    this.m_borderColor = (m_renderer as UpDownOffice2010Renderer).BorderColor;
                    if (IsHandleCreated)
                    {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.WndProc"/>.
        /// </summary>
        /// <param name="m">Parameter messsage</param>
        protected override void WndProc(ref Message m)
        {
            if (this.cachedRgn != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(this.cachedRgn);
                this.cachedRgn = IntPtr.Zero;
            }
            if (m.Msg == 0x031A/*WM_THEMECHANGED*/)
            {
                this.RecreateHandle();
                this.Invalidate();
            }
            if (m.Msg == Syncfusion.Runtime.InteropServices.NativeMethods.WM_NCPAINT
                && !(XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled))
            {
                this.cachedRgn = DrawingUtils.NCPaintHelper(this, this, ref m);
            }
            if (this.ReadOnly && m.Msg == NativeMethods.WM_MOUSEWHEEL)
                return;
            base.WndProc(ref m);
        }

        private void OnThemeChanged()
        {
            if (this.VisualStyle != VisualStyle.Default)
            {
                if (themesEnabled)
                {
                    ResetBackColor();
                    this.BorderColor = c_defaultBorderColor;
                }
                else
                {
                    this.OnStyleChanged();
                }
            }

            if (this.BorderStyle == BorderStyle.None)
                this.BorderStyle = BorderStyle.Fixed3D;

            this.RaiseThemeChangedEvent(EventArgs.Empty);

            SetupThemesSupport(false);
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
        /// Occurs when Border3DStyle property is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when Border3DStyle property is changed.")]
        public event EventHandler Border3DStyleChanged;

        /// <summary>
        /// Occurs when BorderColor property is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when BorderColor property is changed.")]
        public event EventHandler BorderColorChanged;

        /// <summary>
        /// Occurs when BorderSides property is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when BorderSides property is changed.")]
        public event EventHandler BorderSidesChanged;

        /// <summary>
        /// Occurs when a <see cref="ReadOnly"/> is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when a ReadOnly is changed.")]
        public event EventHandler ReadOnlyChanged;

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

        private void RaiseReadOnlyChanged()
        {
            if (ReadOnlyChanged != null)
            {
                ReadOnlyChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises ReadOnlyChanged event.Called inorder to set the buttons disabled when control edit portion is set to readonly.
        /// </summary>
        protected virtual void OnReadOnlyChanged()
        {
            Control button = this.GetButtons();

            if (button != null)
            {
                button.Enabled = !this.ReadOnly;
            }

            this.InterceptArrowKeys = !this.ReadOnly;

            RaiseReadOnlyChanged();
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

#if SyncfusionFramework1_0 || SyncfusionFramework1_1
		private void Buttons_MouseLeave(object sender, EventArgs e)
		{
			this.HitButton = ButtonID.None;
			this.CapturedButton = ButtonID.None;
		}
#endif

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        /// <remarks >Captures Up and Down buttons based on position</remarks>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point ptScreen = Control.MousePosition;
            Point ptButtonsClient = this.GetButtons().PointToClient(ptScreen);

            if (this.GetButtons().ClientRectangle.Contains(ptButtonsClient))
            {
                this.CapturedButton = this.HitButton;
            }
            base.OnMouseDown(e);
        }
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
                if (ptButtonsClient.Y < (this.GetButtons().Height / 2))
                    this.HitButton = ButtonID.Up;
                else
                    this.HitButton = ButtonID.Down;
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
    }
    public class NumericUpDownExtDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public NumericUpDownExtDesigner()
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
                        new NumericUpDownExtActionList(this.Component));
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

    public class UpDownVisualStyleEnumFilter : DefaultVisualStyleEnumFilter
    {
        private static readonly VisualStyle[] r_valuesToSkip = new VisualStyle[] { VisualStyle.Office2003, VisualStyle.OfficeXP, VisualStyle.VS2005 };

        public UpDownVisualStyleEnumFilter(Type type)
            : base(type, (VisualStyle[])r_valuesToSkip.Clone())
        {
        }
    }
}

