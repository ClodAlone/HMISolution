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
using System.Reflection;
using System.Diagnostics;

using Syncfusion.XlsIO.Implementation;
using System.IO;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsoFactory.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsoFactory
  {
    #region Class static members
    /// <summary>
    /// Represents new hashtable
    /// key - code; value - mso record.
    /// </summary>
    private static Dictionary<int, MsoBase> m_hashCodeToMSORecord = new Dictionary<int, MsoBase>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    private MsoFactory()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    static MsoFactory()
    {
      ////Assembly curAssembly = Assembly.GetAssembly( typeof( MsoFactory ) );
      
      //Type[] arrTypes = ApplicationImpl.AssemblyTypes;//curAssembly.GetTypes();

      //for( int i = arrTypes.Length - 1; i >= 0; i-- )
      //{
      //  Type type = arrTypes[ i ];

      //  MsoDrawingAttribute[] attributes = ( MsoDrawingAttribute[] )
      //    type.GetCustomAttributes( typeof( MsoDrawingAttribute ), false );

      //  if( attributes == null || attributes.Length == 0 ) continue;

      //  RegisterType( type, attributes );
      //}
      RegisterAllTypes();
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Create Mso Record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="recordType">Type of created record.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    /// <returns>Returns new instance of MsoBase record.</returns>
    public static MsoBase CreateMsoRecord( MsoBase parent, MsoRecords recordType, byte[] data
      , ref int iOffset )
    {
      MsoBase instance;
      int iRecordType = ( int )recordType;

      if( !m_hashCodeToMSORecord.ContainsKey( iRecordType ) )
      {
        instance = m_hashCodeToMSORecord[ ( int )MsoRecords.msoUnknown ];
      }
      else
      {
        instance = m_hashCodeToMSORecord[ iRecordType ];
      }

      instance = ( MsoBase )instance.Clone();

      instance.FillRecord( data, iOffset );
      iOffset += instance.Length + 8;

      return instance;
    }
    /// <summary>
    /// Create Mso Record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="recordType">Type of created record.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    /// <param name="dataGetter">Data getter.</param>
    /// <returns>Returns new instance of Mso Base.</returns>
    public static MsoBase CreateMsoRecord( MsoBase parent, MsoRecords recordType, byte[] data
      , ref int iOffset
      , GetNextMsoDrawingData dataGetter )
    {
      MsoBase instance;

      if( !m_hashCodeToMSORecord.ContainsKey( ( int )recordType ) )
      {
        instance = ( MsoBase )m_hashCodeToMSORecord[ ( int )MsoRecords.msoUnknown ];
      }
      else
      {
        instance = ( MsoBase )m_hashCodeToMSORecord[ ( int )recordType ];
      }

      instance = ( MsoBase )instance.Clone();


      instance.DataGetter = dataGetter;
      instance.FillRecord( data, iOffset );
      instance.UpdateNextMsoDrawingData();
      iOffset += instance.Length + 8;

      return instance;
    }
    /// <summary>
    /// Creates Mso record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    /// <returns>Returns new instance of Mso record.</returns>
    public static MsoBase CreateMsoRecord( MsoBase parent, byte[] data, ref int iOffset )
    {
      MsoRecords recordType = ( MsoRecords )BitConverter.ToUInt16( data, iOffset + 2 );

      return CreateMsoRecord( parent, recordType, data, ref iOffset );
    }
    /// <summary>
    /// Creates Mso record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    /// <param name="dataGetter">Data getter.</param>
    /// <returns>Returns new instance of Mso record.</returns>
    public static MsoBase CreateMsoRecord( MsoBase parent, byte[] data, ref int iOffset
      , GetNextMsoDrawingData dataGetter )
    {
      MsoRecords recordType = ( MsoRecords )BitConverter.ToUInt16( data, iOffset + 2 );

      return CreateMsoRecord( parent, recordType, data, ref iOffset, dataGetter );
    }
    /// <summary>
    /// Creates Mso record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="stream">Stream to get data from.</param>
    /// <returns>Returns new instance of Mso record.</returns>
    public static MsoBase CreateMsoRecord( MsoBase parent, Stream stream )
    {
      //MsoRecords recordType = ( MsoRecords )BitConverter.ToUInt16( data, iOffset + 2 );
      byte[] arrBuffer = new byte[ 4 ];
      stream.Read( arrBuffer, 0, 4 );
      stream.Position -= 4;
      MsoRecords recordType = ( MsoRecords )BitConverter.ToUInt16( arrBuffer, 2 );

      return CreateMsoRecord( parent, recordType, stream );
    }
    /// <summary>
    /// Create Mso Record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="recordType">Type of created record.</param>
    /// <param name="stream">Stream with record's data.</param>
    /// <returns>Returns new instance of MsoBase record.</returns>
    public static MsoBase CreateMsoRecord( MsoBase parent, MsoRecords recordType, Stream stream )
    {
      MsoBase instance;
      int iRecordType = ( int )recordType;

      if( !m_hashCodeToMSORecord.ContainsKey( iRecordType ) )
      {
        instance = m_hashCodeToMSORecord[ ( int )MsoRecords.msoUnknown ];
      }
      else
      {
        instance = m_hashCodeToMSORecord[ iRecordType ];
      }

      instance = ( MsoBase )instance.Clone();

      instance.FillRecord( stream );

      return instance;
    }
    /// <summary>
    /// Creates Mso record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="dataGetter">Data getter.</param>
    /// <returns>Returns new instance of Mso record.</returns>
    public static MsoBase CreateMsoRecord( MsoBase parent, Stream stream
      , GetNextMsoDrawingData dataGetter )
    {
      byte[] arrBuffer = new byte[ 4 ];
      stream.Read( arrBuffer, 0, 4 );
      stream.Position -= 4;
      MsoRecords recordType = ( MsoRecords )BitConverter.ToUInt16( arrBuffer, 2 );

      return CreateMsoRecord( parent, recordType, stream, dataGetter );
    }
    /// <summary>
    /// Create Mso Record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="recordType">Type of created record.</param>
    /// <param name="stream">Stream to get data from.</param>
    /// <param name="dataGetter">Data getter.</param>
    /// <returns>Returns new instance of Mso Base.</returns>
    public static MsoBase CreateMsoRecord( MsoBase parent, MsoRecords recordType,
      Stream stream, GetNextMsoDrawingData dataGetter )
    {
      MsoBase instance;

      if( !m_hashCodeToMSORecord.ContainsKey( ( int )recordType ) )
      {
        instance = m_hashCodeToMSORecord[ ( int )MsoRecords.msoUnknown ];
      }
      else
      {
        instance = m_hashCodeToMSORecord[ ( int )recordType ];
      }

      instance = ( MsoBase )instance.Clone();


      instance.DataGetter = dataGetter;
      instance.FillRecord( stream );
      instance.UpdateNextMsoDrawingData();

      return instance;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static MsoBase GetRecord( MsoRecords type )
    {
      MsoBase instance = ( MsoBase )m_hashCodeToMSORecord[ ( int )type ];

      if( instance != null )
      {
        instance = ( MsoBase )instance.Clone();
      }

      return instance;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="attributes"></param>
    private static void RegisterType( Type type, MsoDrawingAttribute[] attributes )
    {
      ConstructorInfo constr = type.GetConstructor( new Type[] { typeof( MsoBase ) } );

      if( constr == null )
        throw new ApplicationException( "Cannot find constructor" );

      object instance = constr.Invoke( new object[] { null } );

      m_hashCodeToMSORecord.Add( ( int )attributes[ 0 ].RecordType, ( MsoBase )instance );

      //Console.WriteLine( "item = new {0}();", type.Name );
      //Console.WriteLine( "m_hashCodeToMSORecord.Add( ( int )MsoRecords.{0}, item );", attributes[ 0 ].RecordType );
    }
    /// <summary>
    /// Registers all known mso record types inside internal collections.
    /// </summary>
    private static void RegisterAllTypes()
    {
      MsoBase item;

      item = new MsofbtClientTextBox( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtClientTextbox, item );

      item = new MsofbtSp( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtSp, item );

      item = new MsofbtSpgrContainer( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtSpgrContainer, item );

      item = new MsofbtAnchor( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtAnchor, item );

      item = new MsofbtClientAnchor( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtClientAnchor, item );

      item = new MsofbtDgContainer( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtDgContainer, item );

      item = new MsofbtRegroupItems( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtRegroupItems, item );

      item = new MsofbtDg( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtDg, item );

      item = new MsofbtDggContainer( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtDggContainer, item );

      item = new MsofbtOPT( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtOPT, item );

      item = new MsofbtSpContainer( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtSpContainer, item );

      item = new MsofbtSplitMenuColors( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtSplitMenuColors, item );

      item = new MsofbtDgg( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtDgg, item );

      item = new MsofbtBSE( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtBSE, item );

      item = new MsofbtSpgr( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtSpgr, item );

      item = new MsofbtBstoreContainer( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtBstoreContainer, item );

      item = new MsofbtClientData( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtClientData, item );

      item = new MsoUnknown( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msoUnknown, item );

      item = new MsofbtChildAnchor( null );
      m_hashCodeToMSORecord.Add( ( int )MsoRecords.msofbtChildAnchor, item );
    }
    #endregion
  }
}
