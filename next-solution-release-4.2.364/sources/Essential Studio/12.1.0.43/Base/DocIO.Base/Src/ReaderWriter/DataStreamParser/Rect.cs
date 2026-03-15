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
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for Rect.
    /// </summary>
    internal class Rect : BaseWordRecord
    {
        #region Class Members
        /// <summary>
        /// 
        /// </summary>
        private long m_left;
        /// <summary>
        /// 
        /// </summary>
        private long m_right;
        /// <summary>
        /// 
        /// </summary>
        private long m_top;
        /// <summary>
        /// 
        /// </summary>
        private long m_bottom;
        #endregion

        #region Class Properties
        /// <summary>
        /// 
        /// </summary>
        public long Left
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Right
        {
            get
            {
                return m_right;
            }
            set
            {
                m_right = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public long Top
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public long Bottom
        {
            get
            {
                return m_bottom;
            }
            set
            {
                m_bottom = value;
            }
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            m_left = stream.ReadByte();
            m_top = stream.ReadByte();
            m_right = stream.ReadByte();
            m_bottom = stream.ReadByte();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            stream.WriteByte((byte)m_left);
            stream.WriteByte((byte)m_top);
            stream.WriteByte((byte)m_right);
            stream.WriteByte((byte)m_bottom);
        }
        #endregion
    }
}
