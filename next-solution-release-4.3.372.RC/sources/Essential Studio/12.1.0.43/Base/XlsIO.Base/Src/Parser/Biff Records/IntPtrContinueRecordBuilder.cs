#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for IntPtrContinueRecordBuilder.
	/// </summary>
	[ CLSCompliant( false ) ]
	public class IntPtrContinueRecordBuilder : IDisposable
	{
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected BiffRecordWithContinue m_parent;
    /// <summary>
    /// 
    /// </summary>
    protected int m_iPos;
    /// <summary>
    /// 
    /// </summary>
    private int m_iContinuePos = -1;
    /// <summary>
    /// 
    /// </summary>
    private int m_iContinueSize;
    /// <summary>
    /// 
    /// </summary>
    private int m_iTotal;
    /// <summary>
    /// 
    /// </summary>
    protected int m_iMax;
//    /// <summary>
//    /// 
//    /// </summary>
//    private TBIFFRecord m_firstContinueType = TBIFFRecord.Continue;
    /// <summary>
    /// Type of additional records.
    /// </summary>
    private TBIFFRecord m_continueType = TBIFFRecord.Continue;
    /// <summary>
    /// 
    /// </summary>
    private int m_iContinueCount;
    /// <summary>
    /// Size of the first record.
    /// </summary>
    private int m_iFirstRecordLength = -1;
    /// <summary>
    /// Size of the header for continue records.
    /// </summary>
    private int m_iContinueHeaderSize = 0;
    #endregion

    #region Class Properties
    /// <summary>
    /// Returns the unused bytes.
    /// </summary>
    public int FreeSpace
    {
      get
      {
        return m_iMax - m_iContinueSize;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Total
    {
      get
      {
        return m_iTotal;
      }
      set
      {
        m_iTotal = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Position
    {
      get
      {
        return m_iPos;
      }
      set
      {
        m_iPos = value;
      }
    }
    /// <summary>
    /// Offset from the current record (or continue record) start. Read-only.
    /// </summary>
    public int Offset
    {
      get
      {
        return ( m_iContinuePos > 0 ) ?
          m_iPos - m_iContinuePos - m_iContinueHeaderSize :
          m_iPos;
      }
    }
    /// <summary>
    /// Return maximum size of record.
    /// </summary>
    public int Max
    {
      get
      {
        return m_iMax;
      }
    }
    /// <summary>
    /// Type of the first additional record.
    /// </summary>
    public TBIFFRecord FirstContinueType
    {
      get
      {
        return m_parent.FirstContinueType;
      }
    }
    /// <summary>
    /// Type of additional records.
    /// </summary>
    public TBIFFRecord ContinueType
    {
      get
      {
        return m_continueType;
      }
      set
      {
        m_continueType = value;
      }
    }
    /// <summary>
    /// Maximum size of the continue record data.
    /// </summary>
    public virtual int MaximumSize
    {
      get
      {
        return BiffRecordRaw.DEF_RECORD_MAX_SIZE - m_iContinueHeaderSize;
      }
    }
    /// <summary>
    /// Size of the first record.
    /// </summary>
    public int FirstRecordLength
    {
      get
      {
        return m_iFirstRecordLength;
      }
    }
    #endregion

    #region Class Events
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler OnFirstContinue;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the continue record builder.
    /// </summary>
    /// <param name="parent">Parent record.</param>
    /// <param name="continueHeaderSize">Size of the continue record header.</param>
    public IntPtrContinueRecordBuilder( BiffRecordWithContinue parent, int continueHeaderSize )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_parent = parent;
      m_iMax = m_parent.MaximumRecordSize;
      m_iContinueHeaderSize = continueHeaderSize;

      //m_iContinuePos = 2;

      m_iContinueSize = m_parent.Length;
      m_iPos = m_iContinueSize; // Set offset position in data array.
      m_iTotal = m_iContinueSize;
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void AppendByte( byte value )
    {
      if( CheckIfSpaceNeeded( 1 ) )
      {
        UpdateContinueRecordSize();
        StartContinueRecord();
      }

      m_parent.SetByte( m_iPos, value );
      UpdateCounters( 1 );

      //UpdateContinueRecordSize();
    }

    /// <summary>
    /// Write array of data into output stream.
    /// </summary>
    /// <param name="data">Array of data.</param>
    /// <param name="start">Start index of an array.</param>
    /// <param name="length">Length of data to copy.</param>
    /// <returns>Quantity of created Continue Records.</returns>
    public virtual int AppendBytes( byte[] data, int start, int length )
    {
      int counter = 0;

      // If data array is too large, then save it by parts.
      if( CheckIfSpaceNeeded( length ) )
      {
        int endPoint = start + length;
        int i = start;

        for( ; i < endPoint; i += m_iMax )
        {
          UpdateContinueRecordSize();
          StartContinueRecord();
          counter++;         

          int iLen = ( endPoint - i < m_iMax ) ? endPoint - i : m_iMax;
          m_parent.SetBytes( m_iPos, data, i, iLen );
          UpdateCounters( iLen );
        }
      }
      else
      {
        m_parent.SetBytes( m_iPos, data, start, length );
        UpdateCounters( length );
      }

      UpdateContinueRecordSize();

      return counter;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void AppendUInt16( ushort value )
    {
      if( CheckIfSpaceNeeded( 2 ) )
      {
        UpdateContinueRecordSize();
        StartContinueRecord();
      }

      m_parent.SetUInt16( m_iPos, value );
      UpdateCounters( 2 );

      //UpdateContinueRecordSize();
    }

    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Method that checks if Continue Record is needed.
    /// </summary>
    /// <param name="length">Length of data that needs to be stored.</param>
    /// <returns>True if Continue Record will be needed for data storage;
    /// otherwise False.</returns>
    public bool CheckIfSpaceNeeded( int length )
    {
      return ( m_iContinueSize + length > m_iMax );
    }
    /// <summary>
    /// 
    /// </summary>
    public void StartContinueRecord()
    {
      if( OnFirstContinue != null )
      {
        OnFirstContinue( this, EventArgs.Empty );
      }

      if( m_iContinueCount == 0 )
      {
        m_iFirstRecordLength = m_iPos;
      }

      m_iContinueCount++;

      TBIFFRecord recordType = ( m_iContinueCount == 1 )
        ? FirstContinueType
        : ContinueType;

      m_parent.m_arrContinuePos.Add( m_iPos );
      m_parent.SetUInt16( m_iPos, ( ushort )recordType );

      m_iPos += 2;
      m_iContinuePos = m_iPos;
      m_iContinueSize = 0;

      m_parent.SetUInt16( m_iPos, ( ushort )m_iContinueSize );

      m_iPos += 2;
      m_iTotal += 4;

      // Set maximum to continue record max Size.
      m_iMax = MaximumSize;
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateContinueRecordSize()
    {
      if( m_iContinuePos >= 0 )
      {
        m_parent.SetUInt16( m_iContinuePos, ( ushort )m_iContinueSize );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iLen"></param>
    protected void UpdateCounters( int iLen )
    {
      m_iPos += iLen;
      m_iTotal += iLen;
      m_iContinueSize += iLen;
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if( m_parent != null )
      {
        m_parent = null;
        GC.SuppressFinalize( this );
      }
    }

    #endregion
  }
}
