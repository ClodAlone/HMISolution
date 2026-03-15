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
    [Biff(TBIFFRecord.Compatibility)]
    [CLSCompliant(false)]
    class CompatibilityRecord: BiffRecordRaw
    {
        #region Members
        /// <summary>
        /// Specifies the Header structure of this record
        /// </summary>
        FutureHeader m_header;
        /// <summary>
        /// Specifies wheather the workbook checks the Compability of earlier version
        /// </summary>
        uint m_bNoCompCheck = 0;
        #endregion

        #region Initialization
        public CompatibilityRecord()
            : base()
        {
            m_header = new FutureHeader();
            m_header.Type = (ushort)TBIFFRecord.Compatibility;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Specifies wheather the workbook checks the Compability of earlier version
        /// </summary>
        public uint NoComptabilityCheck
        {
            get
            {
                return m_bNoCompCheck;
            }
            set
            {
                m_bNoCompCheck = value;
            }
        }
        #endregion

        #region Parse and Serialize
        public override void ParseStructure(DataProvider arrData, int iOffset, int iLength, ExcelVersion version)
        {
            m_bNoCompCheck=arrData.ReadUInt32(iOffset + 12);
        }
        public override void InfillInternalData(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_header.Type);

            provider.WriteUInt16(iOffset + 2, m_header.Attributes);

            provider.WriteInt64(iOffset + 4, 0); //Reserved

            provider.WriteUInt32(iOffset + 12, m_bNoCompCheck);
        }

        public override int GetStoreSize(ExcelVersion version)
        {
            return 16;
        }
        #endregion

    }
}
