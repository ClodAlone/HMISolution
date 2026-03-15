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
using System.Collections.Specialized;

using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for ListFormat.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ListProperties
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ListInfo m_listInfo;

        /// <summary>
        /// 
        /// </summary>
        private ParagraphProperties m_pap;

        /// <summary>
        /// Index of current list
        /// </summary>
        private short m_currentListIndex = 0;

        /// <summary>
        /// Index of current list's level
        /// </summary>
        private byte m_currentListLevel = 0;

        /// <summary>
        /// Key = override list style name, value = override list index.
        /// </summary>
        private Dictionary<string, short> m_overrideStyles = new Dictionary<string, short>();

        /// <summary>
        /// Key = list style name, value = list index.
        /// </summary>
        private Dictionary<string, short> m_styles = new Dictionary<string, short>();
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listInfo"></param>
        /// <param name="pap"></param>
        internal ListProperties(ListInfo listInfo, ParagraphProperties pap)
        {
            m_listInfo = listInfo;
            m_pap = pap;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets level of current list
        /// </summary>
        public byte ListLevelNumber
        {
            get
            {
                return m_pap.ListLevelIndex;
            }
            set
            {
                if (value < 8 || value >= 0)
                {
                    m_pap.ListLevelIndex = value;
                    m_currentListLevel = m_pap.ListLevelIndex;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, short> StyleListIndexes
        {
            get
            {
                return m_styles;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Apply bulleting for current paragraph
        /// </summary>
        public void ApplyBulletList()
        {
            m_pap.ListFormatIndex = m_listInfo.ApplyBulletList();
            m_currentListIndex = m_pap.ListFormatIndex;
            m_pap.ListLevelIndex = 0;
        }

        /// <summary>
        /// Continues the current list.
        /// </summary>
        public void ContinueCurrentList()
        {
            m_pap.ListFormatIndex = m_currentListIndex;
            m_pap.ListLevelIndex = m_currentListLevel;
        }

        /// <summary>
        /// Apply numbering for current paragraph
        /// </summary>
        public void ApplyNumberList()
        {
            m_pap.ListFormatIndex = m_listInfo.ApplyNumberList();
            m_currentListIndex = m_pap.ListFormatIndex;
            m_pap.ListLevelIndex = 0;
        }
       
        /// <summary>
        /// Increase indent of current list paragraph
        /// </summary>
        public void IncreaseIndent()
        {
            ListLevelNumber++;
        }

        /// <summary>
        /// Decrease indent of current list paragraph
        /// </summary>
        public void DecreaseIndent()
        {
            ListLevelNumber--;
        }

        /// <summary>
        /// Remove list from current paragraph
        /// </summary>
        public void RemoveList()
        {
            m_pap.ListFormatIndex = 0;
            m_pap.ListLevelIndex = 0;
        }

        /// <summary>
        /// Continue current list
        /// </summary>
        internal void ContinueCurrentList(ListData listData, WListFormat listFormat, WordStyleSheet styleSheet)
        {
            string lfoStyleName = listFormat.LFOStyleName;
            if (lfoStyleName != null)
            {
                if (m_overrideStyles.ContainsKey(lfoStyleName))
                {
                    m_pap.ListFormatIndex = m_overrideStyles[lfoStyleName];
                }
                else
                {
                    m_pap.ListFormatIndex = m_listInfo.ApplyLFO(listData, listFormat as WListFormat, styleSheet);
                    m_overrideStyles.Add(lfoStyleName, m_pap.ListFormatIndex);
                }
            }
            else
            {
                if (m_styles.ContainsKey(listFormat.CustomStyleName))
                    m_pap.ListFormatIndex = m_styles[listFormat.CustomStyleName];
            }

            m_pap.ListLevelIndex = (byte)listFormat.ListLevelNumber;
        }

        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="listData">The list data.</param>
        /// <param name="listFormat">The list format.</param>
        /// <param name="styleSheet">The style sheet.</param>
        /// <param name="applyToPap">if it is apply to pap, set to <c>true</c>.</param>
        /// <returns></returns>
        internal int ApplyList(ListData listData, WListFormat listFormat, WordStyleSheet styleSheet, bool applyToPap)
        {
            short listFormatIndex = (short)m_listInfo.ApplyList(listData, listFormat as WListFormat, styleSheet);

            if (applyToPap)
            {
                m_pap.ListFormatIndex = listFormatIndex;
                m_pap.ListLevelIndex = (byte)listFormat.ListLevelNumber;
            }

            if (listFormat.LFOStyleName != null)
            {
                if (!m_overrideStyles.ContainsKey(listFormat.LFOStyleName))
                {
                    m_overrideStyles.Add(listFormat.LFOStyleName, listFormatIndex);
                }
            }

            if (!m_styles.ContainsKey(listFormat.CustomStyleName))
            {
                m_styles.Add(listFormat.CustomStyleName, listFormatIndex);
            }
            else
            {
                // Change listFormatIndex in the collection of
                // style name/listFormatIndex on RestartNumbering.
                m_styles[listFormat.CustomStyleName] = listFormatIndex;
            }

            return listFormatIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listData"></param>
        /// <param name="listFormat"></param>
        /// <param name="styleSheet"></param>
        /// <returns></returns>
        internal int ApplyBaseStyleList(ListData listData, WListFormat listFormat, WordStyleSheet styleSheet)
        {
            short currentListIndex = m_listInfo.ApplyList(listData, listFormat as WListFormat, styleSheet);
            m_styles.Add(listFormat.CustomStyleName, currentListIndex);
            return currentListIndex;
        }

        /// <summary>
        /// Set paragraph's list level index. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isOutLineListStyle"></param>
        internal void SetListLevelNumber(byte value, bool isOutLineListStyle)
        {
            if (value < 8 || value >= 0)
            {
                if (isOutLineListStyle) m_pap.ListLevelIndex = value;
                m_currentListLevel = m_pap.ListLevelIndex;
            }
        }
        #endregion

        #region Class internal properties
        /// <summary>
        /// 
        /// </summary>
        internal ParagraphProperties Pap
        {
            get
            {
                return m_pap;
            }
        }
        #endregion
    }
}
