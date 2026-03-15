#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
////
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    #region Enum
    /// <summary>
    /// Represents type of width and height properties.
    /// </summary>
    public enum SizeTypeEx
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Number's type of size (px, em, etc...).
        /// </summary>
        Number,

        /// <summary>
        /// Percentage type of width and height properties.
        /// </summary>
        Percent
    }

    /// <summary>
    /// Indicates which type of background image is in format.
    /// </summary>
    public enum RepeatStyle
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Image repeats by X and by Y.
        /// </summary>
        Repeat,

        /// <summary>
        /// Image repeats by X.
        /// </summary>
        RepeatX,

        /// <summary>
        /// Image repeats by Y.
        /// </summary>
        RepeatY,

        /// <summary>
        /// Image does not repeat.
        /// </summary>
        NoRepeat
    }
    #endregion

    /// <summary>
    /// Implementation of the HTMLFormat interface.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter)), AttributeHolder(typeof(HTMLFormat)), Serializable]
    public class HTMLFormat
      : IHTMLFormat, ICloneable, IDisposable, ISerializable
    {
        #region Class constants
        /// <summary>
        /// Default case insensitive comparer for internal use.
        /// </summary>
        private static readonly IComparer DEF_COMPARER = new CaseInsensitiveComparer();
        #endregion

        #region Class members
        /// <summary>
        /// Indicates whether skip event is raised.
        /// </summary>
        private bool m_bSkipEvents;

        /// <summary>
        /// Format which was inherited by this format.
        /// </summary>
        private IHTMLFormat m_FormatParent;

        /// <summary>
        /// XML storage of the current format.
        /// </summary>
        [NonSerialized]
        private XmlElement m_storage;

        /// <summary>
        /// Type of the format.
        /// Indicates in what manner the format is connected to the element.
        /// </summary>
        private FormatType m_type;

        /// <summary>
        /// Index of the format.
        /// </summary>
        private long m_index = -1;

        /// <summary>
        /// Name of the format.
        /// </summary>
        private string m_Name;

        /// <summary>
        /// Font of the format.
        /// </summary>
        private Font m_Font; // = Control.DefaultFont;

        /// <summary>
        /// Foreground color of the format.
        /// </summary>
        private Color m_ForeColor; // = Control.DefaultForeColor;

        /// <summary>
        /// Background color of the format.
        /// </summary>
        private Color m_BgColor; // = Control.DefaultBackColor;

        /// <summary>
        /// Background image for the element.
        /// </summary>
        private Bitmap m_bgImage;

        /// <summary>
        /// Type of drawing background image.
        /// </summary>
        private RepeatStyle m_bgRepeat;

        /// <summary>
        /// Horizontal aligment of the format.
        /// </summary>
        private StringAlignment m_HAligment;

        /// <summary>
        /// Vertical aligment of the format.
        /// </summary>
        private StringAlignment m_VAligment;

        /// <summary>
        /// Cursor for the format.
        /// </summary>
        private Cursor m_cursor = Cursors.Default;

        /// <summary>
        /// Left border of the format.
        /// </summary>
        private Border m_BorderLeft;

        /// <summary>
        /// Right border of the format.
        /// </summary>
        private Border m_BorderRight;

        /// <summary>
        /// Top border of the format.
        /// </summary>
        private Border m_BorderTop;

        /// <summary>
        /// Bottom border of the format.
        /// </summary>
        private Border m_BorderBottom;

        /// <summary>
        /// Rectangle which holds all paddings (top, left, right, bottom).
        /// </summary>
        private Rectangle m_Padding;

        /// <summary>
        /// Mask enumeration for merging elements of the format.
        /// </summary>
        private MergeMask m_enMerge;

        /// <summary>
        /// Holds the width of the element region.
        /// </summary>
        private int m_width = -1;

        /// <summary>
        /// Holds the height of the element region.
        /// </summary>
        private int m_height = -1;

        /// <summary>
        /// Type of the width.
        /// </summary>
        private SizeTypeEx m_widthType;

        /// <summary>
        /// Type of the height.
        /// </summary>
        private SizeTypeEx m_heightType;

        /// <summary>
        /// Indicates whether the object was disposed before.
        /// </summary>
        private bool m_bDisposed;

        /// <summary>
        /// FontStyle property value.
        /// </summary>
        private FontStyle m_fontStyle;

        /// <summary>
        /// FontWeight property value.
        /// </summary>
        private FontStyle m_fontWeight;

        /// <summary>
        /// TextDecoration property value.
        /// </summary>
        private FontStyle m_textDecoration;

        /// <summary>
        /// Family of the font for the current format.
        /// </summary>
        private string m_fontFamily;

        /// <summary>
        /// Size of the font for the current format.
        /// </summary>
        private float m_fontSize;

        /// <summary>
        /// Indicates whether the format is already merged.
        /// </summary>
        private bool m_bIsMerged;

        /// <summary>
        /// Indicates whether the font is created after format merging.
        /// </summary>
        private bool m_bIsFontCreated;

        /// <summary>
        /// Graphical units for the font size.
        /// </summary>
        private GraphicsUnit m_unit;

        /// <summary>
        /// Indicates whether the 'display' attribute was none.
        /// </summary>
        private bool m_bDisplayNone;

        /// <summary>
        /// Indicates whether the format should be disposed when the parent HTML element is being disposed.
        /// </summary>
        private bool m_bDisposeWithElement = true;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets the format from which this format is inherited.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IHTMLFormat FormatParent
        {
            get
            {
                return m_FormatParent;
            }
        }

        /// <summary>
        /// Gets the index of the format.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long Index
        {
            get
            {
                return m_index;
            }
        }

        /// <summary>
        /// Gets or sets the name of the format.
        /// </summary>
        [Browsable(false), ReactionType("Name", ReactType.None), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Name");

                if (value != m_Name)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_Name, value);
                    m_Name = value;
                    OnNameChanged("Name", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the manner in which the format is to be connected to the element.
        /// </summary>
        [Category("Behaviour"), Browsable(true), ReadOnly(true), DefaultValue(typeof(FormatType), "Class"), Description("Indicate in what manner format will be used."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FormatType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                if (m_type != value)
                {
                    m_type = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), AmbientValue(null), ReactionType("Font", ReactType.ReChildrenFormatMergeAndRecalcDoc), Description("Gets or sets font of the format.")]
        public Font Font
        {
            get
            {
                if (m_bIsMerged && !m_bIsFontCreated)
                {
                    m_Font = new Font(new FontFamily(this.FontFamilyName), this.FontSize, this.FontStyle, this.Unit);

                    m_bIsFontCreated = true;
                }

                return m_Font;
            }
            set
            {
                if (value != m_Font)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_Font, value);

                    //// Set font and its properties.
                    m_Font = (value == null) ? Control.DefaultFont : value;
                    SetFontPropertiesFrom(m_Font);
                    m_bIsFontCreated = true;

                    OnFontChanged("Font", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color for the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), ReactionType("Forecolor", ReactType.ReChildrenFormatMerge), Description("Gets or sets foreground color for the format.")]
        public Color ForeColor
        {
            get
            {
                return m_ForeColor;
            }
            set
            {
                if (value != m_ForeColor)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_ForeColor, value);
                    m_ForeColor = value;
                    OnForeColorChanged("Forecolor", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), ReactionType("BackgroundColor", ReactType.RePaintElement), Description("Gets or sets background color of the format.")]
        public Color BackgroundColor
        {
            get
            {
                return m_BgColor;
            }
            set
            {
                if (m_BgColor != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_BgColor, value);
                    m_BgColor = value;
                    OnBackgroundColorChanged("BackgroundColor", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), ReactionType("VerticalAlign", ReactType.ReCalculatingDocument), Description("Gets or sets Vertical Alignment of the format.")]
        public StringAlignment VerticalAlign
        {
            get
            {
                return m_VAligment;
            }
            set
            {
                if (m_VAligment != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_VAligment, value);
                    m_VAligment = value;
                    OnVerticalAlignChanged("VerticalAlign", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), ReactionType("HorizontalAlign", ReactType.ReChildrenFormatMergeAndRecalcDoc), Description("Gets or sets Horizontal Alignment of the format.")]
        public StringAlignment HorizontalAlign
        {
            get
            {
                return m_HAligment;
            }
            set
            {
                if (m_HAligment != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_HAligment, value);
                    m_HAligment = value;
                    OnHorizontalAlignChanged("HorizontalAlign", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the left border of the format.
        /// </summary>
        [Browsable(true), ReactionType("Left", ReactType.ReCalculatingDocument), Description("Gets or sets left border of the format."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IBorder Left
        {
            get
            {
                return m_BorderLeft;
            }
            set
            {
                if (m_BorderLeft != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_BorderLeft, value);
                    m_BorderLeft = (Border)value;
                    OnLeftChanged("Left", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the right border of the format.
        /// </summary>
        [Browsable(true), ReactionType("Right", ReactType.ReCalculatingDocument), Description("Gets or sets right border of the format."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IBorder Right
        {
            get
            {
                return m_BorderRight;
            }
            set
            {
                if (m_BorderRight != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_BorderRight, value);
                    m_BorderRight = (Border)value;
                    OnRightChanged("Right", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the top border of the format.
        /// </summary>
        [Browsable(true), ReactionType("Top", ReactType.ReCalculatingDocument), Description("Gets or sets top border of the format."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IBorder Top
        {
            get
            {
                return m_BorderTop;
            }
            set
            {
                if (m_BorderTop != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_BorderTop, value);
                    m_BorderTop = (Border)value;
                    OnTopChanged("Top", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the bottom border of the format.
        /// </summary>
        [Browsable(true), ReactionType("Bottom", ReactType.ReCalculatingDocument), Description("Gets or sets bottom border of the format."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IBorder Bottom
        {
            get
            {
                return m_BorderBottom;
            }
            set
            {
                if (m_BorderBottom != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_BorderBottom, value);
                    m_BorderBottom = (Border)value;
                    OnBottomChanged("Bottom", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the cursor of the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), AmbientValue(null), ReactionType("Cursor", ReactType.ReChildrenFormatMerge), Description("Gets or sets cursor of the format.")]
        public Cursor Cursor
        {
            get
            {
                return m_cursor;
            }
            set
            {
                if (m_cursor != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_cursor, value);
                    m_cursor = value;
                    OnCursorChanged("Cursor", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the paddings for the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), ReactionType("Padding", ReactType.ReCalculatingDocument), Description("Gets or sets paddings for format.")]
        public Rectangle Padding
        {
            get
            {
                return m_Padding;
            }
            set
            {
                if (m_Padding != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_Padding, value);
                    m_Padding = value;
                    OnPaddingChanged("Padding", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the left padding for the format.
        /// </summary>
        [Browsable(false), ReactionType("Padding", ReactType.ReCalculatingDocument), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PaddingLeft
        {
            get
            {
                return m_Padding.Left;
            }
            set
            {
                if (m_Padding.Left != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_Padding, value);
                    m_Padding.X = value;
                    OnPaddingChanged("Padding", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the top padding for the format.
        /// </summary>
        [Browsable(false), ReactionType("Padding", ReactType.ReCalculatingDocument), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PaddingTop
        {
            get
            {
                return m_Padding.Top;
            }
            set
            {
                if (m_Padding.Top != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_Padding, value);
                    m_Padding.Y = value;
                    OnPaddingChanged("Padding", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the right padding for the format.
        /// </summary>
        [Browsable(false), ReactionType("Padding", ReactType.ReCalculatingDocument), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PaddingRight
        {
            get
            {
                return m_Padding.Width;
            }
            set
            {
                if (m_Padding.Width != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_Padding, value);
                    m_Padding.Width = value;
                    OnPaddingChanged("Padding", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the bottom padding for the format.
        /// </summary>
        [Browsable(false), ReactionType("Padding", ReactType.ReCalculatingDocument), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PaddingBottom
        {
            get
            {
                return m_Padding.Height;
            }
            set
            {
                if (m_Padding.Height != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_Padding, value);
                    m_Padding.Height = value;
                    OnPaddingChanged("Padding", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the element region.
        /// </summary>
        [Category("Appearance"), Browsable(true), ReactionType("Width", ReactType.ReCalculatingDocument), Description("Gets or sets width of the element region.")]
        public int Width
        {
            get
            {
                return m_width;
            }
            set
            {
                if (m_width != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_width, value);
                    m_width = value;
                    OnWidthChanged("Width", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the element region.
        /// </summary>
        [Category("Appearance"), Browsable(true), ReactionType("Height", ReactType.ReCalculatingDocument), Description("Gets or sets height of the element region.")]
        public int Height
        {
            get
            {
                return m_height;
            }
            set
            {
                if (m_height != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_height, value);
                    m_height = value;
                    OnHeightChanged("Height", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the background image of the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), DefaultValue(null), ReactionType("BackgroundImage", ReactType.RePaintElement), Description("Gets or sets background image of the format.")]
        public Bitmap BackgroundImage
        {
            get
            {
                return m_bgImage;
            }
            set
            {
                if (m_bgImage != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_bgImage, value);
                    m_bgImage = value;
                    OnBackgroundImageChanged("BackgroundImage", args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the background repeat property of the format.
        /// </summary>
        [Category("Appearance"), Browsable(true), DefaultValue(RepeatStyle.Repeat), ReactionType("BackgroundImageRepeat", ReactType.RePaintElement), Description("Gets or sets background repeat property of the format.")]
        public RepeatStyle BackgroundImageRepeat
        {
            get
            {
                return m_bgRepeat;
            }
            set
            {
                if (m_bgRepeat != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_bgRepeat, value);
                    m_bgRepeat = value;
                    OnBackgroundImageRepeatChanged("BackgroundImageRepeat", args);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the instance is already disposed.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDisposed
        {
            get
            {
                return m_bDisposed;
            }
        }

        /// <summary>
        /// Gets or sets the type of the width attribute.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeTypeEx WidthType
        {
            get
            {
                return m_widthType;
            }
            set
            {
                if (m_widthType != value)
                {
                    m_widthType = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the height attribute.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeTypeEx HeightType
        {
            get
            {
                return m_heightType;
            }
            set
            {
                if (m_heightType != value)
                {
                    m_heightType = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the merge mask to be used for merge style operation. This cannot be
        /// modified by the user directly.
        /// </summary>
        internal MergeMask Merge
        {
            get
            {
                return m_enMerge;
            }
            set
            {
                m_enMerge = value;
            }
        }

        /// <summary>
        /// Gets or sets the XML storage of the current format.
        /// </summary>
        internal XmlElement Storage
        {
            get
            {
                return m_storage;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Storage");

                if (m_storage != value)
                {
                    m_storage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the format should be disposed when the
        /// parent HTML element is being disposed.
        /// </summary>
        protected internal bool DisposeWithElement
        {
            get
            {
                return m_bDisposeWithElement;
            }
            set
            {
                if (m_bDisposeWithElement != value)
                {
                    m_bDisposeWithElement = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the width of the format has default values.
        /// </summary>
        protected internal bool IsWidthDefault
        {
            get
            {
                return this.Width == -1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the height of the format has default values.
        /// </summary>
        protected internal bool IsHeightDefault
        {
            get
            {
                return this.Height == -1;
            }
        }

        /// <summary>
        /// Gets the left space to the content.
        /// </summary>
        protected internal int LeftSpace
        {
            get
            {
                return this.Left.Width + this.PaddingLeft;
            }
        }

        /// <summary>
        /// Gets the top space to the content.
        /// </summary>
        protected internal int TopSpace
        {
            get
            {
                return this.Top.Width + this.PaddingTop;
            }
        }

        /// <summary>
        /// Gets the right space to the content.
        /// </summary>
        protected internal int RightSpace
        {
            get
            {
                return this.Right.Width + this.PaddingRight;
            }
        }

        /// <summary>
        /// Gets the bottom space to the content.
        /// </summary>
        protected internal int BottomSpace
        {
            get
            {
                return this.Bottom.Width + this.PaddingBottom;
            }
        }

        /// <summary>
        /// Gets or sets the FontStyle property of the format.
        /// </summary>
        protected internal FontStyle FontStyle
        {
            get
            {
                return m_fontStyle;
            }
            set
            {
                if (m_fontStyle != value)
                {
                    m_fontStyle = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the FontWeight property of the format.
        /// </summary>
        protected internal FontStyle FontWeight
        {
            get
            {
                return m_fontWeight;
            }
            set
            {
                if (m_fontWeight != value)
                {
                    m_fontWeight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the TextDecoration property of the format.
        /// </summary>
        protected internal FontStyle TextDecoration
        {
            get
            {
                return m_textDecoration;
            }
            set
            {
                if (m_textDecoration != value)
                {
                    m_textDecoration = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the family of the font for the current format.
        /// </summary>
        protected internal string FontFamilyName
        {
            get
            {
                return m_fontFamily;
            }
            set
            {
                if (m_fontFamily != value)
                {
                    m_fontFamily = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the font for the current format.
        /// </summary>
        protected internal float FontSize
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                if (value != m_fontSize)
                {
                    m_fontSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the graphical unit for the font size.
        /// </summary>
        protected internal GraphicsUnit Unit
        {
            get
            {
                return m_unit;
            }
            set
            {
                if (m_unit != value)
                {
                    m_unit = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the font has been created after merging the format.
        /// </summary>
        protected internal bool IsFontCreated
        {
            get
            {
                return m_bIsFontCreated;
            }
            set
            {
                if (m_bIsFontCreated != value)
                {
                    m_bIsFontCreated = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the format is already merged.
        /// </summary>
        protected internal bool IsMerged
        {
            get
            {
                return m_bIsMerged;
            }
            set
            {
                if (m_bIsMerged != value)
                {
                    m_bIsMerged = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the format is in quiet mode, skip event.
        /// </summary>
        protected internal bool QuietMode
        {
            get
            {
                return m_bSkipEvents;
            }
            set
            {
                if (value != m_bSkipEvents)
                {
                    m_bSkipEvents = value;
                    OnQuietModeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 'display' attribute was set to none.
        /// </summary>
        protected internal bool DisplayNone
        {
            get
            {
                return m_bDisplayNone;
            }
            set
            {
                if (m_bDisplayNone != value)
                {
                    m_bDisplayNone = value;
                }
            }
        }
        #endregion

        #region Class Events
        /// <summary>
        /// Delegate. Raised when quiet mode property is changed.
        /// </summary>
        public event EventHandler QuietModeChanged;

        /// <summary>
        /// Utility event. Raised on property change.
        /// </summary>
        public event BeforeValueChangeEventHandler OnChanged;

        /// <summary>
        /// Utility event. Raised on FormatParent property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler ParentChanged;

        /// <summary>
        /// Utility event. Raised on Name property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler NameChanged;

        /// <summary>
        /// Utility event. Raised on Font property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler FontChanged;

        /// <summary>
        /// Utility event. Raised on ForeColor property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler ForeColorChanged;

        /// <summary>
        /// Utility event. Raised on Background property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler BackgroundColorChanged;

        /// <summary>
        /// Utility event. Raised on Background image property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler BackgroundImageChanged;

        /// <summary>
        /// Utility event. Raised on Background repeat property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler BackgroundImageRepeatChanged;

        /// <summary>
        /// Utility event. Raised on VerticalAlign property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler VerticalAlignChanged;

        /// <summary>
        /// Utility event. Raised on HorizontalAlign property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler HorizontalAlignChanged;

        /// <summary>
        /// Utility event. Raised on Left property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler LeftChanged;

        /// <summary>
        /// Utility event. Raised on Right property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler RightChanged;

        /// <summary>
        /// Utility event. Raised on Top property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler TopChanged;

        /// <summary>
        /// Utility event. Raised on Bottom property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler BottomChanged;

        /// <summary>
        /// Utility event. Raised on Cursor property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler CursorChanged;

        /// <summary>
        /// Utility event. Raised on Padding property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler PaddingChanged;

        /// <summary>
        /// Utility event. Raised on Width property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler WidthChanged;

        /// <summary>
        /// Utility event. Raised on Height property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler HeightChanged;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the HTMLFormat class
        /// </summary>
        internal HTMLFormat()
        {
            m_bSkipEvents = true;
            this.Font = Control.DefaultFont.Clone() as Font;
            this.ForeColor = Control.DefaultForeColor;
            this.BackgroundColor = Control.DefaultBackColor;
            this.VerticalAlign = StringAlignment.Near;
            this.HorizontalAlign = StringAlignment.Near;

            this.Left = new Border();
            this.Top = new Border();
            this.Right = new Border();
            this.Bottom = new Border();
            this.Cursor = Cursors.Default;
            this.Padding = Rectangle.Empty;

            m_enMerge = MergeMask.None;
            m_widthType = SizeTypeEx.Unknown;
            m_heightType = SizeTypeEx.Unknown;

            m_fontStyle = FontStyle.Regular;
            m_fontWeight = FontStyle.Regular;
            m_textDecoration = FontStyle.Regular;

            m_fontFamily = this.Font.FontFamily.Name;
            m_fontSize = this.Font.Size;
            m_unit = this.Font.Unit;
            m_bgRepeat = RepeatStyle.Repeat;

            m_bIsMerged = true;
            m_bIsFontCreated = false;
            m_bSkipEvents = false;
        }

        /// <summary>
        /// Initializes a new instance of the HTMLFormat class
        /// </summary>
        /// <param name="name">Name of the format.</param>
        public HTMLFormat(string name)
            : this()
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");
            m_Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the HTMLFormat class
        /// </summary>
        /// <param name="name">Name of the format.</param>
        /// <param name="index">Index of the format.</param>
        public HTMLFormat(string name, long index)
            : this(name)
        {
            if (index < 0)
                throw new ArgumentException("index must be greater of zero");

            m_index = index;
        }

        /// <summary>
        /// Initializes a new instance of the HTMLFormat class
        /// </summary>
        /// <param name="name">string name value</param>
        /// <param name="index">index value</param>
        /// <param name="parent">IHTMLFormat instance</param>
        public HTMLFormat(string name, long index, IHTMLFormat parent)
            : this(name, index)
        {
            m_FormatParent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the HTMLFormat class
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="ctxt">Serialization context.</param>
        protected HTMLFormat(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();

            while (enumerator.MoveNext())
            {
                string name = enumerator.Name;
                object value = enumerator.Value;
                Type returnType = enumerator.ObjectType;

                Type type = GetType();
                PropertyInfo property = type.GetProperty(name, returnType);

                if (property != null)
                {
                    property.SetValue(this, value, null);
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the HTMLFormat class
        /// </summary>
        ~HTMLFormat()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes objects and frees its resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                if (m_Font != null)
                {
                    if (m_Font != Control.DefaultFont) m_Font.Dispose();
                    m_Font = null;
                }

                if (m_cursor != null)
                {
                    //// m_cursor.Dispose();
                    m_cursor = null;
                }

                if (m_bgImage != null)
                {
                    m_bgImage = null;
                }

                m_bDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Overridden. Returns the string representation of the object.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            /*
            StringBuilder output = new StringBuilder();

            output.Append( this.Storage.Attributes[ "name" ].Value );
            output.Append( " {" );
            XmlNodeList nodeList = this.Storage.ChildNodes;
            foreach( XmlNode node in nodeList )
            {
              if( node is XmlElement )
              {
                output.Append( node.Attributes[ "name" ].Value + ":" );
                output.Append( node.InnerText + "; " );
              }
            }

            output.Append( "}" );
            */

            StringBuilder output = new StringBuilder();
            output.Append("Format name: " + this.Name);
            output.Append("  Type: " + this.Type);
            output.Append("  ForeColor: " + this.ForeColor);
            output.Append("  BgColor: " + this.BackgroundColor);
            output.Append("  Horizontal Align: " + this.HorizontalAlign);
            output.Append("  Vertical Align: " + this.VerticalAlign);

            output.Append("  Left Border: " + this.Left.Style + " " +
              this.Left.Width + " " + this.Left.Color);

            output.Append("  Top Border: " + this.Top.Style + " " +
              this.Top.Width + " " + this.Top.Color);

            output.Append("  Right Border: " + this.Right.Style + " " +
              this.Right.Width + " " + this.Right.Color);

            output.Append("  Bottom Border: " + this.Bottom.Style + " " +
              this.Bottom.Width + " " + this.Bottom.Color);

            output.Append("  Cursor: " + this.Cursor.ToString());
            output.Append("  Padding: " + this.Padding);
            output.Append("  Width: " + this.Width);
            output.Append("  Height: " + this.Height);

            return output.ToString();
        }

        /// <summary>
        /// Serializes object.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Serialization context.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Font", Font);
            info.AddValue("ForeColor", ForeColor);
            info.AddValue("BackgroundColor", BackgroundColor);
            info.AddValue("VerticalAlign", VerticalAlign);
            info.AddValue("HorizontalAlign", HorizontalAlign);
            info.AddValue("Left", Left);
            info.AddValue("Top", Top);
            info.AddValue("Right", Right);
            info.AddValue("Bottom", Bottom);
            info.AddValue("Cursor", Cursor);
            info.AddValue("Padding", Padding);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
            info.AddValue("BackgroundImage", BackgroundImage);
            info.AddValue("Name", Name);
            info.AddValue("FormatType", Type);
            info.AddValue("BackgroundImageRepeat", BackgroundImageRepeat);
        }
        #endregion

        #region Class Event Raisers
        /// <summary>
        /// Raises event when mode changes.
        /// </summary>
        protected void RaiseQuietModeChangedEvent()
        {
            if (QuietModeChanged != null)
            {
                QuietModeChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises event if any format property has been changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseOnChanged(string name, ValueChangedEventArgs args)
        {
            if (OnChanged != null && !this.QuietMode && this.IsMerged)
            {
                BeforeValueChangedEventArgs arg1 = new BeforeValueChangedEventArgs(
                  name, args);

                OnChanged(this, arg1);
            }
        }

        /// <summary>
        /// Raises ParentChanged event if parent property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseParentChanged(string name, ValueChangedEventArgs args)
        {
            if (ParentChanged != null && !this.QuietMode)
            {
                ParentChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises NameChanged event if name property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseNameChanged(string name, ValueChangedEventArgs args)
        {
            if (NameChanged != null && !this.QuietMode)
            {
                NameChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises FontChanged event if font property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseFontChanged(string name, ValueChangedEventArgs args)
        {
            if (FontChanged != null && !this.QuietMode)
            {
                FontChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises ForeColorChanged event if ForeColor property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseForeColorChanged(string name, ValueChangedEventArgs args)
        {
            if (ForeColorChanged != null && !this.QuietMode)
            {
                ForeColorChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises BackgroundColorChanged event if BackgroundColor property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseBackgroundColorChanged(string name, ValueChangedEventArgs args)
        {
            if (BackgroundColorChanged != null && !this.QuietMode)
            {
                BackgroundColorChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises BackgroundImageChanged event if BackgroundImage
        /// property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseBackgroundImageChanged(string name, ValueChangedEventArgs args)
        {
            if (BackgroundImageChanged != null && !this.QuietMode)
            {
                BackgroundImageChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises BackgroundImageTypeChanged event if BackgroundImageType
        /// property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseBackgroundImageRepeatChanged(string name, ValueChangedEventArgs args)
        {
            if (BackgroundImageRepeatChanged != null && !this.QuietMode)
            {
                BackgroundImageRepeatChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises VerticalAlignChanged event if VerticalAlign property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseVerticalAlignChanged(string name, ValueChangedEventArgs args)
        {
            if (VerticalAlignChanged != null && !this.QuietMode)
            {
                VerticalAlignChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises HorizontalAlignChanged event if HorizontalAlign property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseHorizontalAlignChanged(string name, ValueChangedEventArgs args)
        {
            if (HorizontalAlignChanged != null && !this.QuietMode)
            {
                HorizontalAlignChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises LeftChanged event if Left property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseLeftChanged(string name, ValueChangedEventArgs args)
        {
            if (LeftChanged != null && !this.QuietMode)
            {
                LeftChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises RightChanged event if Right property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseRightChanged(string name, ValueChangedEventArgs args)
        {
            if (RightChanged != null && !this.QuietMode)
            {
                RightChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises TopChanged event if Top property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseTopChanged(string name, ValueChangedEventArgs args)
        {
            if (TopChanged != null && !this.QuietMode)
            {
                TopChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises BottomChanged event if Bottom property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseBottomChanged(string name, ValueChangedEventArgs args)
        {
            if (BottomChanged != null && !this.QuietMode)
            {
                BottomChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises CursorChanged event if Cursor property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseCursorChanged(string name, ValueChangedEventArgs args)
        {
            if (CursorChanged != null && !this.QuietMode)
            {
                CursorChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises PaddingChanged event if Padding property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaisePaddingChanged(string name, ValueChangedEventArgs args)
        {
            if (PaddingChanged != null && !this.QuietMode)
            {
                PaddingChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises WidthChanged event if Width property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseWidthChanged(string name, ValueChangedEventArgs args)
        {
            if (WidthChanged != null && !this.QuietMode)
            {
                WidthChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }

        /// <summary>
        /// Raises HeightChanged event if Height property value is changed.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseHeightChanged(string name, ValueChangedEventArgs args)
        {
            if (HeightChanged != null && !this.QuietMode)
            {
                HeightChanged(this, args);
            }

            RaiseOnChanged(name, args);
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Raised when Quiet Mode property is changed.
        /// </summary>
        protected virtual void OnQuietModeChanged()
        {
            RaiseQuietModeChangedEvent();
        }

        /// <summary>
        /// Method called by Parent property set part. This is the best place for
        /// any logic which must control the Parent property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnParentChanged(string name, ValueChangedEventArgs args)
        {
            RaiseParentChanged(name, args);
        }

        /// <summary>
        /// Method called by Name property set part. This is the best place for
        /// any logic which must control the Name property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnNameChanged(string name, ValueChangedEventArgs args)
        {
            RaiseNameChanged(name, args);
        }

        /// <summary>
        /// Method called by Font property set part. This is the best place for
        /// any logic which must control the Font property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnFontChanged(string name, ValueChangedEventArgs args)
        {
            RaiseFontChanged(name, args);
        }

        /// <summary>
        /// Method called by ForeColor property set part. This is the best place for
        /// any logic which must control the ForeColor property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnForeColorChanged(string name, ValueChangedEventArgs args)
        {
            RaiseForeColorChanged(name, args);
        }

        /// <summary>
        /// Method called by BackgroundColor property set part. This is the best place for
        /// any logic which must control the BackgroundColor property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnBackgroundColorChanged(string name, ValueChangedEventArgs args)
        {
            RaiseBackgroundColorChanged(name, args);
        }

        /// <summary>
        /// Method called by BackgroundImage property set part. This is the best place for
        /// any logic which must control the BackgroundImage property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnBackgroundImageChanged(string name, ValueChangedEventArgs args)
        {
            RaiseBackgroundImageChanged(name, args);
        }

        /// <summary>
        /// Method called by BackgroundImageRepeat property set part. This is the best place for
        /// any logic which must control the BackgroundImageRepeat property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnBackgroundImageRepeatChanged(string name, ValueChangedEventArgs args)
        {
            RaiseBackgroundImageRepeatChanged(name, args);
        }

        /// <summary>
        /// Method called by VerticalAlign property set part. This is the best place for
        /// any logic which must control the VerticalAlign property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnVerticalAlignChanged(string name, ValueChangedEventArgs args)
        {
            RaiseVerticalAlignChanged(name, args);
        }

        /// <summary>
        /// Method called by HorizontalAlign property set part. This is the best place for
        /// any logic which must control the HorizontalAlign property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnHorizontalAlignChanged(string name, ValueChangedEventArgs args)
        {
            RaiseHorizontalAlignChanged(name, args);
        }

        /// <summary>
        /// Method called by Left property set part. This is the best place for
        /// any logic which must control the Left property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnLeftChanged(string name, ValueChangedEventArgs args)
        {
            RaiseLeftChanged(name, args);
        }

        /// <summary>
        /// Method called by Right property set part. This is the best place for
        /// any logic which must control the Right property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnRightChanged(string name, ValueChangedEventArgs args)
        {
            RaiseRightChanged(name, args);
        }

        /// <summary>
        /// Method called by Top property set part. This is the best place for
        /// any logic which must control the Top property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnTopChanged(string name, ValueChangedEventArgs args)
        {
            RaiseTopChanged(name, args);
        }

        /// <summary>
        /// Method called by Bottom property set part. This is the best place for
        /// any logic which must control the Bottom property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnBottomChanged(string name, ValueChangedEventArgs args)
        {
            RaiseBottomChanged(name, args);
        }

        /// <summary>
        /// Method called by Cursor property set part. This is the best place for
        /// any logic which must control the Cursor property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnCursorChanged(string name, ValueChangedEventArgs args)
        {
            RaiseCursorChanged(name, args);
        }

        /// <summary>
        /// Method called by Padding property set part. This is the best place for
        /// any logic which must control the Padding property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnPaddingChanged(string name, ValueChangedEventArgs args)
        {
            RaisePaddingChanged(name, args);
        }

        /// <summary>
        /// Method called by Width property set part. This is the best place for
        /// any logic which must control the Width property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnWidthChanged(string name, ValueChangedEventArgs args)
        {
            RaiseWidthChanged(name, args);
        }

        /// <summary>
        /// Method called by Height property set part. This is thebest place for
        /// any logic which must control Height property changes (for inheritors).
        /// </summary>
        /// <param name="name">String name value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnHeightChanged(string name, ValueChangedEventArgs args)
        {
            RaiseHeightChanged(name, args);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Indicates whether the specified attribute is set.
        /// </summary>
        /// <param name="flag">Attribute type.</param>
        /// <returns>True if yes; otherwise False.</returns>
        private bool IsMergeFlagSet(MergeMask flag)
        {
            return (m_enMerge & flag) == flag;
        }

        /// <summary>
        /// Sets the font properties from the specified font.
        /// </summary>
        /// <param name="font">Font object.</param>
        private void SetFontPropertiesFrom(Font font)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            this.FontSize = font.Size;
            this.FontStyle = font.Italic ? FontStyle.Italic : FontStyle.Regular;
            this.FontWeight = font.Bold ? FontStyle.Bold : FontStyle.Regular;
            this.TextDecoration = font.Strikeout ? FontStyle.Strikeout :
              font.Underline ? FontStyle.Underline : FontStyle.Regular;
            this.FontFamilyName = font.FontFamily.Name;

            // Merge all font styles.
            MergeMask oldMask = this.Merge;
            this.Merge = MergeMask.FontAll;
            MergeFontStyleFinal(this, this);
            this.Merge = oldMask;
        }
        #endregion

        #region Merge methods
        /// <summary>
        /// Merges formats from an array of possible formats.
        /// </summary>
        /// <param name="formats">Array of formats.</param>
        /// <param name="inputFormat">First format object.</param>
        /// <param name="default">Default format object.</param>
        /// <returns>Format after merging.</returns>
        public static HTMLFormat MergeFormats(ArrayList formats, HTMLFormat inputFormat, HTMLFormat @default)
        {
            if (formats == null)
                throw new ArgumentNullException("formats");

            if (inputFormat == null)
                throw new ArgumentNullException("inputFormat");

            if (@default == null)
                throw new ArgumentNullException("default");

            HTMLFormat format = inputFormat.Clone();
            HTMLFormat defaultFormat = @default.Clone();

            format.IsMerged = false;
            format.QuietMode = true;
            format.IsFontCreated = false;

            defaultFormat.IsMerged = false;
            defaultFormat.QuietMode = true;
            defaultFormat.IsFontCreated = false;

            // Merge parent format and default format.
            format = MergeTwoFormats(defaultFormat, format);

            // Merge rest of formats.
            for (int index = 0; index < formats.Count; index++)
            {
                HTMLFormat nextFormat = formats[index] as HTMLFormat;
                format = MergeTwoFormats(format, nextFormat);
            }

            return format;
        }

        /// <summary>
        /// Merges two formats and returns the final format from them.
        /// </summary>
        /// <param name="oldFormat">First format object.</param>
        /// <param name="newFormat">Second format object.</param>
        /// <returns>Format after merging.</returns>
        public static HTMLFormat MergeTwoFormats(HTMLFormat oldFormat, HTMLFormat newFormat)
        {
            if (oldFormat == null)
                throw new ArgumentNullException("oldFormat");

            if (newFormat == null)
                throw new ArgumentNullException("newFormat");

            HTMLFormat finalFormat = oldFormat;

            MergeForeColor(finalFormat, newFormat);

            MergeBgColor(finalFormat, newFormat);

            MergeHAlign(finalFormat, newFormat);

            MergeVAlign(finalFormat, newFormat);

            MergeCursor(finalFormat, newFormat);

            MergePaddingLeft(finalFormat, newFormat);

            MergePaddingTop(finalFormat, newFormat);

            MergePaddingRight(finalFormat, newFormat);

            MergePaddingBottom(finalFormat, newFormat);

            MergeBorderStyle(finalFormat, newFormat);

            MergeBorderWidth(finalFormat, newFormat);

            MergeBorderColor(finalFormat, newFormat);

            MergeFontFamily(finalFormat, newFormat);

            MergeFontStyle(finalFormat, newFormat);

            MergeFontWeight(finalFormat, newFormat);

            MergeTextDecoration(finalFormat, newFormat);

            MergeFontStyleFinal(finalFormat, newFormat);

            MergeFontSize(finalFormat, newFormat);

            MergeWidth(finalFormat, newFormat);

            MergeHeight(finalFormat, newFormat);

            MergeBgImage(finalFormat, newFormat);

            MergeBgRepeat(finalFormat, newFormat);

            MergeDisplay(finalFormat, newFormat);

            return finalFormat;
        }

        /// <summary>
        /// Merges the forecolor of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeForeColor(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.ForeColor))
            {
                finalFormat.ForeColor = newFormat.ForeColor;
            }
        }

        /// <summary>
        /// Merges the background color of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeBgColor(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.BgColor))
            {
                finalFormat.BackgroundColor = newFormat.BackgroundColor;
            }
        }

        /// <summary>
        /// Merges the horizontal align of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeHAlign(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.HAlignment))
            {
                finalFormat.HorizontalAlign = newFormat.HorizontalAlign;
            }
        }

        /// <summary>
        /// Merges the vertical align of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeVAlign(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.VAlignmnet))
            {
                finalFormat.VerticalAlign = newFormat.VerticalAlign;
            }
        }

        /// <summary>
        /// Merges the cursor of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeCursor(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.Cursor))
            {
                finalFormat.Cursor = newFormat.Cursor;
            }
        }

        /// <summary>
        /// Merges the padding of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergePaddingAll(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.PaddingAll))
            {
                finalFormat.Padding = newFormat.Padding;
            }
        }

        /// <summary>
        /// Merges the left padding of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergePaddingLeft(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.PaddingLeft))
            {
                finalFormat.PaddingLeft = newFormat.PaddingLeft;
            }
        }

        /// <summary>
        /// Merges the top padding of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergePaddingTop(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.PaddingTop))
            {
                finalFormat.PaddingTop = newFormat.PaddingTop;
            }
        }

        /// <summary>
        /// Merges the right padding of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergePaddingRight(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.PaddingRight))
            {
                finalFormat.PaddingRight = newFormat.PaddingRight;
            }
        }

        /// <summary>
        /// Merges the bottom padding of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergePaddingBottom(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.PaddingBottom))
            {
                finalFormat.PaddingBottom = newFormat.PaddingBottom;
            }
        }

        /// <summary>
        /// Merges the border style of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeBorderStyle(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.BorderStyle))
            {
                if (newFormat.IsMergeFlagSet(MergeMask.BorderLeft))
                {
                    finalFormat.Left.Style = newFormat.Left.Style;
                }

                if (newFormat.IsMergeFlagSet(MergeMask.BorderTop))
                {
                    finalFormat.Top.Style = newFormat.Top.Style;
                }

                if (newFormat.IsMergeFlagSet(MergeMask.BorderRight))
                {
                    finalFormat.Right.Style = newFormat.Right.Style;
                }

                if (newFormat.IsMergeFlagSet(MergeMask.BorderBottom))
                {
                    finalFormat.Bottom.Style = newFormat.Bottom.Style;
                }
            }
        }

        /// <summary>
        /// Merges the border width of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeBorderWidth(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.BorderWidth))
            {
                if (newFormat.IsMergeFlagSet(MergeMask.BorderLeft))
                {
                    finalFormat.Left.Width = newFormat.Left.Width;
                }

                if (newFormat.IsMergeFlagSet(MergeMask.BorderTop))
                {
                    finalFormat.Top.Width = newFormat.Top.Width;
                }

                if (newFormat.IsMergeFlagSet(MergeMask.BorderRight))
                {
                    finalFormat.Right.Width = newFormat.Right.Width;
                }

                if (newFormat.IsMergeFlagSet(MergeMask.BorderBottom))
                {
                    finalFormat.Bottom.Width = newFormat.Bottom.Width;
                }
            }
        }

        /// <summary>
        /// Merges the border color of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeBorderColor(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.BorderColor))
            {
                if (newFormat.IsMergeFlagSet(MergeMask.BorderLeft))
                {
                    finalFormat.Left.Color = newFormat.Left.Color;
                }
                if (newFormat.IsMergeFlagSet(MergeMask.BorderTop))
                {
                    finalFormat.Top.Color = newFormat.Top.Color;
                }
                if (newFormat.IsMergeFlagSet(MergeMask.BorderRight))
                {
                    finalFormat.Right.Color = newFormat.Right.Color;
                }
                if (newFormat.IsMergeFlagSet(MergeMask.BorderBottom))
                {
                    finalFormat.Bottom.Color = newFormat.Bottom.Color;
                }
            }
        }

        /// <summary>
        /// Merges the font family of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeFontFamily(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.FontFamily))
            {
                finalFormat.FontFamilyName = newFormat.FontFamilyName;

                finalFormat.Merge |= MergeMask.FontFamily;
            }
        }

        /// <summary>
        /// Merges the font style of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeFontStyle(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.FontStyle))
            {
                finalFormat.FontStyle = newFormat.FontStyle;
            }
        }

        /// <summary>
        /// Merges the font weight of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeFontWeight(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.FontWeight))
            {
                finalFormat.FontWeight = newFormat.FontWeight;
            }
        }

        /// <summary>
        /// Merges the text decoration of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeTextDecoration(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.TextDecoration))
            {
                finalFormat.TextDecoration = newFormat.TextDecoration;
            }
        }

        /// <summary>
        /// Merges the final font style of formats.
        /// </summary>
        /// <param name="finalFormat">Format for merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeFontStyleFinal(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            FontStyle newStyle = finalFormat.FontStyle;

            if (newFormat.IsMergeFlagSet(MergeMask.FontStyle))
            {
                newStyle |= finalFormat.FontStyle;
                if (finalFormat.FontStyle == FontStyle.Regular)
                {
                    newStyle &= ~FontStyle.Italic;
                }
            }

            if (newFormat.IsMergeFlagSet(MergeMask.FontWeight))
            {
                newStyle |= finalFormat.FontWeight;
                if (finalFormat.FontWeight == FontStyle.Regular)
                {
                    newStyle &= ~FontStyle.Bold;
                }
            }

            if (newFormat.IsMergeFlagSet(MergeMask.TextDecoration))
            {
                newStyle |= finalFormat.TextDecoration;
                if (finalFormat.TextDecoration == FontStyle.Regular)
                {
                    newStyle &= ~FontStyle.Strikeout;
                    newStyle &= ~FontStyle.Underline;
                }
            }

            finalFormat.FontStyle = newStyle;
        }

        /// <summary>
        /// Merges the font size of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeFontSize(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.FontSize))
            {
                finalFormat.FontSize = newFormat.FontSize;
                finalFormat.Unit = newFormat.Unit;
                finalFormat.Merge |= MergeMask.FontSize;
            }
        }

        /// <summary>
        /// Merges the width of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeWidth(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.Width))
            {
                finalFormat.Width = newFormat.Width;
                finalFormat.WidthType = newFormat.WidthType;
            }
        }

        /// <summary>
        /// Merges the height of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeHeight(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.Height))
            {
                finalFormat.Height = newFormat.Height;
                finalFormat.HeightType = newFormat.HeightType;
            }
        }

        /// <summary>
        /// Merges the background image of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeBgImage(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.BgImage))
            {
                finalFormat.BackgroundImage = newFormat.BackgroundImage;
            }
        }

        /// <summary>
        /// Merges the background repeat of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeBgRepeat(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            if (newFormat.IsMergeFlagSet(MergeMask.BgRepeat))
            {
                finalFormat.BackgroundImageRepeat = newFormat.BackgroundImageRepeat;
            }
        }

        /// <summary>
        /// Merges the display attribute of formats.
        /// </summary>
        /// <param name="finalFormat">Format after merging.</param>
        /// <param name="newFormat">New format object.</param>
        private static void MergeDisplay(HTMLFormat finalFormat, HTMLFormat newFormat)
        {
            finalFormat.DisplayNone = newFormat.DisplayNone;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        object ICloneable.Clone()
        {
            HTMLFormat format = (HTMLFormat)this.MemberwiseClone();
            format.QuietMode = true;

            if (format.Storage != null)
            {
                format.Storage = (XmlElement)this.Storage.Clone();
            }

            if (format.FontSize != 0 && format.Font != null)
            {
                format.m_Font = (Font)m_Font.Clone();
            }

            if (format.Left != null)
            {
                format.Left = this.Left.Clone();
            }

            if (format.Top != null)
            {
                format.Top = this.Top.Clone();
            }

            if (format.Right != null)
            {
                format.Right = this.Right.Clone();
            }

            if (format.Bottom != null)
            {
                format.Bottom = this.Bottom.Clone();
            }

            // Event handlers detaching.
            ClearEvents(format);

            format.QuietMode = false;

            return format;
        }

        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public HTMLFormat Clone()
        {
            return (HTMLFormat)((ICloneable)this).Clone();
        }

        /// <summary>
        /// Returns the format from parent element with inherited and not inherited options.
        /// </summary>
        /// <returns>New object with inherited cloned options.</returns>
        public HTMLFormat GetInheritedFormat()
        {
            HTMLFormat format = Clone();
            format.Merge = MergeMask.Inherit;

            return format;
        }

        /// <summary>
        /// Clears all events for the specified format object.
        /// </summary>
        /// <param name="format">New format object.</param>
        private void ClearEvents(HTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            if (format.QuietModeChanged != null)
            {
                format.QuietModeChanged = null;
            }

            if (format.OnChanged != null)
            {
                format.OnChanged = null;
            }

            if (format.ParentChanged != null)
            {
                format.ParentChanged = null;
            }

            if (format.NameChanged != null)
            {
                format.NameChanged = null;
            }

            if (format.FontChanged != null)
            {
                format.FontChanged = null;
            }

            if (format.ForeColorChanged != null)
            {
                format.ForeColorChanged = null;
            }

            if (format.BackgroundColorChanged != null)
            {
                format.BackgroundColorChanged = null;
            }

            if (format.BackgroundImageChanged != null)
            {
                format.BackgroundImageChanged = null;
            }

            if (format.BackgroundImageRepeatChanged != null)
            {
                format.BackgroundImageRepeatChanged = null;
            }
            if (format.VerticalAlignChanged != null)
            {
                format.VerticalAlignChanged = null;
            }

            if (format.HorizontalAlignChanged != null)
            {
                format.HorizontalAlignChanged = null;
            }

            if (format.LeftChanged != null)
            {
                format.LeftChanged = null;
            }

            if (format.RightChanged != null)
            {
                format.RightChanged = null;
            }

            if (format.TopChanged != null)
            {
                format.TopChanged = null;
            }

            if (format.BottomChanged != null)
            {
                format.BottomChanged = null;
            }

            if (format.CursorChanged != null)
            {
                format.CursorChanged = null;
            }
            if (format.PaddingChanged != null)
            {
                format.PaddingChanged = null;
            }

            if (format.WidthChanged != null)
            {
                format.WidthChanged = null;
            }

            if (format.HeightChanged != null)
            {
                format.HeightChanged = null;
            }
        }
        #endregion

        #region Class Serializable methods
        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeLeft()
        {
            return !(Left as Border).IsEmpty;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeTop()
        {
            return !(Top as Border).IsEmpty;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeRight()
        {
            return !(Right as Border).IsEmpty;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeBottom()
        {
            return !(Bottom as Border).IsEmpty;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeWidth()
        {
            return this.Width != -1;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeHeight()
        {
            return this.Height != -1;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeVerticalAlign()
        {
            return this.VerticalAlign != StringAlignment.Near;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeHorizontalAlign()
        {
            return this.HorizontalAlign != StringAlignment.Near;
        }

        /// <summary>
        /// Indicates whether the forecolor property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeForeColor()
        {
            return this.ForeColor != Color.Black;
        }

        /// <summary>
        /// Indicates whether the background color property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeBackgroundColor()
        {
            return this.BackgroundColor != Color.Empty;
        }

        /// <summary>
        /// Indicates whether the font property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeFont()
        {
            return this.Font != null &&
              (this.Font.Size != Control.DefaultFont.Size ||
              this.Font.Name != Control.DefaultFont.Name ||
              this.Font.Style != Control.DefaultFont.Style ||
              this.Font.Unit != Control.DefaultFont.Unit);
        }

        /// <summary>
        /// Serialization helper property. It indicates when the cursor property value
        /// must be serialized into code.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeCursor()
        {
            return this.Cursor != Cursors.Default;
        }

        /// <summary>
        /// Serialization helper property. It indicates when the padding
        /// property must be serialized into code.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializePadding()
        {
            return this.Padding != Rectangle.Empty;
        }

        /// <summary>
        /// Serialization helper property. It indicates when the BackgroundImage
        /// property must be serialized into code.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeBackgroundImage()
        {
            return BackgroundImage != null;
        }
        #endregion
    }
}
