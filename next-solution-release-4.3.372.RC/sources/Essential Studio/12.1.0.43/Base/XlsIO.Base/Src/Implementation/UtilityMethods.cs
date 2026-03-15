#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT || WP
using System.Windows.Media;
using System.Xml;
#endif

#if SILVERLIGHT
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using System.Xml;
using Syncfusion.XlsIO;
#else
using System.Drawing;
using System.Xml;
#endif

namespace Syncfusion.XlsIO.Implementation
{
    ///<exclude/>
	/// <summary>
	/// This class contains utility methods, that cannot be logically placed in any other class.
	/// </summary>
	public sealed class UtilityMethods
	{
    #region Class constants
    /// <summary>
    /// Number of days that are incorrectly displayed by MS Excel.
    /// </summary>
    private const int DEF_WRONG_DATE = 61;
    /// <summary>
    /// Excel 2007 maximum row count.
    /// </summary>
    private const int DEF_EXCEL2007_MAX_ROW_COUNT = 1048576;
    /// <summary>
    /// Excel 2007 maximum column count.
    /// </summary>
    private const int DEF_EXCEL2007_MAX_COLUMN_COUNT = 16384;
    /// <summary>
    /// Excel 97-03 maximum row count.
    /// </summary>
    private const int DEF_EXCEL97TO03_MAX_ROW_COUNT = 65536;
    /// <summary>
    /// Excel 97-03 maximum column count.
    /// </summary>
    private const int DEF_EXCEL97TO03_MAX_COLUMN_COUNT = 256;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent creation instances of this class.
    /// </summary>
    private UtilityMethods()
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Determines if rectangles intersect with each other.
    /// </summary>
    /// <param name="rect1">The first rectangle to test.</param>
    /// <param name="rect2">The second rectangle to test.</param>
    /// <returns>This method returns true if there is any intersection.</returns>
    public static bool Intersects( Rectangle rect1, Rectangle rect2 )
    {
      return ( rect1.X <= rect2.X + rect2.Width )
        && ( rect2.X <= rect1.X + rect1.Width )
        && ( rect1.Y <= rect2.Y + rect2.Height )
        && ( rect2.Y <= rect1.Y + rect1.Height);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Contains( Rectangle rect, int x, int y )
    {
      if( rect.X <= x && x <= rect.X + rect.Width && rect.Y <= y )
      {
        return y <= rect.Y + rect.Height;
      }

      return false;
    }

    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to locate in the array.</param>
    /// <returns>
    /// The index of the first occurrence of value within the entire array, if found;
    ///  otherwise, -1.</returns>
    public static int IndexOf( TBIFFRecord[] array, TBIFFRecord value )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      for( int i = 0, len = array.Length; i < len; i++ )
      {
        if( array[ i ] == value ) return i;
      }

      return -1;
    }
    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to locate in the array.</param>
    /// <returns>
    /// The index of the first occurrence of value within the entire array, if found;
    ///  otherwise, -1.</returns>
    public static int IndexOf( int[] array, int value )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      for( int i = 0, len = array.Length; i < len; i++ )
      {
        if( array[ i ] == value ) return i;
      }

      return -1;
    }
    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to locate in the array.</param>
    /// <returns>
    /// The index of the first occurrence of value within the entire array, if found;
    ///  otherwise, -1.</returns>
    public static int IndexOf( short[] array, short value )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      for( int i = 0, len = array.Length; i < len; i++ )
      {
        if( array[ i ] == value ) return i;
      }

      return -1;
    }
    /// <summary>
    /// Converts DateTime into number.
    /// </summary>
    /// <param name="dateTime">Value to convert.</param>
    /// <returns>Converted value.</returns>
    public static double ConvertDateTimeToNumber( DateTime dateTime )
    {
      double dNumber = dateTime.ToOADate();

      if( dNumber < DEF_WRONG_DATE )
      {
        // We are decreasing one day because OADate starts from 31 December 1899,
        // but MS Excel date from 1 January 1900.
        dNumber--;
      }

      return dNumber;
    }
    /// <summary>
    /// Converts number into DateTime.
    /// </summary>
    /// <param name="dNumber">Number to convert.</param>
    /// <returns>Converted value.</returns>
    public static DateTime ConvertNumberToDateTime(double dNumber, bool is1904DateSystem)
    {
        if (is1904DateSystem)
            dNumber += WorkbookImpl.Date1904SystemDifference;

        else if (dNumber < DEF_WRONG_DATE)
        {
            // We are adding one day, because FromOADate starts from 31 December 1899
            // and Excel date starts from 1 January 1900
            dNumber++;
        }
     
        
      return
#if ( WINRT )
          DateTimeExtension.FromOADate(dNumber);
#else
          DateTime.FromOADate( dNumber );
#endif
    }
    /// <summary>
    /// Creates new cell without adding it to the collection..
    /// </summary>
    /// <param name="iRow">Zero-based row index of the cell to create.</param>
    /// <param name="iColumn">Zero-based column index of the cell to create.</param>
    /// <param name="recordType">Record type.</param>
    /// <returns>Created cell.</returns>
    [ CLSCompliant( false ) ]
    public static ICellPositionFormat CreateCell( int iRow, int iColumn, TBIFFRecord recordType )
    {
      ICellPositionFormat cell = ( ICellPositionFormat )
        BiffRecordFactory.GetRecord( recordType );

      cell.Row = iRow;
      cell.Column = iColumn;

      return cell;
    } 
    /// <summary>
    /// Removes first character from the string.
    /// Warning: this method doesn't performs any argument check for performance purposes.
    /// </summary>
    /// <param name="value">Value to remove first character from.</param>
    /// <returns>Updated string.</returns>
    public static string RemoveFirstCharUnsafe( string value )
    {
      return value.Substring( 1, value.Length - 1 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="separator"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Join( string separator, List<string> value )
    {
      if( separator == null )
        separator = string.Empty;

      if( value == null )
        throw new ArgumentNullException( "value" );

      int iTotalLength = 0;
      int iCount = value.Count;
      string strCurrentValue;

      for( int i = 0; i < iCount; i++ )
      {
        strCurrentValue = value[ i ];

        if( strCurrentValue != null )
          iTotalLength += strCurrentValue.Length;
      }

      iTotalLength += ( iCount - 1 ) * separator.Length;

      if( iTotalLength < 0 || iTotalLength + 1 < 0 )
        throw new OutOfMemoryException();

      if( iTotalLength == 0 )
        return string.Empty;

      StringBuilder builder = new StringBuilder();
      strCurrentValue = value[ 0 ];

      if( value != null )
        builder.Append( value[ 0 ] );

      for( int j = 1; j < iCount; j++ )
      {
        builder.Append( separator );
        strCurrentValue = value[ j ];

        if( strCurrentValue == null )
          strCurrentValue = string.Empty;

        builder.Append( strCurrentValue );
      }

      return builder.ToString();
    }
    /// <summary>
    /// Gets maximum row and column count for specific version.
    /// </summary>
    /// <param name="iRows"></param>
    /// <param name="iColumns"></param>
    /// <param name="version"></param>
    public static void GetMaxRowColumnCount( out int iRows, out int iColumns, ExcelVersion version )
    {
      switch( version )
      {
        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          iRows = DEF_EXCEL2007_MAX_ROW_COUNT;
          iColumns = DEF_EXCEL2007_MAX_COLUMN_COUNT;
          break;

        case ExcelVersion.Excel97to2003:
          iRows = DEF_EXCEL97TO03_MAX_ROW_COUNT;
          iColumns = DEF_EXCEL97TO03_MAX_COLUMN_COUNT;
          break;

        default:
          throw new ArgumentException( "Unknown version" );
      }
    }
    /// <summary>
    /// Copies one stream into another.
    /// </summary>
    /// <param name="source">Source stream to copy from.</param>
    /// <param name="destination">Destination stream to copy into.</param>
    public static void CopyStreamTo( Stream source, Stream destination )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      const int BufferSize = 32768;
      int iReadCount;
      byte[] arrBuffer = new byte[ BufferSize ];

      while( ( iReadCount = source.Read( arrBuffer, 0, BufferSize ) ) > 0 )
      {
        destination.Write( arrBuffer, 0, iReadCount );
      }
    }
    /// <summary>
    /// Creates copy of the MemoryStream.
    /// </summary>
    /// <param name="source">Source stream to copy.</param>
    /// <returns>A copy of the original MemoryStream.</returns>
    public static MemoryStream CloneStream( MemoryStream source )
    {
      MemoryStream result = new MemoryStream( ( int )source.Length );
      long lStartPosition = source.Position;
      source.Position = 0;
      CopyStreamTo( source, result );
      result.Position = source.Position = lStartPosition;

      return result;
    }
    /// <summary>
    /// Creates xml reader to read data from the stream.
    /// </summary>
    /// <param name="data">Data to read.</param>
    /// <returns>Created xml reader.</returns>
    public static XmlReader CreateReader( Stream data, bool skipToElement )
    {
        if (data.CanSeek && data.Position != 0)
                data.Position = 0;

      XmlReader result;
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
      result = new XmlTextReader( data );
#else
      result = XmlReader.Create( data );
#endif

      if( skipToElement )
      {
        while( result.NodeType != XmlNodeType.Element )
          result.Read();
      }

      return result;
    }
    /// <summary>
    /// Creates xml reader to read data from the stream.
    /// </summary>
    /// <param name="data">Data to read.</param>
    /// <returns>Created xml reader.</returns>
    public static XmlReader CreateReader( Stream data )
    {
      return CreateReader( data, true );
    }
    /// <summary>
    /// Creates xml writer to read data from the stream.
    /// </summary>
    /// <param name="data">Data to read.</param>
    /// <returns>Created xml writer.</returns>
    public static XmlWriter CreateWriter( Stream data, Encoding encoding )
    {
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
      return new XmlTextWriter( data, encoding );
#else
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Encoding = encoding;
      return XmlWriter.Create( data, settings );
#endif
    }
    /// <summary>
    /// Creates xml writer to read data from the stream.
    /// </summary>
    /// <param name="data">Data to read.</param>
    /// <returns>Created xml writer.</returns>
    public static XmlWriter CreateWriter( TextWriter data )
    {
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
      return new XmlTextWriter( data );
#else
      XmlWriterSettings settings = new XmlWriterSettings();
      return XmlWriter.Create( data, settings );
#endif
    }
    /// <summary>
    /// Creates xml writer to read data from the stream.
    /// </summary>
    /// <param name="data">Data to read.</param>
    /// <returns>Created xml writer.</returns>
    public static XmlWriter CreateWriter( TextWriter data, bool indent )
    {
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
      XmlTextWriter result = new XmlTextWriter( data );
      result.Formatting = Formatting.Indented;
      return result;
#else
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = indent;
      return XmlWriter.Create( data, settings );
#endif
    }
    #endregion
  }
}
