#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// A special attribute control token - typically either a SUM function or an IF function.
  /// </summary>
  [ Token ( FormulaToken.tAttr ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class AttrPtg : FunctionVarPtg
  {
    #region Class constants
    /// <summary>
    /// Size of the token.
    /// </summary>
    public const int SIZE = 4;
    /// <summary>
    /// Size of word in bytes.
    /// </summary>
    private const int DEF_WORD_SIZE = 2;
    /// <summary>
    /// Sum function name.
    /// </summary>
    private const string DEF_SUM = "SUM";
    /// <summary>
    /// If function name.
    /// </summary>
    private const string DEF_IF = "IF";
    /// <summary>
    /// Goto function name.
    /// </summary>
    private const string DEF_GOTO = "GOTO";
    /// <summary>
    /// Choose function name.
    /// </summary>
    private const string DEF_CHOOSE = "CHOOSE";
    /// <summary>
    /// Not implemented message.
    /// </summary>
    private const string DEF_NOT_IMPLEMENTED = "( tAttr not implemented )";
    /// <summary>
    /// 
    /// </summary>
    private const ushort DEF_SPACE_AFTER_MASK = 0x04;
    #endregion

    #region Class members
    /// <summary>
    /// The options used by the attribute.
    /// </summary>
    private byte    m_Options;
    /// <summary>
    /// The word contained in this attribute.
    /// </summary>
    private ushort  m_usData;
    /// <summary>
    /// Offsets to the CHOOSE cases. Must be valid only if HasOptimizedChoose is true.
    /// </summary>
    private ushort[] m_arrOffsets;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor for this token.
    /// </summary>
    public AttrPtg()
    {
      m_Options = 0;
      m_usData = 0;
      this.NumberOfArguments = 1;
      this.TokenCode = FormulaToken.tAttr;
    }
    /// <summary>
    /// Constructs token and fills it with data from the byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public AttrPtg( DataProvider provider, int iOffset, ExcelVersion version )
      : base( provider, iOffset, version )
    {
    }
    /// <summary>
    /// Constructs token by options value and data.
    /// </summary>
    /// <param name="options">Attribute options.</param>
    /// <param name="usData">Attribute data.</param>
    public AttrPtg( byte options, ushort usData )
    {
      this.TokenCode = FormulaToken.tAttr;
      m_Options = options;
      m_usData  = usData;
      this.NumberOfArguments = ( m_Options == 1 ) ? (byte)0 : (byte)1;
    }
    /// <summary>
    /// Constructs token by options value and data.
    /// </summary>
    /// <param name="options">Attribute options.</param>
    /// <param name="data">Attribute data.</param>
    public AttrPtg( int options, int data )
      : this( ( byte )options, ( ushort )data )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Options used by the attribute.
    /// </summary>
    public byte   Options
    {
      get
      {
        return m_Options;
      }
    }

    /// <summary>
    /// The word contained in this attribute.
    /// </summary>
    public ushort AttrData
    {
      get
      {
        return m_usData;
      }
      set
      {
        m_usData = value;
      }
    }

//#if DEBUG
    /// <summary>
    /// 
    /// </summary>
    public int AttrData1
    {
      get
      {
        return m_usData & byte.MaxValue;
      }
      internal set
      {
        m_usData = ( ushort )( ( m_usData & 0xFF00 ) | ( value & byte.MaxValue ) );
      }
    }
//#endif
    /// <summary>
    /// Gets / sets number of spaces in the case of space token.
    /// </summary>
    public int SpaceCount
    {
      get
      {
        if( HasSpace )
        {
          return m_usData >> BiffRecordRaw.DEF_BITS_IN_BYTE;
        }

        return -1;
        //throw new NotSupportedException();
      }
      set
      {
        if( HasSpace )
        {
          if( value < 1 || value > 255 )
            throw new ArgumentOutOfRangeException( "value", "Value cannot be less than 1 and greater than 255." );

          m_usData = ( ushort )( ( m_usData & byte.MaxValue ) + value << BiffRecordRaw.DEF_BITS_IN_BYTE );
        }
        else
        {
          throw new NotSupportedException();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool SpaceAfterToken
    {
      get
      {
        return ( HasSpace && ( ( m_usData & DEF_SPACE_AFTER_MASK ) != 0 ) );
      }
      set
      {
        if( HasSpace )
        {
          m_usData = ( ushort )( value
            ? m_usData | DEF_SPACE_AFTER_MASK
            : m_usData & ~DEF_SPACE_AFTER_MASK );
        }
        else
        {
          throw new NotSupportedException();
        }
      }
    }
    /// <summary>
    /// True when following function has semivolatile value,
    /// that can change without user interaction (such as NOW, TODAY, etc.).
    /// </summary>
    public bool   HasSemiVolatile
    {
      get
      {
        return ( m_Options & 0x1 ) == 0x1;
      }
      set
      {
        if( value )
          m_Options |= 0x1;
        else
        {
          unchecked{ m_Options &= (byte)~0x1; };
        }
      }
    }

    /// <summary>
    /// Returns True if this is an IF; otherwise False.
    /// </summary>
    public bool   HasOptimizedIf
    {
      get
      {
        return ( m_Options & 0x2 ) == 0x2;
      }
      set
      {
        if( value )
          m_Options |= 0x2;
        else
        {
          unchecked{ m_Options &= (byte)~0x2; };
        }
      }
    }
    /// <summary>
    /// Returns True if this is a CHOOSE; otherwise False.
    /// </summary>
    public bool   HasOptimizedChoose
    {
      get
      {
        return ( m_Options & 0x4 ) == 0x4;
      }
      set
      {
        if( value )
          m_Options |= 0x4;
        else
        {
          unchecked{ m_Options &= (byte)~0x4; };
        }
      }
    }
    /// <summary>
    /// Returns True if this is a goto; otherwise False.
    /// </summary>
    public bool   HasOptGoto
    {
      get
      {
        return ( m_Options & 0x8 ) == 0x8;
      }
      set
      {
        if( value )
          m_Options |= 0x8;
        else
        {
          unchecked{ m_Options &= (byte)~0x8; };
        }
      }
    }
    /// <summary>
    /// Returns True if this is SUM; otherwise False.
    /// </summary>
    public bool   HasSum
    {
      get
      {
        return ( m_Options & 0x10 ) == 0x10;
      }
      set
      {
        if( value )
          m_Options |= 0x10;
        else
        {
          unchecked{ m_Options &= (byte)~0x10; };
        }
      }
    }
    /// <summary>
    ///
    /// </summary>
    public bool   HasBaxcel
    {
      get
      {
        return ( m_Options & 0x20 ) == 0x20;
      }
      set
      {
        if( value )
          m_Options |= 0x20;
        else
        {
          unchecked{ m_Options &= (byte)~0x20; };
        }
      }
    }
    /// <summary>
    /// Returns True if space exist; otherwise False.
    /// </summary>
    public bool   HasSpace
    {
      get
      {
        return ( m_Options & 0x40 ) == 0x40;
      }
      set
      {
        if( value )
          m_Options |= 0x40;
        else
        {
          unchecked{ m_Options &= (byte)~0x40; };
        }
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      if( HasOptimizedChoose )
      {
        return SIZE + m_arrOffsets.Length * DEF_WORD_SIZE;
      }

      return SIZE;
    }
    /// <summary>
    /// Takes all needed operands from the stack and pushes the result of the function.
    /// </summary>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <param name="operands">
    /// Stack that contains all operands that receive result of the operation.
    /// </param>
    public override void PushResultToStack( FormulaUtil formulaUtil, Stack<object> operands, bool isForSerialization )
    {
      if( HasSpace )
      {
        string strToken = new string( ' ', SpaceCount );

        if( SpaceAfterToken )
        {
          object operand = operands.Pop();
          string strOperand = operand.ToString();

          if( strOperand.EndsWith( " " ) && strOperand[ 0 ] == ' ' )
          {
            strOperand = operands.Pop().ToString();
          }
          else
          {
            operand = null;
          }

          operands.Push( strOperand + strToken );

          if( operand != null )
            operands.Push( operand );
        }
        else
        {
          //          operands.Push( ToString() );
          operands.Push( this );
        }

        return;
      }
      else if( m_Options == 0 )
      {
        return;
      }

      base.PushResultToStack( formulaUtil, operands, isForSerialization );
    }

    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1,
      NumberFormatInfo numberFormat, bool isForSerialization )
    {
      if( HasSum )
      {
        return DEF_SUM;
      }
      else if( HasOptimizedIf )
      {
        return DEF_IF;
      }
      else if( HasOptGoto )
      {
        return DEF_GOTO;
      }
      else if( HasOptimizedChoose )
      {
        return DEF_CHOOSE;
      }
      else if( HasSpace )
      {
        //return string.Empty;
        // NOTE: Don't remove this comment - will be used in later versions.
        int count = SpaceCount;
        return new string( ' ', count );
      }
      else if( m_Options == 0 )
      {
        return string.Empty;
      }
      else
      {
        return DEF_NOT_IMPLEMENTED;
      }
    }
    /// <summary>
    /// Converts token to array of bytes.
    /// </summary>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = new byte[ GetSize( version ) ];
      int iOffset = 0;
      result[ iOffset++ ] = ( byte )FormulaToken.tAttr;
      result[ iOffset++ ] = m_Options;
      Buffer.BlockCopy( BitConverter.GetBytes( m_usData ), 0, result, iOffset, 2 );
      iOffset += 2;

      if( HasOptimizedChoose )
      {
        Buffer.BlockCopy( m_arrOffsets, 0, result, iOffset, GetSize( version ) - iOffset );
      }

      return result;
    }

    #endregion

    #region Class infill methods
    /// <summary>
    /// Infill PTG structure.
    /// </summary>
    /// <param name="provider">Represents storage.</param>
    /// <param name="offset">Offset in storage.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public override void InfillPTG( DataProvider provider, ref int offset, ExcelVersion version )
    {
      int iOffset = offset;
      TokenCode = FormulaToken.tAttr;
      m_Options = provider.ReadByte( offset++ );
      m_usData  = provider.ReadUInt16( offset );
      offset += 2;

      if( HasOptimizedChoose )
      {
        int iCount = m_usData + 1;
        m_arrOffsets = new ushort[ iCount ];

        //offset += 2;
        for( int i = 0; i < iCount; i++ )
        {
          m_arrOffsets[ i ] = provider.ReadUInt16( offset );
          offset += DEF_WORD_SIZE;

          //provider.CopyTo( offset + 4, m_arrOffsets, 0, iCount * DEF_WORD_
          //Buffer.BlockCopy( data, offset + 4, m_arrOffsets, 0, iCount * DEF_WORD_SIZE );
        }
      }
      else
      {
        NumberOfArguments = ( m_Options == 1 ) ? ( byte )0 : ( byte )1;
      }

      offset = iOffset + GetSize( version ) - 1;
    }
    #endregion
  }
}
