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
using System.IO;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for ContainerCollection.
    /// </summary>
    internal class ContainerCollection : List<Object>
    {
        #region Fields
        private WordDocument m_doc;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal ContainerCollection(WordDocument doc)
        {
            m_doc = doc;
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        internal int Write(Stream stream)
        {
            long startPos = stream.Position;

            foreach (BaseEscherRecord container in this)
            {
                container.WriteMsofbhWithRecord(stream);
            }

            return (int)(stream.Position - startPos);
        }
        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="length">The length.</param>
        internal void Read(Stream stream, int length)
        {
            long endPos = stream.Position + length;
            while (stream.Position < endPos && stream.Position < stream.Length)
            {
                BaseEscherRecord record = _MSOFBH.ReadHeaderWithRecord(stream, m_doc);

                if (record != null)
                {
                    Add(record);
                }
            }
        }
        #endregion
    }
}
