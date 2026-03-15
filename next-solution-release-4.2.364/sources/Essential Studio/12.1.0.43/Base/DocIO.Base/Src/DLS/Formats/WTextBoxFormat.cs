#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region File using directives
using System;
using System.Collections;
using System.IO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.Escher;
using System.Collections.Generic;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Class describes format of textbox ( colors and lines, size, etc.)
    /// </summary>
    public class WTextBoxFormat : FormatBase
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const float DEF_LINE_WIDTH = 0.75f;
        #endregion

        #region Class members
        /// <summary>
        /// TextBoxFormat class members
        /// </summary>
        private HorizontalOrigin m_horizRelation;
        private VerticalOrigin m_vertRelation;
        private WidthOrigin m_widthRelation;
        private HeightOrigin m_heightRelation;
        private float m_width;
        private float m_height;
        private Color m_fillColor;
        private Color m_lineColor;
        private TextBoxLineStyle m_lineStyle;
        private TextWrappingStyle m_wrapStyle;
        private float m_wrapDistanceBottom = 0.0f;
        private float m_wrapDistanceLeft = 9.0f;
        private float m_wrapDistanceRight = 9.0f;
        private float m_wrapDistanceTop = 0.0f;
        private float m_horPosition;
        private float m_verPosition;
        private int m_spid;
        private float m_txbxLineWidth;
        private LineDashing m_lineDashing;
        private TextWrappingType m_wrappingType;
        private WrapMode m_wrapMode;
        private float m_txID;
        private bool m_belowText;
        private bool m_noLine;
        private bool m_isHeader;
        private ShapeHorizontalAlignment m_horizAlignment;
        private ShapeVerticalAlignment m_verticalAlignment;
        private VerticalAlignment m_textVerticalAlignment;
        private InternalMargin m_intMargin;
        private Background m_background;
        private bool m_allowInCell = true;
        private int m_orderIndex = int.MaxValue;
        private List<String> m_styleProps;
        private bool m_fitTextToShape;
        private float m_widthRelPercent = 0;
        private float m_heightRelPercent = 0;
        private float m_horRelPercent = float.MinValue;
        private float m_verRelPercent = float.MinValue;
        private TextDirection m_textDirection = TextDirection.Horizontal;
        private Color m_textThemeColor;
        //indicate whether current wrapping bounds points added to the list or not. 
        internal bool IsWrappingBoundsAdded = false;
        //hold the wrapping bounds index.
        internal int WrapCollectionIndex = -1;
        private bool m_allowoverlap = true;
        private WrapPolygon m_wrapPolygon;
        #endregion

        #region Class properties
        /// <summary>
        /// Get/set width horizontal relative
        /// </summary>
        public WidthOrigin WidthOrigin
        {
            get
            {
                return m_widthRelation;
            }
            set
            {
                m_widthRelation = value;
            }
        }
        /// <summary>
        /// Get/set height vertical origin 
        /// </summary>
        public HeightOrigin HeightOrigin
        {
            get
            {
                return m_heightRelation;
            }
            set
            {
                m_heightRelation = value;
            }
        }
        /// <summary>
        /// Get/set horizontal origin
        /// </summary>
        public HorizontalOrigin HorizontalOrigin
        {
            get
            {
                return m_horizRelation;
            }
            set
            {
                m_horizRelation = value;
            }
        }
        /// <summary>
        /// Get/set vertical origin 
        /// </summary>
        public VerticalOrigin VerticalOrigin
        {
            get
            {
                return m_vertRelation;
            }
            set
            {
                m_vertRelation = value;
            }
        }
        /// <summary>
        /// Get/set text Wrapping style
        /// </summary>
        public TextWrappingStyle TextWrappingStyle
        {
            get
            {
                return m_wrapStyle;
            }
            set
            {
                m_wrapStyle = value;
            }
        }
        /// <summary>
        /// DistanceBottom	Returns or sets the distance (in points) between the document text and the bottom edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        internal float WrapDistanceBottom
        {
            get { return m_wrapDistanceBottom; }
            set { m_wrapDistanceBottom = value; }
        }
        /// <summary>
        /// DistanceLeft	Returns or sets the distance (in points) between the document text and the left edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        internal float WrapDistanceLeft
        {
            get { return m_wrapDistanceLeft; }
            set { m_wrapDistanceLeft = value; }
        }
        /// <summary>
        /// DistanceRight	Returns or sets the distance (in points) between the document text and the right edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        internal float WrapDistanceRight
        {
            get { return m_wrapDistanceRight; }
            set { m_wrapDistanceRight = value; }
        }
        /// <summary>
        /// DistanceTop	Returns or sets the distance (in points) between the document text and the top edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        internal float WrapDistanceTop
        {
            get { return m_wrapDistanceTop; }
            set { m_wrapDistanceTop = value; }
        }
        /// <summary>
        /// Get/set fill color for textbox
        /// </summary>
        public Color FillColor
        {
            get
            {
                if (m_background != null)
                {
                    return m_background.Color;
                }
                return Color.White;
            }
            set
            {
                FillEfects.Color = value;
                FillEfects.Type = BackgroundType.Color;
            }
        }
        /// <summary>
        /// Get/set text box linestyle
        /// </summary>
        public TextBoxLineStyle LineStyle
        {
            get
            {
                return m_lineStyle;
            }
            set
            {
                m_lineStyle = value;
            }
        }
        /// <summary>
        /// Get/set textbox width
        /// </summary>
        public float Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }
        /// <summary>
        /// Get/set textbox height
        /// </summary>
        public float Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }
        /// <summary>
        /// Get/set line color.
        /// </summary>
        public Color LineColor
        {
            get
            {
                return m_lineColor;
            }
            set
            {
                m_lineColor = value;
            }
        }
        /// <summary>
        /// Get/set value which defines if
        /// there is a line around textbox shape
        /// </summary>
        public bool NoLine
        {
            get
            {
                return m_noLine;
            }
            set
            {
                m_noLine = value;
            }
        }
        /// <summary>
        /// Get/set textbox wrapping mode
        /// </summary>
        internal WrapMode WrappingMode
        {
            get
            {
                return m_wrapMode;
            }
            set
            {
                m_wrapMode = value;
            }
        }
        /// <summary>
        /// Get/set textbox horizontal position
        /// </summary>
        public float HorizontalPosition
        {
            get
            {
                return m_horPosition;
            }
            set
            {
                m_horPosition = value;
            }
        }
        /// <summary>
        /// Get/set true/false value of IsBelowText property 
        /// </summary>
        internal bool IsBelowText
        {
            get
            {
                return m_belowText;
            }
            set
            {
                m_belowText = value;
            }
        }
        /// <summary>
        /// Get/set textbox vertical position
        /// </summary>
        public float VerticalPosition
        {
            get
            {
                return m_verPosition;
            }
            set
            {
                m_verPosition = value;
            }
        }
        /// <summary>
        /// Get/set wrapping type for textbox
        /// </summary>
        public TextWrappingType TextWrappingType
        {
            get
            {
                return m_wrappingType;
            }
            set
            {
                m_wrappingType = value;
            }
        }
        /// <summary>
        /// Get/set texbox's shape identifier
        /// </summary>
        internal int TextBoxShapeID
        {
            get
            {
                return m_spid;
            }
            set
            {
                m_spid = value;
            }
        }
        /// <summary>
        /// Get/set textbox line width
        /// </summary>
        public float LineWidth
        {
            get
            {
                return m_txbxLineWidth;
            }
            set
            {
                m_txbxLineWidth = value;
            }
        }
        /// <summary>
        /// Get/set line dashing for textbox
        /// </summary>
        public LineDashing LineDashing
        {
            get
            {
                return m_lineDashing;
            }
            set
            {
                m_lineDashing = value;
            }
        }
        /// <summary>
        /// Get/set textbox horizontal alignment
        /// </summary>
        public ShapeHorizontalAlignment HorizontalAlignment
        {
            get
            {
                return m_horizAlignment;
            }
            set
            {
                m_horizAlignment = value;
            }
        }
        /// <summary>
        /// Get/set textbox vertical alignment
        /// </summary>
        public ShapeVerticalAlignment VerticalAlignment
        {
            get
            {
                return m_verticalAlignment;
            }
            set
            {
                m_verticalAlignment = value;
            }
        }
        /// <summary>
        /// Get/set vertical alignment of the Text in textbox (DocX specific)
        /// </summary>
        internal VerticalAlignment TextVerticalAlignment
        {
            get
            {
                return m_textVerticalAlignment;
            }
            set
            {
                m_textVerticalAlignment = value;
            }
        }
        /// <summary>
        /// Get/set textbox identifier;
        /// </summary>
        internal float TextBoxIdentificator
        {
            get
            {
                return m_txID;
            }
            set
            {
                m_txID = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsHeaderTextBox
        {
            get
            {
                return m_isHeader;
            }
            set
            {
                m_isHeader = value;
            }
        }
        /// <summary>
        /// Gets the internal margin.
        /// </summary>
        /// <value>The internal margin.</value>
        public InternalMargin InternalMargin
        {
            get
            {
                if (m_intMargin == null)
                {
                    m_intMargin = new InternalMargin();
                }
                return m_intMargin;
            }
        }
        /// <summary>
        /// Gets the fill effects.
        /// </summary>
        /// <value>The fill effects.</value>
        public Background FillEfects
        {
            get
            {
                if (m_background == null)
                {
                    m_background = new Background(Document, BackgroundType.NoBackground);
                }
                return m_background;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether allow textbox in cell.
        /// </summary>
        /// <value>if allow in cell, set to <c>true</c>.</value>
        internal bool AllowInCell
        {
            get
            {
                return m_allowInCell;
            }
            set
            {
                m_allowInCell = value;
            }
        }
        /// <summary>
        /// Gets or sets the index of the order.
        /// </summary>
        /// <value>The index of the order.</value>
        internal int OrderIndex
        {
            get
            {
                if (m_orderIndex == int.MaxValue)
                {
                    //Update shape order index
                    if (Document != null && !Document.IsOpening && Document.Escher != null)
                    {
                        int index = Document.Escher.GetShapeOrderIndex(this.TextBoxShapeID);
                        if (index != -1)
                            m_orderIndex = index;
                    }
                }
                return m_orderIndex;
            }
            set
            {
                m_orderIndex = value;
            }
        }
        /// <summary>
        /// Gets the docx style properties.
        /// </summary>
        /// <value>The docx style props.</value>
        internal List<String> DocxStyleProps
        {
            get
            {
                if (m_styleProps == null)
                    m_styleProps = new List<String>();
                return m_styleProps;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has docx props.
        /// </summary>
        /// <value>
        /// 	if this instance has docx props, set to <c>true</c>.
        /// </value>
        internal bool HasDocxProps
        {
            get
            {
                return (m_styleProps == null) ? false : true;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to fit text to shape.
        /// </summary>
        /// <value>if fit text to shape, set to <c>true</c>.</value>
        internal bool FitTextToShape
        {
            get
            {
                return m_fitTextToShape;
            }
            set
            {
                m_fitTextToShape = value;
            }
        }
        /// <summary>
        /// Gets or sets the width relative percent.
        /// </summary>
        /// <value>The width relative percent.</value>
        internal float WidthRelativePercent
        {
            get
            {
                return m_widthRelPercent;
            }
            set
            {
                m_widthRelPercent = value;
            }
        }
        /// <summary>
        /// Gets or sets the height relative percent.
        /// </summary>
        /// <value>The height relative percent.</value>
        internal float HeightRelativePercent
        {
            get
            {
                return m_heightRelPercent;
            }
            set
            {
                m_heightRelPercent = value;
            }
        }
        /// <summary>
        /// Gets or sets the horizontal relative percent.
        /// </summary>
        /// <value>The horizontal relative percent.</value>
        internal float HorizontalRelativePercent
        {
            get
            {
                return m_horRelPercent;
            }
            set
            {
                m_horRelPercent = value;
            }
        }
        /// <summary>
        /// Gets or sets the vertical relative percent.
        /// </summary>
        /// <value>The vertical relative percent.</value>
        internal float VerticalRelativePercent
        {
            get
            {
                return m_verRelPercent;
            }
            set
            {
                m_verRelPercent = value;
            }
        }
        /// <summary>
        /// Gets/sets Text Box text direction.
        /// </summary>
        public TextDirection TextDirection
        {
            get
            {
                return m_textDirection;
            }
            set
            {
                m_textDirection = value;
            }
        }
        /// <summary>
        /// Gets/sets Text Theme color.
        /// </summary>
        internal Color TextThemeColor
        {
            get
            {
                return m_textThemeColor;
            }
            set
            {
                m_textThemeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow overlap].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow overlap]; otherwise, <c>false</c>.
        /// </value>
        internal bool AllowOverlap
        {
            get
            {
                return m_allowoverlap;
            }
            set
            {
                m_allowoverlap = value;
            }
        }

        /// <summary>
        /// Gets or sets the wrap polygon.
        /// </summary>
        /// <value>
        /// The wrap polygon.
        /// </value>
        internal WrapPolygon WrapPolygon
        {
            get
            {
                if (m_wrapPolygon == null)
                {
                    m_wrapPolygon = new WrapPolygon();
                    m_wrapPolygon.Edited = false;
                    //Handled to add default wrap polygon veritces
                    m_wrapPolygon.Vertices.Add(new PointF(0, 0));
                    m_wrapPolygon.Vertices.Add(new PointF(0, 21600));
                    m_wrapPolygon.Vertices.Add(new PointF(21600, 21600));
                    m_wrapPolygon.Vertices.Add(new PointF(21600, 0));
                    m_wrapPolygon.Vertices.Add(new PointF(0, 0));
                }
                return m_wrapPolygon;
            }
            set { m_wrapPolygon = value; }
        }
        #endregion

        #region Class constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTextBoxFormat"/> class.
        /// </summary>
        public WTextBoxFormat(WordDocument doc)
            : base(doc)
        {
            //m_wrapStyle = TextWrappingStyle.Square;
            m_wrapStyle = TextWrappingStyle.InFrontOfText;
            m_fillColor = Color.White;
            m_lineColor = Color.Black;
            m_lineStyle = TextBoxLineStyle.Simple;
            m_horizRelation = HorizontalOrigin.Column;
            m_vertRelation = VerticalOrigin.Paragraph;
            m_txbxLineWidth = DEF_LINE_WIDTH;
            m_lineDashing = LineDashing.Solid;
            m_wrapMode = WrapMode.None;
            m_horizAlignment = ShapeHorizontalAlignment.None;
            m_verticalAlignment = ShapeVerticalAlignment.None;
            m_textVerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Top;
            m_background = new Background(doc, BackgroundType.NoBackground);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Get default text box values
        /// </summary>
        /// <param name="key"></param>
        /// <returns>null ( don't use keys )</returns>
        protected override object GetDefValue(int key)
        {
            return null;
        }
//#if !SILVERLIGHT
        /// <summary>
        /// Read Xml attributes for textbox.
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);
            //TextBoxFillColor
            if (reader.HasAttribute(XDLSConstants.ShapeFillColorAttr))
            {
                FillColor = reader.ReadColor(XDLSConstants.ShapeFillColorAttr);
            }
            //TextBoxHeight
            if (reader.HasAttribute(XDLSConstants.ShapeHeightAttr))
            {
                Height = reader.ReadFloat(XDLSConstants.ShapeHeightAttr);
            }
            //TextBoxHorizOrigin
            if (reader.HasAttribute(XDLSConstants.ShapeHorizOriginAttr))
            {
                HorizontalOrigin = (HorizontalOrigin)reader.ReadEnum(XDLSConstants.ShapeHorizOriginAttr, typeof(HorizontalOrigin));// Syncfusion.DocIO.DLS.HorizontalOrigin );
            }
            //TextBoxLineStyle
            if (reader.HasAttribute(XDLSConstants.ShapeLineStyleAttr))
            {
                LineStyle = (TextBoxLineStyle)reader.ReadEnum(XDLSConstants.ShapeLineStyleAttr, typeof(TextBoxLineStyle));
            }
            //TextBoxTextWrappingStyle
            if (reader.HasAttribute(XDLSConstants.ShapeTextWrappingStyleAttr))
            {
                TextWrappingStyle = (TextWrappingStyle)reader.ReadEnum(XDLSConstants.ShapeTextWrappingStyleAttr, typeof(TextWrappingStyle));
            }
            //TextBoxTextBoxVertOrigin
            if (reader.HasAttribute(XDLSConstants.ShapeVertOriginAttr))
            {
                VerticalOrigin = (VerticalOrigin)reader.ReadEnum(XDLSConstants.ShapeVertOriginAttr, typeof(VerticalOrigin));
            }
            //TextBoxWidth
            if (reader.HasAttribute(XDLSConstants.ShapeWidthAttr))
            {
                Width = reader.ReadFloat(XDLSConstants.ShapeWidthAttr);
            }
            //Shape line color
            if (reader.HasAttribute(XDLSConstants.ShapeLineColorAttr))
            {
                LineColor = reader.ReadColor(XDLSConstants.ShapeLineColorAttr);
            }

            // TextBoxHorizPosition
            if (reader.HasAttribute(XDLSConstants.ShapeHorizPositionAttr))
            {
                HorizontalPosition = reader.ReadFloat(XDLSConstants.ShapeHorizPositionAttr);
            }
            // TextBoxLineDashing
            if (reader.HasAttribute(XDLSConstants.ShapeLineDashingAttr))
            {
                LineDashing = (LineDashing)reader.ReadEnum(XDLSConstants.ShapeLineDashingAttr, typeof(LineDashing));
            }
            // TextBoxLineWidth
            if (reader.HasAttribute(XDLSConstants.ShapeLineWidthAttr))
            {
                LineWidth = reader.ReadFloat(XDLSConstants.ShapeLineWidthAttr);
            }
            // TextBoxVertPosition
            if (reader.HasAttribute(XDLSConstants.ShapeVertPositionAttr))
            {
                VerticalPosition = reader.ReadFloat(XDLSConstants.ShapeVertPositionAttr);
            }
            // TextBoxWrappingMode
            if (reader.HasAttribute(XDLSConstants.ShapeWrappingModeAttr))
            {
                WrappingMode = (WrapMode)reader.ReadEnum(XDLSConstants.ShapeWrappingModeAttr, typeof(WrapMode));
            }
            // TextBoxTextWrappingType
            if (reader.HasAttribute(XDLSConstants.ShapeWrappingTypeAttr))
            {
                TextWrappingType = (TextWrappingType)
                  reader.ReadEnum(XDLSConstants.ShapeWrappingTypeAttr,
                  typeof(TextWrappingType));
            }
            if (reader.HasAttribute(XDLSConstants.ShapeIsBelowTextAttr))
            {
                IsBelowText = reader.ReadBoolean(XDLSConstants.ShapeIsBelowTextAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeNoLineAttr))
            {
                NoLine = reader.ReadBoolean(XDLSConstants.ShapeNoLineAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeNoFillAttr))
            {
                FillColor = Color.Empty;
            }
            if (reader.HasAttribute(XDLSConstants.ShapeHorizAlignAttr))
            {
                HorizontalAlignment = (ShapeHorizontalAlignment)
                  reader.ReadEnum(XDLSConstants.ShapeHorizAlignAttr,
                  typeof(ShapeHorizontalAlignment));
            }
            if (reader.HasAttribute(XDLSConstants.ShapeVertAlignAttr))
            {
                VerticalAlignment = (ShapeVerticalAlignment)
                  reader.ReadEnum(XDLSConstants.ShapeVertAlignAttr,
                  typeof(ShapeVerticalAlignment));
            }
            if (reader.HasAttribute(XDLSConstants.ShapeIdentAttr))
            {
                TextBoxShapeID = reader.ReadInt(XDLSConstants.ShapeIdentAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeIsHeaderAttr))
            {
                IsHeaderTextBox = reader.ReadBoolean(XDLSConstants.ShapeIsHeaderAttr);
            }
        }
        /// <summary>
        /// Write XML attributes for textbox.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (FillColor == Color.Empty)
            {
                writer.WriteValue(XDLSConstants.ShapeNoFillAttr, true);
            }
            else if (FillColor != Color.White)
            {
                writer.WriteValue(XDLSConstants.ShapeFillColorAttr, FillColor);
            }

            if (Height != 0)
                writer.WriteValue(XDLSConstants.ShapeHeightAttr, Height);

            if (HorizontalOrigin != HorizontalOrigin.Column)
                writer.WriteValue(XDLSConstants.ShapeHorizOriginAttr, HorizontalOrigin);

            if (LineStyle != TextBoxLineStyle.Simple)
                writer.WriteValue(XDLSConstants.ShapeLineStyleAttr, LineStyle);

            if (TextWrappingStyle != TextWrappingStyle.Square)
                writer.WriteValue(XDLSConstants.ShapeTextWrappingStyleAttr, TextWrappingStyle);

            if (VerticalOrigin != VerticalOrigin.Paragraph)
                writer.WriteValue(XDLSConstants.ShapeVertOriginAttr, VerticalOrigin);

            if (Width != 0)
                writer.WriteValue(XDLSConstants.ShapeWidthAttr, Width);

            if (LineColor != Color.Black)
                writer.WriteValue(XDLSConstants.ShapeLineColorAttr, LineColor);

            if (HorizontalPosition != 0)
                writer.WriteValue(XDLSConstants.ShapeHorizPositionAttr, HorizontalPosition);

            if (LineDashing != LineDashing.Solid)
                writer.WriteValue(XDLSConstants.ShapeLineDashingAttr, LineDashing);

            if (LineWidth != DEF_LINE_WIDTH)
                writer.WriteValue(XDLSConstants.ShapeLineWidthAttr, LineWidth);

            if (VerticalPosition != 0)
                writer.WriteValue(XDLSConstants.ShapeVertPositionAttr, VerticalPosition);

            if (WrappingMode != WrapMode.None)
                writer.WriteValue(XDLSConstants.ShapeWrappingModeAttr, WrappingMode);

            if (TextWrappingType != TextWrappingType.Both)
                writer.WriteValue(XDLSConstants.ShapeWrappingTypeAttr, TextWrappingType);

            if (IsBelowText)
                writer.WriteValue(XDLSConstants.ShapeIsBelowTextAttr, IsBelowText);

            if (NoLine)
                writer.WriteValue(XDLSConstants.ShapeNoLineAttr, NoLine);

            if (HorizontalAlignment != ShapeHorizontalAlignment.None)
                writer.WriteValue(XDLSConstants.ShapeHorizAlignAttr, HorizontalAlignment);

            if (VerticalAlignment != ShapeVerticalAlignment.None)
                writer.WriteValue(XDLSConstants.ShapeVertAlignAttr, VerticalAlignment);

            if (TextBoxShapeID != 0)
                writer.WriteValue(XDLSConstants.ShapeIdentAttr, TextBoxShapeID);

            if (IsHeaderTextBox)
                writer.WriteValue(XDLSConstants.ShapeIsHeaderAttr, IsHeaderTextBox);
        }        
//#endif
        /// <summary>
        /// Clone textbox format.
        /// </summary>
        /// <returns></returns>
        public WTextBoxFormat Clone()
        {
          WTextBoxFormat format = new WTextBoxFormat(Document);

          format.FillColor = FillColor;
          format.LineColor = LineColor;
          format.Height = Height;
          format.HorizontalOrigin = HorizontalOrigin;
          format.LineStyle = LineStyle;
          format.TextWrappingStyle = TextWrappingStyle;
          format.VerticalOrigin = VerticalOrigin;
          format.Width = Width;
          format.TextDirection = TextDirection;
          format.HorizontalPosition = HorizontalPosition;
          format.VerticalPosition = VerticalPosition;
          format.TextBoxShapeID = TextBoxShapeID;
          format.LineWidth = LineWidth;
          format.LineDashing = LineDashing;
          format.TextWrappingType = TextWrappingType;
          format.WrappingMode = WrappingMode;
          format.TextBoxIdentificator = TextBoxIdentificator;
          format.IsBelowText = IsBelowText;
          format.NoLine = NoLine;
          format.HorizontalAlignment = HorizontalAlignment;
          format.VerticalAlignment = VerticalAlignment;
          format.TextVerticalAlignment = TextVerticalAlignment;
          format.IsHeaderTextBox = IsHeaderTextBox;

          if( m_intMargin != null )
          {
            format.m_intMargin = m_intMargin.Clone();
          }

          if( m_background != null )
          {
            format.m_background = m_background.Clone();
          }

          return format;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="nextOwner"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            if (m_background != null)
                m_background.UpdateImageRecord(doc);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the fill effects.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="doc">The doc.</param>
        internal void UpdateFillEffects(MsofbtSpContainer container, WordDocument doc)
        {
            m_background = new Background(doc, container);
        }
        /// <summary>
        /// Get width relative to percent
        /// </summary>
        /// <returns></returns>
        internal float GetWidthRelativeToPercent()
        {

            Entity ent = GetBaseEntity(OwnerBase as WTextBox);
            if (ent is WSection)
            {
                WPageSetup pageSetup = (ent as WSection).PageSetup;
                switch (WidthOrigin)
                {
                    case WidthOrigin.Page:
                        return pageSetup.PageSize.Width * (WidthRelativePercent / 100);
                    case WidthOrigin.LeftMargin:
                    case WidthOrigin.InsideMargin:
                        return pageSetup.Margins.Left * (WidthRelativePercent / 100);
                    case WidthOrigin.RightMargin:
                    case WidthOrigin.OutsideMargin:
                        return pageSetup.Margins.Right * (WidthRelativePercent / 100);
                    default:
                        return pageSetup.ClientWidth * (WidthRelativePercent / 100);
                }
            }
            else
                return m_width;
        }
        /// <summary>
        /// Get height relative to percent
        /// </summary>
        /// <returns></returns>
        internal float GetHeightRelativeToPercent()
        {
            Entity ent = GetBaseEntity(OwnerBase as WTextBox);
            if (ent is WSection)
            {
                WPageSetup pageSetup = (ent as WSection).PageSetup;
                switch (HeightOrigin)
                {
                    case HeightOrigin.Page:
                        return pageSetup.PageSize.Height * (HeightRelativePercent / 100);
                    case HeightOrigin.TopMargin:
                    case HeightOrigin.InsideMargin:
                        return pageSetup.Margins.Top * (HeightRelativePercent / 100);
                    case HeightOrigin.BottomMargin:
                    case HeightOrigin.OutsideMargin:
                        return pageSetup.Margins.Bottom * (HeightRelativePercent / 100);
                    default:
                        return (pageSetup.PageSize.Height - pageSetup.Margins.Top - pageSetup.Margins.Bottom) * (HeightRelativePercent / 100);
                }
            }
            else
                return m_height;
        }
        /// <summary>
        /// Get the base entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Entity GetBaseEntity(Entity entity)
        {
            Entity baseEntity = entity;
            do
            {
                if (baseEntity == null || baseEntity.Owner == null)
                    return baseEntity;
                baseEntity = baseEntity.Owner;
            }
            while (!(baseEntity is WSection));

            return baseEntity;
        }
        #endregion
    }
    /// <summary>    /// Class describes textbox internal margin format.
    /// </summary>
    public class InternalMargin
    {
        #region Constants
        internal const float DEF_HORIZMARGIN = 7.087f;
        internal const float DEF_VERTMARGIN = 3.685f;
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private float m_intLeftMarg;
        private float m_intRightMarg;
        private float m_intTopMarg;
        private float m_intBottomMarg;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the internal left margin (in points).
        /// </summary>
        /// <value>The internal left margin.</value>
        public float Left
        {
            get
            {
                return m_intLeftMarg;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Left", "Internal left margin must be higher than 0");
                m_intLeftMarg = value;
            }
        }
        /// <summary>
        /// Gets or sets the internal right margin (in points).
        /// </summary>
        /// <value>The internal right margin.</value>
        public float Right
        {
            get
            {
                return m_intRightMarg;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Right", "Internal right margin must be higher than 0");
                m_intRightMarg = value;
            }
        }
        /// <summary>
        /// Gets or sets the internal top margin (in points).
        /// </summary>
        /// <value>The internal top margin.</value>
        public float Top
        {
            get
            {
                return m_intTopMarg;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Top", "Internal top margin must be higher than 0");
                m_intTopMarg = value;
            }
        }
        /// <summary>
        /// Gets or sets the internal bottom margin (in points).
        /// </summary>
        /// <value>The internal bottom margin.</value>
        public float Bottom
        {
            get
            {
                return m_intBottomMarg;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Bottom", "Internal bottom margin must be higher than 0");
                m_intBottomMarg = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalMargin"/> class.
        /// </summary>
        public InternalMargin()
        {
            m_intLeftMarg = DEF_HORIZMARGIN;
            m_intRightMarg = DEF_HORIZMARGIN;
            m_intTopMarg = DEF_VERTMARGIN;
            m_intBottomMarg = DEF_VERTMARGIN;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal InternalMargin Clone()
        {
            InternalMargin margin = new InternalMargin();
            margin.Left = m_intLeftMarg;
            margin.Right = m_intRightMarg;
            margin.Top = m_intTopMarg;
            margin.Bottom = m_intBottomMarg;

            return margin;
        }
        #endregion
    }

    internal class WrapPolygon
    {
        #region fields
        private bool m_edited = false;
        private List<PointF> m_vertices;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WrapPolygon"/> is edited.
        /// </summary>
        /// <value>
        ///   <c>true</c> if edited; otherwise, <c>false</c>.
        /// </value>
        internal bool Edited
        {
            get
            {
                return m_edited;
            }
            set
            {
                m_edited = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        internal List<PointF> Vertices
        {
            get
            {
                return m_vertices;
            }
            set
            {
                m_vertices = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WrapPolygon"/> class.
        /// </summary>
        internal WrapPolygon()
        {
            m_vertices = new List<PointF>();
        }
        #endregion

    }
}
