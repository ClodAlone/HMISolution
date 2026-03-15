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
    /// This element specifies all drawing objects within the worksheet.
    /// </summary>
    public const string WorksheetDrawingTagName = "wsDr";
    /// <summary>
    /// This element specifies a two cell anchor placeholder for a group, a shape,
    /// or a drawing element. It moves with cells and its extents are in EMU units.
    /// </summary>
    public const string TwoCellAnchorTagName = "twoCellAnchor";
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
    /// This element specifies the non-visual properties for the picture canvas.
    /// </summary>
    public const string NVPictureCanvasPropertiesTag = "cNvPicPr";
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
    #endregion
  }
}
