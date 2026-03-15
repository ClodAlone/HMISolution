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

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for BookmarkFirstStructure.
    /// </summary>
    internal class BookmarkFirstStructure
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_beginCP;

        /// <summary>
        /// 
        /// </summary>
        private short m_endIndex;

        /// <summary>
        /// 
        /// </summary>
        private int m_props;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int BeginPos
        {
            get
            {
                return m_beginCP;
            }
            set
            {
                m_beginCP = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int Props
        {
            get
            {
                return m_props;
            }
            set
            {
                m_props = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal short EndIndex
        {
            get
            {
                return m_endIndex;
            }
            set
            {
                m_endIndex = value;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Saves bookmark positions.
        /// </summary>
        /// <returns></returns>
        internal byte[] SavePos()
        {
            byte[] arr = new byte[4];
            byte[] buf = BitConverter.GetBytes(m_beginCP);
            buf.CopyTo(arr, 0);

            return arr;
        }

        /// <summary>
        /// Saves bookmark properties.
        /// </summary>
        /// <returns></returns>
        internal byte[] SaveProps()
        {
            byte[] arr = new byte[4];
            byte[] buf = BitConverter.GetBytes(m_endIndex);
            buf.CopyTo(arr, 0);
            buf = BitConverter.GetBytes((ushort)m_props);
            buf.CopyTo(arr, 2);

            return arr;
        }
        #endregion
    }
}
