#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// CheckBoxAdv BackStyle
    /// </summary>
    public enum CheckBoxAdvBackStyle
    {
        /// <summary>
        /// Represent sDefault
        /// </summary>
        Default,

        /// <summary>
        /// Represents Horizontal Gradient
        /// </summary>
        HorizontalGradient,

        /// <summary>
        /// Represents Vertical Gradient
        /// </summary>
        VerticalGradient
    }

    /// <summary>
    /// CheckBoxAdv Style
    /// </summary>
    public enum CheckBoxAdvStyle
    {
        /// <summary>
        /// Classic appearance.
        /// </summary>
        Default,

        /// <summary>
        /// Office 2007-like appearance.
        /// </summary>
        Office2007,

        /// <summary>
        /// Office 2010-like appearance.
        /// </summary>
        Office2010,
        /// <summary>
        /// Metro-like appearance.
        /// </summary>
        Metro

    }

    /// <summary>
    /// The CheckBoxAdv control is an alternative to the .Net CheckBox control. It has a very 
    /// customizable border and background and supports advanced DataBinding.
    /// </summary>
    [Designer(typeof(CheckBoxAdvDesigner), typeof(System.ComponentModel.Design.IDesigner)),
    ToolboxBitmap(typeof(CheckBoxAdv), "ToolboxIcons.CheckBoxAdv.bmp"),
    DefaultEvent("CheckStateChanged"),
    DefaultProperty(@"BoolValue"),
    ToolboxItem(true),
    Description("Advanced CheckBox with customizable border, background and supports advanced DataBinding.")]
    public class CheckBoxAdv : CheckRadioBase, ISupportInitialize,IVisualStyle 
    {
        #region constants
        /// <summary>
        /// Internal rectangle border gradient angle.
        /// </summary>
        private const float InternalRectangleBorderAngle = 55f;

        /// <summary>
        /// Internal rectangle gradient angle.
        /// </summary>
        private const float InternalRectangleAngle = 50f;
        #endregion

        #region Fields

        /// <summary>
        /// Indicates the read only mode of the CheckBox.
        /// </summary>
        private bool breadOnly = false;

        /// <summary>
        /// Indicates the auto check mode of the CheckBoxAdv
        /// </summary>
        private bool autoCheck = true;  

        /// <summary>
        /// The check state of the checkbox.
        /// </summary>
        private CheckState mcheckState = CheckState.Unchecked;

        /// <summary>
        /// Indicates whether the undetermined state can be accessed through clicking.
        /// </summary>
        private bool tristate = false;

        /// <summary>
        /// The integer to get/set to the IntValue property when indeterminate.
        /// </summary>
        private int indeterminateInt = -1;

        /// <summary>
        /// The string to get/set to the StringValue property when indeterminate.
        /// </summary>
        private string strIndeterminateString = "Indeterminate";

        /// <summary>
        /// Specifies an advanced appearance this control.
        /// </summary>
        private CheckBoxAdvStyle style = CheckBoxAdvStyle.Default;

        /// <summary>
        /// Specifies office 2007 color scheme.
        /// </summary>
        private Office2007Theme colorScheme = Office2007Theme.Blue;

        /// <summary>
        /// Specifies office 2010 color scheme.
        /// </summary>
        private Office2010Theme color2010Scheme = Office2010Theme.Blue;
        /// <summary>
        /// Specifies Metro color.
        /// </summary>
        private Color metroColor = Color.FromArgb(88,89,91);

        /// <summary>
        /// Blend used for drawing normal rectangle.
        /// </summary>
        private Blend internalNormalRectBlend = null;

        /// <summary>
        /// Blend used for drawing selected and pushed rectangle.
        /// </summary>
        private Blend internalSelectedRectBlend = null;

        /// <summary>
        /// Points that used for drawing the tick.
        /// </summary>
        private Point[] tickPositions;
        #endregion Fields

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the CheckBoxAdv class.
        /// </summary>
        public CheckBoxAdv()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(CheckBoxAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.Init();


        }
  

        #endregion

        #region Events
        /// <summary>
        /// Occurs when the CheckState property changes.
        /// </summary>
        [Description("Occurs when the CheckState property changes.")]
        public event EventHandler CheckStateChanged;

        /// <summary>
        /// Occurs when the CheckState property changes.
        /// </summary>
        [Description("Occurs when the CheckState property changes.")]
        public event EventHandler CheckedChanged;

        /// <summary>
        /// Occurs when the BoolValue property changes.
        /// </summary>
        [Description("Occurs when the BoolValue property changes.")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler BoolValueChanged
        {
            add
            {
                this.CheckStateChanged += value;
            }

            remove
            {
                this.CheckStateChanged -= value;
            }
        }
        #endregion Events

        #region Properties

        #region Appearance
        /// <summary>
        /// Gets or sets a value indicating whether the read only mode of the CheckBox is set.
        /// </summary>
        [Description("Indicates the read only mode of the CheckBox.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ReadOnlyMode
        {
            get
            {
                return this.breadOnly;
            }

            set
            {
                this.breadOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets an advanced appearance for the checkBoxAdv.
        /// </summary>
        [Description("Gets or sets an advanced appearance for the checkBoxAdv.")]
        [Category("Appearance")]
        [DefaultValue(CheckBoxAdvStyle.Default)]
        public CheckBoxAdvStyle Style
        {
            get
            {
                return this.style;
            }

            set
            {
                if (this.style != value)
                {
                    this.style = value;
                    if (this.Style == CheckBoxAdvStyle.Metro)
                        this.DrawFocusRectangle = false;
                    this.OnStyleChanged();
                }
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string vStyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return vStyle;
            }
            set
            {
                vStyle = value;

                if (value == "Office2007Blue")
                    Office2007ColorScheme = Office2007Theme.Blue;
                else if (value == "Office2007Silver")
                    Office2007ColorScheme = Office2007Theme.Silver;
                else if (value == "Office2007Black")
                    Office2007ColorScheme = Office2007Theme.Black;
                else if (value == "Office2010Blue")
                    Office2010ColorScheme = Office2010Theme.Blue;
                else if (value == "Office2010Silver")
                    Office2010ColorScheme = Office2010Theme.Silver;
                else if (value == "Office2010Black")
                    Office2010ColorScheme = Office2010Theme.Black;
                else if (value == "Managed")
                {
                    if (this.Style == CheckBoxAdvStyle.Office2010)
                        Office2010ColorScheme = Office2010Theme.Managed;
                    else
                        Office2007ColorScheme = Office2007Theme.Managed;
                }
            }
        }
        /// <summary>
        /// Gets or sets office 2007 color scheme.
        /// </summary>
        [Description("Gets or sets office 2007 color scheme.")]
        [Category("Appearance")]
        [DefaultValue(Office2007Theme.Blue)]
        public Office2007Theme Office2007ColorScheme
        {
            get
            {
                return this.colorScheme;
            }

            set
            {
                if (this.colorScheme != value)
                {
                    this.colorScheme = value;
                    this.OnStyleChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets office 2010 color scheme.
        /// </summary>
        [Description("Gets or sets office 2010 color scheme.")]
        [Category("Appearance")]
        [DefaultValue(Office2010Theme.Blue)]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return this.color2010Scheme;
            }

            set
            {
                if (this.color2010Scheme != value)
                {
                    this.color2010Scheme = value;
                    this.OnStyleChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets the theme color of the CheckboxAdv
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

        /// <summary>
        /// Gets or sets a value indicating whether the CheckBox to automatically change state when clicked.
        /// </summary>
        [Description("Causes CheckBox to automatically change state when clicked.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool AutoCheck
        {
            get
            {
                return this.autoCheck;
            }

            set
            {
                this.autoCheck = value;
                this.Invalidate();
            }
        }

        #region Behavior Propertes

        /// <summary>
        ///Gets or sets a value indicating whether checked state of the CheckBox is checked or not.
        /// </summary>
        [Description("Gets or Sets the checked state of the CheckBox.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool Checked
        {
            get
            {
                return this.CheckState != CheckState.Unchecked;
            }

            set
            {
                if (value != this.Checked)
                {
                    this.CheckState = value ? CheckState.Checked : CheckState.Unchecked;
                }
            }
        }
        private bool useGDITextRender=false;
        /// <summary>
        ///Gets or sets a value indicating whether GDI Text renderer in CheckBox or not.
        /// </summary>
        [Description("Gets or Sets the use GDI Text renderer in CheckBox.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool UseGDITextRendering
        {
            get
            {
                return useGDITextRender;
            }

            set
            {
                if (value != useGDITextRender)
                {
                    useGDITextRender = value;
                    base.UseGDITextRendering = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the check state of the CheckBox.
        /// </summary>
        [Description("Gets or Sets the check state of the CheckBox.")]
        [Category("Behavior")]
        [DefaultValue(CheckState.Unchecked)]
        public CheckState CheckState
        {
            get
            {
                return this.mcheckState;
            }

            set
            {
                if (value == CheckState.Indeterminate)
                {
                    buttonState = ButtonState.All;
                }

                if (value == CheckState.Checked)
                {
                    buttonState = ButtonState.Checked;
                }

                if (value == CheckState.Unchecked)
                {
                    if (this.Enabled == true)
                    {
                        buttonState = ButtonState.Normal;
                    }
                }

                if (this.mcheckState != value)
                {
                    this.mcheckState = value;

                    Invalidate(true);

                    this.OnCheckedChanged(new CheckedChangedEventArgs(CheckedChangedEventArgs.SourceType.Programmatic));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether undetermined state can be accessed through clicking.
        /// </summary>
        [Description("Determines if undetermined state can be accessed through clicking."),
        Category("Behavior"),
        DefaultValue(false)]
        public bool Tristate
        {
            get
            {
                return this.tristate;
            }

            set
            {
                this.tristate = value;
                Invalidate();
            }
        }

       
        #endregion Behavior Propertes

        #region StateImage Properties
        /// <summary>
        /// Gets or sets the image used to draw the checkbox when indeterminate and mouse not over.
        /// </summary>
        [Description("The image used to draw the checkbox when indeterminate and mouse not over.")]
        [Category("StateImages")]
        [DefaultValue(null)]
        public Image IndeterminateImage
        {
            get
            {
                return mindeterminateImage;
            }

            set
            {
                if (mindeterminateImage != value)
                {
                    mindeterminateImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the image used to draw the checkbox when indeterminate and mouse over.
        /// </summary>
        [Description("Gets or sets the image used to draw the checkbox when  indeterminate and mouse over.")]
        [Category("StateImages")]
        [DefaultValue(null)]
        public Image MouseOverIndetermImage
        {
            get
            {
                return mmouseOverIndetermImage;
            }

            set
            {
                if (mmouseOverIndetermImage != value)
                {
                    mmouseOverIndetermImage = value;
                }
            }
        }

        #endregion StateImage Properties

        #region DataBinding Properties
        /// <summary>
        /// Gets or sets the integer value used when indeterminate.
        /// </summary>
        [Category("DataBinding")]
        [DefaultValue(-1)]
        [Description("Gets or sets the integer value used when indeterminate.")]
        public int IndeterminateInt
        {
            get
            {
                return this.indeterminateInt;
            }

            set
            {
                this.indeterminateInt = value;
            }
        }

        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        [Category("DataBinding")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the int value.")]

        public int IntValue
        {
            get
            {
                int val = 0;
                switch (this.mcheckState)
                {
                    case CheckState.Checked:
                        val = CheckedInt;
                        break;
                    case CheckState.Unchecked:
                        val = UncheckedInt;
                        break;
                    case CheckState.Indeterminate:
                        val = this.IndeterminateInt;
                        break;
                }

                return val;
            }

            set
            {
                if (value == CheckedInt)
                {
                    CheckState = CheckState.Checked;
                }

                if (value == UncheckedInt)
                {
                    CheckState = CheckState.Unchecked;
                }

                if (value == this.IndeterminateInt)
                {
                    CheckState = CheckState.Indeterminate;
                }

                Invalidate(true);
            }
        }

        /// <summary>
        /// Gets or sets the indeterminate string.
        /// </summary>       
        [Category("DataBinding")]
        [DefaultValue("Indeterminate")]
        [Description("Gets or sets the indeterminate string.")]
        public string IndeterminateString
        {
            get 
            { 
                return this.strIndeterminateString; 
            }

            set 
            { 
                this.strIndeterminateString = value; 
            }
        }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        [Category("DataBinding")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the string value.")]
        public string StringValue
        {
            get
            {
                // if(m_checkState == CheckState.Checked) return m_strCheckedString;
                // if(m_checkState == CheckState.Unchecked) return m_strUncheckedString;
                if (this.mcheckState == CheckState.Indeterminate)
                {
                    return this.strIndeterminateString;
                }

                return string.Empty;
            }

            set
            {
                // if(value == m_strCheckedString) CheckState = CheckState.Checked;
                // if(value == m_strUncheckedString) CheckState = CheckState.Unchecked;
                if (value == this.strIndeterminateString)
                {
                    CheckState = CheckState.Indeterminate;
                }

                Invalidate(true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Boolvalue is true or false  Used for data-binding.
        /// </summary>
        [Category("DataBinding"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DefaultValue(true),
        Description("Gets or sets a value indicating CheckState.")]
        public bool BoolValue
        {
            get
            {
                return this.mcheckState == CheckState.Checked;
            }

            set
            {
                if (this.BoolValue != value)
                {
                    CheckState = value ? CheckState.Checked : CheckState.Unchecked;
                    Invalidate(true);
                }
            }
        }
        #endregion DataBinding Properties

        #endregion Properties
  
        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        protected Office2007Colors Office2007ColorTable
        {
            get
            {
                Office2007Colors colorTable = Office2007Colors.GetColorTable(this.Office2007ColorScheme);

                return colorTable;
            }
        }
        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        protected Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = Office2010Colors.GetColorTable(this.Office2010ColorScheme);

                return colorTable;
            }
        }
        #endregion

        #region ISupportInitialize Members
        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            mbinitializing = true;
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            mbinitializing = false;
            RecalculatePositions();
            Invalidate();
        }

        #endregion
        #region Overrides
        /// <summary>
        /// Raises the CheckStateChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnCheckStateChanged(EventArgs e)
        {
            if (this.CheckStateChanged != null)
            {
                this.CheckStateChanged(this, e);
            }
        }

      
        /// <summary>
        /// Raises when Check property of the checkbox changes.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected virtual void OnCheckedChanged(CheckedChangedEventArgs e)
        {
            this.OnCheckStateChanged(e);

            if (this.CheckedChanged != null)
            {
                this.CheckedChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the paint event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <remarks >Overriden</remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!mbinitializing)
            {
                base.OnPaint(e);
                this.DrawCheckBox(e.Graphics);
            }
        }

        /// <summary>
        /// Raises when key down
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data. </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyData == Keys.Space)
            {
                if (autoCheck)
                {
                    this.ToggleCheckState(CheckedChangedEventArgs.SourceType.Keyboard);
                }
            }
        }

        /// <summary>
        /// Raises the MouseUp event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks >Overridden</remarks>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!this.breadOnly)
            {
                base.OnMouseUp(e);
                if (autoCheck)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        this.ToggleCheckState(CheckedChangedEventArgs.SourceType.Mouse);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the MouseEnter event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this.breadOnly)
            {
                base.OnMouseEnter(e);
            }
        }

        /// <summary>
        /// Raises the MouseDown event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.breadOnly)
            {
                base.OnMouseDown(e);
            }
        }

        /// <summary>
        /// Raises the Clicked event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            if (Enabled)
            {
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Raises the EnabledChanged event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        /// <remarks >Overridden  </remarks>
        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
            {
                if (this.mcheckState == CheckState.Unchecked)
                {
                    buttonState = ButtonState.Normal;
                }

                if (this.mcheckState == CheckState.Checked)
                {
                    buttonState = ButtonState.Checked;
                }

                if (this.mcheckState == CheckState.Indeterminate)
                {
                    buttonState = ButtonState.All;
                }
            }
            else
            {
                buttonState = ButtonState.Inactive;

                if (this.CheckState == CheckState.Checked)
                {
                    buttonState |= ButtonState.Checked;
                }
            }

            base.OnEnabledChanged(e);
        }

        /// <summary>
        /// Processes a mnemonic character.
        /// </summary>
        /// <param name="charCode">The character to process.</param>
        /// <returns>
        /// true depending on CheckState.
        /// </returns>
        protected override bool ProcessMnemonic(char charCode)
        {
            bool bsuccess = false;

            if (!this.breadOnly)
            {
                if (CanSelect && IsMnemonic(charCode, this.Text))
                {
                    this.CheckState = (this.CheckState == CheckState.Checked) ? CheckState.Unchecked : CheckState.Checked;

                    if (!this.Focused)
                        this.Focus();

                    bsuccess = true;
                }
            }

            return bsuccess;
        }

        /// <summary>
        /// Overrideen for raises when handle created.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this.IsHandleCreated)
            {
                RecalculatePositions();
            }
        }

        #endregion Overrides

        #region Helper Methods

        /// <summary>
        /// Init method
        /// </summary>
        private void Init()
        {
            this.internalNormalRectBlend = new Blend();
            this.internalNormalRectBlend.Positions = new float[] { 0f, 0.35f, 0.35f, 1f };
            this.internalNormalRectBlend.Factors = new float[] { 0f, 0.4f, 0.45f, 1f };

            this.internalSelectedRectBlend = new Blend();
            this.internalSelectedRectBlend.Positions = new float[] { 0f, 0.6f, 0.6f, 1f };
            this.internalSelectedRectBlend.Factors = new float[] { 0f, 0.6f, 0.65f, 1f };
        }

        /// <summary>
        /// Drawing CheckBox
        /// </summary>
        /// <param name="g"> Graphics object</param>
        private void DrawCheckBox(Graphics g)
        {
            if (mimageCheckBox)
            {
                Image image = null;
                if (mmouseOver && CheckState == CheckState.Checked)
                {
                    image = mmouseOverCheckedImage;
                    if (image == null)
                    {
                        image = mcheckedImage;
                    }
                }

                if (mmouseOver && CheckState == CheckState.Unchecked)
                {
                    image = mmouseOverUncheckedImage;
                    if (image == null)
                    {
                        image = muncheckedImage;
                    }
                }

                if (mmouseOver && CheckState == CheckState.Indeterminate)
                {
                    image = mmouseOverIndetermImage;
                    if (image == null)
                    {
                        image = mindeterminateImage;
                    }
                }

                if (!mmouseOver && CheckState == CheckState.Checked)
                {
                    image = mcheckedImage;
                }

                if (!mmouseOver && CheckState == CheckState.Unchecked)
                {
                    image = muncheckedImage;
                }

                if (!mmouseOver && CheckState == CheckState.Indeterminate)
                {
                    image = mindeterminateImage;
                }

                if (!Enabled)
                {
                    image = DisabledImage;
                }

                if (image == null)
                {
                    return;
                }

                if (mstretchImage)
                {
                    g.DrawImage(image, this.GetSpecificRectangle(ImageCheckBoxSize), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel); // checkBox.ClientRectangle,0,0,image.Width,image.Height,GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(image, this.GetSpecificRectangle(ImageCheckBoxSize), 0, 0, this.GetRectBoxWidth(), this.GetRectBoxHeight(), GraphicsUnit.Pixel);
                }
            }
            else
            {
                if (XPThemes.IsThemedOS && XPThemes.IsAppThemed && XPThemes.IsThemeActive && ThemesEnabled)
                {
                    int state = this.DetermineState();
                    tcd.DrawThemeBackground(g, ThemeParts.BP_CHECKBOX, state, mrectBox);
                }
                else if (this.Style == CheckBoxAdvStyle.Office2007)
                {
                    this.DrawOffice2007Style(g);
                }
                else if (this.Style == CheckBoxAdvStyle.Office2010)
                {
                    this.DrawOffice2010Style(g);
                }
                else if (this.Style == CheckBoxAdvStyle.Metro)
                {
                    this.DrawMetroStyle(g);
                }
                else
                {
                    ButtonState btnState = ButtonState.Normal;
                    if (!(CheckState == CheckState.Unchecked))
                    {
                        btnState |= ButtonState.Checked;
                    }

                    if (!Enabled)
                    {
                        btnState |= ButtonState.Inactive;
                    }

                    if (mmouseDown)
                    {
                        btnState |= ButtonState.Pushed;
                    }

                    if (CheckState == CheckState.Indeterminate)
                    {
                        btnState = ButtonState.All;
                        ControlPaint.DrawMixedCheckBox(g, mrectBox, btnState);
                    }
                    else
                    {
                        ControlPaint.DrawCheckBox(g, mrectBox, btnState);
                    }
                }
            }
        }

        /// <summary>
        /// Drawing office 2007 style.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        private void DrawOffice2007Style(Graphics g)
        {
            if (!this.Enabled)
            {
                this.DrawDisabledBackground(g);
                if (this.CheckState == CheckState.Checked)
                {
                    this.DrawTick(g, this.Office2007ColorTable.CheckBoxAdvDisabledTickColor);
                }

                this.DrawBorders(g, this.Office2007ColorTable.CheckBoxAdvDisabledBorderColor, this.Office2007ColorTable.CheckBoxAdvDisabledBackColor);
            }
            else
            {
                if (mmouseDown)
                {
                    this.DrawPushedBackground(g);
                    if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, this.Office2007ColorTable.CheckBoxAdvPushedTickColor);
                    }

                    this.DrawBorders(g, this.Office2007ColorTable.CheckBoxAdvPushedBorderColor, this.Office2007ColorTable.CheckBoxAdvPushedInternalBorderColor);
                }
                else if (mmouseOver)
                {
                    this.DrawSelectedBackground(g);
                    if (this.CheckState == CheckState.Indeterminate)
                    {
                        this.DrawIndeterminateRectangle(g);
                    }
                    else if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, this.Office2007ColorTable.CheckBoxAdvSelectedTickColor);
                    }

                    this.DrawBorders(g, this.Office2007ColorTable.CheckBoxAdvSelectedBorderColor, this.Office2007ColorTable.CheckBoxAdvSelectedInternalBorderColor);
                }
                else
                {
                    this.DrawNormalBackground(g);
                    if (this.CheckState == CheckState.Indeterminate)
                    {
                        this.DrawIndeterminateRectangle(g);
                    }
                    else if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, this.Office2007ColorTable.CheckBoxAdvNormalTickColor);
                    }

                    this.DrawBorders(g, this.Office2007ColorTable.CheckBoxAdvNormalBorderColor, this.Office2007ColorTable.CheckBoxAdvNormalInternalBorderColor);
                }
            }
        }
        /// <summary>
        /// Drawing office 2010 style.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        private void DrawOffice2010Style(Graphics g)
        {
            if (!this.Enabled)
            {
                this.DrawDisabledBackground(g);
                if (this.CheckState == CheckState.Checked)
                {
                    this.DrawTick(g, this.Office2010ColorTable.CheckBoxAdvDisabledTickColor);
                }

                this.DrawBorders(g, this.Office2010ColorTable.CheckBoxAdvDisabledBorderColor, this.Office2007ColorTable.CheckBoxAdvDisabledBackColor);
            }
            else
            {
                if (mmouseDown)
                {
                    this.DrawPushedBackground(g);
                    if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, this.Office2010ColorTable.CheckBoxAdvPushedTickColor);
                    }

                    this.DrawBorders(g, this.Office2010ColorTable.CheckBoxAdvPushedBorderColor, this.Office2007ColorTable.CheckBoxAdvPushedInternalBorderColor);
                }
                else if (mmouseOver)
                {
                    this.DrawSelectedBackground(g);
                    if (this.CheckState == CheckState.Indeterminate)
                    {
                        this.DrawIndeterminateRectangle(g);
                    }
                    else if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, this.Office2010ColorTable.CheckBoxAdvSelectedTickColor);
                    }

                    this.DrawBorders(g, this.Office2010ColorTable.CheckBoxAdvSelectedBorderColor, this.Office2007ColorTable.CheckBoxAdvSelectedInternalBorderColor);
                }
                else
                {
                    this.DrawNormalBackground(g);
                    if (this.CheckState == CheckState.Indeterminate)
                    {
                        this.DrawIndeterminateRectangle(g);
                    }
                    else if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, this.Office2010ColorTable.CheckBoxAdvNormalTickColor);
                    }

                    this.DrawBorders(g, this.Office2010ColorTable.CheckBoxAdvNormalBorderColor, this.Office2007ColorTable.CheckBoxAdvNormalInternalBorderColor);
                }
            }
        }
        /// <summary>
        /// Drawing Metro style.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        private void DrawMetroStyle(Graphics g)
        {
            if (!this.Enabled)
            {
                this.DrawMetroDisabledBackground(g);
                if (this.CheckState == CheckState.Checked)
                {
                    this.DrawTick(g, ColorTranslator.FromHtml("#BCBCBC"));
                }

                this.DrawBorder(g, ColorTranslator.FromHtml("#BCBCBC"), ColorTranslator.FromHtml("#BCBCBC"));
            }
            else
            {
                if (mmouseDown)
                {
                    this.DrawMetroPushedBackground(g);
                    if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, metroColor);
                    }

                    this.DrawBorder(g, metroColor, metroColor);
                }
                else if (mmouseOver)
                {
                    this.DrawMetroPushedBackground(g);
                    if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, ControlPaint.Light(metroColor));
                    }
                    else if (this.CheckState == CheckState.Indeterminate)
                    {
                        this.DrawMetroIndeterminateRectangle(g);
                    }
                    this.DrawBorder(g, ControlPaint.Light(metroColor), ControlPaint.Light(metroColor));
                }
                else
                {
                    this.DrawMetroSelectedBackground(g);
                    if (this.CheckState == CheckState.Indeterminate)
                    {
                        this.DrawMetroIndeterminateRectangle(g);
                    }
                    else if (this.CheckState == CheckState.Checked)
                    {
                        this.DrawTick(g, metroColor);
                    }

                    this.DrawBorder(g, metroColor, metroColor);
                }
            }
        }
        /// <summary>
        /// Drawing borders.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="color1">Color 1 for border</param>
        /// <param name="color2">Color 2 border</param>
        private void DrawBorders(Graphics g, Color color1, Color color2)
        {
            Rectangle rect = new Rectangle(mrectBox.X, mrectBox.Y, mrectBox.Width - 1, mrectBox.Height - 1);

            using (Pen pen = new Pen(color1))
            {
                g.DrawRectangle(pen, rect);
            }

            rect.Inflate(-1, -1);

            using (Pen pen = new Pen(color2))
            {
                g.DrawRectangle(pen, rect);
            }
        }
        private void DrawBorder(Graphics g, Color color1, Color color2)
        {
            Rectangle rect = new Rectangle(mrectBox.X, mrectBox.Y, mrectBox.Width - 1, mrectBox.Height - 1);

            using (Pen pen = new Pen(color1))
            {
                g.DrawRectangle(pen, rect);
            }
        }
        /// <summary>
        /// Drawing Normal background.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawNormalBackground(Graphics g)
        {
            using (Brush brush = new SolidBrush(this.Office2007ColorTable.CheckBoxAdvNormalBackColor))
            {
                g.FillRectangle(brush, mrectBox);
            }

            Rectangle rect = new Rectangle(mrectBox.X, mrectBox.Y, mrectBox.Width - 1, mrectBox.Height - 1);
            rect.Inflate(-1, -1);

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.CheckBoxAdvNormalInternalRectangleBorderColor, Color.FromArgb(20, this.Office2007ColorTable.CheckBoxAdvNormalInternalRectangleBorderColor), InternalRectangleBorderAngle))
            {
                using (Pen pen = new Pen(brush))
                {
                    rect.Inflate(-1, -1);
                    g.DrawRectangle(pen, rect);
                }
            }

            rect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 1, rect.Height - 1);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.CheckBoxAdvNormalInternalRectangleColor, SystemColors.Window, InternalRectangleAngle))
            {
                brush.Blend = this.internalNormalRectBlend;
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Drawing selected background
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawSelectedBackground(Graphics g)
        {
            using (Brush brush = new SolidBrush(this.Office2007ColorTable.CheckBoxAdvSelectedBackColor))
            {
                g.FillRectangle(brush, mrectBox);
            }

            Rectangle rect = new Rectangle(mrectBox.X, mrectBox.Y, mrectBox.Width - 1, mrectBox.Height - 1);
            rect.Inflate(-1, -1);

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.CheckBoxAdvSelectedInternalRectangleBorderColor, Color.FromArgb(50, this.Office2007ColorTable.CheckBoxAdvSelectedInternalRectangleBorderColor), InternalRectangleBorderAngle))
            {
                using (Pen pen = new Pen(brush))
                {
                    rect.Inflate(-1, -1);
                    g.DrawRectangle(pen, rect);
                }
            }

            rect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 1, rect.Height - 1);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.CheckBoxAdvSelectedInternalRectangleColor, SystemColors.Window, InternalRectangleAngle))
            {
                brush.Blend = this.internalSelectedRectBlend;
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Drawing selected background
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroSelectedBackground(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, mrectBox);
            }
        }
        /// <summary>
        /// Drawing pushed button background
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawPushedBackground(Graphics g)
        {
            using (Brush brush = new SolidBrush(this.Office2007ColorTable.CheckBoxAdvPushedBackColor))
            {
                g.FillRectangle(brush, mrectBox);
            }

            Rectangle rect = new Rectangle(mrectBox.X, mrectBox.Y, mrectBox.Width - 1, mrectBox.Height - 1);
            rect.Inflate(-1, -1);

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.CheckBoxAdvPushedInternalRectangleBorderColor, Color.FromArgb(50, this.Office2007ColorTable.CheckBoxAdvPushedInternalRectangleBorderColor), InternalRectangleBorderAngle))
            {
                using (Pen pen = new Pen(brush))
                {
                    rect.Inflate(-1, -1);
                    g.DrawRectangle(pen, rect);
                }
            }

            rect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 1, rect.Height - 1);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.CheckBoxAdvPushedInternalRectangleColor, SystemColors.Window, InternalRectangleAngle))
            {
                brush.Blend = this.internalSelectedRectBlend;
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Drawing pushed button background
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroPushedBackground(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, mrectBox);
            }           
        }
        /// <summary>
        /// Drawing Disabled Background.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawDisabledBackground(Graphics g)
        {
            Rectangle rect = mrectBox;
            using (Brush brush = new SolidBrush(this.Office2007ColorTable.CheckBoxAdvDisabledBackColor))
            {
                g.FillRectangle(brush, rect);
            }

            rect.Inflate(-2, -2);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.CheckBoxAdvDisabledInternalBorderColor, this.Office2007ColorTable.CheckBoxAdvDisabledBackColor, InternalRectangleAngle))
            {
                using (Pen pen = new Pen(brush))
                {
                    rect = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        /// <summary>
        /// Drawing Disabled Metro Background.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroDisabledBackground(Graphics g)
        {
            Rectangle rect = mrectBox;

            rect.Inflate(-2, -2);           

            using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#E6E6E6")))
            {
                    rect = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                    g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Drawing Tick for checkbox
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="tickColor">Tick color for drawinf tick</param>
        private void DrawTick(Graphics g, Color tickColor)
        {

            this.tickPositions = new Point[] { new Point(mrectBox.X + 3, mrectBox.Y + ((mrectBox.Height / 2))), new Point(mrectBox.X + 5, mrectBox.Bottom - 4), new Point(mrectBox.Right - 4, mrectBox.Y + 2) };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen penWhite = new Pen(SystemColors.Window, 4))
            {
                using (Pen penBlue = new Pen(tickColor, 2))
                {
                    if (this.Style != CheckBoxAdvStyle.Metro)
                    {
                        g.DrawLines(penWhite, this.tickPositions);
                    }
                    g.DrawLines(penBlue, this.tickPositions);
                }
            }

            g.SmoothingMode = SmoothingMode.Default;
        }

        /// <summary>
        /// Drawing the interminate Rectangle.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawIndeterminateRectangle(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-4, -4);
            using (Brush brush = new SolidBrush(this.Office2007ColorTable.CheckBoxAdvIndeterminateRectangleColor))
            {
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Drawing the interminate Rectangle.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroIndeterminateRectangle(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-4, -4);
            if (mmouseOver)
            {
                using (Brush brush = new SolidBrush(ControlPaint.Light(this.MetroColor)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else
            {
                using (Brush brush = new SolidBrush(this.MetroColor))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }
        /// <summary>
        /// Determines  the state of checkbox.
        /// </summary>
        /// <returns>Returns integer value for the state of check box</returns>
        private int DetermineState()
        {
            int state = 1;

            if (!Enabled)
            {
                switch (this.mcheckState)
                {
                    case CheckState.Unchecked: state = ThemeStates.CBS_UNCHECKEDDISABLED;
                        break;
                    case CheckState.Checked: state = ThemeStates.CBS_CHECKEDDISABLED; 
                        break;
                    case CheckState.Indeterminate: state = ThemeStates.CBS_MIXEDDISABLED; 
                        break;
                }
            }
            else if (mmouseOver && mmouseDown)
            {
                // Mouse pressed and over checkbox
                switch (this.mcheckState)
                {
                    case CheckState.Unchecked: state = ThemeStates.CBS_UNCHECKEDPRESSED; 
                        break;
                    case CheckState.Checked: state = ThemeStates.CBS_CHECKEDPRESSED; 
                        break;
                    case CheckState.Indeterminate: state = ThemeStates.CBS_MIXEDPRESSED; 
                        break;
                }
            }
            else if (mmouseDown)
            {
                // Mouse pressed inside checkbox but moved outside
                switch (this.mcheckState)
                {
                    case CheckState.Unchecked: state = ThemeStates.CBS_UNCHECKEDNORMAL; 
                        break;
                    case CheckState.Checked: state = ThemeStates.CBS_CHECKEDNORMAL; 
                        break;
                    case CheckState.Indeterminate: state = ThemeStates.CBS_MIXEDNORMAL; 
                        break;
                }
            }
            else if (mmouseOver)
            {
                // Mouse not pressed but over checkbox
                switch (this.mcheckState)
                {
                    case CheckState.Unchecked: state = ThemeStates.CBS_UNCHECKEDHOT; 
                        break;
                    case CheckState.Checked: state = ThemeStates.CBS_CHECKEDHOT; 
                        break;
                    case CheckState.Indeterminate: state = ThemeStates.CBS_MIXEDHOT; 
                        break;
                }
            }
            else
            {
                // No Mouse pressed inside and no hovering
                switch (this.mcheckState)
                {
                    case CheckState.Unchecked: state = ThemeStates.CBS_UNCHECKEDNORMAL;
                        break;
                    case CheckState.Checked: state = ThemeStates.CBS_CHECKEDNORMAL; 
                        break;
                    case CheckState.Indeterminate: state = ThemeStates.CBS_MIXEDNORMAL; 
                        break;
                }
            }

            return state;
        }

        /// <summary>
        /// On Style changed
        /// </summary>
        private void OnStyleChanged()
        {
            this.Invalidate();
        }

        /// <summary>
        /// Toggle CheckState
        /// </summary>
        /// <param name="source">Source tyle of Checked changed event </param>
        private void ToggleCheckState(CheckedChangedEventArgs.SourceType source)
        {
            if (!this.breadOnly && Enabled)
            {
                switch (this.mcheckState)
                {
                    case CheckState.Checked:
                        {
                            if (this.Tristate)
                            {
                                this.mcheckState = CheckState.Indeterminate;
                            }
                            else
                            {
                                this.mcheckState = CheckState.Unchecked;
                            }
                        }

                        break;
                    case CheckState.Unchecked:
                        this.mcheckState = CheckState.Checked;
                        break;
                    case CheckState.Indeterminate:
                        this.mcheckState = CheckState.Unchecked;
                        break;
                }

                Invalidate(true);

                this.OnCheckedChanged(new CheckedChangedEventArgs(source));
            }
        }

        #endregion Helper Methods
    }

    /// <summary>
    /// CheckBoxAdv Designer
    /// </summary>
    public class CheckBoxAdvDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
       private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public CheckBoxAdvDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new CheckBoxAdvActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
        /// <summary>
        /// Overridden Initialize method.
        /// </summary>
        /// <param name="component">Componnent object</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }
    }

    /// <summary>
    /// CheckedChanged EventArgs
    /// </summary>
    public class CheckedChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Source type
        /// </summary>
        private SourceType source;

        /// <summary>
        ///  Initializes a new instance of the CheckedChangedEventArgs class
        /// </summary>
        /// <param name="sourcetype"> Source type</param>
        public CheckedChangedEventArgs(SourceType sourcetype)
        {
            this.source = sourcetype;
        }

        /// <summary>
        /// Specifies possible sources of the Checked state changing. 
        /// </summary>
        public enum SourceType
        {
            /// <summary>
            /// Specifies that Checked state was changed by mouse. 
            /// </summary>
            Mouse,

            /// <summary>
            /// Specifies that Checked state was changed by keyboard. 
            /// </summary>
            Keyboard,

            /// <summary>
            /// Specifies that Checked state was changed from code. 
            /// </summary>
            Programmatic
        }

        /// <summary>
        /// Gets the source of the Checked state changing.
        /// </summary>
        [Description("Gets the source of the Checked state changing.")]
        public SourceType Source
        {
            get
            {
                return this.source;
            }
        }
    }

    /// <summary>
    /// Checked ChangedEvent Handler
    /// </summary>
    /// <param name="sender"> object sender</param>
    /// <param name="e"> EventArgs that contains the event data.</param>
    public delegate void CheckedChangedEventHandler(object sender, CheckedChangedEventArgs e);
}