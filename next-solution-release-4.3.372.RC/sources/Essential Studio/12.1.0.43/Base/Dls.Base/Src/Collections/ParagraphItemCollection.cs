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

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Represents paragraph item collection
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class ParagraphItemCollection
    : EntityCollectionBase,
      IParagraphItemCollection
  {
    #region Class properties
    /// <summary>
    /// Gets document by index
    /// </summary>
    public IParagraphItem this[ int index ]
    {
      get
      {
        return List[ index ] as IParagraphItem;
      }
    }
    #endregion

    #region Class helper properties
    /// <summary>
    /// 
    /// </summary>
    protected IParagraph OwnerParagraph
    {
      get
      {
        return base.Owner as IParagraph;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor for specified document
    /// </summary>
    /// <param name="doc"></param>
    public ParagraphItemCollection( IDocument doc )
      : base( doc )
    {
      UpdateRestrictedForItemOwner = false;
    }
    /// <summary>
    /// Initializing constructor for specified section
    /// </summary>
    /// <param name="owner"></param>
    internal protected ParagraphItemCollection( IParagraph owner )
      : base( owner )
    {
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds paragraph item to collection
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public int Add( IParagraphItem pItem )
    { 
      int index = List.Add( pItem );
      UpdateItem( pItem as ParagraphItem );
      return index;
    }
    /// <summary>
    /// Gets index of item in collection
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf( IParagraphItem item )
    {
      return List.IndexOf( item );
    }
    /// <summary>
    /// Inserts an paragraph item to collection
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pItem"></param>
    public void Insert( int index, IParagraphItem pItem )
    {
      List.Insert( index, pItem );
      UpdateItem( pItem as ParagraphItem, index - 1 );
    }
    /// <summary>
    /// Removes specific object from collection
    /// </summary>
    /// <param name="pItem"></param>
    public void Remove( IParagraphItem pItem )
    {
      if( !List.Contains( pItem ) )
        throw new ArgumentException( "Collection must contains item", "pItem" );
        
      ITextRange trItem = pItem as ITextRange;
      
      if( trItem != null ) trItem.Text = "";
      
      List.Remove( pItem );
    }
    /// <summary>
    /// Removes object specified by index from collection
    /// </summary>
    /// <param name="index"></param>
    new public void RemoveAt( int index )
    {
      Remove( this[ index ] );
    }
    /// <summary>
    /// Unsafe method for remove item at index.
    /// </summary>
    /// <param name="index">The index.</param>
    internal void UnsafeRemoveAt( int index )
    {
      List.Remove( this[ index ] );
    }
    #endregion

    #region IXDLSSerializableCollection implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object CreateItem( IXDLSContentReader reader )
    {
      object oItemType = GetItemType( reader, typeof( ParagraphItemType ), ParagraphItemType.TextRange );
      object oItem = DocumentEx.CreateParagraphItem( oItemType );
      
      ParagraphItem pItem = oItem as ParagraphItem;
      
      if( pItem != null )
      {
        pItem.SetOwnerParagraph( OwnerParagraph, 0 );
      }
     
      return oItem;
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.ItemTag;
      }
    }
    #endregion
    
    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal void AddWithoutUpdate( IParagraphItem item )
    {
      List.Add( item );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pItem"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private void UpdateItem( ParagraphItem pItem )
    {
      IParagraph ownerPara = OwnerParagraph;
      pItem.SetOwnerParagraph( ownerPara,
                               ownerPara.Text.Length );
         
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pItem"></param>
    /// <param name="index"></param>
    private void UpdateItem( ParagraphItem pItem, int index )
    {
      if( !UpdateRestrictedForItemOwner )
      {
        int startIndex = 0;
        
        if( index > -1 )
        {
          ParagraphItem prevPItem = this[ index ] as ParagraphItem;
          startIndex = prevPItem.StartIndex;

          TextRange prevTextRange = prevPItem as TextRange;
        
          if( prevTextRange != null )
          {
            startIndex += prevTextRange.TextLength;
          }
        }

        pItem.SetOwnerParagraph( OwnerParagraph, startIndex );
      }
    }
    #endregion
  }
}