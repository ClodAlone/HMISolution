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
    /// Summary description for FOPTEBase.
    /// </summary>
    internal abstract class FOPTEBase : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_id;
        private bool m_isBid;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsBid
        {
            get
            {
                return m_isBid;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isBid"></param>
        internal FOPTEBase(int id, bool isBid)
        {
            m_id = id;
            m_isBid = isBid;
        }
        #endregion

        #region Class abstract methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal abstract void Write(Stream stream);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal abstract FOPTEBase Clone();
        #endregion
    }
}
