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

using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Group Shape Record msofbtSpgr
    /// This record is present only in group shapes (not shapes in groups, shapes that are groups). 
    /// The group shape record defines the coordinate system of the shape, which the anchors of the 
    /// child shape are expressed in. All other information is stored in the shape records that follow. 
    ///
    ///typedef struct _FSPGR
    ///        {
    ///          RECT   rcgBounds;
    ///        } FSPGR;
    /// </summary>
    internal class MsofbtSpgr : BaseEscherRecord
    {
        #region Class members
        /// <summary>
        /// Boundary of metafile drawing commands
        /// </summary>
        private int m_rectLeft;
        private int m_rectTop;
        private int m_rectRight;
        private int m_rectBottom;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSpgr(WordDocument doc)
            : base(MSOFBT.msofbtSpgr, 1, doc)
        { }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_rectLeft = ReadInt32(stream);
            m_rectTop = ReadInt32(stream);
            m_rectRight = ReadInt32(stream);
            m_rectBottom = ReadInt32(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            WriteInt32(stream, m_rectLeft);
            WriteInt32(stream, m_rectTop);
            WriteInt32(stream, m_rectRight);
            WriteInt32(stream, m_rectBottom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtSpgr spgr = (MsofbtSpgr)this.MemberwiseClone();
            spgr.m_doc = m_doc;
            return spgr;
        }

        #endregion
    }
}
