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
using System.IO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
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
    /// Summary description for InlineShapeObject.
    /// </summary>
    public class InlineShapeObject : ShapeObject
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private PICF m_inlinePictDesc;
        /// <summary>
        /// 
        /// </summary>
        private MsofbtBSE m_curBSE;
        /// <summary>
        /// 
        /// </summary>
        private MsofbtSpContainer m_shapeContainer;
        /// <summary>
        /// Defines if current image is embedded;
        /// </summary>
        private bool m_isOLE;
        /// <summary>
        /// Defines id of OLE container in obeject pool.
        /// </summary>
        private int m_oleContainerId = -1;
        /// <summary>
        /// Unparsed data stream
        /// </summary>
        private byte[] m_unparsedData;
        private GradientFill m_lineGradient;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the line gradient.
        /// </summary>
        /// <value>The line gradient.</value>
        internal GradientFill LineGradient
        {
            get
            {
                if (m_lineGradient == null)
                    m_lineGradient = new GradientFill();
                return m_lineGradient;
            }
        }
        /// <summary>
        /// Gets/sets inline shape object's format.
        /// </summary>
        internal PICF PictureDescriptor
        {
            get
            {
                return m_inlinePictDesc;
            }
            set
            {
                m_inlinePictDesc = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSpContainer ShapeContainer
        {
            get
            {
                return m_shapeContainer;
            }
            set
            {
                m_shapeContainer = value;
            }
        }
        /// <summary>
        /// Gets/sets IsOLE property
        /// </summary>
        internal bool IsOLE
        {
            get
            {
                return m_isOLE;
            }
            set
            {
                m_isOLE = true;
            }
        }
        /// <summary>
        /// Get/set id for OLE container which has image data.
        /// </summary>
        internal int OLEContainerId
        {
            get
            {
                return m_oleContainerId;
            }
            set
            {
                m_oleContainerId = value;
            }
        }
        /// <summary>
        /// Get unparsed data stream
        /// </summary>
        internal byte[] UnparsedData
        {
            get
            {
                return m_unparsedData;
            }
            set
            {
                m_unparsedData = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineShapeObject"/> class.
        /// </summary>
        /// <param name="doc"></param>
        internal InlineShapeObject(IWordDocument doc)
            : base(doc)
        {
            m_curBSE = new MsofbtBSE(doc as WordDocument);
            m_shapeContainer = new MsofbtSpContainer(doc as WordDocument);
            m_inlinePictDesc = new PICF();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            InlineShapeObject iso = (InlineShapeObject)base.CloneImpl();
            iso.m_inlinePictDesc = PictureDescriptor.Clone();

            if (ShapeContainer != null)
            {
                iso.ShapeContainer = (MsofbtSpContainer)ShapeContainer.Clone();
            }

            iso.Cloned = true;

            return iso;
        }
        /// <summary>
        /// Gets the dash style.
        /// </summary>
        /// <param name="borderStyle">The border style.</param>
        /// <param name="lineStyle">The line style.</param>
        /// <returns></returns>
        internal LineDashing GetDashStyle(BorderStyle borderStyle, ref TextBoxLineStyle lineStyle)
        {
            LineDashing dashStyle = LineDashing.Solid;
            lineStyle = TextBoxLineStyle.Simple;
            switch (borderStyle)
            {
                case BorderStyle.DashSmallGap:
                case BorderStyle.Dot:
                    dashStyle = LineDashing.DotGEL;
                    break;
                case BorderStyle.DashLargeGap:
                    dashStyle = LineDashing.DashGEL;
                    break;
                case BorderStyle.DotDash:
                    dashStyle = LineDashing.DashDotGEL;
                    break;
                case BorderStyle.DotDotDash:
                    dashStyle = LineDashing.LongDashDotDotGEL;
                    break;
                case BorderStyle.Double:
                case BorderStyle.DoubleWave:
                    lineStyle = TextBoxLineStyle.Double;
                    break;
                case BorderStyle.ThinThinSmallGap:
                case BorderStyle.ThickThinMediumGap:
                case BorderStyle.ThickThinLargeGap:
                case BorderStyle.Outset:
                    lineStyle = TextBoxLineStyle.ThickThin;
                    break;
                case BorderStyle.ThinThickSmallGap:
                case BorderStyle.ThinThickMediumGap:
                case BorderStyle.ThinThickLargeGap:
                case BorderStyle.Inset:
                    lineStyle = TextBoxLineStyle.ThinThick;
                    break;
                case BorderStyle.Triple:
                case BorderStyle.ThinThickThinSmallGap:
                case BorderStyle.ThickThickThinMediumGap:
                case BorderStyle.ThinThickThinLargeGap:
                    lineStyle = TextBoxLineStyle.Triple;
                    break;
            }
            return dashStyle;
        }
        /// <summary>
        /// Gets the border style.
        /// </summary>
        /// <param name="dashStyle">The dash style.</param>
        /// <param name="lineStyle">The line style.</param>
        /// <returns></returns>
        internal BorderStyle GetBorderStyle(LineDashing dashStyle, TextBoxLineStyle lineStyle)
        {
            BorderStyle borderStyle = BorderStyle.None;
            if (dashStyle != LineDashing.Solid)
            {
                switch (dashStyle)
                {
                    case LineDashing.Dash:
                    case LineDashing.DashGEL:
                    case LineDashing.LongDashGEL:
                        borderStyle = BorderStyle.DashLargeGap;
                        break;
                    case LineDashing.DashDotGEL:
                        borderStyle = BorderStyle.DotDash;
                        break;
                    case LineDashing.Dot:
                    case LineDashing.DotGEL:
                        borderStyle = BorderStyle.Dot;
                        break;
                    case LineDashing.DashDot:
                    case LineDashing.LongDashDotGEL:
                        borderStyle = BorderStyle.DotDash;
                        break;
                    case LineDashing.DashDotDot:
                    case LineDashing.LongDashDotDotGEL:
                        borderStyle = BorderStyle.DotDotDash;
                        break;
                    default:
                        borderStyle = BorderStyle.Single;
                        break;
                }
            }
            else if (lineStyle != TextBoxLineStyle.Simple)
            {
                switch (lineStyle)
                {
                    case TextBoxLineStyle.Double:
                        borderStyle = BorderStyle.Double;
                        break;
                    case TextBoxLineStyle.ThinThick:
                        borderStyle = BorderStyle.ThinThickMediumGap;
                        break;
                    case TextBoxLineStyle.ThickThin:
                        borderStyle = BorderStyle.ThickThinMediumGap;
                        break;
                    case TextBoxLineStyle.Triple:
                        borderStyle = BorderStyle.ThickThickThinMediumGap;
                        break;
                    default:
                        borderStyle = BorderStyle.Single;
                        break;
                }
            }
            return borderStyle;
        }
        /// <summary>
        /// Converts to inline shape.
        /// </summary>
        internal void ConvertToInlineShape()
        {
            uint width = 0;
            if (ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineWidth))
            {
                width = ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineWidth);
                ShapeContainer.ShapeOptions.Properties.Remove((int)FOPTELineStyle.lineWidth);
            }
            width = (uint)Math.Round(((double)width / DLSConstants.EmusPerPoint) * DLSConstants.BorderLineFactor);
            PictureDescriptor.BorderLeft.LineWidth = (byte)width;
            PictureDescriptor.BorderTop.LineWidth = (byte)width;
            PictureDescriptor.BorderRight.LineWidth = (byte)width;
            PictureDescriptor.BorderBottom.LineWidth = (byte)width;
            BorderStyle borderStyle = BorderStyle.None;
            if (ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStyle))
            {
                TextBoxLineStyle lineStyle = (TextBoxLineStyle)ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineStyle);
                borderStyle = GetBorderStyle(LineDashing.Solid, lineStyle);
                if (lineStyle == TextBoxLineStyle.Simple)
                    borderStyle = BorderStyle.Single;
                ShapeContainer.ShapeOptions.Properties.Remove((int)FOPTELineStyle.lineStyle);
            }
            if (ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineDashing))
            {
                LineDashing dashStyle = (LineDashing)ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineDashing);
                borderStyle = GetBorderStyle(dashStyle, TextBoxLineStyle.Simple);
                if (dashStyle == LineDashing.Solid && borderStyle == BorderStyle.None)
                    borderStyle = BorderStyle.Single;
                ShapeContainer.ShapeOptions.Properties.Remove((int)FOPTELineStyle.lineDashing);
            }
            if (borderStyle != BorderStyle.None)
            {
                PictureDescriptor.BorderLeft.BorderType = (byte)borderStyle;
                PictureDescriptor.BorderTop.BorderType = (byte)borderStyle;
                PictureDescriptor.BorderRight.BorderType = (byte)borderStyle;
                PictureDescriptor.BorderBottom.BorderType = (byte)borderStyle;
            }
            if (ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineColor))
            {
                uint rgb = ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineColor);
                Color color = WordColor.ConvertRGBToColor(rgb);
                int id = WordColor.ConvertColorToId(color);
                PictureDescriptor.BorderLeft.LineColor = (byte)id;
                PictureDescriptor.BorderTop.LineColor = (byte)id;
                PictureDescriptor.BorderRight.LineColor = (byte)id;
                PictureDescriptor.BorderBottom.LineColor = (byte)id;
                ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderLeftColor, rgb);
                ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderTopColor, rgb);
                ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderRightColor, rgb);
                ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderBottomColor, rgb);
                ShapeContainer.ShapeOptions.Properties.Remove((int)FOPTELineStyle.lineColor);
            }
            //Clears shape border properties.
            if (ShapeContainer.ShapeOptions.LineProperties.HasDefined)
                ShapeContainer.ShapeOptions.Properties.Remove((int)FOPTELineStyle.lineStyleBooleanProperties);
        }
        /// <summary>
        /// Converts to shape.
        /// </summary>
        internal void ConvertToShape()
        {
            uint width = (uint)Math.Round(((double)PictureDescriptor.BorderLeft.LineWidth / DLSConstants.BorderLineFactor) * DLSConstants.EmusPerPoint);
            ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineWidth, width);
            Color color = PictureDescriptor.BorderLeft.LineColorExt;
            if (ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderLeftColor))
            {
                color = WordColor.ConvertRGBToColor(ShapeContainer.ShapePosition.GetPropertyValue((int)FOPTEGroupShape.borderLeftColor));
                ShapeContainer.ShapePosition.Properties.Remove((int)FOPTEGroupShape.borderLeftColor);
            }
            ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineColor, WordColor.ConvertColorToRGB(color));
            TextBoxLineStyle lineStyle = TextBoxLineStyle.Simple;
            LineDashing dashStyle = GetDashStyle((BorderStyle)PictureDescriptor.BorderLeft.BorderType, ref lineStyle);
            ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyle, (uint)lineStyle);
            ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineDashing, (uint)dashStyle);

            //Clears inline shape border properties.
            if (ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderTopColor))
                ShapeContainer.ShapePosition.Properties.Remove((int)FOPTEGroupShape.borderTopColor);
            if (ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderRightColor))
                ShapeContainer.ShapePosition.Properties.Remove((int)FOPTEGroupShape.borderRightColor);
            if (ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderBottomColor))
                ShapeContainer.ShapePosition.Properties.Remove((int)FOPTEGroupShape.borderBottomColor);
            PictureDescriptor.brcLeft = new BorderCode();
            PictureDescriptor.brcTop = new BorderCode();
            PictureDescriptor.brcRight = new BorderCode();
            PictureDescriptor.brcBottom = new BorderCode();
        }
        /// <summary>
        /// Gets the effect extent.
        /// </summary>
        /// <param name="borderWidth">Width of the border.</param>
        /// <param name="leftTop">The left top.</param>
        /// <param name="rightBottom">The right bottom.</param>
        internal void GetEffectExtent(double borderWidth, ref long leftTop, ref long rightBottom)
        {
            int count = (int)(borderWidth / 1.5);
            if (borderWidth % 1.5 >= 1)
                count += (int)(borderWidth % 1.5);
            if (count == 0)
                count = 1;
            leftTop = (long)(count * 1.5 * DLSConstants.EmusPerPoint);
            rightBottom = 0;
            if (count > 1)
                rightBottom = (long)((count - 1) * 1.5 * DLSConstants.EmusPerPoint);
        }
        #endregion

        #region Class overrides
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);

            //Write Picture descriptor
            byte[] serializableData = null;
            if (m_inlinePictDesc != null)
            {
                MemoryStream memStream = new MemoryStream();
                m_inlinePictDesc.Write(memStream);
                serializableData = memStream.ToArray();
#if WINRT
                memStream.Dispose();
#else
                memStream.Close();
#endif
                writer.WriteChildBinaryElement(XDLSConstants.PictureDescriptorTag, serializableData);
            }

            //Write shape container
            byte[] containerData = null;
            if (ShapeContainer != null)
            {
                MemoryStream memStream = new MemoryStream();
                ShapeContainer.WriteMsofbhWithRecord(memStream);
                containerData = memStream.ToArray();
#if WINRT
                memStream.Dispose();
#else
                memStream.Close();
#endif
                writer.WriteChildBinaryElement(XDLSConstants.ShapeContainerDataTag, containerData);

                //Write MsofbtBSE data
                //Write shape's picture
                m_curBSE = (ShapeContainer as MsofbtSpContainer).Bse;
                if (m_curBSE != null)
                {
                    MemoryStream dataStream = new MemoryStream();
                    m_curBSE.Write(dataStream);
                    byte[] dataContainer = dataStream.ToArray();
                    writer.WriteChildBinaryElement(XDLSConstants.ShapeBlipTag, dataContainer);

                    //Write Fbse data
                    MemoryStream fbseStream = new MemoryStream();
                    m_curBSE.WriteMsofbhWithRecord(fbseStream);
                    byte[] fbseData = fbseStream.ToArray();
                    writer.WriteChildBinaryElement(XDLSConstants.ShapeFbseTag, fbseData);
#if WINRT
                    dataStream.Dispose();
                    fbseStream.Dispose();
#else
                    dataStream.Close();
                    fbseStream.Close();
#endif
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlContent(IXDLSContentReader reader)
        {
            bool retValue = base.ReadXmlContent(reader);

            //Read Picture descriptor
            if (reader.TagName == XDLSConstants.PictureDescriptorTag)
            {
                byte[] serializableData = reader.ReadChildBinaryElement();
                if (serializableData.Length != 0)
                {
                    MemoryStream memStream = new MemoryStream(serializableData, 0, serializableData.Length);
                    BinaryReader breader = new BinaryReader(memStream);
                    m_inlinePictDesc.Read(breader);
#if WINRT
                    memStream.Dispose();
#else
                    memStream.Close();
#endif
                }
            }

            if (reader.TagName == XDLSConstants.ShapeContainerDataTag)
            {
                byte[] containerData = reader.ReadChildBinaryElement();
                if (containerData.Length != 0)
                {
                    MemoryStream memStream = new MemoryStream(containerData, 0, containerData.Length);
                    ShapeContainer = new MsofbtSpContainer(Document);
                    memStream.Position = 0;
                    ShapeContainer.ReadMsofbhWithRecord(memStream);
                   
#if WINRT
                    memStream.Dispose();
#else
                     memStream.Close();
#endif
                    retValue = true;
                }
            }

            //Read shape's picture
            if (reader.TagName == XDLSConstants.ShapeBlipTag)
            {
                byte[] dataContainer = reader.ReadChildBinaryElement();
                MemoryStream dataStream = new MemoryStream(dataContainer, 0, dataContainer.Length);
                MsofbtBSE bse = new MsofbtBSE(Document);
                bse.Read(dataStream);
                m_curBSE = bse;
            }
            //Read shape's fbse
            if (reader.TagName == XDLSConstants.ShapeFbseTag)
            {
                byte[] fbseContainer = reader.ReadChildBinaryElement();
                MemoryStream fbseStream = new MemoryStream(fbseContainer, 0, fbseContainer.Length);
                m_curBSE.ReadMsofbhWithRecord(fbseStream);
                (ShapeContainer as MsofbtSpContainer).Bse = new MsofbtBSE(Document);
                (ShapeContainer as MsofbtSpContainer).Bse = m_curBSE;
            }
            return retValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            if (reader.HasAttribute(XDLSConstants.ShapeObjIsOLEAttr))
            {
                m_isOLE = reader.ReadBoolean(XDLSConstants.ShapeObjIsOLEAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeObjOLEContId))
            {
                m_oleContainerId = reader.ReadInt(XDLSConstants.ShapeObjOLEContId);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            writer.WriteValue(XDLSConstants.TypeTag, ParagraphItemType.InlineShapeObject);

            if (IsOLE)
            {
                writer.WriteValue(XDLSConstants.ShapeObjIsOLEAttr, IsOLE);
                writer.WriteValue(XDLSConstants.ShapeObjOLEContId, OLEContainerId);
            }
        }
//#endif
        /// <summary>
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            Document.CloneShapeEscher(doc, this);
            if (ShapeContainer != null)
                ShapeContainer.CloneRelationsTo(doc);
            this.Cloned = false;
        }

        #endregion
    }
    /// <summary>
    /// Summary description for GradientFill.
    /// </summary>
    internal class GradientFill
    {
        #region Fields
        private bool m_bRotate;
        private FlipOrientation m_flip;
        private List<GradientStop> m_gradientStops;
        private LinearGradient m_linearGradient;
        private PathGradient m_pathGradient;
        private TileRectangle m_tileRectangle;
        //private GradientShadingVariant m_ShadingVariant;
        private GradientShadingStyle m_ShadingStyle;
        private string m_Focus;
        
        #endregion

        #region Properties
        internal string Focus
        {
            get { return m_Focus; }
            set { m_Focus = value; }
        }
        internal GradientShadingStyle ShadingStyle
        {
            get { return m_ShadingStyle; }
            set { m_ShadingStyle = value; }
        }
        //internal GradientShadingVariant ShadingVariant
        //{
        //    get { return m_ShadingVariant; }
        //    set { m_ShadingVariant = value; }
        //}
        /// <summary>
        /// Gets or sets a value indicating whether [rotate with shape].
        /// </summary>
        /// <value><c>true</c> if [rotate with shape]; otherwise, <c>false</c>.</value>
        internal bool RotateWithShape
        {
            get
            {
                return m_bRotate;
            }
            set
            {
                m_bRotate = value;
            }
        }
        /// <summary>
        /// Gets or sets the flip.
        /// </summary>
        /// <value>The flip.</value>
        internal FlipOrientation Flip
        {
            get
            {
                return m_flip;
            }
            set
            {
                m_flip = value;
            }
        }
        /// <summary>
        /// Gets the gradient stops.
        /// </summary>
        /// <value>The gradient stops.</value>
        internal List<GradientStop> GradientStops
        {
            get
            {
                if (m_gradientStops == null)
                    m_gradientStops = new List<GradientStop>();
                return m_gradientStops;
            }
        }
        /// <summary>
        /// Gets or sets the linear gradient.
        /// </summary>
        /// <value>The linear gradient.</value>
        internal LinearGradient LinearGradient
        {
            get
            {
                return m_linearGradient;
            }
            set
            {
                m_linearGradient = value;
            }
        }
        /// <summary>
        /// Gets or sets the path gradient.
        /// </summary>
        /// <value>The path gradient.</value>
        internal PathGradient PathGradient
        {
            get
            {
                return m_pathGradient;
            }
            set
            {
                m_pathGradient = value;
            }
        }
        /// <summary>
        /// Gets the tile rectangle.
        /// </summary>
        /// <value>The tile rectangle.</value>
        internal TileRectangle TileRectangle
        {
            get
            {
                if (m_tileRectangle == null)
                    m_tileRectangle = new TileRectangle();
                return m_tileRectangle;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientFill"/> class.
        /// </summary>
        internal GradientFill()
        {
        }
        #endregion
    }
    /// <summary>
    /// Summary description for GradientFill.
    /// </summary>
    internal class GradientStop
    {
        #region Fields
        private byte m_position = byte.MaxValue;
        private Color m_color;
        private byte m_opacity = byte.MaxValue;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        internal byte Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        internal Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }
        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        internal byte Opacity
        {
            get
            {
                return m_opacity;
            }
            set
            {
                m_opacity = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStop"/> class.
        /// </summary>
        internal GradientStop()
        {
        }
        #endregion
    }
    /// <summary>
    /// Summary description for LinearGradient.
    /// </summary>
    internal class LinearGradient
    {
        #region Fields
        private short m_angle;
        private bool m_bScaled;
        private bool m_isAnglePositive = true;
        #endregion

        #region Properties
        internal bool AnglePositive
        {
            get { return m_isAnglePositive; }
            set { m_isAnglePositive = value; }
        }
        /// <summary>
        /// Gets or sets the angle.
        /// </summary>
        /// <value>The angle.</value>
        internal short Angle
        {
            get
            {
                return m_angle;
            }
            set
            {
                m_angle = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LinearGradient"/> is scaled.
        /// </summary>
        /// <value><c>true</c> if scaled; otherwise, <c>false</c>.</value>
        internal bool Scaled
        {
            get
            {
                return m_bScaled;
            }
            set
            {
                m_bScaled = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradient"/> class.
        /// </summary>
        internal LinearGradient()
        {
        }
        #endregion
    }
    /// <summary>
    /// Summary description for PathGradient.
    /// </summary>
    internal class PathGradient
    {
        #region Fields
        private GradientShadeType m_pathShade;
        private int m_bottomOffset;
        private int m_leftOffset;
        private int m_rightOffset;
        private int m_topOffset;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the path shade.
        /// </summary>
        /// <value>The path shade.</value>
        internal GradientShadeType PathShade
        {
            get
            {
                return m_pathShade;
            }
            set
            {
                m_pathShade = value;
            }
        }
        /// <summary>
        /// Gets or sets the bottom offset.
        /// </summary>
        /// <value>The bottom offset.</value>
        internal int BottomOffset
        {
            get
            {
                return m_bottomOffset;
            }
            set
            {
                m_bottomOffset = value;
            }
        }
        /// <summary>
        /// Gets or sets the left offset.
        /// </summary>
        /// <value>The left offset.</value>
        internal int LeftOffset
        {
            get
            {
                return m_leftOffset;
            }
            set
            {
                m_leftOffset = value;
            }
        }
        /// <summary>
        /// Gets or sets the right offset.
        /// </summary>
        /// <value>The right offset.</value>
        internal int RightOffset
        {
            get
            {
                return m_rightOffset;
            }
            set
            {
                m_rightOffset = value;
            }
        }
        /// <summary>
        /// Gets or sets the top offset.
        /// </summary>
        /// <value>The top offset.</value>
        internal int TopOffset
        {
            get
            {
                return m_topOffset;
            }
            set
            {
                m_topOffset = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PathGradient"/> class.
        /// </summary>
        internal PathGradient()
        {
        }
        #endregion
    }
    /// <summary>
    /// Summary description for TileRectangle.
    /// </summary>
    internal class TileRectangle
    {
        #region Fields
        private int m_bottomOffset;
        private int m_leftOffset;
        private int m_rightOffset;
        private int m_topOffset;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the bottom offset.
        /// </summary>
        /// <value>The bottom offset.</value>
        internal int BottomOffset
        {
            get
            {
                return m_bottomOffset;
            }
            set
            {
                m_bottomOffset = value;
            }
        }
        /// <summary>
        /// Gets or sets the left offset.
        /// </summary>
        /// <value>The left offset.</value>
        internal int LeftOffset
        {
            get
            {
                return m_leftOffset;
            }
            set
            {
                m_leftOffset = value;
            }
        }
        /// <summary>
        /// Gets or sets the right offset.
        /// </summary>
        /// <value>The right offset.</value>
        internal int RightOffset
        {
            get
            {
                return m_rightOffset;
            }
            set
            {
                m_rightOffset = value;
            }
        }
        /// <summary>
        /// Gets or sets the top offset.
        /// </summary>
        /// <value>The top offset.</value>
        internal int TopOffset
        {
            get
            {
                return m_topOffset;
            }
            set
            {
                m_topOffset = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TileRectangle"/> class.
        /// </summary>
        internal TileRectangle()
        {
        }
        #endregion
    }
}
