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

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
  public class LbsDropData
  {
    #region Members
    /// <summary>
    /// Option flags.
    /// </summary>
    private short m_sOptions;
    /// <summary>
    /// An unsigned integer that specifies the number of lines to be displayed
    /// in the dropdown. If there are more lines than that in the list,
    /// a scrollbar can appear.
    /// </summary>
    private short m_sLinesNumber;
    /// <summary>
    /// An unsigned integer that specifies the smallest width in pixels allowed for the dropdown window.
    /// </summary>
    private short m_sMinimum;
    /// <summary>
    /// Current string value in the dropdown.
    /// </summary>
    private string m_strValue;
    #endregion

    #region Properites
    /// <summary>
    /// Option flags.
    /// </summary>
    public short Options
    {
      get
      {
        return m_sOptions;
      }
      set
      {
        m_sOptions = value;
      }
    }
    /// <summary>
    /// An unsigned integer that specifies the number of lines to be displayed
    /// in the dropdown. If there are more lines than that in the list,
    /// a scrollbar can appear.
    /// </summary>
    public short LinesNumber
    {
      get
      {
        return m_sLinesNumber;
      }
      set
      {
        m_sLinesNumber = value;
      }
    }
    /// <summary>
    /// An unsigned integer that specifies the smallest width in pixels allowed for the dropdown window.
    /// </summary>
    public short Minimum
    {
      get
      {
        return m_sMinimum;
      }
      set
      {
        m_sMinimum = value;
      }
    }
    /// <summary>
    /// Current string value in the dropdown.
    /// </summary>
    public string Value
    {
      get
      {
        return m_strValue;
      }
      set
      {
        m_strValue = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Serializes object inside specified data provider.
    /// </summary>
    /// <param name="provider">Data provider to serialize into.</param>
    /// <param name="offset">Offset to start serialization from.</param>
    public void Serialize( DataProvider provider, int offset )
    {
      int iStartOffset = offset;
      provider.WriteInt16( offset, m_sOptions );
      offset += ExcelConstants.ShortSize;

      provider.WriteInt16( offset, m_sLinesNumber );
      offset += ExcelConstants.ShortSize;

      provider.WriteInt16( offset, m_sMinimum );
      offset += ExcelConstants.ShortSize;

      provider.WriteString16BitUpdateOffset( ref offset, m_strValue );
      int iLength = offset - iStartOffset;

      if( iLength % 2 != 0 )
        provider.WriteByte( offset, 10 );
    }
    /// <summary>
    /// Extracts object from specified data provider.
    /// </summary>
    /// <param name="provider">Data provider to get data from.</param>
    /// <param name="offset">Offset to start getting data from.</param>
    public int Parse( DataProvider provider, int offset )
    {
      int iStartOffset = offset;
      m_sOptions = provider.ReadInt16( offset );
      offset += ExcelConstants.ShortSize;

      m_sLinesNumber = provider.ReadInt16( offset );
      offset += ExcelConstants.ShortSize;

      if( provider.Capacity > offset + 2 )
      {
        m_sMinimum = provider.ReadInt16( offset );
        offset += ExcelConstants.ShortSize;

        if( provider.Capacity > offset + 2 )
          m_strValue = provider.ReadString16BitUpdateOffset( ref offset );
      }

      int iLength = offset - iStartOffset;

      if( iLength % 2 != 0 )
        offset++;
      //  provider.WriteByte( offset, 10 );

      return offset;
    }
    /// <summary>
    /// Gets 
    /// </summary>
    /// <returns></returns>
    public int GetStoreSize()
    {
      int iResult = 0;

      if( m_strValue != null )
      {
        iResult += m_strValue.Length * 2;
        iResult += 3; // string header
      }

      iResult += ExcelConstants.ShortSize * 3; // options, lines number and minimum

      if( iResult % 2 != 0 )
        iResult++;

      return iResult;
    }
    /// <summary>
    /// Creates a copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public LbsDropData Clone()
    {
      return ( LbsDropData )MemberwiseClone();
    }
    #endregion
  }
}
