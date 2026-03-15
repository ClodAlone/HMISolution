#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for text nodes and labels.
    /// </summary>
    [Serializable]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class TextNode
        : Node
    {
        #region Class members

        /// <summary>
        /// Determines the default size of the TextNode when the text is empty.
        /// </summary>
        private SizeF c_DEFAULT_SIZE = new SizeF(100, 20);

        private string m_strText;
        private TextCases m_txtCase = TextCases.None;
        private string m_strOriginalText = string.Empty;

        /// <summary>
        /// Font properties.
        /// </summary>
        private FontStyle m_styleFont;

        /// <summary>
        /// Fill style used to draw Text.
        /// </summary>
        private FillStyle m_styleFill;

        /// <summary>
        /// Fill style used to draw Text.
        /// </summary>
        private FillStyle m_styleBackground;
        private StringAlignment m_alignHorizontal;
        private StringAlignment m_alignVertical;
        private StringFormatFlags m_fmtStringFormat;
        private bool m_bReadOnly;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TextNode"/> class.
        /// </summary>
        /// <param name="strText">The text.</param>
        public TextNode(string strText)
        {
            m_strText = strText;

            // calc new size
            RectangleF rectBounds = RectangleF.Empty;
            rectBounds.Size = SizeToText(new Size(0, 0));

            if (rectBounds.Size == SizeF.Empty)
            {
                rectBounds.Size = c_DEFAULT_SIZE;
            }

            Initialize(rectBounds, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextNode"/> class.
        /// </summary>
        /// <param name="strText">The text.</param>
        /// <param name="rectBounds">The rect bounds.</param>
        /// <param name="measureUnits">The measure units.</param>
        public TextNode(string strText, RectangleF rectBounds, MeasureUnits measureUnits)
        {
            m_strText = strText;

            Initialize(rectBounds, measureUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextNode"/> class.
        /// </summary>
        /// <param name="strText">The text.</param>
        /// <param name="rectBounds">The rect bounds.</param>
        public TextNode(string strText, RectangleF rectBounds)
        {
            m_strText = strText;

            Initialize(rectBounds, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextNode"/> class.
        /// </summary>
        /// <param name="src">The text node source.</param>
        public TextNode(TextNode src)
            : base(src)
        {
            m_strText = src.m_strText;

            if (src.m_styleFont != null)
                m_styleFont = (FontStyle)src.m_styleFont.Clone();

            if (src.m_styleFill != null)
                m_styleFill = (FillStyle)src.m_styleFill.Clone();

            if (src.m_styleBackground != null)
                m_styleBackground = (FillStyle)src.m_styleBackground.Clone();

            m_alignHorizontal = src.m_alignHorizontal;
            m_alignVertical = src.m_alignVertical;
            m_fmtStringFormat = src.m_fmtStringFormat;
            m_bReadOnly = src.m_bReadOnly;
            m_txtCase = src.m_txtCase;
            if (m_txtCase == TextCases.None)
                m_strOriginalText = m_strText;
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextNode"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected TextNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
             foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "text":
                    m_strText = info.GetString("text");
                    break;
                    case "fontStyle":
                    m_styleFont = (FontStyle)info.GetValue("fontStyle", typeof(FontStyle));
                    break;
                    case "fillStyle":
                    m_styleFill = (FillStyle)info.GetValue("fillStyle", typeof(FillStyle));
                    break;
                    case "backgroundStyle":
                    m_styleBackground = (FillStyle)info.GetValue("backgroundStyle", typeof(FillStyle));
                    break;
                    case "alignHorizontal":
                    m_alignHorizontal = (StringAlignment)info.GetValue("alignHorizontal", typeof(StringAlignment));
                    break;
                    case "alignVertical":
                    m_alignVertical = (StringAlignment)info.GetValue("alignVertical", typeof(StringAlignment));
                    break;
                    case "stringFormat":
                    m_fmtStringFormat = (StringFormatFlags)info.GetValue("stringFormat", typeof(StringFormatFlags));
                    break;
                    case "readOnly":
                    m_bReadOnly = info.GetBoolean("readOnly");
                    break;
                    case "textcase":
                    m_txtCase = (TextCases)info.GetValue("textcase", typeof(TextCases));
                    break;
                }
            }
             if (m_txtCase == TextCases.None)
                 m_strOriginalText = m_strText;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the value contained by the text object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Derived classes override this property in order to supply the
        /// text value in an implementation-specific way.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [Category("General")]
        [Description("Text value to be displayed.")]
        public string Text
        {
            get 
            { 
                return m_strText; 
            }
            set
            {
                if (!this.ReadOnly && m_strText != value && OnPropertyChanging(this.FullContainerName, DPN.Text, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.Text);

                    // set new value
                    m_strText = value;

                    if (m_txtCase == TextCases.None)
                        m_strOriginalText = m_strText;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.Text);
                }
            }
        }

        /// <summary>
        /// Gets or sets case of text in the RichTextNode
        /// </summary>
        [Browsable(true)]
        [DefaultValue(TextCases.None)]
        [Category("Formatting")]
        [Description("Specifies the text case sensitive.")]
        public TextCases TextCase
        {
            get
            {
                return m_txtCase;
            }
            set
            {
                if (value != m_txtCase && OnPropertyChanging(this.FullContainerName, DPN.TextCase, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.TextCase);

                    // set new value
                    m_txtCase = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.TextCase);
                    UpdateText();
                }
            }
        }

        /// <summary>
        /// Gets font color style.
        /// </summary>
        [Browsable(true)]
        [TypeConverter(typeof(FillStyleConverter))]
        [Category("Appearance")]
        [Description("Determines font fill style.")]
        public FillStyle FontColorStyle
        {
            get
            {
                if (m_styleFill == null)
                {
                    m_styleFill = new FillStyle();
                    m_styleFill.Color = Color.Black;
                }

                return m_styleFill;
            }
        }

        /// <summary>
        /// Gets text background style.
        /// </summary>
        [Browsable(true)]
        [TypeConverter(typeof(FillStyleConverter))]
        [Category("Appearance")]
        [Description("Determines text background style.")]
        public FillStyle BackgroundStyle
        {
            get
            {
                if (m_styleBackground == null)
                {
                    m_styleBackground = new BackgroundStyle();
                    m_styleBackground.Color = Color.Transparent;
                    m_styleBackground.ColorAlphaFactor = 0;
                }

                return m_styleBackground;
            }
        }

        /// <summary>
        /// Gets the font used to draw the text.
        /// </summary>
        [Browsable(true)]
        [TypeConverter(typeof(FontStyleConverter))]
        [Category("Appearance")]
        [Description("Determines the font used to draw the text.")]
        public FontStyle FontStyle
        {
            get
            {
                if (m_styleFont == null)
                    m_styleFont = new FontStyle();

                return m_styleFont;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text object is Read-only or not.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Flag indicating if the text object is Read-only or not.")]
        public bool ReadOnly
        {
            get 
            { 
                return m_bReadOnly; 
            }
            set
            {
                if (m_bReadOnly != value && OnPropertyChanging(this.FullContainerName, DPN.ReadOnly, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.ReadOnly);

                    // set new value
                    m_bReadOnly = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.ReadOnly);
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        /// <remarks>
        /// This property is used by the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.TextNode.GetStringFormat"/>
        /// method to generate a System.Drawing.StringFormat object. This property
        /// corresponds to the Alignment property in the System.Drawing.StringFormat
        /// class.
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(StringAlignment.Near)]
        [Category("Appearance")]
        [Description("Horizontal alignment of the text.")]
        public StringAlignment HorizontalAlignment
        {
            get 
            { 
                return m_alignHorizontal; 
            }
            set
            {
                if (m_alignHorizontal != value && OnPropertyChanging(this.FullContainerName, DPN.HorizontalAlignment, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.HorizontalAlignment);

                    // set new value
                    m_alignHorizontal = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.HorizontalAlignment);
                }
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the text.
        /// </summary>
        /// <remarks>
        /// This property is used by the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.TextNode.GetStringFormat"/>
        /// method to generate a System.Drawing.StringFormat object.  This property
        /// corresponds to the LineAlignment property in the System.Drawing.StringFormat
        /// class.
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(StringAlignment.Near)]
        [Category("Appearance")]
        [Description("Vertical alignment of the text.")]
        public StringAlignment VerticalAlignment
        {
            get 
            { 
                return m_alignVertical; 
            }
            set
            {
                if (m_alignVertical != value && OnPropertyChanging(this.FullContainerName, DPN.VerticalAlignment, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.VerticalAlignment);

                    // set new value
                    m_alignVertical = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.VerticalAlignment);
                }
            }
        }

        /// <summary>
        /// Gets flags used to format the text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See System.Drawing.StringFormatFlags for more details.
        /// </para>
        /// </remarks>
        [Browsable(false)]
        public StringFormatFlags FormatFlags
        {
            get { return m_fmtStringFormat; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text should be wrapped when it exceeds the width of
        /// the bounding box.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Formatting")]
        [Description("Indicates if text should be wrapped when it exceeds the width the bounding box.")]
        public bool WrapText
        {
            get 
            { 
                return !((m_fmtStringFormat & StringFormatFlags.NoWrap) == StringFormatFlags.NoWrap); 
            }
            set
            {
                if (this.WrapText != value && OnPropertyChanging(this.FullContainerName, DPN.WrapText, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.WrapText);

                    // set new value
                    if (!value)
                    {
                        m_fmtStringFormat |= StringFormatFlags.NoWrap;
                    }
                    else
                    {
                        m_fmtStringFormat = m_fmtStringFormat & (~StringFormatFlags.NoWrap);
                    }

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.WrapText);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text is right to left.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Formatting")]
        [Description("Specifies that text is right to left.")]
        public bool DirectionRightToLeft
        {
            get 
            { 
                return (m_fmtStringFormat & StringFormatFlags.DirectionRightToLeft) == StringFormatFlags.DirectionRightToLeft; 
            }
            set
            {
                if (this.DirectionRightToLeft != value && OnPropertyChanging(this.FullContainerName, DPN.DirectionRightToLeft, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.DirectionRightToLeft);

                    // set new value
                    if (value)
                    {
                        m_fmtStringFormat |= StringFormatFlags.DirectionRightToLeft;
                    }
                    else
                    {
                        m_fmtStringFormat = m_fmtStringFormat & (~StringFormatFlags.DirectionRightToLeft);
                    }

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.DirectionRightToLeft);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text is vertical.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Formatting")]
        [Description("Specifies that text is vertical.")]
        public bool DirectionVertical
        {
            get 
            { 
                return (m_fmtStringFormat & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical; 
            }
            set
            {
                if (this.DirectionVertical != value && OnPropertyChanging(this.FullContainerName, DPN.DirectionVertical, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.DirectionVertical);

                    // set new value
                    if (value)
                    {
                        m_fmtStringFormat |= StringFormatFlags.DirectionVertical;
                    }
                    else
                    {
                        m_fmtStringFormat = m_fmtStringFormat & (~StringFormatFlags.DirectionVertical);
                    }

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.DirectionVertical);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether no part of any glyph overhangs the bounding rectangle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default some glyphs overhang the rectangle slightly where necessary to
        /// appear at the edge visually. For example when an italic lowercase letter
        /// f in a font such as Garamond is aligned at the far left of a rectangle,
        /// the lower part of the f will reach slightly further left than the left
        /// edge of the rectangle. Setting this flag will ensure no painting outside
        /// the rectangle but will cause the aligned edges of adjacent lines of text
        /// to appear uneven.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Formatting")]
        [Description("Specifies that no part of any glyph overhangs the bounding rectangle.")]
        public bool FitBlackBox
        {
            get 
            { 
                return (m_fmtStringFormat & StringFormatFlags.FitBlackBox) == StringFormatFlags.FitBlackBox; 
            }
            set
            {
                if (this.FitBlackBox != value && OnPropertyChanging(this.FullContainerName, DPN.FitBlackBox, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.FitBlackBox);

                    // set new value
                    if (value)
                    {
                        m_fmtStringFormat |= StringFormatFlags.FitBlackBox;
                    }
                    else
                    {
                        m_fmtStringFormat = m_fmtStringFormat & (~StringFormatFlags.FitBlackBox);
                    }

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.FitBlackBox);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether only entire lines are laid out in the formatting rectangle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default, layout continues until the end of the text, or until no
        /// more lines are visible as a result of clipping, whichever comes first.
        /// Note that the default settings allow the last line to be partially
        /// obscured by a formatting rectangle that is not a whole multiple of
        /// the line height. To ensure that only whole lines are seen, specify
        /// this value and be careful to provide a formatting rectangle at least
        /// as tall as the height of one line.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Formatting")]
        [Description("Only entire lines are laid out in the formatting rectangle.")]
        public bool LineLimit
        {
            get 
            { 
                return (m_fmtStringFormat & StringFormatFlags.LineLimit) == StringFormatFlags.LineLimit; 
            }
            set
            {
                if (this.LineLimit != value && OnPropertyChanging(this.FullContainerName, DPN.LineLimit, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.LineLimit);

                    // set new value
                    if (value)
                    {
                        m_fmtStringFormat |= StringFormatFlags.LineLimit;
                    }
                    else
                    {
                        m_fmtStringFormat = m_fmtStringFormat & (~StringFormatFlags.LineLimit);
                    }

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.LineLimit);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether space at the end of each line in calculations that measure
        /// the size of the text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default, the boundary rectangle returned by the MeasureString
        /// method excludes the space at the end of each line. Set this flag
        /// to include that space in measurement.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Formatting")]
        [Description("Include space at the end of each line in calculations that measure the size of the text.")]
        public bool MeasureTrailingSpaces
        {
            get 
            { 
                return (m_fmtStringFormat & StringFormatFlags.MeasureTrailingSpaces) == StringFormatFlags.MeasureTrailingSpaces; 
            }
            set
            {
                if (this.MeasureTrailingSpaces != value && OnPropertyChanging(this.FullContainerName, DPN.MeasureTrailingSpaces, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.MeasureTrailingSpaces);

                    // set new value
                    if (value)
                    {
                        m_fmtStringFormat |= StringFormatFlags.MeasureTrailingSpaces;
                    }
                    else
                    {
                        m_fmtStringFormat = m_fmtStringFormat & (~StringFormatFlags.MeasureTrailingSpaces);
                    }

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.MeasureTrailingSpaces);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether overhanging parts of glyphs and unwrapped text reaching outside the
        /// formatting rectangle are allowed to show.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default, all text and glyph parts reaching outside the formatting
        /// rectangle are clipped.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Formatting")]
        [Description("Overhanging parts of glyphs and unwrapped text reaching outside the formatting rectangle are allowed to show.")]
        public bool NoClip
        {
            get 
            { 
                return !((m_fmtStringFormat & StringFormatFlags.NoClip) == StringFormatFlags.NoClip); 
            }
            set
            {
                if (this.NoClip != value && OnPropertyChanging(this.FullContainerName, DPN.NoClip, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.NoClip);

                    // set new value
                    if (!value)
                    {
                        m_fmtStringFormat |= StringFormatFlags.NoClip;
                    }
                    else
                    {
                        m_fmtStringFormat = m_fmtStringFormat & (~StringFormatFlags.NoClip);
                    }

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.NoClip);
                }
            }
        }
        #endregion

        #region Class overrides

        #region rendering
        /// <summary>
        /// Renders shapes visual representation on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void Render(Graphics gfx)
        {
            // 1 - call base method impementation
            base.Render(gfx);

            // 2 - Draw interior
            DrawInterior(gfx);

            // 3 - Draw border
            DrawBorder(gfx);
        }

        /// <summary>
        /// Draws the border.
        /// </summary>
        /// <param name="gfx">The graphics to draw on.</param>
        private void DrawBorder(Graphics gfx)
        {
            if (this.LineStyle.LineWidth > 0)
            {
                using (Pen pen = this.LineStyle.CreatePen())
                    gfx.DrawPath(pen, this.GraphicsPath);
            }
        }

        /// <summary>
        /// Draws node's interior on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        private void DrawInterior(Graphics gfx)
        {
            SizeF szSizeUnitIndependent = this.BoundsInfo.GetSize(MeasureUnits.Pixel);
            RectangleF rectBounds = new RectangleF(new PointF(0, 0), szSizeUnitIndependent);

            // Draw text background.
            if (!(this.BackgroundStyle.Color == Color.Transparent) || this.BackgroundStyle.Type == FillStyleType.Texture)
            {
                using (Brush backgroundBrush = this.BackgroundStyle.CreateBrush(gfx, rectBounds))
                {
                    gfx.FillRectangle(backgroundBrush, rectBounds.Left, rectBounds.Top, rectBounds.Width, rectBounds.Height);
                }
            }
            // Create text attibutes used to draw text.
            using (Font font = this.FontStyle.CreateFont())
            {
                // create brush to draw text.
                using (Brush fillBrush = this.FontColorStyle.CreateBrush(gfx, rectBounds))
                {
                    StringFormat fmt = GetStringFormat();
                    // Draw text.
                    // 6 is the minimum font size to draw text (1/72 in inch)
                    float fontSize = (MeasureUnitsConverter.Convert(MeasureUnitsConverter.FromPixelX(gfx.PageScale, MeasureUnits.Point) * this.FontStyle.Size, MeasureUnits.Point, MeasureUnits.Inch));
                    if (fontSize > (1 / 72f))
                    gfx.DrawString(this.Text, font, fillBrush, rectBounds, fmt);
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets the property container.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <returns>Property container.</returns>
        protected override object GetPropertyContainer(string strPropertyContainerName)
        {
            object objToReturn = base.GetPropertyContainer(strPropertyContainerName);

            if (strPropertyContainerName == "FontStyle")
            {
                objToReturn = this.FontStyle;
            }
            else if (strPropertyContainerName == "FillStyle")
            {
                objToReturn = this.FontColorStyle;
            }
            else if (strPropertyContainerName == "BackgroundStyle")
            {
                objToReturn = this.BackgroundStyle;
            }

            return objToReturn;
        }

        /// <summary>
        /// Updates the references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public override void UpdateReferences(IServiceReferenceProvider provider)
        {
            base.UpdateReferences(provider);

            this.FontColorStyle.UpdateServiceReferences(provider);
            this.BackgroundStyle.UpdateServiceReferences(provider);
            this.FontStyle.UpdateServiceReferences(provider);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("text", m_strText);
            info.AddValue("fontStyle", m_styleFont);
            info.AddValue("fillStyle", m_styleFill);
            info.AddValue("backgroundStyle", m_styleBackground);
            info.AddValue("alignHorizontal", m_alignHorizontal);
            info.AddValue("alignVertical", m_alignVertical);
            info.AddValue("stringFormat", m_fmtStringFormat);
            info.AddValue("readOnly", m_bReadOnly);
            info.AddValue("textcase", m_txtCase);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new TextNode(this);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Creates a StringFormat object that encapsulates the properties of
        /// the text object.
        /// </summary>
        /// <returns>System.Drawing.StringFormat object.</returns>
        /// <remarks>
        /// <para>
        /// The System.Drawing.StringFormat object returned by this method is
        /// used to draw the text using the System.Drawing.Graphics.DrawString
        /// method.
        /// </para>
        /// </remarks>
        public StringFormat GetStringFormat()
        {
            StringFormat fmt = new StringFormat();
            fmt.Alignment = this.HorizontalAlignment;
            fmt.LineAlignment = this.VerticalAlignment;
            fmt.FormatFlags = this.FormatFlags;
            return fmt;
        }

        /// <summary>
        /// Adjusts the size of the bounding box to fit the text.
        /// </summary>
        /// <param name="grfx">Graphics context used to measure the text.</param>
        /// <param name="layoutArea">Maximum layout size of the string.</param>
        /// <returns>New size of the bounding box.</returns>
        /// <remarks>
        /// <para>
        /// Uses the System.Drawing.Graphics.MeasureString method to calculate the
        /// size of the bounding box based on the font and text value.
        /// </para>
        /// </remarks>
        public SizeF SizeToText(Graphics grfx, SizeF layoutArea)
        {
            string textBuffer = this.Text;
            StringFormat strFormat = this.GetStringFormat();
            string curLine;
            SizeF curLineSize;
            int startOfLinePos = 0;
            int endOfLinePos = -1;
            bool moreLines = true;
            int termLen = 1;
            SizeF szText = new SizeF(0.0f, 0.0f);

            MeasureUnits measurementunits = this.BoundsInfo.Unit;

            if (measurementunits != MeasureUnits.Pixel)
            {
                layoutArea = MeasureUnitsConverter.ToPixels(layoutArea, measurementunits);
            }

            if (textBuffer != null && textBuffer.Length > 0)
            {
                Font font = this.FontStyle.CreateFont();

                while (moreLines)
                {
                    endOfLinePos = textBuffer.IndexOf("\r\n", startOfLinePos);
                    if (endOfLinePos >= 0)
                    {
                        termLen = 2;
                    }
                    else
                    {
                        endOfLinePos = textBuffer.IndexOf('\n', startOfLinePos);
                        termLen = 1;
                    }

                    if (endOfLinePos >= 0 && endOfLinePos < textBuffer.Length)
                    {
                        curLine = textBuffer.Substring(startOfLinePos, endOfLinePos - startOfLinePos);
                        startOfLinePos = endOfLinePos + termLen;
                    }
                    else
                    {
                        curLine = textBuffer.Substring(startOfLinePos);
                        moreLines = false;
                    }

                    curLineSize = grfx.MeasureString(curLine, font, layoutArea, strFormat);

                    if (curLineSize.Width > szText.Width)
                    {
                        szText.Width = curLineSize.Width;
                    }

                    if ((curLine == String.Empty) && (curLineSize.Height == 0))
                        szText.Height += font.Height;
                    else
                        szText.Height += curLineSize.Height;
                }

                font.Dispose();
            }

            if (measurementunits != MeasureUnits.Pixel)
            {
                szText = MeasureUnitsConverter.ToPixels(szText, measurementunits);
            }

            this.Size = szText;

            return szText;
        }

        /// <summary>
        /// Adjusts the size of the bounding box to fit the text.
        /// </summary>
        /// <param name="layoutArea">Maximum layout size of the string.</param>
        /// <returns>New size of the bounding box.</returns>
        /// <remarks>
        /// <para>
        /// Uses the System.Drawing.Graphics.MeasureString method to calculate the
        /// size of the bounding box based on the font and text value.
        /// </para>
        /// </remarks>
        public SizeF SizeToText(SizeF layoutArea)
        {
            SizeF szText;

            using (Graphics grfx = Graphics.FromHwnd(IntPtr.Zero))
            {
                szText = this.SizeToText(grfx, layoutArea);
            }

            return szText;
        }
        #endregion

        #region Class helper methods
        private void UpdateText()
        {
            if (m_txtCase == TextCases.AllUpper)
                Text = Text.ToUpper();
            else if (m_txtCase == TextCases.AllLower)
                Text = Text.ToLower();
            else
                Text = m_strOriginalText;
        }

        private void Initialize(RectangleF rectBounds, MeasureUnits measureUnits)
        {
            if (rectBounds.Height <= 0 || rectBounds.Height <= 0)
                throw new ArgumentOutOfRangeException("Neither of RichTextNode dimension values can be 0 or less!!");

            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);
            //// calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);
            //// assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);
            //// assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;
            //// Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            this.BoundsInfo.Unit = measureUnits;
            UpdateBoundingRectangle();
        }
        #endregion
    }
}
