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
    /// Summary description for ParagraphHeightStructure.
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Explicit)]
#endif
    [CLSCompliant(false)]
    internal class ParagraphHeightStructure : DataStructure
    {
        #region Class constants
        private const int DEF_RECORD_SIZE = 12;
        #endregion

        #region Class members
        /// <summary>
        /// Options.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(0)]
#endif
        internal uint Options;
        /// <summary>
        /// Width of lines in paragraph.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(4)]
#endif
        internal int Width;
        /// <summary>
        /// when IsDiffLines is 0, is height of every line in paragraph in pixels
        /// when IsDiffLines is 1, is the total height in pixels of the paragraph
        /// 
        /// If the PHE is stored in a PAP whose fTtp field is set (non-zero),
        /// height of table row
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(8)]
#endif
        internal int Height;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
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
            Options = ReadUInt32(arrData, ref iOffset);
            Width = ReadInt32(arrData, ref iOffset);
            Height = ReadInt32(arrData, ref iOffset);
        }

        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        internal override int Save(byte[] arrData, int iOffset)
        {
            WriteUInt32(arrData, ref iOffset, Options);
            WriteInt32(arrData, ref iOffset, Width);
            WriteInt32(arrData, ref iOffset, Height);

            return DEF_RECORD_SIZE;
        }
        #endregion
    }
}
