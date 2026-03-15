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
	/// Summary description for LayoutInfo.
	/// </summary>
  public class LayoutInfo
    : ILayoutInfo
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsClippedV = false;
    private bool m_bIsClippedH = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsSkip = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsSkipBottomAlign = false;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_bIsLineContainer = false;
    /// <summary>
    /// 
    /// </summary>
    private ChildrenLayoutDirection m_childrenLayoutDirection = ChildrenLayoutDirection.Horizontal;
    /// <summary>
    /// 
    /// </summary>
    private Spacings m_paddings;
    /// <summary>
    /// 
    /// </summary>
    private Spacings m_margins;
    /// <summary>
    /// 
    /// </summary>
    private bool m_isLineBreak = false;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_textWrap = true;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_bPageBreakItem = false;
    #endregion
	  
    #region Properties / ILayoutInfo
    /// <summary>
    /// Gets or sets a value indicating whether [text wrap].
    /// </summary>
    /// <value><c>true</c> if [text wrap]; otherwise, <c>false</c>.</value>
    public bool TextWrap
    {
      get
      {
        return m_textWrap;
      }
      set
      {
        m_textWrap = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is clipped.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is clipped; otherwise, <c>false</c>.
    /// </value>
    public bool IsClippedVertical
    {
      get
      {
        return m_bIsClippedV;
      }
      set
      {
        m_bIsClippedV = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is clipped horizontal.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is clipped horizontal; otherwise, <c>false</c>.
    /// </value>
    public bool IsClippedHorizontal
    {
      get
      {
        return m_bIsClippedH;
      }
      set
      {
        m_bIsClippedH = value;
      }
    }
    /// <summary>
    /// Gets a value indicating whether this instance is skip.
    /// </summary>
    /// <value><c>true</c> if this instance is skip; otherwise, <c>false</c>.</value>
    public bool IsSkip
    {
      get
      {
        return m_bIsSkip;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is skip bottom align.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is skip bottom align; otherwise, <c>false</c>.
    /// </value>
    public bool IsSkipBottomAlign
    {
      get
      {
        return m_bIsSkipBottomAlign;
      }
      set
      {
        m_bIsSkipBottomAlign = value;
      }
    }
    /// <summary>
    /// Gets a value indicating whether this instance is line container.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is line container; otherwise, <c>false</c>.
    /// </value>
    public bool IsLineContainer
    {
      get
      {
        return m_bIsLineContainer;
      }
    }
    /// <summary>
    /// Gets the children layout direction.
    /// </summary>
    /// <value>The children layout direction.</value>
    public ChildrenLayoutDirection ChildrenLayoutDirection
    {
      get
      {
        return m_childrenLayoutDirection;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is line break.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is line break; otherwise, <c>false</c>.
    /// </value>
    public bool IsLineBreak
    {
      get
      {
        return m_isLineBreak;
      }
      set
      {
        m_isLineBreak = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPageBreakItem
    {
      get
      {
        return m_bPageBreakItem;
      }
      set
      {
        m_bPageBreakItem = value;
      }
    }
    #endregion

    #region Properties / ILayoutSpacingsInfo
    /// <summary>
    /// Gets the paddings.
    /// </summary>
    /// <value>The paddings.</value>
    public Spacings Paddings
    {
      get
      {
        if( m_paddings == null )
          m_paddings = new Spacings();
        
        return m_paddings;
      }
    }
    /// <summary>
    /// Gets the margins.
    /// </summary>
    /// <value>The margins.</value>
    public Spacings Margins
    {
      get
      {
        if( m_margins == null )
          m_margins = new Spacings();
        
        return m_margins;
      }
    }
    #endregion
	  
    #region Constructors
    /// <summary>
    /// 
    /// </summary>
    public LayoutInfo()
    {
      m_bIsSkip = true;
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutInfo( ChildrenLayoutDirection childLayoutDirection )
    {
      m_childrenLayoutDirection = childLayoutDirection;
    }
    #endregion
  }
}
