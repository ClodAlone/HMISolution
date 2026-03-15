#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

/// <summary>
/// The Syncfusion.Pdf.Tables namespace contains classes for creating tables.
/// </summary>
namespace Syncfusion.Pdf.Tables
{
    /// <summary>
    /// Specifies values specifying where the header should formed from.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// PdfPage page = doc.Pages.Add();
    /// PdfLightTable table = new PdfLightTable();
    /// // Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
    /// // Create new Columns
    /// table.Columns.Add(new PdfColumn("Roll Number"));
    /// table.Columns.Add(new PdfColumn("Name"));
    /// table.Columns.Add(new PdfColumn("Class"));
    /// table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
    /// // Add new Rows
    /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
    /// // Draws the table 
    /// table.Draw(page, new PointF(0, 0));
    /// doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Setting the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
    /// ' Create new Columns
    /// table.Columns.Add(New PdfColumn("Roll Number"))
    /// table.Columns.Add(New PdfColumn("Name"))
    /// table.Columns.Add(New PdfColumn("Class"))
    /// table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions
    /// ' Add new Rows
    /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
    /// ' Draws the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class  
    /// <seealso cref="PdfColumn"/> Class  
    public enum PdfHeaderSource
    {
        /// <summary>
        /// The header is formed from column captions' values.
        /// </summary>
        ColumnCaptions,
        /// <summary>
        /// The header is formed from rows.
        /// </summary>
        Rows,
    }

     /// <summary>
    /// Specifies the datasource type.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// PdfPage page = doc.Pages.Add();
    /// PdfLightTable table = new PdfLightTable();
    /// // Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
    /// // Create new Columns
    /// table.Columns.Add(new PdfColumn("Roll Number"));
    /// table.Columns.Add(new PdfColumn("Name"));
    /// table.Columns.Add(new PdfColumn("Class"));
    /// table.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Inside;
    /// // Add new Rows
    /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
    /// // Draws the table 
    /// table.Draw(page, new PointF(0, 0));
    /// doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Setting the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
    /// ' Create new Columns
    /// table.Columns.Add(New PdfColumn("Roll Number"))
    /// table.Columns.Add(New PdfColumn("Name"))
    /// table.Columns.Add(New PdfColumn("Class"))
    /// table.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Inside
    /// ' Add new Rows
    /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
    /// ' Draws the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class  
    /// <seealso cref="PdfColumn"/> Class  
    public enum PdfLightTableDataSourceType
    {
        /// <summary>
        /// Specifies that the PdfLightTable has been binded to an external datasource.
        /// </summary>
        External,
        /// <summary>
        /// Specifies that the values are directly binded to the PdfLightTable.
        /// </summary>
        TableDirect,
    }
}

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Specifies values of the border overlap style.
    /// </summary>
    /// <example>
    public enum PdfBorderOverlapStyle
    {
        /// <summary>
        /// Cell borders overlap (are drawn using the same coordinates).
        /// </summary>
        Overlap,
        /// <summary>
        /// Cell borders are drawns in the cell's interior.
        /// </summary>
        Inside,
    }
}
