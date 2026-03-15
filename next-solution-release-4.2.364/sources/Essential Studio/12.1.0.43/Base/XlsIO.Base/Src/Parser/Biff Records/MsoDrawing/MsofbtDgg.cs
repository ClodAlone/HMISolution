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
using System.IO;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Represents MsofbtDgg in MsoDrawing.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtDgg ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtDgg : MsoBase
  {
    #region Class internal declarations
    /// <summary>
    /// Cluster Id.
    /// </summary>
    public class ClusterID : ICloneable
    {
      #region Class constants
      /// <summary>
      /// Size of the record.
      /// </summary>
      private const int DEF_SIZE = 8;
      #endregion

      #region Class members
      /// <summary>
      /// DG owning the SPIDs in this cluster.
      /// </summary>
      private uint m_uiGroupId;
      /// <summary>
      /// Number of SPIDs used so far.
      /// </summary>
      private uint m_uiNumber;
      #endregion

      #region Class Initialize/Finalize methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="groupId"></param>
      /// <param name="number"></param>
      public ClusterID( uint groupId, uint number )
      {
        m_uiGroupId = groupId;
        m_uiNumber = number;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="data"></param>
      /// <param name="iOffset"></param>
      public ClusterID( byte[] data, int iOffset )
      {
        GroupId = BitConverter.ToUInt32( data, iOffset );
        iOffset += 4;
        Number = BitConverter.ToUInt32( data, iOffset );
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="stream"></param>
      public ClusterID( Stream stream )
      {
        GroupId = ReadUInt32( stream );
        Number = ReadUInt32( stream );
      }
      #endregion

      #region Class properties
      /// <summary>
      /// DG owning the SPIDs in this cluster.
      /// </summary>
      public uint GroupId
      {
        get
        {
          return m_uiGroupId;
        }
        set
        {
          m_uiGroupId = value;
        }
      }
      /// <summary>
      /// Number of SPIDs used so far.
      /// </summary>
      public uint Number
      {
        get
        {
          return m_uiNumber;
        }
        set
        {
          m_uiNumber = value;
        }
      }
      /// <summary>
      /// Record's size.
      /// </summary>
      public static int  Size
      {
        get
        {
          return DEF_SIZE;
        }
      }
      #endregion

      #region Class helper methods
      /// <summary>
      /// Converts record to the bytes array.
      /// </summary>
      /// <returns>Array of bytes with record's data.</returns>
      public byte[] GetBytes()
      {
        byte[] result = new byte[ Size ];
        BitConverter.GetBytes( m_uiGroupId ).CopyTo( result, 0 );
        BitConverter.GetBytes( m_uiNumber ).CopyTo( result, 4 );
        
        return result;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="stream"></param>
      public void Write( Stream stream )
      {
        MsoBase.WriteUInt32( stream, m_uiGroupId );
        MsoBase.WriteUInt32( stream, m_uiNumber );
      }
      #endregion

      #region ICloneable Methods
      /// <summary>
      /// Clone current instance.
      /// </summary>
      /// <returns>Return shallow copy of current instance.</returns>
      public object Clone()
      {
        return ( ClusterID )MemberwiseClone();
      }
      #endregion
    }
    #endregion

    #region Class constants
    /// <summary>
    /// Default offset to the array.
    /// </summary>
    private const int DEF_ARRAY_OFFSET = 16;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 4 ) ]
    private uint m_uiIdMax;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 4, 4 ) ]
    private uint m_uiNumberOfIdClus;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 8, 4 ) ]
    private uint m_uiTotalShapes;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 12, 4 ) ]
    private uint m_uiTotalDrawings;
    /// <summary>
    /// 
    /// </summary>
    private List<ClusterID> m_arrClusters = new List<ClusterID>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtDgg( MsoBase parent )
      : base( parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtDgg( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public uint IdMax
    {
      get
      {
        return m_uiIdMax;
      }
      set
      {
        m_uiIdMax = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public uint NumberOfIdClus
    {
      get
      {
        return m_uiNumberOfIdClus;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public uint TotalShapes
    {
      get
      {
        return m_uiTotalShapes;
      }
      set
      {
        m_uiTotalShapes = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public uint TotalDrawings
    {
      get
      {
        return m_uiTotalDrawings;
      }
      set
      {
        m_uiTotalDrawings = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ClusterID[] ClusterIDs
    {
      get
      {
        return m_arrClusters.ToArray();
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Parse Structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      m_uiIdMax = ReadUInt32( stream );
      m_uiNumberOfIdClus = ReadUInt32( stream );
      m_uiTotalShapes = ReadUInt32( stream );
      m_uiTotalDrawings = ReadUInt32( stream );
      int iOffset = DEF_ARRAY_OFFSET;

      if( m_uiNumberOfIdClus > 0 )
      {
        for( int i = 0; i < m_uiNumberOfIdClus - 1; i++, iOffset += ClusterID.Size )
        {
          ClusterID id = new ClusterID( stream );
          m_arrClusters.Add( id );
        }
      }
    }
    /// <summary>
    /// In this method, the class must pack all of it's properties into
    /// an internal Data array: m_data. This method is called by
    /// FillStream, when the record must be serialized into stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      WriteUInt32( stream, m_uiIdMax );
      WriteUInt32( stream, m_uiNumberOfIdClus );
      WriteUInt32( stream, m_uiTotalShapes );
      WriteUInt32( stream, m_uiTotalDrawings );
      m_iLength = 16;

      for( int i = 0, len = m_arrClusters.Count; i < len; i++, m_iLength += ClusterID.Size )
      {
        ClusterID id = m_arrClusters[ i ];
        //SetBytes( m_iLength, id.GetBytes() );
        //stream.Write( id.GetBytes(), 0, ClusterID.Size );
        id.Write( stream );
      }
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns cloned instance.</returns>
    protected override object InternalClone()
    {
      MsofbtDgg instance = ( MsofbtDgg )base.InternalClone();

      if( m_arrClusters != null )
      {
        int iLen = m_arrClusters.Count;

        List<ClusterID> arr = new List<ClusterID>( iLen );

        for( int i = 0; i < iLen; i++ )
        {
          ClusterID o = ( ClusterID )m_arrClusters[ i ].Clone();
          arr.Add( o );
        }

        instance.m_arrClusters = arr;
      }

      return instance;
    }
    #endregion

    #region Class Public methods
    /// <summary>
    /// Adds cluster.
    /// </summary>
    /// <param name="uiGroupId">Group id.</param>
    /// <param name="uiNumber">Number.</param>
    public void AddCluster( uint uiGroupId, uint uiNumber )
    {
      ClusterID id = new ClusterID( uiGroupId, uiNumber );
      m_arrClusters.Add( id );

      m_uiNumberOfIdClus = ( uint ) ( m_arrClusters.Count + 1 );
      m_uiTotalDrawings = ( uint ) m_arrClusters.Count;
    }
    #endregion
  }
}
