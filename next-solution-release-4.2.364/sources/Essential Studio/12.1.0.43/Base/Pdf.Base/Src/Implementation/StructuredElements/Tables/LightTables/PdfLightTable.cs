#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
#if !SILVERLIGHT && !NETFX_CORE && !WP
using System.Data;
#endif
using System.Drawing;
using System.Reflection;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;



/// <summary>
/// The Syncfusion.Pdf.Tables namespace contains classes for creating tables.
/// </summary>
namespace Syncfusion.Pdf.Tables
{
    /// <summary>
    /// Represents fast light table with few features.
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
    /// object[] values = new object[] { "Table Features Demo", "" };
    /// dataTable.Rows.Add(values);
    /// PdfPage page = doc.Pages.Add();
    /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
    /// PdfLightTable table = new PdfLightTable();
    /// table.DataSource = dataTable;
    /// table.Style.CellPadding = 16;
    /// // Draws the table in page
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
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
    /// Dim values() As Object = New Object() { "Table Features Demo", "" }
    /// dataTable.Rows.Add(values)
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// table.DataSource = dataTable	
    /// table.Style.CellPadding = 16
    /// ' Draws the table in page
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class  
    /// <seealso cref="PdfPageTemplateElement"/> Class      
    /// <seealso cref="PdfLayoutElement"/> Class  
    public class PdfLightTable : PdfLayoutElement
    {
        #region Fields
        /// <summary>
        /// Stores current column collection.
        /// </summary>
        private PdfColumnCollection m_columns;
        /// <summary>
        /// Stores current row collection.
        /// </summary>
        private PdfRowCollection m_rows;
        private object m_dataSource;
        /// <summary>
        /// Indicates the datasource type.
        /// </summary>
        private PdfLightTableDataSourceType m_dataSourceType;
        private PdfLightTableStyle m_properties;
#if !SILVERLIGHT && !NETFX_CORE && !WP
        private PdfDataSource m_dsParser;
#endif

        /// <summary>
        /// The table name from the data set.
        /// </summary>
        private string m_dataMember;
        /// <summary>
        /// Specifies whether to break the last row of the table or not when the space is not enough
        /// </summary>
        private bool m_allowRowBreakAcrossPages=true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The table column collection</value>
        /// <example>
        /// <code lang="C#">
        ///  // Creates a PDF document
        ///  PdfDocument doc = new PdfDocument();
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  PdfLightTable table = new PdfLightTable();
        ///  // Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
        /// // Gets the columns collection
        /// PdfColumnCollection tableColumns = table.Columns;           
        /// // Creating Columns
        /// tableColumns.Add(new PdfColumn("Roll Number"));
        /// tableColumns.Add(new PdfColumn("Name"));
        /// tableColumns.Add(new PdfColumn("Class"));
        /// // Adding Rows
        /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
        /// // Draws the table 
        /// table.Draw(page, new PointF(0, 0));
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// ' Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
        /// ' Gets the columns collection
        /// Dim tableColumns As PdfColumnCollection = table.Columns
        /// ' Creating Columns
        /// tableColumns.Add(New PdfColumn("Roll Number"))
        /// tableColumns.Add(New PdfColumn("Name"))
        /// tableColumns.Add(New PdfColumn("Class"))
        /// ' Adding Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class      
        /// <seealso cref="PdfLightTable"/> Class  
        public PdfColumnCollection Columns
        {
            get
            {
                if (m_columns == null)
                {
                    m_columns = CreateColumns();
                }

                return m_columns;
            }
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfLightTable table = new PdfLightTable();
        /// // Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
        /// // Gets the columns collection
        /// PdfColumnCollection tableColumns = table.Columns;           
        /// // Creating Columns
        /// tableColumns.Add(new PdfColumn("Roll Number"));
        /// tableColumns.Add(new PdfColumn("Name"));
        /// tableColumns.Add(new PdfColumn("Class"));
        /// // Gets the row collection
        /// PdfRowCollection rows = table.Rows;
        /// // Adding Rows
        /// rows.Add(new object[] { "111", "Maxim", "III" });
        /// // Draws the table 
        /// table.Draw(page, new PointF(0, 0));
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// ' Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
        /// ' Gets the columns collection
        /// Dim tableColumns As PdfColumnCollection = table.Columns
        /// ' Creating Columns
        /// tableColumns.Add(New PdfColumn("Roll Number"))
        /// tableColumns.Add(New PdfColumn("Name"))
        /// tableColumns.Add(New PdfColumn("Class"))
        /// ' Gets the row collection
        /// Dim rows As PdfRowCollection = table.Rows
        /// ' Adding Rows
        /// rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class      
        /// <seealso cref="PdfLightTable"/> Class  
        public PdfRowCollection Rows
        {
            get
            {
                if (m_rows == null)
                {
                    m_rows = CreateRows();
                }
                return m_rows;
            }
        }
        /// <summary>
        /// Gets or sets the data source.
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
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// table.Style.CellPadding = 16;
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        ///  ' Creates a new document
        ///  Dim doc As PdfDocument = New PdfDocument()
        ///  Dim rect As RectangleF = New RectangleF(0, 0, 500, 50)
        ///  'Create DataTable for source
        ///  Dim dataTable As DataTable = New DataTable("myTable")
        ///  dataTable.Columns.Add("ID1")
        ///  dataTable.Columns(0).Caption = "id"
        ///  dataTable.Columns.Add("ID2")
        ///  Dim values() As Object = New Object() { "Table Features Demo", "" }
        ///  dataTable.Rows.Add(values)
        ///  ' Create a page
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  Dim table As PdfLightTable = New PdfLightTable()
        ///  table.DataSource = dataTable
        ///  table.Style.CellPadding = 16
        ///  table.Draw(page.Graphics)
        ///  doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class      
        /// <seealso cref="PdfLightTable"/> Class  
        public object DataSource
        {
            get
            {
                return m_dataSource;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("DataSource");

                m_dataSource = value;
#if !SILVERLIGHT && !NETFX_CORE && !WP
                m_dsParser = CreateDataSourceConsumer(value);
#endif

#if SILVERLIGHT || NETFX_CORE || WP
                if(this.DataSourceType==PdfLightTableDataSourceType.External)
                {
                    SetDataSource();
                }
#else
                if (!(m_dataSource is object))
                {
                    m_dataMember = null;
                }
                if (DataSourceType != PdfLightTableDataSourceType.TableDirect)
                    m_columns = null;
#endif
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets or sets the data member.
        /// </summary>
        /// <value>The data member.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfLightTable table = new PdfLightTable();            
        /// table.DataSource = dataTable;
        /// table.DataMember = "ID1";
        /// table.Style.CellPadding = 16;
        /// //Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        ///  ' Creates a new document
        ///  Dim doc As PdfDocument = New PdfDocument()
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// table.DataMember = "ID1"
        /// table.Style.CellPadding = 16
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class      
        /// <seealso cref="PdfLightTable"/> Class  
        public string DataMember
        {
            get
            {
                return m_dataMember;
            }
            set
            {
                if (m_dataSource is DataSet)
                {
                    m_dataMember = value;
                    m_dsParser = CreateDataSourceConsumer(m_dataSource);
                }
            }
        }
#endif
        /// <summary>
        /// Gets or sets the datasource type of the PdfLightTable
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        ///  //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
        /// table.DataSource = dataTable;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Sample.pdf");
        /// </code>
        /// <code lang="VB">
        ///  ' Creates a new document
        ///  Dim doc As PdfDocument = New PdfDocument()
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
        /// table.DataSource = dataTable
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Sample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class      
        /// <seealso cref="PdfLightTable"/> Class  
        public PdfLightTableDataSourceType DataSourceType
        {
            get
            {
                return m_dataSourceType;
            }
            set
            {
                m_dataSourceType = value;
            }
        }

        /// <summary>
        /// Gets or sets the properties.
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
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// PdfPage page = doc.Pages.Add();
        /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
        /// PdfLightTable table = new PdfLightTable();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        /// // Alternative cell style
        /// PdfCellStyle altStyle = new PdfCellStyle(font, PdfBrushes.White, PdfPens.Green);
        /// altStyle.BackgroundBrush = PdfBrushes.DarkGray;
        /// // Table header cell style
        /// PdfCellStyle headerStyle = new PdfCellStyle(font, PdfBrushes.White, PdfPens.Brown);
        /// headerStyle.BackgroundBrush = PdfBrushes.Red;
        /// //Set the table style
        /// table.Style.AlternateStyle = altStyle;
        /// table.Style.HeaderStyle = headerStyle;
        /// table.DataSource = dataTable;
        /// table.Style.CellPadding = 16;
        /// // Draws the table in page
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
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
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Alternative cell style
        /// Dim altStyle As PdfCellStyle = New PdfCellStyle(font, PdfBrushes.White, PdfPens.Green)
        /// altStyle.BackgroundBrush = PdfBrushes.DarkGray
        /// ' Table header cell style
        /// Dim headerStyle As PdfCellStyle = New PdfCellStyle(font, PdfBrushes.White, PdfPens.Brown)
        /// headerStyle.BackgroundBrush = PdfBrushes.Red
        /// ' Set the table style
        /// table.Style.AlternateStyle = altStyle
        /// table.Style.HeaderStyle = headerStyle
        /// table.DataSource = dataTable	
        /// table.Style.CellPadding = 16
        /// ' Draws the table in page
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPageTemplateElement"/> Class      
        /// <seealso cref="PdfLayoutElement"/> Class  
        public PdfLightTableStyle Style
        {
            get
            {
                if (m_properties == null)
                {
                    m_properties = new PdfLightTableStyle();
                }

                return m_properties;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Properties");

                m_properties = value;
            }
        }
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets or sets a value indicating whether
        /// PdfLightTable should ignore sorting in data table.
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
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// PdfPage page = doc.Pages.Add();
        /// PdfPageTemplateElement top = new PdfPageTemplateElement(rect);
        /// PdfLightTable table = new PdfLightTable();
        /// // Disabling sorting 
        /// table.IgnoreSorting = true;
        /// table.DataSource = dataTable;
        /// table.Style.CellPadding = 16;
        /// // Draws the table in page
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
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
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim top As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// ' Disabling sorting 
        /// table.IgnoreSorting = True
        /// table.DataSource = dataTable	
        /// table.Style.CellPadding = 16
        /// ' Draws the table in page
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPageTemplateElement"/> Class      
        /// <seealso cref="PdfLayoutElement"/> Class      
        public bool IgnoreSorting
        {
            get
            {
                bool result = true;

                if (m_dsParser != null)
                {
                    result = m_dsParser.UseSorting;
                }

                return result;
            }
            set
            {
                if (m_dsParser != null)
                {
                    m_dsParser.UseSorting = !value;
                }
            }
        }
#else
        
        public void SetDataSource()
        {
            if (m_dataSource is IEnumerable)
            {
                int i = 0;
                PdfRow row;
                PropertyInfo[] props = null;
                foreach (object obj in (m_dataSource as IEnumerable))
                {
                    if (obj != null)
                    {
#if NETFX_CORE || WP
                        IEnumerable<PropertyInfo> enumerable = obj.GetType().GetRuntimeProperties();
                        props = new List<PropertyInfo>(enumerable).ToArray();
#else
                        props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
#endif
                        foreach (var prop in props)
                        {
                            string header = prop.Name;
                            PdfColumn column = new PdfColumn(header);
                            this.Columns.Add(column);
                        }
                        break;
                    }
                }
                int columnCount=Columns.Count;
                List<String> rowValues = new List<string>();
                foreach (var item in (m_dataSource as IEnumerable))
                {
                    row = new PdfRow();
                    rowValues = new List<string>();
                    foreach (var prop in props)
                    {
                        Type currentRecordType = item.GetType();
#if NETFX_CORE || WP
                        PropertyInfo property = currentRecordType.GetRuntimeProperty(prop.Name);
#else
                        PropertyInfo property = currentRecordType.GetProperty(prop.Name);
#endif
                        rowValues.Add(Convert.ToString(property.GetValue(item, null)));
                    }
                    row.Values = rowValues.ToArray();
                    Rows.Add(row);
                }
            }
            
        }
#endif
        /// <summary>
        /// Gets a value indicating whether to raise start row layout event.
        /// </summary>
        internal bool RaiseBeginRowLayout
        {
            get
            {
                return (BeginRowLayout != null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether to raise end row layout event.
        /// </summary>
        internal bool RaiseEndRowLayout
        {
            get
            {
                return (EndRowLayout != null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the start cell layout event should be raised.
        /// </summary>
        internal bool RaiseBeginCellLayout
        {
            get
            {
                return (BeginCellLayout != null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the end cell layout event should be raised.
        /// </summary>
        internal bool RaiseEndCellLayout
        {
            get
            {
                return (EndCellLayout != null);
            }
        }

        ///<summary>
        ///Gets a value indicating the row break is to be made or not
        ///</summary>
        public bool AllowRowBreakAcrossPages
        {
            get
            {
                return m_allowRowBreakAcrossPages;
            }
            set
            {
                m_allowRowBreakAcrossPages = value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// The event raised on starting row lay outing.
        /// </summary>
        public event BeginRowLayoutEventHandler BeginRowLayout;

        /// <summary>
        /// The event raised on having finished row lay outing.
        /// </summary>
        public event EndRowLayoutEventHandler EndRowLayout;

        /// <summary>
        /// The event raised on starting cell lay outing.
        /// </summary>
        public event BeginCellLayoutEventHandler BeginCellLayout;

        /// <summary>
        /// The event raised on having finished cell layout.
        /// </summary>
        public event EndCellLayoutEventHandler EndCellLayout;

        /// <summary>
        /// The event raised when the next row data is requested.
        /// </summary>
        public event QueryNextRowEventHandler QueryNextRow;

        /// <summary>
        /// The event raised when the column number is requested.
        /// </summary>
        public event QueryColumnCountEventHandler QueryColumnCount;
        /// <summary>
        /// The event raised when the row number is requested.
        /// </summary>
        public event QueryRowCountEventHandler QueryRowCount;
        #endregion

        #region Public methods
        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        /// <param name="location">The location of the element.</param>
        /// <param name="width">The width of the table.</param>
        public void Draw(PdfGraphics graphics, PointF location, float width)
        {
            Draw(graphics, location.X, location.Y, width);
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        /// <param name="x">X co-ordinate of the element.</param>
        /// <param name="y">Y co-ordinate of the element.</param>
        /// <param name="width">The width of the table.</param>
        public void Draw(PdfGraphics graphics, float x, float y, float width)
        {
            RectangleF boundaries = new RectangleF(x, y, width, 0);
            Draw(graphics, boundaries);
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        /// <param name="bounds">The bounds.</param>
        public void Draw(PdfGraphics graphics, RectangleF bounds)
        {
            if (m_dataSourceType == PdfLightTableDataSourceType.TableDirect)
                DataSource = FillData();

            LightTableLayouter layouter = new LightTableLayouter(this);
            layouter.Layout(graphics, bounds);
        }

        /// <summary>
        /// Draws the table starting from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="location">The location.</param>
        /// <returns>The results of the lay outing.</returns>
        new public PdfLightTableLayoutResult Draw(PdfPage page, PointF location)
        {
            if (m_dataSourceType == PdfLightTableDataSourceType.TableDirect)
                DataSource = FillData();

            PdfLayoutResult lr = base.Draw(page, location);
            return (PdfLightTableLayoutResult)lr;
        }

        /// <summary>
        /// Draws the table starting from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="location">The location.</param>
        /// <param name="format">The format.</param>
        /// <returns>The results of the lay outing.</returns>
        public PdfLightTableLayoutResult Draw(PdfPage page, PointF location,
            PdfLightTableLayoutFormat format)
        {
            if (m_dataSourceType == PdfLightTableDataSourceType.TableDirect)
                DataSource = FillData();

            PdfLayoutResult lr = base.Draw(page, location, format);
            return (PdfLightTableLayoutResult)lr;
        }

        /// <summary>
        /// Draws the table starting from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns>The results of the lay outing.</returns>
        new public PdfLightTableLayoutResult Draw(PdfPage page, RectangleF bounds)
        {
            if (m_dataSourceType == PdfLightTableDataSourceType.TableDirect)
                DataSource = FillData();

            PdfLayoutResult lr = base.Draw(page, bounds);
            return (PdfLightTableLayoutResult)lr;
        }

        /// <summary>
        /// Draws the table starting from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="format">The format.</param>
        /// <returns>The results of the lay outing.</returns>
        public PdfLightTableLayoutResult Draw(PdfPage page, RectangleF bounds,
            PdfLightTableLayoutFormat format)
        {
            if (m_dataSourceType == PdfLightTableDataSourceType.TableDirect)
                DataSource = FillData();

            PdfLayoutResult lr = base.Draw(page, bounds, format);
            return (PdfLightTableLayoutResult)lr;
        }

        /// <summary>
        /// Draws the table starting from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The results of the lay outing.</returns>
        new public PdfLightTableLayoutResult Draw(PdfPage page, float x, float y)
        {
            if (m_dataSourceType == PdfLightTableDataSourceType.TableDirect)
                DataSource = FillData();

            PdfLayoutResult lr = base.Draw(page, x, y);
            return (PdfLightTableLayoutResult)lr;
        }

        /// <summary>
        /// Draws the table starting from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="format">The format.</param>
        /// <returns>The results of the lay outing.</returns>
        public PdfLightTableLayoutResult Draw(PdfPage page,
            float x, float y, PdfLightTableLayoutFormat format)
        {
            if (m_dataSourceType == PdfLightTableDataSourceType.TableDirect)
                DataSource = FillData();

            PdfLayoutResult lr = base.Draw(page, x, y, format);
            return (PdfLightTableLayoutResult)lr;
        }

        /// <summary>
        /// Draws the table starting from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <returns>The results of the lay outing.</returns>
        public PdfLightTableLayoutResult Draw(PdfPage page,
            float x, float y, float width)
        {
            return Draw(page, x, y, width, null);
        }

        /// <summary>
        /// Draws the table starting from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="format">The format.</param>
        /// <returns>The results of the lay outing.</returns>
        public PdfLightTableLayoutResult Draw(PdfPage page,
            float x, float y, float width, PdfLightTableLayoutFormat format)
        {

            if (m_dataSourceType == PdfLightTableDataSourceType.TableDirect)
                DataSource = FillData();

            RectangleF rect = new RectangleF(x, y, width, 0);
            PdfLayoutResult lr = base.Draw(page, rect, format);
            return (PdfLightTableLayoutResult)lr;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        /// <param name="x">X co-ordinate of the element.</param>
        /// <param name="y">Y co-ordinate of the element.</param>
        public override void Draw(PdfGraphics graphics, float x, float y)
        {
            SizeF size = graphics.ClientSize;
            size.Width -= x;
            size.Height -= y;

            Draw(graphics, x, y, size.Width);
        }

        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Returns lay outing results.</returns>
        protected override PdfLayoutResult Layout(PdfLayoutParams param)
        {
            if (param.Bounds.Width < 0)
                throw new ArgumentOutOfRangeException("Width");

            LightTableLayouter layouter = new LightTableLayouter(this);
            PdfLightTableLayoutResult result = (PdfLightTableLayoutResult)layouter.Layout(param);

            return result;
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        protected override void DrawInternal(PdfGraphics graphics)
        {
            LightTableLayouter layouter = new LightTableLayouter(this);
            layouter.Layout(graphics, PointF.Empty);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:BeginRowLayout"/> event.
        /// </summary>
        /// <param name="args">The <see cref="T:Syncfusion.Pdf.Tables.StartRowLayoutEventArgs"/>
        /// instance containing the event data.</param>
        internal void OnBeginRowLayout(BeginRowLayoutEventArgs args)
        {
            if (RaiseBeginRowLayout)
            {
                BeginRowLayout(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:EndRowLayout"/> event.
        /// </summary>
        /// <param name="args">The <see cref="T:Syncfusion.Pdf.Tables.EndRowLayoutEventArgs"/>
        /// instance containing the event data.</param>
        internal void OnEndRowLayout(EndRowLayoutEventArgs args)
        {
            if (RaiseEndRowLayout)
            {
                EndRowLayout(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:BeginCellLayout"/> event.
        /// </summary>
        /// <param name="args">The <see cref="T:Syncfusion.Pdf.Tables.StartCellLayoutEventArgs"/>
        /// instance containing the event data.</param>
        internal void OnBeginCellLayout(BeginCellLayoutEventArgs args)
        {
            if (RaiseBeginCellLayout)
            {
                BeginCellLayout(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:EndCellLayout"/> event.
        /// </summary>
        /// <param name="args">The <see cref="T:Syncfusion.Pdf.Tables.EndCellLayoutEventArgs"/>
        /// instance containing the event data.</param>
        internal void OnEndCellLayout(EndCellLayoutEventArgs args)
        {
            if (RaiseEndCellLayout)
            {
                EndCellLayout(this, args);
            }
        }

        /// <summary>
        /// Gets the next row.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The array of the strings.</returns>
        internal string[] GetNextRow(ref int index)
        {
#if !SILVERLIGHT && !NETFX_CORE && !WP
            string[] arr = (m_dsParser != null) ? m_dsParser.GetRow(ref index) : OnGetNextRow(index);
#else

            string[] arr = null;
            arr = this.Rows[index].Values as string[];
#endif
            return arr;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets the column captions.
        /// </summary>
        /// <returns>Column captions</returns>
        internal string[] GetColumnCaptions()
        {
            PdfColumnCollection columns = Columns;

            string[] captions = (m_dsParser != null) ? m_dsParser.ColumnCaptions : null;

            for (int i = 0; i < m_dsParser.ColumnCount; i++)
            {
                if (columns[i].ColumnName != null)
                {
                    if (captions == null)
                        captions = new string[m_dsParser.ColumnCount];

                    captions[i] = columns[i].ColumnName;
                }
            }

            return captions;
        }
#else
        internal string[] GetColumnCaptions()
        {
            PdfColumnCollection columns = Columns;
            List<string> tempCaption = new List<string>();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].ColumnName != null)
                    tempCaption.Add(Columns[i].ColumnName);
            }
            return tempCaption.ToArray();
        }
#endif
        /// <summary>
        /// Creates a data source consumer.
        /// </summary>
        /// <param name="value">The data source.</param>
        /// <returns>The proper data source consumer</returns>
#if !SILVERLIGHT && !NETFX_CORE && !WP
        private PdfDataSource CreateDataSourceConsumer(object value)
        {
            Array array = value as Array;
            DataSet dataSet = value as DataSet;
            DataColumn dataColumn = value as DataColumn;
            DataTable dataTable = value as DataTable;
            DataView dataView = value as DataView;
            IEnumerable customSource = value as IEnumerable;

            PdfDataSource ds = null;

            if (array != null)
            {
                ds = new PdfDataSource(array);
            }
            else if (dataColumn != null)
            {
                ds = new PdfDataSource(dataColumn);
            }
            else if (dataTable != null)
            {
                ds = new PdfDataSource(dataTable);
            }
            else if (dataView != null)
            {
                ds = new PdfDataSource(dataView);
            }
            else if (dataSet != null)
            {
                ds = new PdfDataSource(dataSet, m_dataMember);
            }
            else if (customSource != null)
            {

            }
            

            return ds;
        }

#endif
        /// <summary>
        /// Initializes the PdfLightTable data source.
        /// </summary>

        private object FillData()
        {
#if SILVERLIGHT || NETFX_CORE || WP
            return 0;
#else
            try
            {

                DataTable dt = new DataTable();
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (Columns[i].ColumnName != null)
                        dt.Columns.Add(Columns[i].ColumnName);
                    else
                        dt.Columns.Add(string.Empty);

                    this.Columns[i].Width = Columns[i].Width;
                    this.Columns[i].StringFormat = Columns[i].StringFormat;
                }

                foreach (PdfRow rows in Rows)
                {
                    if (rows.Values != null)
                        dt.Rows.Add(rows.Values);

                }
                return dt;
            }
            catch (Exception msg)
            {
                throw new PdfException("Please check whether the number of rows matches the column count.", msg);
            }
#endif
        }


        /// <summary>
        /// Creates the columns.
        /// </summary>
        /// <returns>The filled column collection.</returns>
        private PdfColumnCollection CreateColumns()
        {
            PdfColumnCollection columns = new PdfColumnCollection();
#if SILVERLIGHT || NETFX_CORE || WP
            int count = columns.Count;
            for (int i = 0; i < count; ++i)
            {
                PdfColumn column = new PdfColumn(10); // Creates a column with the default width.
                columns.Add(column);
            }

            return columns;
#else
            int count = (m_dsParser != null) ? m_dsParser.ColumnCount : OnGetColumnNumber();

            for (int i = 0; i < count; ++i)
            {
                PdfColumn column = new PdfColumn(10); // Creates a column with the default width.
                columns.Add(column);
            }

            return columns;
#endif
        }

        /// <summary>
        /// Creates the row.
        /// </summary>
        /// <returns>The filled row collection.</returns>
        private PdfRowCollection CreateRows()
        {
            PdfRowCollection rows = new PdfRowCollection();
#if SILVERLIGHT ||NETFX_CORE || WP
            int count = rows.Count;
            for (int i = 0; i < count; ++i)
            {
                PdfRow row = new PdfRow();
                rows.Add(row);
            }
            return rows;
#else
            int count = (m_dsParser != null) ? m_dsParser.RowCount : OnGetRowNumber();
            for (int i = 0; i < count; ++i)
            {
                PdfRow row = new PdfRow();
                rows.Add(row);
            }
            return rows;
#endif

        }

        /// <summary>
        /// Called when geting next row.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The row data passed by the user.</returns>
        private string[] OnGetNextRow(int rowIndex)
        {
            string[] data = null;

            if (QueryNextRow != null)
            {
                QueryNextRowEventArgs args = new QueryNextRowEventArgs(Columns.Count, rowIndex);

                QueryNextRow(this, args);
                data = args.RowData;
            }

            return data;
        }

        /// <summary>
        /// Called when getting column number.
        /// </summary>
        /// <returns>The number provided by the user.</returns>
        private int OnGetColumnNumber()
        {
            int value = 0;

            if (QueryColumnCount != null)
            {
                QueryColumnCountEventArgs args = new QueryColumnCountEventArgs();

                QueryColumnCount(this, args);
                value = args.ColumnCount;
            }



            if (value < 0)
                throw new PdfLightTableException("There is no columns.");

            return value;
        }

        /// <summary>
        /// Called when getting row number.
        /// </summary>
        /// <returns>The number provided by the user.</returns>
        private int OnGetRowNumber()
        {
            int value = 0;

            if (QueryColumnCount != null)
            {
                QueryRowCountEventArgs args = new QueryRowCountEventArgs();
                QueryRowCount(this, args);
                value = args.RowCount;

            }

            if (value < 0)
                throw new PdfLightTableException("There is no Rows.");

            return value;
        }
        #endregion
    }

    /// <summary>
    /// Represents parameters of PdfLightTable.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// //Set font
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
    /// //Create Pdf ben for drawing border
    /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
    /// borderPen.Width = 0;
    /// //Create brush
    /// PdfColor color = new PdfColor(192, 201, 219);
    /// PdfSolidBrush brush = new PdfSolidBrush(color);
    /// //Create alternative cell styles
    /// PdfCellStyle altStyle = new PdfCellStyle();
    /// altStyle.Font = font;
    /// altStyle.BackgroundBrush = brush;
    /// altStyle.BorderPen = borderPen;
    /// // Creates default cell style
    /// PdfCellStyle defStyle = new PdfCellStyle();
    /// defStyle.Font = font;
    /// defStyle.BackgroundBrush = PdfBrushes.White;
    /// defStyle.BorderPen = borderPen;
    /// // Creates header cell style
    /// PdfCellStyle headerStyle = new PdfCellStyle(font, PdfBrushes.White, PdfPens.DarkBlue);
    /// brush = new PdfSolidBrush(Color.FromArgb(33, 67, 126));
    /// headerStyle.BackgroundBrush = brush;
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Table Features Demo", "" };
    /// dataTable.Rows.Add(values);
    /// PdfLightTable table = new PdfLightTable();
    /// table.DataSource = dataTable;
    /// // Set the cell style
    /// table.Style.AlternateStyle = altStyle;
    /// table.Style.DefaultStyle = defStyle;
    /// table.Style.HeaderStyle = headerStyle;
    /// // Draws the table
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create PDF document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// 'Set font
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
    /// 'Create Pdf ben for drawing border
    /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
    /// borderPen.Width = 0
    /// 'Create brush
    /// Dim color As PdfColor = New PdfColor(192, 201, 219)
    /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
    /// 'Create alternative cell styles
    /// Dim altStyle As PdfCellStyle = New PdfCellStyle()
    /// altStyle.Font = font
    /// altStyle.BackgroundBrush = brush
    /// altStyle.BorderPen = borderPen
    /// ' Creates default cell style
    /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
    /// defStyle.Font = font
    /// defStyle.BackgroundBrush = PdfBrushes.White
    /// defStyle.BorderPen = borderPen
    /// ' Creates header cell style
    /// Dim headerStyle As PdfCellStyle = New PdfCellStyle(font, PdfBrushes.White, PdfPens.DarkBlue)
    /// brush = New PdfSolidBrush(Color.FromArgb(33, 67, 126))
    /// headerStyle.BackgroundBrush = brush
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Table Features Demo", "" }
    /// dataTable.Rows.Add(values)
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// table.DataSource = dataTable
    /// ' Set cell style
    /// table.Style.AlternateStyle = altStyle
    /// table.Style.DefaultStyle = defStyle
    /// table.Style.HeaderStyle = headerStyle
    /// ' Draws the table
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class  
    /// <seealso cref="PdfPageTemplateElement"/> Class      
    /// <seealso cref="PdfLightTable"/> Class  
    public class PdfLightTableStyle
    {
        #region Fields
        private PdfCellStyle m_defaultStyle;
        private PdfCellStyle m_alternateStyle;
        private PdfHeaderSource m_headerSource;
        private int m_headerRowCount;
        private PdfCellStyle m_headerStyle;
        private bool m_bRepeateHeader;
        private bool m_bShowHeader;
        private float m_cellSpacing;
        private float m_cellPadding;
        private PdfBorderOverlapStyle m_overlappedBorders;
        private PdfPen m_borderPen;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the default cell style.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);
        /// //Create alternative cell styles
        /// PdfCellStyle altStyle = new PdfCellStyle();
        /// altStyle.Font = font;
        /// altStyle.BackgroundBrush = brush;
        /// altStyle.BorderPen = borderPen;
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.Font = font;
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;
        /// // Creates header cell style
        /// PdfCellStyle headerStyle = new PdfCellStyle(font, PdfBrushes.White, PdfPens.DarkBlue);
        /// brush = new PdfSolidBrush(Color.FromArgb(33, 67, 126));
        /// headerStyle.BackgroundBrush = brush;
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// // Set the cell style
        /// table.Style.AlternateStyle = altStyle;
        /// table.Style.DefaultStyle = defStyle;
        /// table.Style.HeaderStyle = headerStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// 'Create alternative cell styles
        /// Dim altStyle As PdfCellStyle = New PdfCellStyle()
        /// altStyle.Font = font
        /// altStyle.BackgroundBrush = brush
        /// altStyle.BorderPen = borderPen
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.BorderPen = borderPen
        /// ' Creates header cell style
        /// Dim headerStyle As PdfCellStyle = New PdfCellStyle(font, PdfBrushes.White, PdfPens.DarkBlue)
        /// brush = New PdfSolidBrush(Color.FromArgb(33, 67, 126))
        /// headerStyle.BackgroundBrush = brush
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// ' Set cell style
        /// table.Style.AlternateStyle = altStyle
        /// table.Style.DefaultStyle = defStyle
        /// table.Style.HeaderStyle = headerStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPageTemplateElement"/> Class      
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfCellStyle DefaultStyle
        {
            get
            {
                return m_defaultStyle;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("DefaultStyle");

                m_defaultStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the alternate style, which is the style of the odd rows.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);
        /// //Create alternative cell styles
        /// PdfCellStyle altStyle = new PdfCellStyle();
        /// altStyle.Font = font;
        /// altStyle.BackgroundBrush = brush;
        /// altStyle.BorderPen = borderPen;
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.Font = font;
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;
        /// // Creates header cell style
        /// PdfCellStyle headerStyle = new PdfCellStyle(font, PdfBrushes.White, PdfPens.DarkBlue);
        /// brush = new PdfSolidBrush(Color.FromArgb(33, 67, 126));
        /// headerStyle.BackgroundBrush = brush;
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// // Set the cell style
        /// table.Style.AlternateStyle = altStyle;
        /// table.Style.DefaultStyle = defStyle;
        /// table.Style.HeaderStyle = headerStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// 'Create alternative cell styles
        /// Dim altStyle As PdfCellStyle = New PdfCellStyle()
        /// altStyle.Font = font
        /// altStyle.BackgroundBrush = brush
        /// altStyle.BorderPen = borderPen
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.BorderPen = borderPen
        /// ' Creates header cell style
        /// Dim headerStyle As PdfCellStyle = New PdfCellStyle(font, PdfBrushes.White, PdfPens.DarkBlue)
        /// brush = New PdfSolidBrush(Color.FromArgb(33, 67, 126))
        /// headerStyle.BackgroundBrush = brush
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// ' Set cell style
        /// table.Style.AlternateStyle = altStyle
        /// table.Style.DefaultStyle = defStyle
        /// table.Style.HeaderStyle = headerStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPageTemplateElement"/> Class      
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfCellStyle AlternateStyle
        {
            get
            {
                return m_alternateStyle;
            }
            set
            {
                m_alternateStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// to use rows or column captions for forming header.
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
        public PdfHeaderSource HeaderSource
        {
            get
            {
                return m_headerSource;
            }
            set
            {
                m_headerSource = value;
            }
        }

        /// <summary>
        /// Gets or sets the header rows count.
        /// </summary>
        /// <example>
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
        /// table.Style.ShowHeader = true;
        /// table.Style.HeaderRowCount = 2;
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
        /// table.Style.ShowHeader = True
        /// table.Style.HeaderRowCount = 2
        /// ' Add new Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class 
        public int HeaderRowCount
        {
            get
            {
                return m_headerRowCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("HeaderRowsCount", "This parameter can't be less then zero");

                m_headerRowCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the header cell style.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);
        /// //Create alternative cell styles
        /// PdfCellStyle altStyle = new PdfCellStyle();
        /// altStyle.Font = font;
        /// altStyle.BackgroundBrush = brush;
        /// altStyle.BorderPen = borderPen;
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.Font = font;
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;
        /// // Creates header cell style
        /// PdfCellStyle headerStyle = new PdfCellStyle(font, PdfBrushes.White, PdfPens.DarkBlue);
        /// brush = new PdfSolidBrush(Color.FromArgb(33, 67, 126));
        /// headerStyle.BackgroundBrush = brush;
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// // Set the cell style
        /// table.Style.AlternateStyle = altStyle;
        /// table.Style.DefaultStyle = defStyle;
        /// table.Style.HeaderStyle = headerStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// 'Create alternative cell styles
        /// Dim altStyle As PdfCellStyle = New PdfCellStyle()
        /// altStyle.Font = font
        /// altStyle.BackgroundBrush = brush
        /// altStyle.BorderPen = borderPen
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.BorderPen = borderPen
        /// ' Creates header cell style
        /// Dim headerStyle As PdfCellStyle = New PdfCellStyle(font, PdfBrushes.White, PdfPens.DarkBlue)
        /// brush = New PdfSolidBrush(Color.FromArgb(33, 67, 126))
        /// headerStyle.BackgroundBrush = brush
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// ' Set cell style
        /// table.Style.AlternateStyle = altStyle
        /// table.Style.DefaultStyle = defStyle
        /// table.Style.HeaderStyle = headerStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPageTemplateElement"/> Class      
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfCellStyle HeaderStyle
        {
            get
            {
                return m_headerStyle;
            }
            set
            {
                m_headerStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to repeat header on each page.
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
        /// table.Style.ShowHeader = true;
        /// table.Style.HeaderRowCount = 2;
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
        /// table.Style.ShowHeader = True
        /// table.Style.HeaderRowCount = 2
        /// ' Add new Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class  
        public bool RepeatHeader
        {
            get
            {
                return m_bRepeateHeader;
            }
            set
            {
                m_bRepeateHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the header is visible.
        /// </summary>
        /// <remarks>If the header is made up with ordinary rows they aren't visible
        /// while this property is set to false.</remarks>
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
        /// table.Style.ShowHeader = true;
        /// table.Style.HeaderRowCount = 2;
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
        /// table.Style.ShowHeader = True
        /// table.Style.HeaderRowCount = 2
        /// ' Add new Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class  
        public bool ShowHeader
        {
            get
            {
                return m_bShowHeader;
            }
            set
            {
                m_bShowHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets the cell spacing.
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
        ///  //Set the cell spacing
        /// table.Style.CellSpacing = 10;
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
        ///  'Set the cell spacing
        /// table.Style.CellSpacing = 10
        /// ' Add new Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class  
        public float CellSpacing
        {
            get
            {
                return m_cellSpacing;
            }
            set
            {
                m_cellSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the cell padding.
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
        /// // Set the cell padding
        /// table.Style.CellPadding = 8;
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
        /// ' Set the cell padding
        /// table.Style.CellPadding = 8
        /// ' Add new Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class
        public float CellPadding
        {
            get
            {
                return m_cellPadding;
            }
            set
            {
                m_cellPadding = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell borders
        /// should overlap its neighbour's borders or be drawn in the cell interior.
        /// </summary>
        /// <remarks>Please, use this property with caution,
        /// because it might cause unexpected results if borders
        /// are not the same width and colour.</remarks>
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
        public PdfBorderOverlapStyle BorderOverlapStyle
        {
            get
            {
                return m_overlappedBorders;
            }
            set
            {
                m_overlappedBorders = value;
            }
        }

        /// <summary>
        /// Gets or sets the pen of the table border.
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
        /// table.Style.BorderPen = new PdfPen(PdfBrushes.BlueViolet);
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
        /// ' Set the border pen for table
        /// table.Style.BorderPen = New PdfPen(PdfBrushes.BlueViolet)
        /// ' Add new Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfColumn"/> Class 
        public PdfPen BorderPen
        {
            get
            {
                return m_borderPen;
            }
            set
            {
                m_borderPen = value;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLightTableStyle"/> class.
        /// </summary>
        public PdfLightTableStyle()
        {
            m_defaultStyle = new PdfCellStyle();
            m_bRepeateHeader = true;
            m_overlappedBorders = PdfBorderOverlapStyle.Overlap;
        }
        #endregion
    }

    /// <summary>
    /// Represents information about cell style.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// //Set font
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
    /// //Create Pdf ben for drawing border
    /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
    /// borderPen.Width = 0;
    /// //Create brush
    /// PdfColor color = new PdfColor(192, 201, 219);
    /// PdfSolidBrush brush = new PdfSolidBrush(color);           
    /// // Creates default cell style
    /// PdfCellStyle defStyle = new PdfCellStyle();
    /// defStyle.Font = font;
    /// defStyle.BackgroundBrush = PdfBrushes.White;
    /// defStyle.BorderPen = borderPen;          
    /// //Create DataTable for source
    /// DataTable dataTable = new DataTable("myTable");
    /// dataTable.Columns.Add("ID1");
    /// dataTable.Columns[0].Caption = "id";
    /// dataTable.Columns.Add("ID2");
    /// object[] values = new object[] { "Table Features Demo", "" };
    /// dataTable.Rows.Add(values);
    /// // Creates a new table
    /// PdfLightTable table = new PdfLightTable();
    /// table.DataSource = dataTable;
    /// //Set the cell style
    /// table.Style.DefaultStyle = defStyle;
    /// // Draws the table
    /// table.Draw(page.Graphics);
    /// doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create PDF document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// 'Set font
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
    /// 'Create Pdf ben for drawing border
    /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
    /// borderPen.Width = 0
    /// 'Create brush
    /// Dim color As PdfColor = New PdfColor(192, 201, 219)
    /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
    /// ' Creates default cell style
    /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
    /// defStyle.Font = font
    /// defStyle.BackgroundBrush = PdfBrushes.White
    /// defStyle.BorderPen = borderPen
    /// 'Create DataTable for source
    /// Dim dataTable As DataTable = New DataTable("myTable")
    /// dataTable.Columns.Add("ID1")
    /// dataTable.Columns(0).Caption = "id"
    /// dataTable.Columns.Add("ID2")
    /// Dim values() As Object = New Object() { "Table Features Demo", "" }
    /// dataTable.Rows.Add(values)
    /// ' Creates a new table
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// table.DataSource = dataTable
    /// 'Set the cell style
    /// table.Style.DefaultStyle = defStyle
    /// ' Draws the table
    /// table.Draw(page.Graphics)
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class  
    /// <seealso cref="PdfLightTable"/> Class 
    public class PdfCellStyle
    {
        #region Fields
        private PdfFont m_font;
        private PdfStringFormat m_stringFormat;
        private PdfPen m_textPen;
        private PdfBrush m_textBrush;
        private PdfPen m_borderPen;
        private PdfBrush m_backgroundBrush;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);           
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.Font = font;
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;          
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// // Creates a new table
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// //Set the cell style
        /// table.Style.DefaultStyle = defStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.BorderPen = borderPen
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// ' Creates a new table
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// 'Set the cell style
        /// table.Style.DefaultStyle = defStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfFont Font
        {
            get
            {
                if (m_font == null)
                {
                    m_font = PdfDocument.DefaultFont;
                }

                return m_font;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Font");

                m_font = value;
            }
        }

        /// <summary>
        /// Gets or sets the string format of the cell text.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);           
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.Font = font;
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;   
        /// defStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Justify);
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// // Creates a new table
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// //Set the cell style
        /// table.Style.DefaultStyle = defStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.StringFormat = New PdfStringFormat(PdfTextAlignment.Justify)
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.BorderPen = borderPen
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// ' Creates a new table
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// 'Set the cell style
        /// table.Style.DefaultStyle = defStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfStringFormat StringFormat
        {
            get
            {
                return m_stringFormat;
            }
            set
            {
                m_stringFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the font which will be used to draw text outlines.
        /// </summary>
        /// <remarks>It should be null for default text representation.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);           
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.TextPen = new PdfPen(PdfBrushes.BlueViolet);
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;   
        /// defStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Justify);
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// // Creates a new table
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// //Set the cell style
        /// table.Style.DefaultStyle = defStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.TextPen = New PdfPen(PdfBrushes.BlueViolet)
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.BorderPen = borderPen
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// ' Creates a new table
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// 'Set the cell style
        /// table.Style.DefaultStyle = defStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfPen TextPen
        {
            get
            {
                return m_textPen;
            }
            set
            {
                m_textPen = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush which will be used to draw font.
        /// </summary>
        /// <remarks>This brush will be used to fill glyphs interior, which is the default.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);           
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.Font = font;
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;          
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// // Creates a new table
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// //Set the cell style
        /// table.Style.DefaultStyle = defStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.TextBrush = PdfBrushes.Brown;
        /// defStyle.BorderPen = borderPen
        /// defStyle.TextBrush = PdfBrushes.Brown
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// ' Creates a new table
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// 'Set the cell style
        /// table.Style.DefaultStyle = defStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfBrush TextBrush
        {
            get
            {
                return m_textBrush;
            }
            set
            {
                m_textBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the pen with which the border will be drawn.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);           
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.Font = font;
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;          
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// // Creates a new table
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// //Set the cell style
        /// table.Style.DefaultStyle = defStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.BorderPen = borderPen
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// ' Creates a new table
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// 'Set the cell style
        /// table.Style.DefaultStyle = defStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfPen BorderPen
        {
            get
            {
                return m_borderPen;
            }
            set
            {
                m_borderPen = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush with which the background will be drawn.
        /// </summary>
        /// <remarks>It's null by default.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Create PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// //Set font
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        /// //Create Pdf ben for drawing border
        /// PdfPen borderPen = new PdfPen(PdfBrushes.DarkBlue);
        /// borderPen.Width = 0;
        /// //Create brush
        /// PdfColor color = new PdfColor(192, 201, 219);
        /// PdfSolidBrush brush = new PdfSolidBrush(color);           
        /// // Creates default cell style
        /// PdfCellStyle defStyle = new PdfCellStyle();
        /// defStyle.Font = font;
        /// defStyle.BackgroundBrush = PdfBrushes.White;
        /// defStyle.BorderPen = borderPen;          
        /// //Create DataTable for source
        /// DataTable dataTable = new DataTable("myTable");
        /// dataTable.Columns.Add("ID1");
        /// dataTable.Columns[0].Caption = "id";
        /// dataTable.Columns.Add("ID2");
        /// object[] values = new object[] { "Table Features Demo", "" };
        /// dataTable.Rows.Add(values);
        /// // Creates a new table
        /// PdfLightTable table = new PdfLightTable();
        /// table.DataSource = dataTable;
        /// //Set the cell style
        /// table.Style.DefaultStyle = defStyle;
        /// // Draws the table
        /// table.Draw(page.Graphics);
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// 'Set font
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
        /// 'Create Pdf ben for drawing border
        /// Dim borderPen As PdfPen = New PdfPen(PdfBrushes.DarkBlue)
        /// borderPen.Width = 0
        /// 'Create brush
        /// Dim color As PdfColor = New PdfColor(192, 201, 219)
        /// Dim brush As PdfSolidBrush = New PdfSolidBrush(color)
        /// ' Creates default cell style
        /// Dim defStyle As PdfCellStyle = New PdfCellStyle()
        /// defStyle.Font = font
        /// defStyle.BackgroundBrush = PdfBrushes.White
        /// defStyle.BorderPen = borderPen
        /// 'Create DataTable for source
        /// Dim dataTable As DataTable = New DataTable("myTable")
        /// dataTable.Columns.Add("ID1")
        /// dataTable.Columns(0).Caption = "id"
        /// dataTable.Columns.Add("ID2")
        /// Dim values() As Object = New Object() { "Table Features Demo", "" }
        /// dataTable.Rows.Add(values)
        /// ' Creates a new table
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// table.DataSource = dataTable
        /// 'Set the cell style
        /// table.Style.DefaultStyle = defStyle
        /// ' Draws the table
        /// table.Draw(page.Graphics)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfLightTable"/> Class 
        public PdfBrush BackgroundBrush
        {
            get
            {
                return m_backgroundBrush;
            }
            set
            {
                m_backgroundBrush = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfCellStyle"/> class.
        /// </summary>
        public PdfCellStyle()
        {
            m_textBrush = PdfBrushes.Black;
            m_borderPen = PdfPens.Black;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfCellStyle"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="fontBrush">The font brush.</param>
        /// <param name="borderPen">The border pen.</param>
        public PdfCellStyle(PdfFont font, PdfBrush fontBrush, PdfPen borderPen)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            if (fontBrush == null)
                throw new ArgumentNullException("fontBrush");

            if (borderPen == null)
                throw new ArgumentNullException("borderPen");


            m_font = font;
            m_textBrush = fontBrush;
            m_borderPen = borderPen;
        }
        #endregion
    }

    /// <summary>
    /// Represents the collection of the columns.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  // Creates a PDF document
    ///  PdfDocument doc = new PdfDocument();
    ///  //Creates a new page and adds it as the last page of the document
    ///  PdfPage page = doc.Pages.Add();
    ///  PdfLightTable table = new PdfLightTable();
    ///  // Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
    /// // Gets the columns collection
    /// PdfColumnCollection tableColumns = table.Columns;           
    /// // Creating Columns
    /// tableColumns.Add(new PdfColumn("Roll Number"));
    /// tableColumns.Add(new PdfColumn("Name"));
    /// tableColumns.Add(new PdfColumn("Class"));
    /// // Adding Rows
    /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
    /// // Draws the table 
    /// table.Draw(page, new PointF(0, 0));
    /// doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a PDF document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
    /// ' Gets the columns collection
    /// Dim tableColumns As PdfColumnCollection = table.Columns
    /// ' Creating Columns
    /// tableColumns.Add(New PdfColumn("Roll Number"))
    /// tableColumns.Add(New PdfColumn("Name"))
    /// tableColumns.Add(New PdfColumn("Class"))
    /// ' Adding Rows
    /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
    /// ' Draws the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfCollection"/> Class 
    public class PdfColumnCollection : PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="T:PdfColumn"/> at the specified index.
        /// </summary>
        public PdfColumn this[int index]
        {
            get
            {
                PdfColumn column = List[index] as PdfColumn;

                return column;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColumnCollection"/> class.
        /// </summary>
        internal PdfColumnCollection()
            : base()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <example>
        /// <example>
        /// <code lang="C#">
        ///  // Creates a PDF document
        ///  PdfDocument doc = new PdfDocument();
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  PdfLightTable table = new PdfLightTable();
        ///  // Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
        /// // Gets the columns collection
        /// PdfColumnCollection tableColumns = table.Columns;           
        /// // Creating Columns
        /// tableColumns.Add(new PdfColumn("Roll Number"));
        /// tableColumns.Add(new PdfColumn("Name"));
        /// tableColumns.Add(new PdfColumn("Class"));
        /// // Adding Rows
        /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
        /// // Draws the table 
        /// table.Draw(page, new PointF(0, 0));
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// ' Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
        /// ' Gets the columns collection
        /// Dim tableColumns As PdfColumnCollection = table.Columns
        /// ' Creating Columns
        /// tableColumns.Add(New PdfColumn("Roll Number"))
        /// tableColumns.Add(New PdfColumn("Name"))
        /// tableColumns.Add(New PdfColumn("Class"))
        /// ' Adding Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfCollection"/> Class 
        public void Add(PdfColumn column)
        {
            List.Add(column);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the widths of the columns.
        /// </summary>
        /// <param name="totalWidth">The total width.</param>
        /// <returns>An array containing widths.</returns>
        internal float[] GetWidths(float totalWidth)
        {
            return GetWidths(totalWidth, 0, Count - 1);
        }

        /// <summary>
        /// Gets the widths of the columns.
        /// </summary>
        /// <param name="totalWidth">The total width.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="endColumn">The end column.</param>
        /// <returns>An array containing widths.</returns>
        internal float[] GetWidths(float totalWidth, int startColumn, int endColumn)
        {
            int count = endColumn - startColumn + 1;

            if (count > Count)
                throw new ArgumentException("The start and end column indices doesn't match.");

            // TODO: optimize.
            float[] widths = new float[count];
            float summ = 0.0f;

            for (int i = startColumn; i <= endColumn; ++i)
            {
                float width = this[i].Width;
                widths[i - startColumn] = width;
                summ += width;
            }

            if (totalWidth > 0)
            {
                float factor = totalWidth / summ;

                for (int j = 0; j < count; ++j)
                {
                    widths[j] *= factor;
                }
            }

            return widths;
        }
        #endregion

    }

    /// <summary>
    /// Represents a single column of the table.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  // Creates a PDF document
    ///  PdfDocument doc = new PdfDocument();
    ///  //Creates a new page and adds it as the last page of the document
    ///  PdfPage page = doc.Pages.Add();
    ///  PdfLightTable table = new PdfLightTable();
    ///  // Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
    /// // Gets the columns collection
    /// PdfColumnCollection tableColumns = table.Columns;           
    /// // Creating Columns
    /// tableColumns.Add(new PdfColumn("Roll Number"));
    /// tableColumns.Add(new PdfColumn("Name"));
    /// tableColumns.Add(new PdfColumn("Class"));
    /// // Adding Rows
    /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
    /// // Draws the table 
    /// table.Draw(page, new PointF(0, 0));
    /// doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a PDF document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
    /// ' Gets the columns collection
    /// Dim tableColumns As PdfColumnCollection = table.Columns
    /// ' Creating Columns
    /// tableColumns.Add(New PdfColumn("Roll Number"))
    /// tableColumns.Add(New PdfColumn("Name"))
    /// tableColumns.Add(New PdfColumn("Class"))
    /// ' Adding Rows
    /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
    /// ' Draws the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    public class PdfColumn
    {
        #region Constants
        private const float DefaultWidth = 10f;
        #endregion

        #region Fields
        private float m_width;
        private PdfStringFormat m_stringFormat;
        private string m_columnName;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the string format.
        /// </summary>
        /// <value>The string format.</value>
        /// <example>
        /// <code lang="C#">
        ///  // Creates a PDF document
        ///  PdfDocument doc = new PdfDocument();
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  PdfLightTable table = new PdfLightTable();
        ///  // Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
        /// // Gets the columns collection
        /// PdfColumnCollection tableColumns = table.Columns;           
        /// // Creating Columns
        /// tableColumns.Add(new PdfColumn("Name"));
        /// tableColumns.Add(new PdfColumn("Class"));
        ///  // Creates a new column
        /// PdfColumn rollNumber = new PdfColumn("Roll Number");
        ///  //Set the string format
        /// rollNumber.StringFormat = new PdfStringFormat(PdfTextAlignment.Left);
        /// // Adding Rows
        /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
        /// // Draws the table 
        /// table.Draw(page, new PointF(0, 0));
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// ' Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
        /// ' Gets the columns collection
        /// Dim tableColumns As PdfColumnCollection = table.Columns
        ///  ' Creates a new column
        ///  Dim rollNumber As PdfColumn = New PdfColumn("Roll Number")
        ///  'Set the string format
        ///  rollNumber.StringFormat = New PdfStringFormat(PdfTextAlignment.Left)
        /// tableColumns.Add(New PdfColumn("Name"))
        /// tableColumns.Add(New PdfColumn("Class"))
        /// ' Adding Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>       
        public PdfStringFormat StringFormat
        {
            get
            {
                return m_stringFormat;
            }
            set
            {
                m_stringFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        ///  // Creates a PDF document
        ///  PdfDocument doc = new PdfDocument();
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  PdfLightTable table = new PdfLightTable();
        ///  // Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
        /// // Gets the columns collection
        /// PdfColumnCollection tableColumns = table.Columns;           
        /// // Creating Columns
        /// tableColumns.Add(new PdfColumn("Name"));
        /// tableColumns.Add(new PdfColumn("Class"));
        ///  // Creates a new column
        /// PdfColumn rollNumber = new PdfColumn("Roll Number");
        /// //Set the string format
        /// rollNumber.Width = 20f;
        /// // Adding Rows
        /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
        /// // Draws the table 
        /// table.Draw(page, new PointF(0, 0));
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// ' Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
        /// ' Gets the columns collection
        /// Dim tableColumns As PdfColumnCollection = table.Columns
        ///  ' Creates a new column
        ///  Dim rollNumber As PdfColumn = New PdfColumn("Roll Number")
        /// 'Set the string format
        /// rollNumber.Width = 20f
        /// tableColumns.Add(New PdfColumn("Name"))
        /// tableColumns.Add(New PdfColumn("Class"))
        /// ' Adding Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>  
        public float Width
        {
            get
            {
                return m_width;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("The width should be a positive number.", "Width");

                m_width = value;
            }
        }
        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        ///  // Creates a PDF document
        ///  PdfDocument doc = new PdfDocument();
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  PdfLightTable table = new PdfLightTable();
        ///  // Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
        /// // Gets the columns collection
        /// PdfColumnCollection tableColumns = table.Columns;           
        /// // Creating Columns
        /// tableColumns.Add(new PdfColumn("Name"));
        /// tableColumns.Add(new PdfColumn("Class"));
        ///  // Creates a new column
        /// PdfColumn rollNumber = new PdfColumn("Roll Number");
        /// //Set the string format
        /// rollNumber.ColumnName = "Roll Number";
        /// // Adding Rows
        /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
        /// // Draws the table 
        /// table.Draw(page, new PointF(0, 0));
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// ' Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
        /// ' Gets the columns collection
        /// Dim tableColumns As PdfColumnCollection = table.Columns
        ///  ' Creates a new column
        ///  Dim rollNumber As PdfColumn = New PdfColumn("Roll Number")
        /// 'Set the string format
        /// rollNumber.ColumnName = "Roll Number"
        /// tableColumns.Add(New PdfColumn("Name"))
        /// tableColumns.Add(New PdfColumn("Class"))
        /// ' Adding Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>  
        public string ColumnName
        {
            get
            {
                return m_columnName;
            }
            set
            {
                m_columnName = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfColumn"/> class.
        /// </summary>
        public PdfColumn()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColumn"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        internal PdfColumn(float width)
            : this()
        {
            Width = width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfColumn"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        public PdfColumn(string columnName)
        {
            m_columnName = columnName;
            m_width = DefaultWidth;
        }
        #endregion
    }

    /// <summary>
    /// Represents a single column of the table.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// // Creates a new table
    /// PdfLightTable table = new PdfLightTable();
    /// // Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
    /// // Creating Columns
    /// table.Columns.Add(new PdfColumn("Roll Number"));
    /// table.Columns.Add(new PdfColumn("Name"));
    /// table.Columns.Add(new PdfColumn("Class"));          
    /// // Adding Rows
    /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
    /// // Draws the table 
    /// table.Draw(page, new PointF(0, 0));
    /// doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Creates a new table
    /// Dim table As PdfLightTable = New PdfLightTable()
    /// ' Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
    /// ' Creating Columns
    /// table.Columns.Add(New PdfColumn("Roll Number"))
    /// table.Columns.Add(New PdfColumn("Name"))
    /// table.Columns.Add(New PdfColumn("Class"))
    /// ' Adding Rows
    /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
    /// ' Draws the table 
    /// table.Draw(page, New PointF(0, 0))
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    public class PdfRow
    {
        #region Fields
        private object[] m_values;
        #endregion

        #region Properties
        /// <summary>
        /// The array of values that are used to create the new row.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Creates a new table
        /// PdfLightTable table = new PdfLightTable();
        /// // Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
        /// // Creating Columns
        /// table.Columns.Add(new PdfColumn("Roll Number"));
        /// table.Columns.Add(new PdfColumn("Name"));
        /// table.Columns.Add(new PdfColumn("Class"));          
        /// // Adding Rows
        /// table.Rows.Add(new object[] { "111", "Maxim", "III" });
        /// // Draws the table 
        /// table.Draw(page, new PointF(0, 0));
        /// doc.Save("Tables.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates a new table
        /// Dim table As PdfLightTable = New PdfLightTable()
        /// ' Set the DataSourceType as Direct
        /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect
        /// ' Creating Columns
        /// table.Columns.Add(New PdfColumn("Roll Number"))
        /// table.Columns.Add(New PdfColumn("Name"))
        /// table.Columns.Add(New PdfColumn("Class"))
        /// ' Adding Rows
        /// table.Rows.Add(New Object() { "111", "Maxim", "III" })
        /// ' Draws the table 
        /// table.Draw(page, New PointF(0, 0))
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        public object[] Values
        {
            get
            {
                return m_values;
            }
            set
            {
                m_values = value;
            }
        }
        #endregion

        #region Constructors
        internal PdfRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfRow"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        internal PdfRow(object[] values)
            : this()
        {
            m_values = values;
        }
        #endregion
    }

    /// <summary>
    /// Represents the collection of the columns.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// // Creates a new table
    /// PdfLightTable table = new PdfLightTable();
    /// // Set the DataSourceType as Direct
    /// table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
    /// // Creating Columns
    /// table.Columns.Add(new PdfColumn("Roll Number"));
    /// table.Columns.Add(new PdfColumn("Name"));
    /// table.Columns.Add(new PdfColumn("Class"));          
    /// // Adding rows
    /// PdfRowCollection rowCollection = table.Rows;
    /// rowCollection.Add(new object[] { "111", "Maxim", "III" });
    /// // Draws the table 
    /// table.Draw(page, new PointF(0, 0));
    /// doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    ///  ' Creates a new document
    ///  Dim doc As PdfDocument = New PdfDocument()
    ///  ' Create a page
    ///  Dim page As PdfPage = doc.Pages.Add()
    ///  ' Creates a new table
    ///  Dim table As PdfLightTable = New PdfLightTable()
    ///  ' Set the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.TableDirect
    ///  ' Creating Columns
    ///  table.Columns.Add(New PdfColumn("Roll Number"))
    ///  table.Columns.Add(New PdfColumn("Name"))
    ///  table.Columns.Add(New PdfColumn("Class"))
    ///  ' Adding rows
    ///  Dim rowCollection As PdfRowCollection = table.Rows
    ///  rowCollection.Add(New Object() { "111", "Maxim", "III" })
    ///  ' Draws the table 
    ///  table.Draw(page, New PointF(0, 0))
    ///  doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfCollection"/> Class  
    public class PdfRowCollection : PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="T:PdfColumn"/> at the specified index.
        /// </summary>
        public PdfRow this[int index]
        {
            get
            {
                PdfRow row = List[index] as PdfRow;

                return row;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColumnCollection"/> class.
        /// </summary>
        internal PdfRowCollection()
            : base()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified row.
        /// </summary>
        /// <param name="row">The row.</param> 
        public void Add(PdfRow row)
        {
            List.Add(row);
        }
        /// <summary>
        /// The array of values that are used to create the new row.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        ///  // Adding rows
        ///  PdfRowCollection rowCollection = table.Rows;
        ///  // Gets the first row from the collection.          
        ///  rowCollection.Add(new object[] { "111", "Maxim", "III" });
        /// </code>
        /// <code lang="VB">
        ///  ' Adding rows
        ///  Dim rowCollection As PdfRowCollection = table.Rows
        ///  ' Gets the first row from the collection.          
        ///  rowCollection.Add(New Object() { "111", "Maxim", "III" })
        /// </code>
        /// </example>
        public void Add(object[] values)
        {
            PdfRow row = new PdfRow(values);
            List.Add(row);
        }
        #endregion
    }


    /// <summary>
    /// Represents as a message deliverer from PdfLightTable class to the user.
    /// </summary>
    /// <seealso cref="PdfException"/> Class 
    public class PdfLightTableException : PdfException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLightTableException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        internal PdfLightTableException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
