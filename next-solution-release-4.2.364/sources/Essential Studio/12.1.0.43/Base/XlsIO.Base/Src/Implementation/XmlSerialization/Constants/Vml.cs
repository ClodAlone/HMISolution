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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// This class contains constants required for vml shapes parsing and serialization.
  /// </summary>
  public sealed class Vml
  {
    /// <summary>
    /// Name of a namespace used by VML (marked by 'v' in MS Excel 2007 documents).
    /// </summary>
    public const string VNamespace = "urn:schemas-microsoft-com:vml";
    /// <summary>
    /// Name of a namespace used by VML (marked by 'o' in MS Excel 2007 documents).
    /// </summary>
    public const string ONamespace = "urn:schemas-microsoft-com:office:office";
    /// <summary>
    /// Name of a namespace used by VML (marked by 'x' in MS Excel 2007 documents).
    /// </summary>
    public const string XNamespace = "urn:schemas-microsoft-com:office:excel";
    /// <summary>
    /// Namespace prefix used in vml shapes.
    /// </summary>
    public const string VPreffix = "v";
    /// <summary>
    /// Namespace prefix used in vml shapes.
    /// </summary>
    public const string OPreffix = "o";
    /// <summary>
    /// Namespace prefix used in vml shapes.
    /// </summary>
    public const string XPreffix = "x";
    /// <summary>
    /// Root tag for vml shapes.
    /// </summary>
    public const string XmlTagName = "xml";
    /// <summary>
    /// Name of the xml tag that represents shape type definition in VML.
    /// </summary>
    public const string ShapeTypeTagName = "shapetype";
    /// <summary>
    /// String format used to create shape type id. {0} must be replaced with instance
    /// field of the shape object.
    /// </summary>
    public const string ShapeTypeIdFormat = "_x0000_t{0}";
    /// <summary>
    /// String format used to create shape id. {0} must be replaced with correct shape id number.
    /// </summary>
    public const string ShapeIdFormat = "_x0000_s{0}";
    /// <summary>
    /// Name of the xml attribute that stores shape type id.
    /// </summary>
    public const string ShapeIdAttributeName = "id";
    public const string SpIdAttributeName = "spid";
    /// <summary>
    /// Name of the xml attribute that stores shape (or shape type) coordinate size.
    /// </summary>
    public const string CoordSizeAttributeName = "coordsize";
    /// <summary>
    /// Default value of the coord size for comment shape.
    /// </summary>
    public const string CommentCoordSize = "21600,21600";
    /// <summary>
    /// Name of the xml attribute that stores spt option (it looks like it equals to instance field of the shape).
    /// </summary>
    public const string SptAttriubteName = "spt";
    /// <summary>
    /// Name of the xml attribute that stores shape path value.
    /// </summary>
    public const string PathAttributeName = "path";
    /// <summary>
    /// Default value of the path value for comment shape.
    /// </summary>
    public const string CommentPathValue = "m,l,21600r21600,l21600,xe";
    /// <summary>
    /// Default value of the path value for bitmap shape.
    /// </summary>
    public const string BitmapPathValue = "m@4@5l@4@11@9@11@9@5xe";
    /// <summary>
    /// Name of the xml tag that stores vml shape client data.
    /// </summary>
    public const string ClientDataTagName = "ClientData";
    /// <summary>
    /// Name of the xml attribute that stores vml shape type.
    /// </summary>
    public const string ObjectTypeAttribute = "ObjectType";
    /// <summary>
    /// Name of the xml tag that indicates whether shape should be moved with cells or not.
    /// </summary>
    public const string MoveWithCellsTagName = "MoveWithCells";
    /// <summary>
    /// Name of the xml tag that indicates whether shape should be sized with cells or not.
    /// </summary>
    public const string SizeWithCellsTagName = "SizeWithCells";
    /// <summary>
    /// Name of the xml tag that stores anchor settings.
    /// </summary>
    public const string AnchorTagName = "Anchor";
    /// <summary>
    /// Name of the xml tag that stores all shape settings.
    /// </summary>
    public const string ShapeTagName = "shape";
    /// <summary>
    /// Name of the xml tag that stores all shape layout settings.
    /// </summary>
    public const string ShapeLayoutTagName = "shapelayout";
    /// <summary>
    /// Name of the xml tag that stores shape type attribute.
    /// </summary>
    public const string TypeAttributeName = "type";
    /// <summary>
    /// Name of the xml tag that stores information about vml drawings inside worksheet part.
    /// </summary>
    public const string LegacyDrawing = "legacyDrawing";
    /// <summary>
    /// Name of the xml tag that stores information about vml header/footer drawings inside
    /// worksheet part.
    /// </summary>
    public const string LegacyDrawingHF = "legacyDrawingHF";
    /// <summary>
    /// Name of the xml tag that stores row index of the note shape.
    /// </summary>
    public const string RowTagName = "Row";
    /// <summary>
    /// Name of the xml tag that stores column index of the note shape.
    /// </summary>
    public const string ColumnTagName = "Column";
    /// <summary>
    /// Style attribute for vml shapes.
    /// </summary>
    public const string StyleAttribute = "style";
    /// <summary>
    /// Name of the xml attribute that stores vml shape fill color value.
    /// </summary>
    public const string FillColorAttribute = "fillcolor";
    /// <summary>
    /// Name of the xml tag that stores vml shape shadows settings.
    /// </summary>
    public const string ShadowTagName = "shadow";
    /// <summary>
    /// Name of the xml attribute that specifies whether to show a shadow.
    /// </summary>
    public const string ShadowOnAttribute = "on";
    /// <summary>
    /// Name of the xml attribute that specifies whether a shadow is transparent.
    /// Default is false. If true, the shadow is transparent if there is no fill on the shape.
    /// </summary>
    public const string ShadowObscuredAttribute = "obscured";
    /// <summary>
    /// Name of the xml attribute that specifies the color of the primary shadow.
    /// Default is gray (RGB 128,128,128).
    /// </summary>
    public const string ShadowColorAttribute = "color";
    /// <summary>
    /// Specifies text alignment.
    /// </summary>
    public const string CommentAlignment = "\n  <v:textbox style='mso-direction-alt:auto'>\n   <div style='text-align:left'></div>\n  </v:textbox>\n";
    /// <summary>
    /// Name of xml attribute that specifies whether the application calculates the
    /// internal text margin instead of using the inset attribute. Default is custom.
    /// This attribute is only meaningful for text boxes.
    /// </summary>
    public const string InsetModeAttribute = "insetmode";
    /// <summary>
    /// Name of the xml tag that stores some of text box settings.
    /// </summary>
    public const string TextBoxTagName = "textbox";
    /// <summary>
    /// Name of the xml tag that stores div settings.
    /// </summary>
    public const string DivTagName = "div";
    /// <summary>
    /// Part of the style attribute value that defines whether shape is visible or hidden.
    /// </summary>
    public const string VisibilityAttribute = "visibility";
    /// <summary>
    /// One of possible visibility values. Indicates that shape is hidden.
    /// </summary>
    public const string VisibilityHiddenValue = "hidden";
    /// <summary>
    /// Name of the xml tag that specifies whether text is locked or not.
    /// </summary>
    public const string LockText = "LockText";
    /// <summary>
    /// Name of the xml tag that specifies the horizontal text alignment for the object.
    /// </summary>
    public const string TextHAlign = "TextHAlign";
    /// <summary>
    /// Name of the xml tag that specifies the vertical text alignment for the object.
    /// </summary>
    public const string TextVAlign = "TextVAlign";
    /// <summary>
    /// This element is used to draw an image that has been loaded from an external source.
    /// </summary>
    public const string ImageDataTag = "imagedata";
    /// <summary>
    /// Specifies the relationship ID of the relationship to the image.
    /// </summary>
    public const string RelationId = "relid";
    /// <summary>
    /// Determines the flow of the text layout in a textbox.
    /// </summary>
    public const string LayoutFlow = "layout-flow";
    /// <summary>
    /// CheckStateChanged of layout-flow attribute which indicates that text is displayed vertically.
    /// </summary>
    public const string LayoutFlowVertical = "vertical";
    /// <summary>
    /// Specifies the alternate layout flow for text in textboxes.
    /// </summary>
    public const string MsoLayoutFlow = "mso-layout-flow-alt";
    /// <summary>
    /// Specifies the top to bottom layout flow for text in textboxes.
    /// </summary>
    public const string MsoLayoutFlowTopToBottom = "top-to-bottom";
    /// <summary>
    /// /// Specifies the bottom to top layout flow for text in textboxes.
    /// </summary>
    public const string MsoLayoutFlowBottomToTop = "bottom-to-top";
    /// <summary>
    /// Specifies whether the shape will stretch to fit the text in the textbox
    /// </summary>
    public const string MsoFitShapeToText = "mso-fit-shape-to-text";
    /// <summary>
    /// Represents true expression
    /// </summary>
    public const string TrueExpression = "t";
    /// <summary>
    /// Represents false expression
    /// </summary>
    public const string FalseExpression = "f";
    /// <summary>
    /// This element defines a set of formulas whose calculated values are referenced by other attributes.
    /// </summary>
    public const string FormulasTagName = "formulas";
    /// <summary>
    /// This element defines a single value as the result of the evaluation of an expression.
    /// </summary>
    public const string SingleFormulaTagName = "f";
    /// <summary>
    /// Specifies a single formula, which consists of a named operation followed
    /// by up to three parameters.
    /// </summary>
    public const string EquationTagName = "eqn";
    /// <summary>
    /// This element defines the path that makes up the shape.
    /// </summary>
    public const string ShapePathTagName = "path";
    /// <summary>
    /// Specifies whether an extrusion is allowed to be displayed.
    /// </summary>
    public const string Extrusionok = "extrusionok";
    /// <summary>
    /// Specifies whether a gradient path will be made up of repeated concentric paths.
    /// </summary>
    public const string GradientShapeOk = "gradientshapeok";
    /// <summary>
    /// Specifies the type of connection points used for attaching shapes to other shapes.
    /// </summary>
    public const string ConnectType = "connecttype";
    /// <summary>
    /// Specifies whether the original size of an object is saved after reformatting.
    /// </summary>
    public const string PreferRelative = "preferrelative";
    /// <summary>
    /// Specifies whether the closed path will be filled.
    /// </summary>
    public const string FilledAttribute = "filled";
    /// <summary>
    /// Specifies whether the path defining the shape is stroked with a solid line.
    /// </summary>
    public const string StrokedAttribute = "stroked";
    /// <summary>
    /// This element describes how to draw the path if something beyond solid line
    /// with a solid color is desired.
    /// </summary>
    public const string Stroke = "stroke";
    /// <summary>
    /// Specifies the join style for line ends.
    /// </summary>
    public const string JoinStyle = "joinstyle";
    /// <summary>
    /// This element specifies locks against actions that can be effected in the UI
    /// of an authoring application or programmatically through an object model.
    /// </summary>
    public const string Lock = "lock";
    /// <summary>
    /// Specifies an optional value that indicates how applications that implement
    /// VML should interpret extensions not defined as part of the original
    /// specification of core VML.
    /// </summary>
    public const string Ext = "ext";
    /// <summary>
    /// Specifies whether the aspect ratio of a shape is locked from being edited.
    /// </summary>
    public const string AspectRatio = "aspectratio";
    /// <summary>
    /// Tag that stores checkbox checked state.
    /// </summary>
    public const string Checked = "Checked";
    /// <summary>
    /// Specifies font and text inside div element.
    /// </summary>
    public const string FontTag = "font";
    /// <summary>
    /// Font face.
    /// </summary>
    public const string Face = "face";
    /// <summary>
    /// Font size.
    /// </summary>
    public const string Size = "size";
    /// <summary>
    /// Font color.
    /// </summary>
    public const string Color = "color";
    /// <summary>
    /// Fill style color.
    /// </summary>
    public const string ColorAttribute = "color";
    /// <summary>
    /// Represents check box shape type.
    /// </summary>
    public const string Checkbox = "Checkbox";
    /// <summary>
    /// /// <summary>
    /// Represents OptionButton shape type.
    /// </summary>
    public const string OptionButton = "Radio";
    /// <summary>
    /// Represents combo box shape type.
    /// </summary>
    public const string Drop = "Drop";
    /// <summary>
    /// This element specifies that the object is an AutoLine object. If this
    /// element is specified without a value, it is assumed to be true.
    /// </summary>
    public const string AutoLineTag = "AutoLine";
    /// <summary>
    /// This element specifies that the object is an AutoFill object. If this
    /// element is specified without a value, it is assumed to be true.
    /// </summary>
    public const string AutoFillTag = "AutoFill";
    /// <summary>
    /// This element specifies the cell the object is linked to, using standard cell reference syntax.
    /// </summary>
    public const string FormulaLink = "FmlaLink";
    /// <summary>
    /// This element specifies whether the object is first button of the Grouped objects,
    /// </summary>
    public const string FirstButton = "FirstButton";
    /// <summary>
    /// This element specifies the scroll bar position as the index of the list
    /// item just above the item at the top of the view, given the current scroll
    /// position. The list indexes are 1-based.
    /// If omitted, the value is assumed to be 0.
    /// </summary>
    public const string ScrollPosition = "Val";
    /// <summary>
    /// This element specifies the minimum scroll bar position as the index of
    /// the list item just above the item at the top of the view when the control
    /// is scrolled all the way up, typically 0. The list indexes are 1-based.
    /// If omitted, the value is assumed to be 0.
    /// </summary>
    public const string ScrollMinimum = "Min";
    /// <summary>
    /// Specifies the position of the center rectangle of a radial gradient. The vector is a fraction
    ///of the width and height of the shape. The first is a percentage of the fill to the left edge;
    ///the second is a percentage of the fill to the top. Default is 0,0. To position a radial fill at
    ///the center of a shape, use a value of 50%,50%.
    /// </summary>
    public const string FocusPositionAttribute = "focusposition";
    /// <summary>
    /// Specifies the size of the center rectangle of a radial gradient. The vector is a fraction of
    ///the width and height of the shape. The first is a percentage of the fill to the right edge;
    ///the second is a percentage of the fill to the bottom. Default is 0,0.
    /// </summary>
    public const string FocusSizeAttribute = "focussize";
    /// <summary>
    /// This element specifies the maximum scroll bar position as the index of
    /// the list item just above the item at the top of the view when the control
    /// is scrolled all the way down. The list indexes are 1-based. If omitted,
    /// the value is assumed to be that which allows the last item to be viewed
    /// when the control is scrolled all the way down.
    /// </summary>

    /// <summary>
    /// lightness or darkness of "one color" option in Gradient
    /// </summary>
    public const string GradientOneColorAttributeValueStart = "fill";
    /// <summary>
    /// Colors attributes specifies the combination of colors
    /// applied to the shape. this attribute usde for preset colors
    /// </summary>
    public const string ColorsAttribute = "colors";

    public const string ScrollMaximum = "Max";
    /// <summary>
    /// This element specifies the number of lines to move the scroll bar on an increment click.
    /// If omitted, the increment is 0.
    /// </summary>
    public const string ScrollIncrement = "Inc";
    /// <summary>
    /// This element specifies the number of lines to move the scroll bar on a page click.
    /// </summary>
    public const string ScrollPageIncrement = "Page";
    /// <summary>
    /// This element specifies the width of the scroll bar in screen pixels.
    /// </summary>
    public const string ScrollBarWidth = "Dx";
    /// <summary>
    /// This element specifies that 3D effects are disabled. If this element
    /// is specified without a value, it is assumed to be true.
    /// </summary>
    public const string NoThreeD = "NoThreeD";
    /// <summary>
    /// This element specifies that 3D effects are disabled. If this element
    /// is specified without a value, it is assumed to be true.
    /// </summary>
    public const string NoThreeD2 = "NoThreeD2";
    /// <summary>
    /// This element specifies the range of source data cells used to populate
    /// the list box, using standard cell reference syntax.
    /// </summary>
    public const string ListSourceRange = "FmlaRange";
    /// <summary>
    /// This element specifies that the object represents a password edit field.
    /// If this element is specified without a value, it is assumed to be true.
    /// </summary>
    public const string SelectedItem = "Sel";
    /// <summary>
    /// This element specifies the selection type for the list box. If omitted, the control is assumed to be Single.
    /// </summary>
    public const string SelectionType = "SelType";
    /// <summary>
    /// This enum specifies possible selection types.
    /// </summary>
    public enum SelectionTypes
    {
      /// <summary>
      /// The listbox may only have one selected item.
      /// </summary>
      Single,
      /// <summary>
      /// The listbox may have multiple items selected by clicking on each item.
      /// </summary>
      Multi,
      /// <summary>
      /// The listbox may have multiple items selected by holding a control key and clicking on each item.
      /// </summary>
      Extend,
    }
    /// <summary>
    /// This element specifies the list box callback type. The application should
    /// use the callback to determine how to handle user actions on the list box.
    /// The only allowed value is Normal.
    /// </summary>
    public const string CallbackType = "LCT";
    /// <summary>
    /// Normal value of the callback type.
    /// </summary>
    public const string NormalLCT = "Normal";
    /// <summary>
    /// This element specifies the style of the dropdown.
    /// </summary>
    public const string DropStyle = "DropStyle";
    /// <summary>
    /// Possible drop styles.
    /// </summary>
    public enum DropStyles
    {
      /// <summary>
      /// Standard combo box.
      /// </summary>
      Combo,
      /// <summary>
      /// Editable combo box.
      /// </summary>
      ComboEdit,
      /// <summary>
      /// Standard combo box with only the dropdown button visible when the box is not expanded.
      /// </summary>
      Simple,
    }
    /// <summary>
    /// This element specifies the maximum number of lines in the dropdown before
    /// scrollbars are added.
    /// </summary>
    public const string DropLines = "DropLines";
    public const string MarginLeft = "margin-left";
    public const string MarginTop = "margin-top";
    public const string Width = "width";
    public const string Height = "height";
    public const string Millimeters = "mm";
    public const string AutoPicture = "AutoPict";
    public const string CF = "CF";
    public const string OleObjects = "oleObjects";
    public const string OleObject = "oleObject";
    public const string ProgramID = "progId";
    public const string DevAspect = "dvAspect";
    public const string ShapeID = "shapeId";
    public const string FormulaMacro = "FmlaMacro";
    /// <summary>
    /// this part contains tags used in VML 
    /// </summary>
    public const string StrokeColorAttribute = "strokecolor";
    /// <summary>
    /// Gradient Fill Method
    /// </summary>
    public const string MethodAttribute = "method";
    /// <summary>
    /// Gradient fill method value none.
    /// </summary>
    public const string MethodNoneValue = "none";
    /// <summary>
    /// This element specifies how the shape should be filled
    /// </summary>
    public const string FillTag = "fill";
    /// <summary>
    /// for two color fill
    /// </summary>
    public const string Color2Attribute = "color2";
    /// <summary>
    /// link attribute
    /// </summary>
    public const string LinkAttribute = "link";
    /// <summary>
    /// solid fill tag
    /// </summary>
    public const string SolidFillTag = "solid";
    /// <summary>
    /// Texture Attribute value
    /// </summary>
    public const string TextureAttributeValue = "tile";
    /// <summary>
    /// Picture Attribute Value
    /// </summary>
    public const string PictureAttributeValue = "frame";
    /// <summary>
    /// Pattern Attribute Value
    /// </summary>
    public const string PatternAttributeValue = "pattern";

    /// <summary>
    /// Fill type Tag value represents gradient fill type
    /// </summary>
    public const string GradientTypeTagValue = "gradient";
    /// <summary>
    /// Fill type Tag value represents gradient radial (From corner or Center) fill type
    /// </summary>
    public const string GradientRadialTypeTagValue = "gradientRadial";
    /// <summary>
    /// Fill type Tag value represents gradient radial (From corner or Center) fill type
    /// </summary>
    public const string GradientCenterTypeTagValue = "gradientCenter";
    /// <summary>
    /// fill One Color Darkeness
    /// </summary>
    public const string GradientDarkFillValue = "fill darken";
    /// <summary>
    /// fillOne Color Lightness
    /// </summary>
    public const string GradientLightFillValue = "fill lighten";
    /// <summary>
    /// "Transparency From" of the filled color
    /// </summary>
    public const string OpacityAttribute = "opacity";
    /// <summary>
    /// "Transparency To" of the filed color
    /// </summary>
    public const string Opacity2Attribute = "opacity2";
    /// <summary>
    /// Filled color Shading  (Horizontal , vertical...)
    /// </summary>
    public const string AngleAttribute = "angle";
    /// <summary>
    /// filled color rotates with the shape
    /// </summary>
    public const string RotateAttribute = "rotate";
    /// <summary>
    /// shape's border Line wieght 
    /// </summary>
    public const string StrokeWeightAttribute = "strokeweight";
    /// <summary>
    /// focus attribute specifies to shading variants
    /// Values range from 100% to -100%. Default is 0.
    /// </summary>
    public const string FocusAttribute = "focus";
    /// <summary>
    /// Relation id for the resource
    /// </summary>
    public const string RelationIDAttribute = "relid";
    /// <summary>
    /// Title of the texture or picture or pattern
    /// </summary>
    public const string TitleAttibute = "title";

    /// <summary>
    /// FillType specifies the pattern fill of 
    /// Line in VMLtexbox shapes
    /// </summary>
    public const string FillTypeAttribute = "filltype";
    /// <summary>
    /// Represent Dash Style  (solid, shortDash)
    /// </summary>
    public const string DashStyleAttribute = "dashstyle";
    /// <summary>
    /// Represent line style (ThinThick,..)
    /// </summary>
    public const string LineStyleAttribute = "linestyle";
    /// <summary>
    /// Represents solid fill type of the shape
    /// </summary>
    public const string SolidFillTypeAttributeValue = "solid";
    /// <summary>
    /// Alternate text for the Shape
    /// </summary>
    public const string AlternateTextAttribute = "alt";
    /// <summary>
    /// represents the place holder image
    /// </summary>
    public const string PictureObjectTypeAttributeValue = "Pict";
    /// <summary>
    /// Link Attribute value
    /// </summary>
    public const string LinkAttributeValue = "[{0}]!''''";
    /// <summary>
    /// Specifies that the fill uses an image
    /// </summary>
    public const string ReColorAttribute = "recolor";
    /// <summary>
    /// represents the path
    /// </summary>
    public const string PathAttribute = "path";
    /// <summary>
    /// represents the extrusion
    /// </summary>
    public const string ExtrusionOkAttribute = "extrusionok";
    /// <summary>
    /// represent strokeok
    /// </summary>
    public const string StrokeOkAttribute = "strokeok";
    /// <summary>
    /// represent fillok
    /// </summary>
    public const string FillOkAttribute = "fillok";
    /// <summary>
    /// represent conect type
    /// </summary>
    public const string ConnectTypeAttribute = "connecttype";
    /// <summary>
    /// represent lock
    /// </summary>
    public const string LockTypeAttribute = "lock";
    /// <summary>
    /// represent extension
    /// </summary>
    public const string ExtAttribute = "ext";
    /// <summary>
    /// represent Shape type
    /// </summary>
    public const string ShapeTypeAttribute = "shapetype";
    /// <summary>
    /// represent shadoe ok
    /// </summary>
    public const string ShadowOkAttribute = "shadowok";
    /// <summary>
    /// represent ole Update
    /// </summary>
    public const string OleUpdateAttribute = "oleUpdate";
    /// <summary>
    /// Colors are prefixed with this char
    /// </summary>
    public const char RGBColorPrefixChar = '#';
    /// <summary>
    /// indexed color enclosed with this char
    /// </summary>
    public const char IndexedColorPrefix = '[';
    /// <summary>
    /// Size value prefixed with this char
    /// </summary>
    public const char SizeInPointsPrefix = 'p';
    /// <summary>
    /// Size value store with this string
    /// </summary>
    public const string SizeInPoints = "pt";
    /// <summary>
    /// opacity value divided by this
    /// </summary>
    public const int OpacityDegree = 65536;
    /// <summary>
    /// Degree value divided by this
    /// </summary>
    public const int DegreeDivider = 255;
    /// <summary>
    /// degree's dark limit 
    /// </summary>
    public const double DarkLimit = 0.50;
  }
}
