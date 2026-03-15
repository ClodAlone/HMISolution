#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Summary description for GradientWrapper.
  /// </summary>
  public class InteriorWrapper
    : CommonWrapper
    , IInterior
    , IOptimizedUpdate
  {
    #region Class members
    /// <summary>
    /// Extended format.
    /// </summary>
    private ExtendedFormatImpl m_xFormat;
    /// <summary>
    /// Gradient wrapper.
    /// </summary>
    private GradientWrapper m_gradient;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new interior wrapper.
    /// </summary>
    public InteriorWrapper()
    {
    }
    /// <summary>
    /// Creates new interior wrapper.
    /// </summary>
    /// <param name="format">Extended format.</param>
    public InteriorWrapper( ExtendedFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      m_xFormat = format;

      if( format.FillPattern == ExcelPattern.Gradient )
        CreateGradientWrapper();
    }
    #endregion

    #region IInterior members
    /// <summary>
    /// Returns or sets the color of the interior pattern as an index into the current color palette.
    /// </summary>
    public ExcelKnownColors PatternColorIndex
    {
      get
      {
        return m_xFormat.PatternColorIndex;
      }
      set
      {
        BeginUpdate();

        if( m_gradient != null )
          FillPattern = ExcelPattern.Solid;

        m_xFormat.PatternColorIndex = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an Color value.
    /// </summary>
    public Color PatternColor
    {
      get
      {
        return m_xFormat.PatternColor;
      }
      set
      {
        BeginUpdate();

        if( m_gradient != null )
          FillPattern = ExcelPattern.Solid;

        m_xFormat.PatternColor = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior. The color is specified as
    /// an index value into the current color palette.
    /// </summary>
    public ExcelKnownColors ColorIndex
    {
      get
      {
        return m_xFormat.ColorIndex;
      }
      set
      {
        BeginUpdate();

        if( m_gradient != null )
          FillPattern = ExcelPattern.Solid;

        m_xFormat.ColorIndex = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns or sets the cell shading color.
    /// </summary>
    public Color Color
    {
      get
      {
        return m_xFormat.Color;
      }
      set
      {
        BeginUpdate();

        if( m_gradient != null )
          FillPattern = ExcelPattern.Solid;

        m_xFormat.Color = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns gradient object for this extended format.
    /// </summary>
    public IGradient Gradient
    {
      get
      {
        return m_gradient;
      }
    }
    /// <summary>
    /// Gets / Sets fill pattern.
    /// </summary>
    public ExcelPattern FillPattern
    {
      get
      {
        return m_xFormat.FillPattern;
      }
      set
      {
        WorkbookImpl book = m_xFormat.Workbook;

        if( book.Version == ExcelVersion.Excel97to2003 && value == ExcelPattern.Gradient )
          throw new ArgumentException( "Excel97to2003 version does not support gradient fill type." );

        BeginUpdate();
        m_xFormat.FillPattern = value;

        if( value == ExcelPattern.Gradient )
        {
          CreateGradientWrapper();
        }
        else
        {
          m_gradient = null;
          m_xFormat.Gradient = null;
        }

        EndUpdate();
      }
    }
    #endregion

    #region Class events
    /// <summary>
    /// Event raised after wrapped font changed.
    /// </summary>
    public event EventHandler AfterChangeEvent;
    #endregion

    #region Class helper methods
    /// <summary>
    /// Event handler for gradient AfterChange event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void WrappedGradientAfterChangeEvent( object sender, EventArgs e )
    {
      BeginUpdate();
      m_xFormat.Gradient = m_gradient.Wrapped;
      EndUpdate();
    }
    /// <summary>
    /// Creates gradient wrapper.
    /// </summary>
    private void CreateGradientWrapper()
    {
      WorkbookImpl book = m_xFormat.Workbook;

      ShapeFillImpl gradient = ( ShapeFillImpl )m_xFormat.Gradient;

      if( gradient == null )
      {
        gradient = new ShapeFillImpl( book.Application, m_xFormat );
        gradient.FillType = ExcelFillType.Gradient;
      }

      m_gradient = new GradientWrapper( gradient );
      m_gradient.AfterChangeEvent += new EventHandler( WrappedGradientAfterChangeEvent );

      BeginUpdate();
      m_xFormat.Gradient = m_gradient.Wrapped;
      EndUpdate();
    }
    #endregion

    #region Class public properties
    /// <summary>
    /// Returns wrapped interior. Read-only.
    /// </summary>
    public ExtendedFormatImpl Wrapped
    {
      get
      {
        return m_xFormat;
      }
    }
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public override void BeginUpdate()
    {
      if( BeginCallsCount == 0 )
      {
        m_xFormat = ( ExtendedFormatImpl )Wrapped.Clone();
      }

      base.BeginUpdate();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
        WorkbookImpl book = m_xFormat.Workbook;
        book.SetChanged();

        if( AfterChangeEvent != null )
        {
          AfterChangeEvent( this, EventArgs.Empty );
        }
      }
    }
    #endregion

    internal void Dispose()
    {
        m_xFormat.clearAll();
        m_gradient.Dispose();
    }
  }

  /// <summary>
  /// Class that is created when user accesses the interior in a multicell range.
  /// Redirects all calls to the interiors of the individual cells.
  /// </summary>
  public class InteriorArrayWrapper
    : CommonObject
    , IInterior
  {
    #region Class members
    /// <summary>
    /// Array that contains all cells of the range.
    /// </summary>
    private List<IRange> m_arrCells = new List<IRange>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="range">Base range.</param>
    public InteriorArrayWrapper( IRange range )
      : base( range.Application, range )
    {
      m_arrCells.AddRange( range.Cells );
    }
    #endregion

    #region IInterior members
    /// <summary>
    /// Returns or sets the color of the interior pattern as an index into the current color palette.
    /// </summary>
    public ExcelKnownColors PatternColorIndex
    {
      get
      {
        ExcelKnownColors value = ExcelKnownColors.None;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.PatternColorIndex;
            first = false;
          }
          else if( range.CellStyle.Interior.PatternColorIndex != value )
          {
            return ExcelKnownColors.None;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.PatternColorIndex = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an Color value.
    /// </summary>
    public Color PatternColor
    {
      get
      {
        Color value = ColorExtension.Empty;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.PatternColor;
            first = false;
          }
          else if( range.CellStyle.Interior.PatternColor != value )
          {
            return ColorExtension.Empty;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.PatternColor = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior. The color is specified as
    /// an index value into the current color palette.
    /// </summary>
    public ExcelKnownColors ColorIndex
    {
      get
      {
        ExcelKnownColors value = ExcelKnownColors.None;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.ColorIndex;
            first = false;
          }
          else if( range.CellStyle.Interior.ColorIndex != value )
          {
            return ExcelKnownColors.None;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.ColorIndex = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the cell shading color.
    /// </summary>
    public Color Color
    {
      get
      {
        Color value = ColorExtension.Empty;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.Color;
            first = false;
          }
          else if( range.CellStyle.Interior.Color != value )
          {
            return ColorExtension.Empty;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.Color = value;
        }
      }
    }
    /// <summary>
    /// Returns gradient object.
    /// </summary>
    public IGradient Gradient
    {
      get
      {
        IGradient gradient = null;
        bool first = true;

        for( int index = 0, last = m_arrCells.Count; index < last; index++ )
        {
          IRange range = m_arrCells[ index ];

          if( first )
          {
            gradient = range.CellStyle.Interior.Gradient;
            first = false;
          }
          else if( range.CellStyle.Interior.Gradient != gradient )
          {
            return new GradientArrayWrapper( ( IRange )Parent );
          }
        }

        return gradient;
      }
    }
    /// <summary>
    /// Gets / Sets fill pattern.
    /// </summary>
    public ExcelPattern FillPattern
    {
      get
      {
        ExcelPattern value = ExcelPattern.None;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.FillPattern;
            first = false;
          }
          else if( range.CellStyle.Interior.FillPattern != value )
          {
            return ExcelPattern.None;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.FillPattern = value;
        }
      }
    }
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      // TODO: implement BeginUpdate if necessary
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      // TODO: implement EndUpdate if necessary
    }
    #endregion
  }
}