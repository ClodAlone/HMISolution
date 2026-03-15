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

using System;
using System.Collections;
using System.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

namespace Syncfusion.Pdf.IO
{
    /// <property name="flag" value="Finished" />
    ///
    /// <summary>
    /// Implements functionality of PDF object stream (or PDF
    /// archive).
    /// </summary>
#if NETFX_CORE || WP
    public class PdfArchiveStream : PdfStream
#else
    internal class PdfArchiveStream : PdfStream
#endif
    {
        #region Fields
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Holds sorted indices.
        /// </summary>
        private SortedListEx m_indices;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Holds the objects.
        /// </summary>
        private MemoryStream m_objects;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// PDF stream writer.
        /// </summary>
        private StreamWriter m_writer;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// PDF stream writer.
        /// </summary>
        private IPdfWriter m_objectWriter;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// The current document.
        /// </summary>
        private PdfDocumentBase m_document;
        #endregion

        #region Properties
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Gets the count of objects.
        /// </summary>
        internal int ObjCount
        {
            get
            {
                return m_indices.Count;
            }
        }
        #endregion

        #region Initialize / Finalize
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PDFArchiveStream" />
        /// class.
        /// </summary>
        /// <param name="document">The document.</param>
        internal PdfArchiveStream(PdfDocumentBase document)
            : base()
        {
            if (document == null)
                throw new ArgumentNullException("document");

            m_document = document;
            m_objects = new MemoryStream(1000);
            m_objectWriter = new PdfWriter(m_objects);
            m_objectWriter.Document = m_document;
            m_indices = new SortedListEx(16);
        }
        #endregion

        #region Public methods
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Saves the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="reference">The reference.</param>
        public void SaveObject(IPdfPrimitive obj, PdfReference reference)
        {
            long position = m_objectWriter.Position;
            m_indices[position] = reference.ObjNum;

            bool state;
#if !SILVERLIGHT && !NETFX_CORE && !WP
            PdfSecurity sec = m_document.Security;
            state = sec.Enabled;
            sec.Enabled = false;
#endif

            obj.Save(m_objectWriter);

#if !SILVERLIGHT && !NETFX_CORE && !WP
            sec.Enabled = state;
#endif

            m_objectWriter.Write(Operators.NewLine);
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Gets the index of the object referenced by its number.
        /// </summary>
        /// <param name="objNum">The object number.</param>
        /// <returns>
        /// The index of the object.
        /// </returns>
        public int GetIndex(long objNum)
        {
            return m_indices.IndexOfValue(objNum);
        }
        #endregion

        #region Overloads
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Writes object to defined writer.
        /// </summary>
        /// <param name="writer">Writer for object saving.</param>
        public override void Save(IPdfWriter writer)
        {
            using (MemoryStream data = new MemoryStream((int)m_objects.Length + 100))
            using (m_writer = new StreamWriter(data))
            {
                SaveIndices();
                m_writer.Flush();
                this[DictionaryProperties.First] = new PdfNumber(m_writer.BaseStream.Position);
                SaveObjects();
                m_writer.Flush();
                Data = data.ToArray();
            }
            this[DictionaryProperties.N] = new PdfNumber(m_indices.Count);
            this[DictionaryProperties.Type] = new PdfName("ObjStm");

            //// NOTE: Debug.
            //NeedCompression = false;
            //BlockEncryption();

            base.Save(writer);
        }

        /// <summary>
        /// Clears the PdfArchiveStream.
        /// </summary>
        internal void Clear()
        {
            m_indices.Clear();

#if !NETFX_CORE && !WP
            if (m_objects != null)
                m_objects.Close();

            if (m_writer != null)
                m_writer.Close();
# endif

            if (m_objectWriter != null)
                m_objectWriter = null;

            base.Clear();
        }
        #endregion

        #region Helper Methods
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Saves objects.
        /// </summary>
        private void SaveObjects()
        {
            byte[] buffer = m_objects.ToArray();
            m_writer.BaseStream.Write(buffer, 0, buffer.Length);
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Saves indices.
        /// </summary>
        private void SaveIndices()
        {
            foreach (long position in m_indices.Keys)
            {
                m_writer.Write(m_indices[position]);
                m_writer.Write(Operators.WhiteSpace);
                m_writer.Write(position);
                m_writer.Write(Operators.NewLine);
            }
        }

        #endregion

        #region Internals
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Stores information about current object and index.
        /// </summary>
        private class ObjInfo
        {
            #region Fields
            /// <property name="flag" value="Finished" />
            ///
            /// <summary>
            /// Current object.
            /// </summary>
            internal IPdfPrimitive Obj;
            /// <property name="flag" value="Finished" />
            ///
            /// <summary>
            /// The current index within the object.
            /// </summary>
            internal int Index;
            #endregion

            #region Initialize / Finalize
            /// <property name="flag" value="Finished" />
            ///
            /// <summary>
            /// Initializes a new instance of the <see cref="T:ObjInfo" />
            /// class.
            /// </summary>
            /// <param name="obj">The object.</param>
            internal ObjInfo(IPdfPrimitive obj)
            {
                Obj = obj;
                Index = 0;
            }
            #endregion
        }
        #endregion
    }
}
