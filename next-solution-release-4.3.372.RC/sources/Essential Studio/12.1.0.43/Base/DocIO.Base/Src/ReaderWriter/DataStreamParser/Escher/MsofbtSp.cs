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
    /// The instance field of the record header contains the shape type; 
    /// the record itself contains the shape ID and a group of persistent flags.
    /// </summary>
    internal class MsofbtSp : BaseEscherRecord
    {
        //    typedef struct
        //    {
        //      ULONG fGroup : 1;        // This shape is a group shape
        //      ULONG fChild : 1;        // Not a top-level shape
        //      ULONG fPatriarch : 1;    // This is the topmost group shape.
        //      // Exactly one of these per drawing. 
        //      ULONG fDeleted : 1;      // The shape has been deleted
        //      ULONG fOleShape : 1;     // The shape is an OLE object
        //      ULONG fHaveMaster : 1;   // Shape has a hspMaster property
        //      ULONG fFlipH : 1;        // Shape is flipped horizontally
        //      ULONG fFlipV : 1;        // Shape is flipped vertically
        //      ULONG fConnector : 1;    // Connector type of shape
        //      ULONG fHaveAnchor : 1;   // Shape has an anchor of some kind
        //      ULONG fBackground : 1;   // Background shape
        //      ULONG fHaveSpt : 1;      // Shape has a shape type property
        //      ULONG reserved : 20;     // Not yet used
        //    }

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_shapeId;
        private int m_shapeFlags;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal bool IsGroup
        {
            get
            {
                //        return ( ( m_shapeFlags & 1 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 1, 0) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 1, 0, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsChild
        {
            get
            {
                //        return ( ( m_shapeFlags & 2 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 2, 1) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0002, 1, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsPatriarch
        {
            get
            {
                //        return ( ( m_shapeFlags & 4 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 4, 2) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 4, 2, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsDeleted
        {
            get
            {
                //        return ( ( m_shapeFlags & 8 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 8, 3) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 8, 3, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsOle
        {
            get
            {
                //        return ( ( m_shapeFlags & 0x0010 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 0x0010, 4) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0010, 4, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool HasMaster
        {
            get
            {
                //        return ( ( m_shapeFlags & 0x0020 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 0x0020, 5) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0020, 5, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsFlippedHor
        {
            get
            {
                //        return ( ( m_shapeFlags & 0x0040 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 0x0040, 6) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0040, 6, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsFlippedVert
        {
            get
            {
                //        return ( ( m_shapeFlags & 0x0080 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 0x0080, 7) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0080, 7, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsConnector
        {
            get
            {
                //        return ( ( m_shapeFlags & 0x0100 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 0x0100, 8) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0100, 8, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool HasAnchor
        {
            get
            {
                //        return ( ( m_shapeFlags & 0x0200 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 0x0200, 9) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0200, 9, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsBackground
        {
            get
            {
                //        return ( ( m_shapeFlags & 0x0400 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 0x0400, 10) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0400, 10, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool HasShapeTypeProperty
        {
            get
            {
                //        return ( ( m_shapeFlags & 0x0800 ) == 1 );
                return (GetBitsByMask(m_shapeFlags, 0x0800, 11) == 1);
            }
            set
            {
                m_shapeFlags = SetBitsByMask(m_shapeFlags, 0x0800, 11, (value) ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int ShapeId
        {
            get
            {
                return m_shapeId;
            }
            set
            {
                m_shapeId = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal EscherShapeType ShapeType
        {
            get
            {
                return (EscherShapeType)Header.Instance;
            }
            set
            {
                Header.Instance = (int)value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSp(WordDocument doc)
            : base(MSOFBT.msofbtSp, 2, doc)
        { }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_shapeId = ReadInt32(stream);
            m_shapeFlags = ReadInt32(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            WriteInt32(stream, m_shapeId);
            WriteInt32(stream, m_shapeFlags);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtSp shape = (MsofbtSp)this.MemberwiseClone();
            shape.m_doc = m_doc;
            return shape;
        }

        //	  /// <summary>
        //	  /// 
        //	  /// </summary>
        //	  /// <returns></returns>
        //    public override string ToString()
        //    {
        //      StringBuilder builder1 = new StringBuilder();
        //      builder1.AppendFormat("{0}\n", base.ToString());
        //      builder1.AppendFormat("ShapeId:{0:X}, ShapeFlags:{1:X}\n", m_shapeId, m_shapeFlags);
        //      return builder1.ToString();
        //    }
        #endregion

    }
}
