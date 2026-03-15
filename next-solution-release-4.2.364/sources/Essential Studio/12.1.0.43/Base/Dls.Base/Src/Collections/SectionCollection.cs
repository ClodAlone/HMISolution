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

using System;

using Syncfusion.DLS.XML;

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Represents section collection
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class SectionCollection
    : EntityCollectionBase,
      ISectionCollection
  {
    #region Class properties
    /// <summary>
    /// Gets section by index
    /// </summary>
    public ISection this[ int index ]
    {
      get
      {
        return ( ISection )List[ index ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    protected internal SectionCollection( IDocument doc )
      : base( doc )
    {}
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds section to collection
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    public int Add( ISection section )
    {
      return List.Add( section );
    }
    #endregion

    #region IDLSXmlEntityCollection implements
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object CreateItem( IXDLSContentReader reader )
    {
      return (Document as Document).CreateSectionImpl();
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.SectionItemTag;
      }
    }
    #endregion
  }
}