#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Globalization;
using Syncfusion.CompoundFile.XlsIO;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
	/// <summary>
	/// Class used for Relations Collection.
	/// </summary>
	public class RelationCollection :
    IEnumerable,
    ICloneable
	{
    #region Class constants
    /// <summary>
    /// Relation id start.
    /// </summary>
    private const string RelationIdStart = "rId";
    /// <summary>
    ///  Length of the relation id start.
    /// </summary>
    private static readonly int RelationIdStartLen = RelationIdStart.Length;
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary with relations. Key - relation id, value = relation object.
    /// </summary>
    private Dictionary<string, Relation> m_dicRelations = new Dictionary<string, Relation>();
    /// <summary>
    /// Path to the item (this member should be filled when extracting collection from file).
    /// </summary>
    private string m_strItemPath;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the RelationsCollection class.
    /// </summary>
    public RelationCollection()
    {
    }
    #endregion

    #region Properites
    /// <summary>
    /// Gets / sets relation by id.
    /// </summary>
    public Relation this[ string id ]
    {
      get
      {
        Relation result;
        m_dicRelations.TryGetValue( id, out result );

        return result;
      }
      set
      {
        m_dicRelations[ id ] = value;
      }
    }
    /// <summary>
    /// Gets number of items in the collection. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return m_dicRelations.Count;
      }
    }
    /// <summary>
    /// Gets or sets path to the item (this member should be filled when extracting collection from file).
    /// </summary>
    public string ItemPath
    {
      get
      {
        return m_strItemPath;
      }
      set
      {
        m_strItemPath = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Removes relation by id.
    /// </summary>
    /// <param name="id">Relation id to remove.</param>
    public void Remove( string id )
    {
      m_dicRelations.Remove( id );
    }
    /// <summary>
    /// Removes relation by content type.
    /// </summary>
    /// <param name="contentType">Content type of relation to delete.</param>
    public void RemoveByContentType( string contentType )
    {
      if( contentType != null && contentType.Length > 0 )
      {
        foreach( KeyValuePair<string, Relation> entry in m_dicRelations )
        {
          Relation relation = entry.Value;

          if( relation.Type == contentType )
          {
            m_dicRelations.Remove( entry.Key );
            break;
          }
        }
      }
    }
    /// <summary>
    /// Searches for relation with appropriate content type.
    /// </summary>
    /// <param name="contentType">Content type to find.</param>
    /// <param name="relationId">Relation id.</param>
    /// <returns>Relation that contains desired content type or null if not found.</returns>
    public Relation FindRelationByContentType( string contentType, out string relationId )
    {
      Relation result = null;
      relationId = null;

      if( contentType != null && contentType.Length > 0 )
      {
        foreach( KeyValuePair<string, Relation> entry in m_dicRelations )
        {
          Relation relation = entry.Value;

          if( relation.Type == contentType )
          {
            result = relation;
            relationId = entry.Key;
            break;
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Searches for relation with appropriate target path.
    /// </summary>
    /// <param name="itemName">Item name to find.</param>
    /// <returns>Relation id that contains points to specified content type or null if not found.</returns>
    public string FindRelationByTarget( string itemName )
    {
      string result = null;

      if( itemName != null && itemName.Length > 0 )
      {
        foreach( KeyValuePair<string, Relation> entry in m_dicRelations )
        {
          Relation relation = entry.Value;

          if( relation != null && relation.Target == itemName )
          {
            result = entry.Key;
            break;
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Generates relation id that can be used as relation key.
    /// </summary>
    /// <returns>Free relation key.</returns>
    public string GenerateRelationId()
    {
      string strRelationId = null;

      for( int i = 1; i < int.MaxValue; i++ )
      {
        strRelationId = RelationIdStart + i;

        if( !m_dicRelations.ContainsKey( strRelationId ) )
          break;
      }

      return strRelationId;
    }
    /// <summary>
    /// Generates id and adds relations to there collection.
    /// </summary>
    /// <param name="relation">Relation to add.</param>
    /// <returns>Relation id.</returns>
    public string Add( Relation relation )
    {
      string strId = string.Empty;

      if (relation == null)
          return strId;

      strId = GenerateRelationId();
      
      this[ strId ] = relation;

      return strId;
    }
    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear()
    {
      m_dicRelations.Clear();
    }
    /// <summary>
    /// Creates copy of the current collection.
    /// </summary>
    /// <returns>A copy of the current collection.</returns>
    public RelationCollection Clone()
    {
      RelationCollection result = ( RelationCollection )MemberwiseClone();
      result.m_dicRelations = CloneUtils.CloneHash( m_dicRelations );
      return result;
    }
    /// <summary>
    /// Creates copy of the current collection.
    /// </summary>
    /// <returns>A copy of the current collection.</returns>
    object ICloneable.Clone()
    {
      return Clone();
    }
    #endregion

    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that can iterate through a collection.
    /// </summary>
    /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
    public IEnumerator GetEnumerator()
    {
      return m_dicRelations.GetEnumerator();
    }

    #endregion
  }
}
