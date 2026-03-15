#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// A label is a text object that is attached to a container and
	/// is positioned relative to some control point on the container.
	/// </summary>
	[ Serializable ]
	[ TypeConverter( typeof( ExpandableObjectConverter ) ) ]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
	public class Label
		: AnchoringPrimitive
	{
		#region Class members
		private LabelPropertyBinding m_LabelPropertyBinding;
		private string m_strText;
		private bool m_bReadOnly;
        private FillStyle m_styleFill;
		private FontStyle m_styleFont;
        private FillStyle m_styleBackground;
        private LineStyle m_styleLine;
        private bool m_bVisible = true;
        private string m_strName;
		[NonSerialized]
		private SizeF m_szSize;
        private bool m_sizeToNode;
        private bool m_UpdatePosition = false;
        private bool m_AdjustRotateAngle = false;
        private TextCases m_txtCase = TextCases.None;
        private string m_strOriginalText = string.Empty;
        private StringAlignment m_alignHorizontal;
        private StringAlignment m_alignVertical;
        private StringFormatFlags m_fmtStringFormat;
        private bool m_bInheritContainerSize;
        private LabelOrientation m_orientation = LabelOrientation.Auto;
		#endregion

		#region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
		public Label()
		{
			m_strText = string.Empty;
            m_UpdatePosition = false;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="strText">The label text.</param>
		public Label( Node container, string strText )
			: base( container )
		{
			if( container == null )
				throw new ArgumentNullException( "container" );

			m_strText = strText;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="src">The label source.</param>
		public Label( Label src )
			: base( src )
		{
			m_strText = src.m_strText;
            m_UpdatePosition = src.m_UpdatePosition;
            m_AdjustRotateAngle = src.m_AdjustRotateAngle;
            m_szSize = src.m_szSize;
            m_sizeToNode = src.m_sizeToNode;
		    m_LabelPropertyBinding = src.PropertyBinding;
            m_strName = src.m_strName;
            m_bInheritContainerSize = src.m_bInheritContainerSize;
			if( src.m_styleFont != null )
				m_styleFont = ( FontStyle )src.FontStyle.Clone();

            if (src.m_styleFill != null)
                m_styleFill = (FillStyle)src.m_styleFill.Clone();

            if (src.m_styleBackground != null)
                m_styleBackground = (FillStyle)src.m_styleBackground.Clone();

            if (src.m_styleLine != null)
                m_styleLine = (LineStyle)src.LineStyle.Clone();
            m_alignHorizontal = src.m_alignHorizontal;
            m_alignVertical = src.m_alignVertical;
            m_fmtStringFormat = src.m_fmtStringFormat;
            m_txtCase = src.m_txtCase;
            if (m_txtCase == TextCases.None)
                m_strOriginalText = m_strText;
            m_orientation = src.m_orientation;
            m_bVisible = src.m_bVisible;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Label(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_LabelPropertyBinding = LabelPropertyBinding.Text;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "text":
                        m_strText = info.GetString("text");
                        break;
                    case "name":
                        m_strName = info.GetString("name");
                        break;
                    case "styleFont":
                        m_styleFont = (FontStyle)info.GetValue("styleFont", typeof(FontStyle));
                        break;
                    case "propertybindnig":
                        m_LabelPropertyBinding = (LabelPropertyBinding)info.GetValue("propertybindnig", typeof(LabelPropertyBinding));
                        break;
                    case "fillStyle":
                        m_styleFill = (FillStyle)info.GetValue("fillStyle", typeof(FillStyle));
                        break;

                    case "backgroundStyle":
                        m_styleBackground = (FillStyle)info.GetValue("backgroundStyle", typeof(FillStyle));
                        break;
                    case "lineStyle":
                        m_styleLine = (LineStyle)info.GetValue("lineStyle", typeof(LineStyle));
                        break;
                    case "size":
                        m_szSize = (SizeF)info.GetValue("size", typeof(SizeF));
                        break;

                    case "sizeToNode":
                        m_sizeToNode = (bool)info.GetValue("sizeToNode", typeof(bool));
                        break;

                    case "updatePosition":
                        m_UpdatePosition = (bool)info.GetValue("updatePosition", typeof(bool));
                        break;

                    case "adjustRotateAngle":
                        m_AdjustRotateAngle = (bool)info.GetValue("adjustRotateAngle", typeof(bool));
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
                    case "textcase":
                        m_txtCase = (TextCases)info.GetValue("textcase", typeof(TextCases));
                        break;
                    case "inheritContainerSize":
                        m_bInheritContainerSize = info.GetBoolean("inheritContainerSize");
                        break;
                    case "orientation":
                        m_orientation = (LabelOrientation)info.GetValue("orientation", typeof(LabelOrientation));
                        break;
                    case "visible":
                        m_bVisible = (bool)info.GetValue("visible", typeof(bool));
                        break;
                }

            }
            if (m_txtCase == TextCases.None)
                m_strOriginalText = m_strText;
            this.FontStyle.UpdateServiceReferences(this);
        }
		#endregion

		#region Class properties
        /// <summary>
        /// Gets or sets the orientation of label.
        /// </summary>
        [DefaultValue(LabelOrientation.Auto)]
        [Category("Behavior")]
        [Description("Gets or sets the orientation of label text.")]
        public LabelOrientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                if (m_orientation != value && OnPropertyChanging(DPN.Orientation, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.Orientation);
                    m_orientation = value;

                    InvokeUpdateCallback();
                    // raise property changed event
                    OnPropertyChanged(DPN.Orientation);
                }
            }
        }

        /// <summary>
        /// Gets the line drawing properties for the label.
        /// </summary>
        /// <value>The line style.</value>
        /// <remarks>
        /// Gets the line style determines the configuration of the pen used to
        /// render line.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LineStyle"/>
        /// </remarks>
        [Browsable(true)]
        [TypeConverter(typeof(LineStyleConverter))]
        [Category("Appearance")]
        [Description("Properties of the pen used for drawing line.")]
        public LineStyle LineStyle
        {
            get
            {
                if (m_styleLine == null)
                {
                    m_styleLine = new LineStyle();
                    m_styleLine.LineColor = Color.Black;
                    m_styleLine.LineWidth = 0;
                }
                return m_styleLine;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether current instance inherit container size.
        /// </summary>
        /// <value>
        /// <c>true</c> if current instance inherit container size; otherwise, <c>false</c>.
        /// </value>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Indicates whether node update its size when node's container size changes.")]
        public bool InheritContainerSize
        {
            get
            {
                return m_bInheritContainerSize;
            }
            set
            {
                if (Position != Position.Custom)
                    throw new InvalidOperationException("InheritContainerSize can be set only when the label position is Custom.");
                if (value != m_bInheritContainerSize && OnPropertyChanging(DPN.InheritContainerSize, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.InheritContainerSize);
                    // set new value
                    m_bInheritContainerSize = value;
                    
                    // raise property changed event
                    OnPropertyChanged(DPN.InheritContainerSize);
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique label full name.
        /// </summary>
        /// <value>The unique label full name.</value>
        [Description("Label's full name")]
        public string FullName
        {
            get
            {
                if (this.Container == null)
                    return this.Name;
                else
                    return this.Container.FullName + "." + this.Name;
            }
        }
        /// <summary>
        /// Gets or sets the unique label name.
        /// </summary>
        /// <value>The unique label name.</value>
        [Description("Label's Point's name")]
        public string Name
        {
            get { return m_strName; }
            set
            {
                if (m_strName != value && OnPropertyChanging(DPN.Name, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.Name);
                    // set new value
                    m_strName = value;
                    // raise property changed event
                    OnPropertyChanged(DPN.Name);
                }
            }
        }
		/// <summary>
		/// Flag indicating if the text object is Read-only or not.
		/// </summary>
		[ Browsable( true ) ]
		[ Category( "Behavior" ) ]
		[ Description( "Flag indicating if the text object is Read-only or not." ) ]
		public bool ReadOnly
		{
			get{ return m_bReadOnly; }
			set
			{
				if( m_bReadOnly != value )
					m_bReadOnly = value;
			}
		}
        /// <summary>
        /// Specifies visibility of the label.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Specifies visibility of the label")]
        public bool Visible
        {
            get
            {
                return m_bVisible;
            }
            set
            {
                if (value != m_bVisible)
                    m_bVisible = value;
            }
        }
        /// <summary>
        /// Determines font color style.
        /// </summary>
        [ Browsable(true) ]
        [ TypeConverter(typeof(FillStyleConverter)) ]
        [ Category("Appearance")]
        [ Description("Determines font fill style.") ]
        public FillStyle FontColorStyle
        {
            get
            {
                if ( m_styleFill == null )
                {
                    m_styleFill = new FillStyle();
                    m_styleFill.Color = Color.Black;
                }

                return m_styleFill;
            }
        }
        /// <summary>
        /// Determines text background style.
        /// </summary>
        [ Browsable(true) ]
        [ TypeConverter(typeof(FillStyleConverter)) ]
        [ Category("Appearance") ]
        [ Description("Determines text background style.") ]
        public FillStyle BackgroundStyle
        {
            get
            {
                if ( m_styleBackground == null )
                {
                    m_styleBackground = new BackgroundStyle();
                    m_styleBackground.Color = Color.Transparent;
                    m_styleBackground.ColorAlphaFactor = 0;
                }

                return m_styleBackground;
            }
        }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
		[ Browsable( true ) ]
		[ Category( "General" ) ]
		[ Description( "Text value to be displayed." ) ]
        [ Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)) ]
		public string Text
		{
			get
			{
				string strToReturn = m_strText;
				
				if( this.PropertyBinding == LabelPropertyBinding.ContainerName && this.Container != null )
				{
					strToReturn = this.Container.Name;
				}
					
				return strToReturn;
			}
			set
			{
				if( !this.ReadOnly && m_strText != value && OnPropertyChanging( DPN.Text, value ) )
				{
					// make history record
					RecordPropertyChanged( DPN.Text );
					// set new value
					m_strText = value;
					// update Label text size
					
						
					InvokeUpdateCallback();
					// raise property changed event
					OnPropertyChanged( DPN.Text );
				}
			}
		}

        /// <summary>
        /// Gets the font style.
        /// </summary>
        /// <value>The font style.</value>
		[ Browsable( true ) ]
		[ Category( "General" ) ]
		[ Description( "Displayed text value to be ." ) ]
		public FontStyle FontStyle
		{
			get
			{
				if( m_styleFont == null )
				{
					m_styleFont = new FontStyle();
					m_styleFont.UpdateServiceReferences( this );
				}
				
				return m_styleFont;
			}
		}
		/// <summary>
		/// Binds the text value of the label to a property.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property allows the label to be attached to any property of the
		/// label. The default binding is to the "Text" property, which is a
		/// string property maintained by the label. Setting this property to
		/// "ContainerName" will cause the label to display the name of the
		/// container (i.e. symbol) that is hosting it.
		/// </para>
		/// </remarks>
		[ Browsable( true ) ]
		[ Category( "General" ), ]
		[ Description( "Binds the text value of the label to a property." ) ]
		public LabelPropertyBinding PropertyBinding
		{
			get{ return m_LabelPropertyBinding; }
			set
			{
				if( m_LabelPropertyBinding != value )
				{
					m_LabelPropertyBinding = value;
				}
			}
		}
		/// <summary>
		/// Gets the label text size.
		/// </summary>
		/// <value>The size.</value>
		[Browsable( true )]
		public SizeF Size
		{
            get
            {
                if (m_bInheritContainerSize)
                {
                    m_szSize = this.Container.Size;
                }
                else if (this.Text != string.Empty)
                {
                    // calculate label text size
                    using (Graphics gfx = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        // current label's bounding rect
                        Font fntTemp = this.FontStyle.CreateFont();
                        // calc label rect
                        if (SizeToNode)
                        {
                            if (WrapText)
                                m_szSize = gfx.MeasureString(this.Text, fntTemp, (int)this.Container.Size.Width, this.GetStringFormat());
                            else
                                m_szSize = gfx.MeasureString(this.Text, fntTemp);
                        }
                        else
                        {
                            SizeF size = SizeF.Empty;
                            if (WrapText)
                            {
                                if (DirectionVertical)
                                    size.Height = this.Container.Size.Height;
                                else
                                    size.Width = this.Container.Size.Width;
                            }
                            m_szSize = gfx.MeasureString(this.Text, fntTemp, size, this.GetStringFormat());
                        }
                    }
                }

                return m_szSize;
            }
            set
            {
                if (value != m_szSize)
                    m_szSize = value;
            }
		}
        /// <summary>
        /// Gets or Sets Label fit into Parent Node.
        /// </summary>
        [Browsable(true)]
        [Description("Set node size to label size. Can be true if node has only one label and label's position is in center.")]
        [DefaultValue ( false )]
        public bool SizeToNode
        {
            get
            {
                if (m_sizeToNode == true && this.Container != null)
                {
                    System.Reflection.PropertyInfo nodePI = this.Container.GetType().GetProperty("Labels");
                    if (nodePI != null)
                    {
                        LabelCollection lcLabels = nodePI.GetValue(this.Container, null) as LabelCollection;
                        if (lcLabels != null)
                        {
                            if (lcLabels.Count == 1 && lcLabels[0].Position == Position.Center)
                            {
                                if (this.WrapText)
                                {
                                    if (this.Container.Size.Height < m_szSize.Height && this.Container.Size.Height != 0)
                                    {
                                        this.Container.Size = new SizeF(this.Container.Size.Width, m_szSize.Height);
                                    }                                    
                                }
                                else
                                {
                                    if (this.Container.Size.Width < m_szSize.Width && this.Container.Size.Width != 0)
                                    {
                                        this.Container.Size = new SizeF(m_szSize.Width, this.Container.Size.Height);
                                    }
                                }
                                m_sizeToNode = true;
                            }
                        }
                    }
                }
                return m_sizeToNode;
            }
            set
            {
                m_sizeToNode = value;
                if (value == true && this.Container != null)
                {
                    System.Reflection.PropertyInfo nodePI = this.Container.GetType().GetProperty("Labels");
                    if (nodePI != null)
                    {
                        LabelCollection lcLabels = nodePI.GetValue(this.Container, null) as LabelCollection;
                        if (lcLabels != null)
                        {
                            if (lcLabels.Count > 1 || lcLabels[0].Position != Position.Center)
                            {
                                m_sizeToNode = false;                           
                                throw new InvalidOperationException("SizeToNode can be set only when the label position is Center.");
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets or Sets whether default positioning has to be used.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or Sets whether default positioning has to be used.")]
        [DefaultValue(false)]
        public bool UpdatePosition
        {
            get
            {
                return m_UpdatePosition;
            }
            set
            {
                if (m_UpdatePosition != value && OnPropertyChanging(DPN.UpdatePosition, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.UpdatePosition);
                    m_UpdatePosition = value;
                    InvokeUpdateCallback();
                    // raise property changed event
                    OnPropertyChanged(DPN.UpdatePosition);
                }
            }
        }
        /// <summary>
        /// When set will prevent the label from being inverted.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or Sets whether the label should remain horizontal on rotation of node.")]
        [DefaultValue(false)]
        public bool AdjustRotateAngle 
        {
            get
            {
                return m_AdjustRotateAngle;
            }
            set
            {
                if (m_AdjustRotateAngle != value && OnPropertyChanging(DPN.AdjustRotateAngle, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.AdjustRotateAngle);
                    m_AdjustRotateAngle = value;
                    InvokeUpdateCallback();
                    // raise property changed event
                    OnPropertyChanged(DPN.AdjustRotateAngle);
                }
            }
        }

        /// <summary>
        /// Gets or sets case of text in the label
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
                if (value != m_txtCase && OnPropertyChanging(DPN.TextCase, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.TextCase);

                    // set new value
                    m_txtCase = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.TextCase);
                    UpdateText();
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
                if (m_alignHorizontal != value && OnPropertyChanging(DPN.HorizontalAlignment, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.HorizontalAlignment);

                    // set new value
                    m_alignHorizontal = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.HorizontalAlignment);
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
                if (m_alignVertical != value && OnPropertyChanging(DPN.VerticalAlignment, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.VerticalAlignment);

                    // set new value
                    m_alignVertical = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.VerticalAlignment);
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
                if (this.WrapText != value && OnPropertyChanging(DPN.WrapText, value))
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
                    OnPropertyChanged(DPN.WrapText);
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
                if (this.DirectionRightToLeft != value && OnPropertyChanging(DPN.DirectionRightToLeft, value))
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
                    OnPropertyChanged(DPN.DirectionRightToLeft);
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
                if (this.DirectionVertical != value && OnPropertyChanging(DPN.DirectionVertical, value))
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
                    OnPropertyChanged(DPN.DirectionVertical);
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
                if (this.FitBlackBox != value && OnPropertyChanging(DPN.FitBlackBox, value))
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
                    OnPropertyChanged(DPN.FitBlackBox);
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
                if (this.LineLimit != value && OnPropertyChanging(DPN.LineLimit, value))
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
                    OnPropertyChanged(DPN.LineLimit);
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
                if (this.MeasureTrailingSpaces != value && OnPropertyChanging(DPN.MeasureTrailingSpaces, value))
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
                    OnPropertyChanged(DPN.MeasureTrailingSpaces);
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
                if (this.NoClip != value && OnPropertyChanging(DPN.NoClip, value))
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
                    OnPropertyChanged(DPN.NoClip);
                }
            }
        }
		#endregion

		#region Class overrides
        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns></returns>
		protected override string GetPropertyContainerName()
		{
			return null;
		}

        /// <summary>
        /// Renders the primitive object to specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
		protected override void Render( Graphics gfx )
		{
            if (!this.Visible)
                return;
			// calc position
			PointF ptPrimitivePosition = GetPosition();

            using (Font font = this.FontStyle.CreateFont())
			{
				// get string bounding rectangle
                SizeF szStringSize = Size;
                PointF ptRenderringOrigin = ptPrimitivePosition;

                if (m_UpdatePosition == false)
                {
                    switch (Position)
                    {
                        case Position.BottomCenter:
                            ptRenderringOrigin = new PointF(ptPrimitivePosition.X - szStringSize.Width / 2, ptPrimitivePosition.Y);
                            break;
                        case Position.Center:
                            ptRenderringOrigin = new PointF(ptPrimitivePosition.X - szStringSize.Width / 2, ptPrimitivePosition.Y - szStringSize.Height / 2);
                            break;
                        case Position.TopCenter:
                            ptRenderringOrigin = new PointF(ptPrimitivePosition.X - szStringSize.Width / 2, ptPrimitivePosition.Y - szStringSize.Height);
                            break;
                        case Position.TopLeft:
                            ptRenderringOrigin = new PointF(ptPrimitivePosition.X, ptPrimitivePosition.Y - szStringSize.Height);
                            break;
                        case Position.MiddleLeft:
                            ptRenderringOrigin = new PointF(ptPrimitivePosition.X - szStringSize.Width, ptPrimitivePosition.Y - szStringSize.Height / 2);
                            break;
                        case Position.MiddleRight:
                            ptRenderringOrigin = new PointF(ptPrimitivePosition.X, ptPrimitivePosition.Y - szStringSize.Height / 2);
                            break;
                        case Position.TopRight:
                            ptRenderringOrigin = new PointF(ptPrimitivePosition.X - szStringSize.Width, ptPrimitivePosition.Y - szStringSize.Height);
                            break;
                        case Position.BottomRight:
                            ptRenderringOrigin = new PointF(ptPrimitivePosition.X - szStringSize.Width, ptPrimitivePosition.Y);
                            break;
                    }
                }
                
                RectangleF rectBounds = new RectangleF( ptRenderringOrigin.X, ptRenderringOrigin.Y, szStringSize.Width, szStringSize.Height);//new RectangleF(new PointF(0, 0), szSizeUnitIndependent);

                GraphicsState save = null;
                if (m_AdjustRotateAngle || m_orientation == LabelOrientation.Horizontal)
                {
                    // Save Graphics state
                    save = gfx.Save();

                    Matrix matrixTemp = new Matrix();

                    //Get required rotation angle.
                    float m_fRotationAngle = 0f;
                    if (this.Container != null)
                    {
                        if (m_orientation == LabelOrientation.Horizontal)
                            m_fRotationAngle = -this.Container.RotationAngle;
                        else if (this.Container.RotationAngle >= -90 && this.Container.RotationAngle <= 90)
                            m_fRotationAngle = 0;
                        else
                            m_fRotationAngle = -180;
                    }

                    matrixTemp.RotateAt(m_fRotationAngle, new PointF(ptRenderringOrigin.X + szStringSize.Width / 2, ptRenderringOrigin.Y + szStringSize.Height / 2), MatrixOrder.Append);

                    // Append transformation.
                    gfx.MultiplyTransform(matrixTemp);
                }
              
                //// Draw text background.
                using (Brush backgroundBrush = this.BackgroundStyle.CreateBrush(gfx, rectBounds))
                {
                    gfx.FillRectangle(backgroundBrush, rectBounds);
                }

				// draw text
                using (Brush fontcolorBrush = this.FontColorStyle.CreateBrush(gfx, rectBounds))
                {
                    StringFormat fmt = GetStringFormat();
                    float fontSize = (MeasureUnitsConverter.Convert(MeasureUnitsConverter.FromPixelX(gfx.PageScale, MeasureUnits.Point) * this.FontStyle.Size, MeasureUnits.Point, MeasureUnits.Inch));
                    if (fontSize > (1 / 72f))
                        if(SizeToNode)
                            gfx.DrawString(this.Text, font, fontcolorBrush,rectBounds);
                        else
                            gfx.DrawString(this.Text, font, fontcolorBrush, rectBounds, fmt);
                }

                //Draw Line around the label
                if (this.LineStyle.LineWidth > 0)
                {
                    RectangleF rcBounds = new RectangleF(ptRenderringOrigin.X, ptRenderringOrigin.Y, szStringSize.Width, szStringSize.Height);
                    rcBounds = Geometry.WidenRect(rcBounds, this.LineStyle.LineWidth);
                    using (Pen lineBorder = this.LineStyle.CreatePen())
                    {
                        gfx.DrawRectangle(lineBorder,rcBounds.X,rcBounds.Y,rcBounds.Width,rcBounds.Height);
                    }
                }

                if (m_AdjustRotateAngle || m_orientation == LabelOrientation.Horizontal)
                {
                    // Restore Graphics state
                    gfx.Restore(save);
                }
			}
		}
        /// <summary>
        /// Positions the Label based on the Position property.
        /// </summary>        
        /// <returns>The new location for the label control</returns>
        public override PointF GetPosition()
        {
            if (m_sizeToNode)
            {
                if (this.Position != Position.Center)
                {
                    m_sizeToNode = false;
                    throw new InvalidOperationException("SizeToNode can be set only when the label position is Center.");
                }

                if (this.Container.Size.Height < m_szSize.Height && this.Container.Size.Height != 0)
                {
                    this.Container.Size = new SizeF(this.Container.Size.Width, m_szSize.Height);
                }
                if (this.Container.Size.Width < m_szSize.Width && this.Container.Size.Width != 0)
                {
                    this.Container.Size = new SizeF(m_szSize.Width, this.Container.Size.Height);
                }
            }
            bool isConnector = false;
            if (this.Container is IEndPointContainer)
            {
                isConnector = true;
            }
            PointF ptPrimitivePosition = base.GetPosition();
            if (m_UpdatePosition == false)
            {
                return ptPrimitivePosition;
            }

            PointF ptRenderringOrigin = PointF.Empty;
            SizeF szStringSize = this.Size;
            switch (this.Position)
            {
                case Position.TopLeft:
                    ptRenderringOrigin.X = ptPrimitivePosition.X;
                    ptRenderringOrigin.Y = ptPrimitivePosition.Y - (isConnector ? szStringSize.Height : 0);
                    break;
                case Position.TopCenter:
                    ptRenderringOrigin.X = ptPrimitivePosition.X - szStringSize.Width / 2;
                    ptRenderringOrigin.Y = ptPrimitivePosition.Y - (isConnector ? szStringSize.Height : 0);
                    break;
                case Position.TopRight:
                    ptRenderringOrigin.X = ptPrimitivePosition.X - szStringSize.Width;
                    ptRenderringOrigin.Y = ptPrimitivePosition.Y - (isConnector ? szStringSize.Height : 0);
                    break;
                case Position.MiddleLeft:
                    ptRenderringOrigin = new PointF(ptPrimitivePosition.X, ptPrimitivePosition.Y - szStringSize.Height / 2);
                    break;
                case Position.MiddleRight:
                    ptRenderringOrigin = new PointF(ptPrimitivePosition.X - szStringSize.Width
                    , ptPrimitivePosition.Y - szStringSize.Height / 2);
                    break;
                case Position.BottomLeft:
                    ptRenderringOrigin.X = ptPrimitivePosition.X;
                    ptRenderringOrigin.Y = ptPrimitivePosition.Y - (isConnector ? 0 : szStringSize.Height);
                    break;
                case Position.BottomCenter:
                    ptRenderringOrigin.X = ptPrimitivePosition.X - szStringSize.Width / 2;
                    ptRenderringOrigin.Y = ptPrimitivePosition.Y - (isConnector ? 0 : szStringSize.Height);
                    break;
                case Position.BottomRight:
                    ptRenderringOrigin.X = ptPrimitivePosition.X - szStringSize.Width;
                    ptRenderringOrigin.Y = ptPrimitivePosition.Y - (isConnector ? 0 : szStringSize.Height);
                    break;
                default:
                    ptRenderringOrigin = new PointF(ptPrimitivePosition.X - szStringSize.Width / 2
                    , ptPrimitivePosition.Y - szStringSize.Height / 2);
                    break;
            }
            return ptRenderringOrigin;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
		public override object Clone()
		{
			return new Label( this );
		}

        /// <summary>
        /// Gets the object data to serialize instance.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
		protected override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			
			info.AddValue( "text", m_strText );
			info.AddValue( "styleFont", this.FontStyle );
			info.AddValue("propertybindnig", this.PropertyBinding);
            info.AddValue("fillStyle", m_styleFill);
            info.AddValue("backgroundStyle", m_styleBackground);
            info.AddValue("lineStyle", m_styleLine);
            info.AddValue("size", m_szSize);
            info.AddValue("sizeToNode", m_sizeToNode);
            info.AddValue("updatePosition", m_UpdatePosition);
            info.AddValue("adjustRotateAngle", m_AdjustRotateAngle);
            info.AddValue("name", m_strName);
            info.AddValue("alignHorizontal", m_alignHorizontal);
            info.AddValue("alignVertical", m_alignVertical);
            info.AddValue("stringFormat", m_fmtStringFormat);
            info.AddValue("readOnly", m_bReadOnly);
            info.AddValue("textcase", m_txtCase);
            info.AddValue("inheritContainerSize", m_bInheritContainerSize);
            info.AddValue("orientation", m_orientation);
            info.AddValue("visible", m_bVisible);
		}

        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
		public override void UpdateServiceReferences(IServiceReferenceProvider provider)
		{
			base.UpdateServiceReferences( provider );
			
			this.FontStyle.UpdateServiceReferences( this );
            this.BackgroundStyle.UpdateServiceReferences(this);
            this.FontColorStyle.UpdateServiceReferences(this);
            this.LineStyle.UpdateServiceReferences(this);
		}

        /// <summary>
        /// Get the service reference from provider.
        /// </summary>
        /// <param name="typeHandle">The Runtime type handle.</param>
        /// <returns></returns>
        public override object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            object objToReturn = base.ProvideServiceReference(typeHandle);

            if (typeHandle.Equals(typeof(IPropertyObserver).TypeHandle))
            {
                objToReturn = this;
            }

            return objToReturn;
        }

        /// <summary>
        /// Gets the container of the property by name.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <returns></returns>
		public override object GetPropertyContainerByName( string strPropertyName )
		{
			object objToReturn = base.GetPropertyContainerByName( strPropertyName );
			
			if( strPropertyName == "FontStyle" )
			{
				objToReturn = this.FontStyle;
			}
			
			return objToReturn;
		}
		#endregion

		#region Class helper methods
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
        private StringFormat GetStringFormat()
        {
            StringFormat fmt = new StringFormat();
            fmt.Alignment = this.HorizontalAlignment;
            fmt.LineAlignment = this.VerticalAlignment;
            fmt.FormatFlags = this.FormatFlags;
            return fmt;
        }

        /// <summary>
        /// Updates the label text according to TextCase
        /// </summary>
        private void UpdateText()
        {
            if (m_txtCase == TextCases.AllUpper)
                Text = Text.ToUpper();
            else if (m_txtCase == TextCases.AllLower)
                Text = Text.ToLower();
            else
                Text = m_strOriginalText;
        }

		#endregion
	}
}