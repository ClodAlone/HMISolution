#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation;

#if !SILVERLIGHT && !WINRT && !WP
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data;
#endif
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Implementation.Tables
{
  class ListObject : IListObject
  {
    #region Members
    /// <summary>
    /// Name of the list object.
    /// </summary>
    private string m_strName;
    /// <summary>
    /// Location of the list object.
    /// </summary>
    private IRange m_location;
    /// <summary>
    /// List object's columns.
    /// </summary>
    private IList<IListObjectColumn> m_columns;
    /// <summary>
    /// Index of the list object.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Built-in table style index.
    /// </summary>
    private TableBuiltInStyles m_builtInStyle;
    /// <summary>
    /// Display name.
    /// </summary>
    private string m_strDisplayName;
    /// <summary>
    /// Parent worksheet object.
    /// </summary>
    private WorksheetImpl m_worksheet;
    /// <summary>
    /// Count of rows with totals.
    /// </summary>
    private int m_iTotalsRowCount;
    /// <summary>
    /// Indicates whether row stripes are shown.
    /// </summary>
    private bool m_bRowStripes = true;
    /// <summary>
    /// Indicates whether column stripes are shown.
    /// </summary>
    private bool m_bColumnStripes;
    /// <summary>
    /// A Boolean indicating whether the totals row has ever been shown in the past for this table.
    /// </summary>
    private bool m_bTotalsRowShown;
    /// <summary>
    /// Row shift.
    /// </summary>
    private int m_iInsertRowShift = 0;
    /// <summary>
    /// Stream with autofilter data.
    /// </summary>
    internal Stream AutoFilterStream;
    /// <summary>
    /// Custom table style name.
    /// </summary>
    private string m_TableStyleName;
    /// <summary>
    /// A Boolean indicating whether the First Columns are shown.
    /// </summary>
    private bool m_bFirstColumn;
    /// <summary>
    /// A Boolean indicating whether the Last Columns are shown.
    /// </summary>
    private bool m_bLastColumn;
    /// <summary>
    /// A Boolean indicating whether the Header Row is shown.
    /// </summary>
    private bool m_bHeaderRow = true;    
    /// <summary>
    /// Query table for External connection.
    /// </summary>
    private QueryTableImpl m_queryTable;
    /// <summary>
    /// Represent the Table Type
    /// </summary>
    private ExcelTableType m_tableType = ExcelTableType.worksheet;
    #endregion

    #region IListObject Members
    /// <summary>
    /// Gets or sets name of the list object.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }
    /// <summary>
    /// Gets or sets list object's location.
    /// </summary>
    public IRange Location
    {
      get
      {
        return m_location;
      }
      set
      {
        m_location = value;
        AutoFilterStream = null;
      }
    }
    /// <summary>
    /// Gets collection of all columns of the list object.
    /// </summary>
    public IList<IListObjectColumn> Columns
    {
      get
      {
        return m_columns;
      }
    }
    /// <summary>
    /// Gets index of the current list object.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
      internal set
      {
        m_iIndex = value;
      }
    }
    /// <summary>
    /// Gets or sets the built-in table style for the specified ListObject object.
    /// </summary>
    public TableBuiltInStyles BuiltInTableStyle
    {
      get
      {
        return m_builtInStyle;
      }
      set
      {
        m_builtInStyle = value;
      }
    }
    /// <summary>
    /// Gets parent worksheet object.
    /// </summary>
    public IWorksheet Worksheet
    {
      get
      {
        return m_worksheet;
      }
    }
    /// <summary>
    /// Gets or sets list object name.
    /// </summary>
    public string DisplayName
    {
      get
      {
        return m_strDisplayName;
      }
      set
      {
        m_strDisplayName = value;
      }
    }
    /// <summary>
    /// Gets number of rows with totals.
    /// </summary>
    public int TotalsRowCount
    {
      get
      {
        return m_iTotalsRowCount;
      }
      internal set
      {
        m_iTotalsRowCount = value;
      }
    }
    /// <summary>
    /// Gets or sets a boolean indicating whether the totals row has ever been shown in the past for this table.
    /// </summary>
    public bool TotalsRowShown
    {
      get
      {
        return m_bTotalsRowShown;
      }
      set
      {
        m_bTotalsRowShown = value;
      }
    }
    public QueryTableImpl QueryTable
    {
        get
        {
            if (m_queryTable!=null && m_queryTable.IsDeleted)
            {
                m_queryTable = null;
                m_tableType = ExcelTableType.worksheet;
            }
            return m_queryTable;
        }
        set
        {
            m_queryTable = value;
        }
    }
    
    #endregion

    #region Properties
    /// <summary>
    /// Indicates whether row stripes are shown.
    /// </summary>
    public bool ShowTableStyleRowStripes
    {
      get
      {
        return m_bRowStripes;
      }
      set
      {
        m_bRowStripes = value;
      }
    }
    /// <summary>
    /// Indicates whether column stripes are shown.
    /// </summary>
    public bool ShowTableStyleColumnStripes
    {
      get
      {
        return m_bColumnStripes;
      }
      set
      {
        m_bColumnStripes = value;
      }
    }
    public int InsertRowShift
    {
      get
      {
        return m_iInsertRowShift;
      }
      set
      {
        m_iInsertRowShift = value;
      }
    }
    /// <summary>
    ///  /// Gets or sets a table style name.
    /// </summary>
    internal string TableStyleName
    {
        get
        {
            return m_TableStyleName;
        }
        set
        {
            m_TableStyleName = value;
        }
    }
    /// <summary>
    ///  /// Gets or sets a value indicating whether [show first column].
    /// </summary>    
    public bool ShowFirstColumn
    {
        get
        {
            return m_bFirstColumn;
        }
        set
        {
            m_bFirstColumn = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show last column].
    /// </summary>    
    public bool ShowLastColumn
    {
        get
        {
            return m_bLastColumn;
        }
        set
        {
            m_bLastColumn = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show header row].
    /// </summary>   
    public bool ShowHeaderRow
    {
        get
        {
            return m_bHeaderRow;
        }
        set
        {
          if( m_bHeaderRow != value )
          {
            if( !m_worksheet.ParentWorkbook.Loading && Location != null )
            {
              if (value == false)
              {
                //((WorksheetBaseImpl)this.Worksheet).FirstRow += 1;
                this.Worksheet[ Location.Row, Location.Column, Location.Row, Location.LastColumn ].Text = string.Empty;
                this.Location = this.Worksheet[Location.Row + 1, Location.Column, Location.LastRow, Location.LastColumn];                
              }
              else
              {
                int iHeaderRow = Location.Row - 1;
                this.Location = this.Worksheet[ iHeaderRow, Location.Column, Location.LastRow, Location.LastColumn];
                int iFirstColumn = Location.Column;

                for( int i = 0, len = m_columns.Count; i < len; i++ )
                {
                  int iColumn = iFirstColumn + i;
                  Location[ iHeaderRow, iColumn, iHeaderRow, iColumn ].Text = m_columns[ i ].Name;
                }
              }
            }

            m_bHeaderRow = value;
          }
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the Total row is visible.
    /// </summary>
    public bool ShowTotals
    {
      get
      {
        return m_iTotalsRowCount != 0;
      }
      set
      {
        if( value != ShowTotals )
        {
          IWorksheet sheet = m_location.Worksheet;
          IWorkbook book = sheet.Workbook;
          int iMaxRow = book.MaxRowCount;

          if( value )
          {
            //m_iTotalsRowCount = value ? 1 : 0;
            // Add one row to location if possible and set totals row:

            if( m_location.LastRow != iMaxRow )
            {
              // Move items one row below.
              int iTotalRowIndex = m_location.LastRow + 1;
              IRange rangeBelow = sheet[ iTotalRowIndex, m_location.Column, iMaxRow - 1, m_location.LastColumn ];
              rangeBelow.MoveTo( sheet[ iTotalRowIndex + 1, m_location.Column ] );
              IRange beforeTotalRow = sheet[ iTotalRowIndex - 1, m_location.Column, iTotalRowIndex - 1, m_location.LastColumn ];
              beforeTotalRow.CopyTo( sheet[ iTotalRowIndex, m_location.Column ], ExcelCopyRangeOptions.CopyStyles );

              m_location = sheet[ m_location.Row, m_location.Column, m_location.LastRow + 1, m_location.LastColumn ];
              m_iTotalsRowCount = 1;
            }

            TotalsRowShown = true;
          }
          else
          {
            int iTotalRowIndex = m_location.LastRow;
            IRange rangeBelow = sheet[ iTotalRowIndex + 1, m_location.Column, iMaxRow - 1, m_location.LastColumn ];
            rangeBelow.MoveTo( sheet[ iTotalRowIndex, m_location.Column ] );

            m_location = sheet[ m_location.Row, m_location.Column, m_location.LastRow - m_iTotalsRowCount, m_location.LastColumn ];
            m_iTotalsRowCount = 0;
          }
        }
      }
    }
    /// <summary>
    /// Represent the Table Type
    /// </summary>
    public ExcelTableType TableType
    {
        get
        {
            return m_tableType;
        }
        set
        {
            m_tableType = value;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the list object.
    /// </summary>
    /// <param name="name">Name to set.</param>
    /// <param name="location">Location of the new object.</param>
    /// <param name="index">New object's index.</param>
    public ListObject( string name, IRange location, int index )
    {
      m_worksheet = location.Worksheet as WorksheetImpl;
      m_strDisplayName = m_strName = name;
      m_location = location;
      m_iIndex = index;

      int iRow = location.Row;
      IWorksheet sheet = location.Worksheet;
      m_columns = new List<IListObjectColumn>();
      int iUnusedColumnIndex = 0;

      bool bLoading = m_worksheet.ParentWorkbook.Loading;

      for( int i = m_location.Column, last = m_location.LastColumn; i <= last; i++ )
      {
        IRange range = sheet[ iRow, i ];
          string strColumnName = (range.CellStyle.IsFirstSymbolApostrophe) ? range.DisplayText : range.Text;

        if( strColumnName == null || strColumnName.Length == 0 )
        {
          if (range.Value != null && range.Value!=string.Empty)
              strColumnName = range.Value.ToString();
          else
              strColumnName = "Column" + ( ++iUnusedColumnIndex ).ToString();

          if( !bLoading )
            range.Text = strColumnName;
        }

        m_columns.Add( new ListObjectColumn( strColumnName, m_columns.Count + 1, this, i + 1 ) );
      }
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <param name="worksheet">Parent worksheet for the new object.</param>
    /// <param name="hashWorksheetNames">Dictionary with modified worksheet names.</param>
    /// <returns>Copy of the current object.</returns>
    internal ListObject Clone( WorksheetImpl worksheet, Dictionary<string, string> hashWorksheetNames )
    {
      ListObject result = ( ListObject )MemberwiseClone();
      WorkbookImpl book = worksheet.ParentWorkbook;
      // Clone location
      result.m_location = ( m_location as ICombinedRange ).Clone( worksheet, hashWorksheetNames, book );
      // Clone columns.
      if( m_columns != null )
      {
        int iCount = m_columns.Count;
        result.m_columns = new List<IListObjectColumn>( iCount );

        for( int i = 0; i < iCount; i++ )
        {
          ListObjectColumn column = ( ListObjectColumn )m_columns[ i ];
          result.m_columns.Add( column.Clone( result ) );
        }
      }

      // Clone stream
      AutoFilterStream = CloneUtils.CloneStream( AutoFilterStream );

      // Name
      result.Name = null; // make it zero so we won't search it in the GenerateUniqueName method.
      string tempName = GenerateUniqueName(book, Name);
      if (!Area3DPtg.ValidateSheetName(tempName))
      {
          result.DisplayName = result.Name = GenerateUniqueName(book, Name);
      }
      else
      {
          result.DisplayName = result.Name = GenerateUniqueName(book, "table");
      }
      if (result.QueryTable != null)
      {
          QueryTableImpl query = result.QueryTable;             
          string ConnectionName = query.ExternalConnection.Name;
          string stringPart;
          int numberPart;
          SplitName(ConnectionName, out stringPart,out numberPart);
          numberPart++;
          ConnectionName = stringPart + numberPart;
          bool successs=true;
          for (; ; )
          {
              if (Checkconn_name(book.Connections, ConnectionName) || Checkconn_name(book.DeletedConnections, ConnectionName))
              {
                  numberPart++;
                  ConnectionName = stringPart + numberPart;
              }
              else
                  break;
          }
          result.QueryTable = query.Clone(result, book,ConnectionName);          
      }
      return result;
    }
    private bool Checkconn_name(IConnections connections,string Name)
    {
        foreach (IConnection conn in connections)
        {
            if (conn.Name == Name)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Generates unique table name.
    /// </summary>
    /// <param name="book">Workbook to generate unique name for.</param>
    /// <param name="proposedName">Starting name.</param>
    /// <returns>Unique name of the table.</returns>
    private string GenerateUniqueName( WorkbookImpl book, string proposedName )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      if( String.IsNullOrEmpty( proposedName ) )
        throw new ArgumentOutOfRangeException( "proposedName" );

      if( ListObjectNameExist( book, proposedName ) )
      {
        string stringPart;
        int iNumberPart;
        SplitName( proposedName, out stringPart, out iNumberPart );
        iNumberPart++;
        proposedName = stringPart + iNumberPart;

        while( ListObjectNameExist( book, proposedName ) )
        {
          iNumberPart++;
          proposedName = stringPart + iNumberPart;
        }
      }

      return proposedName;
    }
    /// <summary>
    /// Splits name into two parts - string part and index part.
    /// </summary>
    /// <param name="proposedName">Name to split.</param>
    /// <param name="stringPart">String part.</param>
    /// <param name="numberPart">Index part.</param>
    private void SplitName( string proposedName, out string stringPart, out int numberPart )
    {
      stringPart = string.Empty;
      numberPart = 0;
      int iLastDigit = proposedName.Length;

      while( iLastDigit > 0 && Char.IsDigit( proposedName[ iLastDigit - 1 ] ) )
      {
        iLastDigit--;
      }

      if( iLastDigit > 0 )
      {
        stringPart = proposedName.Substring( 0, iLastDigit );
      }

      if( iLastDigit < proposedName.Length )
      {
        string strNumberPart = proposedName.Substring( iLastDigit );
        numberPart = int.Parse( strNumberPart );
      }
    }
    /// <summary>
    /// Checks whether list object with specified name exists in the specified workbook.
    /// </summary>
    /// <param name="book">Workbook to check.</param>
    /// <param name="name">Name to check.</param>
    /// <returns>True if list object was found.</returns>
    private bool ListObjectNameExist( WorkbookImpl book, string name )
    {
      IWorksheets sheets = book.Worksheets;

      for( int i = 0, len = sheets.Count; i < len; i++ )
      {
        WorksheetImpl sheet = ( WorksheetImpl )sheets[ i ];
        ListObjectCollection listObjects = sheet.InnerListObjects;

        if( listObjects == null || listObjects.Count == 0 )
          continue;

        if( listObjects[ name ] != null )
          return true;
      }

      return false;
    }
      #if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// List Object has been Refreshed.
    /// </summary>
    public void Refresh()
    {
        string SpaceKey = "_x000d_";
        string EnterKey = "_x000a_";
        string TabKey = "_x0009_";
        if (QueryTable == null)
            throw new ArgumentException("Query Table Not Exist");        
        ExternalConnection connection = QueryTable.ExternalConnection;
        ConnectionPassword password=new ConnectionPassword();
        connection.RaiseEvent(this, password);
        if (connection.DataBaseType == ExcelConnectionsType.ConnectionTypeOLEDB)
        {
            OleDbConnection ole_connection = new OleDbConnection();
            string Connect_string = connection.DBConnectionString;
            if (password.PasswordToConnectDB != null)
                Connect_string = Connect_string.Insert(Connect_string.LastIndexOf(";")+ 1,"password="+password.PasswordToConnectDB + ";");

            ole_connection.ConnectionString = Connect_string;
            string Query = QueryTable.CommandText.ToString().ToUpper();
            Query.Replace(SpaceKey, "");
            Query.Replace(EnterKey, "");
            Query.Replace(TabKey, "");
            if (checkCommandText(Query) && QueryTable.CommandType!=ExcelCommandType.Sql)
            {
                if(Path.GetExtension(connection.SourceFile).ToLower().Contains("xls"))              
                Query = "SELECT * FROM " + "["+ Query+"]";
                else
                Query = "SELECT * FROM " + Query; 
            }
            OleDbCommand command = new OleDbCommand(Query, ole_connection);
            DataTable Table = new DataTable();
            OleDbDataAdapter Adapter = new OleDbDataAdapter(command);
            Adapter.Fill(Table);
            FillTableData(Table);
            ole_connection.Dispose();
            Table.Dispose();
            Adapter.Dispose();
            command.Dispose();
        }
        else if (connection.DataBaseType == ExcelConnectionsType.ConnectionTypeODBC)
        {
            if (this.QueryTable.CommandType != ExcelCommandType.Sql)
                throw new ArgumentException("Command should be SQl for ODBC connection");
            OdbcConnection odbc_connection = new OdbcConnection();
            string Connect_string = connection.DBConnectionString;
            if (password.PasswordToConnectDB != null)
                Connect_string = Connect_string.Insert(Connect_string.LastIndexOf(";") + 1, "password=" + password.PasswordToConnectDB + ";");
            odbc_connection.ConnectionString = Connect_string;
            string Query = QueryTable.CommandText.ToString();
            Query=Query.Replace(SpaceKey, " ");
            Query = Query.Replace(EnterKey, " ");
            Query = Query.Replace(TabKey, " ");            
            OdbcCommand command = new OdbcCommand(Query, odbc_connection);
            DataTable Table = new DataTable();
            OdbcDataAdapter Adapter = new OdbcDataAdapter(command);
            Adapter.Fill(Table);
            FillTableData(Table);
            Adapter.Dispose();
            odbc_connection.Dispose();
            Table.Dispose();
            command.Dispose();
        }
            
    }
#endif
    private bool checkCommandText(string Query)
    {
        string[] queries = { "SELECT", "UPDATE", "DELETE", "INSERT", "ALTER" };
        for(int i=0;i<queries.Length;i++)
        {
         if (Query.Contains(queries[i]))
            return false;        
        }
        return true;
    }
#if !SILVERLIGHT && !WINRT && !WP
    private void FillTableData(DataTable Table)
    {
        this.Location.Clear();
        int sheetrow = this.Location.Row;
        int sheetcolumn = this.Location.Column;
        int tableRowsCount = Table.Rows.Count;
        int tableColumnsCount = Table.Columns.Count;
        if(Location.LastRow<Location.Row+tableRowsCount)
        ListObjectCollection.CheckOverLab(this.Worksheet[Location.LastRow + 1, sheetcolumn, sheetrow + tableRowsCount, sheetcolumn + tableColumnsCount]);
        if(Location.LastColumn<Location.Column+tableColumnsCount)
        ListObjectCollection.CheckOverLab(this.Worksheet[Location.Row , Location.LastColumn + 1 , sheetrow + tableRowsCount, sheetcolumn + tableColumnsCount]);
        this.Columns.Clear();
        for (int k = 0; k < Table.Columns.Count; k++)
        {
            object h = Table.Columns[k];
            this.Worksheet[sheetrow, sheetcolumn].Value = h.ToString();
            ListObjectColumn a1 = new ListObjectColumn(h.ToString(), k+1, this, k + 1);            
            this.Columns.Add(a1);
            a1.QueryTableFieldId = k + 1;            
            sheetcolumn++;
        }
            for (int row = 0; row < Table.Rows.Count; row++)
            {
                sheetrow++;
                sheetcolumn = this.Location.Column;
                for (int column = 0; column < Table.Columns.Count; column++)
                {
                    object h = Table.Rows[row][column];
                    this.Worksheet[sheetrow, sheetcolumn].Value = h.ToString();
                    sheetcolumn++;
                }
            }
        if(Table.Rows.Count==0)
            this.Location = this.Worksheet[this.Location.Row, this.Location.Column, sheetrow+1, sheetcolumn - 1];
        else
            this.Location = this.Worksheet[this.Location.Row, this.Location.Column, sheetrow, sheetcolumn - 1];
            WorkbookImpl book = this.Worksheet.Workbook as WorkbookImpl;
            IName name;
            if (book.Names[this.QueryTable.Name] == null)
                name = this.Worksheet.Names[this.QueryTable.Name];
            else
                name = book.Names[this.QueryTable.Name];
            name.RefersToRange = Location;
            sheetrow.ToString();
            sheetcolumn.ToString();       
    }
#endif
    internal void Dispose()
    {
        if (this.QueryTable != null)
        {
            if (this.QueryTable.ExternalConnection != null)
                this.QueryTable.ExternalConnection.Dispose();
            this.QueryTable = null;
        }
    }
    #endregion
  }
    
}
