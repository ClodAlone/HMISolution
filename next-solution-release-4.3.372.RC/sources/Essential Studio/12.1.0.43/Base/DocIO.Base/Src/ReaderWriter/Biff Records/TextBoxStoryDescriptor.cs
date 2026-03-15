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
using System.Diagnostics;
using System.IO;

using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for TextBoxDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TextBoxStoryDescriptor : BaseWordRecord
    {
        #region Class constants
        internal static int DEF_TXBX_LENGTH = 22;
        #endregion

        #region  Class Members
        //private Stream m_stream;
        /// <summary>
        /// when not fReusable count of textboxes in story chain.
        /// </summary>
        private int m_cTxbxAndiNextReuse;
        //		/// <summary>
        //		///when fReusable, the index of the next in the linked list of reusable FTXBXSs
        //		/// </summary>
        //		private int m_iNextReuse;
        /// <summary>
        /// if fReusable, counts the number of reusable textboxes follow 
        /// this one in the linked list.
        /// </summary>
        private int m_cReusable;
        /// <summary>
        /// this textbox is not currently in use.
        /// </summary>
        private bool m_fReusable;
        /// <summary>
        /// Reserved
        /// </summary>
        private uint m_reserved;
        /// <summary>
        /// Shape Identifier for first Office Shape in textbox chain.
        /// </summary>
        private int m_lid;
        /// <summary>
        /// 
        /// </summary>
        private int m_txidUndo;
        #endregion

        #region Class Properties
        /// <summary>
        /// Count of textboxes in story chain
        /// </summary>
        internal int TextBoxCnt
        {
            get
            {
                return m_cTxbxAndiNextReuse;
            }
            set
            {
                m_cTxbxAndiNextReuse = value;
            }
        }
        //		/// <summary>
        //		/// Index of the next reusable textBox
        //		/// </summary>
        //		internal int NextReuse
        //		{
        //			get
        //			{
        //				return m_textBoxStruct.NextReuse;
        //			}
        //			set
        //			{
        //				m_textBoxStruct.NextReuse = value;
        //			}
        //		}
        /// <summary>
        /// The number of reusable 
        /// textboxes follow this one
        /// </summary>
        internal int ReusableCnt
        {
            get
            {
                return m_cReusable;
            }
            set
            {
                m_cReusable = value;
            }

        }
        /// <summary>
        ///Is current textbox reusable 
        /// </summary>
        internal bool IsReusable
        {
            get
            {
                return m_fReusable;
            }
            set
            {
                m_fReusable = value;
            }
        }
        /// <summary>
        /// Shape Identifier for first Office Shape
        /// </summary>
        internal int ShapeIdent
        {
            get
            {
                return m_lid;
            }
            set
            {
                m_lid = value;
            }
        }
        internal uint Reserved
        {
            get
            {
                return m_reserved;
            }
            set
            {
                m_reserved = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal TextBoxStoryDescriptor()
        { }
        internal TextBoxStoryDescriptor(Stream stream)
        {
            Read(stream);
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            m_cTxbxAndiNextReuse = ReadInt32(stream);
            m_cReusable = ReadInt32(stream);
            m_fReusable = ReadInt16(stream) == 1;
            m_reserved = ReadUInt32(stream);
            m_lid = ReadInt32(stream);
            m_txidUndo = ReadInt32(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal void Write(Stream stream)
        {
            WriteInt32(stream, m_cTxbxAndiNextReuse);
            WriteInt32(stream, m_cReusable);
            if (m_fReusable)
            {
                WriteInt16(stream, 1);
            }
            else WriteInt16(stream, 0);
            WriteInt32(stream, (int)m_reserved);
            WriteInt32(stream, m_lid);
            WriteInt32(stream, m_txidUndo);
        }
        #endregion
    }
}
