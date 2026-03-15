#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Tables;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// This class is responsible for table object serialization in Excel 2007 xml format.
  /// </summary>
  class TableSerializator
  {
    /// <summary>
    /// Serializes specified list object inside XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="table">Table to serialize.</param>
    public void Serialize( XmlWriter writer, IListObject table )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( table == null )
        throw new ArgumentNullException( "table" );

      writer.WriteStartElement( ListObjects.Table, Excel2007Serializator.XmlNamespaceMain );
      writer.WriteAttributeString( ListObjects.IdAttribute, table.Index.ToString() );
      writer.WriteAttributeString( ListObjects.NameAttribute, table.Name );
      writer.WriteAttributeString( ListObjects.DisplayName, table.DisplayName );
      writer.WriteAttributeString( ListObjects.Reference, table.Location.AddressLocal );
      ListObject typedTable = table as ListObject;
      
      if (!typedTable.ShowHeaderRow)
          writer.WriteAttributeString(ListObjects.ShowHeaderRow, Excel2007Serializator.FalseValue);
      if (table.QueryTable!=null)
      {
          writer.WriteAttributeString(ListObjects.TableType, ExcelTableType.queryTable.ToString());
          writer.WriteAttributeString(ListObjects.InsertRow, "1");
      }

      Excel2007Serializator.SerializeAttribute( writer, "insertRowShift", typedTable.InsertRowShift, 0 );

      if( !( table as ListObject ).TotalsRowShown )
      {
        Excel2007Serializator.SerializeAttribute( writer, ListObjects.TotalsRowShown, false, true );
      }
      else
      {
        Excel2007Serializator.SerializeAttribute( writer, ListObjects.TotalsRowCount, table.TotalsRowCount, 0 );
        //writer.WriteAttributeString( ListObjects.TotalsRowCount, table.TotalsRowCount.ToString() );
      }

      SerializeAutoFilter( writer, table );
      SerializeColumns( writer, table.Columns,typedTable.TableType );
      SerializeStyle( writer, table );
      if (typedTable.TableType == ExcelTableType.queryTable)
      {
          //SerializeQueryTable(writer, typedTable);
      }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes autofilter part of the table.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="table">Table to serialize.</param>
    private void SerializeAutoFilter( XmlWriter writer, IListObject table )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( table == null )
        throw new ArgumentNullException( "table" );

      if( !table.ShowHeaderRow )
        return;

      // <autoFilter ref="B5:D9" />
      ListObject listObject = ( ListObject )table;

      Stream autofilter = listObject.AutoFilterStream;

      if( autofilter != null )
      {
        autofilter.Position = 0;
        ShapeParser.WriteNodeFromStream( writer, autofilter );
      }
      else
      {
        int iTotalsCount = table.TotalsRowCount;
        IRange autoFilterLocation = table.Location;

        if( iTotalsCount > 0 )
        {
          autoFilterLocation = autoFilterLocation.Worksheet[ autoFilterLocation.Row, autoFilterLocation.Column,
            autoFilterLocation.LastRow - iTotalsCount, autoFilterLocation.LastColumn ];
        }

        writer.WriteStartElement( ListObjects.AutoFilter );
        writer.WriteAttributeString( ListObjects.Reference, autoFilterLocation.AddressLocal );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes table style settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize style settings into.</param>
    /// <param name="table">Table to serializes style settings for.</param>
    private void SerializeStyle( XmlWriter writer, IListObject table )
    {
      ListObject listObject = ( ListObject )table;
      TableBuiltInStyles style = table.BuiltInTableStyle;
      if((listObject.TableStyleName != null) || (style != 0))
      {
        writer.WriteStartElement( ListObjects.TableStyleInfo );
        if (style != 0)
            writer.WriteAttributeString(ListObjects.NameAttribute, style.ToString());
        else
            writer.WriteAttributeString(ListObjects.NameAttribute, listObject.TableStyleName);
        
        writer.WriteAttributeString(ListObjects.ShowFirstColumn, (listObject.ShowFirstColumn ? 1 : 0).ToString());
        writer.WriteAttributeString(ListObjects.ShowLastColumn, (listObject.ShowLastColumn ? 1 : 0).ToString());
        writer.WriteAttributeString( ListObjects.ShowRowStripes, ( listObject.ShowTableStyleRowStripes ? 1 : 0 ).ToString() );
        writer.WriteAttributeString( ListObjects.ShowColumnStripes, ( listObject.ShowTableStyleColumnStripes ? 1 : 0 ).ToString() );

        writer.WriteEndElement();
      }

      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Serializes all columns of the table.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="columns">List with columns to serializes.</param>
    private void SerializeColumns( XmlWriter writer, IList<IListObjectColumn> columns ,ExcelTableType Type )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( columns == null )
        throw new ArgumentNullException( "columns" );

      writer.WriteStartElement( ListObjects.TableColumns );

      for( int i = 0, len = columns.Count; i < len; i++ )
      {
        IListObjectColumn column = columns[ i ];
        SerializeColumn( writer, column ,Type);
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single column.
    /// </summary>
    /// <param name="writer">XmlWriter to serializes into.</param>
    /// <param name="column">Column to serialize.</param>
    private void SerializeColumn( XmlWriter writer, IListObjectColumn column,ExcelTableType type )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( column == null )
        throw new ArgumentNullException( "column" );

      writer.WriteStartElement( ListObjects.TableColumn );
      writer.WriteAttributeString( ListObjects.IdAttribute, column.Id.ToString() );
      writer.WriteAttributeString( ListObjects.NameAttribute, column.Name );
      Excel2007Serializator.SerializeAttribute( writer, ListObjects.TotalsRowLabel, column.TotalsRowLabel, null );
      if (type == ExcelTableType.queryTable)
      {
          writer.WriteAttributeString(ListObjects.QueryTableFieldId, column.QueryTableFieldId.ToString());
          writer.WriteAttributeString(ListObjects.UniqueName, column.QueryTableFieldId.ToString());          
      }
      if( column.TotalsCalculation != ExcelTotalsCalculation.None )
      {
        string totalName = column.TotalsCalculation.ToString();
        totalName = Excel2007Serializator.LowerFirstLetter( totalName );
        writer.WriteAttributeString( ListObjects.TotalsRowFunction, totalName );
      }

      string strCalculatedFormula = column.CalculatedFormula;

      if( strCalculatedFormula != null )
      {
        writer.WriteElementString( "calculatedColumnFormula", strCalculatedFormula );
      }

      writer.WriteEndElement();
    }
    public void SerializeQueryTable(IListObject Table,XmlWriter writer)
    {
        if (writer == null)
            throw new ArgumentNullException("Writer");
        if (writer == null)
            throw new ArgumentNullException("writer");

        writer.WriteStartDocument(true);
        writer.WriteStartElement(ListObjects.QueryTable, Excel2007Serializator.XmlNamespaceMain);
        writer.WriteAttributeString(ListObjects.NameAttribute, Table.QueryTable.Name);
        Excel2007Serializator.SerializeBool(writer, ListObjects.RefreshOnLoad, Table.QueryTable.RefreshOnFileOpen);
        writer.WriteAttributeString(ListObjects.ConnectionId, Table.QueryTable.ConncetionId.ToString());
        if (!Table.QueryTable.BackgroundQuery)
            writer.WriteAttributeString(Excel2007Serializator.BackgroundRefresh, "0");
        writer.WriteStartElement(ListObjects.QueryTableRefresh);
        int column_count = Table.Columns.Count;
        writer.WriteAttributeString(ListObjects.NextId, (column_count + 1).ToString());
        writer.WriteStartElement(ListObjects.QueryTableFields);
        writer.WriteAttributeString(ListObjects.Count, column_count.ToString());
        IList<IListObjectColumn> listcolumn = Table.Columns;
        for (int i = 0; i < column_count; i++)
        {
            writer.WriteStartElement(ListObjects.QueryTableField);
            writer.WriteAttributeString(ListObjects.IdAttribute, listcolumn[i].QueryTableFieldId.ToString());
            writer.WriteAttributeString(ListObjects.NameAttribute, listcolumn[i].Name);
            writer.WriteAttributeString(ListObjects.TableColumnId, listcolumn[i].Id.ToString());
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();

    }    
  }
}
