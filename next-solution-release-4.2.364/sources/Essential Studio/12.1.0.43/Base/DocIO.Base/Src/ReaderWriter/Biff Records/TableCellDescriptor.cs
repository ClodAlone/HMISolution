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

#region File using directives
using System;

using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.DLS;

#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for TableCellDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TableCellDescriptor : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        internal TableCellStructure m_tableStruct = new TableCellStructure();
        /// <summary>
        /// 
        /// </summary>
        private uint m_topBorderColorExt = 0xff000000;
        private uint m_leftBorderColorExt = 0xff000000;
        private uint m_bottomBorderColorExt = 0xff000000;
        private uint m_rightBorderColorExt = 0xff000000;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCellDescriptor"/> class.
        /// </summary>
        /// <param name="arr">The arr.</param>
        /// <param name="iOffset">The i offset.</param>
        internal TableCellDescriptor(byte[] arr, int iOffset)
        {
            Parse(arr, iOffset, m_tableStruct.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        internal TableCellDescriptor()
        {
        }
        #endregion

        #region Class override methods
        /// <summary>
        /// Parse byte array to m_tableStruct
        /// </summary>
        /// <param name="arr">The arr.</param>
        /// <param name="iOffset">The i offset.</param>
        internal override void Parse(byte[] arr, int iOffset)
        {
            if (arr.Length < m_tableStruct.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            base.Parse(arr, iOffset, m_tableStruct.Length);
        }
        /// <summary>
        /// Returns array of bytes from m_tableStruct
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            int iCount = base.Save(arrData, iOffset);
            return iCount;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override DataStructure UnderlyingStructure
        {
            get
            {
                return m_tableStruct;
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets cell text direction
        /// </summary>
        internal TextDirection TextDirection
        {
            get
            {
                return GetTextDirection();
            }
            set
            {
                SetTextDirection(value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool FirstMerged
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0001);
                return (val == 1);
            }
            set
            {
                ushort res = (value == true) ? (ushort)1 : (ushort)0;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xFFFE);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool Merged
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0002);
                val = (ushort)(val >> 1);
                return (val == 1);
            }
            set
            {
                ushort res = (value == true) ? (ushort)1 : (ushort)0;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xFFFD);
                res = (ushort)(res << 1);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool Vertical
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0004);
                val = (ushort)(val >> 2);
                return (val == 1);
            }
            set
            {
                ushort res = (value == true) ? (ushort)1 : (ushort)0;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xFFFB);
                res = (ushort)(res << 2);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool Backward
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0008);
                val = (ushort)(val >> 3);
                return (val == 1);
            }
            set
            {
                ushort res = (value == true) ? (ushort)1 : (ushort)0;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xFFF7);
                res = (ushort)(res << 3);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool RotateFont
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0010);
                val = (ushort)(val >> 4);
                return (val == 1);
            }
            set
            {
                ushort res = (value == true) ? (ushort)1 : (ushort)0;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xFFEF);
                res = (ushort)(res << 4);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool VertMerge
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0020);
                val = (ushort)(val >> 5);
                return (val == 1);
            }
            set
            {
                ushort res = (value == true) ? (ushort)1 : (ushort)0;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xFFDF);
                res = (ushort)(res << 5);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool VertRestart
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0040);
                val = (ushort)(val >> 6);
                return (val == 1);
            }
            set
            {
                ushort res = (value == true) ? (ushort)1 : (ushort)0;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xFFBF);
                res = (ushort)(res << 6);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte VertAllign
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0180);
                val = (ushort)(val >> 7);
                return (byte)val;
            }
            set
            {
                ushort res = value;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xFE7F);
                res = (ushort)(res << 7);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte WidthUnit
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x0E00);
                val = (ushort)(val >> 9);
                return (byte)val;
            }
            set
            {
                ushort res = value;
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xF1FF);
                res = (ushort)(res << 9);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// Get/set table cell fit text property
        /// </summary>
        internal bool FitText
        {
            get
            {
                ushort val = (ushort)(m_tableStruct.RGF & 0x1000);
                val = (ushort)(val >> 12);
                return (val == 1);
            }
            set
            {
                ushort res = (ushort)((value) ? 1 : 0);
                m_tableStruct.RGF = (ushort)(m_tableStruct.RGF & 0xEFFF);
                res = (ushort)(res << 12);
                m_tableStruct.RGF += res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderStructure BRCTop
        {
            get
            {
                return m_tableStruct.BRCTop;
            }
            set
            {
                m_tableStruct.BRCTop = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderStructure BRCLeft
        {
            get
            {
                return m_tableStruct.BRCLeft;
            }
            set
            {
                m_tableStruct.BRCLeft = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderStructure BRCBottom
        {
            get
            {
                return m_tableStruct.BRCBottom;
            }
            set
            {
                m_tableStruct.BRCBottom = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderStructure BRCRight
        {
            get
            {
                return m_tableStruct.BRCRight;
            }
            set
            {
                m_tableStruct.BRCRight = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint TopBorderColorExt
        {
            get
            {
                return m_topBorderColorExt;
            }
            set
            {
                m_topBorderColorExt = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint LeftBorderColorExt
        {
            get
            {
                return m_leftBorderColorExt;
            }
            set
            {
                m_leftBorderColorExt = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint BottomBorderColorExt
        {
            get
            {
                return m_bottomBorderColorExt;
            }
            set
            {
                m_bottomBorderColorExt = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint RightBorderColorExt
        {
            get
            {
                return m_rightBorderColorExt;
            }
            set
            {
                m_rightBorderColorExt = value;
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Clones the TableCellDescriptor instance.
        /// </summary>
        /// <returns></returns>
        internal TableCellDescriptor Clone()
        {
            TableCellDescriptor cellDescriptor = new TableCellDescriptor();
            cellDescriptor.m_bottomBorderColorExt = m_bottomBorderColorExt;
            cellDescriptor.m_leftBorderColorExt = m_leftBorderColorExt;
            cellDescriptor.m_rightBorderColorExt = m_rightBorderColorExt;
            cellDescriptor.m_topBorderColorExt = m_topBorderColorExt;
            cellDescriptor.m_tableStruct = m_tableStruct.Clone();
            return cellDescriptor;
        }
        /// <summary>
        /// Sets the text direction.
        /// </summary>
        /// <param name="textDirection">The text direction.</param>
        private void SetTextDirection(TextDirection textDirection)
        {
            switch (textDirection)
            {
                case TextDirection.VerticalBottomToTop:
                    Vertical = true;
                    Backward = true;
                    break;
                case TextDirection.VerticalTopToBottom:
                    Vertical = true;
                    Backward = false;
                    break;
                default:
                    Vertical = false;
                    Backward = false;
                    break;
            }
        }
        /// <summary>
        /// Gets text direction.
        /// </summary>
        /// <returns></returns>
        /// <value>The get text direction.</value>
        private TextDirection GetTextDirection()
        {
            if (!Vertical)
            {
                return TextDirection.Horizontal;
            }
            else if (Vertical && Backward)
            {
                return TextDirection.VerticalBottomToTop;
            }
            else
            {
                return TextDirection.VerticalTopToBottom;
            }
        }
        #endregion
    }
}
