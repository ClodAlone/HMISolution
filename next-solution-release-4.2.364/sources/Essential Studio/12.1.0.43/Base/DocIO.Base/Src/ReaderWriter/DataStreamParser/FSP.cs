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

namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for FSP.
    /// </summary>
    internal class FSP : BaseWordRecord
    {
        #region class members
        /// <summary>
        /// shape ID
        /// </summary>
        private uint m_spid;
        /// <summary>
        /// 
        /// </summary>
        private uint m_grfPersistent;
        #endregion

        #region Class Initialize/Finalize Methods
        /// <summary>
        /// 
        /// </summary>
        public FSP()
        {
        }
        #endregion

        #region Class Public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            m_spid = ReadUInt32(stream);
            m_grfPersistent = ReadUInt32(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            WriteUInt32(stream, m_spid);
            WriteUInt32(stream, m_grfPersistent);
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Shape ID
        /// </summary>
        public uint Spid
        {
            get
            {
                return m_spid;
            }
            set
            {
                m_spid = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public uint GzfPersistent
        {
            get
            {
                return m_grfPersistent;
            }
            set
            {
                m_grfPersistent = value;
            }
        }
        /// <summary>
        /// This shape is a group shape
        /// </summary>
        public bool IsGroup
        {
            get
            {
                return (m_grfPersistent & 0x0001) == 1;
            }
        }
        /// <summary>
        /// Not a top-level shape
        /// </summary>
        public bool ISChild
        {
            get
            {
                return (m_grfPersistent & 0x0002) == 1;
            }
        }
        /// <summary>
        /// This is the topmost group shape
        /// </summary>
        public bool IsPatriarch
        {
            get
            {
                return (m_grfPersistent & 0x0004) == 1;
            }
        }
        /// <summary>
        /// The shape has been deleted
        /// </summary>
        public bool IsDeleted
        {
            get
            {
                return (m_grfPersistent & 0x0008) == 1;
            }
        }
        /// <summary>
        ///  The shape is an OLE object
        /// </summary>
        public bool IsOleShape
        {
            get
            {
                return (m_grfPersistent & 0x0010) == 1;
            }
        }
        /// <summary>
        /// Shape has a hspMaster property
        /// </summary>
        public bool IsHaveMaster
        {
            get
            {
                return (m_grfPersistent & 0x0020) == 1;
            }
        }
        /// <summary>
        /// Shape is flipped horizontally
        /// </summary>
        public bool IsFliph
        {
            get
            {
                return (m_grfPersistent & 0x0040) == 1;
            }
        }
        /// <summary>
        /// Shape is flipped vertically
        /// </summary>
        public bool IsFlipv
        {
            get
            {
                return (m_grfPersistent & 0x0080) == 1;
            }
        }
        /// <summary>
        /// Connector type of shape
        /// </summary>
        public bool IsConnector
        {
            get
            {
                return (m_grfPersistent & 0x0100) == 1;
            }
        }
        /// <summary>
        /// Shape has an anchor of some kind
        /// </summary>
        public bool IsHaveAnchor
        {
            get
            {
                return (m_grfPersistent & 0x0200) == 1;
            }
        }
        /// <summary>
        /// Background shape
        /// </summary>
        public bool IsBackground
        {
            get
            {
                return (m_grfPersistent & 0x0400) == 1;
            }
        }
        /// <summary>
        /// Shape has a shape type property
        /// </summary>
        public bool IsHavespt
        {
            get
            {
                return (m_grfPersistent & 0x0800) == 1;
            }
        }
        #endregion

    }
}
