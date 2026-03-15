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
    /// <summary>
    /// This structure specifies a future record type header.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    [CLSCompliant(false)]
    class FutureHeader 
    {
        #region Member
        /// <summary>
        /// Type of the following data:
        /// </summary>
        [BiffRecordPos(0, 2)]
        private ushort m_usType;
        /// <summary>
        /// Option Attribute Flags
        /// </summary>
        [BiffRecordPos(2, 2)]
        private ushort m_usAttributes;
        #endregion

        #region Properties
        /// <summary>
        /// Type of the following data:
        /// </summary>
        public ushort Type
        {
            get
            {
                return m_usType;
            }
            set
            {
                m_usType = value;
            }
        }
        public ushort Attributes
        {
            get
            {
                return m_usAttributes;
            }
            set
            {
                m_usAttributes = value;
            }
        }
        #endregion

        #region Methods
        public void InfillInternalData(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, Type);
            iOffset += 2;
            provider.WriteUInt16(iOffset, Attributes);
            iOffset += 2;
            provider.WriteInt64(iOffset, 0); //Reserved
        }
        public int GetStoreSize()
        {
            return 12;
        }
        #endregion

    }
}
