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
using System.IO; 
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    internal class FootnotesRW
      : SubDocumentRW
    {
        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int InitialDescriptorNumber
        {
            get
            {
                return m_iInitialDesctiptorNumber;
            }
            set
            {
                m_iInitialDesctiptorNumber = value;
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal FootnotesRW()
            : base()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        internal FootnotesRW(Stream stream, WPFIBData fib)
            : base(stream, fib)
        {
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="autoNumbered"></param>
        internal void AddReferense(int pos, bool autoNumbered)
        {
            m_refPositions.Add(pos);

            if (autoNumbered)
                m_autoCount++;

            m_descrFootEndntes.Add((short)((autoNumbered) ? m_autoCount : 0));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal int GetDescriptor(int index)
        {
            return m_descrFootEndntes[index];
        }
        #endregion

        #region Class overrides / writes
        /// <summary>
        /// 
        /// </summary>
        protected override void WriteTxtPositions()
        {
            if (m_txtPositions.Count > 0)
            {
                m_fib.fcPlcffndTxt = (int)m_writer.BaseStream.Position;
                WriteTxtPositionsBase();
                m_fib.lcbPlcffndTxt = (int)(m_writer.BaseStream.Position - m_fib.fcPlcffndTxt);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void WriteDescriptors()
        {
            if (m_descrFootEndntes.Count > 0)
            {
                m_fib.fcPlcffndRef = (int)m_writer.BaseStream.Position;
                WriteRefPositions(m_endReference);

                foreach (short entry in m_descrFootEndntes)
                {
                    m_writer.Write(entry);
                }

                m_fib.lcbPlcffndRef = (int)(m_writer.BaseStream.Position - m_fib.fcPlcffndRef);
            }
        }
        #endregion

        #region Class overrides / reads
        /// <summary>
        /// 
        /// </summary>
        protected override void ReadTxtPositions()
        {
            int length = m_fib.lcbPlcffndTxt;

            if (length > 0)
            {
                m_reader.BaseStream.Position = m_fib.fcPlcffndTxt;
                int count = length / 4;
                ReadTxtPositions(count);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void ReadDescriptors()
        {
            if (m_fib.lcbPlcffndRef > 0)
            {
                m_reader.BaseStream.Position = m_fib.fcPlcffndRef;
                byte[] data = new byte[m_fib.lcbPlcffndRef];
                data = m_reader.ReadBytes(m_fib.lcbPlcffndRef);
                m_reader.BaseStream.Position = m_fib.fcPlcffndRef;
                ReadDescriptors(m_fib.lcbPlcffndRef, 2);
                base.ReadDescriptors();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pos"></param>
        /// <param name="posNext"></param>
        protected override void ReadDescriptor(BinaryReader reader, int pos, int posNext)
        {
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                m_descrFootEndntes.Add(reader.ReadInt16());
                base.ReadDescriptor(reader, pos, posNext);
            }
        }
        #endregion
    }
}
