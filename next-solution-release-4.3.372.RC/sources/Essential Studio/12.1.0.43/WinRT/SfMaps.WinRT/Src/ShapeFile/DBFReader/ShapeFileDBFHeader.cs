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
    using System.IO;
    using System.Collections.Generic;
#if WINRT
    using Windows.Storage.Streams;
#else
#if !WINDOWSPHONE && !WINDOWSFORMS
    using System.Dynamic;
#endif
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
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
}
