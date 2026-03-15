#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


#region file using directives
using System;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for TabsDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    internal class TabsInfo
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private short[] m_tabPositions = new short[0];
        /// <summary>
        /// 
        /// </summary>
        private short[] m_tabDeletePositions = new short[0];
        /// <summary>
        /// 
        /// </summary>
        private TabDescriptor[] m_tabDescriptors = new TabDescriptor[0];
        /// <summary>
        /// 
        /// </summary>
        private byte m_tabsCount = 0;
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_data;
        /// <summary>
        /// 
        /// </summary>
        private byte m_opcode;
        /// <summary>
        /// 
        /// </summary>
        private int m_deleteOffset;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the tab count.
        /// </summary>
        /// <value>The tab count.</value>
        internal byte TabCount
        {
            get
            {
                return m_tabsCount;
            }
        }
        /// <summary>
        /// Gets or sets the tab positions.
        /// </summary>
        /// <value>The tab positions.</value>
        internal short[] TabPositions
        {
            get
            {
                return m_tabPositions;
            }
        }
        /// <summary>
        /// Gets or sets the tab delete positions.
        /// </summary>
        /// <value>The tab delete positions.</value>
        internal short[] TabDeletePositions
        {
            get
            {
                return m_tabDeletePositions;
            }
            set
            {
                m_tabDeletePositions = value;
            }
        }
        /// <summary>
        /// Gets or sets the descriptors.
        /// </summary>
        /// <value>The descriptors.</value>
        internal TabDescriptor[] Descriptors
        {
            get
            {
                return m_tabDescriptors;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TabsInfo"/> class.
        /// </summary>
        internal TabsInfo(byte length)
        {
            m_tabsCount = length;
            m_tabPositions = new short[length];
            m_tabDescriptors = new TabDescriptor[length];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="sprm"></param>
        internal TabsInfo(SinglePropertyModifierArray sprms, int sprm)
        {
            Parse(sprms[sprm]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        internal TabsInfo(SinglePropertyModifierRecord record)
        {
            Parse(record);
        }
        #endregion

        #region Class internal Methods
        /// <summary>
        /// Saves the specified SPRMS.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        /// <param name="sprmOption">The SPRM option.</param>
        internal void Save(SinglePropertyModifierArray sprms, int sprmOption)
        {
            bool savePositions = SaveInit();

            if (m_data == null)
                return;

            SaveDeletePositions();

            if (savePositions)
            {
                SavePositions();
                SaveDescriptors();
            }

            sprms.SetValue(sprmOption, m_data);
        }
        #endregion

        #region Class helper methods
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="sprm"></param>
        //    private void Parse( SinglePropertyModifierArray sprms, WordSprmOptions sprm )
        //    {
        //      m_data = sprms.GetByteArray( sprm );
        //
        //      if( m_data == null )
        //        return;
        //
        //      ParseInit();
        //      ParseDeletePositions();
        //      ParsePositions();
        //      ParseDescriptors();
        //    }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        private void Parse(SinglePropertyModifierRecord record)
        {
            m_data = record.ByteArray;

            if (m_data == null)
                return;

            ParseInit();
            ParseDeletePositions();
            ParsePositions();
            ParseDescriptors();
        }
        /// <summary>
        /// 
        /// </summary>
        private void ParseInit()
        {
            m_opcode = m_data[0];
            m_deleteOffset = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        private void ParseDeletePositions()
        {
            if (m_opcode > 0)
            {
                // fill tab delete positions
                m_deleteOffset = m_opcode * Constants.BytesInWord;
                m_tabDeletePositions = new short[m_opcode];

                if (m_data.Length > m_deleteOffset)
                {
                    Buffer.BlockCopy(m_data, 1, m_tabDeletePositions, 0, m_opcode * Constants.BytesInWord);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ParsePositions()
        {
            if (m_data.Length > m_deleteOffset + 1)
            {
                m_tabsCount = m_data[m_deleteOffset + 1];
                m_tabPositions = new short[m_tabsCount];
                m_tabDescriptors = new TabDescriptor[m_tabsCount];
            }

            if (m_data.Length > m_deleteOffset + m_tabsCount * Constants.BytesInWord + 1)
            {
                Buffer.BlockCopy(m_data, m_deleteOffset + Constants.BytesInWord, m_tabPositions, 0, m_tabsCount * Constants.BytesInWord);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ParseDescriptors()
        {
            if (m_data.Length > m_deleteOffset + (m_tabsCount + 1) * Constants.BytesInWord)
            {
                int offset = (m_tabsCount + 1) * Constants.BytesInWord;

                for (int i = 0; i < m_tabsCount; i++)
                {
                    m_tabDescriptors[i] = new TabDescriptor(m_data[offset + m_deleteOffset]);
                    offset++;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private bool SaveInit()
        {
            m_opcode = (byte)((m_tabDeletePositions != null) ? m_tabDeletePositions.Length : 0);
            m_deleteOffset = m_opcode * Constants.BytesInWord;
            int dataLen = (m_opcode + 1) * Constants.BytesInWord;

            dataLen += m_tabsCount * (Constants.BytesInWord + TabDescriptor.DEF_TAB_LENGTH);

            if (dataLen == 0)
                return false;

            m_data = new byte[dataLen];

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void SaveDeletePositions()
        {
            if (m_data.Length > m_deleteOffset && m_opcode > 0)
            {
                // save tab delete positions
                m_data[0] = m_opcode;
                Buffer.BlockCopy(m_tabDeletePositions, 0, m_data, 1, m_deleteOffset);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void SavePositions()
        {
            if (m_data.Length > m_deleteOffset + m_tabsCount * Constants.BytesInWord + 1)
            {
                m_data[m_deleteOffset + 1] = m_tabsCount;
                Buffer.BlockCopy(m_tabPositions, 0, m_data, m_deleteOffset + Constants.BytesInWord, m_tabsCount * Constants.BytesInWord);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void SaveDescriptors()
        {
            if (m_data.Length > m_deleteOffset + m_tabsCount * (Constants.BytesInWord + TabDescriptor.DEF_TAB_LENGTH) + 1)
            {
                int offset = (m_tabsCount + 1) * Constants.BytesInWord;

                for (int i = 0; i < m_tabsCount; i++)
                {
                    m_data[m_deleteOffset + offset] = m_tabDescriptors[i].Save();
                    offset += TabDescriptor.DEF_TAB_LENGTH;
                }
            }
        }
        #endregion
    }
}
