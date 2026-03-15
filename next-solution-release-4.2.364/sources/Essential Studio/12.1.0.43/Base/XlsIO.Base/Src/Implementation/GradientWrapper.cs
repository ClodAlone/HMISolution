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
  public class GradientWrapper
    : CommonWrapper
    , IGradient
    , IOptimizedUpdate
  {
    #region Class members
    /// <summary>
    /// Wrapped shape fill.
    /// </summary>
    private ShapeFillImpl m_gradient;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new gradient wrapper.
    /// </summary>
    public GradientWrapper()
    {
    }
    /// <summary>
    /// Creates new gradient wrapper.
    /// </summary>
    /// <param name="gradient">Gradient to wrap.</param>
    public GradientWrapper( ShapeFillImpl gradient )
    {
      if( gradient == null )
        throw new ArgumentNullException( "gradient" );

      m_gradient = gradient;
    }
    #endregion

    #region IGradient members
    /// <summary>
    /// Represents background color.
    /// </summary>
    public ColorObject BackColorObject
    {
      get
      {
        return m_gradient.BackColorObject;
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public Color BackColor
    {
      get
      {
        return m_gradient.BackColor;
      }
      set
      {
        if( value != BackColor )
        {
          BeginUpdate();
          m_gradient.BackColor = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Represents background color index.
    /// </summary>
    public ExcelKnownColors BackColorIndex
    {
      get
      {
        return m_gradient.BackColorIndex;
      }
      set
      {
        if( value != BackColorIndex )
        {
          BeginUpdate();
          m_gradient.BackColorIndex = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public ColorObject ForeColorObject
    {
      get
      {
        return m_gradient.ForeColorObject;
      }
    }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public Color ForeColor
    {
      get
      {
        return m_gradient.ForeColor;
      }
      set
      {
        if( value != ForeColor )
        {
          BeginUpdate();
          m_gradient.ForeColor = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Represents foreground color index.
    /// </summary>
    public ExcelKnownColors ForeColorIndex
    {
      get
      {
        return m_gradient.ForeColorIndex;
      }
      set
      {
        if( value != ForeColorIndex )
        {
          BeginUpdate();
          m_gradient.ForeColorIndex = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ExcelGradientStyle GradientStyle
    {
      get
      {
        return m_gradient.GradientStyle;
      }
      set
      {
        if( value != GradientStyle )
        {
          BeginUpdate();
          m_gradient.GradientStyle = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Represents gradient shading style.
    /// </summary>
    public ExcelGradientVariants GradientVariant
    {
      get
      {
        return m_gradient.GradientVariant;
      }
      set
      {
        ValidateGradientVariant( value );

        if( value != GradientVariant )
        {
          BeginUpdate();
          m_gradient.GradientVariant = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Compares with shape fill impl.
    /// </summary>
    /// <param name="gradient">Shape fill to compare with.</param>
    /// <returns>Zero if shape fills are equal.</returns>
    public int CompareTo( IGradient gradient )
    {
      return m_gradient.CompareTo( gradient );
    }
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    public void TwoColorGradient()
    {
      BeginUpdate();
      m_gradient.TwoColorGradient();
      EndUpdate();
    }
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    /// <param name="variant">Represents shading variant.</param>
    public void TwoColorGradient( ExcelGradientStyle style, ExcelGradientVariants variant )
    {
      BeginUpdate();
      m_gradient.TwoColorGradient( style, variant );
      EndUpdate();
    }
    #endregion

    #region Class public properties
    /// <summary>
    /// Returns wrapped gradient. Read-only.
    /// </summary>
    public ShapeFillImpl Wrapped
    {
      get
      {
        return m_gradient;
      }
    }
    #endregion

    #region Class events
    /// <summary>
    /// Event raised after wrapped font changed.
    /// </summary>
    public event EventHandler AfterChangeEvent;
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public override void BeginUpdate()
    {
      if( BeginCallsCount == 0 )
      {
        m_gradient = m_gradient.Clone( m_gradient.Parent );
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
        WorkbookImpl book = ( ( ExtendedFormatImpl )m_gradient.Parent ).Workbook;
        book.SetChanged();

        if( AfterChangeEvent != null )
        {
          AfterChangeEvent( this, EventArgs.Empty );
        }
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Validates whether gradient variant is correct for current gradient style.
    /// </summary>
    /// <param name="gradientVariant">Gradient variant to validate.</param>
    private void ValidateGradientVariant( ExcelGradientVariants gradientVariant )
    {
      switch( GradientStyle )
      {
        case ExcelGradientStyle.Horizontal:
        case ExcelGradientStyle.Vertical:
        case ExcelGradientStyle.Diagonl_Down:
        case ExcelGradientStyle.Diagonl_Up:

          if( gradientVariant == ExcelGradientVariants.ShadingVariants_4 )
            throw new ArgumentException( "Shading variant 4 is not valid for current gradient style." );

          break;

        case ExcelGradientStyle.From_Center:

          if( gradientVariant == ExcelGradientVariants.ShadingVariants_2 ||
            gradientVariant == ExcelGradientVariants.ShadingVariants_3 ||
            gradientVariant == ExcelGradientVariants.ShadingVariants_4 )
          {
            throw new ArgumentException( "Current shading variant is not valid for from center gradient style." );
          }

          break;
      }
    }
    #endregion

    internal void Dispose()
    {
        this.AfterChangeEvent = null;
        this.m_gradient.Clear();
    }
  }

  /// <summary>
  /// Summary description for GradientArrayWrapper.
  /// </summary>
  public class GradientArrayWrapper
    : CommonObject
    , IGradient
  {
    #region Class members
    /// <summary>
    /// Array that contains all cells of the range.
    /// </summary>
    private List<IRange> m_arrCells = new List<IRange>();
    #endregion

    #region Class constructors
    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="range">Base range.</param>
    public GradientArrayWrapper( IRange range )
      : base( range.Application, range )
    {
      m_arrCells.AddRange( range.Cells );
    }
    #endregion

    #region IGradient members
    /// <summary>
    /// Represents background color.
    /// </summary>
    public ColorObject BackColorObject
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public Color BackColor
    {
      get
      {
        int value = 0;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.Gradient.BackColor.ToArgb();
            first = false;
          }
          else if( range.CellStyle.Interior.Gradient.BackColor.ToArgb() != value )
          {
            return ColorExtension.Empty;
          }
        }

        return ColorExtension.FromArgb( value );
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.Gradient.BackColor = value;
        }
      }
    }
    /// <summary>
    /// Represents background color index.
    /// </summary>
    public ExcelKnownColors BackColorIndex
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
            value = range.CellStyle.Interior.Gradient.BackColorIndex;
            first = false;
          }
          else if( range.CellStyle.Interior.Gradient.BackColorIndex != value )
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

          range.CellStyle.Interior.Gradient.BackColorIndex = value;
        }
      }
    }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public ColorObject ForeColorObject
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public Color ForeColor
    {
      get
      {
        int value = 0;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.Gradient.ForeColor.ToArgb();
            first = false;
          }
          else if( range.CellStyle.Interior.Gradient.ForeColor.ToArgb() != value )
          {
            return ColorExtension.Empty;
          }
        }

        return ColorExtension.FromArgb( value );
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.Gradient.ForeColor = value;
        }
      }
    }
    /// <summary>
    /// Represents foreground color index.
    /// </summary>
    public ExcelKnownColors ForeColorIndex
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
            value = range.CellStyle.Interior.Gradient.ForeColorIndex;
            first = false;
          }
          else if( range.CellStyle.Interior.Gradient.ForeColorIndex != value )
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
          range.CellStyle.Interior.Gradient.ForeColorIndex = value;
        }
      }
    }
    /// <summary>
    /// Represents gradient shading style.
    /// </summary>
    public ExcelGradientStyle GradientStyle
    {
      get
      {
        ExcelGradientStyle value = ExcelGradientStyle.Horizontal;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.Gradient.GradientStyle;
            first = false;
          }
          else if( range.CellStyle.Interior.Gradient.GradientStyle != value )
          {
            return ExcelGradientStyle.Horizontal;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.Gradient.GradientStyle = value;
        }
      }
    }
    /// <summary>
    /// Represents gradient shading variant.
    /// </summary>
    public ExcelGradientVariants GradientVariant
    {
      get
      {
        ExcelGradientVariants value = ExcelGradientVariants.ShadingVariants_1;
        bool first = true;

        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Interior.Gradient.GradientVariant;
            first = false;
          }
          else if( range.CellStyle.Interior.Gradient.GradientVariant != value )
          {
            return ExcelGradientVariants.ShadingVariants_1;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Interior.Gradient.GradientVariant = value;
        }
      }
    }
    /// <summary>
    /// Compares with shape fill impl.
    /// </summary>
    /// <param name="gradient">Gradient to compare with.</param>
    /// <returns>Zero if shape fills are equal.</returns>
    public int CompareTo( IGradient gradient )
    {
      for( int i = 0, len = m_arrCells.Count; i < len; i++ )
      {
        IRange range = m_arrCells[ i ];

        if( range.CellStyle.Interior.Gradient.CompareTo( gradient ) != 0 )
          return 1;
      }

      return 0;
    }
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    public void TwoColorGradient()
    {
      for( int i = 0, len = m_arrCells.Count; i < len; i++ )
      {
        IRange range = m_arrCells[ i ];
        range.CellStyle.Interior.Gradient.TwoColorGradient();
      }
    }
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    /// <param name="variant">Represents shading variant.</param>
    public void TwoColorGradient( ExcelGradientStyle style, ExcelGradientVariants variant )
    {
      for( int i = 0, len = m_arrCells.Count; i < len; i++ )
      {
        IRange range = m_arrCells[ i ];
        range.CellStyle.Interior.Gradient.TwoColorGradient( style, variant );
      }
    }
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      for( int index = 0, last = m_arrCells.Count; index < last; index++ )
      {
        IRange range = m_arrCells[ index ] as IRange;
        ( ( GradientWrapper )range.CellStyle.Interior.Gradient ).BeginUpdate();
      }
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      for( int index = 0, last = m_arrCells.Count; index < last; index++ )
      {
        IRange range = m_arrCells[ index ] as IRange;
        ( ( GradientWrapper )range.CellStyle.Interior.Gradient ).EndUpdate();
      }
    }
    #endregion
  }
}
