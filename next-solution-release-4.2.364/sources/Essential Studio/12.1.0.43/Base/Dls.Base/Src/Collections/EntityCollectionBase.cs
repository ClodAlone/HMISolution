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

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Base implemntation of entity collection
  /// </summary>
  public abstract class EntityCollectionBase
    : CollectionBase,
      IXDLSSerializableCollection
  {
    #region Class members
    /// <summary>
    /// The entity collection document
    /// </summary>
    private IDocument m_doc;
    /// <summary>
    /// The entity collection owner
    /// </summary>
    private IEntityBase m_owner = null;
    /// <summary>
    /// The restriction flag for item Owner updating.
    /// </summary>
    private bool m_bUpdateRestrictedForItemOwner = false;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets entity document.
    /// </summary>
    public IDocument Document
    {
      get
      {
        return m_doc;
      }
    }
    #endregion

    #region Class helper properties
    /// <summary>
    /// Gets entity collection owner.
    /// </summary>
    protected IEntityBase Owner
    {
      get
      {
        return m_owner;
      }
    }
    /// <summary>
    /// Gets DLS document implementation.
    /// </summary>
    protected Document DocumentEx
    {
      get
      {
        return m_doc as Document;
      }
    }
    /// <summary>
    /// When set this property to TRUE, item Owner property will not updated.
    /// </summary>
    internal protected bool UpdateRestrictedForItemOwner
    {
      get
      {
        return m_bUpdateRestrictedForItemOwner;
      }
      set
      {
        m_bUpdateRestrictedForItemOwner = value;
      }
    }
    #endregion

    #region Class initialize / finalize methods
    /// <summary>
    /// Creates new collection by specified owner.
    /// </summary>
    protected EntityCollectionBase( IEntityBase owner ) 
    {
      m_owner = owner;

      if( m_owner != null )
      {
        m_doc = owner.Document;
        if( m_doc == null )
          throw new DLSException( "Collection owner must have not NULL Document property!");
      }
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Gets index of the specified item in the collection.
    /// </summary>
    /// <param name="item">The specified entity item.</param>
    /// <returns></returns>
    public int IndexOf( IEntityBase item )
    {
      return List.IndexOf( item );
    }
    #endregion

    #region IXDLSSerializableCollection implement
    /// <summary>
    /// Collection must creates and adds new empty item.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    IXDLSSerializable IXDLSSerializableCollection.AddNewItem( IXDLSContentReader reader )
    {
      if( Document == null )
        throw new XDLSException( "Collection cannot serialize if Document property is NULL" );
      if( Owner == null )
        throw new XDLSException( "Collection cannot serialize if Owner property is NULL" );

      object item = CreateItem( reader );
      if( item != null )
      {
        List.Add( item );
      }

      return item as IXDLSSerializable;
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public virtual string TagItemName
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Creates the item.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected virtual object CreateItem( IXDLSContentReader reader )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Gets the type of the item.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="defValue">The def value.</param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected object GetItemType( IXDLSContentReader reader, Type enumType, object defValue )
    {
      string type = reader.GetAttributeValue( XDLSConstants.TypeTag );

      if( type != null )
      {
        string[] names = Enum.GetNames( enumType );
        
        foreach( string name in names )
        {
          if( name == type )
            return Enum.Parse( enumType, type, true );
        }
        return type;
      }

      return defValue;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"></see> instance.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at index.</param>
    protected override void OnInsertComplete( int index, object value )
    {
      EntityBase ent = value as EntityBase;
      
      if( ent != null )
      {
        UpdateOwner( ent );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void OnSetComplete( int index, object oldValue, object newValue )
    {
      EntityBase newEnt = newValue as EntityBase;
      EntityBase oldEnt = oldValue as EntityBase;

      // Removes owner for old item
      if( oldEnt != null )
      {
        RemoveOwner( oldEnt );
      }

      // Updates owner for new item
      if( newEnt != null )
      {
        UpdateOwner( newEnt );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    protected override void OnRemoveComplete( int index, object value )
    {
      EntityBase ent = value as EntityBase;

      if( ent != null )
      {
        RemoveOwner( ent );
      }
    }
    /// <summary>
    /// Updates owner in specified entity.
    /// </summary>
    /// <param name="ent"></param>
    protected void UpdateOwner( EntityBase ent )
    {
      if( Owner != null && !UpdateRestrictedForItemOwner )
      {
        ent.SetOwner( Owner );
      }
    }
    /// <summary>
    /// Updates owner in specified entity.
    /// </summary>
    /// <param name="ent"></param>
    protected void RemoveOwner( EntityBase ent )
    {
      if( !UpdateRestrictedForItemOwner )
      {
        ent.SetOwner( null );
      }
    }
    #endregion
  }
}