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

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for _MsoEnums.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum MsoRecords
  {
    /// <summary>
    /// Represents the msofbtDggContainer MsoRecord type.
    /// </summary>
    msofbtDggContainer    = 0xF000,
    /// <summary>
    /// Represents the Dgg type.
    /// </summary>
    msofbtDgg             = 0xF006,
    /// <summary>
    /// Represents the CLSID type.
    /// </summary>
    msofbtCLSID           = 0xF016,
    /// <summary>
    /// Represents the OPT type.
    /// </summary>
    msofbtOPT             = 0xF00B,
    /// <summary>
    /// Represents the ColorMRU type.
    /// </summary>
    msofbtColorMRU        = 0xF11A,
    /// <summary>
    /// Represents the SplitMenuColors type.
    /// </summary>
    msofbtSplitMenuColors = 0xF11E,
    /// <summary>
    /// Represents the BstoreContainer type.
    /// </summary>
    msofbtBstoreContainer = 0xF001,
    /// <summary>
    /// Represents the BSE type.
    /// </summary>
    msofbtBSE             = 0xF007,
    /// <summary>
    /// Represents the DgContainer type.
    /// </summary>
    msofbtDgContainer     = 0xF002,
    /// <summary>
    /// Represents the Dg type.
    /// </summary>
    msofbtDg              = 0xF008,
    /// <summary>
    /// Represents the RegroupItems type.
    /// </summary>
    msofbtRegroupItems    = 0xF118,
    /// <summary>
    /// Represents the ColorScheme type.
    /// </summary>
    msofbtColorScheme     = 0xF120,
    /// <summary>
    /// Represents the SpgrContainer type.
    /// </summary>
    msofbtSpgrContainer   = 0xF003,
    /// <summary>
    /// Represents the SpContainer type.
    /// </summary>
    msofbtSpContainer     = 0xF004,
    /// <summary>
    /// Represents the Spgr type.
    /// </summary>
    msofbtSpgr            = 0xF009,
    /// <summary>
    /// Represents the Sp type.
    /// </summary>
    msofbtSp              = 0xF00A,
    /// <summary>
    /// Represents the Textbox type.
    /// </summary>
    msofbtTextbox         = 0xF00C,
    /// <summary>
    /// Represents the ClientTextbox type.
    /// </summary>
    msofbtClientTextbox   = 0xF00D,
    /// <summary>
    /// Represents the Anchor type.
    /// </summary>
    msofbtAnchor          = 0xF00E,
    /// <summary>
    /// Represents the ChildAnchor type.
    /// </summary>
    msofbtChildAnchor     = 0xF00F,
    /// <summary>
    /// Represents the ClientAnchor type.
    /// </summary>
    msofbtClientAnchor    = 0xF010,
    /// <summary>
    /// Represents the ClientData type.
    /// </summary>
    msofbtClientData      = 0xF011,
    /// <summary>
    /// Represents the OleObject type.
    /// </summary>
    msofbtOleObject       = 0xF11F,
    /// <summary>
    /// Represents the DeletedPspl type.
    /// </summary>
    msofbtDeletedPspl     = 0xF11D,
    /// <summary>
    /// Represents the SolverContainer type.
    /// </summary>
    msofbtSolverContainer = 0xF005,
    /// <summary>
    /// Represents the ConnectorRule type.
    /// </summary>
    msofbtConnectorRule   = 0xF012,
    /// <summary>
    /// Represents the AlignRule type.
    /// </summary>
    msofbtAlignRule       = 0xF013,
    /// <summary>
    /// Represents the ArcRule type.
    /// </summary>
    msofbtArcRule         = 0xF014,
    /// <summary>
    /// Represents the ClientRule type.
    /// </summary>
    msofbtClientRule      = 0xF015,
    /// <summary>
    /// Represents the CalloutRule type.
    /// </summary>
    msofbtCalloutRule     = 0xF017,
    /// <summary>
    /// Represents the Selection type.
    /// </summary>
    msofbtSelection       = 0xF119,
    /// <summary>
    /// Represents the Unknown type.
    /// </summary>
    msoUnknown            = 0xFFFF,
  }
  /// <summary>
  /// Represents the MsoBlipUsage options.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum MsoBlipUsage
  {
    /// <summary>
    /// Represents the  Default option.
    /// </summary>
    msoblipUsageDefault,
    /// <summary>
    /// Represents the Texture option.
    /// </summary>
    msoblipUsageTexture,
    /// <summary>
    /// Represents the UsageMax option.
    /// </summary>
    msoblipUsageMax = 255
  } ;

  /// <summary>
  /// Represents the MsoBlipType options.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum MsoBlipType
  {
    /// <summary>
    /// Represents the ERROR option.
    /// </summary>
    msoblipERROR = 0,
    /// <summary>
    /// Represents the UNKNOWN option.
    /// </summary>
    msoblipUNKNOWN,
    /// <summary>
    /// Represents the EMF option.
    /// </summary>
    msoblipEMF,
    /// <summary>
    /// Represents the WMF option.
    /// </summary>
    msoblipWMF,
    /// <summary>
    /// Represents the PICT option.
    /// </summary>
    msoblipPICT,
    /// <summary>
    /// Represents the JPEG option.
    /// </summary>
    msoblipJPEG,
    /// <summary>
    /// Represents the PNG option.
    /// </summary>
    msoblipPNG,
    /// <summary>
    /// Represents the DIB option.
    /// </summary>
    msoblipDIB,
    /// <summary>
    /// Represents the FirstClient option.
    /// </summary>
    msoblipFirstClient = 32,
    /// <summary>
    /// Represents the LastClient option.
    /// </summary>
    msoblipLastClient  = 255
  };

  /// <summary>
  /// Represents the MsoBlipCompression options.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum MsoBlipCompression
  {
    /// <summary>
    /// Represents the Deflate option.
    /// </summary>
    msoCompressionDeflate = 0,
    /// <summary>
    /// Represents the None option.
    /// </summary>
    msoCompressionNone    = 254,
    /// <summary>
    /// Represents the Test option.
    /// </summary>
    msoCompressionTest    = 255,
  };

  /// <summary>
  /// Represents the MsoBlipFilter options.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum MsoBlipFilter
  {
    /// <summary>
    /// Represents the Adaptive option.
    /// </summary>
    msofilterAdaptive = 0,
    /// <summary>
    /// Represents the None option.
    /// </summary>
    msofilterNone = 254,
    /// <summary>
    /// Represents the Test option.
    /// </summary>
    msofilterTest = 255,
  };

  /// <summary>
  /// Represents the Mso options.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum MsoOptions
  {
    /// <summary>
    /// Do not group this shape.
    /// </summary>
    LockAgainstGrouping = 127,
    /// <summary>
    /// Represents the TextId options.
    /// </summary>
    TextId              = 128,
    /// <summary>
    /// Represents the wrap text options . 
    /// </summary>
    WrapText            = 133,
    /// <summary>
    /// Represents the TextDirection options.
    /// </summary>
    TextDirection       = 139,
    /// <summary>
    /// Represents the SizeTextToFitShape options.
    /// </summary>
    SizeTextToFitShape  = 191,
    /// <summary>
    /// Represents the BlipId options.
    /// </summary>
    BlipId              = 260,
    /// <summary>
    /// Represents the BlipName options.
    /// </summary>
    BlipName            = 261,
    /// <summary>
    /// Represents fill type.
    /// </summary>
    FillType            = 384,
    /// <summary>
    /// Represents the location of the top of the crop rectangle
    /// </summary>
    CropFromTop=256,
    /// <summary>
    /// Represents the location of the bottom of the crop rectangle.
    /// </summary>
    CropFromBottom=257,
    /// <summary>
    /// Represents the location of the left side of the crop rectangle.
    /// </summary>
    CropFromLeft=258,
    /// <summary>
    /// Represents the location of the right side of the crop rectangle.
    /// </summary>
    CropFromRight=259,
    /// <summary>
    /// Host-defined ID for OLE objects (usually a pointer).
    /// </summary>
    PictureId           = 267,
    /// <summary>
    /// Represents the ForeColor options.
    /// </summary>
    ForeColor           = 385,
    /// <summary>
    /// Represents the transparency.
    /// </summary>
    Transparency        = 386,
    /// <summary>
    /// Represents the BackColor options.
    /// </summary>
    BackColor           = 387,
    /// <summary>
    /// Represents the gradient transparency options.
    /// </summary>
    GradientTransparency           = 388,
    /// <summary>
    /// Represents shape pattern.
    /// </summary>
    PatternTexture             = 390,
    /// <summary>
    /// Represents shape pattern, texture name.
    /// </summary>
    PattTextName             = 391,
    /// <summary>
    /// Represents shape gradient shading style.
    /// </summary>
    ShadStyle             = 395,
    /// <summary>
    /// Represents shape gradient shading variants.
    /// </summary>
    ShadVariant             = 396,
    /// <summary>
    /// Represents first record for shape gradient shading style.
    /// </summary>
    ShadingStyleCorner_1             = 397,
    /// <summary>
    /// Represents second record for shape gradient shading style.
    /// </summary>
    ShadingStyleCorner_2             = 398,
    /// <summary>
    /// Represents third record for shape gradient shading style.
    /// </summary>
    ShadingStyleCorner_3             = 399,
    /// <summary>
    /// Represents fourth record for shape gradient shading style.
    /// </summary>
    ShadingStyleCorner_4             = 400,
    /// <summary>
    /// Represents preset gradient data.
    /// </summary>
    PresetGradientData             = 407,
    /// <summary>
    /// Represents gradient color type.
    /// </summary>
    GradientColorType              = 412,
    /// <summary>
    /// Hit test a shape as though filled.
    /// </summary>
    NoFillHitTest       = 447,
    /// <summary>
    /// Line color.
    /// </summary>
    LineColor           = 448,
    /// <summary>
    /// Line color.
    /// </summary>
    LineTransparency           = 449,
    /// <summary>
    /// Line Weight.
    /// </summary>
    LineWeight          = 459,
    /// <summary>
    /// Line color.
    /// </summary>
    LineBackColor           = 450,
    /// <summary>
    /// Contain line pattern.
    /// </summary>
    ContainLinePattern           = 452,
    /// <summary>
    /// Line pattern.
    /// </summary>
    LinePattern           = 453,
    /// <summary>
    /// Line style
    /// </summary>
    LineStyle           = 461,
    /// <summary>
    /// Line dash style
    /// </summary>
    LineDashStyle           = 462,
    /// <summary>
    /// Line start arrow.
    /// </summary>
    LineStartArrow      = 464,
    /// <summary>
    /// Line end arrow.
    /// </summary>
    LineEndArrow        = 465,
    /// <summary>
    /// Start arrow width.
    /// </summary>
    StartArrowWidth     = 466,
    /// <summary>
    /// Start arrow len.
    /// </summary>
    StartArrowLength    = 467,
    /// <summary>
    /// End arrow width.
    /// </summary>
    EndArrowWidth       = 468,
    /// <summary>
    /// End arrow len.
    /// </summary>
    EndArrowLength      = 469,
    /// <summary>
    /// Represents if dot value is round.
    /// </summary>
    ContainRoundDot     = 471,
    /// <summary>
    /// Represents the NoLineDrawDash options.
    /// </summary>
    NoLineDrawDash      = 511,
    /// <summary>
    /// Represents the ForeShadowColor options.
    /// </summary>
    ForeShadowColor     = 513,
    /// <summary>
    /// Excel5-style shadow.
    /// </summary>
    ShadowObscured      = 575,
    /// <summary>
    /// Name of the shape (only if explicitly set).
    /// </summary>
    ShapeName           = 896,
    /// <summary>
    /// Alternative text.
    /// </summary>
    AlternativeText     = 897,
    /// <summary>
    /// Represents in comment shape show always property.
    /// </summary>
    CommentShowAlways   = 959
  }

}
