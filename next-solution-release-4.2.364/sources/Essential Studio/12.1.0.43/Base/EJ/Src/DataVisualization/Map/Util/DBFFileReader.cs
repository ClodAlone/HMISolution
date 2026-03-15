#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Syncfusion.JavaScript.DataVisualization.Maps
{
    internal class ShapeFileRecord
    {
        #region Constructor
        /// <summary>
        /// Constructor for the ShapeFileRecord class.
        /// </summary>
        public ShapeFileRecord()
        {
            this.Parts = new List<int>();
            this.Points = new List<ShapePoint>();
            this.Attributes = new Dictionary<string, object>();
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Indicates the record number (or index) which starts at 1.
        /// </summary>
        public int RecordIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Specifies the length of this shape record in 16-bit words.
        /// </summary>
        public int ContentLength
        {
            get;
            internal set;
        }

        /// <summary>
        /// Specifies the shape type for this record.
        /// </summary>
        public ShapeType ShapeType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the minimum x-position of the bounding
        /// box for the shape (expressed in degrees longitude).
        /// </summary>
        public double MinX
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the minimum y-position of the bounding
        /// box for the shape (expressed in degrees latitude).
        /// </summary>
        public double MinY
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the maximum x-position of the bounding
        /// box for the shape (expressed in degrees longitude).
        /// </summary>
        public double MaxX
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the maximum y-position of the bounding
        /// box for the shape (expressed in degrees latitude).
        /// </summary>
        public double MaxY
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the number of parts for this shape.
        /// A part is a connected set of points, analogous to
        /// a PathFigure in WPF.
        /// </summary>
        public int GetPartsCount()
        {
            return this.Parts.Count;
        }

        /// <summary>
        /// Specifies the total number of points defining
        /// this shape record.
        /// </summary>
        public int GetPointsCount()
        {
            return this.Points.Count;
        }

        /// <summary>      
        /// A collection of indices for the points array.
        /// Each index identifies the starting point of the
        /// corresponding part (or PathFigure using WPF
        /// terminology).
        /// </summary>
        public List<int> Parts
        {
            get;
            private set;
        }

        /// <summary>
        /// A collection of all of the points defining the
        /// shape record.
        /// </summary>
        public List<ShapePoint> Points
        {
            get;
            private set;
        }

        /// <summary>
        /// Access the (dBASE) attribute values associated
        /// with this shape record.
        /// </summary>
        public Dictionary<string, object> Attributes
        {
            get;
            internal set;
        }
        #endregion Properties

        /// <summary>
        /// Output some of the fields of the shapefile record.
        /// </summary>
        /// <returns>A string representation of the record.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ShapeFileRecord: RecordIndex={0}, ContentLength={1}, ShapeType={2}", this.RecordIndex, this.ContentLength, this.ShapeType);
            return sb.ToString();
        }
    }

    #region HelperClasses

    internal static class Helpers
    {
        /// <summary>
        /// Get the array slice between the two indexes.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Split<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
        public static T[] SplitReverse<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            int k = 0;
            for (int i = len - 1; i >= 0; i--)
            {
                res[k] = source[i + start];
                k++;
            }
            return res;
        }
    }
    #endregion

    internal class ShapeFileHeader
    {
        public ShapeFileHeader()
        {
        }

        #region Properties
        /// <summary>
        /// Indicate the fixed-length of this header in bytes.
        /// </summary>
        public static int Length
        {
            get { return 100; }
        }

        /// <summary>
        /// Specifies the file code for an ESRI shapefile, which
        /// should be the value, 9994.
        /// </summary>
        public int FileCode
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the length of the shapefile, expressed
        /// as the number of 16-bit words in the file.
        /// </summary>
        public int FileLength
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the shapefile version number.
        /// </summary>
        public int Version
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the shape type for the file. A shapefile
        /// contains only one type of shape.
        /// </summary>
        public ShapeType ShapeType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the minimum x-position of the bounding box.
        /// </summary>
        public double MinX
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the minimum y-position of the bounding box.
        /// </summary>
        public double MinY
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the minimium z-position of the bounding box.
        /// </summary>
        public double MinZ
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the minimum m-position of the bounding polygon
        /// </summary>
        public double MinM
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum x-position of the bounding box.
        /// </summary>       
        public double MaxX
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum y-position of the bounding box.
        /// </summary>
        public double MaxY
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum z-position of the bounding box.
        /// </summary>
        public double MaxZ
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum m-position of the bounding polygon.
        /// </summary>
        public double MaxM
        {
            get;
            set;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ShapeFileHeader: FileCode={0}, FileLength={1}, Version={2}, ShapeType={3}", this.FileCode, this.FileLength, this.Version, this.ShapeType);
            return sb.ToString();
        }
    }

    internal class ShapeFileDBFReader
    {
        #region CLR Properties

        public ShapeFileDBFData DBFData
        {
            get
            {
                return this.dbfData;
            }
            internal set
            {
                this.dbfData = value;
            }

        }


        #endregion

        #region PrivateFields

        private BinaryReader dbfBinaryReader;
        private ShapeFileDBFData dbfData;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DBFReader.ShapeFileDBFReader">ShapeFileDBFReader</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ShapeFileDBFReader()
        {
        }

        /// <summary>
        /// Method that Reads the DBF Data.
        /// </summary>
        /// <remarks></remarks>
        internal void ReadDBFData(string shapefilename)
        {
            if (!shapefilename.Equals(string.Empty))
            {
                shapefilename = shapefilename.Replace(".shp", ".dbf");
                Stream dbfStream = null;
                if (File.Exists(shapefilename))
                {
                    dbfStream = File.OpenRead(shapefilename);
                }

                if (dbfStream != null)
                {
                    this.ReadFromStream(dbfStream);
                }
            }
        }

        internal void ReadFromStream(Stream dbfStream)
        {
            this.dbfBinaryReader = new BinaryReader(dbfStream);
            this.dbfData = new ShapeFileDBFData();
            this.dbfData.DBFHeader = this.ReadHeader(this.dbfBinaryReader);
            this.dbfData.DBFFields = this.ReadDBFDataValues(this.dbfBinaryReader);
        }

        public ShapeFileDBFHeader ReadHeader(BinaryReader reader)
        {
            ShapeFileDBFHeader header = new ShapeFileDBFHeader();
            header.FileType = reader.ReadByte();
            header.Year = reader.ReadByte();
            header.Month = reader.ReadByte();
            header.Day = reader.ReadByte();
            header.NumberOfRecords = reader.ReadInt32();
            header.Length = reader.ReadInt16();
            header.RecordLength = reader.ReadInt16();
            header.Reserve1 = reader.ReadInt16();
            header.IncompleteTransaction = reader.ReadByte();
            header.EncriptionFlag = reader.ReadByte();
            header.FreeRecordThread = reader.ReadInt32();
            header.Reserve2 = reader.ReadInt32();
            header.Reserve3 = reader.ReadInt32();
            header.MDXFlag = reader.ReadByte();
            header.LanguageDriver = reader.ReadByte();
            header.Reserve4 = reader.ReadInt16();
            return header;
        }

        private List<DBFValues> ReadDBFDataValues(BinaryReader reader)
        {
            ShapeFileDBFField field = new ShapeFileDBFField();
            List<ShapeFileDBFField> m_tempFields = new List<ShapeFileDBFField>();
            List<DBFValues> vals = new List<DBFValues>();
            while (field.ReadFields(reader))
            {
                m_tempFields.Add(field);
                field = new ShapeFileDBFField();
            }
            int t_dataStartIndex = this.dbfData.DBFHeader.Length - (32 + (32 * m_tempFields.Count)) - 1;
            reader.ReadByte();
            for (int i = 0; i < this.dbfData.DBFHeader.NumberOfRecords; i++)
            {
                DBFValues val = new DBFValues();
                for (int j = 0; j < m_tempFields.Count; j++)
                {
                    val.Add(new DBFFieldValues { Key = m_tempFields[j].Name.ToString(), Value = ASCIIEncoder.GetString(reader.ReadBytes(m_tempFields[j].m_Length)) });

                }
                vals.Add(val);
                if (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    reader.ReadByte();
                }
            }
            return vals;
        }


    }

    internal class ShapeFileDBFField
    {
        #region Internal Fieds

        internal const int m_Size = 32;
        internal byte m_DataType;
        internal byte m_DecimalCount;
        internal int m_Length;
        internal byte[] m_Name = new byte[11];
        internal byte m_IndexFieldFlag;
        internal int nullIndex = 0;
        internal int m_Reserve1;
        internal short m_Reserve2;
        internal short m_Reserve3;
        internal byte[] m_Reserve4 = new byte[7];
        internal byte m_SetFieldFlag;
        internal byte m_workAreaID;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DBFReader.ShapeFileDBFField">ShapeFileDBFField</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ShapeFileDBFField()
        {
        }

        #region Properties

        public String Name
        {
            get
            {
                return ASCIIEncoder.GetString(m_Name);
            }
            internal set
            {
                if (value == null)
                {
                    throw new ArgumentException("Field name cannot be null");
                }

                if (value.Length == 0
                    || value.Length > 10)
                {
                    throw new ArgumentException(
                        "Field name should be of length 0-10");
                }

                m_Name = ASCIIEncoder.GetBytes(value.ToCharArray());
            }
        }

        #endregion

        #region Methods

        internal bool ReadFields(BinaryReader reader)
        {
            byte readByte = reader.ReadByte();
            if (readByte == ShapeFileDBFValues.EndOfField)
            {
                return false;
            }

            m_Name[0] = readByte;
            reader.Read(m_Name, 1, 10);
            this.m_DataType = reader.ReadByte();
            this.m_Reserve1 = reader.ReadInt32();
            this.m_Length = reader.ReadByte();
            this.m_DecimalCount = reader.ReadByte();
            this.m_Reserve2 = reader.ReadInt16();
            this.m_workAreaID = reader.ReadByte();
            this.m_Reserve3 = reader.ReadInt16();
            this.m_SetFieldFlag = reader.ReadByte();
            reader.Read(this.m_Reserve4, 0, 7);
            this.m_IndexFieldFlag = reader.ReadByte();
            return true;
        }

        #endregion

    }


    internal class ShapeFileDBFData
    {
        #region PrivateFields

        private ShapeFileDBFHeader m_dbfHeader;
        private List<DBFValues> m_dbfFields;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DBFReader.ShapeFileDBFData">ShapeFileDBFData</see> class. 
        /// </summary>
        public ShapeFileDBFData()
        {
        }

        #region Properties

        /// <summary>
        /// Gets  the dbf data file header. .
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ShapeFileDBFHeader DBFHeader
        {
            get
            {
                return this.m_dbfHeader;
            }
            internal set
            {
                this.m_dbfHeader = value;
            }
        }


        /// <summary>
        /// Gets or sets .
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public List<DBFValues> DBFFields
        {
            get
            {
                return this.m_dbfFields;
            }
            set
            {
                this.m_dbfFields = value;
            }

        }


        #endregion
    }

    internal class ShapeFileDBFHeader
    {
        #region Internal Fields

        internal ShapeFileDBFField[] m_Fields;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DBFReader.ShapeFileDBFHeader">ShapeFileDBFHeader</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ShapeFileDBFHeader()
        {
            this.m_Fields = new ShapeFileDBFField[this.NumberOfRecords];
        }

        #region Properties


        /// <summary>
        /// Gets or sets Length of the Header.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Int16 Length
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets or sets the DBF file type.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int FileType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the Record length of the DBF file.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Int16 RecordLength
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets or sets number of Reacords in the.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Int32 NumberOfRecords
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets or sets the ShapeFileDBFFields.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ShapeFileDBFField[] ShapeFileDBFFields
        {
            get
            {
                return this.m_Fields;
            }
            set
            {
                this.m_Fields = value;
            }
        }

        /// <summary>
        /// Gets or sets Day of the DBF file last update.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public byte Day
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets Month of the DBF file last updated.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public byte Month
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets Year of the DBF file last updated.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public byte Year
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets Cuncription flag of the DBF file.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public byte EncriptionFlag
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the FreeRecordThread of the DBF file.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int FreeRecordThread
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets the IncompleteTransaction of the DBF File.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public byte IncompleteTransaction
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the Language Driver of the DBF File.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public byte LanguageDriver
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the MDX Flag of the DBF File.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public byte MDXFlag
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets the First Reserved byte in the DBF File.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public short Reserve1
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the Second Reserved byte in the DBF File.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Reserve2
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the Third Reserved byte in the DBF file.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Reserve3
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the fourth Reserved byte in the DBF file.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public short Reserve4
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the Terminator flag of the DBF file.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public byte Terminator
        {
            get;
            internal set;
        }

        #endregion
    }

    internal class DBFFieldValues
    {
        public DBFFieldValues()
        {
        }

        public string Key
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }
    }

    internal class DBFValues : List<DBFFieldValues>
    {
    }

    internal class ShapeFileDBFValues
    {
        public const byte EndOfData = 0x1A; //^Z End of File
        public const byte EndOfField = 0x0D; //End of Field
        public const byte False = 0x46; //F in Ascci
        public const byte Space = 0x20; //Space in ascii
        public const byte True = 0x54; //T in ascii
        public const string Unknown = "?"; //Unknown value
    }

    internal class ASCIIEncoder
    {
        public ASCIIEncoder()
        {
        }

        public static char GetChar(byte byteArray)
        {
            char result = (char)byteArray;
            return result;
        }

        public static string GetString(byte[] byteArray)
        {
            StringBuilder strBild = new StringBuilder();
            for (int i = 0; i <= byteArray.Length - 1; i++)
            {
                if (byteArray[i] != 0)
                {
                    strBild.Append((char)byteArray[i]);
                }
            }
            return strBild.ToString().Trim();
        }

        public static byte[] GetBytes(char[] chars)
        {
            byte[] result = new byte[chars.Length];
            for (int i = 0; i <= chars.Length - 1; i++)
            {
                result[i] = (byte)chars[i];
            }
            return result;
        }
    }
}
