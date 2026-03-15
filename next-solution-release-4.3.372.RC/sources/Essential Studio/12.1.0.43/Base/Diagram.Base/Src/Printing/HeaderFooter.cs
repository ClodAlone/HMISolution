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
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Specifies the layout used for drawing the header/footer image.
    /// </summary>
    public enum ImageLayout
    {
        /// <summary>
        /// Specifies that the image will be tiled.
        /// </summary>
        Tile = 1,

        /// <summary>
        /// Specifies that the image will be centered.
        /// </summary>
        Center,

        /// <summary>
        /// Specifies that the image will be stretched.
        /// </summary>
        Stretch,

        /// <summary>
        /// Specifies that the image will be aligned along the left edge.
        /// </summary>
        AlignLeft,

        /// <summary>
        /// Specifies that the image will be aligned along the right edge.
        /// </summary>
        AlignRight
    }

    /// <summary>
    /// Specifies the alignment used for drawing the header/footer text.
    /// </summary>
    public enum HeaderFooter
    {
        /// <summary>
        /// Specifies that the text will be aligned along the left edge.
        /// </summary>
        Left = 1,

        /// <summary>
        /// Specifies that the text will be centered.
        /// </summary>
        Center,

        /// <summary>
        /// Specifies that the text will be aligned along the right edge.
        /// </summary>
        Right
    }

    /// <summary>
    /// Serializable class for working around Font serialization bug with SoapFormatter.
    /// </summary>
    [
    Serializable(),
    Syncfusion.Documentation.DocumentationExclude
    ]
    public class FontInfo
    {
        /// <summary>
        /// Font family
        /// </summary>
        public string family;

        /// <summary>
        /// Size of the font.
        /// </summary>
        public float size;

        /// <summary>
        /// Style of the font.
        /// </summary>
        public System.Drawing.FontStyle style;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontInfo"/> class.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <param name="size">The font size.</param>
        /// <param name="style">The font style.</param>
        public FontInfo(string family, float size, System.Drawing.FontStyle style)
        {
            this.family = family;
            this.size = size;
            this.style = style;
        }
    }

    /// <summary>
    /// The HFPosition class is used by the Essential Diagram printing system to specify the bounds used for 
    /// drawing the header/footer.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterBase"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterBase.Bounds"/>
    /// </summary>
    [
    Serializable,
    TypeConverter(typeof(ExpandableObjectConverter))
    ]
    public class HFPosition
        : ISerializable,
          ICloneable
    {
        #region Class members
        private float m_fRealHeight;
        private float m_fRealWidth;
        private float m_fWidth;
        private float m_fHeight;
        private float m_fLeft;
        private float m_fRight;
        private float m_fX;
        private float m_fY;
        private bool m_bResizing;
        private float m_fPageWidth;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HFPosition"/> class.
        /// </summary>
        public HFPosition()
        {
            m_fWidth = HeaderFooterData.DefaultPaperSize.Width;
            m_fPageWidth = HeaderFooterData.DefaultPaperSize.Width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HFPosition"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public HFPosition(SerializationInfo info, StreamingContext context)
        {
            this.m_fWidth = info.GetSingle("width");
            this.m_fHeight = info.GetSingle("height");
            this.m_fLeft = info.GetSingle("left");
            this.m_fRight = info.GetSingle("right");
            this.m_fPageWidth = info.GetSingle("pagewidth");
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the width of the page.
        /// </summary>
        /// <value>The width of the page.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public float PageWidth
        {
            get
            {
                return m_fPageWidth;
            }
            set
            {
                if (m_fPageWidth != value)
                {
                    if (m_fPageWidth == 0)
                    {
                        m_fWidth = value;
                        m_fPageWidth = value;
                    }
                    else
                    {
                        m_bResizing = true;

                        float temp = (value - m_fPageWidth) / m_fPageWidth;
                        this.LeftMargin += temp * this.m_fLeft;
                        this.RightMargin += temp * this.m_fRight;
                        this.Width += temp * this.m_fWidth;

                        m_bResizing = false;
                        m_fPageWidth = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the real.
        /// </summary>
        /// <value>The width of the real.</value>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public float RealWidth
        {
            get
            {
                return this.m_fRealWidth;
            }
            set
            {
                if (value != m_fRealWidth)
                {
                    m_fRealWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the real.
        /// </summary>
        /// <value>The height of the real.</value>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public float RealHeight
        {
            get
            {
                return this.m_fRealHeight;
            }
            set
            {
                if (value != m_fRealHeight)
                {
                    m_fRealHeight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the header/footer height(In Hundreds of Inches).
        /// </summary>
        /// <remarks>
        /// Applies only when the <see cref="HeaderFooterBase.AutoBounds"/> property is False.
        /// </remarks>
        [
        Browsable(true),
        Description("The header or footer height(In Hundreds of Inches)."),
        DefaultValue(0.0f)
        ]
        public float Height
        {
            get
            {
                return this.m_fHeight;
            }
            set
            {
                if (value != this.m_fHeight)
                {
                    this.m_fHeight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the header or footer width(In Hundreds of Inches).
        /// </summary>
        /// <remarks>
        /// Applies only when the <see cref="HeaderFooterBase.AutoBounds"/> property is False.
        /// </remarks>
        [
        Browsable(true),
        RefreshProperties(RefreshProperties.All),
        Description("The header or footer width(In Hundreds of Inches).")
        ]
        public float Width
        {
            get
            {
                return this.m_fWidth;
            }
            set
            {
                if (m_bResizing)
                {
                    if (value != m_fWidth)
                    {
                        m_fWidth = value;
                    }
                }
                else
                {
                    m_bResizing = true;
                    if (value > m_fPageWidth)
                    {
                        m_fWidth = m_fPageWidth;
                        this.LeftMargin = 0;
                        this.RightMargin = 0;
                    }
                    else
                    {
                        if (this.m_fLeft != 0 || this.m_fRight != 0)
                        {
                            float temp = (m_fWidth - value) / (this.m_fLeft + this.m_fRight);
                            this.LeftMargin += temp * this.m_fLeft;
                            this.RightMargin += temp * this.m_fRight;
                        }
                        m_fWidth = value;
                    }
                    m_bResizing = false;
                }
            }
        }

        /// <summary>
        /// Check if should to serialize the width.
        /// </summary>
        /// <returns>true, if serialize width.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeWidth()
        {
            return (int)this.m_fWidth != Syncfusion.Windows.Forms.Diagram.HeaderFooterData.DefaultPaperSize.Width;
        }

        /// <summary>
        /// Resets the width.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetWidth()
        {
            this.m_fWidth = Syncfusion.Windows.Forms.Diagram.HeaderFooterData.DefaultPaperSize.Width;
        }

        /// <summary>
        /// Gets or sets the X location.
        /// </summary>
        /// <value>The X location.</value>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public float XLocation
        {
            get
            {
                return this.m_fX;
            }
            set
            {
                if (this.m_fX != value)
                {
                    this.m_fX = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Y location.
        /// </summary>
        /// <value>The Y location.</value>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public float YLocation
        {
            get
            {
                return this.m_fY;
            }
            set
            {
                if (this.m_fY != value)
                {
                    this.m_fY = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the left margin.
        /// </summary>
        /// <value>The left margin.</value>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Left Margin (In Hundreds of Inches)."),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public float LeftMargin
        {
            get
            {
                return m_fLeft;
            }
            set
            {
                if (value < (this.PageWidth / 2))
                {
                    if (m_bResizing)
                    {
                        if (value != m_fLeft)
                        {
                            m_fLeft = value;
                        }
                    }
                    else
                    {
                        m_bResizing = true;
                        this.Width += m_fLeft - value;
                        m_fLeft = value;
                        m_bResizing = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the right margin.
        /// </summary>
        /// <value>The right margin.</value>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Right Margin (In Hundreds of Inches)."),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public float RightMargin
        {
            get
            {
                return m_fRight;
            }
            set
            {
                if (value < (this.PageWidth / 2))
                {
                    if (m_bResizing)
                    {
                        if (value != m_fRight)
                        {
                            m_fRight = value;
                        }
                    }
                    else
                    {
                        m_bResizing = true;
                        this.Width += m_fRight - value;
                        m_fRight = value;
                        m_bResizing = false;
                    }
                }
            }
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [Syncfusion.Documentation.DocumentationExclude()]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("height", this.m_fHeight);
            info.AddValue("width", this.m_fWidth);
            info.AddValue("left", this.m_fLeft);
            info.AddValue("right", this.m_fRight);
            info.AddValue("pagewidth", this.m_fPageWidth);
        }

        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// The HeaderFooterBase class implements the base functionality for the header and footer classes used by 
    /// the Essential Diagram printing system.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Header"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Footer"/>
    /// </summary>
    [
        Serializable,
        TypeConverter(typeof(ExpandableObjectConverter))
    ]
    public class HeaderFooterBase
        : ISerializable,
          ICloneable
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterBase"/> class.
        /// </summary>
        public HeaderFooterBase()
        {
            this.hfFont = new Font("Times New Roman", 12);
            this.border = new HeaderFooterBorder();
            this.position = new HFPosition();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterBase"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        public HeaderFooterBase(SerializationInfo info, StreamingContext context)
        {
            this.hfImage = (Image)SafelyGetValue(info, "image", typeof(Image));
            this.imgLayout = (ImageLayout)SafelyGetValue(info, "imgLayout", typeof(ImageLayout));
            this.bAutoMargins = (bool)SafelyGetValue(info, "automargins", typeof(bool));
            this.left = (string)SafelyGetValue(info, "Left", typeof(string));
            this.right = (string)SafelyGetValue(info, "Right", typeof(string));
            this.center = (string)SafelyGetValue(info, "Center", typeof(string));
            FontInfo fontinfo = (FontInfo)SafelyGetValue(info, "fontinfo", typeof(FontInfo));
            this.hfFont = new Font(fontinfo.family, fontinfo.size, fontinfo.style);
            this.visible = (bool)SafelyGetValue(info, "visible", typeof(bool));
            this.border = (HeaderFooterBorder)SafelyGetValue(info, "borderstyle", typeof(HeaderFooterBorder));
            this.position = (HFPosition)SafelyGetValue(info, "position", typeof(HFPosition));
        }

        /// <summary>
        /// Get value from serialization info without exceptions.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="name">The string.</param>
        /// <param name="type">The type.</param>
        /// <returns>The object.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected object SafelyGetValue(SerializationInfo info, string name, Type type)
        {
            object retval = null;
            try
            {
                retval = info.GetValue(name, type);
            }
            catch (Exception)
            {
                retval = this.GetDefaultValue(name);
            }

            return retval;
        }

        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the file path of the background image.
        /// </summary>
        [
        Browsable(true),
        Description("The file path of the background image.")
        ]
        public string ImagePath
        {
            get
            {
                return m_strImagePath;
            }
            set
            {
                if (m_strImagePath != value)
                    m_strImagePath = value;
            }
        }

        /// <summary>
        /// Serialize the image path.
        /// </summary>
        /// <returns>true, if serialize image path.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeImagePath()
        {
            return this.m_strImagePath != String.Empty;
        }

        /// <summary>
        /// Resets the image path.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetImagePath()
        {
            this.m_strImagePath = String.Empty;
        }

        /// <summary>
        /// Gets or sets the image to set as background.
        /// </summary>
        [
        DefaultValue(null),
        Browsable(true),
        Description("The Image to set as background.")
        ]
        public Image Image
        {
            get 
            { 
                return this.hfImage; 
            }
            set
            {
                if (value != this.hfImage)
                {
                    this.hfImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Syncfusion.Windows.Forms.Diagram.ImageLayout"/> value describing the image layout.
        /// </summary>
        /// <value></value>
        [
        Browsable(true),
        Editor(typeof(ImageLayoutEditor), typeof(UITypeEditor)),
        Description("The Image Layout."),
        DefaultValue(ImageLayout.Tile)
        ]
        public ImageLayout ImageLayout
        {
            get 
            { 
                return this.imgLayout; 
            }
            set
            {
                if (value != this.imgLayout)
                {
                    this.imgLayout = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto layout calculation is enabled for the header and footer.
        /// </summary>
        [
        Browsable(true),
        Description("Specifies if auto layout is enabled."),
        DefaultValue(true)
        ]
        public bool AutoBounds
        {
            get 
            { 
                return this.bAutoMargins; 
            }
            set
            {
                this.bAutoMargins = value;
            }
        }

        /// <summary>
        /// Gets the header/footer left output text.
        /// </summary>
        [
           Browsable(true),
           Description("Composed Left Text")
        ]
        public string ComposedLeft
        {
            get
            {
                return ComposeString(this.left);
            }
        }

        /// <summary>
        /// Gets the header/footer center output text.
        /// </summary>
        [
            Browsable(true),
            Description("Composed Center Text")
        ]
        public string ComposedCenter
        {
            get
            {
                return ComposeString(this.center);
            }
        }

        /// <summary>
        /// Gets the header/footer right output text.
        /// </summary>
        [
            Browsable(true),
            Description("Composed Right Text")
        ]
        public string ComposedRight
        {
            get { return ComposeString(this.right); }
        }

        /// <summary>
        /// Gets or sets text describing the left header or footer value.
        /// </summary>
        [Browsable(true)]
        [Editor(typeof(HeaderFooterEditor), typeof(UITypeEditor))]
        [Description("The left header/footer descriptor.")]
        public string Left
        {
            get 
            { 
                return this.left; 
            }
            set
            {
                value = value.Trim();
                if (value != this.left)
                {
                    if (value.Length == 0)
                    {
                        CheckText(HeaderFooter.Left);
                        value = String.Empty;
                    }
                    this.left = value;
                }
            }
        }

        /// <summary>
        /// Check if should the serialize left header or footer descriptor.
        /// </summary>
        /// <returns>true, if serialize left</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeLeft()
        {
            return this.left != String.Empty;
        }

        /// <summary>
        /// Reset the left header/footer descriptor.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetLeft()
        {
            this.left = String.Empty;
        }

        /// <summary>
        /// Gets or sets text describing the center header/footer value.
        /// </summary>
        [
        Browsable(true),
        Editor(typeof(HeaderFooterEditor), typeof(UITypeEditor)),
        Description("The center header/footer descriptor.")
        ]
        public string Center
        {
            get 
            { 
                return this.center; 
            }
            set
            {
                value = value.Trim();
                if (value != this.center)
                {
                    if (value.Length == 0)
                    {
                        CheckText(HeaderFooter.Center);
                        value = String.Empty;
                    }
                    this.center = value;
                }
            }
        }

        /// <summary>
        /// Check if should the serialize center header or footer descriptor.
        /// </summary>
        /// <returns>true, if serialize center.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeCenter()
        {
            return this.center != String.Empty;
        }

        /// <summary>
        /// Reset the center header or footer descriptor.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetCenter()
        {
            this.center = String.Empty;
        }

        /// <summary>
        /// Gets or sets text describing the right header or footer value.
        /// </summary>
        [
           Browsable(true),
           Editor(typeof(HeaderFooterEditor), typeof(UITypeEditor)),
           Description("The right header/footer descriptor.")
        ]
        public string Right
        {
            get 
            { 
                return this.right; 
            }
            set
            {
                value = value.Trim();
                if (value != this.right)
                {
                    if (value.Length == 0)
                    {
                        CheckText(HeaderFooter.Center);
                        value = String.Empty;
                    }
                    this.right = value;
                }
            }
        }

        /// <summary>
        /// Serialize the right header and footer descriptor.
        /// </summary>
        /// <returns>true, if serialize right.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeRight()
        {
            return this.right != String.Empty;
        }

        /// <summary>
        /// Resets the right header/footer descriptor.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetRight()
        {
            this.right = String.Empty;
        }

        /// <summary>
        /// Gets or sets the font used to draw the text.
        /// </summary>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Description("Specifies the font used to draw the text.")
        ]
        public Font Font
        {
            get
            {
                if (this.hfFont == null)
                    this.hfFont = new Font("Times New Roman", 12);
                return this.hfFont;
            }
            set
            {
                if (value != this.hfFont)
                {
                    this.hfFont = value;
                }
            }
        }

        /// <summary>
        /// Serialize the description font.
        /// </summary>
        /// <returns>true, if serialize font.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeFont()
        {
            return ((this.hfFont != null) && ((this.hfFont.FontFamily.Name != "Times New Roman") || (this.hfFont.Size != 12)));
        }

        /// <summary>
        /// Resets the descriptionfont.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetFont()
        {
            this.hfFont = new Font("Times New Roman", 12);
        }

        /// <summary>
        /// Gets or sets the color value used for the text.
        /// </summary>
        [
         Browsable(true),
         Description("Specifies the text color.")
        ]
        public Color ForeColor
        {
            get 
            { 
                return this.foreColor; 
            }
            set
            {
                if (value != this.foreColor)
                {
                    this.foreColor = value;
                }
            }
        }

        /// <summary>
        /// Check if should serialize the description fore color.
        /// </summary>
        /// <returns>true, if serialize forecolor.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeForeColor()
        {
            return (this.foreColor != SystemColors.ControlText);
        }

        /// <summary>
        /// Resets the description fore color .
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetForeColor()
        {
            this.foreColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the header or footer is visible.
        /// </summary>
        [
            Browsable(true),
            Description("Indicates whether the header or footer is visible"),
            DefaultValue(false)
        ]
        public bool Visible
        {
            get 
            { 
                return this.visible; 
            }
            set
            {
                if (value != this.visible)
                {
                    this.visible = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the description culture.
        /// </summary>
        /// <value>The culture.</value>
        [
        Syncfusion.Documentation.DocumentationExclude(),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public CultureInfo Culture
        {
            get
            {
                if (this.culture == null)
                {
                    this.culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
                }
                return this.culture;
            }
            set
            {
                if (value != this.culture)
                {
                    this.culture = value;
                }
            }
        }

        /// <summary>
        /// Gets style options for drawing the header/footer border.
        /// </summary>
        /// <value>A <see cref="HeaderFooterBorder"/> value.</value>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Description("Style options for drawing the border.")
        ]
        public HeaderFooterBorder BorderStyle
        {
            get
            {
                if (this.border == null)
                    this.border = new HeaderFooterBorder();
                return this.border;
            }
        }

        /// <summary>
        /// Check if should to serialize border style.
        /// </summary>
        /// <returns>true, if serialize border style.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeBorderStyle()
        {
            if ((this.BorderStyle.Style != DashStyle.Solid) || (this.BorderStyle.Weight != BorderWeight.Thin) || (this.BorderStyle.Color != Color.Black) || (this.BorderStyle.ShowBorder != false))
                return true;
            return false;
        }

        /// <summary>
        /// Reset the border style.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetBorderStyle()
        {
            this.border = new HeaderFooterBorder(DashStyle.Solid, Color.Black, BorderWeight.Thin);
        }

        /// <summary>
        /// Gets or sets the current page number. Used during printing.
        /// </summary>
        [
        Browsable(false),
        Description("Gets or sets current page number."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DefaultValue(0)
        ]
        public int CurrentPage
        {
            get { return this.curPage; }
            set { this.curPage = value; }
        }

        /// <summary>
        /// Gets or sets the total number of pages. Used during printing.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the total number of pages."),
        DefaultValue(0)
        ]
        public int TotalPages
        {
            get { return this.totalPages; }
            set { this.totalPages = value; }
        }

        /// <summary>
        /// Gets or sets the position of the left margin.
        /// </summary>
        [
        Browsable(true),
        RefreshProperties(RefreshProperties.All),
        Description("The position of the left margin."),
        DefaultValue(0F)
        ]
        public float MarginLeft
        {
            get
            {
                return this.position.LeftMargin;
            }
            set
            {
                if (this.position.LeftMargin != value)
                {
                    this.position.LeftMargin = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the right margin.
        /// </summary>
        [
        Browsable(true),
        RefreshProperties(RefreshProperties.All),
        Description("The position of the right margin."),
        DefaultValue(0F)
        ]
        public float MarginRight
        {
            get
            {
                return this.position.RightMargin;
            }
            set
            {
                if (this.position.RightMargin != value)
                {
                    this.position.RightMargin = value;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Syncfusion.Windows.Forms.Diagram.HFPosition"/> value describing the header or footer bounds.
        /// </summary>
        [
        Browsable(true),
        Description("The header and footer bounds. Applies only when the AutoBounds property is False"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        ]
        public HFPosition Bounds
        {
            get
            {
                if (this.position == null)
                    this.position = new HFPosition();
                return this.position;
            }
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [Syncfusion.Documentation.DocumentationExclude()]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("image", this.hfImage, typeof(Image));
            info.AddValue("imgLayout", this.imgLayout, typeof(ImageLayout));
            info.AddValue("automargins", this.bAutoMargins);
            info.AddValue("Left", this.left);
            info.AddValue("Right", this.right);
            info.AddValue("Center", this.center);
            info.AddValue("fontinfo", new FontInfo(this.hfFont.FontFamily.Name, this.hfFont.Size, this.hfFont.Style), typeof(FontInfo));
            info.AddValue("visible", this.visible);
            info.AddValue("borderstyle", this.border, typeof(HeaderFooterBorder));
            info.AddValue("position", this.position, typeof(HFPosition));
        }

        #endregion

        #region Class helper methods
        [Syncfusion.Documentation.DocumentationExclude()]
        private bool CheckText(HeaderFooter hf)
        {
            bool bSuccess = false;
            switch (hf)
            {
                case HeaderFooter.Left:
                    bSuccess = ((this.Center != String.Empty) || (this.Right != String.Empty));
                    break;
                case HeaderFooter.Center:
                    bSuccess = ((this.Left != String.Empty) || (this.Right != String.Empty));
                    break;
                case HeaderFooter.Right:
                    bSuccess = ((this.Center != String.Empty) || (this.Left != String.Empty));
                    break;
            }

            // this.Visible = !bSuccess;
            return bSuccess;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private string ComposeString(string str)
        {
            if ((str == null) || (str.Length == 0))
                return null;
           
            // if our string have only one character skip replacing
            else if (str.Length > 1)
            {
                string pattern = "(?<LongTime>&T)|(?<LongDate>&D)|(?<ShortDate>&d)|(?<Double>&&)|(?<Page>&p)|(?<TotalPages>&P)";
                MatchEvaluator match = new MatchEvaluator(this.CreateOutputString);
                str = Regex.Replace(str, pattern, match);
            }
            return str;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private string CreateOutputString(Match mat)
        {
            string str = string.Empty;
            if (mat.Groups["LongTime"].Success)
            {
                str = (DateTime.Now).ToString("T", this.Culture);
            }
            else if (mat.Groups["LongDate"].Success)
            {
                str = (DateTime.Now).ToString("D", this.Culture);
            }
            else if (mat.Groups["ShortDate"].Success)
            {
                str = (DateTime.Now).ToString("d", this.Culture);
            }
            else if (mat.Groups["Double"].Success)
            {
                str = "&";
            }
            else if (mat.Groups["Page"].Success)
            {
                str = "Page " + CurrentPage.ToString();
            }
            else if (mat.Groups["TotalPages"].Success)
            {
                str = "Total Pages " + TotalPages.ToString();
            }
            return str;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="pagewidth">The pagewidth.</param>
        /// <param name="pageheight">The pageheight.</param>
        /// <param name="marginBounds">The margin bounds.</param>
        /// <returns>The height.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public virtual float GetHeight(Graphics g, float pagewidth, float pageheight, System.Drawing.RectangleF marginBounds)
        {
            if (this.AutoBounds)
            {
                this.Bounds.RealWidth = this.Bounds.PageWidth - this.MarginLeft - this.MarginRight;
                this.Bounds.RealHeight = Calculate(g, this.Bounds.RealWidth);
            }
            else
            {
                this.Bounds.RealWidth = this.Bounds.Width;
                this.Bounds.RealHeight = this.Bounds.Height;
            }

            if (this.BorderStyle.ShowBorder)
            {
                this.Bounds.RealHeight += this.BorderStyle.Width;
                this.Bounds.RealWidth -= this.BorderStyle.Width;
            }

            this.Bounds.XLocation = marginBounds.X + this.MarginLeft;
            return this.Bounds.RealHeight;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private float Calculate(Graphics g, float pageWidth)
        {
            float fHighest = 0;

            Font font = this.Font;

            string headerRight = this.ComposeString(this.Right);
            string headerLeft = this.ComposeString(this.Left);
            string headerCenter = this.ComposeString(this.Center);

            // Checking for empty string after trim operation
            if (headerCenter == null)
            {
                if (headerLeft == null)
                {
                    if (headerRight == null)
                    {
                        // if there is no text header height will be as image height
                        if (this.Image != null)
                            return this.Image.Height;
                        else
                            return 0;
                    }
                    else
                    {
                        // if there is only right header draw it using full page width
                        MeasureString(g, headerRight, font, pageWidth, ref fHighest);
                    }
                }
                else if (headerRight != null)
                {
                    // if there is no center header but left and right make left and right headers width halh page width
                    MeasureString(g, headerRight, font, pageWidth / 2, ref fHighest);
                    MeasureString(g, headerLeft, font, pageWidth / 2, ref fHighest);
                }
                else
                {
                    MeasureString(g, headerLeft, font, pageWidth, ref fHighest);
                }
            }
            else
            {
                // center header is present
                // if there are no left and right headers draw center header using full page width
                if ((headerLeft == null) && (headerRight == null))
                {
                    MeasureString(g, headerCenter, font, pageWidth, ref fHighest);
                }
                else
                {
                    // if there is left or right header or both set each header width equal
                    MeasureString(g, headerLeft, font, pageWidth / 3, ref fHighest);
                    MeasureString(g, headerRight, font, pageWidth / 3, ref fHighest);
                    MeasureString(g, headerCenter, font, pageWidth / 3, ref fHighest);
                }
            }

            return fHighest;
        }

        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="str">The STR.</param>
        /// <param name="font">The font.</param>
        /// <param name="width">The width.</param>
        /// <param name="highest">The highest.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void MeasureString(Graphics g, string str, Font font, float width, ref float highest)
        {
            SizeF size = g.MeasureString(str, font, (int)width);
            if (size.Height > highest)
                highest = size.Height;
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The object.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected object GetDefaultValue(string name)
        {
            object retval = null;
            switch (name)
            {
                case "image":
                        retval = this.Image;
                        break;
                case "imgLayout":
                        retval = this.ImageLayout;
                        break;
                case "automargins":
                        retval = this.AutoBounds;
                        break;
                case "Left":
                        retval = this.Left;
                        break;
                case "Right":
                        retval = this.Right;
                        break;
                case "Center":
                        retval = Center;
                        break;
                case "font":
                        retval = this.Font;
                        break;
                case "fontfamily":
                        Font deffont = this.Font;
                        retval = new FontInfo(deffont.FontFamily.Name, deffont.Size, deffont.Style);
                        break;
                case "visible":
                        retval = this.Visible;
                        break;
                case "borderstyle":
                        retval = this.BorderStyle;
                        break;
                default: break;
            }
            return retval;
        }

        #endregion

        #region Fields
        private string m_strImagePath = String.Empty;
        private HeaderFooterBorder border = null;
        private HFPosition position = null;
        private bool bAutoMargins = true;
        private Image hfImage = null;
        private ImageLayout imgLayout = ImageLayout.Tile;
        private string left = String.Empty;
        private string center = String.Empty;
        private string right = String.Empty;
        private Font hfFont = null;
        private Color foreColor = SystemColors.ControlText;
        private bool visible = false;
        private CultureInfo culture;
        private int curPage = 0;
        private int totalPages = 0;
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            HeaderFooterBase hfBase = (HeaderFooterBase)MemberwiseClone();
            if (hfBase.position != null)
                hfBase.position = (HFPosition)this.Bounds.Clone();
            else
                hfBase.position = new HFPosition();

            if (hfBase.border != null)
                hfBase.border = (HeaderFooterBorder)this.BorderStyle.Clone();
            else
                hfBase.border = new HeaderFooterBorder();
            return hfBase;
        }

        #endregion
    }

    /// <summary>
    /// Implements the header class used by the Essential Diagram printing system.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterBase"/>
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Footer"/>
    /// </summary>
    [Serializable,
    TypeConverter(typeof(HeaderConverter))
    ]
    public class Header : HeaderFooterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class.
        /// </summary>
        public Header()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public Header(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="pagewidth">The pagewidth.</param>
        /// <param name="pageheight">The pageheight.</param>
        /// <param name="marginBounds">The margin bounds.</param>
        /// <returns>The height.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override float GetHeight(Graphics g, float pagewidth, float pageheight, System.Drawing.RectangleF marginBounds)
        {
            float height = 0;
            if (this.Visible)
            {
                height = base.GetHeight(g, pagewidth, pageheight, marginBounds);
                this.Bounds.YLocation = marginBounds.Top;
                height = this.Bounds.RealHeight;
            }
            return height;
        }
    }

    /// <summary>
    /// Implements the footer class used by the Essential Diagram printing system.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterBase"/>
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Header"/>
    /// </summary>
    [Serializable,
    TypeConverter(typeof(FooterConverter))
    ]
    public class Footer : HeaderFooterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Footer"/> class.
        /// </summary>
        public Footer()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Footer"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public Footer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <param name="g">The graphics to draw.</param>
        /// <param name="pagewidth">The pagewidth.</param>
        /// <param name="pageheight">The pageheight.</param>
        /// <param name="marginBounds">The margin bounds.</param>
        /// <returns>The height.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override float GetHeight(Graphics g, float pagewidth, float pageheight, System.Drawing.RectangleF marginBounds)
        {
            float height = 0;
            if (this.Visible)
            {
                height = base.GetHeight(g, pagewidth, pageheight, marginBounds);
                this.Bounds.YLocation = marginBounds.Top + pageheight - this.Bounds.RealHeight;
                height = this.Bounds.RealHeight;
            }
            return height;
        }
    }
}
