#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Represents range protection and error indicators.
  /// </summary>
  [ Biff( TBIFFRecord.RangeProtection ) ]
  [ CLSCompliant( false ) ]
	public class RangeProtectionRecord : BiffRecordRaw
	{
    #region Class constants
    /// <summary>
    /// Represents length offset.
    /// </summary>
    private const int DEF_LENGTH_OFFSET = 19;
    /// <summary>
    /// Represents first unknown bytes.
    /// </summary>
    private readonly byte[] DEF_FIRST_UNKNOWN_BYTES = { 0x68, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0 };
    /// <summary>
    /// Represents data offset.
    /// </summary>
    private const int DEF_DATA_OFFSET = 27;
    /// <summary>
    /// Represents second unknown bytes.
    /// </summary>
    private readonly byte[] DEF_SECOND_UNKNOWN_BYTES = { 4, 0, 0, 0, 0, 0 };
    /// <summary>
    /// Represents default subrecord size.
    /// </summary>
    private const int DEF_SUBRECORD_SIZE = 8;
    /// <summary>
    /// Represents record end unknown bytes count.
    /// </summary>
    private const int DEF_FINISH_OFFSET = 4;
    /// <summary>
    /// Represents default subrecords size.
    /// </summary>
    public const int DEF_MAX_SUBRECORDS_SIZE = 1024;
    #endregion

    #region Class members
    /// <summary>
    /// Represents error indicator hide options.
    /// </summary>
    private ExcelIgnoreError m_ignoreOpt;
    /// <summary>
    /// Represents array of error indicator structure.
    /// </summary>
    private ErrorIndicatorImpl m_errorIndicator;
    /// <summary>
    /// Preserves range protection record
    /// </summary>
    internal MemoryStream m_preservedData;
    internal List<UnknownRecord> m_continueRecords;
    #endregion

    #region Class properties
    /// <summary>
    /// Represents hide options.
    /// </summary>
    public ExcelIgnoreError IgnoreOptions
    {
      get
      {
        return m_ignoreOpt;
      }
      set
      {
        m_ignoreOpt = value;
      }
    }
    /// <summary>
    /// Represents error indicators object.
    /// </summary>
    public ErrorIndicatorImpl ErrorIndicator
    {
      get
      {
        return m_errorIndicator;
      }
      set
      {
        m_errorIndicator = value;
      }
    }
    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_DATA_OFFSET + DEF_SUBRECORD_SIZE + DEF_FINISH_OFFSET;
      }
    }
#if DEBUG
    public List<Rectangle> Ranges
    {
      get
      {
        return ( m_errorIndicator != null ) ? 
          m_errorIndicator.CellList :
          null;
      }
    }
#endif
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  RangeProtectionRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  RangeProtectionRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  RangeProtectionRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      int iCount = provider.ReadUInt16( iOffset + DEF_LENGTH_OFFSET );

      int iOptionOffset = DEF_DATA_OFFSET + iCount * DEF_SUBRECORD_SIZE;

      if (iOptionOffset > iLength)
      {
          //Preserves range protection record if its data splitted into continue records
          m_preservedData = new MemoryStream(8224);
          byte[] data = new byte[iLength];
          provider.ReadArray(iOffset, data);
          m_preservedData.Write(data, 0, data.Length);
      }
      else
      {
          m_ignoreOpt = (ExcelIgnoreError)provider.ReadUInt16(iOffset + iOptionOffset);
          iOffset += DEF_DATA_OFFSET;

          if (m_ignoreOpt == ExcelIgnoreError.None)
              return;

          m_errorIndicator = new ErrorIndicatorImpl(m_ignoreOpt);

          for (int i = 0; i < iCount; i++)
          {
              int iFirstRow = provider.ReadUInt16(iOffset);
              iOffset += 2;

              int iLastRow = provider.ReadUInt16(iOffset);
              iOffset += 2;

              int iFirstCol = provider.ReadUInt16(iOffset);
              iOffset += 2;

              int iLastCol = provider.ReadUInt16(iOffset);
              iOffset += 2;

              Rectangle range = new Rectangle(iFirstCol, iFirstRow, iLastCol - iFirstCol, iLastRow - iFirstRow);
              m_errorIndicator.AddRange(range);
          }
      }
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
        if (m_preservedData != null)
        {
            m_preservedData.Position = 0;
            provider.WriteBytes(iOffset, m_preservedData.ToArray());
        }
        else
        {
            m_iLength = GetStoreSize(version);
            provider.WriteBytes(iOffset, DEF_FIRST_UNKNOWN_BYTES, 0, DEF_FIRST_UNKNOWN_BYTES.Length);
            iOffset += DEF_LENGTH_OFFSET;

            List<Rectangle> arrCells = m_errorIndicator.CellList;
            int iCount = arrCells.Count;
            provider.WriteUInt16(iOffset, (ushort)iCount);
            iOffset += 2;

            int iSecondLength = DEF_SECOND_UNKNOWN_BYTES.Length;
            provider.WriteBytes(iOffset, DEF_SECOND_UNKNOWN_BYTES, 0, iSecondLength);
            iOffset += iSecondLength;

            for (int i = 0; i < iCount; i++)
            {
                Rectangle rectangle = arrCells[i];

                provider.WriteUInt16(iOffset, (ushort)(rectangle.Y));
                iOffset += 2;

                provider.WriteUInt16(iOffset, (ushort)(rectangle.Bottom));
                iOffset += 2;

                provider.WriteUInt16(iOffset, (ushort)(rectangle.X));
                iOffset += 2;

                provider.WriteUInt16(iOffset, (ushort)(rectangle.Right));
                iOffset += 2;
            }

            provider.WriteInt32(iOffset, (int)m_ignoreOpt);
        }
    }
    /// <summary>
    /// Gets default record store size. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      OptimizeStorage();
      int iLength = ( m_errorIndicator == null )
        ? 0
        : m_errorIndicator.CellList.Count;

      return DEF_DATA_OFFSET + iLength * DEF_SUBRECORD_SIZE + DEF_FINISH_OFFSET;
    }
    /// <summary>
    /// Tries to reduce number of ranges.
    /// </summary>
    private void OptimizeStorage()
    {
      List<Rectangle> arrCells = m_errorIndicator.CellList;

      if( arrCells.Count > 1 )
      {
        int iStartCount;
        int iCurrentCount = arrCells.Count;

        do
        {
          iStartCount = iCurrentCount;
          SortedDictionary<int, SortedList<int, Rectangle>> dictionary = new SortedDictionary<int, SortedList<int, Rectangle>>();

          // Sort existing ranges by row
          for( int i = 0, len = arrCells.Count; i < len; i++ )
          {
            Rectangle rect = arrCells[ i ];

            SortedList<int, Rectangle> list;

            if( !dictionary.TryGetValue( rect.Top, out list ) )
            {
              list = new SortedList<int, Rectangle>();
              dictionary.Add( rect.Top, list );
            }

            list.Add( rect.Left, rect );
          }

          //IList<int> lstKeys = dictionary.Keys;
          // Re-add all ranges to decrease their count if possible.
          m_errorIndicator.Clear();

          //for( int i = 0, lenKeys = lstKeys.Count; i < lenKeys; i++ )
          foreach( int iKey in dictionary.Keys )
          {
            IList<Rectangle> lstRects = dictionary[ iKey ].Values;

            lstRects = CombineSameRowRectangles( lstRects );

            for( int j = 0, lenRects = lstRects.Count; j < lenRects; j++ )
            {
              m_errorIndicator.AddRange( lstRects[ j ] );
            }
          }

          iCurrentCount = m_errorIndicator.CellList.Count;
        }
        while( iStartCount != iCurrentCount );
      }
    }

    private IList<Rectangle> CombineSameRowRectangles( IList<Rectangle> lstRects )
    {
      if( lstRects == null  || lstRects.Count == 0 )
        return lstRects;

      List<Rectangle> lstCombined = new List<Rectangle>();
      lstCombined.Add( lstRects[ 0 ] );

      for( int i = 1, len = lstRects.Count; i < len; i++ )
      {
        int iLastIndex = lstCombined.Count - 1;
        Rectangle lastRect = lstCombined[ iLastIndex ];
        Rectangle currentRect = lstRects[ i ];

        if( lastRect.Top == currentRect.Top && lastRect.Bottom == currentRect.Bottom &&
          lastRect.Right + 1 == currentRect.Left )
        {
          lastRect = Rectangle.FromLTRB( lastRect.Left, lastRect.Top, currentRect.Right, lastRect.Bottom );
          lstCombined[ iLastIndex ] = lastRect;
        }
        else
        {
          lstCombined.Add( currentRect );
        }
      }

      return lstCombined;
    }
    #endregion
	}
}
