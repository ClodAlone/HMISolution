#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !DEBUG

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for _BiffRecordRawWithStreamPos.
  /// </summary>
  [ CLSCompliant( false ) ]
  public class BiffRecordWithStreamPos : BiffRecordRaw
  {
    #region Class members
    /// <summary>
    /// Position of the Biff record in the stream.
    /// </summary>
    protected long    m_lStreamPosition;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor, gets code value using reflection and attributes.
    /// </summary>
    protected  BiffRecordWithStreamPos()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    protected  BiffRecordWithStreamPos( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="reader">BinaryReader from which record data should be read.</param>
    /// <param name="itemSize">Size of the read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified reader is NULL.
    /// </exception>
    protected  BiffRecordWithStreamPos( BinaryReader reader, out int itemSize )
      : base( reader, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    protected  BiffRecordWithStreamPos( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates record position in stream. This is a utility member of class and
    /// is used only in the serialization process. Does not influence the data.
    /// </summary>
    public override long StreamPos
    {
      get
      {
        return m_lStreamPosition;
      }
      set
      {
        m_lStreamPosition = value;
      }
    }
    #endregion
  }
}
#endif