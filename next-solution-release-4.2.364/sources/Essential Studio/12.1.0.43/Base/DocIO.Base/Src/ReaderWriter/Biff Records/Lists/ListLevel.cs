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
using System.IO;

using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    internal class ListLevel : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        internal int m_startAt = 0;

        internal ListPatternType m_nfc;
        internal ListNumberAlignment m_jc;
        internal bool m_bLegal;
        internal bool m_bNoRestart;
        internal bool m_bPrev;
        internal bool m_bPrevSpace;
        internal bool m_bWord6;
        internal bool m_unused;
        internal byte[] m_rgbxchNums;
        internal FollowCharacterType m_ixchFollow;
        internal int m_dxaSpace;
        internal int m_dxaIndent;
        internal int m_reserved;

        internal CharacterProperties m_charProps = null;
        internal ParagraphProperties m_parProps = null;

        /// <summary>
        /// 
        /// </summary>
        internal string m_str;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal ListLevel()
        {
            m_rgbxchNums = new byte[9];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal ListLevel(Stream stream)
        {
            Read(stream);
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="memConvertor"></param>
        internal void Write(Stream stream)
        {
            int pos = (int)stream.Position;

            WriteInt32(stream, m_startAt);
            stream.WriteByte((byte)m_nfc);
            int temp = 0;
            temp = (int)(((ListNumberAlignment)temp) | m_jc);
            temp |= (m_bLegal ? 4 : 0);
            temp |= (m_bNoRestart ? 8 : 0);
            temp |= (m_bPrev ? 0x10 : 0);
            temp |= (m_bPrevSpace ? 0x20 : 0);
            temp |= (m_bWord6 ? 0x40 : 0);
            temp |= (m_unused ? 0x80 : 0);
            stream.WriteByte((byte)temp);
            stream.Write(m_rgbxchNums, 0, m_rgbxchNums.Length);
            stream.WriteByte((byte)m_ixchFollow);

            WriteInt32(stream, m_dxaSpace);
            WriteInt32(stream, m_dxaIndent);

            stream.WriteByte((byte)m_charProps.Sprms.Length);
            stream.WriteByte((byte)m_parProps.Sprms.Length);

            WriteUInt16(stream, (ushort)m_reserved);

            m_parProps.Sprms.Save(stream);
            m_charProps.Sprms.Save(stream);

            // write number of chars then string
            WriteString(stream, m_str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void Read(Stream stream)
        {
            int pos = (int)stream.Position;

            m_rgbxchNums = new byte[9];
            m_startAt = (int)ReadUInt32(stream);
            m_nfc = (ListPatternType)stream.ReadByte();
            int temp = stream.ReadByte();
            m_jc = (ListNumberAlignment)((byte)(temp & 3));
            m_bLegal = (temp & 4) != 0;
            m_bNoRestart = (temp & 8) != 0;
            m_bPrev = (temp & 0x10) != 0;
            m_bPrevSpace = (temp & 0x20) != 0;
            m_bWord6 = (temp & 0x40) != 0;
            m_unused = (temp & 0x80) != 0;
            m_rgbxchNums = ReadBytes(stream, 9);
            m_ixchFollow = (FollowCharacterType)stream.ReadByte();

            m_dxaSpace = (int)ReadUInt32(stream);
            m_dxaIndent = (int)ReadUInt32(stream);

            int cbGrpprlChpx = stream.ReadByte();
            int cbGrpprlPapx = stream.ReadByte();

            m_reserved = ReadUInt16(stream);

            //Get paragraph properties
            //      m_charProps = new CharacterProperties( WordReaderBase.StyleSheet );
            m_charProps = new CharacterProperties(null);
            m_parProps = new ParagraphProperties();

            ReadListSprms(cbGrpprlPapx, stream, false);
            ReadListSprms(cbGrpprlChpx, stream, true);

            // read short then read string of read short characters
            m_str = ReadString(stream);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Reads list's character or paragraph properties.
        /// </summary>
        /// <param name="dataLen"></param>
        /// <param name="stream"></param>
        /// <param name="isChpx"></param>
        private void ReadListSprms(int dataLen, Stream stream, bool isChpx)
        {
            //      long startStreamPos = stream.Position;
            int iOffset = 0;
            if (dataLen != 0)
            {
                SinglePropertyModifierArray sprms = isChpx ? m_charProps.Sprms : m_parProps.Sprms;
                byte[] arrData = new byte[dataLen];
                stream.Read(arrData, 0, dataLen);
                while (dataLen - iOffset > 1)
                {
                    SinglePropertyModifierRecord sprm = new SinglePropertyModifierRecord();
                    try
                    {
                        iOffset = sprm.Parse(arrData, iOffset);
                    }
                    catch
                    {
                        iOffset = dataLen;
                    }
                    sprms.Add(sprm);
                }

                //        if( isChpx )
                //        {
                //          ReadChpxBytes( stream, startStreamPos, dataLen ); 
                //        }         
            }
        }

        //    /// <summary>
        //    /// Reads chpx byte array fron stream.
        //    /// </summary>
        //    /// <param name="stream">The stream.</param>
        //    /// <param name="streamPos">The stream pos.</param>
        //    /// <param name="dataLen">The data len.</param>
        //    private void ReadChpxBytes( Stream stream, long streamPos, int dataLen )
        //    {
        //      stream.Position = streamPos;
        //      byte[] sprmsByteArr = new byte[ dataLen ];
        //      stream.Read( sprmsByteArr, 0, dataLen );
        //      m_charProps.CharacterPropertyException.SprmsByteArray = sprmsByteArr;
        //    }
        #endregion
    }
}

