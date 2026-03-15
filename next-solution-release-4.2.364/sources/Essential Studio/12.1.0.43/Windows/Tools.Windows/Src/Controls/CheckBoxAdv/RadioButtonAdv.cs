#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws.
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

    /// <summary>
    /// Defines RadiobuttonAdv Styles
    /// </summary>
    public enum RadioButtonAdvStyle
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
        /// Office 2007-like appearance.
        /// </summary>
        Office2010,
        /// <summary>
        /// Metro-like appearance.
        /// </summary>
        Metro
    }

    /// <summary>
    /// The RadioButtonAdv control is an alternative to the .Net RadioButton control. It has a very 
    /// customizable border and background and supports advanced DataBinding.
    /// </summary>
    [Designer(typeof(Syncfusion.Windows.Forms.Tools.RadioButtonAdvDesigner),
       typeof(System.ComponentModel.Design.IDesigner)),
    ToolboxBitmap(typeof(RadioButtonAdv), "ToolboxIcons.RadioButtonAdv.bmp"),
    DefaultEvent("CheckChanged"),
    DefaultProperty(@"BoolValue"),
    ToolboxItem(true),
    Description("Advanced Radio Button with customizable border, background and supports advanced DataBinding")]
    public class RadioButtonAdv : CheckRadioBase, ISupportInitialize,IVisualStyle 
    {
        #region Fields

        /// <summary>
        /// Indicates whether RadioBoxAdv is checked.
        /// </summary>
        private bool mbchecked = false;

        /// <summary>
        /// Determines whether OnClick event should be fired.
        /// </summary>
        private bool braiseEventOnClick = true;

        /// <summary>
        /// Specifies an advanced appearance this control.
        /// </summary>
        private RadioButtonAdvStyle mstyle = RadioButtonAdvStyle.Default;

        /// <summary>
        /// Specifies office 2007 color scheme.
        /// </summary>
        private Office2007Theme mcolorScheme = Office2007Theme.Blue;

        /// <summary>
        /// Color table for Office2007 visual style.
        /// </summary>
        private Office2007Colors moffice2007ColorTable = null;

        /// <summary>
        /// Specifies office 2010 color scheme.
        /// </summary>
        private Office2010Theme mcolor2010Scheme = Office2010Theme.Blue;

        /// <summary>
        /// Color table for Office2007 visual style.
        /// </summary>
        private Office2010Colors moffice2010ColorTable = null;
        /// <summary>
        /// Specifies Metro color.
        /// </summary>
        private Color metroColor = Color.FromArgb(88, 89, 91);

        /// <summary>
        /// Blend used for drawing normal background.
        /// </summary>
        private Blend mnormalInternalRectBlend = null;

        /// <summary>
        /// Blend used for drawing selected background.
        /// </summary>
        private Blend mselectedInternalRectBlend = null;

        /// <summary>
        /// Blend used for drawing internal border.
        /// </summary>
        private Blend mnormalInternalRectBorderBlend = null;
        #endregion Fields

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the RadioButtonAdv class.
        /// </summary>
        public RadioButtonAdv()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(RadioButtonAdv));
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
        /// Occurs when the Checked property changes.
        /// </summary>
        [Description("Occurs when the Checked property changes.")]
        public event EventHandler CheckChanged;

        /// <summary>
        /// Occurs when the Checked property of <see cref="RadioButtonAdv"/> in group changes.
        /// </summary>
        [Description("Occurs when the Checked property of RadioButtonAdv in group changes.")]
        public event EventHandler GroupCheckChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether check state of the RadioButton.
        /// </summary>
        [Description("Indicates whether RadioButtonAdv is checked.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool Checked
        {
            get
            {
                return this.mbchecked;
            }

            set
            {
                bool bprevChecked = this.mbchecked;

                this.mbchecked = value;

                // Enumerate all radiobuttons and set their Checked property to false
                if (this.mbchecked)
                {
                    Control ctrlParent = this.Parent;
                    if (ctrlParent != null)
                    {
                        this.TabStop = true;
                        foreach (Control ctrl in ctrlParent.Controls)
                        {
                            if ((ctrl is RadioButtonAdv) && (ctrl != this))
                            {
                                ctrl.TabStop = false;
                                ((RadioButtonAdv)ctrl).Checked = false;
                            }
                        }
                    }
                }

                Invalidate();

                if (!mbinitializing)
                {
                    if (bprevChecked != this.mbchecked)
                    {
                        this.OnCheckChanged(EventArgs.Empty);
                    }

                    this.OnGroupCheckChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether OnClick event should be fired.
        /// </summary>
        [Description("Specifies whether the OnClick event should be fired")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool RaiseEventOnClick
        {
            get
            {
                return this.braiseEventOnClick;
            }

            set
            {
                this.braiseEventOnClick = value;
            }
        }

        /// <summary>
        /// Gets or sets an advanced appearance for the radioButtonAdv.
        /// </summary>
        [Description("Gets or sets an advanced appearance for the radioButtonAdv.")]
        [Category("Appearance")]
        [DefaultValue(RadioButtonAdvStyle.Default)]
        public RadioButtonAdvStyle Style
        {
            get
            {
                return this.mstyle;
            }

            set
            {
                if (this.mstyle != value)
                {
                    this.mstyle = value;
                    if (Style == RadioButtonAdvStyle.Metro)
                        this.DrawFocusRectangle = false;
                    this.OnStyleChanged();
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
                    Style = RadioButtonAdvStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Style = RadioButtonAdvStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Style = RadioButtonAdvStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    Office2010ColorScheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    Office2010ColorScheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    Office2010ColorScheme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    if (this.Style == RadioButtonAdvStyle.Office2010)
                        Office2010ColorScheme = Office2010Theme.Managed;
                    else
                        Office2007ColorScheme = Office2007Theme.Managed;                    
                }
                else if (value == "Metro")
                    Style = RadioButtonAdvStyle.Metro;
                else if (value == "Default")
                    Style = RadioButtonAdvStyle.Default;

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
                return this.mcolorScheme;
            }

            set
            {
                if (this.mcolorScheme != value)
                {
                    this.mcolorScheme = value;
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
                return this.mcolor2010Scheme;
            }

            set
            {
                if (this.mcolor2010Scheme != value)
                {
                    this.mcolor2010Scheme = value;
                    this.OnStyleChanged();
                }
            }
        }
        private bool useGDITextRender = false;
        /// <summary>
        ///Gets or sets a value indicating whether GDI Text renderer in RadioButton or not.
        /// </summary>
        [Description("Gets or Sets the use GDI Text renderer in RadioButton.")]
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
        /// Gets or sets the theme color of the RadioButtonAdv
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
                this.Invalidate();
            }
        }
        #region DataBinding
        /// <summary>
        /// Gets or sets checked RadioButtonAdv in current container according to TabIndex.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or Sets checked RadioButtonAdv in current container according to TabIndex.")]
        [Category("DataBinding")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IntValue
        {
            get
            {
                int nvalueToReturn = 0;

                if (!mbinitializing)
                {
                    Control ctrlParent = this.Parent;
                    if (ctrlParent != null)
                    {
                        ArrayList lstTabInexes = new ArrayList();
                        this.FillAndSort(ctrlParent, lstTabInexes);
                        nvalueToReturn = this.GetCheck(lstTabInexes);
                    }
                }

                return nvalueToReturn;
            }

            set
            {
                Control ctrlParent = this.Parent;
                if (ctrlParent != null)
                {
                    ArrayList lstTabInexes = new ArrayList();
                    this.FillAndSort(ctrlParent, lstTabInexes);
                    this.SetCheck(lstTabInexes, value);
                }
            }
        }

        #endregion DataBinding

        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        internal Office2007Colors Office2007ColorTable
        {
            get
            {
                Office2007Colors colorTable = (this.moffice2007ColorTable == null) ?
                    Office2007Colors.Default : this.moffice2007ColorTable;

                return colorTable;
            }
        }
        /// <summary>
        /// Gets color table for Office2010 visual style.
        /// </summary>
        internal Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = (this.moffice2010ColorTable == null) ?
                    Office2010Colors.Default : this.moffice2010ColorTable;

                return colorTable;
            }
        }
        #endregion Properties

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
        /// Raises the check changed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnCheckChanged(EventArgs e)
        {
            if (this.CheckChanged != null)
            {
                this.CheckChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the group check changed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnGroupCheckChanged(EventArgs e)
        {
            if (this.GroupCheckChanged != null)
            {
                this.GroupCheckChanged(this, e);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.None)
            {
                this.Checked = true;
            }
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Raises the paint event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!mbinitializing)
            {
                base.OnPaint(e);
                this.DrawRadioBox(e.Graphics);
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Click"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            if (this.Enabled)
            {
                if (!this.Checked)
                {
                    this.Checked = !this.Checked;
                }
            }
             
            if (this.braiseEventOnClick)
            {
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Processes a mnemonic character.
        /// </summary>
        /// <param name="charCode">The character to process.</param>
        /// <returns>
        /// true if the character was processed as a mnemonic by the control; otherwise, false.
        /// </returns>
        protected override bool ProcessMnemonic(char charCode)
        {
            bool bsuccess = false;

            if (CanSelect && IsMnemonic(charCode, this.Text))
            {
                if (!this.Checked || !this.Focused)
                {
                    this.Checked = true;
                    this.Focus();
                    Invalidate(true);
                    bsuccess = true;
                }
            }

            return bsuccess;
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.ParentChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            this.Checked = this.Checked;
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.HandleCreated"></see> event.
        /// </summary>
        /// <param name="e"> Event Argument</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this.IsHandleCreated)
            {
                RecalculatePositions();
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.OnKeyDown"></see> event.
        /// </summary>
        /// <param name="e">Key Event argument</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                mmouseDown = true;
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.OnKeyUp"></see> event.
        /// </summary>
        /// <param name="e">Key Event argument</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                if (mmouseDown && !this.Checked)
                {
                    this.Checked = true;
                }

                mmouseDown = false;
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.OnLeave"></see> event.
        /// </summary>
        /// <param name="e">Event argument</param>
        protected override void OnLeave(EventArgs e)
        {
            mmouseDown = false;
            base.OnLeave(e);
        }

        #endregion Overrides

        #region Helper Methods

        /// <summary>
        /// Init method
        /// </summary>
        private void Init()
        {
            this.mnormalInternalRectBlend = new Blend();
            this.mnormalInternalRectBlend.Positions = new float[] { 0f, 0.3f, 0.3f, 1f };
            this.mnormalInternalRectBlend.Factors = new float[] { 0f, 0.4f, 0.45f, 1f };
            this.mselectedInternalRectBlend = new Blend();
            this.mselectedInternalRectBlend.Positions = new float[] { 0f, 0.8f, 0.8f, 1f };
            this.mselectedInternalRectBlend.Factors = new float[] { 0f, 0.65f, 0.9f, 1f };
            this.mnormalInternalRectBorderBlend = new Blend();
            this.mnormalInternalRectBorderBlend.Positions = new float[] { 0f, 0.62f, 0.63f, 1f };
            this.mnormalInternalRectBorderBlend.Factors = new float[] { 0f, 0.3f, 0.65f, 1f };
        }

        /// <summary>
        /// Performs checkbox drawing routine.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        private void DrawRadioBox(Graphics g)
        {
            if (mimageCheckBox)
            {
                Image image = null;
                if (mmouseOver && this.Checked)
                {
                    image = mmouseOverCheckedImage;
                    if (image == null)
                    {
                        image = mcheckedImage;
                    }
                }

                if (mmouseOver && !this.Checked)
                {
                    image = mmouseOverUncheckedImage;
                    if (image == null)
                    {
                        image = muncheckedImage;
                    }
                }

                if (!mmouseOver && this.Checked)
                {
                    image = mcheckedImage;
                }

                if (!mmouseOver && !this.Checked)
                {
                    image = muncheckedImage;
                }

                if (!Enabled)
                {
                    image = this.DisabledImage;
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
                    tcd.DrawThemeBackground(g, ThemeParts.BP_RADIOBUTTON, state, mrectBox);
                }
                else if (this.Style == RadioButtonAdvStyle.Office2007)
                {
                    SmoothingMode previousMode = g.SmoothingMode;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    this.DrawOffice2007Style(g);

                    g.SmoothingMode = previousMode;
                }
                else if (this.Style == RadioButtonAdvStyle.Office2010)
                {
                    SmoothingMode previousMode = g.SmoothingMode;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    this.DrawOffice2010Style(g);

                    g.SmoothingMode = previousMode;
                }
                else if (this.Style == RadioButtonAdvStyle.Metro)
                {
                    SmoothingMode previousMode = g.SmoothingMode;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    this.DrawMetroStyle(g);

                    g.SmoothingMode = previousMode;
                }
                else
                {
                    ButtonState btnState = ButtonState.Normal;
                    if (this.Checked)
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

                    ControlPaint.DrawRadioButton(g, mrectBox, btnState);
                }
            }
        }

        /// <summary>
        /// Draws office 2007 style for radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawOffice2007Style(Graphics g)
        {
            if (!this.Enabled)
            {
                this.FillBackGround(g, this.Office2007ColorTable.RadioButtonAdvNormalBackColor);
                if (this.Checked)
                    this.DrawDisabledCheckMark(g);
                this.DrawDisabledBackGround(g);
                this.DrawBorder(g, Color.FromArgb(150, this.Office2007ColorTable.RadioButtonAdvNormalBorderColor));
            }
            else
            {
                if (mmouseDown)
                {
                    this.FillBackGround(g, this.Office2007ColorTable.RadioButtonAdvPushedBackColor);
                    if (this.Checked)
                    {
                        this.DrawPushedCheckMark(g);
                    }
                    else
                    {
                        this.DrawPushedBackGround(g);
                    }

                    this.DrawBorder(g, this.Office2007ColorTable.RadioButtonAdvPushedBorderColor);
                }
                else if (mmouseOver)
                {
                    this.FillBackGround(g, this.Office2007ColorTable.RadioButtonAdvSelectedBackColor);
                    if (this.Checked)
                    {
                        this.DrawSelectedCheckMark(g);
                    }
                    else
                    {
                        this.DrawSelectedBackGround(g);
                    }

                    this.DrawBorder(g, this.Office2007ColorTable.RadioButtonAdvSelectedBorderColor);
                }
                else
                {
                    this.FillBackGround(g, this.Office2007ColorTable.RadioButtonAdvNormalBackColor);
                    if (this.Checked)
                    {
                        this.DrawNormalCheckMark(g);
                    }
                    else
                    {
                        this.DrawNormalBackGround(g);
                    }

                    this.DrawBorder(g, this.Office2007ColorTable.RadioButtonAdvNormalBorderColor);
                }
            }
        }
        /// <summary>
        /// Draws office 2010 style for radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawOffice2010Style(Graphics g)
        {
            if (!this.Enabled)
            {
                this.FillBackGround(g, this.Office2010ColorTable.RadioButtonAdvNormalBackColor);
                if (this.Checked)
                    this.DrawDisabledCheckMark(g);
                this.DrawDisabledBackGround(g);
                this.DrawBorder(g, Color.FromArgb(150, this.Office2010ColorTable.RadioButtonAdvNormalBorderColor));
            }
            else
            {
                if (mmouseDown)
                {
                    this.FillBackGround(g, this.Office2010ColorTable.RadioButtonAdvPushedBackColor);
                    if (this.Checked)
                    {
                        this.DrawPushedCheckMark(g);
                    }
                    else
                    {
                        this.DrawPushedBackGround(g);
                    }

                    this.DrawBorder(g, this.Office2010ColorTable.RadioButtonAdvPushedBorderColor);
                }
                else if (mmouseOver)
                {
                    this.FillBackGround(g, this.Office2010ColorTable.RadioButtonAdvSelectedBackColor);
                    if (this.Checked)
                    {
                        this.DrawSelectedCheckMark(g);
                    }
                    else
                    {
                        this.DrawSelectedBackGround(g);
                    }

                    this.DrawBorder(g, this.Office2010ColorTable.RadioButtonAdvSelectedBorderColor);
                }
                else
                {
                    this.FillBackGround(g, this.Office2010ColorTable.RadioButtonAdvNormalBackColor);
                    if (this.Checked)
                    {
                        this.DrawNormalCheckMark(g);
                    }
                    else
                    {
                        this.DrawNormalBackGround(g);
                    }

                    this.DrawBorder(g, this.Office2010ColorTable.RadioButtonAdvNormalBorderColor);
                }
            }
        }
        /// <summary>
        /// Draws Metro style for radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroStyle(Graphics g)
        {
            if (!this.Enabled)
            {
                if (this.Checked)
                    this.DrawMetroDisabledCheckMark(g);
                this.DrawMetroDisabledBackGround(g);
                this.DrawBorder(g, ColorTranslator.FromHtml("#BCBCBC"));
            }
            else
            {
                if (mmouseDown)
                {
                    this.FillBackGround(g, Color.White);
                    if (this.Checked)
                    {
                        this.DrawMetroPushedCheckMark(g);
                    }
                    else
                    {
                        this.DrawMetroPushedBackGround(g);
                    }

                    this.DrawBorder(g, metroColor);
                }
                else if (mmouseOver)
                {
                    this.FillBackGround(g, Color.White);
                    if (this.Checked)
                    {
                        this.DrawMetroPushedCheckMark(g);
                    }
                    else
                    {
                        this.DrawMetroPushedBackGround(g);
                    }
                    this.DrawBorder(g, ControlPaint.Light(metroColor));
                }
                else
                {
                    this.FillBackGround(g, Color.White);
                    if (this.Checked)
                    {
                        this.DrawMetroSelectedCheckMark(g);
                    }
                    else
                    {
                        this.DrawMetroSelectedBackGround(g);
                    }

                    this.DrawBorder(g, metroColor);
                }

            }
        }
        /// <summary>
        /// Fills the background of the radio button with specified color.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="backColor">Back color</param>
        private void FillBackGround(Graphics g, Color backColor)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-1, -1);
            using (Brush brush = new SolidBrush(backColor))
            {
                g.FillEllipse(brush, rect);
            }
        }

        /// <summary>
        /// Draws disabled background of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawDisabledBackGround(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);
            if (this.Style == RadioButtonAdvStyle.Office2010)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(175, this.Office2010ColorTable.RadioButtonAdvNormalInternalBorderColor), Color.FromArgb(55, this.Office2007ColorTable.RadioButtonAdvNormalInternalBorderColor), 60f))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(175, this.Office2007ColorTable.RadioButtonAdvNormalInternalBorderColor), Color.FromArgb(55, this.Office2007ColorTable.RadioButtonAdvNormalInternalBorderColor), 60f))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
        }

        /// <summary>
        /// Draws disabled background of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroDisabledBackGround(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);

            using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#E6E6E6")))
            {
                using (Pen pen = new Pen(brush))
                {
                    g.DrawEllipse(pen, rect);
                }
            }
        }
        /// <summary>
        /// Draws normal background of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawNormalBackGround(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);
            if (this.Style == RadioButtonAdvStyle.Office2010)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(225, this.Office2010ColorTable.RadioButtonAdvNormalInternalBorderColor), SystemColors.Window, 50f))
                {
                    brush.Blend = this.mnormalInternalRectBlend;
                    g.FillEllipse(brush, rect);
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2010ColorTable.RadioButtonAdvNormalInternalBorderColor, this.Office2010ColorTable.RadioButtonAdvNormalBackColor, 60f))
                {
                    brush.Blend = this.mnormalInternalRectBorderBlend;
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(225, this.Office2007ColorTable.RadioButtonAdvNormalInternalBorderColor), SystemColors.Window, 50f))
                {
                    brush.Blend = this.mnormalInternalRectBlend;
                    g.FillEllipse(brush, rect);
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.RadioButtonAdvNormalInternalBorderColor, this.Office2007ColorTable.RadioButtonAdvNormalBackColor, 60f))
                {
                    brush.Blend = this.mnormalInternalRectBorderBlend;
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
        }

        /// <summary>
        /// Draws selected background of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawSelectedBackGround(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);
            if (this.Style == RadioButtonAdvStyle.Office2010)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(150, this.Office2010ColorTable.RadioButtonAdvSelectedInternalBorderColor), SystemColors.Window, 50f))
                {
                    brush.Blend = this.mselectedInternalRectBlend;
                    g.FillEllipse(brush, rect);
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2010ColorTable.RadioButtonAdvSelectedInternalBorderColor, Color.FromArgb(225, this.Office2010ColorTable.RadioButtonAdvSelectedBackColor), 60f))
                {
                    brush.Blend = this.mnormalInternalRectBorderBlend;
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(150, this.Office2007ColorTable.RadioButtonAdvSelectedInternalBorderColor), SystemColors.Window, 50f))
                {
                    brush.Blend = this.mselectedInternalRectBlend;
                    g.FillEllipse(brush, rect);
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.RadioButtonAdvSelectedInternalBorderColor, Color.FromArgb(225, this.Office2007ColorTable.RadioButtonAdvSelectedBackColor), 60f))
                {
                    brush.Blend = this.mnormalInternalRectBorderBlend;
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
        }

        /// <summary>
        /// Draws selected background of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroSelectedBackGround(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);

            using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#FFF3F3F3")))
            {
                using (Pen pen = new Pen(brush))
                {
                    g.DrawEllipse(pen, rect);
                }
            }
        }
        /// <summary>
        /// Draws pushed background of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawPushedBackGround(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);
            if (this.Style == RadioButtonAdvStyle.Office2010)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(150, this.Office2010ColorTable.RadioButtonAdvPushedInternalBorderColor), SystemColors.Window, 50f))
                {
                    brush.Blend = this.mselectedInternalRectBlend;
                    g.FillEllipse(brush, rect);
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2010ColorTable.RadioButtonAdvPushedInternalBorderColor, Color.FromArgb(225, this.Office2010ColorTable.RadioButtonAdvPushedBackColor), 60f))
                {
                    brush.Blend = this.mnormalInternalRectBorderBlend;
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(150, this.Office2007ColorTable.RadioButtonAdvPushedInternalBorderColor), SystemColors.Window, 50f))
                {
                    brush.Blend = this.mselectedInternalRectBlend;
                    g.FillEllipse(brush, rect);
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, this.Office2007ColorTable.RadioButtonAdvPushedInternalBorderColor, Color.FromArgb(225, this.Office2007ColorTable.RadioButtonAdvPushedBackColor), 60f))
                {
                    brush.Blend = this.mnormalInternalRectBorderBlend;
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
        }

        /// <summary>
        /// Draws pushed background of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroPushedBackGround(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);            

            using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#FFF3F3F3")))
            {
                using (Pen pen = new Pen(brush))
                {
                    g.DrawEllipse(pen, rect);
                }
            }
        }
        /// <summary>
        /// Draws normal check mark of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawNormalCheckMark(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);
            if (this.Style == RadioButtonAdvStyle.Office2010)
            {
                using (Pen pen = new Pen(this.Office2010ColorTable.RadioButtonAdvNormalInternalBorderColor))
                {
                    g.DrawEllipse(pen, rect);
                }

                rect.Inflate(-1, -1);

                using (GraphicsPath path = this.GetCheckMarkPath(rect))
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = this.Office2010ColorTable.RadioButtonAdvSelectedBackColor;
                        brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y);
                        brush.SurroundColors = new Color[] { this.Office2010ColorTable.RadioButtonAdvCheckMarkNormalBottomColor };

                        g.FillPath(brush, path);
                    }
                }

                using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                {
                    using (Pen pen = new Pen(this.Office2010ColorTable.RadioButtonAdvCheckMarkBorderColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else
            {
                using (Pen pen = new Pen(this.Office2007ColorTable.RadioButtonAdvNormalInternalBorderColor))
                {
                    g.DrawEllipse(pen, rect);
                }

                rect.Inflate(-1, -1);

                using (GraphicsPath path = this.GetCheckMarkPath(rect))
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = this.Office2007ColorTable.RadioButtonAdvSelectedBackColor;
                        brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y);
                        brush.SurroundColors = new Color[] { this.Office2007ColorTable.RadioButtonAdvCheckMarkNormalBottomColor };

                        g.FillPath(brush, path);
                    }
                }

                using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                {
                    using (Pen pen = new Pen(this.Office2007ColorTable.RadioButtonAdvCheckMarkBorderColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private void DrawDisabledCheckMark(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);
            if (this.Style == RadioButtonAdvStyle.Office2010)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(175, this.Office2010ColorTable.RadioButtonAdvNormalInternalBorderColor), Color.FromArgb(55, this.Office2010ColorTable.RadioButtonAdvNormalInternalBorderColor), 60f))
                {
                    using (GraphicsPath path = this.GetCheckMarkPath(rect))
                    {
                        g.FillPath(brush, path);
                    }

                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);

                        rect.Inflate(-1, -1);

                        using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(175, this.Office2007ColorTable.RadioButtonAdvNormalInternalBorderColor), Color.FromArgb(55, this.Office2007ColorTable.RadioButtonAdvNormalInternalBorderColor), 60f))
                {
                    using (GraphicsPath path = this.GetCheckMarkPath(rect))
                    {
                        g.FillPath(brush, path);
                    }

                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawEllipse(pen, rect);

                        rect.Inflate(-1, -1);

                        using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }
        }

        private void DrawMetroDisabledCheckMark(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);

            using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#BCBCBC")))
            {
                using (GraphicsPath path = this.GetCheckMarkPath(rect))
                {
                    g.FillPath(brush, path);
                }

                using (Pen pen = new Pen(brush))
                {
                    g.DrawEllipse(pen, rect);

                    rect.Inflate(-1, -1);

                    using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
        /// <summary>
        /// Draws normal check mark of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawSelectedCheckMark(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);

            if (this.Style == RadioButtonAdvStyle.Office2010)
            {
                using (Pen pen = new Pen(this.Office2010ColorTable.RadioButtonAdvSelectedInternalBorderColor))
                {
                    g.DrawEllipse(pen, rect);
                }

                rect.Inflate(-1, -1);

                using (GraphicsPath path = this.GetCheckMarkPath(rect))
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = Color.White;
                        brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y + 1);
                        brush.SurroundColors = new Color[] { this.Office2010ColorTable.RadioButtonAdvCheckMarkSelectedBottomColor };

                        g.FillPath(brush, path);
                    }
                }

                using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                {
                    using (Pen pen = new Pen(this.Office2010ColorTable.RadioButtonAdvCheckMarkBorderColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else
            {
                using (Pen pen = new Pen(this.Office2007ColorTable.RadioButtonAdvSelectedInternalBorderColor))
                {
                    g.DrawEllipse(pen, rect);
                }

                rect.Inflate(-1, -1);

                using (GraphicsPath path = this.GetCheckMarkPath(rect))
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = Color.White;
                        brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y + 1);
                        brush.SurroundColors = new Color[] { this.Office2007ColorTable.RadioButtonAdvCheckMarkSelectedBottomColor };

                        g.FillPath(brush, path);
                    }
                }

                using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                {
                    using (Pen pen = new Pen(this.Office2007ColorTable.RadioButtonAdvCheckMarkBorderColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        /// <summary>
        /// Draws normal check mark of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroSelectedCheckMark(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);

            using (Pen pen = new Pen(ColorTranslator.FromHtml("#FFF3F3F3")))
            {
                g.DrawEllipse(pen, rect);
            }

            rect.Inflate(-1, -1);

            using (GraphicsPath path = this.GetCheckMarkPath(rect))
            {
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = metroColor;
                    brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y + 1);
                    brush.SurroundColors = new Color[] { metroColor };
                    SmoothingMode mode = SmoothingMode.AntiAlias;
                    g.SmoothingMode = mode;
                    g.FillPath(brush, path);
                }
            }

            using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
            {
                using (Pen pen = new Pen(metroColor))
                {
                    g.DrawPath(pen, path);
                }
            }
        }
        /// <summary>
        /// Draws normal check mark of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawPushedCheckMark(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);
            if (this.Style == RadioButtonAdvStyle.Office2010)
            {
                using (Pen pen = new Pen(Color.FromArgb(200, this.Office2010ColorTable.RadioButtonAdvPushedInternalBorderColor)))
                {
                    g.DrawEllipse(pen, rect);
                }

                rect.Inflate(-1, -1);

                using (GraphicsPath path = this.GetCheckMarkPath(rect))
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = this.Office2010ColorTable.RadioButtonAdvSelectedBackColor;
                        brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y);
                        brush.SurroundColors = new Color[] { this.Office2010ColorTable.RadioButtonAdvCheckMarkPushedBottomColor };

                        g.FillPath(brush, path);
                    }
                }

                using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                {
                    using (Pen pen = new Pen(this.Office2010ColorTable.RadioButtonAdvCheckMarkBorderColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.FromArgb(200, this.Office2007ColorTable.RadioButtonAdvPushedInternalBorderColor)))
                {
                    g.DrawEllipse(pen, rect);
                }

                rect.Inflate(-1, -1);

                using (GraphicsPath path = this.GetCheckMarkPath(rect))
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = this.Office2007ColorTable.RadioButtonAdvSelectedBackColor;
                        brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y);
                        brush.SurroundColors = new Color[] { this.Office2007ColorTable.RadioButtonAdvCheckMarkPushedBottomColor };

                        g.FillPath(brush, path);
                    }
                }

                using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
                {
                    using (Pen pen = new Pen(this.Office2007ColorTable.RadioButtonAdvCheckMarkBorderColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        /// <summary>
        /// Draws normal check mark of the radio button.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawMetroPushedCheckMark(Graphics g)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-3, -3);

            using (Pen pen = new Pen(metroColor))
            {
                if(!mmouseOver)
                    g.DrawEllipse(pen, rect);
            }

            rect.Inflate(-1, -1);

            using (GraphicsPath path = this.GetCheckMarkPath(rect))
            {
                if (mmouseOver)
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = ControlPaint.Light(metroColor);
                        brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y);
                        brush.SurroundColors = new Color[] { ControlPaint.Light(metroColor) };
                        SmoothingMode mode = SmoothingMode.AntiAlias;
                        g.SmoothingMode = mode;
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = metroColor;
                        brush.CenterPoint = new PointF((float)rect.X + 1, (float)rect.Y);
                        brush.SurroundColors = new Color[] {metroColor };
                        SmoothingMode mode = SmoothingMode.AntiAlias;
                        g.SmoothingMode = mode;
                        g.FillPath(brush, path);
                    }
                }
            }

            using (GraphicsPath path = this.GetCheckMarkBorderPath(rect))
            {
                if (mmouseOver)
                {
                    using (Pen pen = new Pen(ControlPaint.Light(metroColor)))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    using (Pen pen = new Pen(metroColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
        /// <summary>
        /// Draws border of the radio button.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="borderColor">Border color</param>
        private void DrawBorder(Graphics g, Color borderColor)
        {
            Rectangle rect = mrectBox;
            rect.Inflate(-1, -1);

            using (Pen pen = new Pen(borderColor))
            {
                g.DrawEllipse(pen, rect);
            }
        }

        /// <summary>
        /// Gets check mark rectangle of the radio button.
        /// </summary>
        /// <param name="rect"> Represents Rectangle</param>
        /// <returns> Returns Graphics Path</returns>
        private GraphicsPath GetCheckMarkPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(rect);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Gets path for the border of check mark of the radio button.
        /// </summary>
        /// <param name="rect"> Represents Rectangle</param>
        /// <returns>Returns Graphics path</returns>
        private GraphicsPath GetCheckMarkBorderPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            
            #region Fortouch


            path.AddEllipse(rect);
            path.CloseFigure();
            #endregion
            return path;
        }

        /// <summary>
        /// Onstyle Changed method
        /// </summary>
        private void OnStyleChanged()
        {
            if (this.Style == RadioButtonAdvStyle.Office2007)
                this.moffice2007ColorTable = Office2007Colors.GetColorTable(this.mcolorScheme);
            else if (this.Style == RadioButtonAdvStyle.Office2010)
                this.moffice2010ColorTable = Office2010Colors.GetColorTable(this.mcolor2010Scheme);
            this.Invalidate();
        }

        /// <summary>
        /// Presents current RadionButtonAdv state using ThemeStates enum.
        /// </summary>
        /// <returns>Return integer value</returns>
        private int DetermineState()
        {
            int state = 1;

            if (!this.Enabled)
            {
                if (this.mbchecked)
                {
                    state = ThemeStates.RBS_CHECKEDDISABLED;
                }
                else
                {
                    state = ThemeStates.RBS_UNCHECKEDDISABLED;
                }
            }
            else if (mmouseOver && mmouseDown)
            {
                // Mouse pressed and over checkbox
                if (this.mbchecked)
                {
                    state = ThemeStates.RBS_CHECKEDPRESSED;
                }
                else
                {
                    state = ThemeStates.RBS_UNCHECKEDPRESSED;
                }
            }
            else if (mmouseDown)
            {
                // Mouse pressed inside checkbox but moved outside
                if (this.mbchecked)
                {
                    state = ThemeStates.RBS_CHECKEDNORMAL;
                }
                else
                {
                    state = ThemeStates.RBS_UNCHECKEDNORMAL;
                }
            }
            else if (mmouseOver)
            {
                // Mouse not pressed but over checkbox
                if (this.mbchecked)
                {
                    state = ThemeStates.RBS_CHECKEDHOT;
                }
                else
                {
                    state = ThemeStates.RBS_UNCHECKEDHOT;
                }
            }
            else
            {
                // No Mouse pressed inside and no hovering
                if (this.mbchecked)
                {
                    state = ThemeStates.RBS_CHECKEDNORMAL;
                }
                else
                {
                    state = ThemeStates.RBS_UNCHECKEDNORMAL;
                }
            }

            return state;
        }

        /// <summary>
        /// Fills lstTabIndexes arrayList with RBInfo <see cref="RBInfo"/> and then sorts it.
        /// </summary>
        /// <param name="ctrlParent">ContainerControl whose children are enumerated.</param>
        /// <param name="lstTabInexes">ArrayList to fill.</param>
        private void FillAndSort(Control ctrlParent, ArrayList lstTabInexes)
        {
            foreach (Control ctrl in ctrlParent.Controls)
            {
                RadioButtonAdv radioBtn = ctrl as RadioButtonAdv;

                if (null != radioBtn)
                {
                    lstTabInexes.Add(new RBInfo(ctrl.TabIndex, radioBtn));
                }
            }

            lstTabInexes.Sort();
        }

        /// <summary>
        /// Determines RadioButtonAdv control to set checked.
        /// </summary>
        /// <param name="lstTabInexes">First tab index</param>
        /// <param name="value">Integer Value</param>
        private void SetCheck(ArrayList lstTabInexes, int value)
        {
            int nradioButtonCount = 0;

            for (int ncounter = 0; ncounter < lstTabInexes.Count; ncounter++)
            {
                RBInfo rinfo = lstTabInexes[ncounter] as RBInfo;
                RadioButtonAdv radioCurrent = rinfo.Control as RadioButtonAdv;

                if (radioCurrent.Enabled)
                {
                    nradioButtonCount++;
                }

                if (nradioButtonCount == value)
                {
                    radioCurrent.Checked = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Determines checked RadioButtonAdv control.
        /// </summary>
        /// <param name="lstTabInexes">First tab index</param>
        /// <returns>Return integer value</returns>
        private int GetCheck(ArrayList lstTabInexes)
        {
            bool bcheckFound = false;
            int ncheckToReturn = 0;

            for (int ncounter = 0; ncounter < lstTabInexes.Count; ncounter++)
            {
                RBInfo nfoIndex = lstTabInexes[ncounter] as RBInfo;

                RadioButtonAdv radioCurrent = nfoIndex.Control;
                if (radioCurrent.Enabled)
                {
                    ncheckToReturn++;
                }

                if (radioCurrent.Checked)
                {
                    bcheckFound = true;
                    break;
                }
            }

            if (!bcheckFound)
            {
                ncheckToReturn = 0;
            }

            return ncheckToReturn;
        }

        #endregion Helper Methods
    }

    /// <summary>
    /// RadioButtonAdv designer class
    /// </summary>
    public class RadioButtonAdvDesigner : System.Windows.Forms.Design.ControlDesigner
    {       
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Designer Action List collection
        /// </summary>
       private System.ComponentModel.Design.DesignerActionListCollection actionLists;

       /// <summary>
       /// Initializes a new instance of the RadioButtonAdvDesigner class.
       /// </summary>
       public RadioButtonAdvDesigner()
           : base()
       {
       }

        /// <summary>
        /// Gets a ActionList collection
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new RadioButtonAdvActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
        /// <summary>
        /// Initialize method
        /// </summary>
        /// <param name="component">Represents component</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }
    }

    /// <summary>
    /// Class containig Control reference and its TabIndex.
    /// </summary> 
    internal class RBInfo : IComparable
    {
        /// <summary>
        /// Integer value Tab Index
        /// </summary>
        private int ntabIndex = 0;

        /// <summary>
        /// Initailizes a Radio button with Defaut null value
        /// </summary>
        private RadioButtonAdv mradioButton = null;

        #region Initialize / Finalize

        /// <summary>
        /// Initializes a new instance of the RBInfo class.
        /// </summary>
        /// <param name="ntabIndex"> Integer value Tab Index</param>
        /// <param name="ctrl"> RadioButtonAdv control</param>
        public RBInfo(int ntabIndex, RadioButtonAdv ctrl)
        {
            this.ntabIndex = ntabIndex;
            this.mradioButton = ctrl;
        }
        #endregion Initialize / Finalize

        #region Properties

        /// <summary>
        /// Gets a TabIndex value
        /// </summary>
        public int TabIndex
        {
            get 
            { 
                return this.ntabIndex; 
            }
        }

        /// <summary>
        /// Gets a object RadioButton.
        /// </summary>
        public RadioButtonAdv Control
        {
            get
            {
                return this.mradioButton; 
            }
        }
        #endregion Properties

        #region IComparable Members

        /// <summary>
        /// Comparing the Taindex values
        /// </summary>
        /// <param name="x"> Object value for button info</param>
        /// <returns>Returns the integer vvalue for tab index</returns>
        public int CompareTo(object x)
        {
            if (!(x is RBInfo))
            {
                throw new ArgumentException("object is not a RBInfo");
            }

            RBInfo rb = x as RBInfo;
            return this.TabIndex.CompareTo(rb.TabIndex);
        }

        #endregion
    }
}
