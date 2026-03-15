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
#endregion

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Represents paragraph collection
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class ParagraphCollection
    : EntityCollectionBase,
      IParagraphCollection
  {
    #region Class properties
    /// <summary>
    /// Gets collection owner
    /// </summary>
    new internal protected ITextBody Owner
    {
      get
      {
        return base.Owner as ITextBody;
      }
    }
    /// <summary>
    /// Gets document by index
    /// </summary>
    public IParagraph this[ int index ]
    {
      get
      {
        return ( IParagraph )List[ index ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor for specified owner
    /// </summary>
    /// <param name="owner"></param>
    protected internal ParagraphCollection( IEntityBase owner )
      : base( owner )
    {}
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds paragraph to collection
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public int Add( IParagraph paragraph )
    {
      DocumentEx.EnsureParagraphStyle( paragraph );
      return List.Add( paragraph );
    }
    /// <summary>
    /// Inserts paragraph to specified position
    /// </summary>
    /// <param name="index"></param>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public void Insert( int index, IParagraph paragraph )
    {
      DocumentEx.EnsureParagraphStyle( paragraph );
      List.Insert( index, paragraph );
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object CreateItem( IXDLSContentReader reader )
    {
      return Document.CreateParagraph();
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.ParagraphItemTag;
      }
    }
    #endregion
  }
}