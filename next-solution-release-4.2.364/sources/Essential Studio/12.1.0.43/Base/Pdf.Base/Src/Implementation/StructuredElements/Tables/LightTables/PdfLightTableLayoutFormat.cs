#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Tables namespace contains classes for creating tables.
/// </summary>
namespace Syncfusion.Pdf.Tables
{
    /// <summary>
    /// Represents the parameters for Light Table layout.
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
    /// // Adding rows
    /// PdfRowCollection rowCollection = table.Rows;
    /// // Gets the first row from the collection.          
    /// rowCollection.Add(new object[] { "111", "Maxim", "III" });
    /// // Creates the layout format
    /// PdfLightTableLayoutFormat format = new PdfLightTableLayoutFormat();
    /// format.Layout = PdfLayoutType.Paginate;
    /// format.Break = PdfLayoutBreakType.FitElement;
    /// format.StartColumnIndex = 1;
    /// format.EndColumnIndex = 2;
    /// // Draws the table with the layout format
    /// table.Draw(page, new PointF(0, 0), format);
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
    /// ' Adding rows
    /// Dim rowCollection As PdfRowCollection = table.Rows
    /// ' Gets the first row from the collection.          
    /// rowCollection.Add(New Object() { "111", "Maxim", "III" })
    /// ' Creates the layout format
    /// Dim format As PdfLightTableLayoutFormat = New PdfLightTableLayoutFormat()
    /// format.Layout = PdfLayoutType.Paginate
    /// format.Break = PdfLayoutBreakType.FitElement
    /// format.StartColumnIndex = 1
    /// format.EndColumnIndex = 2
    /// ' Draws the table with the layout format
    /// table.Draw(page, New PointF(0, 0), format)
    /// doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLayoutFormat"/> Class    
    /// <seealso cref="PdfDocument"/> Class    
    /// <seealso cref="PdfLayoutBreakType"/> Class  
    public class PdfLightTableLayoutFormat : PdfLayoutFormat
    {
        #region Fields
        private int m_startColumn;
        private int m_endColumn;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the start column index.
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
        /// // Adding rows
        /// PdfRowCollection rowCollection = table.Rows;
        /// // Gets the first row from the collection.          
        /// rowCollection.Add(new object[] { "111", "Maxim", "III" });
        /// // Creates the layout format
        /// PdfLightTableLayoutFormat format = new PdfLightTableLayoutFormat();
        /// format.Layout = PdfLayoutType.Paginate;
        /// format.Break = PdfLayoutBreakType.FitElement;
        /// format.StartColumnIndex = 1;
        /// format.EndColumnIndex = 2;
        /// // Draws the table with the layout format
        /// table.Draw(page, new PointF(0, 0), format);
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
        /// ' Adding rows
        /// Dim rowCollection As PdfRowCollection = table.Rows
        /// ' Gets the first row from the collection.          
        /// rowCollection.Add(New Object() { "111", "Maxim", "III" })
        /// ' Creates the layout format
        /// Dim format As PdfLightTableLayoutFormat = New PdfLightTableLayoutFormat()
        /// format.Layout = PdfLayoutType.Paginate
        /// format.Break = PdfLayoutBreakType.FitElement
        /// format.StartColumnIndex = 1
        /// format.EndColumnIndex = 2
        /// ' Draws the table with the layout format
        /// table.Draw(page, New PointF(0, 0), format)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLayoutFormat"/> Class    
        /// <seealso cref="PdfDocument"/> Class    
        /// <seealso cref="PdfLayoutBreakType"/> Class  
        public int StartColumnIndex
        {
            get
            {
                return m_startColumn;
            }
            set
            {
                m_startColumn = value;
            }
        }

        /// <summary>
        /// Gets or sets the end column index.
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
        /// // Adding rows
        /// PdfRowCollection rowCollection = table.Rows;
        /// // Gets the first row from the collection.          
        /// rowCollection.Add(new object[] { "111", "Maxim", "III" });
        /// // Creates the layout format
        /// PdfLightTableLayoutFormat format = new PdfLightTableLayoutFormat();
        /// format.Layout = PdfLayoutType.Paginate;
        /// format.Break = PdfLayoutBreakType.FitElement;
        /// format.StartColumnIndex = 1;
        /// format.EndColumnIndex = 2;
        /// // Draws the table with the layout format
        /// table.Draw(page, new PointF(0, 0), format);
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
        /// ' Adding rows
        /// Dim rowCollection As PdfRowCollection = table.Rows
        /// ' Gets the first row from the collection.          
        /// rowCollection.Add(New Object() { "111", "Maxim", "III" })
        /// ' Creates the layout format
        /// Dim format As PdfLightTableLayoutFormat = New PdfLightTableLayoutFormat()
        /// format.Layout = PdfLayoutType.Paginate
        /// format.Break = PdfLayoutBreakType.FitElement
        /// format.StartColumnIndex = 1
        /// format.EndColumnIndex = 2
        /// ' Draws the table with the layout format
        /// table.Draw(page, New PointF(0, 0), format)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLayoutFormat"/> Class    
        /// <seealso cref="PdfDocument"/> Class    
        /// <seealso cref="PdfLayoutBreakType"/> Class  
        public int EndColumnIndex
        {
            get
            {
                return m_endColumn;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("EndColumnIndex");

                m_endColumn = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLightTableLayoutFormat"/> class.
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
        /// // Adding rows
        /// PdfRowCollection rowCollection = table.Rows;
        /// // Gets the first row from the collection.          
        /// rowCollection.Add(new object[] { "111", "Maxim", "III" });
        /// // Creates the layout format
        /// PdfLightTableLayoutFormat format = new PdfLightTableLayoutFormat();
        /// format.Layout = PdfLayoutType.Paginate;
        /// format.Break = PdfLayoutBreakType.FitElement;
        /// format.StartColumnIndex = 1;
        /// format.EndColumnIndex = 2;
        /// // Draws the table with the layout format
        /// table.Draw(page, new PointF(0, 0), format);
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
        /// ' Adding rows
        /// Dim rowCollection As PdfRowCollection = table.Rows
        /// ' Gets the first row from the collection.          
        /// rowCollection.Add(New Object() { "111", "Maxim", "III" })
        /// ' Creates the layout format
        /// Dim format As PdfLightTableLayoutFormat = New PdfLightTableLayoutFormat()
        /// format.Layout = PdfLayoutType.Paginate
        /// format.Break = PdfLayoutBreakType.FitElement
        /// format.StartColumnIndex = 1
        /// format.EndColumnIndex = 2
        /// ' Draws the table with the layout format
        /// table.Draw(page, New PointF(0, 0), format)
        /// doc.Save("Tables.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLayoutFormat"/> Class    
        /// <seealso cref="PdfDocument"/> Class    
        /// <seealso cref="PdfLayoutBreakType"/> Class  
        public PdfLightTableLayoutFormat()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLightTableLayoutFormat"/> class.
        /// </summary>
        /// <param name="baseFormat">The base format.</param>
        public PdfLightTableLayoutFormat(PdfLayoutFormat baseFormat)
            : base(baseFormat)
        {
        }
        #endregion
    }
}
