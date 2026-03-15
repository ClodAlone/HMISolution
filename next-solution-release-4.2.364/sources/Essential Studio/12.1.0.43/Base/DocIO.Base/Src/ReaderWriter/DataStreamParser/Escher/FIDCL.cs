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
using System.IO;

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// The ID clusters are used internally for the translation of shape ids (SPIDs) 
    /// to shape handles (MSOHSPs). 
    /// </summary>
    internal class FIDCL : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        internal int m_dgid;
        internal int m_cspidCur;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal FIDCL(Stream stream)
        {
            Read(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dgid"></param>
        /// <param name="cspidCur"></param>
        internal FIDCL(int dgid, int cspidCur)
        {
            m_dgid = dgid;
            m_cspidCur = cspidCur;
        }
        #endregion

        #region Class internal methods
        ///<summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            WriteInt32(stream, m_dgid);
            WriteInt32(stream, m_cspidCur);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            m_dgid = ReadInt32(stream);
            m_cspidCur = ReadInt32(stream);
        }
        #endregion
    }
}
