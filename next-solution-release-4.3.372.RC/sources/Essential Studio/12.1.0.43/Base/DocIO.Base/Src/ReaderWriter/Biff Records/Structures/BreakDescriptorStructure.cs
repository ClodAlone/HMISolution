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
    /// BreaK Descriptor (BKD).
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Explicit)]
#endif
    [CLSCompliant(false)]
    internal class BreakDescriptorStructure : DataStructure
    {
        #region Constants
        private const int DEF_RECORD_SIZE = 6;
        #endregion

        #region Fields
        /// <summary>
        /// except in textbox BKD, index to PGD in plfpgd that describes the page this break is on.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(0)]
#endif
        internal short ipgd;
        /// <summary>
        /// in textbox BKD,
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(0)]
#endif
        internal short itxbxs;
        /// <summary>
        /// number of cp's considered for this break; note that the CP's described by cpDepend in this break reside in the next BKD
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(2)]
#endif
        internal short dcpDepend;
        /// <summary>
        /// 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(4)]
#endif
        internal byte iCol;
        /// <summary>
        /// Option flags.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(5)]
#endif
        internal byte Options;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the size of the structure.
        /// </summary>
        /// <value>The length.</value>
        internal override int Length
        {
            get
            {
                return DEF_RECORD_SIZE;
            }
        }
        #endregion

        #region Implementation / overrides
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            itxbxs = ReadInt16(arrData, ref iOffset);
            dcpDepend = ReadInt16(arrData, ref iOffset);

            iCol = arrData[iOffset];
            iOffset += 1;
            Options = arrData[iOffset];
            iOffset += 1;
        }

        /// <summary>
        /// Saves the specified arr.
        /// </summary>
        /// <param name="arr">The arr.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns></returns>
        internal override int Save(byte[] arr, int iOffset)
        {
            WriteInt16(arr, ref iOffset, itxbxs);
            WriteInt16(arr, ref iOffset, dcpDepend);

            arr[iOffset] = iCol;
            iOffset += 1;
            arr[iOffset] = Options;
            iOffset += 1;

            return DEF_RECORD_SIZE;
        }
        #endregion
    }
}