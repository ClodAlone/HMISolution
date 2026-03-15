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

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for FKPForCharacterProperties = ParagraphPropertiesPage.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ParagraphPropertiesPage : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Size of the FC (File Character position).
        /// </summary>
        private const int DEF_FC_SIZE = 4;
        /// <summary>
        /// Each FC is the limit FC of a run of exception text.
        /// </summary>
        private uint[] m_arrFC;
        /// <summary>
        /// An array of the BX data structure. The ith BX entry in the array describes
        /// the paragraph beginneing at fkp.rgfc[ i ].
        /// </summary>
        private BXStructure[] m_arrHeight;
        /// <summary>
        /// Consists of all of the CHPXs stored in FKP concatenated end to end.
        /// Each CHPX is prefixed with a count of bytes which records its length.
        /// </summary>
        private ParagraphExceptionInDiskPage[] m_arrPAPX;
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
        /// An array of the BX data structure. The ith BX entry in the array describes
        /// the paragraph beginneing at fkp.rgfc[ i ].
        /// </summary>
        internal BXStructure[] Heights
        {
            get
            {
                return m_arrHeight;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ParagraphExceptionInDiskPage[] ParagraphProperties
        {
            get
            {
                return m_arrPAPX;
            }
        }
        /// <summary>
        /// Count of runs.
        /// </summary>
        internal int RunsCount
        {
            get
            {
                if (m_arrPAPX == null)
                    return 0;

                return m_arrPAPX.Length;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("RunsCount");

                if (RunsCount != value)
                {
                    m_arrPAPX = new ParagraphExceptionInDiskPage[value];
                    m_arrFC = new uint[value + 1];
                    m_arrHeight = new BXStructure[value];
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
        /// 
        /// </summary>
        internal ParagraphPropertiesPage()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="converter"></param>
        internal ParagraphPropertiesPage(FKPStructure structure)
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

            int iFCSize = (structure.Count + 1) * Constants.FileCharPosSize;
            Buffer.BlockCopy(arrData, 0, m_arrFC, 0, iFCSize);
            //API.CopyMemory( m_arrFC, arrData, iFCSize );

            //API.CopyMemory( arrOffsets, ref arrData[ iFCSize ], structure.Count );
            //Array.Copy( arrData, iFCSize, arrOffsets, 0, structure.Count );

            m_arrPAPX = new ParagraphExceptionInDiskPage[structure.Count];
            m_arrHeight = new BXStructure[structure.Count];
            int iOffset = iFCSize;

            for (int i = 0; i < structure.Count; i++)
            {
                m_arrHeight[i] = new BXStructure();
                m_arrHeight[i].Parse(arrData, iOffset);
                //converter.Copy( arrData, iOffset, BXStructure.DEF_RECORD_SIZE, m_arrHeight[ i ] );
                iOffset += BXStructure.DEF_RECORD_SIZE;
            }

            for (int i = 0; i < structure.Count; i++)
            {
                iOffset = m_arrHeight[i].Offset * 2;
                m_arrPAPX[i] = new ParagraphExceptionInDiskPage();
                m_arrPAPX[i].Parse(arrData, iOffset);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal FKPStructure Save()
        {
            FKPStructure result = new FKPStructure();

            int iCount = RunsCount;

            int iFCSize = m_arrFC.Length * Constants.FileCharPosSize;

            result.Count = (byte)iCount;
            //      API.CopyMemory( ref result.PageData[ 0 ], m_arrFC, iFCSize );
            Buffer.BlockCopy(m_arrFC, 0, result.PageData, 0, iFCSize);

            int iOffset = iFCSize;
            byte bxOffset = 255;

            for (int i = 0; i < iCount; i++)
            {
                if (m_arrPAPX[i] != null)
                {
                    bxOffset -= (byte)(m_arrPAPX[i].Length / 2);

                    if (m_arrHeight[i] == null)
                    {
                        m_arrHeight[i] = new BXStructure();
                    }

                    BXStructure bx = m_arrHeight[i];
                    bx.Offset = bxOffset;

                    bx.Save(result.PageData, iOffset);
                    //converter.Copy( bx, result.PageData, iOffset, BXStructure.DEF_RECORD_SIZE );


                    iOffset += BXStructure.DEF_RECORD_SIZE;

                    m_arrPAPX[i].Save(result.PageData, bx.Offset * 2);
                }
            }

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
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset + FKPStructure.DEF_RECORD_SIZE > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            FKPStructure structure = Save();
            structure.Save(arrData, iOffset);
            //converter.Copy( structure, arrData, iOffset, FKPStructure.DEF_RECORD_SIZE );

            return FKPStructure.DEF_RECORD_SIZE;
        }
        internal void SaveToStream(BinaryWriter writer, Stream stream)
        {
            //stream = new MemoryStream( 512 );
            //writer = new BinaryWriter( stream );

            long start = stream.Position;
            int iCount = RunsCount;

            int iFCSize = m_arrFC.Length * Constants.FileCharPosSize;

            //result.Count = ( byte )iCount;
            for (int i = 0, length = m_arrFC.Length; i < length; i++)
            {
                writer.Write(m_arrFC[i]);
            }
            //Buffer.BlockCopy( m_arrFC, 0, result.PageData, 0, iFCSize );

            int iOffset = iFCSize;
            byte bxOffset = 255;

            for (int i = 0; i < iCount; i++)
            {
                if (m_arrPAPX[i] != null)
                {
                    bxOffset -= (byte)(m_arrPAPX[i].Length / 2);

                    if (m_arrHeight[i] == null)
                    {
                        m_arrHeight[i] = new BXStructure();
                    }

                    BXStructure bx = m_arrHeight[i];
                    bx.Offset = bxOffset;

                    //converter.Copy( bx, result.PageData, iOffset, BXStructure.DEF_RECORD_SIZE );
                    stream.Position = start + iOffset;
                    bx.Save(writer);


                    iOffset += BXStructure.DEF_RECORD_SIZE;

                    //m_arrPAPX[ i ].Save( result.PageData, bx.Offset * 2, converter );
                    stream.Position = start + bx.Offset * 2;
                    m_arrPAPX[i].Save(writer, stream);
                }
            }

            stream.Position = start + 511;
            stream.WriteByte((byte)iCount);

            //return ( int )stream.Position;

        }
        #endregion
    }
}
