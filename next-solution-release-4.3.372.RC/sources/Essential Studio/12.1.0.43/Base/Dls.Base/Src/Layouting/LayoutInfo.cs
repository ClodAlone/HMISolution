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
using System.Drawing;
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Summary description for Spacings.
  /// </summary>
  public class Spacings
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private double m_left = 0;
    private double m_top = 0;
    private double m_right = 0;
    private double m_bottom = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>The left.</value>
    public double Left
    {
      get
      {
        return m_left;
      }
      set
      {
        m_left = value;
      }
    }
    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>The top.</value>
    public double Top
    {
      get
      {
        return m_top;
      }
      set
      {
        m_top = value;
      }
    }
    /// <summary>
    /// Gets or sets the right.
    /// </summary>
    /// <value>The right.</value>
    public double Right
    {
      get
      {
        return m_right;
      }
      set
      {
        m_right = value;
      }
    }
    /// <summary>
    /// Gets or sets the bottom.
    /// </summary>
    /// <value>The bottom.</value>
    public double Bottom
    {
      get
      {
        return m_bottom;
      }
      set
      {
        m_bottom = value;
      }
    }
    #endregion
  }

  /// <summary>
  /// Summary description for ILayoutSpacingsInfo.
  /// </summary>
  public interface ILayoutSpacingsInfo
  {                                                                    
    /// <summary>
    /// Gets the paddings.
    /// </summary>
    /// <value>The paddings.</value>
    Spacings Paddings { get; }
    /// <summary>
    /// Gets the margins.
    /// </summary>
    /// <value>The margins.</value>
    Spacings Margins { get; }
  }
  /// <summary>
  /// Summary description for LayoutInfo.
  /// </summary>
  public class LayoutInfo
    : ILayoutSpacingsInfo
  {
    #region Internal declarations
    /// <summary>
    /// 
    /// </summary>
    public enum LayoutDirection
    {
      /// <summary>
      /// 
      /// </summary>
      Horizontal,
      /// <summary>
      /// 
      /// </summary>
      Vertical
    }
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private bool m_bRowSplitted = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bTextWrap = true;
    /// <summary>
    /// 
    /// </summary>
    private byte m_bVerticalAlignment;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bKeepTogether = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bKeepWithNext = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bColumnMergeStart = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bColumnMergeContinue = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bRowMergeStart = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bRowMergeContinue = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bClipped = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bPageBreak;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bLineContainer = false;
    /// <summary>
    /// 
    /// </summary>
    private string m_strName = "(Range)";
    private HorizontalAlignment m_hrAlignment = HorizontalAlignment.Left;
    /// <summary>
    /// 
    /// </summary>
    private Spacings m_paddings = new Spacings();
    /// <summary>
    /// 
    /// </summary>
    private Spacings m_margins = new Spacings();
    private LayoutDirection m_layoutDir = LayoutDirection.Horizontal;
    private bool m_bSkip = false;
    private bool m_bLineBreak = false;
    private bool m_bSkipBottomAlign = false;
    private int m_levelNumber = -1;
    private int m_listItemIndex = 0;
    private bool m_bListRestart = false;
    private int m_iFieldType = -1;
    /// <summary>
    /// 
    /// </summary>
    private float m_fRowHeight;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bExactlyRowHeight;
    /// <summary>
    /// 
    /// </summary>
    private float m_fFirstLineIndent;
    /// <summary>
    /// 
    /// </summary>
    private LayoutTabs m_tabsInfo = new LayoutTabs();
    #endregion

    #region Class properties
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
        return m_bRowSplitted;
      }
      set
      {
        m_bRowSplitted = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [text wrap].
    /// </summary>
    /// <value><c>true</c> if [text wrap]; otherwise, <c>false</c>.</value>
    public bool TextWrap
    {
      get
      {
        return m_bTextWrap;
      }
      set
      {
        if( value != m_bTextWrap )
        {
          m_bTextWrap = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is vertical align center.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is vertical align center; otherwise, <c>false</c>.
    /// </value>
    public byte VerticalAlignment
    {
      get
      {
        return m_bVerticalAlignment; 
      }
      set
      {
        m_bVerticalAlignment = value;
      }
    }
    /// <summary>
    /// Gets the tabs info.
    /// </summary>
    /// <value>The tabs info.</value>
    public LayoutTabs LayoutTabs
    {
      get
      {
        return m_tabsInfo;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is fixed row height.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is fixed row height; otherwise, <c>false</c>.
    /// </value>
    public bool IsExactlyRowHeight
    {
      get
      {
        return m_bExactlyRowHeight;
      }
      set
      {
        if( value != m_bExactlyRowHeight )
        {
          m_bExactlyRowHeight = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets the height of the row.
    /// </summary>
    /// <value>The height of the row.</value>
    public float RowHeight
    {
      get
      {
        return m_fRowHeight;
      }
      set
      {
        if( value != m_fRowHeight )
        {
          m_fRowHeight = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is cell merge start.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is cell merge start; otherwise, <c>false</c>.
    /// </value>
    public bool IsColumnMergeStart
    {
      get
      {
        return m_bColumnMergeStart;
      }
      set
      {
        if( value != m_bColumnMergeStart )
        {
          m_bColumnMergeStart = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is cell continue.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is cell continue; otherwise, <c>false</c>.
    /// </value>
    public bool IsColumnMergeContinue
    {
      get
      {
        return m_bColumnMergeContinue;
      }
      set
      {
        if( value != m_bColumnMergeContinue )
        {
          m_bColumnMergeContinue = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is row merge start.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is row merge start; otherwise, <c>false</c>.
    /// </value>
    public bool IsRowMergeStart
    {
      get
      {
        return m_bRowMergeStart;
      }
      set
      {
        if( value != m_bRowMergeStart )
        {
          m_bRowMergeStart = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is row merge continue.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is row merge continue; otherwise, <c>false</c>.
    /// </value>
    public bool IsRowMergeContinue
    {
      get
      {
        return m_bRowMergeContinue;
      }
      set
      {
        if( value != m_bRowMergeContinue )
        {
          m_bRowMergeContinue = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is keep together.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is keep together; otherwise, <c>false</c>.
    /// </value>
    public bool IsKeepTogether
    {
      get
      {
        return m_bKeepTogether;
      }
      set
      {
        if( m_bKeepTogether != value )
        {
          m_bKeepTogether = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is keep with next.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is keep with next; otherwise, <c>false</c>.
    /// </value>
    public bool IsKeepWithNext
    {
      get
      {
        return m_bKeepWithNext;
      }
      set
      {
        if( value != m_bKeepWithNext )
        {
          m_bKeepWithNext = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is clipped.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is clipped; otherwise, <c>false</c>.
    /// </value>
    public bool IsClipped
    {
      get
      {
        return m_bClipped;
      }
      set
      {
        m_bClipped = value;
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
        return m_bLineBreak;
      }
      set
      {
        m_bLineBreak = value;
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
        return m_bSkip;
      }
    }
    /// <summary>
    /// Sets all paddings.
    /// </summary>
    /// <value>All paddings.</value>
    public double AllPaddings
    {
      set
      {
        Paddings.Left =
          Paddings.Right =
          Paddings.Top =
          Paddings.Bottom = value;
      }
    }
    /// <summary>
    /// Gets or sets the justification.
    /// </summary>
    /// <value>The justification.</value>
    public HorizontalAlignment Justification
    {
      get
      {
        return m_hrAlignment;
      }
      set
      {
        m_hrAlignment = value;
      }
    }
    /// <summary>
    /// Gets the childs layout direction.
    /// </summary>
    /// <value>The childs layout direction.</value>
    public LayoutDirection ChildsLayoutDirection
    {
      get
      {
        return m_layoutDir;
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
        return m_bLineContainer;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is page break.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is page break; otherwise, <c>false</c>.
    /// </value>
    public bool IsPageBreak
    {
      get
      {
        return m_bPageBreak;
      }
      set
      {
        m_bPageBreak = value;
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
        return m_bSkipBottomAlign;
      }
      set
      {
        m_bSkipBottomAlign = value;
      }
    }
    /// <summary>
    /// Gets or sets the level number.
    /// </summary>
    /// <value>The level number.</value>
    public int LevelNumber
    {
      get
      {
        return m_levelNumber;
      }
      set
      {
        m_levelNumber = value;
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
    /// Gets or sets a value indicating whether [list restart].
    /// </summary>
    /// <value><c>true</c> if [list restart]; otherwise, <c>false</c>.</value>
    public bool ListRestart
    {
      get
      {
        return m_bListRestart;
      }
      set
      {
        m_bListRestart = value;
      }
    }
    /// <summary>
    /// Gets or sets the type of the field.
    /// </summary>
    /// <value>The type of the field.</value>
    public int FieldType
    {
      get
      {
        return m_iFieldType;
      }
      set
      {
        m_iFieldType = value;
      }
    }
    /// <summary>
    /// Gets the name of the DBG.
    /// </summary>
    /// <value>The name of the DB g_.</value>
    public string DBG_Name
    {
      get
      {
        return m_strName;
      }
    }
    /// <summary>
    /// Gets or sets the first line indent.
    /// </summary>
    /// <value>The first line indent.</value>
    public float FirstLineIndent
    {
      get
      {
        return m_fFirstLineIndent;
      }
      set
      {
        if( value != m_fFirstLineIndent )
        {
          m_fFirstLineIndent = value;
        }
      }
    }
    #endregion

    #region Class properties / ILayoutSpacingsInfo
    /// <summary>
    /// 
    /// </summary>
    public Spacings Paddings
    {
      get
      {
        return m_paddings;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Spacings Margins
    {
      get
      {
        return m_margins;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public LayoutInfo()
    {
      m_bSkip = true;
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutInfo( bool bTopSubtractArea, bool bLineContainer,
                       string name )
    {
      m_bLineContainer = bLineContainer;
      m_layoutDir = bTopSubtractArea
                      ? LayoutDirection.Vertical
                      : LayoutDirection.Horizontal;
      m_strName = name;
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutInfo( LayoutDirection childLayoutDirection, bool bLineContainer,
                       string name )
    {
      m_bLineContainer = bLineContainer;
      m_layoutDir = childLayoutDirection;
      m_strName = name;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    internal RectangleF GetClientRectangle( RectangleF rect )
    {
      double leftPad = Margins.Left + Paddings.Left;
      double topPad = Margins.Top + Paddings.Top;
      double x = rect.X + leftPad;
      double y = rect.Y + topPad;
      double width = rect.Width - leftPad - Margins.Right - Paddings.Right;
      double height = rect.Height - topPad - Margins.Bottom - Paddings.Bottom;

      if( width < 0 )
      {
        width = 0;
      }
      if( height < 0 )
      {
        height = 0;
      }

      return new RectangleF( ( float )x, ( float )y, ( float )width, ( float )height );
    }
    #endregion
  }
}