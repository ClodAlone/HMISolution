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
	/// Summary description for LayoutParagraphInfo.
	/// </summary>
	public class LayoutParagraphInfo
    : LayoutInfo
	{
    #region Members
    /// <summary>
    /// 
    /// </summary>
    protected bool m_listRestart = false;
    /// <summary>
    /// 
    /// </summary>
    protected int m_listItemIndex = 0;
    /// <summary>
    /// 
    /// </summary>
    protected int m_levelNumber = -1;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_isPageBreak = false;
    /// <summary>
    /// 
    /// </summary>
    protected HorizontalAlignment m_justification;
    /// <summary>
    /// 
    /// </summary>
    protected float m_firstLineIndent;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_isKeepWithNext = false;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_isKeepTogether = false;
    /// <summary>
    /// 
    /// </summary>
    protected float m_listTab;
    /// <summary>
    /// 
    /// </summary>
    protected byte m_listType = 0;
    /// <summary>
    /// 
    /// </summary>
    protected string m_strListStyleName = string.Empty;
    #endregion
	  
    #region Properties
    /// <summary>
    /// Gets a value indicating whether this instance is keep together.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is keep together; otherwise, <c>false</c>.
    /// </value>
    public bool IsKeepTogether
    {
      get
      {
        return m_isKeepTogether;
      }
    }
    /// <summary>
    /// Gets a value indicating whether this instance is keep with next.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is keep with next; otherwise, <c>false</c>.
    /// </value>
    public bool IsKeepWithNext
    {
      get
      {
        return m_isKeepWithNext;
      }
    }
    /// <summary>
    /// Gets the first line indent.
    /// </summary>
    /// <value>The first line indent.</value>
    public float FirstLineIndent
    {
      get
      {        
        return m_firstLineIndent;
      }      
    }
    /// <summary>
    /// Gets the justification.
    /// </summary>
    /// <value>The justification.</value>
    public HorizontalAlignment Justification
    {
      get
      {
        return m_justification;
      }
    }
    /// <summary>
    /// Gets a value indicating whether this instance is page break.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is page break; otherwise, <c>false</c>.
    /// </value>
    public bool IsPageBreak
    {
      get
      {
        return m_isPageBreak;
      }
    }
    /// <summary>
    /// Gets the level number.
    /// </summary>
    /// <value>The level number.</value>
    public int LevelNumber
    {
      get
      {
        return m_levelNumber;
      }
    }
    /// <summary>
    /// Gets or sets the index of the list item.
    /// </summary>
    /// <value>The index of the list item.</value>
    public int ListItemIndex
    {
      get
      {
        return m_listItemIndex;
      }
      set
      {
        m_listItemIndex = value;
      }
    }
    /// <summary>
    /// Gets a value indicating whether [list restart].
    /// </summary>
    /// <value><c>true</c> if [list restart]; otherwise, <c>false</c>.</value>
    public bool ListRestart
    {
      get
      {
        return m_listRestart;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public float ListTab
    {
      get
      {
        return m_listTab;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public byte ListTypeInfo
    {
      get
      {
        return m_listType;
      }
    }
    /// <summary>
    /// Gets the list style name.
    /// </summary>
    public string ListStyleName
    {
      get
      {
        return m_strListStyleName;
      }
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Signs the first line indent.
    /// </summary>
    public void SignFirstLineIndent()
    {
      m_firstLineIndent = -m_firstLineIndent;      
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutParagraphInfo"/> class.
    /// </summary>
    /// <param name="childLayoutDirection">The child layout direction.</param>
    public LayoutParagraphInfo( ChildrenLayoutDirection childLayoutDirection )
      : base( childLayoutDirection )
    {}
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutParagraphInfo"/> class.
    /// </summary>
    /// <param name="childLayoutDirection">The child layout direction.</param>
    /// <param name="isPageBreak">Is page break.</param>
    public LayoutParagraphInfo( ChildrenLayoutDirection childLayoutDirection, bool isPageBreak )
      : this( childLayoutDirection )
    {
      m_isPageBreak = isPageBreak;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutParagraphInfo"/> class.
    /// </summary>
    public LayoutParagraphInfo()
      : base()
    {
    }
    #endregion
	}
}
