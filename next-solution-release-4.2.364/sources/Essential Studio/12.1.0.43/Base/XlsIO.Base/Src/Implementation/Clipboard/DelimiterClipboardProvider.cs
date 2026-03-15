#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.Clipboard
{
  /// <summary>
  /// DelimiterClipboardProvider - derived from the ClipboardProvider.
  /// Facilitates reading from and writing to the clipboard in unicode text 
  /// format when there is a specified string delimiter between worksheet
  /// columns and another delimiter between rows.
  /// </summary>
  public class DelimiterClipboardProvider : ClipboardProvider
  {
    #region Class constants
    /// <summary>
    /// Default format name.
    /// </summary>
    public const string DEF_DEFAULT_FORMAT_NAME = "UnicodeText";
    /// <summary>
    /// Default delimiter between worksheet columns.
    /// </summary>
    public const string DEF_DEFAULT_COLUMN_DELIMITER = "\t";
    /// <summary>
    /// Default delimiter between worksheet rows.
    /// </summary>
    public const string DEF_DEFAULT_ROW_DELIMITER = "\r\n";
    #endregion

    #region Class members
    /// <summary>
    /// String that will be copied to the clipboard, contains worksheet's data.
    /// </summary>
    private StringBuilder m_strCSVValue = new StringBuilder();
    /// <summary>
    /// Delimiter between worksheet columns.
    /// </summary>
    private string m_strColDelim = DEF_DEFAULT_COLUMN_DELIMITER;
    /// <summary>
    /// Delimiter between worksheet rows.
    /// </summary>
    private string m_strRowDelim = DEF_DEFAULT_ROW_DELIMITER;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor - just for getting the workbook from the clipboard.
    /// If data needs to be copied from the clipboard, then the Initialize 
    /// method or set Workbook or Worksheet properties should be called.
    /// </summary>
    public DelimiterClipboardProvider() : this( null )
    {
    }

    /// <summary>
    /// Creates provider for the specified worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet that should be copied to the clipboard.</param>
    public DelimiterClipboardProvider( IWorksheet sheet ) : this( sheet, null )
    {
    }

    /// <summary>
    /// Creates provider for the specified worksheet and 
    /// sets the next clipboard provider.
    /// </summary>
    /// <param name="sheet">Worksheet that should be copied to the clipboard.</param>
    /// <param name="next">Next clipboard provider in the list.</param>
    public DelimiterClipboardProvider( IWorksheet sheet, ClipboardProvider next )
      : base( sheet, next )
    {
      FormatName = DEF_DEFAULT_FORMAT_NAME;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Returns IDataObject (for copying to the clipboard)
    /// that contains data from workbook or worksheet.
    /// </summary>
    /// <returns>IDataObject for copying to the clipboard.</returns>
    public override System.Windows.Forms.IDataObject GetForClipboard()
    {
      FillData();

      return new DataObject( FormatName, m_strCSVValue.ToString() );
    }

    /// <summary>
    /// Returns IDataObject (for copying to the clipboard)
    /// that contains data from workbook or worksheet.
    /// </summary>
    /// <param name="range">Range to copy.</param>
    /// <returns>IDataObject for copying to the clipboard.</returns>
    public override System.Windows.Forms.IDataObject GetForClipboard( IRange range )
    {
      FillData( range );

      return new DataObject( FormatName, m_strCSVValue.ToString() );
    }

    /// <summary>
    /// Fills internal string that will be copied to the clipboard.
    /// </summary>
    protected virtual void FillData()
    {
      IRange usedRange = Worksheet.UsedRange;
      FillData( usedRange );
    }

    /// <summary>
    /// Fills internal string that will be copied to the clipboard.
    /// </summary>
    /// <param name="range">Range to copy to the clipboard.</param>
    protected virtual void FillData( IRange range )
    {
      m_strCSVValue.Length = 0;
      int iFirstColumn = range.Column;
      int iLastColumn = range.LastColumn;
      int iLastRow = range.LastRow;

      WorksheetImpl sheet = ( WorksheetImpl )Worksheet;
      CellRecordCollection arrCells = sheet.CellRecords;

      for( int i = range.Row; i <= iLastRow; i++ )
      {
        for( int j = iFirstColumn; j <= iLastColumn; j++ )
        {
          if( arrCells.Contains( i, j ) )
          {
            m_strCSVValue.Append( Worksheet.Range[ i, j ].Value );
          }

          if( j != iLastColumn )
            m_strCSVValue.Append( ColumnDelimiter );
        }

        m_strCSVValue.Append( RowDelimiter );
      }
    }
    /// <summary>
    /// Extracts workbook from the data object.
    /// </summary>
    /// <param name="dataObject">Data object that contains workbook data.</param>
    /// <param name="workbooks">Workbooks collection to add workbook to.</param>
    /// <returns>Extracted workbook.</returns>
    protected override IWorkbook ExtractWorkbook( IDataObject dataObject, IWorkbooks workbooks )
    {
      if( dataObject != null )
      {
        if( dataObject.GetDataPresent( FormatName ) )
        {
          object objData = dataObject.GetData( FormatName );
          
          if( objData is string )
          {
            return GetBookFromString( ( string )objData, workbooks );
          }
          else if( objData is MemoryStream )
          {
            return GetBookFromStream( ( MemoryStream )objData, workbooks );
          }
        }
      }
      
      return null;
    }
    /// <summary>
    /// Fills the dataobject.
    /// </summary>
    /// <param name="dataObject">Dataobject to extract.</param>
    protected override void FillDataObject( IDataObject dataObject )
    {
      FillData();
      dataObject.SetData( FormatName,  m_strCSVValue.ToString() );
    }
    /// <summary>
    /// Extracts the workbook.
    /// </summary>
    /// <param name="dataObject">Data object to extract.</param>
    /// <param name="range">Range to copy into data object.</param>
    protected override void FillDataObject(IDataObject dataObject, IRange range)
    {
      FillData( range );
      dataObject.SetData( FormatName,  m_strCSVValue.ToString() );
    }

    #endregion

    #region Private Class Methods
    /// <summary>
    /// Converts string to the worksheet.
    /// </summary>
    /// <param name="sheetData">String that should be converted.</param>
    /// <param name="application">Application object for the new worksheet.</param>
    /// <param name="parent">Parent object for the new worksheet.</param>
    /// <returns>Newly created worksheet with values from the string.</returns>
    private IWorksheet GetSheetFromString( string sheetData, 
      IApplication application, object parent )
    {
      IWorksheet result = new WorksheetImpl( application, parent );
      FillSheet( result, sheetData );
      
      return result;
    }
//    /// <summary>
//    /// Converts memory stream data to the worksheet.
//    /// </summary>
//    /// <param name="sheetData">MemoryStream that contains data for the worksheet.</param>
//    /// <param name="application">Application object for the worksheet.</param>
//    /// <param name="parent">Parent object for the worksheet.</param>
//    /// <returns>Newly created worksheet with values from the MemoryStream.</returns>
//    private IWorksheet GetSheetFromStream( MemoryStream sheetData, 
//      IApplication application, object parent )
//    {
//      string strData = Encoding.Unicode.GetString( sheetData.ToArray() );
//      return GetSheetFromString( strData, application, parent );
//    }
    /// <summary>
    /// Converts string to the workbook.
    /// </summary>
    /// <param name="stringData">String that should be converted.</param>
    /// <param name="workbooks">Workbooks collection to add workbook to.</param>
    /// <returns>Newly created workbook with values from the string.</returns>
    private IWorkbook  GetBookFromString( string stringData, 
      IWorkbooks workbooks )
    {
      if( stringData == null )
        throw new ArgumentNullException( "stringData" );

      if( stringData.Length == 0 )
        throw new ArgumentException( "stringData - string cannot be empty" );

      if( workbooks == null )
        throw new ArgumentNullException( "workbooks" );

      IWorkbook resultBook = workbooks.Create( 1 );

      IWorksheet sheet = resultBook.Worksheets[ 0 ];

      FillSheet( sheet, stringData );
      
      return resultBook;
    }
    /// <summary>
    /// Converts memory stream data to the workbook.
    /// </summary>
    /// <param name="streamData">MemoryStream that contains data for the workbook.</param>
    /// <param name="workbooks">Workbooks collection to add workbook to.</param>
    /// <returns>Newly created workbook with values from the MemoryStream.</returns>
    private IWorkbook GetBookFromStream( MemoryStream streamData, 
      IWorkbooks workbooks )
    {
      if( streamData == null )
        throw new ArgumentNullException( "streamData" );

      if( workbooks == null )
        throw new ArgumentNullException( "workbooks" );

      string strData = Encoding.Unicode.GetString( streamData.ToArray() );
      return GetBookFromString( strData, workbooks );
    }
    /// <summary>
    /// Fills a specified sheet with values from the specified string.
    /// </summary>
    /// <param name="sheet">Sheet that should receive data from the string.</param>
    /// <param name="sheetData">String with worksheet data.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When the sheet is NULL.
    /// </exception>
    private void FillSheet( IWorksheet sheet, string sheetData )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      int i = 0;
      int iCol = 1;
      int iRow = 1;
      int iStartPos = 0;
      int lastPossibleCol = sheetData.Length - ColumnDelimiter.Length;
      int lastPossibleRow = sheetData.Length - RowDelimiter.Length;

      while( i < sheetData.Length )
      {
        if( sheetData[ i ] == '"' || sheetData[ i ] == '\'' )
        {
          i = SkipString( sheetData, i );
        }
        else if ( i <= lastPossibleCol && 
          sheetData.Substring( i, ColumnDelimiter.Length ) == ColumnDelimiter )
        {
          sheet.Range[ iRow, iCol ].Value = sheetData.Substring( iStartPos, 
            i - iStartPos );
          iStartPos = i + ColumnDelimiter.Length;
          i = iStartPos;
          iCol++;
        }
        else if ( i <= lastPossibleRow &&
          sheetData.Substring( i, RowDelimiter.Length ) == RowDelimiter )
        {
          sheet.Range[ iRow, iCol ].Value = sheetData.Substring( iStartPos, 
            i - iStartPos );
          iStartPos = i + RowDelimiter.Length;
          i = iStartPos;
          iRow++;
          iCol = 1;
        }
        else i++;
      }
    }
    /// <summary>
    /// Returns index of the character after string 
    /// (sequence of characters inside "" or '').
    /// </summary>
    /// <param name="sheetData">Contains string.</param>
    /// <param name="pos">Position of the opening " or '.</param>
    /// <returns>Index of the character after closing " or '.</returns>
    private int SkipString( string sheetData, int pos )
    {
      return sheetData.IndexOf( sheetData[ pos ], pos + 1 ) + 1;
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Delimiter between worksheet columns.
    /// </summary>
    public string ColumnDelimiter
    {
      get
      {
        return m_strColDelim;
      }
      set
      {
        m_strColDelim = value;
      }
    }
    /// <summary>
    /// Delimiter between worksheet rows.
    /// </summary>
    public string RowDelimiter
    {
      get
      {
        return m_strRowDelim;
      }
      set
      {
        m_strRowDelim = value;
      }
    }
    #endregion
  }
}
