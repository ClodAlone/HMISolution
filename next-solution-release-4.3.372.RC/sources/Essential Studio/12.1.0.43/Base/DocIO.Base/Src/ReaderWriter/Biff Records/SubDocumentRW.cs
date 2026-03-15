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
using System.Collections;

using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for SubDocumentRW.
    /// </summary>
    [CLSCompliant(false)]
    internal abstract class SubDocumentRW
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected WPFIBData m_fib;
        /// <summary>
        /// Text position table.
        /// </summary>
        protected List<Int32> m_txtPositions;
        /// <summary>
        /// Referense position table.
        /// </summary>
        protected List<Int32> m_refPositions;
        /// <summary>
        /// 
        /// </summary>
        protected List<AnnotationDescriptor> m_descriptorsAnnot;
        /// <summary>
        /// 
        /// </summary>
        protected List<Int16> m_descrFootEndntes;
        /// <summary>
        /// 
        /// </summary>
        protected BinaryReader m_reader;
        /// <summary>
        /// 
        /// </summary>
        protected BinaryWriter m_writer;
        /// <summary>
        /// 
        /// </summary>
        private int m_endRefPosition = -1;
        /// <summary>
        /// 
        /// </summary>
        protected int m_iCount;
        /// <summary>
        /// 
        /// </summary>
        protected int m_iInitialDesctiptorNumber = 0;
        /// <summary>
        /// 
        /// </summary>
        protected int m_autoCount = 0;
        /// <summary>
        /// 
        /// </summary>
        protected int m_endReference;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        internal int Count
        {
            get
            {
                return m_iCount;
            }
        }
        #endregion

        #region Class Initilize / Finalize mehods
        /// <summary>
        /// Initializes a new instance of the <see cref="SubDocumentRW"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fib">The fib.</param>
        internal SubDocumentRW(Stream stream, WPFIBData fib)
            : this()
        {
            Read(stream, fib);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SubDocumentRW"/> class.
        /// </summary>
        internal SubDocumentRW()
        {
            Init();
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Determines whether the specified reference has reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        /// 	<c>true</c> if the specified reference has reference; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasReference(int reference)
        {
            return m_refPositions.Contains(reference);
        }
        /// <summary>
        /// Determines whether the specified position has position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>
        /// 	<c>true</c> if the specified position has position; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasPosition(int position)
        {
            return m_txtPositions.Contains(position);
        }
        /// <summary>
        /// Reads.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fib">The fib.</param>
        internal virtual void Read(Stream stream, WPFIBData fib)
        {
            m_fib = fib;
            m_reader = new BinaryReader(stream);
            ReadTxtPositions();
            ReadDescriptors();
        }
        /// <summary>
        /// Writes.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fib">The fib.</param>
        internal virtual void Write(Stream stream, WPFIBData fib)
        {
            m_fib = fib;
            m_writer = new BinaryWriter(stream);
            m_endReference = m_fib.ccpText + m_fib.ccpFtn
                + m_fib.ccpHdr + m_fib.ccpAtn
                + m_fib.ccpEdn + m_fib.ccpTxbx + m_fib.ccpHdrTxbx;
            WriteTxtPositions();
            WriteDescriptors();
        }
        /// <summary>
        /// Adds the text position.
        /// </summary>
        /// <param name="position">The position.</param>
        internal virtual void AddTxtPosition(int position)
        {
            m_txtPositions.Add(position);
        }
        /// <summary>
        /// Gets the text position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal virtual int GetTxtPosition(int index)
        {
            return (m_txtPositions.Count == 0) ? 0 : m_txtPositions[index];
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Reads the descriptors.
        /// </summary>
        protected virtual void ReadDescriptors()
        {
            if (m_endRefPosition != -1)
            {
                AddRefPosition(m_endRefPosition);
            }
        }
        /// <summary>
        /// Writes the descriptors.
        /// </summary>
        protected abstract void WriteDescriptors();
        /// <summary>
        /// Reads the descriptors.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="size">The size.</param>
        protected void ReadDescriptors(int length, int size)
        {
            PosStructReader.Read(m_reader, length, size, new PosStructReaderDelegate(ReadDescriptor));
        }
        /// <summary>
        /// Adds the reference position.
        /// </summary>
        /// <param name="position">The position.</param>
        protected void AddRefPosition(int position)
        {
            m_refPositions.Add(position);
        }
        /// <summary>
        /// Inits this instance.
        /// </summary>
        protected virtual void Init()
        {
            m_txtPositions = new List<Int32>();
            m_refPositions = new List<Int32>();
            m_descriptorsAnnot = new List<AnnotationDescriptor>();
            m_descrFootEndntes = new List<Int16>();
        }
        /// <summary>
        /// Reads the text positions.
        /// </summary>
        protected abstract void ReadTxtPositions();
        /// <summary>
        /// Reads the text positions.
        /// </summary>
        /// <param name="count">The count.</param>
        protected void ReadTxtPositions(int count)
        {
            m_iCount = count - 1;

            for (int i = 0; i < count; i++)
            {
                AddTxtPosition(m_reader.ReadInt32());
            }
        }
        /// <summary>
        /// Writes the text positions base.
        /// </summary>
        protected void WriteTxtPositionsBase()
        {
            foreach (int entry in m_txtPositions)
            {
                m_writer.Write(entry);
            }
        }
        /// <summary>
        /// Writes the text positions.
        /// </summary>
        protected abstract void WriteTxtPositions();
        /// <summary>
        /// Writes the reference positions.
        /// </summary>
        /// <param name="endPos">The end pos.</param>
        protected virtual void WriteRefPositions(int endPos)
        {
            foreach (int entry in m_refPositions)
            {
                m_writer.Write(entry);
            }
            // write end position
            if (m_refPositions.Count > 0)
            {
                m_writer.Write(endPos);
            }
        }
        /// <summary>
        /// Reads the descriptor.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="posNext">The pos next.</param>
        protected virtual void ReadDescriptor(BinaryReader reader, int pos, int posNext)
        {
            AddRefPosition(pos);
            m_endRefPosition = posNext;
        }
        #endregion
    }
}
