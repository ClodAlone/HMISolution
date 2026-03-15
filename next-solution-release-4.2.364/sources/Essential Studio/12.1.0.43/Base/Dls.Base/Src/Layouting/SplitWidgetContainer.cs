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
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Summary description for SplitWidgetContainer.
  /// </summary>
  public class SplitWidgetContainer : IWidgetContainer
  {
    #region Class members
    private IWidgetContainer m_container;
    private IWidget m_currentChild;
    private int m_firstIndex;
    private int m_count = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets the real widget container.
    /// </summary>
    /// <value>The real widget container.</value>
    public IWidgetContainer RealWidgetContainer
    {
      get
      {
        SplitWidgetContainer splitContainer = m_container as SplitWidgetContainer;
        
        if( splitContainer == null )
        {
          return m_container;
        }
        
        return splitContainer.RealWidgetContainer;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitWidgetContainer"/> class.
    /// </summary>
    /// <param name="container">The container.</param>
    public SplitWidgetContainer( IWidgetContainer container )
    {
      m_container = container;
      m_currentChild = null;
      m_firstIndex = m_container.Count;
      m_count = 0;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitWidgetContainer"/> class.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="currentChild">The current child.</param>
    /// <param name="firstIndex">Index of the first.</param>
    public SplitWidgetContainer( IWidgetContainer container, IWidget currentChild, int firstIndex )
    {
      if( container == null )
        throw new ArgumentNullException( "container" );
      if( currentChild == null )
        throw new ArgumentNullException( "currentChild" );
      if( firstIndex < 0 )
        throw new ArgumentOutOfRangeException( "firstIndex", firstIndex, "Value can not be less 0" );

      m_container = container;
      m_currentChild = currentChild;
      m_firstIndex = firstIndex;
      m_count = m_container.Count - firstIndex; 
    }
    #endregion

    #region IWidget implement
    /// <summary>
    /// Gets layout info.
    /// </summary>
    /// <value></value>
    public ILayoutInfo LayoutInfo
    {
      get
      {
        return m_container.LayoutInfo;
      }
    }
    /// <summary>
    /// Draw range to graphics.
    /// </summary>
    /// <param name="g">The g.</param>
    /// <param name="layoutedWidget">The layouted widget.</param>
    public void Draw( CustomGraphics g, LayoutedWidget layoutedWidget )
    {
      m_container.Draw( g, layoutedWidget );
    }
    #endregion

    #region IWidgetContainer implement
    /// <summary>
    /// Gets count of child widgets.
    /// </summary>
    /// <value></value>
    public int Count
    {
      get
      {
        return m_count;
      }
    }
    /// <summary>
    /// Gets child widget by index.
    /// </summary>
    /// <value></value>
    public IWidget this[ int index ]
    {
      get
      {
        if( index == 0 )
        {
          return m_currentChild;
        }
        
        return m_container[ index + m_firstIndex ];
      }
    }
    #endregion
  }
}