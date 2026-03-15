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

namespace Syncfusion.Layouting
{
	/// <summary>
	/// Summary description for LayoutTableInfo.
	/// </summary>
	public class LayoutTableInfo
    : LayoutParagraphInfo
	{
    #region Members
    /// <summary>
    /// 
    /// </summary>
    protected byte m_verticalAlignment;
    /// <summary>
    /// 
    /// </summary>
    protected double m_rowHeight;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_isRowMergeContinue = false;
    /// <summary>
    /// /
    /// </summary>
    protected bool m_isRowMergeStart = false;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_isRowSplitted = false;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_isExactlyRowHeight = false;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_isColumnMergeContinue = false;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_isColumnMergeStart = false;
    #endregion
	  
    #region Properties
    /// <summary>
    /// Gets a value indicating whether this instance is column merge start.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is column merge start; otherwise, <c>false</c>.
    /// </value>
    public bool IsColumnMergeStart
    {
      get
      {
        return m_isColumnMergeStart;
      }
    }
    /// <summary>
    /// Gets a value indicating whether this instance is column merge continue.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is column merge continue; otherwise, <c>false</c>.
    /// </value>
    public bool IsColumnMergeContinue
    {
      get
      {
        return m_isColumnMergeContinue;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is exactly row height.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is exactly row height; otherwise, <c>false</c>.
    /// </value>
    public bool IsExactlyRowHeight
    {
      get
      {
        return m_isExactlyRowHeight;
      }
      set
      {
        m_isExactlyRowHeight = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is row splitted.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is row splitted; otherwise, <c>false</c>.
    /// </value>
    public bool IsRowSplitted
    {
      get
      {
        return m_isRowSplitted;
      }
      set
      {
        m_isRowSplitted = value;
      }
    }
    /// <summary>
    /// Gets a value indicating whether this instance is row merge start.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is row merge start; otherwise, <c>false</c>.
    /// </value>
    public bool IsRowMergeStart
    {
      get
      {
        return m_isRowMergeStart;
      }
    }
    /// <summary>
    /// Gets a value indicating whether this instance is row merge continue.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is row merge continue; otherwise, <c>false</c>.
    /// </value>
    public bool IsRowMergeContinue
    {
      get
      {
        return m_isRowMergeContinue;
      }
    }
    /// <summary>
    /// Gets or sets the height of the row.
    /// </summary>
    /// <value>The height of the row.</value>
    public double RowHeight
    {
      get
      {
        return m_rowHeight;
      }
      set
      {
        m_rowHeight = value;
      }
    }
    /// <summary>
    /// Gets the vertical alignment.
    /// </summary>
    /// <value>The vertical alignment.</value>
    public byte VerticalAlignment
    {
      get
      {
        return m_verticalAlignment;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutTableInfo"/> class.
    /// </summary>
    /// <param name="childLayoutDirection">The child layout direction.</param>
    public LayoutTableInfo( ChildrenLayoutDirection childLayoutDirection )
      : base( childLayoutDirection)
    {}
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutTableInfo"/> class.
    /// </summary>
    /// <param name="isExactlyRow">if set to <c>true</c> [is exactly row].</param>
    /// <param name="rowHeight">Height of the row.</param>
    public LayoutTableInfo( bool isExactlyRow, float rowHeight )
      : base()
    {
      m_isExactlyRowHeight = isExactlyRow;
      m_rowHeight = rowHeight;
    }
    #endregion
	}
}
