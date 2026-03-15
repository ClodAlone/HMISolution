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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;

#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for BookmarkNameRecord.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class BookmarkNameStringTable
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private List<String> m_strArr;
        /// <summary>
        /// 
        /// </summary>
        private int m_bkmkCount;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int BookmarkCount
        {
            get
            {
                return m_bkmkCount;
            }
            set
            {
                m_bkmkCount = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string this[int index]
        {
            get
            {
                return m_strArr[index];
            }
            //      set
            //      {
            //        m_strArr[ index ] = value;
            //      }
        }
        /// <summary>
        /// Gets the length of the bookmark names.
        /// </summary>
        /// <value>The length of the bookmark names.</value>
        internal int BookmarkNamesLength
        {
            get
            {
                return (m_strArr == null) ? 0 : m_strArr.Count;
            }

        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal BookmarkNameStringTable(Stream stream, int length)
        {
            int DEF_RECORDS_COUNT = 6;

            long startPos = stream.Position;
            byte[] buf = new byte[DEF_RECORDS_COUNT];
            short symbCount;

            stream.Read(buf, 0, DEF_RECORDS_COUNT);
            m_bkmkCount = BitConverter.ToInt32(buf, 2);
            m_strArr = new List<String>(m_bkmkCount);

            for (int i = 0; i < m_bkmkCount; i++)
            {
                buf = new byte[2];
                stream.Read(buf, 0, 2);
                symbCount = BitConverter.ToInt16(buf, 0);
                buf = new byte[symbCount * 2];
                stream.Read(buf, 0, buf.Length);
#if SILVERLIGHT || WP
                string s = Encoding.Unicode.GetString( buf, 0, buf.Length );
#else
                string s = Encoding.Unicode.GetString(buf);
#endif

                m_strArr.Add(s);
            }

            long endPos = stream.Position;

            if ((endPos - startPos) > length)
                throw new StreamReadException("");

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkNameStringTable"/> class.
        /// </summary>
        internal BookmarkNameStringTable()
        {
            m_bkmkCount = 0;
            m_strArr = new List<String>(m_bkmkCount);
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        internal void Save(Stream stream, WPFIBData fib)
        {
            if (m_strArr.Count > 0)
            {
                int DEF_RECORDS_COUNT = 6;
                //int bytesCount = 0;
                byte[] strBuf;

                fib.fcSttbfbkmk = (int)stream.Position;
                byte[] arr = new byte[DEF_RECORDS_COUNT];
                byte[] buf = BitConverter.GetBytes(m_strArr.Count);
                buf.CopyTo(arr, 2);

                arr[0] = arr[1] = 255;
                stream.Write(arr, 0, arr.Length);

                //bytesCount += arr.Length;

                m_bkmkCount = m_strArr.Count;

                for (int i = 0; i < m_bkmkCount; i++)
                {
                    strBuf = Encoding.Unicode.GetBytes(m_strArr[i]);
                    buf = BitConverter.GetBytes((short)(strBuf.Length / 2));
                    stream.Write(buf, 0, buf.Length);
                    stream.Write(strBuf, 0, strBuf.Length);
                    //bytesCount += buf.Length + strBuf.Length;
                }

                fib.lcbSttbfbkmk = (int)(stream.Position - fib.fcSttbfbkmk);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        internal void Add(string name)
        {
            m_strArr.Add(name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal int Find(string name)
        {
            int index = -1;
            for (int i = 0; i < m_strArr.Count; i++)
            {
                if (this[i] == name)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        #endregion

    }
}
