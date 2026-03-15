#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;

#endif
namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The GradientLabel class provides a way to create fancy and appealing
    /// labels in all your forms.
    /// </summary>
    /// <remarks>The GradientLabel class is fully compatible
    /// with the Windows Forms <see cref="Label"/> that it derives from.
    /// The GradientLabel class gets most of its uniqueness from the
    /// <see cref="BrushInfo"/> class that is used for the <see cref="BackgroundColor"/>
    /// property.
    /// The look and feel of the GradientLabel is almost completely configurable
    /// through the <see cref="BackgroundColor"/> property.
    /// The <see cref="Border3DStyle"/> is another property that can specify the
    /// look and feel of the GradientLabel.
    /// </remarks>
    /// <example>
    /// <coderef file="tools\samples\editors package\GradientLabelDemo\CS\MainForm.cs" name="GradientLabel InitializeComponent" lang="C#">
    /// <code lang="C#">
    /// // InitializeComponent
    ///             // Create the Gradient Label
    ///             this.gradientLabel1 = new GradientLabel();
    ///             // Set formatting properties
    ///             this.gradientLabel1.Text = "Essential Suite Gradient Label";
    ///             this.gradientLabel1.BackgroundColor = new BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, System.Drawing.SystemColors.Highlight, System.Drawing.SystemColors.HighlightText);
    ///             this.gradientLabel1.BorderStyle = Border3DStyle.Etched;
    ///             this.gradientLabel1.Font = new Font("Microsoft Sans Serif", 14.25F, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
    ///             this.gradientLabel1.Location = new Point(24, 16);
    ///             this.gradientLabel1.Name = "gradientLabel1";
    ///             this.gradientLabel1.Size = new Size(440, 56);
    ///             this.gradientLabel1.TextAlign = ContentAlignment.MiddleCenter;
    ///             // Add the GradientLabel control to the form
    ///             this.Controls.Add(this.gradientLabel1);
    /// </code></coderef>
    /// <coderef file="tools\samples\editors package\GradientLabelDemo\VB\MainForm.vb" name="GradientLabel InitializeComponent" lang="VB">
    /// <code lang="VB">
    ///            ' InitializeComponent
    ///            ' Create the GradientLabel control.
    ///            Me.gradientLabel1 = New GradientLabel()
    ///            ' Set formatting properties
    ///            Me.gradientLabel1.Text = "Essential Suite Gradient Label"
    ///            Me.gradientLabel1.BackgroundColor = New BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, System.Drawing.SystemColors.Highlight, System.Drawing.SystemColors.HighlightText)
    ///            Me.gradientLabel1.BorderStyle = Border3DStyle.Etched
    ///            Me.gradientLabel1.Font = New Font("Microsoft Sans Serif", 14.25!, (System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    ///            Me.gradientLabel1.Location = New Point(24, 16)
    ///            Me.gradientLabel1.Name = "gradientLabel1"
    ///            Me.gradientLabel1.Size = New Size(440, 56)
    ///            Me.gradientLabel1.TextAlign = ContentAlignment.MiddleCenter
    ///            ' Add the GradientLabel control to the form
    ///            Me.Controls.Add(Me.gradientLabel1)
    /// </code></coderef>
    /// </example>
    [
     Designer(
        typeof(Syncfusion.Windows.Forms.Tools.GradientLabelDesigner),
        typeof(System.ComponentModel.Design.IDesigner)),
    ToolboxItem(true),
    ToolboxBitmap(typeof(GradientLabel), "ToolboxIcons.GradientLabel.bmp"),
    Description("Provides a way to create fancy and appealing labels in designer.")
    ]
    public class GradientLabel : Label
    {
        /// <summary>
        /// Specifies the border color of the gradient label.
        /// </summary>
        private Color m_borderColor = Color.Black;

        /// <summary>
        /// Specifies the appearance of the border.
        /// </summary>
        private BorderStyle m_borderAppearance = System.Windows.Forms.BorderStyle.Fixed3D;

        /// <summary>
        /// The border 3D style.
        /// </summary>
        private Border3DStyle border3dStyleValue;

        /// <summary>
        /// What sides have a border.
        /// </summary>
        private Border3DSide border3dSideValue;

        /// <summary>
        /// The background Brush info.
        /// </summary>
        private BrushInfo backgroundColorValue;

        /// <summary>
        /// Indicates whether the text should be drawn active when the control is disabled.
        /// </summary>
        private bool m_bDrawActiveWhenDisabled = false;

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);


        /// <summary>
        /// <summary>
        /// Initializes a new instance of the GradientLabel class.
        /// it.
        /// </summary>
        /// <remarks>This constructor initializes the look and feel of
        /// the <see cref="GradientLabel"/> by setting the  <see cref="Border3DStyle"/>
        /// and the <see cref="BackgroundColor"/> property.</remarks>
        public GradientLabel()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GradientLabel));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.border3dStyleValue = Border3DStyle.Flat;
            base.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.backgroundColorValue = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(237, 240, 247), System.Drawing.Color.LightCyan);
            this.border3dSideValue = Border3DSide.Bottom | Border3DSide.Top | Border3DSide.Right | Border3DSide.Left;

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            CTRLSIZE = this.Size;
        }

        /// <summary>
        /// Gets or sets a value indicating the border color of the gradient label.
        /// Can be set only in case if BorderAppearance is FixedSingle.
        /// </summary>
        [
        Category("Appearance"),
        Description("Indicates the border color. Can be set only in case if BorderAppearance is FixedSingle."),
        DefaultValue(typeof(Color), "Black")
        ]
        public Color BorderColor
        {
            get
            {
                return this.m_borderColor;
            }
            set
            {
                if (m_borderColor != value)
                {
                    m_borderColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the border appearance of the gradient label.
        /// </summary>
        [
        Category("Appearance"),
        Description("Indicates the border apperance"),
        DefaultValue(typeof(System.Windows.Forms.BorderStyle), "Fixed3D")
        ]
        public BorderStyle BorderAppearance
        {
            get
            {
                return this.m_borderAppearance;
            }
            set
            {
                if (this.m_borderAppearance != value)
                {
                    m_borderAppearance = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the BackColor. (overridden property)
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get
            {
                return Color.Transparent;
            }
            set
            {
                base.BackColor = Color.Transparent;
            }
        }

        /// <summary>
        /// Gets or sets the background color and other styles.
        /// </summary>
        /// <remarks>This property is the most important attribute of the 
        /// <see cref="GradientLabel"/> class. The <see cref="BrushInfo"/> class
        /// that is used by this property is a very extensive implementation 
        /// for custom painting. You can configure the different look and feel
        /// you want to give your <see cref="GradientLabel"/> by changing this 
        /// value.</remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("The background color and other styles can be set through this property.")
        ]
        public BrushInfo BackgroundColor
        {
            get
            {
                return this.backgroundColorValue;
            }

            set
            {
                this.backgroundColorValue = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the 3D border style for the GradientLabel.
        /// </summary>
        /// <remarks>The GradientLabel replaces the default <see cref="BorderStyle"/>
        /// provided for <see cref="Label"/> classes with the Border3DStyle type in
        /// this property.
        /// This property uses the <see cref="Border3DStyle"/> enumeration.
        /// Setting the value to <see cref="Border3DStyle.Adjust"/> shows no border.
        /// </remarks>
        [
        Category("Appearance"),
        Description("The 3D border style for the GradientLabel."),
        DefaultValue(typeof(Border3DStyle), "Flat")
        ]
        public new Border3DStyle BorderStyle
        {
            get
            {
                return this.border3dStyleValue;
            }

            set
            {
                this.border3dStyleValue = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text should be drawn active 
        /// when the control is disabled.
        /// </summary>
        [
        Category("Appearance"),
        Description("Indicates whether the text should be drawn active when the control is disabled."),
        DefaultValue(false)
        ]
        public bool DrawActiveWhenDisabled
        {
            get
            {
                return m_bDrawActiveWhenDisabled;
            }
            set
            {
                if (value != m_bDrawActiveWhenDisabled)
                {
                    m_bDrawActiveWhenDisabled = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Overrides the OnPainBackground method of the <see cref="Label"/>
        /// class.
        /// </summary>
        /// <param name="pe">The event args value for the event.</param>
        /// <remarks>This method is overriden to paint the background of the
        /// <see cref="GradientLabel"/> so that the text drawn by the default
        /// painting routines will not be affected. 
        /// The 3D border specified through the <see cref="Border3DStyle"/>
        /// is also painted through this method.</remarks>
        protected override void OnPaintBackground(PaintEventArgs pe)
        {
            base.OnPaintBackground(pe);

            if (this.BackgroundColor != null)
            {
                DrawBackground(pe.Graphics);
                DrawBorder(pe.Graphics);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.Enabled && this.DrawActiveWhenDisabled)
            {
                DrawImage(e);
                DrawText(e);
            }
            else
            {
                base.OnPaint(e);
            }
        }

        /// <summary>
        /// Draws the <see cref="Image"/>.
        /// </summary>
        /// <param name="e">PaintEventArgs that contains the event data.</param>
        private void DrawImage(PaintEventArgs e)
        {
            if (this.Image != null)
            {
                Rectangle imageRect = CalcImageRenderBounds(this.Image, this.ClientRectangle, RtlTranslateAlignment(this.ImageAlign));
                e.Graphics.DrawImage(this.Image, imageRect);
            }
        }

        /// <summary>
        /// Daras the <see cref="Text"/>.
        /// </summary>
        /// <param name="e">PaintEventArgs that contains the event data.</param>
        private void DrawText(PaintEventArgs e)
        {
            using (Brush textBrush = new SolidBrush(this.ForeColor))
            {
                StringFormat textFormat = GetStringFormat();
                e.Graphics.DrawString(this.Text, this.Font, textBrush, this.ClientRectangle, textFormat);
                textFormat.Dispose();
            }
        }

        /// <summary>
        /// Gets StringFormat for <see cref="Text"/>.
        /// </summary>
        /// <returns>Returns String Format</returns>
        private StringFormat GetStringFormat()
        {
            using (StringFormat textFormat = new StringFormat())
            {

                textFormat.LineAlignment = GetVerticalAlignment();
                textFormat.Alignment = GetHorizontalAlignment();

                if (this.RightToLeft == RightToLeft.Yes)
                {
                    textFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                }

                if (this.AutoSize)
                {
                    textFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                }

                if (!this.UseMnemonic)
                {
                    textFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
                }
                else if (base.ShowKeyboardCues)
                {
                    textFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                }
                else
                {
                    textFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
                }

                return textFormat;
            }

        }

        /// <summary>
        /// Gets horizontal alignment of the <see cref="Text"/>.
        /// </summary>
        /// <returns>Return String Alignment</returns>
        private StringAlignment GetHorizontalAlignment()
        {
            StringAlignment alignment = StringAlignment.Near;

            if (this.TextAlign == ContentAlignment.BottomCenter
                || this.TextAlign == ContentAlignment.MiddleCenter
                || this.TextAlign == ContentAlignment.TopCenter)
            {
                alignment = StringAlignment.Center;
            }
            else if (this.TextAlign == ContentAlignment.BottomLeft
                || this.TextAlign == ContentAlignment.MiddleLeft
                || this.TextAlign == ContentAlignment.TopLeft)
            {
                alignment = StringAlignment.Near;
            }
            else if (this.TextAlign == ContentAlignment.BottomRight
                || this.TextAlign == ContentAlignment.MiddleRight
                || this.TextAlign == ContentAlignment.TopRight)
            {
                alignment = StringAlignment.Far;
            }

            return alignment;
        }

        /// <summary>
        /// Gets vertical alignment of the <see cref="Text"/>.
        /// </summary>
        /// <returns>Return String Alignment</returns>
        private StringAlignment GetVerticalAlignment()
        {
            StringAlignment alignment = StringAlignment.Near;

            if (this.TextAlign == ContentAlignment.TopCenter
                || this.TextAlign == ContentAlignment.TopLeft
                || this.TextAlign == ContentAlignment.TopRight)
            {
                alignment = StringAlignment.Near;
            }
            else if (this.TextAlign == ContentAlignment.BottomCenter
                || this.TextAlign == ContentAlignment.BottomLeft
                || this.TextAlign == ContentAlignment.BottomRight)
            {
                alignment = StringAlignment.Far;
            }
            else if (this.TextAlign == ContentAlignment.MiddleCenter
                || this.TextAlign == ContentAlignment.MiddleLeft
                || this.TextAlign == ContentAlignment.MiddleRight)
            {
                alignment = StringAlignment.Center;
            }

            return alignment;
        }

        /// <summary>
        /// Draws the background. This is invoked by the <see cref="OnPaintBackground"/>
        /// override.
        /// </summary>
        /// <param name="g">The graphics object that the background is to be drawn on.</param>
        /// <remarks>You can override this virtual function to provide your own
        /// drawing methods.</remarks>
        protected virtual void DrawBackground(Graphics g)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                BrushPaint.FillRectangle(g, rect, this.BackgroundColor);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Draws the 3D border for the <see cref="GradientLabel"/>. This is invoked by the <see cref="OnPaintBackground"/>
        /// override.
        /// </summary>
        /// <param name="g">The graphics object that the background is to be drawn on.</param>
        /// <remarks>The 3D border specified in the <see cref="Border3DStyle"/> property
        /// is drawn in this method on the provided <see cref="Graphics"/> object.</remarks>
        protected virtual void DrawBorder(Graphics g)
        {
            Border3DSide borders;
            if (this.BorderSides == Border3DSide.All || this.BorderSides == Border3DSide.Middle)
                borders = Border3DSide.Bottom | Border3DSide.Top | Border3DSide.Right | Border3DSide.Left;
            else
                borders = this.BorderSides;

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            if (BorderAppearance == System.Windows.Forms.BorderStyle.FixedSingle)
            {
                int leftBorderWidth = 0, rightBorderWidth = 0, topBorderWidth = 0, bottomBorderWidth = 0;

                if (this.BorderSides == (Border3DSide.All | Border3DSide.Middle))
                {
                    leftBorderWidth = rightBorderWidth = topBorderWidth = bottomBorderWidth = 1;
                }
                else
                {
                    if ((this.BorderSides & Border3DSide.Left) > 0)
                    {
                        leftBorderWidth = 1;
                    }
                    if ((this.BorderSides & Border3DSide.Right) > 0)
                    {
                        rightBorderWidth = 1;
                    }
                    if ((this.BorderSides & Border3DSide.Top) > 0)
                    {
                        topBorderWidth = 1;
                    }
                    if ((this.BorderSides & Border3DSide.Bottom) > 0)
                    {
                        bottomBorderWidth = 1;
                    }
                }

                ControlPaint.DrawBorder(g, rect, this.BorderColor, leftBorderWidth, ButtonBorderStyle.Solid, this.BorderColor, topBorderWidth, ButtonBorderStyle.Solid, this.BorderColor, rightBorderWidth, ButtonBorderStyle.Solid, this.BorderColor, bottomBorderWidth, ButtonBorderStyle.Solid);                 
            }
            else if (BorderAppearance == System.Windows.Forms.BorderStyle.Fixed3D)
                ControlPaint.DrawBorder3D(g, rect, this.BorderStyle, borders);
        }

        /// <summary>
        /// Gets or sets the sides of the label that has borders drawn.
        /// </summary>
        /// <remarks>
        /// This property uses the <see cref="Border3DSide"/> enumeration.
        /// Setting the value to <see cref="Border3DSide.All"/> shows borders
        /// on all sides.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),

        Description("The sides of the gradient label that will have a border.")
        ]
        public Border3DSide BorderSides
        {
            get
            {
                return border3dSideValue;
            }

            set
            {
                this.border3dSideValue = value;
                this.Invalidate();
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
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true), DefaultValue(false),
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
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (!EnableTouchMode && this.DesignMode)
                CTRLSIZE = this.Size;
            base.OnSizeChanged(e);
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
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

        }
        #endregion
    }

    /// <summary>
    /// The clipping mode to be used by the control
    /// when returning the text content of the control.
    /// </summary>
    public enum BorderMode
    {
        /// <summary>
        /// Include all literals in the data that's returned.
        /// </summary>
        ThreeDimensional = 0,

        /// <summary>
        /// Exclude all literals in the data that's returned.
        /// </summary>
        Flat
    }

    #region Gradient Label Designer Class
    public class GradientLabelDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public GradientLabelDesigner()
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
                        new GradientLabelActionList(this.Component));
                }
                return actionLists;
            }
        }

#endif
    }
    #endregion

    #region Gradient Label ActionList Class
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    public class GradientLabelActionList : SyncActionListBase<GradientLabel>
    {
        public GradientLabelActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - Gradient Label");

            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the Name used in code to identify the Object.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("Image", "Image", "Appearance", "Gets or sets the image that is displayed on a Gradient Label.");
            this.AddDesignerActionPropertyItem("ImageAlign", "Image Align", "Appearance", "Gets or sets the alignment of an image that is displayed in the control.");
            this.AddDesignerActionPropertyItem("UseMnemonic", "UseMnemonic", "Appearance", "Gets or sets a value indicating whether the control interprets an ampersand character (&) in the control's System.Windows.Forms.Control.Text property to be an access key prefix character.");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");
            this.AddDesignerActionPropertyItem("TextAlign", "Text Alignment", "Appearance", "Indicates the alignment of Text in the Gradient Label.");
        }
        public string Name
        {
            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    GradientLabel control = this.Control as GradientLabel;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }
        public Image Image
        {
            get
            {
                Image bImage = null;
                if (this.Control != null)
                {
                    GradientLabel control = this.Control as GradientLabel;
                    bImage = control.Image;
                }
                return bImage;
            }
            set
            {
                SetValue("Image", value);
            }
        }
        public string Text
        {
            get
            {
                string text = " ";
                if (this.Control != null)
                {
                    GradientLabel control = this.Control as GradientLabel;
                    text = control.Text;
                }
                return text;
            }
            set
            {
                SetValue("Text", value);
            }
        }
        public ContentAlignment TextAlign
        {
            get
            {
                ContentAlignment textAlign = ContentAlignment.MiddleLeft;
                if (this.Control != null)
                {
                    GradientLabel control = this.Control as GradientLabel;
                    textAlign = control.TextAlign;
                }
                return textAlign;
            }
            set
            {
                SetValue("TextAlign", value);
            }
        }
        public ContentAlignment ImageAlign
        {
            get
            {
                ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
                if (this.Control != null)
                {
                    GradientLabel control = this.Control as GradientLabel;
                    imageAlign = control.ImageAlign;
                }
                return imageAlign;
            }
            set
            {
                SetValue("ImageAlign", value);
            }
        }
        public bool UseMnemonic
        {
            get
            {
                bool useMnemonic = true;
                if (this.Control != null)
                {
                    GradientLabel control = this.Control as GradientLabel;
                    useMnemonic = control.UseMnemonic;
                }
                return useMnemonic;
            }
            set
            {
                SetValue("UseMnemonic", value);
            }
        }
    }
#endif
    #endregion
}

