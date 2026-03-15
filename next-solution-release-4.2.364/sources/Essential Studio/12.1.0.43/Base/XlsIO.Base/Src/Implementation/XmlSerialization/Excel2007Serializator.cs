#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;

using Syncfusion.Compression.Zip;

using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.CompoundFile.XlsIO;
using System.Diagnostics;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Drawing;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif !(WINRT )
using System.Drawing;
using System.Drawing.Imaging;

#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// Class used for Excel 2007 Serialization.
  /// </summary>
  public class Excel2007Serializator: IDisposable
  {
    #region Constants
    /// <summary>
    /// Maximum allowed formula length.
    /// </summary>
    private const int MaximumFormulaLength = 8000;
    /// <summary>
    /// File heading.
    /// </summary>
    public const string XmlFileHeading = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>";
    /// <summary>
    /// Namespace with ContentTypes items.
    /// </summary>
    public const string ContentTypesNamespace = "http://schemas.openxmlformats.org/package/2006/content-types";
    /// <summary>
    /// Namespace for hyperlink.
    /// </summary>
    public const string HyperlinkNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
    /// <summary>
    /// Namespace for relation tags/attributes.
    /// </summary>
    public const string RelationNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    /// <summary>
    /// Main xml namespace.
    /// </summary>
    public const string XmlNamespaceMain = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    /// <summary>
    /// Name of the relation type that indicates that part contains worksheet data.
    /// </summary>
    public const string WorksheetPartType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet";
    /// <summary>
    /// Name of the relation type that indicates that part contains chartsheet data.
    /// </summary>
    public const string ChartSheetPartType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chartsheet";
    /// <summary>
    /// Name of the relation type that indicates that part contains extended document properties.
    /// </summary>
    public const string ExtendedPropertiesPartType = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
    /// <summary>
    /// Name of the relation type that indicates that part contains core properties.
    /// </summary>
    public const string CorePropertiesPartType = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
    /// <summary>
        /// Name of the relation type that indicates that part contains the Sparkline properties.
        /// </summary>
    public const string X14Namespace = "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main";
    public const string PivotFieldUri = "{E15A36E0-9728-4e99-A89B-3F7291B0FE68}";
    public const string ExternListUri = "{962EF5D1-5CA2-4c93-8EF4-DBF5C05439D2}";
    public const string SlicerExtensionUri = "{A8765BA9-456A-4dab-B4F3-ACF838C121DE}";
    public const string X14NameSpaceAttribute = "xmlns";
    public const string HideValuesRowAttribute = "hideValuesRow";
    /// <summary>
    /// Main Microsoft Namespace
    /// </summary>
    public const string MSNamespaceMain = "http://schemas.microsoft.com/office/excel/2006/main";
    public const string MSNamespaceMainAttribute = "xmlns";
    public const string XMNamespaceMain = "http://schemas.microsoft.com/office/excel/2006/main";
    public const string XMNamespaceMainAttribute = "xmlns";
    public const string x14PivotTableDefinitionAttributes="pivotTableDefinition";
    /// <summary>
    /// Uri for the Sparkline properties.
    /// </summary>
    public const string SparklineUri = "{05C60535-1F16-4fd2-B633-F4F36F0B64E0}";
    public const string MCPrefix = "mc";
    public const string MCNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
    /// <summary>
    /// Prefix for core properties namespace.
    /// </summary>
    public const string CorePropertiesPrefix = "cp";
    /// <summary>
    /// Name of the relation type that indicates dublin core part.
    /// </summary>
    public const string DublinCorePartType = "http://purl.org/dc/elements/1.1/";
    /// <summary>
    /// Prefix for dublin core namespace.
    /// </summary>
    public const string DublinCorePrefix = "dc";
    /// <summary>
    /// Name of the relation type that indicates dublin core terms part.
    /// </summary>
    public const string DublinCoreTermsPartType = "http://purl.org/dc/terms/";
    /// <summary>
    /// Prefix for dublin core terms namespace.
    /// </summary>
    public const string DublinCoreTermsPrefix = "dcterms";
    /// <summary>
    /// Name of the relation type that indicates DCMIType part.
    /// </summary>
    public const string DCMITypePartType = "http://purl.org/dc/dcmitype/";
    /// <summary>
    /// Prefix for DCMIType namespace.
    /// </summary>
    public const string DCMITypePrefix = "dcmitype";
    /// <summary>
    /// Name of the relation type that indicates XSI part.
    /// </summary>
    public const string XSIPartType = "http://www.w3.org/2001/XMLSchema-instance";
    /// <summary>
    /// Prefix for XSI namespace.
    /// </summary>
    public const string XSIPrefix = "xsi";
    /// <summary>
    /// Name of the relation type that indicates that part contains custom properties.
    /// </summary>
    public const string CustomPropertiesPartType = "http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
    /// <summary>
    /// Name of the relation type that indicates DocPropsVTypes namespace
    /// </summary>
    public const string DocPropsVTypesPartType = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";
    /// <summary>
    /// OleObject Content Type
    /// </summary>
    public const string OleObjectContentType = "application/vnd.openxmlformats-officedocument.oleObject";
    /// <summary>
    /// OleObject File Extension
    /// </summary>
    public const string OleObjectFileExtension = "bin";
    /// <summary>
    /// Prefix for DocPropsVTypes namespace.
    /// </summary>
    public const string DocPropsVTypesPrefix = "vt";
    /// <summary>
    /// Prefix for relation namespace.
    /// </summary>
    public const string RelationPrefix = "r";
    /// <summary>
        /// Prefix for Sparkline namespace.
        /// </summary>
    public const string X14Prefix = "x14";
    /// <summary>
    /// Prefix for Microsoft Main Namespace.
    /// </summary>
    public const string MSPrefix = "xm";
    /// <summary>
    /// Name of the xml tag that stores content types definitions.
    /// </summary>
    public const string TypesTagName = "Types";
    /// <summary>
    /// Name of the xml attribute with part extension.
    /// </summary>
    public const string ExtensionAttributeName = "Extension";
    /// <summary>
    /// Name of the xml tag that stores default type name.
    /// </summary>
    public const string DefaultTagName = "Default";
    /// <summary>
    /// Name of the xml tag that stores content type string.
    /// </summary>
    public const string ContentTypeAttributeName = "ContentType";
    /// <summary>
    /// Name of the xml tag that stores type override.
    /// </summary>
    public const string OverrideTagName = "Override";
    /// <summary>
    /// Name of the xml attribute that stores part name.
    /// </summary>
    public const string PartNameAttributeName = "PartName";
    /// <summary>
    /// Name of the xml tag that defines the structure of the workbook.
    /// </summary>
    public const string WorkbookTagName = "workbook";
    /// <summary>
    /// Name of the xml tag that represents the collection of sheets in the workbook.
    /// </summary>
    public const string SheetsTagName = "sheets";
    /// <summary>
    /// Name of the xml tag that defines a sheet in this workbook.
    /// </summary>
    public const string SheetTagName = "sheet";
    /// <summary>
    /// Name of the xml attribute that stores sheet name.
    /// </summary>
    public const string SheetNameAttribute = "name";
    /// <summary>
    /// Default worksheet path format
    /// </summary>
    public const string DefaultWorksheetPathFormat = "worksheets/sheet{0}.xml";
    /// <summary>
    /// Relation id format
    /// </summary>
    public const string RelationIdFormat = "rId{0}";
    /// <summary>
    /// Name of the xml attribute that stores unique sheet id.
    /// </summary>
    public const string SheetIdAttribute = "sheetId";
    /// <summary>
    /// Name of the xml attribute that stores relation id.
    /// </summary>
    public const string RelationAttribute = "id";
    /// <summary>
    /// Name of the xml attribute that stores relation id,
    /// it is used in .rels file (starts with capital letter).
    /// </summary>
    public const string RelationIdAttribute = "Id";
    /// <summary>
    /// Name of the xml attribute that specifies the visible state of this sheet.
    /// </summary>
    public const string SheetStateAttributeName = "state";
    /// <summary>
    /// This element defines the collection of properties the application uses
    /// to record calculation status and details.
    /// </summary>
    public const string CalcProperties = "calcPr";
    public const string CalculationId = "calcId";
    public const string TabSelected = "tabSelected";
    /// <summary>
    /// Default delimiter between worksheet rows.
    /// </summary>
    public const string DEF_DEFAULT_ROW_DELIMITER = "\r\n";

    #region Sheet Visibility Types
    /// <summary>
    /// Indicates the book window is hidden, but can be shown by the user via the user interface.
    /// </summary>
    public const string StateHidden = "hidden";
    /// <summary>
    /// Indicates the sheet is hidden and cannot be shown in the user interface (UI).
    /// This state is only available programmatically.
    /// </summary>
    public const string StateVeryHidden = "veryHidden";
    /// <summary>
    /// Indicates the sheet is visible.
    /// </summary>
    public const string StateVisible = "visible";
    #endregion

    /// <summary>
    /// Name of the xml tag that holds relationships data.
    /// </summary>
    public const string RelationsTagName = "Relationships";
    /// <summary>
    /// Name of the xml tag that holds relationship data.
    /// </summary>
    public const string RelationTagName = "Relationship";
    /// <summary>
    /// Name of the xml attribute that holds relationship type.
    /// </summary>
    public const string RelationTypeAttribute = "Type";
    /// <summary>
    /// Name of the xml attribute that holds relationship target.
    /// </summary>
    public const string RelationTargetAttribute = "Target";
    /// <summary>
    /// Name of the xml attribute that holds relationship target mode.
    /// </summary>
    public const string RelationTargetModeAttribute = "TargetMode";
    /// <summary>
    /// Name of relationship external target mode.
    /// </summary>
    public const string RelationExternalTargetMode = "External";
    /// <summary>
    /// Name of the xml tag that contains merged cells data.
    /// </summary>
    public const string MergeCellsXmlTagName = "mergeCells";
    /// <summary>
    /// Name of the count attribute.
    /// </summary>
    public const string CountAttributeName = "count";
    /// <summary>
    /// Name of the xml tag that contains single merge region's data.
    /// </summary>
    public const string MergeCellXmlTagName = "mergeCell";
    /// <summary>
    /// Name of the ref attribute.
    /// </summary>
    public const string RefAttributeName = "ref";
    /// <summary>
    /// Name of the xml tag that contains all defined named ranges.
    /// </summary>
    public const string DefinedNamesXmlTagName = "definedNames";
    /// <summary>
    /// Name of the xml tag that contains single defined named range.
    /// </summary>
    public const string DefinedNameXmlTagName = "definedName";
    /// <summary>
    /// Name of the name attribute.
    /// </summary>
    public const string NameAttributeName = "name";
    /// <summary>
    /// Name of the attribute that stores sheet id for named range.
    /// </summary>
    public const string NameSheetIdAttribute = "localSheetId";
    /// <summary>
    /// Name of the xml tag that contains all styles settings (fonts, number formats, fills, etc.) inside.
    /// </summary>
    public const string StyleSheetTagName = "styleSheet";
   
    public const string SlicerList = "slicerList";
    /// <summary>
    /// Name of the xml tag that contains all fonts settings inside.
    /// </summary>
    public const string FontsTagName = "fonts";
    /// <summary>
    /// Name of the xml tag that contains all font settings inside.
    /// </summary>
    public const string FontTagName = "font";
    /// <summary>
    /// Name of the xml tag that indicates that font is bold.
    /// </summary>
    public const string FontBoldTagName = "b";
    /// <summary>
    /// Name of the xml tag that indicates that font is italic.
    /// </summary>
    public const string FontItalicTagName = "i";
    /// <summary>
    /// Name of the xml tag that stores font underline settings.
    /// </summary>
    public const string FontUnderlineTagName = "u";
    /// <summary>
    /// Name of the value attribute.
    /// </summary>
    public const string ValueAttributeName = "val";
    /// <summary>
    /// Name of the xml tag that stores font underline settings.
    /// </summary>
    public const string FontSizeTagName = "sz";
    /// <summary>
    /// Name of the xml tag that indicates that font has strike settings.
    /// </summary>
    public const string FontStrikeTagName = "strike";
    /// <summary>
    /// Name of the xml tag that represents font name.
    /// </summary>
    public const string FontNameTagName = "name";
    /// <summary>
    /// Name of the xml tag that contains color settings.
    /// </summary>
    public const string ColorTagName = "color";
    /// <summary>
    /// Name of the indexed attribute in the color tag.
    /// </summary>
    public const string ColorIndexedAttributeName = "indexed";
    /// <summary>
    /// Name of the xml attribute that represents index into the theme colors collection.
    /// </summary>
    public const string ColorThemeAttributeName = "theme";
    /// <summary>
    /// Name of the xml attribute that represents the tint value applied to the color.
    /// </summary>
    public const string ColorTintAttributeName = "tint";
    /// <summary>
    /// Name of the rgb attribute in the color tag.
    /// </summary>
    public const string ColorRgbAttribute = "rgb";
    /// <summary>
    /// Name of the xml tag that stores indexed color values.
    /// </summary>
    public const string IndexedColorsTagName = "indexedColors";
    /// <summary>
    /// Name of the xml tag that stores color settings.
    /// </summary>
    public const string ColorsTagName = "colors";
    /// <summary>
    /// Name of the xml tag that stores rgb color.
    /// </summary>
    public const string RgbColorTagName = "rgbColor";
    /// <summary>
    /// Name of the xml tag that stores MacOSShadow settings.
    /// </summary>
    public const string MacOSShadowTagName = "shadow";
    /// <summary>
    /// Name of the xml tag that stores font vertical alignment settings.
    /// </summary>
    public const string FontVerticalAlignmentTagName = "vertAlign";
    /// <summary>
    /// Name of the xml tag that stores font family settings.
    /// </summary>
    public const string FontFamilyTagName = "family";
    /// <summary>
    /// Name of the xml tag that stores font charset settings.
    /// </summary>
    public const string FontCharsetTagName = "charset";
    /// <summary>
    /// Name of the xml tag that represents number formats.
    /// </summary>
    public const string NumberFormatsTagName = "numFmts";
    /// <summary>
    /// Name of the xml tag that represents single number format.
    /// </summary>
    public const string NumberFormatTagName = "numFmt";
    /// <summary>
    /// Name of the xml attribute that defines number format id.
    /// </summary>
    public const string NumberFormatIdAttributeName = "numFmtId";
    /// <summary>
    /// Name of the xml attribute that defines number format string.
    /// </summary>
    public const string NumberFormatStringAttributeName = "formatCode";
    /// <summary>
    /// Name of the xml tag that represents fills collection.
    /// </summary>
    public const string FillsTagName = "fills";
    /// <summary>
    /// Name of the xml tag that represents single fill object.
    /// </summary>
    public const string FillTagName = "fill";
    /// <summary>
    /// Name of the xml tag that represents pattern fill object.
    /// </summary>
    public const string PatternFillTagName = "patternFill";
    /// <summary>
    /// Name of the xml tag that represents gradient-style cell fill.
    /// </summary>
    public const string GradientFillTagName = "gradientFill";
    /// <summary>
    /// Name of the xml attribute that represents type of gradient fill.
    /// </summary>
    public const string GradientFillTypeAttributeName = "type";

    /// <summary>
    /// This gradient fill is of linear gradient type.
    /// </summary>
    public const string GradientFillTypeLinear = "linear";
    /// <summary>
    /// This gradient fill is of path gradient type.
    /// </summary>
    public const string GradientFillTypePath = "path";
    /// <summary>
    /// Name of the xml attribute that represents angle of the linear gradient - vertical, horizontal, diagonal.
    /// </summary>
    public const string LinearGradientDegreeAttributeName = "degree";
    /// <summary>
    /// Name of the xml attribute that specifies in percentage format (from the top to the bottom) the
    /// position of the bottom edge of the inner rectangle (color 1).
    /// </summary>
    public const string BottomConvergenceAttributeName = "bottom";
    /// <summary>
    /// Name of the xml attribute that specifies in percentage format (from the left to the right) the
    /// position of the left edge of the inner rectangle (color 1).
    /// </summary>
    public const string LeftConvergenceAttributeName = "left";
    /// <summary>
    /// Name of the xml attribute that specifies in percentage format (from the left to the right) the
    /// position of the right edge of the inner rectangle (color 1).
    /// </summary>
    public const string RightConvergenceAttributeName = "right";
    /// <summary>
    /// Name of the xml attribute that specifies in percentage format (from the top to the bottom) the
    /// position of the top edge of the inner rectangle (color 1).
    /// </summary>
    public const string TopConvergenceAttributeName = "top";
    /// <summary>
    /// Name of the xml tag name that represents one of a sequence of two or more gradient stops, constituting this gradient fill.
    /// </summary>
    public const string GradientStopTagName = "stop";
    /// <summary>
    /// Name of the xml attribute that represents position information for this gradient stop.
    /// </summary>
    public const string GradientStopPositionAttributeName = "position";

    /// <summary>
    /// Name of the xml attribute that defines pattern.
    /// </summary>
    public const string PatternAttributeName = "patternType";
    /// <summary>
    /// Name of the xml tag that represents background color.
    /// </summary>
    public const string BackgroundColorTagName = "bgColor";
    /// <summary>
    /// Name of the xml tag that represents foreground color.
    /// </summary>
    public const string ForegroundColorTagName = "fgColor";
    /// <summary>
    /// Name of the xml tag that stores all border collections inside.
    /// </summary>
    public const string BordersTagName = "borders";
    /// <summary>
    /// Name of the xml tag that stores single border collection.
    /// </summary>
    public const string BordersCollectionTagName = "border";
    /// <summary>
    /// Name of the xml attribute that represents border style.
    /// </summary>
    public const string BorderStyleAttributeName = "style";
    /// <summary>
    /// Name of the xml tag that stores border color.
    /// </summary>
    public const string BorderColorTagName = "color";
    /// <summary>
    /// This is the root element of Sheet Parts that are of type 'worksheet'.
    /// </summary>
    public const string WorksheetTagName = "worksheet";
    /// <summary>
    /// Name of the xml tag that stores worksheet dimension.
    /// </summary>
    public const string DimensionTagName = "dimension";
    /// <summary>
    /// Name of the xml tag that stores sheet data.
    /// </summary>
    public const string SheetDataTagName = "sheetData";
    /// <summary>
    /// Name of the xml tag that stores cell in the worksheet.
    /// </summary>
    public const string CellTagName = "c";
    /// <summary>
    /// Name of the xml attribute that represents the cell metadata record associated with this cell.
    /// </summary>
    public const string CellMetadataIndexAttributeName = "cm";
    /// <summary>
    /// Name of the xml attribute that represents boolean value to show phonetic information.
    /// </summary>
    public const string ShowPhoneticAttributeName = "ph";
    /// <summary>
    /// Name of the xml attribute that represents an A1 style reference to the location of this cell.
    /// </summary>
    public const string ReferenceAttributeName = "r";
    /// <summary>
    /// Name of the xml attribute that represents the index of this cell's style.
    /// </summary>
    public const string StyleIndexAttributeName = "s";
    /// <summary>
    /// Name of the xml attribute that represents an enumeration representing the cell's data type.
    /// </summary>
    public const string CellDataTypeAttributeName = "t";
    /// <summary>
    /// Name of the xml attribute that represents index of the value metadata
    /// record associated with this cell's value.
    /// </summary>
    public const string ValueMetadataIndexAttributeName = "vm";
    /// <summary>
    /// Name of xml tag that represents formula.
    /// </summary>
    public const string FormulaTagName = "f";
    /// <summary>
    /// Name of xml tag that represents cell value.
    /// </summary>
    public const string CellValueTagName = "v";
    /// <summary>
    /// Name of xml tag that represents rich text.
    /// </summary>
    public const string RichTextInlineTagName = "is";
    /// <summary>
    /// Name of xml tag that represents rich text run properties.
    /// </summary>
    public const string RichTextRunPropertiesTagName = "rPr";
    /// <summary>
    /// Name of xml tag that represents rich text run font name.
    /// </summary>
    public const string RichTextRunFontTagName = "rFont";
    /// <summary>
    /// Name of the xml tag that stores all column settings.
    /// </summary>
    public const string ColsTagName = "cols";
    /// <summary>
    /// Name of the xml tag that stores column settings range.
    /// </summary>
    public const string ColTagName = "col";
    /// <summary>
    /// Name of the xml attribute that stores minimum column index of the column range.
    /// </summary>
    public const string ColumnMinAttribute = "min";
    /// <summary>
    /// Name of the xml attribute that stores maximum column index of the column range.
    /// </summary>
    public const string ColumnMaxAttribute = "max";
    /// <summary>
    /// Name of the xml attribute that stores width of each column from column range.
    /// </summary>
    public const string ColumnWidthAttribute = "width";
    /// <summary>
    /// Name of the xml attribute that stores style of each column from column range.
    /// </summary>
    public const string ColumnStyleAttribute = "style";
    /// <summary>
    /// Name of the xml attribute that indicates whether column width differs from the default one.
    /// </summary>
    public const string ColumnCustomWidthAttribute = "customWidth";
    /// <summary>
    /// 
    /// </summary>
    public const string BestFitAttribute = "bestFit";
    /// <summary>
    /// Name of xml tag that represents row.
    /// </summary>
    public const string RowTagName = "row";
    /// <summary>
    /// Name of xml attribute that represents row index.
    /// </summary>
    public const string RowIndexAttributeName = "r";
    /// <summary>
    /// Name of xml attribute that represents row height in point size.
    /// </summary>
    public const string RowHeightAttributeName = "ht";
    /// <summary>
    /// Name of xml attribute that shows whether row is visible or not.
    /// </summary>
    public const string RowHiddenAttributeName = "hidden";
    /// <summary>
    /// Name of xml attribute that shows whether row style should be applied or not.
    /// </summary>
    public const string RowCustomFormatAttributeName = "customFormat";
    /// <summary>
    /// Name of xml attribute that shows whether row height has been manually set or not.
    /// </summary>
    public const string RowCustomHeightAttributeName = "customHeight";
    /// <summary>
    /// Name of xml attribute that shows whether row has been collapsed or not.
    /// </summary>
    public const string RowColumnCollapsedAttribute = "collapsed";
    /// <summary>
    /// Name of xml attribute that represents outlining level of the row.
    /// </summary>
    public const string RowColumnOutlineLevelAttribute = "outlineLevel";
    /// <summary>
    /// Name of xml attribute that represents thick bottom flag.
    /// </summary>
    public const string RowThickBottomAttributeName = "thickBot";
    /// <summary>
    /// Name of xml attribute that represents thick top flag.
    /// </summary>
    public const string RowThickTopAttributeName = "thickTop";
    /// <summary>
    /// Name of xml attribute that represents formula type.
    /// </summary>
    public const string FormulaTypeAttributeName = "t";
    /// <summary>
    /// True indicates that this formula is an array formula and the entire array shall be
    /// calculated in full. If false the individual cells of the array shall be calculated as needed.
    /// </summary>
    public const string AlwaysCalculateArray = "aca";
    /// <summary>
    /// Name of xml attribute that represents shared formula group index.
    /// </summary>
    public const string SharedGroupIndexAttributeName = "si";
    /// <summary>
    /// Name of xml attribute that represents range of cells which the formula applies to.
    /// </summary>
    public const string RangeOfCellsAttributeName = "ref";
    /// <summary>
    /// Name of the xml tag that holds list of all comment authors.
    /// </summary>
    public const string CommentAuthorsTagName = "authors";
    /// <summary>
    /// Name of the xml tag that holds single author name.
    /// </summary>
    public const string CommentAuthorTagName = "author";
    /// <summary>
    /// Name of the xml tag that holds all comment notes.
    /// </summary>
    public const string CommentListTagName = "commentList";
    /// <summary>
    /// Name of the xml tag that holds single comment note.
    /// </summary>
    public const string CommentTagName = "comment";
    /// <summary>
    /// Name of the xml tag that stores comment text.
    /// </summary>
    public const string CommentTextTagName = "text";
    /// <summary>
    /// Name of the xml tag that stores all comment notes settings.
    /// </summary>
    public const string CommentsTagName = "comments";
    /// <summary>
    /// Name of the xml attribute that stores comment author id.
    /// </summary>
    public const string AuthorIdAttributeName = "authorId";
    /// <summary>
    /// Represents the cell's data type.
    /// </summary>
    public enum CellType
    {
      /// <summary>
      /// Cell containing a boolean.
      /// </summary>
      b,
      /// <summary>
      /// Cell containing an error.
      /// </summary>
      e,
      /// <summary>
      /// Cell containing an (inline) rich string.
      /// </summary>
      inlineStr,
      /// <summary>
      /// Cell containing a number.
      /// </summary>
      n,
      /// <summary>
      /// Cell containing a shared string.
      /// </summary>
      s,
      /// <summary>
      /// Cell containing a formula string.
      /// </summary>
      str
    }
    /// <summary>
    /// Defines default cell data type.
    /// </summary>
    public const string DefaultCellDataType = "n";
    /// <summary>
    /// Represents type of formula.
    /// </summary>
    public enum FormulaType
    {
      /// <summary>
      /// Formula is an array entered formula.
      /// </summary>
      array,
      /// <summary>
      /// Formula is a data table formula.
      /// </summary>
      dataTable,
      /// <summary>
      /// Formula is a regular cell formula.
      /// </summary>
      normal,
      /// <summary>
      /// Formula is part of a shared formula.
      /// </summary>
      shared
    }
    /// <summary>
    /// Name of the xml tag that stores extended formats for named styles.
    /// </summary>
    public const string NamedStyleXFsTagName = "cellStyleXfs";
    /// <summary>
    /// Name of the xml tag that stores extended formats for cell formats.
    /// </summary>
    public const string CellFormatXFsTagName = "cellXfs";
    /// <summary>
    /// Name of the xml tag that stores extended formats for differential formats.
    /// </summary>
    public const string DiffXFsTagName = "dxfs";
    /// <summary>
    /// Name of the xml tag that stores table styles.
    /// </summary>
    public const string TableStylesTagName = "tableStyles";
    /// <summary>
    /// Name of the xml tag that stores single extended format settings.
    /// </summary>
    public const string ExtendedFormatTagName = "xf";
    /// <summary>
    /// Name of the xml attribute that represents font id.
    /// </summary>
    public const string FontIdAttributeName = "fontId";
    /// <summary>
    /// Name of the xml attribute that represents fill id.
    /// </summary>
    public const string FillIdAttributeName = "fillId";
    /// <summary>
    /// Name of the xml attribute that represents border id.
    /// </summary>
    public const string BorderIdAttributeName = "borderId";
    /// <summary>
    /// Name of the xml attribute that represents extended format index.
    /// </summary>
    public const string XFIdAttributeName = "xfId";
    /// <summary>
    /// Name of the xml tag that stores named cell styles settings.
    /// </summary>
    public const string CellStylesTagName = "cellStyles";
    /// <summary>
    /// Name of the xml tag that stores single named cell style settings.
    /// </summary>
    public const string CellStyleTagName = "cellStyle";
    /// <summary>
    /// Name of the xml attribute that stores built in style id.
    /// </summary>
    public const string StyleBuiltinIdAttributeName = "builtinId";
    /// <summary>
    /// Name of the xml attribute that define the customized buildinstyles.
    /// </summary>
    public const string StyleCustomizedAttributeName = "customBuiltin";
    /// <summary>
    /// Indicates that this formatting is for an outline style.
    /// </summary>
    public const string OutlineLevelAttribute = "iLevel";
    /// <summary>
    /// Name of the xml attribute indicating whether the alignment formatting
    /// specified for this xf should be applied.
    /// </summary>
    public const string IncludeAlignmentAttributeName = "applyAlignment";
    /// <summary>
    /// Name of the xml attribute indicating whether the border formatting
    /// specified for this xf should be applied.
    /// </summary>
    public const string IncludeBorderAttributeName = "applyBorder";
    /// <summary>
    /// Name of the xml attribute indicating whether the font formatting
    /// specified for this xf should be applied.
    /// </summary>
    public const string IncludeFontAttributeName = "applyFont";
    /// <summary>
    /// Name of the xml attribute indicating whether the number formatting
    /// specified for this xf should be applied.
    /// </summary>
    public const string IncludeNumberFormatAttributeName = "applyNumberFormat";
    /// <summary>
    /// Name of the xml attribute indicating whether the fill formatting
    /// specified for this xf should be applied.
    /// </summary>
    public const string IncludePatternsAttributeName = "applyFill";
    /// <summary>
    /// Name of the xml attribute indicating whether the protection formatting
    /// specified for this xf should be applied.
    /// </summary>
    public const string IncludeProtectionAttributeName = "applyProtection";
    /// <summary>
    /// Name of the xml tag that hold all style's alignment information.
    /// </summary>
    public const string AlignmentTagName = "alignment";
    /// <summary>
    /// Name of the xml tag that hold all style's protection information.
    /// </summary>
    public const string ProtectionTagName = "protection";
    /// <summary>
    /// Name of the xml attribute that stores style indent level settings.
    /// </summary>
    public const string IndentAttributeName = "indent";
    /// <summary>
    /// Name of the xml attribute that specifies the type of horizontal alignment in cells.
    /// </summary>
    public const string HAlignAttributeName = "horizontal";
    /// <summary>
    /// Name of the xml attribute indicating if the cells justified or distributed
    /// alignment should be used on the last line of text.
    /// </summary>
    public const string JustifyLastLineAttributeName = "justifyLastLine";
    /// <summary>
    /// Name of the xml attribute that holds reading order settings.
    /// </summary>
    public const string ReadingOrderAttributeName = "readingOrder";
    /// <summary>
    /// Name of the xml attribute that holds shrink to fit option.
    /// </summary>
    public const string ShrinkToFitAttributeName = "shrinkToFit";
    /// <summary>
    /// Name of the xml attribute that holds text rotation value.
    /// </summary>
    public const string TextRotationAttributeName = "textRotation";
    /// <summary>
    /// Name of the xml attribute that holds wrap text option.
    /// </summary>
    public const string WrapTextAttributeName = "wrapText";
    /// <summary>
    /// Name of the xml attribute that specifies the type of vertical alignment in cells.
    /// </summary>
    public const string VerticalAttributeName = "vertical";
    /// <summary>
    /// Name of the xml attribute that indicates whether cell is hidden.
    /// </summary>
    public const string HiddenAttributeName = "hidden";
    /// <summary>
    /// Name of the xml attribute that indicates whether cell is locked.
    /// </summary>
    public const string LockedAttributeName = "locked";
    /// <summary>
    /// Default value for FormulaHidden value.
    /// </summary>
    public const bool HiddenDefaultValue = false;
    /// <summary>
    /// Default value for Locked value.
    /// </summary>
    public const bool LockedDefaultValue = true;
    /// <summary>
    /// Name of the xml attribute that stores value indicating whether the text
    /// string in a cell should be prefixed by a single quote mark.
    /// </summary>
    public const string QuotePreffixAttributeName = "quotePrefix";
    /// <summary>
    /// Name of the xml attribute that indicates whether diagonalDown border is present.
    /// </summary>
    public const string DiagonalDownAttributeName = "diagonalDown";
    /// <summary>
    /// Name of the xml attribute that indicates whether diagonalUp border is present.
    /// </summary>
    public const string DiagonalUpAttributeName = "diagonalUp";
    /// <summary>
    /// Name of the xml tag that represents shared string table.
    /// </summary>
    public const string SharedStringTableTagName = "sst";
    /// <summary>
    /// Name of the xml attribute that represents the total count of unique strings in the SST.
    /// </summary>
    public const string UniqueStringCountAttributeName = "uniqueCount";
    /// <summary>
    /// Name of the xml tag that represents individual string in SST.
    /// </summary>
    public const string StringItemTagName = "si";
    /// <summary>
    /// Name of the xml tag that represents the text content shown as part of a string.
    /// </summary>
    public const string TextTagName = "t";
    /// <summary>
    /// Name of the xml tag that represents rich text run.
    /// </summary>
    public const string RichTextRunTagName = "r";
    /// <summary>
    /// Root items for data preserving.
    /// </summary>
    public const string TemporaryRoot = "root";
    /// <summary>
    /// Attribute to specify string spacing.
    /// </summary>
    public const string SpaceAttributeName = "space";
    /// <summary>
    /// Xml prefix.
    /// </summary>
    public const string XmlPrefix = "xml";
    /// <summary>
    /// Value specifying white space preservation in a string.
    /// </summary>
    public const string PreserveValue = "preserve";
    /// <summary>
    /// Represents Number data type.
    /// </summary>
    private const string CellTypeNumber = "n";
    /// <summary>
    /// Represents string data type.
    /// </summary>
    private const string CellTypeString = "s";
    /// <summary>
    /// Represents boolean data type.
    /// </summary>
    private const string CellTypeBool = "b";
    /// <summary>
    /// Represents error.
    /// </summary>
    private const string CellTypeError = "e";
    /// <summary>
    /// Represents formula.
    /// </summary>
    private const string CellTypeFormulaString = "str";
    /// <summary>
    /// Represents inline string
    /// </summary>
    private const string CellTypeInlineString = "inlineStr";
    /// <summary>
    /// Name of the xml tag that represents the root level complex type associated with a shared style sheet (or theme).
    /// </summary>
    public const string ThemeTagName = "theme";
    /// <summary>
    /// Name of the xml tag that represents the theme formatting options for the theme and is the workhorse of the theme.
    /// </summary>
    public const string ThemeElementsTagName = "themeElements";
    /// <summary>
    /// Name of the xml tag that represents a set of colors which are referred to as a color scheme.
    /// </summary>
    public const string ColorSchemeTagName = "clrScheme";
    /// <summary>
    /// Name of the xml attribute name that the actual color value, expressed as a sequence of hex digits RRGGBB.
    /// </summary>
    public const string RGBHexColorValueAttributeName = "val";
    /// <summary>
    /// Name of the xml tag that represents a color bound to predefined operating system elements.
    /// </summary>
    public const string SystemColorTagName = "sysClr";
    /// <summary>
    /// Name of the xml attribute that represents the system color value.
    /// </summary>
    public const string SystemColorValueAttributeName = "val";
    /// <summary>
    /// Name of the xml attribute that represents the color value that was last computed by the generating application.
    /// </summary>
    public const string SystemColorLastColorAttributeName = "lastClr";
    /// <summary>
    /// Name of the xml tag that represents a single dxf record, expressing incremental formatting to be applied.
    /// </summary>
    public const string DxfFormattingTagName = "dxf";
    /// <summary>
    /// Name of the xml tag that represents a collection of phonetic properties that affect the display of phonetic text.
    /// </summary>
    public const string PhoneticPr = "phoneticPr";
    public const string Phonetic = "phonetic";
    /// <summary>
    /// Name of the xml tag that represents a collection of hyperlinks.
    /// </summary>
    public const string HyperlinksTagName = "hyperlinks";
    /// <summary>
    /// Name of the xml tag that represents a single hyperlink.
    /// </summary>
    public const string HyperlinkTagName = "hyperlink";
    /// <summary>
    /// Name of the xml attribute that represents display string, if different from string in string table.
    /// </summary>
    public const string DisplayStringAttributeName = "display";
    /// <summary>
    /// Name of the xml attribute that represents relationship Id in this sheet's relationships part,
    /// expressing the target location of the resource.
    /// </summary>
    public const string RelationshipIdAttributeName = "id";
    /// <summary>
    /// Name of the xml attribute that represents location within target.
    /// </summary>
    public const string LocationAttributeName = "location";
    /// <summary>
    /// Name of the xml attribute that represents cell location of hyperlink on worksheet.
    /// </summary>
    public const string HyperlinkReferenceAttributeName = "ref";
    /// <summary>
    /// Name of the xml attribute that represents additional text to help the user understand more about the hyperlink.
    /// </summary>
    public const string ToolTipAttributeName = "tooltip";
    /// <summary>
    /// Name of the xml tag that represents sheet level properties.
    /// </summary>
    public const string SheetLevelPropertiesTagName = "sheetPr";
    /// <summary>
    /// Page setup properties of the worksheet.
    /// </summary>
    public const string PageSetupPropertiesTag = "pageSetUpPr";
    /// <summary>
    /// Flag indicating whether the Fit to Page print option is enabled.
    /// </summary>
    public const string FitToPageAttribute = "fitToPage";
    /// <summary>
    /// Name of the xml tag that represents background color of the sheet tab.
    /// </summary>
    public const string SheetTabColorTagName = "tabColor";
    /// <summary>
    /// Name of the xml tag that represents sheet outline properties.
    /// </summary>
    public const string SheetOutlinePropertiesTagName = "outlinePr";
    /// <summary>
    /// Name of the xml tag that represents Summary Row below property of sheet outline
    /// </summary>
    public const string SummaryRowBelow = "summaryBelow";
    /// <summary>
    /// Name of the xml tag that represents Summary column right property of sheet outline
    /// </summary>
    public const string SummaryColumnRight = "summaryRight";
    /// <summary>
    /// Name of the xml tag that represents Background sheet image.
    /// </summary>
    public const string BackgroundImageTagName = "picture";
    /// <summary>
    /// Represent file hyperlink string prefix.
    /// </summary>
    public const string FileHyperlinkStartString = @"file:///";
    /// <summary>
    /// Represent http hyperlink string prefix.
    /// </summary>
    public const string HttpStartString = "http://";
    /// <summary>
    /// Name of the xml tag that stores sheet format properties.
    /// </summary>
    public const string SheetFormatPropertiesTag = "sheetFormatPr";
    /// <summary>
    /// Name of the xml attribute that stores rows are hidden by default.
    /// </summary>
    public const string ZeroHeightAttribute = "zeroHeight";
    /// <summary>
    /// Name of the xml attribute that stores default row height setting.
    /// </summary>
    public const string DefaultRowHeightAttribute = "defaultRowHeight";
    /// <summary>
    /// Name of the xml attribute that stores default col width setting.
    /// </summary>
    public const string DefaultColumWidthAttribute = "defaultColWidth";
       ///<summary>
        ///Name of the xml attribute that stores base column width setting
        /// </summary>
        public const string BaseColWidthAttribute = "baseColWidth";
        ///<summary>
        ///Name of the xml attribute that stores thickBottom setting
        ///</summary>
        public const string ThickBottomAttribute = "thickBottom";
        ///<summary>
        ///Name of the xml attribute that stores thickTop setting
        ///</summary>
        public const string ThickTopAttribute = "thickTop";
        ///<summary>
        ///Name of the xml attribute that stores outlineLevelCol setting
        /// </summary>
       public const string OutlineLevelColAttribute = "outlineLevelCol";
        ///<summary>
        ///Name of the xml attribute that stores outlineLevelRow setting
        /// </summary>
        public const string OutlineLevelRowAttribute = "outlineLevelRow";
    /// <summary>
    /// Name of the xml tag that specifies the collection of workbook views.
    /// </summary>
    public const string WorkbookViewsTagName = "bookViews";
    /// <summary>
    /// Name of the xml tag that specifies a single Workbook view.
    /// </summary>
    public const string WorkbookViewTagName = "workbookView";
    /// <summary>
    /// Name of the xml attribute that specifies an unsignedInt that contains the index to the active sheet in this book view.
    /// </summary>
    public const string ActiveSheetIndexAttributeName = "activeTab";
    /// <summary>
    /// Name of the xml attribute that specifies a boolean value that indicates whether to group dates
    /// when presenting the user with filtering options in the user interface.
    /// </summary>
    public const string AutoFilterDateGroupingAttributeName = "autoFilterDateGrouping";
    /// <summary>
    /// Name of the xml attribute that specifies the index to the first sheet in this book view.
    /// </summary>
    public const string FirstSheetAttributeName = "firstSheet";
    /// <summary>
    /// Name of the xml attribute that specifies a boolean value that indicates whether the book window is minimized.
    /// </summary>
    public const string MinimizedAttributeName = "minimized";
    /// <summary>
    /// Name of the xml attribute that specifies a boolean value that indicates whether
    /// to display the horizontal scroll bar in the user interface.
    /// </summary>
    public const string ShowHorizontalScrollAttributeName = "showHorizontalScroll";
    /// <summary>
    /// Name of the xml attribute that specifies a boolean value that indicates whether
    /// to display the sheet tabs in the user interface.
    /// </summary>
    public const string ShowSheetTabsAttributeName = "showSheetTabs";
    /// <summary>
    /// Name of the xml attribute that specifies a boolean value that indicates whether to display the vertical scroll bar.
    /// </summary>
    public const string ShowVerticalScrollAttributeName = "showVerticalScroll";
    /// <summary>
    /// Name of the xml attribute that specifies ratio between the workbook tabs bar and the horizontal scroll bar.
    /// </summary>
    public const string SheetTabRatioAttributeName = "tabRatio";
    /// <summary>
    /// Name of the xml attribute that specifies visible state of the book window.
    /// </summary>
    public const string VisibilityAttributeName = "visibility";
    /// <summary>
    /// Name of the xml attribute that specifies the height of the workbook window.
    /// </summary>
    public const string WindowHeightAttributeName = "windowHeight";
    /// <summary>
    /// Name of the xml attribute that specifies the width of the workbook window.
    /// </summary>
    public const string WindowWidthAttributeName = "windowWidth";
    /// <summary>
    /// Name of the xml attribute that specifies the X coordinate for the upper left corner of the book window.
    /// </summary>
    public const string UpperLeftCornerXAttributeName = "xWindow";
    /// <summary>
    /// Name of the xml attribute that specifies the Y coordinate for the upper left corner of the book window.
    /// </summary>
    public const string UpperLeftCornerYAttributeName = "yWindow";
    /// <summary>
    /// Name of the xml tag that represents horizontal page break information.
    /// </summary>
    public const string HorizontalPageBreaksTagName = "rowBreaks";
    /// <summary>
    /// Name of the xml tag that represents vertical page break information
    /// </summary>
    public const string VerticalPageBreaksTagName = "colBreaks";
    /// <summary>
    /// Name of the xml attribute that represents number of breaks in the collection.
    /// </summary>
    public const string PageBreakCountAttributeName = "count";
    /// <summary>
    /// Name of the xml attribute that represents number of manual breaks in the collection.
    /// </summary>
    public const string ManualBreakCountAttributeName = "manualBreakCount";
    /// <summary>
    /// Name of the xml tag that represents individual row or column breaks;
    /// </summary>
    public const string BreakTagName = "brk";
    /// <summary>
    /// Name of the xml attribute that represents zero-based row or column Id of the page break.
    /// Breaks occur above the specified row and left of the specified column.
    /// </summary>
    public const string IdAttributeName = "id";
    /// <summary>
    /// Name of the xml attribute that represents manual Break flag.
    /// '1' means the break is a manually inserted break.
    /// </summary>
    public const string ManualPageBreakAttributeName = "man";
    /// <summary>
    /// Name of the xml attribute that represents zero-based index of end row or column of the break.
    /// For row breaks, specifies column index; for column breaks, specifies row index.
    /// </summary>
    public const string MaximumAttributeName = "max";
    /// <summary>
    /// Name of the xml attribute that represents zero-based index of start row or column of the break.
    /// For row breaks, specifies column index; for column breaks, specifies row index.
    /// </summary>
    public const string MinimumAttributeName = "min";
    /// <summary>
    /// Worksheet/chartsheet views collection.
    /// </summary>
    public const string SheetViewsTag = "sheetViews";
    /// <summary>
    /// This element specifies a chart sheet view.
    /// </summary>
    public const string SheetViewTag = "sheetView";
    /// <summary>
    /// Flag indicating whether this sheet should display zero values.
    /// </summary>
    public const string ShowZeros = "showZeros";
    /// <summary>
    /// Zero-based index of this workbook view, pointing to a workbookView
    /// element in the bookViews collection.
    /// </summary>
    public const string WorkbookViewIdAttribute = "workbookViewId";
    /// <summary>
    /// String used to serialize sheet zooming
    /// </summary>
    public const string SheetZoomScale = "zoomScale";
    /// <summary>
    /// String used to serialize sheet view
    /// </summary>
    public const string ViewTag = "view";
    /// <summary>
    /// String used to serialize sheet zooming
    /// </summary>
    public const string Layout = "pageLayout";
    /// <summary>
    /// String used to serialize Page break preview layout.
    /// </summary>
    public const string PageBreakPreview = "pageBreakPreview";
    /// <summary>
    /// String used to serialize Normal layout.
    /// </summary>
    public const string Normal = "normal";
    /// <summary>
    /// String used to serialize boolean True value.
    /// </summary>
    public const string TrueValue = "1";
    /// <summary>
    /// String used to serialize boolean False value.
    /// </summary>
    public const string FalseValue = "0";
    /// <summary>
        /// String used to serialize Sparkline Column type.
        /// </summary>
        public const string SparklineColumnValue = "column";
        /// <summary>
        /// String used to serialize Sparkline WinLoss type.
        /// </summary>
        public const string SparklineWinLossValue = "stacked";
        /// <summary>
        /// String used to serialize Display of Empty Cells as gaps.
        /// </summary>
        public const string EmptyCellsGapValue = "gap";
        /// <summary>
        /// String used to serialize Display of Empty Cells as zero.
        /// </summary>
        public const string EmptyCellsZeroValue = "zero";
        /// <summary>
        /// String used to serialize Display of Empty Cells as Continued lines.
        /// </summary>
        public const string EmptyCellsLineValue = "span";
        /// <summary>
        /// String used to serialize Vertical Axis type as custom.
        /// </summary>
        public const string VerticalCustomTypeValue = "custom";
        /// <summary>
        /// String used to serialize Vertical Axis type as same.
        /// </summary>
        public const string VerticalSameTypeValue = "group";
        /// <summary>
    /// Flag indicating whether this sheet should display gridlines.
    /// </summary>
    public const string ShowGridLines = "showGridLines";
    /// <summary>
    /// Flag indicating whether the sheet is in 'right to left' display mode.
    /// When in this mode, Column A is on the far right, Column B ;is one column
    /// left of Column A, and so on. Also, information in cells is displayed in
    /// the Right to Left format.
    /// </summary>
    public const string RightToLeft = "rightToLeft";
    /// <summary>
    /// Flag indicating whether this sheet gridline is modified by user
    /// </summary>
    public const string SheetGridColor = "defaultGridColor";
    /// <summary>
    /// String used to specify the color of grid line
    /// </summary>
    public const string ColorID = "colorId";
    /// <summary>
    /// This collection is used to reference binary parts containing arbitrary user-defined data.
    /// </summary>
    public const string CustomPropertiesTagName = "customProperties";
    /// <summary>
    /// The custom property element provides a mechanism to store name/value pairs
    /// of arbitrary user-defined data. The name is stored in the attribute name,
    /// the arbitrary data is stored in the binary part referenced by the relationshipId.
    /// </summary>
    public const string CustomPropertyTagName = "customPr";
    /// <summary>
    /// A collection of ignored errors, by cell range.
    /// </summary>
    public const string IgnoredErrorsTag = "ignoredErrors";
    /// <summary>
    /// A single ignored error type for a range of cells.
    /// </summary>
    public const string IgnoredErrorTag = "ignoredError";
    /// <summary>
    /// 
    /// </summary>
    public const string OnCall = "OLEUPDATE_ONCALL";
    /// <summary>
    /// 
    /// </summary>
    public const string Always = "OLEUPDATE_ALWAYS";
    /// <summary>
    /// Sequence of error types (must be in the same order as in ErrorTagsSequence).
    /// </summary>
    public static ExcelIgnoreError[] ErrorsSequence = new ExcelIgnoreError[]
    {
      ExcelIgnoreError.EmptyCellReferences,
      ExcelIgnoreError.EvaluateToError,
      ExcelIgnoreError.InconsistentFormula,
      ExcelIgnoreError.NumberAsText,
      ExcelIgnoreError.OmittedCells,
      ExcelIgnoreError.TextDate,
      ExcelIgnoreError.UnlockedFormulaCells,
    };
    /// <summary>
    /// String that formulas for Ole images.
    /// </summary>
    private static string[] s_arrFormulas = new string[]
    {
      "if lineDrawn pixelLineWidth 0", 
      "sum @0 1 0", 
      "sum 0 0 @1", 
      "prod @2 1 2", 
      "prod @3 21600 pixelWidth", 
      "prod @3 21600 pixelHeight", 
      "sum @0 0 1", 
      "prod @6 1 2", 
      "prod @7 21600 pixelWidth", 
      "sum @8 21600 0", 
      "prod @7 21600 pixelHeight", 
      "sum @10 21600 0", 
    };
    /// <summary>
    /// String for the BorderColor of OleObject Image
    /// </summary>
    private static string s_color = "windowText [{0}]";
    /// <summary>
    /// Sequence of error xml attribute names corresponding to error types defined in ErrorsSequence.
    /// </summary>
    public static string[] ErrorTagsSequence = new string[]
    {
      "emptyCellReference",
      "evalError",
      "formula",
      "numberStoredAsText",
      "formulaRange",
      "twoDigitTextYear",
      "unlockedFormula",
      //listDataValidation - not supported?
    };
    /// <summary>
    /// Attribute used to store range reference.
    /// </summary>
    public const string RangeReferenceAttribute = "sqref";
    /// <summary>
    /// This element defines properties that track which version of the application
    /// accessed the data and source code contained in the file.
    /// </summary>
    public const string FileVersionTag = "fileVersion";
    /// <summary>
    /// Specifies the incremental public release of the application.
    /// For example, betas, service packs, and versions.
    /// </summary>
    public const string RupBuild = "rupBuild";
    public const string LastEdited = "lastEdited";
    public const string LowestEdited = "lowestEdited";
    /// <summary>
    /// This xml element stores the details of WorkbookPr.
    /// </summary>
    public const string WorkbookPr = "workbookPr";
    /// <summary>
    /// This element stores information about the Date format to be used.
    /// </summary>
    public const string WorkbookDate1904 = "date1904";
    /// <summary>
    /// This element stores information about the Precision to be followed.
    /// </summary>
    public const string WorkbookPrecision = "fullPrecision";
    /// <summary>
    /// Specifies the application name.
    /// </summary>
    public const string ApplicationNameAttribute = "appName";
    /// <summary>
    /// Value of the application name attribute.
    /// </summary>
    public const string ApplicationNameValue = "xl";
    /// <summary>
    /// Flag indicating whether the panes in the window are locked due to workbook protection.
    /// This is an option when the workbook structure is protected.
    /// </summary>
    private const string WindowProtection = "windowProtection";
    public const string FunctionGroups = "functionGroups";
    /// <summary>
    /// Xml attribute containing sheet codename.
    /// </summary>
    public const string CodeName = "codeName";
      /// <summary>
      /// Attribute for showing or hiding the fields list of pivot table in workbook
      /// </summary>
    public const string HidePivotFieldList = "hidePivotFieldList";
        /// <summary>
        /// Xml Tags and Attributes containing Sheet Sparklines.
        /// </summary>
    private const string SpansTag = "spans";
    public const string Extensionlist = "extLst";
    public const string Extension = "ext";
    public const string CalculateOnOpen = "ca";
    /// <summary>
    /// Attribute to show two-dimenssional data table
    /// </summary>
    public const string dataTable2DTag = "dt2D";
    /// <summary>
    /// Attribute to show one-dimentional data table is a row or column
    /// </summary>
    public const string dataTableRowTag = "dtr";
    /// <summary>
    /// Attribute to represent first input cell for data table.
    /// </summary>
    public const string dataTableCellTag = "r1";
    /// <summary>
    /// Chars with codes less than 0x20 which are allowed to be used inside xml strings.
    /// </summary>
    private static readonly char[] allowedChars = new char[] { '\n', '\r', '\t' };
    /// <summary>
    /// First visible character.
    /// </summary>
    private const int FirstVisibleChar = 0x20;
    public const string TransitionEvaluation = "transitionEvaluation";
    /// <summary>
    /// Flag indicating whether this sheet should display row and column headers.
    /// </summary>
    public const string ShowRowColHeaders = "showRowColHeaders";
    private const string VersionValue = "12.0000";
    /// <summary>
    /// Properties Constant
    /// </summary>
    public const string Properties = "properties";
    /// <summary>
    /// DocumentManagement Constalt
    /// </summary>
    public const string DocumentManagement = "documentManagement";
    public const string PCPrefix = "pc";
    public const string PPrefix = "p";
    /// <summary>
    /// Properties NameSpace
    /// </summary>
    public const string PropertiesNameSpace = "http://schemas.microsoft.com/office/2006/metadata/properties";
    /// <summary>
    /// PartnerControls NameSpace
    /// </summary>
    public const string PartnerControlsNameSpace = "http://schemas.microsoft.com/office/infopath/2007/PartnerControls";
    /// <summary>
    /// Default Theme Version
    /// </summary>
    public const string DefaultThemeVersion="defaultThemeVersion" ;
    /// <summary>
    /// Represent the connections
    /// </summary>
    public const string ConnectionsTag = "connections";
    /// <summary>
    /// Represent the connections
    /// </summary>
    public const string ConnectionTag = "connection";
    public const string OdbcFileAttribute = "odcFile";
    public const string DataBaseNameAttribute = "name";
    public const string DataBaseTypeAttribute = "type";
    public const string RefreshedVersionAttribute = "refreshedVersion";
    public const string BackGroundAttribute = "background";
    public const string SaveData = "saveData";
    public const string DataBasePrTag = "dbPr";
    public const string CommandTextAttribute = "command";
    public const string CommandTypeAttribute = "commandType";
    public const string ConnectionIdAttribute = "id";
    public const string SourceFile = "sourceFile";
    public const string DescriptionTag = "description";
    public const string Interval = "interval";
    public const string SavePassword = "savePassword";
    public const string OnlyUseConnectionFile = "onlyUseConnectionFile";
    public const string BackgroundRefresh = "backgroundRefresh";
    public const string Credentials = "credentials";
    public const string Deleted = "deleted";
    public const string TextPr = "textPr";

    /// <summary>
    /// Represent the WebProperties
    /// </summary>
    public const string WebPrTag = "webPr";
    public const string Xml = "xml";
    public const string URL = "url";

    /// <summary>
    /// Represent the Olap Property
    /// </summary>
    public const string OlapPrTag = "olapPr";   

    #region CustomXmlParts Constants

    /// <summary>
    /// CustomXmlParts Constants
    /// </summary>
    public const string CustomXmlPartName = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/customXml";
    public const string CustomXmlName = "customXml/item{0}.xml";
    public const string CustomXmlPropertiesName = "customXml/itemProps{0}.xml";
    public const string CustomXmlRelation = "{0}/_rels/item{1}.xml.rels";
    public const string DataStoreItem = "datastoreItem";
    public const string ItemIdAttribute = "ds:itemID";
    public const string CustomXmlSchemaReferences = "schemaRefs";
    public const string CustomXmlSchemaReference = "schemaRef";
    public const string CustomXmlUriAttribute = "ds:uri";
    public const string XmlItemName = "item{0}.xml";
    public const string XmlPropertiesName = "itemProps{0}.xml";
    public const string CustomXmlNameSpace = "http://schemas.openxmlformats.org/officeDocument/2006/customXml";
    public const string ItemPropertiesPrefix = "ds";
    public const string CustomXmlItemID = "itemID";
    public const string ItemPrpertiesUri = "uri";
    public const string CustomXmlItemPropertiesRelation = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/customXmlProps";

    #endregion
    #endregion

    #region Members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Formula utils.
    /// </summary>
    private FormulaUtil m_formulaUtil;
    /// <summary>
    /// Record extractor.
    /// </summary>
    private RecordExtractor m_recordExtractor;
    /// <summary>
    /// Collection with vml shapes seriliazators.
    /// </summary>
    private Dictionary<int, ShapeSerializator> m_shapesVmlSerializators = new Dictionary<int, ShapeSerializator>();
    /// <summary>
    /// Collection with header/footer vml shapes seriliazators.
    /// </summary>
    private Dictionary<int, ShapeSerializator> m_shapesHFVmlSerializators = new Dictionary<int, ShapeSerializator>();
    /// <summary>
    /// Collection with shape serializators.
    /// </summary>
    private Dictionary<Type, ShapeSerializator> m_shapesSerializators = new Dictionary<Type, ShapeSerializator>();
    /// <summary>
    /// Conditional formats list.
    /// </summary>
    private List<Stream> m_streamsSheetsCF;
    private WorksheetImpl m_worksheetImpl;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the Excel2007Serializator class.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    public Excel2007Serializator( WorkbookImpl book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      m_book = book;
      m_formulaUtil = new FormulaUtil( m_book.Application, m_book, NumberFormatInfo.InvariantInfo,
        ApplicationImpl.DEF_ARGUMENT_SEPARATOR, ApplicationImpl.DEF_ROW_SEPARATOR );
      m_recordExtractor = new RecordExtractor();

      m_shapesVmlSerializators.Add( CommentShapeImpl.ShapeInstance, new CommentShapeSerializator() );
      m_shapesVmlSerializators.Add( CheckBoxShapeImpl.ShapeInstance, new VmlFormControlsSerializator() );
      m_shapesVmlSerializators.Add( BitmapShapeImpl.ShapeInstance, new VmlBitmapSerializator() );

      m_shapesHFVmlSerializators.Add( BitmapShapeImpl.ShapeInstance, new HFImageSerializator() );

      m_shapesSerializators.Add( typeof( BitmapShapeImpl ), new BitmapShapeSerializator() );
      m_shapesSerializators.Add( typeof( ChartShapeImpl ), new ChartShapeSerializator() );
      m_shapesSerializators.Add( typeof( TextBoxShapeImpl ), new TextBoxSerializator() );
    }
      public void Dispose()
      {
          m_shapesVmlSerializators.Clear();
          m_shapesVmlSerializators = null;
          m_shapesSerializators.Clear();
          m_shapesSerializators = null;
          m_shapesHFVmlSerializators.Clear();
          m_shapesHFVmlSerializators = null;
      }
    #endregion

    #region Properties
    /// <summary>
    /// Gets Dictionary with serializators that can be used to serialize header/footer vml shapes. Read-only.
    /// </summary>
    public Dictionary<int, ShapeSerializator> HFVmlSerializators
    {
      get
      {
        return m_shapesHFVmlSerializators;
      }
    }
    /// <summary>
    /// Gets Dictionary with serializators that can be used to serialize vml shapes. Read-only.
    /// </summary>
    public Dictionary<int, ShapeSerializator> VmlSerializators
    {
      get
      {
        return m_shapesVmlSerializators;
      }
    }
    /// <summary>
    /// Gets version that is supported by this serializator.
    /// </summary>
    public virtual ExcelVersion Version
    {
      get
      {
        return ExcelVersion.Excel2007;
      }
    }
    internal WorksheetImpl Worksheet
    {
        get
        {
            return m_worksheetImpl;
    }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Serialize content types.
    /// </summary>
    /// <param name="writer">XmlWriter to save into.</param>
    /// <param name="contentDefaults">
    /// Dictionary which is used to identify content type and stores default types.
    /// Key - file extension (string), Value - content type (string).
    /// </param>
    /// <param name="contentOverrides">
    /// Dictionary which is used to identify content type and stores type overrides.
    /// Key - part name, Value - content type (string).
    /// </param>
    public void SerializeContentTypes( XmlWriter writer, IDictionary<string, string> contentDefaults,
      IDictionary<string, string> contentOverrides )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( contentDefaults == null )
        throw new ArgumentNullException( "contentDefaults" );

      if( contentOverrides == null )
        throw new ArgumentNullException( "contentOverrides" );

      writer.WriteStartDocument( true );
      writer.WriteStartElement( TypesTagName, ContentTypesNamespace );

      SerializeDictionary( writer, contentDefaults, DefaultTagName,
        ExtensionAttributeName, ContentTypeAttributeName, null );

      bool hasStyles = false;
      foreach (KeyValuePair<string, string> dictionary in contentOverrides)
      {
          if (dictionary.Key.Contains("xl/styles.xml"))
          {
              string[] target = dictionary.Key.Split('/');

              if ((target.Length > 3) && hasStyles)
              {
                  contentOverrides.Remove(dictionary.Key);
                  break;
              }
              else
                  hasStyles = true;
          }
      }

      SerializeDictionary( writer, contentOverrides, OverrideTagName,
        PartNameAttributeName, ContentTypeAttributeName, new AddSlashPreprocessor() );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes workbook part (workbook.xml content) into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize workbook properties into.</param>
    /// <param name="streamStart">Stream that contains additional workbook information that
    /// wasn't parsed on load and that is placed before sheets.</param>
    /// <param name="streamEnd">Stream that contains additional workbook information that
    /// wasn't parsed on load and that is placed after named ranges.</param>
    /// <param name="lstBookViews">Workbook views collection.</param>
    /// <param name="relations">Workbook relations.</param>
    /// <param name="cacheFiles">Dictionary that will contain pivot cache files
    /// (key - cache object, value - cache file name).</param>
    /// <param name="functionGroups">Stream containing functionGroups tag data.</param>
    public void SerializeWorkbook( XmlWriter writer, Stream streamStart, Stream streamEnd,
      List<Dictionary<string, string>> lstBookViews, RelationCollection relations,
      Dictionary<PivotCacheImpl, string> cacheFiles, Stream functionGroups )
    {
#if MEASURE_PERFORMANCE
      DateTime methodStart2 = DateTime.Now;
#endif
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( WorkbookTagName, XmlNamespaceMain );
      //writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, XmlNamespaceMain );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, RelationPrefix,
        null, RelationNamespace );

      SerializeFileVersion( writer, m_book.DataHolder.FileVersion );
      SerializeWorkbookPr( writer );
      //SerializeStream( writer, streamStart );
      SerializeWorkbookProtection( writer );
      SerializeBookViews( writer, lstBookViews );
      SerializeSheets( writer );
      SerializeStream( writer, functionGroups );
      SerializeBookExternalLinks( writer, relations );
      SerializeNamedRanges( writer );
      SerializeCalculation( writer );
      SerializePivotCaches( writer, cacheFiles, relations );
      SerializeStream( writer, streamEnd );
      writer.WriteEndElement();
#if MEASURE_PERFORMANCE
      DateTime methodEnd2 = DateTime.Now;
      Console.CursorLeft++;
      Console.WriteLine( "SerializeWorkbook() took: {0}", methodEnd2 - methodStart2 );
#endif
    }

    /// <summary>
    /// Serializes Calculation property
    /// </summary>
    /// <param name="writer"></param>
    private void SerializeCalculation( XmlWriter writer )
    {
      writer.WriteStartElement( CalcProperties );
      bool value = !m_book.PrecisionAsDisplayed;
      SerializeAttribute( writer, WorkbookPrecision, value, !value );
      writer.WriteAttributeString( CalculationId, m_book.DataHolder.CalculationId );
      writer.WriteEndElement();
    }

    /// <summary>
    /// Serializes the workbookPr element
    /// </summary>
    /// <param name="writer"></param>
    private void SerializeWorkbookPr( XmlWriter writer )
    {
      string date = m_book.Date1904 ? "1" : "0";
      writer.WriteStartElement( WorkbookPr );
      SerializeAttribute( writer, WorkbookDate1904, m_book.Date1904, false );

      if( m_book.CodeName != null )
        writer.WriteAttributeString( CodeName, m_book.CodeName );
      //TODO:Theme Record is not preserved

        string defaultThemeVersion=m_book.DefaultThemeVersion;

        if(defaultThemeVersion!=null && defaultThemeVersion.Length>0)
            writer.WriteAttributeString(DefaultThemeVersion, defaultThemeVersion);

      if (!m_book.HidePivotFieldList)
          writer.WriteAttributeString(HidePivotFieldList,"1");
      //writer.WriteAttributeString( WorkbookDate1904, date );
      writer.WriteEndElement();
    }

    private void SerializePivotCaches( XmlWriter writer, Dictionary<PivotCacheImpl, string> cacheFiles,
      RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( cacheFiles == null )
        throw new ArgumentNullException( "cacheFiles" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      PivotCacheCollection caches = m_book.PivotCaches;
      int iCount = ( caches != null ) ? caches.Count : 0;
      FileDataHolder holder = m_book.DataHolder;

      if( iCount > 0 || holder.PreservedCaches.Count > 0 )
      {
        writer.WriteStartElement( PivotTable.PivotCachesTag );

        foreach( KeyValuePair<string, string> pair in holder.PreservedCaches )
        {
          SerializePivotCache( writer, pair.Key, pair.Value );
        }

        //for( int i = 0; i < iCount; i++ )
        if( iCount > 0 )
        {
          foreach( PivotCacheImpl cache in caches )
          {
            //PivotCacheImpl cache = caches[ i ];
            string strCacheFile = cacheFiles[ cache ];
            string strRelationId = relations.GenerateRelationId();
              int index ;
              if (m_book.Options == ExcelParseOptions.DoNotParsePivotTable)
                  index = cache.Index;
              else
                  index = cache.Index + 1;

              SerializePivotCache(writer, index.ToString(), strRelationId);

            relations[ strRelationId ] = new Relation( '/' + strCacheFile, RelationTypes.PivotCacheDefinition );
          }
        }

        writer.WriteEndElement();
      }
    }
    private void SerializePivotCache( XmlWriter writer, string cacheId, string relationId )
    {
      writer.WriteStartElement( PivotTable.PivotCacheTag );
      writer.WriteAttributeString( PivotTable.PivotCacheId, cacheId );
      writer.WriteAttributeString( RelationAttribute, RelationNamespace, relationId );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes file version tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    private void SerializeFileVersion( XmlWriter writer, FileVersion fileVersion )
    {
      // NOTE: this tag is important for example for some shapes, checkboxes won't be displayed correctly without it.
      // <fileVersion appName="xl" lastEdited="4" lowestEdited="4" rupBuild="4506"/>
      writer.WriteStartElement( FileVersionTag );

      if( fileVersion.ApplicationName != null )
        writer.WriteAttributeString( ApplicationNameAttribute, fileVersion.ApplicationName );

      if( fileVersion.LastEdited != null )
        writer.WriteAttributeString( LastEdited, fileVersion.LastEdited );

      if( fileVersion.LowestEdited != null )
        writer.WriteAttributeString( LowestEdited, fileVersion.LowestEdited );

      if( fileVersion.BuildVersion != null )
        writer.WriteAttributeString( RupBuild, fileVersion.BuildVersion );

      if( fileVersion.CodeName != null )
        writer.WriteAttributeString( CodeName, fileVersion.CodeName );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes workbook protection options.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize protection into.</param>
    private void SerializeWorkbookProtection( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( m_book.IsWindowProtection || m_book.IsCellProtection )
      {
        writer.WriteStartElement( Protection.WorkbookProtectionTag );
        PasswordRecord password = m_book.Password;

        if( password != null )
        {
          ushort usPassword = password.IsPassword;

          if( usPassword != 0 )
          {
            writer.WriteAttributeString( Protection.WorkbookPassword, password.IsPassword.ToString( "X4" ) );
          }
        }

        SerializeAttribute( writer, Protection.LockStructureTag, m_book.IsCellProtection, false );
        SerializeAttribute( writer, Protection.LockWindowsTag, m_book.IsWindowProtection, false );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes merged cells.
    /// </summary>
    /// <param name="writer">XmlWriter to save into.</param>
    /// <param name="mergedCells">Object to serialize.</param>
    public void SerializeMerges( XmlWriter writer, MergeCellsImpl mergedCells )
    {
      if( mergedCells == null )
        return;

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      List<Rectangle> arrMergeRegions = mergedCells.MergedRegions;

      if( arrMergeRegions == null || arrMergeRegions.Count == 0 )
        return;

      int iCount = arrMergeRegions.Count;

      writer.WriteStartElement( MergeCellsXmlTagName );
      writer.WriteAttributeString( CountAttributeName, iCount.ToString() );

      for( int i = 0; i < iCount; i++ )
      {
        Rectangle rect = arrMergeRegions[ i ];
        MergeRegion region = mergedCells.RectangleToMergeRegion( rect );
        writer.WriteStartElement( MergeCellXmlTagName );
        writer.WriteAttributeString( RefAttributeName, GetRangeName( region ) );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes named ranges into specified writer.
    /// </summary>
    /// <param name="writer">XmlWriter to save named ranges into.</param>
    public void SerializeNamedRanges( XmlWriter writer )
    {
#if MEASURE_PERFORMANCE
      DateTime methodStart = DateTime.Now;
#endif
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      WorkbookNamesCollection names = m_book.InnerNamesColection;
      int iCount = ( names != null ) ? names.Count : 0;

      if( iCount > 0 )
      {
        names.SortForSerialization();
        writer.WriteStartElement( DefinedNamesXmlTagName );

        for( int i = 0; i < iCount; i++ )
        {
          NameImpl name = ( NameImpl )names[ i ];

          if( !name.Record.IsFunctionOrCommandMacro )
            SerializeNamedRange( writer, name );
        }

        writer.WriteEndElement();
      }
#if MEASURE_PERFORMANCE
      DateTime methodEnd = DateTime.Now;
      Console.CursorLeft += 2;
      Console.WriteLine( "SerializeNamedRanges() took: {0}", methodEnd - methodStart );
#endif
    }
    /// <summary>
    /// Serializes ItemProperties and Schemas Collections
    /// </summary>
    /// <param name="writer">XmlWriter to serialize Schemas into.</param>
    /// <param name="customXmlPart">CustomXmlPartparam>
    public void SerializeCustomXmlPartProperty(XmlWriter writer,ICustomXmlPart customXmlPart)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (customXmlPart == null)
            throw new ArgumentNullException("customXmlPart");

        string id = customXmlPart.Id;
        ICustomXmlSchemaCollection schemaCollection = customXmlPart.Schemas;

        writer.WriteStartDocument(true);
        writer.WriteStartElement(ItemPropertiesPrefix, DataStoreItem, CustomXmlNameSpace);

        writer.WriteAttributeString(ItemPropertiesPrefix, CustomXmlItemID, null, id);

        if (schemaCollection != null && schemaCollection.Count > 0)
        {
            writer.WriteStartElement(ItemPropertiesPrefix, CustomXmlSchemaReferences, null);

            foreach (string schemas in schemaCollection)
            {
                writer.WriteStartElement(ItemPropertiesPrefix, CustomXmlSchemaReference, null);
                writer.WriteAttributeString(ItemPropertiesPrefix, ItemPrpertiesUri, null, schemas);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes styles collection into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize styles into.</param>
    /// <param name="streamDxfs">Stream that contains Dxfs formatting tags.</param>
    /// <returns>Dictionary with new XF indexes.</returns>
    public Dictionary<int, int> SerializeStyles( XmlWriter writer, ref Stream streamDxfs )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartDocument( true );
      writer.WriteStartElement( StyleSheetTagName, XmlNamespaceMain );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, "mc",
        null, "http://schemas.openxmlformats.org/markup-compatibility/2006" );

      writer.WriteAttributeString( "Ignorable", "http://schemas.openxmlformats.org/markup-compatibility/2006", "x14ac" );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, "x14ac", null, "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac" );
      //xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      //mc:Ignorable="x14ac"
      //xmlns:x14ac="http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac"

      SerializeNumberFormats( writer );
      SerializeFonts( writer );
      int[] arrFillIndex = SerializeFills( writer );
      int[] arrBorderIndex = SerializeBorders( writer );

      ExtendedFormatsCollection arrExtFormats = m_book.InnerExtFormats;
      int iCount = arrExtFormats.Count;

      Dictionary<int, int> hashNamedStyleIndexes = SerializeNamedStyleXFs( writer, arrFillIndex,
        arrBorderIndex );

      Dictionary<int, int> hashNewXFIndexes = SerializeNotNamedXFs( writer, arrFillIndex,
        arrBorderIndex, hashNamedStyleIndexes );

      SerializeStyles( writer, hashNamedStyleIndexes );

      if( streamDxfs == null || m_book.DataHolder.ParsedDxfsCount != int.MinValue )
      {
        WorksheetImpl sheet;
        Stream streamCF;
        Stream tempStreamDxfx = new MemoryStream();
        m_streamsSheetsCF = new List<Stream>();
        WorksheetsCollection worksheets = m_book.InnerWorksheets;
        int iDxfIndex = 0;
        for( int i = 0, iSheetCount = worksheets.Count; i < iSheetCount; i++ )
        {
          sheet = ( WorksheetImpl )worksheets[ i ];
          WorksheetConditionalFormats conditionalFormats = sheet.ConditionalFormats;
          streamCF = SerializeDxfs(ref tempStreamDxfx, conditionalFormats, ref iDxfIndex);
          m_streamsSheetsCF.Add( streamCF );
        }
        m_book.BookCFPriorityCount = 0;
        streamDxfs = tempStreamDxfx;
      }

      SerializeStream( writer, streamDxfs );

      SerializeStream(writer, m_book.CustomTableStylesStream);

      SerializeColors( writer );

      Stream extensions = m_book.DataHolder.ExtensionStream;

      if( extensions != null )
      {
        extensions.Position = 0;
        ShapeParser.WriteNodeFromStream( writer, extensions, true );
      }

      writer.WriteEndElement();
      return hashNewXFIndexes;
    }
    /// <summary>
    /// Serializes relations collection into specified xml writer.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="relations">Relations to serialize.</param>
    public void SerializeRelations( XmlWriter writer, RelationCollection relations, WorksheetDataHolder holder )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( relations == null || relations.Count == 0 )
        return;

      writer.WriteStartDocument( true );
      writer.WriteStartElement( RelationsTagName, /*RelationNamespace*/RelationTypes.PackageNamespace );

      foreach( KeyValuePair<string, Relation> entry in relations )
      {
          if (holder != null)
          {
              //TODO: Need to remove this, once Chart Color and Chart Style support is implemented
#if (SILVERLIGHT)
              if (!(Array.IndexOf((ZipArchiveItem[])holder.ParentHolder.Archive.Items, entry.Value.Target, 0, holder.ParentHolder.Archive.Items.Length) < 0 &&
#else
              if (!(Array.IndexOf((ZipArchiveItem[])holder.ParentHolder.Archive.Items, entry.Value.Target) < 0 &&
#endif
                    (entry.Value.Type == ChartConstants.CColorNamespace2013 ||
                     entry.Value.Type == ChartConstants.CStyleNamespace2013)))
                  SerializeRelation(writer, entry.Key, entry.Value);
          }
          else
              SerializeRelation(writer, entry.Key, entry.Value);
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single worksheet object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    /// <param name="streamStart">Stream with xml text starting from the
    /// beginning of worksheet to sheetData or cols tag.</param>
    /// <param name="streamConFormats">Stream with conditional formatting.</param>
    /// <param name="hashXFIndexes">Dictionary with new XF indexes, key - old index, value - new index.</param>
    public void SerializeWorksheet( XmlWriter writer, WorksheetImpl sheet,
      Stream streamStart, Stream streamConFormats, Dictionary<int, int> hashXFIndexes, Stream streamExtCondFormats )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( streamStart == null )
        throw new ArgumentNullException( "streamStart" );

      m_worksheetImpl = sheet;
      writer.WriteStartDocument( true );
      writer.WriteStartElement( WorksheetTagName, XmlNamespaceMain );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, RelationPrefix,
        null, RelationNamespace );


  //    writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, "xdr",
  //null, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing" );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, X14Prefix,
        null, X14Namespace );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, MCPrefix,
        null, MCNamespace );

      //writer.WriteAttributeString( "Ignorable", "http://schemas.openxmlformats.org/markup-compatibility/2006", "x14ac" );
      //writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, "x14ac",
      //  null, "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac" );

      SerializeSheetlevelProperties( writer, sheet );
      SerializeDimensions( writer, sheet );
      SerializeSheetViews( writer, sheet );

         writer.WriteStartElement( SheetFormatPropertiesTag );
        if(sheet.StandardWidth != 8.43)
            writer.WriteAttributeString(DefaultColumWidthAttribute, XmlConvert.ToString(sheet.StandardWidth));
        if (m_worksheetImpl.IsZeroHeight)
            writer.WriteAttributeString(ZeroHeightAttribute, XmlConvert.ToString(true));
        if (sheet.CustomHeight && sheet.ListObjects.Count==0)
            writer.WriteAttributeString(RowCustomHeightAttributeName, XmlConvert.ToString(true));
        writer.WriteAttributeString( DefaultRowHeightAttribute, XmlConvert.ToString( sheet.StandardHeight ) );
          if (sheet.OutlineLevelColumn > 0)
                    writer.WriteAttributeString(OutlineLevelColAttribute, XmlConvert.ToString(sheet.OutlineLevelColumn));
          if (sheet.OutlineLevelRow > 0)
                    writer.WriteAttributeString(OutlineLevelRowAttribute, XmlConvert.ToString(sheet.OutlineLevelRow));
          if (sheet.BaseColumnWidth != 8)
                    writer.WriteAttributeString(BaseColWidthAttribute, XmlConvert.ToString(sheet.BaseColumnWidth));
          if (sheet.IsThickTop)
                    writer.WriteAttributeString(ThickTopAttribute, XmlConvert.ToString(true));
          if (sheet.IsThickBottom)
                    writer.WriteAttributeString(ThickBottomAttribute, XmlConvert.ToString(true));
                
        writer.WriteEndElement();

        if (sheet.PivotTables.Count > 0)
        {
            for (int index = 0; index < sheet.PivotTables.Count; index++)
            {
                PivotTableImpl pivotTable = sheet.PivotTables[index] as PivotTableImpl;
                PivotTableOptions options = pivotTable.Options as PivotTableOptions;
                if (pivotTable.IsChanged && options.RowLayout == PivotTableRowLayout.Tabular)
                {
                    pivotTable.AutoFitPivotTable(pivotTable);
                }
            }
        }
      SerializeColumns( writer, sheet, hashXFIndexes );
      SerializeSheetData( writer, sheet.CellRecords, hashXFIndexes, CellTagName, null, true );

      //writer.WriteStartElement( "sheetCalcPr" );
      //writer.WriteAttributeString( "fullCalcOnLoad", "1" );
      //writer.WriteEndElement();

      SerializeSheetProtection( writer, sheet );
      SerializeAutoFilters( writer, sheet.AutoFilters );
      SerializeMerges( writer, sheet.MergeCells );

      ////<phoneticPr fontId="0" type="noConversion"/>
      //writer.WriteStartElement( "phoneticPr" );
      //writer.WriteAttributeString( "fontId", "0" );
      //writer.WriteAttributeString( "type", "noConversion" );
      //writer.WriteEndElement();

      Stream stream = ( streamConFormats != null && streamConFormats.Length != 0 ) ? streamConFormats :
        GetWorksheetCFStream( sheet.Index );

      SerializeStream( writer, stream );
      SerializeDataValidations( writer, sheet.DVTable );
      SerializeHyperlinks( writer, sheet );
        
      IPageSetupConstantsProvider constants = new WorksheetPageSetupConstants();
      SerializePrintSettings( writer, sheet.PageSetup, constants,false );
      SerializePagebreaks( writer, sheet );
      SerializeCustomProperties( writer, sheet );
      SerializeIgnoreErrors( writer, sheet );
      SerializeDrawingsWorksheetPart( writer, sheet );
      SerializeVmlShapesWorksheetPart( writer, sheet );
      SerializeVmlHFShapesWorksheetPart( writer, sheet, constants, null );

# if !SILVERLIGHT && !WINRT && !WP
      SerializeOle( writer, sheet );
#endif
      SerilizeBackgroundImage( writer, sheet );
      SerializeControls( writer, sheet );
      sheet.DataHolder.SerializeTables( writer, sheet );

      //if( sheet.Version == ExcelVersion.Excel2010 && sheet.SparklineGroups.Count > 0 )
      //{
      //  Excel2010Serializator serializator = new Excel2010Serializator( m_book );
      //  serializator.SerilaizeExtensions( writer, sheet );
      //}

      if (sheet.Version != ExcelVersion.Excel97to2003 && sheet.Version != ExcelVersion.Excel2007)
      {
          Excel2010Serializator serializator = new Excel2010Serializator(m_book);
          serializator.SerilaizeExtensions(writer, sheet);
      }

      if (sheet.WorksheetSlicerStream!= null )
      {
          writer.WriteStartElement(Excel2007Serializator.Extensionlist);
          writer.WriteStartElement(Excel2007Serializator.Extension);
          writer.WriteAttributeString(SparkConstants.UriAttribute, Excel2007Serializator.SlicerExtensionUri);
          writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, Excel2007Serializator.X14Prefix, null, Excel2007Serializator.X14Namespace);
          sheet .WorksheetSlicerStream.Position = 0;
          ShapeParser.WriteNodeFromStream(writer, sheet .WorksheetSlicerStream );
          writer.WriteEndElement();
          writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    protected virtual void SerilaizeExtensions( XmlWriter writer, WorksheetImpl sheet )
    {
    }
    /// <summary>
    /// Serializes controls additional information.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    private void SerializeControls( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      WorksheetDataHolder holder = sheet.DataHolder;
      Stream stream = holder.ControlsStream;

      if( stream != null )
      {
        bool bAlternate = HasAlternateContent( sheet.Shapes );

        if (bAlternate && sheet.HasAlternateContent)
            WriteAlternateContentControlsHeader(writer);

        stream.Position = 0;
        XmlReader reader = UtilityMethods.CreateReader( stream );
        writer.WriteNode( reader, false );
        writer.Flush();

        if (bAlternate && sheet.HasAlternateContent)
            WriteAlternateContentFooter(writer);
      }
    }

    public static void WriteAlternateContentFooter( XmlWriter writer )
    {
      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    public static void WriteAlternateContentHeader( XmlWriter writer )
    {
      // TODO: move to constants.
      writer.WriteStartElement( Excel2007Serializator.MCPrefix, Drawings.AlternateContentTag, Excel2007Serializator.MCNamespace );
      writer.WriteStartElement( Excel2007Serializator.MCPrefix, Drawings.ChoiceTag, Excel2007Serializator.MCNamespace );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, "a14", null, "http://schemas.microsoft.com/office/drawing/2010/main" );
      writer.WriteAttributeString( "Requires", "a14" );
    }
    public static void WriteAlternateContentControlsHeader(XmlWriter writer)
    {
        //// TODO: Remove this method once the Parsing support is added for the Alternative Content elements for Controls.

        writer.WriteStartElement(Excel2007Serializator.MCPrefix, Drawings.AlternateContentTag, Excel2007Serializator.MCNamespace);
        
        writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, Excel2007Serializator.MCPrefix,null, "http://schemas.openxmlformats.org/markup-compatibility/2006");
        writer.WriteStartElement(Excel2007Serializator.MCPrefix, Drawings.ChoiceTag, Excel2007Serializator.MCNamespace);
        writer.WriteAttributeString("Requires", "x14");
    }
    /// <summary>
    /// Serializes protection options if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to get protection options from.</param>
    public void SerializeSheetProtection( XmlWriter writer, WorksheetBaseImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      ExcelSheetProtection protection = sheet.InnerProtection;
      bool isChart = ( sheet is Syncfusion.XlsIO.Implementation.Charts.ChartImpl );

      if( sheet.ProtectContents || ( isChart && protection != ExcelSheetProtection.None ) )
      {
        writer.WriteStartElement( Protection.SheetProtectionTag );

        if( sheet.IsPasswordProtected )
        {
            if (sheet.Password.IsPassword != 1)
            {
                string password = sheet.Password.IsPassword.ToString("X");
                writer.WriteAttributeString(Protection.PasswordAttribute, password);
            }
        }

        string[] attributes;
        bool[] defaultValues;
        ProtectionAttributeSerializator serialize;

        if( !isChart )
        {
          serialize = SerializeProtectionAttribute;
          attributes = Protection.ProtectionAttributes;
          defaultValues = Protection.DefaultValues;
        }
        else
        {
          serialize = SerializeChartProtectionAttribute;
          attributes = Protection.ChartProtectionAttributes;
          defaultValues = Protection.ChartDefaultValues;
        }

        for( int i = 0, iCount = attributes.Length; i < iCount; i++ )
        {
          serialize( writer,
            attributes[ i ],
            Protection.ProtectionFlags[ i ],
            defaultValues[ i ],
            protection );
        }

        writer.WriteEndElement();
      }
    }
    delegate void ProtectionAttributeSerializator( XmlWriter writer, string attributeName,
      ExcelSheetProtection flag, bool defaultValue, ExcelSheetProtection protection );
    /// <summary>
    /// Serializes single protection option.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize protection into.</param>
    /// <param name="attributeName">Name of the attribute to serialize.</param>
    /// <param name="flag">Flag that corresponds to the attribute.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <param name="protection">Current protection settings.</param>
    private void SerializeProtectionAttribute( XmlWriter writer, string attributeName,
      ExcelSheetProtection flag, bool defaultValue, ExcelSheetProtection protection )
    {
      bool value = ( protection & flag ) == 0;
      SerializeAttribute( writer, attributeName, value, defaultValue );
    }
    /// <summary>
    /// Serializes single protection option.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize protection into.</param>
    /// <param name="attributeName">Name of the attribute to serialize.</param>
    /// <param name="flag">Flag that corresponds to the attribute.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <param name="protection">Current protection settings.</param>
    private void SerializeChartProtectionAttribute( XmlWriter writer, string attributeName,
      ExcelSheetProtection flag, bool defaultValue, ExcelSheetProtection protection )
    {
      bool value = ( protection & flag ) != 0;
      SerializeAttribute( writer, attributeName, value, defaultValue );
    }
    /// <summary>
    /// Serializes ignore errors options.
    /// </summary>
    /// <param name="writer">XmlWriter writer to serialize options into.</param>
    /// <param name="sheet">Worksheet to serialize options for.</param>
    private void SerializeIgnoreErrors( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      ErrorIndicatorsCollection indicators = sheet.ErrorIndicators;

      if( indicators == null || indicators.Count == 0 )
        return;

      int iCount = indicators.Count;
      bool bNeedSerialization = false;

      for( int i = 0; i < iCount; i++ )
      {
        if( indicators[ i ].IgnoreOptions != ExcelIgnoreError.None )
        {
          bNeedSerialization = true;
          break;
        }
      }

      if( bNeedSerialization )
      {
        writer.WriteStartElement( IgnoredErrorsTag );

        for( int i = 0, len = indicators.Count; i < len; i++ )
        {
          ErrorIndicatorImpl indicator = indicators[ i ];
          SerializeErrorIndicator( writer, indicator );
        }

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes single error indicator.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="indicator">Error indicator to serialize.</param>
    private void SerializeErrorIndicator( XmlWriter writer, ErrorIndicatorImpl indicator )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( indicator == null )
        throw new ArgumentNullException( "indicator" );

      if( indicator.IgnoreOptions != ExcelIgnoreError.None )
      {
        writer.WriteStartElement( IgnoredErrorTag );
        string strCells = GetCellList( indicator );
        writer.WriteAttributeString( RangeReferenceAttribute, strCells );
        SerializeErrorType( writer, indicator.IgnoreOptions );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes all required attributes for error indicator option.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="excelIgnoreError">Ignore error value.</param>
    private void SerializeErrorType( XmlWriter writer, ExcelIgnoreError excelIgnoreError )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      for( int i = 0, iCount = ErrorsSequence.Length; i < iCount; i++ )
      {
        if( ( excelIgnoreError & ErrorsSequence[ i ] ) != 0 )
        {
          writer.WriteAttributeString( ErrorTagsSequence[ i ], TrueValue );
        }
      }
    }
    /// <summary>
    /// Converts cell list of the error indicator into string.
    /// </summary>
    /// <param name="indicator">Error indicator to get list for.</param>
    /// <returns>String representation of cell list for specified error indicator.</returns>
    private string GetCellList( ErrorIndicatorImpl indicator )
    {
      if( indicator == null )
        throw new ArgumentNullException( "indicator" );

      List<Rectangle> list = indicator.CellList;
      StringBuilder result = new StringBuilder();

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        Rectangle rect = list[ i ];
        string strCell = RangeImpl.GetCellName( rect.Left + 1, rect.Top + 1 );
        result.Append( strCell );

        if( rect.Left != rect.Right || rect.Top != rect.Bottom )
        {
          result.Append( ':' );
          strCell = RangeImpl.GetCellName( rect.Right + 1, rect.Bottom + 1 );
          result.Append( strCell );
        }

        if( i != len - 1 )
          result.Append( ' ' );
      }

      return result.ToString();
    }
    /// <summary>
    /// Serializes custom worksheet properties if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    private void SerializeCustomProperties( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      IWorksheetCustomProperties properties = sheet.InnerCustomProperties;

      if( properties == null || properties.Count == 0 )
        return;

      writer.WriteStartElement( CustomPropertiesTagName );

      for( int i = 0, len = properties.Count; i < len; i++ )
      {
        SerializeWorksheetProperty( writer, sheet, properties[ i ] ,i);
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes worksheet custom property
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    /// <param name="property">Represents custom document property.</param>
    private void SerializeWorksheetProperty( XmlWriter writer, WorksheetImpl sheet, ICustomProperty property ,int counter)
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( property == null )
        throw new ArgumentNullException( "property" );

      WorksheetDataHolder sheetHolder = sheet.DataHolder;
      FileDataHolder dataHolder = sheetHolder.ParentHolder;
      RelationCollection sheetRelations = sheetHolder.Relations;

      writer.WriteStartElement( CustomPropertyTagName );
      writer.WriteAttributeString( NameAttributeName, property.Name );
      // TODO: replace counter;
      ZipArchiveItem zipItem;

      string relationId = dataHolder.PrepareNewItem( FileDataHolder.CustomPropertyPathStart,
        FileDataHolder.BinaryExtension,
        ContentTypes.WorksheeetCustomProperty,
        sheetRelations,
        RelationTypes.WorksheetCustomProperty,
        ref counter,
        out zipItem );

      writer.WriteAttributeString( RelationAttribute, RelationNamespace, relationId );
      WritePropertyValue( sheet, zipItem, property );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Writes property data into specified item.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="item">Zip archive item to write property value into.</param>
    /// <param name="property">Property to get data from.</param>
    private void WritePropertyValue( WorksheetImpl sheet, ZipArchiveItem item, ICustomProperty property )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( item == null )
        throw new ArgumentNullException( "item" );

      if( property == null )
        throw new ArgumentNullException( "property" );

      byte[] arrPropertyData = Encoding.Unicode.GetBytes( property.Value );
      Stream stream = item.DataStream;
      stream.Write( arrPropertyData, 0, arrPropertyData.Length );
    }
    /// <summary>
    /// Returns worksheet stream that contains its conditional formats.
    /// </summary>
    /// <param name="iSheetIndex">Sheet index.</param>
    /// <returns>Stream with conditional formats.</returns>
    private Stream GetWorksheetCFStream( int iSheetIndex )
    {
      return ( m_streamsSheetsCF != null ) ? m_streamsSheetsCF[ iSheetIndex ] : null;
    }
    /// <summary>
    /// Serializes comment notes part (authors, text and parent cell address).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to get comments from.</param>
    public void SerializeCommentNotes( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      writer.WriteStartDocument( true );
      writer.WriteStartElement( CommentsTagName, XmlNamespaceMain );
      // 1. we have to go through all comments and get list of authors
      IDictionary<string, int> dicAuthors = SerializeAuthors( writer, sheet );
      // 2. we have to serialize comments itself.
      SerializeCommentsList( writer, sheet, dicAuthors );
      writer.WriteEndElement();

      // Create comments relation id.
      WorksheetDataHolder holder = sheet.DataHolder;
      string strCommentsRelation = holder.CommentNotesId;

      if( strCommentsRelation == null )
      {
        holder.CommentNotesId = holder.Relations.GenerateRelationId();
      }
    }
    /// <summary>
    /// Serializes vml shapes.
    /// WARNING: this method doesn't check whether there are any shapes to serialize or not.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shapes">Shapes to serialize.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="dictSerializators">Dictionary with all known vml shapes serializators.</param>
    public void SerializeVmlShapes( XmlWriter writer, ShapeCollectionBase shapes,
      WorksheetDataHolder holder, Dictionary<int, ShapeSerializator> dictSerializators,
      RelationCollection vmlRelations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      //CommentsCollection arrComments = shapes.InnerComments;
      //int iCount = ( arrComments != null ) ? arrComments.Count : 0;

      writer.WriteStartElement( Vml.XmlTagName );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Vml.VPreffix,
        null, Vml.VNamespace );

      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Vml.OPreffix,
        null, Vml.ONamespace );

      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Vml.XPreffix,
        null, Vml.XNamespace );

      ShapeSerializator shapeSerializator;

      //if( m_shapesVmlSerializators.TryGetValue( typeof( CommentShapeImpl ),
      //  out shapeSerializator ) )
      //{
      //  shapeSerializator.SerializeShapeType( writer );
      //}
      UniqueInstanceTypeList uniqueList = new UniqueInstanceTypeList();
      Dictionary<Stream, object> unknownShapeTypes = new Dictionary<Stream, object>();

      for( int i = 0, len = shapes.Count; i < len; i++ )
      {
        ShapeImpl shape = shapes[ i ] as ShapeImpl;
        shape.PrepareForSerialization();

        if( shape.VmlShape )
        {
          if( shape.XmlTypeStream == null )
          {
            uniqueList.AddShape( shape );
          }
          else
          {
            unknownShapeTypes[ shape.XmlTypeStream ] = null;

            if( shape.ImageRelation != null )
            {
              vmlRelations[ shape.ImageRelationId ] = shape.ImageRelation;
            }
          }
        }
      }
      if (shapes.ShapeLayoutStream != null)
      {
          Stream shapeLayoutStream = shapes.ShapeLayoutStream;
          shapeLayoutStream.Position = 0;
          ShapeParser.WriteNodeFromStream(writer, shapeLayoutStream);
      }

      foreach( KeyValuePair<int, Type> pair in uniqueList.UniquePairs() )
      {
        int instance = pair.Key;
        Type shapeType = pair.Value;
        ShapeSerializator serializator;

        if( dictSerializators.TryGetValue( instance, out serializator ) )
        {
          serializator.SerializeShapeType( writer, shapeType );
        }
      }

      foreach( Stream stream in unknownShapeTypes.Keys )
      {
        stream.Position = 0;
        XmlReader reader = UtilityMethods.CreateReader( stream );
        //reader.Read();
        writer.WriteNode( reader, false );
      }

      for( int i = 0, len = shapes.Count; i < len; i++ )
      {
        ShapeImpl shape = ( ShapeImpl )shapes[ i ];
        shape.PrepareForSerialization();
        int instance = shape.Instance;

        if( shape.VmlShape )
        {
          if( ( shape.XmlDataStream == null || shape.EnableAlternateContent ) && dictSerializators.TryGetValue( instance, out shapeSerializator ) )
          {
            shapeSerializator.Serialize( writer, shape, holder, vmlRelations );
          }
          else if( shape.XmlDataStream != null )
          {
            Stream stream = shape.XmlDataStream;
            stream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader( stream );
            //reader.Read();
            writer.WriteNode( reader, false );
          }
        }
      }

      writer.WriteEndElement();
      writer.Flush();
    }
    /// <summary>
    /// Serializes drawings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shapes">Shapes to serialize.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    public void SerializeDrawings( XmlWriter writer, ShapesCollection shapes,
      WorksheetDataHolder holder )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      if( shapes.Count - shapes.WorksheetBase.VmlShapesCount <= 0 && !HasAlternateContent( shapes ) )
        return;

      bool bChartSheet = shapes.Worksheet == null;
      string strPreffix;
      string strNamespace;
      string strMainTag;
      string strMainTagNamespace = null;
      string strPrefix2 = null;

      if( bChartSheet )
      {
        strPreffix = Drawings.CdrPreffix;
        strNamespace = Drawings.CdrNamespace;
        strMainTag = ChartConstants.UserShapesTag;
        strMainTagNamespace = ChartConstants.CNamespace;
      }
      else
      {
        strPreffix = Drawings.XdrPreffix;
        strNamespace = Drawings.XdrNamespace;
        strPrefix2 = Drawings.XdrPreffix;
        strMainTag = Drawings.WorksheetDrawings;
        strMainTagNamespace = strNamespace;
      }

      writer.WriteStartDocument( true );
      writer.WriteStartElement( strPrefix2, strMainTag, strMainTagNamespace );

      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, strPreffix,
        null, strNamespace );

      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Drawings.APreffix,
        null, Drawings.ANamespace );

      ShapeSerializator shapeSerializator;

      int index = 0;

      for( int i = 0, len = shapes.Count; i < len; i++ )
      {
        ShapeImpl shape = ( ShapeImpl )shapes[ i ];
        switch (shape.ShapeType)
        {
            case ExcelShapeType.AutoShape:
                Serializator serialize = new Serializator();
                AutoShapeImpl autoShapeImpl =shape as AutoShapeImpl;
                if (autoShapeImpl.ShapeExt.ShapeID <= 0)
                    autoShapeImpl.ShapeExt.ShapeID = i + 1;
                serialize.AddShape(autoShapeImpl.ShapeExt, (XmlWriter)writer);
                //index += 1;
                break;
            default :
                if (!shape.VmlShape || shape.EnableAlternateContent)
                {
                    if (m_shapesSerializators.TryGetValue(shape.GetType(), out shapeSerializator))
                    {
                        shapeSerializator.Serialize(writer, shape, holder, holder.DrawingsRelations);
                    }
                    else if ((!shape.VmlShape || shape.EnableAlternateContent) && shape.XmlDataStream != null && !bChartSheet)
                    {
                        DrawingShapeSerializator serializator = new DrawingShapeSerializator();
                        serializator.Serialize(writer, shape, holder, holder.DrawingsRelations);
                    }
                    else
                    {
                        if (Worksheet.preservedStreams != null ||
                            shape.preservedCnxnShapeStreams != null ||
                            shape.preservedPictureStreams != null ||
                            shape.preservedShapeStreams != null)
                        {
                            SerializeShape(writer, shape, holder, holder.DrawingsRelations, index);
                            index += 1;
                        }
                    }
                }
                break;

        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// This method serializes specified shape into specified writer.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize shape settings into.</param>
    /// <param name="shape">Shape to serialize.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    public void SerializeShape(XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
      RelationCollection vmlRelations, int index)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (shape == null)
            throw new ArgumentNullException("shape");

        if (holder == null)
            throw new ArgumentNullException("holder");

       
        if (shape.GetType() is ChartShapeImpl)
        {
            ChartShapeImpl chartShape = ((ChartShapeImpl)shape);
            Implementation.Charts.ChartImpl chart = chartShape.ChartObject;
            string strChartFileName;
            ChartShapeSerializator chartShapeSerie = new ChartShapeSerializator();
            string strRelationId = (chartShapeSerie as ChartShapeSerializator).SerializeChartFile(holder, chart, out strChartFileName);

            (chartShapeSerie as ChartShapeSerializator).SerializeChartProperties(writer, chartShape, strRelationId, holder, false);

            writer.WriteEndElement();

            holder.SerializeRelations(chart.Relations, strChartFileName.Substring(1), null);
        }
        else
        {
                writer.WriteStartElement(Drawings.TwoCellAnchorTagName, Drawings.XdrNamespace);                

                DrawingShapeSerializator drawingShapeSerie = new DrawingShapeSerializator();
                (drawingShapeSerie as DrawingShapeSerializator).SerializeAnchorPoint(writer, Drawings.FromTagName,
                  shape.LeftColumn, shape.LeftColumnOffset,
                  shape.TopRow, shape.TopRowOffset, shape.Worksheet,
                  Drawings.XdrNamespace);

                (drawingShapeSerie as DrawingShapeSerializator).SerializeAnchorPoint(writer, Drawings.ToTagName,
                  shape.RightColumn, shape.RightColumnOffset,
                  shape.BottomRow, shape.BottomRowOffset, shape.Worksheet,
                  Drawings.XdrNamespace);

                if (shape.preservedCnxnShapeStreams != null)
                    for (int i = 0; i < shape.preservedCnxnShapeStreams.Count; i++)
                        SerializeStream(writer, shape.preservedCnxnShapeStreams[i]);
                if (shape.GraphicFrameStream != null)
                {
                    ChartShapeImpl chartShape = ((ChartShapeImpl)shape.ChildShapes[0]);
                    Implementation.Charts.ChartImpl chart = chartShape.ChartObject;
                    string strChartFileName;
                    ChartShapeSerializator chartShapeSerie = new ChartShapeSerializator();
                    string strRelationId = (chartShapeSerie as ChartShapeSerializator).SerializeChartFile(holder, chart, out strChartFileName);
                    writer.WriteStartElement(Drawings.GraphicFrame, Drawings.XdrNamespace);
                    writer.WriteAttributeString(Drawings.MacroAttribute, string.Empty);
                    chartShapeSerie.SerializeNonVisualGraphicFrameProperties(writer, chartShape, holder);
                    DrawingShapeSerializator.SerializeForm(writer, Drawings.XdrNamespace, Drawings.ANamespace, chartShape.OffsetX, chartShape.OffsetY, chartShape.ExtentsX, chartShape.ExtentsY);
                    chartShapeSerie.SerializeSlicerGraphics(writer, chartShape);
                    shape.ChildShapes.Remove(chartShape);
                }
                else if (shape.preservedShapeStreams != null || 
                    shape.preservedPictureStreams != null ||
                    shape.ChildShapes.Count > 0)
                {
                    writer.WriteStartElement(Drawings.GroupShape, Drawings.XdrNamespace);

                    index = index * 2;

                    if (Worksheet.preservedStreams != null && index < Worksheet.preservedStreams.Count)
                    {
                        SerializeStream(writer, Worksheet.preservedStreams[index]);
                        SerializeStream(writer, Worksheet.preservedStreams[index + 1]);
                    }

                    if (shape.preservedShapeStreams != null)
                        for (int i = 0; i < shape.preservedShapeStreams.Count; i++)
                            SerializeStream(writer, shape.preservedShapeStreams[i]);

                    if (shape.preservedPictureStreams != null)
                        for (int i = 0; i < shape.preservedPictureStreams.Count; i++)
                            SerializeStream(writer, shape.preservedPictureStreams[i]);

                    if (shape.preservedInnerCnxnShapeStreams != null)
                        for (int i = 0; i < shape.preservedInnerCnxnShapeStreams.Count; i++)
                            SerializeStream(writer, shape.preservedInnerCnxnShapeStreams[i]);

                    for (int j = 0; j < shape.ChildShapes.Count; j++)
                    {
                        ChartShapeImpl chartShape = ((ChartShapeImpl)shape.ChildShapes[j]);
                        Implementation.Charts.ChartImpl chart = chartShape.ChartObject;
                        string strChartFileName;
                        ChartShapeSerializator chartShapeSerie = new ChartShapeSerializator();
                        string strRelationId = (chartShapeSerie as ChartShapeSerializator).SerializeChartFile(holder, chart, out strChartFileName);
                        (chartShapeSerie as ChartShapeSerializator).SerializeChartProperties(writer, chartShape, strRelationId, holder, true);

                        //writer.WriteEndElement();

                        holder.SerializeRelations(chart.Relations, strChartFileName.Substring(1), null);
                    }

                    writer.WriteEndElement();
                }
                writer.WriteElementString(Drawings.ClientDataTagName, Drawings.XdrNamespace, string.Empty);
                writer.WriteEndElement();
        }

        
    }

    public static bool HasAlternateContent( IShapes shapes )
    {
      foreach( ShapeImpl shape in shapes )
      {
        if( shape.EnableAlternateContent )
          return true;
      }

      return false;
    }
#if MEASURE_PERFORMANCE
    public TimeSpan m_externalLinksTotal = new TimeSpan( 0 );
#endif
    public RelationCollection SerializeLinkItem( XmlWriter writer, ExternWorkbookImpl book )
    {
      if( book.IsAddInFunctions || book.IsInternalReference )
        return null;

      return ( book.IsOleLink ) ?
        SerializeOleObjectLink( writer, book ) :
        SerializeExternalLink( writer, book );
    }

    /// <summary>
    /// Serializes external workbook into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="book">External workbook to serialize.</param>
    /// <returns>Collection with external link item relations.</returns>
    public RelationCollection SerializeExternalLink( XmlWriter writer, ExternWorkbookImpl book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      if( book.IsInternalReference || book.IsAddInFunctions )
        return null;

      RelationCollection relations = new RelationCollection();
      string strRelationId = relations.GenerateRelationId();
      string strUrl = ConvertAddressString( book.URL );
      bool bExist = true;

      bool bNotLink = !( strUrl.StartsWith( FileHyperlinkStartString ) || strUrl.StartsWith( HttpStartString ) ) &&
        strUrl[ 0 ] != '/';

      if( bNotLink )
      {
        if( !( strUrl.Contains( ":\\" ) || strUrl.StartsWith( "\\" ) ) )
        {
#if MEASURE_PERFORMANCE
          DateTime methodStart = DateTime.Now;
#endif
#if !(WINRT )
            //TODO:WINRT Implement properly.
          bExist = File.Exists( strUrl );
#endif
#if MEASURE_PERFORMANCE
          DateTime methodEnd = DateTime.Now;
          m_externalLinksTotal += methodEnd - methodStart;
#endif
        }

        if( bExist )
        {
          strUrl = FileHyperlinkStartString + strUrl;
        }
      }

      string strRelationType = bExist ? RelationTypes.ExternLinkPath : RelationTypes.MissingPath;
      relations[ strRelationId ] = new Relation( strUrl, strRelationType, true );

      writer.WriteStartElement( ExternalLinks.ExternalLinkTag, XmlNamespaceMain );
      writer.WriteStartElement( ExternalLinks.ExternalBookTag );
      writer.WriteAttributeString( RelationAttribute, RelationNamespace, strRelationId );
      SerializeSheetNames( writer, book );
      SerializeExternNames( writer, book );
      SerializeSheetDataSet( writer, book );

      //if( iSheetCount > 0 )
      //{
      //  writer.WriteStartElement( ExternalLinks.SheetDataSetTag );

      //  for( int i = 0, len = iSheetCount; i < len; i++ )
      //  {
      //    //ExternWorksheetImpl sheet = ( ExternWorksheetImpl )lstSheets.GetByIndex( i );
      //    writer.WriteStartElement( SheetDataTagName );
      //    writer.WriteAttributeString( ExternalLinks.SheetIdAttribute, i.ToString() );
      //    writer.WriteEndElement();
      //  }

      //  writer.WriteEndElement();
      //}

      // TODO: implement and uncomment if necessary.
      //SerializeExternNames( writer, book );

      writer.WriteEndElement();
      writer.WriteEndElement();

      return relations;
    }
    /// <summary>
    /// Serializes external workbook into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="book">External workbook to serialize.</param>
    /// <returns>Collection with external link item relations.</returns>
    public RelationCollection SerializeOleObjectLink( XmlWriter writer, ExternWorkbookImpl book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      RelationCollection relations = new RelationCollection();
      string strRelationId = relations.GenerateRelationId();
      string strUrl = ConvertAddressString( book.URL );
      bool bExist = true;

      bool bNotLink = !( strUrl.StartsWith( FileHyperlinkStartString ) || strUrl.StartsWith( HttpStartString ) ) &&
        strUrl[ 0 ] != '/';

      if( bNotLink )
      {
        if( !( strUrl.Contains( ":\\" ) || strUrl.StartsWith( "\\" ) ) )
        {
#if MEASURE_PERFORMANCE
          DateTime methodStart = DateTime.Now;
#endif
#if !(WINRT )
          bExist = File.Exists( strUrl );
#endif
#if MEASURE_PERFORMANCE
          DateTime methodEnd = DateTime.Now;
          m_externalLinksTotal += methodEnd - methodStart;
#endif
        }

        if( bExist )
        {
          strUrl = FileHyperlinkStartString + strUrl;
        }
      }

      string strRelationType = bExist ? RelationTypes.OleObject : RelationTypes.MissingPath;
      relations[ strRelationId ] = new Relation( strUrl, strRelationType, true );
      writer.WriteStartDocument( true );
      writer.WriteStartElement( ExternalLinks.ExternalLinkTag, XmlNamespaceMain );
      writer.WriteStartElement( ExternalLinks.OleLink );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, RelationPrefix,
       null, RelationNamespace );
      writer.WriteAttributeString( RelationAttribute, RelationNamespace, strRelationId );
      writer.WriteAttributeString( Vml.ProgramID, book.ProgramId );
      writer.WriteStartElement( ExternalLinks.OleItems );
      writer.WriteStartElement( ExternalLinks.OleItem );
      writer.WriteAttributeString( ExternalLinks.NameAttribute, "'" );

      //if( oleObject.DisplayAsIcon == true )
      //  writer.WriteAttributeString( ExternalLinks.IconAttribute, "1" );

      writer.WriteAttributeString( ExternalLinks.AdviseAttribute, "1" );
      writer.WriteAttributeString( ExternalLinks.PreferPictureAttribute, "1" );
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();

      return relations;
    }
    /// <summary>
    /// Serializes external worksheets data set.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="book">External workbook to serialize.</param>
    private void SerializeSheetDataSet( XmlWriter writer, ExternWorkbookImpl book )
    {
      int iSheetCount = book.Worksheets.Count;

      if( iSheetCount > 0 )
      {
        writer.WriteStartElement( ExternalLinks.SheetDataSetTag );

        Dictionary<string, string> additionalAttributes;

        for( int i = 0; i < iSheetCount; i++ )
        {
          //ExternWorksheetImpl sheet = ( ExternWorksheetImpl )lstSheets.GetByIndex( i );

          ExternWorksheetImpl sheet = book.Worksheets.Values[ i ];
          additionalAttributes = sheet.AdditionalAttributes;

          if( additionalAttributes == null )
            additionalAttributes = new Dictionary<string, string>();

          additionalAttributes[ ExternalLinks.SheetIdAttribute ] = sheet.Index.ToString();
          SerializeSheetData( writer, sheet.CellRecords, null, ExternalLinks.CellTag, additionalAttributes, false );
        }

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes external sheet names.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="book">External workbook to serialize.</param>
    private void SerializeSheetNames( XmlWriter writer, ExternWorkbookImpl book )
    {
      int iSheetCount = book.SheetNumber;

      if( iSheetCount > 0 )
      {
        writer.WriteStartElement( ExternalLinks.SheetNamesTag );

        for( int i = 0; i < iSheetCount; i++ )
        {
          //ExternWorksheetImpl sheet = ( ExternWorksheetImpl )lstSheets.GetByIndex( i );
          string strSheetName = book.GetSheetName( i );
          writer.WriteStartElement( ExternalLinks.SheetNameTag );
          writer.WriteAttributeString( ExternalLinks.ExternalSheetNameAttribute, strSheetName );
          writer.WriteEndElement();
        }

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes all external names from the external workbook.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize named ranges into.</param>
    /// <param name="book">External workbook to get named ranges from.</param>
    private void SerializeExternNames( XmlWriter writer, ExternWorkbookImpl book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      ExternNamesCollection externNames = book.ExternNames;
      int iCount = externNames.Count;

      if( iCount > 0 )
      {
        writer.WriteStartElement( ExternalLinks.DefinedNamesTag );

        for( int i = 0; i < iCount; i++ )
        {
          SerializeExternName( writer, externNames[ i ] );
        }

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes single external name.
    /// </summary>
    /// <param name="writer">Writer to serialize name into.</param>
    /// <param name="externName">Name to serialize.</param>
    private void SerializeExternName( XmlWriter writer, ExternNameImpl externName )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( externName == null )
        throw new ArgumentNullException( "externName" );

      ExternNameRecord name = externName.Record;

      writer.WriteStartElement( ExternalLinks.DefinedNameTag );
      string ExternNameValue = string.Empty;
      if (externName.Name.Contains("\0"))
          ExternNameValue = externName.Name.Replace("\0", string.Empty);
      else
          ExternNameValue = externName.Name;
      writer.WriteAttributeString(ExternalLinks.NameAttribute, ExternNameValue);
      if ( externName.RefersTo != null )
          writer.WriteAttributeString(ExternalLinks.RefersToAttribute, externName.RefersTo); 

      int iSheetId = externName.sheetId ;

     // if( iSheetId != 0 )
        writer.WriteAttributeString( ExternalLinks.SheetIdAttribute, iSheetId.ToString() );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes drawings (images and other shapes).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    private void SerializeDrawingsWorksheetPart( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( sheet.Shapes.Count - sheet.VmlShapesCount - sheet.AutoFilters.Count > 0 || HasAlternateContent( sheet.Shapes ) )
      {
        WorksheetDataHolder holder = sheet.DataHolder;
        string strDrawingsRelation = holder.DrawingsId;

        if( strDrawingsRelation == null )
        {
          holder.DrawingsId = strDrawingsRelation = holder.Relations.GenerateRelationId();
          holder.Relations[ strDrawingsRelation ] = null;
        }

        writer.WriteStartElement( Drawings.DrawingTagName );
        writer.WriteAttributeString( RelationAttribute, RelationNamespace, strDrawingsRelation );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes part of the worksheet that contains information about
    /// vml shapes (legacydrawing tag).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    public void SerializeVmlShapesWorksheetPart( XmlWriter writer, WorksheetBaseImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( !sheet.HasVmlShapes )
        return;

      WorksheetDataHolder holder = sheet.DataHolder;
      string strVmlDrawingRelation = holder.VmlDrawingsId;

      if( strVmlDrawingRelation == null )
      {
        holder.VmlDrawingsId = strVmlDrawingRelation = holder.Relations.GenerateRelationId();
        holder.Relations[ strVmlDrawingRelation ] = null;
      }

      writer.WriteStartElement( Vml.LegacyDrawing );
      writer.WriteAttributeString( RelationAttribute, RelationNamespace, strVmlDrawingRelation );
      writer.WriteEndElement();
    }
      # if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Serializes the OLE.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sheet">The sheet.</param>
    private void SerializeOle( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( ( sheet.HasOleObject == false ) || ( sheet.OleObjects.Count == 0 ) )
      {
        return;
      }

      string strRelationId = null;
      WorksheetDataHolder holder = sheet.DataHolder;
      writer.WriteStartElement( Vml.OleObjects );

      OleObjects oleObjects = ( OleObjects )sheet.OleObjects;

      for( int j = 0, len = oleObjects.Count; j < len; j++ )
      {
        OleObject oleObject = ( OleObject )oleObjects[ j ];

        if( oleObject.OleType == OleLinkType.Embed )
        {
          strRelationId = oleObject.ShapeRId;
          
          if( strRelationId == null )
          {
            strRelationId = holder.Relations.GenerateRelationId();
            oleObject.ShapeRId = strRelationId;
          }

          holder.Relations[ strRelationId ] = null;
        }

        //oleObject.ShapeID = GetNextShapeId( sheet.Index ).ToString(); ;
        writer.WriteStartElement( Vml.OleObject );
        writer.WriteAttributeString( Vml.ProgramID,OleTypeConvertor.ToOleString(oleObject.OleObjectType) );

        if( oleObject.DvAspect == DVAspect.DVASPECT_ICON )
          writer.WriteAttributeString( Vml.DevAspect, oleObject.DvAspect.ToString() );

        if( oleObject.OleType == OleLinkType.Link )
        {
          int index = oleObject.GetWorkbookIndex() + 1;
          string link = String.Format( Vml.LinkAttributeValue, index );

          writer.WriteAttributeString( Vml.LinkAttribute, link );

          string updateValue = ( oleObject.DvAspect == DVAspect.DVASPECT_ICON ) ?
            OnCall :
            Always;

          writer.WriteAttributeString( Vml.OleUpdateAttribute, updateValue );
        }

        int shapeId = ( oleObject.Shape as ShapeImpl ) !=null ?( oleObject.Shape as ShapeImpl ).ShapeId : oleObject.ShapeID;
        writer.WriteAttributeString( Vml.ShapeID, shapeId.ToString() );

        if( oleObject.OleType == OleLinkType.Embed )
        {
          writer.WriteAttributeString( RelationAttribute, RelationNamespace, strRelationId );
        }

        writer.WriteEndElement();
      }

      writer.WriteEndElement();

    }
#endif
    /// <summary>
    /// Serializes part of the worksheet that contains information about
    /// vml shapes (legacydrawing tag).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    public static void SerializeVmlHFShapesWorksheetPart( XmlWriter writer, WorksheetBaseImpl sheet,
      IPageSetupConstantsProvider constants, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      HeaderFooterShapeCollection shapes = sheet.InnerHeaderFooterShapes;
      if( shapes == null || shapes.Count == 0 )
        return;

      WorksheetDataHolder holder = sheet.DataHolder;
      string strRelation = holder.VmlHFDrawingsId;

      if( strRelation == null )
      {
        if( relations == null )
        {
          relations = holder.Relations;
        }

        holder.VmlHFDrawingsId = strRelation = relations.GenerateRelationId();
        relations[ strRelation ] = null;
      }

      writer.WriteStartElement( Vml.LegacyDrawingHF);
      writer.WriteAttributeString(Excel2007Serializator.RelationshipIdAttributeName,
      Excel2007Serializator.RelationNamespace, strRelation);      
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes comments list (following comment settings: note, cell address, author id).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to get comments from.</param>
    /// <param name="dicAuthors">Dictionary with all used authors: key - author name, value - author id.</param>
    private void SerializeCommentsList( XmlWriter writer, WorksheetImpl sheet,
      IDictionary<string, int> dicAuthors )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( dicAuthors == null )
        throw new ArgumentNullException( "dicAuthors" );

      CommentsCollection arrComments = sheet.InnerComments;
      int iCommentsCount = arrComments.Count;

      if( iCommentsCount > 0 )
      {
        writer.WriteStartElement( CommentListTagName );

        for( int i = 0, len = arrComments.Count; i < len; i++ )
        {
          ICommentShape comment = arrComments[ i ];
          SerializeComment( writer, comment, dicAuthors );
        }

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serialize single comment settings into specified writer.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="comment">Comment to serialize.</param>
    /// <param name="dicAuthors">Dictionary containing author name.</param>
    private void SerializeComment( XmlWriter writer, ICommentShape comment,
      IDictionary<string, int> dicAuthors )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( comment == null )
        throw new ArgumentNullException( "comment" );

      if( dicAuthors == null )
        throw new ArgumentNullException( "dicAuthors" );

      string strCellName = RangeImpl.GetCellName( comment.Column, comment.Row );
      int iAuthorId = dicAuthors[ comment.Author ];
      CommentShapeImpl commentShape = ( CommentShapeImpl )comment;

      writer.WriteStartElement( CommentTagName );

      writer.WriteAttributeString( RefAttributeName, strCellName );
      writer.WriteAttributeString( AuthorIdAttributeName, iAuthorId.ToString() );
      writer.WriteStartElement( CommentTextTagName );
      SerializeRichTextRun( writer, commentShape.InnerRichText.TextObject );
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes list of authors for comments collection.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to get comments from.</param>
    /// <returns>Dictionary with authors: key - author name, value - author id.</returns>
    private IDictionary<string, int> SerializeAuthors( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      CommentsCollection arrComments = sheet.InnerComments;
      int iAuthorsCount = 0;
      IDictionary<string, int> dicAuthors = null;
      int iCommentsCount = arrComments.Count;

      if( iCommentsCount > 0 )
      {
        dicAuthors = new Dictionary<string, int>();
        writer.WriteStartElement( CommentAuthorsTagName );

        for( int i = 0; i < iCommentsCount; i++ )
        {
          ICommentShape comment = arrComments[ i ];
          string strAuthor = comment.Author;

          if( !dicAuthors.ContainsKey( strAuthor ) )
          {
            writer.WriteElementString( CommentAuthorTagName, strAuthor );
            dicAuthors.Add( strAuthor, iAuthorsCount );
            iAuthorsCount++;
          }
        }

        writer.WriteEndElement();
      }

      return dicAuthors;
    }
    /// <summary>
    /// Serializes worksheet's dimension.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize dimension of.</param>
    private void SerializeDimensions( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet.FirstRow > 0 && sheet.FirstColumn > 0 && sheet.LastColumn <= sheet.Workbook.MaxColumnCount )
      {
        writer.WriteStartElement( DimensionTagName );
        writer.WriteAttributeString( RefAttributeName, sheet.UsedRange.AddressLocal );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes worksheet's view properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    private void SerializeSheetViews( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      writer.WriteStartElement( SheetViewsTag );
      writer.WriteStartElement( SheetViewTag );

      const string WorkbookViewId = "workbookViewId";

      IWorkbook book = sheet.Workbook;
      SerializeAttribute( writer, WindowProtection, book.IsWindowProtection, false );

      IRange topLeftCell = sheet.TopLeftCell;

      if( !sheet.IsFreezePanes && topLeftCell != null &&
        ( topLeftCell.Row != 1 || topLeftCell.Column != 1 ) )
      {
        writer.WriteAttributeString( Pane.TopLeftCell, topLeftCell.AddressLocal );
      }

      SerializeAttribute( writer, ShowGridLines, sheet.IsGridLinesVisible, true );
      SerializeAttribute( writer, ShowRowColHeaders, sheet.IsRowColumnHeadersVisible, true);
      SerializeAttribute( writer, ShowZeros, sheet.IsDisplayZeros, true );
      SerializeAttribute( writer, SheetZoomScale, sheet.Zoom, 100 );
      SerializeAttribute( writer, RightToLeft, sheet.IsRightToLeft, false );

      if( !sheet.WindowTwo.IsDefaultHeader )
      {
        writer.WriteAttributeString( SheetGridColor, FalseValue );
        writer.WriteAttributeString( ColorID, ( ( int )sheet.GridLineColor ).ToString() );
      }

      if (sheet.WindowTwo.IsSavedInPageBreakPreview)
      {
          writer.WriteAttributeString(ViewTag, PageBreakPreview);
      }
      else
      {
          string viewType = Normal;
          if (sheet.View == SheetView.PageLayout)
              viewType = Layout;
          writer.WriteAttributeString(ViewTag, viewType);
      }

      //writer.WriteAttributeString( SelectedTabIndex, ( sheet.Workbook.ActiveSheetIndex + 1 ).ToString() );
      writer.WriteAttributeString( WorkbookViewId, "0" );

      SerializePane( writer, sheet );
      SerializeSelection( writer, sheet );

      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes active cell.
    /// </summary>
    /// <param name="writer">Writer to write selection information into.</param>
    /// <param name="sheet">Worksheet to get active cell from.</param>
    private void SerializeSelection( XmlWriter writer, WorksheetImpl sheet )
    {
      IRange activeRange = sheet.GetActiveCell();

      if( activeRange != null )
      {
        string address = activeRange.AddressLocal;
        writer.WriteStartElement( Pane.Selection );

        if (sheet.Pane != null)               
          writer.WriteAttributeString( Pane.TagName, ( ( Pane.ActivePane )GetActivePane(sheet.Pane) ).ToString() );

        writer.WriteAttributeString( Pane.ActiveCell, address );
        writer.WriteAttributeString( Pane.Sqref, address );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes pane record.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize pane for.</param>
    private void SerializePane( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( sheet.IsFreezePanes || sheet.VerticalSplit != 0 || sheet.HorizontalSplit != 0 )
      {
        PaneRecord pane = sheet.Pane;

        if( pane != null && ( pane.VerticalSplit > 0 || pane.HorizontalSplit > 0 ) )
        {
          writer.WriteStartElement( Pane.TagName );

          SerializeAttribute( writer, Pane.XSplit, pane.VerticalSplit, 0 );
          SerializeAttribute( writer, Pane.YSplit, pane.HorizontalSplit, 0 );

          string cellName = RangeImpl.GetCellName( pane.FirstColumn + 1, pane.FirstRow + 1 );
          writer.WriteAttributeString( Pane.TopLeftCell, cellName );

          string strActivePane = ( ( Pane.ActivePane )pane.ActivePane ).ToString();
          writer.WriteAttributeString( Pane.Active, strActivePane );

          WindowTwoRecord windowTwo = sheet.WindowTwo;
          string strState;

          if( windowTwo.IsFreezePanes && !windowTwo.IsFreezePanesNoSplit )
          {
            strState = Pane.StateFrozenSplit;
            writer.WriteAttributeString( Pane.State, strState );
          }
          else if( windowTwo.IsFreezePanes && windowTwo.IsFreezePanesNoSplit )
          {
            strState = Pane.StateFrozen;
            writer.WriteAttributeString( Pane.State, strState );
          }
          else
          {
            strState = Pane.StateSplit;
          }

          writer.WriteEndElement();
        }
      }
    }
    /// <summary>
    /// Serializes stream.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="data">Data to be serialized.</param>
    private void SerializeStream( XmlWriter writer, Stream data )
    {
      SerializeStream( writer, data, TemporaryRoot );
    }
    /// <summary>
    /// Serializes stream data.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="data">Data to be serialized</param>
    /// <param name="strRootName">Root name of the data. </param>
    public static void SerializeStream( XmlWriter writer, Stream data, string strRootName )
    {
      if( data != null && data.Length > 0 )
      {
        data.Position = 0;
        XmlReader reader = UtilityMethods.CreateReader( data );
        //reader.Read();

        //if( reader.NodeType == XmlNodeType.XmlDeclaration )
        //  reader.Read();

        while( reader.Name == strRootName || reader.Name == TemporaryRoot )
          reader.Read();

        while( !reader.EOF && ( ( reader.Name != strRootName && reader.Name != TemporaryRoot )
          || reader.NodeType != XmlNodeType.EndElement ) )
        {
          writer.WriteNode( reader, false );
        }
      }
    }
    /// <summary>
    /// Serializes single relation object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="key">Relation key.</param>
    /// <param name="relation">Relation object.</param>
    private void SerializeRelation( XmlWriter writer, string key, Relation relation )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( key == null )
        throw new ArgumentNullException( "key" );

      if( relation == null )
        throw new ArgumentNullException( "relation" );

      writer.WriteStartElement( RelationTagName );
      writer.WriteAttributeString( RelationIdAttribute, key );
      writer.WriteAttributeString( RelationTypeAttribute, relation.Type );
      writer.WriteAttributeString( RelationTargetAttribute, relation.Target );

      if( relation.IsExternal )
        writer.WriteAttributeString( RelationTargetModeAttribute, RelationExternalTargetMode );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes sheets collection into workbook part.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    private void SerializeSheets( XmlWriter writer )
    {
#if MEASURE_PERFORMANCE
      DateTime methodStart = DateTime.Now;
#endif

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( SheetsTagName );

      ITabSheets arrSheets = m_book.TabSheets;

      for( int i = 0, len = arrSheets.Count; i < len; i++ )
      {
        if( ( ( WorksheetBaseImpl )arrSheets[ i ] ).m_dataHolder != null )
          SerializeSheetTag( writer, arrSheets[ i ] );
      }

      writer.WriteEndElement();
#if MEASURE_PERFORMANCE
      DateTime methodEnd = DateTime.Now;
      Console.CursorLeft += 2;
      Console.WriteLine( "SerializeSheets() took: {0}", methodEnd - methodStart );
#endif
    }
    /// <summary>
    /// Serializes single sheet entry inside of sheets tag from workbook part.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">TabSheet to serialize.</param>
    private void SerializeSheetTag( XmlWriter writer, ITabSheet sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      WorksheetDataHolder dataHolder = ( ( WorksheetBaseImpl )sheet ).m_dataHolder;
      string strSheetId = ( dataHolder != null ) ? dataHolder.SheetId : null;

      if( strSheetId == null )
      {
        strSheetId = GenerateSheetId();

        if( dataHolder != null )
          dataHolder.SheetId = strSheetId;
      }

      writer.WriteStartElement( SheetTagName );

      writer.WriteAttributeString( SheetNameAttribute, sheet.Name );
      writer.WriteAttributeString( SheetIdAttribute, strSheetId );
      writer.WriteAttributeString( RelationAttribute, RelationNamespace, dataHolder.RelationId );

      Worksheet2007Visibility visibility = ( Worksheet2007Visibility )sheet.Visibility;

      if( visibility != Worksheet2007Visibility.Visible )
      {
        string strState = visibility.ToString();
        strState = LowerFirstLetter( strState );
        writer.WriteAttributeString( SheetStateAttributeName, strState );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Generates free sheetId.
    /// </summary>
    /// <returns>Extracted sheet id.</returns>
    private string GenerateSheetId()
    {
      //1. We have to iterate through all items and find maximum sheet id number
      WorkbookObjectsCollection arrObjects = m_book.Objects;
      int iMaximum = 0;

      for( int i = 0, len = arrObjects.Count; i < len; i++ )
      {
        WorksheetBaseImpl sheet = ( WorksheetBaseImpl )arrObjects[ i ];
        WorksheetDataHolder dataHolder = sheet.DataHolder;

        if( dataHolder != null )
        {
          string strSheetId = dataHolder.SheetId;
          int iSheetId;

          if( strSheetId != null && int.TryParse( strSheetId, out iSheetId ) )
          {
            if( iSheetId > iMaximum )
              iMaximum = iSheetId;
          }
        }
      }

      return ( iMaximum + 1 ).ToString();
    }
    /// <summary>
    /// Converts MergeRegion into string representing range.
    /// </summary>
    /// <param name="region">Region to convert.</param>
    /// <returns>Converted value.</returns>
    private string GetRangeName( MergeRegion region )
    {
      if( region == null )
        throw new ArgumentNullException( "region" );

      string strCellFrom = RangeImpl.GetCellName( region.ColumnFrom + 1, region.RowFrom + 1 );
      string strCellTo = RangeImpl.GetCellName( region.ColumnTo + 1, region.RowTo + 1 );

      return strCellFrom + ":" + strCellTo;
    }
    /// <summary>
    /// Serializes named range into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize named range into.</param>
    /// <param name="name">Named range to serialize.</param>
    private void SerializeNamedRange( XmlWriter writer, IName name )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( name == null )
        throw new ArgumentNullException( "name" );

      string strNameValue = ( ( NameImpl )name ).GetValue( m_formulaUtil );

      //if( strNameValue == null )
      //  strNameValue = "#NAME?";

      if( !string.IsNullOrEmpty( strNameValue ) )//!= null )
      {
        writer.WriteStartElement( DefinedNameXmlTagName );
        writer.WriteAttributeString(NameAttributeName, m_book.RemoveInvalidXmlCharacters(name.Name));

        if( name.IsLocal )
        {
          NameImpl typedName = ( NameImpl )name;
          WorksheetImpl sheet = typedName.Worksheet;
          //string strSheetId = sheet.m_dataHolder.SheetId;

          //if( strSheetId == null )
          //{
          //  // TODO: fix this.
          //  strSheetId = typedName.Record.IndexOrGlobal.ToString();
          //}
          string strSheetId =(GetLocalSheetIndex(sheet)).ToString();
          writer.WriteAttributeString( NameSheetIdAttribute, strSheetId );
        }
        else if ((name as NameImpl) != null && (name as NameImpl).IsQueryTableRange)
        {
            writer.WriteAttributeString(NameSheetIdAttribute, (name as NameImpl).SheetIndex.ToString());
        }
        SerializeAttribute( writer, HiddenAttributeName, !name.Visible, false );

        //string strNameValue = ( ( NameImpl )name ).GetValue( m_formulaUtil );

        if( strNameValue == null )
          strNameValue = "#NAME?";

        if(!m_book.HasApostrophe && ! CheckSheetName(strNameValue))
           strNameValue = strNameValue.Replace("'", "");

        if (strNameValue.StartsWith("#REF"))
            strNameValue = "#REF!";
        if ((name as NameImpl).IsCommon)
        {
            strNameValue = strNameValue.Substring(strNameValue.IndexOf("!"));
            writer.WriteString(strNameValue);
        }
        else
        {
            writer.WriteString(strNameValue);
        }
        
            

        writer.WriteEndElement();
      }
    }

    /// <summary>
    /// Gets the index of the local sheet.
    /// </summary>
    /// <param name="sheet">The sheet.</param>
    /// <returns></returns>
    private int GetLocalSheetIndex(WorksheetImpl sheet)
    {
        int localSheetIndex = -1;
        ITabSheets sheets = m_book.TabSheets;
        for (int index = 0; index < sheets.Count; index++)
        {
            if (sheets[index].Name == sheet.Name)
            {
                localSheetIndex = index;
                break;
            }
        }
        if (localSheetIndex == -1)
            throw new ArgumentException("Invalid Sheet");

        return localSheetIndex;
    }
    /// <summary>
    /// Checks the name of the sheet.
    /// </summary>
    /// <param name="strNameValue">The STR name value.</param>
    /// <returns></returns>
    private bool CheckSheetName(string strNameValue)
    {
        char[] nameArray = strNameValue.ToCharArray();
        for (int index = 0, len = nameArray.Length; index < len; index++)
        {
            if(!Char.IsLetterOrDigit(nameArray[index]))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Serializes fonts collection into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize fonts into.</param>
    private void SerializeFonts( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      FontsCollection arrFonts = m_book.InnerFonts;
      int iCount = arrFonts.Count;

      writer.WriteStartElement( FontsTagName );
      writer.WriteAttributeString( CountAttributeName, iCount.ToString() );

      for( int i = 0; i < iCount; i++ )
      {
        IFont font = arrFonts[ i ];
        SerializeFont( writer, font, FontTagName );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize single font object into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize fonts into.</param>
    /// <param name="font">Font to serialize.</param>
    /// <param name="strElement">Element tag name to serialize font for.</param>
    private void SerializeFont( XmlWriter writer, IFont font, string strElement )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( font == null )
        throw new ArgumentNullException( "font" );

      //writer.WriteStartElement( FontTagName );
      writer.WriteStartElement( strElement );

      if (font.Bold)
          writer.WriteElementString(FontBoldTagName, string.Empty);

      if (font.VerticalAlignment != ExcelFontVertialAlignment.Baseline)
      {
          writer.WriteStartElement(FontVerticalAlignmentTagName);
          writer.WriteAttributeString(ValueAttributeName, font.VerticalAlignment.ToString().
            ToLower(CultureInfo.InvariantCulture));
          writer.WriteEndElement();
      }

      if( font.Italic )
        writer.WriteElementString( FontItalicTagName, string.Empty );

      ExcelUnderline underline = font.Underline;

      if( underline != ExcelUnderline.None )
      {
        writer.WriteStartElement( FontUnderlineTagName );

        string strUnderline = underline.ToString();
        strUnderline = Char.ToLower( strUnderline[ 0 ] ) + UtilityMethods.RemoveFirstCharUnsafe( strUnderline );
        writer.WriteAttributeString( ValueAttributeName, strUnderline );
        writer.WriteEndElement();
      }

      if( font.Strikethrough )
        writer.WriteElementString( FontStrikeTagName, string.Empty );

      writer.WriteStartElement( FontSizeTagName );
      writer.WriteAttributeString( ValueAttributeName, XmlConvert.ToString( font.Size ) );
      writer.WriteEndElement();

      if ((int)font.Color != FontRecord.DefaultFontColor)
          SerializeFontColor(writer, ColorTagName, (font as IInternalFont).Font.ColorObject);

      string strFontTag = FontNameTagName;

      if( strElement == RichTextRunPropertiesTagName )
        strFontTag = RichTextRunFontTagName;

      writer.WriteStartElement( strFontTag );
      writer.WriteAttributeString( ValueAttributeName, font.FontName );
      writer.WriteEndElement();

      int iCharSet = ( ( FontImpl )font ).CharSet;

      if( iCharSet != 1 )
      {
        writer.WriteStartElement( FontCharsetTagName );
        writer.WriteAttributeString( ValueAttributeName, iCharSet.ToString() );
        writer.WriteEndElement();
      }

      

      if( font.MacOSShadow )
        writer.WriteElementString( MacOSShadowTagName, string.Empty );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes font color object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Name of the tag to use.</param>
    /// <param name="color">Color object to serialize.</param>
    private void SerializeFontColor( XmlWriter writer, string tagName, ColorObject color )
    {
      writer.WriteStartElement( tagName );

      switch( color.ColorType )
      {
        case ColorType.Indexed:
          writer.WriteAttributeString( ColorIndexedAttributeName, color.Value.ToString() );
          break;

        case ColorType.RGB:
          writer.WriteAttributeString( ColorRgbAttribute, color.Value.ToString( "X8" ) );
          break;

        case ColorType.Theme:
          writer.WriteAttributeString( ColorThemeAttributeName, color.Value.ToString() );
          break;
      }

      SerializeAttribute( writer, ColorTintAttributeName, color.Tint, 0 );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes number formats collection into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    private void SerializeNumberFormats( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      List<FormatRecord> arrFormats = m_book.InnerFormats.GetUsedFormats( ExcelVersion.Excel2007 );

      int iCount = arrFormats.Count;

      if( iCount == 0 )
        return;

      writer.WriteStartElement( NumberFormatsTagName );
      writer.WriteAttributeString( CountAttributeName, iCount.ToString() );

      for( int i = 0; i < iCount; i++ )
      {
        SerializeNumberFormat( writer, arrFormats[ i ] );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize single instance of number format into XmlWriter.
    /// </summary>
    /// <param name="writer">Writer to save number format into.</param>
    /// <param name="format">Number format to serialize.</param>
    private void SerializeNumberFormat( XmlWriter writer, FormatRecord format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      writer.WriteStartElement( NumberFormatTagName );
      writer.WriteAttributeString( NumberFormatIdAttributeName, format.Index.ToString() );
      string formatString = format.FormatString;
        if(format.FormatString.Equals(ChartConstants.StandardFormatAttribute))
            formatString = RangeImpl.DEF_GENERAL_FORMAT;
      writer.WriteAttributeString( NumberFormatStringAttributeName, formatString );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes fills into specified writer.
    /// </summary>
    /// <param name="writer">XmlWriter to save fills into.</param>
    /// <returns>Array with fill indexes, index in array means extended format index, value means fill index.</returns>
    private int[] SerializeFills( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      Dictionary<FillImpl, int> hashFills = new Dictionary<FillImpl, int>();
      ExtendedFormatsCollection arrExtFormats = m_book.InnerExtFormats;

      int iCount = arrExtFormats.Count;
      int[] arrFillIndexes = new int[ iCount ];
      FillImpl[] arrFills = new FillImpl[ iCount ];
      int iFillIndex = -1;

      for( int i = 0; i < iCount; i++ )
      {
        FillImpl fill;

        //MS Excel 2007 serializes "None" pattern type on the first position,
        //so we should do the same operation here.
        if (iFillIndex == -1)
        {
            fill = new FillImpl();
            fill.Pattern = ExcelPattern.None;
            fill.PatternColorObject.SetIndexed((ExcelKnownColors)ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX);
            fill.ColorObject.SetIndexed((ExcelKnownColors)ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX);
        }
        //MS Excel 2007 serializes "gray125" pattern type on the second position,
        //so we should do the same operation here.  
        else if (iFillIndex == 0)
        {
          fill = new FillImpl();
          fill.Pattern = ExcelPattern.Percent125Gray;
          fill.PatternColorObject.SetIndexed( ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX );
          fill.ColorObject.SetIndexed((ExcelKnownColors)ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX);
        }
        else
        {
          fill = new FillImpl( arrExtFormats[ i ] );
        }

        if( hashFills.ContainsKey( fill ) )
        {
          arrFillIndexes[ i ] = hashFills[ fill ];
        }
        else
        {
          iFillIndex = hashFills.Count;
          hashFills.Add( fill, iFillIndex );
          if (iFillIndex >= arrFills.Length)
              Array.Resize<FillImpl>(ref arrFills, iFillIndex + 1);
          arrFills[ iFillIndex ] = fill;
          arrFillIndexes[ i ] = iFillIndex;

          if ((iFillIndex == 0) || (iFillIndex == 1))
            i--;
        }
      }

      writer.WriteStartElement( FillsTagName );
      writer.WriteAttributeString( CountAttributeName, ( iFillIndex + 1 ).ToString() );

      for( int i = 0; i <= iFillIndex; i++ )
      {
        SerializeFill( writer, arrFills[ i ] );
      }

      writer.WriteEndElement();
      return arrFillIndexes;
    }
    /// <summary>
    /// Serializes single fill object into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="fill">Object to serialize.</param>
    internal void SerializeFill( XmlWriter writer, FillImpl fill )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( fill == null )
        throw new ArgumentNullException( "fill" );

      writer.WriteStartElement( FillTagName );

      if( fill.Pattern == ExcelPattern.Gradient )
      {
        SerializeGradientFill( writer, fill );
      }
      else
      {
        SerializePatternFill( writer, fill );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single pattern fill object into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="fill">Object to serialize.</param>
    private void SerializePatternFill( XmlWriter writer, FillImpl fill )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( fill == null )
        throw new ArgumentNullException( "fill" );

      writer.WriteStartElement( PatternFillTagName );

      writer.WriteAttributeString( PatternAttributeName, ConvertPatternToString( fill.Pattern ) );

      if( fill.Pattern == ExcelPattern.Solid )
      {
        SerializeColorObject( writer, ForegroundColorTagName, fill.ColorObject );
        SerializeColorObject( writer, BackgroundColorTagName, fill.PatternColorObject );
      }
      else
      {
        ColorObject color = fill.PatternColorObject;

        if( color.ColorType != ColorType.Indexed ||
          ( int )color.GetIndexed( m_book ) != ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX )
        {
          SerializeColorObject( writer, ForegroundColorTagName, color );
        }

        color = fill.ColorObject;

        if( color.ColorType != ColorType.Indexed ||
          ( int )color.GetIndexed( m_book ) != ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX )
        {
          SerializeColorObject( writer, BackgroundColorTagName, color );
        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single gradient fill object into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="fill">Object to serialize.</param>
    private void SerializeGradientFill( XmlWriter writer, FillImpl fill )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( fill == null )
        throw new ArgumentNullException( "fill" );

      writer.WriteStartElement( GradientFillTagName );
      ExcelGradientStyle gradientStyle = fill.GradientStyle;

      if( gradientStyle == ExcelGradientStyle.From_Center || gradientStyle == ExcelGradientStyle.From_Corner )
      {
        SerializeFromCenterCornerGradientFill( writer, fill );
      }
      else
      {
        SerializeDegreeGradientFill( writer, fill );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single degree gradient fill object into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="fill">Object to serialize.</param>
    private void SerializeDegreeGradientFill( XmlWriter writer, FillImpl fill )
    {
      ExcelGradientStyle gradientStyle = fill.GradientStyle;
      ExcelGradientVariants gradientVariant = fill.GradientVariant;

      double dGradientDegree = 0;

      if( gradientVariant == ExcelGradientVariants.ShadingVariants_3 )
      {
        switch( gradientStyle )
        {
          case ExcelGradientStyle.Horizontal:
            dGradientDegree = 90;
            break;

          case ExcelGradientStyle.Vertical:
            break;

          case ExcelGradientStyle.Diagonl_Up:
            dGradientDegree = 45;
            break;

          case ExcelGradientStyle.Diagonl_Down:
            dGradientDegree = 135;
            break;

          default:
            throw new ArgumentException( "Unknown gradient style" );
        }

        SerializeAttribute( writer, LinearGradientDegreeAttributeName, dGradientDegree, 0 );

        SerializeStopColorElements( writer, 0, fill.ColorObject );
        SerializeStopColorElements( writer, 0.5, fill.PatternColorObject );
        SerializeStopColorElements( writer, 1, fill.ColorObject );
      }
      else
      {
        switch( gradientStyle )
        {
          case ExcelGradientStyle.Horizontal:
            dGradientDegree = ( gradientVariant == ExcelGradientVariants.ShadingVariants_1 ) ? 90 : 270;
            break;

          case ExcelGradientStyle.Vertical:
            dGradientDegree = ( gradientVariant == ExcelGradientVariants.ShadingVariants_1 ) ? 0 : 180;
            break;

          case ExcelGradientStyle.Diagonl_Up:
            dGradientDegree = ( gradientVariant == ExcelGradientVariants.ShadingVariants_1 ) ? 45 : 225;
            break;

          case ExcelGradientStyle.Diagonl_Down:
            dGradientDegree = ( gradientVariant == ExcelGradientVariants.ShadingVariants_1 ) ? 135 : 315;
            break;

          default:
            throw new ArgumentException( "Unknown gradient style" );
        }

        SerializeAttribute( writer, LinearGradientDegreeAttributeName, dGradientDegree, 0 );

        SerializeStopColorElements( writer, 0, fill.ColorObject );
        SerializeStopColorElements( writer, 1, fill.PatternColorObject );
      }
    }
    /// <summary>
    /// Serializes single from corner or from center gradient fill object into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="fill">Object to serialize.</param>
    private void SerializeFromCenterCornerGradientFill( XmlWriter writer, FillImpl fill )
    {
      ExcelGradientStyle gradientStyle = fill.GradientStyle;
      ExcelGradientVariants gradientVariant = fill.GradientVariant;
      SerializeAttribute( writer, GradientFillTypeAttributeName, GradientFillTypePath, string.Empty );

      double dTop = double.MinValue;
      double dBottom = double.MinValue;
      double dLeft = double.MinValue;
      double dRight = double.MinValue;

      if( gradientStyle == ExcelGradientStyle.From_Center )
      {
        dTop = dBottom = dLeft = dRight = 0.5;
      }
      else
      {
        switch( gradientVariant )
        {
          case ExcelGradientVariants.ShadingVariants_1:
            break;

          case ExcelGradientVariants.ShadingVariants_2:
            dLeft = dRight = 1;
            break;

          case ExcelGradientVariants.ShadingVariants_3:
            dTop = dBottom = 1;
            break;

          case ExcelGradientVariants.ShadingVariants_4:
            dTop = dBottom = dLeft = dRight = 1;
            break;

          default:
            throw new ArgumentException( "Unknown gradient variant" );
        }
      }

      SerializeAttribute( writer, TopConvergenceAttributeName, dTop, double.MinValue );
      SerializeAttribute( writer, BottomConvergenceAttributeName, dBottom, double.MinValue );
      SerializeAttribute( writer, LeftConvergenceAttributeName, dLeft, double.MinValue );
      SerializeAttribute( writer, RightConvergenceAttributeName, dRight, double.MinValue );

      SerializeStopColorElements( writer, 0, fill.ColorObject );
      SerializeStopColorElements( writer, 1, fill.PatternColorObject );
    }
    /// <summary>
    /// Serializes stop color for gradient fill object into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dPosition">Stop position attribute value.</param>
    /// <param name="color">Represents Color.</param>
    private void SerializeStopColorElements( XmlWriter writer, double dPosition, ColorObject color )
    {
      writer.WriteStartElement( GradientStopTagName );
      SerializeAttribute( writer, GradientStopPositionAttributeName, dPosition, double.MinValue );
      SerializeColorObject( writer, ColorTagName, color );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Converts specified pattern into string that can be understood by MS Excel 2007.
    /// </summary>
    /// <param name="pattern">Pattern to convert.</param>
    /// <returns>Converted value.</returns>
    private string ConvertPatternToString( ExcelPattern pattern )
    {
      Excel2007Pattern newPattern = ( Excel2007Pattern )pattern;
      return newPattern.ToString();
    }
    /// <summary>
    /// Serializes borders into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to save borders into.</param>
    /// <returns>Border indexes.</returns>
    private int[] SerializeBorders( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      Dictionary<BordersCollection, int> hashBorders = new Dictionary<BordersCollection, int>();
      ExtendedFormatsCollection arrExtFormats = m_book.InnerExtFormats;

      int iCount = arrExtFormats.Count;
      int[] arrBorderIndexes = new int[ iCount ];
      BordersCollection[] arrBorders = new BordersCollection[ iCount ];
      int iBordersIndex = -1;

      for( int i = 0; i < iCount; i++ )
      {
          BordersCollection borders = null;

          //MS Excel 2007 serializes "default" border type on the first position,
          //so we should do the same operation here.
          if (iBordersIndex == -1)
          {
              WorkbookImpl workbook = new WorkbookImpl(m_book.Application, m_book, ExcelVersion.Excel2007);
              BordersCollection bordersCollection = new BordersCollection(workbook.Application, workbook, true);
              ExtendedFormatWrapper wrap = new ExtendedFormatWrapper(workbook, 0);

              bordersCollection.InnerList.Clear();
              bordersCollection.InnerList.Add(new BorderImpl(workbook.Application, workbook, wrap, ExcelBordersIndex.DiagonalDown));
              bordersCollection.InnerList.Add(new BorderImpl(workbook.Application, workbook, wrap, ExcelBordersIndex.DiagonalUp));
              bordersCollection.InnerList.Add(new BorderImpl(workbook.Application, workbook, wrap, ExcelBordersIndex.EdgeBottom));
              bordersCollection.InnerList.Add(new BorderImpl(workbook.Application, workbook, wrap, ExcelBordersIndex.EdgeLeft));
              bordersCollection.InnerList.Add(new BorderImpl(workbook.Application, workbook, wrap, ExcelBordersIndex.EdgeRight));
              bordersCollection.InnerList.Add(new BorderImpl(workbook.Application, workbook, wrap, ExcelBordersIndex.EdgeTop));

              borders = bordersCollection;
          }
          else
          {
              borders = (BordersCollection)arrExtFormats[i].Borders;
          }

          if (hashBorders.ContainsKey(borders))
          {
              arrBorderIndexes[i] = hashBorders[borders];
          }
          else
          {
              iBordersIndex = hashBorders.Count;
              hashBorders.Add(borders, iBordersIndex);
              arrBorders[iBordersIndex] = borders;
              arrBorderIndexes[i] = iBordersIndex;

              if (iBordersIndex == 0)
                  i--;
          }
      }

      writer.WriteStartElement( BordersTagName );
      writer.WriteAttributeString( CountAttributeName, ( iBordersIndex + 1 ).ToString() );

      for( int i = 0; i <= iBordersIndex; i++ )
      {
        SerializeBordersCollection( writer, arrBorders[ i ] );
      }

      writer.WriteEndElement();
      return arrBorderIndexes;
    }
    /// <summary>
    /// Serializes color into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize color into.</param>
    /// <param name="tagName">Name of the color tag.</param>
    /// <param name="color">Color value.</param>
    private void SerializeIndexedColor( XmlWriter writer, string tagName, ExcelKnownColors color )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      writer.WriteStartElement( tagName );

      if( ( int )color > 65 )
      {
        writer.WriteAttributeString( "auto", "1" );
      }
      else
      {
        writer.WriteAttributeString( ColorIndexedAttributeName, ( ( int )color ).ToString() );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes rgb color value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Xml tag name to use for color serialization.</param>
    /// <param name="color">Color to serialize.</param>
    public void SerializeRgbColor( XmlWriter writer, string tagName, ColorObject color )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      int value = color.Value;
      writer.WriteStartElement( tagName );
      writer.WriteAttributeString( ColorRgbAttribute, value.ToString( "X8" ) );
      SerializeAttribute( writer, ColorTintAttributeName, color.Tint, 0 );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes theme color into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize color into.</param>
    /// <param name="tagName">Name of the color tag.</param>
    /// <param name="color">Color value.</param>
    private void SerializeThemeColor( XmlWriter writer, string tagName, ColorObject color )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      writer.WriteStartElement( tagName );
      writer.WriteAttributeString( ColorThemeAttributeName, color.Value.ToString() );
      SerializeAttribute( writer, ColorTintAttributeName, color.Tint, 0 );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes color into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize color into.</param>
    /// <param name="tagName">Name of the color tag.</param>
    /// <param name="color">Color value.</param>
    private void SerializeColorObject( XmlWriter writer, string tagName, ColorObject color )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      switch( color.ColorType )
      {
        case ColorType.Indexed:
          SerializeIndexedColor( writer, tagName, ( ExcelKnownColors )color.Value );
          break;

        case ColorType.RGB:
          SerializeRgbColor( writer, tagName, color );
          break;

        case ColorType.Theme:
          SerializeThemeColor( writer, tagName, color );
          break;

        default:
          throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Serializes borders collection into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize borders into.</param>
    /// <param name="borders">Borders collection to serialize.</param>
    private void SerializeBordersCollection( XmlWriter writer, BordersCollection borders )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( borders == null )
        throw new ArgumentNullException( "borders" );

      writer.WriteStartElement( BordersCollectionTagName );

      SerializeAttribute( writer, DiagonalUpAttributeName,
        borders[ ExcelBordersIndex.DiagonalUp ].ShowDiagonalLine, false );

      SerializeAttribute( writer, DiagonalDownAttributeName,
        borders[ ExcelBordersIndex.DiagonalDown ].ShowDiagonalLine, false );

      //for( int i = 0, len = borders.Count; i < len; i++ )
      // NOTE: order is important so we cannot use this code and should explictly choose borders to serialize.
      //foreach( BorderImpl border in borders )
      //{
      //  //BorderImpl border = ( BorderImpl )borders[ i ];
      //  SerializeBorder( writer, border );
      //}

      SerializeBorder( writer, ( BorderImpl )borders[ ExcelBordersIndex.EdgeLeft ] );
      SerializeBorder( writer, ( BorderImpl )borders[ ExcelBordersIndex.EdgeRight ] );
      SerializeBorder( writer, ( BorderImpl )borders[ ExcelBordersIndex.EdgeTop ] );
      SerializeBorder( writer, ( BorderImpl )borders[ ExcelBordersIndex.EdgeBottom ] );
      SerializeBorder( writer, ( BorderImpl )borders[ ExcelBordersIndex.DiagonalUp ] );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single border entry inside BordersCollection.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize border into.</param>
    /// <param name="border">Border to serialize.</param>
    private void SerializeBorder( XmlWriter writer, BorderImpl border )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( border == null )
        throw new ArgumentNullException( "border" );

      string strBorderTag = GetBorderTag( border.BorderIndex );

      if( strBorderTag != null )
      {
        writer.WriteStartElement( strBorderTag );

        if( border.LineStyle != ExcelLineStyle.None )
        {
          writer.WriteAttributeString( BorderStyleAttributeName, GetBorderLineStyle( border ) );
          SerializeColorObject( writer, BorderColorTagName, border.ColorObject );
        }

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Gets border.
    /// </summary>
    /// <param name="borderIndex">Represents Border line type</param>
    /// <returns>Border extracted.</returns>
    private static string GetBorderTag( ExcelBordersIndex borderIndex )
    {
      string strResult = null;
      Excel2007BorderIndex newBorderIndex = ( Excel2007BorderIndex )borderIndex;

      if( newBorderIndex != Excel2007BorderIndex.none )
      {
        strResult = newBorderIndex.ToString();
      }

      return strResult;
    }
    /// <summary>
    /// Gets Border line style.
    /// </summary>
    /// <param name="border">Border line style to be extracted for.</param>
    /// <returns>Extracted line style.</returns>
    private string GetBorderLineStyle( BorderImpl border )
    {
      Excel2007BorderLineStyle borderLineStyle = ( Excel2007BorderLineStyle )border.LineStyle;
      string strResult = borderLineStyle.ToString();
      return LowerFirstLetter( strResult );
    }
    /// <summary>
    /// Serializes extended formats used by named styles.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="arrFillIndexes">Index to fill objects (position in array - xf index,
    /// value - fill index).</param>
    /// <param name="arrBorderIndexes">Index to border objects (position in array - xf index,
    /// value - border index).</param>
    /// <returns>Dictionary containing extended format indexes.</returns>
    private Dictionary<int, int> SerializeNamedStyleXFs( XmlWriter writer, int[] arrFillIndexes,
      int[] arrBorderIndexes )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( arrFillIndexes == null )
        throw new ArgumentNullException( "arrFillIndexes" );

      if( arrBorderIndexes == null )
        throw new ArgumentNullException( "arrBorderIndexes" );

      writer.WriteStartElement( NamedStyleXFsTagName );

      ExtendedFormatsCollection arrExtendedFormats = m_book.InnerExtFormats;
      int iCount = arrExtendedFormats.Count;
      writer.WriteAttributeString( CountAttributeName, iCount.ToString() );
      Dictionary<int, int> result = new Dictionary<int, int>();

      int iIndex = 0;

      for( int i = 0, iExtFormatsCount = arrExtendedFormats.Count; i < iExtFormatsCount; i++ )
      {
        ExtendedFormatImpl extFormat = arrExtendedFormats[ i ];

        if( !extFormat.HasParent )
        {
          SerializeExtendedFormat( writer, arrFillIndexes, arrBorderIndexes, extFormat, null, true );
          result.Add( extFormat.Index, iIndex );
          iIndex++;
        }
      }

      writer.WriteEndElement();
      return result;
    }
    /// <summary>
    /// Serialize extended formats that are responsible for cell format (do not have names).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="arrFillIndexes">Index to fill objects (position in array - xf index,
    /// value - fill index).</param>
    /// <param name="arrBorderIndexes">Index to border objects (position in array - xf index,
    /// value - border index).</param>
    /// <param name="hashNewParentIndexes">Dictionary with new parent indexes.</param>
    /// <returns>Dictionary containing extended format indexes.</returns>
    private Dictionary<int, int> SerializeNotNamedXFs( XmlWriter writer, int[] arrFillIndexes,
      int[] arrBorderIndexes, Dictionary<int, int> hashNewParentIndexes )
    {
      ExtendedFormatsCollection arrExtFormats = m_book.InnerExtFormats;
      int iCount = arrExtFormats.Count;
      int iXFToSerializeCount = iCount - m_book.InnerStyles.Count;

      writer.WriteStartElement( CellFormatXFsTagName );
      //writer.WriteAttributeString( CountAttributeName, iXFToSerializeCount.ToString() );
      int iCurrentIndex = 0;
      Dictionary<int, int> hashNewXFIndexes = new Dictionary<int, int>();

      for( int i = 0; i < iCount; i++ )
      {
        ExtendedFormatImpl format = arrExtFormats[ i ];

        if( format.HasParent )
        {
          hashNewXFIndexes.Add( format.Index, iCurrentIndex );
          SerializeExtendedFormat( writer, arrFillIndexes, arrBorderIndexes,
            format, hashNewParentIndexes, false );
          iCurrentIndex++;
        }
      }

      writer.WriteEndElement();
      return hashNewXFIndexes;
    }
    /// <summary>
    /// Serializes single extended format.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="arrFillIndexes">Array with fill indexes.</param>
    /// <param name="arrBorderIndexes">Array with border indexes.</param>
    /// <param name="format">Format to serialize.</param>
    /// <param name="newParentIndexes">Dictionary with new parent indexes.</param>
    /// <param name="defaultApplyValue">Default value for ApplyAlignment, ApplyBorder, etc.</param>
    private void SerializeExtendedFormat( XmlWriter writer, int[] arrFillIndexes,
      int[] arrBorderIndexes, ExtendedFormatImpl format, Dictionary<int, int> newParentIndexes,
      bool defaultApplyValue )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( arrFillIndexes == null )
        throw new ArgumentNullException( "arrFillIndexes" );

      if( arrBorderIndexes == null )
        throw new ArgumentNullException( "arrBorderIndexes" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      int iXFIndex = format.Index;
      writer.WriteStartElement( ExtendedFormatTagName );
      writer.WriteAttributeString( NumberFormatIdAttributeName, format.NumberFormatIndex.ToString() );
      writer.WriteAttributeString( FontIdAttributeName, format.FontIndex.ToString() );
      writer.WriteAttributeString( FillIdAttributeName, arrFillIndexes[ iXFIndex ].ToString() );
      writer.WriteAttributeString( BorderIdAttributeName, arrBorderIndexes[ iXFIndex ].ToString() );

      if( format.HasParent )
      {
          int iNewIndex;
          if(!newParentIndexes.TryGetValue(format.ParentIndex,out iNewIndex))
          {
              iNewIndex= format.ParentIndex;
          }
          if(newParentIndexes.Count-1<iNewIndex)
              writer.WriteAttributeString(XFIdAttributeName, newParentIndexes[0].ToString());      
          else
              writer.WriteAttributeString(XFIdAttributeName, iNewIndex.ToString());
              
      }

      SerializeAttribute( writer, IncludeAlignmentAttributeName, format.IncludeAlignment, defaultApplyValue );
      SerializeAttribute( writer, IncludeBorderAttributeName, format.IncludeBorder, defaultApplyValue );
      SerializeAttribute( writer, IncludeFontAttributeName, format.IncludeFont, defaultApplyValue );
      SerializeAttribute( writer, IncludeNumberFormatAttributeName, format.IncludeNumberFormat, defaultApplyValue );
      SerializeAttribute( writer, IncludePatternsAttributeName, format.IncludePatterns, defaultApplyValue );
      SerializeAttribute( writer, IncludeProtectionAttributeName, format.IncludeProtection, defaultApplyValue );
      SerializeAttribute( writer, QuotePreffixAttributeName, format.IsFirstSymbolApostrophe, false );

      SerializeAlignment( writer, format );
      SerializeProtection( writer, format );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes alignment options if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="format">ExtendedFormat to serialize.</param>
    private void SerializeAlignment( XmlWriter writer, ExtendedFormatImpl format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      if( /*format.IncludeAlignment &&*/ !IsDefaultAlignment( format ) )
      {
        writer.WriteStartElement( AlignmentTagName );

        if( format.HorizontalAlignment != ExcelHAlign.HAlignGeneral )
        {
          string strHAlign = ( ( Excel2007HAlign )format.HorizontalAlignment ).ToString();
          writer.WriteAttributeString( HAlignAttributeName, strHAlign );
        }

        if( format.VerticalAlignment != ExcelVAlign.VAlignBottom )
        {
          string strVerticalAlign = ( ( Excel2007VAlign )format.VerticalAlignment ).ToString();
          writer.WriteAttributeString( VerticalAttributeName, strVerticalAlign );
        }

        SerializeAttribute( writer, TextRotationAttributeName, format.Rotation, 0 );
        SerializeAttribute( writer, WrapTextAttributeName, format.WrapText, false );
        SerializeAttribute( writer, IndentAttributeName, format.IndentLevel, 0 );
        SerializeAttribute( writer, JustifyLastLineAttributeName, format.JustifyLast, false );
        SerializeAttribute( writer, ShrinkToFitAttributeName, format.ShrinkToFit, false );
        SerializeAttribute( writer, ReadingOrderAttributeName, ( int )format.ReadingOrder, 0 );

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Checks whether specified extended format has default alignment options.
    /// </summary>
    /// <param name="format">Extended format to check.</param>
    /// <returns>True if extended format has default alignment options.</returns>
    private bool IsDefaultAlignment( ExtendedFormatImpl format )
    {
      return format.HorizontalAlignment == ExcelHAlign.HAlignGeneral &&
      format.IndentLevel == 0 &&
      !format.JustifyLast &&
      format.ReadingOrder == ExcelReadingOrderType.Context &&
      !format.ShrinkToFit &&
      format.Rotation == 0 &&
      !format.WrapText &&
      format.VerticalAlignment == ExcelVAlign.VAlignBottom;
    }
    /// <summary>
    /// Serializes protection options if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="format">ExtendedFormat to serialize.</param>
    private void SerializeProtection( XmlWriter writer, ExtendedFormatImpl format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );
     
      if( ( format.FormulaHidden != HiddenDefaultValue ||format.Locked != LockedDefaultValue ) )
      {
        writer.WriteStartElement( ProtectionTagName );
        SerializeAttribute( writer, HiddenAttributeName, format.FormulaHidden, HiddenDefaultValue );
        SerializeAttribute( writer, LockedAttributeName, format.Locked, LockedDefaultValue );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes all named styles.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    private void SerializeStyles( XmlWriter writer, Dictionary<int, int> hashNewParentIndexes )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( hashNewParentIndexes == null )
        throw new ArgumentNullException( "hashNewParentIndexes" );

      StylesCollection arrStyles = m_book.InnerStyles;
      int iCount = arrStyles.Count;

      writer.WriteStartElement( CellStylesTagName );
      writer.WriteAttributeString( CountAttributeName, iCount.ToString() );

      for( int i = 0; i < iCount; i++ )
      {
        StyleImpl style = ( StyleImpl )arrStyles[ i ];
        SerializeStyle( writer, style, hashNewParentIndexes );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single named style object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="style">Style object to serialize.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    private void SerializeStyle( XmlWriter writer, StyleImpl style, Dictionary<int, int> hashNewParentIndexes )
    {
      if( writer == null )
        throw new ArgumentNullException();

      if( style == null )
        throw new ArgumentNullException( "style" );

      if( hashNewParentIndexes == null )
        throw new ArgumentNullException( "hashNewParentIndexes" );

      writer.WriteStartElement( CellStyleTagName );
      writer.WriteAttributeString( NameAttributeName, style.Name );

      int iNewParentIndex = hashNewParentIndexes[ style.XFormatIndex ];

      writer.WriteAttributeString( XFIdAttributeName, iNewParentIndex.ToString() );

      if( style.BuiltIn )
      {
        StyleRecord record = style.Record;
        writer.WriteAttributeString( StyleBuiltinIdAttributeName, record.BuildInOrNameLen.ToString() );

        if( record.OutlineStyleLevel != byte.MaxValue )
          writer.WriteAttributeString( OutlineLevelAttribute, record.OutlineStyleLevel.ToString() );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes dictionary where key and value are strings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="toSerialize">Dictionary to serialize.</param>
    /// <param name="tagName">Tag name for single entry.</param>
    /// <param name="keyAttributeName">Attribute name for key.</param>
    /// <param name="valueAttributeName">Attribute name for value.</param>
    /// <param name="keyPreprocessor">File name preprocessor that allows user to change item names.</param>
    private void SerializeDictionary( XmlWriter writer, IDictionary<string, string> toSerialize,
      string tagName, string keyAttributeName, string valueAttributeName,
      IFileNamePreprocessor keyPreprocessor )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( toSerialize == null )
        throw new ArgumentNullException( "toSerialize" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      if( keyAttributeName == null || keyAttributeName.Length == 0 )
        throw new ArgumentOutOfRangeException( "keyAttributeName" );

      if( valueAttributeName == null || valueAttributeName.Length == 0 )
        throw new ArgumentOutOfRangeException( "valueAttributeName" );

      foreach( KeyValuePair<string, string> entry in toSerialize )
      {
        string strKey = entry.Key;
        string strValue = entry.Value;

        if( keyPreprocessor != null )
        {
          //strKey = keyPreprocessor.PreprocessName( strKey );
        }

        writer.WriteStartElement( tagName );
        writer.WriteAttributeString( keyAttributeName, strKey );

        writer.WriteAttributeString( valueAttributeName, strValue );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Lower first letter in the string.
    /// </summary>
    /// <param name="value">String to process.</param>
    /// <returns>New string with the first letter in lower case.</returns>
    public static string LowerFirstLetter( string value )
    {
      return Char.ToLower( value[ 0 ] ) + value.Remove( 0, 1 );
    }
    /// <summary>
    /// Serializes attribute if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="attributeName">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="defaultValue">Default value.</param>
    internal static void SerializeAttribute( XmlWriter writer, string attributeName, bool value, bool defaultValue )
    {
      if( value != defaultValue )
      {
        string strValue = value ?
          TrueValue :
          FalseValue;

        writer.WriteAttributeString( attributeName, strValue );
      }
    }
    /// <summary>
    /// Serielaize bool value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="attributeName"></param>
    /// <param name="value"></param>
    internal static void SerializeBool(XmlWriter writer, string attributeName, bool value )
    {
         string strValue = value ?
          TrueValue :
          FalseValue;

        writer.WriteAttributeString( attributeName, strValue );
    }
    /// <summary>
    /// Serializes attribute if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="attributeName">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="defaultValue">Default value.</param>
    internal static void SerializeAttribute( XmlWriter writer, string attributeName, int value, int defaultValue )
    {
      if( value != defaultValue )
      {
        string strValue = value.ToString();
        writer.WriteAttributeString( attributeName, strValue );
      }
    }
    /// <summary>
    /// Serializes attribute if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="attributeName">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="defaultValue">Default value.</param>
    internal static void SerializeAttribute( XmlWriter writer, string attributeName,
      double value, double defaultValue )
    {
      SerializeAttribute( writer, attributeName, value, defaultValue, null );
    }
    /// <summary>
    /// Serializes attribute if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="attributeName">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <param name="attributeNamespace">Namespace of the attribute that is being serialized.</param>
    internal static void SerializeAttribute( XmlWriter writer, string attributeName,
      double value, double defaultValue, string attributeNamespace )
    {
      if( value != defaultValue )
      {
        string strValue = XmlConvert.ToString( value );
        writer.WriteAttributeString( attributeName, attributeNamespace, strValue );
      }
    }
    /// <summary>
    /// Serializes attribute if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="attributeName">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="defaultValue">Default value.</param>
    internal static void SerializeAttribute( XmlWriter writer, string attributeName,
      string value, string defaultValue )
    {
      if( value != defaultValue )
      {
        writer.WriteAttributeString( attributeName,value );
      }
    }
    /// <summary>
    /// Serializes attribute if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="attributeName">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="defaultValue">Default value.</param>
    internal static void SerializeAttribute( XmlWriter writer, string attributeName,
      Enum value, Enum defaultValue )
    {
      if( value != defaultValue )
      {
        writer.WriteAttributeString( attributeName, LowerFirstLetter( value.ToString() ) );
      }
    }
    /// <summary>
    /// Serializes element if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="elementName">Element name.</param>
    /// <param name="value">Element value.</param>
    /// <param name="defaultValue">Default value.</param>
    protected static void SerializeElementString( XmlWriter writer, string elementName,
      string value, string defaultValue )
    {
      if( value != defaultValue )
      {
        writer.WriteElementString( elementName, value );
      }
    }
    /// <summary>
    /// Serializes element if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="elementName">Element name.</param>
    /// <param name="value">Element value.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <param name="prefix">Element prefix.</param>
    private static void SerializeElementString( XmlWriter writer, string elementName,
      string value, string defaultValue, string prefix )
    {
      if( value != defaultValue )
      {
        writer.WriteElementString( prefix, elementName, null, value );
      }
    }
    /// <summary>
    /// Serializes element if it differs from default value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="elementName">Element name.</param>
    /// <param name="value">Element value.</param>
    /// <param name="defaultValue">Default value.</param>
    private static void SerializeElementString( XmlWriter writer, string elementName,
      int value, int defaultValue )
    {
      if( value != defaultValue )
      {
        string strValue = value.ToString();
        writer.WriteElementString( elementName, strValue );
      }
    }
    /// <summary>
    /// Serializes single sheet.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="sheet">Sheet to serialize.</param>
    public void SeiralizeSheet( XmlWriter writer, WorksheetBaseImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      throw new NotImplementedException();
    }
    /// <summary>
    /// Serializes sheet data. 
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="cells">Cell record collection.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    public void SerializeSheetData( XmlWriter writer, CellRecordCollection cells,
      Dictionary<int, int> hashNewParentIndexes, string cellTag,
      Dictionary<string, string> additionalAttributes, bool isSpansNeeded )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( cells == null )
        throw new ArgumentNullException( "cells" );

      writer.WriteStartElement( SheetDataTagName );
      SerializeAttributes( writer, additionalAttributes );

      for( int i = cells.FirstRow, iLen = cells.LastRow; i <= iLen; i++ )
      {
        if( cells.ContainsRow( i - 1 ) )
        {
          RowStorage row = ( RowStorage )cells.Table.Rows[ i - 1 ];
          SerializeRow( writer, row, cells, i - 1, hashNewParentIndexes, cellTag, isSpansNeeded );
        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes specified attributes if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize.</param>
    /// <param name="additionalAttributes">Collection with attributes to serialize.
    /// Key - attribute name, value - attribute value.</param>
    private void SerializeAttributes( XmlWriter writer, Dictionary<string, string> additionalAttributes )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( additionalAttributes == null || additionalAttributes.Count == 0 )
        return;

      foreach( KeyValuePair<string, string> pair in additionalAttributes )
      {
        writer.WriteAttributeString( pair.Key, pair.Value );
      }
    }
    /// <summary>
    /// Serializes single row.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="row">Row to serialize.</param>
    /// <param name="cells">Cell record collection.</param>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    private void SerializeRow( XmlWriter writer, RowStorage row, CellRecordCollection cells,
      int iRowIndex, Dictionary<int, int> hashNewParentIndexes, string cellTag, bool isSpansNeeded )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( row == null )
        throw new ArgumentNullException( "row" );

      writer.WriteStartElement( RowTagName );

      SerializeAttribute( writer, RowIndexAttributeName, ( iRowIndex + 1 ).ToString(), string.Empty );

      if( isSpansNeeded && row.FirstColumn >= 0 )
      {
        string spans = ( row.FirstColumn + 1 ).ToString() + ":" + ( row.LastColumn + 1 ).ToString();
        SerializeAttribute( writer, SpansTag, spans, string.Empty );
      }

      if( hashNewParentIndexes != null && hashNewParentIndexes.Count>0 )
      {
        if ((hashNewParentIndexes.ContainsKey(row.ExtendedFormatIndex)) && row.ExtendedFormatIndex!=m_book.DefaultXFIndex)
            SerializeAttribute( writer, StyleIndexAttributeName, 
                hashNewParentIndexes[ row.ExtendedFormatIndex ], 0 );

        SerializeAttribute( writer, RowCustomFormatAttributeName, row.IsFormatted, false );
        SerializeAttribute( writer, RowHeightAttributeName, ( double )row.Height /
          WorkbookXmlSerializator.DEF_ROW_DIV, m_worksheetImpl.StandardHeight  );
      }
      SerializeAttribute( writer, RowColumnCollapsedAttribute, row.IsCollapsed, false );
      SerializeAttribute( writer, RowCustomHeightAttributeName, row.IsBadFontHeight, false );
      SerializeAttribute( writer, RowHiddenAttributeName, row.IsHidden, false );
      SerializeAttribute( writer, RowColumnOutlineLevelAttribute, row.OutlineLevel, 0 );
      SerializeAttribute( writer, RowThickTopAttributeName, row.IsSpaceAboveRow, false );
      SerializeAttribute( writer, RowThickBottomAttributeName, row.IsSpaceBelowRow, false );

      SerializeCells( writer, row, cells, hashNewParentIndexes, cellTag );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes cells.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="row">Current row.</param>
    /// <param name="cells">Cell record collection.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    private void SerializeCells( XmlWriter writer, RowStorage row, CellRecordCollection cells,
      Dictionary<int, int> hashNewParentIndexes, string cellTag )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( row == null )
        throw new ArgumentNullException( "row" );

      if( cells == null )
        throw new ArgumentNullException( "cells" );

      RowStorageEnumerator rowStorageEnumerator = (row.GetEnumerator(m_recordExtractor) as RowStorageEnumerator);

      while( rowStorageEnumerator.MoveNext() )
      {
        BiffRecordRaw record = (rowStorageEnumerator.Current as BiffRecordRaw);

        switch( record.TypeCode )
        {
          case TBIFFRecord.MulRK:
            SerializeMulRKRecordValues( writer, ( MulRKRecord )record, hashNewParentIndexes );
            break;

          case TBIFFRecord.MulBlank:
            SerializeMulBlankRecord( writer, ( MulBlankRecord )record, hashNewParentIndexes );
            break;

          case TBIFFRecord.Blank:
            BlankRecord blankRecord = ( BlankRecord )record;
            SerializeBlankCell( writer, blankRecord.Row + 1, blankRecord.Column + 1,
              blankRecord.ExtendedFormatIndex, hashNewParentIndexes );
            break;

          default:
            SerializeCell( writer, record, rowStorageEnumerator, cells, hashNewParentIndexes, cellTag );
            break;
        }
      }
    }
    /// <summary>
    /// Serializes cells, which are not represented by Blank, MulBlank and MulRK records.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="record">Record to serialize.</param>
    /// <param name="rowStorageEnumerator">Row storage enumerator.</param>
    /// <param name="cells">Cell record collection.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    private void SerializeCell( XmlWriter writer, BiffRecordRaw record,
      RowStorageEnumerator rowStorageEnumerator, CellRecordCollection cells,
      Dictionary<int, int> hashNewParentIndexes, string cellTag )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      if( cells == null )
        throw new ArgumentNullException( "cells" );

      if( rowStorageEnumerator == null )
        throw new ArgumentNullException( "rowStorageEnumerator" );

      writer.WriteStartElement( cellTag );

      ICellPositionFormat cell = (record as ICellPositionFormat);

      string strCellName = RangeImpl.GetCellName( cell.Column + 1, cell.Row + 1 );
      SerializeAttribute( writer, ReferenceAttributeName, strCellName, null );

      if( hashNewParentIndexes != null && hashNewParentIndexes.Count>0 )
      {
        int iIndex = cell.ExtendedFormatIndex;

        if (hashNewParentIndexes.ContainsKey(iIndex))
            iIndex = hashNewParentIndexes[iIndex];
        else
            iIndex -= 1;
        
        SerializeAttribute( writer, StyleIndexAttributeName, iIndex, 0 );
      }

      string strCellType;
      CellType cellType = GetCellDataType( record, out strCellType );

      string cellInlineValue=null;
      if (this.Worksheet.InlineStrings.TryGetValue(strCellName, out cellInlineValue))
      {
            cellType = CellType.inlineStr;
            strCellType = CellTypeInlineString;
      } 

      SerializeAttribute( writer, CellDataTypeAttributeName, strCellType, DefaultCellDataType );

      if( record.TypeCode == TBIFFRecord.Formula )
      {
        FormulaRecord formulaRecord = ( FormulaRecord )record;
        ArrayRecord arrayRecord;

        if( ( arrayRecord = rowStorageEnumerator.GetArrayRecord() ) != null )
        {
          SerializeArrayFormula( writer, arrayRecord );
        }
        else
        {
          SerializeSimpleFormula( writer, formulaRecord, cells );
        }

        SerializeFormulaValue( writer, formulaRecord, cellType, rowStorageEnumerator );
      }

      SerializeCellValue( writer, record, cellType,cellInlineValue );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize blank cell.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="iXFIndex">Extended format index.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    private void SerializeBlankCell( XmlWriter writer, int iRowIndex, int iColumnIndex, int iXFIndex,
      Dictionary<int, int> hashNewParentIndexes )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( hashNewParentIndexes == null )
        throw new ArgumentNullException( "hashNewParentIndexes" );

      writer.WriteStartElement( CellTagName );
      SerializeAttribute( writer, ReferenceAttributeName,
        RangeImpl.GetCellName( iColumnIndex, iRowIndex ), string.Empty );
      int iHashIndex;

      if (!hashNewParentIndexes.TryGetValue(iXFIndex, out iHashIndex))
          if (hashNewParentIndexes.ContainsKey(iXFIndex-1))
              iHashIndex = m_book.DefaultXFIndex;
          else
              iHashIndex = iXFIndex-1;
      SerializeAttribute(writer, StyleIndexAttributeName, iHashIndex, 0);

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes MulBlank record.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="mulBlankRecord">MulBlank record to serialize.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    private void SerializeMulBlankRecord( XmlWriter writer, MulBlankRecord mulBlankRecord,
      Dictionary<int, int> hashNewParentIndexes )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( mulBlankRecord == null )
        throw new ArgumentNullException( "mulBlankRecord" );

      if( hashNewParentIndexes == null )
        throw new ArgumentNullException( "hashNewParentIndexes" );

      int iRow = mulBlankRecord.Row + 1;
      int iCol = mulBlankRecord.FirstColumn + 1;
      List<ushort> formatIndexes = mulBlankRecord.ExtendedFormatIndexes;

      for( int i = 0, iLength = formatIndexes.Count; i < iLength; i++ )
      {
        SerializeBlankCell( writer, iRow, iCol + i, Convert.ToInt32( formatIndexes[ i ] ),
          hashNewParentIndexes );
      }
    }
    /// <summary>
    /// Serialize MulRKRecord cell values.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="mulRKRecord">MulRK record.</param>
    /// <param name="hashNewParentIndexes">Dictionary with new extended format indexes.
    /// Key - index in our collection of extended formats, value - index in the cellStyleXfs block.</param>
    private void SerializeMulRKRecordValues( XmlWriter writer, MulRKRecord mulRKRecord,
      Dictionary<int, int> hashNewParentIndexes )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( mulRKRecord == null )
        throw new ArgumentNullException( "mulRkRecord" );

      if( hashNewParentIndexes == null )
        throw new ArgumentNullException( "hashNewParentIndexes" );

      int iRow = mulRKRecord.Row + 1;
      int iCol = mulRKRecord.FirstColumn + 1;
      List<MulRKRecord.RkRec> rkRecords = mulRKRecord.Records;

      for( int i = 0, iLength = rkRecords.Count; i < iLength; i++ )
      {
        writer.WriteStartElement( CellTagName );

        MulRKRecord.RkRec rkRecord = ( MulRKRecord.RkRec )rkRecords[ i ];
        SerializeAttribute( writer, ReferenceAttributeName,
          RangeImpl.GetCellName( iCol + i, iRow ), string.Empty );

        int iIndex = ( int )rkRecord.ExtFormatIndex;
        int iHashIndex = 0;
        if (hashNewParentIndexes.ContainsKey(iIndex))
            iHashIndex = hashNewParentIndexes[iIndex];
        SerializeAttribute( writer, StyleIndexAttributeName, iHashIndex, 0 );

        writer.WriteElementString( CellValueTagName, XmlConvert.ToString( rkRecord.RkNumber ) );

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes array formula.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="arrayRecord">Formula array record.</param>
    private void SerializeArrayFormula( XmlWriter writer, ArrayRecord arrayRecord )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( arrayRecord == null )
        throw new ArgumentNullException( "arrayRecord" );

      writer.WriteStartElement( FormulaTagName );
      writer.WriteAttributeString( FormulaTypeAttributeName, FormulaType.array.ToString() );
      //SerializeAttribute( writer, CalculateOnOpen, arrayRecord.IsRecalculateOnOpen, false );
      writer.WriteAttributeString( AlwaysCalculateArray, "true" );


      string strArrayFormula = m_formulaUtil.ParsePtgArray( arrayRecord.Formula );

      if( strArrayFormula.Length > MaximumFormulaLength )
        throw new ApplicationException( "Formula length is too big. Maximum formula length is " + MaximumFormulaLength + "." );

      string strCellsRange = RangeImpl.GetAddressLocal( arrayRecord.FirstRow + 1,
        arrayRecord.FirstColumn + 1, arrayRecord.LastRow + 1, arrayRecord.LastColumn + 1 );

      writer.WriteAttributeString( RangeOfCellsAttributeName, strCellsRange );
      writer.WriteString( strArrayFormula );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes formula.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="formulaRecord">Formula record.</param>
    /// <param name="cells">Cell record collection.</param>
    private void SerializeSimpleFormula( XmlWriter writer, FormulaRecord formulaRecord,
      CellRecordCollection cells )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( formulaRecord == null )
        throw new ArgumentNullException( "formulaRecord" );

      if( cells == null )
        throw new ArgumentNullException( "cells" );

      string strFormula = String.Empty;

      if (formulaRecord.Formula != null && formulaRecord.Formula.Length == 0)
      {
          strFormula = (cells.Sheet as WorksheetImpl).m_formulaString;
      }
      else
      {
          Ptg token = formulaRecord.Formula[0];

          if (token.TokenCode == FormulaToken.tExp)
          {
              ControlPtg control = token as ControlPtg;
              RowStorage rowStorage = cells.Table.Rows[control.RowIndex];

              if (rowStorage.HasFormulaArrayRecord(control.ColumnIndex))
                  return;
          }

          m_formulaUtil.CheckFormulaVersion(formulaRecord.Formula);
          strFormula = m_formulaUtil.ParsePtgArray(formulaRecord.Formula, 0, 0, false, null, false, true, cells.Sheet);
      }

      if( strFormula[ 0 ] == '=' )
        strFormula = UtilityMethods.RemoveFirstCharUnsafe( strFormula );

      if( strFormula.Length > MaximumFormulaLength )
        throw new ApplicationException( "Formula length is too big. Maximum formula length is " + MaximumFormulaLength + "." );

      writer.WriteStartElement( FormulaTagName );

      bool bCalculateOnOpen = (m_formulaUtil.HasExternalReference(formulaRecord.Formula)) ?
      formulaRecord.CalculateOnOpen :
      true;

      SerializeAttribute(writer, CalculateOnOpen, bCalculateOnOpen, false);

      writer.WriteString( strFormula );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes cell value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="record">Record to get value from.</param>
    /// <param name="cellType">Cell type.</param>
    private void SerializeCellValue( XmlWriter writer, BiffRecordRaw record, CellType cellType,string inlineValue )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      switch( record.TypeCode )
      {
        case TBIFFRecord.BoolErr:
          BoolErrRecord boolErrRecord = ( BoolErrRecord )record;
          string strResult;

          if( boolErrRecord.IsErrorCode )
          {
            strResult = ( string )FormulaUtil.ErrorCodeToName[ ( int )boolErrRecord.BoolOrError ];
          }
          else
          {
            strResult = boolErrRecord.BoolOrError.ToString();
          }
          writer.WriteElementString( CellValueTagName, strResult );
          break;

        case TBIFFRecord.Number:
        case TBIFFRecord.RK:
          writer.WriteElementString( CellValueTagName,
            XmlConvert.ToString( ( ( IDoubleValue )record ).DoubleValue ) );
          break;

        case TBIFFRecord.LabelSST:
          if (cellType == CellType.inlineStr)
          {
              writer.WriteStartElement(RichTextInlineTagName);
              writer.WriteElementString(CellDataTypeAttributeName,inlineValue);              
              writer.WriteEndElement();
          }
          else
          {
              writer.WriteElementString(CellValueTagName,
                (record as LabelSSTRecord).SSTIndex.ToString());
          }
          break;

        case TBIFFRecord.Label:
          writer.WriteElementString( CellValueTagName, ( ( LabelRecord )record ).Label );
          break;

        default:
          break;
      }
    }
    /// <summary>
    /// Serializes formula value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="record">Formula record to serialize from.</param>
    /// <param name="cellType">Cell type.</param>
    /// <param name="rowStorageEnumerator">Row storage enumerator.</param>
    private void SerializeFormulaValue( XmlWriter writer, FormulaRecord record, CellType cellType,
      RowStorageEnumerator rowStorageEnumerator )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      if( rowStorageEnumerator == null )
        throw new ArgumentNullException( "rowStorageEnumerator" );

      switch( cellType )
      {
        case CellType.b:
          string strValue = record.BooleanValue ? TrueValue : FalseValue;
          writer.WriteElementString( CellValueTagName, strValue );
          break;

        case CellType.e:
          string strError = ( string )FormulaUtil.ErrorCodeToName[ ( int )record.ErrorValue ];
          writer.WriteElementString( CellValueTagName, strError );
          break;

        case CellType.str:
          writer.WriteElementString( CellValueTagName,
            rowStorageEnumerator.GetFormulaStringValue() );
          break;

        case CellType.n:
          double value = record.DoubleValue;

          if( !Double.IsNaN( value ) )
            writer.WriteElementString( CellValueTagName, XmlConvert.ToString( record.DoubleValue ) );
          break;

        default:
          break;
      }
    }
    /// <summary>
    /// Defines cell data type to serialize.
    /// </summary>
    /// <param name="record">Biff record.</param>
    /// <param name="strCellType">String representation of the cell type.</param>
    /// <returns>Extracted Cell data type.</returns>
    private CellType GetCellDataType( BiffRecordRaw record, out string strCellType )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      CellType result;

      switch( record.TypeCode )
      {
        case TBIFFRecord.BoolErr:
          BoolErrRecord boolErrRecord = ( BoolErrRecord )record;
          if( boolErrRecord.IsErrorCode )
          {
            result = CellType.e;
            strCellType = CellTypeError;
          }
          else
          {
            result = CellType.b;
            strCellType = CellTypeBool;
          }
          break;

        case TBIFFRecord.MulRK:
        case TBIFFRecord.RK:
        case TBIFFRecord.Number:
          result = CellType.n;
          strCellType = CellTypeNumber;
          break;

        case TBIFFRecord.LabelSST:
        case TBIFFRecord.RString:
          result = CellType.s;
          strCellType = CellTypeString;
          break;

        case TBIFFRecord.Label:
          result = CellType.str;
          strCellType = CellTypeFormulaString;
          break;

        case TBIFFRecord.Formula:
          FormulaRecord formulaRecord = ( FormulaRecord )record;
          if( formulaRecord.IsBool )
          {
            result = CellType.b;
            strCellType = CellTypeBool;
          }
          else if( formulaRecord.IsError )
          {
            result = CellType.e;
            strCellType = CellTypeError;
          }
          else if( formulaRecord.HasString )
          {
            result = CellType.str;
            strCellType = CellTypeFormulaString;
          }
          else
          {
            result = CellType.n;
            strCellType = CellTypeNumber;
          }
          break;

        default:
          throw new NotImplementedException( "type" );
      }

      return result;
    }
    /// <summary>
    /// Serializes shared string table into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to save SST into.</param>
    public void SerializeSST( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if (m_book.HasInlineStrings && m_book.SSTStream != null)
      {
          m_book.SSTStream.Position = 0;
          ShapeParser.WriteNodeFromStream(writer, m_book.SSTStream);
      }
      else
      {
          SSTDictionary dictionarySST = m_book.InnerSST;
          //if (!m_book.IsCreated && !m_book.ParseOnDemand)
          //dictionarySST.RemoveUnnecessaryStrings();

          //writer.WriteRaw( XmlFileHeading );
          writer.WriteStartDocument();
          writer.WriteStartElement(SharedStringTableTagName, XmlNamespaceMain);

          int iCount = dictionarySST.Count;

          int LabelSSTCount = dictionarySST.GetLabelSSTCount();
          writer.WriteAttributeString(UniqueStringCountAttributeName, iCount.ToString());
          writer.WriteAttributeString("count", LabelSSTCount.ToString());

          for (int i = 0; i < iCount; i++)
          {
              object objTextOrString = dictionarySST.GetSSTContentByIndex(i);
              SerializeStringItem(writer, objTextOrString);
          }

          writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes string item into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to save string item into.</param>
    /// <param name="objTextOrString">Rich text or string object to serialize.</param>
    private void SerializeStringItem( XmlWriter writer, object objTextOrString )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( objTextOrString == null )
        throw new ArgumentNullException( "text" );

      writer.WriteStartElement( StringItemTagName );

      TextWithFormat text = objTextOrString as TextWithFormat;

      if( text != null && text.FormattingRunsCount > 0 )
      {
        SerializeRichTextRun( writer, text );
      }
      else
      {
        string strText = ( text == null ) ?
          objTextOrString.ToString() :
          text.Text;

        if (!strText.Contains(DEF_DEFAULT_ROW_DELIMITER))
            strText = strText.Replace("\n", DEF_DEFAULT_ROW_DELIMITER);

        int iLength = strText.Length;

        writer.WriteStartElement( TextTagName );

        if( iLength > 0 && ( strText[ 0 ] == ' ' || strText[ iLength - 1 ] == ' ' )|| ( text!=null && text.IsPreserved) )
          writer.WriteAttributeString( XmlPrefix, SpaceAttributeName, null, PreserveValue );

        strText = PrepareString( strText );
        strText = ReplaceWrongChars( strText );
        writer.WriteString( strText );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Replaces characters with codes less then 0x20.
    /// </summary>
    /// <param name="strText">Text to replace chars in.</param>
    /// <returns></returns>
    private string ReplaceWrongChars( string strText )
    {
      StringBuilder result = new StringBuilder();
      int iLength = ( strText == null ) ? 0 : strText.Length;

     foreach(char currentChar in strText.ToCharArray())
      { 
        int iChar = ( int )currentChar;

        if( iChar < FirstVisibleChar && Array.IndexOf( allowedChars, currentChar) < 0 || char.IsSurrogate(currentChar))
        {
          result.Append( string.Format( "_x{0}_", iChar.ToString( "X4" ) ) );
        }
        else
        {
          result.Append( currentChar );
        }
      }

      return result.ToString();
    }
    /// <summary>
    /// Ignores the hexa string with the hexa code before serialize
    /// </summary>
    /// <param name="text">string to ignore the hexa string</param>
    /// <returns>ignored hexa string</returns>
    private string PrepareString( string text )
    {
      const int HexaLength = 4;
      const string IgnoreHexaString = "_x005F";
      const string HexaStart = "_x";
      const string HexaEnd = "_";
      const int PrefixStartLen = 2;
      StringBuilder sb = new StringBuilder( text );
      int indexIncrement = 0;

      for( int i = 0; i < text.Length; )
      {
        int startIndex = text.IndexOf( HexaStart, i );
        
        if( startIndex == -1 )
          break;

        startIndex += PrefixStartLen;
        int endIndex = text.IndexOf( HexaEnd, startIndex );

        if( endIndex == -1 )
          break;

        int length = endIndex - startIndex;
        
        if( length == HexaLength )
        {
          string value = text.Substring( startIndex, 4 );
          
          if( IsHexa( value ) )
          {
            sb.Insert( startIndex - PrefixStartLen + indexIncrement, IgnoreHexaString );
            indexIncrement += IgnoreHexaString.Length;
          }
        }
        i = endIndex;
      }
      return sb.ToString();
    }
    /// <summary>
    /// Checks whether specified string is hexadecimal number.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsHexa( string value )
    {
      int result;

      return int.TryParse( value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result );
    }
    /// <summary>
    /// Serializes rich text run into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to save rich text into.</param>
    /// <param name="text">Rich text run to serialize.</param>
    private void SerializeRichTextRun( XmlWriter writer, TextWithFormat text )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( text == null )
        throw new ArgumentNullException( "text" );

      text.Defragment();

      FontsCollection fonts = m_book.InnerFonts;

      SortedList<int, int> formattingRuns = text.FormattingRuns;
      string strText = text.Text;

      string strToInsert = string.Empty;
      int iCurrentFont = -1;
      int iCountToCut = 0;
      int iCuted = 0;
      int ilength = strText.Length;
      foreach( KeyValuePair<int, int> keyValue in formattingRuns )
      {
        iCountToCut = keyValue.Key - iCuted;
        if (ilength >= iCountToCut)
        strToInsert = strText.Substring( iCuted, iCountToCut );
        SerializeRichTextRunSingleEntry( writer, fonts, strToInsert, iCurrentFont );
        iCurrentFont = keyValue.Value;
        iCuted += iCountToCut;
      }

      if (ilength >= iCuted)
      strToInsert = strText.Substring( iCuted );


      SerializeRichTextRunSingleEntry( writer, fonts, strToInsert, iCurrentFont );
    }
    /// <summary>
    /// Serializes rich text run single entry.
    /// </summary>
    /// <param name="writer">XmlWriter to save rich text into.</param>
    /// <param name="fonts">Fonts collection.</param>
    /// <param name="strString">String to serialize.</param>
    /// <param name="iFontIndex">Font index.</param>
    private void SerializeRichTextRunSingleEntry( XmlWriter writer, FontsCollection fonts, string strString, int iFontIndex )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( fonts == null )
        throw new ArgumentNullException( "fonts" );

      if( strString == null )
        throw new ArgumentNullException( "strString" );

      if (!strString.Contains(DEF_DEFAULT_ROW_DELIMITER))
          strString = strString.Replace("\n", DEF_DEFAULT_ROW_DELIMITER);

      IFont font;

      writer.WriteStartElement( RichTextRunTagName );

      if( iFontIndex != -1 )
      {
        font = fonts[ iFontIndex ];
        SerializeFont( writer, font, RichTextRunPropertiesTagName );
      }

      writer.WriteStartElement( TextTagName );

      int iLength = strString.Length;

      if( iLength > 0 )
      {
        char chLast = strString[ iLength - 1 ];

        if (strString[0] == ' ' || chLast == ' ' || strString.StartsWith(DEF_DEFAULT_ROW_DELIMITER) || strString.EndsWith(DEF_DEFAULT_ROW_DELIMITER) 
            || strString.EndsWith("\t") || strString.StartsWith("\t"))
          writer.WriteAttributeString( XmlPrefix, SpaceAttributeName, null, PreserveValue );
      }

      writer.WriteValue( strString );
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes colors settings. In the current implementation it serializes palette if necessary.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    private void SerializeColors( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( !IsPaletteDefault() )
      {
        writer.WriteStartElement( ColorsTagName );
        SerializePalette( writer );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes palette.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize palette into.</param>
    private void SerializePalette( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( IndexedColorsTagName );
      Color[] arrColors = m_book.Palette;

      for( int i = 0, len = arrColors.Length; i < len; i++ )
      {
        Color color = arrColors[ i ];
        SerializeRgbColor( writer, RgbColorTagName, color );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Determines whether palette contains default colors.
    /// </summary>
    /// <returns>True if all colors have default value; false otherwise.</returns>
    private bool IsPaletteDefault()
    {
      List<Color> arrColors = m_book.InnerPalette;
      Color[] arrDefaultColors = WorkbookImpl.DEF_PALETTE;
      bool bResult = true;

      for( int i = 0, len = arrColors.Count; i < len; i++ )
      {
        Color color1 = arrColors[ i ];
        Color color2 = arrDefaultColors[ i ];

        if( color1.ToArgb() != color2.ToArgb() )
        {
          bResult = false;
          break;
        }
      }

      return bResult;
    }
    /// <summary>
    /// Serializes column settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet that stores column settings.</param>
    /// <param name="dicStyles">Dictionary with modified style indexes.</param>
    private void SerializeColumns( XmlWriter writer, WorksheetImpl sheet, Dictionary<int, int> dicStyles )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( dicStyles == null )
        throw new ArgumentNullException( "dicStyles" );

      ColumnInfoRecord[] arrColumns = sheet.ColumnInformation;
      bool bWriteStart = true;

      double dDefaultWidth = sheet.StandardWidth;

      //if( sheet.FirstColumn != WorksheetImpl.DEF_MIN_COLUMN_INDEX )
      {
        for( int i = 1, iLast = /*sheet.LastColumn*/m_book.MaxColumnCount; i <= iLast; i++ )
        {
          ColumnInfoRecord column = arrColumns[ i ];

          if( column != null )
          {
            if( bWriteStart )
              writer.WriteStartElement( ColsTagName );

            i = SerializeColumn( writer, column, dicStyles, dDefaultWidth, sheet );
            bWriteStart = false;
          }
        }
      }

      if( !bWriteStart )
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize single ColumnInfoRecord into specified XmlWriter.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="columnInfo">Record to serialize. This value can be null,
    /// in this case no data will be serialized.</param>
    /// <param name="dicStyles">Dictionary with modified style indexes.</param>
    /// <param name="defaultWidth">Default column width.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <returns>Last index in the sequence of the same columns.</returns>
    private int SerializeColumn( XmlWriter writer, ColumnInfoRecord columnInfo,
      Dictionary<int, int> dicStyles, double defaultWidth, WorksheetImpl sheet )
    {
      if( columnInfo == null )
        return int.MaxValue;

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dicStyles == null )
        throw new ArgumentNullException( "dicStyles" );

      int iLastColumn = FindSameColumns( sheet, columnInfo.FirstColumn + 1 );

      writer.WriteStartElement( ColTagName );
      writer.WriteAttributeString( ColumnMinAttribute, ( columnInfo.FirstColumn + 1 ).ToString() );
      writer.WriteAttributeString( ColumnMaxAttribute, iLastColumn.ToString() );

      double dWidth = sheet.EvaluateFileColumnWidth( columnInfo.ColumnWidth ) / 256.0;
      if (dWidth > sheet.MaxColumnWidth)
          writer.WriteAttributeString(ColumnWidthAttribute, sheet.MaxColumnWidth.ToString(NumberFormatInfo.InvariantInfo));
        else
          writer.WriteAttributeString(ColumnWidthAttribute, dWidth.ToString(NumberFormatInfo.InvariantInfo));   
     
      if( columnInfo.ExtendedFormatIndex != sheet.ParentWorkbook.DefaultXFIndex )
      {
        int iCurrentStyle = columnInfo.ExtendedFormatIndex;
        int iNewStyle;

        if( !dicStyles.TryGetValue( iCurrentStyle, out iNewStyle ) )
          iNewStyle = iCurrentStyle;

        writer.WriteAttributeString( ColumnStyleAttribute, iNewStyle.ToString() );
      }

      SerializeAttribute( writer, HiddenAttributeName, columnInfo.IsHidden, false );
      SerializeAttribute(writer, BestFitAttribute, columnInfo.IsBestFit, false);
      
      SerializeAttribute(writer, Phonetic, columnInfo.IsPhenotic, false);
      SerializeAttribute(writer, ColumnCustomWidthAttribute, columnInfo.IsUserSet ? true : (defaultWidth != dWidth), false);
      SerializeAttribute( writer, RowColumnCollapsedAttribute, columnInfo.IsCollapsed, false );
      SerializeAttribute( writer, RowColumnOutlineLevelAttribute, columnInfo.OutlineLevel, 0 );
      writer.WriteEndElement();

      return iLastColumn;
    }
    /// <summary>
    /// Checks whether columns after iColumnIndex have the same settings and
    /// returns the last number in the sequence.
    /// </summary>
    /// <param name="sheet">Worksheet to search in.</param>
    /// <param name="iColumnIndex">Column index to start searching from.</param>
    /// <returns>Last index in the sequence of the same columns.</returns>
    private int FindSameColumns( WorksheetImpl sheet, int iColumnIndex )
    {
      ColumnInfoRecord[] arrColumns = sheet.ColumnInformation;
      ColumnInfoRecord currentColumn = arrColumns[ iColumnIndex ];

      while( iColumnIndex < m_book.MaxColumnCount )
      {
        int iCurrentColumn = iColumnIndex + 1;

        ColumnInfoRecord columnToCompare = arrColumns[ iCurrentColumn ];

        if( columnToCompare != null &&
          columnToCompare.ExtendedFormatIndex == currentColumn.ExtendedFormatIndex &&
          columnToCompare.ColumnWidth == currentColumn.ColumnWidth &&
          columnToCompare.IsCollapsed == currentColumn.IsCollapsed &&
          columnToCompare.IsHidden == currentColumn.IsHidden &&
          columnToCompare.OutlineLevel == currentColumn.OutlineLevel )
        {
          iColumnIndex++;
        }
        else
        {
          break;
        }
      }

      return iColumnIndex;
    }
    /// <summary>
    /// Serializes data validations.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize data validations.</param>
    /// <param name="dataValidationTable">Data validation table.</param>
    private void SerializeDataValidations( XmlWriter writer, DataValidationTable dataValidationTable )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataValidationTable == null || dataValidationTable.Count == 0 )
        return;

      for( int i = 0, iCount = dataValidationTable.Count; i < iCount; i++ )
      {
        DataValidationCollection dataValidationCollection = dataValidationTable[ i ];
        SerializeDataValidationCollection( writer, dataValidationCollection );
      }
    }
    /// <summary>
    /// Serializes data validation collection.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize data validation collection.</param>
    /// <param name="dataValidationCollection">Data validation table.</param>
    private void SerializeDataValidationCollection( XmlWriter writer, DataValidationCollection dataValidationCollection )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataValidationCollection == null || dataValidationCollection.Count == 0 )
        return;

      writer.WriteStartElement( DV.DataValidationsTagName );

      int iCount = dataValidationCollection.Count;

      SerializeAttribute( writer, DV.ItemCountAttributeName, iCount, 0 );
      SerializeAttribute( writer, DV.DisablePromptsAttributeName, dataValidationCollection.IsPromptBoxVisible, false );

      if( dataValidationCollection.IsPromptBoxPositionFixed )
      {
        SerializeAttribute( writer, DV.XCoodrinateAttributeName, dataValidationCollection.PromptBoxVPosition, 0 );
        SerializeAttribute( writer, DV.YCoodrinateAttributeName, dataValidationCollection.PromptBoxHPosition, 0 );
      }

      for( int i = 0; i < iCount; i++ )
      {
        SerializeDataValidation( writer, dataValidationCollection[ i ] );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes data validation.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize data validation.</param>
    /// <param name="dataValidation">Data validation implementation.</param>
    private void SerializeDataValidation( XmlWriter writer, DataValidationImpl dataValidation )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataValidation == null )
        throw new ArgumentNullException( "dataValidation" );

      writer.WriteStartElement( DV.DataValidationTagName );

      ExcelDataType dataType = dataValidation.AllowType;

      if( dataType != ExcelDataType.Any )
        writer.WriteAttributeString( DV.TypeAttributeName, GetDVTypeName( dataType ) );

      ExcelErrorStyle errorStyle = dataValidation.ErrorStyle;

      if( errorStyle != ExcelErrorStyle.Stop )
        writer.WriteAttributeString( DV.ErrorStyleAttributeName, GetDVErrorStyleType( errorStyle ) );

      ExcelDataValidationComparisonOperator compareOperator = dataValidation.CompareOperator;

      if( compareOperator != ExcelDataValidationComparisonOperator.Between )
        writer.WriteAttributeString( DV.OperatorAttributeName, GetDVCompareOperatorType( compareOperator ) );

      SerializeAttribute( writer, DV.AllowBlankAttributeName, dataValidation.IsEmptyCellAllowed, false );
      SerializeAttribute( writer, DV.ShowDropDownAttributeName, dataValidation.IsSuppressDropDownArrow, false );
      SerializeAttribute( writer, DV.ShowInputMessageAttributeName, dataValidation.ShowPromptBox, false );
      SerializeAttribute( writer, DV.ShowErrorMessageAttributeName, dataValidation.ShowErrorBox, false );
      SerializeAttribute( writer, DV.ErrorAlertTextAttributeName, dataValidation.ErrorBoxTitle, string.Empty );
      SerializeAttribute( writer, DV.ErrorMessageAttributeName, dataValidation.ErrorBoxText, string.Empty );
      SerializeAttribute( writer, DV.PromptTitleAttributeName, dataValidation.PromptBoxTitle, string.Empty );
      SerializeAttribute( writer, DV.InputPromptAttributeName, dataValidation.PromptBoxText, string.Empty );
      string strSequenceOfReferences = string.Join( " ", dataValidation.DVRanges );
      SerializeAttribute( writer, DV.CFSequenceOfReferencesAttributeName, strSequenceOfReferences, null );

      string strFirstFormula = dataValidation.GetFirstSecondFormula( m_formulaUtil, true );
      string strSecondFormula = dataValidation.GetFirstSecondFormula( m_formulaUtil, false );

      if( strFirstFormula != null && strFirstFormula != string.Empty )
      {
        strFirstFormula = strFirstFormula.Replace( '\0', ',' );
        writer.WriteElementString( DV.FormulaOneTagName, strFirstFormula );
      }

      if( strSecondFormula != null && strSecondFormula != string.Empty )
      {
        strSecondFormula = strSecondFormula.Replace( '\0', ',' );
        writer.WriteElementString( DV.FormulaTwoTagName, strSecondFormula );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Returns DV type name.
    /// </summary>
    /// <param name="dataType">Data validation type.</param>
    /// <returns>DV type name.</returns>
    private string GetDVTypeName( ExcelDataType dataType )
    {
      switch( dataType )
      {
        case ExcelDataType.Any:
          return DV.TypeNone;

        case ExcelDataType.Date:
          return DV.TypeDate;

        case ExcelDataType.Decimal:
          return DV.TypeDecimal;

        case ExcelDataType.Formula:
          return DV.TypeCustom;

        case ExcelDataType.Integer:
          return DV.TypeWhole;

        case ExcelDataType.TextLength:
          return DV.TypeTextLength;

        case ExcelDataType.Time:
          return DV.TypeTime;

        case ExcelDataType.User:
          return DV.TypeList;

        default:
          throw new ArgumentOutOfRangeException( "dataType" );
      }
    }
    /// <summary>
    /// Returns DV error style name.
    /// </summary>
    /// <param name="errorStyle">Data validation error style.</param>
    /// <returns>DV error style.</returns>
    private string GetDVErrorStyleType( ExcelErrorStyle errorStyle )
    {
      switch( errorStyle )
      {
        case ExcelErrorStyle.Info:
          return DV.ErrorStyleInformationIcon;

        case ExcelErrorStyle.Stop:
          return DV.ErrorStyleStopIcon;

        case ExcelErrorStyle.Warning:
          return DV.ErrorStyleWarningIcon;

        default:
          throw new ArgumentOutOfRangeException( "errorStyle" );
      }
    }
    /// <summary>
    /// Returns DV compare operator type name.
    /// </summary>
    /// <param name="compareOperator">Data validation compare operator.</param>
    /// <returns>DV compare operator type name.</returns>
    private string GetDVCompareOperatorType( ExcelDataValidationComparisonOperator compareOperator )
    {
      switch( compareOperator )
      {
        case ExcelDataValidationComparisonOperator.Between:
          return DV.OperatorBetween;

        case ExcelDataValidationComparisonOperator.Equal:
          return DV.OperatorEqual;

        case ExcelDataValidationComparisonOperator.Greater:
          return DV.OperatorGreaterThan;

        case ExcelDataValidationComparisonOperator.GreaterOrEqual:
          return DV.OperatorGreaterThanOrEqual;

        case ExcelDataValidationComparisonOperator.Less:
          return DV.OperatorLessThan;

        case ExcelDataValidationComparisonOperator.LessOrEqual:
          return DV.OperatorLessThanOrEqual;

        case ExcelDataValidationComparisonOperator.NotBetween:
          return DV.OperatorNotBetween;

        case ExcelDataValidationComparisonOperator.NotEqual:
          return DV.OperatorNotEqual;

        default:
          throw new ArgumentOutOfRangeException( "compareOperator" );
      }
    }
    /// <summary>
    /// Serializes auto filters.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize auto filters.</param>
    /// <param name="autoFilters">Auto filters collection.</param>
    public void SerializeAutoFilters( XmlWriter writer, IAutoFilters autoFilters )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( autoFilters == null || autoFilters.Count == 0 )
        return;

      writer.WriteStartElement( AF.AutoFilterSettingsTagName );
      writer.WriteAttributeString( AF.CellOrRangeReferenceAttributeName, autoFilters.FilterRange.AddressLocal );

      for( int i = 0, iCount = autoFilters.Count; i < iCount; i++ )
      {
        AutoFilterImpl autoFilter = ( AutoFilterImpl )autoFilters[ i ];

        if( autoFilter.IsFiltered )
        {
          SerializeFilterColumn( writer, autoFilter );
        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes filter column.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize filter column.</param>
    /// <param name="autoFilter">Auto filter to serialize.</param>
    private void SerializeFilterColumn( XmlWriter writer, AutoFilterImpl autoFilter )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( autoFilter == null )
        throw new ArgumentNullException( "autoFilter" );

      writer.WriteStartElement( AF.AutoFilterColumnTagName );
      SerializeAttribute( writer, AF.FilterColumnDataAttributeName, autoFilter.Index - 1, -1 );

      if( autoFilter.IsTop10 )
      {
        SerializeAutoFilterTopTen( writer, autoFilter );
      }
      else if( !autoFilter.IsSimple1 && !autoFilter.IsSimple2 )
      {
        SerializeCustomFilters( writer, autoFilter );
      }
      else
      {
        string strFirstCondition = null;
        string strSecondCondition = null;

        if( autoFilter.IsFirstCondition )
          strFirstCondition = autoFilter.FirstCondition.String;

        if( autoFilter.IsSecondCondition )
          strSecondCondition = autoFilter.SecondCondition.String;

        SerializeFilters(writer, (AutoFilterConditionImpl)autoFilter.FirstCondition, (AutoFilterConditionImpl)autoFilter.SecondCondition);
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes filters.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize filters.</param>
    /// <param name="strFirstCondition">First condition.</param>
    /// <param name="strSecondCondition">Second condition.</param>
    private void SerializeFilters( XmlWriter writer, AutoFilterConditionImpl firstCondition, AutoFilterConditionImpl secondCondition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( firstCondition.String == null && secondCondition.String == null && 
          firstCondition.DataType != ExcelFilterDataType.MatchAllBlanks && secondCondition.DataType != ExcelFilterDataType.MatchAllBlanks )
        return;

      writer.WriteStartElement( AF.FilterCriteriaTagName );

      if(firstCondition.DataType == ExcelFilterDataType.MatchAllBlanks || secondCondition.DataType == ExcelFilterDataType.MatchAllBlanks )
          writer.WriteAttributeString(AF.FilterBlankAttributeName, "1");

      if( firstCondition.String != null )
          SerializeFilter(writer, firstCondition.String);

      if (secondCondition.String != null)
          SerializeFilter(writer, secondCondition.String);

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes filter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize filter.</param>
    /// <param name="strFilterValue">Condition value.</param>
    private void SerializeFilter( XmlWriter writer, string strFilterValue )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( AF.FilterTagName );
      writer.WriteAttributeString( AF.FilterValueAttributeName, strFilterValue );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes auto filter top ten value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize top ten.</param>
    /// <param name="autoFilter">Auto filter implementation.</param>
    private void SerializeAutoFilterTopTen( XmlWriter writer, AutoFilterImpl autoFilter )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( autoFilter == null )
        throw new ArgumentNullException( "autoFilter" );

      writer.WriteStartElement( AF.AutoFilterTopTenTagName );

      SerializeAttribute( writer, AF.TopAttributeAttributeName, autoFilter.IsTop, true );
      SerializeAttribute( writer, AF.FilterByPercentAttributeName, autoFilter.IsPercent, false );
      SerializeAttribute( writer, AF.TopOrBottomValueAttributeName, autoFilter.Top10Number, -1 );
      SerializeAttribute( writer, AF.FilterValAttributeName, autoFilter.FirstCondition.Double, -1 );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes custom filters.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize custom filters.</param>
    /// <param name="autoFilter">Auto filter implementation.</param>
    private void SerializeCustomFilters( XmlWriter writer, AutoFilterImpl autoFilter )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( autoFilter == null )
        throw new ArgumentNullException( "autoFilter" );

      writer.WriteStartElement( AF.CustomFiltersCriteriaTagName );

      SerializeAttribute( writer, AF.AndCriteriaAttributeName, autoFilter.IsAnd, false );

      if( autoFilter.IsFirstCondition )
        SerializeCustomFilter( writer, autoFilter.FirstCondition );

      if( autoFilter.IsSecondCondition )
        SerializeCustomFilter( writer, autoFilter.SecondCondition );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes custom filter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize custom filters.</param>
    /// <param name="autoFilterCondition">Auto filter condition.</param>
    private void SerializeCustomFilter( XmlWriter writer, IAutoFilterCondition autoFilterCondition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( autoFilterCondition == null )
        throw new ArgumentNullException( "autoFilterCondition" );

      writer.WriteStartElement( AF.CustomFilterCriteriaTagName );

      if (autoFilterCondition.DataType == ExcelFilterDataType.MatchAllNonBlanks)
      {
          SerializeAttribute(writer, AF.FilterComparisonOperatorAttributeName, AF.OperatorNotEqual, AF.OperatorEqual);
          SerializeAttribute(writer, AF.FilterValueAttributeName, " ", null);
      }

      else
      {
          string strOperatorName = GetAFConditionOperatorName(autoFilterCondition.ConditionOperator);
          SerializeAttribute(writer, AF.FilterComparisonOperatorAttributeName, strOperatorName, AF.OperatorEqual);

          string strValue = GetAFFilterValue(autoFilterCondition);
          SerializeAttribute(writer, AF.FilterValueAttributeName, strValue, null);
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Returns auto filter condition operator name.
    /// </summary>
    /// <param name="filterCondition">Filter condition.</param>
    /// <returns>Auto filter condition operator name.</returns>
    private string GetAFConditionOperatorName( ExcelFilterCondition filterCondition )
    {
      switch( filterCondition )
      {
        case ExcelFilterCondition.Equal:
          return AF.OperatorEqual;

        case ExcelFilterCondition.Greater:
          return AF.OperatorGreaterThan;

        case ExcelFilterCondition.GreaterOrEqual:
          return AF.OperatorGreaterThanOrEqual;

        case ExcelFilterCondition.Less:
          return AF.OperatorLessThan;

        case ExcelFilterCondition.LessOrEqual:
          return AF.OperatorLessThanOrEqual;

        case ExcelFilterCondition.NotEqual:
          return AF.OperatorNotEqual;

        default:
          throw new ArgumentOutOfRangeException( "filterCondition" );
      }
    }
    /// <summary>
    /// Gets the active pane.
    /// </summary>
    /// <param name="paneRecord">The pane record.</param>
    /// <returns></returns>
    private ushort GetActivePane(PaneRecord paneRecord)
    {
        if (paneRecord != null)
        {
            if (paneRecord.VerticalSplit == 0 && paneRecord.HorizontalSplit == 0)
            {
                paneRecord.ActivePane = 3;
            }
            else if (paneRecord.VerticalSplit == 0)
            {
                paneRecord.ActivePane = 2;
            }
            else if (paneRecord.HorizontalSplit == 0)
            {
                paneRecord.ActivePane = 1;
            }
        }
        return paneRecord.ActivePane;
    }
    /// <summary>
    /// Returns auto filter condition value.
    /// </summary>
    /// <param name="autoFilterCondition">Filter condition.</param>
    /// <returns>Auto filter condition value.</returns>
    private string GetAFFilterValue( IAutoFilterCondition autoFilterCondition )
    {
      switch( autoFilterCondition.DataType )
      {
        case ExcelFilterDataType.String:
          return autoFilterCondition.String;

        case ExcelFilterDataType.FloatingPoint:
          return autoFilterCondition.Double.ToString();

        case ExcelFilterDataType.Boolean:
          return ( autoFilterCondition.Boolean ) ? TrueValue : FalseValue;

        case ExcelFilterDataType.ErrorCode:
          return ( string )FormulaUtil.ErrorCodeToName[ autoFilterCondition.ErrorCode ];

        default:
          throw new ArgumentOutOfRangeException( "dataType" );
      }
    }

    /// <summary>
    /// Serializes conditional formats and corresponding Dxf styles for this formats. 
    /// </summary>
    /// <param name="writer">XmlWriter to serialize formats into.</param>
    /// <param name="writerDxf">XmlWriter to serialize Dfx styles into.</param>
    /// <param name="conditionalFormats">Conditional formatting.</param>
    /// <param name="iDxfIndex">Current Dxf index.</param>
    /// <param name="iPriority">Currents CF priority.</param>
    private void SerializeCondionalFormats( XmlWriter writer, XmlWriter writerDxf,
      ConditionalFormats conditionalFormats, ref int iDxfIndex, ref int iPriority )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( writerDxf == null )
        throw new ArgumentNullException( "writerDxf" );

      if( conditionalFormats == null )
        throw new ArgumentNullException( "conditionalFormats" );

      int iRulesCount = conditionalFormats.Count;

      if( iRulesCount == 0 )
        return;
      bool serializeCF = false;
      foreach (ConditionalFormatImpl format in conditionalFormats)
      {
          if (format.Range == null)
              format.Range = conditionalFormats.sheet.Range[conditionalFormats.Address];
          if (!format.CFHasExtensionList)
              serializeCF = true ;
      }
      if (!conditionalFormats.IsFutureRecord && serializeCF )
      {
          writer.WriteStartElement(CF.ConditionalFormattingTagName);
          string[] cellList = conditionalFormats.CellsList;

          string strAddress = (cellList.Length>0)?string.Join(" ", cellList) : conditionalFormats.Address.ToString();
          SerializeAttribute(writer, DV.CFSequenceOfReferencesAttributeName, strAddress, null);

          for (int i = 0; i < iRulesCount; i++)
          {
              IInternalConditionalFormat condition = conditionalFormats[i] as IInternalConditionalFormat;
              if(!( condition as ConditionalFormatImpl) .CFHasExtensionList )
              SerializeCondition(writer, writerDxf, condition, ref iDxfIndex, ref iPriority);
          }

          writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes conditional format and corresponding Dxf style for this format. 
    /// </summary>
    /// <param name="writer">XmlWriter to serialize format into.</param>
    /// <param name="writerDxf">XmlWriter to serialize Dfx style into.</param>
    /// <param name="condition">Represents Condition.</param>
    /// <param name="iDxfIndex">Current Dxf index.</param>
    /// <param name="iPriority">Currents CF priority.</param>
    private void SerializeCondition( XmlWriter writer, XmlWriter writerDxf,
      IInternalConditionalFormat condition, ref int iDxfIndex, ref int iPriority )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( writerDxf == null )
        throw new ArgumentNullException( "writerDxf" );

      if( condition == null )
        throw new ArgumentNullException( "condition" );

      ConditionalFormatImpl condFormat = condition as ConditionalFormatImpl;
 
      ExcelCFType cfType = condFormat.FormatType;
      ExcelComparisonOperator comparisonOperator=condFormat.Operator;
      CFTimePeriods cfTimePeriod = condFormat.TimePeriodType;
       
      writer.WriteStartElement( CF.RuleTagName );

      writer.WriteAttributeString( CF.TypeAttributeName, GetCFType( cfType,comparisonOperator ) );

      if (condFormat.IsBackgroundColorPresent || condFormat.IsFontFormatPresent || condFormat.IsBorderFormatPresent || condFormat.IsPatternFormatPresent || condFormat.HasNumberFormatPresent)
      {
        SerializeDxf(writerDxf, condFormat);
        SerializeAttribute( writer, CF.DifferentialFormattingIdAttributeName, iDxfIndex, int.MinValue );
        iDxfIndex++;
      }

      SerializeAttribute(writer, CF.StopIfTrueAttributeName, condition.StopIfTrue, false);

      if( cfType == ExcelCFType.CellValue )
      {
        SerializeAttribute( writer, CF.OperatorAttributeName,
          GetCFComparisonOperatorName( condition.Operator ), string.Empty );
      }
      if (cfType== ExcelCFType.SpecificText)
      {
          SerializeAttribute(writer, CF.OperatorAttributeName,
              GetCFComparisonOperatorName(condition.Operator), string.Empty);

          writer.WriteAttributeString(CF.TextAttributeName, condFormat.Text);
      }
      if (cfType == ExcelCFType.TimePeriod)
      {
          SerializeAttribute(writer, CF.TimePeriodAttributeName,GetCFTimePeriodType(cfTimePeriod),string.Empty);
      }
      ConditionalFormatImpl formatImp = condition as ConditionalFormatImpl;
      if (formatImp != null & formatImp.Priority!=0)
          iPriority = formatImp.Priority;
      SerializeAttribute( writer, CF.PriorityAttributeName, iPriority, int.MinValue );
      iPriority++;

      ConditionalFormatImpl conFormatImpl = ( ConditionalFormatImpl )condition;

      if (conFormatImpl.m_customFunction != string.Empty && condFormat.FormatType==ExcelCFType.Formula)
      {
          writer.WriteElementString(CF.FormulaTagName, conFormatImpl.m_customFunction);
      }
      else
      {

      if( condition.FirstFormula != null && condition.FirstFormula != string.Empty )
      {
        string strFirstFormula = conFormatImpl.GetFirstSecondFormula( m_formulaUtil, true );
        writer.WriteElementString( CF.FormulaTagName, strFirstFormula );
      }

      if( condition.SecondFormula != null && condition.SecondFormula != string.Empty )
      {
        string strSecondFormula = conFormatImpl.GetFirstSecondFormula( m_formulaUtil, false );
        writer.WriteElementString( CF.FormulaTagName, strSecondFormula );
      }
      }

      switch( cfType )
      {
        case ExcelCFType.DataBar:
          SerializeDataBar( writer, condition.DataBar );
          break;

        case ExcelCFType.IconSet:
          SerializeIconSet( writer, condition.IconSet );
          break;

        case ExcelCFType.ColorScale:
          SerializeColorScale( writer, condition.ColorScale );
          break;
      }

      if (condition.DataBar != null && (condition.DataBar as DataBarImpl).HasExtensionList)
      {
          writer.WriteStartElement(Extensionlist, XmlNamespaceMain);
          writer.WriteStartElement(Vml.Ext, XmlNamespaceMain);
          
          writer.WriteAttributeString(SparkConstants.UriAttribute, Excel2010Serializator.DataBarUri);
          writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, X14Prefix, null, X14Namespace);

          writer.WriteElementString(X14Prefix, Excel2010Serializator.IdAttributeName, null, (condition.DataBar as DataBarImpl).ST_GUID);
          
          writer.WriteEndElement();
          writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes color scale of conditional format.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="colorScale">Represents color scale.</param>
    private void SerializeColorScale( XmlWriter writer, IColorScale colorScale )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( colorScale == null )
        throw new ArgumentNullException( "colorScale" );

      writer.WriteStartElement( CF.ColorScaleTag );

      IList<IColorConditionValue> arrConditions = colorScale.Criteria;
      for( int i = 0, len = arrConditions.Count; i < len; i++ )
      {
          SerializeConditionValueObject(writer, arrConditions[i], false);
      }

      for( int i = 0, len = arrConditions.Count; i < len; i++ )
      {
        //SerializeConditionValueObject( writer, arrConditions[ i ] );
        SerializeRgbColor( writer, ColorTagName, arrConditions[ i ].FormatColorRGB );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes icon set.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="iconSet">Icon set to serialize.</param>
    private void SerializeIconSet( XmlWriter writer, IIconSet iconSet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( iconSet == null )
        throw new ArgumentNullException( "iconSet" );

      writer.WriteStartElement( CF.IconSetTag );
      writer.WriteAttributeString( CF.IconSetAttribute, CF.IconSetTypeNames[ ( int )iconSet.IconSet ] );
      SerializeAttribute( writer, CF.PercentAttribute, iconSet.PercentileValues, false );
      SerializeAttribute( writer, CF.ReverseAttribute, iconSet.ReverseOrder, false );
      SerializeAttribute( writer, CF.ShowValueAttribute, !iconSet.ShowIconOnly, true );
      IList<IConditionValue> arrConditions = iconSet.IconCriteria;

      for( int i = 0, len = arrConditions.Count; i < len; i++ )
      {
          SerializeConditionValueObject(writer, arrConditions[i], true);
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes data bar.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dataBar">Data bar to serialize.</param>
    private void SerializeDataBar( XmlWriter writer, IDataBar dataBar )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataBar == null )
        throw new ArgumentNullException( "dataBar" );

      writer.WriteStartElement( CF.DataBarTag );
      SerializeAttribute( writer, CF.MinLengthTag, dataBar.PercentMin, CF.DefaultDataBarMinLength );
      SerializeAttribute( writer, CF.MaxLengthTag, dataBar.PercentMax, CF.DefaultDataBarMaxLength );
      SerializeAttribute( writer, CF.ShowValueAttribute, dataBar.ShowValue, true );

      SerializeConditionValueObject( writer, dataBar.MinPoint,false);
      SerializeConditionValueObject( writer, dataBar.MaxPoint,false );
      SerializeRgbColor( writer, ColorTagName, dataBar.BarColor );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes conditional value object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="conditionValue">Object to serialize.</param>
    public void SerializeConditionValueObject(XmlWriter writer, IConditionValue conditionValue, bool isIconSet)
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( CF.ValueObjectTag );
      int index = ( int )conditionValue.Type;
      string strType = CF.ValueTypes[ index ];
      writer.WriteAttributeString( CF.TypeAttributeName, strType );
      writer.WriteAttributeString( ValueAttributeName, conditionValue.Value );
      if (isIconSet)
          writer.WriteAttributeString(CF.GreaterAttribute, ((int)conditionValue.Operator).ToString());
      writer.WriteEndElement();
    }
    /// <summary>
    /// Returns CF comparison operator string name.
    /// </summary>
    /// <param name="comparisonOperator">Excel comparison operator.</param>
    /// <returns>CF comparison operator string name.</returns>
    internal string GetCFComparisonOperatorName( ExcelComparisonOperator comparisonOperator )
    {
      switch( comparisonOperator )
      {
        case ExcelComparisonOperator.Between:
          return CF.OperatorBetween;
        case ExcelComparisonOperator.BeginsWith:
          return CF.OperatorBeginsWith;
        case ExcelComparisonOperator.ContainsText:
          return CF.OperatorContains;
        case ExcelComparisonOperator.EndsWith:
          return CF.OperatorEndsWith;
        case ExcelComparisonOperator.NotContainsText:
          return CF.OperatorDoesNotContain;
        case ExcelComparisonOperator.Equal:
          return CF.OperatorEqual;

        case ExcelComparisonOperator.Greater:
          return CF.OperatorGreaterThan;

        case ExcelComparisonOperator.GreaterOrEqual:
          return CF.OperatorGreaterThanOrEqual;

        case ExcelComparisonOperator.Less:
          return CF.OperatorLessThan;

        case ExcelComparisonOperator.LessOrEqual:
          return CF.OperatorLessThanOrEqual;

        case ExcelComparisonOperator.None:
          return CF.OperatorDoesNotContain;

        case ExcelComparisonOperator.NotBetween:
          return CF.OperatorNotBetween;

        case ExcelComparisonOperator.NotEqual:
          return CF.OperatorNotEqual;

        default:
          throw new ArgumentOutOfRangeException( "filterCondition" );
      }
    }
    /// <summary>
    /// Return the CF time period string name
    /// </summary>
    /// <param name="timePeriod">Time period type</param>
    /// <returns>Time period string name</returns>
    internal string GetCFTimePeriodType(CFTimePeriods timePeriod)
    {
        switch (timePeriod)
        {
            case CFTimePeriods.Today:
                return CF.TimePeriodToday;

            case CFTimePeriods.Yesterday:
                return CF.TimePeriodYesterday;

            case CFTimePeriods.Tomorrow:
                return CF.TimePeriodTomorrow;

            case CFTimePeriods.Last7Days:
                return CF.TimePeriodLastsevenDays;

            case CFTimePeriods.LastWeek:
                return CF.TimePeriodLastWeek;

            case CFTimePeriods.ThisWeek:
                return CF.TimePeriodThisWeek;

            case CFTimePeriods.NextWeek:
                return CF.TimePeriodNextWeek;

            case CFTimePeriods.LastMonth:
                return CF.TimePeriodLastMonth;

            case CFTimePeriods.ThisMonth:
                return CF.TimePeriodThisMonth;

            case CFTimePeriods.NextMonth:
                return CF.TimePeriodNextMonth;

            default:
                throw new ArgumentOutOfRangeException("timePeriod");
        }
    }
    /// <summary>
    /// Returns CF type string name.
    /// </summary>
    /// <param name="typeCF">Excel CF type.</param>
    /// <returns>CF type string name.</returns>
    internal string GetCFType( ExcelCFType typeCF ,ExcelComparisonOperator compOperator)
    {
      switch( typeCF )
      {
        case ExcelCFType.CellValue:
          return CF.TypeCellIs;

        case ExcelCFType.SpecificText:
          switch (compOperator)
          {
              case ExcelComparisonOperator.BeginsWith:
                  return CF.OperatorBeginsWith;
              case ExcelComparisonOperator.ContainsText:
                  return CF.OperatorContains;
              case ExcelComparisonOperator.EndsWith:
                  return CF.OperatorEndsWith;
              case ExcelComparisonOperator.NotContainsText:
                  return CF.OperatorDoesNotContain;
              default:
                  throw new ArgumentException("ComOperator");
          }                 
        case ExcelCFType.Formula:
          return CF.TypeExpression;

        case ExcelCFType.DataBar:
          return CF.TypeDataBar;

        case ExcelCFType.IconSet:
          return CF.TypeIconSet;

        case ExcelCFType.ColorScale:
          return CF.TypeColorScale;

        case ExcelCFType.Blank:
          return CF.TypeContainsBlank;

        case ExcelCFType.NoBlank:
          return CF.TypeNotContainsBlank;

        case ExcelCFType.ContainsErrors:
          return CF.TypeContainsError;

        case ExcelCFType .NotContainsErrors:
          return CF.TypeNotContainsError;

        case ExcelCFType.TimePeriod:
          return CF.TimePeriodTypeName;

        default:
          throw new ArgumentOutOfRangeException( "typeCF" );
      }
    }
    /// <summary>
    /// Serializes Dxf styles and conditional formatting.
    /// </summary>
    /// <param name="streamDxfs">Dfx stream.</param>
    /// <param name="conditionalFormats">Conditional formatting.</param>
    /// <returns>Stream containing conditional formats.</returns>
    public Stream SerializeDxfs( ref Stream streamDxfs, WorksheetConditionalFormats conditionalFormats ,ref int iDxfIndex)
    {
      if( conditionalFormats == null )
        throw new ArgumentNullException( "conditionalFormats" );

      int iCount = conditionalFormats.Count;

      if( iCount == 0 )
        return null;

      int iParsedDxfsCount = m_book.BookCFPriorityCount;

      MemoryStream streamNewDxfs = new MemoryStream();
      StreamWriter streamWriterDxfs = new StreamWriter( streamNewDxfs );
      XmlWriter xmlWriterDxfs = UtilityMethods.CreateWriter( streamWriterDxfs );

      Stream streamSheetCF = new MemoryStream();
      StreamWriter streamWriterSheetCF = new StreamWriter( streamSheetCF );
      XmlWriter xmlWriterSheetCF = UtilityMethods.CreateWriter( streamWriterSheetCF );

      xmlWriterDxfs.WriteStartElement( TemporaryRoot, XmlNamespaceMain );
      xmlWriterDxfs.WriteStartElement( DiffXFsTagName );

      SerializeStream( xmlWriterDxfs, streamDxfs, DiffXFsTagName );

      xmlWriterSheetCF.WriteStartElement( TemporaryRoot, XmlNamespaceMain );
      int iPriority = 1;

      if (iParsedDxfsCount != int.MinValue && iDxfIndex < iParsedDxfsCount)
      {

          m_book.BookCFPriorityCount = m_book.BookCFPriorityCount + 1;
          iDxfIndex = m_book.BookCFPriorityCount;
          iPriority = m_book.BookCFPriorityCount;

        //iDxfIndex = iParsedDxfsCount;
        //iPriority = iParsedDxfsCount + 1;
      }

      for( int i = 0; i < iCount; i++ )
      {
        ConditionalFormats formats = conditionalFormats[ i ];
        SerializeCondionalFormats( xmlWriterSheetCF, xmlWriterDxfs, formats, ref iDxfIndex, ref iPriority );
      }
      xmlWriterSheetCF.WriteEndElement();
      xmlWriterSheetCF.Flush();

      xmlWriterDxfs.WriteEndElement();
      xmlWriterDxfs.WriteEndElement();
      xmlWriterDxfs.Flush();
      m_book.BookCFPriorityCount = iDxfIndex;
      streamDxfs = streamNewDxfs;

      return streamSheetCF;
    }
    /// <summary>
    /// Serializes Dxf style.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize style into.</param>
    /// <param name="condition">Conditional format.</param>
    private void SerializeDxf(XmlWriter writer, ConditionalFormatImpl condition)
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( condition == null )
        throw new ArgumentNullException( "condition" );

      writer.WriteStartElement( DxfFormattingTagName );

      SerializeDxfFont( writer, condition );
      SerializeDxfNumberFormat(writer, condition);
      SerializeDxfFill( writer, condition );
      SerializeDxfBorders( writer, condition );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes Dfx number format.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize number format</param>
    /// <param name="condition">Represents Condition.</param>
    private void SerializeDxfNumberFormat(XmlWriter writer, ConditionalFormatImpl condition)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (condition == null)
            throw new ArgumentNullException("condition");

        if (!condition.HasNumberFormatPresent)
            return;

        FormatImpl recordImpl = null;
        FormatRecord record = null;

        if (m_book.InnerFormats.ContainsFormat(condition.NumberFormat))
        {
            recordImpl = m_book.InnerFormats[condition.NumberFormatIndex];
        }
        else
        {
            if (m_book.InnerFormats.GetUsedFormats(ExcelVersion.Excel2007).Count > 0)
            {
                List<FormatRecord> arrFormats = m_book.InnerFormats.GetUsedFormats(ExcelVersion.Excel2007);
                
                for (int i = 0; i < arrFormats.Count; i++)
                {
                    if (arrFormats[i].FormatString == condition.NumberFormat)
                    {
                        record = arrFormats[i];
                        break;
                    }
                }
            }
        }
        if (recordImpl == null && record == null) return;

        if (recordImpl != null)
            SerializeNumberFormat(writer, recordImpl.Record);
        else
            SerializeNumberFormat(writer, record);
    }
    /// <summary>
    /// Serializes Dfx style borders.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize borders into.</param>
    /// <param name="condition">Represents Condition.</param>
    internal void SerializeDxfBorders( XmlWriter writer, IInternalConditionalFormat condition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( condition == null )
        throw new ArgumentNullException( "condition" );

      if( !condition.IsBorderFormatPresent )
        return;

      writer.WriteStartElement( BordersCollectionTagName );

      SerializeDxfBorder( writer, Excel2007BorderIndex.left, condition.LeftBorderStyle, condition.LeftBorderColorObject );
      SerializeDxfBorder( writer, Excel2007BorderIndex.right, condition.RightBorderStyle, condition.RightBorderColorObject );
      SerializeDxfBorder( writer, Excel2007BorderIndex.top, condition.TopBorderStyle, condition.TopBorderColorObject );
      SerializeDxfBorder( writer, Excel2007BorderIndex.bottom, condition.BottomBorderStyle, condition.BottomBorderColorObject );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes Dxf border.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize border into.</param>
    /// <param name="borderIndex">Border index.</param>
    /// <param name="lineStyle">Line style.</param>
    /// <param name="color">Represents Color.</param>
    private void SerializeDxfBorder( XmlWriter writer, Excel2007BorderIndex borderIndex,
      ExcelLineStyle lineStyle, ColorObject color )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( lineStyle != ExcelLineStyle.None )
      {
        writer.WriteStartElement( borderIndex.ToString() );

        string strLineStyle = lineStyle.ToString();
        strLineStyle = LowerFirstLetter( strLineStyle );

        writer.WriteAttributeString( BorderStyleAttributeName, strLineStyle );
        SerializeColorObject( writer, BorderColorTagName, color );

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes Dxf style fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize fill into.</param>
    /// <param name="condition">Represents Condition.</param>
    internal void SerializeDxfFill( XmlWriter writer, IInternalConditionalFormat condition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( condition == null )
        throw new ArgumentNullException( "condition" );

      if( !condition.IsPatternFormatPresent )
        return;

      writer.WriteStartElement( FillTagName );
      writer.WriteStartElement( PatternFillTagName );

      if( condition.FillPattern != ExcelPattern.None )
        writer.WriteAttributeString( PatternAttributeName, ConvertPatternToString( condition.FillPattern ) );

      ColorObject color = condition.ColorObject;
      ColorObject backColor = condition.BackColorObject;

      //if( condition.FillPattern != ExcelPattern.Solid )
      //{
      //  ColorObject temp = color;
      //  color = backColor;
      //  backColor = temp;
      //}

      if( color.ColorType != ColorType.Indexed || color.Value != 65 )
        SerializeColorObject( writer, ForegroundColorTagName, color );

      SerializeColorObject( writer, BackgroundColorTagName, backColor );

      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes Dxf style font.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize font into.</param>
    /// <param name="condition">Represents Condition.</param>
    internal void SerializeDxfFont( XmlWriter writer, IInternalConditionalFormat condition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( condition == null )
        throw new ArgumentNullException( "condition" );

      if( !condition.IsFontFormatPresent )
        return;

      writer.WriteStartElement( FontTagName );

      if( condition.IsBold )
        writer.WriteElementString( FontBoldTagName, string.Empty );

      if( condition.IsItalic )
        writer.WriteElementString( FontItalicTagName, string.Empty );

      ExcelUnderline underline = condition.Underline;

      if( underline != ExcelUnderline.None )
      {
        writer.WriteStartElement( FontUnderlineTagName );

        if( !Enum.IsDefined( typeof( ExcelUnderline ), underline ) )
          underline = ExcelUnderline.Single;

        string strUnderline = underline.ToString();
        strUnderline = LowerFirstLetter( strUnderline );
        writer.WriteAttributeString( ValueAttributeName, strUnderline );
        writer.WriteEndElement();
      }

      if( condition.IsStrikeThrough )
        writer.WriteElementString( FontStrikeTagName, string.Empty );

      if( ( uint )condition.FontColor != CFRecord.DefaultColorIndex )
        SerializeColorObject( writer, ColorTagName, condition.FontColorObject );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes hyperlinks.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize hyperlinks.</param>
    /// <param name="sheet">Worksheet with hyperlinks collection.</param>
    private void SerializeHyperlinks( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      HyperLinksCollection hyperlinks = sheet.InnerHyperLinksOrNull;

      if( hyperlinks == null )
        return;

      RelationCollection relations = sheet.DataHolder.Relations;

      int iCount = hyperlinks.Count;

      if( iCount == 0 )
        return;

      writer.WriteStartElement( HyperlinksTagName );

      for( int i = 0; i < iCount; i++ )
      {
        SerializeHyperlink( writer, ( HyperLinkImpl )hyperlinks[ i ], relations );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single hyperlink.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize hyperlink into.</param>
    /// <param name="hyperlink">Hyperlink to serialize.</param>
    /// <param name="relations">Relations collection.</param>
    private void SerializeHyperlink( XmlWriter writer, HyperLinkImpl hyperlink, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( hyperlink == null )
        throw new ArgumentNullException( "hyperlink" );

      writer.WriteStartElement( HyperlinkTagName );
      SerializeAttribute( writer, HyperlinkReferenceAttributeName, hyperlink.Range.AddressLocal, string.Empty );

      if( hyperlink.Type == ExcelHyperLinkType.Workbook )
      {     
        string value=string.Empty;
        if (hyperlink.Address.EndsWith("\0"))
            value = hyperlink.Address.Replace("\0", string.Empty);
        else
            value = hyperlink.Address;
        SerializeAttribute(writer, LocationAttributeName, value.Trim(), string.Empty);
      }
      else
      {
        string strRelationId = relations.GenerateRelationId();

        string strAddress = hyperlink.Address;

        if( ( hyperlink.Type == ExcelHyperLinkType.File && !strAddress.StartsWith( ".." ) && strAddress.Contains( @":\" ) && !strAddress .StartsWith  (FileHyperlinkStartString ))
          || hyperlink.Type == ExcelHyperLinkType.Unc )
        {
          strAddress = FileHyperlinkStartString + strAddress;
        }

        strAddress = ConvertAddressString( strAddress );

        if( strAddress != null )
        {
          Relation relation = new Relation( strAddress, HyperlinkNamespace, true );
          relations.Add( relation );
          writer.WriteAttributeString( RelationPrefix, RelationshipIdAttributeName, null, strRelationId );
          SerializeAttribute( writer, LocationAttributeName, hyperlink.SubAddress, string.Empty );
        }
      }

      SerializeAttribute( writer, ToolTipAttributeName, hyperlink.ScreenTip, null );
      SerializeAttribute( writer, DisplayStringAttributeName, hyperlink.TextToDisplay, string.Empty );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes all print settings (margins, header, footer, etc.).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to get values from.</param>
    public static void SerializePrintSettings( XmlWriter writer, IPageSetupBase pageSetup,
      IPageSetupConstantsProvider constants,bool isChartSettings )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      //PageSetupBaseImpl pageSetup = ( PageSetupBaseImpl )sheet.PageSetup;
      if (isChartSettings)
      {
          SerializePrintOptions(writer, pageSetup, constants);
          SerializeHeaderFooter(writer, pageSetup, constants);
          SerializePageMargins(writer, pageSetup, constants);
          SerializePageSetup(writer, pageSetup, constants);
      }
      else
      {
          SerializePrintOptions(writer, pageSetup, constants);
          SerializePageMargins(writer, pageSetup, constants);
          SerializePageSetup(writer, pageSetup, constants);
          SerializeHeaderFooter(writer, pageSetup, constants);
          
          
      }
    }
    /// <summary>
    /// Serializes print options.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize print options into.</param>
    /// <param name="pageSetup">PageSetup object to get print options from.</param>
    private static void SerializePrintOptions( XmlWriter writer, IPageSetupBase pageSetup,
      IPageSetupConstantsProvider constants )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      IPageSetup sheetSetup = pageSetup as IPageSetup;

      bool bPrintGridlines = ( sheetSetup != null && sheetSetup.PrintGridlines );
      bool bPrintHeadings = ( sheetSetup != null && sheetSetup.PrintHeadings );

      if( bPrintGridlines || bPrintHeadings || pageSetup.CenterHorizontally || pageSetup.CenterVertically )
      {
        writer.WriteStartElement( PageSetup.PrintOptionsTag, constants.Namespace );
        // TODO: find out when we have to serialize GridLinesSet property and whether we have any corresponding value.
        SerializeAttribute( writer, PageSetup.GridLines, bPrintGridlines, false );
        SerializeAttribute( writer, PageSetup.Headings, bPrintHeadings, false );
        SerializeAttribute( writer, PageSetup.HorizontalCentered, pageSetup.CenterHorizontally, false );
        SerializeAttribute( writer, PageSetup.VerticalCentered, pageSetup.CenterVertically, false );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes page margins.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="pageSetup">Object that stores all necessary margin values.</param>
    /// <param name="constants">Object that provides xml tag/attribute names used
    /// for page setup serialization.</param>
    public static void SerializePageMargins( XmlWriter writer, IPageSetupBase pageSetup,
      IPageSetupConstantsProvider constants )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      if( constants == null )
        throw new ArgumentNullException( "constants" );
       
      ValidatePageMargins(pageSetup as PageSetupBaseImpl );
      writer.WriteStartElement( constants.PageMarginsTag, constants.Namespace );
      SerializeAttribute( writer, constants.LeftMargin, pageSetup.LeftMargin, double.MinValue );
      SerializeAttribute( writer, constants.RightMargin, pageSetup.RightMargin, double.MinValue );
      SerializeAttribute( writer, constants.TopMargin, pageSetup.TopMargin, double.MinValue );
      SerializeAttribute( writer, constants.BottomMargin, pageSetup.BottomMargin, double.MinValue );
      SerializeAttribute( writer, constants.HeaderMargin, pageSetup.HeaderMargin, double.MinValue );
      SerializeAttribute( writer, constants.FooterMargin, pageSetup.FooterMargin, double.MinValue );
      writer.WriteEndElement();
    }

      /// <summary>
      /// Validate whether Page margins are fit into the Page
      /// </summary>
      /// <param name="pageSetup"></param>
    private static void ValidatePageMargins(PageSetupBaseImpl pageSetup)
    {
        if (pageSetup.dictPaperHeight.ContainsKey(pageSetup.PaperSize) && pageSetup.dictPaperWidth.ContainsKey(pageSetup.PaperSize))
        {
            double maxWidth = pageSetup.dictPaperWidth [pageSetup.PaperSize];
            double maxHeight= pageSetup.dictPaperHeight[pageSetup.PaperSize];
            if (pageSetup.LeftMargin + pageSetup.RightMargin > maxWidth)
                throw new ArgumentException("Left Margin and Right Margin size exceeds the allowed size");
            if (pageSetup.TopMargin+ pageSetup.BottomMargin > maxHeight )
                throw new ArgumentException("Top Margin and Bottom Margin size exceeds the allowed size");
        }
    }
    /// <summary>
    /// Serialize PageSetup tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="pageSetup">Object to get necessary settings from.</param>
    public static void SerializePageSetup( XmlWriter writer, IPageSetupBase pageSetup,
      IPageSetupConstantsProvider constants )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      if( ( pageSetup as PageSetupBaseImpl ).IsNotValidSettings )
        return;

      writer.WriteStartElement( PageSetup.PageSetupTag, constants.Namespace );
      SerializeAttribute( writer, PageSetup.PaperSize, ( int )pageSetup.PaperSize, 1 );
      if(pageSetup.Zoom!=0)
      SerializeAttribute( writer, PageSetup.Scale, pageSetup.Zoom, 100 );
      SerializeAttribute( writer, PageSetup.FirstPageNumber, (uint)pageSetup.FirstPageNumber, 1 );

      PageSetupImpl worksheetSetup = pageSetup as PageSetupImpl;

      if( worksheetSetup != null )
      {
        SerializeAttribute( writer, PageSetup.FitToWidth, worksheetSetup.FitToPagesWide, 1 );
        SerializeAttribute( writer, PageSetup.FitToHeight, worksheetSetup.FitToPagesTall, 1 );
      }

      if (pageSetup.Order.ToString() != ExcelOrder.DownThenOver.ToString())      
      writer.WriteAttributeString(PageSetup.PageOrder,LowerFirstLetter(pageSetup.Order.ToString()));
      SerializeAttribute( writer, PageSetup.Orientation, pageSetup.Orientation, ( ExcelPageOrientation )0 );
      //SerializeAttribute( writer, PageSetup.UsePrinterDefaults, pageSetup.IsNotValidSettings, true );
      SerializeAttribute( writer, PageSetup.BlackAndWhite, pageSetup.BlackAndWhite, false );
      SerializeAttribute( writer, PageSetup.Draft, pageSetup.Draft, false );

      string strComments = PrintCommentsToString( pageSetup.PrintComments );
      SerializeAttribute( writer, PageSetup.CellComments, strComments, PageSetup.CommentNone );

      SerializeAttribute( writer, PageSetup.UseFirstPageNumber, !pageSetup.AutoFirstPageNumber, false );

      string strErrors = PrintErrorsToString( pageSetup.PrintErrors );
      SerializeAttribute( writer, PageSetup.Errors, strErrors, PageSetup.ErrorsDisplayed );

      PageSetupBaseImpl pageSetupBase = ( PageSetupBaseImpl )pageSetup;
      if (pageSetupBase.HResolution > 0)
      SerializeAttribute( writer, PageSetup.HorizontalDpi, pageSetupBase.HResolution, 600 );
      if (pageSetupBase.VResolution > 0)
      SerializeAttribute( writer, PageSetup.VerticalDpi, pageSetupBase.VResolution, 600 );

      if( !pageSetupBase.IsNotValidSettings )
        SerializeAttribute( writer, PageSetup.Copies, pageSetup.Copies, 1 );

      if( worksheetSetup != null )
      {
        string strRelationId = worksheetSetup.RelationId;

        if( strRelationId != null )
          writer.WriteAttributeString( PageSetup.Id, strRelationId );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes header and footer.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="pageSetup">Object that stores required header/footer strings.</param>
    internal static void SerializeHeaderFooter( XmlWriter writer, IPageSetupBase pageSetup,
      IPageSetupConstantsProvider constants )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      PageSetupBaseImpl pageSetupBase = ( PageSetupBaseImpl )pageSetup;
      string strHeaderString = pageSetupBase.FullHeaderString;
      string strFooterString = pageSetupBase.FullFooterString;

      if( strHeaderString != null && strHeaderString.Length > 0 ||
        strFooterString != null && strFooterString.Length > 0 )
      {
        writer.WriteStartElement( PageSetup.HeaderFooterTag, constants.Namespace );

        SerializeBool(writer, PageSetup.ScaleWithDocTag, pageSetupBase.HFScaleWithDoc);
        SerializeBool(writer, PageSetup.AlignWithMarginsTag, pageSetupBase.AlignHFWithPageMargins);
        SerializeBool(writer, PageSetup.DifferentFirst, pageSetupBase.DifferentFirstPageHF);
        SerializeBool(writer, PageSetup.DifferentOddEvenTag, pageSetupBase.DifferentOddAndEvenPagesHF);

        writer.WriteElementString( PageSetup.OddHeaderTag, constants.Namespace, strHeaderString );
        writer.WriteElementString( PageSetup.OddFooterTag, constants.Namespace, strFooterString );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Converts page setup PrintComments option into string representation used by Excel 2007.
    /// </summary>
    /// <param name="printLocation">Value to convert.</param>
    /// <returns>Converted value.</returns>
    private static string PrintCommentsToString( ExcelPrintLocation printLocation )
    {
      string strResult = null;

      switch( printLocation )
      {
        case ExcelPrintLocation.PrintInPlace:
          strResult = PageSetup.CommentAsDisplayed;
          break;

        case ExcelPrintLocation.PrintNoComments:
          strResult = PageSetup.CommentNone;
          break;

        case ExcelPrintLocation.PrintSheetEnd:
          strResult = PageSetup.CommentAtEnd;
          break;

        default:
          throw new ArgumentOutOfRangeException( "printLocation" );
      }

      return strResult;
    }
    /// <summary>
    /// Converts page setup PrintErrors option into string representation used by Excel 2007.
    /// </summary>
    /// <param name="printErrors">Value to convert.</param>
    /// <returns>Converted value.</returns>
    private static string PrintErrorsToString( ExcelPrintErrors printErrors )
    {
      string strResult = null;

      switch( printErrors )
      {
        case ExcelPrintErrors.PrintErrorsBlank:
          strResult = PageSetup.ErrorsBlank;
          break;

        case ExcelPrintErrors.PrintErrorsDash:
          strResult = PageSetup.ErrorsDash;
          break;

        case ExcelPrintErrors.PrintErrorsDisplayed:
          strResult = PageSetup.ErrorsDisplayed;
          break;

        case ExcelPrintErrors.PrintErrorsNA:
          strResult = PageSetup.ErrorsNA;
          break;

        default:
          throw new ArgumentOutOfRangeException( "printLocation" );
      }

      return strResult;
    }
    /// <summary>
    /// Converts address string to Excel 2007 type.
    /// </summary>
    /// <param name="strAdress">Address string to convert.</param>
    /// <returns>Converted string.</returns>
    private string ConvertAddressString( string strAdress )
    {
      return ( strAdress != null ) ?
        strAdress.Replace( " ", "%20" ) :
        null;
    }
    /// <summary>
    /// Serializes worksheet properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize properties into.</param>
    /// <param name="sheet">Worksheet to get properties from.</param>
    private void SerializeSheetlevelProperties( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      writer.WriteStartElement( SheetLevelPropertiesTagName );

      SerializeAttribute( writer, TransitionEvaluation, sheet.IsTransitionEvaluation, false );
      ExcelKnownColors tabColor = sheet.TabColor;

      string codeName = sheet.CodeName;

      if( m_book.HasMacros && codeName != null && codeName.Length > 0 )
        writer.WriteAttributeString( CodeName, codeName );

      if( tabColor != WorksheetBaseImpl.DEF_DEFAULT_TAB_COLOR )
      {
        writer.WriteStartElement( SheetTabColorTagName );
#if !(SILVERLIGHT || WINRT || WP)
        if (sheet.HasTabColorRGB)
            writer.WriteAttributeString(ColorRgbAttribute, sheet.TabColorRGB.Name.ToString());
        else
#endif
            writer.WriteAttributeString( ColorIndexedAttributeName, ( ( int )tabColor ).ToString() );
        writer.WriteEndElement();
      }

      IPageSetup pageSetup = sheet.PageSetup;

      if( !pageSetup.IsSummaryColumnRight || !pageSetup.IsSummaryRowBelow )
      {
        writer.WriteStartElement( SheetOutlinePropertiesTagName );

        SerializeAttribute( writer, SummaryColumnRight, pageSetup.IsSummaryColumnRight, true );
        SerializeAttribute( writer, SummaryRowBelow, pageSetup.IsSummaryRowBelow, true );

        writer.WriteEndElement();
      }

      if( pageSetup.IsFitToPage )
      {
        writer.WriteStartElement( PageSetupPropertiesTag );
        SerializeAttribute( writer, FitToPageAttribute, pageSetup.IsFitToPage, false );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes background image.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize image into.</param>
    /// <param name="sheet">Worksheet to get image from.</param>
    private void SerilizeBackgroundImage( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );
#if !SILVERLIGHT && !WINRT && !WP
      Bitmap image = sheet.PageSetup.BackgoundImage;

      if( image == null )
        return;

      WorksheetDataHolder sheetDataHolder = sheet.DataHolder;
      FileDataHolder fileDataHolder = sheetDataHolder.ParentHolder;

      string strExtension;
      string strContentType = FileDataHolder.GetPictureContentType( image.RawFormat, out strExtension );
      fileDataHolder.DefaultContentTypes[ strExtension ] = strContentType;

      string strItemName = fileDataHolder.SaveImage( image, null );

      RelationCollection relations = sheetDataHolder.Relations;
      string strRelationId = relations.GenerateRelationId();
      relations[ strRelationId ] = new Relation( '/' + strItemName, RelationTypes.Image );

      writer.WriteStartElement( BackgroundImageTagName );
      writer.WriteAttributeString( RelationPrefix, RelationshipIdAttributeName, null, strRelationId );
      writer.WriteEndElement();
#endif
    }

    /// <summary>
    /// Serializes extended document properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize extended properties into.</param>
    public void SerializeExtendedProperties( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartDocument( true );
      writer.WriteStartElement( DocProp.ApplicationSpecificFilePropertiesTagName, ExtendedPropertiesPartType );

      IBuiltInDocumentProperties builtInProperties = m_book.BuiltInDocumentProperties;
      builtInProperties.ApplicationName = DocProp.EssentialXlsIO;
      SerializeElementString( writer, DocProp.ApplicationNameTagName, builtInProperties.ApplicationName, null );
      SerializeElementString( writer, DocProp.TotalNumberOfCharacters, builtInProperties.CharCount, int.MinValue );
      SerializeElementString( writer, DocProp.NameOfCompanyTagName, builtInProperties.Company, null );
      SerializeElementString( writer, DocProp.NumberOfLinesTagName, builtInProperties.LineCount, int.MinValue );
      SerializeElementString( writer, DocProp.NameOfManagerTagName, builtInProperties.Manager, null );
      SerializeElementString( writer, DocProp.TotalNumberOfMultimediaClipsTagName,
        builtInProperties.MultimediaClipCount, int.MinValue );
      SerializeElementString( writer, DocProp.NumberOfSlidesContainingNotesTagName,
        builtInProperties.SlideCount, int.MinValue );
      SerializeElementString( writer, DocProp.TotalNumberOfPagesTagName, builtInProperties.PageCount, int.MinValue );
      SerializeElementString( writer, DocProp.TotalNumberOfParagraphsTagName, builtInProperties.ParagraphCount, int.MinValue );
      SerializeElementString( writer, DocProp.IntendedFormatOfPresentationTagName, builtInProperties.PresentationTarget, null );
      SerializeElementString( writer, DocProp.NameOfDocumentTemplateTagName, builtInProperties.Template, null );
      TimeSpan editTime = builtInProperties.EditTime;

      if( editTime != TimeSpan.MinValue )
      {
        int iTotalMinutes = ( int )editTime.TotalMinutes;
        writer.WriteElementString( DocProp.TotalEditTimeMetadataElementTagName, iTotalMinutes.ToString() );
      }

      SerializeElementString( writer, DocProp.WordCountTagName, builtInProperties.WordCount, int.MinValue );

      CustomDocumentProperties customProperties = ( CustomDocumentProperties )m_book.CustomDocumentProperties;
      DocumentPropertyImpl property = customProperties.GetProperty( DocProp.RelativeHyperlinkExcel97Name ) as DocumentPropertyImpl;

      if( property != null )
      {
        byte[] blobData = property.Blob;
        string strHyperlinkBase = Encoding.Unicode.GetString( blobData, 0, blobData.Length );
        strHyperlinkBase = strHyperlinkBase.Remove( strHyperlinkBase.Length - 1 );
        writer.WriteElementString( DocProp.RelativeHyperlinkBaseTagName, strHyperlinkBase );
      }

      SerializeAppVersion( writer );

      writer.WriteEndElement();
    }

    protected virtual void SerializeAppVersion( XmlWriter writer )
    {
      SerializeElementString( writer, DocProp.AppVersion, VersionValue, null );
      //throw new NotImplementedException();
    }
    /// <summary>
    /// Serializes core properties into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize core properties into.</param>
    public void SerializeCoreProperties( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      IBuiltInDocumentProperties builtInProperties = m_book.BuiltInDocumentProperties;

      writer.WriteStartDocument( true );
      writer.WriteStartElement( CorePropertiesPrefix, DocProp.CorePropertiesTagName, CorePropertiesPartType );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, DublinCorePrefix,
        null, DublinCorePartType );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, DublinCoreTermsPrefix,
        null, DublinCoreTermsPartType );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, DCMITypePrefix,
        null, DCMITypePartType );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, XSIPrefix,
        null, XSIPartType );

      SerializeElementString( writer, DocProp.CategoryTagName, builtInProperties.Category, null, CorePropertiesPrefix );
      SerializeElementString( writer, DocProp.CreatorTagName, builtInProperties.Author, null, DublinCorePrefix );
      SerializeElementString( writer, DocProp.DescriptionTagName, builtInProperties.Comments, null, DublinCorePrefix );
      SerializeElementString( writer, DocProp.KeywordsTagName, builtInProperties.Keywords, null, CorePropertiesPrefix );
      SerializeElementString( writer, DocProp.LastModifiedByTagName, builtInProperties.LastAuthor, null, CorePropertiesPrefix );
      SerializeCreatedModifiedTimeElement( writer, DocProp.CreatedTagName, builtInProperties.CreationDate );
      SerializeCreatedModifiedTimeElement( writer, DocProp.ModifiedTagName, builtInProperties.LastSaveDate );
      SerializeElementString( writer, DocProp.SubjectTagName, builtInProperties.Subject, null, DublinCorePrefix );

      DateTime printedTime = builtInProperties.LastPrinted;

      if( printedTime != DateTime.MinValue )
      {
          string strPrintedTime = printedTime.ToUniversalTime().ToString(DocProp.DateTimeFormatStructure, CultureInfo.InvariantCulture);
        writer.WriteElementString( CorePropertiesPrefix, DocProp.LastPrintedTagName, null, strPrintedTime );
      }

      SerializeElementString( writer, DocProp.TitleTagName, builtInProperties.Title, null, DublinCorePrefix );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes created and modified datetime element into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Element tag name.</param>
    /// <param name="dateTime">Date time value.</param>
    private void SerializeCreatedModifiedTimeElement( XmlWriter writer, string tagName, DateTime dateTime )
    {
      if( dateTime.Date != DateTime.MinValue/*.AddYears( DocumentPropertyImpl.DEF_FILE_TIME_START_YEAR )*/ )
      {
        writer.WriteStartElement( DublinCoreTermsPrefix, tagName, null );
        writer.WriteAttributeString( XSIPrefix, DocProp.XsiTypeAttributeName, null, DocProp.XsiTypeAttributeValue );
        string strDateValue = dateTime.ToUniversalTime().ToString( DocProp.DateTimeFormatStructure,CultureInfo.InvariantCulture );
        writer.WriteRaw( strDateValue );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes custom properties into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize properties into.</param>
    public void SerializeCustomProperties( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartDocument( true );
      writer.WriteStartElement( DocProp.CustomFilePropertiesTagName, CustomPropertiesPartType );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, DocPropsVTypesPrefix,
        null, DocPropsVTypesPartType );

      CustomDocumentProperties customProperties = ( CustomDocumentProperties )m_book.CustomDocumentProperties;
      int iPropertyId = 2;

      for( int i = 0, iCount = customProperties.Count; i < iCount; i++ )
      {
        DocumentPropertyImpl customProperty = ( DocumentPropertyImpl )customProperties[ i ];

        if( customProperty.Name != DocProp.RelativeHyperlinkExcel97Name &&
          customProperty.Name != DocProp.HyperlinksPropertyExcel97Name )
        {
          SerializeCustomProperty( writer, customProperty, iPropertyId );
          iPropertyId++;
        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes Document Management of the ContentType Properties into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize properties into.</param>
    public void SerializeContentTypeProperties(XmlWriter writer)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        writer.WriteStartDocument(false);
        writer.WriteStartElement(PPrefix, Properties, PropertiesNameSpace);
        writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, XSIPrefix, null, XSIPartType);
        writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, PCPrefix, null, PartnerControlsNameSpace);
        writer.WriteStartElement(DocumentManagement);

        MetaPropertiesImpl metaProperties = (MetaPropertiesImpl)m_book.ContentTypeProperties;

        for (int i = 0; i < metaProperties.Count; i++)
        {
            MetaPropertyImpl metaProperty = (MetaPropertyImpl)metaProperties[i];

            if (m_book.m_childElements.ContainsKey(metaProperty.InternalName))
            {
                writer.WriteStartElement(metaProperty.InternalName, metaProperty.NameSpaceURI);
                List<Stream> listChild = new List<Stream>();
                m_book.m_childElements.TryGetValue(metaProperty.InternalName, out listChild);
                foreach (Stream strm in listChild)
                {
                    SerializeStream(writer, strm);
                }
                writer.WriteEndElement();
            }
            else
            {
                if (metaProperty.NameSpaceURI != null)
                    writer.WriteElementString(metaProperty.ElementName, metaProperty.NameSpaceURI, metaProperty.Value);
            }
        }
        writer.WriteEndElement();
        writer.WriteEndElement();

    }
    /// <summary>
    /// Serializes custom property into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="property">Custom property to serialize.</param>
    /// <param name="iPropertyId">Custom property id.</param>
    private void SerializeCustomProperty( XmlWriter writer, DocumentPropertyImpl property, int iPropertyId )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( property == null )
        throw new ArgumentNullException( "property" );

      writer.WriteStartElement( DocProp.CustomFilePropertyTagName );
      SerializeAttribute( writer, DocProp.FormatIDAttributeName, "{" + CustomDocumentProperties.CustomGuidString + "}", string.Empty );
      SerializeAttribute( writer, DocProp.PropertyIDAttributeName, iPropertyId, int.MinValue );
      SerializeAttribute( writer, DocProp.NameAttributeName, property.Name, string.Empty );

      switch( property.PropertyType )
      {
        case PropertyType.String:
          writer.WriteElementString( DocPropsVTypesPrefix, DocProp.LPWSTRVariantType, null, property.Text );
          break;

        case PropertyType.AsciiString:
          writer.WriteElementString( DocPropsVTypesPrefix, DocProp.LPSTRVariantType, null, property.Text );
          break;

        case PropertyType.DateTime:
          DateTime dateTime = property.DateTime;
          string strDateValue = dateTime.ToUniversalTime().ToString( DocProp.DateTimeFormatStructure );
          writer.WriteElementString( DocPropsVTypesPrefix, DocProp.FileTimeVariantType, null, strDateValue );
          break;

        case PropertyType.Double:
          string strDoubleValue = property.Double.ToString( NumberFormatInfo.InvariantInfo );
          writer.WriteElementString( DocPropsVTypesPrefix, DocProp.EightByteRealNumberVariantType, null, strDoubleValue );
          break;

        case PropertyType.Int32:
          string strInt32Value = property.Int32.ToString();
          writer.WriteElementString( DocPropsVTypesPrefix, DocProp.FourByteSignedIntegerVariantType, null, strInt32Value );
          break;

        case PropertyType.Int:
          string strIntValue = property.Integer.ToString();
          writer.WriteElementString( DocPropsVTypesPrefix, DocProp.IntegerVariantType, null, strIntValue );
          break;

        case PropertyType.Bool:
          string strBoolValue = property.Boolean.ToString();
          strBoolValue = strBoolValue.ToLower( CultureInfo.InvariantCulture );
          writer.WriteElementString( DocPropsVTypesPrefix, DocProp.BooleanVarianYype, null, strBoolValue );
          break;

        default:
          break;
      }

      writer.WriteEndElement();
    }

    /// <summary>
    /// Serializes workbook views.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="lstBookViews">Workbook views collection.</param>
    private void SerializeBookViews( XmlWriter writer, List<Dictionary<string, string>> lstBookViews )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      Dictionary<string, string> dicBookView = ( lstBookViews == null ) ? new Dictionary<string, string>() : lstBookViews[ 0 ];

      ChangeCreateAttributeValue( dicBookView, ActiveSheetIndexAttributeName, m_book.ActiveSheetIndex );
      ChangeCreateAttributeValue( dicBookView, FirstSheetAttributeName, m_book.DisplayedTab );

      if( lstBookViews == null && dicBookView.Count != 0 )
      {
        lstBookViews = new List<Dictionary<string, string>>();
        lstBookViews.Add( dicBookView );
      }

      if (lstBookViews == null)
      {
          writer.WriteStartElement(WorkbookViewsTagName);
          writer.WriteStartElement(WorkbookViewTagName);
          writer.WriteEndElement();
          writer.WriteEndElement();
          return;
      }

      writer.WriteStartElement( WorkbookViewsTagName );

      foreach( Dictionary<string, string> dicView in lstBookViews )
      {
        SerializeWorkbookView( writer, dicView );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes workbook view.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dicView">Workbook view.</param>
    private void SerializeWorkbookView( XmlWriter writer, Dictionary<string, string> dicView )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dicView == null )
        throw new ArgumentNullException( "dicView" );

      writer.WriteStartElement( WorkbookViewTagName );

      foreach( KeyValuePair<string, string> keyValue in dicView )
      {
        SerializeAttribute( writer, keyValue.Key, keyValue.Value, string.Empty );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Changes or adds attribute value.
    /// </summary>
    /// <param name="dicBookView">Workbook view.</param>
    /// <param name="strAttributeName">Attribute to make changes.</param>
    /// <param name="iNewValue">New value for attribute.</param>
    private void ChangeCreateAttributeValue( Dictionary<string, string> dicBookView,
      string strAttributeName, int iNewValue )
    {
      if( dicBookView.ContainsKey( strAttributeName ) )
      {
        if( iNewValue != 0 )
        {
          dicBookView[ strAttributeName ] = iNewValue.ToString();
        }
        else
        {
          dicBookView.Remove( strAttributeName );
        }
      }
      else
      {
        if( iNewValue != 0 )
          dicBookView.Add( strAttributeName, iNewValue.ToString() );
      }
    }
    /// <summary>
    /// Serializes external references.
    /// </summary>
    /// <param name="writer">XmlWriter external links must be serialized into.
    /// This writer should already contain some workbook data.</param>
    /// <param name="relations">Workbook relation collection.</param>
    private void SerializeBookExternalLinks( XmlWriter writer, RelationCollection relations )
    {
#if MEASURE_PERFORMANCE
      DateTime methodStart = DateTime.Now;
#endif

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      ExternBookCollection arrBooks = m_book.ExternWorkbooks;
      IWorksheets worksheets = m_book.Worksheets;
      bool bFirst = true;

      if( arrBooks.Count != 0 )
      {
        for( int i = 0, len = arrBooks.Count; i < len; i++ )
        {
          ExternWorkbookImpl externBook = arrBooks[ i ];

          if( !externBook.IsInternalReference && !string.IsNullOrEmpty( externBook.URL ) && !externBook.IsAddInFunctions )
          {
	          if( bFirst )
	          {
	            bFirst = false;
	            writer.WriteStartElement( ExternalLinks.ExternalReferencesTag );
	          }

            SerializeLink( externBook, writer, relations );
          }
        }
      }
      if (m_book.PreservedExternalLinks.Count > 0)
      {
          foreach (string strRelationId in m_book.PreservedExternalLinks)
          {
              writer.WriteStartElement(ExternalLinks.ExternalReferenceTag);
              writer.WriteAttributeString(RelationAttribute, RelationNamespace, strRelationId);
              writer.WriteEndElement();
          }
      }
     
      if( !bFirst )
        writer.WriteEndElement();

#if MEASURE_PERFORMANCE
      DateTime methodEnd = DateTime.Now;
      Console.CursorLeft += 2;
      Console.WriteLine( "SerializeBookExternalLinks() took: {0}", methodEnd - methodStart );
      Console.WriteLine( "Sum of Links serialization: {0}", m_externalLinksTotal );
#endif
    }
    /// <summary>
    /// Serializes single external link.
    /// </summary>
    /// <param name="externBook">External workbook object to serialize.</param>
    /// <param name="writer">XmlWriter to write into.</param>
    /// <param name="relations">Workbook relations.</param>
    private void SerializeLink( ExternWorkbookImpl externBook, XmlWriter writer, RelationCollection relations )
    {
      if( externBook == null )
        throw new ArgumentNullException( "externBook" );

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      string strItemName = m_book.DataHolder.SerializeExternalLink( externBook );//SerializeExternalLink( externBook );
      writer.WriteStartElement( ExternalLinks.ExternalReferenceTag );

      string strRelationId = relations.GenerateRelationId();
      relations[ strRelationId ] = new Relation( '/' + strItemName, RelationTypes.ExternalLink );
      writer.WriteAttributeString( RelationAttribute, RelationNamespace, strRelationId );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes worksheet horizontal and vertical pagebreaks.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize page breaks for.</param>
    private void SerializePagebreaks( XmlWriter writer, IWorksheet sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      HPageBreaksCollection hPagebreaks = ( HPageBreaksCollection )sheet.HPageBreaks;

      if( hPagebreaks != null )
        SerializeHorizontalPageBreaks( writer, hPagebreaks );

      VPageBreaksCollection vPagebreaks = ( VPageBreaksCollection )sheet.VPageBreaks;

      if( vPagebreaks != null )
        SerializeVerticalPageBreaks( writer, vPagebreaks );
    }
    /// <summary>
    /// Serializes worksheet horizontal pagebreaks.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="hPagebreaks">Sheet horizontal pagebreaks.</param>
    private void SerializeHorizontalPageBreaks( XmlWriter writer, HPageBreaksCollection hPagebreaks )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( hPagebreaks == null )
        throw new ArgumentNullException( "hPagebreaks" );

      int iCount = hPagebreaks.Count;

      if( iCount == 0 )
        return;

      writer.WriteStartElement( HorizontalPageBreaksTagName );
      SerializeAttribute( writer, PageBreakCountAttributeName, iCount, 0 );
      SerializeAttribute( writer, ManualBreakCountAttributeName, hPagebreaks.ManualBreakCount, 0 );

      SortedList<int, List<HPageBreakImpl>> sortedBreaks = new SortedList<int, List<HPageBreakImpl>>();

      for( int i = 0; i < iCount; i++ )
      {
        HPageBreakImpl hPagebreak = ( HPageBreakImpl )hPagebreaks[ i ];
        List<HPageBreakImpl> items;
        int iRow = hPagebreak.HPageBreak.Row;

        if( !sortedBreaks.TryGetValue( iRow, out items ) )
        {
          items = new List<HPageBreakImpl>();
          sortedBreaks.Add( iRow, items );
        }

        items.Add( hPagebreak );
      }

      for( int i = 0, len = sortedBreaks.Count; i < len; i++ )
      {
        List<HPageBreakImpl> items = sortedBreaks.Values[ i ];

        for( int j = 0, lenJ = items.Count; j < lenJ; j++ )
        {
          HPageBreakImpl hPagebreak = items[ j ];
          HorizontalPageBreaksRecord.THPageBreak pageBreak = hPagebreak.HPageBreak;
          SerializeSinglePagebreak( writer, pageBreak.Row, pageBreak.StartColumn, pageBreak.EndColumn, hPagebreak.Type );
        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes worksheet vertical pagebreaks.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="vPagebreaks">Sheet vertical pagebreaks.</param>
    private void SerializeVerticalPageBreaks( XmlWriter writer, VPageBreaksCollection vPagebreaks )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( vPagebreaks == null )
        throw new ArgumentNullException( "vPagebreaks" );

      int iCount = vPagebreaks.Count;

      if( iCount == 0 )
        return;

      writer.WriteStartElement( VerticalPageBreaksTagName );
      SerializeAttribute( writer, PageBreakCountAttributeName, iCount, 0 );
      SerializeAttribute( writer, ManualBreakCountAttributeName, vPagebreaks.ManualBreakCount, 0 );

      //for( int i = 0; i < iCount; i++ )
      //{
      //  VPageBreakImpl vPagebreak = ( VPageBreakImpl )vPagebreaks[ i ];
      //  VerticalPageBreaksRecord.TVPageBreak pageBreak = vPagebreak.VPageBreak;
      //  SerializeSinglePagebreak( writer, pageBreak.Column, ( int )pageBreak.StartRow,
      //    ( int )pageBreak.EndRow, vPagebreak.Type );
      //}
      SortedList<int, List<VPageBreakImpl>> sortedBreaks = new SortedList<int, List<VPageBreakImpl>>();

      for( int i = 0; i < iCount; i++ )
      {
        VPageBreakImpl vPagebreak = ( VPageBreakImpl )vPagebreaks[ i ];
        List<VPageBreakImpl> items;
        int iColumn = vPagebreak.VPageBreak.Column;

        if( !sortedBreaks.TryGetValue( iColumn, out items ) )
        {
          items = new List<VPageBreakImpl>();
          sortedBreaks.Add( iColumn, items );
        }

        items.Add( vPagebreak );
      }

      for( int i = 0, len = sortedBreaks.Count; i < len; i++ )
      {
        List<VPageBreakImpl> items = sortedBreaks.Values[ i ];

        for( int j = 0, lenJ = items.Count; j < lenJ; j++ )
        {
          VPageBreakImpl vPagebreak = items[ j ];
          VerticalPageBreaksRecord.TVPageBreak pageBreak = vPagebreak.VPageBreak;
          SerializeSinglePagebreak( writer, pageBreak.Column, ( int )pageBreak.StartRow, ( int )pageBreak.EndRow, vPagebreak.Type );
        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single pagebreak.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="iRowColumn">Row/column of pagebreak.</param>
    /// <param name="iStart">Start row/column.</param>
    /// <param name="iEnd">End row/column.</param>
    /// <param name="type">Pagebreak type.</param>
    private void SerializeSinglePagebreak( XmlWriter writer, int iRowColumn, int iStart, int iEnd, ExcelPageBreak type )
    {
      writer.WriteStartElement( BreakTagName );
      SerializeAttribute( writer, IdAttributeName, iRowColumn, 0 );
      SerializeAttribute( writer, MinimumAttributeName, iStart, 0 );
      SerializeAttribute( writer, MaximumAttributeName, iEnd, 0 );
      SerializeAttribute( writer, ManualPageBreakAttributeName, type == ExcelPageBreak.PageBreakManual, false );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Extracts extent settings from reader and converts them into pixels.
    /// </summary>
    /// <param name="writer">Writer to save extent data into.</param>
    /// <param name="extent">Size of the shape in pixels.</param>
    public static void SerializeExtent( XmlWriter writer, Size extent )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      int iWidth = extent.Width;
      int iHeight = extent.Height;

      iWidth = ( int )ApplicationImpl.ConvertFromPixel( iWidth, MeasureUnits.EMU );
      iHeight = ( int )ApplicationImpl.ConvertFromPixel( iHeight, MeasureUnits.EMU );

      writer.WriteStartElement( Drawings.Extents, Drawings.XdrNamespace );

      if( iWidth <= 0 )
        iWidth = ChartSerializator.DefaultExtentX;

      if( iHeight <= 0 )
        iHeight = ChartSerializator.DefaultExtentY;

      writer.WriteAttributeString( Drawings.CXAttributeName, iWidth.ToString() );
      writer.WriteAttributeString( Drawings.CYAttributeName, iHeight.ToString() );
      writer.WriteEndElement();

      //return new Size( iWidth, iHeight );
    }
    public void SerializeConnections(XmlWriter writer)
    {
        if (writer == null)
            throw new ArgumentNullException("Writer");
        if (writer == null)
            throw new ArgumentNullException("writer");
        
        writer.WriteStartDocument(true);
        writer.WriteStartElement(ConnectionsTag, XmlNamespaceMain);
        
        for (int i = 0; i < m_book.Connections.Count; i++)
        {            
            SerializeConnection(writer,m_book.Connections[i] as ExternalConnection );
            
        }
        for (int i = 0; i < m_book.DeletedConnections.Count; i++)
        {
            SerializeConnection(writer, m_book.DeletedConnections[i] as ExternalConnection);

        }
       // for(int i=0;i<)
        writer.WriteEndElement();
        
    }
    public void SerializeConnection(XmlWriter writer,ExternalConnection connection)
    {
        if (connection != null)
        {
            DataBaseProperty DBProperty = new DataBaseProperty();
            if (connection.DataBaseType == ExcelConnectionsType.ConnectionTypeODBC)
                DBProperty = connection.ODBCConnection as DataBaseProperty;
            else if (connection.DataBaseType == ExcelConnectionsType.ConnectionTypeOLEDB)
                DBProperty = connection.OLEDBConnection as DataBaseProperty;
            
            writer.WriteStartElement(ConnectionTag);
            writer.WriteAttributeString(ConnectionIdAttribute, connection.ConncetionId.ToString());
            writer.WriteAttributeString(SourceFile, connection.SourceFile);
            if (connection.Deleted)
                writer.WriteAttributeString(Deleted, "1");
            if (connection.ConnectionFile != null && connection.ConnectionFile!="")
            writer.WriteAttributeString(OdbcFileAttribute, connection.ConnectionFile);
            writer.WriteAttributeString(DataBaseTypeAttribute, ((int)connection.DataBaseType).ToString());
            writer.WriteAttributeString(DataBaseNameAttribute, connection.Name);
            writer.WriteAttributeString(RefreshedVersionAttribute, connection.RefershedVersion.ToString());
            
            if (DBProperty.AlwaysUseConnectionFile)
                writer.WriteAttributeString(OnlyUseConnectionFile, "1");
            if (DBProperty.SavePassword)
                writer.WriteAttributeString(SavePassword, "1");
            if (DBProperty.RefreshPeriod > 0)
                writer.WriteAttributeString(Interval, DBProperty.RefreshPeriod.ToString());
            if (DBProperty.ServerCredentialsMethod != ExcelCredentialsMethod.integrated)
                writer.WriteAttributeString(Credentials, DBProperty.ServerCredentialsMethod.ToString());
            if (DBProperty.RefreshOnFileOpen)
            writer.WriteAttributeString(ListObjects.RefreshOnLoad, "1");
            if (DBProperty.BackgroundQuery)
                SerializeBool(writer, BackGroundAttribute, connection.BackgroundQuery);
            if(connection.Description!=null)
                writer.WriteAttributeString(DescriptionTag, connection.Description);
            if (connection.DataBaseType == ExcelConnectionsType.ConnectionTypeODBC || connection.DataBaseType == ExcelConnectionsType.ConnectionTypeOLEDB)
            {
                SerializeDataBaseProerty(writer, DBProperty);
            }
            else if (connection.DataBaseType == ExcelConnectionsType.ConnectionTypeWEB)
            {
                SerializeWebProperty(writer, connection);
            }
            if (connection.OlapProperty != null)
            {
                connection.OlapProperty.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, connection.OlapProperty);
            }
            if (connection.ExtLstProperty != null)
            {
                connection.ExtLstProperty.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, connection.ExtLstProperty);
            }
            if (connection.m_textPr != null)
            {
                connection.m_textPr.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, connection.m_textPr);
            }
            writer.WriteEndElement();
        }
    }

    public void SerializeDataBaseProerty(XmlWriter writer, DataBaseProperty DBProperty)
    {
        writer.WriteStartElement(DataBasePrTag);
        writer.WriteAttributeString(ConnectionTag, (string)DBProperty.ConnectionString);
        writer.WriteAttributeString(CommandTextAttribute, (string)DBProperty.CommandText);
        int CommandType = (int)DBProperty.CommandType;
        writer.WriteAttributeString(CommandTypeAttribute, CommandType.ToString());
        writer.WriteEndElement();
    }
    public void SerializeWebProperty(XmlWriter writer, ExternalConnection Connection)
    {
        writer.WriteStartElement(WebPrTag);
        if (Connection.IsXml)
            SerializeBool(writer, Xml, Connection.IsXml);
        writer.WriteAttributeString(URL, Connection.ConnectionURL);
        writer.WriteEndElement();
    }
    #endregion
  }
}
