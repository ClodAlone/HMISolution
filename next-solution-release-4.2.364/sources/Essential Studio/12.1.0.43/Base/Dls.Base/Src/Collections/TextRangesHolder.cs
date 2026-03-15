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
using System.Collections;

namespace Syncfusion.DLS.Collections
{
	/// <summary>
	/// Represents a part text from paragraph with formatting.
  /// <remarks>
  /// Text ranges inside this class is a only references to textranges 
  /// in original document. 
  /// </remarks>
	/// </summary>
	public class TextRangesHolder
  : IEnumerable
	{
    #region Class members
	  private ArrayList m_itemsCollection = null;
	  private int m_startCut = 0;
    private int m_endCut = 0;
    #endregion
	  
    #region Class properties
	  /// <summary>
	  /// Gets / sets number of characters to cut at the begining.
	  /// </summary>
	  public int StartCut
	  {
	    get
	    {
	      return m_startCut;
	    }
	    set
	    {
	      m_startCut = value;
	    }
	  }
    /// <summary>
    /// Gets / sets number of characters to cut at the end.
    /// </summary>
    public int EndCut
    {
      get
      {
        return m_endCut;
      }
      set
      {
        m_endCut = value;
      }
    }
	  /// <summary>
	  /// Gets count of items.
	  /// </summary>
	  public int Count
	  {
	    get
	    {
	      return m_itemsCollection.Count;
	    }
	  }
    #endregion

    #region Class initialize/finalize methods
	  /// <summary>
	  /// Create a new TextRangesHolder
	  /// </summary>
    public TextRangesHolder()
    {
      m_itemsCollection = new ArrayList();
    }
    #endregion
	  
    #region Class public methods
	  /// <summary>
	  /// Adds a new Text Range to the Holder.
	  /// </summary>
	  /// <param name="textRange"></param>
	  public void Add( TextRange textRange )
	  {
	    m_itemsCollection.Add( textRange );
	  }
	  /// <summary>
	  /// Copies to the Specified Paragraph.
	  /// </summary>
	  /// <param name="paragraph"></param>
	  /// <param name="index"></param>
    public void CopyTo( Paragraph paragraph, int index )
	  {
	    int count = m_itemsCollection.Count;
	    if( count > 0 )
	    {
	      TextRange textRange = (m_itemsCollection[ 0 ] as TextRange ).Clone( paragraph ) as TextRange;
	      textRange.Text = textRange.Text.Remove( 0, m_startCut );
	      if( count == 1 )
	      {
	        textRange.Text = textRange.Text.Remove( textRange.TextLength - m_endCut, m_endCut );
	      }
	      paragraph.InsertItem( index, textRange );
       
	      for( int i = 1; i < count - 1; i++ )
	      {
	        paragraph.InsertItem( index + i, ( m_itemsCollection[ i ] as TextRange ).Clone( paragraph ) );
	      }
        
	      if( count > 1 )
	      {
	        textRange = (m_itemsCollection[ count - 1 ] as TextRange ).Clone( paragraph ) as TextRange;
	        textRange.Text = textRange.Text.Remove( textRange.TextLength - m_endCut, m_endCut );
	        paragraph.InsertItem( index + count - 1, textRange );
	      }
	    }
	  }
    #endregion

	  #region Class overrides
	  ///<summary>
	  ///
	  ///<para>
	  /// 
	  ///       Returns an enumerator that can iterate through a collection.
	  ///</para>
	  ///
	  ///</summary>
	  ///
	  ///<returns>
	  ///
	  ///<para>
	  ///An <see cref="T:System.Collections.IEnumerator" />
	  /// that can be used to iterate through the collection.
	  ///</para>
	  ///
	  ///</returns>
	  ///
	  public IEnumerator GetEnumerator()
	  {
	    return m_itemsCollection.GetEnumerator();
	  }
	  #endregion
	}
}
