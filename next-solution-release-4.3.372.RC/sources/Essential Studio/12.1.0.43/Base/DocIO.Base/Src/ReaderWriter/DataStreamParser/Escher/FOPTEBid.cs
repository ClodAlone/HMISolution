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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for FOPTEBid.
    /// </summary>
    internal class FOPTEBid : FOPTEBase
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private uint m_value;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal uint Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isBid"></param>
        /// <param name="value"></param>
        internal FOPTEBid(int id, bool isBid, uint value)
            : base(id, isBid)
        {
            m_value = value;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal override void Write(Stream stream)
        {
            int tmp = Id;
            tmp |= (IsBid ? 0x4000 : 0);
            WriteInt16(stream, (short)tmp);
            WriteUInt32(stream, m_value);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override FOPTEBase Clone()
        {
            FOPTEBid newFopte = new FOPTEBid(this.Id, this.IsBid, this.Value);
            return newFopte;
        }
        #endregion
    }
}
