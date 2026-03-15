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

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
    /// <summary>
    /// Common object data.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    [CLSCompliant(false)]
    public class ftRboData : ObjSubRecord
    {
      #region Class members
      /// <summary>
      /// Type of the object.
      /// </summary>
      private byte m_isFirstButton;
      /// <summary>
      /// Type of the object.
      /// </summary>
      private byte m_nextButton;
      /// <summary>
      /// Reserved object 1
      /// </summary>
      private byte m_ft;
      /// <summary>
      /// Reserved object 2;
      /// </summary>
      private byte m_cb;

      #endregion

      #region Class Properties

      /// <summary>
        /// Indicates whether object is First Button in the group.
        /// </summary>
        public bool IsFirstButton
        {
            get
            {
                return (m_isFirstButton == 1) ? true : false;
            }
            set
            {
                m_isFirstButton = (byte)((value) ? 1 : 0);
            }
        }
        /// <summary>
        /// Indicates Next Button in the Group.
        /// </summary>
        public byte NextButton
        {
            get
            {
                return m_nextButton;
            }
            set
            {
              m_nextButton = value;
            }
        }
        #endregion

      #region Class Initialize/Finalize methods
      /// <summary>
      /// Default constructor.
      /// </summary>
      public ftRboData()
        : base( TObjSubRecordType.ftRboData )
      {
      }
      /// <summary>
      /// Initialize new instance.
      /// </summary>
      /// <param name="type">Type of the subrecord.</param>
      /// <param name="length">Length of the subrecord's data.</param>
      /// <param name="buffer">Array that contains subrecord's data.</param>
      public ftRboData( ushort length, byte[] buffer )
        : base( TObjSubRecordType.ftRboData, length, buffer )
      {
      }
      #endregion

        #region Class overrides
        /// <summary>
        /// Parses byte array.
        /// </summary>
        /// <param name="buffer">Array to parse.</param>
        protected override void Parse(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            m_nextButton = buffer[0];

            m_ft = buffer[1];

            m_isFirstButton = buffer[2];

            m_cb = buffer[3];
           
        }
        /// <summary>
        /// Fills array with binary representation of the subrecord.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer to copy data to.</param>
        public override void FillArray(DataProvider provider, int iOffset)
        {
            provider.WriteInt16(iOffset, (short)Type);
            iOffset += 2;

            //BitConverter.GetBytes( ( short ) 18 ).CopyTo( arrBuffer, iOffset );
            short sLength = (short)(GetStoreSize(ExcelVersion.Excel97to2003) - 4);
            provider.WriteInt16(iOffset, sLength);
            iOffset += 2;

            provider.WriteByte(iOffset, m_nextButton);
            iOffset++;

            provider.WriteByte(iOffset, m_ft);
            iOffset++;

            provider.WriteByte(iOffset, m_isFirstButton);
            iOffset++;

            provider.WriteByte(iOffset, m_cb);
                       
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public override int GetStoreSize(ExcelVersion version)
        {
            return 8;
        }

        #endregion
    }
}
