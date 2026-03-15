#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.DocIO.DLS
{

    //public enum ShapeDrawingType
    //{
    //    Arc = 4,
    //    Button = 7,
    //    CellsDrawing = 30,
    //    Chart = 5,
    //    CheckBox = 11,
    //    ComboBox = 20,
    //    Comment = 0x19,
    //    DialogBox = 15,
    //    Group = 0,
    //    GroupBox = 0x13,
    //    Label = 14,
    //    Line = 1,
    //    ListBox = 0x12,
    //    OleObject = 0x18,
    //    Oval = 3,
    //    Picture = 8,
    //    Polygon = 9,
    //    RadioButton = 12,
    //    Rectangle = 2,
    //    ScrollBar = 0x11,
    //    Spinner = 0x10,
    //    TextBox = 6,
    //    Unknown = 0x1d
    //}
    ////public enum ShapeType
    //{
    //    sp,
    //    grpSp,
    //    graphicFrame,
    //    cxnSp,
    //    pic,
    //    contentPart,
    //}
    //public enum AnchorType
    //{
    //    Absolute,
    //    RelSize,
    //    OneCell,
    //    TwoCell,
    //}

    public enum AutoShapeType
    {
        Unknown = -1,
        //Rectangles
        Rectangle = 1,
        RoundedRectangle = 5,
        SnipSingleCornerRectangle = 155,
        SnipSameSideCornerRectangle = 156,
        SnipDiagonalCornerRectangle = 157,
        SnipAndRoundSingleCornerRectangle = 154,
        RoundSingleCornerRectangle = 151,
        RoundSameSideCornerRectangle = 152,
        RoundDiagonalCornerRectangle = 153,

        //Basic Shapes
        Oval = 9,
        IsoscelesTriangle = 7,
        RightTriangle = 8,
        Parallelogram = 2,
        Trapezoid = 3,
        Diamond = 4,
        RegularPentagon = 12,
        Hexagon = 10,
        Heptagon = 145,
        Octagon = 6,
        Decagon = 144,
        Dodecagon = 146,
        Pie = 142,
        Chord = 161,
        Teardrop = 160,
        Frame = 158,
        HalfFrame = 159,
        L_Shape = 162,
        DiagonalStripe = 141,
        Cross = 11,
        Plaque = 28,
        Can = 13,
        Cube = 14,
        Bevel = 15,
        Donut = 18,
        NoSymbol = 19,
        BlockArc = 20,
        FoldedCorner = 16,
        SmileyFace = 17,
        Heart = 21,
        LightningBolt = 22,
        Sun = 23,
        Moon = 24,
        Cloud = 179,
        Arc = 25,
        DoubleBracket = 26,
        DoubleBrace = 27,
        LeftBracket = 29,
        RightBracket = 30,
        LeftBrace = 31,
        RightBrace = 32,


        //BlockArrows
        RightArrow = 33,
        LeftArrow = 34,
        UpArrow = 35,
        DownArrow = 36,
        LeftRightArrow = 37,
        UpDownArrow = 38,
        QuadArrow = 39,
        LeftRightUpArrow = 40,
        BentArrow = 41,
        UTurnArrow = 42,
        LeftUpArrow = 43,
        BentUpArrow = 44,
        CurvedRightArrow = 45,
        CurvedLeftArrow = 46,
        CurvedUpArrow = 47,
        CurvedDownArrow = 48,
        StripedRightArrow = 49,
        NotchedRightArrow = 50,
        Pentagon = 51,
        Chevron = 52,
        RightArrowCallout = 53,
        DownArrowCallout = 56,
        LeftArrowCallout = 54,
        UpArrowCallout = 55,
        LeftRightArrowCallout = 57,
        UpDownArrowCallout = 58,
        QuadArrowCallout = 59,
        CircularArrow = 60,

        //Equations
        MathPlus = 163,
        MathMinus = 164,
        MathMultiply = 165,
        MathDivision = 166,
        MathEqual = 167,
        MathNotEqual = 168,

        //FlowCharts
        FlowChartProcess = 61,
        FlowChartAlternateProcess = 62,
        FlowChartDecision = 63,
        FlowChartData = 64,
        FlowChartPredefinedProcess = 65,
        FlowChartInternalStorage = 66,
        FlowChartDocument = 67,
        FlowChartMultiDocument = 68,
        FlowChartTerminator = 69,
        FlowChartPreparation = 70,
        FlowChartManualInput = 71,
        FlowChartManualOperation = 72,
        FlowChartConnector = 73,
        FlowChartOffPageConnector = 74,
        FlowChartCard = 75,
        FlowChartPunchedTape = 76,
        FlowChartSummingJunction = 77,
        FlowChartOr = 78,
        FlowChartCollate = 79,
        FlowChartSort = 80,
        FlowChartExtract = 81,
        FlowChartMerge = 82,
        FlowChartStoredData = 83,
        FlowChartDelay = 84,
        FlowChartSequentialAccessStorage = 85,
        FlowChartMagneticDisk = 86,
        FlowChartDirectAccessStorage = 87,
        FlowChartDisplay = 88,

        //StarsAndBanner
        Explosion1 = 89,
        Explosion2 = 90,
        Star4Point = 91,
        Star5Point = 92,
        Star6Point = 147,
        Star7Point = 148,
        Star8Point = 93,
        Star10Point = 149,
        Star12Point = 150,
        Star16Point = 94,
        Star24Point = 95,
        Star32Point = 96,
        UpRibbon = 97,
        DownRibbon = 98,
        CurvedUpRibbon = 99,
        CurvedDownRibbon = 100,
        VerticalScroll = 101,
        HorizontalScroll = 102,
        Wave = 103,
        DoubleWave = 104,

        //CallOuts
        RectangularCallout = 105,
        RoundedRectangularCallout = 106,
        OvalCallout = 107,
        CloudCallout = 108,
        LineCallout1 = 109,
        LineCallout2 = 111,
        LineCallout3 = 112,
        LineCallout1AccentBar = 114,
        LineCallout2AccentBar = 115,
        LineCallout3AccentBar = 116,
        LineCallout1NoBorder = 113,
        LineCallout2NoBorder = 119,
        LineCallout3NoBorder = 120,
        LineCallout1BorderAndAccentBar = 122,
        LineCallout2BorderAndAccentBar = 123,
        LineCallout3BorderAndAccentBar = 124,

        //Connectors
        Line = 224,
        ElbowConnector = 227,
        CurvedConnector = 228,
        StraightConnector,
        BentConnector2,
        BentConnector4,
        BentConnector5,
        CurvedConnector2,
        CurvedConnector4,
        CurvedConnector5,
    }
#if DOCIO
    //public  enum BackgroundStyle
    //{
    //    BackgroundStyle1 = 1, //Specifies Style1.

    //    BackgroundStyle10 = 10, //Specifies Style10.

    //    BackgroundStyle11 = 11, //Specifies Style11.

    //    BackgroundStyle12 = 12, //Specifies Style12.

    //    BackgroundStyle2 = 2, //Specifies Style2.

    //    BackgroundStyle3 = 3, //Specifies Style3.

    //    BackgroundStyle4 = 4, //Specifies Style4.

    //    BackgroundStyle5 = 5, //Specifies Style5.

    //    BackgroundStyle6 = 6, //Specifies Style6.

    //    BackgroundStyle7 = 7, //Specifies Style7.

    //    BackgroundStyle8 = 8, //Specifies Style8.

    //    BackgroundStyle9 = 9, //Specifies Style9.

    //    BackgroundStyleMixed = -2, //Specifies a combination of styles.

    //    BackgroundStyleNone = 0 //Specifies no styles.
    //}
    public enum FillType
    {
        FillBackground = 5,
        FillGradient = 3,
        FillMixed = -2,
        FillPatterned = 2,
        FillPicture = 6,
        FillSolid = 1,
        FillTextured = 4
    }
    public enum LineFormatType
    {
        Gradient = 3,
        None = -2,
        Patterned = 2,
        Solid = 1,
        //Textured = 4
    }
    //public enum GradientColorType
    //{
    //    ColorMixed = -2, //Mixed gradient.

    //    OneColor = 1, //One-color gradient.

    //    PresetColors = 3, //Gradient colors set according to a built-in gradient of the set defined by the PresetGradientType constant.

    //    TwoColors = 2 //Two-color gradient.
    //}
    //public enum GradientStyle
    //{
    //    DiagonalDown = 4, //Diagonal gradient moving from a top corner down to the opposite corner.

    //    DiagonalUp = 3, //Diagonal gradient moving from a bottom corner up to the opposite corner.

    //    FromCenter = 7, //Gradient running from the center out to the corners.

    //    FromCorner = 5, //Gradient running from a corner to the other three corners.

    //    FromTitle = 6,  //Gradient running from the title outward.

    //    Horizontal = 1, //Gradient running horizontally across the shape.

    //    Mixed = -2, //Gradient is mixed.

    //    Vertical = 2//Gradient running vertically down the shape.
    //}
    //public enum PresetGradientType
    //{
    //    Brass = 20,
    //    CalmWater = 8,
    //    Chrome = 21,
    //    ChromeII = 22,
    //    Daybreak = 4,
    //    Desert = 6,
    //    EarlySunset = 1,
    //    Fire = 9,
    //    Fog = 10,
    //    Gold = 18,
    //    GoldII = 19,
    //    Horizon = 5,
    //    LateSunset = 2,
    //    Mahogany = 15,
    //    Moss = 11,
    //    Nightfall = 3,
    //    Ocean = 7,
    //    Parchment = 14,
    //    Peacock = 12,
    //    Rainbow = 16,
    //    RainbowII = 17,
    //    Sapphire = 24,
    //    Silver = 23,
    //    Wheat = 13,
    //    PresetGradientMixed = -2
    //}
    public enum PatternType
    {
        Pattern10Percent = 2,
        Pattern20Percent = 3,
        Pattern25Percent = 4,
        Pattern30Percent = 5,
        Pattern40Percent = 6,
        Pattern50Percent = 7,
        Pattern5Percent = 1,
        Pattern60Percent = 8,
        Pattern70Percent = 9,
        Pattern75Percent = 10,
        Pattern80Percent = 11,
        Pattern90Percent = 12,
        Cross = 51,
        DarkDownwardDiagonal = 15,
        DarkHorizontal = 13,
        DarkUpwardDiagonal = 16,
        DarkVertical = 14,
        DashedDownwardDiagonal = 28,
        DashedHorizontal = 32,
        DashedUpwardDiagonal = 27,
        DashedVertical = 31,
        DiagonalBrick = 40,
        DiagonalCross = 54,
        Divot = 46,
        DottedDiamond = 24,
        DottedGrid = 45,
        DownwardDiagonal = 52,
        Horizontal = 49,
        HorizontalBrick = 35,
        LargeCheckerBoard = 36,
        LargeConfetti = 33,
        LargeGrid = 34,
        LightDownwardDiagonal = 21,
        LightHorizontal = 19,
        LightUpwardDiagonal = 22,
        LightVertical = 20,
        Mixed = -2,
        NarrowHorizontal = 30,
        NarrowVertical = 29,
        OutlinedDiamond = 41,
        Plaid = 42,
        Shingle = 47,
        SmallCheckerBoard = 17,
        SmallConfetti = 37,
        SmallGrid = 23,
        SolidDiamond = 39,
        Sphere = 43,
        Trellis = 18,
        UpwardDiagonal = 53,
        Vertical = 50,
        Wave = 48,
        Weave = 44,
        WideDownwardDiagonal = 25,
        WideUpwardDiagonal = 26,
        ZigZag = 38
    }
    //public enum PresetTexture
    //{
    //    PresetTextureMixed = -2,
    //    BlueTissuePaper = 17,
    //    Bouquet = 20,
    //    BrownMarble = 11,
    //    Canvas = 2,
    //    Cork = 21,
    //    Denim = 3,
    //    FishFossil = 7,
    //    Granite = 12,
    //    GreenMarble = 9,
    //    MediumWood = 24,
    //    Newsprint = 13,
    //    Oak = 23,
    //    PaperBag = 6,
    //    Papyrus = 1,
    //    Parchment = 15,
    //    PinkTissuePaper = 18,
    //    PurpleMesh = 19,
    //    RecycledPaper = 14,
    //    Sand = 8,
    //    Stationery = 16,
    //    Walnut = 22,
    //    WaterDroplets = 5,
    //    WhiteMarble = 10,
    //    WovenMat = 4

    //}
    public enum TextureAlignment
    {
        AlignmentMixed = -2,
        Bottom = 7,
        BottomLeft = 6,
        BottomRight = 8,
        Center = 4,
        Left = 3,
        Right = 5,
        Top = 1,
        TopLeft = 0,
        TopRight = 2

    }
    public enum LineStyle
    {
        Single = 1,
        StyleMixed = -2,
        ThickBetweenThin = 5,
        ThickThin = 4,
        ThinThick = 3,
        ThinThin = 2

    }
    //public enum ArrowheadWidth
    //{
    //    ArrowheadNarrow = 1,
    //    ArrowheadWide = 3,
    //    ArrowheadWidthMedium = 2,
    //    ArrowheadWidthMixed = -2
    //}
    //public enum ArrowheadStyle
    //{
    //    ArrowheadDiamond = 5,
    //    ArrowheadNone = 1,
    //    ArrowheadOpen = 3,
    //    ArrowheadOval = 6,
    //    ArrowheadStealth = 4,
    //    ArrowheadStyleMixed = -2,
    //    ArrowheadTriangle = 2
    //}
    //public enum ArrowheadLength
    //{
    //    ArrowheadLengthMedium = 2,
    //    ArrowheadLengthMixed = -2,
    //    ArrowheadLong = 3,
    //    ArrowheadShort = 1
    //}
    //public enum LineDashStyle
    //{
    //    Dash = 4,
    //    DashDot = 5,
    //    DashDotDot = 6,
    //    DashStyleMixed = -2,
    //    LongDash = 7,
    //    LongDashDot = 8,
    //    RoundDot = 3,
    //    Solid = 1,
    //    SquareDot = 2
    //}
    
    //public enum TextureType
    //{
    //    Preset = 1,
    //    TypeMixed = -2,
    //    UserDefined = 2

    //}
    //public enum RelativeSize
    //{
    //    BottomMarginArea = 3,
    //    InnerMarginArea = 4,
    //    Margin = 0,
    //    OuterMarginArea = 5,
    //    Page = 1,
    //    TopMarginArea = 2
    //}
    //public enum RelativeVerticalPosition
    //{
    //    Line = 3,
    //    Margin = 0,
    //    Page = 1,
    //    Paragraph = 2,
    //    BottomMarginArea = 5,
    //    InnerMarginArea = 6,
    //    OuterMarginArea = 7,
    //    TopMarginArea = 4

    //}
    //public  enum RelativeHorizontalPosition
    //{
    //    Character = 3,
    //    Column = 2,
    //    Margin = 0,
    //    Page = 1,
    //    InnerMarginArea = 6,
    //    LeftMarginArea = 4,
    //    OuterMarginArea = 7,
    //    RightMarginArea = 5


    //}
    //public enum ColorType
    //{
    //    PictureAutomatic = 1,
    //    PictureBlackAndWhite = 3,
    //    PictureGrayscale = 2,
    //    PictureMixed = -2,
    //    PictureWatermark = 4
    //}
#endif
}
