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
using System.Drawing;
using System.Diagnostics;
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Summary description for LayoutArea.
  /// </summary>
  public class LayoutArea
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private RectangleF m_area;
    /// <summary>
    /// 
    /// </summary>
    private RectangleF m_clientArea;
    /// <summary>
    /// 
    /// </summary>
    private RectangleF m_clientActiveArea;
    /// <summary>
    /// 
    /// </summary>
    private ILayoutSpacingsInfo m_spacings = null;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bSkipSubtractWhenInvalidParameter = true;
    #endregion
    
    #region Properties
    /// <summary>
    /// Gets the width.
    /// </summary>
    /// <value>The width.</value>
    public double Width
    {
      get
      {
        return OuterArea.Width;
      }
    }
    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <value>The height.</value>
    public double Height
    {
      get
      {
        return OuterArea.Height;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [skips subtract when invalid parameter].
    /// </summary>
    /// <value>
    /// true if [skips subtract when invalid parameter]; otherwise, false.
    /// </value>
    public bool SkipSubtractWhenInvalidParameter
    {
      get
      {
        return m_bSkipSubtractWhenInvalidParameter;
      }
      set
      {
        m_bSkipSubtractWhenInvalidParameter = value;
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
        return m_spacings.Margins;
      }
    }
    /// <summary>
    /// Gets the paddings.
    /// </summary>
    /// <value>The paddings.</value>
    public Spacings Paddings
    {
      get
      {
        return m_spacings.Paddings;
      }
    }
    /// <summary>
    /// Gets the outer area.
    /// </summary>
    /// <value>The outer area.</value>
    public RectangleF OuterArea
    {
      get
      {
        return m_area;
      }
    }
    /// <summary>
    /// Gets the client area.
    /// </summary>
    /// <value>The client area.</value>
    public RectangleF ClientArea
    {
      get
      {
        return m_clientArea;
      }
    }
    /// <summary>
    /// Gets the client active area.
    /// </summary>
    /// <value>The client active area.</value>
    public RectangleF ClientActiveArea
    {
      get
      {
        return m_clientActiveArea;
      }
    }
    #endregion

    #region Cconstructors
    /// <summary>
    /// 
    /// </summary>
    public LayoutArea()
      : this( new RectangleF(), null )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutArea( RectangleF area )
      : this( area, null )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutArea( RectangleF area, ILayoutSpacingsInfo spacings )
    {
      m_area = area;
      m_spacings = spacings;
      UpdateClientArea();
    }
    /// <summary>
    /// Updates the client area.
    /// </summary>
    private void UpdateClientArea()
    {
      double leftPad = 0f;
      double topPad = 0f;
      double rightPad = 0f;
      double bottomPad = 0f;

      if( m_spacings != null )
      {
        leftPad = Margins.Left + Paddings.Left;
        topPad = Margins.Top + Paddings.Top;
        rightPad = Margins.Right + Paddings.Right;
        bottomPad = Margins.Bottom + Paddings.Bottom;
      }

      float listTab = 0f;

      if( m_spacings is LayoutParagraphInfo )
      {
        LayoutParagraphInfo parInfo = ( m_spacings as LayoutParagraphInfo );
        if( parInfo.LevelNumber != -1 )
        {
          listTab = parInfo.ListTab;
        }
      }

      double left = m_area.X + leftPad;
      
      if( left < 0 )
      {
        left = m_area.X;
      }
      double top = m_area.Y + topPad;
      double width = m_area.Width - leftPad - rightPad - listTab;
      double height = m_area.Height - topPad - bottomPad;

      if( width < 0 )
        width = 0;
      if( height < 0 )
        height = 0;

      m_clientArea = new RectangleF( ( float )left, ( float )top, ( float )width, ( float )height );
      m_clientActiveArea = m_clientArea;
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Cuts from left.
    /// </summary>
    /// <param name="x">The x.</param>
    public void CutFromLeft( double x )
    {
      if( x < m_clientActiveArea.Left || x > m_clientActiveArea.Right )
      {
        if( SkipSubtractWhenInvalidParameter )
        //return;
        {
          if( x < m_clientActiveArea.Left )
            x = m_clientActiveArea.Left;
          else if( x > m_clientActiveArea.Right )
            x = m_clientActiveArea.Right;
        }
        else
          throw new ArgumentException( "x" );
      }

      RectangleF rect = m_clientActiveArea;
      rect.Width = ( float )(rect.Right - x);
      rect.X = ( float )x;
      m_clientActiveArea = rect;
    }
    /// <summary>
    /// Cuts from top.
    /// </summary>
    /// <param name="y">The y.</param>
    public void CutFromTop( double y )
    {
      if( y < m_clientActiveArea.Top || y > m_clientActiveArea.Bottom )
      {
        if( SkipSubtractWhenInvalidParameter )
        {
          if( y < m_clientActiveArea.Top )
            y = m_clientActiveArea.Top;
          else if( y > m_clientActiveArea.Bottom )
            y = m_clientActiveArea.Bottom;
        }
        else
          throw new ArgumentException( "y" );
      }

      RectangleF rect = m_clientActiveArea;
      rect.Height = ( float )(rect.Bottom - y);
      rect.Y = ( float )y;
      m_clientActiveArea = rect;
    }
    /// <summary>
    /// Cuts from top.
    /// </summary>
    public void CutFromTop()
    {
      CutFromTop( ClientActiveArea.Bottom );
    }
    /// <summary>
    /// Tries the fit.
    /// </summary>
    /// <param name="s">The size.</param>
    /// <returns></returns>
    public bool TryFit( SizeF s )
    {
      return ( s.Width <= m_clientActiveArea.Width && s.Height <= m_clientActiveArea.Height );
    }
    /// <summary>
    /// Tries the fit.
    /// </summary>
    /// <param name="s">The size.</param>
    /// <param name="clippdeVert">if set to <c>true</c> [clippde vert].</param>
    /// <param name="clippedHoriz">if set to <c>true</c> [clipped horiz].</param>
    /// <returns></returns>
    public bool TryFit( SizeF s, bool clippdeVert, bool clippedHoriz )
    {
      return ( ( s.Width <= m_clientActiveArea.Width || clippedHoriz )
        && s.Height <= m_clientActiveArea.Height || clippdeVert );
    }
    #endregion
  }
}