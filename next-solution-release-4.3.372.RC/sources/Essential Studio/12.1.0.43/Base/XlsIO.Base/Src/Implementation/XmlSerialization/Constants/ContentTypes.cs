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
  /// This class stores constants with different content types.
  /// </summary>
  public sealed class ContentTypes
  {
    /// <summary>
    /// Bitmap image content type.
    /// </summary>
    public const string Bitmap = "image/bmp";
    /// <summary>
    /// Jpeg image content type.
    /// </summary>
    public const string Jpeg = "image/jpeg";
    /// <summary>
    /// Png image content type.
    /// </summary>
    public const string Png = "image/png";
    /// <summary>
    /// Emf image content type.
    /// </summary>
    public const string Emf = "image/x-emf";
    /// <summary>
    /// Gif image content type.
    /// </summary>
    public const string Gif = "image/gif";
    /// <summary>
    /// Content type for xml files.
    /// </summary>
    public const string Xml = "application/xml";
    /// <summary>
    /// Content type for rels files.
    /// </summary>
    public const string Relations = "application/vnd.openxmlformats-package.relationships+xml";
    /// <summary>
    /// Content type for workbook item (used in [Content_Types].xml).
    /// </summary>
    public const string Workbook = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";
    /// <summary>
    /// Content type for workbook item that contains macros (used in [Content_Types].xml).
    /// </summary>
    public const string MacroWorkbook = "application/vnd.ms-excel.sheet.macroEnabled.main+xml";
    /// <summary>
    /// Content type for macro template item that contains macros (used in [Content_Types].xml).
    /// </summary>
    public const string MacroTemplate = "application/vnd.ms-excel.template.macroEnabled.main+xml";
    /// <summary>
    /// Content type for template item that contains macros (used in [Content_Types].xml).
    /// </summary>
    public const string Template = "application/vnd.openxmlformats-officedocument.spreadsheetml.template.main+xml";
    /// <summary>
    /// Content type for calculation chain item.
    /// </summary>
    public const string CalcChain = "application/vnd.openxmlformats-officedocument.spreadsheetml.calcChain+xml";
    /// <summary>
    /// Content type of the worksheet.
    /// </summary>
    public const string Worksheet = "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml";
    /// <summary>
    /// Content type of the chartsheet.
    /// </summary>
    public const string Chartsheet = "application/vnd.openxmlformats-officedocument.spreadsheetml.chartsheet+xml";
    /// <summary>
    /// Content type of the shared strings table.
    /// </summary>
    public const string SharedStrings = "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml";
    /// <summary>
    /// Content type of the styles.
    /// </summary>
    public const string Styles = "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml";
    /// <summary>
    /// Content type of the vml drawings item.
    /// </summary>
    public const string Vml = "application/vnd.openxmlformats-officedocument.vmlDrawing";
    /// <summary>
    /// Content type of the comments item.
    /// </summary>
    public const string Comments = "application/vnd.openxmlformats-officedocument.spreadsheetml.comments+xml";
    /// <summary>
    /// Content type for drawings item.
    /// </summary>
    public const string Drawings = "application/vnd.openxmlformats-officedocument.drawing+xml";
    /// <summary>
    /// Content type for chart drawings item.
    /// </summary>
    public const string ChartDrawings = "application/vnd.openxmlformats-officedocument.drawingml.chartshapes+xml";
    /// <summary>
    /// Content type for core properties.
    /// </summary>
    public const string CoreProperties = "application/vnd.openxmlformats-package.core-properties+xml";
    /// <summary>
    /// Content type for extended properties.
    /// </summary>
    public const string ExtendedProperties = "application/vnd.openxmlformats-officedocument.extended-properties+xml";
    /// <summary>
    /// Content type for custom properties.
    /// </summary>
    public const string CustomProperties = "application/vnd.openxmlformats-officedocument.custom-properties+xml";
    /// <summary>
    /// Content type for extern link item.
    /// </summary>
    public const string ExternLink = "application/vnd.openxmlformats-officedocument.spreadsheetml.externalLink+xml";
    /// <summary>
    /// Content type for chart object.
    /// </summary>
    public const string Chart = "application/vnd.openxmlformats-officedocument.drawingml.chart+xml";
    /// <summary>
    /// Content type for worksheet custom property.
    /// </summary>
    public const string WorksheeetCustomProperty = "application/vnd.openxmlformats-officedocument.spreadsheetml.customProperty";
    /// <summary>
    /// Content type for pivot table.
    /// </summary>
    public const string PivotTable ="application/vnd.openxmlformats-officedocument.spreadsheetml.pivotTable+xml";
    /// <summary>
    /// Content type for pivot cache definition.
    /// </summary>
    public const string PivotCacheDefinition = "application/vnd.openxmlformats-officedocument.spreadsheetml.pivotCacheDefinition+xml";
    /// <summary>
    /// Content type for pivot cache records.
    /// </summary>
    public const string PivotCacheRecords = "application/vnd.openxmlformats-officedocument.spreadsheetml.pivotCacheRecords+xml";
    /// <summary>
    /// Content type for table object.
    /// </summary>
    public const string Table = "application/vnd.openxmlformats-officedocument.spreadsheetml.table+xml";
    /// <summary>
    /// Content type for CustomXml Objects
    /// </summary>
    public const string CustomXmlProperties = "application/vnd.openxmlformats-officedocument.customXmlProperties+xml";

    public const string Connections = "application/vnd.openxmlformats-officedocument.spreadsheetml.connections+xml";
    public const string QueryTable = "application/vnd.openxmlformats-officedocument.spreadsheetml.queryTable+xml";
  }
}
