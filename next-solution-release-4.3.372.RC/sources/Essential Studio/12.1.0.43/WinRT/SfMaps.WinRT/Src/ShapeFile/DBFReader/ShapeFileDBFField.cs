#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Windows;
    using System.Text;
    using System.IO;
#if WINRT
    using Windows.Storage.Streams;
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
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

        public bool ReadFields(BinaryReader reader)
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
}
