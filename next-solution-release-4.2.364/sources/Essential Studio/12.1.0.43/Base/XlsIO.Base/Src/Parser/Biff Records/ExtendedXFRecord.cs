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
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    [Syncfusion.Documentation.DocumentationExclude()]
    [Biff(TBIFFRecord.ExtendedXFRecord)]
    [CLSCompliant(false)]
    public class ExtendedXFRecord : BiffRecordRaw
    {
        #region Constants
        public const int StartLength = 20;
        #endregion

        #region Member
        /// <summary>
        /// Header of this record
        /// </summary>
        private FutureHeader m_header;
        /// <summary>
        /// Specifies the index of XF record which is extended
        /// </summary>
        private ushort m_usXFIndex;
        /// <summary>
        /// Property count
        /// </summary>
        private ushort m_propertyCount;
        /// <summary>
        /// Set of properties applied to the XF format
        /// </summary>
        private List<ExtendedProperty> m_properties;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the index of XF record which is extended
        /// </summary>
        public ushort XFIndex
        {
            get
            {
                return m_usXFIndex;
            }
            set
            {
                m_usXFIndex = value;
            }
        }
        /// <summary>
        /// Properties count
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
        /// Set of properties applied to the XF format
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
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor
        /// </summary>
        public ExtendedXFRecord()
            : base()
        {
            InitializeObjects();
        }
        /// <summary>
        /// Read / initialize constructor.
        /// </summary>
        /// <param name="stream">Stream from which record data should be read.</param>
        /// <param name="itemSize">Size of read item.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If stream is not specified.
        /// </exception>
        /// <exception cref="System.ApplicationException">
        /// If stream does not support read or seek operations.
        /// </exception>
        public  ExtendedXFRecord( Stream stream, out int itemSize )
          : base( stream, out itemSize )
        {
        }
        /// <summary>
        /// Reserved for the record's internal data array.
        /// </summary>
        /// <param name="iReserve">Amount of bytes for data array.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If amount of bytes requested is less than zero.
        /// </exception>
        public ExtendedXFRecord(int iReserve)
          : base( iReserve )
        {
          this.m_iCode = ( int ) TBIFFRecord.ExtendedXFRecord;
        }
        /// <summary>
        /// Initialize the variables
        /// </summary>
        private void InitializeObjects()
        {
            m_header = new FutureHeader();
            m_header.Type = (ushort)TBIFFRecord.ExtendedXFRecord;
            m_properties = new List<ExtendedProperty>();
        }
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
        /// If there is any internal error.
        /// </exception>
        public override void ParseStructure(DataProvider provider, int iOffset, int iLength, ExcelVersion version)
        {
            ushort reserved = 0;

            //header
            m_header.Type = provider.ReadUInt16(iOffset);
            iOffset += 2;
            m_header.Attributes = provider.ReadUInt16(iOffset);
            iOffset += 2;
            reserved = provider.ReadUInt16(iOffset);
            iOffset += 8;

            reserved = provider.ReadUInt16(iOffset);
            iOffset += 2;

            //XF index
            m_usXFIndex = provider.ReadUInt16(iOffset);
            iOffset += 2;

            reserved = provider.ReadUInt16(iOffset);
            iOffset += 2;

            // extended properties count
            m_propertyCount = provider.ReadUInt16(iOffset);
            iOffset += 2;

            //parse the extended properties
            for (int i = 0; i < m_propertyCount; i++)
            {
                ExtendedProperty property = new ExtendedProperty();
                iOffset= property.ParseExtendedProperty(provider, iOffset, version);
                m_properties.Add(property);
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
            ushort reserved = 0;

            //header
            provider.WriteUInt16(iOffset, m_header.Type);
            iOffset += 2;
            provider.WriteUInt16(iOffset, m_header.Attributes);
            iOffset += 2;

            provider.WriteUInt16(iOffset, 0); //Reserved
            iOffset += 8;

            provider.WriteUInt16(iOffset, 0);//Reserved
            iOffset += 2;

            //XF index
            provider.WriteUInt16(iOffset, XFIndex);
            iOffset += 2;

            provider.WriteUInt16(iOffset, 0);//Reserved
            iOffset += 2;

            provider.WriteUInt16(iOffset, (ushort)Properties.Count);
            iOffset += 2;

            //List of extended property
            foreach(ExtendedProperty property in Properties)
            {
                iOffset = property.InfillInternalData(provider, iOffset, version);
            }
        }
        /// <summary>
        /// Get the size
        /// </summary>
        public override int GetStoreSize(ExcelVersion version)
        {
            int propertiesLength = 0;
            foreach (ExtendedProperty property in m_properties)
                propertiesLength += property.Size;
            return StartLength + propertiesLength;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Serves as a hash function for a particular type, suitable for use
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current Object.</returns>
        public override int GetHashCode()
        {
            int i_hashCode = m_header.Type.GetHashCode()
                ^ m_header.Attributes.GetHashCode()
                ^ m_usXFIndex.GetHashCode()
                ^ m_propertyCount.GetHashCode()
                ^ Properties.GetHashCode();

            return i_hashCode;
        }
        /// <summary>
        /// Compares with Extended XF format record.
        /// </summary>
        /// <param name="twin">Param to compare.</param>
        /// <returns>Returns compare results.</returns>
        public int CompareTo(ExtendedXFRecord twin)
        {
            if (twin == null)
                throw new ArgumentNullException("twin");

            int result = m_header.Type - twin.m_header.Type;
            if (result != 0) return result;

            result = m_header.Attributes - twin.m_header.Attributes;
            if (result != 0) return result;

            result = m_usXFIndex - twin.m_usXFIndex;
            if (result != 0) return result;

            result = m_propertyCount - twin.m_propertyCount;
            if (result != 0) return result;

            if (m_properties != twin.m_properties)
            {
                result = 1;
                return result;
            }

            return result;
        }
        /// <summary>
        /// Copies data from the current Biff record to the specified Biff record.
        /// </summary>
        /// <param name="raw">Biff record that will receive data from the current record.</param>
        public override void CopyTo(BiffRecordRaw raw)
        {
            if (raw == null)
                throw new ArgumentNullException("raw");

            ExtendedXFRecord formatRecord = raw as ExtendedXFRecord;

            if (formatRecord != null)
            {
                CopyTo(formatRecord);
            }
            else
            {
                throw new ArgumentException("raw");
            }
        }
        /// <summary>
        /// Copies data from the current ExtendedXF record to the specified
        /// ExtendedXF record.
        /// </summary>
        /// <param name="twin">ExtendedXF record that will receive data from
        /// the current record.</param>
        public void CopyTo(ExtendedXFRecord twin)
        {
            twin.m_header.Type = m_header.Type;
            twin.m_header.Attributes = m_header.Attributes;
            twin.m_usXFIndex = m_usXFIndex;
            twin.m_propertyCount = m_propertyCount;
            twin.m_properties = m_properties;
        }
        #endregion
        
        #region ICloneable Members
        /// <summary>
        /// Clone current Record.
        /// </summary>
        /// <returns>Returns memberwise clone on current object.</returns>        
        public override object Clone()
        {
            ExtendedXFRecord record = (ExtendedXFRecord)base.Clone();
            record.Properties = new List<ExtendedProperty>();
            return record;
        }
        /// <summary>
        /// 
        /// </summary>
        public ExtendedXFRecord CloneObject()
        {
            return (ExtendedXFRecord)this.MemberwiseClone();
        }        
        #endregion
    }

    #region Extended property Class
    public class ExtendedProperty
    {
        #region Constants
        /// <summary>
        /// Maximum tint value.
        /// </summary>
        public const int MaxTintValue = 32767;
        #endregion

        #region Member
        /// <summary>
        /// Type of the Extended property
        /// </summary>
        private ushort m_usType;
        /// <summary>
        /// Size of the Extended property
        /// </summary>
        private ushort m_propSize;
        /// <summary>
        /// Type of the stored color.
        /// </summary>
        private ushort m_colorType = 0x2;
        /// <summary>
        /// Color value.
        /// </summary>
        private uint m_colorValue;
        /// <summary>
        /// Tint and shade.
        /// </summary>
        private double m_tintAndShade=0;        
        /// <summary>
        /// Reserved.
        /// </summary>
        private Int64 reserved=0;
        /// <summary>
        /// Font Scheme.
        /// </summary>
        private ushort m_fontScheme;
        /// <summary>
        /// Text indentation level.
        /// </summary>
        private ushort m_textIndentationLevel;

        /// <summary>
        /// Gradient type.
        /// </summary>
        private uint m_gradientType;
        /// <summary>
        /// Gradient angle.
        /// </summary>
        private Int64 m_iAngle;
        /// <summary>
        /// Left coordinate of the inner rectangle. 
        /// </summary>
        private Int64 m_fillToRectLeft;
        /// <summary>
        /// Right coordinate of the inner rectangle. 
        /// </summary>
        private Int64 m_fillToRectRight;
        /// <summary>
        /// Top coordinate of the inner rectangle. 
        /// </summary>
        private Int64 m_fillToRectTop;
        /// <summary>
        /// Bottom coordinate of the inner rectangle. 
        /// </summary>
        private Int64 m_fillToRectBottom;
        /// <summary>
        /// No. of items in the gradstops.
        /// </summary>
        private uint m_gradStopCount;
        /// <summary>
        /// Gradient color value.
        /// </summary>
        private int m_gradColorValue;
        /// <summary>
        /// Gradient position.
        /// </summary>
        private Int64 m_gradPostition;
        /// <summary>
        /// Gradient tint.
        /// </summary>
        private Int64 m_gradTint;
        /// <summary>
        /// List of gradient stops.
        /// </summary>
        private List<GradStops> m_gradstops;
        #endregion

        #region Properties
        /// <summary>
        /// Type of the Extended property
        /// </summary>
        public CellPropertyExtensionType Type
        {
            get
            {
                return (CellPropertyExtensionType)m_usType;
            }
            set
            {
                m_usType = (byte)value;
            }
        }
        /// <summary>
        /// Size of the color Record
        /// </summary>
        public ushort Size
        {
            get
            {
                return m_propSize;
            }
            set
            {
                m_propSize = value;
            }
        }
        /// <summary>
        /// Color type
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
        /// Color value
        /// </summary>
        public uint ColorValue
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
        /// Tint value
        /// </summary>
        public double Tint
        {
            get
            {
                return m_tintAndShade;
            }
            set
            {
                m_tintAndShade = value;
            }
        }
        /// <summary>
        /// Reserved must be ignored.
        /// </summary>
        public Int64 Reserved
        {
            get
            {
                return reserved;
            }
            set
            {
                reserved = value;
            }
        }
        /// <summary>
        /// Font Sheme.
        /// </summary>
        public FontScheme FontScheme
        {
            get
            {
                return (FontScheme)m_fontScheme;
            }
            set
            {
                m_fontScheme = (byte)value;
            }
        }
        /// <summary>
        /// Text Indentation level.
        /// </summary>
        public ushort Indent
        {
            get
            {
                return m_textIndentationLevel;
            }
            set
            {
                if (m_textIndentationLevel > 250)
                    throw new ArgumentOutOfRangeException("Indent level","Text indentation level must be less than or equal to 250");

                m_textIndentationLevel = value;
            }
        }
        /// <summary>
        /// Set of gradstops.
        /// </summary>
        public List<GradStops> GradStops
        {
            get
            {
                return m_gradstops;
            }
            set
            {
                m_gradstops = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor
        /// </summary>
        public ExtendedProperty()
        {
            m_gradstops = new List<GradStops>();
        }
        #endregion

        #region Parse and Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
        /// If there is any internal error.
        /// </exception>
        public int ParseExtendedProperty(DataProvider provider, int iOffset, ExcelVersion version)
        {
            m_usType = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_propSize = provider.ReadUInt16(iOffset);
            iOffset += 2;

            if (Type == CellPropertyExtensionType.GradientFill)
            {
                iOffset = ParseGradient(provider, iOffset, version);
            }
            else if (Type == CellPropertyExtensionType.FontScheme)
            {
                int CalculatedSize = m_propSize - 4;
                if (CalculatedSize == 1)
                {
                    m_fontScheme = provider.ReadByte(iOffset);
                    iOffset++;
                }
                else if (CalculatedSize == 2)
                {
                    m_fontScheme = provider.ReadUInt16(iOffset);
                    iOffset += 2;
                }
            }
            else if (Type == CellPropertyExtensionType.TextIndentationLevel)
            {
                m_textIndentationLevel = provider.ReadUInt16(iOffset);
                iOffset += 2;
            }
            else
            {
                iOffset = ParseFullColor(provider, iOffset, version);
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
        public int InfillInternalData(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_usType);
            iOffset += 2;
            provider.WriteUInt16(iOffset, Size);
            iOffset += 2;

            if (Type == CellPropertyExtensionType.GradientFill)
            {
                iOffset = SerializeGradient(provider, iOffset, version);
            }
            else if (Type == CellPropertyExtensionType.FontScheme)
            {
                int CalculatedSize = m_propSize - 4;
                if (CalculatedSize == 1)
                {
                    provider.WriteByte(iOffset, (byte)m_fontScheme);
                    iOffset++;
                }
                else if (CalculatedSize == 2)
                {
                    provider.WriteUInt16(iOffset, m_fontScheme);
                    iOffset += 2;
                }
            }
            else if (Type == CellPropertyExtensionType.TextIndentationLevel)
            {
                provider.WriteUInt16(iOffset, m_textIndentationLevel);
                iOffset += 2;
            }
            else
            {
                iOffset = SerializeFullColor(provider, iOffset, version);
            }           

            return iOffset;            
        }
        #endregion

        #region Parse and Serialize FullColor
        public int ParseFullColor(DataProvider provider, int iOffset, ExcelVersion version)
        {
            m_colorType = provider.ReadByte(iOffset);
            iOffset += 2;

            m_tintAndShade = provider.ReadInt16(iOffset);
            iOffset += 2;

            m_colorValue=provider.ReadUInt32(iOffset);
            iOffset += 4;

            reserved = provider.ReadInt64(iOffset);
            iOffset += 8;

            return iOffset;
        }
        public int SerializeFullColor(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_colorType);
            iOffset += 2;

            if ( m_usType == 4 && m_colorType == 3)
                m_tintAndShade *= MaxTintValue;

            provider.WriteUInt16(iOffset, (ushort)m_tintAndShade);
            iOffset += 2;

            provider.WriteUInt32(iOffset, m_colorValue);
            iOffset += 4;

            provider.WriteInt64(iOffset, reserved);
            iOffset += 8;

            return iOffset;
        }
        #endregion

        #region Parse and Serialize Gradient
        /// <summary>
        /// Parse the gradient.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
        /// If there is any internal error.
        /// </exception>
        public int ParseGradient(DataProvider provider, int iOffset, ExcelVersion version)
        {
            m_gradientType = provider.ReadByte(iOffset);
            iOffset += 4;

            m_iAngle = provider.ReadInt64(iOffset);
            iOffset += 8;
            m_fillToRectLeft = provider.ReadInt64(iOffset);
            iOffset += 8;

            m_fillToRectRight = provider.ReadInt64(iOffset);
            iOffset += 8;

            m_fillToRectTop = provider.ReadInt64(iOffset);
            iOffset += 8;

            m_fillToRectBottom = provider.ReadInt64(iOffset);
            iOffset += 8;

            m_gradStopCount = provider.ReadUInt32(iOffset);
            iOffset += 4;

            for (int i = 0; i < m_gradStopCount; i++)
            {
                GradStops stop = new GradStops();
                iOffset = stop.ParseGradStops(provider, iOffset, version);
                m_gradstops.Add(stop);
            }

            return iOffset;
        }
        /// <summary>
        /// Serialize the gradient.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public int SerializeGradient(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteByte(iOffset, (byte)m_gradientType);
            iOffset += 4;

            provider.WriteInt64(iOffset, m_iAngle);
            iOffset += 8;

            provider.WriteInt64(iOffset, m_fillToRectLeft);
            iOffset += 8;

            provider.WriteInt64(iOffset, m_fillToRectRight);
            iOffset += 8;

            provider.WriteInt64(iOffset, m_fillToRectTop);
            iOffset += 8;

            provider.WriteInt64(iOffset, m_fillToRectBottom);
            iOffset += 8;

            provider.WriteUInt32(iOffset, m_gradStopCount);
            iOffset += 4;

            foreach (GradStops stop in GradStops)
            {
                iOffset = stop.InfillInternalData(provider, iOffset, version);
            }

            return iOffset;
        }
        #endregion
    }
    #endregion

    #region Grad Stops class
    public class GradStops
    {
        #region Member
        /// <summary>
        /// Type of the stored color.
        /// </summary>
        private ushort m_colorType;
        /// <summary>
        /// Gradient color value.
        /// </summary>
        private int m_gradColorValue;
        /// <summary>
        /// Gradient position.
        /// </summary>
        private Int64 m_gradPostition;
        /// <summary>
        /// Gradient tint.
        /// </summary>
        private Int64 m_gradTint;
        #endregion        

        #region Initialization
        /// <summary>
        /// Default constructor
        /// </summary>
        public GradStops()
        {            
        }

        #endregion

        #region Parse and serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
        /// If there is any internal error.
        /// </exception>
        public int ParseGradStops(DataProvider provider, int iOffset, ExcelVersion version)
        {
            m_colorType = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_gradColorValue = provider.ReadInt32(iOffset);
            iOffset += 4;

            m_gradPostition = provider.ReadInt64(iOffset);
            iOffset += 8;

            m_gradTint = provider.ReadInt64(iOffset);
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
        public int InfillInternalData(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_colorType);
            iOffset += 2;

            provider.WriteInt32(iOffset, m_gradColorValue);
            iOffset += 4;

            provider.WriteInt64(iOffset, m_gradPostition);
            iOffset += 8;

            provider.WriteInt64(iOffset, m_gradTint);
            iOffset += 8;

            return iOffset;
        }
        #endregion
    }
    #endregion
}
