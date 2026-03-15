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
using System.Collections;
using System.Diagnostics;
using System.Drawing;
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Summary description for ILayoutedRange.
  /// </summary>
  public class LayoutedWidget
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private RectangleF m_bounds = RectangleF.Empty;
    /// <summary>
    /// 
    /// </summary>
    private IWidget m_widget = null;
    /// <summary>
    /// 
    /// </summary>
    private LayoutedWidgetList m_ltWidgets = new LayoutedWidgetList();
    /// <summary>
    /// 
    /// </summary>
    private string m_textTag = null;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the text tag.
    /// </summary>
    /// <value>The text tag.</value>
    public string TextTag
    {
      get
      {
        return m_textTag;
      }
      set
      {
        m_textTag = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public RectangleF Bounds
    {
      get
      {
        return m_bounds;
      }
      set
      {
        m_bounds = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IWidget Widget
    {
      get
      {
        return m_widget;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutedWidgetList ChildWidgets
    {
      get
      {
        return m_ltWidgets;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// 
    /// </summary>
    public LayoutedWidget( IWidget widget )
    {
      m_widget = widget;
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutedWidget( IWidget widget, PointF location )
    {
      m_widget = widget;
      m_bounds = new RectangleF( location, new SizeF() );
    }
    #endregion

    #region Public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    public void Draw( CustomGraphics g )
    {
      m_widget.Draw( g, this );

      for( int i = 0, len = m_ltWidgets.Count; i < len; i++ )
      {
        LayoutedWidget ltWidget = m_ltWidgets[ i ];

        if( ltWidget != null )
        {
          ltWidget.Draw( g );
        }
        else
        {
          Trace.WriteLine( "object is null", "LayoutedWidget.Draw()" );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    public void ShiftLocation( double xOffset, double yOffset )
    {
      m_bounds = new RectangleF(
        new PointF( ( float )( m_bounds.X + xOffset ), ( float )(m_bounds.Y + yOffset) ),
        m_bounds.Size
        );

      for( int i = 0; i < ChildWidgets.Count; i++ )
      {
        LayoutedWidget ltWidget = ChildWidgets[ i ];
        if( ltWidget != null )
        {
          ltWidget.ShiftLocation( xOffset, yOffset );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public void AlignBottom( CustomGraphics g )
    {
      double maxHeight;
      double maxAscent;
      CalculateMaxChildWidget( g, out maxHeight, out maxAscent );

      if( maxHeight > m_bounds.Height )
      {
        maxHeight = m_bounds.Height;
      }
      
      if( maxAscent > maxHeight )
      {
        maxAscent = maxHeight;
      }

      for( int i = 0; i < ChildWidgets.Count; i++ )
      {
        LayoutedWidget ltWidget = ChildWidgets[ i ];

        if( ltWidget != null && !ltWidget.Widget.LayoutInfo.IsSkipBottomAlign )
        {
          double shiftY = 0;
          IStringWidget sWidget = ltWidget.Widget as IStringWidget;
          
          if( sWidget == null )
          {
            SplitStringWidget splitWidget = ltWidget.Widget as SplitStringWidget;
            
            if( splitWidget != null )
            {
              sWidget = splitWidget.RealStringWidget;
            }
          }

          shiftY = ( sWidget != null )
                     ? maxAscent - sWidget.GetTextAscent( g )
                     : maxAscent - ltWidget.Bounds.Height;
          ltWidget.ShiftLocation( 0, shiftY );
        }
      }
    }
    /// <summary>
    /// Aligns the justify.
    /// </summary>
    /// <param name="CustomGraphics">The custom graphics.</param>
    /// <param name="subWidth">Width of the sub.</param>
    public void AlignJustify( CustomGraphics CustomGraphics, double subWidth )
    {
      m_bounds.Width += ( float )subWidth;
      int[] widgetSpaces = new int[ ChildWidgets.Count ];
      int countAllSpaces = 0;

      // Calculate whitespaces for each child l-widget
      for (int i = 0; i < widgetSpaces.Length; i++)
      {
        LayoutedWidget ltW = ChildWidgets[ i ];
        IStringWidget sW = ltW.Widget as IStringWidget;
        string text = null;

        if( sW == null )
        {
          SplitStringWidget spW = ltW.Widget as SplitStringWidget;
          if( spW == null )
          {
            widgetSpaces[ i ] = 0;
          }
          else
          {
            text = spW.GetText();
          }
        }
        else
        {
          text = sW.Text;
        }

        if( text != null )
        {
          string[] parts = text.Split( new char[] { ' ' } );
          int cnt = parts.Length - 1;
          widgetSpaces[ i ] = cnt;
          countAllSpaces += cnt;
        }
      }
      
      // Changes width & position for child l-widgets
      double spaceDelta = subWidth / countAllSpaces;
      double shiftDelta = 0f;
      for( int i = 0; i < widgetSpaces.Length; i++ )
      {
        LayoutedWidget ltW = ChildWidgets[ i ];
        IStringWidget sW = ltW.Widget as IStringWidget;
        SplitStringWidget spW = null;
        ltW.ShiftLocation( shiftDelta, 0f );

        if( sW == null )
        {
          spW = ltW.Widget as SplitStringWidget;
        }

        if( sW != null || spW != null )
        {
          RectangleF bounds = ltW.Bounds;
          double currSpacesDelta = spaceDelta * widgetSpaces[ i ];
          bounds.Width += ( float )currSpacesDelta;
          ltW.Bounds = bounds;
          shiftDelta += currSpacesDelta;
        }
      }
    }
    #endregion
    
    #region Implementation
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="maxHeight"></param>
    /// <param name="maxAscent"></param>
    private void CalculateMaxChildWidget( CustomGraphics g, out double maxHeight, out double maxAscent )
    {
      maxHeight = 0;
      maxAscent = 0;
      
      for( int i = 0; i < ChildWidgets.Count; i++ )
      {
        LayoutedWidget ltWidget = ChildWidgets[ i ];

        if( ltWidget != null && !ltWidget.Widget.LayoutInfo.IsSkipBottomAlign )
        {
          if( m_ltWidgets.Count == 1 || maxHeight < ltWidget.Bounds.Height )
          {
            maxHeight = ltWidget.Bounds.Height;
          }

          IStringWidget sWidget = ltWidget.Widget as IStringWidget;
          
          if( sWidget == null )
          {
            SplitStringWidget splitWidget = ltWidget.Widget as SplitStringWidget;
            
            if( splitWidget != null )
            {
              sWidget = splitWidget.RealStringWidget;
            }
          }
          
          if( sWidget != null )
          {
            double textAscent = sWidget.GetTextAscent( g );
            if( m_ltWidgets.Count == 1 || maxAscent < textAscent )
            {
              maxAscent = textAscent;
            }
          }
          else
          {
            maxAscent = maxHeight;
          }
        }
      }
    }
    #endregion
  }
  /// <summary>
  /// 
  /// </summary>
  public class LayoutedWidgetList : ArrayList
  {
    #region Public methods
    /// <summary>
    /// 
    /// </summary>
    new public LayoutedWidget this[ int index ]
    {
      get
      {
        return base[ index ] as LayoutedWidget;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ltWidget"></param>
    /// <returns></returns>
    public int Add( LayoutedWidget ltWidget  )
    {
      return base.Add( ltWidget );
    }
    #endregion
  }
}