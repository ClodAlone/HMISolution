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

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// The base class for DLS entities.
  /// <remarks>Supports Owner and Document properties.</remarks>
  /// </summary>
  public abstract class EntityBase : IEntityBase
  {
    #region Class members
    /// <summary>
    /// The entity owner
    /// </summary>
    private IEntityBase m_owner = null;
    /// <summary>
    /// The entity document
    /// </summary>
    private IDocument m_doc = null;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets entity owner.
    /// </summary>
    public IEntityBase Owner
    {
      get
      {
        return m_owner;
      }
    }
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
    /// Gets / sets DLS document implementation.
    /// </summary>
    protected internal Document DocumentEx
    {
      get
      {
        return m_doc as Document;
      }
      set
      {
        m_doc = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Default empty constructor, used only for Document 
    /// instance creation.
    /// </summary>
    protected EntityBase()
    {
    }
    /// <summary>
    /// Creates new entity by specified document.
    /// </summary>
    /// <param name="doc"></param>
    protected EntityBase( IDocument doc )
    {
      if( doc == null )
        throw new ArgumentNullException( "owner" );
      m_doc = doc;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="owner"></param>
    internal protected void SetOwner( IEntityBase owner )
    {
      m_owner = owner;
    }
    #endregion
  }
}