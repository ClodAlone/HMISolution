#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class PieceDescriptorRecord : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// Size of the record in bytes.
        /// </summary>
        internal const int RECORD_SIZE = 8;
        #endregion

        #region Class members
        /// <summary>
        /// Underlying structure.
        /// </summary>
        private PieceDescriptorStructure m_field = new PieceDescriptorStructure();
        #endregion

        #region Class Properties
        #region BitField 0
        //[ FieldOffset( 0 ) ]
        /// <summary>
        /// when 1, means that piece contains no end of paragraph marks.
        /// </summary>
        internal bool fNoParaLast
        {
            get
            {
                return GetBit(m_field.Options, 1);
            }
            set
            {
                m_field.Options = (ushort)SetBit(m_field.Options, 1, value);
            }
        }
        /// <summary>
        /// used internally by Word
        /// </summary>
        internal bool fPaphNil
        {
            get
            {
                return GetBit(m_field.Options, 2);
            }
            set
            {
                m_field.Options = (ushort)SetBit(m_field.Options, 2, value);
            }
        }
        /// <summary>
        /// used internally by Word
        /// </summary>
        internal bool fCopied
        {
            get
            {
                return GetBit(m_field.Options, 4);
            }
            set
            {
                m_field.Options = (ushort)SetBit(m_field.Options, 4, value);
            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //internal short *
        //{
        //get
        //{ 
        // return GetBit( m_field.*,  );
        //}
        //set
        //{
        //SetBit( m_field.*, , value );
        //}
        //}
        #endregion


        //    #region BitField 1
        //    //[ FieldOffset( 1 ) ]
        //    /// <summary>
        //    /// used internally by Word
        //    /// </summary>
        //    internal short fn
        //    {
        //      get
        //      { 
        //        return GetBit( m_field.fn, FF00 );
        //      }
        //      set
        //      {
        //        SetBit( m_field.fn, FF00, value );
        //      }
        //    }
        //    #endregion

        /// <summary>
        /// file offset of beginning of piece. The size of the ith piece can be determined by subtracting rgcp[i] of the containing plcfpcd from its rgcp[i+1].
        /// </summary>
        internal uint FileOffset
        {
            get
            {
                return m_field.FileOffset;
            }
            set
            {
                m_field.FileOffset = value;
            }
        }
        /// <summary>
        /// contains either a single sprm or else an index number of the grpprl which contains the sprms that modify the properties of the piece.
        /// </summary>
        internal PropertyModifierStructure PropertyModifier
        {
            get
            {
                return m_field.PropertyModifier;
            }
            set
            {
                m_field.PropertyModifier = value;
            }
        }
        /// <summary>
        /// Returns underlying structure. Read-only.
        /// </summary>
        protected override DataStructure UnderlyingStructure
        {
            get
            {
                return m_field;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return m_field.Length;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal PieceDescriptorRecord()
        {
        }
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        internal PieceDescriptorRecord(byte[] arrData)
            : base(arrData)
        {
        }
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>
        internal PieceDescriptorRecord(byte[] arrData, int iOffset)
            : base(arrData, iOffset)
        {
        }
        #endregion
    }
}