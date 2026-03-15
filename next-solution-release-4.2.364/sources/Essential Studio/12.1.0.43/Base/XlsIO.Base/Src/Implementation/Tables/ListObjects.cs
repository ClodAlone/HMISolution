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
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation;
namespace Syncfusion.XlsIO.Implementation.Tables
{
  /// <summary>
  /// This class represents collection of ListObjects in the worksheet.
  /// </summary>
  public class ListObjectCollection :
    List<IListObject>,
    IListObjects
  {
      private int sheetindex;
      public ListObjectCollection(WorksheetImpl sheet)
      {
          sheetindex = sheet.Index;
      }      
    #region IListObjects Members
    /// <summary>
    /// Creates new list object and adds it to the collection.
    /// </summary>
    /// <param name="name">Name of the new list object.</param>
    /// <param name="range">Destination range.</param>
    /// <returns>Newly created object.</returns>
    public IListObject Create( string name, IRange range )
    {
      range = CheckRange(range);
      CheckOverLab(range);      
      ListObject result = new ListObject( name, range, Count + 1 );
      result.Name = name;
      result.Location = range;
      WorkbookImpl book = range.Worksheet.Workbook as WorkbookImpl;
      result.Index = ++book.MaxTableIndex;
      Add( result );
      return result;
    }
    /// <summary>
    /// Check the list object range is overlab the another table or pivot table
    /// </summary>
    /// <param name="range"></param>
    internal static void CheckOverLab(IRange range)
    {
        WorksheetImpl sheet = range.Worksheet as WorksheetImpl;
        if (!sheet.IsParsing)
        {
            int row = range.Row;
            int column = range.Column;
            int LastRow = range.LastRow;
            int LastColumn = range.LastColumn;
            for (int i = 0; i < sheet.ListObjects.Count; i++)
            {
                IListObject table = sheet.ListObjects[i];
                int tableRow = table.Location.Row;
                int tableColumn = table.Location.Column;
                int tableLastRow = table.Location.LastRow;
                int tableLastColumn = table.Location.LastColumn;                
                if ((row > tableRow-1 && row < tableLastRow+1) && (column > tableColumn-1 && column < tableLastColumn+1))
                    throw new ArgumentException("A table cannot overlap a range that contains a PivotTable report, query results, protected cells or another table");
                if ((row > tableRow - 1 && row < tableLastRow + 1) && (LastColumn > tableColumn - 1 && LastColumn < tableLastColumn + 1))
                    throw new ArgumentException("A table cannot overlap a range that contains a PivotTable report, query results, protected cells or another table");
                if ((LastRow > tableRow - 1 && LastRow < tableLastRow + 1) && (column > tableColumn - 1 && column < tableLastColumn + 1))
                    throw new ArgumentException("A table cannot overlap a range that contains a PivotTable report, query results, protected cells or another table");
                if ((LastRow > tableRow - 1 && LastRow < tableLastRow + 1) && (LastColumn > tableColumn - 1 && LastColumn < tableLastColumn + 1))
                    throw new ArgumentException("A table cannot overlap a range that contains a PivotTable report, query results, protected cells or another table");
                if ((row < tableRow -1  && row < tableLastRow + 1) || (LastRow > tableRow -1 && LastRow < tableLastRow + 1))
                {
                    if(tableColumn>column && tableLastColumn<LastColumn)
                        throw new ArgumentException("A table cannot overlap a range that contains a PivotTable report, query results, protected cells or another table");
                }
                if ((column < tableColumn - 1 && column < tableLastColumn + 1) || (LastColumn > tableColumn - 1 && LastColumn < tableLastColumn + 1))
                {
                    if (tableRow > row && tableLastRow < LastRow)
                        throw new ArgumentException("A table cannot overlap a range that contains a PivotTable report, query results, protected cells or another table");
                }
            }
        }
    }
    /// <summary>
    /// Checks whether the range is appropriate.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    private IRange CheckRange( IRange range )
    {
      int iRow = range.Row;

      if( iRow == range.LastRow )
      {
        IWorksheet sheet = range.Worksheet;
        IRange usedRange = sheet.UsedRange;
        int iColumn = range.Column;

        if( usedRange.LastRow > range.LastRow )
        {
          IRange destCell = sheet[ iRow + 2, iColumn ];
          IRange existingData = sheet[ iRow + 1, iColumn, usedRange.LastRow, range.LastColumn ];
          existingData.MoveTo( destCell );
        }

        range = sheet[ iRow, iColumn, iRow + 1, range.LastColumn ];
      }

      return range;
    }
    /// <summary>
    /// Creates copy of the current collection.
    /// </summary>
    /// <param name="worksheet">Parent worksheet for the new collection.</param>
    /// <returns>Created collection.</returns>
    internal ListObjectCollection Clone( WorksheetImpl worksheet, Dictionary<string, string> hashWorksheetNames )
    {
      ListObjectCollection result = new ListObjectCollection(worksheet);
      WorkbookImpl book = worksheet.ParentWorkbook;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ListObject table = ( ListObject )this[ i ];
        table = table.Clone( worksheet, hashWorksheetNames );
        table.Index = ++book.MaxTableIndex;
        result.Add( table );
      }

      return result;
    }
    #endregion

    #region Methods
    public IListObject this[ string name ]
    {
      get
      {
        IListObject result = null;

        for( int i = 0, len = Count; i < len; i++ )
        {
          IListObject current = this[ i ];

          if( current.Name == name )
          {
            result = current;
            break;
          }
        }

        return result;
      }
    }
    public IListObject AddEx(ExcelListObjectSourceType type,IConnection connection,IRange Destinaion)
    {
        RangeImpl range = Destinaion as RangeImpl;
        WorksheetImpl sheet = range.Parent as WorksheetImpl;
        if (range.Worksheet.Index != this.sheetindex)
            throw new ArgumentException("The worksheet range for the table data must be on the same sheet as the table being created");
        ExternalConnection Ext_Connection = connection as ExternalConnection;
        if (!Ext_Connection.IsExist)
            Ext_Connection.IsExist = true;
        else
            throw new ArgumentException("This connection already exist");
        string Ext_ConnStr;
        if (connection.ODBCConnection != null)
            Ext_ConnStr = Ext_Connection.ODBCConnection.ConnectionString.ToString();
        else if (connection.OLEDBConnection != null)
            Ext_ConnStr = Ext_Connection.OLEDBConnection.ConnectionString.ToString();
        else
            throw new ArgumentException("Connection is invalid");          
        WorkbookImpl workbook = sheet.Workbook as WorkbookImpl;
        ListObject table = sheet.ListObjects.Create(connection.Name, Destinaion) as ListObject;
        Ext_Connection.Range = table.Location;
        table.TableType = ExcelTableType.queryTable;
        IListObjectColumn column = table.Columns[0];
        (column as ListObjectColumn).QueryTableFieldId = 1;
        (column as ListObjectColumn).Id = 1;
        ListObject listobject = table as ListObject;
        listobject.QueryTable = new QueryTableImpl(sheet.Application, sheet.Parent,Ext_Connection);
        listobject.QueryTable.ConnectionString = Ext_ConnStr;
        listobject.QueryTable.Name = Ext_Connection.Name.Replace(" ", "") + "Query";
        listobject.DisplayName = Ext_Connection.Name.Replace(" ","") +"_"+ table.Index.ToString();
        listobject.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium2;
        listobject.ShowTableStyleRowStripes = true;
       // QueryTable.Add();
        AddDefineName(table);
       // QueryTable.
        return listobject;
    }
    public void AddDefineName(IListObject list)
    {
        WorksheetImpl sheet1 = list.Location.Worksheet as WorksheetImpl;

        WorkbookImpl book = sheet1.Workbook as WorkbookImpl;
        NameImpl name = book.Names.Add(list.QueryTable.Name) as NameImpl;
        name.SetValue(book.FormulaUtil.ParseString(list.Location.AddressGlobal));
        name.RefersToRange = list.Location;
        name.IsQueryTableRange = true;
        name.SheetIndex = list.Worksheet.Index;
    }
    internal void Dispose()
    {
        int count = this.Count;
        for (int i = count - 1; i > -1; i--)
        {
            ListObject Obj = (ListObject)this[i];
            Obj.Dispose();
        }
    }
    #endregion
    }
}
