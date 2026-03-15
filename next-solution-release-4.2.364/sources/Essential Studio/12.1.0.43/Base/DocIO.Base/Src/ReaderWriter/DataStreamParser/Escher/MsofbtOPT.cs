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

#region file using directives
using System;
using System.IO;
using System.Collections;

using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Default Property Table Record msofbtOPT
    /// This describes the default properties of newly created shapes. 
    /// Only the properties that differ from the per-property defaults are saved. 
    /// The format of the record is the same as that of the property table in a shape; 
    /// a discussion of that format is in the Shape Properties section .
    /// </summary>
    internal class MsofbtOPT : BaseEscherRecord
    {
        #region Class constants
        public const int DEF_PIB_ID = 260;
        public const int DEF_PIBFLAGS_ID = 262;
        public const int DEF_TXID = 128;
        public const int DEF_WRAP_REXT = 133;
        internal const uint DEF_WRAP_DIST = 114300;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private msofbtRGFOPTE m_prop;
        private LineStyleBooleanProperties m_lineProps;
        private WrapPolygonVertices m_wrapPolygonVetrices;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the line properties.
        /// </summary>
        /// <value>The line properties.</value>
        internal LineStyleBooleanProperties LineProperties
        {
            get
            {
                if (m_lineProps == null)
                    m_lineProps = new LineStyleBooleanProperties(m_prop, (int)FOPTELineStyle.lineStyleBooleanProperties);
                return m_lineProps;
            }
        }
        /// <summary>
        /// Gets the wrap polygon vertices.
        /// </summary>
        /// <value>
        /// The wrap polygon vertices.
        /// </value>
        internal WrapPolygonVertices WrapPolygonVertices
        {
            get
            {
                if (m_wrapPolygonVetrices == null)
                    m_wrapPolygonVetrices = new WrapPolygonVertices(m_prop, (int)FOPTEGroupShape.pWrapPolygonVertices);
                return m_wrapPolygonVetrices;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal FOPTEBid Pib
        {
            get
            {
                if (m_prop.ContainsKey(DEF_PIB_ID))
                    return m_prop[DEF_PIB_ID] as FOPTEBid;
                else
                    return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal FOPTEBid Txid
        {
            get
            {
                FOPTEBid fopteBid = null;
                if( m_prop.ContainsKey( DEF_TXID ) )
                    fopteBid =  m_prop[DEF_TXID] as FOPTEBid;
                return fopteBid;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal msofbtRGFOPTE Properties
        {
            get
            {
                return m_prop;
            }
            set
            {
                m_prop = value;
            }
        }
        /// <summary>
        /// Gets or sets the layout in table cell.
        /// </summary>
        /// <value>The layout in table cell.</value>
        internal uint LayoutInTableCell
        {
            get
            {
                return GetPropertyValue((int)FOPTEGroupShape.fPrint);
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.fPrint, value);
            }
        }
        /// <summary>
        /// Gets/sets a value indicating whether allow in table cell.
        /// </summary>
        /// <value><c>true</c> if allow in table cell; otherwise, <c>false</c>.</value>
        internal bool AllowInTableCell
        {
            get
            {
                if (LayoutInTableCell != uint.MaxValue)
                {
                    bool useLayoutInCell = ((LayoutInTableCell & 0x80000000) >> 31) != 0;
                    if (useLayoutInCell)
                        return ((LayoutInTableCell & 0x8000) >> 15) != 0;
                }
                return true;
            }
        }
        /// <summary>
        /// Gets or sets the distance from bottom.
        /// </summary>
        /// <value>
        /// The distance from bottom.
        /// </value>
        internal uint DistanceFromBottom
        {
            get
            {
                uint value= GetPropertyValue((int)FOPTEGroupShape.dyWrapDistBottom);
                if (value == uint.MaxValue)
                {
                    return 0;
                }
                return value;
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.dyWrapDistBottom, value);
            }
        }
        /// <summary>
        /// Gets or sets the distance from left.
        /// </summary>
        /// <value>
        /// The distance from left.
        /// </value>
        internal uint DistanceFromLeft
        {
            get
            {
                uint value = GetPropertyValue((int)FOPTEGroupShape.dxWrapDistLeft);
                if (value == uint.MaxValue)
                {
                    return DEF_WRAP_DIST;
                }
                return value;
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.dxWrapDistLeft, value);
            }
        }
        /// <summary>
        /// Gets or sets the distance from right.
        /// </summary>
        /// <value>
        /// The distance from right.
        /// </value>
        internal uint DistanceFromRight
        {
            get
            {
                uint value = GetPropertyValue((int)FOPTEGroupShape.dxWrapDistRight);
                if (value == uint.MaxValue)
                {
                    return DEF_WRAP_DIST;
                }
                return value;
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.dxWrapDistRight, value);
            }
        }
        /// <summary>
        /// Gets or sets the distance from top.
        /// </summary>
        /// <value>
        /// The distance from top.
        /// </value>
        internal uint DistanceFromTop
        {
            get
            {
                uint value = GetPropertyValue((int)FOPTEGroupShape.dyWrapDistTop);
                if (value == uint.MaxValue)
                {
                    return 0;
                }
                return value;
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.dyWrapDistTop, value);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtOPT(WordDocument doc)
            : base(MSOFBT.msofbtOPT, 3, doc)
        {
            m_prop = new msofbtRGFOPTE();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_prop.Clear();
            m_prop.Read(stream, Header.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            //      Header.Instance = m_prop.Count;
            Header.Instance = CountInstanceValue();
            m_prop.Write(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtOPT options = new MsofbtOPT(m_doc);
            FOPTEBase fopteBase = null;
            //for (int i = 0, cnt = m_prop.Count; i < cnt; i++)
            //{
            //    fopteBase = (FOPTEBase)m_prop.GetByIndex(i);
            //    options.m_prop.Add(fopteBase.Clone());
            //}
            foreach (Object obj in m_prop.Values)
            {
                fopteBase = (FOPTEBase)obj;
                options.m_prop.Add(fopteBase);
            }
            options.m_doc = m_doc;
            return options;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();

            if (m_prop != null)
            {
                m_prop.Clear();
                m_prop = null;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Get uint property value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public uint GetPropertyValue(int key)
        {
            if (this.Properties.ContainsKey(key))
            {
                FOPTEBid fbValue = this.Properties[key] as FOPTEBid;
                if (fbValue != null)
                    return fbValue.Value;
            }
            return uint.MaxValue;
        }
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        internal void SetPropertyValue(int key, uint value)
        {
            if (m_prop.ContainsKey(key))
                (m_prop[key] as FOPTEBid).Value = value;
            else
                m_prop.Add(key, new FOPTEBid(key, false, value));
        }
        /// <summary>
        /// Get complex value by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] GetComplexPropValue(int key)
        {       
            if (this.Properties.ContainsKey(key))
            {
                FOPTEComplex complexVal = (FOPTEComplex)this.Properties[key];
                return complexVal.Value;
            }
            return null;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int CountInstanceValue()
        {
            int retValue = 0;
            foreach (Object obj in m_prop.Values)
            {
                FOPTEBase fopte = obj as FOPTEBase;
                //Handled specifically for duplicate properties.
                if (fopte.Id < 10000)
                {
                    retValue += 1;
                }
            }
            return retValue;
        }

        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public enum FOPTETransform
    {
        rotation = 4
    }

    /// <summary>
    /// Changes the behavior of a shape by restricting direct manipulation.
    /// </summary>
    public enum FOPTEProtection
    {
        fLockRotation = 19,
        fLockAspectRatio = 120,
        fLockPosition = 121,
        fLockAgainstSelect = 122,
        fLockCropping = 123,
        fLockVertices = 124,
        fLockText = 125,
        fLockAdjustHandles = 126,
        fLockAgainstGrouping = 127
    }
    /// <summary>
    /// How text fits in a shape. Text is host-dependent, so some hosts may ignore 
    /// some of these properties.
    /// </summary>
    public enum FOPTEText
    {
        lTxid = 128,
        dxTextLeft = 129,
        dyTextTop = 130,
        dxTextRight = 131,
        dyTextBottom = 132,
        WrapText = 133,
        scaleText = 134,
        anchorText = 135,
        txflTextFlow = 136,
        cdirFont = 137,
        hspNext = 138,
        txdir = 139,
        fSelectText = 187,
        fAutoTextMargin = 188,
        fRotateText = 189,
        fFitShapeToText = 190,
        fFitTextToShape = 191
    }
    /// <summary>
    /// Effect text of the shape - this is what the WordArt tools use, and is separate from 
    /// the attached text present in ordinary textboxes. Theoretically, a shape could have 
    /// both (a WordArt with attached text), but this is not currently allowed by the UI. 
    /// Note that font information is provided here. The default text size is in points, 
    /// the text effect geometry interfaces require the device size of a point to interpret this. 
    /// The default point size is a 16.16 fixed-point number. A text effect is present 
    /// if the fGText boolean is set and either the gtextUNICODE (UNICODE) or gtextRTF (RTF) 
    /// is present, the UNICODE string takes precedence, however it cannot include any additional 
    /// font information (unlike the RTF). 
    /// </summary>
    public enum FOPTEGeoText
    {
        gtextUNICODE = 192,
        gtextRTF = 193,
        gtextAlign = 194,
        gtextSize = 195,
        gtextSpacing = 196,
        gtextFont = 197,
        gtextFReverseRows = 240,
        fGtext = 241,
        gtextFVertical = 242,
        gtextFKern = 243,
        gtextFTight = 244,
        gtextFStretch = 245,
        gtextFShrinkFit = 246,
        gtextFBestFit = 247,
        gtextFNormalize = 248,
        gtextFDxMeasure = 249,
        gtextFBold = 250,
        gtextFItalic = 251,
        gtextFUnderline = 252,
        gtextFShadow = 253,
        gtextFSmallcaps = 254,
        gtextFStrikethrough = 255
    }
    /// <summary>
    /// How a BLIP fits into a shape. This includes cropping information as well as 
    /// picture display modifications such as brightness and contrast. 
    /// </summary>
    public enum FOPTEBlip
    {
        cropFromTop = 256,
        cropFromBottom = 257,
        cropFromLeft = 258,
        cropFromRight = 259,
        pib = 260,
        pibName = 261,
        pibFlags = 262,
        pictureTransparent = 263,
        pictureContrast = 264,
        pictureBrightness = 265,
        pictureGamma = 266,
        pictureId = 267,
        pictureDblCrMod = 268,
        pictureFillCrMod = 269,
        pictureLineCrMod = 270,
        pibPrint = 271,
        pibPrintName = 272,
        pibPrintFlags = 273,
        fNoHitTestPicture = 316,
        pictureGray = 317,
        pictureBiLevel = 318,
        pictureActive = 319
    }
    /// <summary>
    /// The geometry of the shape. Typically, these properties reside in a shape type definition, 
    /// and so are not written to the file. However, freeform shapes drawing using the polygon 
    /// tools set the pVertices and pSegmentInfo properties to define their geometries. 
    /// </summary>
    public enum FOPTEGeometry
    {
        geoLeft = 320,
        geoTop = 321,
        geoRight = 322,
        geoBottom = 323,
        shapePath = 324,
        pVertices = 325,
        pSegmentInfo = 326,
        adjustValue = 327,
        adjust2Value = 328,
        adjust3Value = 329,
        adjust4Value = 330,
        adjust5Value = 331,
        adjust6Value = 332,
        adjust7Value = 333,
        adjust8Value = 334,
        adjust9Value = 335,
        adjust10Value = 336,
        fShadowOK = 378,
        f3DOK = 379,
        fLineOK = 380,
        fGtextOK = 381,
        fFillShadeShapeOK = 382,
        fFillOK = 383
    }
    /// <summary>
    /// Two main colors are defined - a foreground color and a background color. 
    /// Different fillTypes use these values differently. In addition to the foreground 
    /// and background any number of shade colors can be defined. Each shade color 
    /// is associated with a "position" which says how far into the shade the color appears 
    /// � colors must be given in position order. 
    /// </summary>
    public enum FOPTEFillStyle
    {
        fillType = 384,
        fillColor = 385,
        fillOpacity = 386,
        fillBackColor = 387,
        fillBackOpacity = 388,
        fillCrMod = 389,
        fillBlip = 390,
        fillBlipName = 391,
        fillBlipFlags = 392,
        fillWidth = 393,
        fillHeight = 394,
        fillAngle = 395,
        fillFocus = 396,
        fillToLeft = 397,
        fillToTop = 398,
        fillToRight = 399,
        fillToBottom = 400,
        fillRectLeft = 401,
        fillRectTop = 402,
        fillRectRight = 403,
        fillRectBottom = 404,
        fillDztype = 405,
        fillShadePreset = 406,
        fillShadeColors = 407,
        fillOriginX = 408,
        fillOriginY = 409,
        fillShapeOriginX = 410,
        fillShapeOriginY = 411,
        fillShadeType = 412,
        fFilled = 443,
        fHitTestFill = 444,
        fillShape = 445,
        fillUseRect = 446,
        fNoFillHitTest = 447,
    }
    /// <summary>
    /// Lines are centered about the infinitely thin proto-line along which they are drawn. 
    /// Complex dash effects are supported only for simple lines (e.g. changing the end cap) - 
    /// defaults should be used for other line styles. The line width is in EMUs; a line width 
    /// of zero should not be used - there is no logical interpretation on a high-resolution printer. 
    /// </summary>
    public enum FOPTELineStyle
    {
        lineColor = 448,
        lineOpacity = 449,
        lineBackColor = 450,
        lineCrMod = 451,
        lineType = 452,
        lineFillBlip = 453,
        lineFillBlipName = 454,
        lineFillBlipFlags = 455,
        lineFillWidth = 456,
        lineFillHeight = 457,
        lineFillDztype = 458,
        lineWidth = 459,
        lineMiterLimit = 460,
        lineStyle = 461,
        lineDashing = 462,
        lineDashStyle = 463,
        lineStartArrowhead = 464,
        lineEndArrowhead = 465,
        lineStartArrowWidth = 466,
        lineStartArrowLength = 467,
        lineEndArrowWidth = 468,
        lineEndArrowLength = 469,
        lineJoinStyle = 470,
        lineEndCapStyle = 471,
        lineColorExt = 473,
        reserved474 = 474,
        lineColorExtMod = 475,
        reserved476 = 476,
        lineBackColorExt = 477,
        reserved478 = 478,
        lineBackColorExtMod = 479,
        reserved480 = 480,
        reserved481 = 481,
        reserved482 = 482,
        lineStyleBooleanProperties = 511
    }
    public enum FOPTELeftLineStyle
    {
        lineLeftColor = 1344,
        lineLeftOpacity = 1345,
        lineLeftBackColor = 1346,
        lineLeftCrMod = 1347,
        lineLeftType = 1348,
        lineLeftFillBlip = 1349,
        lineLeftFillBlipName = 1350,
        lineLeftFillBlipFlags = 1351,
        lineLeftFillWidth = 1352,
        lineLeftFillHeight = 1353,
        lineLeftFillDztype = 1354,
        lineLeftWidth = 1355,
        lineLeftMiterLimit = 1356,
        lineLeftStyle = 1357,
        lineLeftDashing = 1358,
        lineLeftDashStyle = 1359,
        lineLeftStartArrowhead = 1360,
        lineLeftEndArrowhead = 1361,
        lineLeftStartArrowWidth = 1362,
        lineLeftStartArrowLength = 1363,
        lineLeftEndArrowWidth = 1364,
        lineLeftEndArrowLength = 1365,
        lineLeftJoinStyle = 1366,
        lineLeftEndCapStyle = 1367,
        lineLeftColorExt = 1369,
        reserved1370 = 1370,
        lineLeftColorExtMod = 1371,
        reserved1372 = 1372,
        lineLeftBackColorExt = 1373,
        reserved1374 = 1374,
        lineLeftBackColorExtMod = 1375,
        reserved1376 = 1376,
        reserved1377 = 1377,
        reserved1378 = 1378,
        leftLineStyleBooleanProperties = 1407
    }
    public enum FOPTETopLineStyle
    {
        lineTopColor = 1408,
        lineTopOpacity = 1409,
        lineTopBackColor = 1410,
        lineTopCrMod = 1411,
        lineTopType = 1412,
        lineTopFillBlip = 1413,
        lineTopFillBlipName = 1414,
        lineTopFillBlipFlags = 1415,
        lineTopFillWidth = 1416,
        lineTopFillHeight = 1417,
        lineTopFillDztype = 1418,
        lineTopWidth = 1419,
        lineTopMiterLimit = 1420,
        lineTopStyle = 1421,
        lineTopDashing = 1422,
        lineTopDashStyle = 1423,
        lineTopStartArrowhead = 1424,
        lineTopEndArrowhead = 1425,
        lineTopStartArrowWidth = 1426,
        lineTopStartArrowLength = 1427,
        lineTopEndArrowWidth = 1428,
        lineTopEndArrowLength = 1429,
        lineTopJoinStyle = 1430,
        lineTopEndCapStyle = 1431,
        lineTopColorExt = 1433,
        reserved1434 = 1434,
        lineTopColorExtMod = 1435,
        reserved1436 = 1436,
        lineTopBackColorExt = 1437,
        reserved1438 = 1438,
        lineTopBackColorExtMod = 1439,
        reserved1440 = 1440,
        reserved1441 = 1441,
        reserved1442 = 1442,
        topLineStyleBooleanProperties = 1471
    }
    public enum FOPTERightLineStyle
    {
        lineRightColor = 1472,
        lineRightOpacity = 1473,
        lineRightBackColor = 1474,
        lineRightCrMod = 1475,
        lineRightType = 1476,
        lineRightFillBlip = 1477,
        lineRightFillBlipName = 1478,
        lineRightFillBlipFlags = 1479,
        lineRightFillWidth = 1480,
        lineRightFillHeight = 1481,
        lineRightFillDztype = 1482,
        lineRightWidth = 1483,
        lineRightMiterLimit = 1484,
        lineRightStyle = 1485,
        lineRightDashing = 1486,
        lineRightDashStyle = 1487,
        lineRightStartArrowhead = 1488,
        lineRightEndArrowhead = 1489,
        lineRightStartArrowWidth = 1490,
        lineRightStartArrowLength = 1491,
        lineRightEndArrowWidth = 1492,
        lineRightEndArrowLength = 1493,
        lineRightJoinStyle = 1494,
        lineRightEndCapStyle = 1495,
        lineRightColorExt = 1497,
        reserved1498 = 1498,
        lineRightColorExtMod = 1499,
        reserved1500 = 1500,
        lineRightBackColorExt = 1501,
        reserved1502 = 1502,
        lineRightBackColorExtMod = 1503,
        reserved1504 = 1504,
        reserved1505 = 1505,
        reserved1506 = 1506,
        rightLineStyleBooleanProperties = 1535
    }
    public enum FOPTEBottomLineStyle
    {
        lineBottomColor = 1536,
        lineBottomOpacity = 1537,
        lineBottomBackColor = 1538,
        lineBottomCrMod = 1539,
        lineBottomType = 1540,
        lineBottomFillBlip = 1541,
        lineBottomFillBlipName = 1542,
        lineBottomFillBlipFlags = 1543,
        lineBottomFillWidth = 1544,
        lineBottomFillHeight = 1545,
        lineBottomFillDztype = 1546,
        lineBottomWidth = 1547,
        lineBottomMiterLimit = 1548,
        lineBottomStyle = 1549,
        lineBottomDashing = 1550,
        lineBottomDashStyle = 1551,
        lineBottomStartArrowhead = 1552,
        lineBottomEndArrowhead = 1553,
        lineBottomStartArrowWidth = 1554,
        lineBottomStartArrowLength = 1555,
        lineBottomEndArrowWidth = 1556,
        lineBottomEndArrowLength = 1557,
        lineBottomJoinStyle = 1558,
        lineBottomEndCapStyle = 1559,
        lineBottomColorExt = 1561,
        reserved1562 = 1562,
        lineBottomColorExtMod = 1563,
        reserved1564 = 1564,
        lineBottomBackColorExt = 1565,
        reserved1566 = 1566,
        lineBottomBackColorExtMod = 1567,
        reserved1568 = 1568,
        reserved1569 = 1569,
        reserved1570 = 1570,
        bottomLineStyleBooleanProperties = 1599
    }
    internal enum LineType
    {
        Solid = 0,
        Pattern = 1,
        Texture = 2
    }
    internal enum LineEnd
    {
        NoEnd = 0,
        ArrowEnd = 1,
        ArrowStealthEnd = 2,
        ArrowDiamondEnd = 3,
        ArrowOvalEnd = 4,
        ArrowOpenEnd = 5,
        ArrowChevronEnd = 6,
        ArrowDoubleChevronEnd = 7
    }
    internal enum LineEndWidth
    {
        NarrowArrow = 0,
        MediumWidthArrow = 1,
        WideArrow = 2
    }
    internal enum LineEndLength
    {
        ShortArrow = 0,
        MediumLenArrow = 1,
        LongArrow = 2
    }
    internal enum LineJoin
    {
        Bevel = 0,
        Miter = 1,
        Round = 2
    }
    internal enum LineCap
    {
        Round = 0,
        Square = 1,
        Flat = 2
    }
    /// <summary>
    /// The interpretation of the transform properties depends on the type of shadow: 
    /// msoshadowOffset, msoshadowDouble: 
    /// Only the offset is used. It is interpreted as an absolute offset expressed in EMUs. 
    /// The default corresponds to 1/36" in both X and Y (2 or 3 pixels on screen depending on 
    /// monitor resolution). The offset is relative to the drawing axes (as msoshadowDrawing below, 
    /// not msoshadowRich) so a shadow offset to the bottom right of the drawing is still offset 
    /// (by the same amount) to the bottom right if the shape is rotated. The "double" case causes 
    /// two shadows to be drawn, the first (lower) at the second offset and in the shadowHighlightColor. 
    /// If the second offset is 0,0 it defaults to being the inverse of the first. 
    /// msoshadowRich: 
    /// The offsets and transformation properties are in absolute units measured relative to the shape 
    /// on the drawing - the shadow moves with the shape, but anisotropic scaling of the shape 
    /// changes the proportions of the shadow, not its angles. Compare with the following where 
    /// such scaling scales the shadow in proportion too, thus changes the angle between (e.g.) 
    /// a vertical line in the shape and it's shadow. 
    /// msoshadowShape: 
    /// The offsets and transformation properties are relative to the shape; 1.0 corresponds 
    /// to the shape width/height as appropriate. The shadow is cast relative to the shape 
    /// then scaled with the shape, so it moves with the shape. The units are simple numbers 
    /// (ratios of the G unit space effectively). This transformation type is unnatural in real 
    /// world terms, but behaves nicely in geometric terms. The offset elements of the property 
    /// set are treated as fixed-point 16.16 values. 
    /// msoshadowDrawing: 
    /// A rich shadow cast onto a plane in drawing space. The transform is applied to the drawing 
    /// coordinates of the shape and is thus expressed in EMUs. This shadow type enables creation 
    /// of shadows from multiple objects, however the shadows may overlap higher (different) 
    /// objects if the shadow plane and shape drawing planes overlap on the screen. 
    /// The shadowWeight parameter is used as in the perspective property set to apply addiitonal 
    /// scaling to the perspective parameters - these are divided by the weight. 
    /// Shadow transformations are independent of the perspective transformation applied to a shape - 
    /// either hte perspective transformation or the shadow transformation is used as appropriate. 
    /// </summary>
    public enum FOPTEShadowStyle
    {
        shadowType = 512,
        shadowColor = 513,
        shadowHighlight = 514,
        shadowCrMod = 515,
        shadowOpacity = 516,
        shadowOffsetX = 517,
        shadowOffsetY = 518,
        shadowSecondOffsetX = 519,
        shadowSecondOffsetY = 520,
        shadowScaleXToX = 521,
        shadowScaleYToX = 522,
        shadowScaleXToY = 523,
        shadowScaleYToY = 524,
        shadowPerspectiveX = 525,
        shadowPerspectiveY = 526,
        shadowWeight = 527,
        shadowOriginX = 528,
        shadowOriginY = 529,
        fShadow = 574,
        fshadowObscured = 575
    }
    /// <summary>
    /// Perspective Style
    /// This is just a 2D transformation matrix (3x3). Specifying peculiar values will cause 
    /// the shape to render completely outside its geometry - normally clients will constrain 
    /// the values to get reasonable results. The transformation may be applied at various 
    /// times as the geometry is processed, this affects the behavior of the perspective 
    /// which results in the same way as the corresponding shadow perspective types. 
    /// </summary>
    public enum FOPTEPerspectiveStyle
    {
        perspectiveType = 576,
        perspectiveOffsetX = 577,
        perspectiveOffsetY = 578,
        perspectiveScaleXToX = 579,
        perspectiveScaleYToX = 580,
        perspectiveScaleXToY = 581,
        perspectiveScaleYToY = 582,
        perspectivePerspectiveX = 583,
        perspectivePerspectiveY = 584,
        perspectiveWeight = 585,
        perspectiveOriginX = 586,
        perspectiveOriginY = 587,
        fPerspective = 639,
    }
    /// <summary>
    /// 3D Object
    /// Material properties of a 3D object. A 3D effect overrides the fill and line effects 
    /// and corresponding colors. Extrusion depths are always specified in absolute units. 
    /// </summary>
    public enum FOPTE3dObject
    {
        c3DSpecularAmt = 640,
        c3DDiffuseAmt = 641,
        c3DShininess = 642,
        c3DEdgeThickness = 643,
        c3DExtrudeForward = 644,
        c3DExtrudeBackward = 645,
        c3DExtrudePlane = 646,
        c3DExtrusionColor = 647,
        c3DCrMod = 648,
        f3D = 700,
        fc3DMetallic = 701,
        fc3DUseExtrusionColor = 702,
        fc3DLightFace = 703
    }
    /// <summary>
    /// 3D Style
    /// Properties of a 3D view; note that distances are in drawing units. 
    /// </summary>
    public enum FOPTE3dStyle
    {
        c3DYRotationAngle = 704,
        c3DXRotationAngle = 705,
        c3DRotationAxisX = 706,
        c3DRotationAxisY = 707,
        c3DRotationAxisZ = 708,
        c3DRotationAngle = 709,
        c3DRotationCenterX = 710,
        c3DRotationCenterY = 711,
        c3DRotationCenterZ = 712,
        c3DRenderMode = 713,
        c3DTolerance = 714,
        c3DXViewpoint = 715,
        c3DYViewpoint = 716,
        c3DZViewpoint = 717,
        c3DOriginX = 718,
        c3DOriginY = 719,
        c3DSkewAngle = 720,
        c3DSkewAmount = 721,
        c3DAmbientIntensity = 722,
        c3DKeyX = 723,
        c3DKeyY = 724,
        c3DKeyZ = 725,
        c3DKeyIntensity = 726,
        c3DFillX = 727,
        c3DFillY = 728,
        c3DFillZ = 729,
        c3DFillIntensity = 730,
        fc3DConstrainRotation = 763,
        fc3DRotationCenterAuto = 764,
        fc3DParallel = 765,
        fc3DKeyHarsh = 766,
        fc3DFillHarsh = 767
    }
    /// <summary>
    /// Shape
    /// Miscellaneous properties of a single shape which do not apply to group shapes.
    /// </summary>
    public enum FOPTEShape
    {
        hspMaster = 769,
        cxstyle = 771,
        bWMode = 772,
        bWModePureBW = 773,
        bWModeBW = 774,
        fOleIcon = 826,
        fPreferRelativeResize = 827,
        fLockShapeType = 828,
        fDeleteAttachedObject = 830,
        fBackground = 831
    }
    /// <summary>
    /// Callout
    /// Properties of a callout shape. 
    /// </summary>
    public enum FOPTECallout
    {
        spcot = 832,
        dxyCalloutGap = 833,
        spcoa = 834,
        spcod = 835,
        dxyCalloutDropSpecified = 836,
        dxyCalloutLengthSpecified = 837,
        fCallout = 889,
        fCalloutAccentBar = 890,
        fCalloutTextBorder = 891,
        fCalloutMinusX = 892,
        fCalloutMinusY = 893,
        fCalloutDropAuto = 894,
        fCalloutLengthSpecified = 895
    }
    /// <summary>
    /// Group Shape
    /// Miscellaneous shape properties that can apply to group shapes.
    /// </summary>
    public enum FOPTEGroupShape
    {
        wzName = 896,
        wzDescription = 897,
        pihlShape = 898,
        pWrapPolygonVertices = 899,
        dxWrapDistLeft = 900,
        dyWrapDistTop = 901,
        dxWrapDistRight = 902,
        dyWrapDistBottom = 903,
        lidRegroup = 904,
        unused906 = 906,
        wzTooltip = 909,
        wzScript = 910,
        posh = 911,
        posrelh = 912,
        posv = 913,
        posrelv = 914,
        pctHR = 915,
        alignHR = 916,
        dxHeightHR = 917,
        dxWidthHR = 918,
        wzScriptExtAttr = 919,
        scriptLang = 920,
        wzScriptLangAttr = 922,
        borderTopColor = 923,
        borderLeftColor = 924,
        borderBottomColor = 925,
        borderRightColor = 926,
        tableProperties = 927,
        tableRowProperties = 928,
        wzWebBot = 933,
        metroBlob = 937,
        dhgt = 938,
        fEditedWrap = 953,
        fBehindDocument = 954,
        fOnDblClickNotify = 955,
        fIsButton = 956,
        fOneD = 957,
        fHidden = 958,
        fPrint = 959
    }
}
