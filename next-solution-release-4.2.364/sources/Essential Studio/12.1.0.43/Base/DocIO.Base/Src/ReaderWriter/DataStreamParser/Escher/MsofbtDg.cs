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
using System.Text;

using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Drawing Record msofbtDg
    /// The drawing record is very simple, with just a count and MSOSPID seed. The attentive reader may 
    /// expect to find the size of the drawing recorded here, but that information is stored elsewhere 
    /// by the host application. 
    /// </summary>
    internal class MsofbtDg : BaseEscherRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_shapeCount;
        private int m_spidLast;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int DrawingId
        {
            get
            {
                return Header.Instance;
            }
            set
            {
                Header.Instance = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int ShapeCount
        {
            get
            {
                return m_shapeCount;
            }
            set
            {
                m_shapeCount = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int SpidLast
        {
            get
            {
                return m_spidLast;
            }
            set
            {
                m_spidLast = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtDg(WordDocument doc)
            : base(MSOFBT.msofbtDg, 0, doc)
        {
            m_shapeCount = 1;
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_shapeCount = ReadInt32(stream);
            m_spidLast = ReadInt32(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            WriteInt32(stream, m_shapeCount);
            WriteInt32(stream, m_spidLast);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtDg dg = (MsofbtDg)this.MemberwiseClone();
            dg.m_doc = m_doc;
            return dg;
        }

        #endregion
    }
}
