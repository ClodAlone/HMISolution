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
using System.Text;
using System.Diagnostics;
using System.IO;

using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for AnnotationReferenceDescriptor.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class AnnotationDescriptor
    {
        #region Class constants
        internal const int DEF_LENGTH = 30;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private string m_xstUsrInitl;
        /// <summary>
        /// Index into GrpXstAtnOwners
        /// </summary>
        private short m_ibst;
        /// <summary>
        /// Unused
        /// </summary>
        private short m_ak;
        /// <summary>
        /// Unused
        /// </summary>
        private short m_grfbmc;
        /// <summary>
        /// when not -1, this tag identifies the annotation bookmark that 
        /// locates the range of CPs in the main document which this annotation references.
        /// </summary>
        private int m_lTagBkmk;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal short IndexToGrpOwner
        {
            get
            {
                return m_ibst;
            }
            set
            {
                m_ibst = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string UserInitials
        {
            get
            {
                return m_xstUsrInitl;
            }
            set
            {
                m_xstUsrInitl = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short Ak
        {
            get
            {
                return m_ak;
            }
            set
            {
                m_ak = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short Grfbmc
        {
            get
            {
                return m_grfbmc;
            }
            set
            {
                m_grfbmc = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int TagBkmk
        {
            get
            {
                return m_lTagBkmk;
            }
            set
            {
                m_lTagBkmk = value;
            }
        }
        #endregion

        #region Class Initialise / Finalise methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal AnnotationDescriptor(BinaryReader reader)
        {
            Read(reader);
        }
        /// <summary>
        /// 
        /// </summary>
        internal AnnotationDescriptor()
        {
            TagBkmk = -1;
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal void Read(BinaryReader reader)
        {
            byte[] userInit = reader.ReadBytes(20);
#if SILVERLIGHT || WP
			m_xstUsrInitl = Encoding.Unicode.GetString(userInit, 0, userInit.Length).Substring(1, userInit[0]);
#else
            m_xstUsrInitl = Encoding.Unicode.GetString(userInit).Substring(1, userInit[0]);
#endif
            m_ibst = reader.ReadInt16();
            m_ak = reader.ReadInt16();
            m_grfbmc = reader.ReadInt16();
            m_lTagBkmk = reader.ReadInt32();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>   
        internal void Write(BinaryWriter writer)
        {
            string str = string.Empty;
            if (m_xstUsrInitl.Length > 9)
            {
                str = '9' + m_xstUsrInitl.Substring(0, 9);
            }
            else
            {
                str = (char)m_xstUsrInitl.Length + m_xstUsrInitl;
            }


            if (str.Length < 10)
            {
                for (int i = 0, len = 10 - str.Length; i < len; i++)
                {
                    str += (char)0;
                }
            }

            byte[] userInit = Encoding.Unicode.GetBytes(str);
            writer.Write(userInit, 0, userInit.Length);
            writer.Write(m_ibst);
            writer.Write(m_ak);
            writer.Write(m_grfbmc);
            writer.Write(m_lTagBkmk);
        }
        #endregion
    }
}
