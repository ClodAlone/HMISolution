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

using Syncfusion.XlsIO.Implementation;
using System.IO;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsoContainerBase.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public abstract class MsoContainerBase : MsoBase
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private List<MsoBase> m_arrItems = new List<MsoBase>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsoContainerBase( MsoBase parent )
      : base( parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsoContainerBase( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    /// <param name="dataGetter">Data getter.</param>
    public MsoContainerBase( MsoBase parent, byte[] data, int iOffset
      , GetNextMsoDrawingData dataGetter )
      : base( parent, data, iOffset, dataGetter )
    {
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iOffset"></param>
    private void ParseItems( Stream data, int iOffset )
    {
      long lLastPosition = data.Position + m_iLength;
      while( lLastPosition > data.Position )
      {
        MsoBase record;

        if( DataGetter != null )
        {
          record = MsoFactory.CreateMsoRecord( this, data, DataGetter );
        }
        else
        {
          record = MsoFactory.CreateMsoRecord( this, data );
        }

        m_arrItems.Add( record );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemToAdd"></param>
    public void AddItem( MsoBase itemToAdd )
    {
      if( itemToAdd == null )
        throw new ArgumentNullException( "itemToAdd" );

      m_arrItems.Add( itemToAdd );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    public void AddItems( ICollection<MsoBase> items )
    {
      if( items == null )
        throw new ArgumentNullException( "items" );

      m_arrItems.AddRange( items );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Array of items. Read-only.
    /// </summary>
    public MsoBase[] Items
    {
      get
      {
        return m_arrItems.ToArray();
      }
    }

    /// <summary>
    /// Internal list of items (to increase performance). Read-only.
    /// </summary>
    internal List<MsoBase> ItemsList
    {
      get
      {
        return m_arrItems;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      ParseItems( stream, 0 );
    }
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks,
      List<List<BiffRecordRaw>> arrRecords )
    {
      long lStartPosition = stream.Position;

      for( int i = 0, len = m_arrItems.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        MsoBase record = m_arrItems[ i ] as MsoBase;
        record.FillArray( stream, /*iOffset + arrData.Count + 8*/( int )stream.Position, arrBreaks, arrRecords );
      }

      m_iLength = ( int )( stream.Position - lStartPosition );//arrData.Count;//iDataSize;
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns cloned instance.</returns>
    protected override object InternalClone()
    {
      MsoContainerBase instance = ( MsoContainerBase )base.InternalClone();

      if( instance.m_arrItems != null )
      {
        int iLen = m_arrItems.Count;
        List<MsoBase> arr = new List<MsoBase>( iLen );

        for( int i = 0; i < iLen; i++ )
        {
          MsoBase o = ( MsoBase )m_arrItems[ i ].Clone( instance );
          arr.Add( o );
        }

        instance.m_arrItems = arr;
      }

      return instance;
    }
    #endregion

  }
}
