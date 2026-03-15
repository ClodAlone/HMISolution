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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for ShapeType.
    /// </summary>
    internal enum EscherShapeType
    {
        /// <summary>
        /// Accent Callouts.
        /// </summary>
        msosptAccentBorderCallout1 = 50,
        msosptAccentBorderCallout2 = 51,
        msosptAccentBorderCallout3 = 52,
        msosptAccentBorderCallout90 = 181,
        msosptAccenrCallout1 = 0x2c,
        msosptAccentCallout2 = 0x2d,
        msosptAccentCallout3 = 0x2e,
        msosptAccentCallout90 = 0xb3,
        /// <summary>
        /// Action Buttons.
        /// </summary>
        msosptActionButtonBackPrevious = 0xc2,
        msosptActionButtonBeginning = 0xc4,
        msosptActionButtonBlank = 0xbd,
        msosptActionButtonDocument = 0xc6,
        msosptActionButtonEnd = 0xc3,
        msosptActionButtonForwardNext = 0xc1,
        msosptActionButtonHelp = 0xbf,
        msosptActionButtonHome = 190,
        msosptActionButtonInformation = 0xc0,
        msosptActionButtonMovie = 200,
        msosptActionButtonReturn = 0xc5,
        msosptActionButtonSound = 0xc7,


        msosptArc = 0x13,
        msosptArrow = 13,
        msosptBalloon = 0x11,
        msosptBentArrow = 0x5b,
        /// <summary>
        /// Bent Connectors.
        /// </summary>
        msosptBentConnector2 = 0x21,
        msosptBentConnector3 = 0x22,
        msosptBentConnector4 = 0x23,
        msosptBentConnector5 = 0x24,

        msosptBentUpArrow = 90,
        msosptBevel = 0x54,
        msosptBlockArc = 0x5f,
        /// <summary>
        /// Border Callouts.
        /// </summary>
        msosptBorderCallout1 = 0x2f,
        msosptBorderCallout2 = 0x30,
        msosptBorderCallout3 = 0x31,
        msosptBorderCallout90 = 180,

        /// <summary>
        /// 
        /// </summary>
        msosptBracePair = 0xba,
        msosptBracketPair = 0xb9,
        /// <summary>
        /// Callouts.
        /// </summary>
        msosptCallout1 = 0x29,
        msosptCallout2 = 0x2a,
        msosptCallout3 = 0x2b,
        msosptCallout90 = 0xb2,
        /// <summary>
        /// 
        /// </summary>
        msosptCan = 0x16,
        msosptChevron = 0x37,
        msosptCircularArrow = 0x63,
        msosptCloudCallout = 0x6a,
        msosptCube = 0x10,
        /// <summary>
        /// Curved connectors.
        /// </summary>
        msosptCurvedConnector2 = 0x25,
        msosptCurvedConnector3 = 0x26,
        msosptCurvedConnector4 = 0x27,
        msosptCurvedConnector5 = 40,
        /// <summary>
        /// Curved Arrows.
        /// </summary>
        msosptCurvedDownArrow = 0x69,
        msosptCurvedLeftArrow = 0x67,
        msosptCurvedRightArrow = 0x66,
        msosptCurvedUpArrow = 0x68,
        /// <summary>
        /// 
        /// </summary>
        msosptCustomShape = 100,
        msosptDiamond = 4,
        msosptDonut = 0x17,
        msosptDoubleWave = 0xbc,
        msosptDownArrow = 0x43,
        msosptDownArrowCallout = 80,
        /// <summary>
        /// Ellipse shapes.
        /// </summary>
        msosptEllipse = 3,
        msosptEllipseRibbon = 0x6b,
        msosptEllipseRibbon2 = 0x6c,
        /// <summary>
        /// FlowChart shapes.
        /// </summary>
        msosptFlowChartAlternateProcess = 0xb0,
        msosptFlowChartCollate = 0x7d,
        msosptFlowChartConnector = 120,
        msosptFlowChartDecision = 110,
        msosptFlowChartDelay = 0x87,
        msosptFlowChartDisplay = 0x86,
        msosptFlowChartDocument = 0x72,
        msosptFlowChartExtract = 0x7f,
        msosptFlowChartInputOutput = 0x6f,
        msosptFlowChartInternalStorage = 0x71,
        msosptFlowChartMagneticDisk = 0x84,
        msosptFlowChartMagneticDrum = 0x85,
        msosptFlowChartMagneticTape = 0x83,
        msosptFlowChartManualInput = 0x76,
        msosptFlowChartManualOperation = 0x77,
        msosptFlowChartMerge = 0x80,
        msosptFlowChartMultidocument = 0x73,
        msosptFlowChartOfflineStorage = 0x81,
        msosptFlowChartOffpageConnector = 0xb1,
        msosptFlowChartOnlineStorage = 130,
        msosptFlowChartOr = 0x7c,
        msosptFlowChartPredefinedProcess = 0x70,
        msosptFlowChartPreparation = 0x75,
        msosptFlowChartProcess = 0x6d,
        msosptFlowChartPunchedCard = 0x79,
        msosptFlowChartPunchedTape = 0x7a,
        msosptFlowChartSort = 0x7e,
        msosptFlowChartSummingJunction = 0x7b,
        msosptFlowChartTerminator = 0x74,
        /// <summary>
        /// 
        /// </summary>
        msosptFoldedCorner = 0x41,
        msosptGroup = -1,
        msosptHeart = 0x4a,
        msosptHexagon = 9,
        msosptHomePlate = 15,
        msosptHorizontalScroll = 0x62,
        msosptHostControl = 0xc9,
        /// <summary>
        /// Image shape
        /// </summary>
        msosptPictureFrame = 0x4b,
        msosptIrregularSeal1 = 0x47,
        msosptIrregularSeal2 = 0x48,
        /// <summary>
        /// Leftsided shapes. 
        /// </summary>
        msosptLeftArrow = 0x42,
        msosptLeftArrowCallout = 0x4d,
        msosptLeftBrace = 0x57,
        msosptLeftBracket = 0x55,
        msosptLeftRightArrow = 0x45,
        msosptLeftRightArrowCallout = 0x51,
        msosptLeftRightUpArrow = 0xb6,
        msosptLeftUpArrow = 0x59,
        /// <summary>
        /// 
        /// </summary>
        msosptLightningBolt = 0x49,
        msosptLine = 20,
        msosptMoon = 0xb8,
        msosptMin = 0,
        msosptNoSmoking = 0x39,
        msosptNotchedRightArrow = 0x5e,

        msosptOctagon = 10,
        msosptOleControl = -3,
        msosptOleObject = -2,
        msosptParallelogram = 7,
        msosptPentagon = 0x38,
        msosptPlaque = 0x15,
        msosptPlus = 11,
        msosptQuadArrow = 0x4c,
        msosptQuadArrowCallout = 0x53,

        msosptRectangle = 1,
        msosptRibbon = 0x35,
        msosptRibbon2 = 0x36,

        msosptRightArrowCallout = 0x4e,
        msosptRightBrace = 0x58,
        msosptRightBracket = 0x56,
        msosptRightTriangle = 6,
        msosptRoundRectangle = 2,
        /// <summary>
        /// Seal shape
        /// </summary>
        msosptSeal = 0x12,
        msosptSeal16 = 0x3b,
        msosptSeal24 = 0x5c,
        msosptSeal32 = 60,
        msosptSeal4 = 0xbb,
        msosptSeal8 = 0x3a,

        msosptSmileyFace = 0x60,
        msosptStar = 12,
        msosptStraightConnector1 = 0x20,
        msosptStripedRightArrow = 0x5d,
        msosptSun = 0xb7,


        msosptTextBox = 0xca,
        /// <summary>
        /// WordArt objects.
        /// </summary>
        msosptTextArchDownCurve = 0x91,
        msosptTextArchDownPour = 0x95,
        msosptTextArchUpCurve = 0x90,
        msosptTextArchUpPour = 0x94,
        msosptTextButtonCurve = 0x93,
        msosptTextButtonPour = 0x97,
        msosptTextCanDown = 0xaf,
        msosptTextCanUp = 0xae,
        msosptTextCascadeDown = 0x9b,
        msosptTextCascadeUp = 0x9a,
        msosptTextChevron = 140,
        msosptTextChevronInverted = 0x8d,
        msosptTextCircleCurve = 0x92,
        msosptTextCirclePour = 150,
        msosptTextCurve = 0x1b,
        msosptTextCurveDown = 0x99,
        msosptTextCurveUp = 0x98,
        msosptTextDeflate = 0xa1,
        msosptTextDeflateBottom = 0xa3,
        msosptTextDeflateInflate = 0xa6,
        msosptTextDeflateInflateDeflate = 0xa7,
        msosptTextDeflateTop = 0xa5,
        msosptTextFadeDown = 0xab,
        msosptTextFadeLeft = 0xa9,
        msosptTextFadeRight = 0xa8,
        msosptTextFadeUp = 170,
        msosptTextHexagon = 0x1a,
        msosptTextInflate = 160,
        msosptTextInflateBottom = 0xa2,
        msosptTextInflateTop = 0xa4,
        msosptTextOctagon = 0x19,
        msosptTextOnCurve = 30,
        msosptTextOnRing = 0x1f,
        msosptTextPlainText = 0x88,
        msosptTextRing = 0x1d,
        msosptTextRingInside = 0x8e,
        msosptTextRingOutside = 0x8f,
        msosptTextSimple = 0x18,
        msosptTextSlantDown = 0xad,
        msosptTextSlantUp = 0xac,
        msosptTextStop = 0x89,
        msosptTextTriangle = 0x8a,
        msosptTextTriangleInverted = 0x8b,
        msosptTextWave = 0x1c,
        msosptTextWave1 = 0x9c,
        msosptTextWave2 = 0x9d,
        msosptTextWave3 = 0x9e,
        msosptTextWave4 = 0x9f,
        /// <summary>
        /// 
        /// </summary>
        msosptThickArrow = 14,
        msosptTrapezoid = 8,
        msosptTriangle = 5,
        msosptUpArrow = 0x44,
        msosptUpArrowCallout = 0x4f,
        msosptUpDownArrow = 70,
        msosptUpDownArrowCallout = 0x52,
        msosptUturnArrow = 0x65,
        msosptVerticalScroll = 0x61,
        msosptWave = 0x40,
        msosptWedgeEllipseCallout = 0x3f,
        msosptWedgeRectCallout = 0x3d,
        msosptWedgeRRectCallout = 0x3e

    }
    /// <summary>
    /// Summary description for ShapeDocType.
    /// </summary>
    internal enum ShapeDocType
    {
        /// <summary>
        /// 
        /// </summary>
        Main = 0,
        /// <summary>
        /// 
        /// </summary>
        HeaderFooter = 1
    }

    /// <summary>
    /// Possible values for the orientation of a shape.
    /// </summary>
    internal enum FlipOrientation
    {
        //     Coordinates are not flipped.
        None = 0,
        //     Flip along the y-axis, reversing the x-coordinates.
        Horizontal = 1,
        //     Flip along the x-axis, reversing the y-coordinates.
        Vertical = 2,
        //     Flip along both the y- and x-axis.
        Both = 3,
    }
    /// <summary>
    /// Possible types of background fill.
    /// </summary>
    internal enum BackgroundFillType
    {
        /// <summary>
        /// Fill with a solid color.
        /// </summary>     
        msofillSolid = 0,
        /// <summary>
        /// Fill with a pattern (bitmap).
        /// </summary>
        msofillPattern = 1,
        /// <summary>
        /// A texture (pattern with its own color map).
        /// </summary> 
        msofillTexture = 2,
        /// <summary>
        /// Center a picture in the shape.
        /// </summary>       
        msofillPicture = 3,
        /// <summary>
        /// Shade from start to end points.
        /// </summary>
        msofillShade = 4,
        /// <summary>
        /// Shade from bounding rectangle to end point.
        /// </summary>
        msofillShadeCenter = 5,
        /// <summary>
        /// Shade from shape outline to end point.
        /// </summary>  
        msofillShadeShape = 6,
        /// <summary>
        /// Similar to msofillShade, but the fillAngle
        /// is additionally scaled by the aspect ratio of
        /// the shape. If shape is square, it is the
        /// same as Shade.
        /// </summary>   
        msofillShadeScale = 7,
        /// <summary>
        /// Special type - shade to title ---  for PP
        /// </summary>
        msofillShadeTitle = 8,
        /// <summary>
        /// Use the background fill color/pattern
        /// </summary>
        msofillBackground = 9
    }
    /// <summary>
    /// MSOSHADETYPE � how to interpret the colors in a shaded fill.
    /// </summary>
    internal enum ShadeType
    {
        /// <summary>
        /// Interpolate without correction between RGBs.
        /// </summary>
        msoshadeNone = 0,
        /// <summary>
        /// Apply gamma correction to colors.
        /// </summary>
        msoshadeGamma = 1,
        /// <summary>
        /// Apply a sigma transfer function to position.
        /// </summary>
        msoshadeSigma = 2,
        /// <summary>
        /// Add a flat band at the start of the shade.
        /// </summary>    
        msoshadeBand = 4,
        /// <summary>
        ///  This is a one color shade
        /// </summary>
        msoshadeOneColor = 8,
        /// <summary>
        /// A parameter for the band or sigma function can be stored in the top
        /// 16 bits of the value - this is a proportion of *each* band of the
        /// shade to make flat (or the approximate equal value for a sigma
        /// function).  NOTE: the parameter is not used for the sigma function,
        /// instead a built in value is used.  This value should not be changed
        /// from the default!
        /// </summary>
        msoshadeParameterShift = 16,
        //msoshadeParameterMask =  0xffff0000  /*4294901760**/,
        msoshadeDefault = (msoshadeGamma | msoshadeSigma |
          (16384 << msoshadeParameterShift))
    }
}
