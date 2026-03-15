#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    /// <summary>
    /// The begin record defines the start of a block of records for a (Graphing)
    /// data object. This record is matched with a corresponding EndRecord.
    /// </summary>
    [Biff(TBIFFRecord.CFEx)]
    [Syncfusion.Documentation.DocumentationExclude()]
    [CLSCompliant(false)]
    public class CFExRecord : CondFMTRecord
    {
        #region Constants
        /// <summary>
        /// Minimum size of the record.
        /// </summary>
        private const ushort DEF_MINIMUM_RECORD_SIZE = 18;
        /// <summary>
        /// Size of the record If the record follow CF12.
        /// </summary>
        private const ushort DEF_ISCF12_RECORD_SIZE = 25;
        #endregion

        #region Class member
        /// <summary>
        /// CF12 record.
        /// </summary>
        private CF12Record m_cf12Record;
        /// <summary>
        /// Header of this record.
        /// </summary>
        private FutureHeader m_header;
        /// <summary>
        /// Specifies whether the containing record specifies a range of cells.
        /// </summary>
        private bool m_isRefRange = true;
        /// <summary>
        /// Specifies whether to alert the user of possible problems when saving the file.
        /// </summary>
        private bool m_isFutureAlert = false;
        private byte m_headerAttribute = 1;
        /// <summary>
        /// Cell range address of the range enclosing all
        /// conditionally formatted ranges.
        /// </summary>
        private TAddr m_addrEncloseRange = new TAddr();
        /// <summary>
        /// A boolean that specifies the type of rule this record extends.
        /// Must be one of the following.
        /// 0 - This record extends a rule specified by a CF record and MUST NOT be followed by a CF12 record.
        /// 1 - This record extends a rule specified by a CF12 record and MUST be followed by the CF12 record it extends.
        /// </summary>
        private byte m_isCF12;
        /// <summary>
        /// An integer that specifies which CondFMTRecord record is being extended.
        /// </summary>
        private ushort m_CondFMTIndex;
        /// <summary>
        /// An integer that specifies a zero based index of CF Record.
        /// </summary>
        private ushort m_CFIndex;
        /// <summary>
        /// Comparison operator: 
        /// 00H = No comparison (only valid for formula type, see above)
        /// 01H = Between
        /// 02H = Not between
        /// 03H = Equal
        /// 04H = Not equal
        /// 05H = Greater than
        /// 06H = Less than
        /// 07H = Greater or equal
        /// 08H = Less or equal
        /// </summary>
        private byte m_compareOperator = 0;
        /// <summary>
        /// An integer that specifies the template from which the rule was created.
        /// 0x0000 	Cell value 
        /// 0x0001 	Formula 
        /// 0x0002 	Color scale formatting 
        /// 0x0003 	Data bar formatting 
        /// 0x0004 	Icon set formatting 
        /// 0x0005 	Filter 
        /// 0x0007 	Unique values 
        /// 0x0008 	Contains text 
        /// 0x0009 	Contains blanks 
        /// 0x000A 	Contains no blanks 
        /// 0x000B 	Contains errors 
        /// 0x000C 	Contains no errors 
        /// 0x000F 	Today 
        /// 0x0010 	Tomorrow 
        /// 0x0011 	Yesterday 
        /// 0x0012 	Last 7 days 
        /// 0x0013 	Last month 
        /// 0x0014 	Next month 
        /// 0x0015 	This week 
        /// 0x0016 	Next week 
        /// 0x0017 	Last week 
        /// 0x0018 	This month 
        /// 0x0019 	Above average 
        /// 0x001A 	Below Average 
        /// 0x001B 	Duplicate values 
        /// 0x001D 	Above or equal to average 
        /// 0x001E 	Below or equal to average 
        /// </summary>
        private ushort m_template = 0;
        /// <summary>
        /// An integer that specifies the priority of the rule.
        /// Rules that apply to the same cell are evaluated in increasing order of ipriority.
        /// </summary>
        private ushort m_priority;
        /// <summary>
        /// A byte that specify the active condition and stop Iftrue option.
        /// </summary>
        private byte m_undefined=1;
        private bool m_cfExIsparsed = false;
        /// <summary>
        /// A Boolean that specifies whether cell formatting data is part of this record extension.
        /// Must be one of the following
        /// 0 - No formatting data in this record extension.
        /// 1 - Formatting data is part of this record extension.
        /// </summary>
        private byte m_hasDXF;

        // dxfn12 variable - extension of dxfn
        /// <summary>
        /// An integer that specifies the size of the structure in bytes.
        /// If greater than zero, it MUST be the total byte count of dfxn and xfext. Otherwise it MUST be zero.
        /// </summary>
        private ushort m_sizeOfDXF;
        /// <summary>
        /// Size of the Extended Properties.
        /// </summary>
        private ushort m_propertyCount;
        /// <summary>
        /// Set of properties applied to the XF format
        /// </summary>
        private List<ExtendedProperty> m_properties;
        /// <summary>
        /// An integer that specifies the size of the rgbTemplate Parmeters field in bytes.
        /// </summary>
        private ushort m_templateParamCount = 16;
        /// <summary>
        /// Reserved.
        /// </summary>
        private ushort m_reserved = 0xFFFF;
        /// <summary>
        /// CFEx Default parameter.
        /// </summary>
        private ushort m_defaultParameter;
        /// <summary>
        /// DXFN Structure.
        /// </summary>
        private DXFN m_dxfn;
        /// <summary>
        /// CFEx filter template parameter.
        /// </summary>
        private CFExFilterParameter m_cfExFilterParam;
        /// <summary>
        /// CFEx Text template parameter.
        /// </summary>
        internal CFExTextTemplateParameter m_cfExTextParam;
        /// <summary>
        /// CFEx Date template parameter.
        /// </summary>
        internal CFExDateTemplateParameter m_cfExDateParam;
        /// <summary>
        /// CFEx Average template parameter.
        /// </summary>
        private CFExAverageTemplateParameter m_cfExAverageParam;
        #endregion

        #region Class properties
        /// <summary>
        /// Cell range address of the range enclosing all
        /// conditionally formatted ranges.
        /// </summary>
        public TAddr EncloseRange
        {
            get
            {
                return m_addrEncloseRange;
            }
            set
            {
                m_addrEncloseRange = value;
            }
        }
        /// <summary>
        /// A boolean that specifies the type of rule this record extends.
        /// </summary>
        public byte IsCF12Extends
        {
            get
            {
                return m_isCF12;
            }
            set
            {
                m_isCF12 = value;
            }
        }
        /// <summary>
        /// An integer that specifies which CondFmt record is being extended.
        /// </summary>
        public ushort CondFmtIndex
        {
            get
            {
                return m_CondFMTIndex;
            }
            set
            {
                m_CondFMTIndex = value;
            }
        }
        /// <summary>
        /// An integer that specifies a zero based index of CF Record.
        /// </summary>
        public ushort CFIndex
        {
            get
            {
                return m_CFIndex;
            }
            set
            {
                m_CFIndex = value;
            }
        }
        /// <summary>
        /// Comparison operator: 
        /// 00H = No comparison (only valid for formula type, see above)
        /// 01H = Between
        /// 02H = Not between
        /// 04H = Not equal
        /// 05H = Greater than
        /// 06H = Less than
        /// 03H = Equal
        /// 07H = Greater or equal
        /// 08H = Less or equal
        /// </summary>
        public ExcelComparisonOperator ComparisonOperator
        {
            get
            {
                return (ExcelComparisonOperator)m_compareOperator;
            }
            set
            {
                m_compareOperator = (byte)value;
            }
        }
        /// <summary>
        /// An integer that specifies the template from which the rule was created.
        /// 0x0000 	Cell value 
        /// 0x0001 	Formula 
        /// 0x0002 	Color scale formatting 
        /// 0x0003 	Data bar formatting 
        /// 0x0004 	Icon set formatting 
        /// 0x0005 	Filter 
        /// 0x0007 	Unique values 
        /// 0x0008 	Contains text 
        /// 0x0009 	Contains blanks 
        /// 0x000A 	Contains no blanks 
        /// 0x000B 	Contains errors 
        /// 0x000C 	Contains no errors 
        /// 0x000F 	Today 
        /// 0x0010 	Tomorrow 
        /// 0x0011 	Yesterday 
        /// 0x0012 	Last 7 days 
        /// 0x0013 	Last month 
        /// 0x0014 	Next month 
        /// 0x0015 	This week 
        /// 0x0016 	Next week 
        /// 0x0017 	Last week 
        /// 0x0018 	This month 
        /// 0x0019 	Above average 
        /// 0x001A 	Below Average 
        /// 0x001B 	Duplicate values 
        /// 0x001D 	Above or equal to average 
        /// 0x001E 	Below or equal to average 
        /// </summary>
        public ConditionalFormatTemplate Template
        {
            get
            {
                return (ConditionalFormatTemplate)m_template;
            }
            set
            {
                m_template = (byte)value;
            }
        }
        /// <summary>
        /// An integer that specifies the priority of the rule.
        /// </summary>        
        public ushort Priority
        {
            get
            {
                return m_priority;
            }
            set
            {
                m_priority = value;
            }
        }
        /// <summary>
        /// A bit that specifies whether, when a cell fulfills the condition corresponding to this rule, 
        /// the lower priority conditional formatting rules that apply to this cell are evaluated.
        /// </summary>
        public bool StopIfTrue
        {
            get
            {
                return ((m_undefined & 2) == 2);
            }
            set
            {
                m_undefined = (byte)(value ? (m_undefined | 2) : (m_undefined | 0));
            }
        }
        /// <summary>
        /// A Boolean that specifies whether cell formatting data is part of this record extension.
        /// </summary>
        public byte HasDXF
        {
            get
            {
                return m_hasDXF;
            }
            set
            {
                m_hasDXF = value;
            }
        }
        /// <summary>
        /// An integer that specifies the size of the structure in bytes.
        /// </summary>
        public ushort SizeOfDXF
        {
            get
            {
                return m_sizeOfDXF;
            }
            set
            {
                m_sizeOfDXF = value;
            }
        }
        /// <summary>
        /// Properties count.
        /// </summary>
        public ushort PropertyCount
        {
            get
            {
                return m_propertyCount;
            }
            set
            {
                m_propertyCount = value;
            }
        }
        /// <summary>
        /// Set of properties applied to the XF format.
        /// </summary>
        public List<ExtendedProperty> Properties
        {
            get
            {
                return m_properties;
            }
            set
            {
                m_properties = value;
            }
        }
        /// <summary>
        /// Read-only. Maximum possible size of the record.
        /// </summary>
        public override int MinimumRecordSize
        {
            get
            {
                return DEF_MINIMUM_RECORD_SIZE;
            }
        }
        /// <summary>
        /// Check whether the rule is parsed or not.
        /// </summary>
        public bool IsCFExParsed
        {
            get
            {
                return m_cfExIsparsed;
            }
            set
            {
                m_cfExIsparsed = value;
            }
        }
        /// <summary>
        /// Gets/Sets CF12 record.
        /// </summary>
        public CF12Record CF12RecordIfExtends
        {
            get
            {
                return m_cf12Record;
            }
            set
            {
                m_cf12Record = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public CFExRecord()
            : base()
        {
            m_header = new FutureHeader();
            m_header.Type = (ushort)TBIFFRecord.CFEx;

            m_dxfn = new DXFN();
            m_properties = new List<ExtendedProperty>();
            m_cfExFilterParam = new CFExFilterParameter();
            m_cfExTextParam = new CFExTextTemplateParameter();
            m_cfExDateParam = new CFExDateTemplateParameter();
            m_cfExAverageParam = new CFExAverageTemplateParameter();
        }
        /// <summary>
        /// Read / initialize constructor.
        /// </summary>
        /// <param name="stream">Stream from which record data should be read.</param>
        /// <param name="itemSize">Size of read item.</param>
        /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
        /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
        public CFExRecord(Stream stream, out int itemSize)
            : base(stream, out itemSize)
        {
        }
        /// <summary>
        /// Reserves for record's internal data array iReserve bytes.
        /// </summary>
        /// <param name="iReserve">Amount of bytes for data array.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
        public CFExRecord(int iReserve)
            : base(iReserve)
        {
        }
        #endregion

        #region Record Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public override void ParseStructure(DataProvider provider, int iOffset, int iLength, ExcelVersion version)
        {
            ushort reserved = 0;
            m_cfExIsparsed = true;

            //header
            m_header.Type = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_headerAttribute = provider.ReadByte(iOffset);
            iOffset += 2;

            m_addrEncloseRange = provider.ReadAddr(iOffset);
            iOffset += 8;

            m_isCF12 = (byte)provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_CondFMTIndex = provider.ReadUInt16(iOffset);
            iOffset += 2;

            if (m_isCF12 == 0)
            {
                m_CFIndex = provider.ReadUInt16(iOffset);
                iOffset += 2;

                m_compareOperator = provider.ReadByte(iOffset);
                iOffset++;

                m_template = provider.ReadByte(iOffset);
                iOffset++;

                m_priority = provider.ReadUInt16(iOffset);
                iOffset += 2;
                
                m_undefined = provider.ReadByte(iOffset);
                iOffset++;

                m_hasDXF = provider.ReadByte(iOffset);
                iOffset++;

                if (m_hasDXF != 0)
                {
                    m_sizeOfDXF = (ushort)provider.ReadUInt32(iOffset);
                    iOffset += 4;

                    if (m_sizeOfDXF == 0)
                    {
                        reserved = provider.ReadUInt16(iOffset);
                        iOffset += 2;
                    }

                    int DxfnByteCount = iOffset;
                    if (m_sizeOfDXF > 0)
                    {
                        m_dxfn = new DXFN();
                        iOffset = m_dxfn.ParseDXFN(provider, iOffset, version);
                    }

                    DxfnByteCount = iOffset - DxfnByteCount;

                    if (m_sizeOfDXF != DxfnByteCount)
                    {
                        reserved = provider.ReadUInt16(iOffset);
                        iOffset += 2;

                        m_reserved = provider.ReadUInt16(iOffset);
                        iOffset += 2;

                        reserved = provider.ReadUInt16(iOffset);
                        iOffset += 2;

                        m_propertyCount = provider.ReadUInt16(iOffset);
                        iOffset += 2;

                        //parse the extended properties
                        m_properties = new List<ExtendedProperty>();
                        for (int i = 0; i < m_propertyCount; i++)
                        {
                            ExtendedProperty property = new ExtendedProperty();
                            iOffset = property.ParseExtendedProperty(provider, iOffset, version);
                            m_properties.Add(property);
                        }
                    }
                }

                m_templateParamCount = provider.ReadByte(iOffset);
                iOffset++;

                iOffset = ParseCFExTemplateParameter(provider, iOffset, version);
            }
        }
        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public override void InfillInternalData(DataProvider provider, int iOffset, ExcelVersion version)
        {
            ushort reserved = 0xFFFF;
            m_iLength = GetStoreSize(version);

            //header
            provider.WriteUInt16(iOffset, m_header.Type);
            iOffset += 2;

            provider.WriteByte(iOffset, m_headerAttribute);
            iOffset += 2;

            provider.WriteAddr(iOffset, m_addrEncloseRange);
            iOffset += 8;

            provider.WriteUInt32(iOffset, m_isCF12);
            iOffset += 4;

            provider.WriteUInt16(iOffset, m_CondFMTIndex);
            iOffset += 2;

            if (m_isCF12 == 0)
            {
                provider.WriteUInt16(iOffset, m_CFIndex);
                iOffset += 2;

                provider.WriteByte(iOffset, m_compareOperator);
                iOffset++;

                provider.WriteByte(iOffset, (byte)m_template);
                iOffset++;

                provider.WriteUInt16(iOffset, m_priority);
                iOffset += 2;

                provider.WriteByte(iOffset, m_undefined);
                iOffset++;

                provider.WriteByte(iOffset, m_hasDXF);
                iOffset++;

                if (m_hasDXF != 0)
                {
                    provider.WriteUInt32(iOffset, m_sizeOfDXF);
                    iOffset += 4;

                    if (m_sizeOfDXF == 0)
                    {
                        provider.WriteUInt16(iOffset, 0);
                        iOffset += 2;
                    }

                    int DxfnByteCount = iOffset;
                    if (m_sizeOfDXF > 0)
                    {
                        iOffset = m_dxfn.SerializeDXFN(provider, iOffset, version);
                    }

                    DxfnByteCount = iOffset - DxfnByteCount;

                    if (m_sizeOfDXF != DxfnByteCount)
                    {
                        provider.WriteUInt16(iOffset, 0);
                        iOffset += 2;

                        provider.WriteUInt16(iOffset, reserved);
                        iOffset += 2;

                        provider.WriteUInt16(iOffset, 0);
                        iOffset += 2;

                        provider.WriteUInt16(iOffset, (ushort)m_properties.Count);
                        iOffset += 2;

                        //parse the extended properties
                        foreach (ExtendedProperty property in m_properties)
                        {
                            iOffset = property.InfillInternalData(provider, iOffset, version);
                        }
                    }
                }

                provider.WriteByte(iOffset, (byte)m_templateParamCount);
                iOffset++;

                iOffset = SerializeCFExTemplateParameter(provider, iOffset, version);
            }

        }

        /// <summary>
        /// Parse structure of template parameter. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseCFExTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            if (Template == ConditionalFormatTemplate.Filter)
                m_cfExFilterParam.ParseFilterTemplateParameter(provider, iOffset, version);

            else if (Template == ConditionalFormatTemplate.ContainsText)
                m_cfExTextParam.ParseTextTemplateParameter(provider, iOffset, version);

            else if (Template == ConditionalFormatTemplate.Today || Template == ConditionalFormatTemplate.Tomorrow || Template == ConditionalFormatTemplate.Yesterday
                    || Template == ConditionalFormatTemplate.Last7Days || Template == ConditionalFormatTemplate.LastMonth || Template == ConditionalFormatTemplate.NextMonth
                    || Template == ConditionalFormatTemplate.ThisWeek || Template == ConditionalFormatTemplate.NextWeek || Template == ConditionalFormatTemplate.LastWeek
                    || Template == ConditionalFormatTemplate.ThisMonth)

                m_cfExDateParam.ParseDateTemplateParameter(provider, iOffset, version);

            else if (Template == ConditionalFormatTemplate.AboveAverage || Template == ConditionalFormatTemplate.BelowAverage
                    || Template == ConditionalFormatTemplate.AboveOrEqualToAverage || Template == ConditionalFormatTemplate.BelowOrEqualToAverage)

                m_cfExAverageParam.ParseAverageTemplateParameter(provider, iOffset, version);

            else
            {
                m_defaultParameter = provider.ReadUInt16(iOffset);
                iOffset += 16;
            }

            return iOffset;
        }
        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int SerializeCFExTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            if (Template == ConditionalFormatTemplate.Filter)
                m_cfExFilterParam.SerializeFilterParameter(provider, iOffset, version);

            else if (Template == ConditionalFormatTemplate.ContainsText)
                m_cfExTextParam.SerializeTextTemplateParameter(provider, iOffset, version);

            else if (Template == ConditionalFormatTemplate.Today || Template == ConditionalFormatTemplate.Tomorrow || Template == ConditionalFormatTemplate.Yesterday
                    || Template == ConditionalFormatTemplate.Last7Days || Template == ConditionalFormatTemplate.LastMonth || Template == ConditionalFormatTemplate.NextMonth
                    || Template == ConditionalFormatTemplate.ThisWeek || Template == ConditionalFormatTemplate.NextWeek || Template == ConditionalFormatTemplate.LastWeek
                    || Template == ConditionalFormatTemplate.ThisMonth)

                m_cfExDateParam.SerializeDateTemplateParameter(provider, iOffset, version);

            else if (Template == ConditionalFormatTemplate.AboveAverage || Template == ConditionalFormatTemplate.BelowAverage
                    || Template == ConditionalFormatTemplate.AboveOrEqualToAverage || Template == ConditionalFormatTemplate.BelowOrEqualToAverage)

                m_cfExAverageParam.SerializeAverageTemplateParameter(provider, iOffset, version);

            else
            {
                provider.WriteInt64(iOffset, 0);
                iOffset += 8;

                provider.WriteInt64(iOffset, 0);
                iOffset += 8;
            }

            return iOffset;
        }

        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public override int GetStoreSize(ExcelVersion version)
        {
            int iSize = DEF_MINIMUM_RECORD_SIZE;
            int DxfnByteCount = 0;

            if (m_isCF12 == 0)
            {
                iSize += DEF_ISCF12_RECORD_SIZE;

                if (m_hasDXF != 0)
                {
                    iSize += 4;
                    if (m_sizeOfDXF == 0)
                    {
                        iSize += 2;
                    }

                    if (m_sizeOfDXF != 0)
                    {
                        DxfnByteCount = m_dxfn.GetStoreSize(version);
                        iSize += DxfnByteCount;
                    }

                    if (m_sizeOfDXF != DxfnByteCount)
                    {
                        iSize += 8;
                        if (m_propertyCount > 0)
                        {
                            foreach (ExtendedProperty Exprop in m_properties)
                            {
                                iSize += Exprop.Size;
                            }
                        }
                    }
                }
            }

            return iSize;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Serves as a hash function for a particular type, suitable for use in
        /// hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current Object.</returns>
        public override int GetHashCode()
        {
            // TODO: here we can optimize a little bit by caching hash code.
            int iHashCode = m_header.Type.GetHashCode()
              ^ m_isRefRange.GetHashCode()
              ^ m_isFutureAlert.GetHashCode()
              ^ CellList.GetHashCode()
              ^ m_isCF12.GetHashCode()
              ^ m_CondFMTIndex.GetHashCode()
              ^ m_CFIndex.GetHashCode()
              ^ ComparisonOperator.GetHashCode()
              ^ m_template.GetHashCode()
              ^ m_priority.GetHashCode()
              ^ m_undefined.GetHashCode()
              ^ m_hasDXF.GetHashCode()
              ^ m_sizeOfDXF.GetHashCode()
              ^ m_dxfn.GetHashCode()

              ^ m_propertyCount.GetHashCode()
              ^ m_properties.GetHashCode()
              ^ m_templateParamCount.GetHashCode()
              ^ m_cfExFilterParam.GetHashCode()
              ^ m_cfExTextParam.GetHashCode()
              ^ m_cfExDateParam.GetHashCode()
              ^ m_cfExAverageParam.GetHashCode()
              ^ m_defaultParameter.GetHashCode();

            return iHashCode;
        }
        /// <summary>
        /// A hash code for the current Object without taking cell list into account.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            CFExRecord toCompare = obj as CFExRecord;

            if (toCompare == null) return false;

            bool bResult = m_header.Type == toCompare.m_header.Type
              && m_isRefRange == toCompare.m_isRefRange
              && m_isFutureAlert == toCompare.m_isFutureAlert
              && CellList == toCompare.CellList
              && m_isCF12 == toCompare.m_isCF12
              && m_CondFMTIndex == toCompare.m_CondFMTIndex
              && m_CFIndex == toCompare.m_CFIndex
              && ComparisonOperator == toCompare.ComparisonOperator
              && m_template == toCompare.m_template
              && m_priority == toCompare.m_priority
              && m_undefined==toCompare.m_undefined
              && m_hasDXF == toCompare.m_hasDXF
              && m_sizeOfDXF == toCompare.m_sizeOfDXF
              && m_dxfn==toCompare.m_dxfn

              && m_propertyCount == toCompare.m_propertyCount
              && m_properties == toCompare.m_properties
              && m_templateParamCount == toCompare.m_templateParamCount
              && m_cfExFilterParam == toCompare.m_cfExFilterParam
              && m_cfExTextParam == toCompare.m_cfExTextParam
              && m_cfExDateParam == toCompare.m_cfExDateParam
              && m_cfExAverageParam == toCompare.m_cfExAverageParam
              && m_defaultParameter == toCompare.m_defaultParameter;


            return bResult;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clone current Record.
        /// </summary>
        /// <returns>Returns memberwise clone on current object.</returns>        
        public override object Clone()
        {
            return (CFExRecord)this.MemberwiseClone();
        }
        #endregion

        internal void ClearAll()
        {
            m_cf12Record.ClearAll();
            
        }
    }

    #region CFEx template Parameter Classes

    public class CFExFilterParameter
    {
        #region Members
        /// <summary>
        /// Specifies whether the top or bottom items are displayed with the conditional formatting.
        /// </summary>
        private bool m_isTopOrBottom;
        /// <summary>
        /// Specifies whether a percentage of the top or bottom items are displayed with the conditional formatting,
        /// or whether a set number of the top or bottom items are displayed with the conditional formatting.
        /// </summary>
        private bool m_isPercent;
        /// <summary>
        /// Specifies how many values are displayed with the conditional formatting.
        /// </summary>
        private ushort m_filterValue;
        #endregion

        #region Properties
        /// <summary>
        /// Top or bottom items are displayed with rule.
        /// </summary>
        public bool IsTopOrBottom
        {
            get
            {
                return m_isTopOrBottom;
            }
            set
            {
                m_isTopOrBottom = value;
            }
        }
        /// <summary>
        /// Percentage of the top or bottom items are displayed with the rule.
        /// </summary>
        public bool IsPercent
        {
            get
            {
                return m_isPercent;
            }
            set
            {
                m_isPercent = value;
            }
        }
        /// <summary>
        /// Specifies how many values are displayed with the rule.
        /// </summary>
        public ushort FilterValue
        {
            get
            {
                return m_filterValue;
            }
            set
            {
                m_filterValue = value;
            }
        }
        #endregion

        #region Initialization
        public CFExFilterParameter()
        { }
        #endregion

        #region Record Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public void ParseFilterTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            Int64 reserved = 0;

            m_isTopOrBottom = provider.ReadBit(iOffset, 0);
            m_isPercent = provider.ReadBit(iOffset, 1);
            iOffset++;

            m_filterValue = provider.ReadUInt16(iOffset);
            iOffset += 2;

            reserved = provider.ReadInt64(iOffset);
            iOffset += 13;
        }
        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public void SerializeFilterParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteBit(iOffset, m_isTopOrBottom, 0);
            provider.WriteBit(iOffset, m_isPercent, 1);
            iOffset++;

            provider.WriteUInt16(iOffset, m_filterValue);
            iOffset += 2;

            provider.WriteInt64(iOffset, 0);
            iOffset += 13;
        }
        #endregion
    }

    public class CFExTextTemplateParameter
    {
        #region Member
        /// <summary>
        /// Specifies the type of text rule.
        /// </summary>
        private ushort m_textRuleType;
        #endregion

        #region Properties
        /// <summary>
        /// Type of text rule.
        /// </summary>
        public CFTextRuleType TextRuleType
        {
            get
            {
                return (CFTextRuleType)m_textRuleType;
            }
            set
            {
                m_textRuleType = (byte)value;
            }
        }
        #endregion

        #region Initialization
        public CFExTextTemplateParameter()
        { }
        #endregion

        #region Record Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public void ParseTextTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            Int64 reserved = 0;

            m_textRuleType = provider.ReadUInt16(iOffset);
            iOffset += 2;

            reserved = provider.ReadInt64(iOffset);
            iOffset += 14;
        }
        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public void SerializeTextTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_textRuleType);
            iOffset += 2;

            provider.WriteInt64(iOffset, 0);
            iOffset += 14;
        }
        #endregion
    }

    public class CFExDateTemplateParameter
    {
        #region Member
        /// <summary>
        /// Specifies the type of date comparison.
        /// </summary>
        private ushort m_dateComparisonType;
        #endregion

        #region Properties
        /// <summary>
        /// Type of date comparison operator.
        /// </summary>
        public ushort DateComparisonOperator
        {
            get
            {
                return m_dateComparisonType;
            }
            set
            {
                m_dateComparisonType = value;
            }
        }
        #endregion

        #region Initialization
        public CFExDateTemplateParameter()
        { }
        #endregion

        #region Record Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public void ParseDateTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            Int64 reserved = 0;

            m_dateComparisonType = provider.ReadUInt16(iOffset);
            iOffset += 2;

            reserved = provider.ReadInt64(iOffset);
            iOffset += 14;
        }
        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public void SerializeDateTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_dateComparisonType);
            iOffset += 2;

            provider.WriteInt64(iOffset, 0);
            iOffset += 14;
        }
        #endregion
    }

    public class CFExAverageTemplateParameter
    {
        #region Member
        /// <summary>
        /// Specifies the number of standard deviations above or below the average for the rule.
        /// </summary>
        private ushort m_numberOfStandardDeviation;

        #endregion

        #region Properties
        /// <summary>
        /// Number of standard deviations of average.
        /// </summary>
        public ushort NumberOfDeviations
        {
            get
            {
                return m_numberOfStandardDeviation;
            }
            set
            {
                m_numberOfStandardDeviation = value;
            }
        }
        #endregion

        #region Initialization
        public CFExAverageTemplateParameter()
        { }
        #endregion

        #region Record Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public void ParseAverageTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            Int64 reserved = 0;

            m_numberOfStandardDeviation = provider.ReadUInt16(iOffset);
            iOffset += 2;

            reserved = provider.ReadInt64(iOffset);
            iOffset += 14;
        }
        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public void SerializeAverageTemplateParameter(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_numberOfStandardDeviation);
            iOffset += 2;

            provider.WriteInt64(iOffset, 0);
            iOffset += 14;
        }
        #endregion
    }

    #endregion

    #region DXFN
    public class DXFN
    {
        #region Constants
        /// <summary>
        /// Size of the first block of reserved bytes in the font block.
        /// </summary>
        private const int DEF_FONT_FIRST_RESERVED_SIZE = 64;
        /// <summary>
        /// Size of the second block of reserved bytes in the font block.
        /// </summary>
        private const int DEF_FONT_SECOND_RESERVED_SIZE = 3;
        /// <summary>
        /// Size of the third block of reserved bytes in the font block.
        /// </summary>
        private const int DEF_FONT_THIRD_RESERVED_SIZE = 16;
        /// <summary>
        /// Mask for font posture bit.
        /// </summary>
        private const uint DEF_FONT_POSTURE_MASK = 0x2;
        /// <summary>
        /// Mask for font cancellation (strikethrough) bit.
        /// </summary>
        private const uint DEF_FONT_CANCELLATION_MASK = 0x80;
        /// <summary>
        /// Mask for font style modification bit.
        /// </summary>
        private const uint DEF_FONT_STYLE_MODIFIED_MASK = 0x02;
        /// <summary>
        /// Mask for font cancellation modification bit.
        /// </summary>
        private const uint DEF_FONT_CANCELLATION_MODIFIED_MASK = 0x80;

        /// <summary>
        /// Mask for left border line style bits.
        /// </summary>
        private const ushort DEF_BORDER_LEFT_MASK = 0x000F;
        /// <summary>
        /// Mask for right border line style bits.
        /// </summary>
        private const ushort DEF_BORDER_RIGHT_MASK = 0x00F0;
        /// <summary>
        /// Mask for top border line style bits.
        /// </summary>
        private const ushort DEF_BORDER_TOP_MASK = 0x0F00;
        /// <summary>
        /// Mask for bottom border line style bits.
        /// </summary>
        private const ushort DEF_BORDER_BOTTOM_MASK = 0xF000;

        /// <summary>
        /// Mask for left border color bits.
        /// </summary>
        private const uint DEF_BORDER_LEFT_COLOR_MASK = 0x0000007F;
        /// <summary>
        /// Mask for right border color bits.
        /// </summary>
        private const uint DEF_BORDER_RIGHT_COLOR_MASK = 0x00003F80;
        /// <summary>
        /// Mask for top border color bits.
        /// </summary>
        private const uint DEF_BORDER_TOP_COLOR_MASK = 0x007F0000;
        /// <summary>
        /// Mask for bottom border color bits.
        /// </summary>
        private const uint DEF_BORDER_BOTTOM_COLOR_MASK = 0x3F800000;

        /// <summary>
        /// Start bit of left border color bits.
        /// </summary>
        private const int DEF_BORDER_LEFT_COLOR_START = 0;
        /// <summary>
        /// Start bit of right border color bits.
        /// </summary>
        private const int DEF_BORDER_RIGHT_COLOR_START = 7;
        /// <summary>
        /// Start bit of top border color bits.
        /// </summary>
        private const int DEF_BORDER_TOP_COLOR_START = 16;
        /// <summary>
        /// Start bit of bottom border color bits.
        /// </summary>
        private const int DEF_BORDER_BOTTOM_COLOR_START = 23;

        /// <summary>
        /// Mask for fill pattern bits.
        /// </summary>
        private const ushort DEF_PATTERN_MASK = 0xFC00;
        /// <summary>
        /// Mask for pattern color bits.
        /// </summary>
        private const ushort DEF_PATTERN_COLOR_MASK = 0x007F;
        /// <summary>
        /// Mask for pattern backcolor bits.
        /// </summary>
        private const ushort DEF_PATTERN_BACKCOLOR_MASK = 0x3F80;

        /// <summary>
        /// Start bit of fill pattern bits.
        /// </summary>
        private const int DEF_PATTERN_START = 10;
        /// <summary>
        /// Start bit of fill pattern back color bits.
        /// </summary>
        private const int DEF_PATTERN_BACKCOLOR_START = 7;

        /// <summary>
        /// Size of the font block.
        /// </summary>
        private const int DEF_FONT_BLOCK_SIZE = DEF_FONT_FIRST_RESERVED_SIZE + 13
          + DEF_FONT_SECOND_RESERVED_SIZE + 20 + DEF_FONT_THIRD_RESERVED_SIZE + 2;
        /// <summary>
        /// Size of the border block.
        /// </summary>
        private const int DEF_BORDER_BLOCK_SIZE = 8;
        /// <summary>
        /// Size of the pattern block.
        /// </summary>
        private const int DEF_PATTERN_BLOCK_SIZE = 4;
        /// <summary>
        /// Size of the number format block.
        /// </summary>
        private const int DEF_NUMBER_FORMAT_BLOCK_SIZE = 2;
        /// <summary>
        /// Default color index.
        /// </summary>
        public const uint DefaultColorIndex = 0xFFFFFFFF;
        #endregion

        #region Members
        /// <summary>
        /// Option flags
        /// </summary>
        private byte m_uiOptions;
        /// <summary>
        /// Not used
        /// </summary>
        private byte m_usReserved;
        /// <summary>
        /// False if left border style and color are modified.
        /// </summary>
        private bool m_bLeftBorder = true;
        /// <summary>
        /// False if right border style and color are modified.
        /// </summary>
        private bool m_bRightBorder = true;
        /// <summary>
        /// False if top border style and color are modified.
        /// </summary>
        private bool m_bTopBorder = true;
        /// <summary>
        /// False if bottom border style and color are modified.
        /// </summary>
        private bool m_bBottomBorder = true;
        /// <summary>
        /// False if pattern style is modified.
        /// </summary>
        private bool m_bPatternStyle = true;
        /// <summary>
        /// False if pattern color is modified.
        /// </summary>
        private bool m_bPatternColor = true;
        /// <summary>
        /// False if pattern background color is modified.
        /// </summary>
        private bool m_bPatternBackColor = true;
        /// <summary>
        /// False if the number format is modified.
        /// </summary>
        private bool m_bNumberFormatModified = true;
        /// <summary>
        /// True if record contains number format.
        /// </summary>
        private bool m_bNumberFormatPresent = false;
        /// <summary>
        /// True if record contains font formatting block.
        /// </summary>
        private bool m_bFontFormat = false;
        /// <summary>
        /// True if record contains border formatting block.
        /// </summary>
        private bool m_bBorderFormat = false;
        /// <summary>
        /// True if record contains pattern formatting block.
        /// </summary>
        private bool m_bPatternFormat = false;
        /// <summary>
        /// True if record contains the user defined number format.
        /// </summary>
        private bool m_numberFormatIsUserDefined = false;

        #region Font Formatting Block
        /// <summary>
        /// Font height.
        /// </summary>
        private uint m_uiFontHeight = 0xFFFFFFFF;
        /// <summary>
        /// Font options.
        /// </summary>
        private uint m_uiFontOptions;
        /// <summary>
        /// Font weight (100-1000, only if font - style = 0).
        /// Standard values are 0190H (400) for normal text
        /// and 02BCH (700) for bold text.
        /// </summary>
        private ushort m_usFontWeight = 400;
        /// <summary>
        /// Escapement type (only if font - esc = 0):
        /// 0000H = None; 0001H = Superscript; 0002H = Subscript
        /// </summary>
        private ushort m_usEscapmentType;
        /// <summary>
        /// Underline type (only if font - underl = 0):
        /// 00H = None
        /// 01H = Single
        /// 02H = Double
        /// 21H = Single accounting
        /// 22H = Double accounting
        /// </summary>
        private byte m_Underline;
        /// <summary>
        /// Font color index or FFFFFFFFH to preserve the cell font color:
        /// </summary>
        private uint m_uiFontColorIndex = DefaultColorIndex;
        /// <summary>
        /// Option flags for modified font attributes:
        /// </summary>
        private uint m_uiModifiedFlags = 0x0000000F;
        /// <summary>
        /// 0 = Escapement type modified
        /// </summary>
        private uint m_uiEscapmentModified = 1;
        /// <summary>
        /// 0 = Underline type modified:
        /// </summary>
        private uint m_uiUnderlineModified = 1;
        #endregion

        #region Border Formatting Block
        /// <summary>
        /// Border line styles:
        /// </summary>
        private ushort m_usBorderLineStyles;
        /// <summary>
        /// Border line colour indexes:
        /// </summary>
        private uint m_uiBorderColors;
        #endregion

        #region Pattern Formatting Block
        /// <summary>
        /// Fill pattern style:
        /// </summary>
        private ushort m_usPatternStyle;
        /// <summary>
        /// Fill pattern color indexes:
        /// </summary>
        private ushort m_usPatternColors;
        #endregion

        #region Number Formatting Block
        /// <summary>
        /// Unused
        /// </summary>
        private ushort m_unUsed = 0;
        /// <summary>
        /// Unsigned integer that specifies the identifier of the number format.
        /// </summary>
        private ushort m_numFormatIndex;
        /// <summary>
        /// Unsigned integer that specifies the size of the user defined number format.
        /// </summary>
        private ushort m_userdefNumFormatSize;
        /// <summary>
        /// Unsigned integer that specifies character count of the number format string.
        /// </summary>
        private ushort m_charCount;
        /// <summary>
        /// Boolean that specifies whether the characters are in double byte character or not.
        /// </summary>
        private bool m_isHighByte=false;
        /// <summary>
        /// Number format string value.
        /// </summary>
        private string m_strValue;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public DXFN()
        {
        }
        #endregion

        #endregion Parse and serialization

        #region Parse and serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseDXFN(DataProvider provider, int iOffset, ExcelVersion version)
        {
            m_uiOptions = provider.ReadByte(iOffset);
            iOffset++;

            m_bLeftBorder = provider.ReadBit(iOffset, 2);
            m_bRightBorder = provider.ReadBit(iOffset, 3);
            m_bTopBorder = provider.ReadBit(iOffset, 4);
            m_bBottomBorder = provider.ReadBit(iOffset, 5);
            iOffset++;

            m_bPatternStyle = provider.ReadBit(iOffset, 0);
            m_bPatternColor = provider.ReadBit(iOffset, 1);
            m_bPatternBackColor = provider.ReadBit(iOffset, 2);
            m_bNumberFormatModified = provider.ReadBit(iOffset, 3);
            iOffset++;

            m_bNumberFormatPresent = provider.ReadBit(iOffset, 1);
            m_bFontFormat = provider.ReadBit(iOffset, 2);
            m_bBorderFormat = provider.ReadBit(iOffset, 4);
            m_bPatternFormat = provider.ReadBit(iOffset, 5);
            iOffset++;

            //iOffset += 4;

            m_numberFormatIsUserDefined = provider.ReadBit(iOffset, 0);
            iOffset++;

            m_usReserved = provider.ReadByte(iOffset);
            iOffset += 1;

            if (!m_numberFormatIsUserDefined)
            {
                iOffset = ParseNumberFormatBlock(provider, ref iOffset);
            }
            else
                iOffset = ParseUserdefinedNumberFormatBlock(provider, ref iOffset);

            iOffset = ParseFontBlock(provider, ref iOffset);
            iOffset = ParseBorderBlock(provider, ref iOffset);
            iOffset = ParsePatternBlock(provider, ref iOffset);

            return iOffset;
        }
        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int SerializeDXFN(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteByte(iOffset, m_uiOptions);
            iOffset++;

            provider.WriteBit(iOffset, m_bLeftBorder, 2);
            provider.WriteBit(iOffset, m_bRightBorder, 3);
            provider.WriteBit(iOffset, m_bTopBorder, 4);
            provider.WriteBit(iOffset, m_bBottomBorder, 5);
            iOffset++;

            provider.WriteBit(iOffset, m_bPatternStyle, 0);
            provider.WriteBit(iOffset, m_bPatternColor, 1);
            provider.WriteBit(iOffset, m_bPatternBackColor, 2);
            provider.WriteBit(iOffset, m_bNumberFormatModified, 3);
            iOffset++;

            provider.WriteBit(iOffset, m_bNumberFormatPresent, 1);
            provider.WriteBit(iOffset, m_bFontFormat, 2);
            provider.WriteBit(iOffset, m_bBorderFormat, 4);
            provider.WriteBit(iOffset, m_bPatternFormat, 5);
            iOffset++;

            provider.WriteBit(iOffset, m_numberFormatIsUserDefined, 0);
            iOffset++;
            //iOffset += 4;

            provider.WriteByte(iOffset, m_usReserved);
            iOffset++;

            if (!m_numberFormatIsUserDefined)
            {
                iOffset = SerializeNumberFormatBlock(provider, ref iOffset);
            }
            else
                iOffset = SerializeUserdefinedNumberFormatBlock(provider, ref iOffset);

            iOffset = SerializeFontBlock(provider, ref iOffset);
            iOffset = SerializeBorderBlock(provider, ref iOffset);
            iOffset = SerializePatternBlock(provider, ref iOffset);

            return iOffset;
        }

        /// <summary>
        /// Parses font block if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset to the font block data in the internal data array.
        /// </param>
        public int ParseFontBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bFontFormat) return iOffset;

            iOffset += DEF_FONT_FIRST_RESERVED_SIZE;

            m_uiFontHeight = provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_uiFontOptions = provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_usFontWeight = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_usEscapmentType = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_Underline = provider.ReadByte(iOffset);
            iOffset += 1;

            iOffset += DEF_FONT_SECOND_RESERVED_SIZE;

            m_uiFontColorIndex = provider.ReadUInt32(iOffset);
            iOffset += 4;

            iOffset += 4;

            m_uiModifiedFlags = provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_uiEscapmentModified = provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_uiUnderlineModified = provider.ReadUInt32(iOffset);
            iOffset += 4;

            //      SetByte( iOffset, 0, DEF_FONT_THIRD_RESERVED_SIZE );
            iOffset += DEF_FONT_THIRD_RESERVED_SIZE;

            //      SetUInt16( iOffset, 0x0001 );
            iOffset += 2;

            return iOffset;
        }
        /// <summary>
        /// Parses border block if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset to the border block data in the internal data array.
        /// </param>
        public int ParseBorderBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bBorderFormat) return iOffset;

            m_usBorderLineStyles = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_uiBorderColors = provider.ReadUInt32(iOffset);
            iOffset += 4;

            iOffset += 2;

            return iOffset;
        }
        /// <summary>
        /// Parses pattern block if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset to the pattern block data in the internal data array.
        /// </param>
        public int ParsePatternBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bPatternFormat) return iOffset;

            m_usPatternStyle = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_usPatternColors = provider.ReadUInt16(iOffset);
            iOffset += 2;

            return iOffset;
        }
        /// <summary>
        /// Parses number format block if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset to the number format block data in the internal data array.
        /// </param>
        public int ParseNumberFormatBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bNumberFormatPresent) return iOffset;

            m_unUsed = provider.ReadByte(iOffset);
            iOffset++;

            m_numFormatIndex = provider.ReadByte(iOffset);
            iOffset++;

            return iOffset;
        }

        /// <summary>
        /// Parses user defined number format block if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset to the number format block data in the internal data array.
        /// </param>
        public int ParseUserdefinedNumberFormatBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bNumberFormatPresent) return iOffset;

            m_userdefNumFormatSize = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_charCount = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_isHighByte = provider.ReadBit(iOffset, 0);
            iOffset++;

            int sizeOfArray = 0;
            if (!m_isHighByte)
                sizeOfArray = m_charCount;
            else
                sizeOfArray = 2 * m_charCount;

            m_strValue = provider.ReadString(iOffset, sizeOfArray, Encoding.UTF8, true);
            iOffset += sizeOfArray;

            return iOffset;
        }

        /// <summary>
        /// Writes font block into internal data array
        /// if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset where font block should be written.
        /// </param>
        public int SerializeFontBlock(DataProvider provider, ref int iOffset)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!m_bFontFormat) return iOffset;

            //SetByte( arrBuffer, iOffset, 0, DEF_FONT_FIRST_RESERVED_SIZE );
            for (int i = 0; i < DEF_FONT_FIRST_RESERVED_SIZE; i++, iOffset++)
            {
                provider.WriteByte(iOffset, 0);
            }

            //iOffset += DEF_FONT_FIRST_RESERVED_SIZE;

            provider.WriteUInt32(iOffset, m_uiFontHeight);
            iOffset += 4;

            provider.WriteUInt32(iOffset, m_uiFontOptions);
            iOffset += 4;

            provider.WriteUInt16(iOffset, m_usFontWeight);
            iOffset += 2;

            provider.WriteUInt16(iOffset, m_usEscapmentType);
            iOffset += 2;

            provider.WriteByte(iOffset, m_Underline);
            iOffset += 1;

            //provider.WriteByte( arrBuffer, iOffset, 0, DEF_FONT_SECOND_RESERVED_SIZE );
            for (int i = 0; i < DEF_FONT_SECOND_RESERVED_SIZE; i++, iOffset++)
            {
                provider.WriteByte(iOffset, 0);
            }
            //iOffset += DEF_FONT_SECOND_RESERVED_SIZE;

            provider.WriteUInt32(iOffset, m_uiFontColorIndex);
            iOffset += 4;

            provider.WriteUInt32(iOffset, 0);
            iOffset += 4;

            provider.WriteUInt32(iOffset, m_uiModifiedFlags);
            iOffset += 4;

            provider.WriteUInt32(iOffset, m_uiEscapmentModified);
            iOffset += 4;

            provider.WriteUInt32(iOffset, m_uiUnderlineModified);
            iOffset += 4;

            for (int i = 0; i < DEF_FONT_THIRD_RESERVED_SIZE; i++, iOffset++)
            {
                provider.WriteByte(iOffset, 0);
            }
            //      provider.WriteByte( arrBuffer, iOffset, 0, DEF_FONT_THIRD_RESERVED_SIZE );
            //iOffset += DEF_FONT_THIRD_RESERVED_SIZE;

            provider.WriteUInt16(iOffset, 0x0001);
            iOffset += 2;

            return iOffset;
        }
        /// <summary>
        /// Writes border block into internal data array
        /// if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset where border block should be written.
        /// </param>
        public int SerializeBorderBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bBorderFormat) return iOffset;

            if (provider == null)
                throw new ArgumentNullException("provider");

            provider.WriteUInt16(iOffset, m_usBorderLineStyles);
            iOffset += 2;

            provider.WriteUInt32(iOffset, m_uiBorderColors);
            iOffset += 4;

            provider.WriteUInt16(iOffset, 0);
            iOffset += 2;

            return iOffset;
        }
        /// <summary>
        /// Writes pattern block into internal data array
        /// if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset where pattern block should be written.
        /// </param>
        public int SerializePatternBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bPatternFormat) return iOffset;

            if (provider == null)
                throw new ArgumentNullException("provider");

            provider.WriteUInt16(iOffset, m_usPatternStyle);
            iOffset += 2;

            provider.WriteUInt16(iOffset, m_usPatternColors);
            iOffset += 2;

            return iOffset;
        }
        /// <summary>
        /// Writes number format block into internal data array
        /// if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset where number format block should be written.
        /// </param>
        public int SerializeNumberFormatBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bNumberFormatPresent) return iOffset;

            if (provider == null)
                throw new ArgumentNullException("provider");

            provider.WriteUInt16(iOffset, m_unUsed);
            iOffset++;

            provider.WriteUInt16(iOffset, m_numFormatIndex);
            iOffset++;

            return iOffset;
        }

        /// <summary>
        /// Writes number format block into internal data array
        /// if it is present in the conditional format.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">
        /// Offset where number format block should be written.
        /// </param>
        public int SerializeUserdefinedNumberFormatBlock(DataProvider provider, ref int iOffset)
        {
            if (!m_bNumberFormatPresent) return iOffset;

            if (provider == null)
                throw new ArgumentNullException("provider");

            provider.WriteUInt16(iOffset, m_userdefNumFormatSize);
            iOffset += 2;

            provider.WriteUInt16(iOffset, m_charCount);
            iOffset += 2;

            if (!m_isHighByte)
                provider.WriteByte(iOffset, 0);
            else
                provider.WriteByte(iOffset, 1);
            iOffset++;

            byte[] arrData = Encoding.UTF8.GetBytes(m_strValue);
            int iLength = arrData.Length;
            provider.WriteBytes(iOffset, arrData, 0, iLength);
            iOffset += iLength;

            return iOffset;
        }

        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public int GetStoreSize(ExcelVersion version)
        {
            int iSize = 6;
            if (m_bFontFormat)
            {
                iSize += DEF_FONT_BLOCK_SIZE;
            }

            if (m_bBorderFormat)
            {
                iSize += DEF_BORDER_BLOCK_SIZE;
            }

            if (m_bPatternFormat)
            {
                iSize += DEF_PATTERN_BLOCK_SIZE;
            }

            if (m_bNumberFormatPresent)
            {
                iSize += DEF_NUMBER_FORMAT_BLOCK_SIZE;
            }
            //iSize += 18;//for alignment and protection

            return iSize;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Serves as a hash function for a particular type, suitable for use in
        /// hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current Object.</returns>
        public int GetHashCode()
        {
            int iHashCode = m_uiOptions.GetHashCode()
                ^ m_usReserved.GetHashCode()
                ^ m_bLeftBorder.GetHashCode()
                ^ m_bRightBorder.GetHashCode()
                ^ m_bTopBorder.GetHashCode()
                ^ m_bBottomBorder.GetHashCode()
                ^ m_bPatternStyle.GetHashCode()
                ^ m_bPatternColor.GetHashCode()
                ^ m_bPatternBackColor.GetHashCode()
                ^ m_bNumberFormatModified.GetHashCode()
                ^ m_bNumberFormatPresent.GetHashCode()
                ^ m_bFontFormat.GetHashCode()
                ^ m_bBorderFormat.GetHashCode()
                ^ m_bPatternFormat.GetHashCode()

                ^ m_uiFontHeight.GetHashCode()
                ^ m_uiFontOptions.GetHashCode()
                ^ m_usFontWeight.GetHashCode()
                ^ m_usEscapmentType.GetHashCode()
                ^ m_Underline.GetHashCode()
                ^ m_uiFontColorIndex.GetHashCode()
                ^ m_uiModifiedFlags.GetHashCode()
                ^ m_uiEscapmentModified.GetHashCode()
                ^ m_uiUnderlineModified.GetHashCode()

                ^ m_usBorderLineStyles.GetHashCode()
                ^ m_uiBorderColors.GetHashCode()

                ^ m_usPatternStyle.GetHashCode()
                ^ m_usPatternColors.GetHashCode();

            return iHashCode;
        }
        #endregion
    }
    #endregion
}
