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
using Syncfusion.DLS.XML;
using System;
#endregion

namespace Syncfusion.DLS.Collections
{
	/// <summary>
	/// Summary description for TabCollection.
	/// </summary>
  public class TabCollection
    : EntityCollectionBase
  {
    #region Class properties
    /// <summary>
    /// Gets the <see cref="Tab"/> at the specified index.
    /// </summary>
    /// <value></value>
    public Tab this[ int index ]
    {
      get
      {
        return ( Tab )List[ index ];
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the <see cref="TabCollection"/> class.
    /// </summary>
    protected internal TabCollection( IDocument document )
      : base( document )
    {
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Adds the tab.
    /// </summary>
    /// <returns></returns>
    public Tab AddTab()
    {
      return AddTab( 0, TabJustification.Left, TabLeader.NoLeader );
    }
    /// <summary>
    /// Adds the tab.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="justification">The justification.</param>
    /// <param name="leader">The leader.</param>
    /// <returns></returns>
    public Tab AddTab( float position, TabJustification justification, TabLeader leader )
    {
      Tab tab = new Tab( Document, position, justification, leader );
      List.Add( tab );
      return tab;
    }
    /// <summary>
    /// Adds the tab.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public Tab AddTab( float position )
    {
      return AddTab( position, TabJustification.Left, TabLeader.NoLeader );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tab"></param>
    internal void AddTab( Tab tab )
    {
      List.Add( tab );
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Creates the item.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    protected override object CreateItem( IXDLSContentReader reader )
    {
      return new Tab( Document );
    }

    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    /// <value></value>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.TabTag;
      }
    }
    #endregion
  }
}
