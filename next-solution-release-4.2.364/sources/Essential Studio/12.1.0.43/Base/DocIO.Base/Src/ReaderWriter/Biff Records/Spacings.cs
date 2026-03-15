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

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for Spacings.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Spacings
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private short m_left = -1;
        private short m_right = -1;
        private short m_top = -1;
        private short m_bottom = -1;
        private byte m_cellNumber = 0;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal short Left
        {
            get
            {
                return m_left;
            }
            set
            {
                //If cell margins exceeds 1584pt means set cell margins to 0.
                if (value > (short)31680)
                    m_left = 0;
                else
                    m_left = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short Right
        {
            get
            {
                return m_right;
            }
            set
            {
                //If cell margins exceeds 1584pt means set cell margins to 0.
                if (value > (short)31680)
                    m_right = 0;
                else
                    m_right = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short Top
        {
            get
            {
                return m_top;
            }
            set
            {
                //If cell margins exceeds 1584pt means set cell margins to 0.
                if (value > (short)31680)
                    m_top = 0;
                else
                    m_top = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short Bottom
        {
            get
            {
                return m_bottom;
            }
            set
            {
                //If cell margins exceeds 1584pt means set cell margins to 0.
                if (value > (short)31680)
                    m_bottom = 0;
                else
                    m_bottom = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int CellNumber
        {
            get
            {
                return m_cellNumber;
            }
            set
            {
                m_cellNumber = (byte)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                return (m_left == -1 && m_top == -1 && m_bottom == -1 && m_right == -1);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal Spacings()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprm"></param>
        internal Spacings(SinglePropertyModifierRecord sprm)
        {
            Parse(sprm);
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprm"></param>
        internal void Parse(SinglePropertyModifierRecord sprm)
        {
            m_cellNumber = sprm.ByteArray[0];
            //      byte[] length = new byte[ Constants.BytesInWord ];
            //      for( int i = 3; i < sprm.ByteArray.Length; i++ )
            //      {
            //        length[ i - 3 ] = sprm.ByteArray[ i ];
            //      }
            //      switch( sprm.ByteArray[ 2 ] )
            //      {
            //        case 1:
            //          Top = BitConverter.ToInt16( sprm.ByteArray, 4 );
            //          break;
            //        case 2:
            //          Left = BitConverter.ToInt16( sprm.ByteArray, 4 );
            //          break;
            //        case 4:
            //          Bottom = BitConverter.ToInt16( sprm.ByteArray, 4 );
            //          break;
            //        case 8:
            //          Right = BitConverter.ToInt16( sprm.ByteArray, 4 );
            //          break;
            //      }
            byte type = sprm.ByteArray[2];
            short value = BitConverter.ToInt16(sprm.ByteArray, 4);
            if ((type & 1) != 0)
            {
                Top = value;
            }
            if ((type & 2) != 0)
            {
                Left = value;
            }
            if ((type & 4) != 0)
            {
                Bottom = value;
            }
            if ((type & 8) != 0)
            {
                Right = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifierArray"></param>
        /// <param name="options"></param>
        /// <param name="cellNumber"></param>
        internal void Save(SinglePropertyModifierArray modifierArray, int options, int cellNumber)
        {
            // Top
            if (m_top != -1)
            {
                modifierArray.Add(SaveSingleRecord(1, m_top, options));
            }

            // Left
            if (m_left != -1)
            {
                modifierArray.Add(SaveSingleRecord(2, m_left, options));
            }

            // Bottom
            if (m_bottom != -1)
            {
                modifierArray.Add(SaveSingleRecord(4, m_bottom, options));
            }

            // Right
            if (m_right != -1)
            {
                modifierArray.Add(SaveSingleRecord(8, m_right, options));
            }
        }
        /// <summary>
        /// Clone cell spacing
        /// </summary>
        /// <returns></returns>
        internal Spacings Clone()
        {
            Spacings cellSpacings = new Spacings();
            cellSpacings.m_bottom = Bottom;
            cellSpacings.m_cellNumber = (byte)CellNumber;
            cellSpacings.m_left = Left;
            cellSpacings.m_right = Right;
            cellSpacings.m_top = Top;

            return cellSpacings;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dist"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private SinglePropertyModifierRecord SaveSingleRecord(byte type, short dist, int options)
        {
            byte[] buf = new byte[6];
            buf[0] = m_cellNumber;
            buf[1] = (byte)(m_cellNumber + 1);
            // Type of spacing( top, left, bottom, right )
            buf[2] = type;
            buf[3] = 3;
            byte[] arr = BitConverter.GetBytes(dist);
            arr.CopyTo(buf, 4);
            //CopyData( buf, arr );
            SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(options);
            record.ByteArray = buf;

            return record;
        }

        #endregion
    }
}
