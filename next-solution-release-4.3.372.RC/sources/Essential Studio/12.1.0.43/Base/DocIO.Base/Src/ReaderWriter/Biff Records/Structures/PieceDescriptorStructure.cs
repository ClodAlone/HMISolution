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
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Explicit)]
#endif
    internal class PieceDescriptorStructure : DataStructure
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_PRM_OFFSET = 6;
        private const int DEF_RECORD_SIZE = 8;
        #endregion

        #region Class members
        /// <summary>
        /// Options
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(0)]
#endif
        private ushort m_usOptions;

        #region BitField 0
        //[ FieldOffset( 0 ) ]
        ///// <summary>
        ///// when 1, means that piece contains no end of paragraph marks.
        ///// </summary>
        //1//internal short fNoParaLast // 1;
        ///// <summary>
        ///// used internally by Word
        ///// </summary>
        //2//internal short fPaphNil // 2;
        ///// <summary>
        ///// used internally by Word
        ///// </summary>
        //4//internal short fCopied // 4;
        ///// <summary>
        ///// 
        ///// </summary>
        ////internal short * // ;
        #endregion

        #region BitField 1
        //[ FieldOffset( 1 ) ]
        ///// <summary>
        ///// used internally by Word
        ///// </summary>
        //FF00//internal short fn // FF00;
        #endregion

        /// <summary>
        /// file offset of beginning of piece. The size of the ith piece can be determined by subtracting rgcp[i] of the containing plcfpcd from its rgcp[i+1].
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(2)]
#endif
        private uint m_fc;
        /// <summary>
        /// contains either a single Single Property Modifier Record or else an index number of the grpprl which contains the sprms that modify the properties of the piece.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(6)]
#endif
        private PropertyModifierStructure m_prm = new PropertyModifierStructure();
        #endregion

        #region Class Properties
        /// <summary>
        /// Options.
        /// </summary>
        internal ushort Options
        {
            get
            {
                return m_usOptions;
            }
            set
            {
                m_usOptions = value;
            }
        }

        /// <summary>
        /// File offset of beginning of piece.The size of the ith piece can be determined
        /// by subtracting rgcp[i] of the containing plcfpcd from its rgcp[i+1].
        /// </summary>
        internal uint FileOffset
        {
            get
            {
                return m_fc;
            }
            set
            {
                m_fc = value;
            }
        }

        /// <summary>
        /// Contains either a single Single Property Modifier Record or else an index number of the grpprl
        /// which contains the sprms that modify the properties of the piece.
        /// </summary>
        internal PropertyModifierStructure PropertyModifier
        {
            get
            {
                return m_prm;
            }
            set
            {
                m_prm = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return DEF_PRM_OFFSET + m_prm.Length;
            }
        }
        #endregion

        #region Implementation / override
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_usOptions = ReadUInt16(arrData, ref iOffset);
            m_fc = ReadUInt32(arrData, ref iOffset);
            m_prm.Parse(arrData, ref iOffset);
        }

        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">Destination data array</param>
        /// <param name="iOffset">The offset.</param>
        internal override int Save(byte[] arrData, int iOffset)
        {
            WriteUInt16(arrData, ref iOffset, m_usOptions);
            WriteUInt32(arrData, ref iOffset, m_fc);
            m_prm.Save(arrData, ref iOffset);

            return DEF_RECORD_SIZE;
        }
        #endregion
    }
}