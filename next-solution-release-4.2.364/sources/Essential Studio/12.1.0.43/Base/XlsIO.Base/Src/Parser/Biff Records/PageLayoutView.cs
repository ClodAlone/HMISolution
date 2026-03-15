#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

using System.Text;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    [Syncfusion.Documentation.DocumentationExclude()]
    [CLSCompliant(false)]
    [Biff(TBIFFRecord.PageLayoutView)]
    class PageLayoutView : BiffRecordRaw
    {
        #region Class constants
        /// <summary>
        /// Size of the record.
        /// </summary>
        private const int DEF_FIXED_SIZE = 16;
        #endregion

        #region Memebers

        /// <summary>
        /// Specifies the future record.
        /// </summary>
        private ushort m_futureRecord = 2187;

        /// <summary>
        /// Specifies the zoom value in percentage for layout view.
        /// </summary>
        private ushort m_iScale = 0;

        /// <summary>
        /// Specifies whether sheet is in page layout view.
        /// </summary>
        [BiffRecordPos(14, 0, TFieldType.Bit)]
        private bool m_bPageLayoutView = false ;

        /// <summary>
        /// Specifies whether application displays 'Ruler'
        /// </summary>
        [BiffRecordPos(14, 1, TFieldType.Bit)]
        private bool m_bRulerVisible = false ;

        /// <summary>
        /// Specifies whether the margins between pages are hidden in the Page Layout view.
        /// </summary>
        [BiffRecordPos(14, 2, TFieldType.Bit)]
        private bool m_bWhiteSpaceHidden = false ;

        #endregion

        #region Property
        /// <summary>
        /// Specified the zoom value in percentage for layout view.
        /// </summary>
        internal ushort Scaling
        {
            get
            {
                return m_iScale;
            }
        }
        /// <summary>
        /// Secifies whether sheet is in page layout view.
        /// </summary>
        internal bool LayoutView
        {
            get
            {
                return m_bPageLayoutView;
            }
            set
            {
                m_bPageLayoutView = value;
            }
        }
        /// <summary>
        /// Specifies whether the margins between pages are hidden in the Page Layout view.
        /// </summary>
        internal bool WhiteSpaceHidden
        {
            get
            {
                return m_bWhiteSpaceHidden;
            }
            set
            {
                m_bWhiteSpaceHidden = value;
            }
        }
        /// <summary>
        /// Specifies whether application displays 'Ruler'
        /// </summary>
        internal bool RulerVisible
        {
            get
            {
                return m_bRulerVisible;
            }
            set
            {
                m_bRulerVisible = value;
            }
        }
        #endregion
        /// <summary>
        /// Default constructor
        /// </summary>
        public PageLayoutView()
            : base()
        {

        }

        /// <summary>
        /// Parse the pagelayout view.
        /// </summary>

        public override void ParseStructure(DataProvider provider, int iOffset, int iLength, ExcelVersion version)
        {
            iOffset += 12;

            m_iScale = provider.ReadUInt16(iOffset);

            iOffset = iOffset + 2;

            m_bPageLayoutView = provider.ReadBit(iOffset, 0);

            m_bRulerVisible = provider.ReadBit(iOffset, 1);

            m_bWhiteSpaceHidden = provider.ReadBit(iOffset, 2);
        }
       /// <summary>
       /// Serialize the page layout view.
       /// </summary>

        public override void InfillInternalData(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_futureRecord);
            provider.WriteUInt16(iOffset + 2, 0); //clears the garbage Values.
            provider.WriteInt64(iOffset + 4, 0);  //clears the garbage Values.
            iOffset += 12;

            provider.WriteUInt16(iOffset, m_iScale);
            iOffset += 2;

            provider.WriteBit(iOffset, m_bPageLayoutView, 0);
            provider.WriteBit(iOffset, m_bRulerVisible, 1);
            provider.WriteBit(iOffset, m_bWhiteSpaceHidden, 2);
        
        }

        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public override int GetStoreSize(ExcelVersion version)
        {
            return DEF_FIXED_SIZE ;
        }
    }
}
