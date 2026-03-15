#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region File using directives
using System;
using System.Collections;
using System.IO;
using System.Text;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    [CLSCompliant(false)]
    internal class GrammarSpelling
    {
        #region Fields
        private byte[] m_plcfsplData;
        private byte[] m_plcfgramData;
        private List<Int32> m_gramPositions;
        private List<Int32> m_spellPositions;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GrammarSpelling"/> class.
        /// </summary>
        internal GrammarSpelling()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrammarSpelling"/> class.
        /// </summary>
        /// <param name="fib">The fib.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="hfCharPosTable">The header/footer char position table.</param>
        internal GrammarSpelling(WPFIBData fib, Stream stream, CharPosTableRecord hfCharPosTable)
        {
            int plcfsplLength = fib.lcbPlcfspl;
            int plcfgramLength = fib.lcbPlcfgram;
            m_plcfsplData = new byte[plcfsplLength];
            m_plcfgramData = new byte[plcfgramLength];

            MakeCorrection(hfCharPosTable, fib, stream);

            stream.Position = fib.fcPlcfspl;
            stream.Read(m_plcfsplData, 0, plcfsplLength);

            stream.Position = fib.fcPlcfgram;
            stream.Read(m_plcfgramData, 0, plcfgramLength);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the PLCFSPL data.
        /// </summary>
        /// <value>The PLCFSPL data.</value>
        internal byte[] PlcfsplData
        {
            get
            {
                return m_plcfsplData;
            }

            set
            {
                m_plcfsplData = value;
            }
        }

        /// <summary>
        /// Gets or sets the plcfgram data.
        /// </summary>
        /// <value>The plcfgram data.</value>
        internal byte[] PlcfgramData
        {
            get
            {
                return m_plcfgramData;
            }

            set
            {
                m_plcfgramData = value;
            }
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Writes the specified fib.
        /// </summary>
        /// <param name="fib">The fib.</param>
        /// <param name="stream">The stream.</param>
        internal void Write(WPFIBData fib, Stream stream)
        {
            if (m_plcfgramData != null && m_plcfsplData != null)
            {
                fib.fcPlcfspl = (int)stream.Position;
                stream.Write(m_plcfsplData, 0, m_plcfsplData.Length);
                fib.lcbPlcfspl = m_plcfsplData.Length;

                fib.fcPlcfgram = (int)stream.Position;
                stream.Write(m_plcfgramData, 0, m_plcfgramData.Length);
                fib.lcbPlcfgram = m_plcfgramData.Length;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// 
        /// </summary>
        internal void GetPositions(WPFIBData fib, Stream stream)
        {
            BinaryReader binReader = new BinaryReader(stream);

            if (m_plcfsplData.Length > 0)
            {
                m_spellPositions = new List<Int32>();
                int spellPosCount = (m_plcfsplData.Length + 2) / 6;
                binReader.BaseStream.Position = fib.fcPlcfspl;

                for (int i = 0; i < spellPosCount; i++)
                {
                    m_spellPositions.Add(binReader.ReadInt32());
                }
            }

            if (m_plcfgramData.Length > 0)
            {
                m_gramPositions = new List<Int32>();
                int gramPosCount = (m_plcfgramData.Length + 2) / 6;
                binReader.BaseStream.Position = fib.fcPlcfgram;

                for (int i = 0; i < gramPosCount; i++)
                {
                    m_gramPositions.Add(binReader.ReadInt32());
                }
            }
        }

        /// <summary>
        /// Makes the correction.
        /// </summary>
        /// <param name="hfCharPosTable">The header/footer char pos table.</param>
        /// <param name="fib">The fib.</param>
        /// <param name="stream">The stream.</param>
        private void MakeCorrection(CharPosTableRecord hfCharPosTable, WPFIBData fib, Stream stream)
        {
            if (fib.ccpHdr > 0 && hfCharPosTable != null)
            {
                GetPositions(fib, stream);
                if (MakeHeaderCorrection(hfCharPosTable, fib))
                {
                    UpdateGramSpellData(stream, fib);
                }
            }
        }

        /// <summary>
        /// Makes the header correction.
        /// </summary>
        /// <param name="hfCharPosTable">The header/footer char pos table.</param>
        /// <param name="fib">The fib.</param>
        /// <returns></returns>
        private bool MakeHeaderCorrection(CharPosTableRecord hfCharPosTable, WPFIBData fib)
        {
            bool correctProc = false;
            if (fib.ccpHdr > 0 && hfCharPosTable != null)
            {
                int startHeaderCP = fib.ccpText + fib.ccpFtn;
                int shiftValue = hfCharPosTable.Positions[6];
                int startShiftCP = startHeaderCP + shiftValue;

                if (m_gramPositions != null)
                {
                    correctProc = ShiftHFPos(true, startHeaderCP, startShiftCP, shiftValue);
                }

                if (m_spellPositions != null && correctProc)
                {
                    correctProc = ShiftHFPos(false, startHeaderCP, startShiftCP, shiftValue);
                }
            }

            return correctProc;
        }

        /// <summary>
        /// Shifts the char positions in grammar/spelling array.
        /// </summary>
        /// <param name="isGrammar">if it is grammar, set to <c>true</c>.</param>
        /// <param name="startHeaderCP">The start header CP.</param>
        /// <param name="startShiftCP">The start shift CP.</param>
        /// <param name="shiftValue">The shift value.</param>
        /// <returns></returns>
        private bool ShiftHFPos(bool isGrammar, int startHeaderCP, int startShiftCP, int shiftValue)
        {
            int startSeparatorIndex = GetPosIndex(isGrammar, startHeaderCP);
            int startShiftIndex = GetPosIndex(isGrammar, startShiftCP);
            if (startSeparatorIndex == int.MaxValue || startShiftIndex == int.MaxValue)
            {
                return false;
            }
            startShiftIndex++;
            SetHFSeparatorsPos(startHeaderCP, startSeparatorIndex, startShiftIndex, isGrammar);
            ShiftPositions(startShiftIndex, shiftValue, isGrammar);

            return true;
        }

        /// <summary>
        /// Sets the header/footer separators pos.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="isGrammar">if it is grammar, set to <c>true</c>.</param>
        private void SetHFSeparatorsPos(int value, int startIndex, int endIndex, bool isGrammar)
        {
            List<Int32> dataArray = isGrammar ? m_gramPositions : m_spellPositions;
            for (int i = startIndex; i < endIndex; i++)
            {
                dataArray[i] = value;
            }
        }

        /// <summary>
        /// Gets the index of position in grammar/spelling aray.
        /// </summary>
        /// <param name="isGrammarArray">if it is grammar array, set to <c>true</c>.</param>
        /// <param name="charPos">The char pos.</param>
        /// <returns></returns>
        private int GetPosIndex(bool isGrammarArray, int charPos)
        {
            List<Int32> dataArray = isGrammarArray ? m_gramPositions : m_spellPositions;
            int arrayIndex = int.MaxValue;
            for (int i = 0, cnt = dataArray.Count; i < cnt; i++)
            {
                if (dataArray[i] >= charPos)
                {
                    arrayIndex = i;
                    break;
                }
            }

            return arrayIndex;
        }

        /// <summary>
        /// Shifts the positions.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="shiftValue">The shift value.</param>
        /// <param name="isGrammarArray">if it is grammar array, set to <c>true</c>.</param>
        private void ShiftPositions(int startIndex, int shiftValue, bool isGrammarArray)
        {
            List<Int32> dataArray = isGrammarArray ? m_gramPositions : m_spellPositions;
            for (int i = startIndex, cnt = dataArray.Count; i < cnt; i++)
            {
                dataArray[i] = dataArray[i] - shiftValue;
            }
        }

        /// <summary>
        /// Updates the grammar/spelling data.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fib">The fib.</param>
        private void UpdateGramSpellData(Stream stream, WPFIBData fib)
        {
            BinaryWriter binWriter = new BinaryWriter(stream);

            if (m_spellPositions != null)
            {
                binWriter.BaseStream.Position = fib.fcPlcfspl;
                for (int i = 0, cnt = m_spellPositions.Count; i < cnt; i++)
                {
                    binWriter.Write(m_spellPositions[i]);
                }
            }

            if (m_gramPositions != null)
            {
                binWriter.BaseStream.Position = fib.fcPlcfgram;
                for (int i = 0, cnt = m_gramPositions.Count; i < cnt; i++)
                {
                    binWriter.Write(m_gramPositions[i]);
                }
            }
        }
        #endregion
    }
}
