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
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for ColumnArray.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ColumnArray : List<Object>
    {
        #region Class constants
        /// <summary>
        /// Default distance between coumns in twips
        /// </summary>
        private const int DEF_DISTANCE_BETWEEN_COLUMNS = 720;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private SinglePropertyModifierArray m_sprms = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        internal ColumnArray(SinglePropertyModifierArray sprms)
        {
            m_sprms = sprms;
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal ColumnDescriptor AddColumn()
        {
            //m_sprms.SetValue( WordSprmOptions.sprmSDxaColumns, ( ushort )( DxaColumns ) );
            m_sprms.SetValue(WordSprmOptions.sprmSCcolumns, (ushort)(Count));

            SinglePropertyModifierRecord widthRecord = new SinglePropertyModifierRecord(WordSprmOptions.sprmSDxaColWidth);
            SinglePropertyModifierRecord spaceRecord = new SinglePropertyModifierRecord(WordSprmOptions.sprmSDxaColSpacing);

            widthRecord.ByteArray = new byte[3];
            spaceRecord.ByteArray = new byte[3];

            widthRecord.ByteArray[0] = (byte)Count;
            spaceRecord.ByteArray[0] = (byte)Count;

            m_sprms.Add(widthRecord);
            m_sprms.Add(spaceRecord);
            ColumnDescriptor column = new ColumnDescriptor(widthRecord, spaceRecord);
            Add(column);

            return column;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal ColumnDescriptor AddEmptyColumn()
        {
            m_sprms.SetValue(WordSprmOptions.sprmSCcolumns, (ushort)(Count));

            ColumnDescriptor column = new ColumnDescriptor();
            Add(column);

            return column;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        new internal ColumnDescriptor this[int index]
        {
            get
            {
                return (ColumnDescriptor)base[index];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool ColumnsEvenlySpaced
        {
            get
            {
                return m_sprms.GetByte(WordSprmOptions.sprmSFEvenlySpaced, 1) == 1;
            }
            set
            {
                m_sprms.SetValue(WordSprmOptions.sprmSFEvenlySpaced, (value) ? 1 : 0);
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        internal void ReadColumnsProperties()
        {
            int colCount = m_sprms.GetUShort(WordSprmOptions.sprmSCcolumns, 0) + 1;
            for (int i = 0; i < colCount; i++)
            {
                //AddColumn();
                ColumnDescriptor column = new ColumnDescriptor();
                Add(column);
            }

            // If columns are evenly spaced, then calculate distance of every column
            if (ColumnsEvenlySpaced)
            {
                ushort pageWidth = m_sprms.GetUShort(WordSprmOptions.sprmSXaPage, 0);
                ushort leftMargin = m_sprms.GetUShort(WordSprmOptions.sprmSDxaLeft, 0);
                ushort rightMargin = m_sprms.GetUShort(WordSprmOptions.sprmSDxaRight, 0);
                int usingWidth = pageWidth - leftMargin - rightMargin;
                //Note: hot fix
                ushort dxaBetweenColumns = m_sprms.GetUShort(WordSprmOptions.sprmSDxaColumns, DEF_DISTANCE_BETWEEN_COLUMNS);

                int colWidth = (usingWidth - (colCount - 1) * dxaBetweenColumns) / colCount;
                for (int i = 0; i < colCount; i++)
                {
                    this[i].Width = (ushort)colWidth;
                    this[i].Space = (ushort)dxaBetweenColumns;
                }
            }
            // if not, then read columns properties
            else
            {
                List<UInt16> widthsArr = new List<UInt16>();
                List<UInt16> spacesArr = new List<UInt16>();
                for (int i = 0; i < m_sprms.Count; i++)
                {
                    if (m_sprms.GetSprmByIndex(i).TypedOptions.ToString() == WordSprmOptions.sprmSDxaColWidth.ToString())
                    {
                        byte[] bytes = m_sprms.GetSprmByIndex(i).ByteArray;
                        widthsArr.Add(BitConverter.ToUInt16(bytes, 1));
                    }
                    else if (m_sprms.GetSprmByIndex(i).TypedOptions.ToString() == WordSprmOptions.sprmSDxaColSpacing.ToString())
                    {
                        byte[] bytes = m_sprms.GetSprmByIndex(i).ByteArray;
                        spacesArr.Add(BitConverter.ToUInt16(bytes, 1));
                    }
                }
                if (Count >= 1)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (widthsArr.Count > i)
                        {
                            this[i].Width = widthsArr[i];
                            if (i + 1 < Count)
                            {
                                this[i].Space = spacesArr[i];
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
