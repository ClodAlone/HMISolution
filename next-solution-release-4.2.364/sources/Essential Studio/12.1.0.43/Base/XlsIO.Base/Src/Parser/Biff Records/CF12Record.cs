#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Implementation;

#if  SILVERLIGHT || WP
using System.Windows.Media;
#elif ( WINRT )
using Windows.UI;
#else
using System.Drawing;
#endif


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    /// <summary>
    /// The begin record defines the start of a block of records for a (Graphing)
    /// data object. This record is matched with a corresponding EndRecord.
    /// </summary>
    [Biff(TBIFFRecord.CF12)]
    [Syncfusion.Documentation.DocumentationExclude()]
    [CLSCompliant(false)]
    public class CF12Record : BiffRecordRaw
    {
        #region Constants
        /// <summary>
        /// Minimum size of the record.
        /// </summary>
        private const int DEF_MINIMUM_RECORD_SIZE = 46;
        #endregion

        #region Class members
        /// <summary>
        /// Future header
        /// </summar.y>
        private FutureHeader m_header;
        /// <summary>
        /// A bit that specifies whether the containing record specifies a range of cells.
        /// It MUST be one of the following.
        /// 0 - The containing record does not specify a range of cells.
        /// 1 - The containing record specifies a range of cells.
        /// </summary>
        private bool m_isRange = false;
        /// <summary>
        /// A bit that specifies whether to alert the user of possible problems when saving the file.
        /// </summary>
        private bool m_isFutureAlert = false;
        /// <summary>
        /// Range of cells associated with the containing record.
        /// </summary>
        private TAddr m_addrEncloseRange = new TAddr();
        /// <summary>
        /// Type of the conditional formatting: 
        /// 
        /// 01H = Compare with current cell value 
        /// (the comparison specified below is used)
        /// 
        /// 02H = Evaluate a formula (condition is met 
        /// if formula evaluates to a value not equal to 0)
        /// 
        /// 03H = Color scale
        /// 04H = Data bar
        /// 05H = Filter
        /// 06H = Icon set
        /// </summary>
        private byte m_typeOfCondition=1;
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
        /// Size of the formula data for first value or formula.
        /// </summary>
        private ushort m_usFirstFormulaSize;
        /// <summary>
        /// Size of the formula data for second value or formula
        /// (sz2, used for second part of �Between� and �Not between�
        /// comparison, this field is 0 for other comparisons).
        /// </summary>
        private ushort m_usSecondFormulaSize;
        /// <summary>
        /// Formula data for first value or formula (RPN token array without size field):
        /// </summary>
        private byte[] m_arrFirstFormula = new byte[0];
        /// <summary>
        /// Formula data for second value or formula (RPN token array without size field):
        /// </summary>
        private byte[] m_arrSecondFormula = new byte[0];
        /// <summary>
        /// 
        /// </summary>
        private Ptg[] m_arrFirstFormulaParsed;
        /// <summary>
        /// 
        /// </summary>
        private Ptg[] m_arrSecondFormulaParsed;
        /// <summary>
        /// An integer that specifies the length of the formula.
        /// </summary>
        private ushort m_formulaLength;        
        /// <summary>
        /// Formula data (RPN token array without size field):
        /// </summary>
        private byte[] m_arrFormula = new byte[0];
        /// <summary>
        /// 
        /// </summary>
        private Ptg[] m_arrFormulaParsed;
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
        /// A bit that specifies whether, when a cell fulfills the condition corresponding to this rule, 
        /// the lower priority conditional formatting rules that apply to this cell are evaluated.
        /// </summary>
        private byte m_undefined = 0;
        /// <summary>
        /// An integer that specifies the priority of the rule.
        /// Rules that apply to the same cell are evaluated in increasing order of ipriority.
        /// </summary>
        private ushort m_priority;
        /// <summary>
        /// An integer that specifies the template from which the rule was created.
        ///         
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
        private ushort m_template;
        /// <summary>
        /// An integer that specifies the size of the rgbTemplate Parmeters field in bytes.
        /// </summary>
        private ushort m_templateParamCount = 16;
        /// <summary>
        /// CFEx Default parameter.
        /// </summary>
        private Int64 m_defaultParameter; 
        /// <summary>
        /// Reserved.
        /// </summary>
        private ushort m_reserved = 0xFFFF; 
        /// <summary>
        /// DXFN structure.
        /// </summary>
        private DXFN m_dxfn;
        /// <summary>
        /// CFEx filter template parameter.
        /// </summary>
        private CFExFilterParameter m_cfExFilterParam;
        /// <summary>
        /// CFEx Text template parameter.
        /// </summary>
        private CFExTextTemplateParameter m_cfExTextParam;
        /// <summary>
        /// CFEx Date template parameter.
        /// </summary>
        private CFExDateTemplateParameter m_cfExDateParam;
        /// <summary>
        /// CFEx Average template parameter.
        /// </summary>
        private CFExAverageTemplateParameter m_cfExAverageParam;
        /// <summary>
        /// Data bar.
        /// </summary>
        private DataBar m_dataBar;        
        /// <summary>
        /// Icon set.
        /// </summary>
        private CFIconSet m_iconSet;
        /// <summary>
        /// Color scale.
        /// </summary>
        private ColorScale m_colorScale;
        private IColorScale m_colorImpl;
        private IDataBar m_dataBarImpl;
        private IIconSet m_icondSetImpl;
        /// <summary>
        /// Check whtether the record is parsed.
        /// </summary>
        private bool m_isParsed = false;
        #endregion

        #region Class properties
        /// <summary>
        /// An integer that specifies the template from which the rule was created.        
        /// </summary>
        public ExcelCFType FormatType
        {
            get
            {
                return (ExcelCFType)m_typeOfCondition;
            }
            set
            {
                m_typeOfCondition = (byte)value;
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
        /// Size of the formula data for first value or formula. Read-only.
        /// </summary>
        public ushort FirstFormulaSize
        {
            get
            {
                return m_usFirstFormulaSize;
            }
        }
        /// <summary>
        /// Size of the formula data for second value or formula
        /// (sz2, used for second part of "Between" and "Not between"
        /// comparison, this field is 0 for other comparisons). Read-only.
        /// </summary>
        public ushort SecondFormulaSize
        {
            get
            {
                return m_usSecondFormulaSize;
            }
        }
        /// <summary>
        /// Parsed first formula string.
        /// </summary>
        public Ptg[] FirstFormulaPtgs
        {
            get
            {
                return m_arrFirstFormulaParsed;
            }
            set
            {
                m_arrFirstFormula = FormulaUtil.PtgArrayToByteArray(value, ExcelVersion.Excel2007);
                m_arrFirstFormulaParsed = value;
                m_usFirstFormulaSize = (ushort)m_arrFirstFormula.Length;
            }
        }
        /// <summary>
        /// Parsed second formula string.
        /// </summary>
        public Ptg[] SecondFormulaPtgs
        {
            get
            {                
                return m_arrSecondFormulaParsed;
            }
            set
            {
                m_arrSecondFormula = FormulaUtil.PtgArrayToByteArray(value, ExcelVersion.Excel2007);
                m_arrSecondFormulaParsed = value;
                m_usSecondFormulaSize = (ushort)m_arrSecondFormula.Length;
            }
        }
        /// <summary>
        ///  Returns bytes of the first formula. Read-only.
        /// </summary>
        public byte[] FirstFormulaBytes
        {
            get
            {
                return m_arrFirstFormula;
            }
        }
        /// <summary>
        ///  Returns bytes of the second formula. Read-only.
        /// </summary>
        public byte[] SecondFormulaBytes
        {
            get
            {
                return m_arrSecondFormula;
            }
        }
        /// <summary>
        /// Parsed formula string.
        /// </summary>
        public Ptg[] FormulaPtgs
        {
            get
            {
                return m_arrFormulaParsed;
            }
            set
            {
                m_arrFormula = FormulaUtil.PtgArrayToByteArray(value, ExcelVersion.Excel2007);
                m_arrFormulaParsed = value;
                m_formulaLength = (ushort)m_arrFormula.Length;
            }
        }
        /// <summary>
        ///  Returns bytes of the formula. Read-only.
        /// </summary>
        public byte[] FormulaBytes
        {
            get
            {
                return m_arrFormula;
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
        /// Color Scale implementation class.
        /// </summary>
        public IColorScale Criteria
        {
            get
            {
                return m_colorImpl;
            }
            set
            {
                m_colorImpl = value;
            }
        }
        /// <summary>
        /// Data var.
        /// </summary>
        public IDataBar DataBarImpl
        {
            get
            {
                return m_dataBarImpl;
            }
            set
            {
                m_dataBarImpl = value;
            }
        }
        /// <summary>
        /// Icon set.
        /// </summary>
        public IIconSet IconSetImpl
        {
            get
            {
                return m_icondSetImpl;
            }
            set
            {
                m_icondSetImpl = value;
            }
        }
        /// <summary>
        /// True if the record parsed.
        /// </summary>
        public bool IsParsed
        {
            get
            {
                return m_isParsed;
            }
            set
            {
                m_isParsed = value;
            }
        }
        /// <summary>
        /// CF12 Color Scale class.
        /// </summary>
        public ColorScale ColorScaleCF12
        {
            get
            {
                return m_colorScale;
            }
            set
            {
                m_colorScale = value;
            }
        }
        /// <summary>
        /// CF12 Data bar class.
        /// </summary>
        public DataBar DataBarCF12
        {
            get
            {
                return m_dataBar;
            }
            set
            {
                m_dataBar = value;
            }
        }
        /// <summary>
        /// CF12 Icon set class.
        /// </summary>
        public CFIconSet IconSetCF12
        {
            get
            {
                return m_iconSet;
            }
            set
            {
                m_iconSet = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Default constructor
        /// </summary>
        public CF12Record()
          : base()
        {
            m_header = new FutureHeader();
            m_header.Type = (ushort)TBIFFRecord.CF12;

            m_dxfn = new DXFN();
            m_colorScale = new ColorScale();
            m_dataBar = new DataBar();
            m_iconSet = new CFIconSet();
            m_properties = new List<ExtendedProperty>();
            m_cfExFilterParam = new CFExFilterParameter();
            m_cfExTextParam = new CFExTextTemplateParameter();
            m_cfExDateParam = new CFExDateTemplateParameter();
            m_cfExAverageParam = new CFExAverageTemplateParameter();
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
            IsParsed = true;
            //header
            m_header.Type = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_isRange = provider.ReadBit(iOffset, 0);
            m_isFutureAlert = provider.ReadBit(iOffset, 1);
            iOffset += 2;

            m_addrEncloseRange = provider.ReadAddr(iOffset);
            iOffset += 8;

            m_typeOfCondition = provider.ReadByte(iOffset);
            iOffset++;

            m_compareOperator = provider.ReadByte(iOffset);
            iOffset++;

            m_usFirstFormulaSize = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_usSecondFormulaSize = provider.ReadUInt16(iOffset);
            iOffset += 2;

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

            m_arrFirstFormula = new byte[m_usFirstFormulaSize];
            provider.ReadArray(iOffset, m_arrFirstFormula);
            iOffset += m_usFirstFormulaSize;

            m_arrSecondFormula = new byte[m_usSecondFormulaSize];
            provider.ReadArray(iOffset, m_arrSecondFormula);
            iOffset += m_usSecondFormulaSize;

            m_arrFirstFormulaParsed = FormulaUtil.ParseExpression(
              new ByteArrayDataProvider(m_arrFirstFormula), m_usFirstFormulaSize, version);

            m_arrSecondFormulaParsed = FormulaUtil.ParseExpression(
              new ByteArrayDataProvider(m_arrSecondFormula), m_usSecondFormulaSize, version);

            if (version != ExcelVersion.Excel2007)
            {
                // To compare records correctly regardless original version.
                if (m_usFirstFormulaSize > 0)
                {
                    m_arrFirstFormula = FormulaUtil.PtgArrayToByteArray(m_arrFirstFormulaParsed, ExcelVersion.Excel2007);
                    m_usFirstFormulaSize = (ushort)m_arrFirstFormula.Length;
                }

                if (m_usSecondFormulaSize > 0)
                {
                    m_arrSecondFormula = FormulaUtil.PtgArrayToByteArray(m_arrSecondFormulaParsed, ExcelVersion.Excel2007);
                    m_usSecondFormulaSize = (ushort)m_arrSecondFormula.Length;
                }
            }

            m_formulaLength = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_arrFormula = new byte[m_formulaLength];
            provider.ReadArray(iOffset, m_arrFormula);
            iOffset += m_formulaLength;

            m_arrFormulaParsed = FormulaUtil.ParseExpression(
              new ByteArrayDataProvider(m_arrFormula), m_formulaLength, version);

            if (version != ExcelVersion.Excel2007)
            {
                // To compare records correctly regardless original version.
                if (m_formulaLength > 0)
                {
                    m_arrFormula = FormulaUtil.PtgArrayToByteArray(m_arrFormulaParsed, ExcelVersion.Excel2007);
                    m_formulaLength = (ushort)m_arrFormula.Length;
                }
            }

            m_undefined = provider.ReadByte(iOffset);
            iOffset++;

            m_priority = provider.ReadUInt16(iOffset);
            iOffset += 2; 
            
            m_template = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_templateParamCount = provider.ReadByte(iOffset);
            iOffset++;

            iOffset=ParseCFExTemplateParameter(provider, iOffset, version);

            switch (FormatType)
            {
                case ExcelCFType.ColorScale:
                    m_colorScale = new ColorScale();
                    iOffset = m_colorScale.ParseColorScale(provider, iOffset, version);
                    break;

                case ExcelCFType.DataBar:
                    m_dataBar = new DataBar();
                    iOffset = m_dataBar.ParseDataBar(provider, iOffset, version);
                    break;

                case ExcelCFType.IconSet:
                    m_iconSet = new CFIconSet();
                    iOffset = m_iconSet.ParseIconSet(provider, iOffset, version);
                    break;
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
            m_iLength = GetStoreSize(version);

            if (m_arrFirstFormulaParsed != null && m_arrFirstFormulaParsed.Length > 0)
            {
                m_arrFirstFormula = FormulaUtil.PtgArrayToByteArray(m_arrFirstFormulaParsed, version);
                m_usFirstFormulaSize = (ushort)m_arrFirstFormula.Length;
            }
            else
            {
                m_arrFirstFormula = null;
                m_usFirstFormulaSize = 0;
            }

            if (m_arrSecondFormulaParsed != null && m_arrSecondFormulaParsed.Length > 0)
            {
                m_arrSecondFormula = FormulaUtil.PtgArrayToByteArray(m_arrSecondFormulaParsed, version);
                m_usSecondFormulaSize = (ushort)m_arrSecondFormula.Length;
            }
            else
            {
                m_arrSecondFormula = null;
                m_usSecondFormulaSize = 0;
            }

            if (m_arrFormulaParsed != null && m_arrFormulaParsed.Length > 0)
            {
                m_arrFormula = FormulaUtil.PtgArrayToByteArray(m_arrFormulaParsed, version);
                m_formulaLength = (ushort)m_arrFormula.Length;
            }
            else
            {
                m_arrFormula = null;
                m_formulaLength = 0;
            }

            //header
            provider.WriteUInt16(iOffset, m_header.Type);
            iOffset += 2;

            provider.WriteBit(iOffset, m_isRange, 0);
            provider.WriteBit(iOffset, m_isFutureAlert, 1);
            iOffset += 2;

            provider.WriteAddr(iOffset, m_addrEncloseRange);
            iOffset += 8;

            provider.WriteByte(iOffset, m_typeOfCondition);
            iOffset++;

            provider.WriteByte(iOffset, m_compareOperator);
            iOffset++;

            provider.WriteUInt16(iOffset, m_usFirstFormulaSize);
            iOffset += 2;

            provider.WriteUInt16(iOffset, m_usSecondFormulaSize);
            iOffset += 2;

            if (FormatType == ExcelCFType.ColorScale || FormatType == ExcelCFType.DataBar || FormatType == ExcelCFType.IconSet)
            {
                m_sizeOfDXF = 0;
            }
            
            provider.WriteUInt32(iOffset, m_sizeOfDXF);
            iOffset += 4;

            if (m_sizeOfDXF == 0)
            {
                provider.WriteUInt16(iOffset, 0);
                iOffset += 2;
            }

            int DxfnByteCount = iOffset;

            if (m_sizeOfDXF != 0)
            {
                iOffset = m_dxfn.SerializeDXFN(provider, iOffset, version);
            }
            DxfnByteCount = iOffset - DxfnByteCount;

            if (m_sizeOfDXF != DxfnByteCount)
            {
                provider.WriteUInt16(iOffset, 0);
                iOffset += 2;

                provider.WriteUInt16(iOffset, m_reserved);
                iOffset += 2;

                provider.WriteUInt16(iOffset, 0);
                iOffset += 2;

                provider.WriteUInt16(iOffset, m_propertyCount);
                iOffset += 2;

                //Serialize the extended properties
                foreach (ExtendedProperty property in m_properties)
                {
                    iOffset = property.InfillInternalData(provider, iOffset, version);
                }
            }
            
            provider.WriteBytes(iOffset, m_arrFirstFormula, 0, m_usFirstFormulaSize);
            iOffset += m_usFirstFormulaSize;

            provider.WriteBytes(iOffset, m_arrSecondFormula, 0, m_usSecondFormulaSize);
            iOffset += m_usSecondFormulaSize;

            provider.WriteUInt16(iOffset, m_formulaLength);
            iOffset += 2;

            provider.WriteBytes(iOffset, m_arrFormula, 0, m_formulaLength);
            iOffset += m_formulaLength;

            provider.WriteByte(iOffset, m_undefined);
            iOffset++;

            provider.WriteUInt16(iOffset, m_priority);
            iOffset += 2;

            provider.WriteUInt16(iOffset, m_template);
            iOffset += 2;

            provider.WriteByte(iOffset, (byte)m_templateParamCount);
            iOffset++;

            iOffset=SerializeCFExTemplateParameter(provider, iOffset, version);

            switch (FormatType)
            {
                case ExcelCFType.ColorScale:
                    if (Criteria!=null && Criteria.Criteria.Count != 0)
                        m_colorScale = new ColorScale();
                    iOffset = m_colorScale.SerializeColorScale(provider, iOffset, version, Criteria);
                    break;

                case ExcelCFType.DataBar:
                    if(m_dataBarImpl!=null)
                        m_dataBar = new DataBar();
                    iOffset = m_dataBar.SerializeDataBar(provider, iOffset, version,DataBarImpl);                   
                    break;

                case ExcelCFType.IconSet:
                    if(m_icondSetImpl!=null)
                        m_iconSet = new CFIconSet();
                    iOffset = m_iconSet.SerializeIconSet(provider, iOffset, version,IconSetImpl);
                    break;
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
                m_defaultParameter = provider.ReadInt64(iOffset);
                iOffset += 8;

                m_defaultParameter = provider.ReadInt64(iOffset);
                iOffset += 8;
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
            int DxfnByteCount = 0;
            int Size=DEF_MINIMUM_RECORD_SIZE;
            
            if (m_sizeOfDXF == 0)
            {
                Size += 2;
            }

            if(FormatType==ExcelCFType.ColorScale)
            {
                if (m_colorScale.ListCFInterpolationCurve.Count == 0)
                {
                    for (int i = 0; i < Criteria.Criteria.Count; i++)
                    {
                        CFInterpolationCurve curve = new CFInterpolationCurve();
                        curve.CFVO.CFVOType = Criteria.Criteria[i].Type;

                        CFGradientItem grad = new CFGradientItem();
                        Size = Size + curve.GetStoreSize(version)+grad.GetStoreSize(version);
                    }

                    Size = Size + m_colorScale.DefaultRecordSize;
                }
                else
                {
                    Size = Size+ m_colorScale.GetStoreSize(version);
                }
                Size += DVRecord.GetFormulaSize(m_arrFormulaParsed, version, true);
            }
            if(FormatType==ExcelCFType.DataBar)
            {
                if (m_dataBarImpl != null)
                {
                    m_dataBar.MinCFVO = new CFVO();
                    m_dataBar.MinCFVO.CFVOType = m_dataBarImpl.MinPoint.Type;

                    m_dataBar.MaxCFVO = new CFVO();
                    m_dataBar.MaxCFVO.CFVOType = m_dataBarImpl.MaxPoint.Type;
                }
                Size = Size + m_dataBar.GetStoreSize(version);
                Size += DVRecord.GetFormulaSize(m_arrFormulaParsed, version, true);
            }

            if (FormatType == ExcelCFType.IconSet)
            {
                if (m_iconSet.ListCFIconSet.Count == 0)
                {
                    for (int i = 0; i < IconSetImpl.IconCriteria.Count; i++)
                    {
                        CFIconMultiState icon = new CFIconMultiState();
                        icon.CFVO.CFVOType = IconSetImpl.IconCriteria[i].Type;
                        Size = Size + icon.GetStoreSize(version);
                    }
                    Size = Size + m_iconSet.DefaultRecordSize;
                }
                else
                {
                    Size = Size + m_iconSet.GetStoreSize(version);
                }
                Size += DVRecord.GetFormulaSize(m_arrFormulaParsed, version, true);
            }

            if (FormatType == ExcelCFType.CellValue)
            {
                Size += m_sizeOfDXF;

                Size += DVRecord.GetFormulaSize(m_arrFirstFormulaParsed, version, true);
                Size += DVRecord.GetFormulaSize(m_arrSecondFormulaParsed, version, true);
            }

            return Size;
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
              ^ m_isRange.GetHashCode()
              ^ m_isFutureAlert.GetHashCode()
              ^ m_addrEncloseRange.GetHashCode()
              ^ m_typeOfCondition.GetHashCode()
              ^ m_compareOperator.GetHashCode()
              ^ m_usFirstFormulaSize.GetHashCode()
              ^ m_usSecondFormulaSize.GetHashCode()

              ^ m_sizeOfDXF.GetHashCode()
              ^ m_dxfn.GetHashCode()

              ^ m_propertyCount.GetHashCode()
              ^ m_properties.GetHashCode()
              ^ m_arrFirstFormula.GetHashCode()
              ^ m_arrSecondFormula.GetHashCode()
              ^ m_formulaLength.GetHashCode()
              ^ m_arrFormula.GetHashCode()
              ^ m_undefined.GetHashCode()
              ^ m_priority.GetHashCode()
              ^ m_template.GetHashCode()
              ^ m_templateParamCount.GetHashCode()
              ^ (m_cfExTextParam.GetHashCode() ^ m_cfExDateParam.GetHashCode() ^ m_cfExFilterParam.GetHashCode() ^ m_cfExAverageParam.GetHashCode())
              ^ (m_colorScale.GetHashCode() ^ m_dataBar.GetHashCode() ^ m_iconSet.GetHashCode());

            return iHashCode;
        }
        /// <summary>
        /// A hash code for the current Object without taking cell list into account.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            CF12Record toCompare = obj as CF12Record;

            if (toCompare == null) return false;

            bool bResult = m_typeOfCondition == toCompare.m_typeOfCondition
              && m_compareOperator == toCompare.m_compareOperator
              && m_usFirstFormulaSize == toCompare.m_usFirstFormulaSize
              && m_usSecondFormulaSize == toCompare.m_usSecondFormulaSize
              && m_sizeOfDXF == toCompare.m_sizeOfDXF
              && m_dxfn==toCompare.m_dxfn
              && m_propertyCount == toCompare.m_propertyCount
              && m_properties == toCompare.m_properties

              && m_arrFirstFormula == toCompare.m_arrFirstFormula
              && m_arrSecondFormula == toCompare.m_arrSecondFormula
              && m_formulaLength == toCompare.m_formulaLength
              && m_arrFormula == toCompare.m_arrFormula
              && m_undefined == toCompare.m_undefined
              && m_priority == toCompare.m_priority
              && m_template == toCompare.m_template
              && m_templateParamCount == toCompare.m_templateParamCount
              && (m_cfExAverageParam == toCompare.m_cfExAverageParam && m_cfExDateParam == toCompare.m_cfExDateParam && m_cfExFilterParam == toCompare.m_cfExFilterParam && m_cfExTextParam == toCompare.m_cfExTextParam)
              && (m_colorScale == toCompare.m_colorScale && m_dataBar == toCompare.m_dataBar && m_iconSet == toCompare.m_iconSet);

            return bResult;
        }

        #endregion

        internal void ClearAll()
        {
            m_iconSet.ClearAll();
            m_iconSet = null;
            m_header = null;
            m_dxfn = null;
            m_arrFirstFormulaParsed = null;
            m_arrFirstFormula = null;
            m_arrFormula = null;
            m_arrSecondFormula = null;
            m_arrSecondFormulaParsed = null;
        }
    }   

    /// <summary>
    /// Color scale.
    /// </summary>
    public class ColorScale
    {
        #region Constants
        /// <summary>
        /// Minimum size of the structure.
        /// </summary>
        private const ushort DEF_MINIMUM_SIZE = 6;
        #endregion

        #region Members
        /// <summary>
        /// Undefined.
        /// </summary>
        private ushort m_undefined=0;
        /// <summary>
        /// Interpolation point count.
        /// </summary>
        private byte m_interpCurve;
        /// <summary>
        /// Gradient point count.
        /// </summary>
        private byte m_gradient;
        /// <summary>
        /// Minimum or the maximum of the interpolation curve is used instead of the cell value.
        /// </summary>
        private bool m_clamp = true;
        /// <summary>
        /// Color scale formatting applies to the background of the cells.
        /// </summary>
        private bool m_background = true;
        /// <summary>
        /// clamp and background.
        /// </summary>
        private byte m_clampAndBackground=3;
        /// <summary>
        /// List of Conditional format interpolation curve.
        /// </summary>
        List<CFInterpolationCurve> m_arrCFInterp = new List<CFInterpolationCurve>();
        /// <summary>
        /// List of Conditional format gradient curve.
        /// </summary>
        List<CFGradientItem> m_arrCFGradient = new List<CFGradientItem>();
        private ColorScaleImpl m_colorScale;
        #endregion 
       
        #region Properties
        /// <summary>
        /// Interpolation curve.
        /// </summary>
        public List<CFInterpolationCurve> ListCFInterpolationCurve
        {
            get
            {
                return m_arrCFInterp;
            }
            set
            {
                m_arrCFInterp = value;
            }
        }
        /// <summary>
        /// Gradient Item.
        /// </summary>
        public List<CFGradientItem> ListCFGradientItem
        {
            get
            {
                return m_arrCFGradient;
            }
            set
            {
                m_arrCFGradient = value;
            }
        }
        public ushort DefaultRecordSize
        {
            get
            {
                return DEF_MINIMUM_SIZE;
            }
        }
        public IColorScale ColorScaleImpl
        {
            get
            {
                return m_colorScale;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorScale()
        {
            m_arrCFInterp = new List<CFInterpolationCurve>();
            m_arrCFGradient = new List<CFGradientItem>();
            m_colorScale = new ColorScaleImpl();
        }
        /// <summary>
        /// Copy color scale.
        /// </summary>
        private void CopyColorScale()
        {
            m_colorScale.Criteria.Clear();
            for(int i=0;i<ListCFInterpolationCurve.Count;i++)
            {
                CFInterpolationCurve curve = ListCFInterpolationCurve[i];

                ColorConditionValue condValue = new ColorConditionValue();
                condValue.Type = curve.CFVO.CFVOType;
                condValue.Value = curve.CFVO.Value;
                m_colorScale.Criteria.Add(condValue);
            }

            for (int j = 0; j < ListCFGradientItem.Count; j++)
            {
                CFGradientItem grad = ListCFGradientItem[j];
                m_colorScale.Criteria[j].FormatColorRGB = ConvertRGBAToARGB(UIntToColor(grad.ColorValue));
            }
        }
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of template parameter. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseColorScale(DataProvider provider, int iOffset, ExcelVersion version)
        {
            ushort reserved = 0;

            m_undefined = provider.ReadUInt16(iOffset);
            iOffset += 2;

            reserved = provider.ReadByte(iOffset);
            iOffset++;

            m_interpCurve = provider.ReadByte(iOffset);
            iOffset ++;

            m_gradient = provider.ReadByte(iOffset);
            iOffset++;

            m_clampAndBackground = provider.ReadByte(iOffset);
            iOffset++;

            for (int i = 0; i < m_interpCurve; i++)
            {
                CFInterpolationCurve curve = new CFInterpolationCurve();
                iOffset = curve.ParseCFGradientInterp(provider, iOffset, version);
                m_arrCFInterp.Add(curve);
            }

            for (int i = 0; i < m_gradient; i++)
            {
                CFGradientItem grad = new CFGradientItem();
                iOffset = grad.ParseCFGradient(provider, iOffset, version);
                m_arrCFGradient.Add(grad);
            }

            CopyColorScale();
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
        public int SerializeColorScale(DataProvider provider, int iOffset, ExcelVersion version,IColorScale m_iColorScale)
        {
            bool isParsed = true;
            if (ListCFInterpolationCurve.Count == 0)
            {
                isParsed = false;
                UpdateColorScaleColor(m_iColorScale);
            }

            provider.WriteUInt16(iOffset, m_undefined);
            iOffset += 2;

            provider.WriteByte(iOffset, 0);
            iOffset++;

            provider.WriteByte(iOffset, (byte)ListCFInterpolationCurve.Count);
            iOffset++;

            provider.WriteByte(iOffset, (byte)ListCFGradientItem.Count);
            iOffset++;

            provider.WriteByte(iOffset,m_clampAndBackground);
            iOffset++;
            int position = 1;
            double numValue = 0.0;
            foreach (CFInterpolationCurve curve in ListCFInterpolationCurve)
            {
                numValue=CalculateNumValue(position);
                iOffset = curve.SerializeCFGradientInterp(provider, iOffset, version, numValue,isParsed);
                position++;
            }

            int pos = 1;
            foreach (CFGradientItem grad in ListCFGradientItem)
            {
                numValue = CalculateNumValue(pos);
                iOffset = grad.SerializeCFGradient(provider, iOffset, version, numValue,isParsed);
                pos++;
            }

            return iOffset;
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public int GetStoreSize(ExcelVersion version)
        {
            int lenCurve = 0;
            foreach (CFInterpolationCurve curve in ListCFInterpolationCurve)
                lenCurve += curve.GetStoreSize(version);

            int gradLen = 0;
            foreach (CFGradientItem grad in ListCFGradientItem)
                gradLen += grad.GetStoreSize(version);

            return DEF_MINIMUM_SIZE + lenCurve + gradLen;
        }
        /// <summary>
        /// Calculate Num value.
        /// </summary>
        private double CalculateNumValue(int position)
        {
            double numValue = 0.0;
            if (ListCFInterpolationCurve.Count == 3)
            {
                if (position == 1)
                    numValue = 0.0;
                if (position == 2)
                    numValue = 0.5;
                if (position == 3)
                    numValue = 1.0;
            }
            if (ListCFInterpolationCurve.Count == 2)
            {
                if (position == 1)
                    numValue = 0.0;
                if (position == 2)
                    numValue = 1.0;
            }
            return numValue;
        }
        /// <summary>
        /// Update criteria.
        /// </summary>
        public void UpdateColorScaleColor(IColorScale m_colorScale)
        {
            if (m_colorScale != null)
            {
                ListCFInterpolationCurve.Clear();
                ListCFGradientItem.Clear();

                foreach (IColorConditionValue cond in m_colorScale.Criteria)
                {
                    CFInterpolationCurve curve = new CFInterpolationCurve();
                    curve.CFVO.CFVOType = cond.Type;
                    curve.CFVO.Value = cond.Value;
                    ListCFInterpolationCurve.Add(curve);

                    CFGradientItem grad = new CFGradientItem();
                    grad.ColorType = ColorType.RGB;
                    Color colorValue = ConvertARGBToRGBA(cond.FormatColorRGB);
                    grad.ColorValue = ColorToUInt(colorValue);
                    ListCFGradientItem.Add(grad);
                }
            }
        }
        /// <summary>
        /// Convert Color object to unsigned integer.
        /// </summary>
        private uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }
        /// <summary>
        /// Convert unsigned integer to Color object.
        /// </summary>
        private Color UIntToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// Convert ARGB to RGBA.
        /// </summary>
        private Color ConvertARGBToRGBA(Color colorValue)
        {
            //convert ARGB to RGBA        
            byte New_R = colorValue.B;
            byte New_G = colorValue.G;
            byte New_B = colorValue.R;
            byte New_A = colorValue.A;
            colorValue = Color.FromArgb(New_A, New_R, New_G, New_B);

            return colorValue;
        }
        /// <summary>
        /// Convert ARGB to RGBA.
        /// </summary>
        private Color ConvertRGBAToARGB(Color colorValue)
        {
            //convert ARGB to RGB  
            byte New_A = colorValue.A;
            byte New_R = colorValue.B;
            byte New_G = colorValue.G;
            byte New_B = colorValue.R;            
            colorValue = Color.FromArgb(New_A,New_R, New_G, New_B);

            return colorValue;
        }
        #endregion

        internal void ClearAll()
        {
            m_arrCFInterp.Clear();
            m_arrCFGradient.Clear();
            m_colorScale = null;

            m_arrCFInterp = null;
            m_arrCFGradient = null;

        }
    }

    /// <summary>
    /// Data bar.
    /// </summary>
    public class DataBar
    {
        #region Constants
        /// <summary>
        /// Minimum size of the structure.
        /// </summary>
        private const ushort DEF_MINIMUM_SIZE = 22;
        #endregion

        #region Members
        /// <summary>
        /// Undefined.
        /// </summary>
        private ushort m_undefined;
        /// <summary>
        /// Specifies whether the data bars are drawn starting from the right of the cell.
        /// </summary>
        private bool m_isRightToLeft = false;
        /// <summary>
        /// Specifies whether the numerical value of the cell appears in the cell along with the data bar.
        /// </summary>
        private bool m_isShowValue = true;
        /// <summary>
        /// Minimum percent length of data bar.
        /// </summary>
        private byte m_minDatabarLen;
        /// <summary>
        /// Maximum percent length of data bar.
        /// </summary>
        private byte m_MaxDatabarLen;
        /// <summary>
        /// Type of color.
        /// </summary>
        private UInt32 m_colorType;
        /// <summary>
        /// Color value.
        /// </summary>
        private UInt32 m_colorValue;
        /// <summary>
        /// Tint and shade.
        /// </summary>
        private Int64 m_tintShade;
        /// <summary>
        /// CFVO structure.
        /// </summary>
        private CFVO m_cfvoMin;
        /// <summary>
        /// CFVO structure.
        /// </summary>
        private CFVO m_cfvoMax;
        private DataBarImpl m_dataBar;
        #endregion

        #region Properties
        /// <summary>
        /// Type of color.
        /// </summary>
        public ColorType ColorType
        {
            get
            {
                return (ColorType)m_colorType;
            }
            set
            {
                m_colorType = (byte)value;
            }
        }
        /// <summary>
        /// Color value.
        /// </summary>
        public UInt32 ColorValue
        {
            get
            {
                return m_colorValue;
            }
            set
            {
                m_colorValue = value;

            }
        }
        /// <summary>
        /// Tint and shade.
        /// </summary>
        public Int64 TintShade
        {
            get
            {
                return m_tintShade;
            }
            set
            {
                m_tintShade = value;
            }
        }
        /// <summary>
        /// Min CFVO.
        /// </summary>
        public CFVO MinCFVO
        {
            get
            {
                return m_cfvoMin;
            }
            set
            {
                m_cfvoMin = value;
            }
        }
        /// <summary>
        /// Max CFVO.
        /// </summary>
        public CFVO MaxCFVO
        {
            get
            {
                return m_cfvoMax;
            }
            set
            {
                m_cfvoMax = value;
            }
        }
        /// <summary>
        /// Data bar implementation class.
        /// </summary>
        public IDataBar DataBarImpl
        {
            get
            {
                return m_dataBar;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataBar()
        {
            m_cfvoMin = new CFVO();
            m_cfvoMax = new CFVO();
            m_dataBar = new DataBarImpl();
        }
        /// <summary>
        /// Copy data bar.
        /// </summary>
        private void CopyDataBar()
        {
            m_dataBar.MinPoint.Type = m_cfvoMin.CFVOType;
            m_dataBar.MinPoint.Value = m_cfvoMin.Value;

            m_dataBar.MaxPoint.Type = m_cfvoMax.CFVOType;
            m_dataBar.MaxPoint.Value = m_cfvoMax.Value;

            m_dataBar.BarColor = ConvertRGBAToARGB(UIntToColor(m_colorValue));
            m_dataBar.ShowValue = (!m_isShowValue);
            m_dataBar.PercentMin = m_minDatabarLen;
            m_dataBar.PercentMax = m_MaxDatabarLen;            
        }
        /// <summary>
        /// Convert ARGB to RGBA.
        /// </summary>
        private Color ConvertRGBAToARGB(Color colorValue)
        {
            //convert ARGB to RGB  
            byte New_A = colorValue.A;
            byte New_R = colorValue.B;
            byte New_G = colorValue.G;
            byte New_B = colorValue.R;
            colorValue = Color.FromArgb(New_A, New_R, New_G, New_B);

            return colorValue;
        }
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of template parameter. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseDataBar(DataProvider provider, int iOffset, ExcelVersion version)
        {
            ushort reserved = 0;

            m_undefined = provider.ReadUInt16(iOffset);
            iOffset += 2;

            reserved = provider.ReadByte(iOffset);
            iOffset++;

            m_isRightToLeft = provider.ReadBit(iOffset, 0);
            m_isShowValue = provider.ReadBit(iOffset, 1);
            iOffset++;

            m_minDatabarLen = provider.ReadByte(iOffset);
            iOffset++;

            m_MaxDatabarLen = provider.ReadByte(iOffset);
            iOffset++;

            m_colorType = provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_colorValue = provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_tintShade = provider.ReadInt64(iOffset);
            iOffset += 8;

            m_cfvoMin = new CFVO();
            iOffset=m_cfvoMin.ParseCFVO(provider, iOffset, version);

            m_cfvoMax = new CFVO();
            iOffset = m_cfvoMax.ParseCFVO(provider, iOffset, version);

            CopyDataBar();
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
        public int SerializeDataBar(DataProvider provider, int iOffset, ExcelVersion version,IDataBar m_iDatabar)
        {
            if (m_iDatabar != null)
            {
                MinCFVO = new CFVO();
                MinCFVO.CFVOType = m_iDatabar.MinPoint.Type;
                MinCFVO.Value = m_iDatabar.MinPoint.Value;

                MaxCFVO = new CFVO();
                MaxCFVO.CFVOType = m_iDatabar.MaxPoint.Type;
                MaxCFVO.Value = m_iDatabar.MaxPoint.Value;

                m_minDatabarLen = (byte)m_iDatabar.PercentMin;
                m_MaxDatabarLen = (byte)m_iDatabar.PercentMax;

                m_colorType = (byte)ColorType.RGB;
                Color colorValue = ConvertARGBToRGBA(m_iDatabar.BarColor);
                m_colorValue = ColorToUInt(colorValue);

                m_isShowValue = m_iDatabar.ShowValue;
            }
            provider.WriteUInt16(iOffset, m_undefined);
            iOffset += 2;

            provider.WriteByte(iOffset, 0);
            iOffset++;
            
            byte value = 0;
            if ((!m_isShowValue) && m_isRightToLeft)
                value = 3;
            if ((!m_isShowValue) && (!m_isRightToLeft))
                value = 2;
            if ((m_isShowValue) && m_isRightToLeft)
                value = 1;

            provider.WriteByte(iOffset, value);
            iOffset++;

            provider.WriteByte(iOffset, m_minDatabarLen);
            iOffset++;

            provider.WriteByte(iOffset, m_MaxDatabarLen);
            iOffset++;

            provider.WriteUInt32(iOffset, m_colorType);
            iOffset += 4;

            provider.WriteUInt32(iOffset, m_colorValue);
            iOffset += 4;

            provider.WriteInt64(iOffset, m_tintShade);
            iOffset += 8;

            iOffset = m_cfvoMin.SerializeCFVO(provider, iOffset, version);

            iOffset = m_cfvoMax.SerializeCFVO(provider, iOffset, version);

            return iOffset;
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public int GetStoreSize(ExcelVersion version)
        {
            return DEF_MINIMUM_SIZE + m_cfvoMin.GetStoreSize(version) + m_cfvoMax.GetStoreSize(version);
        }        
        /// <summary>
        /// Convert Color object to unsigned integer.
        /// </summary>
        private uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }
        /// <summary>
        /// Convert unsigned integer to Color object.
        /// </summary>
        private Color UIntToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// Convert ARGB to RGBA.
        /// </summary>
        private Color ConvertARGBToRGBA(Color colorValue)
        {
            //convert ARGB to RGBA        
            byte New_R = colorValue.B;
            byte New_G = colorValue.G;
            byte New_B = colorValue.R;
            byte New_A = colorValue.A;
            colorValue = Color.FromArgb(New_A, New_R, New_G, New_B);

            return colorValue;
        }
        #endregion

        internal void ClearAll()
        {
            m_cfvoMax.ClearAll();
            m_cfvoMin.ClearAll();
            m_dataBar = null;
            m_cfvoMin = null;
            m_cfvoMax = null;
        }
    }

    /// <summary>
    /// Icon set.
    /// </summary>
    public class CFIconSet
    {
        #region Constants
        /// <summary>
        /// Minimum size of the structure.
        /// </summary>
        private const ushort DEF_MINIMUM_SIZE = 6;
        #endregion

        #region Members
        /// <summary>
        /// Undefined.
        /// </summary>
        private ushort m_undefined;
        /// <summary>
        /// Specifies the number of items in the icon set..
        /// </summary>
        private byte m_iconStates;
        /// <summary>
        /// integer that specifies the icon set that represents the cell values.
        /// </summary>
        private byte m_iconSet=0;
        /// <summary>
        /// Specifies whether only the icon will be displayed in the sheet and that the cell value will be hidden.
        /// </summary>
        private bool m_isIconOnly = false;
        /// <summary>
        /// specifies whether the order of the icons in the set is reversed.
        /// </summary>
        private bool m_iconIsReversed = false;
        /// <summary>
        /// List of CF icon MultiState.
        /// </summary>
        private List<CFIconMultiState> m_arrMultistate = new List<CFIconMultiState>();
        private IconSetImpl m_iconSetImpl;
        #endregion        

        #region Properties
        /// <summary>
        /// Specifies the icon set that represents the cell values.
        /// </summary>
        public ExcelIconSetType IconSetType
        {
            get
            {
                return (ExcelIconSetType)m_iconSet;
            }
            set
            {
                m_iconSet = (byte)value;
            }
        }
        /// <summary>
        /// List of CF icon MultiState.
        /// </summary>
        public List<CFIconMultiState> ListCFIconSet
        {
            get
            {
                return m_arrMultistate;
            }
            set
            {
                m_arrMultistate = value;
            }
        }
        /// <summary>
        /// Default minimum structure size.
        /// </summary>
        public ushort DefaultRecordSize
        {
            get
            {
                return DEF_MINIMUM_SIZE;
            }
        }
        /// <summary>
        /// Data bar implementation class.
        /// </summary>
        public IIconSet IconsetImpl
        {
            get
            {
                return m_iconSetImpl;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CFIconSet()
        {
            m_arrMultistate = new List<CFIconMultiState>();
            m_iconSetImpl = new IconSetImpl();
        }
        /// <summary>
        /// Copy IconSet.
        /// </summary>
        private void CopyIconSet()
        {
            m_iconSetImpl.IconSet = IconSetType;
            m_iconSetImpl.ShowIconOnly = m_isIconOnly;
            m_iconSetImpl.ReverseOrder = m_iconIsReversed;

            for (int i = 0; i < m_arrMultistate.Count; i++)
            {
                CFIconMultiState icon = m_arrMultistate[i];

                ConditionValue condValue = new ConditionValue();
                m_iconSetImpl.IconCriteria[i].Type = icon.CFVO.CFVOType;
                m_iconSetImpl.IconCriteria[i].Value = icon.CFVO.Value;
            }
        }
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of template parameter. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseIconSet(DataProvider provider, int iOffset, ExcelVersion version)
        {
            ushort reserved = 0;

            m_undefined = provider.ReadUInt16(iOffset);
            iOffset += 2;

            reserved = provider.ReadByte(iOffset);
            iOffset++;

            m_iconStates = provider.ReadByte(iOffset);
            iOffset++;

            m_iconSet = provider.ReadByte(iOffset);
            iOffset++;

            m_isIconOnly = provider.ReadBit(iOffset, 0);
            bool reservd = provider.ReadBit(iOffset, 1);
            m_iconIsReversed = provider.ReadBit(iOffset, 2);
            iOffset++;

            for (int i = 0; i < m_iconStates; i++)
            {
                CFIconMultiState icon = new CFIconMultiState();
                iOffset = icon.ParseCFIconMultistate(provider, iOffset, version);
                m_arrMultistate.Add(icon);
            }

            CopyIconSet();
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
        public int SerializeIconSet(DataProvider provider, int iOffset, ExcelVersion version,IIconSet iIconSet)
        {
            if (iIconSet != null)
            {
                ListCFIconSet=UpdateIconSet(iIconSet);
            }

            provider.WriteUInt16(iOffset, m_undefined);
            iOffset += 2;

            provider.WriteByte(iOffset, 0);
            iOffset++;

            provider.WriteByte(iOffset, m_iconStates);
            iOffset++;

            provider.WriteByte(iOffset, m_iconSet);
            iOffset++;

            provider.WriteByte(iOffset, CalculateIconOnlyAndReverseOrder());
            iOffset++;

            foreach (CFIconMultiState icon in ListCFIconSet)
            {
                iOffset = icon.SerializeCFIconMultistate(provider, iOffset, version);
            }

            return iOffset;
        }
        /// <summary>
        /// Calucate IsIconOnly and reverse order byte.
        /// </summary>
        private byte CalculateIconOnlyAndReverseOrder()
        {
            byte value = 0;

            if (m_iconIsReversed && m_isIconOnly)
                value = 5;
            if (m_iconIsReversed && (!m_isIconOnly))
                value = 4;
            if ((!m_iconIsReversed) && m_isIconOnly)
                value = 1;

            return value;
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public int GetStoreSize(ExcelVersion version)
        {
            int startLen = 0;
            foreach (CFIconMultiState icon in m_arrMultistate)
            {
                startLen = startLen + icon.GetStoreSize(version);
            }

            return DEF_MINIMUM_SIZE + startLen;
        }
        /// <summary>
        /// Update Icon Set from IIconSet.
        /// </summary>
        private List<CFIconMultiState> UpdateIconSet(IIconSet updateIconset)
        {
            List<CFIconMultiState> listIcon = new List<CFIconMultiState>();
            switch (updateIconset.IconSet)
            {
                case ExcelIconSetType.ThreeArrows:
                    IconSetType = ExcelIconSetType.ThreeArrows;
                    m_iconStates = 3;
                    break;

                case ExcelIconSetType.ThreeArrowsGray:
                    IconSetType = ExcelIconSetType.ThreeArrowsGray;
                    m_iconStates = 3;
                    break;

                case ExcelIconSetType.ThreeFlags:
                    IconSetType = ExcelIconSetType.ThreeFlags;
                    m_iconStates = 3;
                    break;

                case ExcelIconSetType.ThreeTrafficLights1:
                    IconSetType = ExcelIconSetType.ThreeTrafficLights1;
                    m_iconStates = 3;
                    break;

                case ExcelIconSetType.ThreeTrafficLights2:
                    IconSetType = ExcelIconSetType.ThreeTrafficLights2;
                    m_iconStates = 3;
                    break;

                case ExcelIconSetType.ThreeSigns:
                    IconSetType = ExcelIconSetType.ThreeSigns;
                    m_iconStates = 3;
                    break;

                case ExcelIconSetType.ThreeSymbols:
                    IconSetType = ExcelIconSetType.ThreeSymbols;
                    m_iconStates = 3;
                    break;

                case ExcelIconSetType.ThreeSymbols2:
                    IconSetType = ExcelIconSetType.ThreeSymbols2;
                    m_iconStates = 3;
                    break;

                case ExcelIconSetType.FourArrows:
                    IconSetType = ExcelIconSetType.FourArrows;
                    m_iconStates = 4;
                    break;

                case ExcelIconSetType.FourArrowsGray:
                    IconSetType = ExcelIconSetType.FourArrowsGray;
                    m_iconStates = 4;
                    break;

                case ExcelIconSetType.FourRedToBlack:
                    IconSetType = ExcelIconSetType.FourRedToBlack;
                    m_iconStates = 4;
                    break;

                case ExcelIconSetType.FourRating:
                    IconSetType = ExcelIconSetType.FourRating;
                    m_iconStates = 4;
                    break;

                case ExcelIconSetType.FourTrafficLights:
                    IconSetType = ExcelIconSetType.FourTrafficLights;
                    m_iconStates = 4;
                    break;

                case ExcelIconSetType.FiveArrows:
                    IconSetType = ExcelIconSetType.FiveArrows;
                    m_iconStates = 5;
                    break;

                case ExcelIconSetType.FiveArrowsGray:
                    IconSetType = ExcelIconSetType.FiveArrowsGray;
                    m_iconStates = 5;
                    break;

                case ExcelIconSetType.FiveRating:
                    IconSetType = ExcelIconSetType.FiveRating;
                    m_iconStates = 5;
                    break;

                case ExcelIconSetType.FiveQuarters:
                    IconSetType = ExcelIconSetType.FiveQuarters;
                    m_iconStates = 5;
                    break;
            }

            m_isIconOnly = updateIconset.ShowIconOnly;
            m_iconIsReversed = updateIconset.ReverseOrder;            

            if (updateIconset != null)
            {
                ListCFIconSet.Clear();

                foreach (IConditionValue cond in updateIconset.IconCriteria)
                {
                    CFIconMultiState icon = new CFIconMultiState();
                    icon.CFVO.CFVOType = cond.Type;
                    icon.CFVO.Value = cond.Value;
                    icon.IsEqulal = (byte)cond.Operator;
                    listIcon.Add(icon);
                }
            }

            return listIcon;
        }
        /// <summary>
        /// Update criteria.
        /// </summary>
        public void UpdateIconSetColor(IList<IConditionValue> m_IconCriteria)
        {
            if (m_IconCriteria != null)
            {
                ListCFIconSet.Clear();

                foreach (IConditionValue cond in m_IconCriteria)
                {
                    CFIconMultiState icon = new CFIconMultiState();
                    icon.CFVO.CFVOType = cond.Type;
                    icon.CFVO.Value = cond.Value;                   
                    ListCFIconSet.Add(icon);
                }
            }
        }
        #endregion

        internal void ClearAll()
        {
            foreach (CFIconMultiState iconMultiState in m_arrMultistate)
            {
                iconMultiState.ClearAll();
            }
            m_arrMultistate.Clear();
            m_arrMultistate = null;
            m_iconSetImpl.ClearAll();
            m_iconSetImpl = null;
        }
    }

    /// <summary>
    /// Conditional Formatting Value Object (CFVO).
    /// </summary>
    public class CFVO
    {
        #region Constants
        /// <summary>
        /// Minimum size of the structure.
        /// </summary>
        private const ushort DEF_MINIMUM_SIZE = 3;
        #endregion

        #region Members
        /// <summary>
        /// Specifies how the CFVO value is determined.
        /// </summary>
        private byte m_cfvoType=1;
        /// <summary>
        /// Specifies length of the formula.
        /// </summary>
        private ushort m_formulaLength;
        /// <summary>
        /// Formula data (RPN token array without size field):
        /// </summary>
        private byte[] m_arrFormula = new byte[0];
        /// <summary>
        /// 
        /// </summary>
        private Ptg[] m_arrFormulaParsed;
        /// <summary>
        /// Specifies a static value used to calculate the CFVO value.
        /// </summary>
        private double m_numValue;
        private string m_value;
        #endregion

        #region Properties
        /// <summary>
        /// CFVO type.
        /// </summary>
        public ConditionValueType CFVOType
        {
            get
            {
                return (ConditionValueType)m_cfvoType;
            }
            set
            {
                m_cfvoType = (byte)value;
            }
        }
        /// <summary>
        /// Size of the formula data. Read-only.
        /// </summary>
        public ushort FormulaSize
        {
            get
            {
                return m_formulaLength;
            }
        }
        /// <summary>
        /// Parsed formula string.
        /// </summary>
        public Ptg[] FormulaPtgs
        {
            get
            {
                return m_arrFormulaParsed;
            }
            set
            {
                m_arrFormula = FormulaUtil.PtgArrayToByteArray(value, ExcelVersion.Excel2007);
                m_arrFormulaParsed = value;
                m_formulaLength = (ushort)m_arrFormula.Length;
            }
        }
        /// <summary>
        ///  Returns bytes of the formula. Read-only.
        /// </summary>
        public byte[] FormulaBytes
        {
            get
            {
                return m_arrFormula;
            }
        }
        /// <summary>
        /// CFVO type.
        /// </summary>
        public double NumValue
        {
            get
            {
                return m_numValue;
            }
            set
            {
                m_numValue = value;
            }
        }
        /// <summary>
        /// CFVO value.
        /// </summary>
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CFVO()
        { }        
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of template parameter. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseCFVO(DataProvider provider, int iOffset, ExcelVersion version)
        {
            m_cfvoType = provider.ReadByte(iOffset);
            iOffset++;

            m_formulaLength = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_arrFormula = new byte[m_formulaLength];
            provider.ReadArray(iOffset, m_arrFormula);
            iOffset += m_formulaLength;

            m_arrFormulaParsed = FormulaUtil.ParseExpression(
                new ByteArrayDataProvider(m_arrFormula), m_formulaLength, version);

            if (version != ExcelVersion.Excel2007)
            {
                // To compare records correctly regardless original version.
                if (m_formulaLength > 0)
                {
                    m_arrFormula = FormulaUtil.PtgArrayToByteArray(m_arrFormulaParsed, ExcelVersion.Excel2007);
                    m_formulaLength = (ushort)m_arrFormula.Length;
                }
            }

            if (CFVOType == ConditionValueType.Number || CFVOType == ConditionValueType.Percent || CFVOType == ConditionValueType.Percentile
                || m_formulaLength < 0)
            {
                m_value = provider.ReadDouble(iOffset).ToString();
                iOffset += 8;
            }
            else
            {
                m_value = "0";
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
        public int SerializeCFVO(DataProvider provider, int iOffset, ExcelVersion version)
        {
            if (m_arrFormulaParsed != null && m_arrFormulaParsed.Length > 0)
            {
                m_arrFormula = FormulaUtil.PtgArrayToByteArray(m_arrFormulaParsed, version);
                m_formulaLength = (ushort)m_arrFormula.Length;
            }
            else
            {
                m_arrFormula = null;
                m_formulaLength = 0;
            }

            provider.WriteByte(iOffset, m_cfvoType);
            iOffset++;

            provider.WriteUInt16(iOffset, m_formulaLength);
            iOffset += 2;

            provider.WriteBytes(iOffset, m_arrFormula, 0, m_formulaLength);
            iOffset += m_formulaLength;

            if (CFVOType == ConditionValueType.Number || CFVOType == ConditionValueType.Percent || CFVOType == ConditionValueType.Percentile
                || m_formulaLength < 0)
            {
                Int64 val = Convert.ToInt64(m_value);
                provider.WriteDouble(iOffset, val);
                iOffset += 8;
            }

            return iOffset;
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public int GetStoreSize(ExcelVersion version)
        {
            int totalSize = DEF_MINIMUM_SIZE + m_formulaLength;

            if (CFVOType == ConditionValueType.Number || CFVOType == ConditionValueType.Percent || CFVOType == ConditionValueType.Percentile
                || m_formulaLength < 0)
                totalSize=totalSize+8;

            return totalSize;
        }
        #endregion

        internal void ClearAll()
        {
            m_arrFormula = null;
            m_arrFormulaParsed = null;

        }
    }

    /// <summary>
    /// Specifies one control point in the interpolation curve.
    /// </summary>
    public class CFInterpolationCurve
    {
        #region Constants
        /// <summary>
        /// Minimum size of the structure.
        /// </summary>
        private const ushort DEF_MINIMUM_SIZE = 8;
        #endregion

        #region Members
        /// <summary>
        /// Specifies the numerical value of this control point.
        /// </summary>
        private double m_numDomain;
        /// <summary>
        /// CFVO.
        /// </summary>
        private CFVO m_cfvo;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the numerical value of this control point.
        /// </summary>
        public double NumDomain
        {
            get
            {
                return m_numDomain;
            }
            set
            {
                m_numDomain = value;
            }
        }
        /// <summary>
        /// CFVO.
        /// </summary>
        public CFVO CFVO
        {
            get
            {
                return m_cfvo;
            }
            set
            {
                m_cfvo = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CFInterpolationCurve()
        {
            m_cfvo = new CFVO();
        }
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of template parameter. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseCFGradientInterp(DataProvider provider, int iOffset, ExcelVersion version)
        {
            iOffset=m_cfvo.ParseCFVO(provider, iOffset, version);

            m_numDomain = provider.ReadDouble(iOffset);
            iOffset += 8;

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
        public int SerializeCFGradientInterp(DataProvider provider, int iOffset, ExcelVersion version,double numValue,bool isParsed)
        {
            iOffset=m_cfvo.SerializeCFVO(provider, iOffset, version);

            if (!isParsed)
                provider.WriteDouble(iOffset, numValue);
            else
                provider.WriteDouble(iOffset, m_numDomain);
            iOffset += 8;

            return iOffset;
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public int GetStoreSize(ExcelVersion version)
        {
            return DEF_MINIMUM_SIZE + m_cfvo.GetStoreSize(version);
        }
        #endregion

        internal void ClearAll()
        {
            m_cfvo.ClearAll();
            m_cfvo = null;
        }
    }

    /// <summary>
    /// Specifies one control point in the gradient curve.
    /// </summary>
    public class CFGradientItem
    {
        #region Members
        /// <summary>
        /// The numerical value of the control point.
        /// </summary>
        private double m_numGradientRange;
        /// <summary>
        /// Type of color.
        /// </summary>
        private UInt32 m_colorType=2;
        /// <summary>
        /// Color value.
        /// </summary>
        private UInt32 m_colorValue;
        /// <summary>
        /// Tint and shade.
        /// </summary>
        private Int64 m_tintShade;
        #endregion

        #region Properties
        /// <summary>
        /// The numerical value of the control point.
        /// </summary>
        public double NumGradientRange
        {
            get
            {
                return m_numGradientRange;
            }
            set
            {
                m_numGradientRange = value;
            }
        }
        /// <summary>
        /// Type of color.
        /// </summary>
        public ColorType ColorType
        {
            get
            {
                return (ColorType)m_colorType;
            }
            set
            {
                m_colorType =2;
            }
        }
        /// <summary>
        /// Color value.
        /// </summary>
        public UInt32 ColorValue
        {
            get
            {
                return m_colorValue;
            }
            set
            {
                m_colorValue = value;

            }
        }
        /// <summary>
        /// Tint and shade.
        /// </summary>
        public Int64 TintShade
        {
            get
            {
                return m_tintShade;
            }
            set
            {
                m_tintShade = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CFGradientItem()
        { }
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of template parameter. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseCFGradient(DataProvider provider, int iOffset, ExcelVersion version)
        {
            m_numGradientRange = provider.ReadDouble(iOffset);
            iOffset += 8;

            m_colorType = provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_colorValue = provider.ReadUInt32(iOffset);
            iOffset += 4;

            m_tintShade = provider.ReadInt64(iOffset);
            iOffset += 8;

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
        public int SerializeCFGradient(DataProvider provider, int iOffset, ExcelVersion version,double numValue,bool isParsed)
        {
            if (!isParsed)
                provider.WriteDouble(iOffset, numValue);
            else
                provider.WriteDouble(iOffset, m_numGradientRange);
            iOffset += 8;

            provider.WriteUInt32(iOffset, m_colorType);
            iOffset += 4;

            provider.WriteUInt32(iOffset, m_colorValue);
            iOffset += 4;

            provider.WriteInt64(iOffset, m_tintShade);
            iOffset += 8;

            return iOffset;
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public int GetStoreSize(ExcelVersion version)
        {
            return 24;
        }
        #endregion
    }

    /// <summary>
    /// Specifies the threshold value associated with an icon.
    /// </summary>
    public class CFIconMultiState
    {
        #region Constants
        /// <summary>
        /// Minimum size of the structure.
        /// </summary>
        private const ushort DEF_MINIMUM_SIZE = 5;
        #endregion

        #region Members
        /// <summary>
        /// cfvo.
        /// </summary>
        private CFVO m_cfvo;
        /// <summary>
        /// Whether cell value equal to threshold.
        /// </summary>
        private byte m_isEqual=0;
        /// <summary>
        /// Undefined.
        /// </summary>
        private UInt32 m_undefined;
        #endregion        

        #region Properties
        /// <summary>
        /// CFVO.
        /// </summary>
        public CFVO CFVO
        {
            get
            {
                return m_cfvo;
            }
            set
            {
                m_cfvo = value;
            }
        }
        /// <summary>
        /// Whether cell value equal to threshold.
        /// </summary>
        public byte IsEqulal
        {
            get
            {
                return m_isEqual;
            }
            set
            {
                m_isEqual = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CFIconMultiState()
        {
            m_cfvo=new CFVO();
        }
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of template parameter. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int ParseCFIconMultistate(DataProvider provider, int iOffset, ExcelVersion version)
        {
            iOffset=m_cfvo.ParseCFVO(provider,iOffset,version);

            m_isEqual=provider.ReadByte(iOffset);
            iOffset++;

            m_undefined=provider.ReadUInt32(iOffset);
            iOffset+=4;

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
        public int SerializeCFIconMultistate(DataProvider provider, int iOffset, ExcelVersion version)
        {
            iOffset=m_cfvo.SerializeCFVO(provider,iOffset,version);

            provider.WriteByte(iOffset,m_isEqual);
            iOffset++;

            provider.WriteUInt32(iOffset,m_undefined);
            iOffset+=4;

            return iOffset;
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public int GetStoreSize(ExcelVersion version)
        {
            return DEF_MINIMUM_SIZE +m_cfvo.GetStoreSize(version);;
        }
        #endregion

        internal void ClearAll()
        {
            m_cfvo.ClearAll();
            m_cfvo = null;
        }
    }
}
