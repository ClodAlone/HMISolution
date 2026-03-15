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

#endregion

namespace Syncfusion.DLS
{
	/// <summary>
	/// Represents a Bookmark. Holds BookmarkStart and BookmarkEnd in the document.
	/// </summary>
	public class Bookmark
	{
    #region Class members
    internal BookmarkStart m_bkmkStart = null;
    internal ParagraphItem m_bkmkEnd = null;
    #endregion
	  
    #region Class properties
	  /// <summary>
	  /// Gets bookmark name.
	  /// </summary>
	  public string Name
	  {
	    get
	    {
	      return m_bkmkStart.Name;
	    }
	  }
	  /// <summary>
	  /// Gets BookmarkStart object.
	  /// </summary>
	  public BookmarkStart BookmarkStart
	  {
	    get
	    {
	      return m_bkmkStart;
	    }
	  }
    /// <summary>
    /// Gets BookmarkEnd object.
    /// </summary>
    public BookmarkEnd BookmarkEnd
    {
      get
      {
        return ( m_bkmkEnd as BookmarkEnd );
      }
      set
      {
        m_bkmkEnd = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates new Bookmark object for the specified document.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public Bookmark( BookmarkStart start, ParagraphItem end )
    {
      m_bkmkStart = start;
      m_bkmkEnd = end;
    }
    #endregion
  }
}
