#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
	/// <summary>
	/// This class simply hold all constants required by relations.
	/// </summary>
	public sealed class RelationTypes
	{
    #region Constants
    /// <summary>
    /// Content type for workbook item (used in relations).
    /// </summary>
    public const string Workbook = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
    /// <summary>
    /// Content type for styles item (used in relations).
    /// </summary>
    public const string Styles = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";
    /// <summary>
    /// Content type for SST dictionary item (used in relations).
    /// </summary>
    public const string SST = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings";
    /// <summary>
    /// Content type for calculation chain item (used in relations).
    /// </summary>
    public const string CalcChain = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/calcChain";
    /// <summary>
    /// Content type for vml drawings (used in worksheet relations).
    /// </summary>
    public const string VmlDrawings = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/vmlDrawing";
    /// <summary>
    /// Content type of comment notes items (used in worksheet relations).
    /// </summary>
    public const string WorksheetComments = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments";
    /// <summary>
    /// Content type for drawings (used in worksheet relations).
    /// </summary>
    public const string Drawings = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing";
    /// <summary>
    /// Content type for chart drawings (used in worksheet relations).
    /// </summary>
    public const string ChartDrawings = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chartUserShapes";
    /// <summary>
    /// Represents package level relationships namespace.
    /// </summary>
    public const string PackageNamespace = "http://schemas.openxmlformats.org/package/2006/relationships";
    /// <summary>
    /// Content type for theme item (used in relations).
    /// </summary>
    public const string Themes = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme";
    /// <summary>
    /// Content type for image item (used in relations).
    /// </summary>
    public const string Image = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
    /// <summary>
    /// Content type for core properties item (used in relations).
    /// </summary>
    public const string CoreProperties = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties";
    /// <summary>
    /// Content type for extended properties item (used in relations).
    /// </summary>
    public const string ExtendedProperties = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties";
    /// <summary>
    /// Content type for custom properties item (used in relations).
    /// </summary>
    public const string CustomProperties = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties";
    /// <summary>
    /// Content type for path to extern link source (used in relations).
    /// </summary>
    public const string ExternLinkPath = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLinkPath";
    /// <summary>
    /// Content type for external link (used in relations).
    /// </summary>
    public const string ExternalLink = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLink";
    /// <summary>
    /// Content type for chart.
    /// </summary>
    public const string Chart = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart";
    /// <summary>
    /// Relation type for worksheet custom property.
    /// </summary>
    public const string WorksheetCustomProperty = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/customProperty";
    /// <summary>
    /// Content type  for path to extern link source that is missing.
    /// </summary>
    public const string MissingPath = "http://schemas.microsoft.com/office/2006/relationships/xlExternalLinkPath/xlPathMissing";
    /// <summary>
    /// Content type for pivot cache definition.
    /// </summary>
    public const string PivotCacheDefinition = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/pivotCacheDefinition";
    /// <summary>
    /// Content type for pivot cache records.
    /// </summary>
    public const string PivotCacheRecords = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/pivotCacheRecords";
    /// <summary>
    /// Content type for pivot table.
    /// </summary>
    public const string PivotTable = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/pivotTable";
    /// <summary>
    /// Content type for table object.
    /// </summary>
    public const string Table = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/table";
    /// <summary>
    /// Content Type for Ole object.
    /// </summary>
    public const string OleObject = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/oleObject";
    /// <summary>
    /// Content Type for External connection.
    /// </summary>
    public const string Connection = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/connections";
    /// <summary>
    /// Content Type for Query Table.
    /// </summary>
    public const string QueryTable = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/queryTable";

    #endregion

    #region Constructors
    /// <summary>
    /// Prevents a default instance of the RelationTypes class from being created.
    /// </summary>
    private RelationTypes()
    {
    }
    #endregion
  }
}
