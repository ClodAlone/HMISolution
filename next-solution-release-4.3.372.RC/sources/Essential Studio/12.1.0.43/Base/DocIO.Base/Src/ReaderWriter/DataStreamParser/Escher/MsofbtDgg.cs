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
using System.Text;

using Syncfusion.DocIO.ReaderWriter.Escher;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// The drawing group record is a variable length record consisting of a fixed part followed by an array.
    /// </summary>
    internal class MsofbtDgg : BaseEscherRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_spidMax;
        private int m_shapeCount;
        private int m_drawingCount;
        private List<FIDCL> m_filCls;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int DrawingCount
        {
            get
            {
                return m_drawingCount;
            }
            set
            {
                m_drawingCount = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<FIDCL> Fidcls
        {
            get
            {
                return m_filCls;
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
        internal int SpidMax
        {
            get
            {
                return m_spidMax;
            }
            set
            {
                m_spidMax = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtDgg(WordDocument doc)
            : base(MSOFBT.msofbtDgg, 0, doc)
        {
            m_filCls = new List<FIDCL>();
            /////////////////
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
            m_spidMax = ReadInt32(stream);
            int fidclCount = ReadInt32(stream) - 1;
            m_shapeCount = ReadInt32(stream);
            m_drawingCount = ReadInt32(stream);
            for (int i = 0; i < fidclCount; i++)
            {
                m_filCls.Add(new FIDCL(stream));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            WriteInt32(stream, m_spidMax);
            WriteInt32(stream, m_filCls.Count + 1);
            WriteInt32(stream, m_shapeCount);
            WriteInt32(stream, m_drawingCount);
            for (int i = 0; i < m_filCls.Count; i++)
            {
                FIDCL fidcl = m_filCls[i];
                fidcl.Write(stream);
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtDgg dgg = (MsofbtDgg)this.MemberwiseClone();
            dgg.m_filCls = new List<FIDCL>(m_filCls.Count);
            for (int i = 0, cnt = m_filCls.Count; i < cnt; i++)
            {
                dgg.m_filCls.Add(m_filCls[i]);
            }
            dgg.m_doc = m_doc;
            return dgg;
        }

        #endregion
    }
}
