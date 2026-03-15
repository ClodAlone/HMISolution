#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Tables namespace contains classes for creating tables.
/// </summary>
namespace Syncfusion.Pdf.Tables
{
    /// <summary>
    /// Delegate for handling StartRowLayoutEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments of the event.</param>
    /// <remarks>This event is raised when starting a row in a layout.</remarks>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// RectangleF rect = new RectangleF(0, 0, 500, 50);
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Roll Number", "Student Name" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Cris" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Clay" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// // Subscribe the begin row event 
    /// table.BeginRowLayout +=new BeginRowLayoutEventHandler(table_BeginRowLayout);
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    ///     // Begin Row Event Handler 
    /// void table_BeginRowLayout(object sender, BeginRowLayoutEventArgs args)
    /// {
    ///   if (args.RowIndex == 1)
    ///   {
    ///      PdfLightTable table = (PdfLightTable)sender;
    ///      int count = table.Columns.Count;
    ///      int[] spanMap = new int[count];
    ///      // Set just spanned cells. Other values are not important except negatives that are not allowed.
    ///      spanMap[0] = 2;
    ///      spanMap[1] = 3;
    ///      args.ColumnSpanMap = spanMap;
    ///      //Sets row height.
    ///      args.MinimalHeight = 30f;
    ///    }
    /// }
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Roll Number", "Student Name" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Cris" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Clay" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Subscribe the begin row event 
    /// AddHandler table.BeginRowLayout, AddressOf table_BeginRowLayout
    /// table.DataSource = dataTable
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    ///  ' Begin Row Event Handler 
    /// Private Sub table_BeginRowLayout(ByVal sender As Object, ByVal args As BeginRowLayoutEventArgs)
    ///   If args.RowIndex = 1 Then
    ///     Dim table As PdfLightTable = CType(sender, PdfLightTable)
    ///     Dim count As Integer = table.Columns.Count
    ///     Dim spanMap() As Integer = New Integer(count - 1){}
    ///     ' Set just spanned cells. Other values are not important except negatives that are not allowed.
    ///     spanMap(0) = 2
    ///     spanMap(1) = 3
    ///     args.ColumnSpanMap = spanMap
    ///     'Sets row height.
    ///     args.MinimalHeight = 30f
    ///   End If
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public delegate void BeginRowLayoutEventHandler(object sender, BeginRowLayoutEventArgs args);

    /// <summary>
    /// Delegate for handling EndRowLayoutEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments of the event.</param>
    /// <remarks>This event is raised when you are finished laying out a row on a page.</remarks>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// RectangleF rect = new RectangleF(0, 0, 500, 50);
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Roll Number", "Student Name" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Cris" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Clay" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// // Subscribe the end row event 
    /// table.EndRowLayout += new EndRowLayoutEventHandler(table_EndRowLayout);
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    ///     // End Row Event Handler 
    /// void table_EndRowLayout(object sender, EndRowLayoutEventArgs args)
    /// {
    ///    if (args.RowIndex == 1)
    ///    {
    ///       // Cancel property used to cancel the table rendering operation
    ///       args.Cancel = true;
    ///    }
    /// }
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Roll Number", "Student Name" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Cris" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Clay" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Subscribe the end row event 
    ///  AddHandler table.EndRowLayout, AddressOf table_EndRowLayout
    /// table.DataSource = dataTable
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    ///  ' Eegin Row Event Handler 
    /// Private Sub table_EndRowLayout(ByVal sender As Object, ByVal args As EndRowLayoutEventArgs)
    ///   If args.RowIndex = 1 Then
    ///      ' Cancel property used to cancel the table rendering operation
    ///      args.Cancel = True
    ///    End If
    ///  End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public delegate void EndRowLayoutEventHandler(object sender, EndRowLayoutEventArgs args);

    /// <summary>
    /// Delegate for handling StartCellLayoutEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments of the event.</param>
    /// <remarks>This event is raised when laying out a cell on a page.</remarks>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// RectangleF rect = new RectangleF(0, 0, 500, 50);
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Roll Number", "Student Name" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Cris" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Clay" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// // Subscribe the cell layout event 
    /// table.BeginCellLayout += new BeginCellLayoutEventHandler(table_BeginCellLayout);
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    /// // Cell layout event handler
    /// void table_BeginCellLayout(object sender, BeginCellLayoutEventArgs args)
    /// {
    ///   if (args.RowIndex == 1)
    ///   {
    ///     args.Graphics.DrawRectangle(new PdfPen(PdfBrushes.Red, 2), PdfBrushes.White,args.Bounds);
    ///   }
    /// }       
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Roll Number", "Student Name" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Cris" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Clay" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Subscribe the cell layout event 
    /// AddHandler table.BeginCellLayout, AddressOf table_BeginCellLayout
    /// table.DataSource = dataTable
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    /// ' Cell layout event handler
    /// Private Sub table_BeginCellLayout(ByVal sender As Object, ByVal args As BeginCellLayoutEventArgs)
    ///   If args.RowIndex = 1 Then
    ///      args.Graphics.DrawRectangle(New PdfPen(PdfBrushes.Red, 2), PdfBrushes.White,args.Bounds)
    ///   End If
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public delegate void BeginCellLayoutEventHandler(object sender, BeginCellLayoutEventArgs args);

    /// <summary>
    /// Delegate for handling EndCellLayoutEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments of the event.</param>
    /// <remarks>This event is raised when you have finished laying out a page.</remarks>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// RectangleF rect = new RectangleF(0, 0, 500, 50);
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Roll Number", "Student Name" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Cris" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Clay" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// // Subscribe the cell layout event 
    /// table.EndCellLayout += new EndCellLayoutEventHandler(table_EndCellLayout);
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    /// // Cell layout event handler
    /// void table_EndCellLayout(object sender, EndCellLayoutEventArgs args)
    /// {
    ///   if (args.RowIndex == 1)
    ///   {
    ///     args.Graphics.DrawRectangle(new PdfPen(PdfBrushes.Red, 2), PdfBrushes.White, args.Bounds);
    ///   }
    /// }
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Roll Number", "Student Name" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Cris" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Clay" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Subscribe the cell layout event 
    /// AddHandler table.EndCellLayout, AddressOf table_EndCellLayout
    /// table.DataSource = dataTable
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    /// ' Cell layout event handler
    /// Private Sub table_EndCellLayout(ByVal sender As Object, ByVal args As EndCellLayoutEventArgs)
    ///   If args.RowIndex = 1 Then
    ///     args.Graphics.DrawRectangle(New PdfPen(PdfBrushes.Red, 2), PdfBrushes.White, args.Bounds)
    ///   End If
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public delegate void EndCellLayoutEventHandler(object sender, EndCellLayoutEventArgs args);

    /// <summary>
    /// Delegate for handling NextRowEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments of the event.</param>
    /// <example>
    /// <code lang="C#">
    /// public string[][] datastring = new string[2][];
    /// // Specify values for the table
    /// datastring[0] = new string[] { "111", "Maxim", "100" };
    /// datastring[1] = new string[] { "222", "Calvin", "95" };
    ///  // Creates a new document
    ///  PdfDocument doc = new PdfDocument();
    ///  PdfPage page = doc.Pages.Add();
    ///  PdfLightTable table = new PdfLightTable();
    ///  //// Setting the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.External;
    ///  //Subscribing Events
    ///  table.QueryColumnCount += new QueryColumnCountEventHandler(table_QueryColumnCount);             
    ///  table.QueryNextRow += new QueryNextRowEventHandler(table_QueryNextRow);
    ///  // Draw the table 
    ///  table.Draw(page, new PointF(0, 0));
    ///  doc.Save("Tables.pdf");
    ///  void table_QueryColumnCount(object sender, QueryColumnCountEventArgs args)
    ///  {
    ///    args.ColumnCount = 3;    
    ///  }
    /// void table_QueryNextRow(object sender, QueryNextRowEventArgs args)
    /// {
    ///   if (args.RowIndex < datastring.Length)
    ///    args.RowData = new string[] { datastring[args.RowIndex][0], datastring[args.RowIndex][1], datastring[args.RowIndex][2] }; 
    ///  }
    /// </code>
    /// <code lang="VB">
    /// Public datastring()() As String = New String(1)(){}
    /// ' Specify values for the table
    /// Private datastring(0) = New String() { "111", "Maxim", "100" }
    /// Private datastring(1) = New String() { "222", "Calvin", "95" }
    /// ' Creates a new document
    /// Private doc As PdfDocument = New PdfDocument()
    /// Private page As PdfPage = doc.Pages.Add()
    /// Private table As PdfLightTable = New PdfLightTable()
    /// ' Setting the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.External
    /// 'Subscribing Events
    /// AddHandler table.QueryColumnCount, AddressOf table_QueryColumnCount
    /// AddHandler table.QueryNextRow, AddressOf table_QueryNextRow
    /// ' Draw the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// Private Sub table_QueryColumnCount(ByVal sender As Object, ByVal args As QueryColumnCountEventArgs)
    ///   args.ColumnCount = 3
    /// End Sub
    /// Private Sub table_QueryNextRow(ByVal sender As Object, ByVal args As QueryNextRowEventArgs)
    ///  If args.RowIndex < datastring.Length Then
    ///    args.RowData = New String() { datastring(args.RowIndex)(0), datastring(args.RowIndex)(1), datastring(args.RowIndex)(2) }
    ///  End If
    ///  End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public delegate void QueryNextRowEventHandler(object sender, QueryNextRowEventArgs args);

    /// <summary>
    /// Delegate for handling GettingColumnNumber Event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments of the event.</param>
    /// <example>
    /// <code lang="C#">
    /// public string[][] datastring = new string[2][];
    /// // Specify values for the table
    /// datastring[0] = new string[] { "111", "Maxim", "100" };
    /// datastring[1] = new string[] { "222", "Calvin", "95" };
    ///  // Creates a new document
    ///  PdfDocument doc = new PdfDocument();
    ///  PdfPage page = doc.Pages.Add();
    ///  PdfLightTable table = new PdfLightTable();
    ///  //// Setting the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.External;
    ///  //Subscribing Events
    ///  table.QueryColumnCount += new QueryColumnCountEventHandler(table_QueryColumnCount);             
    ///  table.QueryNextRow += new QueryNextRowEventHandler(table_QueryNextRow);
    ///  // Draw the table 
    ///  table.Draw(page, new PointF(0, 0));
    ///  doc.Save("Tables.pdf");
    ///  void table_QueryColumnCount(object sender, QueryColumnCountEventArgs args)
    ///  {
    ///    args.ColumnCount = 3;    
    ///  }
    /// void table_QueryNextRow(object sender, QueryNextRowEventArgs args)
    /// {
    ///   if (args.RowIndex < datastring.Length)
    ///    args.RowData = new string[] { datastring[args.RowIndex][0], datastring[args.RowIndex][1], datastring[args.RowIndex][2] }; 
    ///  }
    /// </code>
    /// <code lang="VB">
    /// Public datastring()() As String = New String(1)(){}
    /// ' Specify values for the table
    /// Private datastring(0) = New String() { "111", "Maxim", "100" }
    /// Private datastring(1) = New String() { "222", "Calvin", "95" }
    /// ' Creates a new document
    /// Private doc As PdfDocument = New PdfDocument()
    /// Private page As PdfPage = doc.Pages.Add()
    /// Private table As PdfLightTable = New PdfLightTable()
    /// ' Setting the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.External
    /// 'Subscribing Events
    /// AddHandler table.QueryColumnCount, AddressOf table_QueryColumnCount
    /// AddHandler table.QueryNextRow, AddressOf table_QueryNextRow
    /// ' Draw the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// Private Sub table_QueryColumnCount(ByVal sender As Object, ByVal args As QueryColumnCountEventArgs)
    ///   args.ColumnCount = 3
    /// End Sub
    /// Private Sub table_QueryNextRow(ByVal sender As Object, ByVal args As QueryNextRowEventArgs)
    ///  If args.RowIndex < datastring.Length Then
    ///    args.RowData = New String() { datastring(args.RowIndex)(0), datastring(args.RowIndex)(1), datastring(args.RowIndex)(2) }
    ///  End If
    ///  End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public delegate void QueryColumnCountEventHandler(object sender, QueryColumnCountEventArgs args);

    /// <summary>
    /// Delegate for handling GettingRowNumber Event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments of the event.</param>
    /// <example>
    /// <code lang="C#">
    /// public string[][] datastring = new string[3][];
    /// // Specify values for the table
    /// datastring[0] = new string[] { "111", "Maxim", "100" };
    /// datastring[1] = new string[] { "222", "Calvin", "95" };
    /// datastring[2] = new string[] { "333", "Criss", "99" };
    ///  // Creates a new document
    ///  PdfDocument doc = new PdfDocument();
    ///  PdfPage page = doc.Pages.Add();
    ///  PdfLightTable table = new PdfLightTable();
    ///  //// Setting the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.External;
    ///  //Subscribing Events
    ///  table.QueryRowCount += new QueryRowCountEventHandler(table_QueryRowCount);    /// 
    ///  table.QueryColumnCount += new QueryColumnCountEventHandler(table_QueryColumnCount);             
    ///  table.QueryNextRow += new QueryNextRowEventHandler(table_QueryNextRow);
    ///  // Draw the table 
    ///  table.Draw(page, new PointF(0, 0));
    ///  doc.Save("Tables.pdf");
    ///  void table_QueryColumnCount(object sender, QueryColumnCountEventArgs args)
    ///  {
    ///    args.ColumnCount = 3;    
    ///  }
    /// void table_QueryNextRow(object sender, QueryNextRowEventArgs args)
    /// {
    ///   if (args.RowIndex < datastring.Length)
    ///    args.RowData = new string[] { datastring[args.RowIndex][0], datastring[args.RowIndex][1], datastring[args.RowIndex][2] }; 
    ///  }
    /// void table_QueryRowCount(object sender, QueryRowCountEventArgs args)
    /// {
    ///   args.RowCount = 2;
    /// }
    /// </code>
    /// <code lang="VB">
    /// Public datastring()() As String = New String(1)(){}
    /// ' Specify values for the table
    /// Private datastring(0) = New String() { "111", "Maxim", "100" }
    /// Private datastring(1) = New String() { "222", "Calvin", "95" }
    /// Private datastring(2) = New String() { "333", "Criss", "99" }
    /// ' Creates a new document
    /// Private doc As PdfDocument = New PdfDocument()
    /// Private page As PdfPage = doc.Pages.Add()
    /// Private table As PdfLightTable = New PdfLightTable()
    /// ' Setting the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.External
    /// 'Subscribing Events
    /// AddHandler table.QueryColumnCount, AddressOf table_QueryColumnCount
    /// AddHandler table.QueryNextRow, AddressOf table_QueryNextRow
    /// AddHandler table.QueryRowCount, AddressOf table_QueryRowCount
    /// ' Draw the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// Private Sub table_QueryColumnCount(ByVal sender As Object, ByVal args As QueryColumnCountEventArgs)
    ///   args.ColumnCount = 3
    /// End Sub
    /// Private Sub table_QueryNextRow(ByVal sender As Object, ByVal args As QueryNextRowEventArgs)
    ///  If args.RowIndex < datastring.Length Then
    ///    args.RowData = New String() { datastring(args.RowIndex)(0), datastring(args.RowIndex)(1), datastring(args.RowIndex)(2) }
    ///  End If
    ///  End Sub
    /// Private Sub table_QueryColumnCount(ByVal sender As Object, ByVal args As QueryColumnCountEventArgs)
    ///   args.ColumnCount = 3
    /// End Sub
    /// Private Sub table_QueryRowCount(ByVal sender As Object, ByVal args As QueryRowCountEventArgs)
    ///   args.RowCount = 3
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public delegate void QueryRowCountEventHandler(object sender, QueryRowCountEventArgs args);

    /// <summary>
    /// Represents StartRowLayout Event arguments.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// RectangleF rect = new RectangleF(0, 0, 500, 50);
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Roll Number", "Student Name" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Cris" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Clay" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// // Subscribe the begin row event 
    /// table.BeginRowLayout +=new BeginRowLayoutEventHandler(table_BeginRowLayout);
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    ///     // Begin Row Event Handler 
    /// void table_BeginRowLayout(object sender, BeginRowLayoutEventArgs args)
    /// {
    ///   if (args.RowIndex == 1)
    ///   {
    ///      PdfLightTable table = (PdfLightTable)sender;
    ///      int count = table.Columns.Count;
    ///      int[] spanMap = new int[count];
    ///      // Set just spanned cells. Other values are not important except negatives that are not allowed.
    ///      spanMap[0] = 2;
    ///      spanMap[1] = 3;
    ///      args.ColumnSpanMap = spanMap;
    ///      //Sets row height.
    ///      args.MinimalHeight = 30f;
    ///    }
    /// }
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Roll Number", "Student Name" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Cris" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Clay" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Subscribe the begin row event 
    /// AddHandler table.BeginRowLayout, AddressOf table_BeginRowLayout
    /// table.DataSource = dataTable
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    ///  ' Begin Row Event Handler 
    /// Private Sub table_BeginRowLayout(ByVal sender As Object, ByVal args As BeginRowLayoutEventArgs)
    ///   If args.RowIndex = 1 Then
    ///     Dim table As PdfLightTable = CType(sender, PdfLightTable)
    ///     Dim count As Integer = table.Columns.Count
    ///     Dim spanMap() As Integer = New Integer(count - 1){}
    ///     ' Set just spanned cells. Other values are not important except negatives that are not allowed.
    ///     spanMap(0) = 2
    ///     spanMap(1) = 3
    ///     args.ColumnSpanMap = spanMap
    ///     'Sets row height.
    ///     args.MinimalHeight = 30f
    ///   End If
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public class BeginRowLayoutEventArgs : EventArgs
    {
        #region Fields
        private int m_rowIndex;
        private PdfCellStyle m_cellStyle;
        private int[] m_spanMap;
        // height
        // frame
        // bounds
        private bool m_bCancel;
        private bool m_bSkip;
        private bool m_ignoreColumnFormat;
        private float m_minHeight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return m_rowIndex;
            }
        }

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        public PdfCellStyle CellStyle
        {
            get
            {
                return m_cellStyle;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("CellStyle");

                m_cellStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the span map.
        /// </summary>
        public int[] ColumnSpanMap
        {
            get
            {
                return m_spanMap;
            }
            set
            {
                m_spanMap = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether table drawing should stop.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return m_bCancel;
            }
            set
            {
                m_bCancel = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this row should be ignored.
        /// </summary>
        public bool Skip
        {
            get
            {
                return m_bSkip;
            }
            set
            {
                m_bSkip = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether column string format should be ignored.
        /// </summary>
        public bool IgnoreColumnFormat
        {
            get
            {
                return m_ignoreColumnFormat;
            }
            set
            {
                m_ignoreColumnFormat = value;
            }
        }

        /// <summary>
        /// Sets the minimal height of the row.
        /// </summary>
        public float MinimalHeight
        {
            get
            {
                return m_minHeight;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("MinimalHeight",
                        "The value can't be less then zero.");

                m_minHeight = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:StartRowLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellStyle">The cell style.</param>
        internal BeginRowLayoutEventArgs(int rowIndex, PdfCellStyle cellStyle)
        {
            m_rowIndex = rowIndex;
            m_cellStyle = cellStyle;
        }
        #endregion
    }

    /// <summary>
    /// Represents arguments of EndRowLayoutEvent.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// RectangleF rect = new RectangleF(0, 0, 500, 50);
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Roll Number", "Student Name" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Cris" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Clay" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// // Subscribe the end row event 
    /// table.EndRowLayout += new EndRowLayoutEventHandler(table_EndRowLayout);
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    ///     // End Row Event Handler 
    /// void table_EndRowLayout(object sender, EndRowLayoutEventArgs args)
    /// {
    ///    if (args.RowIndex == 1)
    ///    {
    ///       // Cancel property used to cancel the table rendering operation
    ///       args.Cancel = true;
    ///    }
    /// }
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Roll Number", "Student Name" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Cris" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Clay" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Subscribe the end row event 
    ///  AddHandler table.EndRowLayout, AddressOf table_EndRowLayout
    /// table.DataSource = dataTable
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    ///  ' Eegin Row Event Handler 
    /// Private Sub table_EndRowLayout(ByVal sender As Object, ByVal args As EndRowLayoutEventArgs)
    ///   If args.RowIndex = 1 Then
    ///      ' Cancel property used to cancel the table rendering operation
    ///      args.Cancel = True
    ///    End If
    ///  End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public class EndRowLayoutEventArgs : EventArgs
    {
        #region Fields
        private int m_rowIndex;
        private bool m_bDrawnCompletely;
        private bool m_bCancel;
        private RectangleF m_bounds;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return m_rowIndex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the row was drawn completely
        /// (nothing should be printed on the next page).
        /// </summary>
        public bool LayoutCompleted
        {
            get
            {
                return m_bDrawnCompletely;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this row should be the last one printed.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return m_bCancel;
            }
            set
            {
                m_bCancel = value;
            }
        }

        /// <summary>
        /// Gets or sets the row bounds.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EndRowLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="drawnCompletely">if set to <c>true</c> the row was drawn completely.</param>
        /// <param name="rowBounds">The row bounds.</param>
        internal EndRowLayoutEventArgs(int rowIndex, bool drawnCompletely, RectangleF rowBounds)
        {
            m_rowIndex = rowIndex;
            m_bDrawnCompletely = drawnCompletely;
            m_bounds = rowBounds;

        }
        #endregion
    }

    /// <summary>
    /// The base class for cell layout arguments.
    /// </summary>
    public abstract class CellLayoutEventArgs : EventArgs
    {
        #region Fields
        private int m_rowIndex;
        private int m_cellIndex;
        private string m_value;
        private RectangleF m_bounds;
        private PdfGraphics m_graphics;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return m_rowIndex;
            }
        }

        /// <summary>
        /// Gets the index of the cell.
        /// </summary>
        public int CellIndex
        {
            get
            {
                return m_cellIndex;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <remarks>The value might be null or an empty string,
        /// which means that either no text were acquired or all
        /// text was on the previous page.</remarks>
        public string Value
        {
            get
            {
                return m_value;
            }
        }

        /// <summary>
        /// Gets the bounds of the cell.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }
        }

        /// <summary>
        /// Gets the graphics, on which the cell should be drawn.
        /// </summary>
        public PdfGraphics Graphics
        {
            get
            {
                return m_graphics;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:StartCellLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="graphics">The graphics, on which the cell should be drawn.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellInder">The cell inder.</param>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="value">The value.</param>
        internal CellLayoutEventArgs(PdfGraphics graphics, int rowIndex, int cellInder, RectangleF bounds, string value)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            m_rowIndex = rowIndex;
            m_cellIndex = cellInder;
            m_value = value;
            m_bounds = bounds;
            m_graphics = graphics;
        }
        #endregion
    }

    /// <summary>
    /// Represents arguments of StartCellLayout Event.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// RectangleF rect = new RectangleF(0, 0, 500, 50);
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Roll Number", "Student Name" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Cris" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Clay" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// // Subscribe the cell layout event 
    /// table.BeginCellLayout += new BeginCellLayoutEventHandler(table_BeginCellLayout);
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    /// // Cell layout event handler
    /// void table_BeginCellLayout(object sender, BeginCellLayoutEventArgs args)
    /// {
    ///   if (args.RowIndex == 1)
    ///   {
    ///     args.Graphics.DrawRectangle(new PdfPen(PdfBrushes.Red, 2), PdfBrushes.White,args.Bounds);
    ///   }
    /// }       
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Roll Number", "Student Name" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Cris" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Clay" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Subscribe the cell layout event 
    /// AddHandler table.BeginCellLayout, AddressOf table_BeginCellLayout
    /// table.DataSource = dataTable
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    /// ' Cell layout event handler
    /// Private Sub table_BeginCellLayout(ByVal sender As Object, ByVal args As BeginCellLayoutEventArgs)
    ///   If args.RowIndex = 1 Then
    ///      args.Graphics.DrawRectangle(New PdfPen(PdfBrushes.Red, 2), PdfBrushes.White,args.Bounds)
    ///   End If
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public class BeginCellLayoutEventArgs : CellLayoutEventArgs
    {
        #region Fields
        private bool m_bSkip;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the value of this cell should be skipped.
        /// </summary>
        public bool Skip
        {
            get
            {
                return m_bSkip;
            }
            set
            {
                m_bSkip = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:StartCellLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="graphics">The graphics, on which the cell should be drawn.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellInder">The cell inder.</param>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="value">The value.</param>
        internal BeginCellLayoutEventArgs(PdfGraphics graphics, int rowIndex, int cellInder, RectangleF bounds, string value)
            : base(graphics, rowIndex, cellInder, bounds, value)
        {
        }
        #endregion
    }

    /// <summary>
    /// Represents arguments of EndCellLayout Event.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// RectangleF rect = new RectangleF(0, 0, 500, 50);
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Roll Number", "Student Name" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Cris" };
    /// dataTable.Rows.Add(values);
    /// values = new object[] { "011", "Clay" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// // Subscribe the cell layout event 
    /// table.EndCellLayout += new EndCellLayoutEventHandler(table_EndCellLayout);
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    /// // Cell layout event handler
    /// void table_EndCellLayout(object sender, EndCellLayoutEventArgs args)
    /// {
    ///   if (args.RowIndex == 1)
    ///   {
    ///     args.Graphics.DrawRectangle(new PdfPen(PdfBrushes.Red, 2), PdfBrushes.White, args.Bounds);
    ///   }
    /// }
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Roll Number", "Student Name" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Cris" }
    /// dataTable.Rows.Add(values)
    /// values = New Object() { "011", "Clay" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Subscribe the cell layout event 
    /// AddHandler table.EndCellLayout, AddressOf table_EndCellLayout
    /// table.DataSource = dataTable
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    /// ' Cell layout event handler
    /// Private Sub table_EndCellLayout(ByVal sender As Object, ByVal args As EndCellLayoutEventArgs)
    ///   If args.RowIndex = 1 Then
    ///     args.Graphics.DrawRectangle(New PdfPen(PdfBrushes.Red, 2), PdfBrushes.White, args.Bounds)
    ///   End If
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public class EndCellLayoutEventArgs : CellLayoutEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EndCellLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="graphics">The graphics, on which the cell should be drawn.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellInder">The cell inder.</param>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="value">The value.</param>
        internal EndCellLayoutEventArgs(PdfGraphics graphics, int rowIndex, int cellInder, RectangleF bounds, string value)
            : base(graphics, rowIndex, cellInder, bounds, value)
        {
        }
        #endregion
    }

    /// <summary>
    /// Represents arguments of the NextRow Event.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// public string[][] datastring = new string[2][];
    /// // Specify values for the table
    /// datastring[0] = new string[] { "111", "Maxim", "100" };
    /// datastring[1] = new string[] { "222", "Calvin", "95" };
    ///  // Creates a new document
    ///  PdfDocument doc = new PdfDocument();
    ///  PdfPage page = doc.Pages.Add();
    ///  PdfLightTable table = new PdfLightTable();
    ///  //// Setting the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.External;
    ///  //Subscribing Events
    ///  table.QueryColumnCount += new QueryColumnCountEventHandler(table_QueryColumnCount);             
    ///  table.QueryNextRow += new QueryNextRowEventHandler(table_QueryNextRow);
    ///  // Draw the table 
    ///  table.Draw(page, new PointF(0, 0));
    ///  doc.Save("Tables.pdf");
    ///  void table_QueryColumnCount(object sender, QueryColumnCountEventArgs args)
    ///  {
    ///    args.ColumnCount = 3;    
    ///  }
    /// void table_QueryNextRow(object sender, QueryNextRowEventArgs args)
    /// {
    ///   if (args.RowIndex < datastring.Length)
    ///    args.RowData = new string[] { datastring[args.RowIndex][0], datastring[args.RowIndex][1], datastring[args.RowIndex][2] }; 
    ///  }
    /// </code>
    /// <code lang="VB">
    /// Public datastring()() As String = New String(1)(){}
    /// ' Specify values for the table
    /// Private datastring(0) = New String() { "111", "Maxim", "100" }
    /// Private datastring(1) = New String() { "222", "Calvin", "95" }
    /// ' Creates a new document
    /// Private doc As PdfDocument = New PdfDocument()
    /// Private page As PdfPage = doc.Pages.Add()
    /// Private table As PdfLightTable = New PdfLightTable()
    /// ' Setting the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.External
    /// 'Subscribing Events
    /// AddHandler table.QueryColumnCount, AddressOf table_QueryColumnCount
    /// AddHandler table.QueryNextRow, AddressOf table_QueryNextRow
    /// ' Draw the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// Private Sub table_QueryColumnCount(ByVal sender As Object, ByVal args As QueryColumnCountEventArgs)
    ///   args.ColumnCount = 3
    /// End Sub
    /// Private Sub table_QueryNextRow(ByVal sender As Object, ByVal args As QueryNextRowEventArgs)
    ///  If args.RowIndex < datastring.Length Then
    ///    args.RowData = New String() { datastring(args.RowIndex)(0), datastring(args.RowIndex)(1), datastring(args.RowIndex)(2) }
    ///  End If
    ///  End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public class QueryNextRowEventArgs : EventArgs
    {
        #region Fields
        private string[] m_rowData;
        private int m_columnCount;
        private int m_rowIndex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the row data.
        /// </summary>
        public string[] RowData
        {
            get
            {
                return m_rowData;
            }
            set
            {
                if (m_columnCount != 0 && value != null && value.Length != m_columnCount)
                    throw new ArgumentException("The data array is not of the proper length.", "RowData");

                m_rowData = value;
            }
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return m_columnCount;
            }
        }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return m_rowIndex;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NextRowEventArgs"/> class.
        /// </summary>
        /// <param name="columnCount">The column count.</param>
        /// <param name="rowIndex">Index of the row.</param>
        internal QueryNextRowEventArgs(int columnCount, int rowIndex)
        {
            if (columnCount < 0)
                throw new ArgumentOutOfRangeException("columnCount");

            m_columnCount = columnCount;
            m_rowIndex = rowIndex;

        }
        #endregion
    }

    /// <summary>
    /// The arguments of the GettingColumnNumber Event.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// public string[][] datastring = new string[2][];
    /// // Specify values for the table
    /// datastring[0] = new string[] { "111", "Maxim", "100" };
    /// datastring[1] = new string[] { "222", "Calvin", "95" };
    ///  // Creates a new document
    ///  PdfDocument doc = new PdfDocument();
    ///  PdfPage page = doc.Pages.Add();
    ///  PdfLightTable table = new PdfLightTable();
    ///  //// Setting the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.External;
    ///  //Subscribing Events
    ///  table.QueryColumnCount += new QueryColumnCountEventHandler(table_QueryColumnCount);             
    ///  table.QueryNextRow += new QueryNextRowEventHandler(table_QueryNextRow);
    ///  // Draw the table 
    ///  table.Draw(page, new PointF(0, 0));
    ///  doc.Save("Tables.pdf");
    ///  void table_QueryColumnCount(object sender, QueryColumnCountEventArgs args)
    ///  {
    ///    args.ColumnCount = 3;    
    ///  }
    /// void table_QueryNextRow(object sender, QueryNextRowEventArgs args)
    /// {
    ///   if (args.RowIndex < datastring.Length)
    ///    args.RowData = new string[] { datastring[args.RowIndex][0], datastring[args.RowIndex][1], datastring[args.RowIndex][2] }; 
    ///  }
    /// </code>
    /// <code lang="VB">
    /// Public datastring()() As String = New String(1)(){}
    /// ' Specify values for the table
    /// Private datastring(0) = New String() { "111", "Maxim", "100" }
    /// Private datastring(1) = New String() { "222", "Calvin", "95" }
    /// ' Creates a new document
    /// Private doc As PdfDocument = New PdfDocument()
    /// Private page As PdfPage = doc.Pages.Add()
    /// Private table As PdfLightTable = New PdfLightTable()
    /// ' Setting the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.External
    /// 'Subscribing Events
    /// AddHandler table.QueryColumnCount, AddressOf table_QueryColumnCount
    /// AddHandler table.QueryNextRow, AddressOf table_QueryNextRow
    /// ' Draw the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// Private Sub table_QueryColumnCount(ByVal sender As Object, ByVal args As QueryColumnCountEventArgs)
    ///   args.ColumnCount = 3
    /// End Sub
    /// Private Sub table_QueryNextRow(ByVal sender As Object, ByVal args As QueryNextRowEventArgs)
    ///  If args.RowIndex < datastring.Length Then
    ///    args.RowData = New String() { datastring(args.RowIndex)(0), datastring(args.RowIndex)(1), datastring(args.RowIndex)(2) }
    ///  End If
    ///  End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public class QueryColumnCountEventArgs : EventArgs
    {
        #region Fields
        private int m_columnCount;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return m_columnCount;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("ColumnNumber");

                m_columnCount = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GettingColumnNumberEventArgs"/> class.
        /// </summary>
        internal QueryColumnCountEventArgs()
        {
        }
        #endregion
    }

    /// <summary>
    /// The arguments of the GettingRowNumber Event.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// public string[][] datastring = new string[3][];
    /// // Specify values for the table
    /// datastring[0] = new string[] { "111", "Maxim", "100" };
    /// datastring[1] = new string[] { "222", "Calvin", "95" };
    /// datastring[2] = new string[] { "333", "Criss", "99" };
    ///  // Creates a new document
    ///  PdfDocument doc = new PdfDocument();
    ///  PdfPage page = doc.Pages.Add();
    ///  PdfLightTable table = new PdfLightTable();
    ///  //// Setting the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.External;
    ///  //Subscribing Events
    ///  table.QueryRowCount += new QueryRowCountEventHandler(table_QueryRowCount);    /// 
    ///  table.QueryColumnCount += new QueryColumnCountEventHandler(table_QueryColumnCount);             
    ///  table.QueryNextRow += new QueryNextRowEventHandler(table_QueryNextRow);
    ///  // Draw the table 
    ///  table.Draw(page, new PointF(0, 0));
    ///  doc.Save("Tables.pdf");
    ///  void table_QueryColumnCount(object sender, QueryColumnCountEventArgs args)
    ///  {
    ///    args.ColumnCount = 3;    
    ///  }
    /// void table_QueryNextRow(object sender, QueryNextRowEventArgs args)
    /// {
    ///   if (args.RowIndex < datastring.Length)
    ///    args.RowData = new string[] { datastring[args.RowIndex][0], datastring[args.RowIndex][1], datastring[args.RowIndex][2] }; 
    ///  }
    /// void table_QueryRowCount(object sender, QueryRowCountEventArgs args)
    /// {
    ///   args.RowCount = 2;
    /// }
    /// </code>
    /// <code lang="VB">
    /// Public datastring()() As String = New String(1)(){}
    /// ' Specify values for the table
    /// Private datastring(0) = New String() { "111", "Maxim", "100" }
    /// Private datastring(1) = New String() { "222", "Calvin", "95" }
    /// Private datastring(2) = New String() { "333", "Criss", "99" }
    /// ' Creates a new document
    /// Private doc As PdfDocument = New PdfDocument()
    /// Private page As PdfPage = doc.Pages.Add()
    /// Private table As PdfLightTable = New PdfLightTable()
    /// ' Setting the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.External
    /// 'Subscribing Events
    /// AddHandler table.QueryColumnCount, AddressOf table_QueryColumnCount
    /// AddHandler table.QueryNextRow, AddressOf table_QueryNextRow
    /// AddHandler table.QueryRowCount, AddressOf table_QueryRowCount
    /// ' Draw the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// Private Sub table_QueryColumnCount(ByVal sender As Object, ByVal args As QueryColumnCountEventArgs)
    ///   args.ColumnCount = 3
    /// End Sub
    /// Private Sub table_QueryNextRow(ByVal sender As Object, ByVal args As QueryNextRowEventArgs)
    ///  If args.RowIndex < datastring.Length Then
    ///    args.RowData = New String() { datastring(args.RowIndex)(0), datastring(args.RowIndex)(1), datastring(args.RowIndex)(2) }
    ///  End If
    ///  End Sub
    /// Private Sub table_QueryColumnCount(ByVal sender As Object, ByVal args As QueryColumnCountEventArgs)
    ///   args.ColumnCount = 3
    /// End Sub
    /// Private Sub table_QueryRowCount(ByVal sender As Object, ByVal args As QueryRowCountEventArgs)
    ///   args.RowCount = 3
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfLightTable"/> Class
    public class QueryRowCountEventArgs : EventArgs
    {
        #region Fields
        private int m_rowCount;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        public int RowCount
        {
            get
            {
                return m_rowCount;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("RowNumber");

                m_rowCount = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GettingColumnNumberEventArgs"/> class.
        /// </summary>
        internal QueryRowCountEventArgs()
        {
        }
        #endregion
    }
}
