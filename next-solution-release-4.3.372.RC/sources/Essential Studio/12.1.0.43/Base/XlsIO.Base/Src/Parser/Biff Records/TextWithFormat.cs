#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Interfaces;
#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif (WP)
using Syncfusion.XlsIO.Implementation.WP;
#endif
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for TextFormat.
  /// </summary>
  public class TextWithFormat
    : IComparable
    , ICloneable 
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const byte DEF_COMPRESSED_MASK  = 0x01;
    /// <summary>
    /// 
    /// </summary>
    private const byte DEF_RICHTEXT_MASK    = 0x08;
    /// <summary>
    /// Size of a single formatting run.
    /// </summary>
    internal const int DEF_FR_SIZE          = 4;
    /// <summary>
    /// 
    /// </summary>
    private const byte DEF_PLAIN_OPTIONS    = DEF_COMPRESSED_MASK;
    /// <summary>
    /// 
    /// </summary>
    private const byte DEF_RTF_OPTIONS      = DEF_COMPRESSED_MASK + DEF_RICHTEXT_MASK;

    /// <summary>
    /// Possible string flags.
    /// </summary>
    [ Flags ]
    public enum StringType : byte
    {
      NonUnicode = 0,
      /// <summary>
      /// The string is saved as double-byte characters.
      /// </summary>
      Unicode = 1,
      /// <summary>
      /// Extended string follows (Far East versions).
      /// </summary>
      FarEast = 4,
      /// <summary>
      /// Rich string follows.
      /// </summary>
      RichText = 8,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Formatting runs, key - position, value - font index.
    /// </summary>
    private SortedList<int, int> m_arrFormattingRuns;// = new SortedList<int, int>();
    /// <summary>
    /// String value.
    /// </summary>
    private string m_strValue = string.Empty;
    /// <summary>
    /// Default font index.
    /// </summary>
    private int m_iDefaultIndex;
    /// <summary>
    /// Options.
    /// </summary>
    private StringType m_options = StringType.Unicode;
    /// <summary>
    /// Indicates whether string was changed and possibly needs defragmentation.
    /// </summary>
    private bool m_bNeedDefragment = true;
    /// <summary>
    /// Number of references to this object.
    /// </summary>
    public int RefCount;
    /// <summary>
    /// Reprsents the Rich Text.
    /// </summary>
    private string m_rtfText;
    /// <summary>
    /// Indicates the string is preserverd type or not
    /// </summary>
    private bool m_isPreserved = false;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public TextWithFormat()
    {
    }
    /// <summary>
    /// Creates instance with specified default font index.
    /// </summary>
    /// <param name="fontIndex">Index of the default font.</param>
    public TextWithFormat( int fontIndex )
      : this()
    {
      m_iDefaultIndex = fontIndex;
    }

    /// <summary>
    /// Converts TextWithFormat to string.
    /// </summary>
    /// <param name="format">Object to convert to string.</param>
    /// <returns>String value of the object.</returns>
    public static implicit operator string( TextWithFormat format )
    {
      return format.Text;
    }
    /// <summary>
    /// Converts string to TextWithFormat.
    /// </summary>
    /// <param name="value">String value.</param>
    /// <returns>Converted TextWithFormat.</returns>
    public static explicit operator TextWithFormat( string value )
    {
      TextWithFormat result = new TextWithFormat();
      result.Text = value;

      return result;
    }
    #endregion
    
    #region Class properties
    /// <summary>
    ///  Reprsents the Rich Text
    /// </summary>
    internal string RtfText
    {
        get
        {
            return this.m_rtfText;
        }
        set
        {
            this.m_rtfText = value;
        }
    }

    /// <summary>
    /// Text string.
    /// </summary>
    public string     Text
    {
      get
      {
        return m_strValue;
      }
      set
      {
        // TODO: not only string value should be changed
        m_strValue = value;
      }
    }
    /// <summary>
    /// Formatting runs, key - position, value - font index.
    /// </summary>
    public SortedList<int, int> FormattingRuns
    {
      get
      {
        if( m_arrFormattingRuns == null )
          m_arrFormattingRuns = new SortedList<int, int>();

        return m_arrFormattingRuns;
      }
    }
    /// <summary>
    /// List of formatting runs. Read-only.
    /// </summary>
    internal SortedList<int, int> InnerFormattingRuns
    {
      get
      {
        return m_arrFormattingRuns;
      }
    }
    /// <summary>
    /// Gets / sets default font index.
    /// </summary>
    public int        DefaultFontIndex
    {
      get
      {
        return m_iDefaultIndex;
      }
      set
      {
//        if( value < 0 || value > m_book.InnerFonts.Length )
//          throw new ArgumentOutOfRangeException( "DefaultFontIndex" );

        m_iDefaultIndex = value;
      }
    }
    /// <summary>
    /// Returns number of formatting runs. Read-only.
    /// </summary>
    public int        FormattingRunsCount
    {
      get
      {
        if( m_arrFormattingRuns == null ) return 0;

        return m_arrFormattingRuns.Count;
      }
    }
    /// <summary>
    /// Returns true if string is preserved type.
    /// </summary>
    public bool IsPreserved
    {
        get
        {
            return m_isPreserved;
        }
        set
        {
            m_isPreserved = value;
        }

    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets font index for specified range of characters.
    /// </summary>
    /// <param name="iStartPos">Start character of the range.</param>
    /// <param name="iEndPos">End character of the range.</param>
    /// <param name="iFontIndex">Font index to set.</param>
    public void SetTextFontIndex( int iStartPos, int iEndPos, int iFontIndex )
    {
      m_bNeedDefragment = true;

      CreateFormattingRuns();
      
      if( iStartPos < 0 || iStartPos > m_strValue.Length )
        throw new ArgumentOutOfRangeException( "iStartPos" );

      if( iEndPos < 0 || iEndPos > m_strValue.Length )
        throw new ArgumentOutOfRangeException( "iEndPos" );

      if( iStartPos > iEndPos )
        throw new ArgumentException( "iStartPos cannot be larger than iEndPos." );

      int iBeforeStartPos = GetPreviousPosition( iStartPos );
      int iBeforeEndPos = GetPreviousPosition( iEndPos );

      int iBeforeStartFont = ( iBeforeStartPos >= 0 )
        ? m_arrFormattingRuns[ iBeforeStartPos ]
        : m_iDefaultIndex;

      int iBeforeEndFont = ( iBeforeEndPos >= 0 )
        ? m_arrFormattingRuns[ iBeforeEndPos ]
        : m_iDefaultIndex;

      RemoveAllInsideRange( iBeforeStartPos, iBeforeEndPos );

//      if( iBeforeStartPos > 0 )
//      {
//        m_arrFormattingRuns[ iBeforeStartPos ] = iBeforeStartFont;
//      }

      m_arrFormattingRuns[ iStartPos ] = iFontIndex;

      if( iEndPos < m_strValue.Length - 1 && !m_arrFormattingRuns.ContainsKey( iEndPos + 1 ) )
      {
        m_arrFormattingRuns[ iEndPos + 1 ] = iBeforeEndFont;
      }
    }
    /// <summary>
    /// Returns font index for the specified character.
    /// </summary>
    /// <param name="iPos">Character index to get font index.</param>
    /// <returns>Font index for the specified character.</returns>
    public int GetTextFontIndex( int iPos )
    {
      if( m_arrFormattingRuns == null ) return m_iDefaultIndex;

      int iFontIndex = m_iDefaultIndex;

      int iCurPos = GetPreviousPosition(iPos);
      
      if( iCurPos >= 0 )
      {
        iFontIndex = m_arrFormattingRuns[ iCurPos ];
      }

      // TODO: create font wrapper and return it.
      //return null;
      return iFontIndex;
    }
    /// <summary>
    /// Returns font index for the specified character.
    /// </summary>
    /// <param name="iPos">Character index to get font index.</param>
    /// <returns>Font index for the specified character.</returns>
    public int GetTextFontIndex(int iPos, bool iscopy)
    {
        if (m_arrFormattingRuns == null) return m_iDefaultIndex;

        int iFontIndex = m_iDefaultIndex;

        int iCurPos = GetPositionByIndex(iPos);

        if (iCurPos >= 0)
        {
            iFontIndex = m_arrFormattingRuns[iCurPos];
        }

        // TODO: create font wrapper and return it.
        //return null;
        return iFontIndex;
    }
    /// <summary>
    /// Returns font index at the specified position in the formatting runs array.
    /// </summary>
    /// <param name="iIndex">Index of the formatting run.</param>
    /// <returns>Font index.</returns>
    public int GetFontByIndex( int iIndex )
    {
      if( m_arrFormattingRuns == null )
        throw new ArgumentOutOfRangeException( "iIndex" );

      CheckOffset( m_arrFormattingRuns.Count, iIndex );

      return m_arrFormattingRuns.Values[ iIndex ];//GetByIndex( iIndex );
    }
    /// <summary>
    /// Returns character position at the specified position in the formatting runs array.
    /// </summary>
    /// <param name="iIndex">Index of the formatting run.</param>
    /// <returns>Character position.</returns>
    public int GetPositionByIndex( int iIndex )
    {
      if( m_arrFormattingRuns.Count <= iIndex || iIndex < 0 )
        throw new ArgumentOutOfRangeException( "iIndex" );

      return m_arrFormattingRuns.Keys[ iIndex ];//GetKey( iIndex );
    }
    /// <summary>
    /// Sets font index at the specified position in the formatting runs array.
    /// </summary>
    /// <param name="index">Index of the formatting run.</param>
    /// <param name="iFontIndex">Font index to set.</param>
    public void SetFontByIndex( int index, int iFontIndex )
    {
      CreateFormattingRuns();

        IList<int> arrkeys = m_arrFormattingRuns.Keys;

      //m_arrFormattingRuns.SetByIndex( index, iFontIndex );
        int iKey = arrkeys[index];
      m_arrFormattingRuns[ iKey ] = iFontIndex;
    }
    /// <summary>
    /// Clears formatting.
    /// </summary>
    public void ClearFormatting()
    {
      if( m_arrFormattingRuns != null ) m_arrFormattingRuns.Clear();
    }
    #endregion

    #region IComparable Members
    /// <summary>
    /// Compares the current instance with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the comparands.
    /// </returns>
    public int CompareTo( object obj )
    {
      int result = 0;

      TextWithFormat text = obj as TextWithFormat;

      if( text != null )
      {
        result = String.CompareOrdinal( text.m_strValue, m_strValue );
        
        if( result == 0 )
        {
          if( FormattingRunsCount == 0 && text.FormattingRunsCount == 0 ) return 0;

          this.Defragment();
          text.Defragment();

          return CompareFormattingRuns( m_arrFormattingRuns, text.m_arrFormattingRuns );
        }
      }

      return result;
    }

    /// <summary>
    /// Compares formatting runs.
    /// </summary>
    /// <param name="fRuns1">First formatting runs to compare.</param>
    /// <param name="fRuns2">Second formatting runs to compare.</param>
    /// <returns>
    /// 0 if they are equal,
    /// -1 if first formatting run is less then second;
    /// otherwise 1.
    /// </returns>
    public static int CompareFormattingRuns( SortedList<int, int> fRuns1, SortedList<int, int> fRuns2 )
    {
      if( fRuns1 == null && fRuns2 == null ) return 0;

      if( fRuns1 == null ) return -1;

      if( fRuns2 == null ) return 1;

      int minLen = Math.Min( fRuns1.Count, fRuns2.Count );
      IList<int> lstKeys1 = fRuns1.Keys;
      IList<int> lstKeys2 = fRuns2.Keys;
      IList<int> lstValues1 = fRuns1.Values;
      IList<int> lstValues2 = fRuns2.Values;

      for( int i = 0; i < minLen; i++ )
      {
        int result = lstKeys1[ i ] - lstKeys2[ i ];

        if( result != 0 ) return result;

        result = lstValues1[ i ] - lstValues2[ i ];

        if( result != 0 ) return result;
      }

      return fRuns1.Count - fRuns2.Count;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    private void CreateFormattingRuns()
    {
      if( m_arrFormattingRuns == null )
        m_arrFormattingRuns = new SortedList<int, int>();
    }
    /// <summary>
    /// Returns starting position for formatting block which contains specified position.
    /// </summary>
    /// <param name="iPos">Position to which formatting is applied.</param>
    /// <returns>Starting position for formatting block which contains specified position.</returns>
    private int   GetPreviousPosition( int iPos )
    {
      int iStartPos = 0;
      int iEndPos = FormattingRunsCount - 1;
      int iMiddle;
      int iCurPos;

      if( iEndPos < 0 )
        return -1;

      IList<int> arrKeys = m_arrFormattingRuns.Keys;
      
      while( true )
      {
        iMiddle = ( iStartPos + iEndPos ) / 2;

        iCurPos = arrKeys[ iMiddle ];

        if( iStartPos >= iEndPos - 1 )
        {
          int iCurEndPos = arrKeys[ iEndPos ];

          if( iCurEndPos <= iPos ) return iCurEndPos;
          if( iCurPos <= iPos )    return iCurPos;
          
          return -1;
        }

        if( iCurPos == iPos )
        {
          return iCurPos;
        }
        else if( iCurPos < iPos )
        {
          iStartPos = Math.Min( iEndPos, iMiddle );
        }
        else
        {
          iEndPos = Math.Max( iStartPos, iMiddle );
        }
      }
    }

    /// <summary>
    /// Removes all formatting within specified range.
    /// </summary>
    /// <param name="iStartPos">Start position of the range.</param>
    /// <param name="iEndPos">End position of specified range.</param>
    internal void  RemoveAllInsideRange( int iStartPos, int iEndPos )
    {
      int iCount = m_arrFormattingRuns.Count;

      if( iCount == 0 ) return;

      IList< int > lstKeys = m_arrFormattingRuns.Keys;
      int[] arrKeys = new int[ iCount ];
      lstKeys.CopyTo( arrKeys, 0 );
      
      int startKeyPos = ( iStartPos == -1 ) ? 0 : Array.BinarySearch( arrKeys, iStartPos );
      int endKeyPos = ( iEndPos == -1 ) ? arrKeys.Length - 1 : Array.BinarySearch( arrKeys, iEndPos );

      int iPos = lstKeys[ startKeyPos ];
      if( iPos == iStartPos ) startKeyPos++;

      for( int i = startKeyPos; i <= endKeyPos; i++ )
      {
        m_arrFormattingRuns.RemoveAt( startKeyPos );
      }
    }
    /// <summary>
    /// Defragments text formatting.
    /// </summary>
    public void   Defragment()
    {
      if( m_arrFormattingRuns == null ) m_bNeedDefragment = false;

      if( !m_bNeedDefragment ) return;

      int len = m_arrFormattingRuns.Count;
      IList<int> lstValues = m_arrFormattingRuns.Values;
      int i = 0;

      while( i < len - 1 )
      {
        if( lstValues[ i ] == lstValues[ i + 1 ] )
        {
          m_arrFormattingRuns.RemoveAt( i + 1 );
          len--;
          continue;
        }

        i++;
      }

      m_bNeedDefragment = false;
    }
    /// <summary>
    /// Checks if specified offset is correct.
    /// </summary>
    /// <param name="len">Length of the array.</param>
    /// <param name="iOffset">Offset in the array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When iOffset is out of range.
    /// </exception>
    public static void CheckOffset( int len, int iOffset )
    {
      if( iOffset < 0 || iOffset > len )
        throw new ArgumentOutOfRangeException( "iOffset" );
    }
    /// <summary>
    /// Copies formatting runs into another TextWithFormat object.
    /// </summary>
    /// <param name="twin">TextWithFormat to copy data into.</param>
    public void CopyFormattingTo( TextWithFormat twin )
    {
      if( twin == null )
        throw new ArgumentNullException( "twin" );

      if( m_arrFormattingRuns == null || m_arrFormattingRuns.Count == 0 )
      {
        twin.m_arrFormattingRuns = null;
      }
      else
      {
        twin.m_arrFormattingRuns = new SortedList<int, int>();
        IList<int> lstKeys = m_arrFormattingRuns.Keys;
        IList<int> lstValues = m_arrFormattingRuns.Values;

        for( int i = 0, len = m_arrFormattingRuns.Count; i < len; i++ )
        {
          twin.m_arrFormattingRuns.Add( lstKeys[ i ], lstValues[ i ] );
        }
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Converts this object to string.
    /// </summary>
    /// <returns>String representation of this object.</returns>
    public override string ToString()
    {
      StringBuilder builder = new StringBuilder( m_strValue + Environment.NewLine );
      builder.Append( "str[ 0 ] ... - " + m_iDefaultIndex );

      if( m_arrFormattingRuns != null )
      {
        IList<int> lstKeys = m_arrFormattingRuns.Keys;

        for( int i = 0, len = m_arrFormattingRuns.Count; i < len; i++ )
        {
          int key = lstKeys[ i ];
          // TODO: maybe we should use Values instead of m_arrFormattingRuns[ key ]?
          builder.AppendFormat( "\nstr[ {0} ] ... - {1}", key, m_arrFormattingRuns[ key ] );
        }
      }

      return builder.ToString();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// True if the specified object is equal to the current object;
    /// otherwise False.
    /// </returns>
    public override bool Equals(object obj)
    {
      if( !( obj is TextWithFormat ) ) return false;

      return CompareTo( obj ) == 0;
    }

    /// <summary>
    /// Returns hashcode for the object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      return m_strValue.GetHashCode();
    }


    #endregion

    #region Class Parse/Serialize methods
    /// <summary>
    /// Parses byte array.
    /// </summary>
    /// <param name="data">Array of bytes to parse.</param>
    /// <param name="iOffset">Offset of the object's data in the array.</param>
    /// <returns>Size of the object in the data array.</returns>
    public virtual int Parse( byte[] data, int iOffset )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      int len = data.Length;

      CheckOffset( len, iOffset );

      if( iOffset < 0 || iOffset > data.Length )
        throw new ArgumentOutOfRangeException( "iOffset" );

      int iCurOffset = iOffset;

      CheckOffset( len, iCurOffset + 2 );
      ushort usCharCount = BitConverter.ToUInt16( data, iCurOffset );
      iCurOffset += 2;
      
      CheckOffset( len, iCurOffset + 1 );
      m_options = ( StringType )data[ iCurOffset ];
      iCurOffset++;

      bool bIsUnicode = ( m_options & StringType.Unicode ) != 0;
      bool bFarEast = ( m_options & StringType.FarEast ) != 0;
      bool bIsRTF = ( m_options & StringType.RichText ) != 0;
      int iFormattingRunsCount = 0;
      int iFarEastDataLen = 0;

      if( bIsRTF )
      {
        CheckOffset( len, iCurOffset + 2 );

        iFormattingRunsCount = BitConverter.ToUInt16( data, iCurOffset );
        iCurOffset += 2;
      }

      if( bFarEast )
      {
        CheckOffset( len, iCurOffset + 4 );
        iFarEastDataLen = BitConverter.ToInt32( data, iCurOffset );
        iCurOffset += 4;
      }

      Text = GetText( data, usCharCount, bIsUnicode, ref iCurOffset );

      if( iFormattingRunsCount > 0 )
        ParseFormattingRuns( data, iCurOffset, iFormattingRunsCount );

      if( iFarEastDataLen > 0 )
      {
        ParseFarEastData( data, iCurOffset, iFarEastDataLen );
      }

      return iCurOffset - iOffset;
    }
    
    /// <summary>
    /// Evaluates size of the text in bytes.
    /// </summary>
    /// <returns>Returns text size in bytes.</returns>
    public int GetTextSize()
    {
      int iCount = Encoding.Unicode.GetByteCount( m_strValue );
      return iCount + 3;
    }
    /// <summary>
    /// Returns size of the formatting runs.
    /// </summary>
    /// <returns>Size of the formatting runs.</returns>
    public int GetFormattingSize()
    {
      Defragment();
      int iCount = FormattingRunsCount;

      if( iCount > 0 )
      {
        m_options |= StringType.RichText;
      }

      return iCount * DEF_FR_SIZE;
    }
    /// <summary>
    /// Serializes formatting.
    /// </summary>
    /// <returns>Returns array of bytes that contains formatting data.</returns>
    public byte[] SerializeFormatting()
    {
      byte[] result = SerializeFormattingRuns();

      if( result != null && result.Length > 0 ) m_options |= StringType.RichText;

      return result;
    }

    /// <summary>
    /// Serializes formatting.
    /// </summary>
    /// <param name="arrBuffer">Buffer for formatting data.</param>
    /// <param name="iOffset">Offset in the buffer where to serialize formatting.</param>
    /// <param name="bDefragment">Indicates whether defragmentation is needed.</param>
    /// <returns>Size of formatting data.</returns>
    public int SerializeFormatting( byte[] arrBuffer, int iOffset, bool bDefragment )
    {
      int iResult = SerializeFormattingRuns( arrBuffer, iOffset, bDefragment );

      if( iResult  > 0 ) m_options |= StringType.RichText;

      return iResult;
    }

    /// <summary>
    /// Returns string options.
    /// </summary>
    /// <returns>Options byte.</returns>
    public StringType GetOptions()
    {
      return m_options;
    }
    /// <summary>
    /// Converts byte array to string.
    /// </summary>
    /// <param name="data">Byte array with string data.</param>
    /// <param name="usChartCount">Desired number of charts.</param>
    /// <param name="bIsUnicode">Indicates whether string is in unicode format.</param>
    /// <param name="iOffset">Offset of the string data.</param>
    /// <returns>Extracted string.</returns>
    private string GetText( byte[] data, ushort usChartCount, bool bIsUnicode, ref int iOffset )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      string result;
      int size;

      if( bIsUnicode )
      {
        size = usChartCount * 2;
        CheckOffset( data.Length, iOffset + size );
        result = Encoding.Unicode.GetString( data, iOffset, size );
      }
      else
      {
        size = usChartCount;
        CheckOffset( data.Length, iOffset + size );
        result = BiffRecordRaw.LatinEncoding.GetString( data, iOffset, size );
      }

      iOffset += size;

      return result;
    }
    /// <summary>
    /// Parses formatting runs.
    /// </summary>
    /// <param name="data">Array with formatting runs data.</param>
    /// <param name="iOffset">Offset to formatting runs.</param>
    /// <param name="iFRCount">Number of formatting runs to parse.</param>
    private void ParseFormattingRuns( byte[] data, int iOffset, int iFRCount )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( iOffset >= data.Length && iFRCount > 0 )
        throw new ArgumentOutOfRangeException( "iOffset" );

      CreateFormattingRuns();

      int len = data.Length;

      for( int i = 0; i < iFRCount; i++, iOffset += 4 )
      {
        CheckOffset( len, iOffset + 4 );
        int iChar = BitConverter.ToUInt16( data, iOffset );

        int iFontIndex = BitConverter.ToUInt16( data, iOffset + 2 );
        //iFontIndex = FontImpl.NormalizeFontIndex( iFontIndex );
        
        //m_arrFormattingRuns.Add( iChar, iFontIndex );
        m_arrFormattingRuns[ iChar ] = iFontIndex;
      }
    }
    /// <summary>
    /// Parses formatting runs.
    /// </summary>
    /// <param name="data">Array with formatting runs.</param>
    internal void ParseFormattingRuns( byte[] data )
    {
      if( data == null || data.Length == 0 )
        return;

      ParseFormattingRuns( data, 0, data.Length / DEF_FR_SIZE );
    }

    /// <summary>
    /// Parses Far East data.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iOffset"></param>
    /// <param name="iFarEastDataLen"></param>
    private void ParseFarEastData( byte[] data, int iOffset, int iFarEastDataLen )
    {
    }
    /// <summary>
    /// Serializes formatting runs.
    /// </summary>
    /// <returns>Array with formatting runs.</returns>
    private byte[] SerializeFormattingRuns()
    {
      Defragment();
      int iCount = FormattingRunsCount;//m_arrFormattingRuns.Count;

      if( iCount == 0 ) return null;

      byte[] arrResult = new byte[ iCount * DEF_FR_SIZE ];
      SerializeFormattingRuns( arrResult, 0, true );

      return arrResult;
    }

    /// <summary>
    /// Serializes formatting runs into specified array.
    /// </summary>
    /// <param name="arrDestination">Destination array.</param>
    /// <param name="iOffset">Offset to the data.</param>
    /// <param name="bDefragment">Indicates whether defragmentation is needed.</param>
    /// <returns>Size of the serialized data.</returns>
    private int SerializeFormattingRuns( byte[] arrDestination, int iOffset, bool bDefragment )
    {
      if( bDefragment ) Defragment();

      int iCount = FormattingRunsCount;

      if( iCount == 0 ) return 0;

      if( arrDestination == null )
        throw new ArgumentNullException( "arrDestination" );

      int iSize = iCount * DEF_FR_SIZE;
      byte[] buf;

      if( iOffset < 0 || iOffset + iSize > arrDestination.Length )
        throw new ArgumentOutOfRangeException( "iOffset" );

      IList<int> lstKeys = m_arrFormattingRuns.Keys;
      IList<int> lstValues = m_arrFormattingRuns.Values;

      for( int i = 0; i < iCount; i++, iOffset += DEF_FR_SIZE )
      {
        buf = BitConverter.GetBytes( ( ushort )lstKeys[ i ] );
        arrDestination[ iOffset ] = buf[ 0 ];
        arrDestination[ iOffset + 1 ] = buf[ 1 ];

        int iFontIndex = lstValues[ i ];

        buf = BitConverter.GetBytes( ( ushort )iFontIndex );
        arrDestination[ iOffset + 2 ] = buf[ 0 ];
        arrDestination[ iOffset + 3 ] = buf[ 1 ];
      }

      return iSize;
    }
    #endregion

    #region IClonable methods
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
      return TypedClone();
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public TextWithFormat TypedClone()
    {
      TextWithFormat result = this.MemberwiseClone() as TextWithFormat;

      if( m_arrFormattingRuns != null )
        CopyFormattingTo( result );

      return result;
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public TextWithFormat Clone( Dictionary<int, int> dicFontIndexes )
    {
      TextWithFormat result = TypedClone();

      if( dicFontIndexes != null )
        result.UpdateFontIndexes( dicFontIndexes );

      return result;
    }
    /// <summary>
    /// Updates font indexes.
    /// </summary>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    private void UpdateFontIndexes( Dictionary<int, int> dicFontIndexes )
    {
      if( dicFontIndexes == null )
        throw new ArgumentNullException( "arrFontIndexes" );

      int iCount = FormattingRunsCount;

      if( iCount > 0 )
      {
        IList<int> lstValues = m_arrFormattingRuns.Values;
        IList<int> lstKeys = m_arrFormattingRuns.Keys;

        for( int i = 0, len = FormattingRunsCount; i < len; i++ )
        {
          int iKey = lstKeys[ i ];
          int iFontIndex = lstValues[ i ];
          iFontIndex = FontImpl.UpdateFontIndexes( iFontIndex, dicFontIndexes, ExcelParseOptions.Default );
          m_arrFormattingRuns[ iKey ] = iFontIndex;
        }
      }
    }
    #endregion

    internal void ReplaceFont( int oldFontIndex, int newFontIndex )
    {
      SortedList<int, int> result = new SortedList<int, int>();

      foreach( KeyValuePair<int, int> pair in m_arrFormattingRuns )
      {
        int value = pair.Value;

        if( value == oldFontIndex )
        {
          value = newFontIndex;
        }

        result.Add( pair.Key, value );
      }

      m_arrFormattingRuns = result;
    }

    internal void RemoveAtStart( int length )
    {
      int iPrevPos = GetPreviousPosition( length );

      if( iPrevPos >= 0 )
      {
        int iPrevFont = m_arrFormattingRuns[ iPrevPos ];
        int index = m_arrFormattingRuns.IndexOfKey( iPrevPos );

        for( int i = index; i >= 0; i-- )
        {
          m_arrFormattingRuns.RemoveAt( i );
        }

        m_arrFormattingRuns[ length ] = iPrevFont;
        SortedList<int, int> newFormattings = new SortedList<int, int>();

        foreach( KeyValuePair<int, int> pair in m_arrFormattingRuns )
        {
          newFormattings[ pair.Key - length ] = pair.Value;
        }

        m_arrFormattingRuns = newFormattings;
      }

      Text = Text.Substring( length );
    }


    internal void RemoveAtEnd( int length )
    {
      int charLeft = Text.Length - length;
      int iLastPos = charLeft - 1;
      int iPrevPos = GetPreviousPosition( iLastPos );

      if( iPrevPos >= 0 )
      {
        int iPrevFont = m_arrFormattingRuns[ iPrevPos ];
        int index = m_arrFormattingRuns.IndexOfKey( iPrevPos ) + 1;

        for( int i = m_arrFormattingRuns.Count - 1; i >= index; i-- )
        {
          m_arrFormattingRuns.RemoveAt( i );
        }
      }
      else
      {
        ClearFormatting();
      }

      Text = Text.Substring( 0, charLeft );
    }
  }
}
