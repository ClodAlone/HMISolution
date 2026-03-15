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
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;
#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Borders collection.
  /// </summary>
  public class BordersCollection
    : CollectionBaseEx<IBorder>
    , IBorders
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
    /// <summary>
    ///  Returns or sets the weight of the border. Read / write ExcelBorderWeight.
    /// </summary>
    public ExcelBorderWeight Weight
    {
      get
      {
        ExcelBorderWeight weight = (( IBorder )InnerList[ 0 ]).Weight;

        for( int i=1; i<Count; i++ )
        {
          if( weight != (( IBorder )InnerList[ i ]).Weight )
          {
            return ExcelBorderWeight.None;
          }
        }

        return weight;
      }
      set
      {
        for( int i=0; i<Count; i++ )
        {
          (( IBorder )InnerList[ i ]).Weight = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the color of all four borders. Returns NULL if all
    /// four borders aren't the same color. The color is specified as an
    /// index value into the current color palette or as one of the
    /// following ExcelColorIndex constants. Read / write ExcelColorIndex.
    /// </summary>
    public ExcelColorIndex ColorIndex
    {
      get
      {
        ExcelColorIndex index = (( IBorder )InnerList[ 0 ]).ColorIndex;

        for( int i=1; i<Count; i++ )
        {
          if( index != (( IBorder )InnerList[ i ]).ColorIndex )
          {
            return ExcelColorIndex.ColorIndexNone;
          }
        }

        return index;
      }
      set
      {
        for( int i=0; i<Count; i++ )
        {
          (( IBorder )InnerList[ i ]).ColorIndex = value;
        }
      }
    }
#endif
    #endregion

    #region Class members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Specifies a boolean value that indicate whether the border collection has a empty border
    /// </summary> 
    private bool m_bIsEmptyBorder = true;
    #endregion

    #region IBorders Members
    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read / write ExcelKnownColors.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        ExcelKnownColors color = ( ( IBorder )InnerList[ 0 ] ).Color;

        for( int i = 1; i < Count; i++ )
        {
          if( color != ( ( IBorder )InnerList[ i ] ).Color )
            return ExcelKnownColors.None;
        }

        return color;
      }
      set
      {
        for( int i = 0; i < Count; i++ )
        {
          ( ( IBorder )InnerList[ i ] ).Color = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object. Use the RGB function to create a color value.
    /// Read / write ExcelKnownColors.
    /// </summary>
    public Color ColorRGB
    {
      get
      {
        Color color = ( ( IBorder )InnerList[ 0 ] ).ColorRGB;
        int value = color.ToArgb();

        for( int i = 1; i < Count; i++ )
        {
          if( value != ( ( IBorder )InnerList[ i ] ).ColorRGB.ToArgb() )
          {
            color = ColorExtension.Empty;
            break;
          }
        }

        return color;
      }
      set
      {
        for( int i = 0; i < Count; i++ )
        {
          ( ( IBorder )InnerList[ i ] ).ColorRGB = value;
        }
      }
    }
    /// <summary>
    /// Returns a Border object that represents one of the borders of either a
    /// range of cells or a style. Read-only.
    /// </summary>
    public IBorder this[ ExcelBordersIndex index ]
    {
      get
      {
        switch( index )
        {
          case ExcelBordersIndex.DiagonalDown: return InnerList[ 0 ] as IBorder;
          case ExcelBordersIndex.DiagonalUp: return InnerList[ 1 ] as IBorder;
          case ExcelBordersIndex.EdgeBottom: return InnerList[ 2 ] as IBorder;
          case ExcelBordersIndex.EdgeLeft: return InnerList[ 3 ] as IBorder;
          case ExcelBordersIndex.EdgeRight: return InnerList[ 4 ] as IBorder;
          case ExcelBordersIndex.EdgeTop: return InnerList[ 5 ] as IBorder;
        }
        
        return null;
      }
    }
    /// <summary>
    /// Returns or sets the line style for the border.
    /// Read / write ExcelLineStyle.
    /// </summary>
    public ExcelLineStyle LineStyle
    {
      get
      {
        ExcelLineStyle style = (( IBorder )InnerList[ 0 ]).LineStyle;

        for( int i=1; i<Count; i++ )
        {
          if( style != (( IBorder )InnerList[ i ]).LineStyle ) return ExcelLineStyle.None;
        }

        return style;
      }
      set
      {
        for( int i=0; i<Count; i++ )
        {
          (( IBorder )InnerList[ i ]).LineStyle = value;
        }
      }
    }
    /// <summary>
    /// Synonym for Borders.LineStyle. Read / write ExcelLineStyle.
    /// </summary>
    public ExcelLineStyle Value
    {
      get
      {
        return LineStyle;
      }
      set
      {
        LineStyle = value;
      }
    }
    /// <summary>
    /// Specifies a boolean value that indicate whether the border collection has a empty border
    /// </summary>  
    internal bool IsEmptyBorder
    {
        get
        {
            return m_bIsEmptyBorder;
        }
        set
        {
            m_bIsEmptyBorder = value;
        }
    }
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Creates collection and sets its application and parent properties.
    /// </summary>
    /// <param name="application">Application object for this collection.</param>
    /// <param name="parent">The parent object for this collection.</param>
    /// <param name="bAddEmpty">Indicates whether null elements must be added to internal array.</param>
    internal BordersCollection( IApplication application, object parent, bool bAddEmpty )
      : base( application, parent )
    {
      SetParents();

      if( bAddEmpty )
      {
        InnerList.AddRange( new BorderImpl[ 6 ] );
      }
    }

    /// <summary>
    /// Creates collection taking borders from ExtendedFormatImplWrapper.
    /// </summary>
    /// <param name="application">The application object for the collection.</param>
    /// <param name="parent">The parent object for the collection.</param>
    /// <param name="wrap">
    /// ExtendedFormatImplWrapper from where all borders will be taken.
    /// </param>
    public BordersCollection( IApplication application, object parent, IInternalExtendedFormat wrap )
      : this( application, parent, false )
    {
      InnerList.Add( new BorderImpl( application, this, wrap, ExcelBordersIndex.DiagonalDown ) );
      InnerList.Add( new BorderImpl( application, this, wrap, ExcelBordersIndex.DiagonalUp ) );
      InnerList.Add( new BorderImpl( application, this, wrap, ExcelBordersIndex.EdgeBottom ) );
      InnerList.Add( new BorderImpl( application, this, wrap, ExcelBordersIndex.EdgeLeft ) );
      InnerList.Add( new BorderImpl( application, this, wrap, ExcelBordersIndex.EdgeRight ) );
      InnerList.Add( new BorderImpl( application, this, wrap, ExcelBordersIndex.EdgeTop ) );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals( object obj )
    {
      BordersCollection toCompare = obj as BordersCollection;

      if( toCompare == null ) return false;

      List<IBorder> list1 = InnerList;
      List<IBorder> list2 = toCompare.InnerList;
      bool bResult = true;

      for( int i = 0, len = Count; i < len && bResult; i++ )
      {
        if( !( list1[ i ].Equals( list2[ i ] ) ) )
        {
          bResult = false;
          break;
        }
      }

      return bResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      int iHash = 0;
      List<IBorder> list = InnerList;

      for( int i = 0, len = Count; i < len; i++ )
      {
        iHash ^= list[ i ].GetHashCode();
      }

      return iHash;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;
      
      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="border"></param>
    internal void SetBorder( ExcelBordersIndex index, IBorder border )
    {
      switch( index )
      {
        case ExcelBordersIndex.DiagonalDown:
          InnerList[ 0 ] = border;
          break;

        case ExcelBordersIndex.DiagonalUp:
          InnerList[ 1 ] = border;
          break;

        case ExcelBordersIndex.EdgeBottom:
          InnerList[ 2 ] = border;
          break;

        case ExcelBordersIndex.EdgeLeft:
          InnerList[ 3 ] = border;
          break;

        case ExcelBordersIndex.EdgeRight:
          InnerList[ 4 ] = border;
          break;

        case ExcelBordersIndex.EdgeTop:
          InnerList[ 5 ] = border;
          break;

        default:
          Add( border );
          break;
      }
    }
    #endregion

    internal void Dispose()
    {
        foreach (BorderImpl borderImpl in this.InnerList)
        {
            borderImpl.Clear();
        }
    }
  }

  /// <summary>
  /// Contains borders for range that contains more than one cell.
  /// </summary>
  public class BordersCollectionArrayWrapper
    : CollectionBaseEx<object>
    , IBorders
  {
    #region Class members
    /// <summary>
    /// All cells of the range.
    /// </summary>
    private List<IRange> m_arrCells = new List<IRange>();
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Application.
    /// </summary>
    private IApplication m_application;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates instances for specified range.
    /// </summary>
    /// <param name="range">Range for which instance must be created.</param>
    public BordersCollectionArrayWrapper( IRange range )
      : base( range.Application, range )
    {
      m_arrCells.AddRange( range.Cells );
    }
    /// <summary>
    /// Creates instances for specified range.
    /// </summary>
    /// <param name="range">Range for which instance must be created.</param>
    public BordersCollectionArrayWrapper(List<IRange> lstRange,IApplication application)
        : base(application, lstRange[0])
    {
        m_arrCells = lstRange;
        m_application = application;
    }
    #endregion

    #region IBorders Members
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read / write ExcelKnownColors.
    /// </summary>
    public Syncfusion.XlsIO.ExcelKnownColors Color
    {
      get
      {
        ExcelKnownColors color = m_arrCells[ 0 ].Borders.Color;

        for( int i = 1; i < m_arrCells.Count; i++ )
        {
          if( m_arrCells[ i ].Borders.Color != color )
            return ExcelKnownColors.None;
        }

        return color;
      }
      set
      {
        for( int i = 0, last = m_arrCells.Count; i < last; i++ )
        {
          m_arrCells[ i ].Borders.Color = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read / write color.
    /// </summary>
    public Color ColorRGB
    {
      get
      {
        Color color = m_arrCells[ 0 ].Borders.ColorRGB;
        int value = color.ToArgb();

        for( int i = 1; i < m_arrCells.Count; i++ )
        {
          if( m_arrCells[ i ].Borders.ColorRGB.ToArgb() != value )
          {
            color = ColorExtension.Empty;
            break;
          }
        }

        return color;
      }
      set
      {
        for( int i = 0, last = m_arrCells.Count; i < last; i++ )
        {
          m_arrCells[ i ].Borders.ColorRGB = value;
        }
      }
    }

    /// <summary>
    /// Returns a Border object that represents one of the borders of either a
    /// range of cells or a style.
    /// </summary>
    public IBorder this[ Syncfusion.XlsIO.ExcelBordersIndex Index ]
    {
      get
      {
          RangeImpl impl = (IRange)Parent as RangeImpl;
          if (impl.IsEntireRow || impl.IsEntireColumn)
              return new BorderImplArrayWrapper(m_arrCells, Index, m_application);
          else
              return new BorderImplArrayWrapper((IRange)Parent, Index);
      }
    }
    /// <summary>
    /// Returns or sets the line style for the border. Read / write ExcelLineStyle.
    /// </summary>
    public Syncfusion.XlsIO.ExcelLineStyle LineStyle
    {
      get
      {
        ExcelLineStyle line = m_arrCells[ 0 ].Borders.LineStyle;
        
        for( int i = 1; i < m_arrCells.Count; i++ )
        {
          if( line != m_arrCells[ i ].Borders.LineStyle )
            return ExcelLineStyle.None;
        }
        
        return line;
      }
      set
      {
        for( int i=0, last = m_arrCells.Count; i<last; i++ )
        {
          m_arrCells[ i ].Borders.LineStyle = value;
        }
      }
    }

    /// <summary>
    /// Synonym for Borders.LineStyle. Read / write Variant.
    /// </summary>
    public Syncfusion.XlsIO.ExcelLineStyle Value
    {
      get
      {
        return LineStyle;
      }
      set
      {
        LineStyle = value;
      }
    }

    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }
    #endregion
  }
}
