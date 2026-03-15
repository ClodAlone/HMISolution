#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for PosStructReader.
    /// </summary>
    internal class PosStructReader
    {
        #region Class constructors
        /// <summary>
        /// 
        /// </summary>
        internal PosStructReader()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="structCount"></param>
        /// <param name="deleg"></param>
        internal static void Read(BinaryReader binaryReader, int structCount, PosStructReaderDelegate deleg)
        {
            int count = structCount + 1;
            int[] positions = new int[count];

            for (int i = 0; i < count; i++)
            {
                positions[i] = binaryReader.ReadInt32();
            }

            for (int j = 0; j < structCount; j++)
            {
                deleg(binaryReader, positions[j], positions[j + 1]);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dataLength"></param>
        /// <param name="structLength"></param>
        /// <param name="deleg"></param>
        internal static void Read(BinaryReader reader, int dataLength, int structLength, PosStructReaderDelegate deleg)
        {
            if (dataLength != 0)
            {
                int count = (int)((dataLength - Constants.BytesInInt) / (Constants.BytesInInt + structLength));
                Read(reader, count, deleg);
            }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal delegate void PosStructReaderDelegate(BinaryReader reader, int pos, int nextPos);
}
