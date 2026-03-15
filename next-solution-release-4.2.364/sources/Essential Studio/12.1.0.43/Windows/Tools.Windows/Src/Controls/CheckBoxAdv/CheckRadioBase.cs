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
using System.Drawing.Text;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices;
    using System.Collections.Generic;

    /// <summary>
    ///  CheckRadioBase class.
    /// </summary>
    public abstract class CheckRadioBase : ThemedControl, IMessageFilter
    {
        #region Constants

        /// <summary>
        /// Default CHeckBox Size
        /// </summary>
        private Size cszDefaultCheckboxSize = new Size(13, 13);

        /// <summary>
        /// Checkbox standard offset value
        /// </summary>
        private int cncheckBoxStandOff = 2;

        /// <summary>
        /// Indicates whether to enable or disable the ampersand(&) in the Text.
        /// </summary>
        private bool useMnemonic = true;

        /// <summary>
        /// Indicates whether Alt pressed or not pressed
        /// </summary>
        bool altPressed = false;
        /// <summary>
        /// Default Rectangle size
        /// </summary>
        private static Size RECTBOXSIZE = default(Size);
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        #endregion Constants

        #region Fields

        /// <summary>
        /// Indicates whether it is initializing stage.
        /// </summary>
        protected bool mbinitializing = false;

        /// <summary>
        /// Internal flag which indicates whether text will be rendered.
        /// </summary>
        protected bool mbtextRender = true;

        /// <summary>
        /// Rectangle used to draw checkbox in.
        /// </summary>
        protected Rectangle mrectBox = Rectangle.Empty;

        /// <summary>
        /// Text Location.
        /// </summary>
        protected Rectangle mrectText = Rectangle.Empty;

        /// <summary>
        /// Image checkbox size.
        /// </summary>
        protected Size mszImagedBox = Size.Empty;

        /// <summary>
        /// Alignment for checkbox.
        /// </summary>
        protected ContentAlignment malignCheckBox = ContentAlignment.MiddleLeft;

        /// <summary>
        /// The alignment of the text.
        /// </summary>
        protected ContentAlignment mtextAlignment = ContentAlignment.MiddleLeft;

        /// <summary>
        /// The alignment of the text.
        /// </summary>
        protected ContentAlignment malignText = ContentAlignment.MiddleLeft;

        /// <summary>
        /// Indicates whether the mouse button is pressed.
        /// </summary>
        protected bool mmouseDown = false;

        /// <summary>
        /// Indicates whether the mouse is over the control.
        /// </summary>
        protected bool mmouseOver = false;

        /// <summary>
        /// Indicates whether the focus rectangle will be visible.
        /// </summary>
        protected bool mdrawFocusRectangle = true;

        /// <summary>
        /// Indicates whether the height of the CheckBox will be automatically calculated.
        /// </summary>
        protected bool mautoHeight = false;
        
        /// <summary>
        /// Indicates whether the checkbox will draw itself with the images provided.
        /// </summary>
        protected bool mimageCheckBox = false;

        /// <summary>
        /// Indicates whether the images will be stretched when drawing over the checkbox.
        /// </summary>
        protected bool mstretchImage = true;

        /// <summary>
        /// Indicates whether the text will be wrapped.
        /// </summary>
        protected bool mwrapText = true;

        /// <summary>
        /// Indicates whether the text shadow is visible.
        /// </summary>
        private bool textShadow = false;

        /// <summary>
        /// Determines the state of the checkbox.
        /// </summary>
        protected ButtonState buttonState = ButtonState.Normal;

        /// <summary>
        /// Determines the position of the text.
        /// </summary>
        protected Point textOffset = Point.Empty;

        /// <summary>
        /// Determines the offset of the shadow.
        /// </summary>
        protected Point mshadowOffset = new Point(2, 2);

        /// <summary>
        /// The focus rectangle.
        /// </summary>
        protected Rectangle focusRect = Rectangle.Empty;

        /// <summary>
        /// The image when checked.
        /// </summary>
        protected Image mcheckedImage = null;

        /// <summary>
        /// The image when unchecked.
        /// </summary>
        protected Image muncheckedImage = null;

        /// <summary>
        /// The image when disabled.
        /// </summary>
        protected Image mdisabledImage = null;

        /// <summary>
        /// The image when checked and mouse over.
        /// </summary>
        protected Image mmouseOverCheckedImage = null;

        /// <summary>
        /// The image when unchecked and mouse over.
        /// </summary>
        protected Image mmouseOverUncheckedImage = null;

        /// <summary>
        /// The image when indeterminate.
        /// </summary>
        protected Image mindeterminateImage = null;

        /// <summary>
        /// The image when indeterminate and mouse over.
        /// </summary>
        protected Image mmouseOverIndetermImage = null;

        /// <summary>
        /// The 3D border style of the CheckBox.
        /// </summary>
        protected Border3DStyle mborder3DStyle = Border3DStyle.Sunken;

        /// <summary>
        /// The 2D border style of the CheckBox.
        /// </summary>
        protected BorderStyle mborderStyle = BorderStyle.None;

        /// <summary>
        /// The border style of the CheckBox.
        /// </summary>
        protected ButtonBorderStyle mborderSingle = ButtonBorderStyle.Solid;

        /// <summary>
        /// The background style of the CheckBox.
        /// </summary>
        protected CheckBoxAdvBackStyle backStyle = CheckBoxAdvBackStyle.Default;

        /// <summary>
        /// The color of the border.
        /// </summary>
        protected Color mborderColor = SystemColors.WindowFrame;

        /// <summary>
        /// The color of the border when mouse over.
        /// </summary>
        protected Color mhotBorderColor = SystemColors.WindowFrame;

        /// <summary>
        /// The color of the text shadow.
        /// </summary>
        protected Color mshadowColor = Color.Black;

        /// <summary>
        /// The start color of the gradient.
        /// </summary>
        protected Color mgradientStart = SystemColors.Control;

        /// <summary>
        /// The end color of the gradient.
        /// </summary>
        protected Color mgradientEnd = SystemColors.ControlDark;

        /// <summary>
        /// The string to get/set to the StringValue property when checked.
        /// </summary>
        private string mstrCheckedString = "Checked";

        /// <summary>
        /// The string to get/set to the StringValue property when unchecked.
        /// </summary>
        private string mstrUncheckedString = "Unchecked";

        /// <summary>
        /// The integer to get/set to the IntValue property when checked.
        /// </summary>
        private int mncheckedInt = 1;

        /// <summary>
        /// The integer to get/set to the IntValue property when unchecked.
        /// </summary>
        private int mnuncheckedInt = 0;

        /// <summary>
        /// Helps in the drawing of the control.
        /// </summary>
        ControlDrawing cd = new ControlDrawing();

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        /// <summary>
        /// Helps in the themed drawing of the control.
        /// </summary>
        protected ThemedControlDrawing tcd;

        // For GDI Text rendering
        protected TextFormatFlags m_TextFormatFlags;
        #endregion Fields

        #region Constructor / Destructor

        /// <summary>
        /// Initializes a new instance of the CheckRadioBase class.
        /// </summary>
        public CheckRadioBase()
        {
            // This call is required by the Windows.Forms Form Designer.

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            this.tcd = new ThemedControlDrawing(ThemedControls.BUTTON, this);
            using (Graphics g = this.CreateGraphics())
            {
                if (g.DpiX > 96)
                {
                    cszDefaultCheckboxSize = CheckBoxRenderer.GetGlyphSize(g, System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal);
                }
            }
            this.InitializeComponent();
            // TODO: Add any initialization after the InitForm call
        }

        #endregion Constructor / Destructor
        #region Properties

        #region Appearance Properties

        /// <summary>
        /// Gets or sets a value indicating whether the first character that is preceded
        /// by an ampersand (&) is used as the mnemonic key of the control.        
        [DefaultValue(true)]
        public bool UseMnemonic
        {
            get
            {
                return this.useMnemonic;
            }
            set
            {
                if (this.useMnemonic != value)
                {
                    this.useMnemonic = value;
                    RecalculatePositions();
                    Invalidate();
                }
            }
        }

        #region For Touch

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
        ///Gets or Sets the touchmode
        /// </summary>
		[DefaultValue(false)]
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
                        ApplyScaleToControl(1f);
                    }
                    
                }
            }
        }

        /// <summary></summary>
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
        ///Applies the scaling
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            cszDefaultCheckboxSize = new Size((int)(RECTBOXSIZE.Width * scaleFactor), (int)(RECTBOXSIZE.Height * scaleFactor));
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            this.mrectBox.Size = cszDefaultCheckboxSize;
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        #endregion

        /// <summary>
        /// Gets or sets the alignment of the text. WrapText must be set to false.
        /// </summary>
        [Browsable(false)]
        [Description("Indicates the alignment of the text.WrapText must be set to false.")]
        [Category("Appearance")]
        [DefaultValue(TextAlignment.Left)]
        [Obsolete]
        public TextAlignment TextAlignment
        {
            get
            {
                // return ModifyAlign( TextContentAlignment );
                return this.ModifyAlign(this.mtextAlignment);
            }

            set
            {
                this.TextContentAlignment = this.ModifyAlign(value);
                this.RecalculatePositions();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text. WrapText must be set to false.
        /// </summary>
        [Description("Indicates the alignment of the text.WrapText must be set to false.")]
        [Category("Appearance")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TextContentAlignment
        {
            get
            {
                return this.malignText;
            }

            set
            {
                if (this.malignText != value)
                {
                    this.malignText = value;
                    this.RecalculatePositions();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the end color of the gradient of the background of the control.
        /// </summary>
        [Description("The end color of the gradient of the background of the control.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color GradientEnd
        {
            get
            {
                return this.mgradientEnd;
            }

            set
            {
                if (this.mgradientEnd != value)
                {
                    this.mgradientEnd = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the start color of the gradient of the background of the control.
        /// </summary>
        [Description("The start color of the gradient of the background of the control.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Control")]
        public Color GradientStart
        {
            get
            {
                return this.mgradientStart;
            }

            set
            {
                if (this.mgradientStart != value)
                {
                    this.mgradientStart = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background style of the CheckBox.
        /// </summary>
        [Description("The background style of the CheckBox.")]
        [Category("Appearance")]
        [DefaultValue(CheckBoxAdvBackStyle.Default)]
        public CheckBoxAdvBackStyle BackgroundStyle
        {
            get
            {
                return this.backStyle;
            }

            set
            {
                if (this.backStyle != value)
                {
                    this.backStyle = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the offset of the text shadow.
        /// </summary>
        [Description("The offset of the text shadow.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Point), "2, 2")]
        public Point ShadowOffset
        {
            get
            {
                return this.mshadowOffset;
            }

            set
            {
                if (this.mshadowOffset != value)
                {
                    this.mshadowOffset = value;
                    if (this.textShadow)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the text shadow.
        /// </summary>
        [Description("The color of the text shadow.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Black")]
        public Color ShadowColor
        {
            get
            {
                return this.mshadowColor;
            }

            set
            {
                if (this.mshadowColor != value)
                {
                    this.mshadowColor = value;
                    if (this.textShadow)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text shadow is visible.
        /// </summary>
        [Description("Determines if the text shadow is visible.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool TextShadow
        {
            get
            {
                return this.textShadow;
            }

            set
            {
                if (this.textShadow != value)
                {
                    this.textShadow = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text in the CheckBox is wrapped.
        /// </summary>
        [Description("Determines if the text in the CheckBox is wrapped.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool WrapText
        {
            get
            {
                return this.mwrapText;
            }

            set
            {
                if (this.mwrapText != value)
                {
                    this.mwrapText = value;
                    this.RecalculatePositions();
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the state images of the CheckBox are stretched.
        /// </summary>
        [Description("Determines if the state images of the CheckBox are stretched.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool StretchImage
        {
            get
            {
                return this.mstretchImage;
            }

            set
            {
                if (this.mstretchImage != value)
                {
                    this.mstretchImage = value;
                    if (this.mimageCheckBox)
                    {
                        Invalidate(true);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the checkbox will be drawn using the images provided.
        /// </summary>
        [Description("Determines if the checkbox will be drawn using the images provided.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ImageCheckBox
        {
            get
            {
                return this.mimageCheckBox;
            }

            set
            {
                if (this.mimageCheckBox != value)
                {
                    this.mimageCheckBox = value;
                    this.RecalculatePositions();
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the checkbox alignment.
        /// </summary>
        [Description("Indicates the alignment of CheckBox."),
        DefaultValue(ContentAlignment.MiddleLeft),
        Category("Appearance")]
        public ContentAlignment CheckAlign
        {
            get
            {
                return this.malignCheckBox;
            }

            set
            {
                if (this.malignCheckBox != value)
                {
                    this.malignCheckBox = value;
                    this.RecalculatePositions();
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CheckBox will automatically calculate it`s height.
        /// </summary>
        [Description("Determines if the CheckBox will automatically calculate it`s height.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool AutoHeight
        {
            get
            {
                return this.mautoHeight;
            }

            set
            {
                if (this.mautoHeight != value)
                {
                    this.mautoHeight = value;

                    if (Parent != null)
                    {
                        Parent.PerformLayout(this, "Bounds");
                    }

                    this.RecalculatePositions();
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the focus rectangle is visible when it gets the focus.
        /// </summary>
        [Description("Determines if the focus rectangle is visible when it gets the focus.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool DrawFocusRectangle
        {
            get
            {
                return this.mdrawFocusRectangle;
            }

            set
            {
                if (this.mdrawFocusRectangle != value)
                {
                    this.mdrawFocusRectangle = value;
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the 3Dborder style of the checkbox.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Border3DStyle.Sunken)]
        [Description("Determines the style of the 3Dborder.")]
        public Border3DStyle Border3DStyle
        {
            get
            {
                return this.mborder3DStyle;
            }

            set
            {
                bool invalidate = false;
                if (this.mborder3DStyle != value && this.mborderStyle == BorderStyle.Fixed3D)
                {
                    invalidate = true;
                }

                this.mborder3DStyle = value;
                if (invalidate)
                {
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the border.It can be None, 3D and 2D.
        /// </summary>
        /// <remarks>
        /// By default it`s value is Fixed3D.
        /// </remarks>
        [Category("Appearance")]
        [DefaultValue(BorderStyle.None)]
        [Description("Determines the style of the border.")]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.mborderStyle;
            }

            set
            {
                if (this.mborderStyle != value)
                {
                    this.mborderStyle = value;
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the border when BorderStyles is FixedSingle.
        /// </summary>
        /// <remarks>
        /// By default it`s value is Solid.
        /// </remarks>
        [Category("Appearance")]
        [DefaultValue(ButtonBorderStyle.Solid)]
        [Description("Determines the style of the thin border.")]
        public ButtonBorderStyle BorderSingle
        {
            get
            {
                return this.mborderSingle;
            }

            set
            {
                bool invalidate = false;
                if (this.mborderSingle != value && this.mborderStyle == BorderStyle.FixedSingle)
                {
                    invalidate = true;
                }

                this.mborderSingle = value;
                if (invalidate)
                {
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the FixedSingle border when mouse over.
        /// </summary>
        [Description("Determines the color of the FixedSingle border when mouse over.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "WindowFrame")]
        public Color HotBorderColor
        {
            get
            {
                return this.mhotBorderColor;
            }

            set
            {
                if (this.mhotBorderColor != value)
                {
                    this.mhotBorderColor = value;
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        [Description("Determines the color of the border.")]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "WindowFrame")]
        public Color BorderColor
        {
            get
            {
                return this.mborderColor;
            }

            set
            {
                if (this.mborderColor != value)
                {
                    this.mborderColor = value;
                    Invalidate(true);
                }
            }
        }

        #endregion

        #region StateImage Properties
        /// <summary>
        /// Gets or sets the image used to draw the checkbox when unchecked and mouse over.
        /// </summary>
        [Description("Gets or sets the image used to draw the checkbox when unchecked and mouse over.")]
        [Category("StateImages")]
        [DefaultValue(null)]
        public Image MouseOverUncheckedImage
        {
            get
            {
                return this.mmouseOverUncheckedImage;
            }

            set
            {
                if (this.mmouseOverUncheckedImage != value)
                {
                    this.mmouseOverUncheckedImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the image used to draw the checkbox when checked and mouse over.
        /// </summary>
        [Description("Gets or sets the image used to draw the checkbox when checked and mouse over.")]
        [Category("StateImages")]
        [DefaultValue(null)]
        public Image MouseOverCheckedImage
        {
            get
            {
                return this.mmouseOverCheckedImage;
            }

            set
            {
                if (this.mmouseOverCheckedImage != value)
                {
                    this.mmouseOverCheckedImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the image used to draw the checkbox when disabled.
        /// </summary>
        [Description("Gets or sets the image used to draw the checkbox when disabled.")]
        [Category("StateImages")]
        [DefaultValue(null)]
        public Image DisabledImage
        {
            get
            {
                return this.mdisabledImage;
            }

            set
            {
                if (this.mdisabledImage != value)
                {
                    this.mdisabledImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the image used to draw the checkbox when unchecked and mouse not over.
        /// </summary>
        [Description("Gets or sets the image used to draw the checkbox when unchecked and mouse not over.")]
        [Category("StateImages")]
        [DefaultValue(null)]
        public Image UncheckedImage
        {
            get
            {
                return this.muncheckedImage;
            }

            set
            {
                if (this.muncheckedImage != value)
                {
                    this.muncheckedImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the image used to draw the checkbox when checked and mouse not over.
        /// </summary>
        [Description("Gets or sets the image used to draw the checkbox when checked and mouse not over.")]
        [Category("StateImages")]
        [DefaultValue(null)]
        public Image CheckedImage
        {
            get
            {
                return this.mcheckedImage;
            }

            set
            {
                if (this.mcheckedImage != value)
                {
                    this.mcheckedImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Image checkbox size.
        /// </summary>
        [Description("Gets or sets the image checkbox size.")]
        [Category("StateImages")]
        [DefaultValue(typeof(Size), "13, 13")]
        public Size ImageCheckBoxSize
        {
            get
            {
                if (this.mszImagedBox == Size.Empty)
                {
                    this.mszImagedBox = this.cszDefaultCheckboxSize;
                }

                return this.mszImagedBox;
            }

            set
            {
                if (this.mbinitializing || (this.mszImagedBox != value) && ((value.Height < ClientRectangle.Size.Height) && (value.Width < ClientRectangle.Size.Width)))
                {
                    this.mszImagedBox = value;

                    if (!this.mbinitializing && this.ImageCheckBox)
                    {
                        this.RecalculatePositions();
                        Invalidate();
                    }
                }
            }
        }

        #endregion StateImage Properties

        #region DataBinding Properties
        /// <summary>
        /// Gets or sets <see cref="IntValue"/> for the checked state.
        /// </summary>
        [Category("DataBinding")]
        [DefaultValue(1)]
        [Description("Gets or sets IntValue for the checked state.")]
        public int CheckedInt
        {
            get
              {
                  return this.mncheckedInt;
              }

            set 
            {
                this.mncheckedInt = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="IntValue"/> for the unchecked state.
        /// </summary>
        [Category("DataBinding")]
        [DefaultValue(0)]
        [Description("Gets or sets IntValue for the unchecked state.")]
        public int UncheckedInt
        {
            get 
            {
                return this.mnuncheckedInt; 
            }

            set 
            {
                this.mnuncheckedInt = value; 
            }
        }

        /// <summary>
        /// Gets or sets <see cref="StringValue"/> for the checked state.
        /// </summary>
        [Category("DataBinding")]
        [DefaultValue("Checked")]
        [Description("Gets or sets StringValue for the checked state.")]
        public string CheckedString
        {
            get { return this.mstrCheckedString; }
            set { this.mstrCheckedString = value; }
        }

        /// <summary>
        /// Gets or sets <see cref="StringValue"/> for the unchecked state.
        /// </summary>
        [Category("DataBinding")]
        [DefaultValue("Unchecked")]
        [Description("Gets or sets StringValue for the unchecked state.")]
        public string UncheckedString
        {
            get { return this.mstrUncheckedString; }
            set { this.mstrUncheckedString = value; }
        }

        #endregion DataBinding Properties

        #endregion Properties

        #region Helper Methods

        /// <summary>
        /// Added to build with version 1.0 of framework
        /// </summary>
        /// <param name="checkBoxSize">Check Box size</param>
        /// <returns>Returns type of rectangle</returns>
        internal Rectangle GetSpecificRectangle(Size checkBoxSize)
        {
            return new Rectangle(this.mrectBox.Location, checkBoxSize);
        }

        /// <summary>
        /// Added to build with version 1.0 of framework
        /// </summary>
        /// <returns>Returns Rectangle Box width</returns>
        internal int GetRectBoxWidth()
        {
            return this.mrectBox.Width;
        }

        /// <summary>
        /// Added to build with version 1.0 of framework
        /// </summary>
        /// <returns> Returns Rectangle box height</returns>
        internal int GetRectBoxHeight()
        {
            return this.mrectBox.Height;
        }

        /// <summary>
        /// Calculates text and checkbox positions.
        /// </summary>
        /// 
        #region Component Designer generated code

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (this.tcd != null)
                {
                    this.tcd.Dispose();
                    this.tcd = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion
        protected void RecalculatePositions()
        {
            if (this.AutoHeight)
            {
                this.RecalculateHeight();
            }

            Rectangle rectAlign = this.ClientRectangle;
            Size szcheckBox;

            if (this.ImageCheckBox)
            {
                szcheckBox = this.ImageCheckBoxSize;
            }
            else
            {
                szcheckBox = this.cszDefaultCheckboxSize;
            }

            if (!this.UseGDITextRendering)
                rectAlign = Rectangle.Inflate(rectAlign, -2, -2);
            this.mrectBox.Size = szcheckBox;
            this.mrectBox.Location = this.CalculatePosition(rectAlign, this.malignCheckBox, szcheckBox);
            this.RecalculateAlignRect(ref rectAlign, szcheckBox);

            this.mrectText.Size = this.GetTextSize(rectAlign.Width, rectAlign.Height);
            this.mrectText.Location = this.CalculatePosition(rectAlign, this.malignText, this.mrectText.Size);
        }
    #region Overrides

        /// <summary>
        /// On paint method
        /// </summary>
        /// <param name="e">Paint event Argument</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            bool bismirrored = this.GetIsMirrored();

            if (this.backStyle == CheckBoxAdvBackStyle.HorizontalGradient)
            {
                Color clrStart = bismirrored ? this.mgradientEnd : this.mgradientStart;
                Color clrEnd = bismirrored ? this.mgradientStart : this.mgradientEnd;

                this.cd.DrawGradient(e.Graphics, ClientRectangle, clrStart, clrEnd);
            }

            if (this.backStyle == CheckBoxAdvBackStyle.VerticalGradient)
            {
                this.cd.DrawVerticalGradient(e.Graphics, ClientRectangle, this.mgradientStart, this.mgradientEnd);
            }

            this.cd.DrawBorder(e.Graphics, this.ClientRectangle, this.mborderStyle, this.mborder3DStyle, this.mborderSingle, this.mmouseOver ? this.mhotBorderColor : this.mborderColor);

            if (this.mdrawFocusRectangle && this.Focused)
            {
                ControlPaint.DrawBorder(e.Graphics, this.mrectText, SystemColors.WindowFrame, ButtonBorderStyle.Dotted);
            }

            this.DrawText(e.Graphics);
        }

        /// <summary>
        /// Overridden OnRightToLeftChanged.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            this.RecalculatePositions();
            base.OnRightToLeftChanged(e);
        }

        /// <summary>
        /// Overridden OnMouseEnter.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (this.Enabled)
            {
                this.mmouseOver = true;
                Invalidate(true);
                base.OnMouseEnter(e);
            }
        }

        /// <summary>
        /// Overridden OnMouseLeave.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            this.mmouseOver = false;
            Invalidate(true);
            base.OnMouseLeave(e);
        }
      
        /// <summary>
        /// Overridden. See<see cref="System.Windows.Forms.Control.OnMouseDown"/>.
        /// </summary>
        /// <param name="e">MouseEventArgs that contains event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.Enabled)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Focus();
                    this.mmouseDown = true;
                }

                base.OnMouseDown(e);
                Invalidate(true);
            }
        }

        /// <summary>
        /// Overridden. <see cref="System.Windows.Forms.Control.OnMouseUp"/>
        /// </summary>
        /// <param name="e">MouseEventArgs that contains the event data. </param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.Enabled)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.mmouseDown = false;
                }

                base.OnMouseUp(e);
                Invalidate(true);
            }
        }
        
        /// <summary>
        /// Overrriden OnSizeChanged .
        /// </summary>
        /// <param name="e">EventArgs that contains the event data. </param>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (!this.mbinitializing)
            {
                if (!EnableTouchMode && this.DesignMode)
                {
                    CTRLSIZE = this.Size;
                }
                this.RecalculatePositions();
                Invalidate(true);
                base.OnSizeChanged(e);
            }
        }

        /// <summary>
        /// Overridden OnEnter.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnEnter(EventArgs e)
        {
            if (this.mdrawFocusRectangle)
            {
                Invalidate(true);
            }

            base.OnEnter(e);
        }

        /// <summary>
        /// Overridden OnLeave.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnLeave(EventArgs e)
        {
            Invalidate(true);
            base.OnLeave(e);
        }

        /// <summary>
        /// Overridden IsInputKey.
        /// </summary>
        /// <param name="keyData">Keys data that contains the event data. </param>
        /// <returns>Return bool property.</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            // So that we can process this in OnKeyDown
            if (keyData == Keys.Space)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Overridden OnKeyDown.
        /// </summary>
        /// <param name="e"> KeyEventArgs KeyEventArgs</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                if (this.Enabled)
                {
                    this.mmouseDown = false;
                    Invalidate(true);
                }

                e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Overridden OnTextChanged.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (!this.mbinitializing)
            {
                this.RecalculatePositions();
                Invalidate();
            }
        }

        /// <summary>
        /// Overridden OnFontChanged
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            RecalculatePositions();
            if (this.AutoHeight)
            {
                this.RecalculateHeight();
            }

        }

        /// <summary>
        /// Overridden OnenableChanged
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate(true);
        }

        /// <summary>
        /// Ovridden OnThemechanged.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data. </param>
        protected override void OnThemeChanged(EventArgs e)
        {
            Invalidate(true);
            base.OnThemeChanged(e);
        }

        /// <summary>
        /// Overridden OnHandleDestroyed.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Application.RemoveMessageFilter(this);

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Overridden OnHandleCreated.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            Application.AddMessageFilter(this);

            this.RecalculatePositions();
        }

        #endregion Overrides
     
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Size = new System.Drawing.Size(150, 21);
            this.mrectBox.Size = this.cszDefaultCheckboxSize;
            CTRLSIZE = new System.Drawing.Size(150, 21);
            RECTBOXSIZE = this.cszDefaultCheckboxSize;
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Retuns the  whether Mirrored or not.
        /// </summary>
        /// <returns>Returns bool property.</returns>
        private bool GetIsMirrored()
        {
            return RightToLeft.Yes == this.RightToLeft;
        }

        /// <summary>
        /// Performs text drawing routine.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        private void DrawText(Graphics g)
        {
            bool bIsMirrored = this.GetIsMirrored();
            m_TextFormatFlags = TextFormatFlags.Default;
            StringFormat sf = new StringFormat();
            sf.HotkeyPrefix = HotkeyPrefix.Show;

            if (this.mwrapText)
            {
                m_TextFormatFlags = TextFormatFlags.WordBreak;
            }
            else
            {
                m_TextFormatFlags |= TextFormatFlags.EndEllipsis;
                sf.Trimming = StringTrimming.EllipsisCharacter;
            }

            if (bIsMirrored)
            {
                m_TextFormatFlags |= TextFormatFlags.RightToLeft;
                sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            if (!this.altPressed && this.useMnemonic && !this.DesignMode && !SystemInformationExt.KeyboardCuesAlwaysOn)
            {
                m_TextFormatFlags |= TextFormatFlags.HidePrefix;
                sf.HotkeyPrefix = HotkeyPrefix.Hide;
            }
            if (!this.useMnemonic)
            {
                m_TextFormatFlags |= TextFormatFlags.NoPrefix;
                sf.HotkeyPrefix = HotkeyPrefix.None;
            }
            int nshadowOffsetX = bIsMirrored ? -this.mshadowOffset.X : this.mshadowOffset.X;
            if (this.mwrapText)
            {
                if (this.textShadow)
                {
                    Rectangle shadowRect = new Rectangle(this.mrectText.Location, this.mrectText.Size);
                    shadowRect.Offset(nshadowOffsetX, this.mshadowOffset.Y);
                    if (this.UseGDITextRendering)
                        TextRenderer.DrawText(g, Text, Font, shadowRect, this.mshadowColor, m_TextFormatFlags);
                    else
                        g.DrawString(Text, Font, new SolidBrush(this.mshadowColor), shadowRect, sf);
                }

                this.AppendStateAndThemeLogic(g, sf);
            }
            else
            {
                if (this.textShadow)
                {
                    if (this.UseGDITextRendering)
                        TextRenderer.DrawText(g, Text, Font, new Point(this.mrectText.X + nshadowOffsetX, this.mrectText.Y + this.mshadowOffset.Y), this.mshadowColor, m_TextFormatFlags);
                    else
                        g.DrawString(Text, Font, new SolidBrush(this.mshadowColor), new Point(this.mrectText.X + nshadowOffsetX, this.mrectText.Y + this.mshadowOffset.Y), sf);
                }

                this.AppendStateAndThemeLogic(g, sf);
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
                }
            }
        }
        /// <summary>
        /// Performs specific text drawing.
        /// </summary>
        /// <param name="e">Graphics to draw on.</param>
        /// <param name="sf">String Format</param>
        private void AppendStateAndThemeLogic(Graphics e, StringFormat sf)
        {
            if (this.mbtextRender)
            {
                if (Enabled)
                {
                    if (this.UseGDITextRendering)
                        TextRenderer.DrawText(e, Text, Font, this.mrectText, ForeColor, m_TextFormatFlags);
                    else
                        e.DrawString(Text, Font, new SolidBrush(ForeColor), this.mrectText, sf);
                }
                else
                {
                    if (ThemesEnabled)
                    {
                        if (!Enabled)
                        {
                            Brush brush1;
                            Color color = SystemColors.ControlDark;
                            brush1 = SystemBrushes.FromSystemColor(color);
                            if (this.UseGDITextRendering)
                                TextRenderer.DrawText(e, Text, Font, this.mrectText, color, m_TextFormatFlags);
                            else
                                e.DrawString(this.Text, this.Font, brush1, this.mrectText, sf);
                        }
                    }
                    else
                    {
                        if (BackColor == SystemColors.Control)
                        {
                            Color highlight = SystemColors.ControlLightLight;
                            Color shadow = SystemColors.ControlDark;
                            this.focusRect.Offset(1, 1);
                            if (this.UseGDITextRendering)
                            {
                                TextRenderer.DrawText(e, Text, Font, this.mrectText, highlight, m_TextFormatFlags);
                                this.focusRect.Offset(-1, -1);
                                TextRenderer.DrawText(e, Text, Font, this.mrectText, shadow, m_TextFormatFlags);

                            }
                            else
                            {
                                using (SolidBrush brush1 = new SolidBrush(highlight))
                                {
                                    e.DrawString(this.Text, this.Font, brush1, this.mrectText, sf);
                                    this.focusRect.Offset(-1, -1);
                                    brush1.Color = shadow;
                                    e.DrawString(this.Text, this.Font, brush1, this.mrectText, sf);
                                }
                            }
                        }
                        else
                        {
                            ControlPaint.DrawStringDisabled(e, Text, Font, BackColor, this.mrectText, sf);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recalculates control height.
        /// </summary>
        private void RecalculateHeight()
        {
            Size sztext = Size.Empty;
            Size szcheck = Size.Empty;
            Size szclientSize = this.ClientSize;
            szclientSize.Width -= this.cncheckBoxStandOff * 2;

            if (this.ImageCheckBox)
            {
                szcheck = this.ImageCheckBoxSize;
            }
            else
            {
                szcheck = this.mrectBox.Size;
            }

            if ((this.CheckAlign == ContentAlignment.BottomLeft) || (this.CheckAlign == ContentAlignment.MiddleLeft)
                || (this.CheckAlign == ContentAlignment.TopLeft) || (this.CheckAlign == ContentAlignment.BottomRight)
                || (this.CheckAlign == ContentAlignment.MiddleRight) || (this.CheckAlign == ContentAlignment.TopRight))
            {
                szclientSize.Width -= szcheck.Width + this.cncheckBoxStandOff;
            }

            sztext = this.GetTextSize(szclientSize.Width, szclientSize.Height);
            this.UpdateControlHeight(sztext);
        }

        /// <summary>
        ///  Update Control Height 
        /// </summary>
        /// <param name="sztext"> Size of the text.</param>
        private void UpdateControlHeight(Size sztext)
        {
            int ncontrolHeight = this.cncheckBoxStandOff + this.cncheckBoxStandOff;
            Size szcheck = Size.Empty;

            if (this.ImageCheckBox)
            {
                szcheck = this.ImageCheckBoxSize;
            }
            else
            {
                szcheck = this.mrectBox.Size;
            }

            switch (this.CheckAlign)
            {
                case ContentAlignment.TopCenter:
                case ContentAlignment.BottomCenter:
                    ncontrolHeight += sztext.Height + this.cncheckBoxStandOff + szcheck.Height;
                    break;

                default:
                    if (sztext.Height > szcheck.Height)
                    {
                        ncontrolHeight += sztext.Height;
                    }
                    else
                    {
                        ncontrolHeight += szcheck.Height;
                    }

                    break;
            }

            this.Height = ncontrolHeight;
        }

        /// <summary>
        /// Calculate rectangle position according to it alignment and size.
        /// </summary>
        /// <param name="rectOperating">Rectangle to position.</param>
        /// <param name="align">Rectangle alignment.</param>
        /// <param name="szrect">Rectangle size.</param>
        /// <returns>New Rectangle location.</returns>
        private Point CalculatePosition(Rectangle rectOperating, ContentAlignment align, Size szrect)
        {
            Point ptlocation = Point.Empty;
            bool brtl = this.GetIsMirrored();

            // auxiliary X
            float fleft = rectOperating.X;
            float fcenter = rectOperating.X + ((rectOperating.Width - szrect.Width) / 2);
            float fright = rectOperating.X + rectOperating.Width - szrect.Width;

            // auxiliary Y
            float ftop = rectOperating.Y;
            float fmiddle = rectOperating.Y + ((rectOperating.Height - szrect.Height) / 2);
            float fbottom = rectOperating.Y + (rectOperating.Height - szrect.Height);

            switch (align)
            {
                case ContentAlignment.TopLeft:
                    if (brtl)
                    {
                        ptlocation = this.SetLocation(fright, ftop);
                    }
                    else
                    {
                        ptlocation = SetLocation(fleft, ftop);
                    }

                    break;
                case ContentAlignment.TopCenter:
                    ptlocation = this.SetLocation(fcenter, ftop);
                    break;
                case ContentAlignment.TopRight:
                    if (brtl)
                    {
                        ptlocation = this.SetLocation(fleft, ftop);
                    }
                    else
                    {
                        ptlocation = SetLocation(fright, ftop);
                    }

                    break;
                case ContentAlignment.MiddleLeft:
                    if (brtl)
                    {
                        ptlocation = this.SetLocation(fright, fmiddle);
                    }
                    else
                    {
                        ptlocation = SetLocation(fleft, fmiddle);
                    }

                    break;
                case ContentAlignment.MiddleCenter:
                    ptlocation = this.SetLocation(fcenter, fmiddle);
                    break;
                case ContentAlignment.MiddleRight:
                    if (brtl)
                    {
                        ptlocation = this.SetLocation(fleft, fmiddle);
                    }
                    else
                    {
                        ptlocation = this.SetLocation(fright, fmiddle);
                    }

                    break;
                case ContentAlignment.BottomLeft:
                    if (brtl)
                    {
                        ptlocation = this.SetLocation(fright, fbottom);
                    }
                    else
                    {
                        ptlocation = SetLocation(fleft, fbottom);
                    }

                    break;
                case ContentAlignment.BottomCenter:
                    ptlocation = this.SetLocation(fcenter, fbottom);
                    break;
                case ContentAlignment.BottomRight:
                    if (brtl)
                    {
                        ptlocation = this.SetLocation(fleft, fbottom);
                    }
                    else
                    {
                        ptlocation = this.SetLocation(fright, fbottom);
                    }

                    break;
            }

            return ptlocation;
        }

        /// <summary>
        /// Sets new location. 
        /// </summary>
        /// <param name="fX">New X coordinate.</param>
        /// <param name="fY">New Y coordinate.</param>
        /// <returns>New location.</returns>
        private Point SetLocation(float fX, float fY)
        {
            return new Point((int)fX, (int)fY);
        }

        /// <summary>
        /// Recalculates rectangle bounds used to position text and checkbox in.
        /// </summary>
        /// <param name="rectAlign">Rectangle recalculating.</param>
        /// <param name="szcheckBox"> Checkbox Size</param>
        private void RecalculateAlignRect(ref Rectangle rectAlign, Size szcheckBox)
        {
            bool brtl = this.GetIsMirrored();

            // auxiliary
            int fwidth = szcheckBox.Width;
            int fheight = szcheckBox.Height;

            switch (this.malignCheckBox)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    if (brtl)
                    {
                        rectAlign.Width -= fwidth;
                    }
                    else
                    {
                        rectAlign.Width -= (fwidth + cncheckBoxStandOff);
                        rectAlign.X += (fwidth + cncheckBoxStandOff);
                    }

                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    if (brtl)
                    {
                        rectAlign.Width -= fwidth + this.cncheckBoxStandOff;
                        rectAlign.X += fwidth + this.cncheckBoxStandOff;
                    }
                    else
                    {
                        rectAlign.Width -= fwidth;
                    }

                    break;
                case ContentAlignment.TopCenter:
                    rectAlign.Height -= fheight + this.cncheckBoxStandOff;
                    rectAlign.Y += fheight + this.cncheckBoxStandOff;
                    break;
                case ContentAlignment.BottomCenter:
                    rectAlign.Height -= fheight + this.cncheckBoxStandOff;
                    break;
            }
        }

        /// <summary>
        /// Calculates text size.
        /// </summary>
        /// <param name="nwidth">Max allowed text width.</param>
        /// <param name="nheight">Max allowed text height.</param>
        /// <returns>New text size.</returns>
        private Size GetTextSize(int nwidth, int nheight)
        {
            Size sztemp = new Size(0, 0);
            if (nwidth <= 0)
            {
                sztemp = new Size(0, 0);
                this.mbtextRender = false;
            }
            else if (this.IsHandleCreated)
            {
                Graphics gph = this.CreateGraphics();
                if (gph != null)
                {
                    SizeF temp;
                    StringFormat sf = new StringFormat();
                    m_TextFormatFlags = TextFormatFlags.Default;

                    sf.HotkeyPrefix = HotkeyPrefix.Hide;
                    m_TextFormatFlags = TextFormatFlags.HidePrefix;

                    if (!this.useMnemonic)
                    {
                        sf.HotkeyPrefix = HotkeyPrefix.None;
                        m_TextFormatFlags = TextFormatFlags.NoPrefix;
                    }

                    if (this.mwrapText)
                    {
                        if (UseGDITextRendering)
                            temp = TextRenderer.MeasureText(Text, Font, new Size(nwidth, nheight), TextFormatFlags.WordBreak);
                        else
                            temp = gph.MeasureString(this.Text, Font, new SizeF(nwidth, nheight), sf);
                    }
                    else
                    {
                        sf.FormatFlags = StringFormatFlags.NoWrap;
                        sf.Trimming = StringTrimming.EllipsisCharacter;

                        m_TextFormatFlags |= TextFormatFlags.SingleLine;
                        m_TextFormatFlags |= TextFormatFlags.EndEllipsis;

                        if (UseGDITextRendering)
                            temp = TextRenderer.MeasureText(Text, Font, new Size(nwidth, nheight), m_TextFormatFlags);
                        else
                            temp = gph.MeasureString(this.Text, Font, nwidth, sf);
                    }

                    if (temp.Height > nheight)
                    {
                        temp.Height = nheight;
                    }

                    if (!this.mwrapText && temp.Width > nwidth)
                    {
                        temp.Width = nwidth;
                    }

                    sztemp = new Size((int)Math.Floor(temp.Width) + 1, (int)temp.Height);
                    this.mbtextRender = true;
                }

                gph.Dispose();
            }

            return sztemp;
        }

        /// <summary>
        /// Modifies ContentAlignment.
        /// </summary>
        /// <param name="alignText">new ContentAlignment value</param>
        /// <returns>Modified ContentAlignment.</returns>
        private ContentAlignment ModifyAlign(TextAlignment alignText)
        {
            ContentAlignment alignToReturn = ContentAlignment.MiddleCenter;
            switch (alignText)
            {
                case Syncfusion.Windows.Forms.Tools.TextAlignment.Center:
                    alignToReturn = ContentAlignment.MiddleCenter;
                    break;
                case Syncfusion.Windows.Forms.Tools.TextAlignment.Right:
                    alignToReturn = ContentAlignment.MiddleRight;
                    break;
                case Syncfusion.Windows.Forms.Tools.TextAlignment.Left:
                    alignToReturn = ContentAlignment.MiddleLeft;
                    break;
            }

            return alignToReturn;
        }

        /// <summary>
        /// Modifies TextAlignment.
        /// </summary>
        /// <param name="alignText">new TextAlignment value</param>
        /// <returns>Modified TextAlignment.</returns>
        private TextAlignment ModifyAlign(ContentAlignment alignText)
        {
            TextAlignment txtAlignToReturn = Syncfusion.Windows.Forms.Tools.TextAlignment.Left;
            switch (alignText)
            {
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                case ContentAlignment.BottomCenter:
                    txtAlignToReturn = Syncfusion.Windows.Forms.Tools.TextAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                    txtAlignToReturn = Syncfusion.Windows.Forms.Tools.TextAlignment.Left;
                    break;
                case ContentAlignment.BottomRight:
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                    txtAlignToReturn = Syncfusion.Windows.Forms.Tools.TextAlignment.Right;
                    break;
            }

            return txtAlignToReturn;
        }

        #endregion Helper Methods

        #region IMessageFilter overrides
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0104:
                    this.altPressed = true;
                    break;
            }
            return false;
        }
        #endregion
    }
}
