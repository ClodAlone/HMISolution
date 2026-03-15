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
using System.IO;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for FKPForCharacterProperties = CharPropertiesPage.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class CharacterPropertiesPage : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Size of the FC (File Character position).
        /// </summary>
        private const int DEF_FC_SIZE = 4;
        //    /// <summary>
        //    /// Formatted disk page for character properties.
        //    /// </summary>
        //    private FKPStructure m_fkp;
        /// <summary>
        /// Each FC is the limit FC of a run of exception text.
        /// </summary>
        private uint[] m_arrFC;
        //    /// <summary>
        //    /// An array of bytes where each byte is the word offset of a CHPX.
        //    /// If the byte stores is 0, there is no difference between run's
        //    /// character properties and the style's character properties.
        //    /// </summary>
        //    private byte[] m_arrOffsets;
        /// <summary>
        /// Consists of all of the CHPXs stored in FKP concatenated end to end.
        /// Each CHPX is prefixed with a count of bytes which records its length.
        /// </summary>
        private CharacterPropertyException[] m_arrCHPX;
        #endregion

        #region Class Properties

        /// <summary>
        /// Each FC is the limit FC of a run of exception text.
        /// </summary>
        internal uint[] FileCharPos
        {
            get
            {
                return m_arrFC;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal CharacterPropertyException[] CharacterProperties
        {
            get
            {
                return m_arrCHPX;
            }
        }
        /// <summary>
        /// Count of runs.
        /// </summary>
        internal int RunsCount
        {
            get
            {
                return m_arrCHPX.Length;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("RunsCount");
                }

                if (m_arrCHPX == null || value != m_arrCHPX.Length)
                {
                    m_arrCHPX = new CharacterPropertyException[value];

                    for (int i = 0; i < value; i++)
                    {
                        m_arrCHPX[i] = new CharacterPropertyException();
                    }

                    m_arrFC = new uint[value + 1];
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return FKPStructure.DEF_RECORD_SIZE;
            }
        }

        #endregion

        #region Class Initialize/Finilize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal CharacterPropertiesPage()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="converter"></param>
        internal CharacterPropertiesPage(FKPStructure structure)
        {
            Parse(structure);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="converter"></param>
        private void Parse(FKPStructure structure)
        {
            byte[] arrData = structure.PageData;

            m_arrFC = new uint[structure.Count + 1];
            byte[] arrOffsets = new byte[structure.Count];

            int iFCSize = (structure.Count + 1) * Constants.FileCharPosSize;
            Buffer.BlockCopy(arrData, 0, m_arrFC, 0, iFCSize);
            //API.CopyMemory( m_arrFC, arrData, iFCSize );

            //API.CopyMemory( arrOffsets, ref arrData[ iFCSize ], structure.Count );
            Array.Copy(arrData, iFCSize, arrOffsets, 0, structure.Count);

            m_arrCHPX = new CharacterPropertyException[structure.Count];

            for (int i = 0; i < structure.Count; i++)
            {
                int iOffset = arrOffsets[i] * 2; // convert value into offset in the data array.
                m_arrCHPX[i] = new CharacterPropertyException(arrData, iOffset);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        private FKPStructure Save()
        {
            FKPStructure result = new FKPStructure();
            int iCount = RunsCount;
            int sumSize = 0;

            result.Count = (byte)iCount;
            int iFCSize = (iCount + 1) * Constants.FileCharPosSize;

            //      API.CopyMemory( ref result.PageData[ 0 ], m_arrFC, iFCSize );
            Buffer.BlockCopy(m_arrFC, 0, result.PageData, 0, iFCSize);

            byte[] arrOffsets = new byte[iCount];
            int iLastOffset = 511;

            for (int i = iCount - 1; i >= 0; i--)
            {
                if (m_arrCHPX[i] != null)
                {
                    int iLength = m_arrCHPX[i].Length;
                    int iStart = iLastOffset - iLength;

                    if (iStart % 2 != 0) iStart--;

                    iLastOffset = iStart;

                    arrOffsets[i] = (byte)(iLastOffset / 2);

                    m_arrCHPX[i].Save(result.PageData, iLastOffset);

                    sumSize += iLength + 4 + 1;
                }
            }

            //Trace.WriteLine( 512 - iLastOffset ,"Chpx size in page" );

            if (sumSize > 512)
                throw new Exception("FKP Chpx buffer overflow: " + sumSize.ToString());

            if (iLastOffset < (iFCSize + arrOffsets.Length))
                throw new Exception("FKP Chpx buffer overflow, ( chpx start at: " +
                  iLastOffset.ToString() + "FC end: " + iFCSize + ", end of rgb: " + (iFCSize + arrOffsets.Length).ToString());

            arrOffsets.CopyTo(result.PageData, iFCSize);

            return result;
        }
        /// <summary>
        /// Saves record into array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            FKPStructure structure = Save();
            return structure.Save(arrData, iOffset);
        }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="arrData"></param>
        //    /// <param name="iOffset"></param>
        //    /// <param name="converter"></param>
        //    /// <returns></returns>
        //    internal int SaveToStream( Stream stream, MemoryConverter converter )
        //    {
        //      FKPStructure structure = Save( converter );
        //      return structure.Save( stream );
        //    }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal int SaveToStream(BinaryWriter writer, Stream stream)
        {
            long start = stream.Position;
            Dictionary<int, byte> chpxOffsetCollection = new Dictionary<int, byte>();
            //      FKPStructure result = new FKPStructure();
            int iCount = RunsCount;
            int sumSize = 0;

            //      result.Count = ( byte )iCount;
            int iFCSize = (iCount + 1) * Constants.FileCharPosSize;

            //      API.CopyMemory( ref result.PageData[ 0 ], m_arrFC, iFCSize );
            //      Buffer.BlockCopy( m_arrFC, 0, result.PageData, 0, iFCSize );
            byte[] arr = new byte[iFCSize];
            Buffer.BlockCopy(m_arrFC, 0, arr, 0, iFCSize);
            stream.Write(arr, 0, arr.Length);

            byte[] arrOffsets = new byte[iCount];
            int iLastOffset = 511;

            for (int i = iCount - 1; i >= 0; i--)
            {
                int prevChpxIndex = -1;
                if (i < iCount - 1 && IsChpxRepeats(i, out prevChpxIndex))
                {
                    arrOffsets[i] = chpxOffsetCollection[prevChpxIndex];
                }
                else if (m_arrCHPX[i] != null)
                {
                    int iLength = m_arrCHPX[i].Length;
                    int iStart = iLastOffset - iLength;

                    if (iStart % 2 != 0) iStart--;

                    iLastOffset = iStart;

                    arrOffsets[i] = (byte)(iLastOffset / 2);
                    chpxOffsetCollection.Add(i, arrOffsets[i]);
                    stream.Position = start + iLastOffset;
                    m_arrCHPX[i].Save(writer, stream, iLength);

                    sumSize += iLength + 4 + 1;
                }
            }

            if (sumSize > 512)
                throw new Exception("FKP Chpx buffer overflow: " + sumSize.ToString());

            //      if( iLastOffset < ( iFCSize + arrOffsets.Length ) )
            //        throw new ApplicationException( "FKP Chpx buffer overflow, ( chpx start at: " + 
            //          iLastOffset.ToString() + "FC end: "+ iFCSize + ", end of rgb: " + ( iFCSize + arrOffsets.Length ).ToString() );

            //      arrOffsets.CopyTo( result.PageData, iFCSize );
            long endPos = stream.Position;
            stream.Position = start + iFCSize;
            stream.Write(arrOffsets, 0, arrOffsets.Length);

            ////////////////////////////////////
            if (stream == null)
                throw new ArgumentNullException("stream");

            //      if( iOffset < 0 || iOffset + DEF_RECORD_SIZE > arrData.Length )
            //        throw new ArgumentOutOfRangeException( "iOffset" );

            //      m_arrPageData.CopyTo( arrData, iOffset );
            //      iOffset += m_arrPageData.Length;
            //      arrData[ iOffset ] = m_btLength;
            if (endPos > start + 511)
            {
                throw new Exception("chpx overflow");
            }

            stream.Position = start + 511;
            stream.WriteByte((byte)iCount);

            return (int)stream.Position;
        }
        /// <summary>
        /// checks whether the Chpx at the current index is already parsed. (already added to offset array)
        /// </summary>
        /// <param name="CurrentIndex">Current Index</param>
        /// <returns></returns>
        internal bool IsChpxRepeats(int CurrentIndex,out int ReturnIndex)
        {
            CharacterPropertyException currentChpx = m_arrCHPX[CurrentIndex];
            CharacterPropertyException prevChpx;
            bool ischpxRepeats = false;
            ReturnIndex = - 1;
            for (int i = m_arrCHPX.Length - 1; i > CurrentIndex; i--)
            {
                prevChpx = m_arrCHPX[i];
                ReturnIndex = i;
                if (ischpxRepeats = currentChpx.Equals(prevChpx))
                    break;
            }
            return ischpxRepeats;
        }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="converter"></param>
        //    /// <returns></returns>
        //    internal int SaveToStream( Stream stream )
        //    {
        ////      FKPStructure result = new FKPStructure();
        //      int iStartPos = ( int )stream.Position;
        //      int iCount = RunsCount;
        //      
        ////      result.Count = ( byte )iCount;
        //      int iFCSize = ( iCount + 1 ) * Constants.FileCharPosSize;
        //      
        //      byte[] arr = new byte[ iFCSize ];
        //      Buffer.BlockCopy( m_arrFC, 0, arr, 0, iFCSize );
        //      stream.Write( arr, 0, arr.Length );
        //
        //      byte[] arrOffsets = new byte[ iCount ];
        //      stream.Write( arrOffsets, 0, arrOffsets.Length );
        //      
        //
        //      for( int i = iCount - 1; i >= 0; i-- )
        //      {
        //        if( m_arrCHPX[ i ] != null )
        //        {
        ////          int iLength = m_arrCHPX[ i ].Length;
        ////          int iStart = iLastOffset - iLength;
        //
        //          if( stream.Position % 2 != 0 ) stream.Position++;
        //
        //          arrOffsets[ i ] = ( byte )( stream.Position - iStartPos / 2 );
        //
        //          m_arrCHPX[ i ].Save( stream );
        //        }
        //      }
        //
        //      //Trace.WriteLine( 512 - iLastOffset ,"Chpx size in page" );
        //      
        //      int sumSize = ( int )( stream.Position - iStartPos );
        //      if( sumSize > 512 )
        //        throw new ApplicationException( "FKP Chpx buffer overflow: " + sumSize.ToString() );
        //      
        ////      if( iLastOffset < ( iFCSize + arrOffsets.Length ) )
        ////        throw new ApplicationException( "FKP Chpx buffer overflow, ( chpx start at: " + 
        ////          iLastOffset.ToString() + "FC end: "+ iFCSize + ", end of rgb: " + ( iFCSize + arrOffsets.Length ).ToString() );
        //        
        ////      arrOffsets.CopyTo( result.PageData, iFCSize );
        //      long endPos = stream.Position;
        //      stream.Position = iStartPos + iFCSize;
        //      stream.Write( arrOffsets, 0, arrOffsets.Length );
        //
        //      ////////////////////////////////////
        //      if( stream == null )
        //        throw new ArgumentNullException( "stream" );
        //
        //      //      if( iOffset < 0 || iOffset + DEF_RECORD_SIZE > arrData.Length )
        //      //        throw new ArgumentOutOfRangeException( "iOffset" );
        //
        //      //      m_arrPageData.CopyTo( arrData, iOffset );
        //      //      iOffset += m_arrPageData.Length;
        //      //      arrData[ iOffset ] = m_btLength;
        //      if( endPos > iStartPos + 511 )
        //      {
        //        throw new ApplicationException( "chpx overflow" );
        //      }
        //      
        //      stream.Position = iStartPos + 511;
        //      stream.WriteByte( ( byte )iCount );
        //
        //      return ( int )stream.Position;
        //    }
        #endregion
    }
}
