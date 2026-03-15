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
using System.IO;
using System.Collections;
#endregion

namespace Syncfusion.DLS
{
	/// <summary>
	/// Summary description for ParagraphEditor.
	/// </summary>
	public class ParagraphEditor
	{
    #region Class members
	  private IDocument m_doc;
	  private IParagraph m_tmpParagraph = null;
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    public ParagraphEditor( IDocument document )
    {
      m_doc = document;
      m_tmpParagraph = document.CreateParagraph();
    }
    #endregion
	  
    #region Class public methods
	  /// <summary>
	  /// 
	  /// </summary>
	  /// <param name="paragraph"></param>
	  /// <param name="startPItemIndex"></param>
	  /// <param name="endpItemIndex"></param>
	  public void Copy( IParagraph paragraph, int startPItemIndex, int endpItemIndex )
	  {
      if( paragraph == null )
        throw new ArgumentNullException( "paragraph" );
      if( startPItemIndex < 0  || startPItemIndex > paragraph.ItemsCount - 1 )
        throw new ArgumentOutOfRangeException( "startPItemIndex", startPItemIndex, "Value can not be less 0" );
      if( endpItemIndex < startPItemIndex || endpItemIndex > paragraph.ItemsCount - 1 )
        throw new ArgumentOutOfRangeException( "endpItemIndex", endpItemIndex, "Value can not be less startPItemIndex" );

	    m_tmpParagraph.Text = string.Empty;
	    
	    for( int i=startPItemIndex; i<= endpItemIndex; i ++ )
	    {
	      IParagraphItem pItem = paragraph[i].Clone(m_tmpParagraph);
	      m_tmpParagraph.InsertItem( m_tmpParagraph.ItemsCount, pItem );
	    }
	  }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    /// <param name="startPItemIndex"></param>
    /// <param name="endpItemIndex"></param>
    public void Cut( IParagraph paragraph, int startPItemIndex, int endpItemIndex )
    {
	    Copy(paragraph, startPItemIndex, endpItemIndex);
      
      for( int i=startPItemIndex; i<=endpItemIndex; i++ )
      {
        paragraph.RemoveItemAt( startPItemIndex );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    /// <param name="startPItemIndex"></param>
    public void Paste( IParagraph paragraph, int startPItemIndex )
    {
      if( paragraph == null )
        throw new ArgumentNullException( "paragraph" );
      if( startPItemIndex < 0  || startPItemIndex > paragraph.ItemsCount )
        throw new ArgumentOutOfRangeException( "startPItemIndex", startPItemIndex, "Value can not be less 0" );
      
      for( int i=0, len = m_tmpParagraph.ItemsCount; i<len; i++ )
      {
        paragraph.InsertItem( i + startPItemIndex, m_tmpParagraph[i].Clone(paragraph) );
      }
    }
    #endregion
  }
}