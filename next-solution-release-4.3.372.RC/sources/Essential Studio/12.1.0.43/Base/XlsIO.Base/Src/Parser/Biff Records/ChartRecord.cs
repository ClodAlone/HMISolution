#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    /// <summary>
    ///
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude]
    [Biff( TBIFFRecord.Chart )]
    public class ChartRecord : BiffRecordRaw
    {
        #region Fields

        /// <summary>
        /// Correct record size.
        /// </summary>
        private const int DEF_RECORD_SIZE = 16;

        /// <summary>
        /// 
        /// </summary>
        [BiffRecordPos( 0, 4, true )]
        private int m_iXPosition = 0;

        /// <summary>
        /// 
        /// </summary>
        [BiffRecordPos( 8, 4, true )]
        private int m_iXSize = 0;

        /// <summary>
        /// 
        /// </summary>
        [BiffRecordPos( 4, 4, true )]
        private int m_iYPosition = 0;

        /// <summary>
        /// 
        /// </summary>
        [BiffRecordPos( 12, 4, true )]
        private int m_iYSize = 0;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ChartRecord()
            : base()
        {
        }

        /// <summary>
        /// Read / initialize constructor.
        /// </summary>
        /// <param name="stream">Stream from which record data should be read.</param>
        /// <param name="itemSize">Size of read item.</param>
        /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
        /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
        public ChartRecord( Stream stream, out int itemSize )
            : base(stream, out itemSize)
        {
        }

        /// <summary>
        /// Reserved for the record's internal data array.
        /// </summary>
        /// <param name="iReserve">Amount of bytes for data array.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
        public ChartRecord( int iReserve )
            : base(iReserve)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Returns maximum possible size of record's internal data array.
        /// </summary>
        public override int MaximumRecordSize
        {
            get
              {
            return DEF_RECORD_SIZE;
              }
        }

        /// <summary>
        /// Returns minimum possible size of record's internal data array.
        /// </summary>
        public override int MinimumRecordSize
        {
            get
              {
            return DEF_RECORD_SIZE;
              }
        }

        /// <summary>
        /// 
        /// </summary>
        public int X
        {
            get
              {
            return m_iXPosition;
              }
              set
              {
            m_iXPosition = value;
              }
        }

        /// <summary>
        /// 
        /// </summary>
        public int XSize
        {
            get
              {
            return m_iXSize;
              }
              set
              {
            m_iXSize = value;
              }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Y
        {
            get
              {
            return m_iYPosition;
              }
              set
              {
            m_iYPosition = value;
              }
        }

        /// <summary>
        /// 
        /// </summary>
        public int YSize
        {
            get
              {
            return m_iYSize;
              }
              set
              {
            m_iYSize = value;
              }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public override int GetStoreSize( ExcelVersion version )
        {
            return DEF_RECORD_SIZE;
        }

        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="iLength">Buffer length.</param>
        public override void InfillInternalData( DataProvider provider, int iOffset, int iLength )
        {
            m_iLength = GetStoreSize( version );

              provider.WriteInt32( iOffset, m_iXPosition );
              iOffset += 4;

              provider.WriteInt32( iOffset, m_iYPosition );
              iOffset += 4;

              provider.WriteInt32( iOffset, m_iXSize );
              iOffset += 4;

              provider.WriteInt32( iOffset, m_iYSize );
        }

        /// <summary>
        /// Parse structure of record. Convert Data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        public override void ParseStructure( DataProvider provider, int iOffset, int iLength )
        {
            m_iXPosition = provider.ReadInt32( iOffset );
              iOffset += 4;

              m_iYPosition = provider.ReadInt32( iOffset );
              iOffset += 4;

              m_iXSize = provider.ReadInt32( iOffset );
              iOffset += 4;

              m_iYSize = provider.ReadInt32( iOffset );
        }

        #endregion Methods
    }
}