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
using System.Xml;
using Syncfusion.XlsIO.Implementation.Tables;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;

namespace Syncfusion.XlsIO.Implementation.XmlReaders
{
  /// <summary>
  /// This class is responsible for table parsing.
  /// </summary>
  class TableParser
  {
    /// <summary>
    /// Extracts specified list object from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get table from.</param>
    /// <param name="sheet">Worksheet to put extracted table into.</param>
    public IListObject Parse( XmlReader reader, IWorksheet sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != ListObjects.Table )
        throw new XmlException();

      string name = null;
      IRange location = null;

      if( reader.MoveToAttribute( ListObjects.NameAttribute ) )
        name = reader.Value;

      if( reader.MoveToAttribute( ListObjects.Reference ) )
        location = sheet[ reader.Value ];

      IListObject table = sheet.ListObjects.Create( name, location );

      if( reader.MoveToAttribute( ListObjects.IdAttribute ) )
      {
        int index = ( table as ListObject ).Index = XmlConvert.ToInt32( reader.Value );
        WorkbookImpl book = sheet.Workbook as WorkbookImpl;
        int iCurrentTableIndex = book.MaxTableIndex;
        book.MaxTableIndex = Math.Max( iCurrentTableIndex, book.MaxTableIndex );
      }
      
      if( reader.MoveToAttribute( ListObjects.DisplayName ) )
        table.DisplayName = reader.Value;

      if (reader.MoveToAttribute(ListObjects.TableType))
      {
#if (!(SILVERLIGHT))
          (table as ListObject).TableType = (ExcelTableType)Enum.Parse(typeof(ExcelTableType), reader.Value);
#else
          (table as ListObject).TableType = (ExcelTableType)Enum.Parse(typeof(ExcelTableType), reader.Value, true);
#endif
      }

      ListObject listObject = table as ListObject;

      if (reader.MoveToAttribute(ListObjects.ShowHeaderRow))
          listObject.ShowHeaderRow = XmlConvert.ToBoolean(reader.Value);

      listObject.TotalsRowCount = reader.MoveToAttribute( ListObjects.TotalsRowCount ) ?
        XmlConvert.ToInt32( reader.Value ) :
        0;

      if( reader.MoveToAttribute( ListObjects.TotalsRowShown ) )
      {
        bool bTotalsShown = listObject.TotalsRowShown = XmlConvert.ToInt32( reader.Value ) != 0;
        listObject.TotalsRowCount = 0;
      }
      else if( listObject.TotalsRowCount != 0 )
      {
        listObject.TotalsRowShown = true;
      }

      if( reader.MoveToAttribute( "insertRowShift" ) )
      {
        string value = reader.Value;
        listObject.InsertRowShift = XmlConvert.ToInt32( value );
      }
      
      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ListObjects.AutoFilter:
                ParseAutoFilter( reader, table );
                break;

              case ListObjects.TableColumns:
                ParseColumns( reader, table.Columns );
                break;

              case ListObjects.TableStyleInfo:
                ParseStyle( reader, table );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();
      return table;
    }
    /// <summary>
    /// Extracts autofilter part of the table.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="table">Table to put extracted data into.</param>
    private void ParseAutoFilter( XmlReader reader, IListObject table )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( table == null )
        throw new ArgumentNullException( "table" );

      if( reader.LocalName != ListObjects.AutoFilter )
        throw new XmlException();

      ListObject listObject = ( ListObject )table;

      listObject.AutoFilterStream = ShapeParser.ReadNodeAsStream( reader );
      //reader.Skip();
      // <autoFilter ref="B5:D9" />
      //writer.WriteStartElement( ListObjects.AutoFilter );
      //writer.WriteAttributeString( ListObjects.Reference, table.Location.AddressLocal );
      //writer.WriteEndElement();
    }
    /// <summary>
    /// Extracts table style settings.
    /// </summary>
    /// <param name="reader">XmlReader to get style settings from.</param>
    /// <param name="table">Table to put extracted style settings into.</param>
    private void ParseStyle( XmlReader reader, IListObject table )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( table == null )
        throw new ArgumentNullException( "table" );

      if( reader.LocalName != ListObjects.TableStyleInfo )
        throw new XmlException();

      ListObject listObject = (ListObject)table;

      if (reader.MoveToAttribute(ListObjects.NameAttribute))
      {
          bool isEnumValue = Enum.IsDefined(typeof(TableBuiltInStyles), reader.Value);
          if (isEnumValue)
              table.BuiltInTableStyle = (TableBuiltInStyles)Enum.Parse(typeof(TableBuiltInStyles), reader.Value, false);
          listObject.TableStyleName = reader.Value;
      }

      if (reader.MoveToAttribute(ListObjects.ShowFirstColumn))
          listObject.ShowFirstColumn = XmlConvert.ToBoolean(reader.Value);

      if (reader.MoveToAttribute(ListObjects.ShowLastColumn))
          listObject.ShowLastColumn = XmlConvert.ToBoolean(reader.Value);

      if( reader.MoveToAttribute( ListObjects.ShowRowStripes ) )
        listObject.ShowTableStyleRowStripes = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( ListObjects.ShowColumnStripes ) )
        listObject.ShowTableStyleColumnStripes = XmlConvert.ToBoolean( reader.Value );

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Extracts all columns of the table.
    /// </summary>
    /// <param name="reader">XmlReader to get columns data from.</param>
    /// <param name="columns">List with columns to serializes.</param>
    private void ParseColumns( XmlReader reader, IList<IListObjectColumn> columns )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( columns == null )
        throw new ArgumentNullException( "columns" );

      if( reader.LocalName != ListObjects.TableColumns )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        for( int i = 0, len = columns.Count; i < len; i++ )
        {
          ParseColumn( reader, columns, i );
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts single column.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="columns">Columns collection to put extracted column into.</param>
    /// <returns>Extracted column.</returns>
    private void ParseColumn( XmlReader reader, IList<IListObjectColumn> columns, int columnIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ListObjects.TableColumn )
        throw new XmlException();

      IListObjectColumn column = null;

      if( reader.MoveToAttribute( ListObjects.IdAttribute ) )
      {
        int index = XmlConvert.ToInt32( reader.Value );
        column = columns[ columnIndex ];
        ( column as ListObjectColumn ).Id = index;
      }

      if( reader.MoveToAttribute( ListObjects.NameAttribute ) )
        column.Name = reader.Value;

      if( reader.MoveToAttribute( ListObjects.TotalsRowLabel ) )
        column.TotalsRowLabel = reader.Value;

      if( reader.MoveToAttribute( ListObjects.TotalsRowFunction ) )
      {
        column.TotalsCalculation = ( ExcelTotalsCalculation )Enum.Parse(
          typeof( ExcelTotalsCalculation ), reader.Value, true );
      }

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case "calculatedColumnFormula":
                column.CalculatedFormula = reader.ReadElementContentAsString();
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();
    }

    public void ParseQueryTable(XmlReader reader,IListObject Table)
    {
        ListObject listobject = Table as ListObject;
        int columnCount=0;
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (Table == null)
            throw new ArgumentNullException("sheet");

        if (reader.LocalName != ListObjects.QueryTable)
            throw new XmlException();
        WorkbookImpl book = Table.Location.Worksheet.Workbook as WorkbookImpl;
        int ConncetionId = 0;
        if (reader.MoveToAttribute(ListObjects.ConnectionId))        
            ConncetionId = reader.ReadContentAsInt();

        ExternalConnection connection = FindConnection(book.Connections as ExternalConnectionCollection, ConncetionId);
        if (connection == null)
            connection = FindConnection(book.DeletedConnections as ExternalConnectionCollection, ConncetionId);
        connection.Range = Table.Location;
        connection.IsExist = true;
        QueryTableImpl queryTable=new QueryTableImpl(Table.Location.Application,Table.Location.Parent,connection);
        if (reader.MoveToAttribute(ListObjects.NameAttribute))
            queryTable.Name = reader.Value;
        
        reader.Read();
        if (reader.LocalName == ListObjects.QueryTableRefresh)
            reader.Read();
        if (reader.LocalName == ListObjects.QueryTableFields)
        {
            if (reader.MoveToAttribute(ListObjects.Count))
            {
                columnCount = reader.ReadContentAsInt();
                reader.Read();
            }
            for (int i = 0; i < columnCount; i++)
            {
                ParseQueryTableField(reader, Table.Columns[i]);
            }
        }
        listobject.QueryTable = queryTable;
    }

    public void ParseQueryTableField(XmlReader reader,IListObjectColumn TableColumn)
    {
        ListObjectColumn Column = TableColumn as ListObjectColumn;
        if (reader.MoveToAttribute(ListObjects.IdAttribute))
            Column.QueryTableFieldId = reader.ReadContentAsInt();
        if (reader.MoveToAttribute(ListObjects.NameAttribute))
            Column.Name = reader.Value;
        if (reader.MoveToAttribute(ListObjects.TableColumnId))
            Column.Id = reader.ReadContentAsInt();
        reader.Read();
    }

    private ExternalConnection FindConnection(ExternalConnectionCollection Connections,int QuertTableId)
    {
        for (int i = 0; i < Connections.Count; i++)
        {
            ExternalConnection connection=Connections[i] as ExternalConnection;
            if (connection.ConncetionId == QuertTableId)
                return connection;
        }
        return null;
    }

   

  }
}
