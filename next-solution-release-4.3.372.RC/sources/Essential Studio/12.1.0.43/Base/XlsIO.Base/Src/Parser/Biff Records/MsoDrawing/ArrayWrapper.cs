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

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
	/// <summary>
	/// Summary description for ArrayWrapper.
	/// </summary>
	public class ArrayWrapper
	{
    #region Class members
    /// <summary>
    /// Wrapped byte array.
    /// </summary>
    private byte[] m_arrBuffer;
    /// <summary>
    /// Hash value.
    /// </summary>
    private int m_iHash;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. To prevent user from creation without arguments.
    /// </summary>
    private ArrayWrapper()
    {
    }
    /// <summary>
    /// Initializes new instance of the wrapper.
    /// </summary>
    /// <param name="arrBuffer">Buffer to wrap.</param>
    public ArrayWrapper( byte[] arrBuffer )
    {
      if( arrBuffer == null )
        throw new ArgumentNullException( "arrBuffer" );

      m_arrBuffer = arrBuffer;
      EvaluateHash();
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Determines whether the specified Object is equal to the current Object.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns>
    /// True if the specified Object is equal to the current Object; otherwise, false.
    /// </returns>
    public override bool Equals( object obj )
    {
      if( obj == null ) return false;

      byte[] arrBuffer = null;

      if( obj is ArrayWrapper )
      {
        arrBuffer = ( ( ArrayWrapper )obj ).m_arrBuffer;
      }
      else if( obj is byte[] )
      {
        arrBuffer = ( byte[] )obj;
      }

      if( arrBuffer == null ) return false;

      return BiffRecordRaw.CompareArrays( m_arrBuffer, arrBuffer );
    }

    /// <summary>
    /// Serves as a hash function for a particular type, suitable for use in
    /// hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current Object.</returns>
    public override int GetHashCode()
    {
      return m_iHash;
    }

    #endregion

    #region Class helper methods
    /// <summary>
    /// Evaluates hash value.
    /// </summary>
    private void EvaluateHash()
    {
      int iLength = m_arrBuffer.Length;
      const int IntSize = 4;
      int iCount = iLength / IntSize;
      m_iHash = 0;

      for( int i = 0, iOffset = 0; i < iCount; i++, iOffset += IntSize )
      {
        m_iHash |= BitConverter.ToInt32( m_arrBuffer, iOffset );
      }
    }
    #endregion
  }
}
