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
using System.Collections;
using System.Text;

using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.IO;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsofbtOPT.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtOPT ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtOPT
    : MsoBase
    , ICloneable
    , IFopteOptionWrapper
  {
    #region Internal classes
    /// <summary>
    /// 
    /// </summary>
    public class FOPTE : ICloneable
    {
      #region Class constants
      /// <summary>
      /// 
      /// </summary>
      private const ushort DEF_ID_MASK = 0x3FFF;
      /// <summary>
      /// 
      /// </summary>
      private const ushort DEF_VALID_MASK = 0x4000;
      /// <summary>
      /// 
      /// </summary>
      private const ushort DEF_COMPLEX_MASK = 0x8000;
      /// <summary>
      /// 
      /// </summary>
      private const int DEF_RECORD_SIZE = 6;
      #endregion

      #region Class members
      /// <summary>
      /// Property ID.
      /// </summary>
      private ushort  m_usId;
      /// <summary>
      /// Value is a blip ID � only valid if fComplex is FALSE.
      /// </summary>
      private bool    m_bIdValid;
      /// <summary>
      /// Complex property, value is length.
      /// </summary>
      private bool    m_bComplex;
      /// <summary>
      /// 
      /// </summary>
      private uint    m_uiValue;
      /// <summary>
      /// 
      /// </summary>
      private byte[]  m_arrData;
      #endregion

      #region Class properties
      /// <summary>
      /// Property ID.
      /// </summary>
      public MsoOptions Id
      {
        get
        {
          return ( MsoOptions )m_usId;
        }
        set
        {
          m_usId = ( ushort )value;
        }
      }
      /// <summary>
      /// Value is a blip ID � only valid if fComplex is FALSE.
      /// </summary>
      public bool   IsValid
      {
        get
        {
          return m_bIdValid;
        }
        set
        {
          m_bIdValid = value;
        }
      }
      /// <summary>
      /// Complex property, value is length.
      /// </summary>
      public bool   IsComplex
      {
        get
        {
          return m_bComplex;
        }
        set
        {
          m_bComplex = value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public uint   UInt32Value
      {
        get
        {
          return m_uiValue;
        }
        set
        {
          m_uiValue = value;
        }
      }

      /// <summary>
      /// 
      /// </summary>
      public int   Int32Value
      {
        get
        {
          return ( int )m_uiValue;
        }
        set
        {
          m_uiValue = ( uint )value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public byte[] AdditionalData
      {
        get
        {
          return m_arrData;
        }
        set
        {
          m_arrData = value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public byte[] MainData
      {
        get
        {
          byte[] buffer = new byte[ Size ];
          ushort value = ( ushort )( m_usId & DEF_ID_MASK );

          if( m_bIdValid ) value += DEF_VALID_MASK;
          if( m_bComplex ) value += DEF_COMPLEX_MASK;

          BitConverter.GetBytes( value ).CopyTo( buffer, 0 );
          BitConverter.GetBytes( m_uiValue ).CopyTo( buffer, 2 );

          return buffer;
        }
      }

      /// <summary>
      /// 
      /// </summary>
      public static int Size
      {
        get
        {
          return DEF_RECORD_SIZE;
        }
      }
      #endregion

      #region Class Initialize/Finalize methods
      /// <summary>
      /// 
      /// </summary>
      public FOPTE()
      {
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="data"></param>
      /// <param name="iOffset"></param>
      public FOPTE( byte[] data, ref int iOffset )
      {
        ushort value = BitConverter.ToUInt16( data, iOffset );
        m_usId = ( ushort ) ( value & DEF_ID_MASK );
        
        m_bIdValid = ( ( value & DEF_VALID_MASK ) != 0 );
        m_bComplex = ( ( value & DEF_COMPLEX_MASK ) != 0 );

        iOffset += 2;

        m_uiValue = BitConverter.ToUInt32( data, iOffset );
        iOffset += 4;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="stream"></param>
      public FOPTE( Stream stream )
      {
        ushort value = ReadUInt16( stream );
        m_usId = ( ushort )( value & DEF_ID_MASK );

        m_bIdValid = ( ( value & DEF_VALID_MASK ) != 0 );
        m_bComplex = ( ( value & DEF_COMPLEX_MASK ) != 0 );

        m_uiValue = ReadUInt32( stream );
      }
      #endregion

      #region Class Public Methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="m_data"></param>
      /// <param name="iOffset"></param>
      public void ReadComplexData( byte[] m_data, ref int iOffset )
      {
        if( IsComplex )
        {
          m_arrData = new byte[ UInt32Value ];
          Array.Copy( m_data, iOffset, m_arrData, 0, (int)UInt32Value );
          iOffset += ( int )UInt32Value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="stream"></param>
      public void ReadComplexData( Stream stream )
      {
        if( IsComplex )
        {
          int iDataSize = ( int )UInt32Value;
          m_arrData = new byte[ iDataSize ];
          stream.Read( m_arrData, 0, ( int )iDataSize );
        }
      }
      #endregion

      #region ICloneable methods
      /// <summary>
      /// Clone current instance.
      /// </summary>
      /// <returns>Return Clone of current object.</returns>
      public object Clone()
      {
        FOPTE result = ( FOPTE )MemberwiseClone();

        if( m_arrData != null )
        {
          result.m_arrData = CloneUtils.CloneByteArray( m_arrData );
        }

        return result;
      }
      #endregion
    };
    #endregion

    #region Class constants
    /// <summary>
    /// Minimum option index.
    /// </summary>
    private const int DEF_MINOPTION_INDEX = 127;
    #endregion

    #region Class members
    /// <summary>
    /// List with shape properties.
    /// </summary>
    private List<FOPTE> m_arrProperties = new List<FOPTE>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtOPT( MsoBase parent )
      : base( parent )
    {
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtOPT( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      int iOffset = 0;
      int iComplexDataStart = m_iLength;//m_data.Length;

      while( iOffset < iComplexDataStart )
      {
        FOPTE prop = new FOPTE( stream );
        AddOptions( prop );

        if( prop.IsComplex )
        {
          iComplexDataStart -= ( int )prop.UInt32Value;
        }

        iOffset += FOPTE.Size;
      }

      for( int i = 0, len = m_arrProperties.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        FOPTE prop = m_arrProperties[ i ];
        prop.ReadComplexData( stream );
      }
    }
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      //TODO: change fill method
      //Instance = m_arrProperties.Count;

      if( Instance == 0 )
      {
        m_usVersionAndInst = 0x33;
      }

      int len = m_arrProperties.Count;
      
      if( len > 0 )
      {
        int previousId = ( int )m_arrProperties[ 0 ].Id;

        int count = ( previousId > 0 ) ? 1 : 0;
        int iFirstId = previousId;
        bool updateInstance = true;
          for( int i = 1; i < len; i++ )
          {
            FOPTE option = m_arrProperties[ i ];

            if( ( int )option.Id > previousId )
            {
              //count++;
              previousId = ( int )option.Id;

              //if( previousId > iFirstId )
                count++;
            }
            else
            {
                updateInstance = false;
            }
          }

          int iPropertiesCount = m_arrProperties.Count;
          
          if( iFirstId <= 4 )
          {
            int lastId = ( int )m_arrProperties[ iPropertiesCount - 1 ].Id;
            if( ( lastId > 1000 || count != m_arrProperties.Count ) && count > 10 && lastId > 100 && updateInstance )
              count--;
          }

          //if( iFirstId <= 4 && iPropertiesCount > 3 &&
          //  ( int )m_arrProperties[ iPropertiesCount - 1 ].Id < ( int )m_arrProperties[ iPropertiesCount - 2 ].Id )
          //  count--;
          Instance = count;
      }

      m_iLength = 0;
      byte[] buffer;

      for( int i = 0;i < len;i++ )
      {
        FOPTE option = m_arrProperties[ i ];

        buffer = option.MainData;
        //SetBytes( m_iLength, buffer );
        int iCount = buffer.Length;
        stream.Write( buffer, 0, iCount );
        m_iLength += iCount;
      }

      for( int i = 0; i < len; i++ )
      {
        FOPTE option = m_arrProperties[ i ];

        if( option.AdditionalData != null )
        {
          buffer = option.AdditionalData;
          int iCount = buffer.Length;
          //SetBytes( m_iLength, buffer );
          stream.Write( buffer, 0, iCount );
          m_iLength += iCount;
        }
      }
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns cloned instance.</returns>
    public override object Clone()
    {
      return InternalClone();
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns cloned instance.</returns>
    protected override object InternalClone()
    {
      MsofbtOPT instance = ( MsofbtOPT )base.InternalClone();
      instance.m_arrProperties = CloneUtils.CloneCloneable( m_arrProperties );
      return instance;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Array with shape properties. Read-only.
    /// </summary>
    public FOPTE[] Properties
    {
      get
      {
        return m_arrProperties.ToArray();
      }
    }
    /// <summary>
    /// Returns singe option from the collection. Read-only.
    /// </summary>
    public FOPTE this[ int index ]
    {
      get
      {
        if( index < 0 || index >= m_arrProperties.Count )
          throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than than Count - 1." );

        return m_arrProperties[ index ];
      }
    }
    /// <summary>
    /// Gets property list with all properties.
    /// </summary>
    public IList<FOPTE> PropertyList
    {
      get
      {
        return m_arrProperties;
      }
    }
    #endregion

    #region Class Public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="option"></param>
    public void AddOptions( FOPTE option )
    {
      m_arrProperties.Add( option );
//      if( m_arrProperties.ContainsKey( option.Id ) )
//      {
//        m_arrProperties[ option.Id ] = option;
//      }
//      else
//      {
//        m_arrProperties.Add( option.Id, option );
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public void AddOptions( ICollection options )
    {
      foreach( FOPTE option in options )
      {
        AddOptions( option );
      }
      //m_arrProperties.AddRange( options );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="option">Option to set</param>
    public void AddOptionsOrReplace( FOPTE option )
    {
      int iIndex = IndexOf( option );
      
      if( iIndex == m_arrProperties.Count )
      {
        m_arrProperties.Add( option );
      }
      else
      {
        m_arrProperties[ iIndex ] = option;
      }
    }
    /// <summary>
    /// Replaces option with specified value.
    /// </summary>
    /// <param name="option">Option to set.</param>
    public void AddOptionSorted( FOPTE option )
    {
      int i = 0;
      int iCount = m_arrProperties.Count;
      MsoOptions currentId = option.Id;
      
      for( int len = iCount; i < len; i++ )
      {
        if( m_arrProperties[ i ].Id >= currentId ) break;
      }

      if( i < iCount )
      {
        FOPTE curOption = m_arrProperties[ i ];

        if( curOption.Id == currentId )
        {
          m_arrProperties[ i ] = option;
        }
        else
        {
          m_arrProperties.Insert( i, option );
        }
      }
      else
      {
        m_arrProperties.Add( option );
      }
    }
    /// <summary>
    /// Searches for option in the record.
    /// </summary>
    /// <param name="option">Option to find.</param>
    /// <returns>Index of the option.</returns>
    private int IndexOf( FOPTE option )
    {
      return IndexOf( option.Id );
    }
    /// <summary>
    /// Removes some option by index.
    /// </summary>
    /// <param name="index">Index of option to remove.</param>
    public void RemoveOption( int index )
    {
      for( int i = 0, iLen = m_arrProperties.Count ; i < iLen; i++ )
      {
        FOPTE rec = m_arrProperties[ i ];

        if( rec.Id == ( MsoOptions )index )
        {
          m_arrProperties.RemoveAt( i );
          break;
        }
      }
    }
    /// <summary>
    /// Returns index of option index.
    /// </summary>
    /// <param name="optionId"></param>
    /// <returns></returns>
    public int IndexOf( MsoOptions optionId )
    {
      int i = 0;

      for( int len = m_arrProperties.Count; i < len; i++ )
      {
        if( m_arrProperties[ i ].Id == optionId )
          break;
      }

      return i;
    }
    #endregion
  }
}
