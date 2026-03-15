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
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for PropertyModifierStructure.
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Explicit)]
#endif
    [CLSCompliant(false)]
    internal struct PropertyModifierStructure
    {
        #region Class constants
        /// <summary>
        /// Bit index for IsComplex value.
        /// </summary>
        private const int DEF_BIT_COMPLEX = 0;

        /// <summary>
        /// Size of the record in bytes;
        /// </summary>
        private const int DEF_RECORD_SIZE = 2;
        #endregion

        #region Class members
        /// <summary>
        /// Options.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(0)]
#endif
        private ushort m_usOptions;
        #endregion

        #region Class Properties
#if DEBUG
    /// <summary>
    /// Options. Read-only.
    /// </summary>
    internal ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
	  /// <summary>
	  /// 
	  /// </summary>
    internal byte IndexToSprm
    {
      get
      {
        return ( byte )(( m_usOptions & 0xFE ) >> 1);
      }
    }
	  /// <summary>
	  /// 
	  /// </summary>
	  internal byte Value
	  {
	    get
	    {
	      return ( byte )( ( m_usOptions & 0x0F ) >> 8 );
	    }
	  }
      /// <summary>
      /// Returns the fully qualified type name of this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> containing a fully qualified type name.
      /// </returns>
	  public override string ToString()
	  {
	    return string.Format( "( {0}: Options = {1}, IsComplex = {2}", base.ToString(), Options, IsComplex );
	    ;
	  }

#endif

        /// <summary>
        /// 
        /// </summary>
        internal bool IsComplex
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions, DEF_BIT_COMPLEX);
            }
            set
            {
                m_usOptions = (ushort)BaseWordRecord.SetBit(m_usOptions, DEF_BIT_COMPLEX, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int Length
        {
            get
            {
                return DEF_RECORD_SIZE;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="iOffset"></param>
        internal void Parse(byte[] arrData, ref int iOffset)
        {
            m_usOptions = DataStructure.ReadUInt16(arrData, ref iOffset);
        }

        /// <summary>
        /// Saves structure to specified data array.
        /// </summary>
        /// <param name="arrData">The array with data.</param>
        /// <param name="iOffset">The offset.</param>
        internal void Save(byte[] arrData, ref int iOffset)
        {
            DataStructure.WriteUInt16(arrData, ref iOffset, m_usOptions);
        }
        #endregion
    }
}
