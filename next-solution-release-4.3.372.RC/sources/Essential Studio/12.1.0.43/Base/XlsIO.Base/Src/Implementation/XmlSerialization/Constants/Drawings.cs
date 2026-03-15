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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// Class used for defining constants and namespace for drawings.
  /// </summary>
  internal sealed class Drawings
  {
    #region Constants
    /// <summary>
    /// Main spreadsheet drawings namespace ('xdr' abbreviation is used in MS Excel documents).
    /// </summary>
    public const string XdrNamespace = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
    /// <summary>
    /// Main drawings namespace ('a' abbreviation is used in MS Excel documents).
    /// </summary>
    public const string ANamespace = "http://schemas.openxmlformats.org/drawingml/2006/main";
    /// <summary>
    /// Main chartsheet drawings namespace ('cdr' abbreviation is used in MS Excel documents).
    /// </summary>
    public const string CdrNamespace = "http://schemas.openxmlformats.org/drawingml/2006/chartDrawing";
    /// <summary>
    /// Prefix used by MS Excel for XdrNamespace definition.
    /// </summary>
    public const string XdrPreffix = "xdr";
    /// <summary>
    /// Prefix used by MS Excel for CdrNamespace definition.
    /// </summary>
    public const string CdrPreffix = "cdr";
    /// <summary>
    /// Prefix used by MS Excel for ANamespace definition.
    /// </summary>
    public const string APreffix = "a";
    public const string AlternateContentTag = "AlternateContent";
    public const string ChoiceTag = "Choice";
    /// <summary>
    /// This element specifies a two cell anchor placeholder for a group, a shape,
    /// or a drawing element. It moves with cells and its extents are in EMU units.
    /// </summary>
    public const string TwoCellAnchorTagName = "twoCellAnchor";
    /// <summary>
    /// This element specifies a one cell anchor placeholder for a group, a shape,
    /// or a drawing element. It moves with the cell and its extents is in EMU units.
    /// </summary>
    public const string OneCellAnchorTagName = "oneCellAnchor";
    /// <summary>
    /// Specifies how the DrawingML contents shall be moved and/or resized when
    /// the rows and columns between its start and ending anchor (the from and
    /// to child elements) are resized, or have additional rows/columns inserted
    /// within them, or additional row/columns are added before them.
    /// </summary>
    public const string EditAsAttribute = "editAs";
    /// <summary>
    /// This element specifies the first anchor point for the drawing element.
    /// This will be used to anchor the top and left sides of the shape within
    /// the spreadsheet. That is when the cell that is specified in the from
    /// element is adjusted, the shape will also be adjusted.
    /// </summary>
    public const string FromTagName = "from";
    /// <summary>
    /// This element specifies the second anchor point for the drawing element.
    /// This will be used to anchor the bottom and right sides of the shape within
    /// the spreadsheet. That is when the cell that is specified in the to element
    /// is adjusted, the shape will also be adjusted.
    /// </summary>
    public const string ToTagName = "to";
    /// <summary>
    /// This element specifies the column that will be used within the from and to
    /// elements to specify anchoring information for a shape within a spreadsheet.
    /// </summary>
    public const string ColumnTagName = "col";
    /// <summary>
    /// This element is used to specify the column offset within a cell.
    /// </summary>
    public const string ColumnOffsetTagName = "colOff";
    /// <summary>
    /// This element specifies the row that will be used within the from and to
    /// elements to specify anchoring information for a shape within a spreadsheet.
    /// </summary>
    public const string RowTagName = "row";
    /// <summary>
    /// This element is used to specify the row offset within a cell.
    /// </summary>
    public const string RowOffsetTagName = "rowOff";
    /// <summary>
    /// This element specifies the non visual properties for a picture.
    /// This allows for additional information that does not affect
    /// the appearance of the picture to be stored.
    /// </summary>
    public const string NVPicturePropertiesTag = "nvPicPr";
    /// <summary>
    /// This element specifies non-visual canvas properties.
    /// </summary>
    public const string NVCanvasPropertiesTag = "cNvPr";
    /// <summary>
    /// This element specifies non-visual group shape properties.
    /// </summary>
    public const string NVGroupShapePropertiesTag = "nvGrpSpPr";
    /// <summary>
    /// This element specifies group shape properties.
    /// </summary>
    public const string GroupShapePropertiesTag = "grpSpPr";
    /// <summary>
    /// This element specifies the non-visual connector shape drawing properties
    /// </summary>
    public const string NVConnectorShapeProperties = "nvCxnSpPr";
    /// <summary>
    /// This element specifies the non-visual properties for the picture canvas.
    /// </summary>
    public const string NVPictureCanvasPropertiesTag = "cNvPicPr";
    /// <summary>
    /// This element specifies the on-click hyperlink information to be applied to a run of text.
    /// When the hyperlink text is clicked the link is fetched.
    /// </summary>
    public const string ClickHyperlinkTag = "hlinkClick";
    /// <summary>
    /// This element specifies the type of picture fill that the picture object
    /// will have. Because a picture has a picture fill already by default,
    /// it is possible to have two fills specified for a picture object.
    /// </summary>
    public const string BlipFillTagName = "blipFill";
    /// <summary>
    /// This element specifies the existence of an image (binary large image
    /// or picture) and contains a reference to the image data.
    /// </summary>
    public const string BlipTagName = "blip";
    /// <summary>
    /// It's define the Picture fill option 
    /// </summary>
    public const string AlphaModFixTag = "alphaModFix";
    /// <summary>
    /// It's define the values of Transparency
    /// </summary>
    public const string AlphaModFixattribute = "amt";
    /// <summary>
    /// Specifies the identification information for an embedded picture. This
    /// attribute is used to specify an image that resides locally within the file.
    /// </summary>
    public const string EmbeddedPicture = "embed";
    /// <summary>
    /// Specifies that the current start and end positions shall be maintained
    /// with respect to the distances from the absolute start point of the worksheet.
    /// </summary>
    public const string PositionSizeAbsolute = "absolute";
    /// <summary>
    /// Specifies that the current drawing shall move with its row and column
    /// (i.e. the object is anchored to the actual from row and column), but
    /// that the size shall remain absolute.
    /// </summary>
    public const string PositionRelative = "oneCell";
    /// <summary>
    /// Specifies that the current drawing shall move and resize to maintain its
    /// row and column anchors (i.e. the object is anchored to the actual from
    /// and to row and column).
    /// </summary>
    public const string PositionSizeRelative = "twoCell";
    /// <summary>
    /// This element specifies that a BLIP should be stretched to fill the target rectangle.
    /// </summary>
    public const string StretchTagName = "stretch";
    /// <summary>
    /// This element specifies that a BLIP should be tiled to fill the available space.
    /// </summary>
    public const string TileTagName = "tile";
    /// <summary>
    /// This element specifies the portion of the blip used for the fill.
    /// </summary>
    public const string SourceRectangleTagName = "srcRect";
    /// <summary>
    /// This element specifies a fill rectangle. When stretching of an image is specified,
    /// a source rectangle, srcRect, is scaled to fit the specified fill rectangle.
    /// </summary>
    public const string FillRectTagName = "fillRect";
    /// <summary>
    /// This element specifies the existence of a picture object within the document.
    /// </summary>
    public const string PictureTagName = "pic";
    /// <summary>
    /// Name of the xml attribute that stores id.
    /// </summary>
    public const string IdAttributeName = "id";
    /// <summary>
    /// Name of the xml attribute that stores shape name.
    /// </summary>
    public const string NameAttributeName = "name";
    /// <summary>
    /// Name of the xml attribute that stores alternative description.
    /// </summary>
    public const string DescriptionAttributeName = "descr";
    /// <summary>
    /// This element specifies all locking properties for a graphic frame.
    /// </summary>
    public const string PictureLocksTag = "picLocks";
    /// <summary>
    /// Specifies that the generating application should not allow aspect ratio
    /// changes for the corresponding connection shape. If this attribute is not
    /// specified, then a value of false is assumed.
    /// </summary>
    public const string NoChangeAspectAttribute = "noChangeAspect";
    /// <summary>
    /// This element specifies the visual shape properties that can be applied to a shape.
    /// </summary>
    public const string ShapePropertiesTag = "spPr";
    /// <summary>
    /// This element specifies the thickness of the walls or floor as a percentage of the largest dimension of the plot volume.
    /// </summary>
    public const string ThicknessTag = "thickness";
    /// <summary>
    /// It's define  picturefill in shapeproperty.
    /// </summary>
    public const string PictureoptionTag = "pictureOptions";
    /// <summary> 
    /// It's define pictureformat options
    /// </summary>
    public const string PictureformatTag = "pictureFormat";
    /// <summary>
    /// It's define the Pictureformatvalue
    /// </summary>
    public const string Valueattribite = "val";
    /// <summary>
    /// This element represents 2-D transforms for ordinary shapes.
    /// </summary>
    public const string Transform2DTag = "xfrm";
    /// <summary>
    /// This element specifies the location of the bounding box of an object.
    /// </summary>
    public const string Offset = "off";
    /// <summary>
    /// This element specifies the size of the bounding box enclosing the referenced object.
    /// </summary>
    public const string Extents = "ext";
    /// <summary>
    /// Specifies a coordinate on the x-axis.
    /// </summary>
    public const string XAttributeName = "x";
    /// <summary>
    /// Specifies a coordinate on the x-axis.
    /// </summary>
    public const string YAttributeName = "y";
    /// <summary>
    /// Specifies the length of the extents rectangle in EMUs.
    /// </summary>
    public const string CXAttributeName = "cx";
    /// <summary>
    /// Specifies the width of the extents rectangle in EMUs.
    /// </summary>
    public const string CYAttributeName = "cy";
    /// <summary>
    /// This element specifies when a preset geometric shape should be used
    /// instead of a custom geometric shape.
    /// </summary>
    public const string PresetGeometryTag = "prstGeom";
    /// <summary>
    /// Specifies the preset geometry that will be used for this shape.
    /// </summary>
    public const string PresetShapeAttribute = "prst";
    /// <summary>
    /// This element indicates that the sheet contains drawing components built
    /// on the drawingML platform.
    /// </summary>
    public const string DrawingTagName = "drawing";
    /// <summary>
    /// This element specifies all drawing objects within the worksheet.
    /// </summary>
    public const string WorksheetDrawings = "wsDr";
    /// <summary>
    /// This element specifies the adjust values that will be applied to the specified shape.
    /// </summary>
    public const string AdjustValuesList = "avLst";
    /// <summary>
    /// This element is used to set certain properties related to a drawing element
    /// on the client spreadsheet application.
    /// </summary>
    public const string ClientDataTagName = "clientData";
    /// <summary>
    /// This element describes a single graphical object frame for
    /// a spreadsheet which contains a graphical object.
    /// </summary>
    public const string GraphicFrame = "graphicFrame";
    public const string Slicer = "slicer";
    /// <summary>
    /// This element specifies the existence of a single shape. A shape can either
    /// be a preset or a custom geometry, defined using the SpreadsheetDrawingML
    /// framework. In addition to a geometry each shape can have both visual and
    /// non-visual properties attached. Text and corresponding styling information
    /// can also be attached to a shape. This shape is specified along with all
    /// other shapes within either the shape tree or group shape elements.
    /// </summary>
    public const string Shape = "sp";
    /// <summary>
    /// This element specifies the properties for a connection shape drawing element.
    /// A connection shape is a line, etc. that connects two other shapes in this drawing.
    /// </summary>
    public const string ConnectionShape = "cxnSp";
    /// <summary>
    /// This element specifies a group shape that represents many shapes grouped together.
    /// This shape is to be treated just as if it were a regular shape but instead of being
    /// described by a single geometry it is made up of all the shape geometries encompassed
    /// within it. Within a group shape each of the shapes that make up the group are
    /// specified just as they normally would. The idea behind grouping elements however
    /// is that a single transform can apply to many shapes at the same time.
    /// </summary>
    public const string GroupShape = "grpSp";
    /// <summary>
    /// This element specifies the custom function associated with the object.
    /// </summary>
    public const string MacroAttribute = "macro";
    /// <summary>
    /// This element specifies the existence of a single graphic object.
    /// </summary>
    public const string GraphicTag = "graphic";
    /// <summary>
    /// This element specifies the reference to a graphic object within the document.
    /// </summary>
    public const string GraphicDataTag = "graphicData";
    /// <summary>
    /// Specifies the URI, or uniform resource identifier that represents the data
    /// stored under this tag. The URI is used to identify the correct 'server' that
    /// can process the contents of this tag.
    /// </summary>
    public const string UriAttribute = "uri";
    /// <summary>
    /// This element specifies all non-visual properties for a graphic frame.
    /// </summary>
    public const string NonVisualGraphicFramePr = "nvGraphicFramePr";
    /// <summary>
    /// This element defines the body properties for the text body within a shape.
    /// </summary>
    public const string TextBodyPropertiesTag = "bodyPr";
    /// <summary>
    /// This element specifies the list of styles associated with this body of text.
    /// </summary>
    public const string ListStylesTag = "lstStyle";
    /// <summary>
    /// This element specifies the presence of a paragraph of text within the containing text body.
    /// </summary>
    public const string Paragraphs = "p";
    /// <summary>
    /// This element contains all paragraph level text properties for the containing paragraph.
    /// These paragraph properties should override any and all conflicting properties that are
    /// associated with the paragraph in question.
    /// </summary>
    public const string ParagraphProperties = "pPr";
    /// <summary>
    /// This element contains all default run level text properties for the text
    /// runs within a containing paragraph. These properties are to be used when
    /// overriding properties have not been defined within the rPr element.
    /// </summary>
    public const string DefaultParagraphProperites = "defRPr";
    /// <summary>
    /// This element specifies the presence of a run of text within the containing text body.
    /// </summary>
    public const string ParagraphRun = "r";
    public const string ParagraphEndProperties = "endParaRPr";
    public const string RotationAttribute = "rot";
    /// <summary>
    /// This element specifies the actual text for this text run. This is the
    /// text that will be formatted using all specified body, paragraph and run
    /// properties. This element must be present within a run of text.
    /// </summary>
    public const string ParagraphText = "t";
    /// <summary>
    /// This element contains all run level text properties for the text runs within a containing paragraph.
    /// </summary>
    public const string TextRunProperites = "rPr";
    /// <summary>
    /// Specifies whether a run of text will be formatted as bold text. If this
    /// attribute is omitted, than a value of 0, or false is assumed.
    /// </summary>
    public const string FontBoldAttribute = "b";
    /// <summary>
    /// Specifies whether a run of text will be formatted as italic text. If this
    /// attribute is omitted, than a value of 0, or false is assumed.
    /// </summary>
    public const string FontItalicAttribute = "i";
    /// <summary>
    /// Specifies whether a run of text will be formatted as strikethrough text.
    /// If this attribute is omitted, than no strikethrough is assumed.
    /// </summary>
    public const string FontStrikeAttribute = "strike";
    /// <summary>
    /// Specifies the size of text within a text run. Whole points are specified
    /// in increments of 100 starting with 100 being a point size of 1. For instance
    /// a font point size of 12 would be 1200 and a font point size of 12.5 would be
    /// 1250. If this attribute is omitted, than the value in defRPr should be used.
    /// </summary>
    public const string FontSizeAttribute = "sz";
    /// <summary>
    /// Specifies whether a run of text will be formatted as underlined text.
    /// If this attribute is omitted, than no underline is assumed.
    /// </summary>
    public const string FontUnterlineAttribute = "u";
    /// <summary>
    /// This element is used as an anchor placeholder for a shape or group of shapes.
    /// It will anchor the object in the same position relative to sheet position
    /// and its extents are in EMU unit.
    /// </summary>
    public const string AbsoluteAnchorTag = "absoluteAnchor";
    /// <summary>
    /// This element describes the position of a drawing element within a spreadsheet.
    /// </summary>
    public const string PositionTag = "pos";
    /// <summary>
    /// Specifies that the generating application should not allow shape grouping
    /// for the corresponding connection shape.
    /// </summary>
    public const string NoShapeGrouping = "noGrp";
    /// <summary>
    /// This element specifies all locking properties for a graphic frame.
    /// </summary>
    public const string GraphicFrameLocksTag = "graphicFrameLocks";
    /// <summary>
    /// This element specifies the non-visual drawing properties for a graphic frame.
    /// </summary>
    public const string CNVGraphicFramePr = "cNvGraphicFramePr";
    /// <summary>
    /// This element specifies an outline style that can be applied to a number
    /// of different objects such as shapes and text.
    /// </summary>
    public const string LineTag = "ln";
    /// <summary>
    /// Specifies the width to be used for the underline stroke. If this
    /// attribute is omitted, then a value of 0 is assumed.
    /// </summary>
    public const string LineWidthAttribute = "w";
    /// <summary>
    /// Specifies the compound line type to be used for the underline stroke.
    /// If this attribute is omitted, then a value of sng is assumed.
    /// </summary>
    public const string CompoundLineTypeAttribute = "cmpd";
    /// <summary>
    /// This element specifies that no fill will be applied to the parent element.
    /// </summary>
    public const string NoFillTag = "noFill";
    /// <summary>
    /// This element specifies a solid color fill. The shape is filled entirely with the specified color.
    /// </summary>
    public const string SolidFillTag = "solidFill";
    /// <summary>
    /// border miter join Tag
    /// </summary>
    public const string MiterJoinTag = "miter";
    /// <summary>
    /// border bevel join tag
    /// </summary>
    public const string BevelJoinTag = "bevel";
    /// <summary>
    /// This element specifies a color using the red, green, blue RGB color model.
    /// Red, green, and blue is expressed as sequence of hex digits, RRGGBB.
    /// A perceptual gamma of 2.2 is used.
    /// </summary>
    public const string SRGBColorTag = "srgbClr";
    /// <summary>
    /// This element specifies a color bound to a user's theme. As with all elements
    /// which define a color, it is possible to apply a list of color transforms to
    /// the base color defined.
    /// </summary>
    public const string SchemeColorTag = "schemeClr";
    /// <summary>
    /// This color is based upon the value that this color currently has
    /// within the system on which the document is being viewed.
    /// </summary>
    public const string SystemColorTag = "sysClr";
    /// <summary>
    /// Applications shall use the lastClr attribute to determine
    /// the absolute value of the last color used if system colors
    ///are not supported.
    /// </summary>
    public const string SystemColorHexAttribute = "lastClr";
    /// <summary>
    /// This element specifies that a preset line dashing scheme should be used.
    /// </summary>
    public const string PresetDashTag = "prstDash";
    /// <summary>
    /// This element specifies the background color of a Pattern fill.
    /// </summary>
    public const string BackgroundColorTag = "bgClr";
    /// <summary>
    /// This element specifies the foreground color of a pattern fill.
    /// </summary>
    public const string ForegroundColorTag = "fgClr";
    /// <summary>
    /// This element specifies a pattern fill. A repeated pattern is used to fill the object.
    /// </summary>
    public const string PatternFillTag = "pattFill";
    /// <summary>
    /// Specifies one of a set of preset patterns to fill the object.
    /// </summary>
    public const string PresetPattern = "prst";
    /// <summary>
    /// This element specifies that lines joined together will have a round join.
    /// </summary>
    public const string RoundTag = "round";
    /// <summary>
    /// This element defines a gradient fill.
    /// </summary>
    public const string GradientFillTag = "gradFill";
    /// <summary>
    /// This element specifies decorations which can be added to the head of a line.
    /// </summary>
    public const string HeadEnd = "headEnd";
    /// <summary>
    /// This element specifies decorations which can be added to the tail of a line.
    /// </summary>
    public const string TailEnd = "tailEnd";
    /// <summary>
    /// The list of gradient stops that specifies the gradient colors and their
    /// relative positions in the color band.
    /// </summary>
    public const string GradientStopsTag = "gsLst";
    /// <summary>
    /// This element specifies a linear gradient.
    /// </summary>
    public const string GradientLiniarTag = "lin";
    /// <summary>
    /// It's define the Tailtag property 
    /// </summary>
    public const string GradientTailTag = "tileRect";
    /// <summary>
    /// Specifies the direction of color change for the gradient. To define this
    /// angle, let its value be x measured clockwise. Then ( -sin x, cos x ) is
    /// a vector parallel to the line of constant color in the gradient fill.
    /// </summary>
    public const string GradientAngleAttribute = "ang";
    /// <summary>
    /// Whether the gradient angle scales with the fill region.
    /// </summary>
    public const string GradientScaledAttribute = "scaled";
    /// <summary>
    /// This element defines that a gradient fill will follow a path vs. a linear line.
    /// </summary>
    public const string GradientPathTag = "path";
    /// <summary>
    /// Specifies the shape of the path to follow.
    /// </summary>
    public const string GradientPathAttribute = "path";
    /// <summary>
    /// This element defines a gradient stop. A gradient stop consists of a position
    /// where the stop appears in the color band.
    /// </summary>
    public const string GradientStopTag = "gs";
    /// <summary>
    /// Specifies where this gradient stop should appear in the color band.
    /// </summary>
    public const string GradientPositionAttribute = "pos";
    /// <summary>
    /// This element specifies its input color with the specific opacity,
    /// but with its color unchanged.
    /// </summary>
    public const string AlphaTag = "alpha";
    /// <summary>
    /// This element defines the "focus" rectangle for the center shade, specified
    /// relative to the fill tile rectangle. The center shade fills the entire tile
    /// except the margins specified by each attribute.
    /// </summary>
    public const string FillToRectTag = "fillToRect";
    /// <summary>
    /// This element specifies a Color Change Effect. Instances of clrFrom are replaced with instances of clrTo
    /// </summary>
    public const string ColorChangeTag = "clrChange";
    /// <summary>
    /// Specifies the left edge of the rectangle.
    /// </summary>
    public const string LeftAttribute = "l";
    /// <summary>
    /// Specifies the top edge of the rectangle.
    /// </summary>
    public const string TopAttribute = "t";
    /// <summary>
    /// Specifies the right edge of the rectangle.
    /// </summary>
    public const string RightAttribute = "r";
    /// <summary>
    /// Specifies the bottom edge of the rectangle.
    /// </summary>
    public const string BottomAttribute = "b";
    /// <summary>
    /// This element specifies a lighter version of its input color. A 10% tint is 10%
    /// of the input color combined with 90% white.
    /// </summary>
    public const string TintTag = "tint";
    /// <summary>
    /// This element specifies a darker version of its input color. A 10% shade is 10%
    /// of the input color combined with 90% black.
    /// </summary>
    public const string ShadeTag = "shade";
    /// <summary>
    /// This element specifies that the output color rendered by the generating
    /// application should be the sRGB gamma shift of the input color.
    /// </summary>
    public const string GammaTag = "gamma";
    /// <summary>
    /// This element specifies that the output color rendered by the generating
    /// application should be the inverse sRGB gamma shift of the input color.
    /// </summary>
    public const string InverseGammaTag = "invGamma";
    /// <summary>
    /// This element specifies the input color with its luminance modulated by the given percentage.
    /// </summary>
    public const string LuminanceModulation = "lumMod";
    /// <summary>
    /// This element specifies the input color with its luminance shifted, but with its hue and saturation unchanged.
    /// </summary>
    public const string LuminanceOffset = "lumOff";
    /// <summary>
    /// This element specifies the input color with its saturation modulated by the given percentage.
    /// </summary>
    public const string SaturationModulation = "satMod";
    /// <summary>
    /// This element specified the rotation of text.
    /// </summary>
    public const string TextRotationAttribute = "rot";
    /// <summary>
    /// Specifies that all text in the parent object shall be aligned to the baseline of 
    /// each character when displayed.
    /// </summary>
    public const string Baseline = "baseline";
    /// <summary>
    /// This element specifies that a Latin font be used for a specific run of text.
    /// </summary>
    public const string LatinTag = "latin";
    public const string EaTag = "ea";
    public const string CsTag = "cs";
    /// <summary>
    /// Represents font type face.
    /// </summary>
    public const string TypefaceTag = "typeface";
    /// <summary>
    /// Represents embedded controls in worksheet.
    /// </summary>
    public const string ControlsTag = "controls";
    /// <summary>
    /// Represents a single embedded control.
    /// </summary>
    public const string ControlTag = "control";
    /// <summary>
    /// This element specifies the non-visual drawing properties for a shape. These properties
    /// are to be used by the generating application to determine how the shape should be dealt with.
    /// </summary>
    public const string NonVisualDrawingProperties = "cNvSpPr";
    /// <summary>
    /// Specifies that the corresponding shape is a text box and thus should be treated
    /// as such by the generating application. If this attribute is omitted then it is
    /// assumed that the corresponding shape is not specifically a text box.
    /// </summary>
    public const string TextBoxAttribute = "txBox";
    /// <summary>
    /// This element specifies all non-visual properties for a shape. This element
    /// is a container for the non-visual identification properties, shape properties
    /// and application properties that are to be associated with a shape. This
    /// allows for additional information that does not affect the appearance
    /// of the shape to be stored.
    /// </summary>
    public const string NonVisualShapeProperties = "nvSpPr";
    /// <summary>
    /// This element specifies the existence of text to be contained within the
    /// corresponding shape. All visible text and visible text related properties
    /// are contained within this element. There can be multiple paragraphs and
    /// within paragraphs multiple runs of text.
    /// </summary>
    public const string TextBody = "txBody";
    /// <summary>
    /// Default value for subscript baseline value.
    /// </summary>
    internal const int SubscriptBaseline = -25000;
    /// <summary>
    /// Default value for superscript baseline value.
    /// </summary>
    internal const int SuperscriptBaseline = 30000;
    /// <summary>
    /// Specifies the anchoring position of the txBody within the shape. If this
    /// attribute is omitted, then a value of t, or top is implied.
    /// </summary>
    public const string AnchorAttribute = "anchor";
    /// <summary>
    /// Determines if the text within the given text body should be displayed vertically.
    /// If this attribute is omitted, then a value of horz, or no vertical text is implied.
    /// </summary>
    public const string TextBoxRotationAttribute = "vert";
    /// <summary>
    /// This attribute indicates whether to allow text editing within this drawing
    /// object when the parent worksheet is protected.
    /// </summary>
    public const string LockTextAttribute = "fLocksText";
    /// <summary>
    /// Specifies the alignment that is to be applied to the paragraph. Possible
    /// values for this include left, right, centered, justified and distributed.
    /// If this attribute is omitted, then a value of left is implied.
    /// </summary>
    public const string HorizontalAlignment = "algn";
    public const string FontLanguage = "lang";

    //specifies the shadow constant for outer
    public const string EffectListTag = "effectLst";
    public const string OuterShadowTag = "outerShdw";
    public const string PresetcolorTag = "prstClr";
    public const string BlurRadiusTag = "blurRad";
    public const string DistanceTag = "dist";
    public const string DirectionTag = "dir";
    public const string AlignmentTag = "algn";
    public const string RotationwithShapeTag = "rotWithShape";
    public const string SizeX = "sx";
    public const string SizeY = "sy";
    public const string OuterAlphaTag = "40000";

    //specifies the shadow constant for inner

    public const string InnerShadowTag = "innerShdw";
    public const string InnerAlphaTag = "50000";


    //specifies the shadow constant for perspective

    public const string KXTag = "kx";
    public const string PerspectiveAlphaTag = "20000";
    public const string BelowAlphaTag = "15000";

    public const string Special3DTag = "sp3d";
    public const string BevelTopTag = "bevelT";
    public const string BevelBottomTag = "bevelB";

    public const string LineHeightAttribute = "h";
    public const string Scene3DTag = "scene3d";
    public const string CameraTag = "camera";
    public const string ViewTag = "orthographicFront";
    public const string LightingTag = "lightRig";
    public const string LightingRightTag = "rig";

    public const string HiddenAttribute = "hidden";
    public const string HorizontalOffsetTag = "tx";
    public const string VerticalOffsetTag = "ty";
    public const string HorizontalRatioTag = "sx";
    public const string VerticalRatioTag = "sy";  
    public const string TileFlippingTag = "flip";
    #endregion
  }
}
