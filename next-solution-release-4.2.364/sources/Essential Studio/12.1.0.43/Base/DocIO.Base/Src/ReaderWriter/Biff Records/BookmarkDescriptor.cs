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
using System.IO;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for BookmarkFirstDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class BookmarkDescriptor
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private int DEF_STRUCT_SIZE = 4;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_bkmkCount;
        /// <summary>
        /// 
        /// </summary>
        private List<BookmarkFirstStructure> m_bkfArr;
        /// <summary>
        /// 
        /// </summary>
        private List<Int32> m_bklArr;
        /// <summary>
        /// 
        /// </summary>
        private int m_lastEnd = 0;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int BookmarkCount
        {
            get
            {
                return m_bkfArr.Count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private BookmarkFirstStructure[] BkfArray
        {
            get
            {
                return m_bkfArr.ToArray();
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal BookmarkDescriptor(Stream stream, int bookmarkCount, int bkfPos, int bkfLength,
          int bklPos, int bklLength)
        {
            // Allocates arrays 
            m_bkmkCount = bookmarkCount;
            m_bkfArr = new List<BookmarkFirstStructure>(m_bkmkCount);
            m_bklArr = new List<Int32>(m_bkmkCount);

            // Read BKF data
            ReadBKF(bkfPos, stream, bkfLength);

            // Moves to BKL data
            ReadBKL(bklPos, stream, bklLength);
        }
        /// <summary>
        /// 
        /// </summary>
        internal BookmarkDescriptor()
        {
            m_bkmkCount = 0;
            m_bkfArr = new List<BookmarkFirstStructure>(m_bkmkCount);
            for (int i = 0; i < BkfArray.Length; i++)
            {
                m_bkfArr.Add(new BookmarkFirstStructure());
            }

            m_bklArr = new List<Int32>(m_bkmkCount);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        internal int GetBeginPos(int i)
        {
            return BkfArray[i].BeginPos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="position"></param>
        internal void SetBeginPos(int i, int position)
        {
            BkfArray[i].BeginPos = position;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        internal int GetEndPos(int i)
        {
            short index = BkfArray[i].EndIndex;
            return m_bklArr[index];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="position"></param>
        internal void SetEndPos(int i, int position)
        {
            if (BkfArray.Length <= i || m_bklArr.Count <= m_lastEnd)
                return;

            BkfArray[i].EndIndex = (short)m_lastEnd;
            //int index = m_bklArr.Count - 1;
            m_bklArr[m_lastEnd] = position;
            m_lastEnd++;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        /// <param name="endChar"></param>
        internal void Save(Stream stream, WPFIBData fib, int endChar)
        {
            if (BookmarkCount > 0)
            {
                WriteBKF(stream, fib, endChar);

                WriteBKL(stream, fib, endChar);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        internal void Add(int startPos)
        {
            int i = m_bkfArr.Count;
            m_bkfArr.Add(new BookmarkFirstStructure());
            m_bklArr.Add(startPos);
            BkfArray[i].BeginPos = startPos;
        }

        /// <summary>
        /// Determines whether specified bookmark covers a group of table cells.
        /// </summary>
        /// <param name="bookmarkIndex">Index of the bookmark.</param>
        /// <returns>
        /// 	<c>true</c> if [is column group] returns true; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsCellGroup(int bookmarkIndex)
        {
            return ((BkfArray[bookmarkIndex].Props & 0x8000) >> 15) == 1;
        }
        /// <summary>
        /// Sets specified bookmark covers a group of table cells.
        /// </summary>
        /// <param name="bookmarkIndex">Index of the bookmark.</param>
        internal void SetCellGroup(int bookmarkIndex, bool isCellGroup)
        {
            if (BkfArray.Length <= bookmarkIndex)
                return;

            BkfArray[bookmarkIndex].Props = (isCellGroup) ?
              (short)BaseWordRecord.SetBitsByMask(BkfArray[bookmarkIndex].Props, 0x8000, 15, 1) :
              (short)BaseWordRecord.SetBitsByMask(BkfArray[bookmarkIndex].Props, 0x8000, 15, 0);
        }
        /// <summary>
        /// Gets the start index of the cell which is covered by the bookmark.
        /// </summary>
        /// <param name="bookmarkIndex">Index of the bookmark.</param>
        /// <returns></returns>
        internal int GetStartCellIndex(int bookmarkIndex)
        {
            return (int)(BkfArray[bookmarkIndex].Props & 0x007F);
        }
        /// <summary>
        /// Sets the start index of the cell bookmark.
        /// </summary>
        /// <param name="bookmarkIndex">Index of the bookmark.</param>
        /// <param name="position">The position.</param>
        internal void SetStartCellIndex(int bookmarkIndex, int position)
        {
            if (BkfArray.Length <= bookmarkIndex)
                return;

            BkfArray[bookmarkIndex].Props = (short)BaseWordRecord.SetBitsByMask(BkfArray[bookmarkIndex].Props,
              0x007F, position);
        }
        /// <summary>
        /// Gets the end index of the cell which is covered by the bookmark.
        /// </summary>
        /// <param name="bookmarkIndex">Index of the bookmark.</param>
        /// <returns></returns>
        internal int GetEndCellIndex(int bookmarkIndex)
        {
            return (int)((BkfArray[bookmarkIndex].Props & 0x7F00) >> 8);
        }
        /// <summary>
        /// Sets the end index of the cell bookmark.
        /// </summary>
        /// <param name="bookmarkIndex">Index of the bookmark.</param>
        /// <param name="position">The position.</param>
        internal void SetEndCellIndex(int bookmarkIndex, int position)
        {
            if (BkfArray.Length <= bookmarkIndex)
                return;

            BkfArray[bookmarkIndex].Props = (short)BaseWordRecord.SetBitsByMask(BkfArray[bookmarkIndex].Props,
              0x7F00, 8, position);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bklPos"></param>
        /// <param name="stream"></param>
        /// <param name="bklLength"></param>
        private void ReadBKL(int bklPos, Stream stream, int bklLength)
        {
            stream.Position = bklPos;

            // Fills BookmarkLimStructure array
            byte[] bytebuf = new byte[m_bkmkCount * Constants.BytesInInt];
            stream.Read(bytebuf, 0, bytebuf.Length);

            int[] temp = new int[m_bkmkCount];
            Buffer.BlockCopy(bytebuf, 0, temp, 0, bytebuf.Length);
            //API.CopyMemory( temp, bytebuf, bytebuf.Length );
            m_bklArr.AddRange(temp);

            if (stream.Position > (bklPos + bklLength))
                throw new StreamReadException("Too many bytes read for BookmarkLimDescriptor");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bkfPos"></param>
        /// <param name="stream"></param>
        /// <param name="bkfLength"></param>
        private void ReadBKF(int bkfPos, Stream stream, int bkfLength)
        {
            byte[] bytebuf = new byte[m_bkmkCount * DEF_STRUCT_SIZE];
            ushort[] shortbuf = new ushort[m_bkmkCount * Constants.BytesInWord];
            int[] intbuf = new int[m_bkmkCount];

            // Reads data
            stream.Position = bkfPos;
            stream.Read(bytebuf, 0, m_bkmkCount * DEF_STRUCT_SIZE);

            Buffer.BlockCopy(bytebuf, 0, intbuf, 0, bytebuf.Length);
            //API.CopyMemory( intbuf, bytebuf, bytebuf.Length );

            // Fills BookmarkFirstStructure array
            for (int i = 0; i < m_bkmkCount; i++)
            {
                m_bkfArr.Add(new BookmarkFirstStructure());
                BkfArray[i].BeginPos = intbuf[i];
            }

            // skip count of bookmarks, it's already known
            stream.Position += 4;
            //  read index of end positiona
            bytebuf = new byte[m_bkmkCount * Constants.BytesInInt];
            shortbuf = new ushort[m_bkmkCount * Constants.BytesInWord];
            stream.Read(bytebuf, 0, bytebuf.Length);

            Buffer.BlockCopy(bytebuf, 0, shortbuf, 0, bytebuf.Length);
            //API.CopyMemory( shortbuf, bytebuf, bytebuf.Length );
            // Fills BookmarkFirstStructure indexes and flags
            for (int i = 0; i < m_bkmkCount; i++)
            {
                BkfArray[i].EndIndex = (short)shortbuf[2 * i];
                BkfArray[i].Props = (ushort)shortbuf[2 * i + 1];
            }

            if (stream.Position > (bkfPos + bkfLength))
                throw new StreamReadException("To many bytes read for BookmarkFirstDescriptor");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        /// <param name="endChar"></param>
        private void WriteBKF(Stream stream, WPFIBData fib, int endChar)
        {
            byte[] buf = null;

            // Allocates arrays 
            byte[] bytebuf = new byte[BookmarkCount * DEF_STRUCT_SIZE];

            // Writes BKF data
            fib.fcPlcfbkf = (int)stream.Position;

            for (int i = 0; i < BkfArray.Length; i++)
            {
                buf = BkfArray[i].SavePos();
                stream.Write(buf, 0, buf.Length);
            }
            buf = BitConverter.GetBytes(endChar);
            stream.Write(buf, 0, buf.Length);

            for (int i = 0; i < BookmarkCount; i++)
            {
                buf = BkfArray[i].SaveProps();
                stream.Write(buf, 0, buf.Length);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        /// <param name="endChar"></param>
        private void WriteBKL(Stream stream, WPFIBData fib, int endChar)
        {
            // Allocates arrays 
            byte[] buf = null;
            byte[] bytebuf = new byte[BookmarkCount * DEF_STRUCT_SIZE];

            // Write BKL data
            fib.lcbPlcfbkf = (int)(stream.Position - fib.fcPlcfbkf);
            fib.fcPlcfbkl = (int)stream.Position;
            int[] temp = m_bklArr.ToArray();
            Buffer.BlockCopy(temp, 0, bytebuf, 0, bytebuf.Length);
            //API.CopyMemory( bytebuf, temp, bytebuf.Length );
            stream.Write(bytebuf, 0, bytebuf.Length);

            buf = BitConverter.GetBytes(endChar);
            stream.Write(buf, 0, buf.Length);
            // 
            fib.lcbPlcfbkl = (int)(stream.Position - fib.fcPlcfbkl);
        }
        #endregion

    }
}
