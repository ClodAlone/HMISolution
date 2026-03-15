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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for ColumnDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ColumnDescriptor
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private const ushort DEF_WIDTH = 1000;
        private const ushort DEF_SPACE = 720;
        /// <summary>
        /// 
        /// </summary>
        private SinglePropertyModifierRecord m_widthRecord = null;
        private SinglePropertyModifierRecord m_spaceRecord = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="space"></param>
        internal ColumnDescriptor(SinglePropertyModifierRecord width, SinglePropertyModifierRecord space)
        {
            m_widthRecord = width;
            m_spaceRecord = space;
            Width = DEF_WIDTH;
            Space = DEF_SPACE;
        }
        /// <summary>
        /// 
        /// </summary>
        internal ColumnDescriptor()
        {
            m_widthRecord = new SinglePropertyModifierRecord(WordSprmOptions.sprmSDxaColWidth);
            m_widthRecord.ByteArray = new byte[3];
            m_spaceRecord = new SinglePropertyModifierRecord(WordSprmOptions.sprmSDxaColSpacing);
            m_spaceRecord.ByteArray = new byte[3];
            Width = DEF_WIDTH;
            Space = DEF_SPACE;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal ushort Width
        {
            get
            {
                byte[] byteArr = m_widthRecord.ByteArray;
                return BitConverter.ToUInt16(byteArr, 1);
            }
            set
            {
                byte[] byteArr = BitConverter.GetBytes(value);
                m_widthRecord.ByteArray[1] = byteArr[0];
                m_widthRecord.ByteArray[2] = byteArr[1];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort Space
        {
            get
            {
                byte[] byteArr = m_spaceRecord.ByteArray;
                return BitConverter.ToUInt16(byteArr, 1);
            }
            set
            {
                byte[] byteArr = BitConverter.GetBytes(value);
                m_spaceRecord.ByteArray[1] = byteArr[0];
                m_spaceRecord.ByteArray[2] = byteArr[1];
            }
        }
        #endregion
    }
}
